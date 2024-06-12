using Godot;
using LLama;
using LLama.Common;
using LLama.Grammars;
using LLama.Native;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MindGame
{
    [Tool]
    public partial class MindAgent : Node
    {

        [Signal]
        public delegate void ChatOutputReceivedEventHandler(string text);

        private ConfigListResource configListResource;
        private MindManager mindManager;
        private Grammar grammar;
        private SafeLLamaGrammarHandle grammarInstance;
        public string[] antiPrompts = { "<|eot_id|>", "<|end|>",  "user:", "User:", "USER:", "\nUser:", "\nUSER:", "}" };
        public float temperature = 0.75f;
        public int maxTokens = 4000;
        public bool outputJson = false;

        public override void _EnterTree()
        {
            configListResource = GD.Load<ConfigListResource>("res://addons/mind_game/assets/resources/custom_resources/ConfigListResource.tres");
        }

        /// <summary>
        /// Function that is called when node and all children are initialized
        /// </summary>
        public async override void _Ready()
        {
            try
            {
                mindManager = GetNode<MindManager>("/root/MindManager");
                if (mindManager.batchedExecutor != null)
                {
                    await InitializeAsync();
                }
            }
            catch (Exception e)
            {
                GD.PrintErr("Please ensure MindManager is enabled in Autoloads!\n" + e);
            }

            mindManager.ClipModelStatusUpdate += OnClipModelStatusUpdate;
            mindManager.EmbedderModelStatusUpdate += OnEmbedderModelStatusUpdate;
            mindManager.ExecutorStatusUpdate += OnExecutorStatusUpdate;

            // Load the grammar definition
            using var file = FileAccess.Open("res://addons/mind_game/assets/grammar/json.gbnf", FileAccess.ModeFlags.Read);
            string gbnf = file.GetAsText().Trim();
            grammar = Grammar.Parse(gbnf, "root");
        }


        private async void OnExecutorStatusUpdate(bool isLoaded)
        {
            if (isLoaded)
            {
                await InitializeAsync();
            }
        }

        private void OnEmbedderModelStatusUpdate(bool isLoaded) { }

        private void OnClipModelStatusUpdate(bool isLoaded) { }

        public async Task InitializeAsync()
        {
            // No longer need a chat session, will likely remove this method
        }


        public async Task InferAsync(string prompt, List<string> imagePaths = null)
        {

            var activeConfig = configListResource.CurrentInferenceConfig;
            if (activeConfig == null)
            {
                GD.PrintErr("No active inference configuration selected.");
                return;
            }

            SafeLLamaGrammarHandle grammarInstance = null;
            if (activeConfig.OutputJson)
            {
                grammarInstance = grammar.CreateInstance();
            }

            InferenceParams inferenceParams = new InferenceParams
            {
                AntiPrompts = activeConfig.AntiPrompts,
                Temperature = activeConfig.Temperature,
                MaxTokens = activeConfig.MaxTokens,
                Grammar = grammarInstance
            };

            try
            {
                await Task.Run(async () =>
                {
                    // Put BatchedExecutor inference here
                });
            }
            finally
            {
                grammarInstance?.Dispose();
            }
        }

        public override void _ExitTree() { }
    }
}
