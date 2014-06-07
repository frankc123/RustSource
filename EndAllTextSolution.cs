using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EndAllTextSolution : MonoBehaviour
{
    public GUIContent content = new GUIContent();
    [SerializeField]
    private int maxLength;
    [SerializeField]
    private bool multiLine;
    [SerializeField]
    private string styleName = "textfield";

    private static void DoTextField(Rect position, int id, GUIContent content, bool multiline, int maxLength, GUIStyle style)
    {
        if ((maxLength >= 0) && (content.text.Length > maxLength))
        {
            content.text = content.text.Substring(0, maxLength);
        }
        GUI2.CheckOnGUI();
        TextEditor stateObject = (TextEditor) GUIUtility.GetStateObject(typeof(TextEditor), id);
        stateObject.content.text = content.text;
        stateObject.SaveBackup();
        stateObject.position = position;
        stateObject.style = style;
        stateObject.multiline = multiline;
        stateObject.controlID = id;
        stateObject.ClampPos();
        Event current = Event.current;
        bool flag = false;
        switch (current.type)
        {
            case EventType.MouseDown:
                if (position.Contains(current.mousePosition))
                {
                    GUIUtility.hotControl = id;
                    GUIUtility.keyboardControl = id;
                    stateObject.MoveCursorToPosition(Event.current.mousePosition);
                    if ((Event.current.clickCount == 2) && skin.settings.doubleClickSelectsWord)
                    {
                        stateObject.SelectCurrentWord();
                        stateObject.DblClickSnap(TextEditor.DblClickSnapping.WORDS);
                        stateObject.MouseDragSelectsWholeWords(true);
                    }
                    if ((Event.current.clickCount == 3) && skin.settings.tripleClickSelectsLine)
                    {
                        stateObject.SelectCurrentParagraph();
                        stateObject.MouseDragSelectsWholeWords(true);
                        stateObject.DblClickSnap(TextEditor.DblClickSnapping.PARAGRAPHS);
                    }
                    current.Use();
                }
                break;

            case EventType.MouseUp:
                if (GUIUtility.hotControl == id)
                {
                    stateObject.MouseDragSelectsWholeWords(false);
                    GUIUtility.hotControl = 0;
                    current.Use();
                }
                break;

            case EventType.MouseDrag:
                if (GUIUtility.hotControl == id)
                {
                    if (!current.shift)
                    {
                        stateObject.SelectToPosition(Event.current.mousePosition);
                    }
                    else
                    {
                        stateObject.MoveCursorToPosition(Event.current.mousePosition);
                    }
                    current.Use();
                    break;
                }
                break;

            case EventType.KeyDown:
                if (GUIUtility.keyboardControl != id)
                {
                    return;
                }
                if (!stateObject.HandleKeyEvent(current))
                {
                    if ((current.keyCode == KeyCode.Tab) || (current.character == '\t'))
                    {
                        return;
                    }
                    char character = current.character;
                    if (((character == '\n') && !multiline) && !current.alt)
                    {
                        return;
                    }
                    Font font = style.font;
                    if (font == null)
                    {
                        font = skin.font;
                    }
                    if (font.HasCharacter(character) || (character == '\n'))
                    {
                        stateObject.Insert(character);
                        flag = true;
                    }
                    else if (character == '\0')
                    {
                        if (Input.compositionString.Length > 0)
                        {
                            stateObject.ReplaceSelection(string.Empty);
                            flag = true;
                        }
                        current.Use();
                    }
                    break;
                }
                current.Use();
                flag = true;
                content.text = stateObject.content.text;
                break;

            case EventType.Repaint:
                if (GUIUtility.keyboardControl != id)
                {
                    style.Draw(position, content, id, false);
                    break;
                }
                stateObject.DrawCursor(content.text);
                break;
        }
        if (GUIUtility.keyboardControl == id)
        {
            GUI2.textFieldInput = true;
        }
        if (flag)
        {
            changed = true;
            content.text = stateObject.content.text;
            if ((maxLength >= 0) && (content.text.Length > maxLength))
            {
                content.text = content.text.Substring(0, maxLength);
            }
            current.Use();
        }
    }

    private void OnGUI()
    {
        int controlID = GUIUtility.GetControlID(FocusType.Keyboard);
        DoTextField(new Rect(0f, 0f, (float) Screen.width, 30f), controlID, this.content, this.multiLine, this.maxLength, this.styleName);
    }

    private static bool changed
    {
        get
        {
            return GUI.changed;
        }
        set
        {
            GUI.changed = value;
        }
    }

    private static GUISkin skin
    {
        get
        {
            return GUI.skin;
        }
    }

    private static class GUI2
    {
        private static readonly object boxed_false = false;
        private static readonly object boxed_true = true;
        public static readonly EndAllTextSolution.VoidCall CheckOnGUI;
        private static readonly PropertyInfo textFieldInputProperty;

        static GUI2()
        {
            MethodInfo method = typeof(GUIUtility).GetMethod("CheckOnGUI", BindingFlags.NonPublic | BindingFlags.Static);
            CheckOnGUI = (EndAllTextSolution.VoidCall) Delegate.CreateDelegate(typeof(EndAllTextSolution.VoidCall), method);
            textFieldInputProperty = typeof(GUIUtility).GetProperty("textFieldInput", BindingFlags.NonPublic | BindingFlags.Static);
        }

        public static bool textFieldInput
        {
            get
            {
                return (bool) textFieldInputProperty.GetValue(null, null);
            }
            set
            {
                textFieldInputProperty.SetValue(null, !value ? boxed_false : boxed_true, null);
            }
        }
    }

    private delegate void VoidCall();
}

