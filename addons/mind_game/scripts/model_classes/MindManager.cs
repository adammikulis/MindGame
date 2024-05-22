using Godot;
using LLama;
using LLama.Common;
using System.Threading.Tasks;
using LLama.Native;

namespace MindGame
{
    [Tool]
    public partial class MindManager : Node
    {
        public ModelParamsConfig CurrentModelConfigs { get; set; }

        [Signal]
        public delegate void ClipModelStatusUpdateEventHandler(bool isLoaded);
        [Signal]
        public delegate void EmbedderModelStatusUpdateEventHandler(bool isLoaded);
        [Signal]
        public delegate void ExecutorStatusUpdateEventHandler(bool isLoaded);

        // Chat Executor
        public LLamaContext context { get; set; }
        public InteractiveExecutor executor { get; private set; }

        // Clip model vars
        public LLavaWeights clipWeights { get; private set; }

        // Embedder model vars
        public LLamaEmbedder embedder { get; private set; }

        public ConfigListResource ConfigListResource;
        private readonly string configListResourcePath = "res://addons/mind_game/assets/resources/custom_resources/ConfigListResource.tres";

        public override void _EnterTree()
        {
            NativeLibraryConfig.Instance.WithCuda(true);
        }

        public override void _Ready()
        {
            EnsureConfigListResourceExists();
        }

        private void EnsureConfigListResourceExists()
        {
            ConfigListResource = GD.Load<ConfigListResource>(configListResourcePath);
            if (ConfigListResource == null)
            {
                ConfigListResource = new ConfigListResource();
                SaveConfigList();
            }
        }

        private void SaveConfigList()
        {
            Error saveError = ResourceSaver.Save(ConfigListResource, configListResourcePath);
            if (saveError != Error.Ok)
            {
                GD.PrintErr("Failed to save configuration list: ", saveError);
            }
        }

        public async Task InitializeAsync(ModelParamsConfig config)
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
                    Embeddings = true
                };

                using var embedderWeights = LLamaWeights.LoadFromFile(parameters);
                embedder = new LLamaEmbedder(embedderWeights, parameters);
            });
        }

        public void UnloadEmbedderModel()
        {
            embedder?.Dispose();
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
                    Embeddings = false
                };

                await Task.Run(() =>
                {
                    using var chatWeights = LLamaWeights.LoadFromFile(parameters);
                    context = chatWeights.CreateContext(parameters);

                    executor = new InteractiveExecutor(context);
                    CallDeferred("emit_signal", SignalName.ExecutorStatusUpdate, true);
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
                clipWeights = null;
                CallDeferred("emit_signal", SignalName.ClipModelStatusUpdate, false);
            });
        }

        public async Task DisposeExecutorAsync()
        {
            await Task.Run(() =>
            {
                executor = null;
                CallDeferred("emit_signal", SignalName.ExecutorStatusUpdate, false);
            });
        }

        public async Task DisposeAll()
        {
            await DisposeClipWeightsAsync();
            await DisposeExecutorAsync();
        }

        public override void _ExitTree()
        {
        }
    }
}
