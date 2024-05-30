using Godot;
using Godot.Collections;

namespace MindGame
{
    public partial class MindGraphExample : Node
    {
        [Export]
        public Resource MindGraphResource;

        public override void _Ready()
        {
            if (MindGraphResource is MindGraph mindGraph)
            {
                // Create node data
                var foodData = new Dictionary<string, Variant>();
                foodData["nutrition"] = 5;
                var poisonData = new Dictionary<string, Variant>();
                poisonData["toxicity"] = 10;
                var appleData = new Dictionary<string, Variant>();
                appleData["type"] = "fruit";
                var oozingBerryData = new Dictionary<string, Variant>();
                oozingBerryData["type"] = "berry";

                // Add nodes with types
                var foodNode = mindGraph.AddNode("Food", foodData);
                var poisonNode = mindGraph.AddNode("Poison", poisonData);
                var appleNode = mindGraph.AddNode("Apple", appleData);
                var oozingBerryNode = mindGraph.AddNode("Oozing Berry", oozingBerryData);

                // Define rules with memory threshold
                var rules = new Dictionary<string, MindRule>
                {
                    { "heals", new MindRule(new Callable(this, nameof(IsHealsRule)), 5.0f) },
                    { "hurts", new MindRule(new Callable(this, nameof(IsHurtsRule)), 5.0f) }
                };

                // Execute rules to set edge types based on the variable change
                //mindGraph.ExecuteRules(appleNode.Id, foodNode.Id, "health", 10, rules);
                //mindGraph.ExecuteRules(oozingBerryNode.Id, poisonNode.Id, "health", -10, rules);

                PrintNodeDetails(foodNode);
                PrintNodeDetails(poisonNode);
                PrintNodeDetails(appleNode);
                PrintNodeDetails(oozingBerryNode);

                // Print all edges
                foreach (var edge in mindGraph.GetEdges())
                {
                    var sourceNode = mindGraph.GetNode(edge.SourceId);
                    var targetNode = mindGraph.GetNode(edge.TargetId);
                    GD.Print($"Edge {edge.Id}: from {sourceNode.Name} to {targetNode.Name} Weight: {edge.Weight}, Type: {edge.Name}");
                }
            }
        }

        private void PrintNodeDetails(MindNode node)
        {
            GD.Print($"Node {node.Id}: {node.Name}");
            foreach (var key in node.Data.Keys)
            {
                GD.Print($"  {key}: {node.Data[key]}");
            }
        }

        private Variant IsHealsRule(string variable, float delta)
        {
            if (variable == "health" && delta > 0)
            {
                return new Godot.Collections.Array { "heals", false };
            }
            return new Godot.Collections.Array { "", false };
        }

        private Variant IsHurtsRule(string variable, float delta)
        {
            if (variable == "health" && delta < 0)
            {
                return new Godot.Collections.Array { "hurts", true };
            }
            return new Godot.Collections.Array { "", false };
        }
    }
}
