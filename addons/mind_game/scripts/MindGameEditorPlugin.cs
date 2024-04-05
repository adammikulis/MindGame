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

    public override void _EnterTree()
    {
        PackedScene mindGameInterfaceScene = (PackedScene)GD.Load("res://addons/mind_game/scenes/EditorInterface.tscn");
        editorInterface = mindGameInterfaceScene.Instantiate<Control>();
        AddControlToBottomPanel(editorInterface, "Mind Game");
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