using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

[Serializable, AddComponentMenu("Daikon Forge/User Interface/Dynamic Font"), ExecuteInEditMode]
public class dfDynamicFont : dfFontBase
{
    [SerializeField]
    private Font baseFont;
    [SerializeField]
    private int baseFontSize = -1;
    [SerializeField]
    private int baseline = -1;
    private static CharacterInfo[] glyphBuffer = new CharacterInfo[0x400];
    private bool invalidatingDependentControls;
    [SerializeField]
    private int lineHeight;
    private static List<dfDynamicFont> loadedFonts = new List<dfDynamicFont>();
    [SerializeField]
    private UnityEngine.Material material;
    private bool wasFontAtlasRebuilt;

    private void ensureGlyphBufferCapacity(int size)
    {
        int length = glyphBuffer.Length;
        if (size >= length)
        {
            while (length < size)
            {
                length += 0x400;
            }
            glyphBuffer = new CharacterInfo[length];
        }
    }

    public static dfDynamicFont FindByName(string name)
    {
        for (int i = 0; i < loadedFonts.Count; i++)
        {
            if (string.Equals(loadedFonts[i].name, name, StringComparison.InvariantCultureIgnoreCase))
            {
                return loadedFonts[i];
            }
        }
        GameObject obj2 = Resources.Load(name) as GameObject;
        if (obj2 == null)
        {
            return null;
        }
        dfDynamicFont component = obj2.GetComponent<dfDynamicFont>();
        if (component == null)
        {
            return null;
        }
        loadedFonts.Add(component);
        return component;
    }

    private void getGlyphData(CharacterInfo[] result, string text, int size, FontStyle style)
    {
        if (text.Length > glyphBuffer.Length)
        {
            glyphBuffer = new CharacterInfo[text.Length + 0x200];
        }
        for (int i = 0; i < text.Length; i++)
        {
            if (!this.baseFont.GetCharacterInfo(text[i], out result[i], size, style))
            {
                result[i] = new CharacterInfo { index = -1, size = size, style = style, width = size * 0.25f };
            }
        }
    }

    public Vector2 MeasureText(string text, int size, FontStyle style)
    {
        CharacterInfo[] infoArray = this.RequestCharacters(text, size, style);
        float num = ((float) size) / ((float) this.FontSize);
        int num2 = Mathf.CeilToInt(this.Baseline * num);
        Vector2 vector = new Vector2(0f, (float) num2);
        for (int i = 0; i < text.Length; i++)
        {
            CharacterInfo info = infoArray[i];
            float num4 = Mathf.Ceil(info.vert.x + info.vert.width);
            if (text[i] == ' ')
            {
                num4 = Mathf.Ceil(info.width * 1.25f);
            }
            else if (text[i] == '\t')
            {
                num4 += size * 4;
            }
            vector.x += num4;
        }
        return vector;
    }

    public override dfFontRendererBase ObtainRenderer()
    {
        return DynamicFontRenderer.Obtain(this);
    }

    private void onFontAtlasRebuilt()
    {
        this.wasFontAtlasRebuilt = true;
        this.OnFontChanged();
    }

    private void OnFontChanged()
    {
        try
        {
            <OnFontChanged>c__AnonStorey5D storeyd = new <OnFontChanged>c__AnonStorey5D {
                <>f__this = this
            };
            if (!this.invalidatingDependentControls)
            {
                storeyd.callback = null;
                storeyd.callback = new dfGUIManager.RenderCallback(storeyd.<>m__30);
                dfGUIManager.AfterRender += storeyd.callback;
            }
        }
        finally
        {
        }
    }

