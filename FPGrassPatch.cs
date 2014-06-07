using System;
using UnityEngine;

[ExecuteInEditMode]
public class FPGrassPatch : MonoBehaviour, IFPGrassAsset
{
    [NonSerialized]
    private Bounds lastBounds;
    [NonSerialized]
    private Vector3 lastRenderPosition;
    [SerializeField]
    public FPGrassLevel level;
    [SerializeField]
    private Mesh mesh;
    [SerializeField]
    private float patchSize;
    [NonSerialized]
    private Mesh runtimeMesh;
    [NonSerialized]
    public Transform transform;

    private void OnDestroy()
    {
    }

    private void OnEnable()
    {
        this.transform = base.transform;
        this.lastRenderPosition.x = this.lastRenderPosition.y = this.lastRenderPosition.z = float.NegativeInfinity;
    }

    internal void Render(ref FPGrass.RenderArguments renderArgs)
    {
        if (renderArgs.terrain != null)
        {
            Bounds lastBounds;
            bool flag;
            Vector3 renderPosition = renderArgs.center + this.transform.position;
            if (((renderPosition.x == this.lastRenderPosition.x) && (renderPosition.y == this.lastRenderPosition.y)) && (renderPosition.z == this.lastRenderPosition.z))
            {
                lastBounds = this.lastBounds;
                flag = false;
            }
            else
            {
                float num2;
                float num3;
                float num4;
                float num5;
                float num6;
                float num7;
                Vector3 worldPosition = renderPosition;
                Vector3 vector3 = worldPosition;
                Vector3 vector4 = worldPosition;
                Vector3 vector5 = worldPosition;
                float num = this.patchSize * 0.5f;
                worldPosition.x -= num;
                worldPosition.z += num;
                vector3.x -= num;
                vector3.z -= num;
                vector4.x += num;
                vector4.z += num;
                vector5.x += num;
                vector5.z -= num;
                float num8 = renderArgs.terrain.SampleHeight(worldPosition);
                float num9 = renderArgs.terrain.SampleHeight(vector3);
                float num10 = renderArgs.terrain.SampleHeight(vector4);
                float num11 = renderArgs.terrain.SampleHeight(vector5);
                if (num8 < num9)
                {
                    num4 = num8;
                    num6 = num9;
                }
                else
                {
                    num4 = num9;
                    num6 = num8;
                }
                if (num10 < num11)
                {
                    num5 = num10;
                    num7 = num11;
                }
                else
                {
                    num5 = num11;
                    num7 = num10;
                }
                if (num4 < num5)
                {
                    num2 = num4;
                }
                else
                {
                    num2 = num5;
                }
                if (num6 > num7)
                {
                    num3 = num6;
                }
                else
                {
                    num3 = num7;
                }
                vector3.y = num2 - 5f;
                vector4.y = num3 + 5f;
                lastBounds = new Bounds();
                lastBounds.SetMinMax(vector3, vector4);
                flag = lastBounds != this.lastBounds;
            }
            if (this.runtimeMesh == null)
            {
                this.runtimeMesh = this.mesh;
            }
            if (flag)
            {
                this.runtimeMesh.bounds = new Bounds(lastBounds.center - renderPosition, lastBounds.size);
                this.lastBounds = lastBounds;
            }
            if (GeometryUtility.TestPlanesAABB(renderArgs.frustum, lastBounds))
            {
                this.level.Draw(this, this.runtimeMesh, ref renderPosition, ref renderArgs);
            }
        }
    }
}

