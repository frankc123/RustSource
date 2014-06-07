using System;
using UnityEngine;

[Serializable, ExecuteInEditMode, AddComponentMenu("Daikon Forge/User Interface/Resize Handle")]
public class dfResizeHandle : dfControl
{
    [SerializeField]
    protected dfAtlas atlas;
    [SerializeField]
    protected string backgroundSprite = string.Empty;
    [SerializeField]
    protected ResizeEdge edges = (ResizeEdge.Bottom | ResizeEdge.Right);
    private Vector2 maxEdgePos;
    private Vector2 minEdgePos;
    private Vector3 mouseAnchorPos;
    private Vector3 startPosition;
    private Vector2 startSize;

    protected internal override void OnMouseDown(dfMouseEventArgs args)
    {
        args.Use();
        Plane plane = new Plane(base.parent.transform.TransformDirection(Vector3.back), base.parent.transform.position);
        Ray ray = args.Ray;
        float enter = 0f;
        plane.Raycast(args.Ray, out enter);
        this.mouseAnchorPos = ray.origin + ((Vector3) (ray.direction * enter));
        this.startSize = base.parent.Size;
        this.startPosition = base.parent.RelativePosition;
        this.minEdgePos = this.startPosition;
        this.maxEdgePos = this.startPosition + this.startSize;
        Vector2 vector = base.parent.CalculateMinimumSize();
        Vector2 maximumSize = base.parent.MaximumSize;
        if (maximumSize.magnitude <= float.Epsilon)
        {
            maximumSize = (Vector2) (Vector2.one * 2048f);
        }
        if ((this.Edges & ResizeEdge.Left) == ResizeEdge.Left)
        {
            this.minEdgePos.x = this.maxEdgePos.x - maximumSize.x;
            this.maxEdgePos.x -= vector.x;
        }
        else if ((this.Edges & ResizeEdge.Right) == ResizeEdge.Right)
        {
            this.minEdgePos.x = this.startPosition.x + vector.x;
            this.maxEdgePos.x = this.startPosition.x + maximumSize.x;
        }
        if ((this.Edges & ResizeEdge.Top) == ResizeEdge.Top)
        {
            this.minEdgePos.y = this.maxEdgePos.y - maximumSize.y;
            this.maxEdgePos.y -= vector.y;
        }
        else if ((this.Edges & ResizeEdge.Bottom) == ResizeEdge.Bottom)
        {
            this.minEdgePos.y = this.startPosition.y + vector.y;
            this.maxEdgePos.y = this.startPosition.y + maximumSize.y;
        }
    }

    protected internal override void OnMouseMove(dfMouseEventArgs args)
    {
        if (args.Buttons.IsSet(dfMouseButtons.Left) && (this.Edges != ResizeEdge.None))
        {
            args.Use();
            Ray ray = args.Ray;
            float enter = 0f;
            Vector3 inNormal = base.GetCamera().transform.TransformDirection(Vector3.back);
            new Plane(inNormal, this.mouseAnchorPos).Raycast(ray, out enter);
            float num2 = base.PixelsToUnits();
            Vector3 vector2 = ray.origin + ((Vector3) (ray.direction * enter));
            Vector3 vector3 = (Vector3) ((vector2 - this.mouseAnchorPos) / num2);
            vector3.y *= -1f;
            float x = this.startPosition.x;
            float y = this.startPosition.y;
            float num5 = x + this.startSize.x;
            float num6 = y + this.startSize.y;
            if ((this.Edges & ResizeEdge.Left) == ResizeEdge.Left)
            {
                x = Mathf.Min(this.maxEdgePos.x, Mathf.Max(this.minEdgePos.x, x + vector3.x));
            }
            else if ((this.Edges & ResizeEdge.Right) == ResizeEdge.Right)
            {
                num5 = Mathf.Min(this.maxEdgePos.x, Mathf.Max(this.minEdgePos.x, num5 + vector3.x));
            }
            if ((this.Edges & ResizeEdge.Top) == ResizeEdge.Top)
            {
                y = Mathf.Min(this.maxEdgePos.y, Mathf.Max(this.minEdgePos.y, y + vector3.y));
            }
            else if ((this.Edges & ResizeEdge.Bottom) == ResizeEdge.Bottom)
            {
                num6 = Mathf.Min(this.maxEdgePos.y, Mathf.Max(this.minEdgePos.y, num6 + vector3.y));
            }
            base.parent.Size = new Vector2(num5 - x, num6 - y);
            base.parent.RelativePosition = new Vector3(x, y, 0f);
            if (base.parent.GetManager().PixelPerfectMode)
            {
                base.parent.MakePixelPerfect(true);
            }
        }
    }

    protected internal override void OnMouseUp(dfMouseEventArgs args)
    {
        base.Parent.MakePixelPerfect(true);
        args.Use();
        base.OnMouseUp(args);
    }

    protected override void OnRebuildRenderData()
    {
        if ((this.Atlas != null) && !string.IsNullOrEmpty(this.backgroundSprite))
        {
            dfAtlas.ItemInfo info = this.Atlas[this.backgroundSprite];
            if (info != null)
            {
                base.renderData.Material = this.Atlas.Material;
                Color32 color = base.ApplyOpacity(!base.IsEnabled ? base.disabledColor : base.color);
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

    public override void Start()
    {
        base.Start();
        if (base.Size.magnitude <= float.Epsilon)
        {
            base.Size = new Vector2(25f, 25f);
            if (base.Parent != null)
            {
                base.RelativePosition = (Vector3) (base.Parent.Size - base.Size);
                base.Anchor = dfAnchorStyle.Right | dfAnchorStyle.Bottom;
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

    public string BackgroundSprite
    {
        get
        {
            return this.backgroundSprite;
        }
        set
        {
            if (value != this.backgroundSprite)
            {
                this.backgroundSprite = value;
                this.Invalidate();
            }
        }
    }

    public ResizeEdge Edges
    {
        get
        {
            return this.edges;
        }
        set
        {
            this.edges = value;
        }
    }

    [Flags]
    public enum ResizeEdge
    {
        Bottom = 8,
        Left = 1,
        None = 0,
        Right = 2,
        Top = 4
    }
}

