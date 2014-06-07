using System;
using UnityEngine;

public class SecurityCamScreen : MonoBehaviour
{
    private bool firstInit = true;
    public Camera RenderCamera;
    public float renderInterval;

    private void Awake()
    {
        base.Invoke("UpdateCam", this.renderInterval);
    }

    private void UpdateCam()
    {
        if (this.RenderCamera != null)
        {
            PlayerClient localPlayer = PlayerClient.GetLocalPlayer();
            Controllable controllable = (localPlayer == null) ? null : localPlayer.controllable;
            if (controllable != null)
            {
                if (this.firstInit)
                {
                    this.RenderCamera.Render();
                    this.firstInit = false;
                }
                if (Vector3.Distance(controllable.transform.position, base.transform.position) < 15f)
                {
                    this.RenderCamera.Render();
                }
            }
            base.Invoke("UpdateCam", this.renderInterval);
        }
    }
}

