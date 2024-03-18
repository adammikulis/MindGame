#if TOOLS
using Godot;
using System;
using System.Threading.Tasks;
using LLama.Common;
using LLama.Native;
using LLama.Sampling;
using LLama;

[Tool]
public partial class MindGameEditorPlugin : EditorPlugin, IDisposable
{

    [Signal]
    public delegate void ModelOutputEventHandler(string text);

    //private const string AutoloadName = "model";
    //private const string pathToScript = @"res://addons/mind_game/scripts/MindGameModel.cs";
    private LLamaEmbedder embedder;
    private ChatSession chatSession;

    private string downloadModelDirectoryPath;
    private Control editorInterface;
    private Button chooseDownloadLocationButton, downloadModelButton;
    private RichTextLabel modelOutputRichTextLabel;
    private LineEdit promptLineEdit;
    private FileDialog downloadModelFileDialog;

    public ModelInterface model;
    

    public string modelPath;
    public override void _EnterTree()
    {
        // Singleton removed for now
        // AddAutoloadSingleton(AutoloadName, pathToScript);
        
        PackedScene mindGameInterfaceScene = (PackedScene)GD.Load("res://addons/mind_game/scenes/MindGameEditorInterface.tscn");
        editorInterface = mindGameInterfaceScene.Instantiate<Control>();
        AddControlToBottomPanel(editorInterface, "Mind Game");

        

        // Button nodes
        chooseDownloadLocationButton = editorInterface.GetNode<Button>("%ChooseDownloadLocationButton");
        downloadModelButton = editorInterface.GetNode<Button>("%DownloadModelButton");
        
        // Label nodes
        modelOutputRichTextLabel = editorInterface.GetNode<RichTextLabel>("%ModelOutputRichTextLabel");
        promptLineEdit = editorInterface.GetNode<LineEdit>("%PromptLineEdit");
        
        // File dialog node
        downloadModelFileDialog = editorInterface.GetNode<FileDialog>("%DownloadModelFileDialog");

        // Signals
        chooseDownloadLocationButton.Pressed += OnChooseDownloadLocationButtonPressed;
        downloadModelButton.Pressed += OnDownloadModelButtonPressed;
        downloadModelFileDialog.DirSelected += OnDownloadModelDirectorySelected;
        promptLineEdit.TextSubmitted += OnPromptSubmitted;
        ModelOutput += OnModelOutput;

    }

    private void OnEmbedderStatusUpdated(bool isModelEmbedderActive)
    {
        if (isModelEmbedderActive)
        {
            embedder = model.GetLLamaEmbedder();
        }
        else
        {
            embedder = null;
        }
    }

    private void OnChatSessionStatusUpdated(bool isChatSessionActive)
    {
        if (isChatSessionActive)
        {
            chatSession = model.GetChatSession();
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

    private void OnDownloadModelDirectorySelected(string dir)
    {
        downloadModelDirectoryPath = dir;
        downloadModelButton.Disabled = false;
    }

    private void OnDownloadModelButtonPressed()
    {
        // Need to add HTTP request here
    }

    private void OnChooseDownloadLocationButtonPressed()
    {
        downloadModelFileDialog.PopupCentered();
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
            modelOutputRichTextLabel.Text = $"Prompt: {prompt}\n\nResponse:\n";
            await Task.Run(async () =>
            {
                await foreach (var output in chatSession.ChatAsync(new ChatHistory.Message(AuthorRole.User, prompt), new InferenceParams { Temperature = 0.5f, AntiPrompts = new[] { "\n\n", "User:" } }))
                {
                    CallDeferred(nameof(DeferredEmitNewOutput), output);
                }
            });
        }
    }

    public void DeferredEmitNewOutput(string output)
    {
        EmitSignal(nameof(ModelOutput), output);
    }




    public override void _Ready()
    {
        Control modelNode = editorInterface.GetNode<Control>("%Model") as ModelInterface;
        model = modelNode as ModelInterface;

        model.ChatSessionStatus += OnChatSessionStatusUpdated;
        model.EmbedderStatus += OnEmbedderStatusUpdated;

    }


    public override void _Process(double delta)
    {
        
    }


    public override void _ExitTree()
    {
        chooseDownloadLocationButton.Pressed -= OnChooseDownloadLocationButtonPressed;
        downloadModelButton.Pressed -= OnDownloadModelButtonPressed;
        downloadModelFileDialog.DirSelected -= OnDownloadModelDirectorySelected;
        promptLineEdit.TextSubmitted -= OnPromptSubmitted;
        ModelOutput -= OnModelOutput;

        //RemoveAutoloadSingleton(AutoloadName);
        RemoveControlFromBottomPanel(editorInterface);
        editorInterface.QueueFree();

    }
}
#endif