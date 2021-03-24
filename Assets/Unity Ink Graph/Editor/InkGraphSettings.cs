using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityInkGraph {
    [CreateAssetMenu]
    public class InkGraphSettings : ScriptableObject {
        [SerializeField] private DefaultAsset m_RootInkScript;
        [SerializeField] private NodeDepth m_ExportDepth = NodeDepth.KnotsStitchesAndLabels;
        [SerializeField] private string m_ExportPath = "Exports/";
        [SerializeField] private TunnelNodesHandling m_TunnelNodesHandling;
        [SerializeField] private bool m_OpenOnComplete = true;
        [SerializeField] private string[] m_ExcludePaths;
        [SerializeField] private string[] m_ForceTunnelPaths;
        [SerializeField] private string[] m_ForceDuplicatePaths;

        public DefaultAsset RootInkScript => m_RootInkScript;
        public NodeDepth ExportDepth => m_ExportDepth;
        public string ExportPath => m_ExportPath.EndsWith(".tgf") ? m_ExportPath : Path.Combine(m_ExportPath, $"{m_RootInkScript.name}.tgf");
        public TunnelNodesHandling TunnelNodesHandling => m_TunnelNodesHandling;
        public bool OpenOnComplete => m_OpenOnComplete;
        public string[] ExcludePaths => m_ExcludePaths;
        public string[] ForceTunnelPaths => m_ForceTunnelPaths;
        public string[] ForceDuplicatePaths => m_ForceDuplicatePaths;

        private void OnValidate() {
            if (m_RootInkScript && !AssetDatabase.GetAssetPath(m_RootInkScript).EndsWith(".ink")) {
                m_RootInkScript = null;
            }
        }
    }
}