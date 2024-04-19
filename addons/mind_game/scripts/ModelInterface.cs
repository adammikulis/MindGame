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
    private Button loadChatModelButton, unloadChatModelButton;
    private Label gpuLayerCountLabel;
    private FileDialog loadChatModelFileDialog;
    private HSlider gpuLayerCountHSlider;

    // Model params
    private int gpuLayerCount;

    // Model vars
    private LLamaWeights weights;
    private LLamaContext context;
    private LLamaEmbedder embedder;

    public override void _EnterTree()
    {
        
    }

    public override void _Ready()
    {
        loadChatModelButton = GetNode<Button>("%LoadChatModelButton");
        unloadChatModelButton = GetNode<Button>("%UnloadChatModelButton");
        gpuLayerCountLabel = GetNode<Label>("%GpuLayerCountLabel");
        gpuLayerCountHSlider = GetNode<HSlider>("%GpuLayerCountHSlider");
        loadChatModelFileDialog = GetNode<FileDialog>("%LoadChatModelFileDialog");

        loadChatModelButton.Pressed += OnLoadChatModelButtonPressed;
        unloadChatModelButton.Pressed += OnUnloadChatModelButtonPressed;
        loadChatModelFileDialog.FileSelected += OnModelSelected;
        gpuLayerCountHSlider.ValueChanged += OnGpuLayerCountHSliderValueChanged;

        gpuLayerCount = (int)gpuLayerCountHSlider.Value;
        gpuLayerCountLabel.Text = gpuLayerCount.ToString();
    }


    private void OnGpuLayerCountHSliderValueChanged(double value)
    {
        gpuLayerCount = (int)gpuLayerCountHSlider.Value;
        gpuLayerCountLabel.Text = gpuLayerCount.ToString();
    }

    private void OnLoadChatModelButtonPressed()
    {
        loadChatModelFileDialog.PopupCentered();
    }

    private void OnModelSelected(string modelPath)
    {
        LoadChatModel(modelPath);
    }

    public void LoadChatModel(string modelPath)
    {
        if (weights != null)
        {
            UnloadChatModel();
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

    public void UnloadChatModel()
    {
        if (weights != null) { weights.Dispose(); }

        if (context != null) { context.Dispose(); }
        if (embedder != null) { embedder.Dispose(); }
    }

    private void OnUnloadChatModelButtonPressed()
    {
        UnloadChatModel();
    }

    public LLamaEmbedder GetLLamaEmbedder()
    {
        return embedder;
    }

    public override void _ExitTree()
    {
        loadChatModelButton.Pressed -= OnLoadChatModelButtonPressed;
        unloadChatModelButton.Pressed -= OnUnloadChatModelButtonPressed;
        loadChatModelFileDialog.FileSelected -= OnModelSelected;
        gpuLayerCountHSlider.ValueChanged -= OnGpuLayerCountHSliderValueChanged;
    }
}
#endif