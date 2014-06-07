using System;
using UnityEngine;

public class TransformLerpTest : MonoBehaviour
{
    public Transform a;
    public float angleXY;
    public float angleYZ;
    public float angleZX;
    public Transform b;
    public bool cap;
    public bool inverse0;
    public bool inverse1;
    [SerializeField]
    private SlerpMode mode;
    public float t;
    public bool transpose;

    private static void DrawAxes(Matrix4x4 m, float alpha)
    {
        Vector3 center = m.MultiplyPoint(Vector3.zero);
        Gizmos.color = new Color(1f, 1f, 1f, alpha);
        Gizmos.DrawSphere(center, 0.01f);
        Gizmos.color = new Color(1f, 0f, 0f, alpha);
        Gizmos.DrawLine(center, m.MultiplyPoint(Vector3.right));
        Gizmos.color = new Color(0f, 1f, 0f, alpha);
        Gizmos.DrawLine(center, m.MultiplyPoint(Vector3.up));
        Gizmos.color = new Color(0f, 0f, 1f, alpha);
        Gizmos.DrawLine(center, m.MultiplyPoint(Vector3.forward));
    }

    private Matrix4x4 GetMatrix(Transform a)
    {
        switch (this.mode)
        {
            case SlerpMode.WorldToCameraSlerp:
            case SlerpMode.WorldToCameraLerp:
            case SlerpMode.WorldToCameraSlerp2:
                return a.camera.worldToCameraMatrix;

            case SlerpMode.CameraToWorldSlerp:
            case SlerpMode.CameraToWorldLerp:
                return a.camera.cameraToWorldMatrix;
        }
        if (a.camera != null)
        {
            return (a.camera.worldToCameraMatrix * a.localToWorldMatrix);
        }
        return a.localToWorldMatrix;
    }

    private Matrix4x4 Interp(float t, Matrix4x4 a, Matrix4x4 b)
    {
        Matrix4x4 matrixx;
        switch (this.mode)
        {
            case SlerpMode.TransformLerp:
            case SlerpMode.WorldToCameraLerp:
            case SlerpMode.CameraToWorldLerp:
                matrixx = TransitionFunctions.Linear(t, a, b);
                break;

            case SlerpMode.WorldToCameraSlerp2:
                matrixx = TransitionFunctions.SlerpWorldToCamera(t, a, b);
                break;

            default:
                matrixx = TransitionFunctions.Slerp(t, a, b);
                break;
        }
        if (this.inverse0)
        {
            if (this.transpose)
            {
                if (!this.inverse1)
                {
                    return matrixx.inverse.transpose;
                }
                return matrixx.inverse.transpose.inverse;
            }
            if (this.inverse1)
            {
                return matrixx.inverse.inverse;
            }
            return matrixx.inverse;
        }
        if (this.transpose)
        {
            if (this.inverse1)
            {
                return matrixx.transpose.inverse;
            }
            return matrixx.transpose;
        }
        if (this.inverse1)
        {
            return matrixx.inverse;
        }
        return matrixx;
    }

    private void OnDrawGizmos()
    {
        if (this.ready)
        {
            Matrix4x4 matrix = this.GetMatrix(this.a);
            Matrix4x4 b = this.GetMatrix(this.b);
            float t = !this.cap ? this.t : Mathf.Clamp01(this.t);
            Matrix4x4 m = this.Interp(0f, matrix, b);
            DrawAxes(m, 0.5f);
            for (int i = 1; i <= 0x20; i++)
            {
                Matrix4x4 matrixx4 = this.Interp(((float) i) / 32f, matrix, b);
                Gizmos.color = (Color) (Color.white * 0.5f);
                Gizmos.DrawLine(m.MultiplyPoint(Vector3.zero), matrixx4.MultiplyPoint(Vector3.zero));
                Gizmos.color = (Color) (Color.red * 0.5f);
                Gizmos.DrawLine(m.MultiplyPoint(Vector3.right), matrixx4.MultiplyPoint(Vector3.right));
                Gizmos.color = (Color) (Color.green * 0.5f);
                Gizmos.DrawLine(m.MultiplyPoint(Vector3.up), matrixx4.MultiplyPoint(Vector3.up));
                Gizmos.color = (Color) (Color.blue * 0.5f);
                Gizmos.DrawLine(m.MultiplyPoint(Vector3.forward), matrixx4.MultiplyPoint(Vector3.forward));
                m = matrixx4;
            }
            DrawAxes(m, 0.5f);
            m = this.Interp(t, matrix, b);
            DrawAxes(m, 1f);
            Vector3 from = m.MultiplyVector(Vector3.right);
            this.angleXY = Vector3.Angle(from, m.MultiplyVector(Vector3.up));
            Vector3 introduced7 = m.MultiplyVector(Vector3.up);
            this.angleYZ = Vector3.Angle(introduced7, m.MultiplyVector(Vector3.forward));
            Vector3 introduced8 = m.MultiplyVector(Vector3.forward);
            this.angleZX = Vector3.Angle(introduced8, m.MultiplyVector(Vector3.right));
        }
    }

    private bool ready
    {
        get
        {
            switch (this.mode)
            {
                case SlerpMode.WorldToCameraSlerp:
                case SlerpMode.WorldToCameraLerp:
                case SlerpMode.CameraToWorldSlerp:
                case SlerpMode.CameraToWorldLerp:
                    return ((((this.a != null) && (this.b != null)) && (this.a.camera != null)) && ((bool) this.b.camera));
            }
            return ((this.a != null) && ((bool) this.b));
        }
    }

    private enum SlerpMode
    {
        TransformSlerp,
        TransformLerp,
        WorldToCameraSlerp,
        WorldToCameraLerp,
        CameraToWorldSlerp,
        CameraToWorldLerp,
        WorldToCameraSlerp2
    }
}

