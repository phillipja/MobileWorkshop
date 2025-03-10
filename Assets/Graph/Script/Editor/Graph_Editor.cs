using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Graph
{
    [CustomEditor(typeof(SO_Graph))]
    public class Graph_Editor : Editor
    {
        SO_Graph _graph;

        private void OnEnable()
        {
            _graph = (SO_Graph)target;
        }

        public override void OnInspectorGUI()
        {
            if(GUILayout.Button("Regenerate Edge Functions"))
            {
                _graph.GenerateEdgeFunctions();
                EditorUtility.SetDirty(_graph);
                AssetDatabase.SaveAssetIfDirty(_graph);
            }

            base.OnInspectorGUI();
        }
    }
}
