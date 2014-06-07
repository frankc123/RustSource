using NGUIHack;
using System;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

[AddComponentMenu("")]
public class UIUnityEvents : MonoBehaviour
{
    private static int blankID;
    private static int controlID;
    private const int controlIDHint = 0x1317bfa4;
    private static bool focusSetInOnGUI;
    public static bool forbidHandlingNewEvents;
    private const int idLoop = 300;
    private static readonly Rect idRect = new Rect(0f, 0f, 69999f, 69999f);
    private static bool inOnGUI;
    private const string kControlName = "ngui-unityevents";
    private const int kGUIDepth = 0x31;
    private static Vector2 lastCursorPosition;
    private static UIInput lastInput;
    private static UICamera lastInputCamera;
    private static UILabel lastLabel;
    private static Vector2 lastMousePosition = new Vector2(-100f, -100f);
    private static UITextPosition lastTextPosition;
    private static bool madeSingleton;
    private UICamera mCamera;
    private UIInput mInput;
    private static bool requiresBinding;
    private static bool submit;
    private static GUIContent textInputContent = null;

    private void Awake()
    {
        base.useGUILayout = false;
    }

    private static void Bind()
    {
        if ((requiresBinding && (lastInput != null)) && (lastInputCamera != null))
        {
            SetKeyboardControl();
            requiresBinding = false;
            focusSetInOnGUI = true;
        }
    }

    public static void CameraCreated(UICamera camera)
    {
        if (Application.isPlaying && (LateLoaded.singleton == null))
        {
            Debug.Log("singleton check failed.");
        }
    }

    private static void ChangeFocus(UICamera camera, UIInput input, UILabel label)
    {
        if (lastInput != input)
        {
            lastInput = input;
            textInputContent = null;
            requiresBinding = (bool) input;
            focusSetInOnGUI = inOnGUI;
        }
        lastInputCamera = camera;
        lastLabel = label;
    }

    private static bool GetKeyboardControl()
    {
        return (GUIUtility.keyboardControl == controlID);
    }

    private static bool GetTextEditor(out TextEditor te)
    {
        submit = false;
        if ((!focusSetInOnGUI && requiresBinding) && ((lastInput != null) && (lastInputCamera != null)))
        {
            GUI.FocusControl("ngui-unityevents");
        }
        Bind();
        te = GUIUtility.GetStateObject(typeof(TextEditor), controlID) as TextEditor;
        if (lastInput != null)
        {
            if (textInputContent == null)
            {
            }
            (textInputContent = new GUIContent()).text = lastInput.inputText;
            te.content.text = textInputContent.text;
            te.SaveBackup();
            te.position = idRect;
            te.style = textStyle;
            te.multiline = lastInput.inputMultiline;
            te.controlID = controlID;
            te.ClampPos();
            return true;
        }
        te = null;
        return false;
    }

    private static bool MoveTextPosition(Event @event, TextEditor te, ref UITextPosition res)
    {
        lastTextPosition = res;
        if (!res.valid)
        {
            return false;
        }
        te.pos = res.uniformPosition;
        if (!@event.shift)
        {
            te.selectPos = te.pos;
        }
        return true;
    }

    private void OnDestroy()
    {
        if (madeSingleton && (LateLoaded.singleton == this))
        {
            LateLoaded.singleton = null;
        }
    }

