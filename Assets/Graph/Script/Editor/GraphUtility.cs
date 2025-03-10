using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Graph
{
    public class GraphUtility : EditorWindow
    {
        const string TEXT_ASSET_BEGINNING = "flowchart";

        const string ASSETS_PATH = "Assets/";
        const string PATH_ENDING = "/";
        const string FALLBACK_ASSET_NAME = "Graph";
        const string FILE_EXTENTION = ".asset";

        const string CATEGORY_PREFIX = "Category_";
        const string NODE_PREFIX = "Node_";
        const string EDGE_PREFIX = "Edge_";

        const string COMMENT = @"(?s)%%.*?\r?\n";
        const string SUBGRAPH = @"subgraph\s+(\w+)?\s*\[""(.+?)\((?:.|\n|\r)+?end(?:\s)";
        const string NODE = @"(\d+)\((.+?)\)";
        const string EDGE = @"(\w+\d+)\s*-->\s*\|(neg|pos)\|\s*(\w+\d+)";

        private string _path;
        private string _assetName;
        private TextAsset _flowchart;
        private bool _showDebug;


        [MenuItem("Graph/GraphUtility")]
        public static void ShowWindow()
        {
            GetWindow<GraphUtility>("Graph Utility");
        }

        private void OnGUI()
        {
            _path = EditorGUILayout.TextField("Path", _path);
            _assetName = EditorGUILayout.TextField("Name", _assetName);
            _flowchart = (TextAsset)EditorGUILayout.ObjectField("Mermaid Diagram", _flowchart, typeof(TextAsset), false);
            _showDebug = EditorGUILayout.Toggle("Show Debug", _showDebug);

            if(GUILayout.Button("Generate Graph"))
            {
                if(_flowchart == null)
                {
                    Debug.LogError("Please select a text asset.");
                    return;
                }

                GenerateGraph();
            }
        }

        private void GenerateGraph()
        {
            string cleanedText = Regex.Replace(_flowchart.text, COMMENT, "").Trim();
            if(cleanedText.StartsWith(TEXT_ASSET_BEGINNING) == false)
            {
                Debug.LogError("Please select a valid mermaid flowchart text asset.");
                return;
            }

            SO_Graph graph = CreateInstance<SO_Graph>();

            ValidatePath();
            ValidateAssetName();

            var path = $"{ASSETS_PATH}{_path}{_assetName}";
            AssetDatabase.CreateAsset(graph, path);

            //Reload graph to work with the asset representation
            graph = AssetDatabase.LoadAssetAtPath<SO_Graph>(path);

            GenerateCategoriesAndNodes(graph, cleanedText);
            GenerateEdges(graph, cleanedText);

            graph.GenerateEdgeFunctions();
            EditorUtility.SetDirty(graph);
            AssetDatabase.SaveAssetIfDirty(graph);
        }

        private void ValidatePath()
        {
            if(string.IsNullOrWhiteSpace(_path) == false
                && _path.EndsWith(PATH_ENDING) == false)
            {
                _path += PATH_ENDING;
            }
        }

        private void ValidateAssetName()
        {
            if(string.IsNullOrEmpty(_assetName))
            {
                _assetName = $"{FALLBACK_ASSET_NAME}{FILE_EXTENTION}";
            }

            if(_assetName.EndsWith(FILE_EXTENTION) == false)
            {
                _assetName += FILE_EXTENTION;
            }
        }

        private void GenerateCategoriesAndNodes(SO_Graph graph, string cleanedText)
        {
            MatchCollection subgraphMatches = Regex.Matches(cleanedText, SUBGRAPH);
            foreach(Match categoryMatch in subgraphMatches)
            {
                SO_GraphCategory category = CreateInstance<SO_GraphCategory>();
                category.id = categoryMatch.Groups[1].Value.ToUpper();
                category.description = categoryMatch.Groups[2].Value.Trim();
                category.name = $"{CATEGORY_PREFIX}{category.id}";

                graph.categories.Add(category);
                AssetDatabase.AddObjectToAsset(category, graph);

                string subgraph = categoryMatch.Groups[0].Value;
                if(_showDebug)
                {
                    Debug.Log($"Found category '{category.id}' - '{category.description}'");
                    Debug.Log(subgraph);
                }

                MatchCollection nodeMatches = Regex.Matches(subgraph, NODE);
                foreach(Match nodeMatch in nodeMatches)
                {
                    SO_GraphNode node = CreateInstance<SO_GraphNode>();
                    node.category = category;
                    node.index = int.Parse(nodeMatch.Groups[1].Value);
                    node.description = nodeMatch.Groups[2].Value;
                    node.name = $"{NODE_PREFIX}{node.id}";

                    graph.nodes.Add(node);
                    AssetDatabase.AddObjectToAsset(node, graph);

                    if(_showDebug)
                    {
                        Debug.Log($"Found node '{node.id}' - '{node.description}'");
                    }
                }
            }

            AssetDatabase.SaveAssetIfDirty(graph);
        }

        private void GenerateEdges(SO_Graph graph, string cleanedText)
        {
            const string positiveWeight = "pos";
            const string negativeWeight = "neg";

            MatchCollection edgeMatches = Regex.Matches(cleanedText, EDGE);
            foreach(Match edgeMatch in edgeMatches)
            {
                string from = edgeMatch.Groups[1].Value.ToUpper();
                string to = edgeMatch.Groups[3].Value.ToUpper();
                string weightInfo = edgeMatch.Groups[2].Value;

                SO_GraphNode fromNode = graph.nodes.FirstOrDefault(n => n.id.Equals(from));
                SO_GraphNode toNode = graph.nodes.FirstOrDefault(n => n.id.Equals(to));

                bool isValid = fromNode != null && toNode != null;

                if(_showDebug)
                {
                    Debug.Log($"Found edge '{from}' - '{to}'. Is Valid: {isValid}");
                }

                if(isValid == false)
                    continue;

                SO_GraphEdge edge = CreateInstance<SO_GraphEdge>();
                edge.from = fromNode;
                edge.to = toNode;
                edge.weight = DetermineWeight(weightInfo);
                edge.name = $"{EDGE_PREFIX}{from}{weightInfo}{to}";

                graph.edges.Add(edge);
                AssetDatabase.AddObjectToAsset(edge, graph);
            }

            AssetDatabase.SaveAssetIfDirty(graph);

            Weight DetermineWeight(string info)
            {
                if(info.Equals(positiveWeight))
                {
                    return Weight.Positive;
                }

                if(info.Equals(negativeWeight))
                {
                    return Weight.Negative;
                }

                return Weight.None;
            }
        }
    }
}
