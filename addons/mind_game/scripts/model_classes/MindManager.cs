// This script is autoloaded and referenced by all MindAgent nodes
// Not seeing your .gguf files? Editor > Editor Settings > Docks > FilesSystem > add gguf to TextFile Extensions

using Godot;
using System;
using LLama;
using LLama.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class MindManager : Node, IDisposable
{

    // Chat model in a .gguf format
    public string ChatModelPath { get; private set; } = "addons/mind_game/assets/models/Phi-3-mini-4k-instruct-q4.gguf";

    // Clip model in a .gguf format
    public string ClipModelPath { get; private set; } = "addons/mind_game/assets/models/llava-phi-3-mini-mmproj-f16.gguf";

    // Embedder model in a .gguf format
    public string EmbedderModelPath { get; private set; } = "addons/mind_game/assets/models/all-MiniLM-L12-v2.Q4_K_M.gguf";

    // Gpu Layers set 0-33
    public int GpuLayerCount { get; private set; } = 33;
    public uint ContextSize { get; private set; } = 4096;
    public uint Seed { get; private set; } = 0;


    [Signal]
    public delegate void ChatModelStatusUpdateEventHandler(bool isLoaded);
    [Signal]
    public delegate void ClipModelStatusUpdateEventHandler(bool isLoaded);
    [Signal]
    public delegate void ContextStatusUpdateEventHandler(bool isLoaded);
    [Signal]
    public delegate void EmbedderModelStatusUpdateEventHandler(bool isLoaded);


    public LLamaWeights chatWeights { get; private set; } = null;
    public LLavaWeights clipWeights { get; private set; } = null;
    public LLamaEmbedder embedder { get; private set; } = null;
    public LLamaContext context { get; private set; } = null;
    

    public async override void _EnterTree()
    {
        await InitializeAsync();
    }

    public override void _Ready()
    {
        
    }

    public async Task InitializeAsync()
    {
        await LoadClipWeightsAsync();
        await LoadModelWeightsAsync();
        await CreateContextAsync();
    }

    // Involves loading a separate LLamaWeights
    private async Task LoadEmbedderAsync()
    {
        GD.Print("Embedder not yet coded");
    }

    public async Task LoadModelWeightsAsync()
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
            EmitSignal(SignalName.ContextStatusUpdate, true);
        }
        else
        {
            GD.PrintErr("Chat weights not set.");
        }

    }

    public async Task DisposeChatWeightsAsync()
    {
        await Task.Run(() =>
        {
            chatWeights?.Dispose();
            EmitSignal(SignalName.ChatModelStatusUpdate, false);
        });
            
    }

    public async Task DisposeClipWeightsAsync()
    {
        await Task.Run(() =>
        {
            clipWeights?.Dispose();
            EmitSignal(SignalName.ClipModelStatusUpdate, false);
        });
        
    }

    public async Task DisposeEmbedderAsync()
    {
        await Task.Run(() =>
        {
            embedder?.Dispose();
            EmitSignal(SignalName.EmbedderModelStatusUpdate, false);
        });
    }

    public async Task DisposeContextAsync()
    {
        await Task.Run(() =>
        {
            context?.Dispose();
            EmitSignal(SignalName.ContextStatusUpdate, false);
        });
    }

    

    public async Task DisposeAll()
    {
        await DisposeContextAsync();
        await DisposeEmbedderAsync();
        await DisposeClipWeightsAsync();
        await DisposeChatWeightsAsync();

    }


    public override void _ExitTree()
    {
        
    }

}