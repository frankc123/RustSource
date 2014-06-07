using NGUI.Meshing;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[ExecuteInEditMode, AddComponentMenu("NGUI/UI/Label")]
public class UILabel : UIWidget
{
    private List<UITextMarkup> _markups;
    private Vector3? lastQueryPos;
    [HideInInspector, SerializeField]
    private char mCarratChar;
    [SerializeField, HideInInspector]
    private Color mEffectColor;
    [SerializeField, HideInInspector]
    private Effect mEffectStyle;
    [HideInInspector, SerializeField]
    private bool mEncoding;
    [SerializeField, HideInInspector]
    private UIFont mFont;
    [SerializeField, HideInInspector]
    private char mHighlightChar;
    [SerializeField, HideInInspector]
    private float mHighlightCharSplit;
    [SerializeField, HideInInspector]
    private Color mHighlightColor;
    [HideInInspector, SerializeField]
    private Color mHighlightTextColor;
    private bool mInvisibleHack;
    private Color mLastColor;
    private int mLastCount;
    private Effect mLastEffect;
    private bool mLastEncoding;
    private bool mLastInvisibleHack;
    private bool mLastPass;
    private Vector3 mLastScale;
    private bool mLastShow;
    private string mLastText;
    private int mLastWidth;
    [HideInInspector, SerializeField]
    private float mLineWidth;
    [SerializeField, HideInInspector]
    private int mMaxLineCount;
    [HideInInspector, SerializeField]
    private int mMaxLineWidth;
    [HideInInspector, SerializeField]
    private bool mMultiline;
    [SerializeField, HideInInspector]
    private bool mOverflowRight;
    [HideInInspector, SerializeField]
    private bool mPassword;
    private string mProcessedText;
    private UITextSelection mSelection;
    private bool mShouldBeProcessed;
    [SerializeField, HideInInspector]
    private bool mShowLastChar;
    private Vector3 mSize;
    [HideInInspector, SerializeField]
    private UIFont.SymbolStyle mSymbols;
    [SerializeField, HideInInspector]
    private string mText;

    public UILabel() : base(UIWidget.WidgetFlags.CustomMaterialGet | UIWidget.WidgetFlags.CustomRelativeSize)
    {
        this.mText = string.Empty;
        this.mEncoding = true;
        this.mEffectColor = Color.black;
        this.mSymbols = UIFont.SymbolStyle.Uncolored;
        this.mCarratChar = '|';
        this.mHighlightTextColor = Color.cyan;
        this.mHighlightColor = Color.black;
        this.mHighlightChar = '|';
        this.mHighlightCharSplit = 0.5f;
        this.mMultiline = true;
        this.mShouldBeProcessed = true;
        this.mLastScale = Vector3.one;
        this.mLastText = string.Empty;
        this.mLastEncoding = true;
        this.mLastColor = Color.black;
        this.mSize = Vector3.zero;
    }

    private void ApplyChanges()
    {
        this.mShouldBeProcessed = false;
        this.mLastText = this.text;
        this.mLastInvisibleHack = this.mInvisibleHack;
        this.mLastWidth = this.mMaxLineWidth;
        this.mLastEncoding = this.mEncoding;
        this.mLastCount = this.mMaxLineCount;
        this.mLastPass = this.mPassword;
        this.mLastShow = this.mShowLastChar;
        this.mLastEffect = this.mEffectStyle;
        this.mLastColor = this.mEffectColor;
    }

    public int CalculateTextPosition(Space space, Vector3[] points, UITextPosition[] positions)
    {
        int num;
        if (this.mFont == null)
        {
            return -1;
        }
        string processedText = this.processedText;
        if (space == Space.Self)
        {
            num = this.mFont.CalculatePlacement(points, positions, processedText);
        }
        else
        {
            num = this.mFont.CalculatePlacement(points, positions, processedText, base.cachedTransform.worldToLocalMatrix);
        }
        int markupCount = -1;
        for (int i = 0; i < num; i++)
        {
            this.ConvertProcessedTextPosition(ref positions[i], ref markupCount);
        }
        return num;
    }

