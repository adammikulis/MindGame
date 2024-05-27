using Godot;
using System;

namespace MindGame
{
    public partial class ModelConfigController : Control
    {
        private ConfigListResource _configListResource;
        private ModelParamsConfig _modelParamsConfig;
        private MindManager _mindManager;
        private readonly string _configListResourcePath = "res://addons/mind_game/assets/resources/custom_resources/ConfigListResource.tres";

        // UI elements
        private Button _addNewConfigButton, _deleteConfigButton, _selectChatPathButton, _clearChatPathButton, _selectEmbedderPathButton, _clearEmbedderPathButton, _selectClipPathButton, _clearClipPathButton, _backButton, _loadConfigButton, _unloadConfigButton;
        private Label _chatContextSizeLabel, _chatGpuLayerCountLabel, _embedderContextSizeLabel, _embedderGpuLayerCountLabel, _chatCurrentModelPathLabel, _embedderCurrentModelPathLabel, _clipCurrentModelPathLabel;
        private FileDialog _selectChatPathFileDialog, _selectClipPathFileDialog, _selectEmbedderPathFileDialog;
        private HSlider _chatContextSizeHSlider, _chatGpuLayerCountHSlider, _embedderContextSizeHSlider, _embedderGpuLayerCountHSlider;
        private LineEdit _configNameLineEdit, _chatRandomSeedLineEdit, _embedderRandomSeedLineEdit;
        private ItemList _savedConfigsItemList;
        private CheckBox _autoloadLastGoodConfigCheckBox;

        // Model params
        private string _configName;
        private int _chatGpuLayerCount, _embedderGpuLayerCount;
        private uint _chatContextSize, _embedderContextSize, _chatRandomSeed, _embedderRandomSeed;
        private string _chatModelPath, _clipModelPath, _embedderModelPath;

        /// <summary>
        /// Function that is called when node and all children are initialized
        /// </summary>
        public override void _Ready()
        {
            InitializeDefaultValues();
            InitializeNodeRefs();
            InitializeConfigList();
            InitializeUiElements();
            InitializeSignals();
            AutoloadLastGoodConfig();
        }

        /// <summary>
        /// Loads the configuration list resource
        /// </summary>
        private void InitializeConfigList()
        {
            _configListResource = GD.Load<ConfigListResource>(_configListResourcePath) ?? new ConfigListResource();
            UpdateConfigItemList();
        }

        /// <summary>
        /// Updates the ItemList with saved model configurations
        /// </summary>
        private void UpdateConfigItemList()
        {
            _savedConfigsItemList.Clear();
            foreach (var config in _configListResource.ModelConfigurations)
            {
                _savedConfigsItemList.AddItem(config.ModelConfigName);
            }
        }


        private void InitializeDefaultValues()
        {
            _configName = "<default>";
            _chatContextSize = 4000;
            _chatGpuLayerCount = 33;
            _chatRandomSeed = 0;
            _chatModelPath = "";
            _embedderContextSize = 4000;
            _embedderGpuLayerCount = 33;
            _embedderRandomSeed = 0;
            _embedderModelPath = "";
            _clipModelPath = "";
        }

