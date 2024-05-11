using Godot;
using System;


namespace MindGame
{
    [Tool]
    public partial class ChatExample : Node
    {
        private MindGame.MindManager mindManager;
        private MindAgent3D mindAgent3D;
        private MindGame.MindManagerController mindManagerController;

        private Button configAndLoadModelsButton, exitButton;
        private LineEdit modelInputLineEdit;


        public override void _Ready()
        {
            mindManager = GetNode<MindGame.MindManager>("/root/MindManager");
            mindAgent3D = GetNode<MindAgent3D>("%MindAgent3D");
            mindManagerController = GetNode<MindGame.MindManagerController>("%MindManagerController");

            modelInputLineEdit = GetNode<LineEdit>("%ModelInputLineEdit");

            configAndLoadModelsButton = GetNode<Button>("%ConfigAndLoadModelsButton");
            exitButton = GetNode<Button>("%ExitButton");

            modelInputLineEdit.TextSubmitted += OnModelInputTextSubmitted;
            mindAgent3D.ChatSessionStatusUpdate += OnChatSessionStatusUpdate;
            
            configAndLoadModelsButton.Pressed += OnConfigAndLoadModelsPressed;
            exitButton.Pressed += OnExitPressed;

        }

        private void OnExitPressed()
        {
            mindManager.DisposeExecutorAsync();
            GetTree().Quit();
        }

        private void OnConfigAndLoadModelsPressed()
        {
            mindManagerController.Visible = true;
        }

        private async void OnModelInputTextSubmitted(string newText)
        {
            modelInputLineEdit.Text = "";
            mindAgent3D.InferAsync(newText);
        }

        private void OnChatSessionStatusUpdate(bool isLoaded)
        {
            modelInputLineEdit.Editable = isLoaded;
            if (isLoaded)
            {
                modelInputLineEdit.PlaceholderText = $"Type prompt and hit Enter";
            }
            else
            {
                modelInputLineEdit.PlaceholderText = $"Load a model to chat!";
            }
        }

        public override void _ExitTree()
        {

        }


    }

}