    private void ConvertProcessedTextPosition(ref UITextPosition p, ref int markupCount)
    {
        if (markupCount == -1)
        {
            markupCount = this.markups.Count;
        }
        if (markupCount != 0)
        {
            UITextMarkup markup = this.markups[0];
            int num = 0;
            while (p.position <= markup.index)
            {
                switch (markup.mod)
                {
                    case UITextMod.End:
                        p.deformed = (short) (this.mText.Length - markup.index);
                        return;

                    case UITextMod.Removed:
                    {
                        p.deformed = (short) (p.deformed + 1);
                        if (++num >= markupCount)
                        {
                            return;
                        }
                        markup = this.markups[num];
                        continue;
                    }
                    case UITextMod.Replaced:
                    {
                        if (++num >= markupCount)
                        {
                            return;
                        }
                        markup = this.markups[num];
                        continue;
                    }
                    case UITextMod.Added:
                    {
                        p.deformed = (short) (p.deformed - 1);
                        if (++num >= markupCount)
                        {
                            return;
                        }
                        markup = this.markups[num];
                        continue;
                    }
                }
                if (++num >= markupCount)
                {
                    break;
                }
                markup = this.markups[num];
            }
        }
    }

    public UITextPosition ConvertUnprocessedPosition(int position)
    {
        UITextPosition position2;
        string processedText = this.processedText;
        int count = this.markups.Count;
        int num2 = position;
        if (count > 0)
        {
            int num3 = 0;
            for (UITextMarkup markup = this.markups[num3]; markup.index <= position; markup = this.markups[num3])
            {
                switch (markup.mod)
                {
                    case UITextMod.End:
                        position -= num2 - markup.index;
                        num3 = count;
                        break;

                    case UITextMod.Removed:
                        position--;
                        break;

                    case UITextMod.Added:
                        position++;
                        break;
                }
                if (++num3 >= count)
                {
                    break;
                }
            }
        }
        CountLinesGetColumn(processedText, position, out position2.position, out position2.line, out position2.column, out position2.region);
        position2.uniformRegion = position2.region;
        position2.deformed = (short) (num2 - position2.position);
        return position2;
    }

    public UITextSelection ConvertUnprocessedSelection(int carratPos, int selectPos)
    {
        UITextSelection selection;
        selection.carratPos = this.ConvertUnprocessedPosition(carratPos);
        if (carratPos == selectPos)
        {
            selection.selectPos = selection.carratPos;
            return selection;
        }
        selection.selectPos = this.ConvertUnprocessedPosition(selectPos);
        return selection;
    }

    private static void CountLinesGetColumn(string text, int inPos, out int pos, out int lines, out int column, out UITextRegion region)
    {
        if (inPos < 0)
        {
            region = UITextRegion.Before;
            pos = 0;
            lines = 0;
            column = 0;
        }
        else if (inPos == 0)
        {
            pos = 0;
            lines = 0;
            column = 0;
            region = UITextRegion.Pre;
        }
        else
        {
            if (inPos > text.Length)
            {
                region = UITextRegion.End;
                pos = text.Length;
            }
            else if (inPos == text.Length)
            {
                region = UITextRegion.Past;
                pos = inPos;
            }
            else
            {
                region = UITextRegion.Inside;
                pos = inPos;
            }
            int startIndex = text.IndexOf('\n', 0, pos);
            if (startIndex == -1)
            {
                lines = 0;
                column = pos;
            }
            else
            {
                int num2 = startIndex;
                lines = 1;
                while (++startIndex < pos)
                {
                    startIndex = text.IndexOf('\n', startIndex, pos - startIndex);
                    if (startIndex == -1)
                    {
                        break;
                    }
                    lines++;
                    num2 = startIndex;
                }
                column = pos - (num2 + 1);
            }
        }
    }

    private void ForceChanges()
    {
        base.ChangedAuto();
        this.mShouldBeProcessed = true;
    }

