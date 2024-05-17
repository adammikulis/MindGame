using Godot;
using System;
using System.Linq;

namespace MindGame
{
    [Tool]
    public partial class ModelConfigController : Control
    {
        public ConfigListResource configListResource;
        public ModelParamsConfig modelParamsConfig;
        public MindManager mindManager;
        private string configListResourcePath = "res://addons/mind_game/assets/resources/custom_resources/ConfigListResource.tres";

        // UI elements
        private Button addNewConfigButton, deleteConfigButton, selectChatPathButton, clearChatPathButton, selectEmbedderPathButton, clearEmbedderPathButton, selectClipPathButton, clearClipPathButton, backButton, loadConfigButton, unloadConfigButton;
        private Label chatContextSizeLabel, chatGpuLayerCountLabel, embedderContextSizeLabel, embedderGpuLayerCountLabel, chatCurrentModelPathLabel, embedderCurrentModelPathLabel, clipCurrentModelPathLabel;
        private FileDialog selectChatPathFileDialog, selectClipPathFileDialog, selectEmbedderPathFileDialog;
        private HSlider chatContextSizeHSlider, chatGpuLayerCountHSlider, embedderContextSizeHSlider, embedderGpuLayerCountHSlider;
        private LineEdit configNameLineEdit, chatRandomSeedLineEdit, embedderRandomSeedLineEdit;
        private ItemList savedConfigsItemList;

        // Model params
        private string configName;
        private int chatGpuLayerCount, embedderGpuLayerCount;
        private double chatContextSizeSliderValue, embedderContextSizeSliderValue;
        private uint chatContextSize, embedderContextSize, chatRandomSeed, embedderRandomSeed;
        private string chatModelPath, clipModelPath, embedderModelPath;

        public override void _EnterTree()
        {

        }

        public override void _Ready()
        {
            InitializeDefaultValues();
            InitializeNodeRefs();
            InitializeUiElements();
            InitializeConfigList();
            InitializeSignals();
        }

        private void InitializeConfigList()
        {
            configListResource = GD.Load<ConfigListResource>(configListResourcePath);
            if (configListResource != null)
            {
                UpdateUIFromLoadedConfigs();
            }
            else
            {
                configListResource = new ConfigListResource();
            }
        }

