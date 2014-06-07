using System;
using UnityEngine;

[Serializable, AddComponentMenu("Daikon Forge/User Interface/Containers/Panel"), ExecuteInEditMode]
public class dfPanel : dfControl
{
    [SerializeField]
    protected dfAtlas atlas;
    [SerializeField]
    protected Color32 backgroundColor = Color.white;
    [SerializeField]
    protected string backgroundSprite;
    [SerializeField]
    protected RectOffset padding = new RectOffset();

    public void CenterChildControls()
    {
        if (base.controls.Count != 0)
        {
            Vector2 lhs = (Vector2) (Vector2.one * 3.402823E+38f);
            Vector2 vector2 = (Vector2) (Vector2.one * -3.402823E+38f);
            for (int i = 0; i < base.controls.Count; i++)
            {
                dfControl control = base.controls[i];
                Vector2 relativePosition = control.RelativePosition;
                Vector2 rhs = relativePosition + control.Size;
                lhs = Vector2.Min(lhs, relativePosition);
                vector2 = Vector2.Max(vector2, rhs);
            }
            Vector2 vector5 = vector2 - lhs;
            Vector2 vector6 = (Vector2) ((base.Size - vector5) * 0.5f);
            for (int j = 0; j < base.controls.Count; j++)
            {
                dfControl control2 = base.controls[j];
                control2.RelativePosition = (control2.RelativePosition - lhs) + vector6;
            }
        }
    }

    public void FitToContents()
    {
        if (base.controls.Count != 0)
        {
            Vector2 zero = Vector2.zero;
            for (int i = 0; i < base.controls.Count; i++)
            {
                dfControl control = base.controls[i];
                Vector2 rhs = control.RelativePosition + control.Size;
                zero = Vector2.Max(zero, rhs);
            }
            base.Size = zero + new Vector2((float) this.padding.right, (float) this.padding.bottom);
        }
    }

    protected internal override Plane[] GetClippingPlanes()
    {
        if (!base.ClipChildren)
        {
            return null;
        }
        Vector3[] corners = base.GetCorners();
        Vector3 inNormal = base.transform.TransformDirection(Vector3.right);
        Vector3 vector2 = base.transform.TransformDirection(Vector3.left);
        Vector3 vector3 = base.transform.TransformDirection(Vector3.up);
        Vector3 vector4 = base.transform.TransformDirection(Vector3.down);
        float num = base.PixelsToUnits();
        RectOffset padding = this.Padding;
        corners[0] += (Vector3) (((inNormal * padding.left) * num) + ((vector4 * padding.top) * num));
        corners[1] += (Vector3) (((vector2 * padding.right) * num) + ((vector4 * padding.top) * num));
        corners[2] += (Vector3) (((inNormal * padding.left) * num) + ((vector3 * padding.bottom) * num));
        return new Plane[] { new Plane(inNormal, corners[0]), new Plane(vector2, corners[1]), new Plane(vector3, corners[2]), new Plane(vector4, corners[0]) };
    }

    public override void OnEnable()
    {
        base.OnEnable();
        if (base.size == Vector2.zero)
        {
            this.SuspendLayout();
            Camera camera = base.GetCamera();
            base.Size = new Vector3(camera.pixelWidth / 2f, camera.pixelHeight / 2f);
            this.ResumeLayout();
        }
    }

    protected internal override void OnLocalize()
    {
        base.OnLocalize();
        this.BackgroundSprite = base.getLocalizedValue(this.backgroundSprite);
    }

    protected override void OnRebuildRenderData()
    {
        if ((this.Atlas != null) && !string.IsNullOrEmpty(this.backgroundSprite))
        {
            dfAtlas.ItemInfo info = this.Atlas[this.backgroundSprite];
            if (info != null)
            {
                base.renderData.Material = this.Atlas.Material;
                Color32 color = base.ApplyOpacity(this.BackgroundColor);
                dfSprite.RenderOptions options = new dfSprite.RenderOptions {
                    atlas = this.atlas,
                    color = color,
                    fillAmount = 1f,
                    offset = base.pivot.TransformToUpperLeft(base.Size),
                    pixelsToUnits = base.PixelsToUnits(),
                    size = base.Size,
                    spriteInfo = info
                };
                if ((info.border.horizontal == 0) && (info.border.vertical == 0))
                {
                    dfSprite.renderSprite(base.renderData, options);
                }
                else
                {
                    dfSlicedSprite.renderSprite(base.renderData, options);
                }
            }
        }
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

    public Color32 BackgroundColor
    {
        get
        {
            return this.backgroundColor;
        }
        set
        {
            if (!object.Equals(value, this.backgroundColor))
            {
                this.backgroundColor = value;
                this.Invalidate();
            }
        }
    }

    public string BackgroundSprite
    {
        get
        {
            return this.backgroundSprite;
        }
        set
        {
            value = base.getLocalizedValue(value);
            if (value != this.backgroundSprite)
            {
                this.backgroundSprite = value;
                this.Invalidate();
            }
        }
    }

    public RectOffset Padding
    {
        get
        {
            if (this.padding == null)
            {
                this.padding = new RectOffset();
            }
            return this.padding;
        }
        set
        {
            value = value.ConstrainPadding();
            if (!object.Equals(value, this.padding))
            {
                this.padding = value;
                this.Invalidate();
            }
        }
    }
}

