using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

[Serializable, RequireComponent(typeof(BoxCollider)), AddComponentMenu("Daikon Forge/User Interface/Textbox"), ExecuteInEditMode]
public class dfTextbox : dfInteractiveBase, IDFMultiRender
{
    [SerializeField]
    protected bool acceptsTab;
    private dfList<dfRenderData> buffers = dfList<dfRenderData>.Obtain();
    private float[] charWidths;
    [SerializeField]
    protected float cursorBlinkTime = 0.45f;
    private int cursorIndex;
    private bool cursorShown;
    [SerializeField]
    protected int cursorWidth = 1;
    [SerializeField]
    protected bool displayAsPassword;
    [SerializeField]
    protected dfFontBase font;
    private float leftOffset;
    [SerializeField]
    protected int maxLength = 0x400;
    [SerializeField]
    protected bool mobileAutoCorrect;
    [SerializeField]
    protected bool mobileHideInputField;
    [SerializeField]
    protected dfMobileKeyboardTrigger mobileKeyboardTrigger;
    [SerializeField]
    protected int mobileKeyboardType;
    private int mouseSelectionAnchor;
    [SerializeField]
    protected RectOffset padding = new RectOffset();
    [SerializeField]
    protected string passwordChar = "*";
    [SerializeField]
    protected bool readOnly;
    private int scrollIndex;
    [SerializeField]
    protected Color32 selectionBackground = new Color32(0, 0x69, 210, 0xff);
    private int selectionEnd;
    [SerializeField]
    protected string selectionSprite = string.Empty;
    private int selectionStart;
    [SerializeField]
    protected bool selectOnFocus;
    [SerializeField]
    protected bool shadow;
    [SerializeField]
    protected Color32 shadowColor = Color.black;
    [SerializeField]
    protected Vector2 shadowOffset = new Vector2(1f, -1f);
    private Vector2 startSize = Vector2.zero;
    [SerializeField]
    protected string text = string.Empty;
    [SerializeField]
    protected UnityEngine.TextAlignment textAlign;
    [SerializeField]
    protected Color32 textColor = Color.white;
    private dfRenderData textRenderData;
    [SerializeField]
    protected float textScale = 1f;
    [SerializeField]
    protected dfTextScaleMode textScaleMode;
    private string undoText = string.Empty;
    [SerializeField]
    protected bool useMobileKeyboard;
    private float whenGotFocus;

    public event PropertyChangedEventHandler<string> PasswordCharacterChanged;

    public event PropertyChangedEventHandler<bool> ReadOnlyChanged;

    public event PropertyChangedEventHandler<string> TextCancelled;

    public event PropertyChangedEventHandler<string> TextChanged;

    public event PropertyChangedEventHandler<string> TextSubmitted;

    private void addQuadIndices(dfList<Vector3> verts, dfList<int> triangles)
    {
        int count = verts.Count;
        int[] numArray = new int[] { 0, 1, 3, 3, 1, 2 };
        for (int i = 0; i < numArray.Length; i++)
        {
            triangles.Add(count + numArray[i]);
        }
    }

    public override void Awake()
    {
        base.Awake();
        this.startSize = base.Size;
    }

    public void ClearSelection()
    {
        this.selectionStart = 0;
        this.selectionEnd = 0;
        this.mouseSelectionAnchor = 0;
    }

    private void copySelectionToClipboard()
    {
        if (this.selectionStart != this.selectionEnd)
        {
            dfClipboardHelper.clipBoard = this.text.Substring(this.selectionStart, this.selectionEnd - this.selectionStart);
        }
    }

    private void cutSelectionToClipboard()
    {
        this.copySelectionToClipboard();
        this.deleteSelection();
    }

    private void deleteNextChar()
    {
        this.ClearSelection();
        if (this.cursorIndex < this.text.Length)
        {
            this.text = this.text.Remove(this.cursorIndex, 1);
            this.cursorShown = true;
            this.OnTextChanged();
            this.Invalidate();
        }
    }

    private void deleteNextWord()
    {
        this.ClearSelection();
        if (this.cursorIndex != this.text.Length)
        {
            int length = this.findNextWord(this.cursorIndex);
            if (length == this.cursorIndex)
            {
                length = this.text.Length;
            }
            this.text = this.text.Remove(this.cursorIndex, length - this.cursorIndex);
            this.OnTextChanged();
            this.Invalidate();
        }
    }

    private void deletePreviousChar()
    {
        if (this.selectionStart != this.selectionEnd)
        {
            int selectionStart = this.selectionStart;
            this.deleteSelection();
            this.setCursorPos(selectionStart);
        }
        else
        {
            this.ClearSelection();
            if (this.cursorIndex != 0)
            {
                this.text = this.text.Remove(this.cursorIndex - 1, 1);
                this.cursorIndex--;
                this.cursorShown = true;
                this.OnTextChanged();
                this.Invalidate();
            }
        }
    }

    private void deletePreviousWord()
    {
        this.ClearSelection();
        if (this.cursorIndex != 0)
        {
            int startIndex = this.findPreviousWord(this.cursorIndex);
            if (startIndex == this.cursorIndex)
            {
                startIndex = 0;
            }
            this.text = this.text.Remove(startIndex, this.cursorIndex - startIndex);
            this.setCursorPos(startIndex);
            this.OnTextChanged();
            this.Invalidate();
        }
    }

