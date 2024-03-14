using Godot;
using LLama;
using LLama.Common;
using System;
using static System.Net.Mime.MediaTypeNames;

public partial class MindGameModel : Node
{
    [Signal]
    public delegate void ModelOutputEventHandler(string output);

    public InteractiveExecutor executor;
    public ChatSession chatSession;

    public void LoadModel(string modelPath)
    {
        var parameters = new ModelParams(modelPath)
        {
            ContextSize = 4096,
            Seed = 0,
            GpuLayerCount = 33,
            EmbeddingMode = true
        };

        using var weights = LLamaWeights.LoadFromFile(parameters);
        using var context = weights.CreateContext(parameters);

        executor = new InteractiveExecutor(context);
        chatSession = new ChatSession(executor);
    }

    private async void InferAsync(string prompt)
    {
        await foreach (var text in chatSession.ChatAsync(new ChatHistory.Message(AuthorRole.User, prompt), new InferenceParams { Temperature = 0.6f, AntiPrompts = [ "\n\n" ]}))
        {
            EmitSignal(nameof(ModelOutputEventHandler), text);
        }
    }
}
