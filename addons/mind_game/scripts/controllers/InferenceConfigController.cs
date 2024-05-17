using Godot;
using System;

namespace MindGame
{
    [Tool]
    public partial class InferenceConfigController : Control
    {
        public Button addInferenceConfigButton, deleteInferenceConfigButton, backButton;
        public CheckBox autoloadLastGoodConfigCheckBox, outputJsonCheckBox;
        public HSlider maxTokensHSlider, temperatureHSlider;
        public LineEdit inferenceConfigNameLineEdit, maxTokensLineEdit, temperatureLineEdit;
        private ConfigListResource ConfigListResource;
        private InferenceParamsConfig InferenceParamsConfig;
        private readonly string configListResourcePath = "res://addons/mind_game/assets/resources/custom_resources/ConfigListResource.tres";

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
            InitializeConfigList();
            InitializeUiElements();
            InitializeSignals();
        }

        private void InitializeUiElements()
        {
            inferenceConfigNameLineEdit.Text = inferenceConfigName;
            maxTokensHSlider.Value = CalculateLogMaxTokens((uint)maxTokens);
            maxTokensLineEdit.Text = maxTokens.ToString();
            temperatureHSlider.Value = (double)temperature;
            temperatureLineEdit.Text = temperature.ToString();
            outputJsonCheckBox.ButtonPressed = outputJson;
            autoloadLastGoodConfigCheckBox.ButtonPressed = ConfigListResource.AutoloadLastGoodInferenceConfig;
        }

        private void InitializeConfigList()
        {
            ConfigListResource = GD.Load<ConfigListResource>(configListResourcePath) ?? new ConfigListResource();
            UpdateUiFromLoadedConfigs();
        }

        private void InitializeDefaultValues()
        {
            inferenceConfigName = "<default>";
            maxTokens = 4000;
            antiPrompts = [ "<|eot_id|>", "<|end|>", "user:", "User:", "USER:", "\nUser:", "\nUSER:", "}" ];
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
            autoloadLastGoodConfigCheckBox.Toggled += OnAutoloadLastGoodConfigToggled;

            // Inference config parameters
            inferenceConfigNameLineEdit.TextChanged += OnInferenceConfigNameTextChanged;
            maxTokensHSlider.ValueChanged += OnMaxTokensValueChanged;
            temperatureHSlider.ValueChanged += OnTemperatureValueChanged;
            outputJsonCheckBox.Toggled += OnOutputJsonToggled;
        }

        private void OnAutoloadLastGoodConfigToggled(bool toggledOn)
        {
            ConfigListResource.AutoloadLastGoodInferenceConfig = toggledOn;
            SaveConfigList();
        }

        public void AutoloadLastGoodConfig()
        {
            if (ConfigListResource != null && ConfigListResource.AutoloadLastGoodInferenceConfig && ConfigListResource.LastGoodInferenceConfig != null)
            {
                InferenceParamsConfig = ConfigListResource.LastGoodInferenceConfig;
                LoadConfig(InferenceParamsConfig);
            }
        }

        private void LoadConfig(InferenceParamsConfig config)
        {
            inferenceConfigNameLineEdit.Text = config.InferenceConfigName;
            temperatureHSlider.Value = (double)config.Temperature;
            temperatureLineEdit.Text = config.Temperature.ToString();
            maxTokensHSlider.Value = CalculateLogMaxTokens((uint)config.MaxTokens);
            maxTokensLineEdit.Text = config.MaxTokens.ToString();
            outputJsonCheckBox.ButtonPressed = config.OutputJson;
        }

        private void OnSavedInferenceConfigSelected(long index)
        {
            if (index >= 0 && index < ConfigListResource.InferenceConfigurations.Count)
            {
                InferenceParamsConfig = ConfigListResource.InferenceConfigurations[(int)index];
                LoadConfig(InferenceParamsConfig);
            }
        }

        private void OnTemperatureValueChanged(double value)
        {
            UpdateSliderValue(value, temperatureLineEdit, v => temperature = (float)v, config => config.Temperature = temperature);
        }

        private void OnMaxTokensValueChanged(double value)
        {
            UpdateSliderValue(CalculateExpMaxTokens(value), maxTokensLineEdit, v => maxTokens = (int)v, config => config.MaxTokens = maxTokens);
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
            InferenceParamsConfig newConfig = new InferenceParamsConfig
            {
                InferenceConfigName = inferenceConfigName,
                MaxTokens = maxTokens,
                AntiPrompts = antiPrompts,
                Temperature = temperature,
                OutputJson = outputJson
            };

            ConfigListResource.InferenceConfigurations.Add(newConfig);
            ConfigListResource.CurrentInferenceConfig = newConfig;
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
        }

        private void OnOutputJsonToggled(bool toggledOn)
        {
            outputJson = toggledOn;
            UpdateConfigurationValue(config => config.OutputJson = outputJson);
        }

        private void InitializeNodeRefs()
        {
            addInferenceConfigButton = GetNode<Button>("%AddInferenceConfigButton");
            deleteInferenceConfigButton = GetNode<Button>("%DeleteInferenceConfigButton");
            backButton = GetNode<Button>("%BackButton");
            autoloadLastGoodConfigCheckBox = GetNode<CheckBox>("%AutoloadLastGoodConfigCheckBox");

            outputJsonCheckBox = GetNode<CheckBox>("%OutputJsonCheckBox");
            maxTokensHSlider = GetNode<HSlider>("%MaxTokensHSlider");
            temperatureHSlider = GetNode<HSlider>("%TemperatureHSlider");
            savedInferenceConfigsItemList = GetNode<ItemList>("%SavedInferenceConfigsItemList");

            inferenceConfigNameLineEdit = GetNode<LineEdit>("%NameLineEdit");
            maxTokensLineEdit = GetNode<LineEdit>("%MaxTokensLineEdit");
            temperatureLineEdit = GetNode<LineEdit>("%TemperatureLineEdit");
        }

        private static uint CalculateExpMaxTokens(double value) => (uint)Math.Pow(2, value) * 1000;
        private static double CalculateLogMaxTokens(uint value) => Math.Log2(value / 1000.0);

        private void UpdateSliderValue<T>(T value, LineEdit lineEdit, Action<T> setValue, Action<InferenceParamsConfig> updateConfig)
        {
            setValue(value);
            lineEdit.Text = value.ToString();
            UpdateConfigurationValue(updateConfig);
        }
    }
}
