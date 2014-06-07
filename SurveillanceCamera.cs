using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SurveillanceCamera : MonoBehaviour
{
    private RenderTexture boundTarget;
    public Camera camera;
    public const float kAspect = 1f;
    public const int kDepth = 0x18;
    public const RenderTextureFormat kFormat = RenderTextureFormat.RGB565;
    public const int kHeight = 0x200;
    private const int kRetireFrameCount = 3;
    public const int kWidth = 0x200;
    private int lastFrameRendered;

    private void Awake()
    {
        this.camera = base.camera;
        this.camera.enabled = false;
        base.enabled = false;
    }

    private void LateUpdate()
    {
        if (Mathf.Abs((int) (this.lastFrameRendered - Time.frameCount)) > 3)
        {
            this.camera.targetTexture = null;
            RenderTexture.ReleaseTemporary(this.boundTarget);
            this.boundTarget = null;
            base.enabled = false;
        }
    }

    private void OnDestroy()
    {
        if (this.boundTarget != null)
        {
            if (this.camera != null)
            {
                this.camera.targetTexture = null;
            }
            RenderTexture.ReleaseTemporary(this.boundTarget);
            this.boundTarget = null;
        }
    }

    public RenderTexture Render()
    {
        int frameCount = Time.frameCount;
        if (this.lastFrameRendered != frameCount)
        {
            bool flag = this.lastFrameRendered != (frameCount - 1);
            this.lastFrameRendered = Time.frameCount;
            if (flag && (this.boundTarget == null))
            {
                this.boundTarget = RenderTexture.GetTemporary(0x200, 0x200, 0x18, RenderTextureFormat.RGB565);
                base.enabled = true;
                this.camera.targetTexture = this.boundTarget;
                this.camera.ResetAspect();
            }
            this.camera.Render();
        }
        return this.boundTarget;
    }
}

