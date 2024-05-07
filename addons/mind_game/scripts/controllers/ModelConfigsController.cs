using Godot;
using System;
using System.Collections.Generic;
using static System.Collections.Specialized.BitVector32;
using System.IO;
using System.Linq;

namespace MindGame
{
    [Tool]
    public partial class ModelConfigsController : Control
    {
        private ConfigListResource configListResource;
        private string modelConfigListPath = "res://addons/mind_game/model_configs.tres";

        // UI elements
        private Button addNewConfigButton, deleteConfigButton, selectChatPathButton, clearChatPathButton, selectEmbedderPathButton, clearEmbedderPathButton, selectClipPathButton, clearClipPathButton;
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
            InitializeSignals();
            InitializeUIElements();
            InitializeConfigList();
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
            foreach (var config in configListResource.Configurations)
            {
                savedConfigsItemList.AddItem(config.ModelConfigsName);
            }
        }

        private void InitializeDefaultValues()
        {
            configName = "";

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

            // Manage configs nodes
            configNameLineEdit = GetNode<LineEdit>("%ConfigNameLineEdit");
            savedConfigsItemList = GetNode<ItemList>("%SavedConfigsItemList");
            addNewConfigButton = GetNode<Button>("%AddNewConfigButton");
            deleteConfigButton = GetNode<Button>("%DeleteConfigButton");

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
            var config = configListResource.Configurations[(int)index];
            configNameLineEdit.Text = config.ModelConfigsName;
            chatContextSizeHSlider.Value = calculateLogContextSize(config.ChatContextSize);
            chatGpuLayerCountHSlider.Value = config.ChatGpuLayerCount;
            chatRandomSeedLineEdit.Text = config.ChatRandomSeed.ToString();
            chatCurrentModelPathLabel.Text = config.ChatModelPath;
            embedderContextSizeHSlider.Value = calculateLogContextSize(config.EmbedderContextSize);
            embedderGpuLayerCountHSlider.Value = config.EmbedderGpuLayerCount;
            embedderRandomSeedLineEdit.Text = config.EmbedderRandomSeed.ToString();
            embedderCurrentModelPathLabel.Text = config.EmbedderModelPath;
            clipCurrentModelPathLabel.Text = config.ClipModelPath;
        }


        private void OnDeleteConfigPressed()
        {
            var selectedIndices = savedConfigsItemList.GetSelectedItems();

            // Check if there is at least one selected item and the array is not empty
            if (selectedIndices.Count() > 0 && configListResource.Configurations.Count() > 0)
            {
                int selectedIndex = selectedIndices[0];  // Get the first selected index

                // Ensure the selected index is within the bounds of the array
                if (selectedIndex >= 0 && selectedIndex < configListResource.Configurations.Count())
                {
                    configListResource.Configurations.RemoveAt(selectedIndex);
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
            ModelConfigsParams newConfig = new ModelConfigsParams
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

            configListResource.Configurations.Add(newConfig);
            SaveConfigList();
            UpdateUIFromLoadedConfigs();
        }

        private void UpdateConfigurationValue(Action<ModelConfigsParams> updateAction)
        {
            var selectedIndices = savedConfigsItemList.GetSelectedItems();
            if (selectedIndices.Count() > 0)
            {
                int selectedIndex = selectedIndices[0];
                if (selectedIndex >= 0 && selectedIndex < configListResource.Configurations.Count)
                {
                    var config = configListResource.Configurations[selectedIndex];
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
                GD.Print("Configuration saved successfully.");
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
            addNewConfigButton.Pressed -= OnAddNewConfigPressed;
            deleteConfigButton.Pressed -= OnDeleteConfigPressed;

            clearChatPathButton.Pressed -= OnClearChatPathPressed;
            clearClipPathButton.Pressed -= OnClearClipPathPressed;
            clearEmbedderPathButton.Pressed -= OnClearEmbedderPathPressed;

            selectChatPathButton.Pressed -= OnSelectChatPathPressed;
            selectClipPathButton.Pressed -= OnSelectClipPathPressed;
            selectEmbedderPathButton.Pressed -= OnSelectEmbedderPathPressed;

            chatContextSizeHSlider.ValueChanged -= OnChatContextSizeHSliderValueChanged;
            chatGpuLayerCountHSlider.ValueChanged -= OnChatGpuLayerCountHSliderValueChanged;
            embedderContextSizeHSlider.ValueChanged -= OnEmbedderContextSizeHSliderValueChanged;
            embedderGpuLayerCountHSlider.ValueChanged -= OnEmbedderGpuLayerCountHSliderValueChanged;

            selectChatPathFileDialog.FileSelected -= OnChatPathSelected;
            selectClipPathFileDialog.FileSelected -= OnClipPathSelected;
            selectEmbedderPathFileDialog.FileSelected -= OnEmbedderPathSelected;

            configNameLineEdit.TextChanged -= OnConfigNameTextChanged;
            savedConfigsItemList.ItemSelected -= OnSavedConfigsItemSelected;
        }
    }

}