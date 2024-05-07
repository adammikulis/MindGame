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
    private LineEdit configNameLineEdit, chatRandomSeedLineEdit, embedderRandomSeedLineEdit;
    private ItemList savedConfigsItemList;

    // Model params
    private string configName;
    private int chatGpuLayerCount, embedderGpuLayerCount;
    private double chatContextSizeSliderValue, embedderContextSizeSliderValue;
    private uint chatContextSize, embedderContextSize, chatRandomSeed, embedderRandomSeed;
    private string chatModelPath, clipModelPath, embedderModelPath;

    public override void _EnterTree()
    {
        
    }

    public override void _Ready()
    {
        InitializeDefaultValues();
        InitializeNodeRefs();
        InitializeSignals();
        InitializeUiElements();
    }

    private void InitializeDefaultValues()
    {
        configName = "new_config";
        
        chatContextSize = 4000;
        chatGpuLayerCount = 33;
        chatRandomSeed = 0;
        chatModelPath = "";

        embedderContextSize = 4000;
        embedderGpuLayerCount = 33;
        embedderRandomSeed = 0;
        embedderModelPath = "";

        clipModelPath = "";
        
    }

    private void InitializeNodeRefs()
    {

        // Manage configs nodes
        configNameLineEdit = GetNode<LineEdit>("%ConfigNameLineEdit");
        savedConfigsItemList = GetNode<ItemList>("%SavedConfigsItemList");
        addNewConfigButton = GetNode<Button>("%AddNewConfigButton");
        deleteConfigButton = GetNode<Button>("%DeleteConfigButton");

        // Chat param nodes
        chatContextSizeHSlider = GetNode<HSlider>("%ChatContextSizeHSlider");
        chatContextSizeLabel = GetNode<Label>("%ChatContextSizeLabel");

        chatGpuLayerCountHSlider = GetNode<HSlider>("%ChatModelGpuLayerCountHSlider");
        chatGpuLayerCountLabel = GetNode<Label>("%ChatModelGpuLayerCountLabel");

        chatRandomSeedLineEdit = GetNode<LineEdit>("%ChatRandomSeedLineEdit");

        // Chat path nodes
        selectChatPathButton = GetNode<Button>("%SelectChatPathButton");
        clearChatPathButton = GetNode<Button>("%ClearChatPathButton");
        selectChatPathFileDialog = GetNode<FileDialog>("%SelectChatPathFileDialog");

        // Embedder param nodes
        embedderContextSizeHSlider = GetNode<HSlider>("%EmbedderContextSizeHSlider");
        embedderContextSizeLabel = GetNode<Label>("%EmbedderContextSizeLabel");

        embedderGpuLayerCountHSlider = GetNode<HSlider>("%EmbedderGpuLayerCountHSlider");
        embedderGpuLayerCountLabel = GetNode<Label>("%EmbedderGpuLayerCountLabel");

        embedderRandomSeedLineEdit = GetNode<LineEdit>("%EmbedderRandomSeedLineEdit");

        // Embedder path nodes
        selectEmbedderPathButton = GetNode<Button>("%SelectEmbedderPathButton");
        clearEmbedderPathButton = GetNode<Button>("%ClearEmbedderPathButton");
        selectEmbedderPathFileDialog = GetNode<FileDialog>("%SelectEmbedderPathFileDialog");

        // Clip path nodes
        selectClipPathButton = GetNode<Button>("%SelectClipPathButton");
        clearClipPathButton = GetNode<Button>("%ClearClipPathButton");
        selectClipPathFileDialog = GetNode<FileDialog>("%SelectClipPathFileDialog");

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

    private void InitializeUiElements()
    {
        // Initialize chat sliders and labels
        chatGpuLayerCountHSlider.Value = (double)chatGpuLayerCount;
        chatGpuLayerCountLabel.Text = chatGpuLayerCount.ToString();

        chatContextSizeHSlider.Value = calculateLogContextSize(chatContextSize);
        chatContextSizeLabel.Text = chatContextSize.ToString();

        chatRandomSeedLineEdit.Text = chatRandomSeed.ToString();

        // Initialize embedder sliders and labels
        embedderGpuLayerCountHSlider.Value = (double)embedderGpuLayerCount;
        embedderGpuLayerCountLabel.Text = embedderGpuLayerCount.ToString();

        embedderContextSizeHSlider.Value = calculateLogContextSize(embedderContextSize);
        embedderContextSizeLabel.Text = embedderContextSize.ToString();

        embedderRandomSeedLineEdit.Text = embedderRandomSeed.ToString();
    }





    private void OnSavedConfigsItemSelected(long index)
    {
        // Load the config from the json
    }

    private void OnDeleteConfigPressed()
    {
        // Remove currently selected entry from model_configs.json
    }

    private void OnAddNewConfigPressed()
    {
        // Add a new entry to model_configs.json with all the current variables
    }

    // If a config is loaded, every change to a variable should automatically save the whole config back to json



    private void OnClearEmbedderPathPressed()
    {
        embedderModelPath = "";
    }

    private void OnClearClipPathPressed()
    {
        clipModelPath = "";
    }

    private void OnClearChatPathPressed()
    {
        chatModelPath = "";
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

    private uint calculateExpContextSize(double value)
    {
        return (uint)Math.Pow(2, value) * 1000;
    }

    private double calculateLogContextSize(uint value)
    {
        return (double)Math.Log2(value / 1000);
    }

    private void OnChatContextSizeHSliderValueChanged(double value)
    {
        chatContextSize = calculateExpContextSize(value);
        chatContextSizeLabel.Text = chatContextSize.ToString();
    }

    private void OnEmbedderContextSizeHSliderValueChanged(double value)
    {
        embedderContextSize = calculateExpContextSize(value);
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