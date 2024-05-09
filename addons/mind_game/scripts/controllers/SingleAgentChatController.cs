using Godot;
using System;

namespace MindGame
{
    public partial class SingleAgentChatController : Control
    {
        private MindGame.MindManager mindManager;
        private MindGame.MindAgent mindAgent;
        private MindGame.MindManagerController mindManagerController;

        private Button configAndLoadModelsButton;

        private RichTextLabel modelOutputRichTextLabel;
        private LineEdit modelInputLineEdit;

        public override void _Ready()
        {
            mindManager = GetNode<MindGame.MindManager>("/root/MindManager");
            mindAgent = GetNode<MindGame.MindAgent>("%MindAgent");
            modelOutputRichTextLabel = GetNode<RichTextLabel>("%ModelOutputRichTextLabel");
            modelInputLineEdit = GetNode<LineEdit>("%ModelInputLineEdit");
            mindManagerController = GetNode<MindGame.MindManagerController>("%MindManagerController");

            configAndLoadModelsButton = GetNode<Button>("%ConfigAndLoadModelsButton");


            mindAgent.ChatSessionStatusUpdate += OnChatSessionStatusUpdate;

            modelInputLineEdit.TextSubmitted += OnModelInputTextSubmitted;
            mindAgent.ChatOutputReceived += OnChatOutputReceived;
            configAndLoadModelsButton.Pressed += OnConfigAndLoadModelsPressed;

        }

        private void OnConfigAndLoadModelsPressed()
        {
            mindManagerController.Visible = true;
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
}