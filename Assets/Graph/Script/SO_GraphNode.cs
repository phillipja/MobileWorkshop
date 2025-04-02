using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
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

        private float? _bufferValue;

        public string id => $"{category.id}{index}";

        private void OnEnable()
        {
            ClearBufferValue();
            foreach(var edge in fromEdges)
            {
                edge.IsActive = false;
            }
        }

        public void SetBufferValue(float value)
        {
            _bufferValue = value;
        }

        public void ClearBufferValue()
        {
            _bufferValue = null;
        }

        public float GetValue(Settings settings)
        {
            if(_bufferValue.HasValue)
                return _bufferValue.Value;

            _bufferValue = 0f;

            int amount = 0;
            foreach(var edge in fromEdges.Where(e => e.IsActive))
            {
                _bufferValue += edge.Evaluate(settings);
                amount++;
            }

            //if(settings.useNormalizedValue && amount > 0)
            if(amount > 0)
            {
                _bufferValue /= amount;
            }

            return _bufferValue.Value;
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

        public void GenerateFunctions()
        {
            foreach(var edge in fromEdges)
            {
                if(edge.weight == Weight.None
                    || edge.strength == Strength.None)
                    continue;

                float r = Random.Range(0f, 1f);
                MathFunc func = edge.strength switch
                {
                    Strength.Weak => r < .45f ? MathFunc.Quad_A
                            : r < .9f ? MathFunc.Sigmuid_A : MathFunc.Bounce_A,

                    Strength.Medium => r < .5f ? MathFunc.Linear : MathFunc.Sawtooth,

                    Strength.Strong => r < .45f ? MathFunc.Quad_B
                            : r < .9f ? MathFunc.Sigmuid_B : MathFunc.Bounce_B,

                    _ => MathFunc.None,
                };

                edge.SetMathFunc(func);
            }
        }
    }
}
