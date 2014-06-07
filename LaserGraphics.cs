using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, AddComponentMenu("")]
public sealed class LaserGraphics : MonoBehaviour
{
    private static int allBeamsMask;
    [NonSerialized]
    private List<LaserBeam> beams;
    private static Matrix4x4 cam2World;
    private static Matrix4x4 camProj;
    private const float kBeamMaxAlpha = 1f;
    private const float kDotMaxAlpha = 12f;
    private const float kNormalPushBack = -0.01f;
    [NonSerialized]
    private bool madeLists;
    private static LaserGraphics singleton;
    private const string singletonName = "__LASER_GRAPHICS__";
    private static readonly Vector2[] uv = new Vector2[] { new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(1f, 0f), new Vector2(1f, 1f) };
    [NonSerialized]
    private List<LaserBeam> willRender;
    private static Matrix4x4 world2Cam;

    public static void EnsureGraphicsExist()
    {
        if (singleton == null)
        {
            GameObject obj2 = GameObject.Find("__LASER_GRAPHICS__");
            if (obj2 == null)
            {
                obj2 = new GameObject {
                    hideFlags = HideFlags.NotEditable | HideFlags.DontSave,
                    name = "__LASER_GRAPHICS__"
                };
                singleton = obj2.AddComponent<LaserGraphics>();
                singleton.hideFlags = HideFlags.NotEditable | HideFlags.DontSave;
            }
            else
            {
                singleton = obj2.GetComponent<LaserGraphics>();
                if (singleton == null)
                {
                    singleton = obj2.AddComponent<LaserGraphics>();
                    singleton.hideFlags = HideFlags.NotEditable | HideFlags.DontSave;
                }
            }
        }
    }

    private static Color RangeBeamColor(Color input)
    {
        float b;
        if (input.r > input.g)
        {
            if (input.b > input.r)
            {
                b = input.b;
            }
            else
            {
                b = input.r;
            }
        }
        else if (input.b > input.g)
        {
            b = input.b;
        }
        else
        {
            b = input.g;
        }
        if (b != 0f)
        {
            input.r /= b;
            input.g /= b;
            input.b /= b;
            input.a = b / 1f;
            return input;
        }
        input.a = 1f;
        return input;
    }

    private static Color RangeDotColor(Color input)
    {
        float b;
        if (input.r > input.g)
        {
            if (input.b > input.r)
            {
                b = input.b;
            }
            else
            {
                b = input.r;
            }
        }
        else if (input.b > input.g)
        {
            b = input.b;
        }
        else
        {
            b = input.g;
        }
        if (b != 0f)
        {
            input.r /= b;
            input.g /= b;
            input.b /= b;
            input.a = b / 12f;
            return input;
        }
        input.a = 0.08333334f;
        return input;
    }

