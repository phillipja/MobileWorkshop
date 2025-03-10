using System;

public static class MathFunctions
{
    public static float EvaluateExponential(float a, float b, float x)
    {
        return a * (float)Math.Pow(Math.E, b * x);
    }

    public static float EvaluateLinear(float a, float b, float x)
    {
        return a * x + b;
    }

    public static float EvaluateQuadratic(float a, float b, float c, float x)
    {
        return a * x * x + b * x + c;
    }

    public static float EvaluateSin(float a, float b, float c, float d, float x)
    {
        return a * (float)Math.Sin(b * x + c) + d;
    }
}
