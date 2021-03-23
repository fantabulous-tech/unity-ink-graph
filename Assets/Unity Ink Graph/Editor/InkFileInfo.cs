using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityInkGraph {
    [Serializable]
    public class InkFileInfo {
        [SerializeField] private DefaultAsset m_InkAsset;
        [SerializeField] private string[] m_Lines;
        [SerializeField] private string m_Path;
        [SerializeField] private string m_DisplayPath;
        [SerializeField] private string[] m_Includes;
        [SerializeField] private string m_Directory;
        [SerializeField] private bool m_Excluded;

        public DefaultAsset InkAsset => Utils.GetOrSet(ref m_InkAsset, () => AssetDatabase.LoadAssetAtPath<DefaultAsset>(m_Path));
        public string Path => Utils.GetOrSet(ref m_Path, () => AssetDatabase.GetAssetPath(m_InkAsset));
        public string DisplayPath => m_DisplayPath;
        private string Directory => Utils.GetOrSet(ref m_Directory, () => System.IO.Path.GetDirectoryName(Path)?.Replace('\\', '/'));
        public string[] Lines => Utils.GetOrSet(ref m_Lines, () => Path.IsNullOrEmpty() ? null : File.ReadAllText(Path).SplitRegex(@"\r?\n"));
        public string[] Includes => Utils.GetOrSet(ref m_Includes, GetIncludes);

        private string[] GetIncludes() {
            IEnumerable<string> includeLines = Lines.Where(l => l.StartsWith("INCLUDE"));
            return includeLines.Select(l => $"{Directory}/{l.ReplaceRegex(@"^INCLUDE\s+", "")}".Replace('\\', '/')).ToArray();
        }

        public InkFileInfo() { }

        public InkFileInfo(DefaultAsset inkAsset, string rootDirectory) {
            m_InkAsset = inkAsset;
            string path = AssetDatabase.GetAssetPath(inkAsset);
            m_Path = path.Replace("\\", "/");
            m_DisplayPath = m_Path.Replace(rootDirectory.TrimEnd('/') + "/", "", StringComparison.OrdinalIgnoreCase);
        }

        public InkFileInfo(string path, string rootDirectory) {
            m_Path = path.Replace("\\", "/");
            m_DisplayPath = m_Path.Replace(rootDirectory.TrimEnd('/') + "/", "", StringComparison.OrdinalIgnoreCase);
        }

        public void Refresh() {
            m_Lines = null;
            m_Includes = null;
            m_Directory = null;
        }

        public void OnGUI() {
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            m_Excluded = GUILayout.Toggle(m_Excluded, DisplayPath);
            GUILayout.EndHorizontal();
        }
    }
}