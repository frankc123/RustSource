using System;
using UnityEngine;

public class NearFarAdjustment : MonoBehaviour
{
    private void Update()
    {
        if (Physics.Raycast(new Ray(base.transform.position, base.transform.forward), (float) 1.2f))
        {
            base.camera.nearClipPlane = 0.21f;
        }
        else
        {
            base.camera.nearClipPlane = 0.8f;
        }
    }
}

