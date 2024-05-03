using Godot;
using System;
using LLama;
using LLama.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class MindManager : Node, IDisposable
{
    [Export]
    public string ChatModelPath { get; private set; }

    [Export]
    public string ClipModelPath { get; private set; }

    [Export]
    public string EmbedderModelPath { get; private set; }


    [Export]
    public int GpuLayerCount { get; private set; } = 16;

    [Export]
    public uint ContextSize { get; private set; } = 4096;


    [Signal]
    public delegate void ModelOutputReceivedEventHandler(string text);

  
    public LLamaWeights chatWeights { get; private set; }
    public LLavaWeights clipWeights { get; private set; }
    public LLamaEmbedder embedder { get; private set; }
    public LLamaContext context { get; private set; }
    public InteractiveExecutor executor { get; private set; }
    public ChatSession chatSession { get; private set; }



    public override void _EnterTree()
    {
        
    }

    public override void _Ready()
    {
        GD.Print("Mind Manager ready!");
    }

    public async Task LoadChatWeights()
    {
        if (!string.IsNullOrEmpty(ChatModelPath))
        {
            chatWeights = LLamaWeights.LoadFromFile(new ModelParams(ChatModelPath));
            
        }
    }

    public async Task CreateContext()
    {
        if (chatWeights != null)
        {
            context = chatWeights.CreateContext(new ModelParams(ChatModelPath));
        }
       
    }

    public async Task LoadClipWeights()
    {
        if (!string.IsNullOrEmpty(ClipModelPath))
        {
            clipWeights = LLavaWeights.LoadFromFile(ClipModelPath);
        }
    }

    public void CreateExecutor()
    {
        executor = new InteractiveExecutor(context);
    }

    public void CreateChatSession()
    { 
        chatSession = new ChatSession(executor);
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

        // Execute the chat session with the current prompt and possibly the images set earlier
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
        EmitSignal(nameof(ModelOutputReceivedEventHandler), output);
    }

    public void DisposeChatModel()
    {
        chatWeights?.Dispose();
    }

    public void DisposeClipModel()
    {
        clipWeights?.Dispose();
    }

    public void DisposeEmbedder()
    {
        embedder?.Dispose();
    }

    public void DisposeAll()
    {
        DisposeChatModel();
        DisposeClipModel();
        DisposeEmbedder();
    }


    public override void _ExitTree()
    {
        
    }

}