// This node must exist somewhere in your project and initialized prior to creating MindAgent nodes

using Godot;
using System;
using LLama;
using LLama.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

[Tool]
public partial class MindManager : Node, IDisposable
{

    // Chat model in a .gguf format
    [Export]
    public string ChatModelPath { get; private set; } = "res://addons/mind_game/assets/models/Phi-3-mini-4k-instruct-q4.gguf";

    // Clip model in a .gguf format
    [Export]
    public string ClipModelPath { get; private set; } = "res://addons/mind_game/assets/models/llava-phi-3-mini-mmproj-f16.gguf";

    // Embedder model in a .gguf format
    [Export]
    public string EmbedderModelPath { get; private set; } = "res://addons/mind_game/assets/models/all-MiniLM-L12-v2.Q4_K_M.gguf";

    // Gpu Layers set 0-33
    [Export]
    public int GpuLayerCount { get; private set; } = 33;

    [Export]
    public uint ContextSize { get; private set; } = 4096;
    [Export]
    public uint Seed { get; private set; } = 0;


    [Signal]
    public delegate void ModelOutputReceivedEventHandler(string text);
    [Signal]
    public delegate void ChatModelStatusEventHandler(bool isLoaded);
    [Signal]
    public delegate void ClipModelStatusEventHandler(bool isLoaded);
    [Signal]
    public delegate void ContextStatusEventHandler(bool isLoaded);
    [Signal]
    public delegate void EmbedderModelStatusEventHandler(bool isLoaded);
    [Signal]
    public delegate void ExecutorStatusEventHandler(bool isLoaded);
    [Signal]
    public delegate void ChatSessionStatusEventHandler(bool isLoaded);


    public LLamaWeights chatWeights { get; private set; } = null;
    public LLavaWeights clipWeights { get; private set; } = null;
    public LLamaEmbedder embedder { get; private set; } = null;
    public LLamaContext context { get; private set; } = null;
    public InteractiveExecutor executor { get; private set; } = null;
    public ChatSession chatSession { get; private set; } = null;



    public override void _EnterTree()
    {
        
    }

    public override void _Ready()
    {
        GD.Print("Mind Manager ready!");
    }

    public async Task InitializeAsync()
    {
        await LoadChatWeightsAsync();
        await LoadClipWeightsAsync();
        await CreateContextAsync();
        await CreateExecutorAsync();
        await CreateChatSessionAsync();
    }

    public async Task LoadChatWeightsAsync()
    {
        if (!string.IsNullOrEmpty(ChatModelPath))
        {
            await Task.Run(() =>
            {
                chatWeights = LLamaWeights.LoadFromFile(new ModelParams(ChatModelPath));
            });
        }
        else
        {
            GD.PrintErr("Chat model path not set.");
        }
    }

    public async Task LoadClipWeightsAsync()
    {
        if (!string.IsNullOrEmpty(ClipModelPath))
        {
            await Task.Run(() => 
            { 
                clipWeights = LLavaWeights.LoadFromFile(ClipModelPath); 
            });
        }
        else
        {
            GD.PrintErr("Clip model path not set.");
        }
    }

    public async Task CreateContextAsync()
    {
        if (chatWeights != null)
        {
            await Task.Run(() =>
            {
                context = chatWeights.CreateContext(new ModelParams(ChatModelPath));
            });
            EmitSignal(SignalName.ContextStatus, true);
        }
        else
        {
            GD.PrintErr("Chat weights not set.");
        }

    }


    public async Task CreateExecutorAsync()
    {
        if (context != null)
        {
            if (clipWeights != null)
            {
                await Task.Run(() =>
                {
                    executor = new InteractiveExecutor(context, clipWeights);
                });
            }
            else
            {
                await Task.Run(() =>
                {
                    executor = new InteractiveExecutor(context);
                });
            }
        }
        else
        {
            GD.PrintErr("Context not initialized.");
        }
    }

    public async Task CreateChatSessionAsync()
    {
        await Task.Run(() =>
        {
            chatSession = new ChatSession(executor);
        });
    }



    public async Task InferAsync(string prompt, List<string> imagePaths = null)
    {
        if (chatSession == null)
        {
            GD.PrintErr("Chat session not initialized. Please check the model configuration.");
            return;
        }

        // Handle image paths by setting them in the executor
        if (imagePaths != null && imagePaths.Count > 0)
        {
            executor.ImagePaths.Clear();
            executor.ImagePaths.AddRange(imagePaths);
        }

        // Execute the chat session with the current prompt and any images
        await Task.Run(async () =>
        {
            await foreach (var output in chatSession.ChatAsync(new ChatHistory.Message(AuthorRole.User, prompt), new InferenceParams { Temperature = 0.5f }))
            {
                CallDeferred(nameof(DeferredEmitModelOutput), output);
            }
        });
    }

    private void DeferredEmitModelOutput(string output)
    {
        EmitSignal(SignalName.ModelOutputReceived, output);
    }

    public void DisposeChatModel()
    {
        chatWeights?.Dispose();
        EmitSignal(SignalName.ChatModelStatus, false);
    }

    public void DisposeClipModel()
    {
        clipWeights?.Dispose();
        EmitSignal(SignalName.ClipModelStatus, false);
    }

    public void DisposeEmbedder()
    {
        embedder?.Dispose();
        EmitSignal(SignalName.EmbedderModelStatus, false);
    }

    public void DisposeContext()
    {
        context?.Dispose();
        EmitSignal(SignalName.ContextStatus, false);
    }

    public void DisposeExecutor()
    {
        executor = null;
        EmitSignal(SignalName.ExecutorStatus, false);
    }

    public void DisposeChatSession()
    {
        chatSession = null;
        EmitSignal(SignalName.ChatSessionStatus, false);
    }

    public void DisposeAll()
    {
        DisposeChatModel();
        DisposeClipModel();
        DisposeEmbedder();
        DisposeContext();
        DisposeExecutor();
        DisposeChatSession();
    }


    public override void _ExitTree()
    {
        
    }

}