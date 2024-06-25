using Godot;
using System;

namespace MindGame
{
    /// <summary>
    /// The controller for configuring inference parameters
    /// </summary>

    public partial class InferenceConfig : Control
    {
        /// <summary>
        /// Godot node variables
        /// </summary>
        private MindManager _mindManager { get; set; }
        private Button _addInferenceConfigButton, _deleteInferenceConfigButton, _backButton, _loadInferenceConfigButton, _unloadInferenceConfigButton;
        private CheckBox _autoloadLastGoodConfigCheckBox, _outputJsonCheckBox;
        private HSlider _maxTokensHSlider, _temperatureHSlider;
        private LineEdit _inferenceConfigNameLineEdit, _maxTokensLineEdit, _temperatureLineEdit;
        private InferenceParams _inferenceParamsConfig;
        
        private string _inferenceConfigName;
        private string[] _antiPrompts;
        private int _maxTokens;
        private float _temperature;
        private bool _outputJson;
        private ItemList _savedInferenceConfigsItemList;


        /// <summary>
        /// Function that is called when node and all children are initialized
        /// </summary>
        public override void _Ready()
        {
            InitializeDefaultValues();
            InitializeNodeRefs();
            InitializeConfigList();
            InitializeSignals();
            AutoloadLastGoodConfig();
            InitializeUiElements();
        }


        private void InitializeDefaultValues()
        {
            _inferenceConfigName = "<default>";
            _maxTokens = 4000;
            _antiPrompts = ["<|eot_id|>", "<|end|>", "user:", "User:", "USER:", "\nUser:", "\nUSER:", "}"];
            _temperature = 0.75f;
            _outputJson = false;
        }

        private void InitializeNodeRefs()
        {
            _mindManager = GetNode<MindGame.MindManager>("/root/MindManager");

            _addInferenceConfigButton = GetNode<Button>("%AddInferenceConfigButton");
            _deleteInferenceConfigButton = GetNode<Button>("%DeleteInferenceConfigButton");
            _backButton = GetNode<Button>("%BackButton");
            _autoloadLastGoodConfigCheckBox = GetNode<CheckBox>("%AutoloadLastGoodConfigCheckBox");

            _outputJsonCheckBox = GetNode<CheckBox>("%OutputJsonCheckBox");
            _maxTokensHSlider = GetNode<HSlider>("%MaxTokensHSlider");
            _temperatureHSlider = GetNode<HSlider>("%TemperatureHSlider");
            _savedInferenceConfigsItemList = GetNode<ItemList>("%SavedInferenceConfigsItemList");

            _inferenceConfigNameLineEdit = GetNode<LineEdit>("%NameLineEdit");
            _maxTokensLineEdit = GetNode<LineEdit>("%MaxTokensLineEdit");
            _temperatureLineEdit = GetNode<LineEdit>("%TemperatureLineEdit");
        }


        private void InitializeUiElements()
        {
            _inferenceConfigNameLineEdit.Text = _inferenceConfigName;
            _maxTokensHSlider.Value = CalculateLogMaxTokens((uint)_maxTokens);
            _maxTokensLineEdit.Text = _maxTokens.ToString();
            _temperatureHSlider.Value = (double)_temperature;
            _temperatureLineEdit.Text = _temperature.ToString();
            _outputJsonCheckBox.ButtonPressed = _outputJson;
            _autoloadLastGoodConfigCheckBox.ButtonPressed = _mindManager.ConfigList.AutoloadLastGoodInferenceConfig;
        }

        private void InitializeConfigList()
        {
            UpdateConfigItemList();
        }


        /// <summary>
        /// Function that is called to connect signals to callbacks
        /// </summary>
        private void InitializeSignals()
        {
            // Config management and UI navigation
            _savedInferenceConfigsItemList.ItemSelected += OnSavedInferenceConfigSelected;
            _addInferenceConfigButton.Pressed += OnAddInferenceConfigPressed;
            _deleteInferenceConfigButton.Pressed += OnDeleteInferenceConfigPressed;
            _backButton.Pressed += OnBackPressed;
            _autoloadLastGoodConfigCheckBox.Toggled += OnAutoloadLastGoodConfigToggled;

            // Inference config parameters
            _inferenceConfigNameLineEdit.TextChanged += OnInferenceConfigNameTextChanged;
            _maxTokensHSlider.ValueChanged += OnMaxTokensValueChanged;
            _temperatureHSlider.ValueChanged += OnTemperatureValueChanged;
            _outputJsonCheckBox.Toggled += OnOutputJsonToggled;
        }

        private void OnAutoloadLastGoodConfigToggled(bool toggledOn)
        {
            if (_mindManager.ConfigList != null)
            {
                _mindManager.ConfigList.AutoloadLastGoodInferenceConfig = toggledOn;
                SaveConfigList();
            }
        }

