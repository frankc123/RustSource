using Facepunch.Cursor;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;

internal class ContextUI : MonoBehaviour
{
    [NonSerialized]
    internal ContextClientState _clientState;
    [NonSerialized]
    internal ContextClientStage clientContext;
    [NonSerialized]
    internal MonoBehaviour clientQuery;
    [NonSerialized]
    internal ulong clientQueryTime;
    [NonSerialized]
    internal int clientSelection;
    [NonSerialized]
    internal UnlockCursorNode clientUnlock;
    [NonSerialized]
    internal static ContextClientWorkingCallback clientWorkingCallbacks;
    [SerializeField]
    private GUISkin skin;
    private static GUIContent temp = new GUIContent();
    [NonSerialized]
    internal string validatingString;

    private void Awake()
    {
        base.useGUILayout = false;
        this.clientUnlock = LockCursorManager.CreateCursorUnlockNode(false, UnlockCursorFlags.DoNotResetInput, "Context Popup");
    }

    private static Rect BoxRect(Vector2 contentSize, GUIStyle box, out int xOffset, out int yOffset)
    {
        Rect rect = box.padding.Add(new Rect(0f, 0f, contentSize.x, contentSize.y));
        int num = Mathf.FloorToInt((Screen.width - rect.width) * 0.5f);
        int num2 = Mathf.FloorToInt((Screen.height - rect.height) * 0.5f);
        rect.x += num;
        rect.y += num2;
        Rect rect2 = box.padding.Remove(rect);
        xOffset = Mathf.FloorToInt(rect2.x);
        yOffset = Mathf.FloorToInt(rect2.y);
        return rect;
    }

    private int GUIOptions(GUIStyle box, GUIStyle button)
    {
        int num4;
        int num5;
        Rect[] rectArray = new Rect[this.clientContext.length];
        int? nullable = (button.fixedWidth != 0f) ? new int?((int) button.fixedWidth) : null;
        int? nullable2 = (button.fixedHeight != 0f) ? new int?((int) button.fixedHeight) : null;
        float negativeInfinity = float.NegativeInfinity;
        float y = 0f;
        for (int i = 0; i < this.clientContext.length; i++)
        {
            Rect rect3;
            temp.text = this.clientContext.option[i].text;
            Vector2 vector = button.CalcSize(temp);
            rectArray[i] = rect3 = button.padding.Add(new Rect(0f, 0f, !nullable.HasValue ? ((float) Mathf.CeilToInt(vector.x)) : ((float) nullable.Value), !nullable2.HasValue ? ((float) Mathf.CeilToInt(vector.y)) : ((float) nullable2.Value)));
            Rect rect = button.margin.Add(rect3);
            if (rect.width > negativeInfinity)
            {
                negativeInfinity = rect.width;
            }
            y += rect.height;
        }
        GUI.Box(BoxRect(new Vector2(negativeInfinity, y), box, out num4, out num5), GUIContent.none, box);
        int num6 = -1;
        for (int j = 0; j < this.clientContext.length; j++)
        {
            Rect rect2 = button.margin.Add(rectArray[j]);
            rect2.width = negativeInfinity;
            rect2.x = num4;
            rect2.y = num5;
            num5 = Mathf.FloorToInt(button.margin.Add(rect2).yMax);
            if (GUI.Button(button.margin.Remove(rect2), this.clientContext.option[j].text, button))
            {
                num6 = j;
            }
        }
        return num6;
    }

    private static void GUIString(string text, GUIStyle box)
    {
        int num;
        int num2;
        GUI.Box(BoxRect(box.CalcSize(temp), box, out num, out num2), temp, box);
    }

    [Conditional("CLIENT_POPUP_LOG")]
    private static void LOG(string shorthand, Object contextual)
    {
    }

    private void OnClientHideMenu()
    {
        base.CancelInvoke("OnClientShowMenu");
        if (((int) this.clientUnlock.TryLock()) == 0)
        {
            Context.UICommands.IsButtonHeld(true);
            Input.ResetInputAxes();
        }
        base.enabled = false;
    }

    private void OnClientOptionsCleared()
    {
        if (this.clientSelection != -1)
        {
            this.clientSelection = -1;
        }
        this.clientContext.length = 0;
    }

    private void OnClientOptionsMade()
    {
        if (this._clientState != ContextClientState.Validating)
        {
            ulong num = NetCull.localTimeInMillis - this.clientQueryTime;
            if (num > 300L)
            {
                this.OnClientShowMenu();
            }
            else
            {
                base.Invoke("OnClientShowMenu", (float) (((double) (num + ((ulong) 50L))) / 1000.0));
            }
            this.SetContextClientState(ContextClientState.Options);
        }
    }

