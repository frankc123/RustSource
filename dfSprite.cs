using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

[Serializable, RequireComponent(typeof(BoxCollider)), AddComponentMenu("Daikon Forge/User Interface/Sprite/Basic"), ExecuteInEditMode]
public class dfSprite : dfControl
{
    [SerializeField]
    protected dfAtlas atlas;
    [SerializeField]
    protected float fillAmount = 1f;
    [SerializeField]
    protected dfFillDirection fillDirection;
    [SerializeField]
    protected dfSpriteFlip flip;
    [SerializeField]
    protected bool invertFill;
    [SerializeField]
    protected string spriteName;
    private static int[] TRIANGLE_INDICES = new int[] { 0, 1, 3, 3, 1, 2 };

    public event PropertyChangedEventHandler<string> SpriteNameChanged;

    public override Vector2 CalculateMinimumSize()
    {
        dfAtlas.ItemInfo spriteInfo = this.SpriteInfo;
        if (spriteInfo == null)
        {
            return Vector2.zero;
        }
        RectOffset border = spriteInfo.border;
        if (((border != null) && (border.horizontal > 0)) && (border.vertical > 0))
        {
            return Vector2.Max(base.CalculateMinimumSize(), new Vector2((float) border.horizontal, (float) border.vertical));
        }
        return base.CalculateMinimumSize();
    }

    private static void doFill(dfRenderData renderData, RenderOptions options)
    {
        int baseIndex = options.baseIndex;
        dfList<Vector3> vertices = renderData.Vertices;
        dfList<Vector2> uV = renderData.UV;
        int num2 = baseIndex;
        int num3 = baseIndex + 1;
        int num4 = baseIndex + 3;
        int num5 = baseIndex + 2;
        if (options.invertFill)
        {
            if (options.fillDirection == dfFillDirection.Horizontal)
            {
                num2 = baseIndex + 1;
                num3 = baseIndex;
                num4 = baseIndex + 2;
                num5 = baseIndex + 3;
            }
            else
            {
                num2 = baseIndex + 3;
                num3 = baseIndex + 2;
                num4 = baseIndex;
                num5 = baseIndex + 1;
            }
        }
        if (options.fillDirection == dfFillDirection.Horizontal)
        {
            vertices[num3] = Vector3.Lerp(vertices[num3], vertices[num2], 1f - options.fillAmount);
            vertices[num5] = Vector3.Lerp(vertices[num5], vertices[num4], 1f - options.fillAmount);
            uV[num3] = Vector2.Lerp(uV[num3], uV[num2], 1f - options.fillAmount);
            uV[num5] = Vector2.Lerp(uV[num5], uV[num4], 1f - options.fillAmount);
        }
        else
        {
            vertices[num4] = Vector3.Lerp(vertices[num4], vertices[num2], 1f - options.fillAmount);
            vertices[num5] = Vector3.Lerp(vertices[num5], vertices[num3], 1f - options.fillAmount);
            uV[num4] = Vector2.Lerp(uV[num4], uV[num2], 1f - options.fillAmount);
            uV[num5] = Vector2.Lerp(uV[num5], uV[num3], 1f - options.fillAmount);
        }
    }

    protected internal override void OnLocalize()
    {
        base.OnLocalize();
        this.SpriteName = base.getLocalizedValue(this.spriteName);
    }

    protected override void OnRebuildRenderData()
    {
        if ((((this.Atlas != null) && (this.Atlas.Material != null)) && base.IsVisible) && (this.SpriteInfo != null))
        {
            base.renderData.Material = this.Atlas.Material;
            Color32 color = base.ApplyOpacity(!base.IsEnabled ? base.disabledColor : base.color);
            RenderOptions options = new RenderOptions {
                atlas = this.Atlas,
                color = color,
                fillAmount = this.fillAmount,
                fillDirection = this.fillDirection,
                flip = this.flip,
                invertFill = this.invertFill,
                offset = base.pivot.TransformToUpperLeft(base.Size),
                pixelsToUnits = base.PixelsToUnits(),
                size = base.Size,
                spriteInfo = this.SpriteInfo
            };
            renderSprite(base.renderData, options);
        }
    }

    protected internal virtual void OnSpriteNameChanged(string value)
    {
        object[] args = new object[] { value };
        base.Signal("OnSpriteNameChanged", args);
        if (this.SpriteNameChanged != null)
        {
            this.SpriteNameChanged(this, value);
        }
    }

    private static void rebuildColors(dfRenderData renderData, RenderOptions options)
    {
        dfList<Color32> colors = renderData.Colors;
        colors.Add(options.color);
        colors.Add(options.color);
        colors.Add(options.color);
        colors.Add(options.color);
    }

    private static void rebuildTriangles(dfRenderData renderData, RenderOptions options)
    {
        int baseIndex = options.baseIndex;
        dfList<int> triangles = renderData.Triangles;
        triangles.EnsureCapacity(triangles.Count + TRIANGLE_INDICES.Length);
        for (int i = 0; i < TRIANGLE_INDICES.Length; i++)
        {
            triangles.Add(baseIndex + TRIANGLE_INDICES[i]);
        }
    }

