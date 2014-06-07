using NGUI.Meshing;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Font"), ExecuteInEditMode]
public class UIFont : MonoBehaviour
{
    private static List<UITextMarkup> _tempMarkup;
    private static readonly UITextPosition[] empty = new UITextPosition[0];
    private static int[] mangleIndices = new int[8];
    private static Vector3[] manglePoints = new Vector3[8];
    private static UITextPosition[] manglePositions = new UITextPosition[8];
    private static readonly MangleSorter mangleSort = new MangleSorter();
    private const int mangleStartSize = 8;
    [HideInInspector, SerializeField]
    private UIAtlas mAtlas;
    private List<Color> mColors = new List<Color>();
    [SerializeField, HideInInspector]
    private BMFont mFont = new BMFont();
    [SerializeField, HideInInspector]
    private Material mMat;
    [SerializeField, HideInInspector]
    private UIFont mReplacement;
    [HideInInspector, SerializeField]
    private int mSpacingX;
    [SerializeField, HideInInspector]
    private int mSpacingY;
    private UIAtlas.Sprite mSprite;
    private bool mSpriteSet;
    [HideInInspector, SerializeField]
    private Rect mUVRect = new Rect(0f, 0f, 1f, 1f);

    private void Align(ref PrintContext ctx)
    {
        if (this.mFont.charSize > 0)
        {
            int num;
            switch (ctx.alignment)
            {
                case Alignment.Left:
                    num = 0;
                    break;

                case Alignment.Center:
                    num = Mathf.Max(0, Mathf.RoundToInt((ctx.lineWidth - ctx.x) * 0.5f));
                    break;

                case Alignment.Right:
                    num = Mathf.Max(0, Mathf.RoundToInt((float) (ctx.lineWidth - ctx.x)));
                    break;

                case Alignment.LeftOverflowRight:
                    num = Mathf.Max(0, Mathf.RoundToInt((float) (ctx.x - ctx.lineWidth)));
                    break;

                default:
                    throw new NotImplementedException();
            }
            if (num != 0)
            {
                float num2 = (float) (((double) num) / ((double) this.mFont.charSize));
                for (int i = ctx.indexOffset; i < ctx.m.vSize; i++)
                {
                    ctx.m.v[i].x += num2;
                }
            }
        }
    }

    [Obsolete("You must specify some point", true)]
    public UITextPosition[] CalculatePlacement(string text)
    {
        return empty;
    }

    public UITextPosition CalculatePlacement(string text, Vector2 point)
    {
        Vector2[] points = new Vector2[] { point };
        UITextPosition[] positions = new UITextPosition[] { new UITextPosition() };
        this.CalculatePlacement(points, positions, text);
        return positions[0];
    }

    public UITextPosition[] CalculatePlacement(string text, params Vector2[] points)
    {
        UITextPosition[] positions = null;
        return ((this.CalculatePlacement(points, ref positions, text) <= 0) ? empty : positions);
    }

    public UITextPosition[] CalculatePlacement(string text, params Vector3[] points)
    {
        UITextPosition[] positions = null;
        return ((this.CalculatePlacement(points, ref positions, text) <= 0) ? empty : positions);
    }

    public UITextPosition CalculatePlacement(string text, Vector3 point)
    {
        Vector3[] points = new Vector3[] { point };
        UITextPosition[] positions = new UITextPosition[] { new UITextPosition() };
        this.CalculatePlacement(points, positions, text);
        return positions[0];
    }

    private int CalculatePlacement(Vector2[] points, ref UITextPosition[] positions, string text)
    {
        if (this.mReplacement != null)
        {
            return this.mReplacement.CalculatePlacement(points, (UITextPosition[]) positions, text);
        }
        int len = this.FillMangle(points, 0, positions, 0, Mathf.Min(points.Length, positions.Length));
        if (len > 0)
        {
            return this.ProcessShared(len, ref positions, text);
        }
        return len;
    }

    public int CalculatePlacement(Vector2[] points, UITextPosition[] positions, string text)
    {
        if (positions == null)
        {
            throw new ArgumentNullException("positions");
        }
        return this.CalculatePlacement(points, ref positions, text, base.transform);
    }

    private int CalculatePlacement(Vector3[] points, ref UITextPosition[] positions, string text)
    {
        if (this.mReplacement != null)
        {
            return this.mReplacement.CalculatePlacement(points, (UITextPosition[]) positions, text);
        }
        int len = this.FillMangle(points, 0, positions, 0, Mathf.Min(points.Length, positions.Length));
        if (len > 0)
        {
            return this.ProcessShared(len, ref positions, text);
        }
        return len;
    }

    public int CalculatePlacement(Vector3[] points, UITextPosition[] positions, string text)
    {
        if (positions == null)
        {
            throw new ArgumentNullException("positions");
        }
        return this.CalculatePlacement(points, ref positions, text, base.transform);
    }

    public UITextPosition CalculatePlacement(string text, Matrix4x4 transform, Vector3 point)
    {
        Vector3[] points = new Vector3[] { point };
        UITextPosition[] positions = new UITextPosition[] { new UITextPosition() };
        this.CalculatePlacement(points, positions, text, transform);
        return positions[0];
    }

    public UITextPosition[] CalculatePlacement(string text, Matrix4x4 transform, params Vector2[] points)
    {
        UITextPosition[] positions = null;
        if (points == null)
        {
            return null;
        }
        if (points.Length == 0)
        {
            return empty;
        }
        return ((this.CalculatePlacement(points, ref positions, text, transform) <= 0) ? empty : positions);
    }

    public UITextPosition[] CalculatePlacement(string text, Matrix4x4 transform, params Vector3[] points)
    {
        UITextPosition[] positions = null;
        if (points == null)
        {
            return null;
        }
        if (points.Length == 0)
        {
            return empty;
        }
        return ((this.CalculatePlacement(points, ref positions, text, transform) <= 0) ? empty : positions);
    }

    public UITextPosition CalculatePlacement(string text, Matrix4x4 transform, Vector2 point)
    {
        Vector2[] points = new Vector2[] { point };
        UITextPosition[] positions = new UITextPosition[] { new UITextPosition() };
        this.CalculatePlacement(points, positions, text, transform);
        return positions[0];
    }

    public UITextPosition CalculatePlacement(string text, Transform self, Vector2 point)
    {
        Vector2[] points = new Vector2[] { point };
        UITextPosition[] positions = new UITextPosition[] { new UITextPosition() };
        this.CalculatePlacement(points, positions, text, self);
        return positions[0];
    }

    public UITextPosition[] CalculatePlacement(string text, Transform self, params Vector2[] points)
    {
        UITextPosition[] positions = null;
        if (points == null)
        {
            return null;
        }
        if (points.Length == 0)
        {
            return empty;
        }
        return ((this.CalculatePlacement(points, ref positions, text, self) <= 0) ? empty : positions);
    }

