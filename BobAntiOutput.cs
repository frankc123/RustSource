using System;
using UnityEngine;

[Serializable]
public class BobAntiOutput
{
    private Vector3 lastPos;
    private Vector3 lastRot;
    public Vector3 positional;
    public BobAntiOutputAxes positionalAxes;
    public Vector3 rotational;
    public BobAntiOutputAxes rotationalAxes;
    private bool wasAdded;

    public void Add(Transform transform, ref Vector3 lp, ref Vector3 lr)
    {
        if (!this.wasAdded)
        {
            this.lastPos = Vector3.Scale(GetVector3(ref lp, this.positionalAxes), this.positional);
            transform.localPosition = this.lastPos;
            this.lastRot = Vector3.Scale(GetVector3(ref lr, this.rotationalAxes), this.rotational);
            transform.localEulerAngles = this.lastRot;
            this.wasAdded = true;
        }
    }

    private static Vector3 GetVector3(ref Vector3 v, BobAntiOutputAxes axes)
    {
        Vector3 vector;
        switch ((((int) axes) & 3))
        {
            case 2:
                vector.x = v.y;
                break;

            case 3:
                vector.x = v.z;
                break;

            default:
                vector.x = v.x;
                break;
        }
        switch ((((int) (axes & 12)) >> 2))
        {
            case 1:
                vector.y = v.x;
                break;

            case 3:
                vector.y = v.z;
                break;

            default:
                vector.y = v.y;
                break;
        }
        switch ((((int) (axes & 0x30)) >> 4))
        {
            case 1:
                vector.z = v.x;
                return vector;

            case 2:
                vector.z = v.y;
                return vector;
        }
        vector.z = v.z;
        return vector;
    }

    public Vector3 Positional(Vector3 v)
    {
        return Vector3.Scale(GetVector3(ref v, this.positionalAxes), this.positional);
    }

    public void Reset()
    {
        this.wasAdded = false;
    }

    public Vector3 Rotational(Vector3 v)
    {
        return Vector3.Scale(GetVector3(ref v, this.rotationalAxes), this.rotational);
    }

    public void Subtract(Transform transform)
    {
        if (this.wasAdded)
        {
            transform.localPosition -= this.lastPos;
            transform.localEulerAngles -= this.lastRot;
            this.wasAdded = false;
        }
    }
}

