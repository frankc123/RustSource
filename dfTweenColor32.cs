using System;
using UnityEngine;

[AddComponentMenu("Daikon Forge/Tweens/Color32")]
public class dfTweenColor32 : dfTweenComponent<Color32>
{
    public override Color32 evaluate(Color32 startValue, Color32 endValue, float time)
    {
        Vector4 vector = (Vector4) startValue;
        Vector4 vector2 = (Vector4) endValue;
        float x = dfTweenComponent<Color32>.Lerp(vector.x, vector2.x, time);
        float y = dfTweenComponent<Color32>.Lerp(vector.y, vector2.y, time);
        float z = dfTweenComponent<Color32>.Lerp(vector.z, vector2.z, time);
        Vector4 vector3 = new Vector4(x, y, z, dfTweenComponent<Color32>.Lerp(vector.w, vector2.w, time));
        return (Color32) vector3;
    }

    public override Color32 offset(Color32 lhs, Color32 rhs)
    {
        return (lhs + rhs);
    }
}

