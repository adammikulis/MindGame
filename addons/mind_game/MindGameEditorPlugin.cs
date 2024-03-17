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

    public LLamaWeights weights;
    public LLamaContext context;
    public LLamaEmbedder embedder;
    public InteractiveExecutor executor;
    public ChatSession session;

    // Autoload isn't able to be located for some reason, commenting out for now
    //private const string AutoloadName = "MindGameModel";
    //private const string pathToScript = @"res://addons/mind_game/MindGameModel.cs";

    private string downloadModelDirectoryPath;

    private Control editorInterface;
    private Button chooseDownloadLocationButton, downloadModelButton, loadModelButton, unloadModelButton;
    private RichTextLabel modelOutputRichTextLabel;
    private LineEdit promptLineEdit;
    private FileDialog downloadModelFileDialog, loadModelFileDialog;
    private HSlider 


    public string modelPath;
    public override void _EnterTree()
    {
        //AddAutoloadSingleton(AutoloadName, pathToScript);
        
        PackedScene mindGameInterfaceScene = (PackedScene)GD.Load("res://addons/mind_game/MindGameEditorInterface.tscn");
        editorInterface = mindGameInterfaceScene.Instantiate<Control>();
        AddControlToBottomPanel(editorInterface, "Mind Game");

        chooseDownloadLocationButton = editorInterface.GetNode<Button>("%ChooseDownloadLocationButton");
        downloadModelButton = editorInterface.GetNode<Button>("%DownloadModelButton");
        loadModelButton = editorInterface.GetNode<Button>("%LoadModelButton");
        unloadModelButton = editorInterface.GetNode<Button>("%UnloadModelButton");
        modelOutputRichTextLabel = editorInterface.GetNode<RichTextLabel>("%ModelOutputRichTextLabel");
        promptLineEdit = editorInterface.GetNode<LineEdit>("%PromptLineEdit");
        loadModelFileDialog = editorInterface.GetNode<FileDialog>("%LoadModelFileDialog");

        chooseDownloadLocationButton.Pressed += OnChooseDownloadLocationButtonPressed;
        downloadModelButton.Pressed += OnDownloadModelButtonPressed;
        loadModelButton.Pressed += OnLoadModelButtonPressed;
        unloadModelButton.Pressed += OnUnloadModelButtonPressed;

        downloadModelFileDialog.DirSelected += OnDownloadModelDirectorySelected;
        loadModelFileDialog.FileSelected += OnModelSelected;


        promptLineEdit.TextSubmitted += OnPromptSubmitted;

        
        ModelOutput += OnModelOutput;

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

    private void OnUnloadModelButtonPressed()
    {
        UnloadModel();

    }

    private void OnLoadModelButtonPressed()
    {
        loadModelFileDialog.PopupCentered();
    }

    private async void OnPromptSubmitted(string prompt)
    {
        InferAsync(prompt);
    }

    private void OnModelSelected(string modelPath)
    {
       LoadModel(modelPath);
    }

    public void LoadModel(string modelPath)
    {
        var parameters = new ModelParams(modelPath)
        {
            ContextSize = 4096,
            Seed = 0,
            GpuLayerCount = 33,
            EmbeddingMode = true
        };

        weights = LLamaWeights.LoadFromFile(parameters);
        context = weights.CreateContext(parameters);
        embedder = new LLamaEmbedder(weights, parameters);
        executor = new InteractiveExecutor(context);
        session = new ChatSession(executor);

        GD.Print("Model loaded!");
    }

    public void UnloadModel()
    {
        weights.Dispose();
        context.Dispose();
        embedder.Dispose();
    }

    public async Task InferAsync(string prompt)
    {
        promptLineEdit.Text = "";
        modelOutputRichTextLabel.Text = $"Prompt: {prompt}\n\nResponse:\n";
        await Task.Run(async () =>
        {
            await foreach (var output in session.ChatAsync(new ChatHistory.Message(AuthorRole.User, prompt), new InferenceParams { Temperature = 0.5f, AntiPrompts = new[] { "\n\n" } }))
            {
                CallDeferred(nameof(DeferredEmitNewOutput), output);
            }
        });
    }

    public void DeferredEmitNewOutput(string output)
    {
        EmitSignal(nameof(ModelOutput), output);
    }




    public override void _Ready()
    {
        
    }


    public override void _Process(double delta)
    {
        
    }


    public override void _ExitTree()
    {
        //loadModelButton.Pressed -= OnLoadModelButtonPressed;
        //unloadModelButton.Pressed -= OnUnloadModelButtonPressed;
        //loadModelFileDialog.FileSelected -= OnModelSelected;
        //promptLineEdit.TextSubmitted -= OnPromptSubmitted;

        //RemoveAutoloadSingleton(AutoloadName);
        RemoveControlFromBottomPanel(editorInterface);
        // editorInterface.QueueFree();

    }
}
#endif