using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                ExportInkEdgeCsv();
            }
            if (GUILayout.Button("Open Graph")) {
                OpenGraph();
            }
        }

        private void ExportInkEdgeCsv() {
            string inkPath = AssetDatabase.GetAssetPath(Target.RootInkScript);
            List<InkFileInfo> inkFiles = GetInkFiles(inkPath);
            Graph graph = new Graph(inkFiles, Target);
            Utils.WriteAllText(Target.ExportPath, graph.ToTgfText());

            if (Target.OpenOnComplete) {
                OpenGraph();
            }
        }

        private void OpenGraph() {
            System.Diagnostics.Process.Start(Target.ExportPath.Replace('/', '\\'));
        }

        private static List<InkFileInfo> GetInkFiles(string storyScriptPath) {
            string rootPath = Path.GetDirectoryName(storyScriptPath)?.Replace('\\', '/').TrimEnd('/');
            DefaultAsset rootInkFile = AssetDatabase.LoadAssetAtPath<DefaultAsset>(storyScriptPath);
            InkFileInfo rootInfo = new InkFileInfo(rootInkFile, rootPath);

            Queue<InkFileInfo> queue = new Queue<InkFileInfo>(new[] {rootInfo});
            Dictionary<string, InkFileInfo> inkFiles = new Dictionary<string, InkFileInfo>(StringComparer.OrdinalIgnoreCase);

            while (queue.Count > 0) {
                InkFileInfo info = queue.Dequeue();
                inkFiles[info.Path] = info;

                foreach (string includePath in info.Includes) {
                    if (!inkFiles.ContainsKey(includePath)) {
                        queue.Enqueue(new InkFileInfo(includePath, rootPath));
                    }
                }
            }

            return inkFiles.Values.ToList();
        }
    }
}