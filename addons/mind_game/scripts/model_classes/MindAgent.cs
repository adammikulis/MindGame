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
        public delegate void ChatSessionStatusUpdateEventHandler(bool isLoaded);
        [Signal]
        public delegate void ChatOutputReceivedEventHandler(string text);

        private ConfigListResource configListResource;
        private MindManager mindManager;
        private Grammar grammar;
        private SafeLLamaGrammarHandle grammarInstance;
        public string[] antiPrompts = ["", "", "", "", "user:", "User:", "USER:", "\nUser:", "\nUSER:", "}"];
        public float temperature = 0.75f;
        public int maxTokens = 4000;
        public bool outputJson = false;


        public ChatSession ChatSession { get; private set; } = null;


        public override void _EnterTree()
        {
            configListResource = GD.Load<ConfigListResource>("res://addons/mind_game/assets/resources/custom_resources/ConfigListResource.tres");
        }


        public async override void _Ready()
        {
            try
            {
                mindManager = GetNode<MindManager>("/root/MindManager");
                if (mindManager.executor != null)
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

        private SafeLLamaGrammarHandle CreateGrammarInstance()
        {
            return grammar.CreateInstance();
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
            await CreateChatSessionAsync();
        }

        public async Task CreateChatSessionAsync()
        {
            await Task.Run(() =>
            {
                ChatSession = new ChatSession(mindManager.executor);
                CallDeferred("emit_signal", SignalName.ChatSessionStatusUpdate, true);
            });
        }

        public async Task InferAsync(string prompt, List<string> imagePaths = null)
        {
            if (ChatSession == null)
            {
                GD.PrintErr("Chat session not initialized. Please check the model configuration.");
                return;
            }

            if (imagePaths != null && imagePaths.Count > 0)
            {
                mindManager.executor.ImagePaths.Clear();
                mindManager.executor.ImagePaths.AddRange(imagePaths);
            }

            var activeConfig = configListResource.CurrentInferenceConfig;
            if (activeConfig == null)
            {
                GD.PrintErr("No active inference configuration selected.");
                return;
            }

            SafeLLamaGrammarHandle grammarInstance = null;
            if (activeConfig.OutputJson)
            {
                grammarInstance = CreateGrammarInstance();
            }

            InferenceParams inferenceParams = new InferenceParams
            {
                AntiPrompts = activeConfig.AntiPrompts,
                Temperature = activeConfig.Temperature,
                MaxTokens = activeConfig.MaxTokens,
                Grammar = grammarInstance
            };

            await Task.Run(async () =>
            {
                await foreach (var output in ChatSession.ChatAsync(new ChatHistory.Message(AuthorRole.User, prompt), inferenceParams))
                {
                    CallDeferred("emit_signal", SignalName.ChatOutputReceived, output);
                }
            });
        }

        public async Task DisposeChatSessionAsync()
        {
            await Task.Run(() =>
            {
                ChatSession = null;
                CallDeferred("emit_signal", SignalName.ChatSessionStatusUpdate, false);
            });
        }

        public async Task DisposeAllAsync()
        {
            await DisposeChatSessionAsync();
        }

        public override void _ExitTree() { }
    }
}
