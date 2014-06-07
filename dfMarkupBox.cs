using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

public class dfMarkupBox
{
    public int Baseline;
    private List<dfMarkupBox> children;
    private dfMarkupBox currentLine;
    private int currentLinePos;
    public dfMarkupDisplayType Display;
    public bool IsNewline;
    public dfMarkupBorders Margins;
    public dfMarkupBorders Padding;
    public Vector2 Position;
    public Vector2 Size;
    public dfMarkupStyle Style;

    private dfMarkupBox()
    {
        this.Position = Vector2.zero;
        this.Size = Vector2.zero;
        this.Margins = new dfMarkupBorders(0, 0, 0, 0);
        this.Padding = new dfMarkupBorders(0, 0, 0, 0);
        this.children = new List<dfMarkupBox>();
        throw new NotImplementedException();
    }

    public dfMarkupBox(dfMarkupElement element, dfMarkupDisplayType display, dfMarkupStyle style)
    {
        this.Position = Vector2.zero;
        this.Size = Vector2.zero;
        this.Margins = new dfMarkupBorders(0, 0, 0, 0);
        this.Padding = new dfMarkupBorders(0, 0, 0, 0);
        this.children = new List<dfMarkupBox>();
        this.Element = element;
        this.Display = display;
        this.Style = style;
        this.Baseline = style.FontSize;
    }

    private void addBlock(dfMarkupBox box)
    {
        if (this.currentLine != null)
        {
            this.currentLine.IsNewline = true;
            this.endCurrentLine(true);
        }
        dfMarkupBox containingBlock = this.GetContainingBlock();
        if (box.Size.sqrMagnitude <= float.Epsilon)
        {
            box.Size = new Vector2(containingBlock.Size.x - box.Margins.horizontal, (float) this.Style.FontSize);
        }
        int num = this.getVerticalPosition(box.Margins.top);
        box.Position = new Vector2((float) box.Margins.left, (float) num);
        this.Size = new Vector2(this.Size.x, Mathf.Max(this.Size.y, box.Position.y + box.Size.y));
        box.Parent = this;
        this.children.Add(box);
    }

    public virtual void AddChild(dfMarkupBox box)
    {
        dfMarkupDisplayType display = box.Display;
        if ((((display == dfMarkupDisplayType.block) || (display == dfMarkupDisplayType.table)) || (display == dfMarkupDisplayType.listItem)) || (display == dfMarkupDisplayType.tableRow))
        {
            this.addBlock(box);
        }
        else
        {
            this.addInline(box);
        }
    }

    private void addInline(dfMarkupBox box)
    {
        dfMarkupBorders margins = box.Margins;
        bool flag = !this.Style.Preformatted && ((this.currentLine != null) && ((this.currentLinePos + box.Size.x) > this.currentLine.Size.x));
        if ((this.currentLine == null) || flag)
        {
            dfDynamicFont font;
            this.endCurrentLine(false);
            int num = this.getVerticalPosition(margins.top);
            dfMarkupBox containingBlock = this.GetContainingBlock();
            if (containingBlock == null)
            {
                Debug.LogError("Containing block not found");
                return;
            }
            if (this.Style.Font != null)
            {
                font = this.Style.Font;
            }
            else
            {
                font = this.Style.Host.Font;
            }
            float num2 = ((float) font.FontSize) / ((float) font.FontSize);
            float num3 = font.Baseline * num2;
            dfMarkupBox box3 = new dfMarkupBox(this.Element, dfMarkupDisplayType.block, this.Style) {
                Size = new Vector2(containingBlock.Size.x, (float) this.Style.LineHeight),
                Position = new Vector2(0f, (float) num),
                Parent = this,
                Baseline = (int) num3
            };
            this.currentLine = box3;
            this.children.Add(this.currentLine);
        }
        if (((this.currentLinePos == 0) && !box.Style.PreserveWhitespace) && (box is dfMarkupBoxText))
        {
            dfMarkupBoxText text = box as dfMarkupBoxText;
            if (text.IsWhitespace)
            {
                return;
            }
        }
        Vector2 vector = new Vector2((float) (this.currentLinePos + margins.left), (float) margins.top);
        box.Position = vector;
        box.Parent = this.currentLine;
        this.currentLine.children.Add(box);
        this.currentLinePos = ((int) (vector.x + box.Size.x)) + box.Margins.right;
        float x = Mathf.Max(this.currentLine.Size.x, vector.x + box.Size.x);
        float y = Mathf.Max(this.currentLine.Size.y, vector.y + box.Size.y);
        this.currentLine.Size = new Vector2(x, y);
    }

    internal void AddLineBreak()
    {
        if (this.currentLine != null)
        {
            this.currentLine.IsNewline = true;
        }
        int num = this.getVerticalPosition(0);
        this.endCurrentLine(false);
        dfMarkupBox containingBlock = this.GetContainingBlock();
        dfMarkupBox box2 = new dfMarkupBox(this.Element, dfMarkupDisplayType.block, this.Style) {
            Size = new Vector2(containingBlock.Size.x, (float) this.Style.FontSize),
            Position = new Vector2(0f, (float) num),
            Parent = this
        };
        this.currentLine = box2;
        this.children.Add(this.currentLine);
    }