    private void deleteSelection()
    {
        if (this.selectionStart != this.selectionEnd)
        {
            this.text = this.text.Remove(this.selectionStart, this.selectionEnd - this.selectionStart);
            this.setCursorPos(this.selectionStart);
            this.ClearSelection();
            this.OnTextChanged();
            this.Invalidate();
        }
    }

    [DebuggerHidden]
    private IEnumerator doCursorBlink()
    {
        return new <doCursorBlink>c__Iterator44 { <>f__this = this };
    }

    private int findNextWord(int startIndex)
    {
        int length = this.text.Length;
        int num2 = startIndex;
        for (int i = num2; i < length; i++)
        {
            char c = this.text[i];
            if ((char.IsWhiteSpace(c) || char.IsSeparator(c)) || char.IsPunctuation(c))
            {
                num2 = i;
                break;
            }
        }
        while (num2 < length)
        {
            char ch2 = this.text[num2];
            if ((!char.IsWhiteSpace(ch2) && !char.IsSeparator(ch2)) && !char.IsPunctuation(ch2))
            {
                return num2;
            }
            num2++;
        }
        return num2;
    }

    private int findPreviousWord(int startIndex)
    {
        int num = startIndex;
        while (num > 0)
        {
            char c = this.text[num - 1];
            if ((!char.IsWhiteSpace(c) && !char.IsSeparator(c)) && !char.IsPunctuation(c))
            {
                break;
            }
            num--;
        }
        for (int i = num; i >= 0; i--)
        {
            if (i == 0)
            {
                return 0;
            }
            char ch2 = this.text[i - 1];
            if ((char.IsWhiteSpace(ch2) || char.IsSeparator(ch2)) || char.IsPunctuation(ch2))
            {
                return i;
            }
        }
        return num;
    }

    private int getCharIndexOfMouse(dfMouseEventArgs args)
    {
        Vector2 hitPosition = base.GetHitPosition(args);
        float num = base.PixelsToUnits();
        int scrollIndex = this.scrollIndex;
        float num3 = this.leftOffset / num;
        for (int i = this.scrollIndex; i < this.charWidths.Length; i++)
        {
            num3 += this.charWidths[i] / num;
            if (num3 < hitPosition.x)
            {
                scrollIndex++;
            }
        }
        return scrollIndex;
    }

    private float getTextScaleMultiplier()
    {
        if ((this.textScaleMode == dfTextScaleMode.None) || !Application.isPlaying)
        {
            return 1f;
        }
        if (this.textScaleMode == dfTextScaleMode.ScreenResolution)
        {
            return (((float) Screen.height) / ((float) base.manager.FixedHeight));
        }
        return (base.Size.y / this.startSize.y);
    }

    private void moveSelectionPointLeft()
    {
        if (this.cursorIndex != 0)
        {
            if (this.selectionEnd == this.selectionStart)
            {
                this.selectionEnd = this.cursorIndex;
                this.selectionStart = this.cursorIndex - 1;
            }
            else if (this.selectionEnd == this.cursorIndex)
            {
                this.selectionEnd--;
            }
            else if (this.selectionStart == this.cursorIndex)
            {
                this.selectionStart--;
            }
            this.setCursorPos(this.cursorIndex - 1);
        }
    }

    private void moveSelectionPointLeftWord()
    {
        if (this.cursorIndex != 0)
        {
            int index = this.findPreviousWord(this.cursorIndex);
            if (this.selectionEnd == this.selectionStart)
            {
                this.selectionEnd = this.cursorIndex;
                this.selectionStart = index;
            }
            else if (this.selectionEnd == this.cursorIndex)
            {
                this.selectionEnd = index;
            }
            else if (this.selectionStart == this.cursorIndex)
            {
                this.selectionStart = index;
            }
            this.setCursorPos(index);
        }
    }

    private void moveSelectionPointRight()
    {
        if (this.cursorIndex != this.text.Length)
        {
            if (this.selectionEnd == this.selectionStart)
            {
                this.selectionEnd = this.cursorIndex + 1;
                this.selectionStart = this.cursorIndex;
            }
            else if (this.selectionEnd == this.cursorIndex)
            {
                this.selectionEnd++;
            }
            else if (this.selectionStart == this.cursorIndex)
            {
                this.selectionStart++;
            }
            this.setCursorPos(this.cursorIndex + 1);
        }
    }

    private void moveSelectionPointRightWord()
    {
        if (this.cursorIndex != this.text.Length)
        {
            int index = this.findNextWord(this.cursorIndex);
            if (this.selectionEnd == this.selectionStart)
            {
                this.selectionStart = this.cursorIndex;
                this.selectionEnd = index;
            }
            else if (this.selectionEnd == this.cursorIndex)
            {
                this.selectionEnd = index;
            }
            else if (this.selectionStart == this.cursorIndex)
            {
                this.selectionStart = index;
            }
            this.setCursorPos(index);
        }
    }

    private void moveToEnd()
    {
        this.ClearSelection();
        this.setCursorPos(this.text.Length);
    }

