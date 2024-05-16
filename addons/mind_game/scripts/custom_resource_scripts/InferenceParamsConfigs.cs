using Godot;
using LLama.Grammars;

namespace MindGame
{
    [Tool]
    public partial class InferenceParamsConfigs : Resource
    {
        [Export]
        public string InferenceParamsName { get; set; }
        [Export]
        public string[] AntiPrompts { get; set; }
        [Export]
        public float Temperature { get; set; }
        [Export]
        public int MaxTokens { get; set; }
        [Export]
        public bool OutputJson { get; set; }

        public InferenceParamsConfigs() : this("<default>", ["<|eot_id|>", "<|end_of_text|>", "<|user|>", "<|end|>", "user:", "User:", "USER:", "\nUser:", "\nUSER:", "}"], 0.5f, 4000, false) { }
        public InferenceParamsConfigs(string inferenceParamsName, string[] antiPrompts, float temperature, int maxTokens, bool outputJson)
        {
            InferenceParamsName = inferenceParamsName;
            AntiPrompts = antiPrompts;
            Temperature = temperature;
            MaxTokens = maxTokens;
            OutputJson = outputJson;
        }
    }
}