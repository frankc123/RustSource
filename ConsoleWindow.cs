using Facepunch.Cursor;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;

public class ConsoleWindow : MonoBehaviour
{
    public dfTextbox consoleInput;
    public dfLabel consoleOutput;
    public dfScrollbar consoleScroller;
    [NonSerialized]
    public UnlockCursorNode cursorManager = LockCursorManager.CreateCursorUnlockNode(false, "Console Window");
    [NonSerialized]
    protected bool shouldScrollDown = true;
    public static ConsoleWindow singleton;

    public void AddText(string str, bool bFromServer = false)
    {
        if (bFromServer)
        {
            str = "[color #00ffff]" + str + "[/color]\n";
        }
        this.consoleOutput.Text = this.consoleOutput.Text + str + "\n";
        this.TrimBuffer();
        if (this.consoleScroller.Value >= ((this.consoleScroller.MaxValue - this.consoleScroller.ScrollSize) - 50f))
        {
            this.shouldScrollDown = true;
        }
    }

    private void Awake()
    {
        singleton = this;
    }

    private void CaptureLog(string log, string stacktrace, LogType type)
    {
        if ((Application.isPlaying && !log.StartsWith("This uLink evaluation license is temporary.")) && !log.StartsWith("Failed to capture screen shot"))
        {
            if (type == LogType.Log)
            {
                this.AddText("[color #eeeeee]> " + log + "[/color]", false);
            }
            else
            {
                this.AddText("[color #ff0000]> " + log + "[/color]", false);
            }
            if (!log.StartsWith("Resynchronize Clock is still in progress") && ((type == LogType.Exception) || (type == LogType.Error)))
            {
                if (stacktrace.Length > 8)
                {
                    Client_Error(log, stacktrace);
                }
                else
                {
                    string strTrace = StackTraceUtility.ExtractStackTrace();
                    if (strTrace.Length > 8)
                    {
                        Client_Error(log, strTrace);
                    }
                    else
                    {
                        Client_Error(log, new StackTrace().ToString());
                    }
                }
            }
        }
    }

    [DllImport("librust")]
    public static extern void Client_Error(string strLog, string strTrace);
    public static bool IsVisible()
    {
        return ((singleton != null) && singleton.GetComponent<dfPanel>().IsVisible);
    }

    private void OnDestroy()
    {
        ConsoleSystem.UnregisterLogCallback(new Application.LogCallback(this.CaptureLog));
    }

    public void OnInput()
    {
        string text = this.consoleInput.Text;
        this.consoleInput.Text = string.Empty;
        this.RunCommand(text);
    }

    public void RunCommand(string strInput)
    {
        this.AddText("[color #00ff00]> " + strInput + "[/color]", false);
        string strOutput = string.Empty;
        if (ConsoleSystem.RunCommand_Clientside(strInput, out strOutput, true))
        {
            if (strOutput != string.Empty)
            {
                this.AddText("[color #ffff00]" + strOutput + "[/color]", false);
            }
        }
        else
        {
            ConsoleNetworker.SendCommandToServer(strInput);
        }
    }

    private void Start()
    {
        ConsoleSystem.RegisterLogCallback(new Application.LogCallback(this.CaptureLog), false);
        singleton.GetComponent<dfPanel>().Hide();
    }

    private void TrimBuffer()
    {
        if (this.consoleOutput.Text.Length >= 0x1388)
        {
            int index = this.consoleOutput.Text.IndexOf('\n');
            if (index != -1)
            {
                this.consoleOutput.Text = this.consoleOutput.Text.Substring(index + 1);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (IsVisible())
            {
                this.consoleInput.Unfocus();
                singleton.GetComponent<dfPanel>().Hide();
                this.cursorManager.On = false;
            }
            else
            {
                singleton.GetComponent<dfPanel>().Show();
                singleton.GetComponent<dfPanel>().BringToFront();
                this.consoleInput.Focus();
                this.cursorManager.On = true;
            }
            this.consoleInput.Text = string.Empty;
        }
        else if (this.shouldScrollDown && (this.consoleScroller.Value != (this.consoleScroller.MaxValue - this.consoleScroller.ScrollSize)))
        {
            this.consoleScroller.Value = this.consoleScroller.MaxValue;
            this.shouldScrollDown = false;
        }
    }
}

