using Godot;
using Godot.Collections;

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

        public MindGraph() { }

        public MindNode AddNode(Dictionary<string, Variant> data)
        {
            var node = new MindNode(nextNodeId, data);
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

        public MindEdge AddEdge(int sourceId, int targetId, float weight = 1.0f, bool isBidirectional = false)
        {
            var sourceNode = GetNode(sourceId);
            var targetNode = GetNode(targetId);

            if (sourceNode == null || targetNode == null)
                throw new System.ArgumentException("Source or target node does not exist");

            var edge = new MindEdge(sourceId, targetId, weight);
            Edges.Add(edge);

            if (isBidirectional)
            {
                var reverseEdge = new MindEdge(targetId, sourceId, weight);
                Edges.Add(reverseEdge);
            }

            return edge;
        }

        public bool RemoveEdge(int sourceId, int targetId)
        {
            var edge = GetEdge(sourceId, targetId);
            if (edge == null)
                return false;

            Edges.Remove(edge);

            var reverseEdge = GetEdge(targetId, sourceId);
            if (reverseEdge != null)
            {
                Edges.Remove(reverseEdge);
            }

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

        public MindEdge GetEdge(int sourceId, int targetId)
        {
            foreach (var edge in Edges)
            {
                if (edge.SourceId == sourceId && edge.TargetId == targetId)
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

        public void UpdateEdgeWeights(float increaseAmount)
        {
            foreach (var edge in Edges)
            {
                edge.IncreaseWeight(increaseAmount);
            }
        }

        public void UseEdge(int sourceId, int targetId, float decreaseAmount)
        {
            var edge = GetEdge(sourceId, targetId);
            if (edge != null)
            {
                edge.DecreaseWeight(decreaseAmount);

                var reverseEdge = GetEdge(targetId, sourceId);
                if (reverseEdge != null)
                {
                    reverseEdge.DecreaseWeight(decreaseAmount);
                }
            }
        }
    }
}
