using System.Collections.Generic;
using UnityEngine;

namespace Graph
{
    public class SO_GraphNode : ScriptableObject
    {
        const float LIN_LIMIT = .45f;
        const float EXP_LIMIT = .7f;
        const float QUAD_LIMIT = .9f;

        public SO_GraphCategory category;
        public int index;
        public string description;

        public List<SO_GraphNode> fromEdges = new(2);
        public List<MathModeledEdge> toEdges = new(2);

        public string id => $"{category.id}{index}";

        private void OnEnable()
        {
            Debug.Log("Enable");
        }

        private void OnDisable()
        {
            Debug.Log("Disable");
        }

        public void GenerateEdgeFunctions()
        {
            foreach(MathModeledEdge edge in toEdges)
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
