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
    }
}
