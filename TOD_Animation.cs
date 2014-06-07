using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TOD_Animation : MonoBehaviour
{
    private TOD_Sky sky;
    public float WindDegrees;
    public float WindSpeed = 3f;

    protected void Start()
    {
        this.sky = base.GetComponent<TOD_Sky>();
    }

    protected void Update()
    {
        Vector2 vector = new Vector2(Mathf.Cos(0.01745329f * (this.WindDegrees + 15f)), Mathf.Sin(0.01745329f * (this.WindDegrees + 15f)));
        Vector2 vector2 = new Vector2(Mathf.Cos(0.01745329f * (this.WindDegrees - 15f)), Mathf.Sin(0.01745329f * (this.WindDegrees - 15f)));
        Vector4 vector3 = (Vector4) ((this.WindSpeed / 100f) * new Vector4(vector.x, vector.y, vector2.x, vector2.y));
        this.CloudUV += (Vector4) (Time.deltaTime * vector3);
        this.CloudUV = new Vector4(this.CloudUV.x % this.sky.Clouds.Scale1.x, this.CloudUV.y % this.sky.Clouds.Scale1.y, this.CloudUV.z % this.sky.Clouds.Scale2.x, this.CloudUV.w % this.sky.Clouds.Scale2.y);
    }

    internal Vector4 CloudUV { get; set; }

    internal Vector4 OffsetUV
    {
        get
        {
            Vector3 position = base.transform.position;
            Vector3 lossyScale = base.transform.lossyScale;
            Vector3 direction = new Vector3(position.x / lossyScale.x, 0f, position.z / lossyScale.z);
            direction = -base.transform.TransformDirection(direction);
            return new Vector4(direction.x, direction.z, direction.x, direction.z);
        }
    }
}

