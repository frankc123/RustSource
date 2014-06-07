using System;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PathSlopScene : MonoBehaviour
{
    public float areaGrid = 8f;
    public float initialWidth = 1f;
    public LayerMask layerMask = 1;
    public float pushup = 0.05f;
    public Vector4[] sloppymess;

    public static PathSlopScene current
    {
        get
        {
            return null;
        }
    }

    public MeshFilter filter
    {
        get
        {
            return base.GetComponent<MeshFilter>();
        }
    }

    public MeshRenderer renderer
    {
        get
        {
            return (MeshRenderer) base.renderer;
        }
    }
}