    private void OnGUI()
    {
        try
        {
            inOnGUI = true;
            GUI.depth = 0x31;
            blankID = GUIUtility.GetControlID(FocusType.Keyboard);
            GUI.SetNextControlName("ngui-unityevents");
            controlID = GUIUtility.GetControlID(FocusType.Keyboard);
            GUI.color = Color.clear;
            Event current = Event.current;
            EventType type = current.type;
            if (type == EventType.MouseMove)
            {
                Debug.Log("Mouse Move");
            }
            switch (type)
            {
                case EventType.MouseDown:
                    if (!forbidHandlingNewEvents)
                    {
                        bool flag = current.button == 0;
                        using (Event event6 = new Event(current))
                        {
                            UICamera.HandleEvent(event6, type);
                        }
                        if ((flag && (current.type == EventType.Used)) && (GUIUtility.hotControl == 0))
                        {
                            GUIUtility.hotControl = blankID;
                        }
                    }
                    break;

                case EventType.MouseUp:
                {
                    bool flag2 = current.button == 0;
                    using (Event event7 = new Event(current))
                    {
                        UICamera.HandleEvent(event7, type);
                    }
                    if (flag2 && (GUIUtility.hotControl == blankID))
                    {
                        GUIUtility.hotControl = 0;
                    }
                    break;
                }
                case EventType.MouseMove:
                case EventType.MouseDrag:
                case EventType.KeyUp:
                case EventType.ScrollWheel:
                    using (Event event4 = new Event(current))
                    {
                        UICamera.HandleEvent(event4, type);
                    }
                    break;

                case EventType.KeyDown:
                    if (!forbidHandlingNewEvents)
                    {
                        using (Event event5 = new Event(current))
                        {
                            UICamera.HandleEvent(event5, type);
                        }
                    }
                    break;

                case EventType.Repaint:
                    if (!forbidHandlingNewEvents && (lastMousePosition != current.mousePosition))
                    {
                        lastMousePosition = current.mousePosition;
                        using (Event event3 = new Event(current, EventType.MouseMove))
                        {
                            UICamera.HandleEvent(event3, EventType.MouseMove);
                        }
                    }
                    break;

                case EventType.Used:
                    Debug.Log("Used");
                    return;
            }
            if (type == EventType.Repaint)
            {
            }
        }
        finally
        {
            inOnGUI = false;
        }
    }

    private static bool Perform(TextEditor te, TextEditOp operation)
    {
        return PerformOperation(te, operation);
    }

    private static bool PerformOperation(TextEditor te, TextEditOp operation)
    {
        switch (operation)
        {
            case TextEditOp.MoveLeft:
                te.MoveLeft();
                break;

            case TextEditOp.MoveRight:
                te.MoveRight();
                break;

            case TextEditOp.MoveUp:
                te.MoveUp();
                break;

            case TextEditOp.MoveDown:
                te.MoveDown();
                break;

            case TextEditOp.MoveLineStart:
                te.MoveLineStart();
                break;

            case TextEditOp.MoveLineEnd:
                te.MoveLineEnd();
                break;

            case TextEditOp.MoveTextStart:
                te.MoveTextStart();
                break;

            case TextEditOp.MoveTextEnd:
                te.MoveTextEnd();
                break;

            case TextEditOp.MoveGraphicalLineStart:
                te.MoveGraphicalLineStart();
                break;

            case TextEditOp.MoveGraphicalLineEnd:
                te.MoveGraphicalLineEnd();
                break;

            case TextEditOp.MoveWordLeft:
                te.MoveWordLeft();
                break;

            case TextEditOp.MoveWordRight:
                te.MoveWordRight();
                break;

            case TextEditOp.MoveParagraphForward:
                te.MoveParagraphForward();
                break;

            case TextEditOp.MoveParagraphBackward:
                te.MoveParagraphBackward();
                break;

            case TextEditOp.MoveToStartOfNextWord:
                te.MoveToStartOfNextWord();
                break;

            case TextEditOp.MoveToEndOfPreviousWord:
                te.MoveToEndOfPreviousWord();
                break;

            case TextEditOp.SelectLeft:
                te.SelectLeft();
                break;

            case TextEditOp.SelectRight:
                te.SelectRight();
                break;

            case TextEditOp.SelectUp:
                te.SelectUp();
                break;

            case TextEditOp.SelectDown:
                te.SelectDown();
                break;

            case TextEditOp.SelectTextStart:
                te.SelectTextStart();
                break;

            case TextEditOp.SelectTextEnd:
                te.SelectTextEnd();
                break;

            case TextEditOp.ExpandSelectGraphicalLineStart:
                te.ExpandSelectGraphicalLineStart();
                break;

            case TextEditOp.ExpandSelectGraphicalLineEnd:
                te.ExpandSelectGraphicalLineEnd();
                break;

            case TextEditOp.SelectGraphicalLineStart:
                te.SelectGraphicalLineStart();
                break;

            case TextEditOp.SelectGraphicalLineEnd:
                te.SelectGraphicalLineEnd();
                break;

            case TextEditOp.SelectWordLeft:
                te.SelectWordLeft();
                break;

            case TextEditOp.SelectWordRight:
                te.SelectWordRight();
                break;

            case TextEditOp.SelectToEndOfPreviousWord:
                te.SelectToEndOfPreviousWord();
                break;

            case TextEditOp.SelectToStartOfNextWord:
                te.SelectToStartOfNextWord();
                break;

            case TextEditOp.SelectParagraphBackward:
                te.SelectParagraphBackward();
                break;

            case TextEditOp.SelectParagraphForward:
                te.SelectParagraphForward();
                break;

            case TextEditOp.Delete:
                return te.Delete();

            case TextEditOp.Backspace:
                return te.Backspace();

            case TextEditOp.DeleteWordBack:
                return te.DeleteWordBack();

            case TextEditOp.DeleteWordForward:
                return te.DeleteWordForward();

            case TextEditOp.Cut:
                return te.Cut();

            case TextEditOp.Copy:
                te.Copy();
                break;

            case TextEditOp.Paste:
                return te.Paste();

            case TextEditOp.SelectAll:
                te.SelectAll();
                break;

            case TextEditOp.SelectNone:
                te.SelectNone();
                break;

            default:
                Debug.Log("Unimplemented: " + operation);
                break;
        }
        return false;
    }

