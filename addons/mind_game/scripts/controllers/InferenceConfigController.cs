using Godot;
using System;

namespace MindGame
{
    [Tool]
    public partial class InferenceConfigController : Control
    {
        public Button addInferenceConfigButton, deleteInferenceConfigButton;
        public CheckBox outputJsonCheckBox;
        public HSlider maxTokensHSlider, temperatureHSlider;
        public LineEdit nameLineEdit, maxTokensLineEdit, temperatureLineEdit;

        private string inferenceParamsName;
        private string[] antiPrompts;
        private int maxTokens;
        private float temperature;
        private bool outputJson;

        public override void _Ready()
        {
            InitializeDefaultValues();
            InitializeNodeRefs();
            InitializeSignals();

        }

        private void InitializeDefaultValues()
        {
            inferenceParamsName = "<default>";
            maxTokens = 4000;
            antiPrompts = ["<|eot_id|>", "<|end_of_text|>", "<|user|>", "<|end|>", "user:", "User:", "USER:", "\nUser:", "\nUSER:", "}"];
            temperature = 0.5f;
            outputJson = false;
        }

        private void InitializeSignals()
        {
            outputJsonCheckBox.Toggled += OnOutputJsonToggled;
        }

        private void OnOutputJsonToggled(bool toggledOn)
        {
            throw new NotImplementedException();
        }

        private void InitializeNodeRefs()
        {
            addInferenceConfigButton = GetNode<Button>("%AddInferenceConfigButton");
            deleteInferenceConfigButton = GetNode<Button>("%DeleteInferenceConfigButton");
            outputJsonCheckBox = GetNode<CheckBox>("%OutputJsonCheckBox");
            maxTokensHSlider = GetNode<HSlider>("%MaxTokensHSlider");
            temperatureHSlider = GetNode<HSlider>("%TemperatureHSlider");

            nameLineEdit = GetNode<LineEdit>("%NameLineEdit");
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

    
