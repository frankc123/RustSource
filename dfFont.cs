using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using UnityEngine;

[Serializable, AddComponentMenu("Daikon Forge/User Interface/Font Definition")]
public class dfFont : dfFontBase
{
    [SerializeField]
    protected int aa;
    [SerializeField]
    protected dfAtlas atlas;
    [SerializeField]
    protected bool bold;
    [SerializeField]
    protected string charset;
    [SerializeField]
    protected string face = string.Empty;
    private Dictionary<int, GlyphDefinition> glyphMap;
    [SerializeField]
    private List<GlyphDefinition> glyphs = new List<GlyphDefinition>();
    [SerializeField]
    protected bool italic;
    [SerializeField]
    protected List<GlyphKerning> kerning = new List<GlyphKerning>();
    private Dictionary<int, GlyphKerningList> kerningMap;
    [SerializeField]
    protected int lineHeight;
    [SerializeField]
    protected int outline;
    [SerializeField]
    protected int[] padding;
    [SerializeField]
    protected int size;
    [SerializeField]
    protected bool smooth;
    [SerializeField]
    protected int[] spacing;
    [SerializeField]
    protected string sprite;
    [SerializeField]
    protected int stretchH;

    public void AddKerning(int first, int second, int amount)
    {
        GlyphKerning item = new GlyphKerning {
            first = first,
            second = second,
            amount = amount
        };
        this.kerning.Add(item);
    }

    private void buildKerningMap()
    {
        Dictionary<int, GlyphKerningList> dictionary = this.kerningMap = new Dictionary<int, GlyphKerningList>();
        for (int i = 0; i < this.kerning.Count; i++)
        {
            GlyphKerning kerning = this.kerning[i];
            if (!dictionary.ContainsKey(kerning.first))
            {
                dictionary[kerning.first] = new GlyphKerningList();
            }
            dictionary[kerning.first].Add(kerning);
        }
    }

    public GlyphDefinition GetGlyph(char id)
    {
        if (this.glyphMap == null)
        {
            this.glyphMap = new Dictionary<int, GlyphDefinition>();
            for (int i = 0; i < this.glyphs.Count; i++)
            {
                GlyphDefinition definition = this.glyphs[i];
                this.glyphMap[definition.id] = definition;
            }
        }
        GlyphDefinition definition2 = null;
        this.glyphMap.TryGetValue(id, out definition2);
        return definition2;
    }

    public int GetKerning(char previousChar, char currentChar)
    {
        try
        {
            if (this.kerningMap == null)
            {
                this.buildKerningMap();
            }
            GlyphKerningList list = null;
            if (!this.kerningMap.TryGetValue(previousChar, out list))
            {
                return 0;
            }
            return list.GetKerning(previousChar, currentChar);
        }
        finally
        {
        }
    }

    public override dfFontRendererBase ObtainRenderer()
    {
        return BitmappedFontRenderer.Obtain(this);
    }

    public void OnEnable()
    {
        this.glyphMap = null;
    }

    public dfAtlas Atlas
    {
        get
        {
            return this.atlas;
        }
        set
        {
            if (value != this.atlas)
            {
                this.atlas = value;
                this.glyphMap = null;
            }
        }
    }

    public bool Bold
    {
        get
        {
            return this.bold;
        }
    }

    public int Count
    {
        get
        {
            return this.glyphs.Count;
        }
    }

    public string FontFace
    {
        get
        {
            return this.face;
        }
    }

    public override int FontSize
    {
        get
        {
            return this.size;
        }
        set
        {
            throw new InvalidOperationException();
        }
    }

    public List<GlyphDefinition> Glyphs
    {
        get
        {
            return this.glyphs;
        }
    }

    public override bool IsValid
    {
        get
        {
            return ((this.Atlas != null) && (this.Atlas[this.Sprite] != null));
        }
    }

    public bool Italic
    {
        get
        {
            return this.italic;
        }
    }

    public List<GlyphKerning> KerningInfo
    {
        get
        {
            return this.kerning;
        }
    }

    public override int LineHeight
    {
        get
        {
            return this.lineHeight;
        }
        set
        {
            throw new InvalidOperationException();
        }
    }

    public override UnityEngine.Material Material
    {
        get
        {
            return this.Atlas.Material;
        }
        set
        {
            throw new InvalidOperationException();
        }
    }

