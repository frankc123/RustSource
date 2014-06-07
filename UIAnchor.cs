using System;
using System.Runtime.InteropServices;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Anchor"), ExecuteInEditMode]
public class UIAnchor : MonoBehaviour
{
    [NonSerialized]
    private Transform __mTrans;
    public float depthOffset;
    public bool halfPixelOffset = true;
    [NonSerialized]
    private Vector3 mLastPosition;
    [NonSerialized]
    private bool mOnce;
    [NonSerialized]
    private bool mTransGot;
    public bool otherThingsMightMoveThis;
    public Vector2 relativeOffset = Vector2.zero;
    public Side side = Side.Center;
    public Camera uiCamera;

    private void OnEnable()
    {
        if (this.uiCamera == null)
        {
            this.uiCamera = NGUITools.FindCameraForLayer(base.gameObject.layer);
        }
    }

    public static void ScreenOrigin(Side side, float xMin, float xMax, float yMin, float yMax, out float x, out float y)
    {
        switch (side)
        {
            case Side.BottomLeft:
                x = xMin;
                y = yMin;
                break;

            case Side.Left:
                x = xMin;
                y = (yMin + yMax) / 2f;
                break;

            case Side.TopLeft:
                x = xMin;
                y = yMax;
                break;

            case Side.Top:
                x = (xMin + xMax) / 2f;
                y = yMax;
                break;

            case Side.TopRight:
                x = xMax;
                y = yMax;
                break;

            case Side.Right:
                x = xMax;
                y = (yMin + yMax) / 2f;
                break;

            case Side.BottomRight:
                x = xMax;
                y = yMin;
                break;

            case Side.Bottom:
                x = (xMin + xMax) / 2f;
                y = yMin;
                break;

            case Side.Center:
                x = (xMin + xMax) / 2f;
                y = (yMin + yMax) / 2f;
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static void ScreenOrigin(Side side, float xMin, float xMax, float yMin, float yMax, Flags flags, out float x, out float y)
    {
        switch (((Flags) ((byte) (flags & (!Info.isWindows ? Flags.CameraIsOrthographic : (Flags.CameraIsOrthographic | Flags.HalfPixelOffset))))))
        {
            case Flags.CameraIsOrthographic:
                float num;
                float num2;
                ScreenOrigin(side, xMin, xMax, yMin, yMax, out num, out num2);
                x = Mathf.Round(num);
                y = Mathf.Round(num2);
                return;

            case (Flags.CameraIsOrthographic | Flags.HalfPixelOffset):
                float num3;
                float num4;
                ScreenOrigin(side, xMin, xMax, yMin, yMax, out num3, out num4);
                x = Mathf.Round(num3) - 0.5f;
                y = Mathf.Round(num4) + 0.5f;
                return;
        }
        ScreenOrigin(side, xMin, xMax, yMin, yMax, out x, out y);
    }

    public static void ScreenOrigin(Side side, float xMin, float xMax, float yMin, float yMax, float relativeOffsetX, float relativeOffsetY, out float x, out float y)
    {
        float num;
        float num2;
        ScreenOrigin(side, xMin, xMax, yMin, yMax, out num, out num2);
        x = num + (relativeOffsetX * (xMax - xMin));
        y = num2 + (relativeOffsetY * (yMax - yMin));
    }

    public static void ScreenOrigin(Side side, float xMin, float xMax, float yMin, float yMax, float relativeOffsetX, float relativeOffsetY, Flags flags, out float x, out float y)
    {
        switch (((Flags) ((byte) (flags & (Flags.CameraIsOrthographic | Flags.HalfPixelOffset)))))
        {
            case Flags.CameraIsOrthographic:
                float num;
                float num2;
                ScreenOrigin(side, xMin, xMax, yMin, yMax, relativeOffsetX, relativeOffsetY, out num, out num2);
                x = Mathf.Round(num);
                y = Mathf.Round(num2);
                return;

            case (Flags.CameraIsOrthographic | Flags.HalfPixelOffset):
                float num3;
                float num4;
                ScreenOrigin(side, xMin, xMax, yMin, yMax, relativeOffsetX, relativeOffsetY, out num3, out num4);
                x = Mathf.Round(num3) - 0.5f;
                y = Mathf.Round(num4) + 0.5f;
                return;
        }
        ScreenOrigin(side, xMin, xMax, yMin, yMax, relativeOffsetX, relativeOffsetY, out x, out y);
    }

    protected void SetPosition(ref Vector3 newPosition)
    {
        Transform mTrans = this.mTrans;
        if (this.otherThingsMightMoveThis || !this.mOnce)
        {
            this.mLastPosition = mTrans.position;
            this.mOnce = true;
        }
        if (((newPosition.x != this.mLastPosition.x) || (newPosition.y != this.mLastPosition.y)) || (newPosition.z != this.mLastPosition.z))
        {
            mTrans.position = newPosition;
        }
    }

    protected void Update()
    {
        if (this.uiCamera != null)
        {
            Vector3 newPosition = WorldOrigin(this.uiCamera, this.side, this.depthOffset, this.relativeOffset.x, this.relativeOffset.y, this.halfPixelOffset);
            this.SetPosition(ref newPosition);
        }
    }

    public static Vector3 WorldOrigin(Camera camera, Side side, bool halfPixel)
    {
        Vector3 vector;
        vector.z = 0f;
        Rect pixelRect = camera.pixelRect;
        float xMin = pixelRect.xMin;
        float xMax = pixelRect.xMax;
        float yMin = pixelRect.yMin;
        float yMax = pixelRect.yMax;
        ScreenOrigin(side, xMin, xMax, yMin, yMax, !camera.isOrthoGraphic ? (!halfPixel ? ((Flags) 0) : Flags.HalfPixelOffset) : (!halfPixel ? Flags.CameraIsOrthographic : (Flags.CameraIsOrthographic | Flags.HalfPixelOffset)), out vector.x, out vector.y);
        return camera.ScreenToWorldPoint(vector);
    }

    public static Vector3 WorldOrigin(Camera camera, Side side, float depthOffset, bool halfPixel)
    {
        Vector3 vector;
        vector.z = depthOffset;
        Rect pixelRect = camera.pixelRect;
        float xMin = pixelRect.xMin;
        float xMax = pixelRect.xMax;
        float yMin = pixelRect.yMin;
        float yMax = pixelRect.yMax;
        ScreenOrigin(side, xMin, xMax, yMin, yMax, !camera.isOrthoGraphic ? (!halfPixel ? ((Flags) 0) : Flags.HalfPixelOffset) : (!halfPixel ? Flags.CameraIsOrthographic : (Flags.CameraIsOrthographic | Flags.HalfPixelOffset)), out vector.x, out vector.y);
        return camera.ScreenToWorldPoint(vector);
    }

    public static Vector3 WorldOrigin(Camera camera, Side side, RectOffset offset, bool halfPixel)
    {
        Vector3 vector;
        vector.z = 0f;
        Rect rect = offset.Add(camera.pixelRect);
        float xMin = rect.xMin;
        float xMax = rect.xMax;
        float yMin = rect.yMin;
        float yMax = rect.yMax;
        ScreenOrigin(side, xMin, xMax, yMin, yMax, !camera.isOrthoGraphic ? (!halfPixel ? ((Flags) 0) : Flags.HalfPixelOffset) : (!halfPixel ? Flags.CameraIsOrthographic : (Flags.CameraIsOrthographic | Flags.HalfPixelOffset)), out vector.x, out vector.y);
        return camera.ScreenToWorldPoint(vector);
    }

    public static Vector3 WorldOrigin(Camera camera, Side side, float relativeOffsetX, float relativeOffsetY, bool halfPixel)
    {
        Vector3 vector;
        vector.z = 0f;
        Rect pixelRect = camera.pixelRect;
        float xMin = pixelRect.xMin;
        float xMax = pixelRect.xMax;
        float yMin = pixelRect.yMin;
        float yMax = pixelRect.yMax;
        ScreenOrigin(side, xMin, xMax, yMin, yMax, relativeOffsetX, relativeOffsetY, !camera.isOrthoGraphic ? (!halfPixel ? ((Flags) 0) : Flags.HalfPixelOffset) : (!halfPixel ? Flags.CameraIsOrthographic : (Flags.CameraIsOrthographic | Flags.HalfPixelOffset)), out vector.x, out vector.y);
        return camera.ScreenToWorldPoint(vector);
    }

    public static Vector3 WorldOrigin(Camera camera, Side side, RectOffset offset, float depthOffset, bool halfPixel)
    {
        Vector3 vector;
        vector.z = depthOffset;
        Rect rect = offset.Add(camera.pixelRect);
        float xMin = rect.xMin;
        float xMax = rect.xMax;
        float yMin = rect.yMin;
        float yMax = rect.yMax;
        ScreenOrigin(side, xMin, xMax, yMin, yMax, !camera.isOrthoGraphic ? (!halfPixel ? ((Flags) 0) : Flags.HalfPixelOffset) : (!halfPixel ? Flags.CameraIsOrthographic : (Flags.CameraIsOrthographic | Flags.HalfPixelOffset)), out vector.x, out vector.y);
        return camera.ScreenToWorldPoint(vector);
    }

    public static Vector3 WorldOrigin(Camera camera, Side side, float depthOffset, float relativeOffsetX, float relativeOffsetY, bool halfPixel)
    {
        Vector3 vector;
        vector.z = depthOffset;
        Rect pixelRect = camera.pixelRect;
        float xMin = pixelRect.xMin;
        float xMax = pixelRect.xMax;
        float yMin = pixelRect.yMin;
        float yMax = pixelRect.yMax;
        ScreenOrigin(side, xMin, xMax, yMin, yMax, relativeOffsetX, relativeOffsetY, !camera.isOrthoGraphic ? (!halfPixel ? ((Flags) 0) : Flags.HalfPixelOffset) : (!halfPixel ? Flags.CameraIsOrthographic : (Flags.CameraIsOrthographic | Flags.HalfPixelOffset)), out vector.x, out vector.y);
        return camera.ScreenToWorldPoint(vector);
    }

    public static Vector3 WorldOrigin(Camera camera, Side side, RectOffset offset, float relativeOffsetX, float relativeOffsetY, bool halfPixel)
    {
        Vector3 vector;
        vector.z = 0f;
        Rect rect = offset.Add(camera.pixelRect);
        float xMin = rect.xMin;
        float xMax = rect.xMax;
        float yMin = rect.yMin;
        float yMax = rect.yMax;
        ScreenOrigin(side, xMin, xMax, yMin, yMax, relativeOffsetX, relativeOffsetY, !camera.isOrthoGraphic ? (!halfPixel ? ((Flags) 0) : Flags.HalfPixelOffset) : (!halfPixel ? Flags.CameraIsOrthographic : (Flags.CameraIsOrthographic | Flags.HalfPixelOffset)), out vector.x, out vector.y);
        return camera.ScreenToWorldPoint(vector);
    }

    public static Vector3 WorldOrigin(Camera camera, Side side, RectOffset offset, float depthOffset, float relativeOffsetX, float relativeOffsetY, bool halfPixel)
    {
        Vector3 vector;
        vector.z = depthOffset;
        Rect rect = offset.Add(camera.pixelRect);
        float xMin = rect.xMin;
        float xMax = rect.xMax;
        float yMin = rect.yMin;
        float yMax = rect.yMax;
        ScreenOrigin(side, xMin, xMax, yMin, yMax, relativeOffsetX, relativeOffsetY, !camera.isOrthoGraphic ? (!halfPixel ? ((Flags) 0) : Flags.HalfPixelOffset) : (!halfPixel ? Flags.CameraIsOrthographic : (Flags.CameraIsOrthographic | Flags.HalfPixelOffset)), out vector.x, out vector.y);
        return camera.ScreenToWorldPoint(vector);
    }

    protected Transform mTrans
    {
        get
        {
            if (!this.mTransGot)
            {
                this.__mTrans = base.transform;
                this.mTransGot = true;
            }
            return this.__mTrans;
        }
    }

    [Flags]
    public enum Flags : byte
    {
        CameraIsOrthographic = 1,
        HalfPixelOffset = 2
    }

    protected static class Info
    {
        public static readonly bool isWindows;

        static Info()
        {
            RuntimePlatform platform = Application.platform;
            isWindows = ((platform == RuntimePlatform.WindowsPlayer) || (platform == RuntimePlatform.WindowsWebPlayer)) || (platform == RuntimePlatform.WindowsEditor);
        }
    }

    public enum Side
    {
        BottomLeft,
        Left,
        TopLeft,
        Top,
        TopRight,
        Right,
        BottomRight,
        Bottom,
        Center
    }
}