    private static void rebuildUV(dfRenderData renderData, RenderOptions options)
    {
        Rect region = options.spriteInfo.region;
        dfList<Vector2> uV = renderData.UV;
        uV.Add(new Vector2(region.x, region.yMax));
        uV.Add(new Vector2(region.xMax, region.yMax));
        uV.Add(new Vector2(region.xMax, region.y));
        uV.Add(new Vector2(region.x, region.y));
        Vector2 zero = Vector2.zero;
        if (options.flip.IsSet(dfSpriteFlip.FlipHorizontal))
        {
            zero = uV[1];
            uV[1] = uV[0];
            uV[0] = zero;
            zero = uV[3];
            uV[3] = uV[2];
            uV[2] = zero;
        }
        if (options.flip.IsSet(dfSpriteFlip.FlipVertical))
        {
            zero = uV[0];
            uV[0] = uV[3];
            uV[3] = zero;
            zero = uV[1];
            uV[1] = uV[2];
            uV[2] = zero;
        }
    }

    private static void rebuildVertices(dfRenderData renderData, RenderOptions options)
    {
        dfList<Vector3> vertices = renderData.Vertices;
        int baseIndex = options.baseIndex;
        float x = 0f;
        float y = 0f;
        float num4 = Mathf.Ceil(options.size.x);
        float num5 = Mathf.Ceil(-options.size.y);
        vertices.Add((Vector3) (new Vector3(x, y, 0f) * options.pixelsToUnits));
        vertices.Add((Vector3) (new Vector3(num4, y, 0f) * options.pixelsToUnits));
        vertices.Add((Vector3) (new Vector3(num4, num5, 0f) * options.pixelsToUnits));
        vertices.Add((Vector3) (new Vector3(x, num5, 0f) * options.pixelsToUnits));
        Vector3 vector = (Vector3) (options.offset.RoundToInt() * options.pixelsToUnits);
        for (int i = 0; i < 4; i++)
        {
            vertices[baseIndex + i] = (vertices[baseIndex + i] + vector).Quantize(options.pixelsToUnits);
        }
    }

    internal static void renderSprite(dfRenderData data, RenderOptions options)
    {
        options.baseIndex = data.Vertices.Count;
        rebuildTriangles(data, options);
        rebuildVertices(data, options);
        rebuildUV(data, options);
        rebuildColors(data, options);
        if ((options.fillAmount > -1f) && (options.fillAmount < 1f))
        {
            doFill(data, options);
        }
    }

    public override string ToString()
    {
        if (!string.IsNullOrEmpty(this.spriteName))
        {
            return string.Format("{0} ({1})", base.name, this.spriteName);
        }
        return base.ToString();
    }

    public dfAtlas Atlas
    {
        get
        {
            if (this.atlas == null)
            {
                dfGUIManager manager = base.GetManager();
                if (manager != null)
                {
                    return (this.atlas = manager.DefaultAtlas);
                }
            }
            return this.atlas;
        }
        set
        {
            if (!dfAtlas.Equals(value, this.atlas))
            {
                this.atlas = value;
                this.Invalidate();
            }
        }
    }

    public float FillAmount
    {
        get
        {
            return this.fillAmount;
        }
        set
        {
            if (!Mathf.Approximately(value, this.fillAmount))
            {
                this.fillAmount = Mathf.Max(0f, Mathf.Min(1f, value));
                this.Invalidate();
            }
        }
    }

    public dfFillDirection FillDirection
    {
        get
        {
            return this.fillDirection;
        }
        set
        {
            if (value != this.fillDirection)
            {
                this.fillDirection = value;
                this.Invalidate();
            }
        }
    }

    public dfSpriteFlip Flip
    {
        get
        {
            return this.flip;
        }
        set
        {
            if (value != this.flip)
            {
                this.flip = value;
                this.Invalidate();
            }
        }
    }

    public bool InvertFill
    {
        get
        {
            return this.invertFill;
        }
        set
        {
            if (value != this.invertFill)
            {
                this.invertFill = value;
                this.Invalidate();
            }
        }
    }

    public dfAtlas.ItemInfo SpriteInfo
    {
        get
        {
            if (this.Atlas == null)
            {
                return null;
            }
            return this.Atlas[this.spriteName];
        }
    }

    public string SpriteName
    {
        get
        {
            return this.spriteName;
        }
        set
        {
            value = base.getLocalizedValue(value);
            if (value != this.spriteName)
            {
                this.spriteName = value;
                if (!Application.isPlaying)
                {
                    dfAtlas.ItemInfo spriteInfo = this.SpriteInfo;
                    if ((base.size == Vector2.zero) && (spriteInfo != null))
                    {
                        base.size = spriteInfo.sizeInPixels;
                        this.updateCollider();
                    }
                }
                this.Invalidate();
                this.OnSpriteNameChanged(value);
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RenderOptions
    {
        public dfAtlas atlas;
        public dfAtlas.ItemInfo spriteInfo;
        public Color32 color;
        public float pixelsToUnits;
        public Vector2 size;
        public dfSpriteFlip flip;
        public bool invertFill;
        public dfFillDirection fillDirection;
        public float fillAmount;
        public Vector3 offset;
        public int baseIndex;
    }
}

