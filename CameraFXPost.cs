using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFXPost : MonoBehaviour
{
    public bool allowPostRenderCalls;
    public static CameraFX cameraFX;
    private static bool didPostRender;
    private static int lastRenderFrame = -100;
    public static MountedCamera mountedCamera;

    private void OnPostRender()
    {
        if (this.allowPostRenderCalls && ((Time.renderedFrameCount == lastRenderFrame) && !didPostRender))
        {
            if (cameraFX != null)
            {
                cameraFX.PostPostRender();
            }
            didPostRender = true;
        }
    }

    private void OnPreCull()
    {
        if (lastRenderFrame != Time.renderedFrameCount)
        {
            lastRenderFrame = Time.renderedFrameCount;
            didPostRender = false;
        }
        else
        {
            return;
        }
        if (cameraFX != null)
        {
            cameraFX.PostPreCull();
            if (mountedCamera != null)
            {
                mountedCamera.PreCullEnd(true);
            }
        }
        else if (mountedCamera != null)
        {
            mountedCamera.PreCullEnd(false);
        }
    }
}