    protected override void GetCustomVector2s(int start, int end, UIWidget.WidgetFlags[] flags, Vector2[] v)
    {
        for (int i = 0; i < end; i++)
        {
            if (flags[i] == UIWidget.WidgetFlags.CustomRelativeSize)
            {
                v[i] = this.relativeSize;
            }
            else
            {
                base.GetCustomVector2s(i, i + 1, flags, v);
            }
        }
    }

    public void GetProcessedIndices(ref int start, ref int end)
    {
        int count = this.markups.Count;
        if (count != 0)
        {
            UITextMarkup markup = this.markups[0];
            if (markup.index <= end)
            {
                UITextMarkup markup3;
                UITextMarkup markup5;
                int num2 = start;
                int num3 = end;
                int num4 = 0;
            Label_00AA:
                markup3 = this.markups[num4];
                if (markup3.index <= start)
                {
                    UITextMarkup markup2 = this.markups[num4];
                    switch (markup2.mod)
                    {
                        case UITextMod.End:
                            num3 = this.mProcessedText.Length - 1;
                            return;

                        case UITextMod.Removed:
                            num2--;
                            num3--;
                            break;

                        case UITextMod.Added:
                            num2++;
                            num3++;
                            break;
                    }
                    if (++num4 >= count)
                    {
                        start = num2;
                        end = num3;
                        return;
                    }
                    goto Label_00AA;
                }
            Label_012F:
                markup5 = this.markups[num4];
                if (markup5.index <= end)
                {
                    UITextMarkup markup4 = this.markups[num4];
                    switch (markup4.mod)
                    {
                        case UITextMod.End:
                            num3 = this.mProcessedText.Length - 1;
                            return;

                        case UITextMod.Removed:
                            num3--;
                            break;

                        case UITextMod.Added:
                            num3++;
                            break;
                    }
                    if (++num4 < count)
                    {
                        goto Label_012F;
                    }
                }
                start = num2;
                end = num3;
            }
        }
    }

    public override void MakePixelPerfect()
    {
        if (this.mFont != null)
        {
            float num = (this.font.atlas == null) ? 1f : this.font.atlas.pixelSize;
            Vector3 localScale = base.cachedTransform.localScale;
            localScale.x = this.mFont.size * num;
            localScale.y = localScale.x;
            localScale.z = 1f;
            Vector2 vector2 = (Vector2) (this.relativeSize * localScale.x);
            int num2 = Mathf.RoundToInt(vector2.x / num);
            int num3 = Mathf.RoundToInt(vector2.y / num);
            Vector3 localPosition = base.cachedTransform.localPosition;
            localPosition.x = Mathf.FloorToInt(localPosition.x / num);
            localPosition.y = Mathf.CeilToInt(localPosition.y / num);
            localPosition.z = Mathf.RoundToInt(localPosition.z);
            if (base.cachedTransform.localRotation == Quaternion.identity)
            {
                if (((num2 % 2) == 1) && (((base.pivot == UIWidget.Pivot.Top) || (base.pivot == UIWidget.Pivot.Center)) || (base.pivot == UIWidget.Pivot.Bottom)))
                {
                    localPosition.x += 0.5f;
                }
                if (((num3 % 2) == 1) && (((base.pivot == UIWidget.Pivot.Left) || (base.pivot == UIWidget.Pivot.Center)) || (base.pivot == UIWidget.Pivot.Right)))
                {
                    localPosition.y -= 0.5f;
                }
            }
            localPosition.x *= num;
            localPosition.y *= num;
            base.cachedTransform.localPosition = localPosition;
            base.cachedTransform.localScale = localScale;
        }
        else
        {
            base.MakePixelPerfect();
        }
    }

