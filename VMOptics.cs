using System;
using UnityEngine;

public class VMOptics : MonoBehaviour
{
    public Socket.CameraSpace sightOverride;

    private void OnDrawGizmosSelected()
    {
        this.sightOverride.DrawGizmos("sights");
    }
}

