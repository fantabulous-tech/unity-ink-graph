using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace UnityInkGraph {
    public static class GraphCreator {
        public static void ExportInkEdgeCsv(InkGraphSettings settings) {
            Utils.WriteAllText(settings.ExportPath, GetInkGraph(settings));

            if (settings.OpenOnComplete) {
                OpenGraph(settings);
            }
        }

        public static void OpenGraph(InkGraphSettings settings) {
            System.Diagnostics.Process.Start(settings.ExportPath.Replace('/', '\\'));
        }

        private static string GetInkGraph(InkGraphSettings settings) {
            string inkPath = AssetDatabase.GetAssetPath(settings.RootInkScript);
            List<InkFileInfo> inkFiles = GetInkFiles(inkPath);
            Graph graph = new Graph(inkFiles, settings);
            return graph.ToTgfText();
        }

        private static List<InkFileInfo> GetInkFiles(string storyScriptPath) {
            string rootPath = Path.GetDirectoryName(storyScriptPath)?.Replace('\\', '/').TrimEnd('/');
            DefaultAsset rootInkFile = AssetDatabase.LoadAssetAtPath<DefaultAsset>(storyScriptPath);
            InkFileInfo rootInfo = new InkFileInfo(rootInkFile, rootPath);

            Queue<InkFileInfo> queue = new Queue<InkFileInfo>(new[] {rootInfo});
            List<InkFileInfo> inkFiles = new List<InkFileInfo>();

            while (queue.Count > 0) {
                InkFileInfo info = queue.Dequeue();
                inkFiles.Add(info);

                foreach (string includePath in info.Includes) {
                    if (inkFiles.Any(f => f.Path.Equals(includePath, StringComparison.OrdinalIgnoreCase))) {
                        continue;
                    }
                    queue.Enqueue(new InkFileInfo(includePath, rootPath));
                }
            }

            return inkFiles;
        }
    }

}