    public void MakePositionPerfect()
    {
        float num = (this.font.atlas == null) ? 1f : this.font.atlas.pixelSize;
        Vector3 localScale = base.cachedTransform.localScale;
        if (((this.mFont.size == Mathf.RoundToInt(localScale.x / num)) && (this.mFont.size == Mathf.RoundToInt(localScale.y / num))) && (base.cachedTransform.localRotation == Quaternion.identity))
        {
            Vector2 vector2 = (Vector2) (this.relativeSize * localScale.x);
            int num2 = Mathf.RoundToInt(vector2.x / num);
            int num3 = Mathf.RoundToInt(vector2.y / num);
            Vector3 localPosition = base.cachedTransform.localPosition;
            localPosition.x = Mathf.FloorToInt(localPosition.x / num);
            localPosition.y = Mathf.CeilToInt(localPosition.y / num);
            localPosition.z = Mathf.RoundToInt(localPosition.z);
            if (((num2 % 2) == 1) && (((base.pivot == UIWidget.Pivot.Top) || (base.pivot == UIWidget.Pivot.Center)) || (base.pivot == UIWidget.Pivot.Bottom)))
            {
                localPosition.x += 0.5f;
            }
            if (((num3 % 2) == 1) && (((base.pivot == UIWidget.Pivot.Left) || (base.pivot == UIWidget.Pivot.Center)) || (base.pivot == UIWidget.Pivot.Right)))
            {
                localPosition.y -= 0.5f;
            }
            localPosition.x *= num;
            localPosition.y *= num;
            if (base.cachedTransform.localPosition != localPosition)
            {
                base.cachedTransform.localPosition = localPosition;
            }
        }
    }

    public override void MarkAsChanged()
    {
        this.ForceChanges();
        base.MarkAsChanged();
    }

    public override void OnFill(MeshBuffer m)
    {
        if (this.mFont != null)
        {
            Color normalColor = !this.mInvisibleHack ? base.color : Color.clear;
            this.MakePositionPerfect();
            UIWidget.Pivot pivot = base.pivot;
            int vSize = m.vSize;
            switch (pivot)
            {
                case UIWidget.Pivot.Left:
                case UIWidget.Pivot.TopLeft:
                case UIWidget.Pivot.BottomLeft:
                    if (this.mOverflowRight)
                    {
                        this.mFont.Print(this.processedText, normalColor, m, this.mEncoding, this.mSymbols, UIFont.Alignment.LeftOverflowRight, Mathf.RoundToInt(this.relativeSize.x * this.mFont.size), ref this.mSelection, this.mCarratChar, this.mHighlightTextColor, this.mHighlightColor, this.mHighlightChar, this.mHighlightCharSplit);
                    }
                    else
                    {
                        this.mFont.Print(this.processedText, normalColor, m, this.mEncoding, this.mSymbols, UIFont.Alignment.Left, 0, ref this.mSelection, this.mCarratChar, this.mHighlightTextColor, this.mHighlightColor, this.mHighlightChar, this.mHighlightCharSplit);
                    }
                    break;

                case UIWidget.Pivot.Right:
                case UIWidget.Pivot.TopRight:
                case UIWidget.Pivot.BottomRight:
                    this.mFont.Print(this.processedText, normalColor, m, this.mEncoding, this.mSymbols, UIFont.Alignment.Right, Mathf.RoundToInt(this.relativeSize.x * this.mFont.size), ref this.mSelection, this.mCarratChar, this.mHighlightTextColor, this.mHighlightColor, this.mHighlightChar, this.mHighlightCharSplit);
                    break;

                default:
                    this.mFont.Print(this.processedText, normalColor, m, this.mEncoding, this.mSymbols, UIFont.Alignment.Center, Mathf.RoundToInt(this.relativeSize.x * this.mFont.size), ref this.mSelection, this.mCarratChar, this.mHighlightTextColor, this.mHighlightColor, this.mHighlightChar, this.mHighlightCharSplit);
                    break;
            }
            m.ApplyEffect(base.cachedTransform, vSize, this.effectStyle, this.effectColor, (float) this.mFont.size);
        }
    }

    protected override void OnStart()
    {
        if (this.mLineWidth > 0f)
        {
            this.mMaxLineWidth = Mathf.RoundToInt(this.mLineWidth);
            this.mLineWidth = 0f;
        }
        if (!this.mMultiline)
        {
            this.mMaxLineCount = 1;
            this.mMultiline = true;
        }
    }

