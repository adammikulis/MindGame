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
    private const string MindGameAutoload = "mg";

    private Control editorInterface;
    private Button loadModelButton, unloadModelButton;
    private RichTextLabel modelOutputRichTextLabel;
    private LineEdit modelInputLineEdit;
    private FileDialog loadModelFileDialog;

    public MindGameModel mg;

    public string modelPath;
    public override void _EnterTree()
    {
        AddAutoloadSingleton(MindGameAutoload, "res://addons/mind_game/MindGameModel.cs");
        mg = GetNode<MindGameModel>("/root/MindGameModel");

        PackedScene mindGameInterfaceScene = (PackedScene)GD.Load("res://addons/mind_game/MindGameEditorInterface.tscn");
        editorInterface = mindGameInterfaceScene.Instantiate<Control>();
        AddControlToBottomPanel(editorInterface, "Mind Game");

        loadModelButton = editorInterface.GetNode<Button>("%LoadModelButton");
        unloadModelButton = editorInterface.GetNode<Button>("%UnloadModelButton");
        modelOutputRichTextLabel = editorInterface.GetNode<RichTextLabel>("%ModelOutputRichTextLabel");
        modelInputLineEdit = editorInterface.GetNode<LineEdit>("%ModelInputLineEdit");
        loadModelFileDialog = editorInterface.GetNode<FileDialog>("%LoadModelFileDialog");

        loadModelButton.Pressed += OnLoadModelButtonPressed;
        loadModelFileDialog.FileSelected += OnModelSelected;

        // base._EnterTree(); // Autofilled by VS, may not be needed
        base._Ready();
    }

    private void OnLoadModelButtonPressed()
    {
        loadModelFileDialog.PopupCentered();
    }

    private void OnModelSelected(string modelPath)
    {
        mg.LoadModel(modelPath);
    }

    public override void _Ready()
    {
        
    }


    public override void _Process(double delta)
    {
        
    }


    public override void _ExitTree()
    {
        RemoveAutoloadSingleton(MindGameAutoload);
        RemoveControlFromBottomPanel(editorInterface);
        editorInterface.QueueFree();

    }
}
#endif