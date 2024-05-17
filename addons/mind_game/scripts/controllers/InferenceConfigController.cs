using Godot;
using System;

namespace MindGame
{
    [Tool]
    public partial class InferenceConfigController : Control
    {
        public Button addInferenceConfigButton, deleteInferenceConfigButton, backButton;
        public CheckBox outputJsonCheckBox;
        public HSlider maxTokensHSlider, temperatureHSlider;
        public LineEdit inferenceConfigNameLineEdit, maxTokensLineEdit, temperatureLineEdit;
        private ConfigListResource ConfigListResource;
        private InferenceParamsConfig InferenceParamsConfig;
        private string configListResourcePath = "res://addons/mind_game/assets/resources/custom_resources/ConfigListResource.tres";


        private string inferenceConfigName;
        private string[] antiPrompts;
        private int maxTokens;
        private float temperature;
        private bool outputJson;
        private ItemList savedInferenceConfigsItemList;

        public override void _Ready()
        {
            InitializeDefaultValues();
            InitializeNodeRefs();
            InitializeSignals();
            InitializeConfigList();

        }

        private void InitializeConfigList()
        {
            ConfigListResource = GD.Load<ConfigListResource>(configListResourcePath);
            if (ConfigListResource != null)
            {
                UpdateUIFromLoadedConfigs();
            }
            else
            {
                ConfigListResource = new ConfigListResource();
            }
        }

        private void InitializeDefaultValues()
        {
            inferenceConfigName = "<default>";
            maxTokens = 4000;
            antiPrompts = ["<|eot_id|>", "<|end_of_text|>", "<|user|>", "<|end|>", "user:", "User:", "USER:", "\nUser:", "\nUSER:", "}"];
            temperature = 0.5f;
            outputJson = false;
        }

        private void InitializeSignals()
        {
            outputJsonCheckBox.Toggled += OnOutputJsonToggled;
            addInferenceConfigButton.Pressed += OnAddInferenceConfigPressed;
            deleteInferenceConfigButton.Pressed += OnDeleteInferenceConfigPressed;
            backButton.Pressed += OnBackPressed;
        }

        private void OnDeleteInferenceConfigPressed()
        {
            throw new NotImplementedException();
        }

        private void OnAddInferenceConfigPressed()
        {
            InferenceParamsConfig newConfig = new()
            {
                InferenceConfigName = inferenceConfigName,
                MaxTokens = maxTokens,
                AntiPrompts = antiPrompts,
                Temperature = temperature,
                OutputJson = outputJson
            };

            ConfigListResource.InferenceConfigurations.Add(newConfig);
            SaveConfigList();
            UpdateUIFromLoadedConfigs();


        }

        private void UpdateUIFromLoadedConfigs()
        {
            savedInferenceConfigsItemList.Clear();
            foreach (var config in ConfigListResource.InferenceConfigurations)
            {
                savedInferenceConfigsItemList.AddItem(config.InferenceConfigName);
            }
        }

        private void UpdateConfigurationValue(Action<InferenceParamsConfig> updateAction)
        {
            var selectedIndices = savedInferenceConfigsItemList.GetSelectedItems();
            if (selectedIndices.Length > 0)
            {
                int selectedIndex = selectedIndices[0];
                if (selectedIndex >= 0 && selectedIndex < ConfigListResource.InferenceConfigurations.Count)
                {
                    var config = ConfigListResource.InferenceConfigurations[selectedIndex];
                    updateAction(config);
                    SaveConfigList();
                }
            }
        }

        private void OnBackPressed()
        {
            Visible = false;
        }

        private void SaveConfigList()
        {
            Error saveError = ResourceSaver.Save(ConfigListResource, configListResourcePath);
            if (saveError != Error.Ok)
            {
                GD.PrintErr("Failed to save configuration list: ", saveError);
            }
            else
            {
                // GD.Print("Configuration saved successfully.");
            }
        }

        private void OnOutputJsonToggled(bool toggledOn)
        {
            outputJson = toggledOn;
        }

        private void OnSavedConfigsItemSelected(long index)
        {
            InferenceParamsConfig = ConfigListResource.InferenceConfigurations[(int)index];
            inferenceConfigNameLineEdit.Text = InferenceParamsConfig.InferenceConfigName;
            maxTokensHSlider.Value = CalculateLogMaxTokens((uint)InferenceParamsConfig.MaxTokens);
            temperatureHSlider.Value = InferenceParamsConfig.Temperature;
            outputJsonCheckBox.ButtonPressed = InferenceParamsConfig.OutputJson;
        }

        private void InitializeNodeRefs()
        {
            addInferenceConfigButton = GetNode<Button>("%AddInferenceConfigButton");
            deleteInferenceConfigButton = GetNode<Button>("%DeleteInferenceConfigButton");
            backButton = GetNode<Button>("%BackButton");

            outputJsonCheckBox = GetNode<CheckBox>("%OutputJsonCheckBox");
            maxTokensHSlider = GetNode<HSlider>("%MaxTokensHSlider");
            temperatureHSlider = GetNode<HSlider>("%TemperatureHSlider");
            savedInferenceConfigsItemList = GetNode<ItemList>("%SavedInferenceConfigsItemList");

            inferenceConfigNameLineEdit = GetNode<LineEdit>("%NameLineEdit");
            maxTokensLineEdit = GetNode<LineEdit>("%MaxTokensLineEdit");
            temperatureLineEdit = GetNode<LineEdit>("%TemperatureLineEdit");
        }

        private static uint CalculateExpMaxTokens(double value)
        {
            return (uint)Math.Pow(2, value) * 1000;
        }

        private static double CalculateLogMaxTokens(uint value)
        {
            return (double)Math.Log2(value / 1000);
        }
    }
}

    
