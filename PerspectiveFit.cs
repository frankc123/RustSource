using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PerspectiveFit : MonoBehaviour
{
    [PrefetchComponent]
    public Camera camera;
    public float targetDistance = 2.2f;
    public Vector2 targetSize = new Vector2(2.4f, 1.1f);

    private void OnPreCull()
    {
        if ((base.enabled && (this.camera != null)) && this.camera.enabled)
        {
            float num3;
            float aspect = this.camera.aspect;
            float num5 = this.targetSize.x / this.targetSize.y;
            float num = Vector2.Angle(new Vector2((this.targetSize.x / aspect) * 0.5f, this.targetDistance), new Vector2(0f, this.targetDistance)) * 2f;
            float num2 = Vector2.Angle(new Vector2(this.targetSize.y * 0.5f, this.targetDistance), new Vector2(0f, this.targetDistance)) * 2f;
            if (num5 < aspect)
            {
                num3 = num2;
            }
            else
            {
                num3 = num;
            }
            this.camera.fieldOfView = num3;
        }
    }

    private void Reset()
    {
        if (this.camera == null)
        {
            this.camera = base.camera;
        }
    }
}

