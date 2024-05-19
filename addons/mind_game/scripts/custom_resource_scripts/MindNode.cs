using Godot;
using Godot.Collections;

namespace MindGame
{
    [Tool]
    public partial class MindNode : Resource
    {
        [Export]
        public int Id { get; set; }

        [Export]
        public Dictionary<string, Variant> Data { get; set; } = new Dictionary<string, Variant>();

        public MindNode() { }

        public MindNode(int id, Dictionary<string, Variant> data)
        {
            Id = id;
            Data = data;
        }
    }
}
