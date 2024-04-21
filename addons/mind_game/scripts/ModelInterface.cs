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
    public event ContextEventHandler ChatContextAvailable;
    public delegate void EmbedderEventHandler(LLamaEmbedder embedder);
    public event EmbedderEventHandler EmbedderAvailable;

    // UI elements
    private Button loadChatModelButton, unloadChatModelButton, loadEmbedderModelButton, unloadEmbedderModelButton;
    private Label modelGpuLayerCountLabel;
    private FileDialog loadChatModelFileDialog;
    private HSlider modelGpuLayerCountHSlider;

    // Chat model params
    private int chatGpuLayerCount;
    private uint chatContextSize;

    // Chat model vars
    private LLamaWeights chatWeights;
    private LLamaContext chatContext;

    // Embedder model vars
    private LLamaWeights embedderWeights;
    private LLamaEmbedder embedder;

    public override void _EnterTree()
    {
        
    }

    public override void _Ready()
    {
        loadChatModelButton = GetNode<Button>("%LoadChatModelButton");
        unloadChatModelButton = GetNode<Button>("%UnloadChatModelButton");
        modelGpuLayerCountLabel = GetNode<Label>("%GpuLayerCountLabel");
        modelGpuLayerCountHSlider = GetNode<HSlider>("%GpuLayerCountHSlider");
        loadChatModelFileDialog = GetNode<FileDialog>("%LoadChatModelFileDialog");

        loadChatModelButton.Pressed += OnLoadChatModelButtonPressed;
        unloadChatModelButton.Pressed += OnUnloadChatModelButtonPressed;
        loadChatModelFileDialog.FileSelected += OnModelSelected;
        modelGpuLayerCountHSlider.ValueChanged += OnModelGpuLayerCountHSliderValueChanged;

        chatGpuLayerCount = (int)modelGpuLayerCountHSlider.Value;
        modelGpuLayerCountLabel.Text = chatGpuLayerCount.ToString();
    }


    private void OnModelGpuLayerCountHSliderValueChanged(double value)
    {
        chatGpuLayerCount = (int)modelGpuLayerCountHSlider.Value;
        modelGpuLayerCountLabel.Text = chatGpuLayerCount.ToString();
    }

    private void OnLoadChatModelButtonPressed()
    {
        loadChatModelFileDialog.PopupCentered();
    }

    private void OnModelSelected(string modelPath)
    {
        LoadChatModel(modelPath);
    }

    public void LoadEmbeddingModel(string modelPath)
    {

        if (embedderWeights != null)
        {
            UnloadEmbedderModel();
        }

        var parameters = new ModelParams(modelPath)
        {
            ContextSize = chatContextSize,
            Seed = 0,
            GpuLayerCount = chatGpuLayerCount,
            EmbeddingMode = true
        };


        embedderWeights = LLamaWeights.LoadFromFile(parameters);
        embedder = new LLamaEmbedder(embedderWeights, parameters);

        EmbedderAvailable?.Invoke(embedder);

    }

    public void UnloadEmbedderModel()
    {

        if (embedderWeights != null) { embedderWeights.Dispose(); }
        if (embedder != null) { embedder.Dispose(); }

    }

    public void LoadChatModel(string modelPath)
    {
        if (chatWeights != null)
        {
            UnloadChatModel();
        }
        
        
        var parameters = new ModelParams(modelPath)
        {
            ContextSize = chatContextSize,
            Seed = 0,
            GpuLayerCount = chatGpuLayerCount,
            EmbeddingMode = false
        };

        chatWeights = LLamaWeights.LoadFromFile(parameters);
        chatContext = chatWeights.CreateContext(parameters);
        embedder = new LLamaEmbedder(chatWeights, parameters);
        
        ChatContextAvailable?.Invoke(chatContext);
        
    }

    public void UnloadChatModel()
    {
        if (chatWeights != null) { chatWeights.Dispose(); }

        if (chatContext != null) { chatContext.Dispose(); }
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
        modelGpuLayerCountHSlider.ValueChanged -= OnModelGpuLayerCountHSliderValueChanged;
    }
}
#endif