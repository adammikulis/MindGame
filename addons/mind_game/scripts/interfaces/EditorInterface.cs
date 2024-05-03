/* This class is deprecated for v0.2

using Godot;
using LLama;
using System;

[Tool]
public partial class EditorInterface : Control
{
    public ModelInterface modelInterface;
    public ChatInterface chatInterface;
    private Button exitProgramButton;
    
    public override void _EnterTree()
    {
        

    }

    public override void _Ready()
    {
        exitProgramButton = GetNode<Button>("%ExitProgramButton");

        Control modelNode = GetNode<Control>("%Models");
        modelInterface = modelNode as ModelInterface;

        Control chatNode = GetNode<Control>("%Chat");
        chatInterface = chatNode as ChatInterface;

        if (modelInterface != null && chatInterface != null)
        {
            modelInterface.ExecutorAvailable += OnExecutorAvailable;
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

        exitProgramButton.Pressed += OnExitProgramPressed;
    }

    private void OnExitProgramPressed()
    {
        modelInterface.UnloadChatModel();
        modelInterface.UnloadEmbedderModel();
        GetTree().Quit();
    }

    private void OnEmbedderAvailable(LLamaEmbedder embedder)
    {
        chatInterface.SetEmbedder(embedder);
    }

    private void OnExecutorAvailable(InteractiveExecutor executor)
    {
        chatInterface.SetExecutor(executor);
        chatInterface.InitializeChatSession();
    }

    

    public override void _ExitTree()
    {
        
    }


}
*/