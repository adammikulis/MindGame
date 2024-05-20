using Godot;
using Godot.Collections;
using System;

namespace MindGame
{
    [Tool]
    public partial class MindGraph : Resource
    {
        [Export]
        public Array<MindNode> Nodes { get; private set; } = new Array<MindNode>();

        [Export]
        public Array<MindEdge> Edges { get; private set; } = new Array<MindEdge>();

        private int nextNodeId = 0;
        private int nextEdgeId = 0;

        public MindGraph() { }

        public MindNode AddNode(string name = "", Dictionary<string, Variant> data = null)
        {
            var node = new MindNode(nextNodeId, name, data);
            Nodes.Add(node);
            nextNodeId++;
            return node;
        }

        public bool RemoveNode(int nodeId)
        {
            var node = GetNode(nodeId);
            if (node == null)
                return false;

            Nodes.Remove(node);
            var edgesToRemove = new Array<MindEdge>();
            foreach (var edge in Edges)
            {
                if (edge.SourceId == nodeId || edge.TargetId == nodeId)
                {
                    edgesToRemove.Add(edge);
                }
            }
            foreach (var edge in edgesToRemove)
            {
                Edges.Remove(edge);
            }
            return true;
        }

        public MindEdge AddEdge(int sourceId, int targetId, string name = "", float weight = 1.0f, Dictionary<string, Variant> data = null, bool isBidirectional = false)
        {
            var sourceNode = GetNode(sourceId);
            var targetNode = GetNode(targetId);

            if (sourceNode == null || targetNode == null)
                throw new ArgumentException("Source or target node does not exist");

            var edge = new MindEdge(nextEdgeId, sourceId, targetId, name, weight, data);
            Edges.Add(edge);
            nextEdgeId++;

            if (isBidirectional)
            {
                var reverseEdge = new MindEdge(nextEdgeId, targetId, sourceId, name, weight, data);
                Edges.Add(reverseEdge);
                nextEdgeId++;
            }

            return edge;
        }

        public bool RemoveEdge(int edgeId)
        {
            var edge = GetEdge(edgeId);
            if (edge == null)
                return false;

            Edges.Remove(edge);

            return true;
        }

        public MindNode GetNode(int nodeId)
        {
            foreach (var node in Nodes)
            {
                if (node.Id == nodeId)
                {
                    return node;
                }
            }
            return null;
        }

        public MindEdge GetEdge(int edgeId)
        {
            foreach (var edge in Edges)
            {
                if (edge.Id == edgeId)
                {
                    return edge;
                }
            }
            return null;
        }

        public Array<MindEdge> GetEdges()
        {
            return Edges;
        }

        public void UpdateEdgeWeight(int edgeId, float amount)
        {
            var edge = GetEdge(edgeId);
            if (edge != null)
            {
                edge.UpdateWeight(amount);
            }
        }

        public void UpdateAllEdgeWeights(float amount)
        {
            foreach (var edge in Edges)
            {
                edge.UpdateWeight(amount);
            }
        }

        public void ExecuteRules(int sourceId, int targetId, string variable, float delta, Dictionary<string, MindRule> rules)
        {
            foreach (var rule in rules)
            {
                var (edgeType, isBidirectional) = rule.Value.Evaluate(variable, delta);
                if (!string.IsNullOrEmpty(edgeType))
                {
                    var edge = AddEdge(sourceId, targetId, edgeType, 1.0f, null, isBidirectional);

                    if (Math.Abs(delta) > rule.Value.MemoryThreshold)
                    {
                        // Create a memory node
                        var memoryData = new Dictionary<string, Variant> { { "description", $"{variable} change: {delta}" } };
                        var memoryNode = AddNode("Memory", memoryData);

                        // Connect the memory node to the relevant nodes and edge
                        AddEdge(memoryNode.Id, sourceId, "related_to", 0.5f, null);
                        AddEdge(memoryNode.Id, targetId, "related_to", 0.5f, null);
                    }

                    break;
                }
            }
        }
    }
}
