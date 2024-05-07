using Godot;
using LLama.Common;
using LLama;
using System;
using static System.Collections.Specialized.BitVector32;

[Tool]
public partial class ModelConfigsController : Control
{

    private const string ConfigFilePath = "res://addons/mind_game/model_configs.json";

    // UI elements
    private Button addNewConfigButton, deleteConfigButton, selectChatPathButton, clearChatPathButton, selectEmbedderPathButton, clearEmbedderPathButton, selectClipPathButton, clearClipPathButton;
    private Label chatContextSizeLabel, chatGpuLayerCountLabel, embedderContextSizeLabel, embedderGpuLayerCountLabel;
    private FileDialog selectChatPathFileDialog, selectClipPathFileDialog, selectEmbedderPathFileDialog;
    private HSlider chatContextSizeHSlider, chatGpuLayerCountHSlider, embedderContextSizeHSlider, embedderGpuLayerCountHSlider;
    private LineEdit configNameLineEdit;
    private ItemList savedConfigsItemList;

    // Model params
    private string configName;
    private int chatGpuLayerCount, embedderGpuLayerCount;
    private double chatContextSizeSliderValue, embedderContextSizeSliderValue;
    private uint chatContextSize, embedderContextSize, chatSeed, embedderSeed;
    private string chatModelPath, clipModelPath, embedderModelPath;

    public override void _EnterTree()
    {
        
    }

    public override void _Ready()
    {
        InitializeNodeRefs();
        InitializeSignals();
        InitializeParameters();
    }

    private void InitializeNodeRefs()
    {
        addNewConfigButton = GetNode<Button>("%AddNewConfigButton");
        deleteConfigButton = GetNode<Button>("%DeleteConfigButton");
        selectChatPathButton = GetNode<Button>("%SelectChatPathButton");
        clearChatPathButton = GetNode<Button>("%ClearChatPathButton");
        selectClipPathButton = GetNode<Button>("%SelectClipPathButton");
        clearClipPathButton = GetNode<Button>("%ClearClipPathButton");
        selectEmbedderPathButton = GetNode<Button>("%SelectEmbedderPathButton");
        clearEmbedderPathButton = GetNode<Button>("%ClearEmbedderPathButton");

        selectChatPathFileDialog = GetNode<FileDialog>("%SelectChatPathFileDialog");
        selectClipPathFileDialog = GetNode<FileDialog>("%SelectClipPathFileDialog");
        selectEmbedderPathFileDialog = GetNode<FileDialog>("%SelectEmbedderPathFileDialog");

        chatContextSizeHSlider = GetNode<HSlider>("%ChatContextSizeHSlider");
        chatGpuLayerCountHSlider = GetNode<HSlider>("%ChatModelGpuLayerCountHSlider");
        embedderContextSizeHSlider = GetNode<HSlider>("%EmbedderContextSizeHSlider");
        embedderGpuLayerCountHSlider = GetNode<HSlider>("%EmbedderGpuLayerCountHSlider");

        savedConfigsItemList = GetNode<ItemList>("%SavedConfigsItemList");

        chatContextSizeLabel = GetNode<Label>("%ChatContextSizeLabel");
        chatGpuLayerCountLabel = GetNode<Label>("%ChatModelGpuLayerCountLabel");
        embedderContextSizeLabel = GetNode<Label>("%EmbedderContextSizeLabel");
        embedderGpuLayerCountLabel = GetNode<Label>("%EmbedderGpuLayerCountLabel");

        configNameLineEdit = GetNode<LineEdit>("%ConfigNameLineEdit");
        
    }

    private void InitializeSignals()
    {
        // Chat signals

        addNewConfigButton.Pressed += OnAddNewConfigPressed;
        deleteConfigButton.Pressed += OnDeleteConfigPressed;

        clearChatPathButton.Pressed += OnClearChatPathPressed;
        clearClipPathButton.Pressed += OnClearClipPathPressed;
        clearEmbedderPathButton.Pressed += OnClearEmbedderPathPressed;

        selectChatPathButton.Pressed += OnSelectChatPathPressed;
        selectClipPathButton.Pressed += OnSelectClipPathPressed;
        selectEmbedderPathButton.Pressed += OnSelectEmbedderPathPressed;


        chatContextSizeHSlider.ValueChanged += OnChatContextSizeHSliderValueChanged;
        chatGpuLayerCountHSlider.ValueChanged += OnChatGpuLayerCountHSliderValueChanged;
        embedderContextSizeHSlider.ValueChanged += OnEmbedderContextSizeHSliderValueChanged;
        embedderGpuLayerCountHSlider.ValueChanged += OnEmbedderGpuLayerCountHSliderValueChanged;

        selectChatPathFileDialog.FileSelected += OnChatPathSelected;
        selectClipPathFileDialog.FileSelected += OnClipPathSelected;
        selectEmbedderPathFileDialog.FileSelected += OnEmbedderPathSelected;

        configNameLineEdit.TextChanged += OnConfigNameTextChanged;

        savedConfigsItemList.ItemSelected += OnSavedConfigsItemSelected;

    }