    public UITextPosition CalculatePlacement(string text, Transform self, Vector3 point)
    {
        Vector3[] points = new Vector3[] { point };
        UITextPosition[] positions = new UITextPosition[] { new UITextPosition() };
        this.CalculatePlacement(points, positions, text, self);
        return positions[0];
    }

    public UITextPosition[] CalculatePlacement(string text, Transform self, params Vector3[] points)
    {
        UITextPosition[] positions = null;
        if (points == null)
        {
            return null;
        }
        if (points.Length == 0)
        {
            return empty;
        }
        return ((this.CalculatePlacement(points, ref positions, text, self) <= 0) ? empty : positions);
    }

    private int CalculatePlacement(Vector2[] points, ref UITextPosition[] positions, string text, Matrix4x4 transform)
    {
        if (this.mReplacement != null)
        {
            return this.mReplacement.CalculatePlacement(points, (UITextPosition[]) positions, text);
        }
        int len = this.FillMangle(points, 0, positions, 0, Mathf.Min(points.Length, positions.Length));
        if (len <= 0)
        {
            return len;
        }
        for (int i = 0; i < len; i++)
        {
            manglePoints[i] = transform.MultiplyPoint(manglePoints[i]);
        }
        return this.ProcessShared(len, ref positions, text);
    }

    public int CalculatePlacement(Vector2[] points, UITextPosition[] positions, string text, Matrix4x4 transform)
    {
        if (positions == null)
        {
            throw new ArgumentNullException("positions");
        }
        return this.CalculatePlacement(points, ref positions, text, transform);
    }

    private int CalculatePlacement(Vector2[] points, ref UITextPosition[] positions, string text, Transform self)
    {
        if (self == null)
        {
            return this.CalculatePlacement(points, (UITextPosition[]) positions, text);
        }
        return this.CalculatePlacement(points, (UITextPosition[]) positions, text, self.worldToLocalMatrix);
    }

    public int CalculatePlacement(Vector2[] points, UITextPosition[] positions, string text, Transform self)
    {
        if (positions == null)
        {
            throw new ArgumentNullException("positions");
        }
        return this.CalculatePlacement(points, ref positions, text, base.transform);
    }

    private int CalculatePlacement(Vector3[] points, ref UITextPosition[] positions, string text, Matrix4x4 transform)
    {
        if (this.mReplacement != null)
        {
            return this.mReplacement.CalculatePlacement(points, (UITextPosition[]) positions, text);
        }
        int len = this.FillMangle(points, 0, positions, 0, points.Length);
        if (len <= 0)
        {
            return len;
        }
        for (int i = 0; i < len; i++)
        {
            manglePoints[i] = transform.MultiplyPoint(manglePoints[i]);
        }
        return this.ProcessShared(len, ref positions, text);
    }

    public int CalculatePlacement(Vector3[] points, UITextPosition[] positions, string text, Matrix4x4 transform)
    {
        if (positions == null)
        {
            throw new ArgumentNullException("positions");
        }
        return this.CalculatePlacement(points, ref positions, text, transform);
    }

    private int CalculatePlacement(Vector3[] points, ref UITextPosition[] positions, string text, Transform self)
    {
        if (self == null)
        {
            return this.CalculatePlacement(points, (UITextPosition[]) positions, text);
        }
        return this.CalculatePlacement(points, (UITextPosition[]) positions, text, self.worldToLocalMatrix);
    }

    public int CalculatePlacement(Vector3[] points, UITextPosition[] positions, string text, Transform self)
    {
        if (positions == null)
        {
            throw new ArgumentNullException("positions");
        }
        return this.CalculatePlacement(points, ref positions, text, base.transform);
    }

    public Vector2 CalculatePrintedSize(string text, bool encoding, SymbolStyle symbolStyle)
    {
        if (this.mReplacement != null)
        {
            return this.mReplacement.CalculatePrintedSize(text, encoding, symbolStyle);
        }
        Vector2 zero = Vector2.zero;
        if (((this.mFont != null) && this.mFont.isValid) && !string.IsNullOrEmpty(text))
        {
            if (encoding)
            {
                text = NGUITools.StripSymbols(text);
            }
            int length = text.Length;
            int num2 = 0;
            int num3 = 0;
            int num4 = 0;
            int previousChar = 0;
            int num6 = this.mFont.charSize + this.mSpacingY;
            for (int i = 0; i < length; i++)
            {
                char index = text[i];
                if (index == '\n')
                {
                    if (num3 > num2)
                    {
                        num2 = num3;
                    }
                    num3 = 0;
                    num4 += num6;
                    previousChar = 0;
                }
                else if (index < ' ')
                {
                    previousChar = 0;
                }
                else
                {
                    BMSymbol symbol;
                    if (this.mFont.MatchSymbol(text, i, length, out symbol))
                    {
                        num3 += this.mSpacingX + symbol.width;
                        i += symbol.sequence.Length - 1;
                        previousChar = 0;
                    }
                    else
                    {
                        BMGlyph glyph;
                        if (this.mFont.GetGlyph(index, out glyph))
                        {
                            num3 += this.mSpacingX + ((previousChar == 0) ? glyph.advance : (glyph.advance + glyph.GetKerning(previousChar)));
                            previousChar = index;
                        }
                    }
                }
            }
            float num8 = (this.mFont.charSize <= 0) ? 1f : (1f / ((float) this.mFont.charSize));
            zero.x = num8 * ((num3 <= num2) ? ((float) num2) : ((float) num3));
            zero.y = num8 * (num4 + num6);
        }
        return zero;
    }

    public static bool CheckIfRelated(UIFont a, UIFont b)
    {
        if ((a == null) || (b == null))
        {
            return false;
        }
        return (((a == b) || a.References(b)) || b.References(a));
    }

    private void DrawCarat(ref PrintContext ctx)
    {
        Vector2 vector;
        Vector2 vector2;
        Vector2 vector3;
        Vector2 vector4;
        vector.x = ctx.scale.x * (ctx.previousX + ctx.carratGlyph.offsetX);
        vector.y = -ctx.scale.y * (ctx.y + ctx.carratGlyph.offsetY);
        vector2.x = vector.x + (ctx.scale.x * ctx.carratGlyph.width);
        vector2.y = vector.y - (ctx.scale.y * ctx.carratGlyph.height);
        vector3.x = this.mUVRect.xMin + (ctx.invX * ctx.carratGlyph.x);
        vector3.y = this.mUVRect.yMax - (ctx.invY * ctx.carratGlyph.y);
        vector4.x = vector3.x + (ctx.invX * ctx.carratGlyph.width);
        vector4.y = vector3.y - (ctx.invY * ctx.carratGlyph.height);
        ctx.m.FastQuad(vector, vector2, vector3, vector4, ctx.normalColor);
    }

    private static UITextMod EndLine(ref StringBuilder s)
    {
        int num = s.Length - 1;
        if ((num > 0) && (s[num] == ' '))
        {
            s[num] = '\n';
            return UITextMod.Replaced;
        }
        s.Append('\n');
        return UITextMod.Added;
    }