    private void doHorizontalAlignment()
    {
        if ((this.Style.Align != dfMarkupTextAlign.Left) && (this.children.Count != 0))
        {
            int num = this.children.Count - 1;
            while (num > 0)
            {
                dfMarkupBoxText text = this.children[num] as dfMarkupBoxText;
                if ((text == null) || !text.IsWhitespace)
                {
                    break;
                }
                num--;
            }
            if (this.Style.Align == dfMarkupTextAlign.Center)
            {
                float num2 = 0f;
                for (int i = 0; i <= num; i++)
                {
                    num2 += this.children[i].Size.x;
                }
                float num4 = ((this.Size.x - this.Padding.horizontal) - num2) * 0.5f;
                for (int j = 0; j <= num; j++)
                {
                    Vector2 position = this.children[j].Position;
                    position.x += num4;
                    this.children[j].Position = position;
                }
            }
            else if (this.Style.Align == dfMarkupTextAlign.Right)
            {
                float num6 = this.Size.x - this.Padding.horizontal;
                for (int k = num; k >= 0; k--)
                {
                    Vector2 vector2 = this.children[k].Position;
                    vector2.x = num6 - this.children[k].Size.x;
                    this.children[k].Position = vector2;
                    num6 -= this.children[k].Size.x;
                }
            }
            else
            {
                if (this.Style.Align != dfMarkupTextAlign.Justify)
                {
                    throw new NotImplementedException("text-align: " + this.Style.Align + " is not implemented");
                }
                if ((this.children.Count > 1) && (!this.IsNewline && !this.children[this.children.Count - 1].IsNewline))
                {
                    float a = 0f;
                    for (int m = 0; m <= num; m++)
                    {
                        dfMarkupBox box = this.children[m];
                        a = Mathf.Max(a, box.Position.x + box.Size.x);
                    }
                    float num10 = ((this.Size.x - this.Padding.horizontal) - a) / ((float) this.children.Count);
                    for (int n = 1; n <= num; n++)
                    {
                        dfMarkupBox local1 = this.children[n];
                        local1.Position += new Vector2(n * num10, 0f);
                    }
                    dfMarkupBox box2 = this.children[num];
                    Vector2 vector3 = box2.Position;
                    vector3.x = (this.Size.x - this.Padding.horizontal) - box2.Size.x;
                    box2.Position = vector3;
                }
            }
        }
    }

    private void doVerticalAlignment()
    {
        if (this.children.Count != 0)
        {
            float minValue = float.MinValue;
            float maxValue = float.MaxValue;
            float a = float.MinValue;
            this.Baseline = (int) (this.Size.y * 0.95f);
            for (int i = 0; i < this.children.Count; i++)
            {
                dfMarkupBox box = this.children[i];
                a = Mathf.Max(a, box.Position.y + box.Baseline);
            }
            for (int j = 0; j < this.children.Count; j++)
            {
                dfMarkupBox box2 = this.children[j];
                dfMarkupVerticalAlign verticalAlign = box2.Style.VerticalAlign;
                Vector2 position = box2.Position;
                if (verticalAlign == dfMarkupVerticalAlign.Baseline)
                {
                    position.y = a - box2.Baseline;
                }
                box2.Position = position;
            }
            for (int k = 0; k < this.children.Count; k++)
            {
                dfMarkupBox box3 = this.children[k];
                Vector2 vector2 = box3.Position;
                Vector2 size = box3.Size;
                maxValue = Mathf.Min(maxValue, vector2.y);
                minValue = Mathf.Max(minValue, vector2.y + size.y);
            }
            for (int m = 0; m < this.children.Count; m++)
            {
                dfMarkupBox box4 = this.children[m];
                dfMarkupVerticalAlign align2 = box4.Style.VerticalAlign;
                Vector2 vector4 = box4.Position;
                Vector2 vector5 = box4.Size;
                switch (align2)
                {
                    case dfMarkupVerticalAlign.Top:
                        vector4.y = maxValue;
                        break;

                    case dfMarkupVerticalAlign.Bottom:
                        vector4.y = minValue - vector5.y;
                        break;

                    case dfMarkupVerticalAlign.Middle:
                        vector4.y = (this.Size.y - vector5.y) * 0.5f;
                        break;
                }
                box4.Position = vector4;
            }
            int num8 = 0x7fffffff;
            for (int n = 0; n < this.children.Count; n++)
            {
                num8 = Mathf.Min(num8, (int) this.children[n].Position.y);
            }
            for (int num10 = 0; num10 < this.children.Count; num10++)
            {
                Vector2 vector6 = this.children[num10].Position;
                vector6.y -= num8;
                this.children[num10].Position = vector6;
            }
        }
    }

