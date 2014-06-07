using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(dfPanel)), ExecuteInEditMode, AddComponentMenu("Daikon Forge/User Interface/Panel Addon/Flow Layout")]
public class dfPanelFlowLayout : MonoBehaviour
{
    [SerializeField]
    protected RectOffset borderPadding = new RectOffset();
    [SerializeField]
    protected dfControlOrientation flowDirection;
    [SerializeField]
    protected bool hideClippedControls;
    [SerializeField]
    protected Vector2 itemSpacing = new Vector2();
    private dfPanel panel;

    private bool canShowControlUnclipped(dfControl control)
    {
        if (this.hideClippedControls)
        {
            Vector3 relativePosition = control.RelativePosition;
            if ((relativePosition.x + control.Width) >= (this.panel.Width - this.borderPadding.right))
            {
                return false;
            }
            if ((relativePosition.y + control.Height) >= (this.panel.Height - this.borderPadding.bottom))
            {
                return false;
            }
        }
        return true;
    }

    private void child_SizeChanged(dfControl control, Vector2 value)
    {
        this.performLayout();
    }

    private void child_ZOrderChanged(dfControl control, int value)
    {
        this.performLayout();
    }

    public void OnControlAdded(dfControl container, dfControl child)
    {
        child.ZOrderChanged += new PropertyChangedEventHandler<int>(this.child_ZOrderChanged);
        child.SizeChanged += new PropertyChangedEventHandler<Vector2>(this.child_SizeChanged);
        this.performLayout();
    }

    public void OnControlRemoved(dfControl container, dfControl child)
    {
        child.ZOrderChanged -= new PropertyChangedEventHandler<int>(this.child_ZOrderChanged);
        child.SizeChanged -= new PropertyChangedEventHandler<Vector2>(this.child_SizeChanged);
        this.performLayout();
    }

    public void OnEnable()
    {
        this.panel = base.GetComponent<dfPanel>();
        this.panel.SizeChanged += new PropertyChangedEventHandler<Vector2>(this.OnSizeChanged);
    }

    public void OnSizeChanged(dfControl control, Vector2 value)
    {
        this.performLayout();
    }

    private void performLayout()
    {
        if (this.panel == null)
        {
            this.panel = base.GetComponent<dfPanel>();
        }
        Vector3 vector = new Vector3((float) this.borderPadding.left, (float) this.borderPadding.top);
        bool flag = true;
        float num = this.panel.Width - this.borderPadding.right;
        float num2 = this.panel.Height - this.borderPadding.bottom;
        int b = 0;
        IList<dfControl> controls = this.panel.Controls;
        int num4 = 0;
        while (num4 < controls.Count)
        {
            if (!flag)
            {
                if (this.flowDirection == dfControlOrientation.Horizontal)
                {
                    vector.x += this.itemSpacing.x;
                }
                else
                {
                    vector.y += this.itemSpacing.y;
                }
            }
            dfControl control = controls[num4];
            if (this.flowDirection == dfControlOrientation.Horizontal)
            {
                if (!flag && ((vector.x + control.Width) >= num))
                {
                    vector.x = this.borderPadding.left;
                    vector.y += b;
                    b = 0;
                    flag = true;
                }
            }
            else if (!flag && ((vector.y + control.Height) >= num2))
            {
                vector.y = this.borderPadding.top;
                vector.x += b;
                b = 0;
                flag = true;
            }
            control.RelativePosition = vector;
            if (this.flowDirection == dfControlOrientation.Horizontal)
            {
                vector.x += control.Width;
                b = Mathf.Max(Mathf.CeilToInt(control.Height + this.itemSpacing.y), b);
            }
            else
            {
                vector.y += control.Height;
                b = Mathf.Max(Mathf.CeilToInt(control.Width + this.itemSpacing.x), b);
            }
            control.IsVisible = this.canShowControlUnclipped(control);
            num4++;
            flag = false;
        }
    }

    public RectOffset BorderPadding
    {
        get
        {
            if (this.borderPadding == null)
            {
                this.borderPadding = new RectOffset();
            }
            return this.borderPadding;
        }
        set
        {
            value = value.ConstrainPadding();
            if (!object.Equals(value, this.borderPadding))
            {
                this.borderPadding = value;
                this.performLayout();
            }
        }
    }

    public dfControlOrientation Direction
    {
        get
        {
            return this.flowDirection;
        }
        set
        {
            if (value != this.flowDirection)
            {
                this.flowDirection = value;
                this.performLayout();
            }
        }
    }

    public bool HideClippedControls
    {
        get
        {
            return this.hideClippedControls;
        }
        set
        {
            if (value != this.hideClippedControls)
            {
                this.hideClippedControls = value;
                this.performLayout();
            }
        }
    }

    public Vector2 ItemSpacing
    {
        get
        {
            return this.itemSpacing;
        }
        set
        {
            value = Vector2.Max(value, Vector2.zero);
            if (!object.Equals(value, this.itemSpacing))
            {
                this.itemSpacing = value;
                this.performLayout();
            }
        }
    }
}

