using Godot;
using MindGame;
using System.Collections.Generic;

namespace MindGame
{
    [Tool]
    public partial class ConfigListResource : Resource
    {
        [Export]
        public Godot.Collections.Array<ModelParamsConfigs> ModelConfigurations { get; set; } = new Godot.Collections.Array<ModelParamsConfigs>();
        [Export]
        public Godot.Collections.Array<InferenceParamsConfigs> InferenceConfigurations { get; set; } = new Godot.Collections.Array<InferenceParamsConfigs>();
    }
}
