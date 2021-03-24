using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace UnityInkGraph {
    internal class Graph {
        private static readonly Regex s_KnotRegex = new Regex(@"^\s*==+\s*(?<result>\w+)");
        private static readonly Regex s_StitchRegex = new Regex(@"^\s*=\s*(?<result>\w+)");
        private static readonly Regex s_LabelRegex = new Regex(@"^(?!\/\/).*[-*+]+\s*\(\s*(?<result>\w+)\s*\)");
        private static readonly Regex s_RedirectRegex = new Regex(@"->\s*(?<result>[\w\.]+)");
        private static readonly Regex s_TunnelRegex = new Regex(@"^(?!\/\/).*->->");

        private Dictionary<string, GraphEdge> Edges { get; } = new Dictionary<string, GraphEdge>();
        private Dictionary<string, GraphNode> NodeLookup { get; } = new Dictionary<string, GraphNode>();
        private Dictionary<string, GraphNode> DuplicateTunnelLookup { get; } = new Dictionary<string, GraphNode>();
        private Dictionary<string, GraphNode> ForceDuplicateLookup { get; } = new Dictionary<string, GraphNode>();
        private Dictionary<int, GraphNode> Nodes { get; } = new Dictionary<int, GraphNode>();

        private string CurrentKnot { get; set; }
        private int CurrentKnotId { get; set; }
        private int CurrentStitchId { get; set; }
        private string CurrentPath { get; set; }
        private GraphNode CurrentNode { get; set; }
        private NodeDepth Depth => m_Settings.ExportDepth;

        private readonly InkGraphSettings m_Settings;
        private int m_CurrentId = 1;

        public Graph(List<InkFileInfo> inkFiles, InkGraphSettings settings) {
            m_Settings = settings;
            
            SetRoot();

            // First pass to collect all nodes.
            foreach (InkFileInfo info in inkFiles) {
                for (int i = 0; i < info.Lines.Length; i++) {
                    string line = info.Lines[i];

                    if (s_TunnelRegex.IsMatch(line) && CurrentNode != null) {
                        Nodes[CurrentNode.StitchId].IsTunnel = true;
                    } else if (TryParse(s_KnotRegex, line, out string knot)) {
                        SetKnot(knot);
                    } else if (TryParse(s_StitchRegex, line, out string stitch)) {
                        SetStitch(stitch);
                    } else if (TryParse(s_LabelRegex, line, out string label)) {
                        SetLabel(label, $"{info.DisplayPath}:{i + 1}");
                    }
                }
            }

            // Add custom targets
            AddNode(NodeType.Knot, "DONE");
            AddNode(NodeType.Knot, "END");

            // Second pass to collect all edges.
            foreach (InkFileInfo info in inkFiles) {
                // Reset current node to 'root' node for each file.
                SetRoot();
                
                for (int i = 0; i < info.Lines.Length; i++) {
                    string line = info.Lines[i];

                    if (line.Trim().StartsWith("//")) {
                        continue;
                    }

                    if (TryParse(s_KnotRegex, line, out string knot)) {
                        SetKnot(knot);
                    } else if (TryParse(s_StitchRegex, line, out string stitch)) {
                        SetStitch(stitch);
                    } else if (TryParse(s_LabelRegex, line, out string label)) {
                        SetLabel(label, $"{info.DisplayPath}:{i + 1}");
                    }

                    MatchCollection matches = s_RedirectRegex.Matches(line);

                    foreach (Match match in matches) {
                        AddEdge(CurrentPath, match.Groups["result"].Value, $"{info.DisplayPath}:{i + 1}");
                    }
                }
            }
        }

        private int GetNextId() {
            return m_CurrentId++;
        }

        private void SetRoot() {
            CurrentPath = CurrentKnot = "<root>";
            CurrentNode = AddNode(NodeType.Knot, CurrentPath);
            CurrentKnotId = CurrentNode.KnotId = CurrentNode.StitchId = CurrentNode.Id;
        }

        private void SetKnot(string knot) {
            if (knot == "function") {
                return;
            }

            CurrentPath = CurrentKnot = knot;
            CurrentNode = AddNode(NodeType.Knot, CurrentPath);
            CurrentKnotId = CurrentNode.KnotId = CurrentNode.StitchId = CurrentNode.Id;
        }

        private void SetStitch(string stitch) {
            CurrentPath = CurrentKnot.IsNullOrEmpty() ? stitch : $"{CurrentKnot}.{stitch}";
            CurrentNode = AddNode(NodeType.Stitch, CurrentPath);
            CurrentNode.KnotId = CurrentKnotId;
            CurrentNode.StitchId = CurrentStitchId = CurrentNode.Id;
        }

        private void SetLabel(string label, string lineInfo) {
            string path = CurrentKnot.IsNullOrEmpty() ? label : $"{CurrentKnot}.{label}";
            CurrentNode = AddNode(NodeType.Label, path);
            CurrentNode.KnotId = CurrentKnotId;
            CurrentNode.StitchId = CurrentStitchId > 0 ? CurrentStitchId : CurrentKnotId;
            AddEdge(CurrentPath, path, lineInfo);
            CurrentPath = path;
        }

        private GraphNode AddNode(NodeType type, string path) {
            GraphNode node = GetOrCreateNode(type, path);
            Nodes[node.Id] = node;
            NodeLookup[node.Label] = node;
            node.IsExcluded = m_Settings.ExcludePaths.Contains(path);

            if (type == NodeType.Knot) {
                node.KnotId = node.Id;
                node.StitchId = node.Id;
            } else if (type == NodeType.Stitch) {
                node.StitchId = node.Id;
            }

            if (m_Settings.ForceTunnelPaths.Contains(path)) {
                node.IsTunnel = true;
            }

            return node;
        }

        private GraphNode GetOrCreateNode(NodeType type, string path) {
            return NodeLookup.TryGetValue(path, out GraphNode node) ? node : NodeLookup[path] = new GraphNode(type, GetNextId(), path);
        }

        private void AddEdge(string sourcePath, string targetPath, string label) {
            if (sourcePath.IsNullOrEmpty()) {
                Debug.LogWarning($"Null source path for {label}.");
                return;
            }

            if (targetPath.IsNullOrEmpty()) {
                Debug.LogWarning($"Null target path for {label}.");
                return;
            }

            if (!NodeLookup.TryGetValue(sourcePath, out GraphNode source)) {
                Debug.LogWarning($"Couldn't find node for '{sourcePath}'.");
                return;
            }

            if (!targetPath.Contains('.') && !CurrentKnot.IsNullOrEmpty()) {
                string testPath = $"{CurrentKnot}.{targetPath}";
                if (NodeLookup.ContainsKey(testPath)) {
                    targetPath = testPath;
                }
            }

            if (!NodeLookup.TryGetValue(targetPath, out GraphNode target)) {
                Debug.LogWarning($"Couldn't find node for '{targetPath}'.");
                return;
            }

            source = GetNodeAtDepth(source);
            target = GetNodeAtDepth(target);

            if (source.IsExcluded || target.IsExcluded) {
                return;
            }

            if (m_Settings.TunnelNodesHandling == TunnelNodesHandling.Remove && (target.IsTunnel || source.IsTunnel)) {
                return;
            }

            bool duplicateTunnel = false;

            // Duplicate tunnels if needed.
            if (m_Settings.TunnelNodesHandling == TunnelNodesHandling.Duplicate && target.IsTunnel && source != target) {
                string tunnelId = source.Label + "->" + target.Label;

                if (DuplicateTunnelLookup.ContainsKey(tunnelId)) {
                    // Already created this one.
                    return;
                }
                
                if (target.IsUsed) {
                    target = DuplicateNode(target);
                } else {
                    target.IsUsed = true;
                }
                    
                DuplicateTunnelLookup[tunnelId] = target;
                duplicateTunnel = true;
            } else if (m_Settings.ForceDuplicatePaths.Contains(target.Label)) {
                string forcedDuplicateId = source.Label + "->" + target.Label;

                if (ForceDuplicateLookup.ContainsKey(forcedDuplicateId)) {
                    // Already created this one.
                    return;
                }

                if (target.IsUsed) {
                    target = DuplicateNode(target);
                } else {
                    target.IsUsed = true;
                }

                ForceDuplicateLookup[forcedDuplicateId] = target;
            }

            AddEdge(source, target, label);

            if (duplicateTunnel) {
                AddEdge(target, source);
            }
        }

        private void AddEdge(GraphNode source, GraphNode target, string label = null) {
            int sourceId = source.GetIdAtDepth(Depth);
            int targetId = target.GetIdAtDepth(Depth);
            string edgeId = $"{sourceId}->{targetId}";

            if (Edges.TryGetValue(edgeId, out GraphEdge edge)) {
                if (!label.IsNullOrEmpty()) {
                    edge.Label += $"<br/>{label}";
                }
            } else {
                Edges[edgeId] = new GraphEdge(sourceId, targetId, label);
            }
        }

        private GraphNode GetNodeAtDepth(GraphNode node) {
            switch (Depth) {
                case NodeDepth.KnotsOnly:
                    node = Nodes[node.KnotId];
                    break;
                case NodeDepth.KnotsAndStitches:
                    node = Nodes[node.StitchId];
                    break;
                case NodeDepth.KnotsStitchesAndLabels:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return node;
        }

        private GraphNode DuplicateNode(GraphNode node) {
            GraphNode newNode = new GraphNode(node, GetNextId());
            Nodes[newNode.Id] = newNode;
            return newNode;
        }

        private static bool TryParse(Regex regex, string line, out string result) {
            Match match = regex.Match(line);
            result = null;

            if (!match.Success) {
                return false;
            }

            result = match.Groups["result"].Value;
            return true;
        }

        public string ToTgfText() {
            StringBuilder sb = new StringBuilder();

            foreach (GraphNode node in Nodes.Values) {
                if (!node.IsExcluded && node.IsAtDepth(Depth)) {
                    if (m_Settings.TunnelNodesHandling == TunnelNodesHandling.Remove && node.IsTunnel) {
                        continue;
                    }

                    sb.AppendLine($"{node.Id} {node.Label}");
                }
            }

            sb.AppendLine("#");

            foreach (GraphEdge edge in Edges.Values) {
                if (!Nodes.ContainsKey(edge.Source)) {
                    Debug.LogWarning($"Couldn't find source id for '{edge.Label}'.");
                    continue;
                }

                if (!Nodes.ContainsKey(edge.Target)) {
                    Debug.LogWarning($"Couldn't find target id for '{edge.Label}'.");
                    continue;
                }

                GraphNode source = GetNodeAtDepth(Nodes[edge.Source]);
                GraphNode target = GetNodeAtDepth(Nodes[edge.Target]);

                if (source.IsExcluded || target.IsExcluded) {
                    continue;
                }

                if (m_Settings.TunnelNodesHandling == TunnelNodesHandling.Remove && (source.IsTunnel || target.IsTunnel)) {
                    continue;
                }
                
                sb.AppendLine($"{source.Id} {target.Id} {edge.Label}");
            }

            return sb.ToString();
        }
    }
}