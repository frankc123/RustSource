using System;
using UnityEngine;

public class Camera_FollowTarget : MonoBehaviour
{
    public bool bDropCamera;
    public float flCameraYawOffset = 45f;
    public float flDistanceFromPlayer = 4f;
    public GameObject goTarget;
    private Quaternion quatCameraAngles;
    private Vector3 vecLastCameraPosition;

    private void Start()
    {
        this.quatCameraAngles = Quaternion.identity;
    }

    private void Update()
    {
        if (!this.bDropCamera)
        {
            this.UpdateCameraPosition();
        }
        else
        {
            base.transform.position = this.vecLastCameraPosition;
        }
        base.transform.rotation = Quaternion.LookRotation((this.goTarget.transform.position + Vector3.up) - base.transform.position);
    }

    private void UpdateCameraPosition()
    {
        Vector3 vector = this.goTarget.transform.TransformDirection(Vector3.forward);
        Quaternion quaternion = Quaternion.AngleAxis(this.flCameraYawOffset, Vector3.up);
        base.transform.position = (this.goTarget.transform.position + Vector3.up) + ((Vector3) ((quaternion * vector) * this.flDistanceFromPlayer));
        this.vecLastCameraPosition = base.transform.position;
    }
}

