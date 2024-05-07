using Godot;
using System;

public partial class SingleAgentChat : Node
{
    private MindGame.MindManager mindManager;
    private MindGame.MindAgent mindAgent;

    private RichTextLabel modelOutputRichTextLabel;
    private LineEdit modelInputLineEdit;
    public override void _Ready()
    {
        mindManager = GetNode<MindGame.MindManager>("/root/MindManager");
        mindAgent = GetNode<MindGame.MindAgent>("%MindAgent");
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
        modelOutputRichTextLabel.Text += $"\n\n{newText}\n\n";
        await mindAgent.InferAsync(newText);
    }

    private void OnChatSessionStatusUpdate(bool isLoaded)
    {
        modelOutputRichTextLabel.Text += $"Chat session loaded: {isLoaded}";
    }
}