    private bool PendingChanges()
    {
        return (((((this.mShouldBeProcessed || (this.mLastText != this.text)) || ((this.mInvisibleHack != this.mLastInvisibleHack) || (this.mLastWidth != this.mMaxLineWidth))) || (((this.mLastEncoding != this.mEncoding) || (this.mLastCount != this.mMaxLineCount)) || ((this.mLastPass != this.mPassword) || (this.mLastShow != this.mShowLastChar)))) || (this.mLastEffect != this.mEffectStyle)) || (this.mLastColor != this.mEffectColor));
    }

    private void ProcessText()
    {
        base.ChangedAuto();
        this.mLastText = this.mText;
        this.markups.Clear();
        string mProcessedText = this.mProcessedText;
        this.mProcessedText = this.mText;
        if (this.mPassword)
        {
            this.mProcessedText = this.mFont.WrapText(this.markups, this.mProcessedText, 100000f, 1, false, UIFont.SymbolStyle.None);
            string str2 = string.Empty;
            if (this.mShowLastChar)
            {
                int num = 1;
                int length = this.mProcessedText.Length;
                while (num < length)
                {
                    str2 = str2 + "*";
                    num++;
                }
                if (this.mProcessedText.Length > 0)
                {
                    str2 = str2 + this.mProcessedText[this.mProcessedText.Length - 1];
                }
            }
            else
            {
                int num3 = 0;
                int num4 = this.mProcessedText.Length;
                while (num3 < num4)
                {
                    str2 = str2 + "*";
                    num3++;
                }
            }
            this.mProcessedText = str2;
        }
        else if (this.mMaxLineWidth > 0)
        {
            this.mProcessedText = this.mFont.WrapText(this.markups, this.mProcessedText, ((float) this.mMaxLineWidth) / base.cachedTransform.localScale.x, this.mMaxLineCount, this.mEncoding, this.mSymbols);
        }
        else if (this.mMaxLineCount > 0)
        {
            this.mProcessedText = this.mFont.WrapText(this.markups, this.mProcessedText, 100000f, this.mMaxLineCount, this.mEncoding, this.mSymbols);
        }
        this.mSize = string.IsNullOrEmpty(this.mProcessedText) ? ((Vector3) Vector2.one) : ((Vector3) this.mFont.CalculatePrintedSize(this.mProcessedText, this.mEncoding, this.mSymbols));
        float x = base.cachedTransform.localScale.x;
        this.mSize.x = Mathf.Max(this.mSize.x, ((this.mFont == null) || (x <= 1f)) ? 1f : (((float) this.lineWidth) / x));
        this.mSize.y = Mathf.Max(this.mSize.y, 1f);
        if (mProcessedText != this.mProcessedText)
        {
            this.mSelection = new UITextSelection();
        }
        this.ApplyChanges();
    }

    public void UnionProcessedChanges(string newProcessedText)
    {
        this.text = newProcessedText;
    }

    public char carratChar
    {
        get
        {
            return this.mCarratChar;
        }
        set
        {
            if (this.mCarratChar != value)
            {
                bool shouldShowCarrat = this.shouldShowCarrat;
                this.mCarratChar = value;
                if (shouldShowCarrat)
                {
                    this.ForceChanges();
                }
            }
        }
    }

    public UITextPosition carratPosition
    {
        get
        {
            return this.mSelection.carratPos;
        }
    }

    private bool carratWouldBeVisibleIfOn
    {
        get
        {
            return (this.mCarratChar != '\0');
        }
    }

    protected override UIMaterial customMaterial
    {
        get
        {
            return this.material;
        }
    }

    public bool drawingCarrat
    {
        get
        {
            return ((this.mCarratChar != '\0') && this.mSelection.showCarrat);
        }
    }

    public Color effectColor
    {
        get
        {
            return this.mEffectColor;
        }
        set
        {
            if (this.mEffectColor != value)
            {
                this.mEffectColor = value;
                if (this.mEffectStyle != Effect.None)
                {
                    this.ForceChanges();
                }
            }
        }
    }

