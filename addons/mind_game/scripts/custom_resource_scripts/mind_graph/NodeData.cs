using Godot;
using Godot.Collections;

namespace MindGame
{
    [Tool]
    public partial class NodeData : Resource
    {
        [Export]
        public Dictionary<string, Variant> Data { get; set; } = new Dictionary<string, Variant>();
    }
}