    private void moveToNextChar()
    {
        this.ClearSelection();
        this.setCursorPos(this.cursorIndex + 1);
    }

    private void moveToNextWord()
    {
        this.ClearSelection();
        if (this.cursorIndex != this.text.Length)
        {
            int index = this.findNextWord(this.cursorIndex);
            this.setCursorPos(index);
        }
    }

    private void moveToPreviousChar()
    {
        this.ClearSelection();
        this.setCursorPos(this.cursorIndex - 1);
    }

    private void moveToPreviousWord()
    {
        this.ClearSelection();
        if (this.cursorIndex != 0)
        {
            int index = this.findPreviousWord(this.cursorIndex);
            this.setCursorPos(index);
        }
    }

    private void moveToStart()
    {
        this.ClearSelection();
        this.setCursorPos(0);
    }

    protected internal virtual void OnCancel()
    {
        this.text = this.undoText;
        object[] args = new object[] { this, this.text };
        base.SignalHierarchy("OnTextCancelled", args);
        if (this.TextCancelled != null)
        {
            this.TextCancelled(this, this.text);
        }
    }

    protected internal override void OnDoubleClick(dfMouseEventArgs args)
    {
        if (args.Source != this)
        {
            base.OnDoubleClick(args);
        }
        else
        {
            if ((!this.ReadOnly && this.HasFocus) && (args.Buttons.IsSet(dfMouseButtons.Left) && ((Time.realtimeSinceStartup - this.whenGotFocus) > 0.5f)))
            {
                int index = this.getCharIndexOfMouse(args);
                this.selectWordAtIndex(index);
            }
            base.OnDoubleClick(args);
        }
    }

    public override void OnEnable()
    {
        if (this.padding == null)
        {
            this.padding = new RectOffset();
        }
        base.OnEnable();
        if (this.size.magnitude == 0f)
        {
            base.Size = new Vector2(100f, 20f);
        }
        this.cursorShown = false;
        this.cursorIndex = this.scrollIndex = 0;
        bool flag = (this.Font != null) && this.Font.IsValid;
        if (Application.isPlaying && !flag)
        {
            this.Font = base.GetManager().DefaultFont;
        }
    }

    protected internal override void OnEnterFocus(dfFocusEventArgs args)
    {
        base.OnEnterFocus(args);
        this.undoText = this.Text;
        if (!this.ReadOnly)
        {
            this.whenGotFocus = Time.realtimeSinceStartup;
            base.StartCoroutine(this.doCursorBlink());
            if (this.selectOnFocus)
            {
                this.selectionStart = 0;
                this.selectionEnd = this.text.Length;
            }
            else
            {
                this.selectionStart = this.selectionEnd = 0;
            }
        }
        this.Invalidate();
    }

    protected internal override void OnKeyDown(dfKeyEventArgs args)
    {
        if (!this.ReadOnly)
        {
            base.OnKeyDown(args);
            if (!args.Used)
            {
                KeyCode keyCode = args.KeyCode;
                switch (keyCode)
                {
                    case KeyCode.RightArrow:
                        if (!args.Control)
                        {
                            if (args.Shift)
                            {
                                this.moveSelectionPointRight();
                            }
                            else
                            {
                                this.moveToNextChar();
                            }
                        }
                        else if (!args.Shift)
                        {
                            this.moveToNextWord();
                        }
                        else
                        {
                            this.moveSelectionPointRightWord();
                        }
                        break;

                    case KeyCode.LeftArrow:
                        if (!args.Control)
                        {
                            if (args.Shift)
                            {
                                this.moveSelectionPointLeft();
                            }
                            else
                            {
                                this.moveToPreviousChar();
                            }
                        }
                        else if (!args.Shift)
                        {
                            this.moveToPreviousWord();
                        }
                        else
                        {
                            this.moveSelectionPointLeftWord();
                        }
                        break;

                    case KeyCode.Insert:
                        if (args.Shift)
                        {
                            string clipBoard = dfClipboardHelper.clipBoard;
                            if (!string.IsNullOrEmpty(clipBoard))
                            {
                                this.pasteAtCursor(clipBoard);
                            }
                        }
                        break;

                    case KeyCode.Home:
                        if (!args.Shift)
                        {
                            this.moveToStart();
                        }
                        else
                        {
                            this.selectToStart();
                        }
                        break;

                    case KeyCode.End:
                        if (!args.Shift)
                        {
                            this.moveToEnd();
                        }
                        else
                        {
                            this.selectToEnd();
                        }
                        break;

                    case KeyCode.A:
                        if (args.Control)
                        {
                            this.selectAll();
                        }
                        break;

                    case KeyCode.C:
                        if (args.Control)
                        {
                            this.copySelectionToClipboard();
                        }
                        break;

                    case KeyCode.V:
                        if (args.Control)
                        {
                            string str2 = dfClipboardHelper.clipBoard;
                            if (!string.IsNullOrEmpty(str2))
                            {
                                this.pasteAtCursor(str2);
                            }
                        }
                        break;

                    case KeyCode.X:
                        if (args.Control)
                        {
                            this.cutSelectionToClipboard();
                        }
                        break;

                    default:
                        switch (keyCode)
                        {
                            case KeyCode.Backspace:
                                if (args.Control)
                                {
                                    this.deletePreviousWord();
                                }
                                else
                                {
                                    this.deletePreviousChar();
                                }
                                break;

                            case KeyCode.Return:
                                this.OnSubmit();
                                break;

                            case KeyCode.Escape:
                                this.ClearSelection();
                                this.cursorIndex = this.scrollIndex = 0;
                                this.Invalidate();
                                this.OnCancel();
                                break;

                            case KeyCode.Delete:
                                if (this.selectionStart != this.selectionEnd)
                                {
                                    this.deleteSelection();
                                }
                                else if (args.Control)
                                {
                                    this.deleteNextWord();
                                }
                                else
                                {
                                    this.deleteNextChar();
                                }
                                break;

                            default:
                                base.OnKeyDown(args);
                                return;
                        }
                        break;
                }
                args.Use();
            }
        }
    }

