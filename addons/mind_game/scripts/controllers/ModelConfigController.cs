using Godot;
using System;

namespace MindGame
{
    [Tool]
    public partial class ModelConfigController : Control
    {
        public ConfigListResource ConfigListResource;
        public ModelParamsConfig ModelParamsConfig;
        public MindManager mindManager;
        private readonly string configListResourcePath = "res://addons/mind_game/assets/resources/custom_resources/ConfigListResource.tres";

        // UI elements
        private Button addNewConfigButton, deleteConfigButton, selectChatPathButton, clearChatPathButton, selectEmbedderPathButton, clearEmbedderPathButton, selectClipPathButton, clearClipPathButton, backButton, loadConfigButton, unloadConfigButton;
        private Label chatContextSizeLabel, chatGpuLayerCountLabel, embedderContextSizeLabel, embedderGpuLayerCountLabel, chatCurrentModelPathLabel, embedderCurrentModelPathLabel, clipCurrentModelPathLabel;
        private FileDialog selectChatPathFileDialog, selectClipPathFileDialog, selectEmbedderPathFileDialog;
        private HSlider chatContextSizeHSlider, chatGpuLayerCountHSlider, embedderContextSizeHSlider, embedderGpuLayerCountHSlider;
        private LineEdit configNameLineEdit, chatRandomSeedLineEdit, embedderRandomSeedLineEdit;
        private ItemList savedConfigsItemList;
        private CheckBox autoloadLastGoodConfigCheckBox;

        // Model params
        private string configName;
        private int chatGpuLayerCount, embedderGpuLayerCount;
        private uint chatContextSize, embedderContextSize, chatRandomSeed, embedderRandomSeed;
        private string chatModelPath, clipModelPath, embedderModelPath;

        public override void _EnterTree() { }

        public override void _Ready()
        {
            InitializeDefaultValues();
            InitializeNodeRefs();
            InitializeConfigList();
            InitializeUiElements();
            InitializeSignals();
        }

        private void InitializeConfigList()
        {
            ConfigListResource = GD.Load<ConfigListResource>(configListResourcePath) ?? new ConfigListResource();
            UpdateUIFromLoadedConfigs();
        }

        private void UpdateUIFromLoadedConfigs()
        {
            savedConfigsItemList.Clear();
            foreach (var config in ConfigListResource.ModelConfigurations)
            {
                savedConfigsItemList.AddItem(config.ModelConfigName);
            }
        }

        private void InitializeDefaultValues()
        {
            configName = "<default>";
            chatContextSize = 4000;
            chatGpuLayerCount = 33;
            chatRandomSeed = 0;
            chatModelPath = "";
            embedderContextSize = 4000;
            embedderGpuLayerCount = 33;
            embedderRandomSeed = 0;
            embedderModelPath = "";
            clipModelPath = "";
        }

