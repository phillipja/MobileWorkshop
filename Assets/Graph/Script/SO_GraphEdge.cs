using UnityEngine;

namespace Graph
{
    public class SO_GraphEdge : ScriptableObject
    {
        public SO_GraphNode from;
        public SO_GraphNode to;
        public Weight weight;

        [SerializeField] private MathFunc _funcType;
        [SerializeField] private float[] _constants;

        public void GenerateConstants(MathFunc funcType)
        {
            _funcType = funcType;
            switch(_funcType)
            {
                case MathFunc.Linear:
                    _constants = new float[2];
                    _constants[0] = DetermineBasedOnWeight(.1f, 2f, -2f, -.1f, 0f);
                    _constants[1] = Random.Range(-1f, 1f);
                    break;

                case MathFunc.Exponential:
                    _constants = new float[2];
                    _constants[0] = Random.Range(.5f, 2f);
                    _constants[1] = DetermineBasedOnWeight(.1f, 1f, -1f, -.1f, 0f);
                    break;

                case MathFunc.Quadratic:
                    _constants = new float[3];
                    _constants[0] = DetermineBasedOnWeight(.1f, 1f, -1f, -.1f, 0f);
                    _constants[1] = Random.Range(-1f, 1f);
                    _constants[2] = Random.Range(-1f, 1f);
                    break;

                case MathFunc.Sinus:
                    _constants = new float[4];
                    _constants[0] = DetermineBasedOnWeight(.5f, 2f, -2f, -.5f, 0f);
                    _constants[1] = Random.Range(-5f, 2f);
                    _constants[2] = Random.Range(0, Mathf.PI);
                    _constants[3] = Random.Range(-1f, 1f);
                    break;

                default:
                    break;
            }

            float DetermineBasedOnWeight(float posMin, float posMax, float negMin, float negMax, float none)
            {
                switch(weight)
                {
                    case Weight.Positive:
                        return Random.Range(posMin, posMax);

                    case Weight.Negative:
                        return Random.Range(negMin, negMax);

                    case Weight.None:
                    default:
                        return none;
                }
            }
        }
    }
}