    public int Outline
    {
        get
        {
            return this.outline;
        }
    }

    public int[] Padding
    {
        get
        {
            return this.padding;
        }
    }

    public int[] Spacing
    {
        get
        {
            return this.spacing;
        }
    }

    public string Sprite
    {
        get
        {
            return this.sprite;
        }
        set
        {
            if (value != this.sprite)
            {
                this.sprite = value;
                this.glyphMap = null;
            }
        }
    }

    public override UnityEngine.Texture Texture
    {
        get
        {
            return this.Atlas.Texture;
        }
    }

    public class BitmappedFontRenderer : dfFontRendererBase
    {
        private dfList<dfFont.LineRenderInfo> lines;
        private static Queue<dfFont.BitmappedFontRenderer> objectPool = new Queue<dfFont.BitmappedFontRenderer>();
        private static Vector2[] OUTLINE_OFFSETS = new Vector2[] { new Vector2(-1f, -1f), new Vector2(-1f, 1f), new Vector2(1f, -1f), new Vector2(1f, 1f) };
        private static Stack<Color32> textColors = new Stack<Color32>();
        private List<dfMarkupToken> tokens;
        private static int[] TRIANGLE_INDICES = new int[] { 0, 1, 3, 3, 1, 2 };

        internal BitmappedFontRenderer()
        {
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

        private Color32 applyOpacity(Color32 color)
        {
            color.a = (byte) (base.Opacity * 255f);
            return color;
        }

        private int calculateLineAlignment(dfFont.LineRenderInfo line)
        {
            float lineWidth = line.lineWidth;
            if ((base.TextAlign == TextAlignment.Left) || (lineWidth == 0f))
            {
                return 0;
            }
            int b = 0;
            if (base.TextAlign == TextAlignment.Right)
            {
                b = Mathf.FloorToInt((base.MaxSize.x / base.TextScale) - lineWidth);
            }
            else
            {
                b = Mathf.FloorToInt(((base.MaxSize.x / base.TextScale) - lineWidth) * 0.5f);
            }
            return Mathf.Max(0, b);
        }

        private dfList<dfFont.LineRenderInfo> calculateLinebreaks()
        {
            try
            {
                if (this.lines == null)
                {
                    this.lines = dfList<dfFont.LineRenderInfo>.Obtain();
                    int num = 0;
                    int start = 0;
                    int end = 0;
                    int num4 = 0;
                    float num5 = base.Font.LineHeight * base.TextScale;
                    while ((end < this.tokens.Count) && ((this.lines.Count * num5) < base.MaxSize.y))
                    {
                        dfMarkupToken token = this.tokens[end];
                        dfMarkupTokenType tokenType = token.TokenType;
                        if (tokenType == dfMarkupTokenType.Newline)
                        {
                            this.lines.Add(dfFont.LineRenderInfo.Obtain(start, end));
                            start = num = ++end;
                            num4 = 0;
                        }
                        else
                        {
                            int num6 = Mathf.CeilToInt(token.Width * base.TextScale);
                            if (((base.WordWrap && (num > start)) && ((tokenType == dfMarkupTokenType.Text) || ((tokenType == dfMarkupTokenType.StartTag) && token.Matches("sprite")))) && ((num4 + num6) >= base.MaxSize.x))
                            {
                                if (num > start)
                                {
                                    this.lines.Add(dfFont.LineRenderInfo.Obtain(start, num - 1));
                                    start = end = ++num;
                                    num4 = 0;
                                }
                                else
                                {
                                    this.lines.Add(dfFont.LineRenderInfo.Obtain(start, num - 1));
                                    start = num = ++end;
                                    num4 = 0;
                                }
                                continue;
                            }
                            if (tokenType == dfMarkupTokenType.Whitespace)
                            {
                                num = end;
                            }
                            num4 += num6;
                            end++;
                        }
                    }
                    if (start < this.tokens.Count)
                    {
                        this.lines.Add(dfFont.LineRenderInfo.Obtain(start, this.tokens.Count - 1));
                    }
                    for (int i = 0; i < this.lines.Count; i++)
                    {
                        this.calculateLineSize(this.lines[i]);
                    }
                }
                return this.lines;
            }
            finally
            {
            }
        }

        private void calculateLineSize(dfFont.LineRenderInfo line)
        {
            line.lineHeight = base.Font.LineHeight;
            int num = 0;
            for (int i = line.startOffset; i <= line.endOffset; i++)
            {
                num += this.tokens[i].Width;
            }
            line.lineWidth = num;
        }

        private void calculateTokenRenderSize(dfMarkupToken token)
        {
            try
            {
                dfFont font = (dfFont) base.Font;
                int num = 0;
                char previousChar = '\0';
                char id = '\0';
                if ((token.TokenType == dfMarkupTokenType.Whitespace) || (token.TokenType == dfMarkupTokenType.Text))
                {
                    int num2 = 0;
                    while (num2 < token.Length)
                    {
                        id = token[num2];
                        if (id == '\t')
                        {
                            num += base.TabSize;
                        }
                        else
                        {
                            dfFont.GlyphDefinition glyph = font.GetGlyph(id);
                            if (glyph != null)
                            {
                                if (num2 > 0)
                                {
                                    num += font.GetKerning(previousChar, id);
                                    num += base.CharacterSpacing;
                                }
                                num += glyph.xadvance;
                            }
                        }
                        num2++;
                        previousChar = id;
                    }
                }
                else if ((token.TokenType == dfMarkupTokenType.StartTag) && token.Matches("sprite"))
                {
                    if (token.AttributeCount < 1)
                    {
                        throw new Exception("Missing sprite name in markup");
                    }
                    Texture texture = font.Texture;
                    int lineHeight = font.LineHeight;
                    string str = token.GetAttribute(0).Value.Value;
                    dfAtlas.ItemInfo info = font.atlas[str];
                    if (info != null)
                    {
                        float num4 = (info.region.width * texture.width) / (info.region.height * texture.height);
                        num = Mathf.CeilToInt(lineHeight * num4);
                    }
                }
                token.Height = base.Font.LineHeight;
                token.Width = num;
            }
            finally
            {
            }
        }

        private void clipBottom(dfRenderData destination, int startIndex)
        {
            float b = base.VectorOffset.y - (base.MaxSize.y * base.PixelRatio);
            dfList<Vector3> vertices = destination.Vertices;
            dfList<Vector2> uV = destination.UV;
            dfList<Color32> colors = destination.Colors;
            for (int i = startIndex; i < vertices.Count; i += 4)
            {
                Vector3 vector = vertices[i];
                Vector3 vector2 = vertices[i + 1];
                Vector3 vector3 = vertices[i + 2];
                Vector3 vector4 = vertices[i + 3];
                float num3 = vector.y - vector4.y;
                if (vector4.y <= b)
                {
                    float t = 1f - (Mathf.Abs((float) (-b + vector.y)) / num3);
                    vector = new Vector3(vector.x, Mathf.Max(vector.y, b), vector2.z);
                    vertices[i] = vector;
                    float y = Mathf.Max(vector2.y, b);
                    vector2 = new Vector3(vector2.x, y, vector2.z);
                    vertices[i + 1] = vector2;
                    float introduced20 = Mathf.Max(vector3.y, b);
                    vector3 = new Vector3(vector3.x, introduced20, vector3.z);
                    vertices[i + 2] = vector3;
                    float introduced21 = Mathf.Max(vector4.y, b);
                    vector4 = new Vector3(vector4.x, introduced21, vector4.z);
                    vertices[i + 3] = vector4;
                    Vector2 vector7 = uV[i + 3];
                    Vector2 vector8 = uV[i];
                    float num5 = Mathf.Lerp(vector7.y, vector8.y, t);
                    Vector2 vector9 = uV[i + 3];
                    uV[i + 3] = new Vector2(vector9.x, num5);
                    Vector2 vector10 = uV[i + 2];
                    uV[i + 2] = new Vector2(vector10.x, num5);
                    Color color = Color.Lerp(colors[i + 3], colors[i], t);
                    colors[i + 3] = color;
                    colors[i + 2] = color;
                }
            }
        }

        private void clipRight(dfRenderData destination, int startIndex)
        {
            float b = base.VectorOffset.x + (base.MaxSize.x * base.PixelRatio);
            dfList<Vector3> vertices = destination.Vertices;
            dfList<Vector2> uV = destination.UV;
            for (int i = startIndex; i < vertices.Count; i += 4)
            {
                Vector3 vector = vertices[i];
                Vector3 vector2 = vertices[i + 1];
                Vector3 vector3 = vertices[i + 2];
                Vector3 vector4 = vertices[i + 3];
                float num3 = vector2.x - vector.x;
                if (vector2.x > b)
                {
                    float t = 1f - (((b - vector2.x) + num3) / num3);
                    float x = Mathf.Min(vector.x, b);
                    vector = new Vector3(x, vector.y, vector.z);
                    vertices[i] = vector;
                    float introduced18 = Mathf.Min(vector2.x, b);
                    vector2 = new Vector3(introduced18, vector2.y, vector2.z);
                    vertices[i + 1] = vector2;
                    float introduced19 = Mathf.Min(vector3.x, b);
                    vector3 = new Vector3(introduced19, vector3.y, vector3.z);
                    vertices[i + 2] = vector3;
                    float introduced20 = Mathf.Min(vector4.x, b);
                    vector4 = new Vector3(introduced20, vector4.y, vector4.z);
                    vertices[i + 3] = vector4;
                    Vector2 vector7 = uV[i + 1];
                    Vector2 vector8 = uV[i];
                    float num5 = Mathf.Lerp(vector7.x, vector8.x, t);
                    Vector2 vector9 = uV[i + 1];
                    uV[i + 1] = new Vector2(num5, vector9.y);
                    Vector2 vector10 = uV[i + 2];
                    uV[i + 2] = new Vector2(num5, vector10.y);
                    num3 = vector2.x - vector.x;
                }
            }
        }

        private dfFont.LineRenderInfo fitSingleLine()
        {
            return dfFont.LineRenderInfo.Obtain(0, 0);
        }

        public override float[] GetCharacterWidths(string text)
        {
            float totalWidth = 0f;
            return this.GetCharacterWidths(text, 0, text.Length - 1, out totalWidth);
        }

        public float[] GetCharacterWidths(string text, int startIndex, int endIndex, out float totalWidth)
        {
            totalWidth = 0f;
            dfFont font = (dfFont) base.Font;
            float[] numArray = new float[text.Length];
            float num = base.TextScale * base.PixelRatio;
            float num2 = base.CharacterSpacing * num;
            for (int i = startIndex; i <= endIndex; i++)
            {
                dfFont.GlyphDefinition glyph = font.GetGlyph(text[i]);
                if (glyph != null)
                {
                    if (i > 0)
                    {
                        numArray[i - 1] += num2;
                        totalWidth += num2;
                    }
                    float num4 = glyph.xadvance * num;
                    numArray[i] = num4;
                    totalWidth += num4;
                }
            }
            return numArray;
        }

        private float getTabStop(float position)
        {
            float num = base.PixelRatio * base.TextScale;
            if ((base.TabStops != null) && (base.TabStops.Count > 0))
            {
                for (int i = 0; i < base.TabStops.Count; i++)
                {
                    if ((((float) base.TabStops[i]) * num) > position)
                    {
                        return (((float) base.TabStops[i]) * num);
                    }
                }
            }
            if (base.TabSize > 0)
            {
                return (position + (base.TabSize * num));
            }
            return (position + ((base.Font.FontSize * 4) * num));
        }

        public override Vector2 MeasureString(string text)
        {
            this.tokenize(text);
            dfList<dfFont.LineRenderInfo> list = this.calculateLinebreaks();
            int b = 0;
            int num2 = 0;
            for (int i = 0; i < list.Count; i++)
            {
                b = Mathf.Max((int) list[i].lineWidth, b);
                num2 += (int) list[i].lineHeight;
            }
            return (Vector2) (new Vector2((float) b, (float) num2) * base.TextScale);
        }

        private Color multiplyColors(Color lhs, Color rhs)
        {
            return new Color(lhs.r * rhs.r, lhs.g * rhs.g, lhs.b * rhs.b, lhs.a * rhs.a);
        }

        public static dfFontRendererBase Obtain(dfFont font)
        {
            dfFont.BitmappedFontRenderer renderer = (objectPool.Count <= 0) ? new dfFont.BitmappedFontRenderer() : objectPool.Dequeue();
            renderer.Reset();
            renderer.Font = font;
            return renderer;
        }

        private Color32 parseColor(dfMarkupToken token)
        {
            Color white = Color.white;
            if (token.AttributeCount == 1)
            {
                string color = token.GetAttribute(0).Value.Value;
                if ((color.Length == 7) && (color[0] == '#'))
                {
                    uint result = 0;
                    uint.TryParse(color.Substring(1), NumberStyles.HexNumber, null, out result);
                    white = (Color) this.UIntToColor(result | 0xff000000);
                }
                else
                {
                    white = dfMarkupStyle.ParseColor(color, (Color) base.DefaultColor);
                }
            }
            return this.applyOpacity(white);
        }

        public override void Release()
        {
            this.Reset();
            this.tokens = null;
            if (this.lines != null)
            {
                this.lines.Release();
                this.lines = null;
            }
            dfFont.LineRenderInfo.ResetPool();
            base.BottomColor = null;
            objectPool.Enqueue(this);
        }

        public override void Render(string text, dfRenderData destination)
        {
            textColors.Clear();
            textColors.Push(Color.white);
            this.tokenize(text);
            dfList<dfFont.LineRenderInfo> list = this.calculateLinebreaks();
            int b = 0;
            int num2 = 0;
            Vector3 vectorOffset = base.VectorOffset;
            float num3 = base.TextScale * base.PixelRatio;
            for (int i = 0; i < list.Count; i++)
            {
                dfFont.LineRenderInfo info = list[i];
                int count = destination.Vertices.Count;
                this.renderLine(list[i], textColors, vectorOffset, destination);
                vectorOffset.y -= base.Font.LineHeight * num3;
                b = Mathf.Max((int) info.lineWidth, b);
                num2 += (int) info.lineHeight;
                if ((info.lineWidth * base.TextScale) > base.MaxSize.x)
                {
                    this.clipRight(destination, count);
                }
                if ((num2 * base.TextScale) > base.MaxSize.y)
                {
                    this.clipBottom(destination, count);
                }
            }
            base.RenderedSize = (Vector2) (new Vector2(Mathf.Min(base.MaxSize.x, (float) b), Mathf.Min(base.MaxSize.y, (float) num2)) * base.TextScale);
        }

        private void renderLine(dfFont.LineRenderInfo line, Stack<Color32> colors, Vector3 position, dfRenderData destination)
        {
            float num = base.TextScale * base.PixelRatio;
            position.x += this.calculateLineAlignment(line) * num;
            for (int i = line.startOffset; i <= line.endOffset; i++)
            {
                dfMarkupToken token = this.tokens[i];
                dfMarkupTokenType tokenType = token.TokenType;
                switch (tokenType)
                {
                    case dfMarkupTokenType.Text:
                        this.renderText(token, colors.Peek(), position, destination);
                        break;

                    case dfMarkupTokenType.StartTag:
                        if (token.Matches("sprite"))
                        {
                            this.renderSprite(token, colors.Peek(), position, destination);
                        }
                        else if (token.Matches("color"))
                        {
                            colors.Push(this.parseColor(token));
                        }
                        break;

                    default:
                        if (((tokenType == dfMarkupTokenType.EndTag) && token.Matches("color")) && (colors.Count > 1))
                        {
                            colors.Pop();
                        }
                        break;
                }
                position.x += token.Width * num;
            }
        }

        private void renderSprite(dfMarkupToken token, Color32 color, Vector3 position, dfRenderData destination)
        {
            try
            {
                dfList<Vector3> vertices = destination.Vertices;
                dfList<int> triangles = destination.Triangles;
                dfList<Color32> colors = destination.Colors;
                dfList<Vector2> uV = destination.UV;
                dfFont font = (dfFont) base.Font;
                string str = token.GetAttribute(0).Value.Value;
                dfAtlas.ItemInfo info = font.Atlas[str];
                if (info != null)
                {
                    float num = (token.Height * base.TextScale) * base.PixelRatio;
                    float num2 = (token.Width * base.TextScale) * base.PixelRatio;
                    float x = position.x;
                    float y = position.y;
                    int count = vertices.Count;
                    vertices.Add(new Vector3(x, y));
                    vertices.Add(new Vector3(x + num2, y));
                    vertices.Add(new Vector3(x + num2, y - num));
                    vertices.Add(new Vector3(x, y - num));
                    triangles.Add(count);
                    triangles.Add(count + 1);
                    triangles.Add(count + 3);
                    triangles.Add(count + 3);
                    triangles.Add(count + 1);
                    triangles.Add(count + 2);
                    Color32 item = !base.ColorizeSymbols ? this.applyOpacity(base.DefaultColor) : this.applyOpacity(color);
                    colors.Add(item);
                    colors.Add(item);
                    colors.Add(item);
                    colors.Add(item);
                    Rect region = info.region;
                    uV.Add(new Vector2(region.x, region.yMax));
                    uV.Add(new Vector2(region.xMax, region.yMax));
                    uV.Add(new Vector2(region.xMax, region.y));
                    uV.Add(new Vector2(region.x, region.y));
                }
            }
            finally
            {
            }
        }

        private void renderText(dfMarkupToken token, Color32 color, Vector3 position, dfRenderData destination)
        {
            try
            {
                dfList<Vector3> vertices = destination.Vertices;
                dfList<int> triangles = destination.Triangles;
                dfList<Color32> colors = destination.Colors;
                dfList<Vector2> uV = destination.UV;
                dfFont font = (dfFont) base.Font;
                dfAtlas.ItemInfo info = font.Atlas[font.sprite];
                Texture texture = font.Texture;
                float num = 1f / ((float) texture.width);
                float num2 = 1f / ((float) texture.height);
                float num3 = num * 0.125f;
                float num4 = num2 * 0.125f;
                float num5 = base.TextScale * base.PixelRatio;
                char previousChar = '\0';
                char id = '\0';
                Color32 item = this.applyOpacity(this.multiplyColors((Color) color, (Color) base.DefaultColor));
                Color32 color3 = item;
                if (base.BottomColor.HasValue)
                {
                    color3 = this.applyOpacity(this.multiplyColors((Color) color, base.BottomColor.Value));
                }
                int num6 = 0;
                while (num6 < token.Length)
                {
                    id = token[num6];
                    if (id != '\0')
                    {
                        dfFont.GlyphDefinition glyph = font.GetGlyph(id);
                        if (glyph != null)
                        {
                            int kerning = font.GetKerning(previousChar, id);
                            float x = position.x + ((glyph.xoffset + kerning) * num5);
                            float y = position.y - (glyph.yoffset * num5);
                            float num10 = glyph.width * num5;
                            float num11 = glyph.height * num5;
                            float num12 = x + num10;
                            float num13 = y - num11;
                            Vector3 vector = new Vector3(x, y);
                            Vector3 vector2 = new Vector3(num12, y);
                            Vector3 vector3 = new Vector3(num12, num13);
                            Vector3 vector4 = new Vector3(x, num13);
                            float num14 = (info.region.x + (glyph.x * num)) - num3;
                            float num15 = (info.region.yMax - (glyph.y * num2)) - num4;
                            float num16 = (num14 + (glyph.width * num)) - num3;
                            float num17 = (num15 - (glyph.height * num2)) + num4;
                            if (base.Shadow)
                            {
                                addTriangleIndices(vertices, triangles);
                                Vector3 vector5 = (Vector3) (base.ShadowOffset * num5);
                                vertices.Add(vector + vector5);
                                vertices.Add(vector2 + vector5);
                                vertices.Add(vector3 + vector5);
                                vertices.Add(vector4 + vector5);
                                Color32 color4 = this.applyOpacity(base.ShadowColor);
                                colors.Add(color4);
                                colors.Add(color4);
                                colors.Add(color4);
                                colors.Add(color4);
                                uV.Add(new Vector2(num14, num15));
                                uV.Add(new Vector2(num16, num15));
                                uV.Add(new Vector2(num16, num17));
                                uV.Add(new Vector2(num14, num17));
                            }
                            if (base.Outline)
                            {
                                for (int i = 0; i < OUTLINE_OFFSETS.Length; i++)
                                {
                                    addTriangleIndices(vertices, triangles);
                                    Vector3 vector6 = (Vector3) ((OUTLINE_OFFSETS[i] * base.OutlineSize) * num5);
                                    vertices.Add(vector + vector6);
                                    vertices.Add(vector2 + vector6);
                                    vertices.Add(vector3 + vector6);
                                    vertices.Add(vector4 + vector6);
                                    Color32 color5 = this.applyOpacity(base.OutlineColor);
                                    colors.Add(color5);
                                    colors.Add(color5);
                                    colors.Add(color5);
                                    colors.Add(color5);
                                    uV.Add(new Vector2(num14, num15));
                                    uV.Add(new Vector2(num16, num15));
                                    uV.Add(new Vector2(num16, num17));
                                    uV.Add(new Vector2(num14, num17));
                                }
                            }
                            addTriangleIndices(vertices, triangles);
                            vertices.Add(vector);
                            vertices.Add(vector2);
                            vertices.Add(vector3);
                            vertices.Add(vector4);
                            colors.Add(item);
                            colors.Add(item);
                            colors.Add(color3);
                            colors.Add(color3);
                            uV.Add(new Vector2(num14, num15));
                            uV.Add(new Vector2(num16, num15));
                            uV.Add(new Vector2(num16, num17));
                            uV.Add(new Vector2(num14, num17));
                            position.x += ((glyph.xadvance + kerning) + base.CharacterSpacing) * num5;
                        }
                    }
                    num6++;
                    previousChar = id;
                }
            }
            finally
            {
            }
        }

        private List<dfMarkupToken> tokenize(string text)
        {
            try
            {
                if (((this.tokens == null) || (this.tokens.Count <= 0)) || (this.tokens[0].Source != text))
                {
                    if (base.ProcessMarkup)
                    {
                        this.tokens = dfMarkupTokenizer.Tokenize(text);
                    }
                    else
                    {
                        this.tokens = dfPlainTextTokenizer.Tokenize(text);
                    }
                    for (int i = 0; i < this.tokens.Count; i++)
                    {
                        this.calculateTokenRenderSize(this.tokens[i]);
                    }
                }
                return this.tokens;
            }
            finally
            {
            }
        }

        private Color32 UIntToColor(uint color)
        {
            byte a = (byte) (color >> 0x18);
            byte r = (byte) (color >> 0x10);
            byte g = (byte) (color >> 8);
            return new Color32(r, g, (byte) color, a);
        }

        public int LineCount
        {
            get
            {
                return this.lines.Count;
            }
        }
    }

