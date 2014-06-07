using NGUI.Meshing;
using System;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Sprite (Basic)"), ExecuteInEditMode]
public class UISprite : UIWidget
{
    private const UIWidget.WidgetFlags kRequiredFlags = (UIWidget.WidgetFlags.CustomMaterialGet | UIWidget.WidgetFlags.CustomPivotOffset);
    [HideInInspector, SerializeField]
    private UIAtlas mAtlas;
    private string mLastName;
    protected Rect mOuter;
    protected Rect mOuterUV;
    protected UIAtlas.Sprite mSprite;
    [SerializeField, HideInInspector]
    private string mSpriteName;
    private bool mSpriteSet;

    public UISprite() : base(UIWidget.WidgetFlags.CustomMaterialGet | UIWidget.WidgetFlags.CustomPivotOffset)
    {
        this.mLastName = string.Empty;
    }

    protected UISprite(UIWidget.WidgetFlags additionalFlags) : base((UIWidget.WidgetFlags) ((byte) ((UIWidget.WidgetFlags.CustomMaterialGet | UIWidget.WidgetFlags.CustomPivotOffset) | additionalFlags)))
    {
        this.mLastName = string.Empty;
    }

    protected override void GetCustomVector2s(int start, int end, UIWidget.WidgetFlags[] flags, Vector2[] v)
    {
        for (int i = start; i < end; i++)
        {
            if (flags[i] == UIWidget.WidgetFlags.CustomPivotOffset)
            {
                v[i] = this.pivotOffset;
            }
            else
            {
                base.GetCustomVector2s(i, i + 1, flags, v);
            }
        }
    }

    public override void MakePixelPerfect()
    {
        if (this.sprite != null)
        {
            Texture mainTexture = base.mainTexture;
            Vector3 localScale = base.cachedTransform.localScale;
            if (mainTexture != null)
            {
                Rect rect = NGUIMath.ConvertToPixels(this.outerUV, mainTexture.width, mainTexture.height, true);
                float pixelSize = this.atlas.pixelSize;
                localScale.x = Mathf.RoundToInt(rect.width * pixelSize);
                localScale.y = Mathf.RoundToInt(rect.height * pixelSize);
                localScale.z = 1f;
                base.cachedTransform.localScale = localScale;
            }
            int num2 = Mathf.RoundToInt(localScale.x * ((1f + this.mSprite.paddingLeft) + this.mSprite.paddingRight));
            int num3 = Mathf.RoundToInt(localScale.y * ((1f + this.mSprite.paddingTop) + this.mSprite.paddingBottom));
            Vector3 localPosition = base.cachedTransform.localPosition;
            localPosition.z = Mathf.RoundToInt(localPosition.z);
            if (((num2 % 2) == 1) && (((base.pivot == UIWidget.Pivot.Top) || (base.pivot == UIWidget.Pivot.Center)) || (base.pivot == UIWidget.Pivot.Bottom)))
            {
                localPosition.x = Mathf.Floor(localPosition.x) + 0.5f;
            }
            else
            {
                localPosition.x = Mathf.Round(localPosition.x);
            }
            if (((num3 % 2) == 1) && (((base.pivot == UIWidget.Pivot.Left) || (base.pivot == UIWidget.Pivot.Center)) || (base.pivot == UIWidget.Pivot.Right)))
            {
                localPosition.y = Mathf.Ceil(localPosition.y) - 0.5f;
            }
            else
            {
                localPosition.y = Mathf.Round(localPosition.y);
            }
            base.cachedTransform.localPosition = localPosition;
        }
    }

    public override void OnFill(MeshBuffer m)
    {
        m.FastQuad(this.mOuterUV, base.color);
    }

    protected override void OnStart()
    {
        if (this.mAtlas != null)
        {
            this.UpdateUVs(true);
        }
    }

    public override bool OnUpdate()
    {
        if (this.mLastName != this.mSpriteName)
        {
            this.mSprite = null;
            base.ChangedAuto();
            this.mLastName = this.mSpriteName;
            this.UpdateUVs(false);
            return true;
        }
        this.UpdateUVs(false);
        return false;
    }

    public virtual void UpdateUVs(bool force)
    {
        if ((this.sprite != null) && (force || (this.mOuter != this.mSprite.outer)))
        {
            Texture mainTexture = base.mainTexture;
            if (mainTexture != null)
            {
                this.mOuter = this.mSprite.outer;
                this.mOuterUV = this.mOuter;
                if (this.mAtlas.coordinates == UIAtlas.Coordinates.Pixels)
                {
                    this.mOuterUV = NGUIMath.ConvertToTexCoords(this.mOuterUV, mainTexture.width, mainTexture.height);
                }
                base.ChangedAuto();
            }
        }
    }

