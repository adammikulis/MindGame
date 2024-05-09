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
        public delegate void ExecutorStatusUpdateEventHandler(bool isLoaded);
        [Signal]
        public delegate void ChatSessionStatusUpdateEventHandler(bool isLoaded);
        [Signal]
        public delegate void ChatOutputReceivedEventHandler(string text);


        private MindManager mm;
        public string[] antiPrompts = ["<|eot_id|>", "<|end_of_text|>", "<|user|>", "User:", "USER:", "\nUser:", "\nUSER:"];
        public float temperature = 0.5f;
        public int maxTokens = 4000;

        public InteractiveExecutor executor { get; private set; } = null;
        public ChatSession chatSession { get; private set; } = null;

    
        public override void _EnterTree()
        {
        
        }

        public async override void _Ready()
        {
            try
            {
                mm = GetNode<MindManager>("/root/MindManager");
                if (mm.isReady == true)
                {
                    await InitializeAsync();
                }
            
            }
            catch (Exception e)
            {
                GD.PrintErr("Please ensure MindManager is enabled in Autoloads!");
            }

            mm.ChatModelStatusUpdate += OnChatModelStatusUpdate;
            mm.ClipModelStatusUpdate += OnClipModelStatusUpdate;
            mm.EmbedderModelStatusUpdate += OnEmbedderModelStatusUpdate;
            mm.ContextStatusUpdate += OnContextStatusUpdate;
        
        }

        private async void OnContextStatusUpdate(bool isLoaded)
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

        private async void OnChatModelStatusUpdate(bool isLoaded)
        {
            
        }


        public async Task InitializeAsync()
        {
        
            await CreateExecutorAsync();
            await CreateChatSessionAsync();
        }

    

        public async Task CreateExecutorAsync()
        {
            if (mm.context != null)
            {
                if (mm.clipWeights != null)
                {
                    await Task.Run(() =>
                    {
                        executor = new InteractiveExecutor(mm.context, mm.clipWeights);
                        CallDeferred("emit_signal", SignalName.ExecutorStatusUpdate, true);
                    });
                }
                else
                {
                    await Task.Run(() =>
                    {
                        executor = new InteractiveExecutor(mm.context);
                        CallDeferred("emit_signal", SignalName.ExecutorStatusUpdate, true);
                    });
                }
            }
            else
            {
                GD.PrintErr("Executor not initialized.");
            }
        }

        public async Task CreateChatSessionAsync()
        {
            await Task.Run(() =>
            {
                chatSession = new ChatSession(executor);
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
                executor.ImagePaths.Clear();
                executor.ImagePaths.AddRange(imagePaths);
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



        public async Task DisposeExecutorAsync()
        {
            await Task.Run(() =>
            {
                executor = null;
                CallDeferred("emit_signal", SignalName.ExecutorStatusUpdate, false);
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
            await DisposeExecutorAsync();
        

        }

        public override void _ExitTree() 
        { 
        
        }
    }
}