using Godot;

namespace MindGame
{
    [Tool]
    public partial class InferenceParams : Resource
    {
        [Export]
        public string InferenceConfigName { get; set; }
        [Export]
        public string[] AntiPrompts { get; set; }
        [Export]
        public float Temperature { get; set; }
        [Export]
        public int MaxTokens { get; set; }
        [Export]
        public bool OutputJson { get; set; }

        public InferenceParams() : this("<default>", ["<|eot_id|>", "<|end_of_text|>", "<|user|>", "<|end|>", "user:", "User:", "USER:", "\nUser:", "\nUSER:", "}"], 0.75f, 4000, false) { }
        public InferenceParams(string inferenceParamsName, string[] antiPrompts, float temperature, int maxTokens, bool outputJson)
        {
            InferenceConfigName = inferenceParamsName;
            AntiPrompts = antiPrompts;
            Temperature = temperature;
            MaxTokens = maxTokens;
            OutputJson = outputJson;
        }
    }
}