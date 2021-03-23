namespace UnityInkGraph {
    public enum NodeType {
        Knot,
        Stitch,
        Label
    }

    public enum NodeDepth {
        KnotsOnly,
        KnotsAndStitches,
        KnotsStitchesAndLabels
    }
}