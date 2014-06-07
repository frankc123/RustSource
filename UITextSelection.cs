using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct UITextSelection
{
    private const Change kSelectChange_None = Change.None;
    private const Change kSelectChange_DropCarrat = Change.CarratToNone;
    private const Change kSelectChange_MoveCarrat = Change.CarratMove;
    private const Change kSelectChange_NewCarrat = Change.NoneToCarrat;
    private const Change kSelectChange_DropSelection = Change.SelectionToCarrat;
    private const Change kSelectChange_MoveSelection = Change.SelectionAdjusted;
    private const Change kSelectChange_NewSelection = Change.CarratToSelection;
    private const Change kSelectChange_DropAll = Change.SelectionToNone;
    private const Change kSelectChange_NewAll = Change.NoneToSelection;
    public UITextPosition carratPos;
    public UITextPosition selectPos;
    public UITextSelection(UITextPosition carratPos, UITextPosition selectPos)
    {
        this.carratPos = carratPos;
        this.selectPos = selectPos;
    }

    public bool hasSelection
    {
        get
        {
            return ((this.carratPos.valid && this.selectPos.valid) && (this.carratPos.position != this.selectPos.position));
        }
    }
    public bool showCarrat
    {
        get
        {
            return (this.carratPos.valid && (!this.selectPos.valid || (this.selectPos.position == this.carratPos.position)));
        }
    }
    public bool valid
    {
        get
        {
            return this.carratPos.valid;
        }
    }
    public bool GetHighlight(out UIHighlight h)
    {
        if (this.selectPos.position < this.carratPos.position)
        {
            if (this.carratPos.valid && this.selectPos.valid)
            {
                h.a.i = this.selectPos.position;
                h.a.L = this.selectPos.line;
                h.a.C = this.selectPos.column;
                h.b.i = this.carratPos.position;
                h.b.L = this.carratPos.line;
                h.b.C = this.carratPos.column;
                return true;
            }
        }
        else if (((this.selectPos.position > this.carratPos.position) && this.carratPos.valid) && this.selectPos.valid)
        {
            h.b.i = this.selectPos.position;
            h.b.L = this.selectPos.line;
            h.b.C = this.selectPos.column;
            h.a.i = this.carratPos.position;
            h.a.L = this.carratPos.line;
            h.a.C = this.carratPos.column;
            return true;
        }
        h = UIHighlight.invalid;
        return false;
    }

    public int highlightBegin
    {
        get
        {
            if ((this.carratPos.valid && this.selectPos.valid) && (this.selectPos.position != this.carratPos.position))
            {
                return ((this.selectPos.position >= this.carratPos.position) ? this.carratPos.position : this.selectPos.position);
            }
            return -1;
        }
    }
    public int highlightEnd
    {
        get
        {
            if ((this.carratPos.valid && this.selectPos.valid) && (this.selectPos.position != this.carratPos.position))
            {
                return ((this.selectPos.position >= this.carratPos.position) ? this.selectPos.position : this.carratPos.position);
            }
            return -1;
        }
    }
    public int carratIndex
    {
        get
        {
            if (((this.carratPos.position == this.selectPos.position) || !this.selectPos.valid) && this.carratPos.valid)
            {
                return this.carratPos.position;
            }
            return -1;
        }
    }
    public int selectIndex
    {
        get
        {
            if ((this.carratPos.valid && this.selectPos.valid) && (this.selectPos.position != this.carratPos.position))
            {
                return this.selectPos.position;
            }
            return -1;
        }
    }
    public Change GetChangesTo(ref UITextSelection value)
    {
        if (this.carratPos.valid)
        {
            if (!value.carratPos.valid)
            {
                return (!this.hasSelection ? Change.CarratToNone : Change.SelectionToNone);
            }
            if (this.hasSelection)
            {
                if (!value.hasSelection)
                {
                    return Change.SelectionToCarrat;
                }
                if ((value.carratPos.position != this.carratPos.position) || (value.selectPos.position != this.selectPos.position))
                {
                    return Change.SelectionAdjusted;
                }
                return Change.None;
            }
            if (value.hasSelection)
            {
                return Change.CarratToSelection;
            }
            if (value.carratPos.position != this.carratPos.position)
            {
                return Change.CarratMove;
            }
            return Change.None;
        }
        if (value.carratPos.valid)
        {
            return (!value.hasSelection ? Change.NoneToCarrat : Change.NoneToSelection);
        }
        return Change.None;
    }

    public override string ToString()
    {
        object[] args = new object[] { this.hasSelection, this.showCarrat, this.highlightBegin, this.highlightEnd, this.carratPos.ToString(), this.selectPos.ToString() };
        return string.Format("[hasSelection={0}, showCarrat={1}, highlight=[{2}->{3}], carratPos={4}, selectPos={5}]", args);
    }
    public enum Change : sbyte
    {
        CarratMove = 2,
        CarratToNone = 3,
        CarratToSelection = 4,
        None = 0,
        NoneToCarrat = 1,
        NoneToSelection = 7,
        SelectionAdjusted = 5,
        SelectionToCarrat = 6,
        SelectionToNone = 8
    }
}