    private static void RenderBeam(Plane[] frustum, Camera camera, LaserBeam beam, ref LaserBeam.FrameData frame)
    {
        Vector3 upwards = world2Cam.MultiplyPoint(frame.origin);
        Vector3 vector2 = world2Cam.MultiplyPoint(frame.point);
        Vector3 forward = vector2 - upwards;
        forward.Normalize();
        float num = 1f - ((1f - Mathf.Abs(forward.z)) * beam.beamOutput);
        Quaternion quaternion = Quaternion.LookRotation(forward, vector2);
        Vector3 vector4 = (Vector3) (Quaternion.LookRotation(forward, upwards) * new Vector3(frame.originWidth, 0f, 0f));
        Vector3 vector5 = (Vector3) (quaternion * new Vector3(frame.pointWidth, 0f, 0f));
        frame.beamVertices.m0 = cam2World.MultiplyPoint(((Vector3) (vector4 * 0.5f)) + upwards);
        frame.beamVertices.m2 = cam2World.MultiplyPoint(((Vector3) (vector5 * 0.5f)) + vector2);
        frame.beamVertices.m1 = cam2World.MultiplyPoint(((Vector3) (vector4 * -0.5f)) + upwards);
        frame.beamVertices.m3 = cam2World.MultiplyPoint(((Vector3) (vector5 * -0.5f)) + vector2);
        frame.beamNormals.m0.x = frame.originWidth;
        frame.beamNormals.m2.x = frame.pointWidth;
        frame.beamNormals.m1.x = -frame.originWidth;
        frame.beamNormals.m3.x = -frame.pointWidth;
        frame.beamNormals.m0.y = -frame.distance;
        frame.beamNormals.m1.y = -frame.distance;
        frame.beamNormals.m2.y = -frame.distance;
        frame.beamNormals.m3.y = -frame.distance;
        frame.beamNormals.m0.z = frame.beamNormals.m1.z = 0f;
        frame.beamNormals.m2.z = frame.beamNormals.m3.z = frame.distanceFraction;
        frame.beamColor.m0 = frame.beamColor.m1 = frame.beamColor.m2 = frame.beamColor.m3 = RangeBeamColor((Color) (beam.beamColor * num));
        frame.beamUVs.m0 = uv[0];
        frame.beamUVs.m0.x *= frame.distanceFraction;
        frame.beamUVs.m1 = uv[1];
        frame.beamUVs.m1.x *= frame.distanceFraction;
        frame.beamUVs.m2 = uv[2];
        frame.beamUVs.m2.x *= frame.distanceFraction;
        frame.beamUVs.m3 = uv[3];
        frame.beamUVs.m3.x *= frame.distanceFraction;
        frame.bufBeam = MeshBuffer.ForBeamMaterial(beam.beamMaterial);
        if (Computation.beams.Add(frame.bufBeam))
        {
            frame.bufBeam.measureSize = 1;
        }
        else
        {
            frame.bufBeam.measureSize++;
        }
        frame.bufBeam.beams.Add(beam);
        if (frame.didHit)
        {
            Vector3 vector6 = world2Cam.MultiplyVector(-frame.hitNormal);
            if (vector6.z < 0f)
            {
                Vector3 start = cam2World.MultiplyPoint(Vector3.zero);
                if (!Physics.Linecast(start, Vector3.Lerp(start, frame.point, 0.95f), (int) beam.cullLayers))
                {
                    Vector3 vector8 = world2Cam.MultiplyPoint(frame.point);
                    Quaternion quaternion3 = Quaternion.LookRotation(vector8, Vector3.up);
                    frame.dotVertices1.m0 = cam2World.MultiplyPoint(vector8 + (quaternion3 * new Vector3(frame.dotRadius, -frame.dotRadius, 0f)));
                    frame.dotVertices1.m1 = cam2World.MultiplyPoint(vector8 + (quaternion3 * new Vector3(frame.dotRadius, frame.dotRadius, 0f)));
                    frame.dotVertices1.m2 = cam2World.MultiplyPoint(vector8 + (quaternion3 * new Vector3(-frame.dotRadius, -frame.dotRadius, 0f)));
                    frame.dotVertices1.m3 = cam2World.MultiplyPoint(vector8 + (quaternion3 * new Vector3(-frame.dotRadius, frame.dotRadius, 0f)));
                    quaternion3 = Quaternion.LookRotation(vector6, Vector3.up);
                    frame.dotVertices2.m0 = cam2World.MultiplyPoint(vector8 + (quaternion3 * new Vector3(frame.dotRadius, -frame.dotRadius, -0.01f)));
                    frame.dotVertices2.m1 = cam2World.MultiplyPoint(vector8 + (quaternion3 * new Vector3(frame.dotRadius, frame.dotRadius, -0.01f)));
                    frame.dotVertices2.m2 = cam2World.MultiplyPoint(vector8 + (quaternion3 * new Vector3(-frame.dotRadius, -frame.dotRadius, -0.01f)));
                    frame.dotVertices2.m3 = cam2World.MultiplyPoint(vector8 + (quaternion3 * new Vector3(-frame.dotRadius, frame.dotRadius, -0.01f)));
                    frame.dotColor1.m0 = frame.dotColor1.m1 = frame.dotColor1.m2 = frame.dotColor1.m3 = frame.dotColor2.m0 = frame.dotColor2.m1 = frame.dotColor2.m2 = frame.dotColor2.m3 = RangeDotColor(beam.dotColor);
                    frame.bufDot = MeshBuffer.ForDotMaterial(beam.dotMaterial);
                    if (Computation.dots.Add(frame.bufDot))
                    {
                        frame.bufDot.measureSize = 2;
                    }
                    else
                    {
                        frame.bufDot.measureSize += 2;
                    }
                    frame.bufDot.beams.Add(beam);
                    frame.drawDot = true;
                }
                else
                {
                    frame.bufDot = null;
                    frame.drawDot = false;
                }
            }
            else
            {
                frame.bufDot = null;
                frame.drawDot = false;
            }
        }
        else
        {
            frame.bufDot = null;
            frame.drawDot = false;
        }
    }

