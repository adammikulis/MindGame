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
    private Button selectChatModelButton, loadChatModelButton, unloadChatModelButton, selectEmbedderModelButton, loadEmbedderModelButton, unloadEmbedderModelButton, selectClipModelButton;
    private CheckBox useClipModelCheckBox, useChatModelAsEmbedder;
    private Label chatContextSizeLabel, chatModelGpuLayerCountLabel;
    private FileDialog selectChatModelFileDialog, selectClipModelFileDialog, selectEmbedderModelFileDialog;
    private HSlider chatContextSizeHSlider, chatModelGpuLayerCountHSlider;

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
    private bool useClipModel = false;

    // Embedder model vars
    private LLamaWeights embedderWeights = null;
    private LLamaEmbedder embedder = null;
    private string embedderModelPath = null;

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

        chatModelGpuLayerCountLabel = GetNode<Label>("%ChatModelGpuLayerCountLabel");
        chatModelGpuLayerCountHSlider = GetNode<HSlider>("%ChatModelGpuLayerCountHSlider");

        selectChatModelButton = GetNode<Button>("%SelectChatModelButton");
        loadChatModelButton = GetNode<Button>("%LoadChatModelButton");
        unloadChatModelButton = GetNode<Button>("%UnloadChatModelButton");
        
        selectChatModelFileDialog = GetNode<FileDialog>("%SelectChatModelFileDialog");

        // Clip model vars
        selectClipModelButton = GetNode<Button>("%SelectClipModelButton");
        useClipModelCheckBox = GetNode<CheckBox>("%UseClipModelCheckBox");
        selectClipModelFileDialog = GetNode<FileDialog>("%SelectClipModelFileDialog");


        // Embedder model vars
        selectEmbedderModelButton = GetNode<Button>("%SelectEmbedderModelButton");
        loadEmbedderModelButton = GetNode<Button>("%LoadEmbedderModelButton");
        unloadEmbedderModelButton = GetNode<Button>("%UnloadEmbedderModelButton");
        selectEmbedderModelFileDialog = GetNode<FileDialog>("%SelectEmbedderModelFileDialog");

        InitializeParameters();
        SetupSignals();
        

    }

    private void InitializeParameters()
    {
        chatGpuLayerCount = (int)chatModelGpuLayerCountHSlider.Value;
        chatModelGpuLayerCountLabel.Text = chatGpuLayerCount.ToString();

        chatContextSizeSliderValue = chatContextSizeHSlider.Value;
        chatContextSize = getContextSize(chatContextSizeSliderValue);
        chatContextSizeLabel.Text = chatContextSize.ToString();
    }

    private void SetupSignals()
    {
        // Chat signals
        chatContextSizeHSlider.ValueChanged += OnChatContextSizeHSliderValueChanged;
        chatModelGpuLayerCountHSlider.ValueChanged += OnModelGpuLayerCountHSliderValueChanged;

        selectChatModelButton.Pressed += OnSelectChatModelButtonPressed;
        loadChatModelButton.Pressed += OnLoadChatModelButtonPressed;
        unloadChatModelButton.Pressed += OnUnloadChatModelButtonPressed;

        selectChatModelFileDialog.FileSelected += OnChatModelSelected;


        // Embedder signals
        selectEmbedderModelButton.Pressed += OnSelectEmbedderModelPressed;
        loadEmbedderModelButton.Pressed += OnLoadEmbedderModelButtonPressed;
        unloadEmbedderModelButton.Pressed += OnUnloadEmbedderModelButtonPressed;

        selectEmbedderModelFileDialog.FileSelected += OnEmbedderModelSelected;

        // Clip signals
        selectClipModelButton.Pressed += OnSelectClipModelButtonPressed;
        useClipModelCheckBox.Toggled += OnUseClipModelCheckBoxToggled;
        selectClipModelFileDialog.FileSelected += OnClipModelSelected;
    }

    private void OnUseClipModelCheckBoxToggled(bool toggledOn)
    {
        useClipModel = toggledOn;
    }

    private void OnEmbedderModelSelected(string path)
    {
        loadEmbedderModelButton.Disabled = false;
        chatModelPath = path;
    }

    private void OnLoadEmbedderModelButtonPressed()
    {
        LoadEmbedderModel();
    }



    private void OnUnloadEmbedderModelButtonPressed()
    {
        UnloadEmbedderModel();
    }

    private void OnSelectEmbedderModelPressed()
    {
        selectEmbedderModelFileDialog.PopupCentered();
    }


    public void LoadEmbedderModel()
    {

        if (embedderWeights != null)
        {
            UnloadEmbedderModel();
        }

        var parameters = new ModelParams(embedderModelPath)
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

    private uint getContextSize(double value)
    {
        // This sets a minimum context size for the slider
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
        chatGpuLayerCount = (int)chatModelGpuLayerCountHSlider.Value;
        chatModelGpuLayerCountLabel.Text = chatGpuLayerCount.ToString();
    }

    private void OnSelectChatModelButtonPressed()
    {
        selectChatModelFileDialog.PopupCentered();
    }

    private void OnChatModelSelected(string path)
    {
        loadChatModelButton.Disabled = false;
        chatModelPath = path;
    }



    public void LoadChatModel()
    {
        if (string.IsNullOrEmpty(chatModelPath))
        {
            GD.Print("Chat model path is not set.");
            return;
        }

        if (chatWeights != null)
        {
            UnloadChatModel();
        }

        var parameters = new ModelParams(chatModelPath)
        {
            ContextSize = chatContextSize,
            Seed = 0,
            GpuLayerCount = chatGpuLayerCount,
            EmbeddingMode = false
        };

        try
        {
            chatWeights = LLamaWeights.LoadFromFile(parameters);
            chatContext = chatWeights.CreateContext(parameters);
            bool executorInitialized = InitializeExecutor();
            if (executorInitialized)
            {
                unloadChatModelButton.Disabled = false;
                useChatModelAsEmbedder.Disabled = false;
            }
        }
        catch (Exception ex)
        {
            GD.Print("Failed to load chat model: ", ex.Message);
        }
    }


    private bool InitializeExecutor()
    {
        if (useClipModel && clipModelPath != null && chatModelPath != null)
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


    public override void _ExitTree()
    {
        //selectChatModelButton.Pressed -= OnSelectChatModelButtonPressed;
        //unloadChatModelButton.Pressed -= OnUnloadChatModelButtonPressed;
        //selectChatModelFileDialog.FileSelected -= OnChatModelSelected;
        //chatModelGpuLayerCountHSlider.ValueChanged -= OnModelGpuLayerCountHSliderValueChanged;
    }
}