        private void InitializeNodeRefs()
        {
            mindManager = GetNode<MindManager>("/root/MindManager");

            // Manage configs nodes
            configNameLineEdit = GetNode<LineEdit>("%ConfigNameLineEdit");
            savedConfigsItemList = GetNode<ItemList>("%SavedConfigsItemList");
            addNewConfigButton = GetNode<Button>("%AddNewConfigButton");
            deleteConfigButton = GetNode<Button>("%DeleteConfigButton");
            backButton = GetNode<Button>("%BackButton");
            loadConfigButton = GetNode<Button>("%LoadConfigButton");
            unloadConfigButton = GetNode<Button>("%UnloadConfigButton");
            autoloadLastGoodConfigCheckBox = GetNode<CheckBox>("%AutoloadLastGoodConfigCheckBox");

            // Chat param nodes
            chatContextSizeHSlider = GetNode<HSlider>("%ChatContextSizeHSlider");
            chatContextSizeLabel = GetNode<Label>("%ChatContextSizeLabel");
            chatGpuLayerCountHSlider = GetNode<HSlider>("%ChatModelGpuLayerCountHSlider");
            chatGpuLayerCountLabel = GetNode<Label>("%ChatModelGpuLayerCountLabel");
            chatRandomSeedLineEdit = GetNode<LineEdit>("%ChatRandomSeedLineEdit");

            // Chat path nodes
            selectChatPathButton = GetNode<Button>("%SelectChatPathButton");
            clearChatPathButton = GetNode<Button>("%ClearChatPathButton");
            selectChatPathFileDialog = GetNode<FileDialog>("%SelectChatPathFileDialog");
            chatCurrentModelPathLabel = GetNode<Label>("%ChatCurrentModelPathLabel");

            // Embedder param nodes
            embedderContextSizeHSlider = GetNode<HSlider>("%EmbedderContextSizeHSlider");
            embedderContextSizeLabel = GetNode<Label>("%EmbedderContextSizeLabel");
            embedderGpuLayerCountHSlider = GetNode<HSlider>("%EmbedderGpuLayerCountHSlider");
            embedderGpuLayerCountLabel = GetNode<Label>("%EmbedderGpuLayerCountLabel");
            embedderRandomSeedLineEdit = GetNode<LineEdit>("%EmbedderRandomSeedLineEdit");

            // Embedder path nodes
            selectEmbedderPathButton = GetNode<Button>("%SelectEmbedderPathButton");
            clearEmbedderPathButton = GetNode<Button>("%ClearEmbedderPathButton");
            selectEmbedderPathFileDialog = GetNode<FileDialog>("%SelectEmbedderPathFileDialog");
            embedderCurrentModelPathLabel = GetNode<Label>("%EmbedderCurrentModelPathLabel");

            // Clip path nodes
            selectClipPathButton = GetNode<Button>("%SelectClipPathButton");
            clearClipPathButton = GetNode<Button>("%ClearClipPathButton");
            selectClipPathFileDialog = GetNode<FileDialog>("%SelectClipPathFileDialog");
            clipCurrentModelPathLabel = GetNode<Label>("%ClipCurrentModelPathLabel");
        }

        private void InitializeSignals()
        {
            // Chat signals
            addNewConfigButton.Pressed += OnAddNewConfigPressed;
            deleteConfigButton.Pressed += OnDeleteConfigPressed;
            backButton.Pressed += OnBackPressed;
            loadConfigButton.Pressed += OnLoadConfigPressed;
            unloadConfigButton.Pressed += OnUnloadConfigPressed;
            autoloadLastGoodConfigCheckBox.Toggled += OnAutoloadLastGoodConfigToggled;

            clearChatPathButton.Pressed += OnClearChatPathPressed;
            clearClipPathButton.Pressed += OnClearClipPathPressed;
            clearEmbedderPathButton.Pressed += OnClearEmbedderPathPressed;

            selectChatPathButton.Pressed += OnSelectChatPathPressed;
            selectClipPathButton.Pressed += OnSelectClipPathPressed;
            selectEmbedderPathButton.Pressed += OnSelectEmbedderPathPressed;

            chatContextSizeHSlider.ValueChanged += OnChatContextSizeHSliderValueChanged;
            chatGpuLayerCountHSlider.ValueChanged += OnChatGpuLayerCountHSliderValueChanged;
            embedderContextSizeHSlider.ValueChanged += OnEmbedderContextSizeHSliderValueChanged;
            embedderGpuLayerCountHSlider.ValueChanged += OnEmbedderGpuLayerCountHSliderValueChanged;

            selectChatPathFileDialog.FileSelected += OnChatPathSelected;
            selectClipPathFileDialog.FileSelected += OnClipPathSelected;
            selectEmbedderPathFileDialog.FileSelected += OnEmbedderPathSelected;

            configNameLineEdit.TextChanged += OnConfigNameTextChanged;
            savedConfigsItemList.ItemSelected += OnSavedConfigsItemSelected;
        }

        private void OnAutoloadLastGoodConfigToggled(bool toggledOn)
        {
            if (ConfigListResource != null)
            {
                ConfigListResource.AutoloadLastGoodModelConfig = toggledOn;
                SaveConfigList();
            }
        }