    private void RenderLasers(Camera camera)
    {
        if (!this.madeLists)
        {
            this.beams = new List<LaserBeam>();
            this.willRender = new List<LaserBeam>();
            this.madeLists = true;
        }
        int cullingMask = camera.cullingMask;
        if (this.beams == null)
        {
            this.beams = new List<LaserBeam>(LaserBeam.Collect());
        }
        else
        {
            this.beams.Clear();
            this.beams.AddRange(LaserBeam.Collect());
        }
        allBeamsMask = 0;
        foreach (LaserBeam beam in this.beams)
        {
            UpdateBeam(ref beam.frame, beam);
        }
        if (((cullingMask & allBeamsMask) != 0) && (this.beams.Count > 0))
        {
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
            foreach (LaserBeam beam2 in this.beams)
            {
                if (beam2.isViewModel || (((cullingMask & beam2.frame.beamsLayer) == beam2.frame.beamsLayer) && GeometryUtility.TestPlanesAABB(planes, beam2.frame.bounds)))
                {
                    this.willRender.Add(beam2);
                }
            }
            if (this.willRender.Count > 0)
            {
                world2Cam = camera.worldToCameraMatrix;
                cam2World = camera.cameraToWorldMatrix;
                camProj = camera.projectionMatrix;
                try
                {
                    foreach (LaserBeam beam3 in this.willRender)
                    {
                        RenderBeam(planes, camera, beam3, ref beam3.frame);
                    }
                    foreach (MeshBuffer buffer in Computation.beams)
                    {
                        Vector3 vector;
                        Vector3 vector2;
                        bool rebindVertexLayout = buffer.Resize();
                        int num2 = 0;
                        VertexBuffer buffer2 = buffer.buffer;
                        vector.x = vector.y = vector.z = float.PositiveInfinity;
                        vector2.x = vector2.y = vector2.z = float.NegativeInfinity;
                        foreach (LaserBeam beam4 in buffer.beams)
                        {
                            int index = num2++;
                            int num4 = num2++;
                            int num5 = num2++;
                            int num6 = num2++;
                            buffer2.v[index] = beam4.frame.beamVertices.m0;
                            buffer2.v[num4] = beam4.frame.beamVertices.m1;
                            buffer2.v[num6] = beam4.frame.beamVertices.m2;
                            buffer2.v[num5] = beam4.frame.beamVertices.m3;
                            buffer2.n[index] = beam4.frame.beamNormals.m0;
                            buffer2.n[num4] = beam4.frame.beamNormals.m1;
                            buffer2.n[num6] = beam4.frame.beamNormals.m2;
                            buffer2.n[num5] = beam4.frame.beamNormals.m3;
                            buffer2.c[index] = beam4.frame.beamColor.m0;
                            buffer2.c[num4] = beam4.frame.beamColor.m1;
                            buffer2.c[num6] = beam4.frame.beamColor.m2;
                            buffer2.c[num5] = beam4.frame.beamColor.m3;
                            buffer2.t[index] = beam4.frame.beamUVs.m0;
                            buffer2.t[num4] = beam4.frame.beamUVs.m1;
                            buffer2.t[num6] = beam4.frame.beamUVs.m2;
                            buffer2.t[num5] = beam4.frame.beamUVs.m3;
                            for (int i = index; i <= num5; i++)
                            {
                                if (buffer2.v[i].x < vector.x)
                                {
                                    vector.x = buffer2.v[i].x;
                                }
                                if (buffer2.v[i].x > vector2.x)
                                {
                                    vector2.x = buffer2.v[i].x;
                                }
                                if (buffer2.v[i].y < vector.y)
                                {
                                    vector.y = buffer2.v[i].y;
                                }
                                if (buffer2.v[i].y > vector2.y)
                                {
                                    vector2.y = buffer2.v[i].y;
                                }
                                if (buffer2.v[i].z < vector.z)
                                {
                                    vector.z = buffer2.v[i].z;
                                }
                                if (buffer2.v[i].z > vector2.z)
                                {
                                    vector2.z = buffer2.v[i].z;
                                }
                            }
                            beam4.frame.bufBeam = null;
                        }
                        buffer.beams.Clear();
                        buffer.BindMesh(rebindVertexLayout, vector, vector2);
                        Graphics.DrawMesh(buffer.mesh, Matrix4x4.identity, buffer.material, 1, camera, 0, null, false, false);
                    }
                    foreach (MeshBuffer buffer3 in Computation.dots)
                    {
                        Vector3 vector3;
                        Vector3 vector4;
                        bool flag2 = buffer3.Resize();
                        int num8 = 0;
                        VertexBuffer buffer4 = buffer3.buffer;
                        vector3.x = vector3.y = vector3.z = float.PositiveInfinity;
                        vector4.x = vector4.y = vector4.z = float.NegativeInfinity;
                        foreach (LaserBeam beam5 in buffer3.beams)
                        {
                            int num9 = num8++;
                            int num10 = num8++;
                            int num11 = num8++;
                            int num12 = num8++;
                            buffer4.v[num9] = beam5.frame.dotVertices1.m0;
                            buffer4.v[num10] = beam5.frame.dotVertices1.m1;
                            buffer4.v[num12] = beam5.frame.dotVertices1.m2;
                            buffer4.v[num11] = beam5.frame.dotVertices1.m3;
                            buffer4.n[num9] = beam5.frame.beamNormals.m0;
                            buffer4.n[num10] = beam5.frame.beamNormals.m1;
                            buffer4.n[num12] = beam5.frame.beamNormals.m2;
                            buffer4.n[num11] = beam5.frame.beamNormals.m3;
                            buffer4.c[num9] = beam5.frame.dotColor1.m0;
                            buffer4.c[num10] = beam5.frame.dotColor1.m1;
                            buffer4.c[num12] = beam5.frame.dotColor1.m2;
                            buffer4.c[num11] = beam5.frame.dotColor1.m3;
                            buffer4.t[num9] = uv[0];
                            buffer4.t[num10] = uv[1];
                            buffer4.t[num12] = uv[2];
                            buffer4.t[num11] = uv[3];
                            for (int j = num9; j <= num11; j++)
                            {
                                if (buffer4.v[j].x < vector3.x)
                                {
                                    vector3.x = buffer4.v[j].x;
                                }
                                if (buffer4.v[j].x > vector4.x)
                                {
                                    vector4.x = buffer4.v[j].x;
                                }
                                if (buffer4.v[j].y < vector3.y)
                                {
                                    vector3.y = buffer4.v[j].y;
                                }
                                if (buffer4.v[j].y > vector4.y)
                                {
                                    vector4.y = buffer4.v[j].y;
                                }
                                if (buffer4.v[j].z < vector3.z)
                                {
                                    vector3.z = buffer4.v[j].z;
                                }
                                if (buffer4.v[j].z > vector4.z)
                                {
                                    vector4.z = buffer4.v[j].z;
                                }
                            }
                            num9 = num8++;
                            num10 = num8++;
                            num11 = num8++;
                            num12 = num8++;
                            buffer4.v[num9] = beam5.frame.dotVertices2.m0;
                            buffer4.v[num10] = beam5.frame.dotVertices2.m1;
                            buffer4.v[num12] = beam5.frame.dotVertices2.m2;
                            buffer4.v[num11] = beam5.frame.dotVertices2.m3;
                            buffer4.n[num9] = beam5.frame.beamNormals.m0;
                            buffer4.n[num10] = beam5.frame.beamNormals.m1;
                            buffer4.n[num12] = beam5.frame.beamNormals.m2;
                            buffer4.n[num11] = beam5.frame.beamNormals.m3;
                            buffer4.c[num9] = beam5.frame.dotColor2.m0;
                            buffer4.c[num10] = beam5.frame.dotColor2.m1;
                            buffer4.c[num12] = beam5.frame.dotColor2.m2;
                            buffer4.c[num11] = beam5.frame.dotColor2.m3;
                            buffer4.t[num9] = uv[0];
                            buffer4.t[num10] = uv[1];
                            buffer4.t[num12] = uv[2];
                            buffer4.t[num11] = uv[3];
                            for (int k = num9; k <= num11; k++)
                            {
                                if (buffer4.v[k].x < vector3.x)
                                {
                                    vector3.x = buffer4.v[k].x;
                                }
                                if (buffer4.v[k].x > vector4.x)
                                {
                                    vector4.x = buffer4.v[k].x;
                                }
                                if (buffer4.v[k].y < vector3.y)
                                {
                                    vector3.y = buffer4.v[k].y;
                                }
                                if (buffer4.v[k].y > vector4.y)
                                {
                                    vector4.y = buffer4.v[k].y;
                                }
                                if (buffer4.v[k].z < vector3.z)
                                {
                                    vector3.z = buffer4.v[k].z;
                                }
                                if (buffer4.v[k].z > vector4.z)
                                {
                                    vector4.z = buffer4.v[k].z;
                                }
                            }
                            beam5.frame.bufDot = null;
                        }
                        buffer3.beams.Clear();
                        if (flag2)
                        {
                            buffer3.mesh.Clear(false);
                            buffer3.mesh.vertices = buffer4.v;
                            buffer3.mesh.normals = buffer4.n;
                            buffer3.mesh.colors = buffer4.c;
                            buffer3.mesh.uv = buffer4.t;
                            buffer3.mesh.SetIndices(buffer4.i, MeshTopology.Quads, 0);
                        }
                        else
                        {
                            buffer3.mesh.vertices = buffer4.v;
                            buffer3.mesh.normals = buffer4.n;
                            buffer3.mesh.colors = buffer4.c;
                            buffer3.mesh.uv = buffer4.t;
                        }
                        buffer3.BindMesh(flag2, vector3, vector4);
                        Graphics.DrawMesh(buffer3.mesh, Matrix4x4.identity, buffer3.material, 1, camera, 0, null, false, false);
                    }
                }
                finally
                {
                    this.willRender.Clear();
                    Computation.beams.Clear();
                    Computation.dots.Clear();
                    MeshBuffer.Reset();
                }
            }
        }
    }