        private void InitializeNodeRefs()
        {
            _mindManager = GetNode<MindManager>("/root/MindManager");

            // Manage configs nodes
            _configNameLineEdit = GetNode<LineEdit>("%ConfigNameLineEdit");
            _savedConfigsItemList = GetNode<ItemList>("%SavedConfigsItemList");
            _addNewConfigButton = GetNode<Button>("%AddNewConfigButton");
            _deleteConfigButton = GetNode<Button>("%DeleteConfigButton");
            _backButton = GetNode<Button>("%BackButton");
            _loadConfigButton = GetNode<Button>("%LoadConfigButton");
            _unloadConfigButton = GetNode<Button>("%UnloadConfigButton");
            _autoloadLastGoodConfigCheckBox = GetNode<CheckBox>("%AutoloadLastGoodConfigCheckBox");

            // Chat param nodes
            _chatContextSizeHSlider = GetNode<HSlider>("%ChatContextSizeHSlider");
            _chatContextSizeLabel = GetNode<Label>("%ChatContextSizeLabel");
            _chatGpuLayerCountHSlider = GetNode<HSlider>("%ChatModelGpuLayerCountHSlider");
            _chatGpuLayerCountLabel = GetNode<Label>("%ChatModelGpuLayerCountLabel");
            _chatRandomSeedLineEdit = GetNode<LineEdit>("%ChatRandomSeedLineEdit");

            // Chat path nodes
            _selectChatPathButton = GetNode<Button>("%SelectChatPathButton");
            _clearChatPathButton = GetNode<Button>("%ClearChatPathButton");
            _selectChatPathFileDialog = GetNode<FileDialog>("%SelectChatPathFileDialog");
            _chatCurrentModelPathLabel = GetNode<Label>("%ChatCurrentModelPathLabel");

            // Embedder param nodes
            _embedderContextSizeHSlider = GetNode<HSlider>("%EmbedderContextSizeHSlider");
            _embedderContextSizeLabel = GetNode<Label>("%EmbedderContextSizeLabel");
            _embedderGpuLayerCountHSlider = GetNode<HSlider>("%EmbedderGpuLayerCountHSlider");
            _embedderGpuLayerCountLabel = GetNode<Label>("%EmbedderGpuLayerCountLabel");
            _embedderRandomSeedLineEdit = GetNode<LineEdit>("%EmbedderRandomSeedLineEdit");

            // Embedder path nodes
            _selectEmbedderPathButton = GetNode<Button>("%SelectEmbedderPathButton");
            _clearEmbedderPathButton = GetNode<Button>("%ClearEmbedderPathButton");
            _selectEmbedderPathFileDialog = GetNode<FileDialog>("%SelectEmbedderPathFileDialog");
            _embedderCurrentModelPathLabel = GetNode<Label>("%EmbedderCurrentModelPathLabel");

            // Clip path nodes
            _selectClipPathButton = GetNode<Button>("%SelectClipPathButton");
            _clearClipPathButton = GetNode<Button>("%ClearClipPathButton");
            _selectClipPathFileDialog = GetNode<FileDialog>("%SelectClipPathFileDialog");
            _clipCurrentModelPathLabel = GetNode<Label>("%ClipCurrentModelPathLabel");
        }

        private void InitializeSignals()
        {
            // Chat signals
            _addNewConfigButton.Pressed += OnAddNewConfigPressed;
            _deleteConfigButton.Pressed += OnDeleteConfigPressed;
            _backButton.Pressed += OnBackPressed;
            _loadConfigButton.Pressed += OnLoadConfigPressed;
            _unloadConfigButton.Pressed += OnUnloadConfigPressed;
            _autoloadLastGoodConfigCheckBox.Toggled += OnAutoloadLastGoodConfigToggled;

            _clearChatPathButton.Pressed += OnClearChatPathPressed;
            _clearClipPathButton.Pressed += OnClearClipPathPressed;
            _clearEmbedderPathButton.Pressed += OnClearEmbedderPathPressed;

            _selectChatPathButton.Pressed += OnSelectChatPathPressed;
            _selectClipPathButton.Pressed += OnSelectClipPathPressed;
            _selectEmbedderPathButton.Pressed += OnSelectEmbedderPathPressed;

            _chatContextSizeHSlider.ValueChanged += OnChatContextSizeHSliderValueChanged;
            _chatGpuLayerCountHSlider.ValueChanged += OnChatGpuLayerCountHSliderValueChanged;
            _embedderContextSizeHSlider.ValueChanged += OnEmbedderContextSizeHSliderValueChanged;
            _embedderGpuLayerCountHSlider.ValueChanged += OnEmbedderGpuLayerCountHSliderValueChanged;

            _selectChatPathFileDialog.FileSelected += OnChatPathSelected;
            _selectClipPathFileDialog.FileSelected += OnClipPathSelected;
            _selectEmbedderPathFileDialog.FileSelected += OnEmbedderPathSelected;

            _configNameLineEdit.TextChanged += OnConfigNameTextChanged;
            _savedConfigsItemList.ItemSelected += OnSavedConfigsItemSelected;
        }

        private void OnAutoloadLastGoodConfigToggled(bool toggledOn)
        {
            if (_configListResource != null)
            {
                _configListResource.AutoloadLastGoodModelConfig = toggledOn;
                SaveConfigList();
            }
        }

        public void AutoloadLastGoodConfig()
        {
            if (_configListResource != null && _configListResource.AutoloadLastGoodModelConfig && _configListResource.LastGoodModelConfig != null)
            {
                _modelParamsConfig = _configListResource.LastGoodModelConfig;
                LoadConfig(_modelParamsConfig);
                OnLoadConfigPressed();
            }
        }

