using System;
using System.Text;
using UnityEngine;

public class DigitEntryRestrictions : MonoBehaviour
{
    public void OnKeyDown(dfControl control, dfKeyEventArgs keyEvent)
    {
        if (!char.IsControl(keyEvent.Character) && !char.IsDigit(keyEvent.Character))
        {
            keyEvent.Use();
        }
    }

    public void OnKeyPress(dfControl control, dfKeyEventArgs keyEvent)
    {
        if (!char.IsControl(keyEvent.Character) && !char.IsDigit(keyEvent.Character))
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
            if (char.IsDigit(value[i]))
            {
                builder.Append(value[i]);
            }
        }
        control.Text = builder.ToString();
    }
}