    protected internal override void OnKeyPress(dfKeyEventArgs args)
    {
        if (this.ReadOnly || char.IsControl(args.Character))
        {
            base.OnKeyPress(args);
        }
        else
        {
            base.OnKeyPress(args);
            if (!args.Used)
            {
                this.processKeyPress(args);
            }
        }
    }

    protected internal override void OnLeaveFocus(dfFocusEventArgs args)
    {
        base.OnLeaveFocus(args);
        this.cursorShown = false;
        this.ClearSelection();
        this.Invalidate();
        this.whenGotFocus = 0f;
    }

    protected internal override void OnMouseDown(dfMouseEventArgs args)
    {
        if (args.Source != this)
        {
            base.OnMouseDown(args);
        }
        else
        {
            if ((!this.ReadOnly && args.Buttons.IsSet(dfMouseButtons.Left)) && ((!this.HasFocus && !this.SelectOnFocus) || ((Time.realtimeSinceStartup - this.whenGotFocus) > 0.25f)))
            {
                int num = this.getCharIndexOfMouse(args);
                if (num != this.cursorIndex)
                {
                    this.cursorIndex = num;
                    this.cursorShown = true;
                    this.Invalidate();
                    args.Use();
                }
                this.mouseSelectionAnchor = this.cursorIndex;
                this.selectionStart = this.selectionEnd = this.cursorIndex;
            }
            base.OnMouseDown(args);
        }
    }

    protected internal override void OnMouseMove(dfMouseEventArgs args)
    {
        if (args.Source != this)
        {
            base.OnMouseMove(args);
        }
        else
        {
            if ((!this.ReadOnly && this.HasFocus) && args.Buttons.IsSet(dfMouseButtons.Left))
            {
                int b = this.getCharIndexOfMouse(args);
                if (b != this.cursorIndex)
                {
                    this.cursorIndex = b;
                    this.cursorShown = true;
                    this.Invalidate();
                    args.Use();
                    this.selectionStart = Mathf.Min(this.mouseSelectionAnchor, b);
                    this.selectionEnd = Mathf.Max(this.mouseSelectionAnchor, b);
                    return;
                }
            }
            base.OnMouseMove(args);
        }
    }

    protected internal virtual void OnPasswordCharacterChanged()
    {
        if (this.PasswordCharacterChanged != null)
        {
            this.PasswordCharacterChanged(this, this.passwordChar);
        }
    }

    protected internal virtual void OnReadOnlyChanged()
    {
        if (this.ReadOnlyChanged != null)
        {
            this.ReadOnlyChanged(this, this.readOnly);
        }
    }

    protected internal virtual void OnSubmit()
    {
        object[] args = new object[] { this, this.text };
        base.SignalHierarchy("OnTextSubmitted", args);
        if (this.TextSubmitted != null)
        {
            this.TextSubmitted(this, this.text);
        }
    }

    protected override void OnTabKeyPressed(dfKeyEventArgs args)
    {
        if (this.acceptsTab)
        {
            base.OnKeyPress(args);
            if (!args.Used)
            {
                args.Character = '\t';
                this.processKeyPress(args);
            }
        }
        else
        {
            base.OnTabKeyPressed(args);
        }
    }

    protected internal virtual void OnTextChanged()
    {
        object[] args = new object[] { this.text };
        base.SignalHierarchy("OnTextChanged", args);
        if (this.TextChanged != null)
        {
            this.TextChanged(this, this.text);
        }
    }

    private string passwordDisplayText()
    {
        return new string(this.passwordChar[0], this.text.Length);
    }

    private void pasteAtCursor(string clipData)
    {
        this.deleteSelection();
        StringBuilder builder = new StringBuilder(this.text.Length + clipData.Length);
        builder.Append(this.text);
        for (int i = 0; i < clipData.Length; i++)
        {
            char ch = clipData[i];
            if (ch >= ' ')
            {
                builder.Insert(this.cursorIndex++, ch);
            }
        }
        builder.Length = Mathf.Min(builder.Length, this.maxLength);
        this.text = builder.ToString();
        this.setCursorPos(this.cursorIndex);
        this.OnTextChanged();
        this.Invalidate();
    }

