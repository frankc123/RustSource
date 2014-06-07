using System;
using UnityEngine;

[Serializable, ExecuteInEditMode, AddComponentMenu("Daikon Forge/User Interface/Drag Handle")]
public class dfDragHandle : dfControl
{
    private Vector3 lastPosition;

    protected internal override void OnMouseDown(dfMouseEventArgs args)
    {
        base.GetRootContainer().BringToFront();
        args.Use();
        Plane plane = new Plane(base.parent.transform.TransformDirection(Vector3.back), base.parent.transform.position);
        Ray ray = args.Ray;
        float enter = 0f;
        plane.Raycast(args.Ray, out enter);
        this.lastPosition = ray.origin + ((Vector3) (ray.direction * enter));
        base.OnMouseDown(args);
    }

    protected internal override void OnMouseMove(dfMouseEventArgs args)
    {
        args.Use();
        if (args.Buttons.IsSet(dfMouseButtons.Left))
        {
            Ray ray = args.Ray;
            float enter = 0f;
            Vector3 inNormal = base.GetCamera().transform.TransformDirection(Vector3.back);
            new Plane(inNormal, this.lastPosition).Raycast(ray, out enter);
            Vector3 vector2 = (ray.origin + ((Vector3) (ray.direction * enter))).Quantize(base.parent.PixelsToUnits());
            Vector3 vector3 = vector2 - this.lastPosition;
            Vector3 vector4 = (base.parent.transform.position + vector3).Quantize(base.parent.PixelsToUnits());
            base.parent.transform.position = vector4;
            this.lastPosition = vector2;
        }
        base.OnMouseMove(args);
    }

    protected internal override void OnMouseUp(dfMouseEventArgs args)
    {
        base.OnMouseUp(args);
        base.Parent.MakePixelPerfect(true);
    }

    public override void Start()
    {
        base.Start();
        if (base.Size.magnitude <= float.Epsilon)
        {
            if (base.Parent != null)
            {
                base.Size = new Vector2(base.Parent.Width, 30f);
                base.Anchor = dfAnchorStyle.Right | dfAnchorStyle.Left | dfAnchorStyle.Top;
                base.RelativePosition = (Vector3) Vector2.zero;
            }
            else
            {
                base.Size = new Vector2(200f, 25f);
            }
        }
    }
}