        public void AutoloadLastGoodConfig()
        {
            if (ConfigListResource != null && ConfigListResource.AutoloadLastGoodModelConfig && ConfigListResource.LastGoodModelConfig != null)
            {
                ModelParamsConfig = ConfigListResource.LastGoodModelConfig;
                LoadConfig(ModelParamsConfig);
                OnLoadConfigPressed();
            }
        }

        private void LoadConfig(ModelParamsConfig config)
        {
            configNameLineEdit.Text = config.ModelConfigName;
            chatContextSizeHSlider.Value = CalculateLogContextSize(config.ChatContextSize);
            chatGpuLayerCountHSlider.Value = config.ChatGpuLayerCount;
            chatRandomSeedLineEdit.Text = config.ChatRandomSeed.ToString();
            chatCurrentModelPathLabel.Text = config.ChatModelPath;
            embedderContextSizeHSlider.Value = CalculateLogContextSize(config.EmbedderContextSize);
            embedderGpuLayerCountHSlider.Value = config.EmbedderGpuLayerCount;
            embedderRandomSeedLineEdit.Text = config.EmbedderRandomSeed.ToString();
            embedderCurrentModelPathLabel.Text = config.EmbedderModelPath;
            clipCurrentModelPathLabel.Text = config.ClipModelPath;
        }

        private async void OnUnloadConfigPressed()
        {
            await mindManager.DisposeExecutorAsync();
        }

        private async void OnLoadConfigPressed()
        {
            if (ModelParamsConfig != null)
            {
                await mindManager.InitializeAsync(ModelParamsConfig);
                ConfigListResource.LastGoodModelConfig = ModelParamsConfig;
                SaveConfigList();
            }
        }

        private void OnBackPressed()
        {
            Visible = false;
        }

