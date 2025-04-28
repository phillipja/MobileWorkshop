using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum EvaluationType
{
    Linear,
    Step,
    EaseInOut,
    Sin,
}

public abstract class EvaluationOption
{
    public abstract bool CanAddVariable { get; }
    public abstract bool CanRemoveVariable { get; }
    public abstract void AddVariable();
    public abstract void RemoveVariable();
    public abstract IEnumerable<FloatRange> GetVariables();
    public abstract void SetVariables(float[] variables);

    public abstract float Evaluate(float t);

    public static EvaluationOption CreateInstance(EvaluationType type)
    {
        return type switch
        {
            EvaluationType.Linear => new LinearEvaluation(),
            EvaluationType.Step => new StepEvaluation(),
            EvaluationType.EaseInOut => new EaseEvaluation(),
            EvaluationType.Sin => new SinEvaluation(),
            _ => throw new System.NotImplementedException($"Type '{type}' not implemented."),
        };
    }

    public EvaluationType GetEvaluationType()
    {
        return this switch
        {
            LinearEvaluation => EvaluationType.Linear,
            StepEvaluation => EvaluationType.Step,
            EaseEvaluation => EvaluationType.EaseInOut,
            SinEvaluation => EvaluationType.Sin,
            _ => throw new System.NotImplementedException($"No evaluation type for '{this.GetType().Name}'"),
        };
    }
}

public class LinearEvaluation : EvaluationOption
{
    private float a;

    public LinearEvaluation()
    {
        a = 1.0f;
    }

    public override bool CanAddVariable => false;
    public override bool CanRemoveVariable => false;

    public override void AddVariable() { }
    public override void RemoveVariable() { }

    public override IEnumerable<FloatRange> GetVariables()
    {
        yield return new()
        {
            start = 0.0f,
            end = 1.0f,
            value = a,
        };
    }
    public override void SetVariables(float[] variables)
    {
        if (variables == null || variables.Length == 0)
        {
            Debug.LogWarning($"Invalid variables parameter.");
            return;
        }

        a = variables[0];
    }

    public override float Evaluate(float t)
    {
        return a * t;
    }
}

public class StepEvaluation : EvaluationOption
{
    const int MAX_VAR = 5;

    private List<(float, float)> _variables;

    public StepEvaluation()
    {
        _variables = new List<(float, float)>(MAX_VAR)
        {
            (0.0f, 0.0f),
            (0.33f, 0.5f),
            (0.66f, 1.0f),
        };
    }

    public override bool CanAddVariable => _variables.Count < MAX_VAR;
    public override bool CanRemoveVariable => _variables.Count > 2;
    public override void AddVariable()
    {
        _variables.Add((1.0f, 1.0f));
    }
    public override void RemoveVariable()
    {
        _variables.Remove(_variables[^1]);
    }

    public override IEnumerable<FloatRange> GetVariables()
    {
        foreach (var variable in _variables)
        {
            yield return new()
            {
                start = 0.0f,
                end = 1.0f,
                value = variable.Item1,
            };
            yield return new()
            {
                start = 0.0f,
                end = 1.0f,
                value = variable.Item2,
            };
        }
    }

    public override void SetVariables(float[] variables)
    {
        if (variables == null || variables.Length == 0
            || variables.Length % 2 != 0
            || variables.Length > MAX_VAR * 2)
        {
            Debug.LogWarning($"Invalid variables parameter.");
            return;
        }

        _variables.Clear();
        for (int i = 0; i < variables.Length; i += 2)
        {
            _variables.Add((variables[i], variables[i + 1]));
        }

        _variables = _variables.OrderBy(t => t.Item1).ToList();
    }

    public override float Evaluate(float t)
    {
        for (int i = _variables.Count - 1; i > -1; i--)
        {
            if (_variables[i].Item1 <= t)
            {
                return _variables[i].Item2;
            }
        }

        return 0.0f;
    }
}

public class EaseEvaluation : EvaluationOption
{
    const float MIN = 2.0f;
    const float MAX = 5.0f;

    private float a;

    public EaseEvaluation()
    {
        a = 2f;
    }

    public override bool CanAddVariable => false;
    public override bool CanRemoveVariable => false;
    public override void AddVariable() { }
    public override void RemoveVariable() { }

    public override IEnumerable<FloatRange> GetVariables()
    {
        yield return new()
        {
            start = MIN,
            end = MAX,
            value = a
        };
    }

    public override void SetVariables(float[] variables)
    {
        if (variables == null || variables.Length == 0)
        {
            Debug.LogWarning($"Invalid variables parameter.");
            return;
        }

        a = Mathf.Clamp(Mathf.Round(variables[0]), MIN, MAX);
    }

    public override float Evaluate(float t)
    {
        return t < 0.5f
            ? Mathf.Pow(2.0f, a - 1.0f) * Mathf.Pow(t, a)
            : 1.0f - Mathf.Pow(-2.0f * t + 2.0f, a) / 2.0f;
    }
}

public class SinEvaluation : EvaluationOption
{
    const int MAX_VAR = 3;
    const float MAX = 17.0f;

    const float PHASE = 3.0f * Mathf.PI / 2.0f;

    private List<float> _variables;

    public SinEvaluation()
    {
        _variables = new List<float>(MAX_VAR)
        {
            2.0f,
            5.0f,
        };
    }

    public override bool CanAddVariable => _variables.Count < MAX_VAR;
    public override bool CanRemoveVariable => _variables.Count > 1;
    public override void AddVariable()
    {
        _variables.Add(1.0f);
    }
    public override void RemoveVariable()
    {
        _variables.Remove(_variables[^1]);
    }

    public override IEnumerable<FloatRange> GetVariables()
    {
        foreach (var variable in _variables)
        {
            yield return new()
            {
                start = 1f,
                end = MAX,
                value = variable,
            };
        }
    }

    public override void SetVariables(float[] variables)
    {
        if (variables == null || variables.Length == 0
            || variables.Length > MAX_VAR)
        {
            Debug.LogWarning($"Invalid variables parameter.");
            return;
        }

        _variables.Clear();
        foreach (var variable in variables)
        {
            _variables.Add(variable);
        }
    }

    public override float Evaluate(float t)
    {
        var sinVal = 0.0f;
        foreach (var a in _variables)
        {
            sinVal += Mathf.Sin(PHASE + a * t * Mathf.PI);
        }

        return 0.5f + (sinVal / (_variables.Count * 2.0f));
    }
}

public struct FloatRange
{
    public float start;
    public float end;
    public float value;
}
