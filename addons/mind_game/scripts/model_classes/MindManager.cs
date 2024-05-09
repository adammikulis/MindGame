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
    public partial class MindManager : Node
    {

        public ModelConfigsParams CurrentModelConfigs { get; set; }


        [Signal]
        public delegate void ClipModelStatusUpdateEventHandler(bool isLoaded);
        [Signal]
        public delegate void EmbedderModelStatusUpdateEventHandler(bool isLoaded);
        [Signal]
        public delegate void ExecutorStatusUpdateEventHandler(bool isLoaded);

        // Chat Executor
        public LLamaContext context { get; set; }
        public InteractiveExecutor executor { get; private set; } = null;

        // Clip model vars
        public LLavaWeights clipWeights { get; private set; } = null;


        // Embedder model vars
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
            // await LoadEmbedderAsync(CurrentModelConfigs.EmbedderModelPath, CurrentModelConfigs.EmbedderContextSize, CurrentModelConfigs.EmbedderGpuLayerCount, CurrentModelConfigs.EmbedderRandomSeed);
            // await LoadClipModelAsync(CurrentModelConfigs.ClipModelPath);
            await InitializeChatExecutorAsync();
        }


        private async Task LoadEmbedderAsync()
        {
            await Task.Run(() =>
            {

                var parameters = new ModelParams(CurrentModelConfigs.EmbedderModelPath)
                {
                    ContextSize = CurrentModelConfigs.EmbedderContextSize,
                    Seed = CurrentModelConfigs.EmbedderRandomSeed,
                    GpuLayerCount = CurrentModelConfigs.EmbedderGpuLayerCount,
                    EmbeddingMode = true
                };

                using var embedderWeights = LLamaWeights.LoadFromFile(parameters);
                embedder = new LLamaEmbedder(embedderWeights, parameters);
            });
        }

        public async void UnloadEmbedderModelAsync()
        {

            if (embedder != null) { embedder.Dispose(); }

        }


        public async Task InitializeChatExecutorAsync()
        {
            if (!string.IsNullOrEmpty(CurrentModelConfigs.ChatModelPath))
            {
                var parameters = new ModelParams(CurrentModelConfigs.ChatModelPath)
                {
                    ContextSize = CurrentModelConfigs.ChatContextSize,
                    Seed = CurrentModelConfigs.ChatRandomSeed,
                    GpuLayerCount = CurrentModelConfigs.ChatGpuLayerCount,
                    EmbeddingMode = false
                };

                await Task.Run(() =>
                {
                    using var chatWeights = LLamaWeights.LoadFromFile(parameters);
                    context = chatWeights.CreateContext(parameters);
                    
                    executor = new InteractiveExecutor(context); // Make another loader with context and clip weights for llava
                    CallDeferred("emit_signal", SignalName.ExecutorStatusUpdate, true);
                    isReady = true;
                });
            }
    
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

        public async Task DisposeClipWeightsAsync()
        {
            await Task.Run(() =>
            {
                // clipWeights?.Dispose();
                CallDeferred("emit_signal", SignalName.ClipModelStatusUpdate, false);
            });
        
        }

        public async Task DisposeExecutorAsync()
        {
            await Task.Run(() =>
            {
                executor = null;
                CallDeferred("emit_signal", SignalName.ExecutorStatusUpdate, false);
                isReady = false;
            });
        }


        public async Task DisposeAll()
        {
            // await DisposeClipWeightsAsync();
            await DisposeExecutorAsync();
        }


        public override void _ExitTree()
        {
        
        }
    }
}