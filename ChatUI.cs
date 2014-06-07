using Facepunch.Cursor;
using Facepunch.Utility;
using System;
using UnityEngine;

public class ChatUI : MonoBehaviour
{
    public dfPanel chatContainer;
    public Object chatLine;
    public static ChatUI singleton;
    public dfTextbox textInput;
    [NonSerialized]
    private UnlockCursorNode unlockNode;

    public static void AddLine(string name, string text)
    {
        if (singleton != null)
        {
            GameObject obj2 = (GameObject) Object.Instantiate(singleton.chatLine);
            if (obj2 != null)
            {
                ChatLine component = obj2.GetComponent<ChatLine>();
                component.Setup(name + ":", text);
                singleton.chatContainer.AddControl(component.GetComponent<dfPanel>());
            }
        }
    }

    private void Awake()
    {
        this.unlockNode = LockCursorManager.CreateCursorUnlockNode(false, "ChatUI");
    }

    public void CancelChatting()
    {
        this.textInput.Text = string.Empty;
        singleton.Invoke("CancelChatting_Delayed", 0.2f);
    }

    public void CancelChatting_Delayed()
    {
        this.unlockNode.TryLock();
        this.textInput.Text = string.Empty;
        this.textInput.Unfocus();
        this.textInput.Hide();
    }

    public void ClearText()
    {
        this.textInput.Text = string.Empty;
    }

    public static void Close()
    {
        if (singleton != null)
        {
            singleton.CancelChatting();
        }
    }

    public static bool IsVisible()
    {
        if (singleton == null)
        {
            return false;
        }
        return singleton.textInput.IsVisible;
    }

    private void OnDestroy()
    {
        if (this.unlockNode != null)
        {
            this.unlockNode.Dispose();
            this.unlockNode = null;
        }
    }

    private void OnLoseFocus()
    {
        this.CancelChatting();
    }

    public static void Open()
    {
        if (singleton != null)
        {
            singleton.textInput.Text = string.Empty;
            singleton.textInput.Show();
            singleton.textInput.Focus();
            singleton.Invoke("ClearText", 0.05f);
            if (singleton.unlockNode != null)
            {
                singleton.unlockNode.On = true;
            }
        }
    }

    public void ReLayout()
    {
        this.chatContainer.RelativePosition = (Vector3) new Vector2(0f, 0f);
        dfPanel[] componentsInChildren = this.chatContainer.GetComponentsInChildren<dfPanel>();
        float num = 0f;
        foreach (dfPanel panel in componentsInChildren)
        {
            if (panel.gameObject != this.chatContainer.gameObject)
            {
                num += panel.Height;
            }
        }
        Vector2 vector = new Vector2(0f, this.chatContainer.Height - num);
        foreach (dfPanel panel2 in componentsInChildren)
        {
            if (panel2.gameObject != this.chatContainer.gameObject)
            {
                panel2.RelativePosition = (Vector3) vector;
                vector.y += panel2.Height;
            }
        }
    }

    public void SendChat()
    {
        if (this.textInput.Text != string.Empty)
        {
            ConsoleNetworker.SendCommandToServer("chat.say " + String.QuoteSafe(this.textInput.Text));
        }
        this.CancelChatting();
    }

    private void Start()
    {
        singleton = this;
        this.textInput.Hide();
    }
}

