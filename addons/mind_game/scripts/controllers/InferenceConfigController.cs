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
            InitializeUiElements();
            InitializeSignals();
            InitializeConfigList();

        }

        private void InitializeUiElements()
        {
            inferenceConfigNameLineEdit.Text = inferenceConfigName;
            maxTokensHSlider.Value = CalculateLogMaxTokens((uint)maxTokens);
            maxTokensLineEdit.Text = maxTokens.ToString();
            temperatureHSlider.Value = (double)temperature;
            temperatureLineEdit.Text = temperature.ToString();
            outputJsonCheckBox.ButtonPressed = outputJson;
        }

        private void InitializeConfigList()
        {
            ConfigListResource = GD.Load<ConfigListResource>(configListResourcePath);
            if (ConfigListResource != null)
            {
                UpdateUiFromLoadedConfigs();
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
            // Config management and UI navigation
            savedInferenceConfigsItemList.ItemSelected += OnSavedInferenceConfigSelected;
            addInferenceConfigButton.Pressed += OnAddInferenceConfigPressed;
            deleteInferenceConfigButton.Pressed += OnDeleteInferenceConfigPressed;
            backButton.Pressed += OnBackPressed;

            // Inference config parameters
            inferenceConfigNameLineEdit.TextChanged += OnInferenceConfigNameTextChanged; 
            maxTokensHSlider.ValueChanged += OnMaxTokensValueChanged;
            temperatureHSlider.ValueChanged += OnTemperatureValueChanged;
            outputJsonCheckBox.Toggled += OnOutputJsonToggled;
        }

        private void OnSavedInferenceConfigSelected(long index)
        {
            if (index >= 0 && index < ConfigListResource.InferenceConfigurations.Count)
            {
                InferenceParamsConfig = ConfigListResource.InferenceConfigurations[(int)index];
                inferenceConfigNameLineEdit.Text = InferenceParamsConfig.InferenceConfigName;
                temperatureHSlider.Value = (double)InferenceParamsConfig.Temperature;
                temperatureLineEdit.Text = InferenceParamsConfig.Temperature.ToString();
                maxTokensHSlider.Value = CalculateLogMaxTokens((uint)InferenceParamsConfig.MaxTokens);
                maxTokensLineEdit.Text = InferenceParamsConfig.MaxTokens.ToString();
                outputJsonCheckBox.ButtonPressed = InferenceParamsConfig.OutputJson;
            }
        }


        private void OnTemperatureValueChanged(double value)
        {
            temperature = (float)value;
            temperatureLineEdit.Text = temperature.ToString();
            UpdateConfigurationValue(config => config.Temperature = temperature);
        }

        private void OnMaxTokensValueChanged(double value)
        {
            maxTokens = (int)CalculateExpMaxTokens(value);
            maxTokensLineEdit.Text = maxTokens.ToString();
            UpdateConfigurationValue(config => config.MaxTokens = maxTokens);

        }

        private void OnInferenceConfigNameTextChanged(string newText)
        {
            inferenceConfigName = newText;
            UpdateConfigurationValue(config =>
            {
                config.InferenceConfigName = inferenceConfigName;
                savedInferenceConfigsItemList.SetItemText(savedInferenceConfigsItemList.GetSelectedItems()[0], inferenceConfigName);
            });
        }

        private void OnDeleteInferenceConfigPressed()
        {
            var selectedIndices = savedInferenceConfigsItemList.GetSelectedItems();

            if (selectedIndices.Length > 0 && ConfigListResource.InferenceConfigurations.Count > 0)
            {
                int selectedIndex = selectedIndices[0];

                if (selectedIndex >= 0 && selectedIndex < ConfigListResource.InferenceConfigurations.Count)
                {
                    ConfigListResource.InferenceConfigurations.RemoveAt(selectedIndex);
                    SaveConfigList();
                    UpdateUiFromLoadedConfigs();
                }
            }
            else
            {
                GD.Print("No configuration selected or list is empty.");
            }
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
            UpdateUiFromLoadedConfigs();
        }

        private void UpdateUiFromLoadedConfigs()
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
            UpdateConfigurationValue(config => config.OutputJson = outputJson);
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

    