    internal static bool RequestKeyboardFocus(UIInput input)
    {
        if (input == lastInput)
        {
            return true;
        }
        if (lastInput == null)
        {
            if ((input.label == null) || !input.label.enabled)
            {
                return false;
            }
            UICamera camera = UICamera.FindCameraForLayer(input.label.gameObject.layer);
            if ((camera != null) && camera.SetKeyboardFocus(input))
            {
                ChangeFocus(camera, input, input.label);
                return true;
            }
        }
        return false;
    }

    private static bool SelectTextPosition(Event @event, TextEditor te, ref UITextPosition res)
    {
        lastTextPosition = res;
        if (res.valid)
        {
            lastCursorPosition = textStyle.GetCursorPixelPosition(idRect, textInputContent, res.uniformPosition);
            te.SelectToPosition(lastCursorPosition);
            return true;
        }
        return false;
    }

    private static bool SetKeyboardControl()
    {
        GUIUtility.keyboardControl = controlID;
        return (GUIUtility.keyboardControl == controlID);
    }

    internal static void TextClickDown(UICamera camera, UIInput input, Event @event, UILabel label)
    {
        TextClickDown(camera, input, @event.real, label);
    }

    private static void TextClickDown(UICamera camera, UIInput input, Event @event, UILabel label)
    {
        UITextPosition res = !@event.shift ? camera.RaycastText(Input.mousePosition, label) : new UITextPosition();
        TextEditor te = null;
        ChangeFocus(camera, input, label);
        if (!GetTextEditor(out te))
        {
            Debug.LogError("Null Text Editor");
        }
        else
        {
            GUIUtility.hotControl = controlID;
            SetKeyboardControl();
            MoveTextPosition(@event, te, ref res);
            switch (@event.clickCount)
            {
                case 2:
                    te.SelectCurrentWord();
                    te.DblClickSnap(TextEditor.DblClickSnapping.WORDS);
                    te.MouseDragSelectsWholeWords(true);
                    break;

                case 3:
                    if (input.trippleClickSelect)
                    {
                        te.SelectCurrentParagraph();
                        te.MouseDragSelectsWholeWords(true);
                        te.DblClickSnap(TextEditor.DblClickSnapping.PARAGRAPHS);
                    }
                    break;
            }
            @event.Use();
        }
        TextSharedEnd(false, te, @event);
    }

