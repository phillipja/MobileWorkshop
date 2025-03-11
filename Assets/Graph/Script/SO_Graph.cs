using System.Collections.Generic;
using UnityEngine;

namespace Graph
{
    public class SO_Graph : ScriptableObject
    {
        public List<SO_GraphCategory> categories = new();
        public List<SO_GraphNode> nodes = new();

        public void GenerateEdgeFunctions()
        {
            foreach(SO_GraphNode node in nodes)
            {

                node.GenerateEdgeFunctions();
            }
        }
    }
}
