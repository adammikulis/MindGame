using Godot;
using System;


namespace MindGame
{
    [Tool]
    public partial class ChatExample : Node
    {
        private MindGame.MindManager mindManager;
        private MindAgent3D mindAgent3D;
        private MindGame.ModelConfigController ModelConfigController;
        private MindGame.InferenceConfigController InferenceConfigController;

        private Button configAndLoadModelsButton, inferenceConfigButton, exitButton;
        private LineEdit modelInputLineEdit;


        public override void _Ready()
        {
            mindManager = GetNode<MindGame.MindManager>("/root/MindManager");
            mindAgent3D = GetNode<MindAgent3D>("%MindAgent3D");
            ModelConfigController = GetNode<MindGame.ModelConfigController>("%ModelConfigController");
            InferenceConfigController = GetNode<MindGame.InferenceConfigController>("%InferenceConfigController");

            modelInputLineEdit = GetNode<LineEdit>("%ModelInputLineEdit");

            configAndLoadModelsButton = GetNode<Button>("%ConfigAndLoadModelsButton");
            inferenceConfigButton = GetNode<Button>("%InferenceConfigButton");
            exitButton = GetNode<Button>("%ExitButton");

            modelInputLineEdit.TextSubmitted += OnModelInputTextSubmitted;
            mindAgent3D.ChatSessionStatusUpdate += OnChatSessionStatusUpdate;

            configAndLoadModelsButton.Pressed += OnConfigAndLoadModelsPressed;
            inferenceConfigButton.Pressed += OnInferenceConfigPressed;
            exitButton.Pressed += OnExitPressed;

            // Call the autoload methods
            ModelConfigController.AutoloadLastGoodConfig();
            InferenceConfigController.AutoloadLastGoodConfig();

        }


        private void OnInferenceConfigPressed()
        {
            InferenceConfigController.Visible = true;
        }

        private async void OnExitPressed()
        {
            await mindManager.DisposeExecutorAsync();
            GetTree().Quit();
        }

        private void OnConfigAndLoadModelsPressed()
        {
            ModelConfigController.Visible = true;
        }

        private async void OnModelInputTextSubmitted(string newText)
        {
            modelInputLineEdit.Text = "";
            await mindAgent3D.InferAsync(newText);
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