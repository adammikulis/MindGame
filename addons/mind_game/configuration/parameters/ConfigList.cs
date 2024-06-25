using Godot;
using MindGame;
using System.Collections.Generic;

namespace MindGame
{
    [Tool]
    public partial class ConfigList : Resource
    {
        [Export]
        public Godot.Collections.Array<ModelParams> ModelConfigurations { get; set; } = [];
        [Export]
        public Godot.Collections.Array<InferenceParams> InferenceConfigurations { get; set; } = [];
        [Export]
        public InferenceParams CurrentInferenceConfig { get; set; }
        [Export]
        public InferenceParams LastGoodInferenceConfig { get; set; }
        [Export]
        public ModelParams CurrentModelConfig { get; set; }
        [Export]
        public ModelParams LastGoodModelConfig { get; set; }
        [Export]
        public bool AutoloadLastGoodModelConfig { get; set; } = false;
        [Export]
        public bool AutoloadLastGoodInferenceConfig { get; set; } = false;
    }
}
