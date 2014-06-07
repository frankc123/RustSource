using System;
using System.Text;
using UnityEngine;

public class TextEntryRestrictions : MonoBehaviour
{
    public string allowedChars = "0123456789";

    public void OnKeyDown(dfControl control, dfKeyEventArgs keyEvent)
    {
        if (!char.IsControl(keyEvent.Character) && (this.allowedChars.IndexOf(keyEvent.Character) == -1))
        {
            keyEvent.Use();
        }
    }

    public void OnKeyPress(dfControl control, dfKeyEventArgs keyEvent)
    {
        if (!char.IsControl(keyEvent.Character) && (this.allowedChars.IndexOf(keyEvent.Character) == -1))
        {
            keyEvent.Use();
        }
    }

    public void OnTextChanged(dfTextbox control, string value)
    {
        int cursorIndex = control.CursorIndex;
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < value.Length; i++)
        {
            if (this.allowedChars.IndexOf(value[i]) != -1)
            {
                builder.Append(value[i]);
            }
            else
            {
                cursorIndex = Mathf.Max(0, cursorIndex + 1);
            }
        }
        control.Text = builder.ToString();
        control.CursorIndex = cursorIndex;
    }
}

