using UnityEngine;

public enum AnimationMode
{
    rough,
    easeIn,
    easeOut,
    easeInOut
}

public class Fluid
{
    public static float Lerp(float a, float b, float t, AnimationMode mode)
    {
        return Mathf.Lerp(a, b, Get(t, mode));
    }

    public static float LerpAngle(float a, float b, float t, AnimationMode mode)
    {
        return Mathf.LerpAngle(a, b, Get(t, mode));
    }

    public static Quaternion Lerp(Quaternion a, Quaternion b, float t, AnimationMode mode)
    {
        return Quaternion.Lerp(a, b, Get(t, mode));
    }

    public static Vector3 Lerp(Vector3 a, Vector3 b, float t, AnimationMode mode)
    {
        return Vector3.Lerp(a, b, Get(t, mode));
    }

    public static float Get(float t, AnimationMode mode)
    {
        switch (mode)
        {
            case AnimationMode.rough: return t;
            case AnimationMode.easeInOut: return t * t * (3.0f - 2.0f * t);
            case AnimationMode.easeIn:
                if (t <= 0.5f)
                    return 2 * t * t;
                else
                    return t;
            case AnimationMode.easeOut:
                if (t <= 0.5f)
                    return t;
                else
                {
                    t -= 0.5f;
                    return 2.0f * t * (1.0f - t) + 0.5f;
                }
        }

        return t;
    }
}
