using NGUI.Meshing;
using NGUI.Structures;
using System;
using UnityEngine;

public class UIGeometricSprite : UISprite
{
    [SerializeField, HideInInspector]
    private bool mFillCenter;
    protected Rect mInner;
    protected Rect mInnerUV;
    protected Vector3 mScale;

    protected UIGeometricSprite(UIWidget.WidgetFlags additionalFlags) : base(additionalFlags)
    {
        this.mFillCenter = true;
        this.mScale = Vector3.one;
    }

    public override void MakePixelPerfect()
    {
        Vector3 localPosition = base.cachedTransform.localPosition;
        localPosition.x = Mathf.RoundToInt(localPosition.x);
        localPosition.y = Mathf.RoundToInt(localPosition.y);
        localPosition.z = Mathf.RoundToInt(localPosition.z);
        base.cachedTransform.localPosition = localPosition;
        Vector3 localScale = base.cachedTransform.localScale;
        localScale.x = Mathf.RoundToInt(localScale.x * 0.5f) << 1;
        localScale.y = Mathf.RoundToInt(localScale.y * 0.5f) << 1;
        localScale.z = 1f;
        base.cachedTransform.localScale = localScale;
    }

    public override void OnFill(MeshBuffer m)
    {
        if (base.mOuterUV == this.mInnerUV)
        {
            base.OnFill(m);
        }
        else
        {
            NineRectangle rectangle;
            NineRectangle rectangle2;
            Vector4 vector;
            Vector4 vector2;
            float3 num = new float3 {
                xyz = base.cachedTransform.localScale
            };
            vector.x = this.mOuterUV.xMin;
            vector.y = this.mInnerUV.xMin;
            vector.z = this.mInnerUV.xMax;
            vector.w = this.mOuterUV.xMax;
            vector2.x = this.mOuterUV.yMin;
            vector2.y = this.mInnerUV.yMin;
            vector2.z = this.mInnerUV.yMax;
            vector2.w = this.mOuterUV.yMax;
            NineRectangle.Calculate(base.pivot, base.atlas.pixelSize, base.mainTexture, ref vector, ref vector2, ref num.xy, out rectangle, out rectangle2);
            Color color = base.color;
            if (this.mFillCenter)
            {
                NineRectangle.Fill9(ref rectangle, ref rectangle2, ref color, m);
            }
            else
            {
                NineRectangle.Fill8(ref rectangle, ref rectangle2, ref color, m);
            }
        }
    }

    public override void UpdateUVs(bool force)
    {
        if (base.cachedTransform.localScale != this.mScale)
        {
            this.mScale = base.cachedTransform.localScale;
            base.ChangedAuto();
        }
        if ((base.sprite != null) && ((force || (this.mInner != base.mSprite.inner)) || (base.mOuter != base.mSprite.outer)))
        {
            Texture mainTexture = base.mainTexture;
            if (mainTexture != null)
            {
                this.mInner = base.mSprite.inner;
                base.mOuter = base.mSprite.outer;
                this.mInnerUV = this.mInner;
                base.mOuterUV = base.mOuter;
                if (base.atlas.coordinates == UIAtlas.Coordinates.Pixels)
                {
                    base.mOuterUV = NGUIMath.ConvertToTexCoords(base.mOuterUV, mainTexture.width, mainTexture.height);
                    this.mInnerUV = NGUIMath.ConvertToTexCoords(this.mInnerUV, mainTexture.width, mainTexture.height);
                }
            }
        }
    }

    public bool fillCenter
    {
        get
        {
            return this.mFillCenter;
        }
        set
        {
            if (this.mFillCenter != value)
            {
                this.mFillCenter = value;
                this.MarkAsChanged();
            }
        }
    }

    public Rect innerUV
    {
        get
        {
            this.UpdateUVs(false);
            return this.mInnerUV;
        }
    }
}

