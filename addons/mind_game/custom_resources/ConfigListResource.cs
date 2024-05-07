using Godot;
using MindGame;
using System.Collections.Generic;

namespace MindGame
{
    public partial class ConfigListResource : Resource
    {
        [Export]
        public Godot.Collections.Array<ModelConfigsParams> Configurations { get; set; } = new Godot.Collections.Array<ModelConfigsParams>();
    }
}