    internal static void RenderLasersOnCamera(Camera camera)
    {
        if (singleton != null)
        {
            singleton.RenderLasers(camera);
        }
    }

    private static void UpdateBeam(ref LaserBeam.FrameData frame, LaserBeam beam)
    {
        Vector3 vector;
        Transform transform = beam.transform;
        frame.origin = transform.position;
        frame.direction = transform.forward;
        frame.direction.Normalize();
        int beamLayers = (int) beam.beamLayers;
        if (beamLayers == 0)
        {
            frame.hit = false;
        }
        else if (beam.isViewModel)
        {
            RaycastHit2 hit;
            if (frame.hit = Physics2.Raycast2(frame.origin, frame.direction, out hit, beam.beamMaxDistance, beamLayers))
            {
                frame.hitPoint = hit.point;
                frame.hitNormal = hit.normal;
            }
        }
        else
        {
            RaycastHit hit2;
            if (frame.hit = Physics.Raycast(frame.origin, frame.direction, out hit2, beam.beamMaxDistance, beamLayers))
            {
                frame.hitPoint = hit2.point;
                frame.hitNormal = hit2.normal;
            }
        }
        if (!frame.hit)
        {
            frame.didHit = false;
            frame.point.x = frame.origin.x + (frame.direction.x * beam.beamMaxDistance);
            frame.point.y = frame.origin.y + (frame.direction.y * beam.beamMaxDistance);
            frame.point.z = frame.origin.z + (frame.direction.z * beam.beamMaxDistance);
            frame.distance = beam.beamMaxDistance;
            frame.distanceFraction = 1f;
            frame.pointWidth = beam.beamWidthEnd;
        }
        else
        {
            frame.point = frame.hitPoint;
            frame.didHit = true;
            frame.distance = (((frame.direction.x * frame.point.x) + (frame.direction.y * frame.point.y)) + (frame.direction.z * frame.point.z)) - (((frame.direction.x * frame.origin.x) + (frame.direction.y * frame.origin.y)) + (frame.direction.z * frame.origin.z));
            frame.distanceFraction = frame.distance / beam.beamMaxDistance;
            frame.pointWidth = Mathf.Lerp(beam.beamWidthStart, beam.beamWidthEnd, frame.distanceFraction);
            frame.dotRadius = Mathf.Lerp(beam.dotRadiusStart, beam.dotRadiusEnd, frame.distanceFraction);
        }
        frame.originWidth = beam.beamWidthStart;
        vector.x = vector.y = vector.z = frame.originWidth;
        frame.bounds = new Bounds(frame.origin, vector);
        vector.x = vector.y = vector.z = frame.pointWidth;
        frame.bounds.Encapsulate(new Bounds(frame.point, vector));
        frame.beamsLayer = ((int) 1) << beam.gameObject.layer;
        allBeamsMask |= frame.beamsLayer;
    }