    private void processKeyPress(dfKeyEventArgs args)
    {
        this.deleteSelection();
        if (this.text.Length < this.MaxLength)
        {
            if (this.cursorIndex == this.text.Length)
            {
                this.text = this.text + args.Character;
            }
            else
            {
                this.text = this.text.Insert(this.cursorIndex, args.Character.ToString());
            }
            this.cursorIndex++;
            this.OnTextChanged();
            this.Invalidate();
        }
        args.Use();
    }

    private void renderCursor(int startIndex, int cursorIndex, float[] charWidths, float leftOffset)
    {
        if (!string.IsNullOrEmpty(this.SelectionSprite) && (base.Atlas != null))
        {
            float num = 0f;
            for (int i = startIndex; i < cursorIndex; i++)
            {
                num += charWidths[i];
            }
            float stepSize = base.PixelsToUnits();
            float x = ((num + leftOffset) + (this.padding.left * stepSize)).Quantize(stepSize);
            float y = -this.padding.top * stepSize;
            float num6 = stepSize * this.cursorWidth;
            float num7 = (this.size.y - this.padding.vertical) * stepSize;
            Vector3 vector = new Vector3(x, y);
            Vector3 vector2 = new Vector3(x + num6, y);
            Vector3 vector3 = new Vector3(x + num6, y - num7);
            Vector3 vector4 = new Vector3(x, y - num7);
            dfList<Vector3> vertices = base.renderData.Vertices;
            dfList<int> triangles = base.renderData.Triangles;
            dfList<Vector2> uV = base.renderData.UV;
            dfList<Color32> colors = base.renderData.Colors;
            Vector3 vector5 = (Vector3) (base.pivot.TransformToUpperLeft(base.size) * stepSize);
            this.addQuadIndices(vertices, triangles);
            vertices.Add(vector + vector5);
            vertices.Add(vector2 + vector5);
            vertices.Add(vector3 + vector5);
            vertices.Add(vector4 + vector5);
            Color32 item = base.ApplyOpacity(this.TextColor);
            colors.Add(item);
            colors.Add(item);
            colors.Add(item);
            colors.Add(item);
            dfAtlas.ItemInfo info = base.Atlas[this.SelectionSprite];
            Rect region = info.region;
            uV.Add(new Vector2(region.x, region.yMax));
            uV.Add(new Vector2(region.xMax, region.yMax));
            uV.Add(new Vector2(region.xMax, region.y));
            uV.Add(new Vector2(region.x, region.y));
        }
    }

    public dfList<dfRenderData> RenderMultiple()
    {
        if ((base.Atlas == null) || (this.Font == null))
        {
            return null;
        }
        if (!base.isVisible)
        {
            return null;
        }
        if (base.renderData == null)
        {
            base.renderData = dfRenderData.Obtain();
            this.textRenderData = dfRenderData.Obtain();
            base.isControlInvalidated = true;
        }
        if (!base.isControlInvalidated)
        {
            for (int i = 0; i < this.buffers.Count; i++)
            {
                this.buffers[i].Transform = base.transform.localToWorldMatrix;
            }
            return this.buffers;
        }
        this.buffers.Clear();
        base.renderData.Clear();
        base.renderData.Material = base.Atlas.Material;
        base.renderData.Transform = base.transform.localToWorldMatrix;
        this.buffers.Add(base.renderData);
        this.textRenderData.Clear();
        this.textRenderData.Material = base.Atlas.Material;
        this.textRenderData.Transform = base.transform.localToWorldMatrix;
        this.buffers.Add(this.textRenderData);
        this.renderBackground();
        this.renderText(this.textRenderData);
        base.isControlInvalidated = false;
        this.updateCollider();
        return this.buffers;
    }

