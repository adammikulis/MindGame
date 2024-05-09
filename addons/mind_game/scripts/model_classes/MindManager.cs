// This script is autoloaded and referenced by all MindAgent nodes
// Not seeing your .gguf files? Editor > Editor Settings > Docks > FilesSystem > add gguf to TextFile Extensions

using Godot;
using System;
using LLama;
using LLama.Common;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace MindGame
{
    [Tool]
    public partial class MindManager : Node, IDisposable
    {

        public ModelConfigsParams CurrentModelConfigs { get; set; }


        [Signal]
        public delegate void ChatModelStatusUpdateEventHandler(bool isLoaded);
        [Signal]
        public delegate void ClipModelStatusUpdateEventHandler(bool isLoaded);
        [Signal]
        public delegate void EmbedderModelStatusUpdateEventHandler(bool isLoaded);
        [Signal]
        public delegate void ContextStatusUpdateEventHandler(bool isLoaded);

        public LLamaWeights chatWeights { get; private set; } = null;
        public LLamaContext context { get; private set; } = null;

        // Clip model vars
        public LLavaWeights clipWeights { get; private set; } = null;


        // Embedder model vars
        public LLamaWeights embedderWeights { get; private set; } = null;
        public LLamaEmbedder embedder { get; private set; } = null;

        
        public bool isReady { get; private set; } = false;



        public override void _EnterTree()
        {

        }

        public override void _Ready()
        {

        }

        public async Task InitializeAsync(ModelConfigsParams config)
        {
            CurrentModelConfigs = config;
            await LoadModelsAsync();
        }

        private async Task LoadModelsAsync()
        {
            await LoadEmbedderAsync(CurrentModelConfigs.EmbedderModelPath, CurrentModelConfigs.EmbedderContextSize, CurrentModelConfigs.EmbedderGpuLayerCount, CurrentModelConfigs.EmbedderRandomSeed);
            await LoadClipModelAsync(CurrentModelConfigs.ClipModelPath);
            await LoadChatModelAsync(CurrentModelConfigs.ChatModelPath, CurrentModelConfigs.ChatContextSize, CurrentModelConfigs.ChatGpuLayerCount, CurrentModelConfigs.ChatRandomSeed);
        }


        private async Task LoadEmbedderAsync(string modelPath, uint contextSize, int gpuLayerCount, uint randomSeed)
        {
            await Task.Run(() =>
            {
                if (embedderWeights != null)
                {
                    UnloadEmbedderModelAsync();
                }

                var parameters = new ModelParams(modelPath)
                {
                    ContextSize = contextSize,
                    Seed = randomSeed,
                    GpuLayerCount = gpuLayerCount,
                    EmbeddingMode = true
                };

                embedderWeights = LLamaWeights.LoadFromFile(parameters);
                embedder = new LLamaEmbedder(embedderWeights, parameters);
            });

        }

        public async void UnloadEmbedderModelAsync()
        {

            if (embedderWeights != null) { embedderWeights.Dispose(); }
            if (embedder != null) { embedder.Dispose(); }

        }


        public async Task LoadChatModelAsync(string modelPath, uint contextSize, int gpuLayerCount, uint randomSeed)
        {

            if (!string.IsNullOrEmpty(modelPath))
            {
                var parameters = new ModelParams(modelPath)
                {
                    ContextSize = contextSize,
                    Seed = randomSeed,
                    GpuLayerCount = gpuLayerCount,
                    EmbeddingMode = false
                };

                await Task.Run(() =>
                {
                    if (chatWeights != null)
                    {
                        UnloadChatWeightsAsync();
                    }

                    chatWeights = LLamaWeights.LoadFromFile(parameters);
                    CallDeferred("emit_signal", SignalName.ChatModelStatusUpdate, true);
                    
                });
                if (chatWeights != null)
                {
                    await Task.Run(() =>
                    {
                        context = chatWeights.CreateContext(parameters);
                        CallDeferred("emit_signal", SignalName.ContextStatusUpdate, true);
                        isReady = true;
                    });
                    
                }
                else
                {
                    GD.PrintErr("Chat weights not set.");
                }
            }
            else
            {
                GD.PrintErr("Chat model path not set.");
            }
        }

        public async void UnloadChatWeightsAsync()
        {

            if (chatWeights != null) { chatWeights.Dispose(); }

        }



        public async Task LoadClipModelAsync(string modelPath)
        {
            if (!string.IsNullOrEmpty(modelPath))
            {
                await Task.Run(() => 
                { 
                    clipWeights = LLavaWeights.LoadFromFile(modelPath);
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
}