        public void AutoloadLastGoodConfig()
        {
            if (_mindManager.ConfigList != null && _mindManager.ConfigList.AutoloadLastGoodInferenceConfig && _mindManager.ConfigList.LastGoodInferenceConfig != null)
            {
                _inferenceParamsConfig = _mindManager.ConfigList.LastGoodInferenceConfig;
                UpdateInferenceConfigUi();
            }
        }

        private void UpdateInferenceConfigUi()
        {
            _inferenceConfigNameLineEdit.Text = _inferenceParamsConfig.InferenceConfigName;
            _temperatureHSlider.Value = (double)_inferenceParamsConfig.Temperature;
            _temperatureLineEdit.Text = _inferenceParamsConfig.Temperature.ToString();
            _maxTokensHSlider.Value = CalculateLogMaxTokens((uint)_inferenceParamsConfig.MaxTokens);
            _maxTokensLineEdit.Text = _inferenceParamsConfig.MaxTokens.ToString();
            _outputJsonCheckBox.ButtonPressed = _inferenceParamsConfig.OutputJson;
        }

        private void OnSavedInferenceConfigSelected(long index)
        {
            if (index >= 0 && index < _mindManager.ConfigList.InferenceConfigurations.Count)
            {
                _inferenceParamsConfig = _mindManager.ConfigList.InferenceConfigurations[(int)index];
                UpdateInferenceConfigUi();
            }
        }

        private void OnTemperatureValueChanged(double value)
        {
            UpdateSliderValue(value, _temperatureLineEdit, v => _temperature = (float)v, config => config.Temperature = _temperature);
        }

        private void OnMaxTokensValueChanged(double value)
        {
            UpdateSliderValue(CalculateExpMaxTokens(value), _maxTokensLineEdit, v => _maxTokens = (int)v, config => config.MaxTokens = _maxTokens);
        }

        private void OnInferenceConfigNameTextChanged(string newText)
        {
            _inferenceConfigName = newText;
            UpdateConfigurationValue(config =>
            {
                config.InferenceConfigName = _inferenceConfigName;
                _savedInferenceConfigsItemList.SetItemText(_savedInferenceConfigsItemList.GetSelectedItems()[0], _inferenceConfigName);
            });
        }

        private void OnDeleteInferenceConfigPressed()
        {
            var selectedIndices = _savedInferenceConfigsItemList.GetSelectedItems();

            if (selectedIndices.Length > 0 && _mindManager.ConfigList.InferenceConfigurations.Count > 0)
            {
                int selectedIndex = selectedIndices[0];
                if (selectedIndex >= 0 && selectedIndex < _mindManager.ConfigList.InferenceConfigurations.Count)
                {
                    _mindManager.ConfigList.InferenceConfigurations.RemoveAt(selectedIndex);
                    SaveConfigList();
                    UpdateConfigItemList();
                }
            }
            else
            {
                GD.Print("No configuration selected or list is empty.");
            }
        }

        private void OnAddInferenceConfigPressed()
        {
            InferenceParams newConfig = new InferenceParams
            {
                InferenceConfigName = _inferenceConfigName,
                MaxTokens = _maxTokens,
                AntiPrompts = _antiPrompts,
                Temperature = _temperature,
                OutputJson = _outputJson
            };

            _mindManager.ConfigList.InferenceConfigurations.Add(newConfig);
            _mindManager.ConfigList.CurrentInferenceConfig = newConfig;
            SaveConfigList();
            UpdateConfigItemList();
        }

        private void UpdateConfigItemList()
        {
            _savedInferenceConfigsItemList.Clear();
            foreach (var config in _mindManager.ConfigList.InferenceConfigurations)
            {
                _savedInferenceConfigsItemList.AddItem(config.InferenceConfigName);
            }
        }

        private void UpdateConfigurationValue(Action<InferenceParams> updateAction)
        {
            var selectedIndices = _savedInferenceConfigsItemList.GetSelectedItems();
            if (selectedIndices.Length > 0)
            {
                int selectedIndex = selectedIndices[0];
                if (selectedIndex >= 0 && selectedIndex < _mindManager.ConfigList.InferenceConfigurations.Count)
                {
                    var config = _mindManager.ConfigList.InferenceConfigurations[selectedIndex];
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
            Error saveError = ResourceSaver.Save(_mindManager.ConfigList, _mindManager.ConfigListPath);
            if (saveError != Error.Ok)
            {
                GD.PrintErr("Failed to save configuration list: ", saveError);
            }
        }

        private void OnOutputJsonToggled(bool toggledOn)
        {
            _outputJson = toggledOn;
            UpdateConfigurationValue(config => config.OutputJson = _outputJson);
        }

        private static uint CalculateExpMaxTokens(double value) => (uint)Math.Pow(2, value) * 1000;
        private static double CalculateLogMaxTokens(uint value) => Math.Log2(value / 1000.0);

        private void UpdateSliderValue<T>(T value, LineEdit lineEdit, Action<T> setValue, Action<InferenceParams> updateConfig)
        {
            setValue(value);
            lineEdit.Text = value.ToString();
            UpdateConfigurationValue(updateConfig);
        }
    }
}
