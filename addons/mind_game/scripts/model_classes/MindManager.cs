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
    public string ChatModelPath { get; private set; }

    // Clip model in a .gguf format
    public string ClipModelPath { get; private set; }

    // Embedder model in a .gguf format
    public string EmbedderModelPath { get; private set; }

    // Gpu Layers set 0-33
    public int GpuLayerCount { get; private set; }
    public uint ContextSize { get; private set; }
    public uint Seed { get; private set; } = 0;


    [Signal]
    public delegate void ChatModelStatusUpdateEventHandler(bool isLoaded);
    [Signal]
    public delegate void ClipModelStatusUpdateEventHandler(bool isLoaded);
    [Signal]
    public delegate void EmbedderModelStatusUpdateEventHandler(bool isLoaded);
    [Signal]
    public delegate void ContextStatusUpdateEventHandler(bool isLoaded);


    // Chat model vars
    public LLamaWeights chatWeights { get; private set; } = null;
    public LLamaContext context { get; private set; } = null;

    // Clip model vars
    public LLavaWeights clipWeights { get; private set; } = null;

   
    // Embedder model vars
    public LLamaEmbedder embedder { get; private set; } = null;

    public LLamaWeights embedderWeights { get; private set; } = null;

    public bool isReady { get; private set; } = false;
 
    

    public override void _EnterTree()
    {

    }

    public override void _Ready()
    {
        
    }

    public async Task InitializeAsync()
    {
        await CreateContextAsync();
        await LoadClipModelWeightsAsync();
        await LoadChatModelWeightsAsync();
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

    //// Involves loading a separate LLamaWeights
    //private async Task LoadEmbedderAsync()
    //{
    //    if (embedderWeights != null)
    //    {
    //        UnloadEmbedderModelAsync();
    //    }

    //    var parameters = new ModelParams(EmbedderModelPath)
    //    {
    //        ContextSize = embedderContextSize,
    //        Seed = 0,
    //        GpuLayerCount = embedderGpuLayerCount,
    //        EmbeddingMode = true
    //    };

    //    embedderWeights = LLamaWeights.LoadFromFile(parameters);
    //    embedder = new LLamaEmbedder(embedderWeights, parameters);

    //}

    //public async void UnloadEmbedderModelAsync()
    //{

    //    if (embedderWeights != null) { embedderWeights.Dispose(); }
    //    if (embedder != null) { embedder.Dispose(); }

    //}


    public async Task LoadChatModelWeightsAsync()
    {
        if (!string.IsNullOrEmpty(ChatModelPath))
        {
            await Task.Run(() =>
            {
                chatWeights = LLamaWeights.LoadFromFile(new ModelParams(ChatModelPath));
                CallDeferred("emit_signal", SignalName.ChatModelStatusUpdate, true);
                isReady = true;
            });
        }
        else
        {
            GD.PrintErr("Chat model path not set.");
        }
    }



    public async Task LoadClipModelWeightsAsync()
    {
        if (!string.IsNullOrEmpty(ClipModelPath))
        {
            await Task.Run(() => 
            { 
                clipWeights = LLavaWeights.LoadFromFile(ClipModelPath);
                CallDeferred("emit_signal", SignalName.ClipModelStatusUpdate, true);
            });
        }
        else
        {
            GD.Print("Clip model path not set, no model loaded.");
        }
    }



    public async Task DisposeChatWeightsAsync()
    {
        await Task.Run(() =>
        {
            chatWeights?.Dispose();
            CallDeferred("emit_signal", SignalName.ChatModelStatusUpdate, false);
        });
            
    }

    public async Task DisposeClipWeightsAsync()
    {
        await Task.Run(() =>
        {
            clipWeights?.Dispose();
            CallDeferred("emit_signal", SignalName.ClipModelStatusUpdate, false);
        });
        
    }

    public async Task DisposeEmbedderAsync()
    {
        await Task.Run(() =>
        {
            embedderWeights?.Dispose();
            CallDeferred("emit_signal", SignalName.EmbedderModelStatusUpdate, false);
        });
    }

    public async Task DisposeContextAsync()
    {
        await Task.Run(() =>
        {
            context?.Dispose();
            CallDeferred("emit_signal", SignalName.ContextStatusUpdate, false);
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