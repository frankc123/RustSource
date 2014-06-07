using System;
using UnityEngine;

public class OptionsKeyBinding : MonoBehaviour
{
    protected static dfRichTextLabel doingKeyListen;
    public dfRichTextLabel keyOne;
    public dfRichTextLabel keyTwo;
    public dfRichTextLabel labelName;
    protected static string strPreviousValue = string.Empty;

    private KeyCode FetchKey()
    {
        for (int i = 0; i < 0x1ad; i++)
        {
            if (Input.GetKey((KeyCode) i))
            {
                return (KeyCode) i;
            }
        }
        return KeyCode.None;
    }

    public void OnClickOne()
    {
        this.StartKeyListen(this.keyOne);
    }

    public void OnClickTwo()
    {
        this.StartKeyListen(this.keyTwo);
    }

    public void Setup(GameInput.GameButton button)
    {
        this.labelName.Text = button.Name;
        this.keyOne.Text = button.bindingOne.ToString();
        this.keyTwo.Text = button.bindingTwo.ToString();
        if (this.keyOne.Text == "None")
        {
            this.keyOne.Text = " ";
        }
        if (this.keyTwo.Text == "None")
        {
            this.keyTwo.Text = " ";
        }
    }

    private void StartKeyListen(dfRichTextLabel key)
    {
        if (doingKeyListen == null)
        {
            strPreviousValue = key.Text;
            key.Text = "...";
            doingKeyListen = key;
        }
    }

    private void Update()
    {
        if (((doingKeyListen == this.keyOne) || (doingKeyListen == this.keyTwo)) && Input.anyKeyDown)
        {
            KeyCode code = this.FetchKey();
            switch (code)
            {
                case KeyCode.None:
                    return;

                case KeyCode.Escape:
                    doingKeyListen.Text = " ";
                    break;

                default:
                    doingKeyListen.Text = code.ToString();
                    break;
            }
            doingKeyListen = null;
        }
    }

    public void UpdateConVars()
    {
        string text = "None";
        string str2 = "None";
        if ((this.keyOne.Text.Length > 0) && (this.keyOne.Text != " "))
        {
            text = this.keyOne.Text;
        }
        if ((this.keyTwo.Text.Length > 0) && (this.keyTwo.Text != " "))
        {
            str2 = this.keyTwo.Text;
        }
        ConsoleSystem.Run("input.bind " + this.labelName.Text + " " + text + " " + str2 + string.Empty, false);
    }
}

