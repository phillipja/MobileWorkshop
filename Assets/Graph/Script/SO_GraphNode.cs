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

        public List<MathModeledEdge> fromEdges = new(2);
        public List<SO_GraphNode> toEdges = new(2);

        private float _bufferValue;

        public string id => $"{category.id}{index}";

        private void OnEnable()
        {
            ClearBufferValue();
        }

        public void SetBufferValue(float value)
        {
            _bufferValue = value;
        }

        public void ClearBufferValue()
        {
            _bufferValue = 0f;
        }

        public float GetValue(bool normalized)
        {
            if(_bufferValue > 0f)
            {
                return _bufferValue;
            }

            foreach(var edge in fromEdges)
            {
                _bufferValue += edge.Evaluate(normalized);
            }

            if(normalized && fromEdges.Count > 0)
            {
                _bufferValue /= fromEdges.Count;
            }

            return _bufferValue;
        }

        public void GenerateEdgeFunctions()
        {
            foreach(MathModeledEdge edge in fromEdges)
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