        private void LoadConfig(ModelParamsConfig config)
        {
            _configNameLineEdit.Text = config.ModelConfigName;
            _chatContextSizeHSlider.Value = CalculateLogContextSize(config.ChatContextSize);
            _chatGpuLayerCountHSlider.Value = config.ChatGpuLayerCount;
            _chatRandomSeedLineEdit.Text = config.ChatRandomSeed.ToString();
            _chatCurrentModelPathLabel.Text = config.ChatModelPath;
            _embedderContextSizeHSlider.Value = CalculateLogContextSize(config.EmbedderContextSize);
            _embedderGpuLayerCountHSlider.Value = config.EmbedderGpuLayerCount;
            _embedderRandomSeedLineEdit.Text = config.EmbedderRandomSeed.ToString();
            _embedderCurrentModelPathLabel.Text = config.EmbedderModelPath;
            _clipCurrentModelPathLabel.Text = config.ClipModelPath;
        }

        private async void OnUnloadConfigPressed()
        {
            await _mindManager.DisposeExecutorAsync();
        }

        private async void OnLoadConfigPressed()
        {
            if (_modelParamsConfig != null)
            {
                await _mindManager.InitializeAsync(_modelParamsConfig);
                _configListResource.LastGoodModelConfig = _modelParamsConfig;
                SaveConfigList();
            }
        }

        private void OnBackPressed()
        {
            Visible = false;
        }

        private void InitializeUiElements()
        {
            _configNameLineEdit.Text = _configName;

            // Initialize chat sliders and labels
            InitializeSliderAndLabel(_chatGpuLayerCountHSlider, _chatGpuLayerCountLabel, _chatGpuLayerCount);
            InitializeSliderAndLabel(_chatContextSizeHSlider, _chatContextSizeLabel, _chatContextSize, CalculateLogContextSize);

            _chatRandomSeedLineEdit.Text = _chatRandomSeed.ToString();

            // Initialize embedder sliders and labels
            InitializeSliderAndLabel(_embedderGpuLayerCountHSlider, _embedderGpuLayerCountLabel, _embedderGpuLayerCount);
            InitializeSliderAndLabel(_embedderContextSizeHSlider, _embedderContextSizeLabel, _embedderContextSize, CalculateLogContextSize);

            _embedderRandomSeedLineEdit.Text = _embedderRandomSeed.ToString();

            // Initialize autoload checkbox
            if (_configListResource != null)
            {
                _autoloadLastGoodConfigCheckBox.ButtonPressed = _configListResource.AutoloadLastGoodModelConfig;
            }
        }

        private void InitializeSliderAndLabel(HSlider slider, Label label, int value)
        {
            slider.Value = value;
            label.Text = value.ToString();
        }

        private void InitializeSliderAndLabel(HSlider slider, Label label, uint value, Func<uint, double> conversionFunc)
        {
            slider.Value = conversionFunc(value);
            label.Text = value.ToString();
        }

        private void OnSavedConfigsItemSelected(long index)
        {
            _modelParamsConfig = _configListResource.ModelConfigurations[(int)index];
            LoadConfig(_modelParamsConfig);
        }

        private void OnDeleteConfigPressed()
        {
            var selectedIndices = _savedConfigsItemList.GetSelectedItems();

            if (selectedIndices.Length > 0 && _configListResource.ModelConfigurations.Count > 0)
            {
                int selectedIndex = selectedIndices[0];
                if (selectedIndex >= 0 && selectedIndex < _configListResource.ModelConfigurations.Count)
                {
                    _configListResource.ModelConfigurations.RemoveAt(selectedIndex);
                    SaveConfigList();
                    UpdateConfigItemList();
                }
            }
            else
            {
                GD.Print("No configuration selected or list is empty.");
            }
        }

        private void OnAddNewConfigPressed()
        {
            ModelParamsConfig newConfig = new()
            {
                ModelConfigName = _configName,
                ChatContextSize = _chatContextSize,
                ChatGpuLayerCount = _chatGpuLayerCount,
                ChatRandomSeed = _chatRandomSeed,
                ChatModelPath = _chatModelPath,
                EmbedderContextSize = _embedderContextSize,
                EmbedderGpuLayerCount = _embedderGpuLayerCount,
                EmbedderRandomSeed = _embedderRandomSeed,
                EmbedderModelPath = _embedderModelPath,
                ClipModelPath = _clipModelPath
            };

            _configListResource.ModelConfigurations.Add(newConfig);
            SaveConfigList();
            UpdateConfigItemList();
        }

