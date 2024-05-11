using Godot;
using LLama;
using LLama.Common;
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

        private MindManager mindManager;
        public string[] antiPrompts = ["<|eot_id|>", "<|end_of_text|>", "<|user|>", "<|end|>", "user:", "User:", "USER:", "\nUser:", "\nUSER:", "}"];
        public float temperature = 0.75f;
        public int maxTokens = 4000;

        
        public ChatSession chatSession { get; private set; } = null;

    
        public override void _EnterTree()
        {
        
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
                GD.PrintErr("Please ensure MindManager is enabled in Autoloads!");
            }

            mindManager.ClipModelStatusUpdate += OnClipModelStatusUpdate;
            mindManager.EmbedderModelStatusUpdate += OnEmbedderModelStatusUpdate;
            mindManager.ExecutorStatusUpdate += OnExecutorStatusUpdate;
        
        }

        private async void OnExecutorStatusUpdate(bool isLoaded)
        {
            if (isLoaded)
            {
                await InitializeAsync();
            }
        }

        private void OnEmbedderModelStatusUpdate(bool isLoaded)
        {
        
        }

        private void OnClipModelStatusUpdate(bool isLoaded)
        {
        
        }

        public async Task InitializeAsync()
        {

            await CreateChatSessionAsync();
        }

        public async Task CreateChatSessionAsync()
        {
            await Task.Run(() =>
            {
                chatSession = new ChatSession(mindManager.executor);
                CallDeferred("emit_signal", SignalName.ChatSessionStatusUpdate, true);
            });
        }

        public async Task InferAsync(string prompt, List<string> imagePaths = null)
        {
            if (chatSession == null)
            {
                GD.PrintErr("Chat session not initialized. Please check the model configuration.");
                return;
            }

            // Handle image paths by setting them in the executor
            if (imagePaths != null && imagePaths.Count > 0)
            {
                mindManager.executor.ImagePaths.Clear();
                mindManager.executor.ImagePaths.AddRange(imagePaths);
            }

            // Execute the chat session with the current prompt and any images
            await Task.Run(async () =>
            {
                await foreach (var output in chatSession.ChatAsync(new ChatHistory.Message(AuthorRole.User, prompt), new InferenceParams { AntiPrompts = antiPrompts, Temperature = temperature, MaxTokens = maxTokens }))
                {
                    CallDeferred("emit_signal", SignalName.ChatOutputReceived, output);
                }
            });
        }

        public async Task DisposeChatSessionAsync()
        {
            await Task.Run(() =>
            {
                chatSession = null;
                CallDeferred("emit_signal", SignalName.ChatSessionStatusUpdate, false);
            });
        }

        public async Task DisposeAllAsync()
        {
            await DisposeChatSessionAsync();

        }

        public override void _ExitTree() 
        { 
        
        }
    }
}