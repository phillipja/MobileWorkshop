using System.Collections.Generic;
using UnityEngine;

namespace Graph
{
    public class SO_Graph : ScriptableObject
    {
        const float LIN_LIMIT = .45f;
        const float EXP_LIMIT = .7f;
        const float QUAD_LIMIT = .9f;

        public List<SO_GraphCategory> categories = new();
        public List<SO_GraphNode> nodes = new();
        public List<SO_GraphEdge> edges = new();

        public void GenerateEdgeFunctions()
        {
            foreach(SO_GraphEdge edge in edges)
            {
                float r = Random.Range(0f, 1f);
                MathFunc funcType = r < LIN_LIMIT
                    ? MathFunc.Linear
                    : r < EXP_LIMIT
                    ? MathFunc.Exponential
                    : r < QUAD_LIMIT
                    ? MathFunc.Quadratic
                    : MathFunc.Sinus;

                edge.GenerateConstants(funcType);
            }
        }
    }
}