    private int FillMangle(Vector2[] points, int pointsOffset, UITextPosition[] positions, int positionsOffset, int len)
    {
        if ((positions == null) || (points == null))
        {
            return 0;
        }
        if (((points.Length - pointsOffset) < len) || ((positions.Length - positionsOffset) < len))
        {
            throw new ArgumentOutOfRangeException();
        }
        if (len > mangleIndices.Length)
        {
            Array.Resize<Vector3>(ref manglePoints, len);
            Array.Resize<int>(ref mangleIndices, len);
            Array.Resize<UITextPosition>(ref manglePositions, len);
        }
        for (int i = 0; i < len; i++)
        {
            manglePoints[i].x = points[i + pointsOffset].x;
            manglePoints[i].y = points[i + pointsOffset].y;
            manglePoints[i].z = 0f;
            mangleIndices[i] = i;
        }
        return len;
    }

    private int FillMangle(Vector3[] points, int pointsOffset, UITextPosition[] positions, int positionsOffset, int len)
    {
        if (points == null)
        {
            throw new ArgumentNullException("null array", "points");
        }
        if ((points.Length - pointsOffset) < len)
        {
            throw new ArgumentException("not large enough", "points");
        }
        if ((positions != null) && ((positions.Length - positionsOffset) < len))
        {
            throw new ArgumentException("not large enough", "positions");
        }
        if (len > mangleIndices.Length)
        {
            Array.Resize<Vector3>(ref manglePoints, len);
            Array.Resize<int>(ref mangleIndices, len);
            Array.Resize<UITextPosition>(ref manglePositions, len);
        }
        for (int i = 0; i < len; i++)
        {
            manglePoints[i] = points[i + pointsOffset];
            mangleIndices[i] = i;
        }
        return len;
    }

    private void MangleSort(int len)
    {
        mangleSort.SetLineSizing((double) this.bmFont.charSize, (double) this.verticalSpacing);
        Array.Sort<Vector3, int>(manglePoints, mangleIndices, 0, len, mangleSort);
    }

    public void MarkAsDirty()
    {
        this.mSprite = null;
        UILabel[] labelArray = NGUITools.FindActive<UILabel>();
        int index = 0;
        int length = labelArray.Length;
        while (index < length)
        {
            UILabel label = labelArray[index];
            if ((label.enabled && label.gameObject.activeInHierarchy) && CheckIfRelated(this, label.font))
            {
                UIFont font = label.font;
                label.font = null;
                label.font = font;
            }
            index++;
        }
    }

    public void Print(string text, Color color, MeshBuffer m, bool encoding, SymbolStyle symbolStyle, Alignment alignment, int lineWidth)
    {
        UITextSelection selection = new UITextSelection();
        this.Print(text, color, m, encoding, symbolStyle, alignment, lineWidth, ref selection, '\0', color, Color.clear, '\0', -1f);
    }

