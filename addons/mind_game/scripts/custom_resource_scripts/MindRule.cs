using Godot;

namespace MindGame
{
    public partial class MindRule : Resource
    {
        [Export]
        public Callable Condition { get; set; }

        [Export]
        public float MemoryThreshold { get; set; } = 0.0f;

        public MindRule() { }

        public MindRule(Callable condition, float memoryThreshold = 0.0f)
        {
            Condition = condition;
            MemoryThreshold = memoryThreshold;
        }

        public (string, bool) Evaluate(string variable, float delta)
        {
            Variant result = Condition.Call(variable, delta);
            if (result.Obj is Godot.Collections.Array resultArray && resultArray.Count == 2)
            {
                return ((string)resultArray[0], (bool)resultArray[1]);
            }
            return ("", false);
        }
    }
}
