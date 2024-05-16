using Godot;
using System;
using System.Linq;

namespace MindGame
{
    [Tool]
    public partial class ModelConfigController : Control
    {
        public ConfigListResource configListResource;
        public ModelParamsConfigs modelParamsConfigs;
        public MindManager mindManager;
        private string modelConfigListPath = "res://addons/mind_game/model_configs.tres";

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
            InitializeUIElements();
            InitializeConfigList();
            InitializeSignals();
        }

        private void InitializeConfigList()
        {
            configListResource = GD.Load<ConfigListResource>(modelConfigListPath);
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
                savedConfigsItemList.AddItem(config.ModelConfigsName);
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
            if (modelParamsConfigs != null)
            {
                await mindManager.InitializeAsync(modelParamsConfigs);
            }
        }

        private void OnBackPressed()
        {
            Visible = false;
        }

        private void InitializeUIElements()
        {
            configNameLineEdit.Text = configName;

            // Initialize chat sliders and labels
            chatGpuLayerCountHSlider.Value = (double)chatGpuLayerCount;
            chatGpuLayerCountLabel.Text = chatGpuLayerCount.ToString();

            chatContextSizeHSlider.Value = calculateLogContextSize(chatContextSize);
            chatContextSizeLabel.Text = chatContextSize.ToString();

            chatRandomSeedLineEdit.Text = chatRandomSeed.ToString();

            // Initialize embedder sliders and labels
            embedderGpuLayerCountHSlider.Value = (double)embedderGpuLayerCount;
            embedderGpuLayerCountLabel.Text = embedderGpuLayerCount.ToString();

            embedderContextSizeHSlider.Value = calculateLogContextSize(embedderContextSize);
            embedderContextSizeLabel.Text = embedderContextSize.ToString();

            embedderRandomSeedLineEdit.Text = embedderRandomSeed.ToString();
        }

        private void OnSavedConfigsItemSelected(long index)
        {
            modelParamsConfigs = configListResource.ModelConfigurations[(int)index];
            configNameLineEdit.Text = modelParamsConfigs.ModelConfigsName;
            chatContextSizeHSlider.Value = calculateLogContextSize(modelParamsConfigs.ChatContextSize);
            chatGpuLayerCountHSlider.Value = modelParamsConfigs.ChatGpuLayerCount;
            chatRandomSeedLineEdit.Text = modelParamsConfigs.ChatRandomSeed.ToString();
            chatCurrentModelPathLabel.Text = modelParamsConfigs.ChatModelPath;
            embedderContextSizeHSlider.Value = calculateLogContextSize(modelParamsConfigs.EmbedderContextSize);
            embedderGpuLayerCountHSlider.Value = modelParamsConfigs.EmbedderGpuLayerCount;
            embedderRandomSeedLineEdit.Text = modelParamsConfigs.EmbedderRandomSeed.ToString();
            embedderCurrentModelPathLabel.Text = modelParamsConfigs.EmbedderModelPath;
            clipCurrentModelPathLabel.Text = modelParamsConfigs.ClipModelPath;
        }


        private void OnDeleteConfigPressed()
        {
            var selectedIndices = savedConfigsItemList.GetSelectedItems();

            // Check if there is at least one selected item and the array is not empty
            if (selectedIndices.Count() > 0 && configListResource.ModelConfigurations.Count() > 0)
            {
                int selectedIndex = selectedIndices[0];  // Get the first selected index

                // Ensure the selected index is within the bounds of the array
                if (selectedIndex >= 0 && selectedIndex < configListResource.ModelConfigurations.Count())
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
            ModelParamsConfigs newConfig = new ModelParamsConfigs
            {
                ModelConfigsName = configName,
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

        private void UpdateConfigurationValue(Action<ModelParamsConfigs> updateAction)
        {
            var selectedIndices = savedConfigsItemList.GetSelectedItems();
            if (selectedIndices.Count() > 0)
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
            Error saveError = ResourceSaver.Save(configListResource, modelConfigListPath);
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
                config.ModelConfigsName = configName;
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

        private uint calculateExpContextSize(double value)
        {
            return (uint)Math.Pow(2, value) * 1000;
        }

        private double calculateLogContextSize(uint value)
        {
            return (double)Math.Log2(value / 1000);
        }

        private void OnChatContextSizeHSliderValueChanged(double value)
        {
            chatContextSize = calculateExpContextSize(value);
            chatContextSizeLabel.Text = chatContextSize.ToString();
            UpdateConfigurationValue(config => config.ChatContextSize = chatContextSize);
        }

        private void OnEmbedderContextSizeHSliderValueChanged(double value)
        {
            embedderContextSize = calculateExpContextSize(value);
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