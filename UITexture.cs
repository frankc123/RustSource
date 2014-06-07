using NGUI.Meshing;
using System;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Texture"), ExecuteInEditMode]
public class UITexture : UIWidget
{
    [SerializeField, HideInInspector]
    private bool _mirrorX;
    [HideInInspector, SerializeField]
    private bool _mirrorY;

    public UITexture() : base(UIWidget.WidgetFlags.KeepsMaterial)
    {
    }

    public override void MakePixelPerfect()
    {
        Texture mainTexture = base.mainTexture;
        if (mainTexture != null)
        {
            Vector3 localScale = base.cachedTransform.localScale;
            localScale.x = mainTexture.width;
            localScale.y = mainTexture.height;
            localScale.z = 1f;
            base.cachedTransform.localScale = localScale;
        }
        base.MakePixelPerfect();
    }

    public override void OnFill(MeshBuffer m)
    {
        Vertex vertex;
        Vertex vertex2;
        Vertex vertex3;
        Vertex vertex4;
        vertex.z = 0f;
        vertex2.z = 0f;
        vertex3.z = 0f;
        vertex4.z = 0f;
        Color color = base.color;
        vertex.r = vertex2.r = vertex3.r = vertex4.r = color.r;
        vertex.g = vertex2.g = vertex3.g = vertex4.g = color.g;
        vertex.b = vertex2.b = vertex3.b = vertex4.b = color.b;
        vertex.a = vertex2.a = vertex3.a = vertex4.a = color.a;
        if (this._mirrorX)
        {
            if (this._mirrorY)
            {
                vertex.x = 0.5f;
                vertex.y = -0.5f;
                vertex2.x = 0.5f;
                vertex2.y = -1f;
                vertex3.x = 0f;
                vertex3.y = -1f;
                vertex4.x = 0f;
                vertex4.y = -0.5f;
                vertex.u = 1f;
                vertex.v = 1f;
                vertex2.u = 1f;
                vertex2.v = 0f;
                vertex3.u = 0f;
                vertex3.v = 0f;
                vertex4.u = 0f;
                vertex4.v = 1f;
                m.TextureQuad(vertex, vertex2, vertex3, vertex4);
                vertex.x = 0.5f;
                vertex.y = 0f;
                vertex2.x = 0.5f;
                vertex2.y = -0.5f;
                vertex3.x = 0f;
                vertex3.y = -0.5f;
                vertex4.x = 0f;
                vertex4.y = 0f;
                vertex.u = 0f;
                vertex.v = 1f;
                vertex2.u = 0f;
                vertex2.v = 0f;
                vertex3.u = 1f;
                vertex3.v = 0f;
                vertex4.u = 1f;
                vertex4.v = 1f;
                m.TextureQuad(vertex, vertex2, vertex3, vertex4);
                vertex.x = 1f;
                vertex.y = -0.5f;
                vertex2.x = 1f;
                vertex2.y = -1f;
                vertex3.x = 0.5f;
                vertex3.y = -1f;
                vertex4.x = 0.5f;
                vertex4.y = -0.5f;
                vertex.u = 1f;
                vertex.v = 1f;
                vertex2.u = 1f;
                vertex2.v = 0f;
                vertex3.u = 0f;
                vertex3.v = 0f;
                vertex4.u = 0f;
                vertex4.v = 1f;
                m.TextureQuad(vertex, vertex2, vertex3, vertex4);
                vertex.x = 1f;
                vertex.y = 0f;
                vertex2.x = 1f;
                vertex2.y = -0.5f;
                vertex3.x = 0.5f;
                vertex3.y = -0.5f;
                vertex4.x = 0.5f;
                vertex4.y = 0f;
                vertex.u = 0f;
                vertex.v = 1f;
                vertex2.u = 0f;
                vertex2.v = 0f;
                vertex3.u = 1f;
                vertex3.v = 0f;
                vertex4.u = 1f;
                vertex4.v = 1f;
                m.TextureQuad(vertex, vertex2, vertex3, vertex4);
            }
            else
            {
                vertex.x = 0.5f;
                vertex.y = 0f;
                vertex2.x = 0.5f;
                vertex2.y = -1f;
                vertex3.x = 0f;
                vertex3.y = -1f;
                vertex4.x = 0f;
                vertex4.y = 0f;
                vertex.u = 1f;
                vertex.v = 1f;
                vertex2.u = 1f;
                vertex2.v = 0f;
                vertex3.u = 0f;
                vertex3.v = 0f;
                vertex4.u = 0f;
                vertex4.v = 1f;
                m.TextureQuad(vertex, vertex2, vertex3, vertex4);
                vertex.x = 1f;
                vertex.y = 0f;
                vertex2.x = 1f;
                vertex2.y = -1f;
                vertex3.x = 0.5f;
                vertex3.y = -1f;
                vertex4.x = 0.5f;
                vertex4.y = 0f;
                vertex.u = 0f;
                vertex.v = 1f;
                vertex2.u = 0f;
                vertex2.v = 0f;
                vertex3.u = 1f;
                vertex3.v = 0f;
                vertex4.u = 1f;
                vertex4.v = 1f;
                m.TextureQuad(vertex, vertex2, vertex3, vertex4);
            }
        }
        else if (this._mirrorY)
        {
            vertex.x = 1f;
            vertex.y = -0.5f;
            vertex2.x = 1f;
            vertex2.y = -1f;
            vertex3.x = 0f;
            vertex3.y = -1f;
            vertex4.x = 0f;
            vertex4.y = -0.5f;
            vertex.u = 1f;
            vertex.v = 0f;
            vertex2.u = 1f;
            vertex2.v = 1f;
            vertex3.u = 0f;
            vertex3.v = 1f;
            vertex4.u = 0f;
            vertex4.v = 0f;
            m.TextureQuad(vertex, vertex2, vertex3, vertex4);
            vertex.x = 1f;
            vertex.y = 0f;
            vertex2.x = 1f;
            vertex2.y = -0.5f;
            vertex3.x = 0f;
            vertex3.y = -0.5f;
            vertex4.x = 0f;
            vertex4.y = 0f;
            vertex.u = 1f;
            vertex.v = 1f;
            vertex2.u = 1f;
            vertex2.v = 0f;
            vertex3.u = 0f;
            vertex3.v = 0f;
            vertex4.u = 0f;
            vertex4.v = 1f;
            m.TextureQuad(vertex, vertex2, vertex3, vertex4);
        }
        else
        {
            vertex.x = 1f;
            vertex.y = 0f;
            vertex2.x = 1f;
            vertex2.y = -1f;
            vertex3.x = 0f;
            vertex3.y = -1f;
            vertex4.x = 0f;
            vertex4.y = 0f;
            vertex.u = 1f;
            vertex.v = 1f;
            vertex2.u = 1f;
            vertex2.v = 0f;
            vertex3.u = 0f;
            vertex3.v = 0f;
            vertex4.u = 0f;
            vertex4.v = 1f;
            m.TextureQuad(vertex, vertex2, vertex3, vertex4);
        }
    }

    public bool mirrorX
    {
        get
        {
            return this._mirrorX;
        }
        set
        {
            if (this._mirrorX != value)
            {
                this._mirrorX = value;
                base.ChangedAuto();
            }
        }
    }

    public bool mirrorY
    {
        get
        {
            return this._mirrorY;
        }
        set
        {
            if (this._mirrorY != value)
            {
                this._mirrorY = value;
                base.ChangedAuto();
            }
        }
    }
}