    public Effect effectStyle
    {
        get
        {
            return this.mEffectStyle;
        }
        set
        {
            if (this.mEffectStyle != value)
            {
                this.mEffectStyle = value;
                this.ForceChanges();
            }
        }
    }

    public UIFont font
    {
        get
        {
            return this.mFont;
        }
        set
        {
            if (this.mFont != value)
            {
                this.mFont = value;
                base.baseMaterial = (this.mFont == null) ? null : ((UIMaterial) this.mFont.material);
                base.ChangedAuto();
                this.ForceChanges();
                this.MarkAsChanged();
            }
        }
    }

    public bool hasSelection
    {
        get
        {
            return this.mSelection.hasSelection;
        }
    }

    public char highlightChar
    {
        get
        {
            return this.mHighlightChar;
        }
        set
        {
            if (this.mHighlightChar != value)
            {
                bool flag = this.hasSelection && (this.mHighlightColor.a > 0f);
                this.mHighlightChar = value;
                if (flag)
                {
                    this.ForceChanges();
                }
            }
        }
    }

    public float highlightCharSplit
    {
        get
        {
            return this.mHighlightCharSplit;
        }
        set
        {
            if (value > 1f)
            {
                value = 1f;
            }
            else if (value < 0f)
            {
                value = 0f;
            }
            if (this.mHighlightCharSplit != value)
            {
                bool flag = (this.hasSelection && (this.mHighlightColor.a > 0f)) && (this.mHighlightChar != '\0');
                this.mHighlightCharSplit = value;
                if (flag)
                {
                    this.ForceChanges();
                }
            }
        }
    }

    public Color highlightColor
    {
        get
        {
            return this.mHighlightColor;
        }
        set
        {
            if (this.mHighlightColor != value)
            {
                bool flag = ((this.hasSelection && (this.mHighlightChar != '\0')) && (this.mHighlightColor.a > 0f)) || (value.a > 0f);
                this.mHighlightColor = value;
                if (flag)
                {
                    this.ForceChanges();
                }
            }
        }
    }

    public Color highlightTextColor
    {
        get
        {
            return this.mHighlightTextColor;
        }
        set
        {
            if (this.mHighlightTextColor != value)
            {
                bool flag = (this.hasSelection && (this.mHighlightColor.a > 0f)) || (value.a > 0f);
                this.mHighlightTextColor = value;
                if (flag)
                {
                    this.ForceChanges();
                }
            }
        }
    }

    private bool highlightWouldBeVisibleIfOn
    {
        get
        {
            return (((this.mHighlightChar != '\0') && (this.mHighlightColor.a > 0f)) || (this.mHighlightTextColor != base.color));
        }
    }

    public bool invisibleHack
    {
        get
        {
            return this.mInvisibleHack;
        }
        set
        {
            if (this.mInvisibleHack != value)
            {
                this.mInvisibleHack = value;
                this.ForceChanges();
            }
        }
    }

    public int lineWidth
    {
        get
        {
            return this.mMaxLineWidth;
        }
        set
        {
            if (this.mMaxLineWidth != value)
            {
                this.mMaxLineWidth = value;
                this.ForceChanges();
            }
        }
    }

    private List<UITextMarkup> markups
    {
        get
        {
            if (this._markups == null)
            {
            }
            return (this._markups = new List<UITextMarkup>());
        }
    }

    public UIMaterial material
    {
        get
        {
            UIMaterial baseMaterial = base.baseMaterial;
            if (baseMaterial == null)
            {
                baseMaterial = (this.mFont == null) ? null : ((UIMaterial) this.mFont.material);
                this.material = baseMaterial;
            }
            return baseMaterial;
        }
        set
        {
            base.material = value;
        }
    }

    public int maxLineCount
    {
        get
        {
            return this.mMaxLineCount;
        }
        set
        {
            if (this.mMaxLineCount != value)
            {
                this.mMaxLineCount = Mathf.Max(value, 0);
                this.ForceChanges();
                if (value == 1)
                {
                    this.mPassword = false;
                }
            }
        }
    }