        private void UpdateConfigurationValue(Action<ModelParamsConfig> updateAction)
        {
            var selectedIndices = _savedConfigsItemList.GetSelectedItems();
            if (selectedIndices.Length > 0)
            {
                int selectedIndex = selectedIndices[0];
                if (selectedIndex >= 0 && selectedIndex < _configListResource.ModelConfigurations.Count)
                {
                    var config = _configListResource.ModelConfigurations[selectedIndex];
                    updateAction(config);
                    SaveConfigList();
                }
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

        private void ClearPath(Action updatePathAction, Label pathLabel, Action<ModelParamsConfig, string> updateAction)
        {
            updatePathAction();
            pathLabel.Text = "";
            UpdateConfigurationValue(config => updateAction(config, ""));
        }

        private void OnClearEmbedderPathPressed() => ClearPath(() => _embedderModelPath = "", _embedderCurrentModelPathLabel, (config, path) => config.EmbedderModelPath = path);
        private void OnClearClipPathPressed() => ClearPath(() => _clipModelPath = "", _clipCurrentModelPathLabel, (config, path) => config.ClipModelPath = path);
        private void OnClearChatPathPressed() => ClearPath(() => _chatModelPath = "", _chatCurrentModelPathLabel, (config, path) => config.ChatModelPath = path);

        private void OnConfigNameTextChanged(string newText)
        {
            _configName = newText;
            UpdateConfigurationValue(config =>
            {
                config.ModelConfigName = _configName;
                _savedConfigsItemList.SetItemText(_savedConfigsItemList.GetSelectedItems()[0], _configName);
            });
        }

        private void OnChatGpuLayerCountHSliderValueChanged(double value) => UpdateSliderValue((int)value, _chatGpuLayerCountLabel, v => _chatGpuLayerCount = v, config => config.ChatGpuLayerCount = _chatGpuLayerCount);
        private void OnEmbedderGpuLayerCountHSliderValueChanged(double value) => UpdateSliderValue((int)value, _embedderGpuLayerCountLabel, v => _embedderGpuLayerCount = v, config => config.EmbedderGpuLayerCount = _embedderGpuLayerCount);
        private void OnChatContextSizeHSliderValueChanged(double value) => UpdateSliderValue(CalculateExpContextSize(value), _chatContextSizeLabel, v => _chatContextSize = v, config => config.ChatContextSize = _chatContextSize);
        private void OnEmbedderContextSizeHSliderValueChanged(double value) => UpdateSliderValue(CalculateExpContextSize(value), _embedderContextSizeLabel, v => _embedderContextSize = v, config => config.EmbedderContextSize = _embedderContextSize);

        private void UpdateSliderValue<T>(T value, Label label, Action<T> setValue, Action<ModelParamsConfig> updateConfig)
        {
            setValue(value);
            label.Text = value.ToString();
            UpdateConfigurationValue(updateConfig);
        }

        private void OnSelectChatPathPressed() => _selectChatPathFileDialog.PopupCentered();
        private void OnChatPathSelected(string path) => UpdatePath(() => _chatModelPath = path, _chatCurrentModelPathLabel, config => config.ChatModelPath = _chatModelPath);
        private void OnSelectClipPathPressed() => _selectClipPathFileDialog.PopupCentered();
        private void OnClipPathSelected(string path) => UpdatePath(() => _clipModelPath = path, _clipCurrentModelPathLabel, config => config.ClipModelPath = _clipModelPath);
        private void OnSelectEmbedderPathPressed() => _selectEmbedderPathFileDialog.PopupCentered();
        private void OnEmbedderPathSelected(string path) => UpdatePath(() => _embedderModelPath = path, _embedderCurrentModelPathLabel, config => config.EmbedderModelPath = _embedderModelPath);

        private void UpdatePath(Action updatePathAction, Label pathLabel, Action<ModelParamsConfig> updateConfig)
        {
            updatePathAction();
            pathLabel.Text = pathLabel.Text;
            UpdateConfigurationValue(updateConfig);
        }

        private static uint CalculateExpContextSize(double value) => (uint)Math.Pow(2, value) * 1000;
        private static double CalculateLogContextSize(uint value) => Math.Log2(value / 1000.0);

        public override void _ExitTree() { }
    }
}