    private void renderSelection(int scrollIndex, float[] charWidths, float leftOffset)
    {
        if (!string.IsNullOrEmpty(this.SelectionSprite) && (base.Atlas != null))
        {
            float num = base.PixelsToUnits();
            float b = (this.size.x - this.padding.horizontal) * num;
            int num3 = scrollIndex;
            float num4 = 0f;
            for (int i = scrollIndex; i < this.text.Length; i++)
            {
                num3++;
                num4 += charWidths[i];
                if (num4 > b)
                {
                    break;
                }
            }
            if ((this.selectionStart <= num3) && (this.selectionEnd >= scrollIndex))
            {
                int num6 = Mathf.Max(scrollIndex, this.selectionStart);
                if (num6 <= num3)
                {
                    int num7 = Mathf.Min(this.selectionEnd, num3);
                    if (num7 > scrollIndex)
                    {
                        float num8 = 0f;
                        float num9 = 0f;
                        num4 = 0f;
                        for (int j = scrollIndex; j <= num3; j++)
                        {
                            if (j == num6)
                            {
                                num8 = num4;
                            }
                            if (j == num7)
                            {
                                num9 = num4;
                                break;
                            }
                            num4 += charWidths[j];
                        }
                        float num11 = base.Size.y * num;
                        this.addQuadIndices(base.renderData.Vertices, base.renderData.Triangles);
                        float x = (num8 + leftOffset) + (this.padding.left * num);
                        float num13 = x + Mathf.Min(num9 - num8, b);
                        float y = -(this.padding.top + 1) * num;
                        float num15 = (y - num11) + ((this.padding.vertical + 2) * num);
                        Vector3 vector = (Vector3) (base.pivot.TransformToUpperLeft(base.Size) * num);
                        Vector3 item = new Vector3(x, y) + vector;
                        Vector3 vector3 = new Vector3(num13, y) + vector;
                        Vector3 vector4 = new Vector3(x, num15) + vector;
                        Vector3 vector5 = new Vector3(num13, num15) + vector;
                        base.renderData.Vertices.Add(item);
                        base.renderData.Vertices.Add(vector3);
                        base.renderData.Vertices.Add(vector5);
                        base.renderData.Vertices.Add(vector4);
                        Color32 color = base.ApplyOpacity(this.SelectionBackgroundColor);
                        base.renderData.Colors.Add(color);
                        base.renderData.Colors.Add(color);
                        base.renderData.Colors.Add(color);
                        base.renderData.Colors.Add(color);
                        dfAtlas.ItemInfo info = base.Atlas[this.SelectionSprite];
                        Rect region = info.region;
                        float num16 = region.width / info.sizeInPixels.x;
                        float num17 = region.height / info.sizeInPixels.y;
                        base.renderData.UV.Add(new Vector2(region.x + num16, region.yMax - num17));
                        base.renderData.UV.Add(new Vector2(region.xMax - num16, region.yMax - num17));
                        base.renderData.UV.Add(new Vector2(region.xMax - num16, region.y + num17));
                        base.renderData.UV.Add(new Vector2(region.x + num16, region.y + num17));
                    }
                }
            }
        }
    }

    private void renderText(dfRenderData textBuffer)
    {
        float num = base.PixelsToUnits();
        Vector2 vector = new Vector2(this.size.x - this.padding.horizontal, this.size.y - this.padding.vertical);
        Vector3 vector2 = base.pivot.TransformToUpperLeft(base.Size);
        Vector3 vector3 = (Vector3) (new Vector3(vector2.x + this.padding.left, vector2.y - this.padding.top, 0f) * num);
        string text = (!this.IsPasswordField || string.IsNullOrEmpty(this.passwordChar)) ? this.text : this.passwordDisplayText();
        Color32 color = !base.IsEnabled ? base.DisabledColor : this.TextColor;
        float num2 = this.getTextScaleMultiplier();
        using (dfFontRendererBase base2 = this.font.ObtainRenderer())
        {
            base2.WordWrap = false;
            base2.MaxSize = vector;
            base2.PixelRatio = num;
            base2.TextScale = this.TextScale * num2;
            base2.VectorOffset = vector3;
            base2.MultiLine = false;
            base2.TextAlign = UnityEngine.TextAlignment.Left;
            base2.ProcessMarkup = false;
            base2.DefaultColor = color;
            base2.BottomColor = new Color32?(color);
            base2.OverrideMarkupColors = false;
            base2.Opacity = base.CalculateOpacity();
            base2.Shadow = this.Shadow;
            base2.ShadowColor = this.ShadowColor;
            base2.ShadowOffset = this.ShadowOffset;
            this.cursorIndex = Mathf.Min(this.cursorIndex, text.Length);
            this.scrollIndex = Mathf.Min(Mathf.Min(this.scrollIndex, this.cursorIndex), text.Length);
            this.charWidths = base2.GetCharacterWidths(text);
            Vector2 vector4 = (Vector2) (vector * num);
            this.leftOffset = 0f;
            if (this.textAlign == UnityEngine.TextAlignment.Left)
            {
                float num3 = 0f;
                for (int i = this.scrollIndex; i < this.cursorIndex; i++)
                {
                    num3 += this.charWidths[i];
                }
                while ((num3 >= vector4.x) && (this.scrollIndex < this.cursorIndex))
                {
                    num3 -= this.charWidths[this.scrollIndex++];
                }
            }
            else
            {
                this.scrollIndex = Mathf.Max(0, Mathf.Min(this.cursorIndex, text.Length - 1));
                float num5 = 0f;
                float num6 = (this.font.FontSize * 1.25f) * num;
                while ((this.scrollIndex > 0) && (num5 < (vector4.x - num6)))
                {
                    num5 += this.charWidths[this.scrollIndex--];
                }
                float num7 = (text.Length <= 0) ? 0f : base2.GetCharacterWidths(text.Substring(this.scrollIndex)).Sum();
                switch (this.textAlign)
                {
                    case UnityEngine.TextAlignment.Center:
                        this.leftOffset = Mathf.Max((float) 0f, (float) ((vector4.x - num7) * 0.5f));
                        break;

                    case UnityEngine.TextAlignment.Right:
                        this.leftOffset = Mathf.Max((float) 0f, (float) (vector4.x - num7));
                        break;
                }
                vector3.x += this.leftOffset;
                base2.VectorOffset = vector3;
            }
            if (this.selectionEnd != this.selectionStart)
            {
                this.renderSelection(this.scrollIndex, this.charWidths, this.leftOffset);
            }
            else if (this.cursorShown)
            {
                this.renderCursor(this.scrollIndex, this.cursorIndex, this.charWidths, this.leftOffset);
            }
            base2.Render(text.Substring(this.scrollIndex), textBuffer);
        }
    }

