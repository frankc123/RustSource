using System;
using UnityEngine;

public sealed class LaserFilter : MonoBehaviour
{
    [NonSerialized]
    private Camera _camera;
    [NonSerialized]
    private bool _gotCam;

    private void OnPreCull()
    {
        if (base.enabled)
        {
            LaserGraphics.RenderLasersOnCamera(this.camera);
        }
    }

    public Camera camera
    {
        get
        {
            if (!this._gotCam)
            {
                this._gotCam = true;
                this._camera = base.camera;
            }
            return this._camera;
        }
    }
}