    private static class Computation
    {
        public static readonly HashSet<LaserGraphics.MeshBuffer> beams = new HashSet<LaserGraphics.MeshBuffer>();
        public static readonly HashSet<LaserGraphics.MeshBuffer> dots = new HashSet<LaserGraphics.MeshBuffer>();
    }

    internal sealed class MeshBuffer : IDisposable, IEquatable<LaserGraphics.MeshBuffer>
    {
        public readonly List<LaserBeam> beams = new List<LaserBeam>();
        internal LaserGraphics.VertexBuffer buffer;
        private readonly int instanceID;
        public readonly Material material;
        public int measureSize;
        public Mesh mesh;
        private int quadCount;

        private MeshBuffer(Material material)
        {
            this.instanceID = material.GetInstanceID();
            Mesh mesh = new Mesh {
                hideFlags = HideFlags.DontSave
            };
            this.mesh = mesh;
            this.mesh.MarkDynamic();
            this.material = material;
        }

        public void BindMesh(bool rebindVertexLayout, Vector3 min, Vector3 max)
        {
            if (rebindVertexLayout)
            {
                this.mesh.Clear(false);
                this.mesh.vertices = this.buffer.v;
                this.mesh.normals = this.buffer.n;
                this.mesh.colors = this.buffer.c;
                this.mesh.uv = this.buffer.t;
                this.mesh.SetIndices(this.buffer.i, MeshTopology.Quads, 0);
            }
            else
            {
                this.mesh.vertices = this.buffer.v;
                this.mesh.normals = this.buffer.n;
                this.mesh.colors = this.buffer.c;
                this.mesh.uv = this.buffer.t;
            }
            Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
            bounds.SetMinMax(min, max);
            this.mesh.bounds = bounds;
        }

