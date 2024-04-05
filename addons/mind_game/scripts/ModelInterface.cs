#if TOOLS
using Godot;
using LLama.Common;
using LLama;
using System;
using static System.Collections.Specialized.BitVector32;

[Tool]
public partial class ModelInterface : Control, IDisposable
{
    public delegate void ContextEventHandler(LLamaContext context);
    public event ContextEventHandler ContextAvailable;
    public delegate void EmbedderEventHandler(LLamaEmbedder embedder);
    public event EmbedderEventHandler EmbedderAvailable;

    // UI elements
    private Button loadModelButton, unloadModelButton;
    private Label gpuLayerCountLabel;
    private FileDialog loadModelFileDialog;
    private HSlider gpuLayerCountHSlider;

    // Model params
    private int gpuLayerCount;
    
    // Model vars
    private LLamaWeights weights;
    private LLamaContext context;
    private LLamaEmbedder embedder;

    public string test;

    public override void _EnterTree()
    {
        loadModelButton = GetNode<Button>("%LoadModelButton");
        unloadModelButton = GetNode<Button>("%UnloadModelButton");
        gpuLayerCountLabel = GetNode<Label>("%GpuLayerCountLabel");
        gpuLayerCountHSlider = GetNode<HSlider>("%GpuLayerCountHSlider");
        loadModelFileDialog = GetNode<FileDialog>("%LoadModelFileDialog");

        loadModelButton.Pressed += OnLoadModelButtonPressed;
        unloadModelButton.Pressed += OnUnloadModelButtonPressed;
        loadModelFileDialog.FileSelected += OnModelSelected;
        gpuLayerCountHSlider.ValueChanged += OnGpuLayerCountHSliderValueChanged;

        gpuLayerCount = (int)gpuLayerCountHSlider.Value;
        gpuLayerCountLabel.Text = gpuLayerCount.ToString();
    }

    private void OnGpuLayerCountHSliderValueChanged(double value)
    {
        gpuLayerCount = (int)gpuLayerCountHSlider.Value;
        gpuLayerCountLabel.Text = gpuLayerCount.ToString();
    }

    private void OnLoadModelButtonPressed()
    {
        loadModelFileDialog.PopupCentered();
    }

    private void OnModelSelected(string modelPath)
    {
        LoadModel(modelPath);
    }

    public void LoadModel(string modelPath)
    {
        if (weights != null)
        {
            UnloadModel();
        }
        
        
        var parameters = new ModelParams(modelPath)
        {
            ContextSize = 4096,
            Seed = 0,
            GpuLayerCount = gpuLayerCount,
            EmbeddingMode = true
        };

        weights = LLamaWeights.LoadFromFile(parameters);
        context = weights.CreateContext(parameters);
        embedder = new LLamaEmbedder(weights, parameters);
        
        ContextAvailable?.Invoke(context);
        EmbedderAvailable?.Invoke(embedder);
    }

    public void UnloadModel()
    {
        weights.Dispose();
        context.Dispose();
        embedder.Dispose();
    }

    private void OnUnloadModelButtonPressed()
    {
        UnloadModel();
    }

    public LLamaEmbedder GetLLamaEmbedder()
    {
        return embedder;
    }

    public override void _Ready()
    {
        
    }

    public override void _ExitTree()
    {
        loadModelButton.Pressed -= OnLoadModelButtonPressed;
        unloadModelButton.Pressed -= OnUnloadModelButtonPressed;
        loadModelFileDialog.FileSelected -= OnModelSelected;
        gpuLayerCountHSlider.ValueChanged -= OnGpuLayerCountHSliderValueChanged;
    }
}
#endif