    internal static void TextClickUp(UICamera camera, UIInput input, Event @event, UILabel label)
    {
        TextClickUp(camera, input, @event.real, label);
    }

    private static void TextClickUp(UICamera camera, UIInput input, Event @event, UILabel label)
    {
        if ((input == lastInput) && (camera == lastInputCamera))
        {
            lastLabel = label;
            TextEditor te = null;
            if (GetTextEditor(out te))
            {
                if (controlID == GUIUtility.hotControl)
                {
                    te.MouseDragSelectsWholeWords(false);
                    GUIUtility.hotControl = 0;
                    @event.Use();
                    SetKeyboardControl();
                }
                else
                {
                    Debug.Log(string.Concat(new object[] { "Did not match ", controlID, " ", GUIUtility.hotControl }));
                }
                TextSharedEnd(false, te, @event);
            }
        }
    }

    internal static void TextDrag(UICamera camera, UIInput input, Event @event, UILabel label)
    {
        TextDrag(camera, input, @event.real, label);
    }

    private static void TextDrag(UICamera camera, UIInput input, Event @event, UILabel label)
    {
        if ((input == lastInput) && (camera == lastInputCamera))
        {
            lastLabel = label;
            TextEditor te = null;
            if (GetTextEditor(out te))
            {
                if (controlID == GUIUtility.hotControl)
                {
                    UITextPosition res = camera.RaycastText(Input.mousePosition, label);
                    if (!@event.shift)
                    {
                        SelectTextPosition(@event, te, ref res);
                    }
                    else
                    {
                        MoveTextPosition(@event, te, ref res);
                    }
                    @event.Use();
                }
                TextSharedEnd(false, te, @event);
            }
        }
    }

    private static bool TextEditorHandleEvent(Event e, TextEditor te)
    {
        EventModifiers modifiers = e.modifiers;
        if ((modifiers & EventModifiers.CapsLock) == EventModifiers.CapsLock)
        {
            try
            {
                e.modifiers = modifiers & ~EventModifiers.CapsLock;
                return TextEditorHandleEvent2(e, te);
            }
            finally
            {
                e.modifiers = modifiers;
            }
        }
        return TextEditorHandleEvent2(e, te);
    }

    private static bool TextEditorHandleEvent2(Event e, TextEditor te)
    {
        if (LateLoaded.Keyactions.Contains(e))
        {
            Perform(te, (TextEditOp) Convert.ToInt32(LateLoaded.Keyactions[e]));
            return true;
        }
        return false;
    }

    internal static void TextGainFocus(UIInput input)
    {
    }

    internal static void TextKeyDown(UICamera camera, UIInput input, Event @event, UILabel label)
    {
        TextKeyDown(camera, input, @event.real, label);
    }

