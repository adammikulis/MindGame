using Godot;

namespace MindGame
{
    [Tool]
    public partial class ModelConfigsParams : Resource
    {
        // Model configs exports
        [Export]
        public string ModelConfigsName { get; set; }

        // Chat model exports
        [Export]
        public uint ChatContextSize { get; set; }
        [Export]
        public int ChatGpuLayerCount { get; set; }
        [Export]
        public uint ChatRandomSeed { get; set; }
        [Export]
        public string ChatModelPath { get; set; }

        // Embedder model exports
        [Export]
        public uint EmbedderContextSize { get; set; }
        [Export]
        public int EmbedderGpuLayerCount { get; set; }
        [Export]
        public uint EmbedderRandomSeed { get; set; }
        [Export]
        public string EmbedderModelPath { get; set; }

        // Clip model exports
        [Export]
        public string ClipModelPath { get; set; }

        public ModelConfigsParams() : this("", 4000, 33, 0, "", 4000, 33, 0, "", "") { }

        public ModelConfigsParams(string modelConfigsName, uint chatContextSize, int chatGpuLayerCount, uint chatRandomSeed, string chatModelPath, uint embedderContextSize, int embedderGpuLayerCount, uint embedderRandomSeed, string embedderModelPath, string clipModelPath)
        {
            ModelConfigsName = modelConfigsName;
            ChatContextSize = chatContextSize;
            ChatGpuLayerCount = chatGpuLayerCount;
            ChatRandomSeed = chatRandomSeed;
            ChatModelPath = chatModelPath;
            EmbedderContextSize = embedderContextSize;
            EmbedderGpuLayerCount = embedderGpuLayerCount;
            EmbedderRandomSeed = embedderRandomSeed;
            EmbedderModelPath = embedderModelPath;
            ClipModelPath = clipModelPath;
        }
    }
}
    