        private void UpdateUIFromLoadedConfigs()
        {
            savedConfigsItemList.Clear();
            foreach (var config in configListResource.ModelConfigurations)
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

        private async void OnUnloadConfigPressed()
        {
            await mindManager.DisposeExecutorAsync();
        }

        private async void OnLoadConfigPressed()
        {
            if (modelParamsConfig != null)
            {
                await mindManager.InitializeAsync(modelParamsConfig);
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
            chatGpuLayerCountHSlider.Value = (double)chatGpuLayerCount;
            chatGpuLayerCountLabel.Text = chatGpuLayerCount.ToString();

            chatContextSizeHSlider.Value = CalculateLogContextSize(chatContextSize);
            chatContextSizeLabel.Text = chatContextSize.ToString();

            chatRandomSeedLineEdit.Text = chatRandomSeed.ToString();

            // Initialize embedder sliders and labels
            embedderGpuLayerCountHSlider.Value = (double)embedderGpuLayerCount;
            embedderGpuLayerCountLabel.Text = embedderGpuLayerCount.ToString();

            embedderContextSizeHSlider.Value = CalculateLogContextSize(embedderContextSize);
            embedderContextSizeLabel.Text = embedderContextSize.ToString();

            embedderRandomSeedLineEdit.Text = embedderRandomSeed.ToString();
        }

        private void OnSavedConfigsItemSelected(long index)
        {
            modelParamsConfig = configListResource.ModelConfigurations[(int)index];
            configNameLineEdit.Text = modelParamsConfig.ModelConfigName;
            chatContextSizeHSlider.Value = CalculateLogContextSize(modelParamsConfig.ChatContextSize);
            chatGpuLayerCountHSlider.Value = modelParamsConfig.ChatGpuLayerCount;
            chatRandomSeedLineEdit.Text = modelParamsConfig.ChatRandomSeed.ToString();
            chatCurrentModelPathLabel.Text = modelParamsConfig.ChatModelPath;
            embedderContextSizeHSlider.Value = CalculateLogContextSize(modelParamsConfig.EmbedderContextSize);
            embedderGpuLayerCountHSlider.Value = modelParamsConfig.EmbedderGpuLayerCount;
            embedderRandomSeedLineEdit.Text = modelParamsConfig.EmbedderRandomSeed.ToString();
            embedderCurrentModelPathLabel.Text = modelParamsConfig.EmbedderModelPath;
            clipCurrentModelPathLabel.Text = modelParamsConfig.ClipModelPath;
        }


        private void OnDeleteConfigPressed()
        {
            var selectedIndices = savedConfigsItemList.GetSelectedItems();

            if (selectedIndices.Length > 0 && configListResource.ModelConfigurations.Count > 0)
            {
                int selectedIndex = selectedIndices[0];

                if (selectedIndex >= 0 && selectedIndex < configListResource.ModelConfigurations.Count)
                {
                    configListResource.ModelConfigurations.RemoveAt(selectedIndex);
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

            configListResource.ModelConfigurations.Add(newConfig);
            SaveConfigList();
            UpdateUIFromLoadedConfigs();
        }

        private void UpdateConfigurationValue(Action<ModelParamsConfig> updateAction)
        {
            var selectedIndices = savedConfigsItemList.GetSelectedItems();
            if (selectedIndices.Length > 0)
            {
                int selectedIndex = selectedIndices[0];
                if (selectedIndex >= 0 && selectedIndex < configListResource.ModelConfigurations.Count)
                {
                    var config = configListResource.ModelConfigurations[selectedIndex];
                    updateAction(config);
                    SaveConfigList();
                }
            }
        }


        private void SaveConfigList()
        {
            Error saveError = ResourceSaver.Save(configListResource, configListResourcePath);
            if (saveError != Error.Ok)
            {
                GD.PrintErr("Failed to save configuration list: ", saveError);
            }
            else
            {
                // GD.Print("Configuration saved successfully.");
            }
        }


        private void OnClearEmbedderPathPressed()
        {
            embedderModelPath = "";
            embedderCurrentModelPathLabel.Text = embedderModelPath;
            UpdateConfigurationValue(config => config.EmbedderModelPath = embedderModelPath);
        }

        private void OnClearClipPathPressed()
        {
            clipModelPath = "";
            clipCurrentModelPathLabel.Text = clipModelPath;
            UpdateConfigurationValue(config => config.ClipModelPath = clipModelPath);
        }

        private void OnClearChatPathPressed()
        {
            chatModelPath = "";
            chatCurrentModelPathLabel.Text = chatModelPath;
            UpdateConfigurationValue(config => config.ChatModelPath = chatModelPath);
        }

        private void OnConfigNameTextChanged(string newText)
        {
            configName = newText;
            UpdateConfigurationValue(config =>
            {
                config.ModelConfigName = configName;
                savedConfigsItemList.SetItemText(savedConfigsItemList.GetSelectedItems()[0], configName);
            });
        }


        private void OnChatGpuLayerCountHSliderValueChanged(double value)
        {
            chatGpuLayerCount = (int)value;
            chatGpuLayerCountLabel.Text = chatGpuLayerCount.ToString();
            UpdateConfigurationValue(config => config.ChatGpuLayerCount = chatGpuLayerCount);
        }
 

        private void OnSelectChatPathPressed()
        {
            selectChatPathFileDialog.PopupCentered();
        }

        private void OnChatPathSelected(string path)
        {
            chatModelPath = path;
            chatCurrentModelPathLabel.Text = $"{path}";
            UpdateConfigurationValue(config => config.ChatModelPath = chatModelPath);
        }


        private void OnEmbedderGpuLayerCountHSliderValueChanged(double value)
        {
            embedderGpuLayerCount = (int)value;
            embedderGpuLayerCountLabel.Text = embedderGpuLayerCount.ToString();
            UpdateConfigurationValue(config => config.EmbedderGpuLayerCount = embedderGpuLayerCount);
        }

        private void OnClipPathSelected(string path)
        {
            clipModelPath = path;
            clipCurrentModelPathLabel.Text = $"{path}";
            UpdateConfigurationValue(config => config.ClipModelPath = clipModelPath);
        }

        private void OnEmbedderPathSelected(string path)
        {
            embedderModelPath = path;
            embedderCurrentModelPathLabel.Text = $"{path}";
            UpdateConfigurationValue(config => config.EmbedderModelPath = embedderModelPath);
        }

        private void OnSelectEmbedderPathPressed()
        {
            selectEmbedderPathFileDialog.PopupCentered();
        }

        private static uint CalculateExpContextSize(double value)
        {
            return (uint)Math.Pow(2, value) * 1000;
        }

        private static double CalculateLogContextSize(uint value)
        {
            return (double)Math.Log2(value / 1000);
        }

        private void OnChatContextSizeHSliderValueChanged(double value)
        {
            chatContextSize = CalculateExpContextSize(value);
            chatContextSizeLabel.Text = chatContextSize.ToString();
            UpdateConfigurationValue(config => config.ChatContextSize = chatContextSize);
        }

        private void OnEmbedderContextSizeHSliderValueChanged(double value)
        {
            embedderContextSize = CalculateExpContextSize(value);
            embedderContextSizeLabel.Text = embedderContextSize.ToString();
            UpdateConfigurationValue(config => config.EmbedderContextSize = embedderContextSize);
        }

        private void OnSelectClipPathPressed()
        {
            selectClipPathFileDialog.PopupCentered();
        }

        public override void _ExitTree()
        {
            //addNewConfigButton.Pressed -= OnAddNewConfigPressed;
            //deleteConfigButton.Pressed -= OnDeleteConfigPressed;

            //clearChatPathButton.Pressed -= OnClearChatPathPressed;
            //clearClipPathButton.Pressed -= OnClearClipPathPressed;
            //clearEmbedderPathButton.Pressed -= OnClearEmbedderPathPressed;

            //selectChatPathButton.Pressed -= OnSelectChatPathPressed;
            //selectClipPathButton.Pressed -= OnSelectClipPathPressed;
            //selectEmbedderPathButton.Pressed -= OnSelectEmbedderPathPressed;

            //chatContextSizeHSlider.ValueChanged -= OnChatContextSizeHSliderValueChanged;
            //chatGpuLayerCountHSlider.ValueChanged -= OnChatGpuLayerCountHSliderValueChanged;
            //embedderContextSizeHSlider.ValueChanged -= OnEmbedderContextSizeHSliderValueChanged;
            //embedderGpuLayerCountHSlider.ValueChanged -= OnEmbedderGpuLayerCountHSliderValueChanged;

            //selectChatPathFileDialog.FileSelected -= OnChatPathSelected;
            //selectClipPathFileDialog.FileSelected -= OnClipPathSelected;
            //selectEmbedderPathFileDialog.FileSelected -= OnEmbedderPathSelected;

            //configNameLineEdit.TextChanged -= OnConfigNameTextChanged;
            //savedConfigsItemList.ItemSelected -= OnSavedConfigsItemSelected;
        }
    }

}