using Godot;
using MindGame;
using System.Collections.Generic;

namespace MindGame
{
    [Tool]
    public partial class ConfigListResource : Resource
    {
        [Export]
        public Godot.Collections.Array<ModelParamsConfig> ModelConfigurations { get; set; } = [];
        [Export]
        public Godot.Collections.Array<InferenceParamsConfig> InferenceConfigurations { get; set; } = [];
        [Export]
        public InferenceParamsConfig CurrentInferenceConfig { get; set; }
        [Export]
        public InferenceParamsConfig LastGoodInferenceConfig { get; set; }
        [Export]
        public ModelParamsConfig CurrentModelConfig { get; set; }
        [Export]
        public ModelParamsConfig LastGoodModelConfig { get; set; }
        [Export]
        public bool UseLastGoodModelConfig { get; set; }
        [Export]
        public bool UseLastGoodInferenceConfig { get; set; }
    }
}