    public void Print(string text, Color normalColor, MeshBuffer m, bool encoding, SymbolStyle symbolStyle, Alignment alignment, int lineWidth, ref UITextSelection selection, char carratChar, Color highlightTextColor, Color highlightBarColor, char highlightChar, float highlightSplit)
    {
        if (this.mReplacement != null)
        {
            this.mReplacement.Print(text, normalColor, m, encoding, symbolStyle, alignment, lineWidth, ref selection, carratChar, highlightTextColor, highlightBarColor, highlightChar, highlightSplit);
        }
        else if ((this.mFont != null) && (text != null))
        {
            if (!this.mFont.isValid)
            {
                Debug.LogError("Attempting to print using an invalid font!");
            }
            else
            {
                PrintContext context;
                int symbolSkipCount = 0;
                this.mColors.Clear();
                this.mColors.Add(normalColor);
                context.m = m;
                context.lineWidth = lineWidth;
                context.alignment = alignment;
                context.scale.x = (this.mFont.charSize <= 0) ? 1f : (1f / ((float) this.mFont.charSize));
                context.scale.y = context.scale.x;
                context.normalColor = normalColor;
                context.indexOffset = context.m.vSize;
                context.maxX = 0;
                context.x = 0;
                context.y = 0;
                context.prev = 0;
                context.lineHeight = this.mFont.charSize + this.mSpacingY;
                context.v0 = new Vector3();
                context.v1 = new Vector3();
                context.u0 = new Vector2();
                context.u1 = new Vector2();
                context.invX = this.uvRect.width / ((float) this.mFont.texWidth);
                context.invY = this.uvRect.height / ((float) this.mFont.texHeight);
                context.textLength = text.Length;
                context.nonHighlightColor = normalColor;
                context.carratChar = carratChar;
                if (context.carratChar == '\0')
                {
                    context.carratIndex = -1;
                    context.carratGlyph = null;
                }
                else
                {
                    context.carratIndex = selection.carratIndex;
                    if (context.carratIndex == -1)
                    {
                        context.carratGlyph = null;
                        context.carratChar = '\0';
                    }
                    else if (!this.mFont.GetGlyph(carratChar, out context.carratGlyph))
                    {
                        context.carratIndex = -1;
                    }
                }
                context.highlightChar = highlightChar;
                context.highlightBarColor = highlightBarColor;
                context.highlightTextColor = highlightTextColor;
                context.highlightSplit = highlightSplit;
                context.highlightBarDraw = (((context.highlightChar != '\0') && (context.highlightSplit >= 0f)) && (context.highlightSplit <= 1f)) && (highlightBarColor.a > 0f);
                if (!context.highlightBarDraw && (context.highlightTextColor == context.normalColor))
                {
                    context.highlight = UIHighlight.invalid;
                    context.highlightGlyph = null;
                }
                else if (!selection.GetHighlight(out context.highlight))
                {
                    context.highlightGlyph = null;
                    context.highlightBarDraw = false;
                }
                else if ((context.highlightChar != context.carratChar) ? !this.mFont.GetGlyph(context.highlightChar, out context.highlightGlyph) : ((context.highlightGlyph = context.carratGlyph) == null))
                {
                    context.highlight = UIHighlight.invalid;
                }
                context.j = 0;
                context.previousX = 0;
                context.isLineEnd = false;
                context.highlightVertex = -1;
                context.glyph = null;
                context.c = '\0';
                context.skipSymbols = !encoding || (symbolStyle == SymbolStyle.None);
                context.printChar = false;
                context.printColor = normalColor;
                context.symbol = null;
                context.text = text;
                context.i = 0;
                while (context.i < context.textLength)
                {
                    context.c = context.text[context.i];
                    if (context.c == '\n')
                    {
                        context.isLineEnd = true;
                    }
                    else
                    {
                        if (context.c < ' ')
                        {
                            context.prev = 0;
                            goto Label_0E19;
                        }
                        if (encoding && (context.c == '['))
                        {
                            int num2 = NGUITools.ParseSymbol(text, context.i, this.mColors, ref symbolSkipCount);
                            if (num2 > 0)
                            {
                                context.nonHighlightColor = this.mColors[this.mColors.Count - 1];
                                context.i += num2 - 1;
                                goto Label_0E19;
                            }
                        }
                        if (context.skipSymbols || !this.mFont.MatchSymbol(context.text, context.i, context.textLength, out context.symbol))
                        {
                            if (!this.mFont.GetGlyph(context.c, out context.glyph))
                            {
                                goto Label_0B6F;
                            }
                            bool flag = context.prev != 0;
                            if (flag)
                            {
                                context.previousX = context.x;
                                context.x += context.glyph.GetKerning(context.prev);
                            }
                            if (context.c == ' ')
                            {
                                if (!flag)
                                {
                                    context.previousX = context.x;
                                }
                                context.x += this.mSpacingX + context.glyph.advance;
                                context.prev = context.c;
                                goto Label_0B6F;
                            }
                            context.v0.x = context.scale.x * (context.x + context.glyph.offsetX);
                            context.v0.y = -context.scale.y * (context.y + context.glyph.offsetY);
                            context.v1.x = context.v0.x + (context.scale.x * context.glyph.width);
                            context.v1.y = context.v0.y - (context.scale.y * context.glyph.height);
                            context.u0.x = this.mUVRect.xMin + (context.invX * context.glyph.x);
                            context.u0.y = this.mUVRect.yMax - (context.invY * context.glyph.y);
                            context.u1.x = context.u0.x + (context.invX * context.glyph.width);
                            context.u1.y = context.u0.y - (context.invY * context.glyph.height);
                            if (!flag)
                            {
                                context.previousX = context.x;
                            }
                            context.x += this.mSpacingX + context.glyph.advance;
                            context.prev = context.c;
                            if ((context.glyph.channel == 0) || (context.glyph.channel == 15))
                            {
                                context.printColor = ((context.highlight.b.i <= context.j) || (context.highlight.a.i > context.j)) ? context.nonHighlightColor : context.highlightTextColor;
                            }
                            else
                            {
                                Color color = ((context.highlight.b.i <= context.j) || (context.highlight.a.i > context.j)) ? context.nonHighlightColor : context.highlightTextColor;
                                color = (Color) (color * 0.49f);
                                switch (context.glyph.channel)
                                {
                                    case 1:
                                        color.b += 0.51f;
                                        break;

                                    case 2:
                                        color.g += 0.51f;
                                        break;

                                    case 4:
                                        color.r += 0.51f;
                                        break;

                                    case 8:
                                        color.a += 0.51f;
                                        break;
                                }
                                context.printColor = color;
                            }
                        }
                        else
                        {
                            context.v0.x = context.scale.x * context.x;
                            context.v0.y = -context.scale.y * context.y;
                            context.v1.x = context.v0.x + (context.scale.x * context.symbol.width);
                            context.v1.y = context.v0.y - (context.scale.y * context.symbol.height);
                            context.u0.x = this.mUVRect.xMin + (context.invX * context.symbol.x);
                            context.u0.y = this.mUVRect.yMax - (context.invY * context.symbol.y);
                            context.u1.x = context.u0.x + (context.invX * context.symbol.width);
                            context.u1.y = context.u0.y - (context.invY * context.symbol.height);
                            context.previousX = context.x;
                            context.x += this.mSpacingX + context.symbol.width;
                            context.i += context.symbol.sequence.Length - 1;
                            context.prev = 0;
                            if (symbolStyle == SymbolStyle.Colored)
                            {
                                context.printColor = ((context.highlight.b.i <= context.j) || (context.highlight.a.i > context.j)) ? context.nonHighlightColor : context.highlightTextColor;
                            }
                            else
                            {
                                context.printColor = ((context.highlight.b.i <= context.j) || (context.highlight.a.i > context.j)) ? new Color(1f, 1f, 1f, context.nonHighlightColor.a) : context.highlightTextColor;
                            }
                        }
                        context.printChar = true;
                    }
                Label_0B6F:
                    if (context.highlight.b.i == context.j)
                    {
                        if (context.printChar)
                        {
                            context.m.FastQuad(context.v0, context.v1, context.u0, context.u1, context.printColor);
                            context.printChar = false;
                        }
                        if (context.highlightBarDraw)
                        {
                            this.PutHighlightEnd(ref context);
                        }
                    }
                    else if (context.highlight.a.i == context.j)
                    {
                        if (context.highlightBarDraw)
                        {
                            this.PutHighlightStart(ref context);
                        }
                        if (context.printChar)
                        {
                            context.m.FastQuad(context.v0, context.v1, context.u0, context.u1, context.printColor);
                            context.printChar = false;
                        }
                    }
                    else if (context.carratIndex == context.j)
                    {
                        if (context.printChar)
                        {
                            context.m.FastQuad(context.v0, context.v1, context.u0, context.u1, context.printColor);
                            context.printChar = false;
                        }
                        this.DrawCarat(ref context);
                    }
                    else if (context.printChar)
                    {
                        context.m.FastQuad(context.v0, context.v1, context.u0, context.u1, context.printColor);
                        context.printChar = false;
                    }
                    context.j++;
                    if (context.isLineEnd)
                    {
                        context.isLineEnd = false;
                        if (context.x > context.maxX)
                        {
                            context.maxX = context.x;
                        }
                        bool flag2 = context.highlightBarDraw && (context.highlightVertex != -1);
                        if (flag2)
                        {
                            this.PutHighlightEnd(ref context);
                        }
                        if (context.indexOffset < context.m.vSize)
                        {
                            this.Align(ref context);
                            context.indexOffset = context.m.vSize;
                        }
                        context.previousX = context.x;
                        context.x = 0;
                        context.y += context.lineHeight;
                        context.prev = 0;
                        if (flag2)
                        {
                            this.PutHighlightStart(ref context);
                        }
                    }
                Label_0E19:
                    context.i++;
                }
                context.previousX = context.x;
                if (context.highlightVertex != -1)
                {
                    this.PutHighlightEnd(ref context);
                }
                else if (context.j == context.carratIndex)
                {
                    this.DrawCarat(ref context);
                }
                if (context.indexOffset < context.m.vSize)
                {
                    this.Align(ref context);
                    context.indexOffset = context.m.vSize;
                }
            }
        }
    }

