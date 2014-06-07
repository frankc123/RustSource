using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable, ExecuteInEditMode, RequireComponent(typeof(BoxCollider)), AddComponentMenu("Daikon Forge/User Interface/Checkbox")]
public class dfCheckbox : dfControl
{
    [SerializeField]
    protected dfControl checkIcon;
    [SerializeField]
    protected dfControl group;
    [SerializeField]
    protected bool isChecked;
    [SerializeField]
    protected dfLabel label;

    public event PropertyChangedEventHandler<bool> CheckChanged;

    protected internal void OnCheckChanged()
    {
        object[] args = new object[] { this.isChecked };
        base.SignalHierarchy("OnCheckChanged", args);
        if (this.CheckChanged != null)
        {
            this.CheckChanged(this, this.isChecked);
        }
        if (this.checkIcon != null)
        {
            if (this.IsChecked)
            {
                this.checkIcon.BringToFront();
            }
            this.checkIcon.IsVisible = this.IsChecked;
        }
    }

    protected internal override void OnClick(dfMouseEventArgs args)
    {
        if (this.group == null)
        {
            this.IsChecked = !this.IsChecked;
        }
        else
        {
            foreach (dfCheckbox checkbox in base.transform.parent.GetComponentsInChildren<dfCheckbox>())
            {
                if (((checkbox != this) && (checkbox.GroupContainer == this.GroupContainer)) && checkbox.IsChecked)
                {
                    checkbox.IsChecked = false;
                }
            }
            this.IsChecked = true;
        }
        args.Use();
        base.OnClick(args);
    }

    protected internal override void OnKeyPress(dfKeyEventArgs args)
    {
        if (args.KeyCode == KeyCode.Space)
        {
            this.OnClick(new dfMouseEventArgs(this, dfMouseButtons.Left, 1, new Ray(), Vector2.zero, 0f));
        }
        else
        {
            base.OnKeyPress(args);
        }
    }

    public override void Start()
    {
        base.Start();
        if (this.checkIcon != null)
        {
            this.checkIcon.BringToFront();
            this.checkIcon.IsVisible = this.IsChecked;
        }
    }

    public override bool CanFocus
    {
        get
        {
            return (base.IsEnabled && base.IsVisible);
        }
    }

    public dfControl CheckIcon
    {
        get
        {
            return this.checkIcon;
        }
        set
        {
            if (value != this.checkIcon)
            {
                this.checkIcon = value;
                this.Invalidate();
            }
        }
    }

    public dfControl GroupContainer
    {
        get
        {
            return this.group;
        }
        set
        {
            if (value != this.group)
            {
                this.group = value;
                this.Invalidate();
            }
        }
    }

    public bool IsChecked
    {
        get
        {
            return this.isChecked;
        }
        set
        {
            if (value != this.isChecked)
            {
                this.isChecked = value;
                this.OnCheckChanged();
            }
        }
    }

    public dfLabel Label
    {
        get
        {
            return this.label;
        }
        set
        {
            if (value != this.label)
            {
                this.label = value;
                this.Invalidate();
            }
        }
    }

    public string Text
    {
        get
        {
            if (this.label != null)
            {
                return this.label.Text;
            }
            return "[LABEL NOT SET]";
        }
        set
        {
            if (this.label != null)
            {
                this.label.Text = value;
            }
        }
    }
}

