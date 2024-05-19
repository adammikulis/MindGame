using Godot;
using Godot.Collections;

namespace MindGame
{
    [Tool]
    public partial class MindEdge : Resource
    {
        [Export]
        public int SourceId { get; set; }

        [Export]
        public int TargetId { get; set; }

        [Export]
        public float Weight { get; set; }

        [Export]
        public Dictionary<string, Variant> Data { get; set; } = new Dictionary<string, Variant>();

        public MindEdge() : this(0, 0, 0) { }

        public MindEdge(int sourceId, int targetId, float weight)
        {
            SourceId = sourceId;
            TargetId = targetId;
            Weight = weight;
        }

        public void DecreaseWeight(float amount)
        {
            Weight = Mathf.Max(0, Weight - amount);
        }

        public void IncreaseWeight(float amount)
        {
            Weight += amount;
        }
    }
}
