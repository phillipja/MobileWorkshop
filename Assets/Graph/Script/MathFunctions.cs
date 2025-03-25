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


    public static float EvaluateExponential(float x)
    {
        return x == 0 ? 0f : (float)Math.Pow(2, 10 * x - 10);
    }

    public static float EvaluateLinear(float x)
    {
        return x;
    }

    public static float EvaluateQuadratic(float x)
    {
        return x * x;
    }

    public static float EvaluateSin(float x)
    {
        return -((float)Math.Cos(Math.PI * x) - 1f) / 2f;
    }


}
