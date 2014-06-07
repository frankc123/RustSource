using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[ExecuteInEditMode]
public sealed class LaserBeam : MonoBehaviour
{
    public Vector4 beamColor = ((Vector4) Color.red);
    public LayerMask beamLayers = 1;
    public Material beamMaterial;
    public float beamMaxDistance = 100f;
    public float beamOutput = 1f;
    public float beamWidthEnd = 0.2f;
    public float beamWidthStart = 0.1f;
    public LayerMask cullLayers = 1;
    public Vector4 dotColor = ((Vector4) Color.red);
    public Material dotMaterial;
    public float dotRadiusEnd = 0.25f;
    public float dotRadiusStart = 0.15f;
    public FrameData frame;
    public bool isViewModel;

    public static List<LaserBeam> Collect()
    {
        g.currentRendering.Clear();
        g.currentRendering.AddRange(g.allActiveBeams);
        return g.currentRendering;
    }

    private void OnDisable()
    {
        g.allActiveBeams.Remove(this);
    }

    private void OnEnable()
    {
        g.allActiveBeams.Add(this);
        LaserGraphics.EnsureGraphicsExist();
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FrameData
    {
        public MaterialPropertyBlock block;
        public Bounds bounds;
        public bool hit;
        public Vector3 hitPoint;
        public Vector3 hitNormal;
        public LaserBeam.Quad<Vector3> beamVertices;
        public LaserBeam.Quad<Vector3> beamNormals;
        public LaserBeam.Quad<Vector2> beamUVs;
        public LaserBeam.Quad<Vector3> dotVertices1;
        public LaserBeam.Quad<Vector3> dotVertices2;
        public LaserBeam.Quad<Color> beamColor;
        public LaserBeam.Quad<Color> dotColor1;
        public LaserBeam.Quad<Color> dotColor2;
        public Vector3 direction;
        public Vector3 origin;
        public Vector3 point;
        public float distance;
        public float distanceFraction;
        public float pointWidth;
        public float originWidth;
        public float dotRadius;
        public bool didHit;
        public bool drawDot;
        public int beamsLayer;
        internal LaserGraphics.MeshBuffer bufBeam;
        internal LaserGraphics.MeshBuffer bufDot;
    }

    private static class g
    {
        public static HashSet<LaserBeam> allActiveBeams = new HashSet<LaserBeam>();
        public static List<LaserBeam> currentRendering = new List<LaserBeam>();
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Quad<T>
    {
        public T m0;
        public T m1;
        public T m2;
        public T m3;
    }
}

