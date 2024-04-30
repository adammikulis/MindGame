using Godot;
using LLama;
using LLama.Common;
using LLama.Abstractions;
using LLama.Native;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System;


[Tool]
public partial class ChatInterface : Control
{
    [Signal]
    public delegate void ModelOutputReceivedEventHandler(string text);

    private Button clearOutputButton, uploadImageButton, uploadViewportButton, loadChatSessionButton, newChatSessionButton, saveChatSessionButton;
    private FileDialog uploadImageFileDialog;

    private LLamaEmbedder embedder;
    private InteractiveExecutor executor;
    private ChatSession chatSession;


    private RichTextLabel modelOutputRichTextLabel;
    private LineEdit promptLineEdit;

    private string[] antiPrompts;


    public override void _Ready()
    {
        // Chat session management
        newChatSessionButton = GetNode<Button>("%NewChatSessionButton");
        
        // Chat session buttons
        clearOutputButton = GetNode<Button>("%ClearOutputButton");
        uploadImageButton = GetNode<Button>("%UploadImageButton");
        uploadImageFileDialog = GetNode<FileDialog>("%UploadImageFileDialog");
        
        // Chat session I/O
        modelOutputRichTextLabel = GetNode<RichTextLabel>("%ModelOutputRichTextLabel");
        promptLineEdit = GetNode<LineEdit>("%PromptLineEdit");

        // Signals
        clearOutputButton.Pressed += OnClearOutputPressed;
        promptLineEdit.TextSubmitted += OnPromptSubmitted;
        ModelOutputReceived += OnModelOutputReceived;
        uploadImageButton.Pressed += OnUploadImagePressed;
        uploadImageFileDialog.FilesSelected += OnImageFilePathsSelected;

        antiPrompts = ["<|eot_id|>", "<|end_of_text|>", "\nUser:", "\nUSER:", "\n\nUser:", "\n\nUSER:"];

    }

    private void OnClearOutputPressed()
    {
        modelOutputRichTextLabel.Text = "";
    }

    private async void OnImageFilePathsSelected(string[] paths)
    {
        executor.ImagePaths.Clear();
        executor.ImagePaths.AddRange(paths);
    }

    private void OnUploadImagePressed()
    {
        uploadImageFileDialog.PopupCentered();
    }

    private void OnChatSessionStatusUpdated()
    {
        if (chatSession != null)
        {
            promptLineEdit.Editable = true;
            promptLineEdit.PlaceholderText = "Enter prompt here";
            clearOutputButton.Disabled = false;
        }
        else
        {
            chatSession = null;
            promptLineEdit.Editable = false;
            promptLineEdit.PlaceholderText = "Load model first to chat";
            clearOutputButton.Disabled = true;
        }
    }

    private void OnModelOutputReceived(string output)
    {
        modelOutputRichTextLabel.Text += output;
    }

    private async void OnPromptSubmitted(string prompt)
    {
        await InferAsync(prompt);
    }

    public async Task InferAsync(string prompt)
    {
        if (chatSession != null)
        {
            promptLineEdit.Text = "";
            modelOutputRichTextLabel.Text += "\n\n" + prompt + "\n\n";
            await Task.Run(async () =>
            {
                await foreach (var output in chatSession.ChatAsync(new ChatHistory.Message(AuthorRole.User, prompt), new InferenceParams { Temperature = 0.5f, AntiPrompts = antiPrompts }))
                {
                    bool isAntiPrompt = false;
                    foreach (var antiPrompt in antiPrompts)
                    {
                        if (output.Contains(antiPrompt))
                        {
                            isAntiPrompt = true;
                            break;
                        }
                    }

                    if (!isAntiPrompt)
                    {
                        CallDeferred(nameof(DeferredEmitNewOutput), output);
                    }
                }
            });
        }
    }

    public void DeferredEmitNewOutput(string output)
    {
        EmitSignal(nameof(ModelOutputReceived), output);
    }


    public ChatSession GetChatSession()
    {
        return chatSession;
    }

    public void InitializeChatSession()
    {
        chatSession = new(executor);
        OnChatSessionStatusUpdated();
        GD.Print("Chat session initialized!");
    }


    public void SetExecutor(InteractiveExecutor executor)
    {
        this.executor = executor;
    }


    public void SetEmbedder(LLamaEmbedder embedder)
    {
        this.embedder = embedder;
    }


    public override void _ExitTree()
    {
        // promptLineEdit.TextSubmitted -= OnPromptSubmitted;
        ModelOutputReceived -= OnModelOutputReceived;

    }
}