    public CharacterInfo[] RequestCharacters(string text, int size, FontStyle style)
    {
        if (this.baseFont == null)
        {
            throw new NullReferenceException("Base Font not assigned: " + base.name);
        }
        this.ensureGlyphBufferCapacity(size);
        if (!loadedFonts.Contains(this))
        {
            this.baseFont.textureRebuildCallback = (Font.FontTextureRebuildCallback) Delegate.Combine(this.baseFont.textureRebuildCallback, new Font.FontTextureRebuildCallback(this.onFontAtlasRebuilt));
            loadedFonts.Add(this);
        }
        this.baseFont.RequestCharactersInTexture(text, size, style);
        this.getGlyphData(glyphBuffer, text, size, style);
        return glyphBuffer;
    }

    public Font BaseFont
    {
        get
        {
            return this.baseFont;
        }
        set
        {
            if (value != this.baseFont)
            {
                this.baseFont = value;
                dfGUIManager.RefreshAll(false);
            }
        }
    }

    public int Baseline
    {
        get
        {
            return this.baseline;
        }
        set
        {
            if (value != this.baseline)
            {
                this.baseline = value;
                dfGUIManager.RefreshAll(false);
            }
        }
    }

    public int Descent
    {
        get
        {
            return (this.LineHeight - this.baseline);
        }
    }

    public override int FontSize
    {
        get
        {
            return this.baseFontSize;
        }
        set
        {
            if (value != this.baseFontSize)
            {
                this.baseFontSize = value;
                dfGUIManager.RefreshAll(false);
            }
        }
    }

    public override bool IsValid
    {
        get
        {
            return (((this.baseFont != null) && (this.Material != null)) && (this.Texture != null));
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
            if (value != this.lineHeight)
            {
                this.lineHeight = value;
                dfGUIManager.RefreshAll(false);
            }
        }
    }

    public override UnityEngine.Material Material
    {
        get
        {
            this.material.mainTexture = this.baseFont.material.mainTexture;
            return this.material;
        }
        set
        {
            if (value != this.material)
            {
                this.material = value;
                dfGUIManager.RefreshAll(false);
            }
        }
    }

    public override UnityEngine.Texture Texture
    {
        get
        {
            return this.baseFont.material.mainTexture;
        }
    }

    [CompilerGenerated]
    private sealed class <OnFontChanged>c__AnonStorey5D
    {
        private static Func<Object, bool> <>f__am$cache2;
        private static Func<dfControl, int> <>f__am$cache3;
        internal dfDynamicFont <>f__this;
        internal dfGUIManager.RenderCallback callback;

