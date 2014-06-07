using System;
using UnityEngine;

[RequireComponent(typeof(Camera)), ExecuteInEditMode, AddComponentMenu("Time of Day/Camera Main Script")]
public class TOD_Camera : MonoBehaviour
{
    public bool DomePosToCamera = true;
    public float DomeScaleFactor = 0.95f;
    public bool DomeScaleToFarClip;
    public TOD_Sky sky;

    protected void OnPreCull()
    {
        if (this.sky != null)
        {
            if (this.DomeScaleToFarClip)
            {
                float x = this.DomeScaleFactor * base.camera.farClipPlane;
                Vector3 vector = new Vector3(x, x, x);
                this.sky.transform.localScale = vector;
            }
            if (this.DomePosToCamera)
            {
                Vector3 position = base.transform.position;
                this.sky.transform.position = position;
            }
        }
    }
}

