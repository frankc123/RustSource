using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

public class dfMarkupTag : dfMarkupElement
{
    public List<dfMarkupAttribute> Attributes;
    private static int ELEMENTID;
    private string id;
    private dfRichTextLabel owner;

    public dfMarkupTag(dfMarkupTag original)
    {
        this.TagName = original.TagName;
        this.Attributes = original.Attributes;
        this.IsEndTag = original.IsEndTag;
        this.IsClosedTag = original.IsClosedTag;
        this.IsInline = original.IsInline;
        this.id = original.id;
        List<dfMarkupElement> childNodes = original.ChildNodes;
        for (int i = 0; i < childNodes.Count; i++)
        {
            dfMarkupElement node = childNodes[i];
            base.AddChildNode(node);
        }
    }

    public dfMarkupTag(string tagName)
    {
        this.Attributes = new List<dfMarkupAttribute>();
        this.TagName = tagName;
        this.id = tagName + ELEMENTID++.ToString("X");
    }

    protected override void _PerformLayoutImpl(dfMarkupBox container, dfMarkupStyle style)
    {
        if (!this.IsEndTag)
        {
            for (int i = 0; i < base.ChildNodes.Count; i++)
            {
                base.ChildNodes[i].PerformLayout(container, style);
            }
        }
    }

    protected dfMarkupStyle applyTextStyleAttributes(dfMarkupStyle style)
    {
        string[] names = new string[] { "font", "font-family" };
        dfMarkupAttribute attribute = this.findAttribute(names);
        if (attribute != null)
        {
            style.Font = dfDynamicFont.FindByName(attribute.Value);
        }
        string[] textArray2 = new string[] { "style", "font-style" };
        dfMarkupAttribute attribute2 = this.findAttribute(textArray2);
        if (attribute2 != null)
        {
            style.FontStyle = dfMarkupStyle.ParseFontStyle(attribute2.Value, style.FontStyle);
        }
        string[] textArray3 = new string[] { "size", "font-size" };
        dfMarkupAttribute attribute3 = this.findAttribute(textArray3);
        if (attribute3 != null)
        {
            style.FontSize = dfMarkupStyle.ParseSize(attribute3.Value, style.FontSize);
        }
        string[] textArray4 = new string[] { "color" };
        dfMarkupAttribute attribute4 = this.findAttribute(textArray4);
        if (attribute4 != null)
        {
            Color color = dfMarkupStyle.ParseColor(attribute4.Value, style.Color);
            color.a = style.Opacity;
            style.Color = color;
        }
        string[] textArray5 = new string[] { "align", "text-align" };
        dfMarkupAttribute attribute5 = this.findAttribute(textArray5);
        if (attribute5 != null)
        {
            style.Align = dfMarkupStyle.ParseTextAlignment(attribute5.Value);
        }
        string[] textArray6 = new string[] { "valign", "vertical-align" };
        dfMarkupAttribute attribute6 = this.findAttribute(textArray6);
        if (attribute6 != null)
        {
            style.VerticalAlign = dfMarkupStyle.ParseVerticalAlignment(attribute6.Value);
        }
        string[] textArray7 = new string[] { "line-height" };
        dfMarkupAttribute attribute7 = this.findAttribute(textArray7);
        if (attribute7 != null)
        {
            style.LineHeight = dfMarkupStyle.ParseSize(attribute7.Value, style.LineHeight);
        }
        string[] textArray8 = new string[] { "text-decoration" };
        dfMarkupAttribute attribute8 = this.findAttribute(textArray8);
        if (attribute8 != null)
        {
            style.TextDecoration = dfMarkupStyle.ParseTextDecoration(attribute8.Value);
        }
        string[] textArray9 = new string[] { "background", "background-color" };
        dfMarkupAttribute attribute9 = this.findAttribute(textArray9);
        if (attribute9 != null)
        {
            style.BackgroundColor = dfMarkupStyle.ParseColor(attribute9.Value, Color.clear);
            style.BackgroundColor.a = style.Opacity;
        }
        return style;
    }

    protected dfMarkupAttribute findAttribute(params string[] names)
    {
        for (int i = 0; i < this.Attributes.Count; i++)
        {
            for (int j = 0; j < names.Length; j++)
            {
                if (this.Attributes[i].Name == names[j])
                {
                    return this.Attributes[i];
                }
            }
        }
        return null;
    }

    internal override void Release()
    {
        base.Release();
    }

    public override string ToString()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("[");
        if (this.IsEndTag)
        {
            builder.Append("/");
        }
        builder.Append(this.TagName);
        for (int i = 0; i < this.Attributes.Count; i++)
        {
            builder.Append(" ");
            builder.Append(this.Attributes[i].ToString());
        }
        if (this.IsClosedTag)
        {
            builder.Append("/");
        }
        builder.Append("]");
        if (!this.IsClosedTag)
        {
            for (int j = 0; j < base.ChildNodes.Count; j++)
            {
                builder.Append(base.ChildNodes[j].ToString());
            }
            builder.Append("[/");
            builder.Append(this.TagName);
            builder.Append("]");
        }
        return builder.ToString();
    }

    public string ID
    {
        get
        {
            return this.id;
        }
    }

    public virtual bool IsClosedTag { get; set; }

    public virtual bool IsEndTag { get; set; }

    public virtual bool IsInline { get; set; }

    public dfRichTextLabel Owner
    {
        get
        {
            return this.owner;
        }
        set
        {
            this.owner = value;
            for (int i = 0; i < base.ChildNodes.Count; i++)
            {
                dfMarkupTag tag = base.ChildNodes[i] as dfMarkupTag;
                if (tag != null)
                {
                    tag.Owner = value;
                }
            }
        }
    }

    public string TagName { get; set; }
}