    private static void TextKeyDown(UICamera camera, UIInput input, Event @event, UILabel label)
    {
        if ((input == lastInput) && (camera == lastInputCamera))
        {
            lastLabel = label;
            TextEditor te = null;
            if (GetTextEditor(out te))
            {
                if (!GetKeyboardControl())
                {
                    Debug.Log("Did not " + @event);
                }
                else
                {
                    bool changed = false;
                    if (TextEditorHandleEvent(@event, te))
                    {
                        @event.Use();
                        changed = true;
                    }
                    else
                    {
                        switch (@event.keyCode)
                        {
                            case KeyCode.Tab:
                                return;

                            case KeyCode.None:
                            {
                                char character = @event.character;
                                if (character == '\t')
                                {
                                    return;
                                }
                                bool flag2 = false;
                                flag2 = character == '\n';
                                if ((flag2 && !input.inputMultiline) && !@event.alt)
                                {
                                    submit = true;
                                }
                                else
                                {
                                    BMFont font;
                                    if ((label.font != null) && ((font = label.font.bmFont) != null))
                                    {
                                        if (flag2 || ((character != '\0') && font.ContainsGlyph(character)))
                                        {
                                            te.Insert(character);
                                            changed = true;
                                        }
                                        else if (character == '\0')
                                        {
                                            if (Input.compositionString.Length > 0)
                                            {
                                                te.ReplaceSelection(string.Empty);
                                                changed = true;
                                            }
                                            @event.Use();
                                        }
                                    }
                                }
                                break;
                            }
                        }
                    }
                    TextSharedEnd(changed, te, @event);
                }
            }
        }
    }

    internal static void TextKeyUp(UICamera camera, UIInput input, Event @event, UILabel label)
    {
        TextKeyUp(camera, input, @event.real, label);
    }

    private static void TextKeyUp(UICamera camera, UIInput input, Event @event, UILabel label)
    {
        if ((input == lastInput) && (camera == lastInputCamera))
        {
            lastLabel = label;
            TextEditor te = null;
            if (GetTextEditor(out te))
            {
                TextSharedEnd(false, te, @event);
            }
        }
    }

    internal static void TextLostFocus(UIInput input)
    {
        if (input == lastInput)
        {
            if ((lastInputCamera != null) && (UICamera.selectedObject == input))
            {
                UICamera.selectedObject = null;
            }
            lastInput = null;
            lastInputCamera = null;
            lastLabel = null;
        }
    }

    private static void TextSharedEnd(bool changed, TextEditor te, Event @event)
    {
        if (GetKeyboardControl())
        {
            LateLoaded.textFieldInput = true;
        }
        if (changed || (@event.type == EventType.Used))
        {
            if (lastInput != null)
            {
                textInputContent.text = te.content.text;
            }
            if (changed)
            {
                GUI.changed = true;
                lastInput.CheckChanges(textInputContent.text);
                lastInput.CheckPositioning(te.pos, te.selectPos);
                @event.Use();
            }
            else
            {
                lastInput.CheckPositioning(te.pos, te.selectPos);
            }
        }
        if (submit)
        {
            submit = false;
            if (lastInput.SendSubmitMessage())
            {
                @event.Use();
            }
        }
    }

    public static bool shouldBlockButtonInput
    {
        get
        {
            return (bool) lastInput;
        }
    }

    private static GUIStyle textStyle
    {
        get
        {
            return GUI.skin.textField;
        }
    }

    private static class LateLoaded
    {
        private static readonly PropertyInfo _textFieldInput;
        private static bool failedInvokeTextFieldInputGet;
        private static bool failedInvokeTextFieldInputSet;
        public static Hashtable Keyactions;
        public static readonly GUIStyle mTextBlockStyle = new GUIStyle();
        public static UIUnityEvents singleton;

