#if TOOLS
using Godot;
using LLama.Common;
using LLama;
using System;
using static System.Collections.Specialized.BitVector32;

[Tool]
public partial class ModelInterface : Control, IDisposable
{
    
    public delegate void EmbedderEventHandler(LLamaEmbedder embedder);
    public event EmbedderEventHandler EmbedderAvailable;
    public delegate void ExecutorEventHandler(InteractiveExecutor executor);
    public event ExecutorEventHandler ExecutorAvailable;

    // UI elements
    private Button selectChatModelButton, loadChatModelButton, unloadChatModelButton, selectEmbeddingModelButton, loadEmbeddingModelButton, unloadEmbeddingModelButton, selectClipModelButton;
    private CheckBox useClipModelCheckBox;
    private Label chatContextSizeLabel, modelGpuLayerCountLabel;
    private FileDialog selectChatModelFileDialog, selectClipModelFileDialog, selectEmbeddingModelFileDialog;
    private HSlider chatContextSizeHSlider, modelGpuLayerCountHSlider;

    // Chat model params
    private int chatGpuLayerCount;
    private double chatContextSizeSliderValue;
    private uint chatContextSize;

    // Chat model vars
    private LLamaWeights chatWeights = null;
    private LLamaContext chatContext = null;
    private string chatModelPath = null;

    // Clip (LLaVa) model vars
    private LLavaWeights clipWeights = null;
    private string clipModelPath = null;
    private bool usingClipModel = false;

    // Embedder model vars
    private LLamaWeights embedderWeights = null;
    private LLamaEmbedder embedder = null;

    // Executor
    private InteractiveExecutor executor = null;


    public override void _EnterTree()
    {
        
    }

    public override void _Ready()
    {
        // Chat model vars
        chatContextSizeHSlider = GetNode<HSlider>("%ChatContextSizeHSlider");
        chatContextSizeLabel = GetNode<Label>("%ChatContextSizeLabel");

        modelGpuLayerCountLabel = GetNode<Label>("%ModelGpuLayerCountLabel");
        modelGpuLayerCountHSlider = GetNode<HSlider>("%ModelGpuLayerCountHSlider");

        selectChatModelButton = GetNode<Button>("%SelectChatModelButton");
        loadChatModelButton = GetNode<Button>("%LoadChatModelButton");
        unloadChatModelButton = GetNode<Button>("%UnloadChatModelButton");
        
        selectChatModelFileDialog = GetNode<FileDialog>("%SelectChatModelFileDialog");

        // Clip model vars
        selectClipModelButton = GetNode<Button>("%SelectClipModelButton");
        useClipModelCheckBox = GetNode<CheckBox>("%UseClipModelCheckBox");
        selectClipModelFileDialog = GetNode<FileDialog>("%SelectClipModelFileDialog");


        // Embedder model vars
        selectEmbeddingModelButton = GetNode<Button>("%SelectEmbeddingModelButton");
        loadEmbeddingModelButton = GetNode<Button>("%LoadEmbeddingModelButton");
        unloadEmbeddingModelButton = GetNode<Button>("%UnloadEmbeddingModelButton");
        selectEmbeddingModelFileDialog = GetNode<FileDialog>("%SelectEmbeddingModelFileDialog");


        // Chat signals
        chatContextSizeHSlider.ValueChanged += OnChatContextSizeHSliderValueChanged;
        modelGpuLayerCountHSlider.ValueChanged += OnModelGpuLayerCountHSliderValueChanged;

        selectChatModelButton.Pressed += OnSelectChatModelButtonPressed;
        unloadChatModelButton.Pressed += OnUnloadChatModelButtonPressed;
        loadChatModelButton.Pressed += OnLoadChatModelButtonPressed;

        selectChatModelFileDialog.FileSelected += OnChatModelSelected;
        

        // Embedder signals


        // Clip signals
        selectClipModelButton.Pressed += OnSelectClipModelButtonPressed;
        selectClipModelFileDialog.FileSelected += OnClipModelSelected;



        // Param value initialization
        chatGpuLayerCount = (int)modelGpuLayerCountHSlider.Value;
        modelGpuLayerCountLabel.Text = chatGpuLayerCount.ToString();

        chatContextSizeSliderValue = chatContextSizeHSlider.Value;
        chatContextSize = getContextSize(chatContextSizeSliderValue);
        chatContextSizeLabel.Text = chatContextSize.ToString();

    }

