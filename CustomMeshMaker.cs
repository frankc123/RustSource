using System;
using UnityEngine;

public class CustomMeshMaker : ScriptableObject
{
    public bool autoBound;
    public bool autoNormals;
    public Bounds bounds;
    public Color[] colors;
    public Vector3[] normals;
    public bool optimize;
    public string output;
    public Vector4[] tangents;
    public int[] triangles;
    public Vector2[] uv1;
    public Vector2[] uv2;
    public Vector3[] vertices;
}

