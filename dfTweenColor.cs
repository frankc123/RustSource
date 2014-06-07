using System;
using UnityEngine;

[AddComponentMenu("Daikon Forge/Tweens/Color")]
public class dfTweenColor : dfTweenComponent<Color>
{
    public override Color evaluate(Color startValue, Color endValue, float time)
    {
        Vector4 vector = (Vector4) startValue;
        Vector4 vector2 = (Vector4) endValue;
        float x = dfTweenComponent<Color>.Lerp(vector.x, vector2.x, time);
        float y = dfTweenComponent<Color>.Lerp(vector.y, vector2.y, time);
        float z = dfTweenComponent<Color>.Lerp(vector.z, vector2.z, time);
        return new Vector4(x, y, z, dfTweenComponent<Color>.Lerp(vector.w, vector2.w, time));
    }

    public override Color offset(Color lhs, Color rhs)
    {
        return (lhs + rhs);
    }
}