    private uint getContextSize(double value)
    {
        double min_exponent = 5;
        if (value == 0) return 0;
        else
        {
            return (uint)Math.Pow(2, value + min_exponent);
        }
    }

    private void OnChatContextSizeHSliderValueChanged(double value)
    {
        chatContextSizeSliderValue = chatContextSizeHSlider.Value;
        chatContextSize = getContextSize(chatContextSizeSliderValue);
        chatContextSizeLabel.Text = chatContextSize.ToString();
    }

    private void OnLoadChatModelButtonPressed()
    {
        LoadChatModel();
    }

    private void OnClipModelSelected(string path)
    {
        useClipModelCheckBox.Disabled = false;
        useClipModelCheckBox.ToggleMode = true;
        clipModelPath = path;
    }

    private void OnSelectClipModelButtonPressed()
    {
        selectClipModelFileDialog.PopupCentered();
    }

    private void OnModelGpuLayerCountHSliderValueChanged(double value)
    {
        chatGpuLayerCount = (int)modelGpuLayerCountHSlider.Value;
        modelGpuLayerCountLabel.Text = chatGpuLayerCount.ToString();
    }

    private void OnSelectChatModelButtonPressed()
    {
        selectChatModelFileDialog.PopupCentered();
    }

    private void OnChatModelSelected(string modelPath)
    {
        loadChatModelButton.Disabled = false;
        chatModelPath = modelPath;
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

    public void LoadChatModel()
    {
        if (chatWeights != null)
        {
            UnloadChatModel();
        }

        var parameters = new ModelParams(chatModelPath)
        {
            ContextSize = chatContextSize,
            Seed = 0,
            GpuLayerCount = chatGpuLayerCount,
            EmbeddingMode = false // Change this if you want to use the chat model instead of BERT for embeddings (not recommended)
        };

        chatWeights = LLamaWeights.LoadFromFile(parameters);
        chatContext = chatWeights.CreateContext(parameters);
        bool executorInitialized = InitializeExecutor();
        if (executorInitialized) { unloadChatModelButton.Disabled = false; }

        // Uncomment to use model as embedder instead of BERT
        // embedder = new LLamaEmbedder(chatWeights, parameters);
        // EmbedderAvailable?.Invoke(embedder);

    }

    private bool InitializeExecutor()
    {
        if (usingClipModel && clipModelPath != null && chatModelPath != null)
        {
            clipWeights = LLavaWeights.LoadFromFile(clipModelPath);
            executor = new InteractiveExecutor(chatContext, clipWeights);
            ExecutorAvailable?.Invoke(executor);
            return true;
        }
        else if (chatModelPath != null)
        {
            executor = new InteractiveExecutor(chatContext);
            ExecutorAvailable?.Invoke(executor);
            return true;
        }
        else { return false; }
    }

    public void UnloadChatModel()
    {
        if (chatWeights != null) { chatWeights.Dispose(); }

        if (chatContext != null) { chatContext.Dispose(); }
        if (embedder != null) { embedder.Dispose(); }
        if (executor != null) { executor = null; }
        unloadChatModelButton.Disabled = true;
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
        selectChatModelButton.Pressed -= OnSelectChatModelButtonPressed;
        unloadChatModelButton.Pressed -= OnUnloadChatModelButtonPressed;
        selectChatModelFileDialog.FileSelected -= OnChatModelSelected;
        modelGpuLayerCountHSlider.ValueChanged -= OnModelGpuLayerCountHSliderValueChanged;
    }
}
#endif