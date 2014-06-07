using System;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    public bool orbitCenter;
    public Vector3 orbitEulerSpeed;
    public Vector3 orbitPosition;

    private void OnDrawGizmosSelected()
    {
        Gizmos.matrix = base.transform.localToWorldMatrix;
        Gizmos.DrawLine(this.orbitPosition, Vector3.zero);
        Gizmos.DrawSphere(this.orbitPosition, 0.01f);
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;
        if (deltaTime != 0f)
        {
            Vector3 vector;
            vector.x = this.orbitEulerSpeed.x * deltaTime;
            vector.y = this.orbitEulerSpeed.y * deltaTime;
            vector.z = this.orbitEulerSpeed.z * deltaTime;
            if (((vector.x != 0f) || (vector.y != 0f)) || (vector.z != 0f))
            {
                Quaternion quaternion = Quaternion.Euler(vector);
                Quaternion quaternion2 = base.transform.localRotation * quaternion;
                if (this.orbitCenter)
                {
                    base.transform.localPosition = (Vector3) (quaternion2 * this.orbitPosition);
                }
                else
                {
                    base.transform.localPosition = ((Vector3) (quaternion2 * -this.orbitPosition)) + this.orbitPosition;
                }
                base.transform.localRotation = quaternion2;
            }
        }
    }
}