    private void OnClientPromptBegin(NetEntityID? useID)
    {
        NetEntityID yid;
        if (useID.HasValue)
        {
            yid = useID.Value;
        }
        else
        {
            NetEntityID.Of(this.clientQuery, out yid);
        }
        this.clientQueryTime = NetCull.localTimeInMillis;
        Context.UICommands.Issue_Request(yid);
        this.SetContextClientState(ContextClientState.Polling);
    }

    private void OnClientPromptEnd()
    {
        this.OnClientHideMenu();
        this.SetContextClientState(ContextClientState.Off);
    }

    private void OnClientSelection(int i)
    {
        this.clientSelection = i;
        Context.UICommands.Issue_Selection(this.clientContext.option[i].name);
        this.validatingString = this.clientContext.option[i].text + "..";
        this.SetContextClientState(ContextClientState.Validating);
    }

    private void OnClientShowMenu()
    {
        this.clientSelection = -1;
        this.clientUnlock.On = true;
        base.enabled = true;
    }

    private void OnClientValidated()
    {
        this.SetContextClientState(ContextClientState.Off);
    }

    private void OnDestroy()
    {
        this.clientUnlock.Dispose();
        this.clientUnlock = null;
    }

    private void OnDisable()
    {
        LockCursorManager.DisableGUICheckOnDisable(this);
    }

    private void OnEnable()
    {
        LockCursorManager.DisableGUICheckOnEnable(this);
    }

    private void OnGUI()
    {
        GUI.depth = 1;
        GUI.skin = this.skin;
        GUIStyle box = "ctxbox";
        GUIStyle button = "ctxbutton";
        int i = -1;
        switch (this.clientState)
        {
            case ContextClientState.Options:
                i = this.GUIOptions(box, button);
                if (((i == -1) && ((NetCull.localTimeInMillis - this.clientQueryTime) > 300L)) && !Context.UICommands.IsButtonHeld(false))
                {
                    Context.EndQuery();
                }
                break;

            case ContextClientState.Validating:
                GUIString(this.validatingString, box);
                break;

            default:
                return;
        }
        if (i != -1)
        {
            this.OnClientSelection(i);
        }
    }

    internal void OnServerCancel()
    {
        this.OnClientPromptEnd();
    }

    internal void OnServerCancelSent()
    {
        Context.UICommands.Issue_Cancel();
        if (this._clientState == ContextClientState.Options)
        {
            this.OnClientHideMenu();
        }
        this.SetContextClientState(ContextClientState.Validating);
    }

    internal void OnServerImmediate(bool success)
    {
        if (success)
        {
            this.OnClientValidated();
            this.OnClientOptionsCleared();
            this.OnClientPromptEnd();
        }
        else
        {
            this.OnClientPromptEnd();
        }
    }

    internal void OnServerMenu(ContextMenuData menu)
    {
        this.clientContext.Set(menu);
        this.OnClientOptionsMade();
    }

    internal void OnServerNoOp()
    {
        this.clientContext.length = 0;
        this.OnClientPromptEnd();
    }

    internal void OnServerQuerySent(MonoBehaviour script, NetEntityID entID)
    {
        this.clientQuery = script;
        this.OnClientPromptBegin(new NetEntityID?(entID));
    }

    internal void OnServerQuickTapSent()
    {
        Context.UICommands.Issue_QuickTap();
        if (this._clientState == ContextClientState.Options)
        {
            this.OnClientHideMenu();
        }
        this.SetContextClientState(ContextClientState.Validating);
    }

    internal void OnServerRestartPolling()
    {
        this.OnClientOptionsCleared();
        this.SetContextClientState(ContextClientState.Polling);
        this.OnClientOptionsMade();
    }

    internal void OnServerSelection(bool success)
    {
        if (success)
        {
            this.OnClientValidated();
            this.OnClientOptionsCleared();
            this.OnClientPromptEnd();
        }
        else
        {
            this.OnClientPromptEnd();
        }
    }

    internal void OnServerSelectionStale()
    {
        this.OnClientPromptEnd();
    }

    private void SetContextClientState(ContextClientState state)
    {
        if (this._clientState != state)
        {
            if (this._clientState == ContextClientState.Off)
            {
                this._clientState = state;
                if (clientWorkingCallbacks != null)
                {
                    clientWorkingCallbacks(true);
                }
            }
            else if (state == ContextClientState.Off)
            {
                this._clientState = state;
                if (clientWorkingCallbacks != null)
                {
                    clientWorkingCallbacks(false);
                }
            }
            else
            {
                this._clientState = state;
            }
        }
    }

    internal ContextClientState clientState
    {
        get
        {
            return this._clientState;
        }
    }
}