        private void InitializeUiElements()
        {
            configNameLineEdit.Text = configName;

            // Initialize chat sliders and labels
            InitializeSliderAndLabel(chatGpuLayerCountHSlider, chatGpuLayerCountLabel, chatGpuLayerCount);
            InitializeSliderAndLabel(chatContextSizeHSlider, chatContextSizeLabel, chatContextSize, CalculateLogContextSize);

            chatRandomSeedLineEdit.Text = chatRandomSeed.ToString();

            // Initialize embedder sliders and labels
            InitializeSliderAndLabel(embedderGpuLayerCountHSlider, embedderGpuLayerCountLabel, embedderGpuLayerCount);
            InitializeSliderAndLabel(embedderContextSizeHSlider, embedderContextSizeLabel, embedderContextSize, CalculateLogContextSize);

            embedderRandomSeedLineEdit.Text = embedderRandomSeed.ToString();

            // Initialize autoload checkbox
            if (ConfigListResource != null)
            {
                autoloadLastGoodConfigCheckBox.ButtonPressed = ConfigListResource.AutoloadLastGoodModelConfig;
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
            ModelParamsConfig = ConfigListResource.ModelConfigurations[(int)index];
            LoadConfig(ModelParamsConfig);
        }

        private void OnDeleteConfigPressed()
        {
            var selectedIndices = savedConfigsItemList.GetSelectedItems();

            if (selectedIndices.Length > 0 && ConfigListResource.ModelConfigurations.Count > 0)
            {
                int selectedIndex = selectedIndices[0];
                if (selectedIndex >= 0 && selectedIndex < ConfigListResource.ModelConfigurations.Count)
                {
                    ConfigListResource.ModelConfigurations.RemoveAt(selectedIndex);
                    SaveConfigList();
                    UpdateUIFromLoadedConfigs();
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
                ModelConfigName = configName,
                ChatContextSize = chatContextSize,
                ChatGpuLayerCount = chatGpuLayerCount,
                ChatRandomSeed = chatRandomSeed,
                ChatModelPath = chatModelPath,
                EmbedderContextSize = embedderContextSize,
                EmbedderGpuLayerCount = embedderGpuLayerCount,
                EmbedderRandomSeed = embedderRandomSeed,
                EmbedderModelPath = embedderModelPath,
                ClipModelPath = clipModelPath
            };

            ConfigListResource.ModelConfigurations.Add(newConfig);
            SaveConfigList();
            UpdateUIFromLoadedConfigs();
        }

        private void UpdateConfigurationValue(Action<ModelParamsConfig> updateAction)
        {
            var selectedIndices = savedConfigsItemList.GetSelectedItems();
            if (selectedIndices.Length > 0)
            {
                int selectedIndex = selectedIndices[0];
                if (selectedIndex >= 0 && selectedIndex < ConfigListResource.ModelConfigurations.Count)
                {
                    var config = ConfigListResource.ModelConfigurations[selectedIndex];
                    updateAction(config);
                    SaveConfigList();
                }
            }
        }

        private void SaveConfigList()
        {
            Error saveError = ResourceSaver.Save(ConfigListResource, configListResourcePath);
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

        private void OnClearEmbedderPathPressed() => ClearPath(() => embedderModelPath = "", embedderCurrentModelPathLabel, (config, path) => config.EmbedderModelPath = path);
        private void OnClearClipPathPressed() => ClearPath(() => clipModelPath = "", clipCurrentModelPathLabel, (config, path) => config.ClipModelPath = path);
        private void OnClearChatPathPressed() => ClearPath(() => chatModelPath = "", chatCurrentModelPathLabel, (config, path) => config.ChatModelPath = path);

        private void OnConfigNameTextChanged(string newText)
        {
            configName = newText;
            UpdateConfigurationValue(config =>
            {
                config.ModelConfigName = configName;
                savedConfigsItemList.SetItemText(savedConfigsItemList.GetSelectedItems()[0], configName);
            });
        }

        private void OnChatGpuLayerCountHSliderValueChanged(double value) => UpdateSliderValue((int)value, chatGpuLayerCountLabel, v => chatGpuLayerCount = v, config => config.ChatGpuLayerCount = chatGpuLayerCount);
        private void OnEmbedderGpuLayerCountHSliderValueChanged(double value) => UpdateSliderValue((int)value, embedderGpuLayerCountLabel, v => embedderGpuLayerCount = v, config => config.EmbedderGpuLayerCount = embedderGpuLayerCount);
        private void OnChatContextSizeHSliderValueChanged(double value) => UpdateSliderValue(CalculateExpContextSize(value), chatContextSizeLabel, v => chatContextSize = v, config => config.ChatContextSize = chatContextSize);
        private void OnEmbedderContextSizeHSliderValueChanged(double value) => UpdateSliderValue(CalculateExpContextSize(value), embedderContextSizeLabel, v => embedderContextSize = v, config => config.EmbedderContextSize = embedderContextSize);

        private void UpdateSliderValue<T>(T value, Label label, Action<T> setValue, Action<ModelParamsConfig> updateConfig)
        {
            setValue(value);
            label.Text = value.ToString();
            UpdateConfigurationValue(updateConfig);
        }

        private void OnSelectChatPathPressed() => selectChatPathFileDialog.PopupCentered();
        private void OnChatPathSelected(string path) => UpdatePath(() => chatModelPath = path, chatCurrentModelPathLabel, config => config.ChatModelPath = chatModelPath);
        private void OnSelectClipPathPressed() => selectClipPathFileDialog.PopupCentered();
        private void OnClipPathSelected(string path) => UpdatePath(() => clipModelPath = path, clipCurrentModelPathLabel, config => config.ClipModelPath = clipModelPath);
        private void OnSelectEmbedderPathPressed() => selectEmbedderPathFileDialog.PopupCentered();
        private void OnEmbedderPathSelected(string path) => UpdatePath(() => embedderModelPath = path, embedderCurrentModelPathLabel, config => config.EmbedderModelPath = embedderModelPath);

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
