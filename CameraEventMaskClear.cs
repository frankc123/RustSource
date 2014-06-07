using System;
using UnityEngine;

public sealed class CameraEventMaskClear : MonoBehaviour
{
    private void Awake()
    {
        base.camera.eventMask = 0;
    }
}