    private int ProcessPlacement(int count, string text)
    {
        if (this.mReplacement != null)
        {
            return this.mReplacement.ProcessPlacement(count, text);
        }
        int index = 0;
        if (manglePoints[mangleIndices[index]].y < 0f)
        {
            do
            {
                manglePositions[index] = new UITextPosition(UITextRegion.Before);
            }
            while ((++index < count) && (manglePoints[mangleIndices[index]].y < 0f));
            if (index >= count)
            {
                return count;
            }
        }
        int length = text.Length;
        int num3 = this.verticalSpacing + this.bmFont.charSize;
        if (length == 0)
        {
            while (manglePoints[mangleIndices[index]].y <= num3)
            {
                if (manglePoints[mangleIndices[index]].x < 0f)
                {
                    manglePositions[index] = new UITextPosition(UITextRegion.Pre);
                }
                else
                {
                    manglePositions[index] = new UITextPosition(UITextRegion.Past);
                }
                if (++index >= count)
                {
                    return count;
                }
            }
            while (index < count)
            {
                manglePositions[index++] = new UITextPosition(UITextRegion.End);
            }
            return count;
        }
        int line = 0;
        int column = 0;
        int position = 0;
        int num7 = 0;
        int num8 = -1;
        int num9 = 0;
        int num10 = 0;
        bool flag = false;
        bool flag2 = false;
        while (index < count)
        {
            Vector3 vector = manglePoints[mangleIndices[index]];
            int num12 = Mathf.FloorToInt(vector.y) / num3;
        Label_019E:
            if (flag2)
            {
                if (num12 == line)
                {
                    manglePositions[index++] = new UITextPosition(line, column, position, UITextRegion.Past);
                }
                else
                {
                    while (index < count)
                    {
                        manglePositions[index++] = new UITextPosition(line, column, position, UITextRegion.End);
                    }
                }
            }
            else
            {
                if (num12 > line)
                {
                    flag = false;
                    do
                    {
                        while (text[position] != '\n')
                        {
                            if (++position >= length)
                            {
                                flag2 = true;
                                goto Label_019E;
                            }
                            column++;
                        }
                        line++;
                        column = 0;
                        num10 = 0;
                        num8 = position;
                        num7 = ++position;
                        num9 = 0;
                    }
                    while (num12 > line);
                    continue;
                }
                if (vector.x < 0f)
                {
                    manglePositions[index++] = new UITextPosition(line, 0, num7, UITextRegion.Pre);
                }
                else
                {
                    if (flag)
                    {
                        manglePositions[index++] = new UITextPosition(line, num10, num8, UITextRegion.Past);
                        continue;
                    }
                    while (num9 < vector.x)
                    {
                        BMGlyph glyph;
                        if (position >= length)
                        {
                            flag2 = true;
                            goto Label_019E;
                        }
                        int num13 = text[position];
                        if (num13 == 10)
                        {
                            num8 = position;
                            num7 = ++position;
                            num10 = column;
                            column = 0;
                            flag = true;
                            goto Label_019E;
                        }
                        if (this.mFont.GetGlyph(num13, out glyph))
                        {
                            if (num8 >= num7)
                            {
                                num9 += glyph.GetKerning(text[num8]);
                            }
                            num9 += this.mSpacingX + glyph.advance;
                        }
                        num8 = position++;
                        num10 = column++;
                    }
                    manglePositions[index++] = new UITextPosition(line, num10, num8, UITextRegion.Inside);
                }
            }
        }
        if (index < count)
        {
            Debug.LogError(" skipped " + (count - index));
        }
        return count;
    }

    private int ProcessShared(int len, ref UITextPosition[] positions, string text)
    {
        if (this.mFont.charSize > 0)
        {
            for (int i = 0; i < len; i++)
            {
                manglePoints[i].x *= this.mFont.charSize;
                manglePoints[i].y *= this.mFont.charSize;
            }
        }
        this.MangleSort(len);
        len = this.ProcessPlacement(len, text);
        if (len > 0)
        {
            if (positions == null)
            {
                positions = new UITextPosition[len];
            }
            for (int j = 0; j < len; j++)
            {
                positions[mangleIndices[j]] = manglePositions[j];
            }
        }
        return len;
    }

    private void PutHighlightEnd(ref PrintContext ctx)
    {
        if (ctx.highlightVertex != -1)
        {
            float num = (ctx.scale.x * (ctx.previousX + ctx.highlightGlyph.offsetX)) - ctx.m.v[ctx.highlightVertex].x;
            if (num > 0f)
            {
                ctx.m.v[ctx.highlightVertex].x += num;
                ctx.m.v[ctx.highlightVertex + 1].x += num;
                ctx.m.v[ctx.highlightVertex + 4].x += num;
                ctx.m.v[(ctx.highlightVertex + 4) + 1].x += num;
                ctx.m.v[(ctx.highlightVertex + 4) + 2].x += num;
                ctx.m.v[(ctx.highlightVertex + 4) + 3].x += num;
            }
            ctx.highlightVertex = -1;
        }
    }

    private void PutHighlightStart(ref PrintContext ctx)
    {
        Vector2 vector;
        Vector2 vector2;
        Vector2 vector3;
        Vector2 vector4;
        if (ctx.highlightVertex != -1)
        {
            this.PutHighlightEnd(ref ctx);
        }
        float num = ctx.scale.x * (ctx.highlightGlyph.width * ctx.highlightSplit);
        vector.x = (ctx.scale.x * (ctx.previousX + ctx.highlightGlyph.offsetX)) - num;
        vector.y = -ctx.scale.y * (ctx.y + ctx.highlightGlyph.offsetY);
        vector2.x = vector.x + num;
        float num2 = vector2.x - vector.x;
        vector.x += num2;
        vector2.x += num2;
        vector2.y = vector.y - (ctx.scale.y * ctx.highlightGlyph.height);
        vector3.x = this.mUVRect.xMin + (ctx.invX * ctx.highlightGlyph.x);
        vector3.y = this.mUVRect.yMax - (ctx.invY * ctx.highlightGlyph.y);
        vector4.x = vector3.x + ((ctx.invX * ctx.highlightGlyph.width) * ctx.highlightSplit);
        vector4.y = vector3.y - (ctx.invY * ctx.highlightGlyph.height);
        ctx.m.FastQuad(vector, vector2, vector3, vector4, ctx.highlightBarColor);
        ctx.highlightVertex = ctx.m.FastQuad(new Vector2(vector2.x, vector.y), vector2, new Vector2(vector4.x, vector3.y), vector4, ctx.highlightBarColor);
        float x = vector2.x;
        vector2.x = vector.x + (ctx.scale.x * ctx.highlightGlyph.width);
        vector.x = x;
        x = vector4.x;
        vector4.x = vector3.x + (ctx.invX * ctx.highlightGlyph.width);
        vector3.x = x;
        ctx.m.FastQuad(vector, vector2, vector3, vector4, ctx.highlightBarColor);
    }

    private bool References(UIFont font)
    {
        if (font == null)
        {
            return false;
        }
        return ((font == this) || ((this.mReplacement != null) && this.mReplacement.References(font)));
    }

    private void Trim()
    {
        Texture texture = this.mAtlas.texture;
        if ((texture != null) && (this.mSprite != null))
        {
            Rect rect = NGUIMath.ConvertToPixels(this.mUVRect, this.texture.width, this.texture.height, true);
            Rect rect2 = (this.mAtlas.coordinates != UIAtlas.Coordinates.TexCoords) ? this.mSprite.outer : NGUIMath.ConvertToPixels(this.mSprite.outer, texture.width, texture.height, true);
            int xMin = Mathf.RoundToInt(rect2.xMin - rect.xMin);
            int yMin = Mathf.RoundToInt(rect2.yMin - rect.yMin);
            int xMax = Mathf.RoundToInt(rect2.xMax - rect.xMin);
            int yMax = Mathf.RoundToInt(rect2.yMax - rect.yMin);
            this.mFont.Trim(xMin, yMin, xMax, yMax);
        }
    }

