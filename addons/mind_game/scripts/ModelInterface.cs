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
    private Label modelGpuLayerCountLabel;
    private FileDialog selectChatModelFileDialog, selectClipModelFileDialog, loadEmbeddingModelFileDialog;
    private HSlider modelGpuLayerCountHSlider;

    // Chat model params
    private int chatGpuLayerCount;
    private uint chatContextSize;

    // Chat model vars
    private LLamaWeights chatWeights;
    private LLamaContext chatContext;
    private string chatModelPath = null;

    // Clip (LLaVa) model vars
    private LLavaWeights clipWeights;
    private bool useClipModel = false;
    private string clipModelPath = null;

    // Embedder model vars
    private LLamaWeights embedderWeights;
    private LLamaEmbedder embedder;

    // Executor
    private InteractiveExecutor executor;


    public override void _EnterTree()
    {
        
    }

    public override void _Ready()
    {
        // Chat model vars
        selectChatModelButton = GetNode<Button>("%SelectChatModelButton");
        loadChatModelButton = GetNode<Button>("%LoadChatModelButton");
        unloadChatModelButton = GetNode<Button>("%UnloadChatModelButton");
        modelGpuLayerCountLabel = GetNode<Label>("%ModelGpuLayerCountLabel");
        modelGpuLayerCountHSlider = GetNode<HSlider>("%ModelGpuLayerCountHSlider");
        selectChatModelFileDialog = GetNode<FileDialog>("%SelectChatModelFileDialog");

        // Clip model vars
        selectClipModelButton = GetNode<Button>("%SelectClipModelButton");
        useClipModelCheckBox = GetNode<CheckBox>("%UseClipModelCheckBox");
        selectClipModelFileDialog = GetNode<FileDialog>("%SelectClipModelFileDialog");


        // Embedder model vars
        selectEmbeddingModelButton = GetNode<Button>("%SelectEmbeddingModelButton");
        loadEmbeddingModelButton = GetNode<Button>("%LoadEmbeddingModelButton");
        unloadEmbeddingModelButton = GetNode<Button>("%UnloadEmbeddingModelButton");
        loadEmbeddingModelFileDialog = GetNode<FileDialog>("%LoadEmbeddingModelFileDialog");


        // Chat signals
        selectChatModelButton.Pressed += OnSelectChatModelButtonPressed;
        unloadChatModelButton.Pressed += OnUnloadChatModelButtonPressed;
        loadChatModelButton.Pressed += OnLoadChatModelButtonPressed;
        selectChatModelFileDialog.FileSelected += OnChatModelSelected;
        modelGpuLayerCountHSlider.ValueChanged += OnModelGpuLayerCountHSliderValueChanged;

        // Embedder signals


        // Clip signals
        selectClipModelButton.Pressed += OnSelectClipModelButtonPressed;
        selectClipModelFileDialog.FileSelected += OnClipModelSelected;


        chatGpuLayerCount = (int)modelGpuLayerCountHSlider.Value;
        modelGpuLayerCountLabel.Text = chatGpuLayerCount.ToString();
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
            EmbeddingMode = false // Change this if you want to use the chat model instead of BERT for embeddings
        };

        chatWeights = LLamaWeights.LoadFromFile(parameters);
        chatContext = chatWeights.CreateContext(parameters);

        if (useClipModel && clipModelPath != null)
        {
            clipWeights = LLavaWeights.LoadFromFile(clipModelPath);
            executor = new InteractiveExecutor(chatContext, clipWeights);
        }
        else
        {
            executor = new InteractiveExecutor(chatContext);
        }


        ExecutorAvailable?.Invoke(executor);

        // Uncomment to use model as embedder instead of BERT
        // embedder = new LLamaEmbedder(chatWeights, parameters);
        // EmbedderAvailable?.Invoke(embedder);



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
        selectChatModelButton.Pressed -= OnSelectChatModelButtonPressed;
        unloadChatModelButton.Pressed -= OnUnloadChatModelButtonPressed;
        selectChatModelFileDialog.FileSelected -= OnChatModelSelected;
        modelGpuLayerCountHSlider.ValueChanged -= OnModelGpuLayerCountHSliderValueChanged;
    }
}
#endif