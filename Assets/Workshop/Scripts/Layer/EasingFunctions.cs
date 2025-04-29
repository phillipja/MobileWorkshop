using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EasingFunctions
{
    public static float Evaluate(this EasingType type, float t)
    {
        return type switch
        {
            EasingType.EaseInSine => EaseInSine(t),
            EasingType.EaseOutSine => EaseOutSine(t),
            EasingType.EaseInOutSine => EaseInOutSine(t),
            EasingType.EaseInQuad => EaseInQuad(t),
            EasingType.EaseOutQuad => EaseOutQuad(t),
            EasingType.EaseInOutQuad => EaseInOutQuad(t),
            EasingType.EaseInCubic => EaseInCubic(t),
            EasingType.EaseOutCubic => EaseOutCubic(t),
            EasingType.EaseInOutCubic => EaseInOutCubic(t),
            EasingType.EaseInQuart => EaseInQuart(t),
            EasingType.EaseOutQuart => EaseOutQuart(t),
            EasingType.EaseInOutQuart => EaseInOutQuart(t),
            EasingType.EaseInQuint => EaseInQuint(t),
            EasingType.EaseOutQuint => EaseOutQuint(t),
            EasingType.EaseInOutQuint => EaseInOutQuint(t),
            EasingType.EaseInExpo => EaseInExpo(t),
            EasingType.EaseOutExpo => EaseOutExpo(t),
            EasingType.EaseInOutExpo => EaseInOutExpo(t),
            EasingType.EaseInCirc => EaseInCirc(t),
            EasingType.EaseOutCirc => EaseOutCirc(t),
            EasingType.EaseInOutCirc => EaseInOutCirc(t),
            _ => t,
        };
    }
    //=========================================================================
    public static float EaseInSine(float t)
    {
        return 1 - Mathf.Cos((t * Mathf.PI) / 2);
    }
    public static float EaseOutSine(float t)
    {
        return Mathf.Sin((t * Mathf.PI) / 2);
    }
    public static float EaseInOutSine(float t)
    {
        return -(Mathf.Cos(Mathf.PI * t) - 1) / 2;
    }
    //=========================================================================
    public static float EaseInQuad(float t)
    {
        return t * t;
    }
    public static float EaseOutQuad(float t)
    {
        return t * (2 - t);
    }
    public static float EaseInOutQuad(float t)
    {
        return t < 0.5f 
            ? 2 * t * t
            : - 1 + (4 * t) - (2 * t * t);
    }
    //=========================================================================
    public static float EaseInCubic(float t)
    {
        return t * t * t;
    }
    public static float EaseOutCubic(float t)
    {
        return 1 - Mathf.Pow(1 - t, 3);
    }
    public static float EaseInOutCubic(float t)
    {
        return t < 0.5f 
            ? 4 * t * t * t 
            : 1 - Mathf.Pow(-2 * t + 2, 3) / 2;
    }
    //=========================================================================
    public static float EaseInQuart(float t)
    {
        return t * t * t * t;
    }
    public static float EaseOutQuart(float t)
    {
        return 1 - Mathf.Pow(1 - t, 4);
    }
    public static float EaseInOutQuart(float t)
    {
        return t < 0.5f 
            ? 8 * t * t * t * t 
            : 1 - Mathf.Pow(-2 * t + 2, 4) / 2;
    }
    //=========================================================================
    public static float EaseInQuint(float t)
    {
        return t * t * t * t * t;
    }
    public static float EaseOutQuint(float t)
    {
        return 1 - Mathf.Pow(1 - t, 5);
    }
    public static float EaseInOutQuint(float t)
    {
        return t < 0.5f 
            ? 16 * t * t * t * t * t 
            : 1 - Mathf.Pow(-2 * t + 2, 5) / 2;
    }
    //=========================================================================
    public static float EaseInExpo(float t)
    {
        return t == 0 ? 0 : Mathf.Pow(2, 10 * (t - 1));
    }
    public static float EaseOutExpo(float t)
    {
        return t == 1 ? 1 : 1 - Mathf.Pow(2, -10 * t);
    }
    public static float EaseInOutExpo(float t)
    {
        if(t == 0) return 0;
        if(t == 1) return 1;
        return t < 0.5f 
            ? Mathf.Pow(2, (20 * t) - 10) / 2 
            : (2 - Mathf.Pow(2, -20 * t + 10)) / 2;
    }
    //=========================================================================
    public static float EaseInCirc(float t)
    {
        return 1 - Mathf.Sqrt(1 - Mathf.Pow(t, 2));
    }
    public static float EaseOutCirc(float t)
    {
        return Mathf.Sqrt(1 - Mathf.Pow(t - 1, 2));
    }
    public static float EaseInOutCirc(float t)
    {
        return t < 0.5f 
            ? (1 - Mathf.Sqrt(1 - Mathf.Pow(2 * t, 2))) / 2 
            : (Mathf.Sqrt(1 - Mathf.Pow(-2 * t + 2, 2)) + 1) / 2;
    }
    //=========================================================================
}

public enum EasingType
{
    None,
    EaseInSine,
    EaseOutSine,
    EaseInOutSine,
    EaseInQuad,
    EaseOutQuad,
    EaseInOutQuad,
    EaseInCubic,
    EaseOutCubic,
    EaseInOutCubic,
    EaseInQuart,
    EaseOutQuart,
    EaseInOutQuart,
    EaseInQuint,
    EaseOutQuint,
    EaseInOutQuint,
    EaseInExpo,
    EaseOutExpo,
    EaseInOutExpo,
    EaseInCirc,
    EaseOutCirc,
    EaseInOutCirc,
}
