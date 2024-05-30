using Godot;
using Godot.Collections;

namespace MindGame
{
    [Tool]
    public partial class MindEdge : Resource
    {
        [Export]
        public int Id { get; set; }

        [Export]
        public int SourceId { get; set; }

        [Export]
        public int TargetId { get; set; }

        [Export]
        public string Name { get; set; } = "";

        [Export]
        public float Weight { get; set; } = 1.0f;

        [Export]
        public Dictionary<string, Variant> Data { get; set; } = new Dictionary<string, Variant>();

        public MindEdge() { }

        public MindEdge(int id, int sourceId, int targetId, string name = "", float weight = 1.0f, Dictionary<string, Variant> data = null)
        {
            Id = id;
            SourceId = sourceId;
            TargetId = targetId;
            Name = name;
            Weight = weight;
            Data = data ?? new Dictionary<string, Variant>();
        }

        public void UpdateWeight(float amount)
        {
            Weight = Mathf.Max(0, Weight + amount);
        }
    }
}
