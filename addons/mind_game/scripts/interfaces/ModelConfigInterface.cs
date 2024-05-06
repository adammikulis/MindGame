using Godot;
using LLama.Common;
using LLama;
using System;
using static System.Collections.Specialized.BitVector32;

[Tool]
public partial class ModelConfigInterface : Control
{
 

    // UI elements
    private Button selectChatPathButton, clearChatPathButton, selectEmbedderPathButton, clearEmbedderPathButton, selectClipPathButton, clearClipPathButton;
    private Label chatContextSizeLabel, chatGpuLayerCountLabel, embedderContextSizeLabel, embedderGpuLayerCountLabel;
    private FileDialog selectChatPathFileDialog, selectClipPathFileDialog, selectEmbedderPathFileDialog;
    private HSlider chatContextSizeHSlider, chatGpuLayerCountHSlider, embedderContextSizeHSlider, embedderGpuLayerCountHSlider;

    // Model params
    private int chatGpuLayerCount, embedderGpuLayerCount;
    private double chatContextSizeSliderValue, embedderContextSizeSliderValue;
    private uint chatContextSize, embedderContextSize;
    private string chatModelPath, clipModelPath, embedderModelPath;

    public override void _EnterTree()
    {
        
    }

    public override void _Ready()
    {
        // Chat model vars
        chatContextSizeHSlider = GetNode<HSlider>("%ChatContextSizeHSlider");
        chatContextSizeLabel = GetNode<Label>("%ChatContextSizeLabel");

        chatGpuLayerCountLabel = GetNode<Label>("%ChatModelGpuLayerCountLabel");
        chatGpuLayerCountHSlider = GetNode<HSlider>("%ChatModelGpuLayerCountHSlider");


        selectChatPathButton = GetNode<Button>("%SelectChatPathButton");
        clearChatPathButton = GetNode<Button>("%ClearChatPathButton");
        selectChatPathFileDialog = GetNode<FileDialog>("%SelectChatPathFileDialog");

        // Clip model vars
        selectClipPathButton = GetNode<Button>("%SelectClipPathButton");
        clearClipPathButton = GetNode<Button>("%ClearClipPathButton");
        selectClipPathFileDialog = GetNode<FileDialog>("%SelectClipPathFileDialog");


        // Embedder model vars
        embedderContextSizeHSlider = GetNode<HSlider>("%EmbedderContextHSlider");
        embedderContextSizeLabel = GetNode<Label>("%EmbedderContextSizeLabel");

        embedderGpuLayerCountHSlider = GetNode<HSlider>("%EmbedderGpuLayerCountHSlider");
        embedderGpuLayerCountLabel = GetNode<Label>("%EmbedderGpuLayerCountLabel");


        selectEmbedderPathButton = GetNode<Button>("%SelectEmbedderPathButton");
        clearEmbedderPathButton = GetNode<Button>("%ClearEmbedderPathButton");
        selectEmbedderPathFileDialog = GetNode<FileDialog>("%SelectEmbedderPathFileDialog");
        
        
        InitializeParameters();
        SetupSignals();
        

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

    private void SetupSignals()
    {
        // Chat signals
        chatContextSizeHSlider.ValueChanged += OnChatContextSizeHSliderValueChanged;
        chatGpuLayerCountHSlider.ValueChanged += OnChatGpuLayerCountHSliderValueChanged;

        selectChatPathButton.Pressed += OnSelectChatPathButtonPressed;
        selectChatPathFileDialog.FileSelected += OnChatPathSelected;


        // Embedder signals
        embedderContextSizeHSlider.ValueChanged += OnEmbedderContextSizeHSliderValueChanged;
        embedderGpuLayerCountHSlider.ValueChanged += OnEmbedderGpuLayerCountHSliderValueChanged;

        selectEmbedderPathButton.Pressed += OnSelectEmbedderPathPressed;
        selectEmbedderPathFileDialog.FileSelected += OnEmbedderPathSelected;

        // Clip signals
        selectClipPathButton.Pressed += OnSelectClipPathButtonPressed;
        selectClipPathFileDialog.FileSelected += OnClipPathSelected;
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
        // This sets a minimum context size for the slider
        if (value == 0) return 0;
        else
        {
            return (uint)Math.Pow(2, value) * 1000;
        }
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

    private void OnSelectClipPathButtonPressed()
    {
        selectClipPathFileDialog.PopupCentered();
    }

    private void OnChatGpuLayerCountHSliderValueChanged(double value)
    {
        chatGpuLayerCount = (int)value;
        chatGpuLayerCountLabel.Text = chatGpuLayerCount.ToString();
    }

    private void OnSelectChatPathButtonPressed()
    {
        selectChatPathFileDialog.PopupCentered();
    }

    private void OnChatPathSelected(string path)
    {
        chatModelPath = path;
    }

    public override void _ExitTree()
    {
        
    }
}