    public string WrapText(List<UITextMarkup> markups, string text, float maxWidth, int maxLineCount)
    {
        return this.WrapText(markups, text, maxWidth, maxLineCount, false, SymbolStyle.None);
    }

    public string WrapText(List<UITextMarkup> markups, string text, float maxWidth, int maxLineCount, bool encoding)
    {
        return this.WrapText(markups, text, maxWidth, maxLineCount, encoding, SymbolStyle.None);
    }

    public string WrapText(List<UITextMarkup> markups, string text, float maxWidth, int maxLineCount, bool encoding, SymbolStyle symbolStyle)
    {
        if (this.mReplacement != null)
        {
            return this.mReplacement.WrapText(markups, text, maxWidth, maxLineCount, encoding, symbolStyle);
        }
        if (markups == null)
        {
        }
        markups = tempMarkup;
        markups.Clear();
        int num = Mathf.RoundToInt(maxWidth * this.size);
        if (num < 1)
        {
            return text;
        }
        StringBuilder s = new StringBuilder();
        int length = text.Length;
        int num3 = num;
        int previousChar = 0;
        int startIndex = 0;
        int offset = 0;
        bool flag = true;
        bool flag2 = maxLineCount != 1;
        int num7 = 1;
        BMSymbol symbol = null;
        int num8 = 0;
        while (offset < length)
        {
            int num9;
            bool flag3;
            UITextMarkup markup24;
            UITextMarkup markup30;
            char ch = text[offset];
            if ((previousChar == 0x5c) && (ch == 'n'))
            {
                if (startIndex < (offset - 1))
                {
                    s.Append(text.Substring(startIndex, offset - (startIndex + 2)));
                }
                else
                {
                    s.Append(ch);
                }
                s.Remove(s.Length - 1, 1);
                UITextMarkup item = new UITextMarkup {
                    index = offset - 1,
                    mod = UITextMod.Removed
                };
                markups.Add(item);
                UITextMarkup markup2 = new UITextMarkup {
                    index = offset,
                    mod = UITextMod.Replaced,
                    value = '\n'
                };
                markups.Add(markup2);
                startIndex = offset + 1;
                ch = '\n';
            }
            if (ch == '\n')
            {
                if (!flag2 || (num7 == maxLineCount))
                {
                    UITextMarkup markup3 = new UITextMarkup {
                        index = offset
                    };
                    markups.Add(markup3);
                    break;
                }
                num3 = num;
                if (startIndex < offset)
                {
                    s.Append(text.Substring(startIndex, (offset - startIndex) + 1));
                }
                else
                {
                    s.Append(ch);
                }
                flag = true;
                num7++;
                startIndex = offset + 1;
                previousChar = 0;
                goto Label_08B0;
            }
            if (((ch == ' ') && (previousChar != 0x20)) && (startIndex < offset))
            {
                s.Append(text.Substring(startIndex, (offset - startIndex) + 1));
                flag = false;
                startIndex = offset + 1;
                previousChar = ch;
            }
            if ((encoding && (ch == '[')) && ((offset + 2) < length))
            {
                if (text[offset + 2] == ']')
                {
                    if (text[offset + 1] == '-')
                    {
                        if (num8 != 0)
                        {
                            goto Label_051E;
                        }
                        UITextMarkup markup4 = new UITextMarkup {
                            index = offset,
                            mod = UITextMod.Removed
                        };
                        markups.Add(markup4);
                        UITextMarkup markup5 = new UITextMarkup {
                            index = offset + 1,
                            mod = UITextMod.Removed
                        };
                        markups.Add(markup5);
                        UITextMarkup markup6 = new UITextMarkup {
                            index = offset + 2,
                            mod = UITextMod.Removed
                        };
                        markups.Add(markup6);
                        offset += 2;
                    }
                    else if (text[offset + 1] == '\x00bb')
                    {
                        if (num8++ != 0)
                        {
                            goto Label_051E;
                        }
                        UITextMarkup markup7 = new UITextMarkup {
                            index = offset,
                            mod = UITextMod.Removed
                        };
                        markups.Add(markup7);
                        UITextMarkup markup8 = new UITextMarkup {
                            index = offset + 1,
                            mod = UITextMod.Removed
                        };
                        markups.Add(markup8);
                        UITextMarkup markup9 = new UITextMarkup {
                            index = offset + 2,
                            mod = UITextMod.Removed
                        };
                        markups.Add(markup9);
                        offset += 2;
                    }
                    else
                    {
                        if ((text[offset + 1] != '\x00ab') || (--num8 != 0))
                        {
                            goto Label_051E;
                        }
                        UITextMarkup markup10 = new UITextMarkup {
                            index = offset,
                            mod = UITextMod.Removed
                        };
                        markups.Add(markup10);
                        UITextMarkup markup11 = new UITextMarkup {
                            index = offset + 1,
                            mod = UITextMod.Removed
                        };
                        markups.Add(markup11);
                        UITextMarkup markup12 = new UITextMarkup {
                            index = offset + 2,
                            mod = UITextMod.Removed
                        };
                        markups.Add(markup12);
                        offset += 2;
                    }
                    goto Label_08B0;
                }
                if ((((offset + 7) < length) && (text[offset + 7] == ']')) && (num8 == 0))
                {
                    UITextMarkup markup13 = new UITextMarkup {
                        index = offset,
                        mod = UITextMod.Removed
                    };
                    markups.Add(markup13);
                    UITextMarkup markup14 = new UITextMarkup {
                        index = offset + 1,
                        mod = UITextMod.Removed
                    };
                    markups.Add(markup14);
                    UITextMarkup markup15 = new UITextMarkup {
                        index = offset + 2,
                        mod = UITextMod.Removed
                    };
                    markups.Add(markup15);
                    UITextMarkup markup16 = new UITextMarkup {
                        index = offset + 3,
                        mod = UITextMod.Removed
                    };
                    markups.Add(markup16);
                    UITextMarkup markup17 = new UITextMarkup {
                        index = offset + 4,
                        mod = UITextMod.Removed
                    };
                    markups.Add(markup17);
                    UITextMarkup markup18 = new UITextMarkup {
                        index = offset + 5,
                        mod = UITextMod.Removed
                    };
                    markups.Add(markup18);
                    UITextMarkup markup19 = new UITextMarkup {
                        index = offset + 6,
                        mod = UITextMod.Removed
                    };
                    markups.Add(markup19);
                    UITextMarkup markup20 = new UITextMarkup {
                        index = offset + 7,
                        mod = UITextMod.Removed
                    };
                    markups.Add(markup20);
                    offset += 7;
                    goto Label_08B0;
                }
            }
        Label_051E:
            flag3 = (encoding && (symbolStyle != SymbolStyle.None)) && this.mFont.MatchSymbol(text, offset, length, out symbol);
            if (flag3)
            {
                num9 = this.mSpacingX + symbol.width;
            }
            else
            {
                BMGlyph glyph;
                if (this.mFont.GetGlyph(ch, out glyph))
                {
                    num9 = this.mSpacingX + ((previousChar == 0) ? glyph.advance : (glyph.advance + glyph.GetKerning(previousChar)));
                }
                else
                {
                    UITextMarkup markup21 = new UITextMarkup {
                        index = offset,
                        mod = UITextMod.Removed
                    };
                    markups.Add(markup21);
                    goto Label_08B0;
                }
            }
            num3 -= num9;
            if (num3 >= 0)
            {
                goto Label_084A;
            }
            if ((!flag && flag2) && (num7 != maxLineCount))
            {
                goto Label_0729;
            }
            s.Append(text.Substring(startIndex, Mathf.Max(0, offset - startIndex)));
            if (!flag2 || (num7 == maxLineCount))
            {
                startIndex = offset;
                UITextMarkup markup22 = new UITextMarkup {
                    index = offset
                };
                markups.Add(markup22);
                break;
            }
            UITextMod mod = EndLine(ref s);
            if (mod == UITextMod.Replaced)
            {
                UITextMarkup markup23 = new UITextMarkup {
                    index = offset,
                    mod = UITextMod.Replaced,
                    value = '\n'
                };
                markups.Add(markup23);
            }
            else if (mod == UITextMod.Added)
            {
                goto Label_0686;
            }
            goto Label_06AC;
        Label_0686:
            markup24 = new UITextMarkup();
            markup24.index = offset;
            markup24.mod = UITextMod.Added;
            markups.Add(markup24);
        Label_06AC:
            flag = true;
            num7++;
            if (ch == ' ')
            {
                startIndex = offset + 1;
                num3 = num;
                UITextMarkup markup25 = new UITextMarkup {
                    index = offset,
                    mod = UITextMod.Removed
                };
                markups.Add(markup25);
            }
            else
            {
                startIndex = offset;
                num3 = num - num9;
            }
            previousChar = 0;
            goto Label_084E;
        Label_0729:
            while ((startIndex < length) && (text[startIndex] == ' '))
            {
                UITextMarkup markup26 = new UITextMarkup {
                    index = startIndex,
                    mod = UITextMod.Removed
                };
                markups.Add(markup26);
                startIndex++;
            }
            flag = true;
            num3 = num;
            offset = startIndex - 1;
            for (int i = markups.Count - 1; i >= 0; i--)
            {
                UITextMarkup markup27 = markups[i];
                if (markup27.index < offset)
                {
                    break;
                }
                markups.RemoveAt(i);
            }
            previousChar = 0;
            if (!flag2 || (num7 == maxLineCount))
            {
                UITextMarkup markup28 = new UITextMarkup {
                    index = offset
                };
                markups.Add(markup28);
                break;
            }
            num7++;
            mod = EndLine(ref s);
            if (mod == UITextMod.Replaced)
            {
                UITextMarkup markup29 = new UITextMarkup {
                    index = offset,
                    mod = UITextMod.Replaced,
                    value = '\n'
                };
                markups.Add(markup29);
            }
            else if (mod == UITextMod.Added)
            {
                goto Label_081A;
            }
            goto Label_08B0;
        Label_081A:
            markup30 = new UITextMarkup();
            markup30.index = offset;
            markup30.mod = UITextMod.Added;
            markups.Add(markup30);
            goto Label_08B0;
        Label_084A:
            previousChar = ch;
        Label_084E:
            if (flag3)
            {
                for (int j = 0; j < symbol.sequence.Length; j++)
                {
                    UITextMarkup markup31 = new UITextMarkup {
                        index = offset + j,
                        mod = UITextMod.Removed
                    };
                    markups.Add(markup31);
                }
                offset += symbol.sequence.Length - 1;
                previousChar = 0;
            }
        Label_08B0:
            offset++;
        }
        if (startIndex < offset)
        {
            s.Append(text.Substring(startIndex, offset - startIndex));
        }
        return s.ToString();
    }