        public void Dispose()
        {
            if (this.mesh != null)
            {
                Object.DestroyImmediate(this.mesh);
            }
        }

        public bool Equals(LaserGraphics.MeshBuffer buf)
        {
            return (!object.ReferenceEquals(buf, null) && (this.instanceID == buf.instanceID));
        }

        public override bool Equals(object obj)
        {
            return ((obj is LaserGraphics.MeshBuffer) && (this.instanceID == ((LaserGraphics.MeshBuffer) obj).instanceID));
        }

        public static LaserGraphics.MeshBuffer ForBeamMaterial(Material material)
        {
            if (!Register.hasBeam || (Register.lastBeam.material != material))
            {
                Register.lastBeam = ForMaterial(Register.beams, material);
                Register.hasBeam = true;
            }
            return Register.lastBeam;
        }

        public static LaserGraphics.MeshBuffer ForDotMaterial(Material material)
        {
            if (!Register.hasDot || (Register.lastDot.material != material))
            {
                Register.lastDot = ForMaterial(Register.dots, material);
                Register.hasDot = true;
            }
            return Register.lastDot;
        }

        private static LaserGraphics.MeshBuffer ForMaterial(Dictionary<Material, LaserGraphics.MeshBuffer> all, Material material)
        {
            LaserGraphics.MeshBuffer buffer;
            if (!all.TryGetValue(material, out buffer))
            {
                buffer = new LaserGraphics.MeshBuffer(material);
                all.Add(material, buffer);
            }
            return buffer;
        }

