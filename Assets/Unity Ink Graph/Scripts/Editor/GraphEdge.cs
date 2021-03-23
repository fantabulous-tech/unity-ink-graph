namespace UnityInkGraph {
    internal class GraphEdge {
        public int Source { get; set; }
        public int Target { get; set; }
        public string Label { get; set; }

        public GraphEdge(int source, int target, string label) {
            Source = source;
            Target = target;
            Label = label;
        }
    }

}