    public UIAtlas atlas
    {
        get
        {
            return ((this.mReplacement == null) ? this.mAtlas : this.mReplacement.atlas);
        }
        set
        {
            if (this.mReplacement != null)
            {
                this.mReplacement.atlas = value;
            }
            else if (this.mAtlas != value)
            {
                if (value == null)
                {
                    if (this.mAtlas != null)
                    {
                        this.mMat = this.mAtlas.spriteMaterial;
                    }
                    if (this.sprite != null)
                    {
                        this.mUVRect = this.uvRect;
                    }
                }
                this.mAtlas = value;
                this.MarkAsDirty();
            }
        }
    }

    public BMFont bmFont
    {
        get
        {
            return ((this.mReplacement == null) ? this.mFont : this.mReplacement.bmFont);
        }
    }

    public int horizontalSpacing
    {
        get
        {
            return ((this.mReplacement == null) ? this.mSpacingX : this.mReplacement.horizontalSpacing);
        }
        set
        {
            if (this.mReplacement != null)
            {
                this.mReplacement.horizontalSpacing = value;
            }
            else if (this.mSpacingX != value)
            {
                this.mSpacingX = value;
                this.MarkAsDirty();
            }
        }
    }

    public Material material
    {
        get
        {
            if (this.mReplacement != null)
            {
                return this.mReplacement.material;
            }
            return ((this.mAtlas == null) ? this.mMat : this.mAtlas.spriteMaterial);
        }
        set
        {
            if (this.mReplacement != null)
            {
                this.mReplacement.material = value;
            }
            else if ((this.mAtlas == null) && (this.mMat != value))
            {
                this.mMat = value;
                this.MarkAsDirty();
            }
        }
    }

    public UIFont replacement
    {
        get
        {
            return this.mReplacement;
        }
        set
        {
            UIFont font = value;
            if (font == this)
            {
                font = null;
            }
            if (this.mReplacement != font)
            {
                if ((font != null) && (font.replacement == this))
                {
                    font.replacement = null;
                }
                if (this.mReplacement != null)
                {
                    this.MarkAsDirty();
                }
                this.mReplacement = font;
                this.MarkAsDirty();
            }
        }
    }

    public int size
    {
        get
        {
            return ((this.mReplacement == null) ? this.mFont.charSize : this.mReplacement.size);
        }
    }

    public UIAtlas.Sprite sprite
    {
        get
        {
            if (this.mReplacement != null)
            {
                return this.mReplacement.sprite;
            }
            if (!this.mSpriteSet)
            {
                this.mSprite = null;
            }
            if (((this.mSprite == null) && (this.mAtlas != null)) && !string.IsNullOrEmpty(this.mFont.spriteName))
            {
                this.mSprite = this.mAtlas.GetSprite(this.mFont.spriteName);
                if (this.mSprite == null)
                {
                    this.mSprite = this.mAtlas.GetSprite(base.name);
                }
                this.mSpriteSet = true;
                if (this.mSprite == null)
                {
                    Debug.LogError("Can't find the sprite '" + this.mFont.spriteName + "' in UIAtlas on " + NGUITools.GetHierarchy(this.mAtlas.gameObject));
                    this.mFont.spriteName = null;
                }
            }
            return this.mSprite;
        }
    }

