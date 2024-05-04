using Godot;
using System;

public partial class SingleAgentChat : Node
{
    private MindManager mm;
    private MindAgent mindAgent;

    private RichTextLabel modelOutputRichTextLabel;
    private LineEdit modelInputLineEdit;
    public override void _Ready()
    {
        mm = GetNode<MindManager>("/root/MindManager");
        mindAgent = GetNode<MindAgent>("%MindAgent");
        modelOutputRichTextLabel = GetNode<RichTextLabel>("%ModelOutputRichTextLabel");
        modelInputLineEdit = GetNode<LineEdit>("%ModelInputLineEdit");

        mindAgent.ChatSessionStatusUpdate += OnChatSessionStatusUpdate;

        modelInputLineEdit.TextSubmitted += OnModelInputTextSubmitted;
        mindAgent.ChatOutputReceived += OnChatOutputReceived;

    }

    private async void OnChatOutputReceived(string text)
    {
        modelOutputRichTextLabel.Text += text;
    }

    private async void OnModelInputTextSubmitted(string newText)
    {
        modelInputLineEdit.Text = "";
        await mindAgent.InferAsync(newText);
    }

    private void OnChatSessionStatusUpdate(bool isLoaded)
    {
        modelOutputRichTextLabel.Text += $"Chat session loaded: {isLoaded}";
    }
}
