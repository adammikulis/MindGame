#if TOOLS
using Godot;
using LLama;
using LLama.Common;
using LLama.Abstractions;
using LLama.Native;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Collections.Generic;


[Tool]
public partial class ChatInterface : Control
{
    [Signal]
    public delegate void ModelOutputEventHandler(string text);

    private Button uploadImageButton, uploadViewportButton, loadChatSessionButton, newChatSessionButton, saveChatSession;
    private FileDialog uploadImageFileDialog;

    private LLamaEmbedder embedder;
    private InteractiveExecutor executor;
    private ChatSession chatSession;


    private RichTextLabel modelOutputRichTextLabel;
    private LineEdit promptLineEdit;

    private string[] antiPrompts;
    private string[] imagePaths;




    public override void _Ready()
    {
        uploadImageButton = GetNode<Button>("%UploadImageButton");
        uploadImageFileDialog = GetNode<FileDialog>("%UploadImageFileDialog");
        modelOutputRichTextLabel = GetNode<RichTextLabel>("%ModelOutputRichTextLabel");
        promptLineEdit = GetNode<LineEdit>("%PromptLineEdit");

        promptLineEdit.TextSubmitted += OnPromptSubmitted;
        ModelOutput += OnModelOutput;
        uploadImageButton.Pressed += OnUploadImagePressed;
        uploadImageFileDialog.FilesSelected += OnImageFilePathsSelected;

        antiPrompts = ["<|eot_id|>", "\nUser:", "\nUSER:"];

    }

    private async void OnImageFilePathsSelected(string[] paths)
    {
        foreach (var imagePath in paths)
        {
            executor.ImagePaths.Add(imagePath);
        }
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
        }
        else
        {
            chatSession = null;
            promptLineEdit.Editable = false;
            promptLineEdit.PlaceholderText = "Load model first to chat";
        }
    }

    private void OnModelOutput(string output)
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
        EmitSignal(nameof(ModelOutput), output);
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
        promptLineEdit.TextSubmitted -= OnPromptSubmitted;
        ModelOutput -= OnModelOutput;

    }
}
#endif