using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEngine;

public class dfMarkupBoxText : dfMarkupBox
{
    private bool isWhitespace;
    private static Queue<dfMarkupBoxText> objectPool = new Queue<dfMarkupBoxText>();
    private dfRenderData renderData;
    private static int[] TRIANGLE_INDICES = new int[] { 0, 1, 2, 0, 2, 3 };
    private static Regex whitespacePattern = new Regex(@"\s+");

    public dfMarkupBoxText(dfMarkupElement element, dfMarkupDisplayType display, dfMarkupStyle style) : base(element, display, style)
    {
        this.renderData = new dfRenderData(0x20);
    }

    private static void addTriangleIndices(dfList<Vector3> verts, dfList<int> triangles)
    {
        int count = verts.Count;
        int[] numArray = TRIANGLE_INDICES;
        for (int i = 0; i < numArray.Length; i++)
        {
            triangles.Add(count + numArray[i]);
        }
    }

    public static dfMarkupBoxText Obtain(dfMarkupElement element, dfMarkupDisplayType display, dfMarkupStyle style)
    {
        if (objectPool.Count > 0)
        {
            dfMarkupBoxText text = objectPool.Dequeue();
            text.Element = element;
            text.Display = display;
            text.Style = style;
            text.Position = Vector2.zero;
            text.Size = Vector2.zero;
            text.Baseline = (int) (style.FontSize * 1.1f);
            text.Margins = new dfMarkupBorders();
            text.Padding = new dfMarkupBorders();
            return text;
        }
        return new dfMarkupBoxText(element, display, style);
    }

    protected override dfRenderData OnRebuildRenderData()
    {
        this.renderData.Clear();
        if (this.Style.Font == null)
        {
            return null;
        }
        if (this.Style.TextDecoration == dfMarkupTextDecoration.Underline)
        {
            this.renderUnderline();
        }
        this.renderText(this.Text);
        return this.renderData;
    }

    public override void Release()
    {
        base.Release();
        this.Text = string.Empty;
        this.renderData.Clear();
        objectPool.Enqueue(this);
    }

    private void renderText(string text)
    {
        dfDynamicFont font = this.Style.Font;
        int fontSize = this.Style.FontSize;
        FontStyle fontStyle = this.Style.FontStyle;
        dfList<Vector3> vertices = this.renderData.Vertices;
        dfList<int> triangles = this.renderData.Triangles;
        dfList<Vector2> uV = this.renderData.UV;
        dfList<Color32> colors = this.renderData.Colors;
        float num2 = ((float) fontSize) / ((float) font.FontSize);
        float num3 = font.Descent * num2;
        float num4 = 0f;
        CharacterInfo[] infoArray = font.RequestCharacters(text, fontSize, fontStyle);
        this.renderData.Material = font.Material;
        for (int i = 0; i < text.Length; i++)
        {
            CharacterInfo info = infoArray[i];
            addTriangleIndices(vertices, triangles);
            float num6 = ((font.FontSize + info.vert.y) - fontSize) + num3;
            float x = num4 + info.vert.x;
            float y = num6;
            float num9 = x + info.vert.width;
            float num10 = y + info.vert.height;
            Vector3 item = new Vector3(x, y);
            Vector3 vector2 = new Vector3(num9, y);
            Vector3 vector3 = new Vector3(num9, num10);
            Vector3 vector4 = new Vector3(x, num10);
            vertices.Add(item);
            vertices.Add(vector2);
            vertices.Add(vector3);
            vertices.Add(vector4);
            Color color = this.Style.Color;
            colors.Add(color);
            colors.Add(color);
            colors.Add(color);
            colors.Add(color);
            Rect uv = info.uv;
            float num11 = uv.x;
            float num12 = uv.y + uv.height;
            float num13 = num11 + uv.width;
            float num14 = uv.y;
            if (info.flipped)
            {
                uV.Add(new Vector2(num13, num14));
                uV.Add(new Vector2(num13, num12));
                uV.Add(new Vector2(num11, num12));
                uV.Add(new Vector2(num11, num14));
            }
            else
            {
                uV.Add(new Vector2(num11, num12));
                uV.Add(new Vector2(num13, num12));
                uV.Add(new Vector2(num13, num14));
                uV.Add(new Vector2(num11, num14));
            }
            num4 += Mathf.CeilToInt(info.vert.x + info.vert.width);
        }
    }

    private void renderUnderline()
    {
    }

    internal void SetText(string text)
    {
        this.Text = text;
        if (this.Style.Font != null)
        {
            this.isWhitespace = whitespacePattern.IsMatch(this.Text);
            string str = (!this.Style.PreserveWhitespace && this.isWhitespace) ? " " : this.Text;
            CharacterInfo[] infoArray = this.Style.Font.RequestCharacters(str, this.Style.FontSize, this.Style.FontStyle);
            int fontSize = this.Style.FontSize;
            Vector2 vector = new Vector2(0f, (float) this.Style.LineHeight);
            for (int i = 0; i < str.Length; i++)
            {
                CharacterInfo info = infoArray[i];
                float a = info.vert.x + info.vert.width;
                if (str[i] == ' ')
                {
                    a = Mathf.Max(a, fontSize * 0.33f);
                }
                else if (str[i] == '\t')
                {
                    a += fontSize * 3;
                }
                vector.x += a;
            }
            base.Size = vector;
            dfDynamicFont font = this.Style.Font;
            float num4 = ((float) fontSize) / ((float) font.FontSize);
            base.Baseline = Mathf.CeilToInt(font.Baseline * num4);
        }
    }

    public bool IsWhitespace
    {
        get
        {
            return this.isWhitespace;
        }
    }

    public string Text { get; private set; }
}