    private void endCurrentLine(bool removeEmpty = false)
    {
        if (this.currentLine != null)
        {
            if (this.currentLinePos == 0)
            {
                if (removeEmpty)
                {
                    this.children.Remove(this.currentLine);
                }
            }
            else
            {
                this.currentLine.doHorizontalAlignment();
                this.currentLine.doVerticalAlignment();
            }
            this.currentLine = null;
            this.currentLinePos = 0;
        }
    }

    public void FitToContents(bool recursive = false)
    {
        if (this.children.Count == 0)
        {
            this.Size = new Vector2(this.Size.x, 0f);
        }
        else
        {
            this.endCurrentLine(false);
            Vector2 zero = Vector2.zero;
            for (int i = 0; i < this.children.Count; i++)
            {
                dfMarkupBox box = this.children[i];
                zero = Vector2.Max(zero, box.Position + box.Size);
            }
            this.Size = zero;
        }
    }

    private dfMarkupBox GetContainingBlock()
    {
        for (dfMarkupBox box = this; box != null; box = box.Parent)
        {
            switch (box.Display)
            {
                case dfMarkupDisplayType.block:
                case dfMarkupDisplayType.inlineBlock:
                case dfMarkupDisplayType.listItem:
                case dfMarkupDisplayType.table:
                case dfMarkupDisplayType.tableRow:
                case dfMarkupDisplayType.tableCell:
                    return box;
            }
        }
        return null;
    }

    public virtual Vector2 GetOffset()
    {
        Vector2 zero = Vector2.zero;
        for (dfMarkupBox box = this; box != null; box = box.Parent)
        {
            zero += box.Position;
        }
        return zero;
    }

    private int getVerticalPosition(int topMargin)
    {
        if (this.children.Count == 0)
        {
            return topMargin;
        }
        int num = 0;
        int num2 = 0;
        for (int i = 0; i < this.children.Count; i++)
        {
            dfMarkupBox box = this.children[i];
            float num4 = (box.Position.y + box.Size.y) + box.Margins.bottom;
            if (num4 > num)
            {
                num = (int) num4;
                num2 = i;
            }
        }
        dfMarkupBox box2 = this.children[num2];
        int num5 = Mathf.Max(box2.Margins.bottom, topMargin);
        return (((int) (box2.Position.y + box2.Size.y)) + num5);
    }

    internal dfMarkupBox HitTest(Vector2 point)
    {
        Vector2 offset = this.GetOffset();
        Vector2 vector2 = offset + this.Size;
        if (((point.x < offset.x) || (point.x > vector2.x)) || ((point.y < offset.y) || (point.y > vector2.y)))
        {
            return null;
        }
        for (int i = 0; i < this.children.Count; i++)
        {
            dfMarkupBox box = this.children[i].HitTest(point);
            if (box != null)
            {
                return box;
            }
        }
        return this;
    }

    protected virtual dfRenderData OnRebuildRenderData()
    {
        return null;
    }

    public virtual void Release()
    {
        for (int i = 0; i < this.children.Count; i++)
        {
            this.children[i].Release();
        }
        this.children.Clear();
        this.Element = null;
        this.Parent = null;
        this.Margins = new dfMarkupBorders();
    }

    internal dfRenderData Render()
    {
        try
        {
            this.endCurrentLine(false);
            return this.OnRebuildRenderData();
        }
        finally
        {
        }
    }

    protected void renderDebugBox(dfRenderData renderData)
    {
        Vector3 zero = Vector3.zero;
        Vector3 item = zero + ((Vector3) (Vector3.right * this.Size.x));
        Vector3 vector3 = item + ((Vector3) (Vector3.down * this.Size.y));
        Vector3 vector4 = zero + ((Vector3) (Vector3.down * this.Size.y));
        renderData.Vertices.Add(zero);
        renderData.Vertices.Add(item);
        renderData.Vertices.Add(vector3);
        renderData.Vertices.Add(vector4);
        renderData.Triangles.AddRange(new int[] { 0, 1, 3, 3, 1, 2 });
        renderData.UV.Add(Vector2.zero);
        renderData.UV.Add(Vector2.zero);
        renderData.UV.Add(Vector2.zero);
        renderData.UV.Add(Vector2.zero);
        Color backgroundColor = this.Style.BackgroundColor;
        renderData.Colors.Add(backgroundColor);
        renderData.Colors.Add(backgroundColor);
        renderData.Colors.Add(backgroundColor);
        renderData.Colors.Add(backgroundColor);
    }

    public List<dfMarkupBox> Children
    {
        get
        {
            return this.children;
        }
    }

    public dfMarkupElement Element { get; protected set; }

    public int Height
    {
        get
        {
            return (int) this.Size.y;
        }
        set
        {
            this.Size = new Vector2(this.Size.x, (float) value);
        }
    }

    public dfMarkupBox Parent { get; protected set; }

    public int Width
    {
        get
        {
            return (int) this.Size.x;
        }
        set
        {
            this.Size = new Vector2((float) value, this.Size.y);
        }
    }
}

