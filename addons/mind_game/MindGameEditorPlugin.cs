#if TOOLS
using Godot;
using System;
using System.Threading.Tasks;
using LLama.Common;
using LLama.Native;
using LLama.Sampling;

[Tool]
public partial class MindGameEditorPlugin : EditorPlugin
{
    private const string AutoloadName = "MindGame";
    private const string pathToScript = @"res://addons/mind_game/MindGameModel.cs";

    private Control editorInterface;
    private Button loadModelButton, unloadModelButton;
    private RichTextLabel modelOutputRichTextLabel;
    private LineEdit promptLineEdit;
    private FileDialog loadModelFileDialog;

    public string modelPath;
    public override void _EnterTree()
    {
        AddAutoloadSingleton(AutoloadName, pathToScript);

        PackedScene mindGameInterfaceScene = (PackedScene)GD.Load("res://addons/mind_game/MindGameEditorInterface.tscn");
        editorInterface = mindGameInterfaceScene.Instantiate<Control>();
        AddControlToBottomPanel(editorInterface, "Mind Game");

        loadModelButton = editorInterface.GetNode<Button>("%LoadModelButton");
        unloadModelButton = editorInterface.GetNode<Button>("%UnloadModelButton");
        modelOutputRichTextLabel = editorInterface.GetNode<RichTextLabel>("%ModelOutputRichTextLabel");
        promptLineEdit = editorInterface.GetNode<LineEdit>("%PromptLineEdit");
        loadModelFileDialog = editorInterface.GetNode<FileDialog>("%LoadModelFileDialog");

        loadModelButton.Pressed += OnLoadModelButtonPressed;
        unloadModelButton.Pressed += OnUnloadModelButtonPressed;
        loadModelFileDialog.FileSelected += OnModelSelected;
        promptLineEdit.TextSubmitted += OnPromptSubmitted;
        //MindGameModel.Instance.ModelOutput += OnModelOutput;

    }

    private void OnModelOutput(string output)
    {
        GD.Print(output);
    }

    private void OnUnloadModelButtonPressed()
    {
        //MindGameModel.Instance.UnloadModel();

    }

    private void OnLoadModelButtonPressed()
    {
        
        loadModelFileDialog.PopupCentered();
    }

    private async void OnPromptSubmitted(string prompt)
    {
        //await MindGameModel.Instance.InferAsync(prompt);
    }

    private void OnModelSelected(string modelPath)
    {
       // MindGameModel.Instance.LoadModel(modelPath);
    }

    public override void _Ready()
    {
        
    }


    public override void _Process(double delta)
    {
        
    }


    public override void _ExitTree()
    {
        loadModelButton.Pressed -= OnLoadModelButtonPressed;
        unloadModelButton.Pressed -= OnUnloadModelButtonPressed;
        loadModelFileDialog.FileSelected -= OnModelSelected;
        promptLineEdit.TextSubmitted -= OnPromptSubmitted;

        RemoveAutoloadSingleton(AutoloadName);
        RemoveControlFromBottomPanel(editorInterface);
        editorInterface.QueueFree();

    }
}
#endif