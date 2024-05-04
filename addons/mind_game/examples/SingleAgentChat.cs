using Godot;
using System;

public partial class SingleAgentChat : Node
{
    private MindManager mm;

    private RichTextLabel modelOutputRichTextLabel;
    private LineEdit modelInputLineEdit;
    public async override void _Ready()
    {
        mm = GetNode<MindManager>("/root/MindManager");
        await mm.InitializeAsync();

    }

    private void OnChatSessionStatusUpdate(bool isLoaded)
    {
        
    }
}
