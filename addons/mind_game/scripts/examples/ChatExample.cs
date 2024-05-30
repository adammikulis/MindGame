using Godot;
using System;


namespace MindGame
{
    [Tool]
    public partial class ChatExample : Node
    {
        private MindGame.MindManager _mindManager;
        private MindAgent3D _mindAgent3D;
        private MindGame.ModelConfigController _modelConfigController;
        private MindGame.InferenceConfigController _inferenceConfigController;

        private Button _configAndLoadModelsButton, _inferenceConfigButton, _exitButton;
        private LineEdit _modelInputLineEdit;
        private ConfigListResource _configListResource;
        private Label _jsonLabel;

        private readonly string _configListResourcePath = "res://addons/mind_game/assets/resources/custom_resources/ConfigListResource.tres";


        public override void _Ready()
        {
            InitializeNodeRefs();
            InitializeSignals();
            EnsureConfigListResourceExists();
            _configListResource = GD.Load<ConfigListResource>(_configListResourcePath) ?? new ConfigListResource();
            InitializeLabels();
        }

        private void InitializeLabels()
        {
            
        }

        /// <summary>
        /// Function that is called to connect signals to callbacks
        /// </summary>
        private void InitializeSignals()
        {
            _modelInputLineEdit.TextSubmitted += OnModelInputTextSubmitted;
            _mindAgent3D.ChatSessionStatusUpdate += OnChatSessionStatusUpdate;

            _configAndLoadModelsButton.Pressed += OnConfigAndLoadModelsPressed;
            _inferenceConfigButton.Pressed += OnInferenceConfigPressed;
            _exitButton.Pressed += OnExitPressed;
        }

        /// <summary>
        /// Function that is called to assign scene tree nodes to script variables
        /// </summary>
        private void InitializeNodeRefs()
        {
            _mindManager = GetNode<MindGame.MindManager>("/root/MindManager");
            _mindAgent3D = GetNode<MindAgent3D>("%MindAgent3D");
            _modelConfigController = GetNode<MindGame.ModelConfigController>("%ModelConfigController");
            _inferenceConfigController = GetNode<MindGame.InferenceConfigController>("%InferenceConfigController");

            _modelInputLineEdit = GetNode<LineEdit>("%ModelInputLineEdit");

            _configAndLoadModelsButton = GetNode<Button>("%ConfigAndLoadModelsButton");
            _inferenceConfigButton = GetNode<Button>("%InferenceConfigButton");
            _exitButton = GetNode<Button>("%ExitButton");

            _jsonLabel = GetNode<Label>("%JsonLabel");
        }

        private void EnsureConfigListResourceExists()
        {
            _configListResource = GD.Load<ConfigListResource>(_configListResourcePath);
            if (_configListResource == null)
            {
                _configListResource = new ConfigListResource();
                SaveConfigList();
            }
        }

        private void SaveConfigList()
        {
            Error saveError = ResourceSaver.Save(_configListResource, _configListResourcePath);
            if (saveError != Error.Ok)
            {
                GD.PrintErr("Failed to save configuration list: ", saveError);
            }
        }

        private void OnInferenceConfigPressed()
        {
            _inferenceConfigController.Visible = true;
        }

        private async void OnExitPressed()
        {
            await _mindManager.DisposeExecutorAsync();
            GetTree().Quit();
        }

        private void OnConfigAndLoadModelsPressed()
        {
            _modelConfigController.Visible = true;
        }

        private async void OnModelInputTextSubmitted(string newText)
        {
            _modelInputLineEdit.Text = "";
            await _mindAgent3D.InferAsync(newText);
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