    private void selectAll()
    {
        this.selectionStart = 0;
        this.selectionEnd = this.text.Length;
        this.scrollIndex = 0;
        this.setCursorPos(0);
    }

    private void selectToEnd()
    {
        if (this.cursorIndex != this.text.Length)
        {
            if (this.selectionEnd == this.selectionStart)
            {
                this.selectionStart = this.cursorIndex;
            }
            else if (this.selectionStart == this.cursorIndex)
            {
                this.selectionStart = this.selectionEnd;
            }
            this.selectionEnd = this.text.Length;
            this.setCursorPos(this.text.Length);
        }
    }

    private void selectToStart()
    {
        if (this.cursorIndex != 0)
        {
            if (this.selectionEnd == this.selectionStart)
            {
                this.selectionEnd = this.cursorIndex;
            }
            else if (this.selectionEnd == this.cursorIndex)
            {
                this.selectionEnd = this.selectionStart;
            }
            this.selectionStart = 0;
            this.setCursorPos(0);
        }
    }

    private void selectWordAtIndex(int index)
    {
        if (!string.IsNullOrEmpty(this.text))
        {
            index = Mathf.Max(Mathf.Min(this.text.Length - 1, index), 0);
            char c = this.text[index];
            if (!char.IsLetterOrDigit(c))
            {
                this.selectionStart = index;
                this.selectionEnd = index + 1;
                this.mouseSelectionAnchor = 0;
            }
            else
            {
                this.selectionStart = index;
                for (int i = index; i > 0; i--)
                {
                    if (!char.IsLetterOrDigit(this.text[i - 1]))
                    {
                        break;
                    }
                    this.selectionStart--;
                }
                this.selectionEnd = index;
                for (int j = index; j < this.text.Length; j++)
                {
                    if (!char.IsLetterOrDigit(this.text[j]))
                    {
                        break;
                    }
                    this.selectionEnd = j + 1;
                }
            }
            this.cursorIndex = this.selectionStart;
            this.Invalidate();
        }
    }

    private void setCursorPos(int index)
    {
        index = Mathf.Max(0, Mathf.Min(this.text.Length, index));
        if (index != this.cursorIndex)
        {
            this.cursorIndex = index;
            this.cursorShown = this.HasFocus;
            this.scrollIndex = Mathf.Min(this.scrollIndex, this.cursorIndex);
            this.Invalidate();
        }
    }

    public float CursorBlinkTime
    {
        get
        {
            return this.cursorBlinkTime;
        }
        set
        {
            this.cursorBlinkTime = value;
        }
    }

    public int CursorIndex
    {
        get
        {
            return this.cursorIndex;
        }
        set
        {
            value = Mathf.Max(0, value);
            value = Mathf.Min(0, this.text.Length - 1);
            this.cursorIndex = value;
        }
    }

    public int CursorWidth
    {
        get
        {
            return this.cursorWidth;
        }
        set
        {
            this.cursorWidth = value;
        }
    }

    public dfFontBase Font
    {
        get
        {
            if (this.font == null)
            {
                dfGUIManager manager = base.GetManager();
                if (manager != null)
                {
                    this.font = manager.DefaultFont;
                }
            }
            return this.font;
        }
        set
        {
            if (value != this.font)
            {
                this.font = value;
                this.Invalidate();
            }
        }
    }

    public bool HideMobileInputField
    {
        get
        {
            return this.mobileHideInputField;
        }
        set
        {
            this.mobileHideInputField = value;
        }
    }

    public bool IsPasswordField
    {
        get
        {
            return this.displayAsPassword;
        }
        set
        {
            if (value != this.displayAsPassword)
            {
                this.displayAsPassword = value;
                this.Invalidate();
            }
        }
    }

    public int MaxLength
    {
        get
        {
            return this.maxLength;
        }
        set
        {
            if (value != this.maxLength)
            {
                this.maxLength = Mathf.Max(0, value);
                if (this.maxLength < this.text.Length)
                {
                    this.Text = this.text.Substring(0, this.maxLength);
                }
                this.Invalidate();
            }
        }
    }

    public bool MobileAutoCorrect
    {
        get
        {
            return this.mobileAutoCorrect;
        }
        set
        {
            this.mobileAutoCorrect = value;
        }
    }

    public dfMobileKeyboardTrigger MobileKeyboardTrigger
    {
        get
        {
            return this.mobileKeyboardTrigger;
        }
        set
        {
            this.mobileKeyboardTrigger = value;
        }
    }

    public RectOffset Padding
    {
        get
        {
            if (this.padding == null)
            {
                this.padding = new RectOffset();
            }
            return this.padding;
        }
        set
        {
            value = value.ConstrainPadding();
            if (!object.Equals(value, this.padding))
            {
                this.padding = value;
                this.Invalidate();
            }
        }
    }

    public string PasswordCharacter
    {
        get
        {
            return this.passwordChar;
        }
        set
        {
            if (!string.IsNullOrEmpty(value))
            {
                this.passwordChar = value[0].ToString();
            }
            else
            {
                this.passwordChar = value;
            }
            this.OnPasswordCharacterChanged();
            this.Invalidate();
        }
    }