    public bool multiLine
    {
        get
        {
            return (this.mMaxLineCount != 1);
        }
        set
        {
            if ((this.mMaxLineCount != 1) != value)
            {
                this.mMaxLineCount = !value ? 1 : 0;
                this.ForceChanges();
                if (value)
                {
                    this.mPassword = false;
                }
            }
        }
    }

    public bool overflowRight
    {
        get
        {
            return this.mOverflowRight;
        }
        set
        {
            if (this.mOverflowRight != value)
            {
                this.mOverflowRight = value;
                switch (base.pivot)
                {
                    case UIWidget.Pivot.TopLeft:
                    case UIWidget.Pivot.Left:
                    case UIWidget.Pivot.BottomLeft:
                        this.ForceChanges();
                        return;
                }
            }
        }
    }

    public bool password
    {
        get
        {
            return this.mPassword;
        }
        set
        {
            if (this.mPassword != value)
            {
                this.mPassword = value;
                this.mMaxLineCount = 1;
                this.mEncoding = false;
                this.ForceChanges();
            }
        }
    }

    public string processedText
    {
        get
        {
            if (this.mLastScale != base.cachedTransform.localScale)
            {
                this.mLastScale = base.cachedTransform.localScale;
                this.mShouldBeProcessed = true;
            }
            if (this.PendingChanges())
            {
                this.ProcessText();
            }
            return this.mProcessedText;
        }
    }

    public Vector2 relativeSize
    {
        get
        {
            if (this.mFont == null)
            {
                return Vector3.one;
            }
            if (this.PendingChanges())
            {
                this.ProcessText();
            }
            return this.mSize;
        }
    }

    public UITextSelection selection
    {
        get
        {
            return this.mSelection;
        }
        set
        {
            UITextSelection.Change changesTo = this.mSelection.GetChangesTo(ref value);
            this.mSelection = value;
            switch (changesTo)
            {
                case UITextSelection.Change.NoneToCarrat:
                case UITextSelection.Change.CarratMove:
                case UITextSelection.Change.CarratToNone:
                    if (this.carratWouldBeVisibleIfOn)
                    {
                        this.ForceChanges();
                    }
                    break;

                case UITextSelection.Change.CarratToSelection:
                case UITextSelection.Change.SelectionToCarrat:
                    if (this.carratWouldBeVisibleIfOn || this.highlightWouldBeVisibleIfOn)
                    {
                        this.ForceChanges();
                    }
                    break;

                case UITextSelection.Change.SelectionAdjusted:
                case UITextSelection.Change.NoneToSelection:
                case UITextSelection.Change.SelectionToNone:
                    if (this.highlightWouldBeVisibleIfOn)
                    {
                        this.ForceChanges();
                    }
                    break;
            }
        }
    }

    public UITextPosition selectPosition
    {
        get
        {
            return this.mSelection.selectPos;
        }
    }

    public bool shouldShowCarrat
    {
        get
        {
            return this.mSelection.showCarrat;
        }
    }

    public bool showLastPasswordChar
    {
        get
        {
            return this.mShowLastChar;
        }
        set
        {
            if (this.mShowLastChar != value)
            {
                this.mShowLastChar = value;
                this.ForceChanges();
            }
        }
    }

    public bool supportEncoding
    {
        get
        {
            return this.mEncoding;
        }
        set
        {
            if (this.mEncoding != value)
            {
                this.mEncoding = value;
                this.ForceChanges();
                if (value)
                {
                    this.mPassword = false;
                }
            }
        }
    }

    public UIFont.SymbolStyle symbolStyle
    {
        get
        {
            return this.mSymbols;
        }
        set
        {
            if (this.mSymbols != value)
            {
                this.mSymbols = value;
                this.ForceChanges();
            }
        }
    }

    public string text
    {
        get
        {
            return this.mText;
        }
        set
        {
            if ((value != null) && (this.mText != value))
            {
                this.mText = value;
                this.ForceChanges();
            }
        }
    }

    public enum Effect
    {
        None,
        Shadow,
        Outline
    }
}