    private void OnSavedConfigsItemSelected(long index)
    {
        // Load the config from the json
    }

    private void OnDeleteConfigPressed()
    {
        // Removes currently selected entry from model_configs.json
    }

    private void OnAddNewConfigPressed()
    {
        // Add a new entry to model_configs.json with all the current variables
    }

    // If a config is loaded, every change to a variable should automatically save the whole config back to json



    private void OnClearEmbedderPathPressed()
    {
        embedderModelPath = null;
    }

    private void OnClearClipPathPressed()
    {
        clipModelPath = null;
    }

    private void OnClearChatPathPressed()
    {
        chatModelPath = null;
    }

    private void InitializeParameters()
    {
        // Initialize chat labels
        chatGpuLayerCount = (int)chatGpuLayerCountHSlider.Value;
        chatGpuLayerCountLabel.Text = chatGpuLayerCount.ToString();

        chatContextSizeSliderValue = chatContextSizeHSlider.Value;
        chatContextSize = calculateContextSize(chatContextSizeSliderValue);
        chatContextSizeLabel.Text = chatContextSize.ToString();

        // Initialize embedder labels
        embedderGpuLayerCount = (int)embedderGpuLayerCountHSlider.Value;
        embedderGpuLayerCountLabel.Text = embedderGpuLayerCount.ToString();

        embedderContextSizeSliderValue = embedderContextSizeHSlider.Value;
        embedderContextSize = calculateContextSize(embedderContextSizeSliderValue);
        embedderContextSizeLabel.Text = embedderContextSize.ToString();
    }

    private void OnConfigNameTextChanged(string newText)
    {
        configName = newText;
    }

    private void OnChatGpuLayerCountHSliderValueChanged(double value)
    {
        chatGpuLayerCount = (int)value;
        chatGpuLayerCountLabel.Text = chatGpuLayerCount.ToString();
    }

    private void OnSelectChatPathPressed()
    {
        selectChatPathFileDialog.PopupCentered();
    }

    private void OnChatPathSelected(string path)
    {
        chatModelPath = path;
    }

    private void OnEmbedderGpuLayerCountHSliderValueChanged(double value)
    {
        embedderGpuLayerCount = (int)value;
        embedderGpuLayerCountLabel.Text = embedderGpuLayerCount.ToString();
    }

    private void OnClipPathSelected(string path)
    {
        clipModelPath = path;
    }

    private void OnEmbedderPathSelected(string path)
    {
        embedderModelPath = path;
    }

    private void OnSelectEmbedderPathPressed()
    {
        selectEmbedderPathFileDialog.PopupCentered();
    }

    private uint calculateContextSize(double value)
    {
        return (uint)Math.Pow(2, value) * 1000;
    }

    private void OnChatContextSizeHSliderValueChanged(double value)
    {
        chatContextSize = calculateContextSize(value);
        chatContextSizeLabel.Text = chatContextSize.ToString();
    }

    private void OnEmbedderContextSizeHSliderValueChanged(double value)
    {
        embedderContextSize = calculateContextSize(value);
        embedderContextSizeLabel.Text = embedderContextSize.ToString();
    }

    private void OnSelectClipPathPressed()
    {
        selectClipPathFileDialog.PopupCentered();
    }



    public override void _ExitTree()
    {
        
    }
}

public class ModelConfigsParams
{
    public string ChatModelPath { get; set; }
    public string ClipModelPath { get; set; }
    public string EmbedderModelPath { get; set; }
    public int ChatGpuLayerCount { get; set; }
    public int EmbedderGpuLayerCount { get; set; }
    public uint ChatContextSize { get; set; }
    public uint EmbedderContextSize { get; set; }
    public uint ChatSeed { get; set; }
    public uint EmbedderSeed { get; set;}
}