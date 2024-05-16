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

        public override void _Ready()
        {
            InitializeNodeRefs();
            InitializeSignals();

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
            deleteInferenceConfigButton = GetNode<Button>("DeleteInferenceConfig");
            outputJsonCheckBox = GetNode<CheckBox>("%OutputJsonCheckBox");
            maxTokensHSlider = GetNode<HSlider>("%MaxTokensHSlider");
            temperatureHSlider = GetNode<HSlider>("%TemperatureHSlider");

            nameLineEdit = GetNode<LineEdit>("%NameLineEdit");
            maxTokensLineEdit = GetNode<LineEdit>("%MaxTokensLineEdit");
            temperatureLineEdit = GetNode<LineEdit>("%TemperatureLineEdit");
        }

        private uint calculateExpMaxTokens(double value)
        {
            return (uint)Math.Pow(2, value) * 1000;
        }

        private double calculateLogMaxTokens(uint value)
        {
            return (double)Math.Log2(value / 1000);
        }
    }
}

    
