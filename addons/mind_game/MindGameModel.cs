using Godot;
using LLama;
using LLama.Common;
using System;
using System.Threading.Tasks;

public partial class MindGameModel : Node, IDisposable
{
    [Signal]
    public delegate void ModelOutputEventHandler(string text);

    public LLamaWeights weights;
    public LLamaContext context;
    public LLamaEmbedder embedder;
    public InteractiveExecutor executor;
    public ChatSession session;

    private static MindGameModel _instance;
    public static MindGameModel Instance => _instance;

    public override void _EnterTree()
    {
        if(_instance != null)
        {
            this.QueueFree();
        }
        _instance = this;
    }


    public void LoadModel(string modelPath)
    {
        var parameters = new ModelParams(modelPath)
        {
            ContextSize = 4096,
            Seed = 0,
            GpuLayerCount = 33,
            EmbeddingMode = true
        };

        weights = LLamaWeights.LoadFromFile(parameters);
        context = weights.CreateContext(parameters);
        embedder = new LLamaEmbedder(weights, parameters);
        executor = new InteractiveExecutor(context);
        session = new ChatSession(executor);

        GD.Print("Model loaded!");
    }

    public void UnloadModel()
    {
        weights.Dispose();
        context.Dispose();
        embedder.Dispose();
    }

    public async Task InferAsync(string prompt)
    {
        await Task.Run(async () =>
        {
            await foreach (var output in session.ChatAsync(new ChatHistory.Message(AuthorRole.User, prompt), new InferenceParams { Temperature = 0.5f, AntiPrompts = new[] { "\n\n" } }))
            {
                CallDeferred(nameof(DeferredEmitNewOutput), output);
            }
        });
    }

    public void DeferredEmitNewOutput(string output)
    {
        EmitSignal(nameof(ModelOutput), output);
    }
}
