using NGUI.Meshing;
using System;
using UnityEngine;

[ExecuteInEditMode, AddComponentMenu("NGUI/UI/Sprite (Tiled)")]
public class UITiledSprite : UIGeometricSprite
{
    public UITiledSprite() : base(0)
    {
    }

    public override void MakePixelPerfect()
    {
        Vector3 localPosition = base.cachedTransform.localPosition;
        localPosition.x = Mathf.RoundToInt(localPosition.x);
        localPosition.y = Mathf.RoundToInt(localPosition.y);
        localPosition.z = Mathf.RoundToInt(localPosition.z);
        base.cachedTransform.localPosition = localPosition;
        Vector3 localScale = base.cachedTransform.localScale;
        localScale.x = Mathf.RoundToInt(localScale.x);
        localScale.y = Mathf.RoundToInt(localScale.y);
        localScale.z = 1f;
        base.cachedTransform.localScale = localScale;
    }

    public override void OnFill(MeshBuffer m)
    {
        Texture mainTexture = base.material.mainTexture;
        if (mainTexture != null)
        {
            Vertex vertex;
            Vertex vertex2;
            Vertex vertex3;
            Vertex vertex4;
            Rect mInner = base.mInner;
            if (base.atlas.coordinates == UIAtlas.Coordinates.TexCoords)
            {
                mInner = NGUIMath.ConvertToPixels(mInner, mainTexture.width, mainTexture.height, true);
            }
            Vector2 localScale = base.cachedTransform.localScale;
            float pixelSize = base.atlas.pixelSize;
            float num2 = Mathf.Abs((float) (mInner.width / localScale.x)) * pixelSize;
            float num3 = Mathf.Abs((float) (mInner.height / localScale.y)) * pixelSize;
            if ((num2 < 0.01f) || (num3 < 0.01f))
            {
                Debug.LogWarning("The tiled sprite (" + NGUITools.GetHierarchy(base.gameObject) + ") is too small.\nConsider using a bigger one.");
                num2 = 0.01f;
                num3 = 0.01f;
            }
            Vector2 vector2 = new Vector2(mInner.xMin / ((float) mainTexture.width), mInner.yMin / ((float) mainTexture.height));
            Vector2 vector3 = new Vector2(mInner.xMax / ((float) mainTexture.width), mInner.yMax / ((float) mainTexture.height));
            Vector2 vector4 = vector3;
            float num4 = 0f;
            Color color = base.color;
            vertex.r = vertex2.r = vertex3.r = vertex4.r = color.r;
            vertex.g = vertex2.g = vertex3.g = vertex4.g = color.g;
            vertex.b = vertex2.b = vertex3.b = vertex4.b = color.b;
            vertex.a = vertex2.a = vertex3.a = vertex4.a = color.a;
            vertex.z = vertex2.z = vertex3.z = vertex4.z = 0f;
            while (num4 < 1f)
            {
                float num5 = 0f;
                vector4.x = vector3.x;
                float num6 = num4 + num3;
                if (num6 > 1f)
                {
                    vector4.y = vector2.y + (((vector3.y - vector2.y) * (1f - num4)) / (num6 - num4));
                    num6 = 1f;
                }
                while (num5 < 1f)
                {
                    float num7 = num5 + num2;
                    if (num7 > 1f)
                    {
                        vector4.x = vector2.x + (((vector3.x - vector2.x) * (1f - num5)) / (num7 - num5));
                        num7 = 1f;
                    }
                    vertex.x = num7;
                    vertex.y = -num4;
                    vertex2.x = num7;
                    vertex2.y = -num6;
                    vertex3.x = num5;
                    vertex3.y = -num6;
                    vertex4.x = num5;
                    vertex4.y = -num4;
                    vertex.u = vector4.x;
                    vertex.v = 1f - vector2.y;
                    vertex2.u = vector4.x;
                    vertex2.v = 1f - vector4.y;
                    vertex3.u = vector2.x;
                    vertex3.v = 1f - vector4.y;
                    vertex4.u = vector2.x;
                    vertex4.v = 1f - vector2.y;
                    m.Quad(vertex, vertex2, vertex3, vertex4);
                    num5 += num2;
                }
                num4 += num3;
            }
        }
    }
}

