#if TOOLS
using Godot;
using LLama;
using System;

[Tool]
public partial class EditorInterface : Control
{
    public ModelInterface modelInterface;
    public ChatInterface chatInterface;
    
    public override void _EnterTree()
    {
        

    }

    private void OnEmbedderAvailable(LLamaEmbedder embedder)
    {
        chatInterface.SetEmbedder(embedder);
    }

    private void OnContextAvailable(LLamaContext context)
    {
        if (chatInterface != null)
        {
            chatInterface.InitializeExecutor(context);
            chatInterface.InitializeChatSession();
        }
        else
        {
            GD.PrintErr("chatInterface is null in EditorInterface");
        }
    }

    public override void _Ready()
    {
        Control modelNode = GetNode<Control>("%Model");
        modelInterface = modelNode as ModelInterface;

        Control chatNode = GetNode<Control>("%Chat");
        chatInterface = chatNode as ChatInterface;

        if (modelInterface != null && chatInterface != null)
        {
            modelInterface.ContextAvailable += OnContextAvailable;
            modelInterface.EmbedderAvailable += OnEmbedderAvailable;
        }
        else
        {
            if (modelInterface == null)
            {
                GD.PrintErr("modelInterface is null in EditorInterface");
            }
            if (chatInterface == null)
            {
                GD.PrintErr("chatInterface is null in EditorInterface");
            }
        }
    }

    public override void _ExitTree()
    {
        
    }


}
#endif