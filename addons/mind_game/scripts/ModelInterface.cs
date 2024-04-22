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
    private Button chooseChatModelButton, loadChatModelButton, unloadChatModelButton, loadEmbeddingModelButton, unloadEmbeddingModelButton, chooseClipModelButton, unloadClipModelButton;
    private CheckBox useClipModelCheckBox;
    private Label modelGpuLayerCountLabel;
    private FileDialog chooseChatModelFileDialog, chooseClipModelFileDialog, loadEmbeddingModelFileDialog;
    private HSlider modelGpuLayerCountHSlider;

    // Chat model params
    private int chatGpuLayerCount;
    private uint chatContextSize;

    // Chat model vars
    private LLamaWeights chatWeights;
    private LLamaContext chatContext;

    // Clip (LLaVa) model vars
    private LLavaWeights clipWeights;

    // Embedder model vars
    private LLamaWeights embedderWeights;
    private LLamaEmbedder embedder;


    public override void _EnterTree()
    {
        
    }

    public override void _Ready()
    {
        // Chat model vars
        chooseChatModelButton = GetNode<Button>("%LoadChatModelButton");
        loadChatModelButton = GetNode<Button>("%LoadChatModelButton");
        unloadChatModelButton = GetNode<Button>("%UnloadChatModelButton");
        modelGpuLayerCountLabel = GetNode<Label>("%GpuLayerCountLabel");
        modelGpuLayerCountHSlider = GetNode<HSlider>("%GpuLayerCountHSlider");
        chooseChatModelFileDialog = GetNode<FileDialog>("%LoadChatModelFileDialog");

        // Clip model vars
        chooseClipModelButton = GetNode<Button>("%ChooseClipModelButton");
        useClipModelCheckBox = GetNode<CheckBox>("%UseClipModelCheckBox");
        chooseClipModelFileDialog = GetNode<FileDialog>("%LoadClipModelFileDialog");


        // Embedder model vars
        loadEmbeddingModelButton = GetNode<Button>("%LoadEmbeddingModelButton");
        unloadEmbeddingModelButton = GetNode<Button>("%UnloadEmbeddingModelButton");
        loadEmbeddingModelFileDialog = GetNode<FileDialog>("%LoadEmbeddingModelFileDialog");


        // Chat signals
        chooseChatModelButton.Pressed += OnChooseChatModelButtonPressed;
        unloadChatModelButton.Pressed += OnUnloadChatModelButtonPressed;
        chooseChatModelFileDialog.FileSelected += OnChatModelSelected;
        modelGpuLayerCountHSlider.ValueChanged += OnModelGpuLayerCountHSliderValueChanged;

        // Embedder signals


        // Clip signals
        chooseClipModelButton.Pressed += OnChooseClipModelButtonPressed;
        chooseClipModelFileDialog.FileSelected += OnClipModelSelected;


        chatGpuLayerCount = (int)modelGpuLayerCountHSlider.Value;
        modelGpuLayerCountLabel.Text = chatGpuLayerCount.ToString();
    }

    private void OnClipModelSelected(string path)
    {

        useClipModelCheckBox.Disabled = false;
        useClipModelCheckBox.ToggleMode = true;
    }

    private void LoadClipModel(string path)
    {
        throw new NotImplementedException();
    }

    private void OnUnloadClipModelButtonPressed()
    {
        throw new NotImplementedException();
    }

    private void OnChooseClipModelButtonPressed()
    {
        chooseClipModelFileDialog.PopupCentered();
    }

    private void OnModelGpuLayerCountHSliderValueChanged(double value)
    {
        chatGpuLayerCount = (int)modelGpuLayerCountHSlider.Value;
        modelGpuLayerCountLabel.Text = chatGpuLayerCount.ToString();
    }

    private void OnChooseChatModelButtonPressed()
    {
        chooseChatModelFileDialog.PopupCentered();
    }

    private void OnChatModelSelected(string modelPath)
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
        chooseChatModelButton.Pressed -= OnChooseChatModelButtonPressed;
        unloadChatModelButton.Pressed -= OnUnloadChatModelButtonPressed;
        chooseChatModelFileDialog.FileSelected -= OnChatModelSelected;
        modelGpuLayerCountHSlider.ValueChanged -= OnModelGpuLayerCountHSliderValueChanged;
    }
}
#endif