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

    public static float EvaluateQuadratic(float x)
    {
        return x * x;
    }

    public static float EvaluateSin(float x)
    {
        return -((float)Math.Cos(Math.PI * x) - 1f) / 2f;
    }

    public static float EvaluateLinear(float x)
    {
        return x;
    }

    public static float EvaluateSawtooth(float x)
    {
        return x < .33f
            ? 0f
            : x < .66f
            ? .5f
            : 1f;
    }

    public static float EvaluateQuad_A(float x)
    {
        return x * x;
    }

    public static float EvaluateQuad_B(float x)
    {
        return (float) Math.Pow(x, 0.5f);
    }

    public static float EvaluateSigmuide_A(float x)
    {
        if(x < 0)
            throw new ArgumentException("This evaluation doesn't work with values smaller 0!");

        return x < .5f
            ? .5f - (float)Math.Pow(2f * (1 - x) - 1, 1f / 3f) / 2f
            : .5f + (float)Math.Pow(2f * x - 1f, 1f / 3f) / 2f;
    }

    public static float EvaluateSigmuide_B(float x)
    {
        return .5f + (float)Math.Pow(2f * x - 1f, 3f) / 2f;
    }

    public static float EvaluateBounce_A(float x)
    {
        return 1 - EvaluateBounce_B(1 - x);
    }

    public static float EvaluateBounce_B(float x)
    {
        const float n1 = 7.5625f;
        const float d1 = 2.75f;

        if(x < 1f / d1)
        {
            return n1 * x * x;
        }
        else if(x < 2f / d1)
        {
            return n1 * (x -= 1.5f / d1) * x + 0.75f;
        }
        else if(x < 2.5f / d1)
        {
            return n1 * (x -= 2.25f / d1) * x + 0.9375f;
        }
        else
        {
            return n1 * (x -= 2.625f / d1) * x + 0.984375f;
        }
    }
}
