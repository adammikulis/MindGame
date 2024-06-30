using Godot;
using System;


namespace MindGame
{
    public partial class ChatExample : Node
    {
        private MindGame.MindManager _mindManager;
        private MindAgent3D _mindAgent3D;
        private MindGame.ModelConfig _modelConfig;
        private MindGame.InferenceConfig _inferenceConfig;

        private Button _configAndLoadModelsButton, _inferenceConfigButton, _exitButton;
        private LineEdit _modelInputLineEdit;
        private ItemList _savedConversationsItemList;
        private RichTextLabel _modelOutputRichTextLabel;
        

        /// <summary>
        /// Function that is called when node and all children are initialized
        /// </summary>
        public override void _Ready()
        {
            InitializeNodeRefs();
            InitializeSignals();
            InitializeLabels();
        }

        private void InitializeLabels()
        {
            
        }

        

        /// <summary>
        /// Function that is called to assign scene tree nodes to script variables
        /// </summary>
        private void InitializeNodeRefs()
        {
            _mindManager = GetNode<MindGame.MindManager>("/root/MindManager");
            _mindAgent3D = GetNode<MindAgent3D>("%MindAgent3D");
            _modelInputLineEdit = GetNode<LineEdit>("%ModelInputLineEdit");
            _inferenceConfig = GetNode<InferenceConfig>("%InferenceConfig");
            _modelConfig = GetNode<ModelConfig>("%ModelConfig");
            _savedConversationsItemList = GetNode<ItemList>("%SavedConversationsItemList");

            _modelOutputRichTextLabel = GetNode<RichTextLabel>("%ModelOutputRichTextLabel");

            _configAndLoadModelsButton = GetNode<Button>("%ConfigAndLoadModelsButton");
           
            _exitButton = GetNode<Button>("%ExitButton");

        }

        /// <summary>
        /// Function that is called to connect signals to callbacks
        /// </summary>
        private void InitializeSignals()
        {
            _modelInputLineEdit.TextSubmitted += OnModelInputTextSubmitted;

            _configAndLoadModelsButton.Pressed += OnConfigAndLoadModelsPressed;
            // _inferenceConfigButton.Pressed += OnInferenceConfigPressed;
            _mindAgent3D.ChatOutputReceived += OnChatOutputReceived;

            _exitButton.Pressed += OnExitPressed;
        }

        private void OnChatOutputReceived(string text)
        {
            _modelOutputRichTextLabel.Text += $"{text}\n";
        }

        /// <summary>
        /// Function to save configuration list
        /// </summary>
        private void SaveConfigList()
        {
            Error saveError = ResourceSaver.Save(_mindManager.ConfigList, _mindManager.ConfigListPath);
            if (saveError != Error.Ok)
            {
                GD.PrintErr("Failed to save configuration list: ", saveError);
            }
        }

        private void OnInferenceConfigPressed()
        {
            _inferenceConfig.Visible = true;
        }

        private async void OnExitPressed()
        {
            await _mindManager.DisposeExecutorAsync();
            GetTree().Quit();
        }

        private void OnConfigAndLoadModelsPressed()
        {
            _modelConfig.Visible = true;
        }

        private async void OnModelInputTextSubmitted(string prompt)
        {
            _modelInputLineEdit.Text = "";
            _modelOutputRichTextLabel.Text += $"{prompt}\n";
            await _mindAgent3D.InferAsync(prompt);
        }

        private void OnChatSessionStatusUpdate(bool isLoaded)
        {
            _modelInputLineEdit.Editable = isLoaded;
            if (isLoaded)
            {
                _modelInputLineEdit.PlaceholderText = $"Type prompt and hit Enter";
            }
            else
            {
                _modelInputLineEdit.PlaceholderText = $"Load a model to chat!";
            }
        }

        public override void _ExitTree()
        {

        }


    }

}