    public string spriteName
    {
        get
        {
            return ((this.mReplacement == null) ? this.mFont.spriteName : this.mReplacement.spriteName);
        }
        set
        {
            if (this.mReplacement != null)
            {
                this.mReplacement.spriteName = value;
            }
            else if (this.mFont.spriteName != value)
            {
                this.mFont.spriteName = value;
                this.MarkAsDirty();
            }
        }
    }

    public static List<UITextMarkup> tempMarkup
    {
        get
        {
            if (_tempMarkup == null)
            {
            }
            return (_tempMarkup = new List<UITextMarkup>());
        }
    }

    public int texHeight
    {
        get
        {
            return ((this.mReplacement == null) ? ((this.mFont == null) ? 1 : this.mFont.texHeight) : this.mReplacement.texHeight);
        }
    }

    public Texture2D texture
    {
        get
        {
            if (this.mReplacement != null)
            {
                return this.mReplacement.texture;
            }
            Material material = this.material;
            return ((material == null) ? null : (material.mainTexture as Texture2D));
        }
    }

    public int texWidth
    {
        get
        {
            return ((this.mReplacement == null) ? ((this.mFont == null) ? 1 : this.mFont.texWidth) : this.mReplacement.texWidth);
        }
    }

    public Rect uvRect
    {
        get
        {
            if (this.mReplacement != null)
            {
                return this.mReplacement.uvRect;
            }
            if (((this.mAtlas != null) && (this.mSprite == null)) && (this.sprite != null))
            {
                Texture texture = this.mAtlas.texture;
                if (texture != null)
                {
                    this.mUVRect = this.mSprite.outer;
                    if (this.mAtlas.coordinates == UIAtlas.Coordinates.Pixels)
                    {
                        this.mUVRect = NGUIMath.ConvertToTexCoords(this.mUVRect, texture.width, texture.height);
                    }
                    if (this.mSprite.hasPadding)
                    {
                        Rect mUVRect = this.mUVRect;
                        this.mUVRect.xMin = mUVRect.xMin - (this.mSprite.paddingLeft * mUVRect.width);
                        this.mUVRect.yMin = mUVRect.yMin - (this.mSprite.paddingBottom * mUVRect.height);
                        this.mUVRect.xMax = mUVRect.xMax + (this.mSprite.paddingRight * mUVRect.width);
                        this.mUVRect.yMax = mUVRect.yMax + (this.mSprite.paddingTop * mUVRect.height);
                    }
                    if (this.mSprite.hasPadding)
                    {
                        this.Trim();
                    }
                }
            }
            return this.mUVRect;
        }
        set
        {
            if (this.mReplacement != null)
            {
                this.mReplacement.uvRect = value;
            }
            else if ((this.sprite == null) && (this.mUVRect != value))
            {
                this.mUVRect = value;
                this.MarkAsDirty();
            }
        }
    }

    public int verticalSpacing
    {
        get
        {
            return ((this.mReplacement == null) ? this.mSpacingY : this.mReplacement.verticalSpacing);
        }
        set
        {
            if (this.mReplacement != null)
            {
                this.mReplacement.verticalSpacing = value;
            }
            else if (this.mSpacingY != value)
            {
                this.mSpacingY = value;
                this.MarkAsDirty();
            }
        }
    }

    public enum Alignment
    {
        Left,
        Center,
        Right,
        LeftOverflowRight
    }

    private class MangleSorter : Comparer<Vector3>
    {
        public double lineHeight = 12.0;
        private bool noLineSize;
        private bool noVSpacing;
        public double vSpacing = 12.0;

        public override int Compare(Vector3 x, Vector3 y)
        {
            int num;
            if (!this.noLineSize)
            {
                double d = ((double) x.y) / this.lineHeight;
                double num3 = ((double) y.y) / this.lineHeight;
                if (!this.noVSpacing)
                {
                    if ((d >= 1.0) || (d <= -1.0))
                    {
                        d = (x.y - this.lineHeight) / (this.lineHeight + this.vSpacing);
                    }
                    if ((num3 >= 1.0) || (num3 <= -1.0))
                    {
                        num3 = (y.y - this.lineHeight) / (this.lineHeight + this.vSpacing);
                    }
                }
                if (d < 0.0)
                {
                    d = -Math.Ceiling(-d);
                }
                else
                {
                    d = Math.Floor(d);
                }
                if (num3 < 0.0)
                {
                    num3 = -Math.Ceiling(-num3);
                }
                else
                {
                    num3 = Math.Floor(num3);
                }
                num = d.CompareTo(num3);
            }
            else
            {
                num = x.y.CompareTo(y.y);
            }
            if (num == 0)
            {
                num = x.x.CompareTo(y.x);
                if (num == 0)
                {
                    num = x.z.CompareTo(y.z);
                }
            }
            return num;
        }

        public void SetLineSizing(double height, double spacing)
        {
            if (height == 0.0)
            {
                if (spacing == 0.0)
                {
                    this.noLineSize = true;
                }
                else
                {
                    this.lineHeight = spacing;
                    this.noVSpacing = true;
                    this.noLineSize = false;
                }
            }
            else
            {
                this.lineHeight = height;
                if (spacing == 0.0)
                {
                    this.noVSpacing = true;
                    this.noLineSize = false;
                }
                else if (spacing == -height)
                {
                    this.noLineSize = true;
                    this.noVSpacing = true;
                }
                else
                {
                    this.vSpacing = spacing;
                    this.noLineSize = false;
                    this.noVSpacing = false;
                }
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct PrintContext
    {
        public MeshBuffer m;
        public BMGlyph glyph;
        public BMGlyph highlightGlyph;
        public BMGlyph carratGlyph;
        public BMSymbol symbol;
        public string text;
        public UIHighlight highlight;
        public Color printColor;
        public Color nonHighlightColor;
        public Color normalColor;
        public Color highlightTextColor;
        public Color highlightBarColor;
        public Vector3 v0;
        public Vector3 v1;
        public Vector2 u0;
        public Vector2 u1;
        public Vector2 scale;
        public float invX;
        public float invY;
        public float highlightSplit;
        public int x;
        public int maxX;
        public int previousX;
        public int y;
        public int highlightVertex;
        public int prev;
        public int lineHeight;
        public int lineWidth;
        public int indexOffset;
        public int textLength;
        public int i;
        public int carratIndex;
        public int j;
        public UIFont.Alignment alignment;
        public char carratChar;
        public char highlightChar;
        public char c;
        public bool highlightBarDraw;
        public bool isLineEnd;
        public bool skipSymbols;
        public bool printChar;
    }

    public enum SymbolStyle
    {
        None,
        Uncolored,
        Colored
    }
}

