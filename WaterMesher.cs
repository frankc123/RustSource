using System;
using UnityEngine;

[AddComponentMenu("Water/Mesher")]
public class WaterMesher : MonoBehaviour
{
    public Vector2 inTangent;
    public bool isRoot;
    private const int kPoints = 0x10;
    public WaterMesher next;
    public Vector2 outTangent;
    public WaterMesher prev;

    public Vector2 Point(float t, Vector2 p3)
    {
        Vector2 position = this.position;
        Vector2 vector2 = position + this.inTangent;
        Vector2 vector3 = p3 + this.outTangent;
        float num = 1f - t;
        position.x = (position.x * num) + (vector2.x * t);
        position.y = (position.y * num) + (vector2.y * t);
        vector2.x = (vector2.x * num) + (vector3.x * t);
        vector2.y = (vector2.y * num) + (vector3.y * t);
        vector3.x = (vector3.x * num) + (p3.x * t);
        vector3.y = (vector3.y * num) + (p3.y * t);
        position.x = (position.x * num) + (vector2.x * t);
        position.y = (position.y * num) + (vector2.y * t);
        vector2.x = (vector2.x * num) + (vector3.x * t);
        vector2.y = (vector2.y * num) + (vector3.y * t);
        position.x = (position.x * num) + (vector2.x * t);
        position.y = (position.y * num) + (vector2.y * t);
        return position;
    }

    public Vector3 Point3(float t, Vector2 p3)
    {
        Vector3 vector;
        Vector2 vector2 = this.Point(t, p3);
        vector.x = vector2.x;
        vector.y = base.transform.position.y;
        vector.z = vector2.y;
        return vector;
    }

    public Vector2 SmoothPoint(float t, Vector2 p3)
    {
        Vector2 position = this.position;
        Vector2 vector2 = position + this.smoothInTangent;
        Vector2 vector3 = p3 + this.smoothOutTangent;
        float num = 1f - t;
        position.x = (position.x * num) + (vector2.x * t);
        position.y = (position.y * num) + (vector2.y * t);
        vector2.x = (vector2.x * num) + (vector3.x * t);
        vector2.y = (vector2.y * num) + (vector3.y * t);
        vector3.x = (vector3.x * num) + (p3.x * t);
        vector3.y = (vector3.y * num) + (p3.y * t);
        position.x = (position.x * num) + (vector2.x * t);
        position.y = (position.y * num) + (vector2.y * t);
        vector2.x = (vector2.x * num) + (vector3.x * t);
        vector2.y = (vector2.y * num) + (vector3.y * t);
        position.x = (position.x * num) + (vector2.x * t);
        position.y = (position.y * num) + (vector2.y * t);
        return position;
    }

    public Vector2 position
    {
        get
        {
            Vector3 position = base.transform.position;
            position.y = position.z;
            position.z = 0f;
            return new Vector2(position.x, position.y);
        }
        set
        {
            Vector3 position = base.transform.position;
            position.x = value.x;
            position.z = value.y;
            base.transform.position = position;
        }
    }

    public Vector3 position3
    {
        get
        {
            return base.transform.position;
        }
    }

    public Vector2 smoothInTangent
    {
        get
        {
            return ((this.prev == null) ? this.inTangent : ((Vector2) ((this.inTangent - this.prev.outTangent) / 2f)));
        }
    }

    public Vector2 smoothOutTangent
    {
        get
        {
            return ((this.next == null) ? this.inTangent : ((Vector2) ((this.outTangent - this.next.inTangent) / 2f)));
        }
    }
}