    [Serializable]
    public class GlyphDefinition : IComparable<dfFont.GlyphDefinition>
    {
        [SerializeField]
        public int height;
        [SerializeField]
        public int id;
        [SerializeField]
        public bool rotated;
        [SerializeField]
        public int width;
        [SerializeField]
        public int x;
        [SerializeField]
        public int xadvance;
        [SerializeField]
        public int xoffset;
        [SerializeField]
        public int y;
        [SerializeField]
        public int yoffset;

        public int CompareTo(dfFont.GlyphDefinition other)
        {
            return this.id.CompareTo(other.id);
        }
    }

    [Serializable]
    public class GlyphKerning : IComparable<dfFont.GlyphKerning>
    {
        public int amount;
        public int first;
        public int second;

        public int CompareTo(dfFont.GlyphKerning other)
        {
            if (this.first == other.first)
            {
                return this.second.CompareTo(other.second);
            }
            return this.first.CompareTo(other.first);
        }
    }

    private class GlyphKerningList
    {
        private Dictionary<int, int> list = new Dictionary<int, int>();

        public void Add(dfFont.GlyphKerning kerning)
        {
            this.list[kerning.second] = kerning.amount;
        }

        public int GetKerning(int firstCharacter, int secondCharacter)
        {
            int num = 0;
            this.list.TryGetValue(secondCharacter, out num);
            return num;
        }
    }

    private class LineRenderInfo
    {
        public int endOffset;
        public float lineHeight;
        public float lineWidth;
        private static dfList<dfFont.LineRenderInfo> pool = new dfList<dfFont.LineRenderInfo>();
        private static int poolIndex = 0;
        public int startOffset;

        private LineRenderInfo()
        {
        }

        public static dfFont.LineRenderInfo Obtain(int start, int end)
        {
            if (poolIndex >= (pool.Count - 1))
            {
                pool.Add(new dfFont.LineRenderInfo());
            }
            dfFont.LineRenderInfo info = pool[poolIndex++];
            info.startOffset = start;
            info.endOffset = end;
            info.lineHeight = 0f;
            return info;
        }

        public static void ResetPool()
        {
            poolIndex = 0;
        }

        public int length
        {
            get
            {
                return ((this.endOffset - this.startOffset) + 1);
            }
        }
    }
}

