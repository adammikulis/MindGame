using Godot;
using LLama.Common;
using LLama;
using System;
using static System.Collections.Specialized.BitVector32;

[Tool]
public partial class ModelInterface : Control, IDisposable
{
    [Signal]
    public delegate void ChatSessionStatusEventHandler(bool isChatSessionActive);
    [Signal]
    public delegate void EmbedderStatusEventHandler(bool isModelEmbedderActive);

    private int gpuLayerCount;
    private Button loadModelButton, unloadModelButton;
    private Label gpuLayerCountLabel;
    private FileDialog loadModelFileDialog;
    private HSlider gpuLayerCountHSlider;

    private LLamaWeights weights;
    private LLamaContext context;
    public LLamaEmbedder embedder;
    private InteractiveExecutor executor;
    private ChatSession chatSession;

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
        executor = new InteractiveExecutor(context);
        chatSession = new ChatSession(executor);

        EmitSignal(nameof(ChatSessionStatus), true);
        EmitSignal(nameof(EmbedderStatus), true);
    }

    public void UnloadModel()
    {
        weights.Dispose();
        context.Dispose();
        embedder.Dispose();

        EmitSignal(nameof(ChatSessionStatus), false);
        EmitSignal(nameof(EmbedderStatus), false);
    }

    private void OnUnloadModelButtonPressed()
    {
        UnloadModel();
    }

    public ChatSession GetChatSession()
    {
        return chatSession;
    }

    public LLamaEmbedder GetLLamaEmbedder()
    {
        return embedder;
    }
}
