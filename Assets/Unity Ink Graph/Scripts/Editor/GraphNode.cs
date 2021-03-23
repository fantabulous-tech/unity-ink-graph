using System;

namespace UnityInkGraph {
    internal class GraphNode {
        public NodeType Type { get; }
        public int Id { get; }
        public int KnotId { get; set; }
        public int StitchId { get; set; }
        public string Label { get; }
        public bool IsTunnel { get; set; }
        public bool IsUsed { get; set; }
        public bool IsExcluded { get; set; }

        public GraphNode(NodeType type, int id, string label) {
            Id = id;
            Label = label;
            Type = type;
        }

        public GraphNode(GraphNode node, int id) {
            Id = id;
            Type = node.Type;
            KnotId = node.Id != node.KnotId ? node.KnotId : id;
            StitchId = node.StitchId != node.Id ? node.StitchId : id;
            Label = node.Label;
            IsTunnel = node.IsTunnel;
            IsExcluded = node.IsExcluded;
        }

        public int GetIdAtDepth(NodeDepth depth) {
            switch (depth) {
                case NodeDepth.KnotsOnly:
                    return KnotId;
                case NodeDepth.KnotsAndStitches:
                    return Type == NodeType.Knot ? Id : StitchId;
                case NodeDepth.KnotsStitchesAndLabels:
                    return Id;
                default:
                    throw new ArgumentOutOfRangeException(nameof(depth), depth, null);
            }
        }

        public bool IsAtDepth(NodeDepth depth) {
            switch (depth) {
                case NodeDepth.KnotsOnly:
                    return Type == NodeType.Knot;
                case NodeDepth.KnotsAndStitches:
                    return Type != NodeType.Label;
                case NodeDepth.KnotsStitchesAndLabels:
                    return true;
                default:
                    throw new ArgumentOutOfRangeException(nameof(depth), depth, null);
            }
        }

        public override string ToString() {
            return $"{GetType().Name} '{Label}'";
        }
    }
}