        static LateLoaded()
        {
            mTextBlockStyle.alignment = TextAnchor.UpperLeft;
            mTextBlockStyle.border = new RectOffset(0, 0, 0, 0);
            mTextBlockStyle.clipping = TextClipping.Overflow;
            mTextBlockStyle.contentOffset = new Vector2();
            mTextBlockStyle.fixedWidth = -1f;
            mTextBlockStyle.fixedHeight = -1f;
            mTextBlockStyle.imagePosition = ImagePosition.TextOnly;
            mTextBlockStyle.margin = new RectOffset(0, 0, 0, 0);
            mTextBlockStyle.name = "BLOCK STYLE";
            mTextBlockStyle.overflow = new RectOffset(0, 0, 0, 0);
            mTextBlockStyle.padding = new RectOffset(0, 0, 0, 0);
            mTextBlockStyle.stretchHeight = false;
            mTextBlockStyle.stretchWidth = false;
            mTextBlockStyle.wordWrap = false;
            GUIStyleState state = new GUIStyleState {
                background = null,
                textColor = Color.clear
            };
            mTextBlockStyle.active = state;
            mTextBlockStyle.focused = state;
            mTextBlockStyle.hover = state;
            mTextBlockStyle.normal = state;
            mTextBlockStyle.onActive = state;
            mTextBlockStyle.onFocused = state;
            mTextBlockStyle.onHover = state;
            mTextBlockStyle.onNormal = state;
            _textFieldInput = typeof(GUIUtility).GetProperty("textFieldInput", BindingFlags.NonPublic | BindingFlags.Static);
            if (_textFieldInput == null)
            {
                failedInvokeTextFieldInputGet = true;
                failedInvokeTextFieldInputSet = true;
                Debug.LogError("Unity has changed. no bool property textFieldInput in GUIUtility");
            }
            Type[] components = new Type[] { typeof(UIUnityEvents) };
            GameObject target = new GameObject("__UIUnityEvents", components);
            singleton = target.GetComponent<UIUnityEvents>();
            UIUnityEvents.madeSingleton = true;
            Object.DontDestroyOnLoad(target);
            TextEditor editor = null;
            if (editor != null)
            {
                Debug.Log("Thats imposible.");
            }
            try
            {
                MethodInfo method = typeof(TextEditor).GetMethod("InitKeyActions", BindingFlags.NonPublic | BindingFlags.Instance);
                if (method == null)
                {
                    throw new MethodAccessException("Unity has changed. no InitKeyActions member in TextEditor");
                }
                method.Invoke(new TextEditor(), new object[0]);
                object obj3 = typeof(TextEditor).InvokeMember("s_Keyactions", BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Static, null, null, new object[0]);
                if (!(obj3 is Hashtable))
                {
                    if (!(obj3 is IDictionary))
                    {
                        throw new MethodAccessException("Unity has changed. no s_Keyactions member in TextEditor");
                    }
                    Keyactions = new Hashtable(obj3 as IDictionary);
                }
                else
                {
                    Keyactions = (Hashtable) obj3;
                }
            }
            catch (MethodAccessException exception)
            {
                Debug.Log("Caught exception \r\n" + exception + "\r\nManually building keyactions.");
                Keyactions = new Hashtable();
                MapKey("left", UIUnityEvents.TextEditOp.MoveLeft);
                MapKey("right", UIUnityEvents.TextEditOp.MoveRight);
                MapKey("up", UIUnityEvents.TextEditOp.MoveUp);
                MapKey("down", UIUnityEvents.TextEditOp.MoveDown);
                MapKey("#left", UIUnityEvents.TextEditOp.SelectLeft);
                MapKey("#right", UIUnityEvents.TextEditOp.SelectRight);
                MapKey("#up", UIUnityEvents.TextEditOp.SelectUp);
                MapKey("#down", UIUnityEvents.TextEditOp.SelectDown);
                MapKey("delete", UIUnityEvents.TextEditOp.Delete);
                MapKey("backspace", UIUnityEvents.TextEditOp.Backspace);
                MapKey("#backspace", UIUnityEvents.TextEditOp.Backspace);
                if (((Application.platform != RuntimePlatform.WindowsPlayer) && (Application.platform != RuntimePlatform.WindowsWebPlayer)) && (Application.platform != RuntimePlatform.WindowsEditor))
                {
                    MapKey("^left", UIUnityEvents.TextEditOp.MoveGraphicalLineStart);
                    MapKey("^right", UIUnityEvents.TextEditOp.MoveGraphicalLineEnd);
                    MapKey("&left", UIUnityEvents.TextEditOp.MoveWordLeft);
                    MapKey("&right", UIUnityEvents.TextEditOp.MoveWordRight);
                    MapKey("&up", UIUnityEvents.TextEditOp.MoveParagraphBackward);
                    MapKey("&down", UIUnityEvents.TextEditOp.MoveParagraphForward);
                    MapKey("%left", UIUnityEvents.TextEditOp.MoveGraphicalLineStart);
                    MapKey("%right", UIUnityEvents.TextEditOp.MoveGraphicalLineEnd);
                    MapKey("%up", UIUnityEvents.TextEditOp.MoveTextStart);
                    MapKey("%down", UIUnityEvents.TextEditOp.MoveTextEnd);
                    MapKey("#home", UIUnityEvents.TextEditOp.SelectTextStart);
                    MapKey("#end", UIUnityEvents.TextEditOp.SelectTextEnd);
                    MapKey("#^left", UIUnityEvents.TextEditOp.ExpandSelectGraphicalLineStart);
                    MapKey("#^right", UIUnityEvents.TextEditOp.ExpandSelectGraphicalLineEnd);
                    MapKey("#^up", UIUnityEvents.TextEditOp.SelectParagraphBackward);
                    MapKey("#^down", UIUnityEvents.TextEditOp.SelectParagraphForward);
                    MapKey("#&left", UIUnityEvents.TextEditOp.SelectWordLeft);
                    MapKey("#&right", UIUnityEvents.TextEditOp.SelectWordRight);
                    MapKey("#&up", UIUnityEvents.TextEditOp.SelectParagraphBackward);
                    MapKey("#&down", UIUnityEvents.TextEditOp.SelectParagraphForward);
                    MapKey("#%left", UIUnityEvents.TextEditOp.ExpandSelectGraphicalLineStart);
                    MapKey("#%right", UIUnityEvents.TextEditOp.ExpandSelectGraphicalLineEnd);
                    MapKey("#%up", UIUnityEvents.TextEditOp.SelectTextStart);
                    MapKey("#%down", UIUnityEvents.TextEditOp.SelectTextEnd);
                    MapKey("%a", UIUnityEvents.TextEditOp.SelectAll);
                    MapKey("%x", UIUnityEvents.TextEditOp.Cut);
                    MapKey("%c", UIUnityEvents.TextEditOp.Copy);
                    MapKey("%v", UIUnityEvents.TextEditOp.Paste);
                    MapKey("^d", UIUnityEvents.TextEditOp.Delete);
                    MapKey("^h", UIUnityEvents.TextEditOp.Backspace);
                    MapKey("^b", UIUnityEvents.TextEditOp.MoveLeft);
                    MapKey("^f", UIUnityEvents.TextEditOp.MoveRight);
                    MapKey("^a", UIUnityEvents.TextEditOp.MoveLineStart);
                    MapKey("^e", UIUnityEvents.TextEditOp.MoveLineEnd);
                    MapKey("&delete", UIUnityEvents.TextEditOp.DeleteWordForward);
                    MapKey("&backspace", UIUnityEvents.TextEditOp.DeleteWordBack);
                }
                else
                {
                    MapKey("home", UIUnityEvents.TextEditOp.MoveGraphicalLineStart);
                    MapKey("end", UIUnityEvents.TextEditOp.MoveGraphicalLineEnd);
                    MapKey("%left", UIUnityEvents.TextEditOp.MoveWordLeft);
                    MapKey("%right", UIUnityEvents.TextEditOp.MoveWordRight);
                    MapKey("%up", UIUnityEvents.TextEditOp.MoveParagraphBackward);
                    MapKey("%down", UIUnityEvents.TextEditOp.MoveParagraphForward);
                    MapKey("^left", UIUnityEvents.TextEditOp.MoveToEndOfPreviousWord);
                    MapKey("^right", UIUnityEvents.TextEditOp.MoveToStartOfNextWord);
                    MapKey("^up", UIUnityEvents.TextEditOp.MoveParagraphBackward);
                    MapKey("^down", UIUnityEvents.TextEditOp.MoveParagraphForward);
                    MapKey("#^left", UIUnityEvents.TextEditOp.SelectToEndOfPreviousWord);
                    MapKey("#^right", UIUnityEvents.TextEditOp.SelectToStartOfNextWord);
                    MapKey("#^up", UIUnityEvents.TextEditOp.SelectParagraphBackward);
                    MapKey("#^down", UIUnityEvents.TextEditOp.SelectParagraphForward);
                    MapKey("#home", UIUnityEvents.TextEditOp.SelectGraphicalLineStart);
                    MapKey("#end", UIUnityEvents.TextEditOp.SelectGraphicalLineEnd);
                    MapKey("^delete", UIUnityEvents.TextEditOp.DeleteWordForward);
                    MapKey("^backspace", UIUnityEvents.TextEditOp.DeleteWordBack);
                    MapKey("^a", UIUnityEvents.TextEditOp.SelectAll);
                    MapKey("^x", UIUnityEvents.TextEditOp.Cut);
                    MapKey("^c", UIUnityEvents.TextEditOp.Copy);
                    MapKey("^v", UIUnityEvents.TextEditOp.Paste);
                    MapKey("#delete", UIUnityEvents.TextEditOp.Cut);
                    MapKey("^insert", UIUnityEvents.TextEditOp.Copy);
                    MapKey("#insert", UIUnityEvents.TextEditOp.Paste);
                }
            }
        }

