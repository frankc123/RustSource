using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class dfPivotExtensions
{
    public static Vector3 TransformToCenter(this dfPivotPoint pivot, Vector2 size)
    {
        switch (pivot)
        {
            case dfPivotPoint.TopLeft:
                return (Vector3) new Vector2(0.5f * size.x, 0.5f * -size.y);

            case dfPivotPoint.TopCenter:
                return (Vector3) new Vector2(0f, 0.5f * -size.y);

            case dfPivotPoint.TopRight:
                return (Vector3) new Vector2(0.5f * -size.x, 0.5f * -size.y);

            case dfPivotPoint.MiddleLeft:
                return (Vector3) new Vector2(0.5f * size.x, 0f);

            case dfPivotPoint.MiddleCenter:
                return (Vector3) new Vector2(0f, 0f);

            case dfPivotPoint.MiddleRight:
                return (Vector3) new Vector2(0.5f * -size.x, 0f);

            case dfPivotPoint.BottomLeft:
                return (Vector3) new Vector2(0.5f * size.x, 0.5f * size.y);

            case dfPivotPoint.BottomCenter:
                return (Vector3) new Vector2(0f, 0.5f * size.y);

            case dfPivotPoint.BottomRight:
                return (Vector3) new Vector2(0.5f * -size.x, 0.5f * size.y);
        }
        object[] objArray1 = new object[] { "Unhandled ", pivot.GetType().Name, " value: ", pivot };
        throw new Exception(string.Concat(objArray1));
    }

    public static Vector3 TransformToUpperLeft(this dfPivotPoint pivot, Vector2 size)
    {
        switch (pivot)
        {
            case dfPivotPoint.TopLeft:
                return (Vector3) new Vector2(0f, 0f);

            case dfPivotPoint.TopCenter:
                return (Vector3) new Vector2(0.5f * -size.x, 0f);

            case dfPivotPoint.TopRight:
                return (Vector3) new Vector2(-size.x, 0f);

            case dfPivotPoint.MiddleLeft:
                return (Vector3) new Vector2(0f, 0.5f * size.y);

            case dfPivotPoint.MiddleCenter:
                return (Vector3) new Vector2(0.5f * -size.x, 0.5f * size.y);

            case dfPivotPoint.MiddleRight:
                return (Vector3) new Vector2(-size.x, 0.5f * size.y);

            case dfPivotPoint.BottomLeft:
                return (Vector3) new Vector2(0f, size.y);

            case dfPivotPoint.BottomCenter:
                return (Vector3) new Vector2(0.5f * -size.x, size.y);

            case dfPivotPoint.BottomRight:
                return (Vector3) new Vector2(-size.x, size.y);
        }
        object[] objArray1 = new object[] { "Unhandled ", pivot.GetType().Name, " value: ", pivot };
        throw new Exception(string.Concat(objArray1));
    }

    public static Vector3 UpperLeftToTransform(this dfPivotPoint pivot, Vector2 size)
    {
        return pivot.TransformToUpperLeft(size).Scale(-1f, -1f, 1f);
    }
}

