using NGUIHack;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Input (Basic)")]
public class UIInput : MonoBehaviour
{
    public Color activeColor = Color.white;
    public static UIInput current;
    public GameObject eventReceiver;
    public string functionName = "OnSubmit";
    public bool inputMultiline;
    public bool isPassword;
    public UILabel label;
    private List<UITextMarkup> markups;
    public int maxChars;
    private Color mDefaultColor = Color.white;
    private string mDefaultText = string.Empty;
    private string mLastIME = string.Empty;
    private string mText = string.Empty;
    public bool trippleClickSelect = true;
    public KeyboardType type;
    public Validator validator;

    private void Awake()
    {
        this.markups = new List<UITextMarkup>();
        this.Init();
    }

    internal string CheckChanges(string newText)
    {
        if (this.mText != newText)
        {
            this.mText = newText;
            this.UpdateLabel();
            return this.mText;
        }
        return this.mText;
    }

    internal void CheckPositioning(int carratPos, int selectPos)
    {
        this.label.selection = this.label.ConvertUnprocessedSelection(carratPos, selectPos);
    }

    internal void GainFocus()
    {
        UIUnityEvents.TextGainFocus(this);
    }

    protected void Init()
    {
        if (this.label == null)
        {
            this.label = base.GetComponentInChildren<UILabel>();
        }
        if (this.label != null)
        {
            this.mDefaultText = this.label.text;
            this.mDefaultColor = this.label.color;
            this.label.supportEncoding = false;
        }
    }

    internal void LoseFocus()
    {
        UIUnityEvents.TextLostFocus(this);
    }

    private void OnDisable()
    {
        if (UICamera.IsHighlighted(base.gameObject))
        {
            this.OnSelect(false);
        }
    }

    private void OnEnable()
    {
        if (UICamera.IsHighlighted(base.gameObject))
        {
            this.OnSelect(true);
        }
    }

    internal void OnEvent(UICamera camera, Event @event, EventType type)
    {
        switch (type)
        {
            case EventType.MouseDown:
                if (@event.button == 0)
                {
                    UIUnityEvents.TextClickDown(camera, this, @event, this.label);
                }
                break;

            case EventType.MouseUp:
                if (@event.button != 0)
                {
                    Debug.Log("Wee");
                    break;
                }
                UIUnityEvents.TextClickUp(camera, this, @event, this.label);
                break;

            case EventType.MouseDrag:
                if (@event.button == 0)
                {
                    UIUnityEvents.TextDrag(camera, this, @event, this.label);
                }
                break;

            case EventType.KeyDown:
                UIUnityEvents.TextKeyDown(camera, this, @event, this.label);
                break;

            case EventType.KeyUp:
                UIUnityEvents.TextKeyUp(camera, this, @event, this.label);
                break;
        }
    }

    private void OnSelect(bool isSelected)
    {
        if (((this.label != null) && base.enabled) && base.gameObject.activeInHierarchy)
        {
            if (isSelected)
            {
                this.mText = !(this.label.text == this.mDefaultText) ? this.label.text : string.Empty;
                this.label.color = this.activeColor;
                if (this.isPassword)
                {
                    this.label.password = true;
                }
                Transform cachedTransform = this.label.cachedTransform;
                Vector3 pivotOffset = (Vector3) this.label.pivotOffset;
                pivotOffset.y += this.label.relativeSize.y;
                pivotOffset = cachedTransform.TransformPoint(pivotOffset);
                this.UpdateLabel();
            }
            else
            {
                if (string.IsNullOrEmpty(this.mText))
                {
                    this.label.text = this.mDefaultText;
                    this.label.color = this.mDefaultColor;
                    if (this.isPassword)
                    {
                        this.label.password = false;
                    }
                }
                else
                {
                    this.label.text = this.mText;
                }
                this.label.showLastPasswordChar = false;
            }
        }
    }

    public bool RequestKeyboardFocus()
    {
        return UIUnityEvents.RequestKeyboardFocus(this);
    }

    public bool SendSubmitMessage()
    {
        if (this.eventReceiver == null)
        {
            this.eventReceiver = base.gameObject;
        }
        string mText = this.mText;
        this.eventReceiver.SendMessage(this.functionName, SendMessageOptions.DontRequireReceiver);
        return (mText != this.mText);
    }

    private void Update()
    {
    }

    internal void UpdateLabel()
    {
        if ((this.maxChars > 0) && (this.mText.Length > this.maxChars))
        {
            this.mText = this.mText.Substring(0, this.maxChars);
        }
        if (this.label.font != null)
        {
            string mText;
            if (this.selected)
            {
                mText = this.mText + this.mLastIME;
            }
            else
            {
                mText = this.mText;
            }
            this.label.supportEncoding = false;
            mText = this.label.font.WrapText(this.markups, mText, ((float) this.label.lineWidth) / this.label.cachedTransform.localScale.x, 0, false, UIFont.SymbolStyle.None);
            this.markups.SortMarkup();
            this.label.text = mText;
            this.label.showLastPasswordChar = this.selected;
        }
    }

    public string inputText
    {
        get
        {
            return this.mText;
        }
    }

    public bool selected
    {
        get
        {
            return (UICamera.selectedObject == base.gameObject);
        }
        set
        {
            if (!value && (UICamera.selectedObject == base.gameObject))
            {
                UICamera.selectedObject = null;
            }
            else if (value)
            {
                UICamera.selectedObject = base.gameObject;
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
            if (value == null)
            {
            }
            value = string.Empty;
            if (this.mText == null)
            {
            }
            bool flag = string.Empty != value;
            this.mText = value;
            if (this.label != null)
            {
                if (value != string.Empty)
                {
                    value = this.mDefaultText;
                }
                this.label.supportEncoding = false;
                this.label.showLastPasswordChar = this.selected;
                this.label.color = (!this.selected && !(value != this.mDefaultText)) ? this.mDefaultColor : this.activeColor;
                if (flag)
                {
                    this.UpdateLabel();
                }
            }
        }
    }

    public enum KeyboardType
    {
        Default,
        ASCIICapable,
        NumbersAndPunctuation,
        URL,
        NumberPad,
        PhonePad,
        NamePhonePad,
        EmailAddress
    }

    public delegate char Validator(string currentText, char nextChar);
}