    public bool ReadOnly
    {
        get
        {
            return this.readOnly;
        }
        set
        {
            if (value != this.readOnly)
            {
                this.readOnly = value;
                this.OnReadOnlyChanged();
                this.Invalidate();
            }
        }
    }

    public string SelectedText
    {
        get
        {
            if (this.selectionEnd == this.selectionStart)
            {
                return string.Empty;
            }
            return this.text.Substring(this.selectionStart, this.selectionEnd - this.selectionStart);
        }
    }

    public Color32 SelectionBackgroundColor
    {
        get
        {
            return this.selectionBackground;
        }
        set
        {
            this.selectionBackground = value;
            this.Invalidate();
        }
    }

    public int SelectionEnd
    {
        get
        {
            return this.selectionEnd;
        }
        set
        {
            if (value != this.selectionEnd)
            {
                this.selectionEnd = Mathf.Max(0, Mathf.Min(value, this.text.Length));
                this.selectionStart = Mathf.Max(this.selectionStart, this.selectionEnd);
                this.Invalidate();
            }
        }
    }

    public int SelectionLength
    {
        get
        {
            return (this.selectionEnd - this.selectionStart);
        }
    }

    public string SelectionSprite
    {
        get
        {
            return this.selectionSprite;
        }
        set
        {
            if (value != this.selectionSprite)
            {
                this.selectionSprite = value;
                this.Invalidate();
            }
        }
    }

    public int SelectionStart
    {
        get
        {
            return this.selectionStart;
        }
        set
        {
            if (value != this.selectionStart)
            {
                this.selectionStart = Mathf.Max(0, Mathf.Min(value, this.text.Length));
                this.selectionEnd = Mathf.Max(this.selectionEnd, this.selectionStart);
                this.Invalidate();
            }
        }
    }

    public bool SelectOnFocus
    {
        get
        {
            return this.selectOnFocus;
        }
        set
        {
            this.selectOnFocus = value;
        }
    }

    public bool Shadow
    {
        get
        {
            return this.shadow;
        }
        set
        {
            if (value != this.shadow)
            {
                this.shadow = value;
                this.Invalidate();
            }
        }
    }

    public Color32 ShadowColor
    {
        get
        {
            return this.shadowColor;
        }
        set
        {
            if (!value.Equals(this.shadowColor))
            {
                this.shadowColor = value;
                this.Invalidate();
            }
        }
    }

    public Vector2 ShadowOffset
    {
        get
        {
            return this.shadowOffset;
        }
        set
        {
            if (value != this.shadowOffset)
            {
                this.shadowOffset = value;
                this.Invalidate();
            }
        }
    }

    public string Text
    {
        get
        {
            return this.text;
        }
        set
        {
            if (value.Length > this.MaxLength)
            {
                value = value.Substring(0, this.MaxLength);
            }
            value = value.Replace("\t", " ");
            if (value != this.text)
            {
                this.text = value;
                this.scrollIndex = this.cursorIndex = 0;
                this.OnTextChanged();
                this.Invalidate();
            }
        }
    }

    public UnityEngine.TextAlignment TextAlignment
    {
        get
        {
            return this.textAlign;
        }
        set
        {
            if (value != this.textAlign)
            {
                this.textAlign = value;
                this.Invalidate();
            }
        }
    }

    public Color32 TextColor
    {
        get
        {
            return this.textColor;
        }
        set
        {
            this.textColor = value;
            this.Invalidate();
        }
    }

    public float TextScale
    {
        get
        {
            return this.textScale;
        }
        set
        {
            value = Mathf.Max(0.1f, value);
            if (!Mathf.Approximately(this.textScale, value))
            {
                this.textScale = value;
                this.Invalidate();
            }
        }
    }

    public dfTextScaleMode TextScaleMode
    {
        get
        {
            return this.textScaleMode;
        }
        set
        {
            this.textScaleMode = value;
            this.Invalidate();
        }
    }

    public bool UseMobileKeyboard
    {
        get
        {
            return this.useMobileKeyboard;
        }
        set
        {
            this.useMobileKeyboard = value;
        }
    }

    [CompilerGenerated]
    private sealed class <doCursorBlink>c__Iterator44 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal dfTextbox <>f__this;

        [DebuggerHidden]
        public void Dispose()
        {
            this.$PC = -1;
        }

        public bool MoveNext()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            switch (num)
            {
                case 0:
                    if (Application.isPlaying)
                    {
                        this.<>f__this.cursorShown = true;
                        while (this.<>f__this.ContainsFocus)
                        {
                            this.$current = new WaitForSeconds(this.<>f__this.cursorBlinkTime);
                            this.$PC = 1;
                            return true;
                        Label_0063:
                            this.<>f__this.cursorShown = !this.<>f__this.cursorShown;
                            this.<>f__this.Invalidate();
                        }
                        this.<>f__this.cursorShown = false;
                        this.$PC = -1;
                        break;
                    }
                    break;

                case 1:
                    goto Label_0063;
            }
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        object IEnumerator<object>.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }

        object IEnumerator.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }
    }
}

