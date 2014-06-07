using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable, AddComponentMenu("Daikon Forge/User Interface/Sprite/Texture"), ExecuteInEditMode]
public class dfTextureSprite : dfControl
{
    private bool createdRuntimeMaterial;
    [SerializeField]
    protected float fillAmount = 1f;
    [SerializeField]
    protected dfFillDirection fillDirection;
    [SerializeField]
    protected dfSpriteFlip flip;
    [SerializeField]
    protected bool invertFill;
    [SerializeField]
    protected UnityEngine.Material material;
    private UnityEngine.Material renderMaterial;
    [SerializeField]
    protected Texture2D texture;
    private static int[] TRIANGLE_INDICES = new int[] { 0, 1, 3, 3, 1, 2 };

    public event PropertyChangedEventHandler<Texture2D> TextureChanged;

    private void disposeCreatedMaterial()
    {
        if (this.createdRuntimeMaterial)
        {
            Object.DestroyImmediate(this.material);
            this.material = null;
            this.createdRuntimeMaterial = false;
        }
    }

    private void doFill(dfRenderData renderData)
    {
        dfList<Vector3> vertices = renderData.Vertices;
        dfList<Vector2> uV = renderData.UV;
        int num = 0;
        int num2 = 1;
        int num3 = 3;
        int num4 = 2;
        if (this.invertFill)
        {
            if (this.fillDirection == dfFillDirection.Horizontal)
            {
                num = 1;
                num2 = 0;
                num3 = 2;
                num4 = 3;
            }
            else
            {
                num = 3;
                num2 = 2;
                num3 = 0;
                num4 = 1;
            }
        }
        if (this.fillDirection == dfFillDirection.Horizontal)
        {
            vertices[num2] = Vector3.Lerp(vertices[num2], vertices[num], 1f - this.fillAmount);
            vertices[num4] = Vector3.Lerp(vertices[num4], vertices[num3], 1f - this.fillAmount);
            uV[num2] = Vector2.Lerp(uV[num2], uV[num], 1f - this.fillAmount);
            uV[num4] = Vector2.Lerp(uV[num4], uV[num3], 1f - this.fillAmount);
        }
        else
        {
            vertices[num3] = Vector3.Lerp(vertices[num3], vertices[num], 1f - this.fillAmount);
            vertices[num4] = Vector3.Lerp(vertices[num4], vertices[num2], 1f - this.fillAmount);
            uV[num3] = Vector2.Lerp(uV[num3], uV[num], 1f - this.fillAmount);
            uV[num4] = Vector2.Lerp(uV[num4], uV[num2], 1f - this.fillAmount);
        }
    }

    private void ensureMaterial()
    {
        if ((this.material == null) && (this.texture != null))
        {
            Shader shader = Shader.Find("Daikon Forge/Default UI Shader");
            if (shader == null)
            {
                Debug.LogError("Failed to find default shader");
            }
            else
            {
                UnityEngine.Material material = new UnityEngine.Material(shader) {
                    name = "Default Texture Shader",
                    hideFlags = HideFlags.DontSave,
                    mainTexture = this.texture
                };
                this.material = material;
                this.createdRuntimeMaterial = true;
            }
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (this.renderMaterial != null)
        {
            Object.DestroyImmediate(this.renderMaterial);
            this.renderMaterial = null;
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();
        this.disposeCreatedMaterial();
        if (Application.isPlaying && (this.renderMaterial != null))
        {
            Object.DestroyImmediate(this.renderMaterial);
            this.renderMaterial = null;
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();
        this.renderMaterial = null;
    }

    protected override void OnRebuildRenderData()
    {
        base.OnRebuildRenderData();
        if (this.texture != null)
        {
            this.ensureMaterial();
            if (this.material != null)
            {
                if (this.renderMaterial == null)
                {
                    UnityEngine.Material material = new UnityEngine.Material(this.material) {
                        hideFlags = HideFlags.DontSave,
                        name = this.material.name + " (copy)"
                    };
                    this.renderMaterial = material;
                }
                this.renderMaterial.mainTexture = this.texture;
                base.renderData.Material = this.renderMaterial;
                float num = base.PixelsToUnits();
                float x = 0f;
                float y = 0f;
                float num4 = this.size.x * num;
                float num5 = -this.size.y * num;
                Vector3 vector = (Vector3) (base.pivot.TransformToUpperLeft(base.size).RoundToInt() * num);
                base.renderData.Vertices.Add(new Vector3(x, y, 0f) + vector);
                base.renderData.Vertices.Add(new Vector3(num4, y, 0f) + vector);
                base.renderData.Vertices.Add(new Vector3(num4, num5, 0f) + vector);
                base.renderData.Vertices.Add(new Vector3(x, num5, 0f) + vector);
                base.renderData.Triangles.AddRange(TRIANGLE_INDICES);
                this.rebuildUV(base.renderData);
                Color32 item = base.ApplyOpacity(base.color);
                base.renderData.Colors.Add(item);
                base.renderData.Colors.Add(item);
                base.renderData.Colors.Add(item);
                base.renderData.Colors.Add(item);
                if (this.fillAmount < 1f)
                {
                    this.doFill(base.renderData);
                }
            }
        }
    }

    protected internal virtual void OnTextureChanged(Texture2D value)
    {
        object[] args = new object[] { value };
        base.SignalHierarchy("OnTextureChanged", args);
        if (this.TextureChanged != null)
        {
            this.TextureChanged(this, value);
        }
    }

    private void rebuildUV(dfRenderData renderData)
    {
        dfList<Vector2> uV = renderData.UV;
        uV.Add(new Vector2(0f, 1f));
        uV.Add(new Vector2(1f, 1f));
        uV.Add(new Vector2(1f, 0f));
        uV.Add(new Vector2(0f, 0f));
        Vector2 zero = Vector2.zero;
        if (this.flip.IsSet(dfSpriteFlip.FlipHorizontal))
        {
            zero = uV[1];
            uV[1] = uV[0];
            uV[0] = zero;
            zero = uV[3];
            uV[3] = uV[2];
            uV[2] = zero;
        }
        if (this.flip.IsSet(dfSpriteFlip.FlipVertical))
        {
            zero = uV[0];
            uV[0] = uV[3];
            uV[3] = zero;
            zero = uV[1];
            uV[1] = uV[2];
            uV[2] = zero;
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

    public UnityEngine.Material Material
    {
        get
        {
            return this.material;
        }
        set
        {
            if (value != this.material)
            {
                this.disposeCreatedMaterial();
                this.renderMaterial = null;
                this.material = value;
                this.Invalidate();
            }
        }
    }

    public UnityEngine.Material RenderMaterial
    {
        get
        {
            return this.renderMaterial;
        }
    }

    public Texture2D Texture
    {
        get
        {
            return this.texture;
        }
        set
        {
            if (value != this.texture)
            {
                this.texture = value;
                this.Invalidate();
                if ((value != null) && (this.size.sqrMagnitude <= float.Epsilon))
                {
                    base.size = new Vector2((float) value.width, (float) value.height);
                }
                this.OnTextureChanged(value);
            }
        }
    }
}