        private static void MapKey(string key, UIUnityEvents.TextEditOp action)
        {
            Keyactions[Event.KeyboardEvent(key)] = action;
        }

        public static bool textFieldInput
        {
            get
            {
                if (!failedInvokeTextFieldInputGet)
                {
                    try
                    {
                        return (bool) _textFieldInput.GetValue(null, null);
                    }
                    catch (MethodAccessException exception)
                    {
                        failedInvokeTextFieldInputGet = true;
                        Debug.Log("Can not get GUIUtility.textFieldInput\r\n" + exception);
                    }
                }
                return false;
            }
            set
            {
                if (!failedInvokeTextFieldInputSet)
                {
                    try
                    {
                        _textFieldInput.SetValue(null, value, null);
                    }
                    catch (MethodAccessException exception)
                    {
                        failedInvokeTextFieldInputSet = true;
                        Debug.Log("Can not set GUIUtility.textFieldInput\r\n" + exception);
                    }
                }
            }
        }
    }

    private enum TextEditOp
    {
        MoveLeft,
        MoveRight,
        MoveUp,
        MoveDown,
        MoveLineStart,
        MoveLineEnd,
        MoveTextStart,
        MoveTextEnd,
        MovePageUp,
        MovePageDown,
        MoveGraphicalLineStart,
        MoveGraphicalLineEnd,
        MoveWordLeft,
        MoveWordRight,
        MoveParagraphForward,
        MoveParagraphBackward,
        MoveToStartOfNextWord,
        MoveToEndOfPreviousWord,
        SelectLeft,
        SelectRight,
        SelectUp,
        SelectDown,
        SelectTextStart,
        SelectTextEnd,
        SelectPageUp,
        SelectPageDown,
        ExpandSelectGraphicalLineStart,
        ExpandSelectGraphicalLineEnd,
        SelectGraphicalLineStart,
        SelectGraphicalLineEnd,
        SelectWordLeft,
        SelectWordRight,
        SelectToEndOfPreviousWord,
        SelectToStartOfNextWord,
        SelectParagraphBackward,
        SelectParagraphForward,
        Delete,
        Backspace,
        DeleteWordBack,
        DeleteWordForward,
        Cut,
        Copy,
        Paste,
        SelectAll,
        SelectNone,
        ScrollStart,
        ScrollEnd,
        ScrollPageUp,
        ScrollPageDown
    }
}

