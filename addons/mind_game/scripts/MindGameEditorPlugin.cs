#if TOOLS
using Godot;
using System;
using System.Threading.Tasks;
using LLama.Common;
using LLama.Native;
using LLama.Sampling;
using LLama;

[Tool]
public partial class MindGameEditorPlugin : EditorPlugin, IDisposable
{
    
    private Control editorInterface;
    private Script modelInterfaceScript = GD.Load<CSharpScript>("res://addons/mind_game/scripts/interfaces/ModelInterface.cs");
    private Texture2D modelIcon = GD.Load<Texture2D>("res://addons/mind_game/assets/logos/brain_pink.png");



    public override void _EnterTree()
    {
        PackedScene mindGameInterfaceScene = (PackedScene)GD.Load("res://addons/mind_game/scenes/EditorInterface.tscn");
        editorInterface = mindGameInterfaceScene.Instantiate<Control>();
        AddControlToBottomPanel(editorInterface, "Mind Game");

        AddCustomType("ModelInterface", "Node", modelInterfaceScript, modelIcon);

    }




    public override void _Ready()
    {

    }


    public override void _Process(double delta)
    {
        
    }


    public override void _ExitTree()
    {
        RemoveControlFromBottomPanel(editorInterface);
        editorInterface.QueueFree();
    }
}
#endif