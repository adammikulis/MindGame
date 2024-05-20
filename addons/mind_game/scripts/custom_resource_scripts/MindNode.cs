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
        public string Name { get; set; } = "";

        [Export]
        public Dictionary<string, Variant> Data { get; set; } = new Dictionary<string, Variant>();

        public MindNode() { }

        public MindNode(int id, string name = "", Dictionary<string, Variant> data = null)
        {
            Id = id;
            Name = name;
            Data = data ?? new Dictionary<string, Variant>();
        }
    }
}
