using System;
using UnityEngine;

[AddComponentMenu(""), RequireComponent(typeof(CCDesc))]
public sealed class CCTotemicFigure : CCTotem<CCTotem.TotemicFigure, CCTotemicFigure>
{
    private void OnDrawGizmos()
    {
        if (base.totemicObject != null)
        {
            float f = (3.141593f * Time.time) + (0.7853982f * base.totemicObject.BottomUpIndex);
            Vector3 vector = Camera.current.cameraToWorldMatrix.MultiplyVector(new Vector3((Mathf.Sin(f) * 0.25f) + 0.75f, 0f, 0f));
            Vector3 vector2 = -vector;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(base.totemicObject.TopOrigin, base.totemicObject.TopOrigin + vector);
            Gizmos.DrawLine(base.totemicObject.SlideTopOrigin + vector, base.totemicObject.TopOrigin + vector);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(base.totemicObject.BottomOrigin, base.totemicObject.BottomOrigin + vector2);
            Gizmos.DrawLine(base.totemicObject.BottomOrigin + vector2, base.totemicObject.SlideBottomOrigin + vector2);
            Gizmos.color = ((base.totemicObject.BottomUpIndex & 1) != 1) ? Color.red : new Color(1f, 0.4f, 0.4f, 1f);
            Gizmos.DrawLine(base.totemicObject.SlideBottomOrigin + vector, base.totemicObject.SlideBottomOrigin + vector2);
            Gizmos.DrawLine(base.totemicObject.SlideTopOrigin + vector, base.totemicObject.SlideTopOrigin + vector2);
            Gizmos.DrawLine(base.totemicObject.SlideBottomOrigin + vector2, base.totemicObject.SlideTopOrigin + vector2);
            Gizmos.DrawLine(base.totemicObject.SlideBottomOrigin + vector, base.totemicObject.SlideTopOrigin + vector);
        }
    }
}