        internal void <>m__30(dfGUIManager manager)
        {
            dfGUIManager.AfterRender -= this.callback;
            this.<>f__this.invalidatingDependentControls = true;
            try
            {
                if (this.<>f__this.wasFontAtlasRebuilt)
                {
                }
                if (<>f__am$cache2 == null)
                {
                    <>f__am$cache2 = x => x is IDFMultiRender;
                }
                if (<>f__am$cache3 == null)
                {
                    <>f__am$cache3 = x => x.RenderOrder;
                }
                List<dfControl> list = Enumerable.OrderBy<dfControl, int>(Enumerable.Where<Object>(Object.FindObjectsOfType(typeof(dfControl)), <>f__am$cache2).Cast<dfControl>(), <>f__am$cache3).ToList<dfControl>();
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].Invalidate();
                }
                if (this.<>f__this.wasFontAtlasRebuilt)
                {
                    manager.Render();
                }
            }
            finally
            {
                this.<>f__this.wasFontAtlasRebuilt = false;
                this.<>f__this.invalidatingDependentControls = false;
            }
        }

        private static bool <>m__31(Object x)
        {
            return (x is IDFMultiRender);
        }

        private static int <>m__32(dfControl x)
        {
            return x.RenderOrder;
        }
    }

    public class DynamicFontRenderer : dfFontRendererBase
    {
        private dfList<dfDynamicFont.LineRenderInfo> lines;
        private static Queue<dfDynamicFont.DynamicFontRenderer> objectPool = new Queue<dfDynamicFont.DynamicFontRenderer>();
        private static Vector2[] OUTLINE_OFFSETS = new Vector2[] { new Vector2(-1f, -1f), new Vector2(-1f, 1f), new Vector2(1f, -1f), new Vector2(1f, 1f) };
        private static Stack<Color32> textColors = new Stack<Color32>();
        private List<dfMarkupToken> tokens;
        private static int[] TRIANGLE_INDICES = new int[] { 0, 1, 3, 3, 1, 2 };

        internal DynamicFontRenderer()
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

        private static void addUVCoords(dfList<Vector2> uvs, CharacterInfo glyph)
        {
            Rect uv = glyph.uv;
            float x = uv.x;
            float y = uv.y + uv.height;
            float num3 = x + uv.width;
            float num4 = uv.y;
            if (glyph.flipped)
            {
                uvs.Add(new Vector2(num3, num4));
                uvs.Add(new Vector2(num3, y));
                uvs.Add(new Vector2(x, y));
                uvs.Add(new Vector2(x, num4));
            }
            else
            {
                uvs.Add(new Vector2(x, y));
                uvs.Add(new Vector2(num3, y));
                uvs.Add(new Vector2(num3, num4));
                uvs.Add(new Vector2(x, num4));
            }
        }

        private Color32 applyOpacity(Color32 color)
        {
            color.a = (byte) (base.Opacity * 255f);
            return color;
        }

        private int calculateLineAlignment(dfDynamicFont.LineRenderInfo line)
        {
            float lineWidth = line.lineWidth;
            if ((base.TextAlign == TextAlignment.Left) || (lineWidth < 1f))
            {
                return 0;
            }
            float b = 0f;
            if (base.TextAlign == TextAlignment.Right)
            {
                b = base.MaxSize.x - lineWidth;
            }
            else
            {
                b = (base.MaxSize.x - lineWidth) * 0.5f;
            }
            return Mathf.CeilToInt(Mathf.Max(0f, b));
        }

        private dfList<dfDynamicFont.LineRenderInfo> calculateLinebreaks()
        {
            try
            {
                if (this.lines == null)
                {
                    this.lines = dfList<dfDynamicFont.LineRenderInfo>.Obtain();
                    dfDynamicFont font = (dfDynamicFont) base.Font;
                    int num = 0;
                    int start = 0;
                    int end = 0;
                    int num4 = 0;
                    float num5 = font.Baseline * base.TextScale;
                    while ((end < this.tokens.Count) && ((this.lines.Count * num5) <= (base.MaxSize.y + num5)))
                    {
                        dfMarkupToken token = this.tokens[end];
                        dfMarkupTokenType tokenType = token.TokenType;
                        if (tokenType == dfMarkupTokenType.Newline)
                        {
                            this.lines.Add(dfDynamicFont.LineRenderInfo.Obtain(start, end));
                            start = num = ++end;
                            num4 = 0;
                        }
                        else
                        {
                            int num6 = Mathf.CeilToInt((float) token.Width);
                            if (((base.WordWrap && (num > start)) && ((tokenType == dfMarkupTokenType.Text) || ((tokenType == dfMarkupTokenType.StartTag) && token.Matches("sprite")))) && ((num4 + num6) >= base.MaxSize.x))
                            {
                                if (num > start)
                                {
                                    this.lines.Add(dfDynamicFont.LineRenderInfo.Obtain(start, num - 1));
                                    start = end = ++num;
                                    num4 = 0;
                                }
                                else
                                {
                                    this.lines.Add(dfDynamicFont.LineRenderInfo.Obtain(start, num - 1));
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
                        this.lines.Add(dfDynamicFont.LineRenderInfo.Obtain(start, this.tokens.Count - 1));
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

        private void calculateLineSize(dfDynamicFont.LineRenderInfo line)
        {
            dfDynamicFont font = (dfDynamicFont) base.Font;
            line.lineHeight = font.Baseline * base.TextScale;
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
                int num = 0;
                char ch = '\0';
                bool flag = (token.TokenType == dfMarkupTokenType.Whitespace) || (token.TokenType == dfMarkupTokenType.Text);
                dfDynamicFont font = (dfDynamicFont) base.Font;
                if (flag)
                {
                    int size = Mathf.CeilToInt(font.FontSize * base.TextScale);
                    CharacterInfo[] infoArray = font.RequestCharacters(token.Value, size, FontStyle.Normal);
                    for (int i = 0; i < token.Length; i++)
                    {
                        ch = token[i];
                        if (ch == '\t')
                        {
                            num += base.TabSize;
                        }
                        else
                        {
                            CharacterInfo info = infoArray[i];
                            num += (ch == ' ') ? Mathf.CeilToInt(info.width) : Mathf.CeilToInt(info.vert.x + info.vert.width);
                            if (i > 0)
                            {
                                num += Mathf.CeilToInt(base.CharacterSpacing * base.TextScale);
                            }
                        }
                    }
                    token.Height = base.Font.LineHeight;
                    token.Width = num;
                }
                else if (((token.TokenType == dfMarkupTokenType.StartTag) && token.Matches("sprite")) && ((this.SpriteAtlas != null) && (token.AttributeCount == 1)))
                {
                    Texture2D texture = this.SpriteAtlas.Texture;
                    float f = font.Baseline * base.TextScale;
                    string str = token.GetAttribute(0).Value.Value;
                    dfAtlas.ItemInfo info2 = this.SpriteAtlas[str];
                    if (info2 != null)
                    {
                        float num5 = (info2.region.width * texture.width) / (info2.region.height * texture.height);
                        num = Mathf.CeilToInt(f * num5);
                    }
                    token.Height = Mathf.CeilToInt(f);
                    token.Width = num;
                }
            }
            finally
            {
            }
        }

        private void clipBottom(dfRenderData destination, int startIndex)
        {
            if (destination != null)
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
                        float introduced15 = Mathf.Max(vector3.y, b);
                        vector3 = new Vector3(vector3.x, introduced15, vector3.z);
                        vertices[i + 2] = vector3;
                        float introduced16 = Mathf.Max(vector4.y, b);
                        vector4 = new Vector3(vector4.x, introduced16, vector4.z);
                        vertices[i + 3] = vector4;
                        uV[i + 3] = Vector2.Lerp(uV[i + 3], uV[i], t);
                        uV[i + 2] = Vector2.Lerp(uV[i + 2], uV[i + 1], t);
                        Color color = Color.Lerp(colors[i + 3], colors[i], t);
                        colors[i + 3] = color;
                        colors[i + 2] = color;
                    }
                }
            }
        }

        private void clipRight(dfRenderData destination, int startIndex)
        {
            if (destination != null)
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
        }

        public override float[] GetCharacterWidths(string text)
        {
            float totalWidth = 0f;
            return this.GetCharacterWidths(text, 0, text.Length - 1, out totalWidth);
        }

        public float[] GetCharacterWidths(string text, int startIndex, int endIndex, out float totalWidth)
        {
            totalWidth = 0f;
            dfDynamicFont font = (dfDynamicFont) base.Font;
            int size = Mathf.CeilToInt(font.FontSize * base.TextScale);
            CharacterInfo[] infoArray = font.RequestCharacters(text, size, FontStyle.Normal);
            float[] numArray = new float[text.Length];
            float num2 = 0f;
            float num3 = 0f;
            int index = startIndex;
            while (index <= endIndex)
            {
                CharacterInfo info = infoArray[index];
                if (text[index] == '\t')
                {
                    num3 += base.TabSize;
                }
                else if (text[index] == ' ')
                {
                    num3 += info.width;
                }
                else
                {
                    num3 += info.vert.x + info.vert.width;
                }
                numArray[index] = (num3 - num2) * base.PixelRatio;
                index++;
                num2 = num3;
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
            dfList<dfDynamicFont.LineRenderInfo> list = this.calculateLinebreaks();
            float b = 0f;
            float y = 0f;
            for (int i = 0; i < list.Count; i++)
            {
                b = Mathf.Max(list[i].lineWidth, b);
                y += list[i].lineHeight;
            }
            return new Vector2(b, y);
        }

        private Color multiplyColors(Color lhs, Color rhs)
        {
            return new Color(lhs.r * rhs.r, lhs.g * rhs.g, lhs.b * rhs.b, lhs.a * rhs.a);
        }

        public static dfFontRendererBase Obtain(dfDynamicFont font)
        {
            dfDynamicFont.DynamicFontRenderer renderer = (objectPool.Count <= 0) ? new dfDynamicFont.DynamicFontRenderer() : objectPool.Dequeue();
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
            dfDynamicFont.LineRenderInfo.ResetPool();
            base.BottomColor = null;
            objectPool.Enqueue(this);
        }

        public override void Render(string text, dfRenderData destination)
        {
            textColors.Clear();
            textColors.Push(Color.white);
            this.tokenize(text);
            dfList<dfDynamicFont.LineRenderInfo> list = this.calculateLinebreaks();
            int b = 0;
            int num2 = 0;
            Vector3 position = ((Vector3) (base.VectorOffset / base.PixelRatio)).CeilToInt();
            for (int i = 0; i < list.Count; i++)
            {
                dfDynamicFont.LineRenderInfo info = list[i];
                int count = destination.Vertices.Count;
                int startIndex = (this.SpriteBuffer == null) ? 0 : this.SpriteBuffer.Vertices.Count;
                this.renderLine(list[i], textColors, position, destination);
                position.y -= info.lineHeight;
                b = Mathf.Max((int) info.lineWidth, b);
                num2 += Mathf.CeilToInt(info.lineHeight);
                if (info.lineWidth > base.MaxSize.x)
                {
                    this.clipRight(destination, count);
                    this.clipRight(this.SpriteBuffer, startIndex);
                }
                this.clipBottom(destination, count);
                this.clipBottom(this.SpriteBuffer, startIndex);
            }
            base.RenderedSize = (Vector2) (new Vector2(Mathf.Min(base.MaxSize.x, (float) b), Mathf.Min(base.MaxSize.y, (float) num2)) * base.TextScale);
        }

        private void renderLine(dfDynamicFont.LineRenderInfo line, Stack<Color32> colors, Vector3 position, dfRenderData destination)
        {
            position.x += this.calculateLineAlignment(line);
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
                        if ((token.Matches("sprite") && (this.SpriteAtlas != null)) && (this.SpriteBuffer != null))
                        {
                            this.renderSprite(token, colors.Peek(), position, this.SpriteBuffer);
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
                position.x += token.Width;
            }
        }

        private void renderSprite(dfMarkupToken token, Color32 color, Vector3 position, dfRenderData destination)
        {
            try
            {
                string str = token.GetAttribute(0).Value.Value;
                dfAtlas.ItemInfo info = this.SpriteAtlas[str];
                if (info != null)
                {
                    dfSprite.RenderOptions options = new dfSprite.RenderOptions {
                        atlas = this.SpriteAtlas,
                        color = color,
                        fillAmount = 1f,
                        offset = position,
                        pixelsToUnits = base.PixelRatio,
                        size = new Vector2((float) token.Width, (float) token.Height),
                        spriteInfo = info
                    };
                    dfSprite.renderSprite(this.SpriteBuffer, options);
                }
            }
            finally
            {
            }
        }

        private void renderText(dfMarkupToken token, Color32 color, Vector3 position, dfRenderData renderData)
        {
            try
            {
                dfDynamicFont font = (dfDynamicFont) base.Font;
                int size = Mathf.CeilToInt(font.FontSize * base.TextScale);
                FontStyle normal = FontStyle.Normal;
                int descent = font.Descent;
                dfList<Vector3> vertices = renderData.Vertices;
                dfList<int> triangles = renderData.Triangles;
                dfList<Vector2> uV = renderData.UV;
                dfList<Color32> colors = renderData.Colors;
                string text = token.Value;
                float x = position.x;
                float y = position.y;
                CharacterInfo[] infoArray = font.RequestCharacters(text, size, normal);
                renderData.Material = font.Material;
                Color32 item = this.applyOpacity(this.multiplyColors((Color) color, (Color) base.DefaultColor));
                Color32 color3 = item;
                if (base.BottomColor.HasValue)
                {
                    color3 = this.applyOpacity(this.multiplyColors((Color) color, base.BottomColor.Value));
                }
                for (int i = 0; i < text.Length; i++)
                {
                    if (i > 0)
                    {
                        x += base.CharacterSpacing * base.TextScale;
                    }
                    CharacterInfo glyph = infoArray[i];
                    float num6 = ((font.FontSize + glyph.vert.y) - size) + descent;
                    float num7 = x + glyph.vert.x;
                    float num8 = y + num6;
                    float num9 = num7 + glyph.vert.width;
                    float num10 = num8 + glyph.vert.height;
                    Vector3 vector = (Vector3) (new Vector3(num7, num8) * base.PixelRatio);
                    Vector3 vector2 = (Vector3) (new Vector3(num9, num8) * base.PixelRatio);
                    Vector3 vector3 = (Vector3) (new Vector3(num9, num10) * base.PixelRatio);
                    Vector3 vector4 = (Vector3) (new Vector3(num7, num10) * base.PixelRatio);
                    if (base.Shadow)
                    {
                        addTriangleIndices(vertices, triangles);
                        Vector3 vector5 = (Vector3) (base.ShadowOffset * base.PixelRatio);
                        vertices.Add(vector + vector5);
                        vertices.Add(vector2 + vector5);
                        vertices.Add(vector3 + vector5);
                        vertices.Add(vector4 + vector5);
                        Color32 color4 = this.applyOpacity(base.ShadowColor);
                        colors.Add(color4);
                        colors.Add(color4);
                        colors.Add(color4);
                        colors.Add(color4);
                        addUVCoords(uV, glyph);
                    }
                    if (base.Outline)
                    {
                        for (int j = 0; j < OUTLINE_OFFSETS.Length; j++)
                        {
                            addTriangleIndices(vertices, triangles);
                            Vector3 vector6 = (Vector3) ((OUTLINE_OFFSETS[j] * base.OutlineSize) * base.PixelRatio);
                            vertices.Add(vector + vector6);
                            vertices.Add(vector2 + vector6);
                            vertices.Add(vector3 + vector6);
                            vertices.Add(vector4 + vector6);
                            Color32 color5 = this.applyOpacity(base.OutlineColor);
                            colors.Add(color5);
                            colors.Add(color5);
                            colors.Add(color5);
                            colors.Add(color5);
                            addUVCoords(uV, glyph);
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
                    addUVCoords(uV, glyph);
                    x += Mathf.CeilToInt(glyph.vert.x + glyph.vert.width);
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

        public dfAtlas SpriteAtlas { get; set; }

        public dfRenderData SpriteBuffer { get; set; }
    }

    private class LineRenderInfo
    {
        public int endOffset;
        public float lineHeight;
        public float lineWidth;
        private static dfList<dfDynamicFont.LineRenderInfo> pool = new dfList<dfDynamicFont.LineRenderInfo>();
        private static int poolIndex = 0;
        public int startOffset;

        private LineRenderInfo()
        {
        }

        public static dfDynamicFont.LineRenderInfo Obtain(int start, int end)
        {
            if (poolIndex >= (pool.Count - 1))
            {
                pool.Add(new dfDynamicFont.LineRenderInfo());
            }
            dfDynamicFont.LineRenderInfo info = pool[poolIndex++];
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

