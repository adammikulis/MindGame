using Godot;
using LLama;
using LLama.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class MindAgent : Node
{

    private MindManager mm;

    public InteractiveExecutor executor { get; private set; } = null;
    public ChatSession chatSession { get; private set; } = null;
    [Signal]
    public delegate void ExecutorStatusEventHandler(bool isLoaded);
    [Signal]
    public delegate void ChatSessionStatusEventHandler(bool isLoaded);
    [Signal]
    public delegate void ChatOutputReceivedEventHandler(string text);
    public override void _EnterTree()
    {
        
    }

    public override void _Ready()
    {
        try
        {
            mm = GetNode<MindManager>("/root/MindManager");
        }
        catch (Exception e)
        {
            GD.PrintErr("Please ensure MindManager is enabled in Autoloads!");
        }
        
    }

    public async Task InitializeAsync()
    {
        await CreateExecutorAsync();
        await CreateChatSessionAsync();
    }

    public async Task CreateExecutorAsync()
    {
        if (mm.context != null)
        {
            if (mm.clipWeights != null)
            {
                await Task.Run(() =>
                {
                    executor = new InteractiveExecutor(mm.context, mm.clipWeights);
                });
            }
            else
            {
                await Task.Run(() =>
                {
                    executor = new InteractiveExecutor(mm.context);
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
        EmitSignal(SignalName.ChatOutputReceived, output);
    }

    public async Task DisposeExecutorAsync()
    {
        await Task.Run(() =>
        {
            executor = null;
            EmitSignal(SignalName.ExecutorStatus, false);
        });
    }

    public async Task DisposeChatSessionAsync()
    {
        await Task.Run(() =>
        {
            chatSession = null;
            EmitSignal(SignalName.ChatSessionStatus, false);
        });
    }

    public async Task DisposeAllAsync()
    {
        await DisposeChatSessionAsync();
        await DisposeExecutorAsync();

    }

    public override void _ExitTree() 
    { 
        
    }
}
