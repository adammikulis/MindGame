// This script is autoloaded and referenced by all MindAgent nodes
// Not seeing your .gguf files? Editor > Editor Settings > Docks > FilesSystem > add gguf to TextFile Extensions

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
    public delegate void ChatModelStatusEventHandler(bool isLoaded);
    [Signal]
    public delegate void ClipModelStatusEventHandler(bool isLoaded);
    [Signal]
    public delegate void ContextStatusEventHandler(bool isLoaded);
    [Signal]
    public delegate void EmbedderModelStatusEventHandler(bool isLoaded);


    public LLamaWeights chatWeights { get; private set; } = null;
    public LLavaWeights clipWeights { get; private set; } = null;
    public LLamaEmbedder embedder { get; private set; } = null;
    public LLamaContext context { get; private set; } = null;
    

    public override void _EnterTree()
    {
        
    }

    public override void _Ready()
    {
        GD.Print("Mind Manager ready!");
    }

    public async Task InitializeAsync()
    {
        await LoadModelWeightsAsync();
        await LoadClipWeightsAsync();
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
            EmitSignal(SignalName.ContextStatus, true);
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
            EmitSignal(SignalName.ChatModelStatus, false);
        });
            
    }

    public async Task DisposeClipWeightsAsync()
    {
        await Task.Run(() =>
        {
            clipWeights?.Dispose();
            EmitSignal(SignalName.ClipModelStatus, false);
        });
        
    }

    public async Task DisposeEmbedderAsync()
    {
        await Task.Run(() =>
        {
            embedder?.Dispose();
            EmitSignal(SignalName.EmbedderModelStatus, false);
        });
    }

    public async Task DisposeContextAsync()
    {
        await Task.Run(() =>
        {
            context?.Dispose();
            EmitSignal(SignalName.ContextStatus, false);
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