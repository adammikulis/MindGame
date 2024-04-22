#if TOOLS
using Godot;
using LLama;
using LLama.Common;
using System;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

[Tool]
public partial class ChatInterface : Control
{
    [Signal]
    public delegate void ModelOutputEventHandler(string text);

    private LLamaEmbedder embedder;
    private InteractiveExecutor executor;
    private ChatSession chatSession;


    private RichTextLabel modelOutputRichTextLabel;
    private LineEdit promptLineEdit;

    private string[] antiPrompts;


    

    public override void _Ready()
    {
        modelOutputRichTextLabel = GetNode<RichTextLabel>("%ModelOutputRichTextLabel");
        promptLineEdit = GetNode<LineEdit>("%PromptLineEdit");

        promptLineEdit.TextSubmitted += OnPromptSubmitted;
        ModelOutput += OnModelOutput;

        antiPrompts = ["<|eot_id|>", "\nUser:", "\nUSER:"];

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
            modelOutputRichTextLabel.Text += "\n" + prompt + "\n";
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

    public void InitializeExecutor(LLamaContext context)
    {
        executor = new InteractiveExecutor(context);

    }


    public void SetEmbedder(LLamaEmbedder embedder)
    { this.embedder = embedder; }


    public override void _ExitTree()
    {
        promptLineEdit.TextSubmitted -= OnPromptSubmitted;
        ModelOutput -= OnModelOutput;

    }
}
#endif