        public override int GetHashCode()
        {
            return this.instanceID;
        }

        public static void Reset()
        {
            Register.lastDot = (LaserGraphics.MeshBuffer) (Register.lastBeam = null);
            Register.hasDot = Register.hasBeam = false;
        }

        public bool Resize()
        {
            return this.SetSize(this.measureSize);
        }

        private bool SetSize(int size)
        {
            if (this.quadCount == size)
            {
                return false;
            }
            if (size == 0)
            {
                this.buffer = null;
            }
            else
            {
                this.buffer = LaserGraphics.VertexBuffer.Size(size);
            }
            this.quadCount = size;
            return true;
        }

        private static class Register
        {
            public static readonly Dictionary<Material, LaserGraphics.MeshBuffer> beams = new Dictionary<Material, LaserGraphics.MeshBuffer>();
            public static readonly Dictionary<Material, LaserGraphics.MeshBuffer> dots = new Dictionary<Material, LaserGraphics.MeshBuffer>();
            public static bool hasBeam;
            public static bool hasDot;
            public static LaserGraphics.MeshBuffer lastBeam;
            public static LaserGraphics.MeshBuffer lastDot;
        }
    }

    internal class VertexBuffer
    {
        public readonly Color[] c;
        public readonly int[] i;
        public readonly Vector3[] n;
        public readonly int quadCount;
        public readonly Vector2[] t;
        public readonly Vector3[] v;
        public readonly int vertexCount;

        private VertexBuffer(int quadCount)
        {
            this.quadCount = quadCount;
            this.vertexCount = quadCount * 4;
            if (this.vertexCount > 0)
            {
                this.v = new Vector3[this.vertexCount];
                this.t = new Vector2[this.vertexCount];
                this.n = new Vector3[this.vertexCount];
                this.c = new Color[this.vertexCount];
                this.i = new int[this.vertexCount];
            }
            for (int i = 0; i < this.vertexCount; i++)
            {
                this.i[i] = i;
            }
        }

        public static LaserGraphics.VertexBuffer Size(int i)
        {
            LaserGraphics.VertexBuffer buffer;
            if (!Register.all.TryGetValue(i, out buffer))
            {
                Register.all.Add(i, buffer = new LaserGraphics.VertexBuffer(i));
            }
            return buffer;
        }

        private static class Register
        {
            public static readonly Dictionary<int, LaserGraphics.VertexBuffer> all = new Dictionary<int, LaserGraphics.VertexBuffer>();
        }
    }
}

