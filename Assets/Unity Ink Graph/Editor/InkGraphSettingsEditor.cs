using UnityEditor;
using UnityEngine;

namespace UnityInkGraph {
    [CustomEditor(typeof(InkGraphSettings))]
    public class InkGraphSettingsEditor : Editor {
        private InkGraphSettings m_Target;
        private InkGraphSettings Target => Utils.GetOrSet(ref m_Target, () => (InkGraphSettings) target);

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            if (GUILayout.Button("Create Graph")) {
                GraphCreator.ExportInkEdgeCsv(Target);
            }
            if (GUILayout.Button("Open Graph")) {
                GraphCreator.OpenGraph(Target);
            }
        }
    }
}