    public UIAtlas atlas
    {
        get
        {
            return this.mAtlas;
        }
        set
        {
            if (this.mAtlas != value)
            {
                this.mAtlas = value;
                this.mSpriteSet = false;
                this.mSprite = null;
                this.material = (this.mAtlas == null) ? null : ((UIMaterial) this.mAtlas.spriteMaterial);
                if ((string.IsNullOrEmpty(this.mSpriteName) && (this.mAtlas != null)) && (this.mAtlas.spriteList.Count > 0))
                {
                    this.sprite = this.mAtlas.spriteList[0];
                    this.mSpriteName = this.mSprite.name;
                }
                if (!string.IsNullOrEmpty(this.mSpriteName))
                {
                    string mSpriteName = this.mSpriteName;
                    this.mSpriteName = string.Empty;
                    this.spriteName = mSpriteName;
                    base.ChangedAuto();
                    this.UpdateUVs(true);
                }
            }
        }
    }

    public Vector4 border
    {
        get
        {
            if (((byte) (base.widgetFlags & UIWidget.WidgetFlags.CustomBorder)) == 0x10)
            {
                return this.customBorder;
            }
            return Vector4.zero;
        }
    }

    protected virtual Vector4 customBorder
    {
        get
        {
            throw new NotSupportedException();
        }
    }

    protected override UIMaterial customMaterial
    {
        get
        {
            return this.material;
        }
    }

    public UIMaterial material
    {
        get
        {
            UIMaterial baseMaterial = base.baseMaterial;
            if (baseMaterial == null)
            {
                baseMaterial = (this.mAtlas == null) ? null : ((UIMaterial) this.mAtlas.spriteMaterial);
                this.mSprite = null;
                base.baseMaterial = baseMaterial;
                if (baseMaterial != null)
                {
                    this.UpdateUVs(true);
                }
            }
            return baseMaterial;
        }
        set
        {
            base.material = value;
        }
    }

    public Rect outerUV
    {
        get
        {
            this.UpdateUVs(false);
            return this.mOuterUV;
        }
    }

    public Vector2 pivotOffset
    {
        get
        {
            Vector2 zero = Vector2.zero;
            if (this.sprite != null)
            {
                UIWidget.Pivot pivot = base.pivot;
                switch (pivot)
                {
                    case UIWidget.Pivot.Top:
                    case UIWidget.Pivot.Center:
                    case UIWidget.Pivot.Bottom:
                        zero.x = ((-1f - this.mSprite.paddingRight) + this.mSprite.paddingLeft) * 0.5f;
                        break;

                    case UIWidget.Pivot.TopRight:
                    case UIWidget.Pivot.Right:
                    case UIWidget.Pivot.BottomRight:
                        zero.x = -1f - this.mSprite.paddingRight;
                        break;

                    default:
                        zero.x = this.mSprite.paddingLeft;
                        break;
                }
                switch (pivot)
                {
                    case UIWidget.Pivot.Left:
                    case UIWidget.Pivot.Center:
                    case UIWidget.Pivot.Right:
                        zero.y = ((1f + this.mSprite.paddingBottom) - this.mSprite.paddingTop) * 0.5f;
                        return zero;

                    case UIWidget.Pivot.BottomLeft:
                    case UIWidget.Pivot.Bottom:
                    case UIWidget.Pivot.BottomRight:
                        zero.y = 1f + this.mSprite.paddingBottom;
                        return zero;
                }
                zero.y = -this.mSprite.paddingTop;
            }
            return zero;
        }
    }

    public UIAtlas.Sprite sprite
    {
        get
        {
            if (!this.mSpriteSet)
            {
                this.mSprite = null;
            }
            if ((this.mSprite == null) && (this.mAtlas != null))
            {
                if (!string.IsNullOrEmpty(this.mSpriteName))
                {
                    this.sprite = this.mAtlas.GetSprite(this.mSpriteName);
                }
                if ((this.mSprite == null) && (this.mAtlas.spriteList.Count > 0))
                {
                    this.sprite = this.mAtlas.spriteList[0];
                    this.mSpriteName = this.mSprite.name;
                }
                if (this.mSprite != null)
                {
                    this.material = (UIMaterial) this.mAtlas.spriteMaterial;
                }
            }
            return this.mSprite;
        }
        set
        {
            this.mSprite = value;
            this.mSpriteSet = true;
            this.material = ((this.mSprite == null) || (this.mAtlas == null)) ? null : ((UIMaterial) this.mAtlas.spriteMaterial);
        }
    }

    public string spriteName
    {
        get
        {
            return this.mSpriteName;
        }
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                if (!string.IsNullOrEmpty(this.mSpriteName))
                {
                    this.mSpriteName = string.Empty;
                    this.mSprite = null;
                    base.ChangedAuto();
                }
            }
            else if (this.mSpriteName != value)
            {
                this.mSpriteName = value;
                this.mSprite = null;
                base.ChangedAuto();
                if (this.mSprite != null)
                {
                    this.UpdateUVs(true);
                }
            }
        }
    }
}

