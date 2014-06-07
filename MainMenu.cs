using Facepunch.Cursor;
using Facepunch.Utility;
using System;
using System.Runtime.InteropServices;
using uLink;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public Camera blurCamera;
    public UnlockCursorNode cursorManager;
    public dfPanel screenOptions;
    public dfPanel screenServers;
    public static MainMenu singleton;

    private void Awake()
    {
        singleton = this;
        LockCursorManager.onEscapeKey += new EscapeKeyEventHandler(this.Show);
        this.screenServers.Hide();
        this.screenOptions.Hide();
    }

    public void DoExit()
    {
        ConsoleSystem.Run("quit", false);
    }

    public void Hide()
    {
        base.GetComponent<dfPanel>().Hide();
        this.cursorManager.On = false;
        this.blurCamera.enabled = false;
        LoadingScreen.Hide();
        HudEnabled.Enable();
    }

    private void HideAllBut(dfPanel but)
    {
        if ((this.screenServers != null) && (this.screenServers != but))
        {
            this.screenServers.Hide();
        }
        if ((this.screenOptions != null) && (this.screenOptions != but))
        {
            this.screenOptions.Hide();
        }
    }

    public static bool IsVisible()
    {
        return ((singleton != null) && singleton.GetComponent<dfPanel>().IsVisible);
    }

    public void LoadBackground()
    {
        Object.DontDestroyOnLoad(base.gameObject.transform.parent.gameObject);
        Application.LoadLevel("MenuBackground");
    }

    private void LogDisconnect(NetError error, NetworkDisconnection? disconnection = new NetworkDisconnection?())
    {
        if (error != NetError.NoError)
        {
            Debug.LogWarning(error);
        }
        if (disconnection.HasValue)
        {
            Debug.Log(disconnection);
        }
    }

    private void OnDestroy()
    {
        LockCursorManager.onEscapeKey -= new EscapeKeyEventHandler(this.Show);
    }

    public void Show()
    {
        base.GetComponent<dfPanel>().Show();
        this.cursorManager.On = true;
        this.blurCamera.enabled = true;
        HudEnabled.Disable();
    }

    public void ShowInformation(string text)
    {
        ConsoleSystem.Run("notice.popup 5 \"\" " + String.QuoteSafe(text), false);
    }

    public void ShowOptions()
    {
        this.HideAllBut(this.screenOptions);
        if (this.screenOptions != null)
        {
            if (this.screenOptions.IsVisible)
            {
                this.screenOptions.Hide();
            }
            else
            {
                this.screenOptions.Show();
            }
            this.screenOptions.SendToBack();
        }
    }

    public void ShowServerlist()
    {
        this.HideAllBut(this.screenServers);
        if (this.screenServers != null)
        {
            if (this.screenServers.IsVisible)
            {
                this.screenServers.Hide();
            }
            else
            {
                this.screenServers.Show();
            }
            this.screenServers.SendToBack();
        }
    }

    private void Start()
    {
        this.cursorManager = LockCursorManager.CreateCursorUnlockNode(false, "Main Menu");
        this.Show();
        if (Object.FindObjectOfType(typeof(ClientConnect)) == null)
        {
            this.LoadBackground();
        }
    }

    private void uLink_OnDisconnectedFromServer(NetworkDisconnection netDisconnect)
    {
        NetError lastKickReason = ServerManagement.GetLastKickReason(true);
        this.LogDisconnect(lastKickReason, new NetworkDisconnection?(netDisconnect));
        DisableOnConnectedState.OnDisconnected();
        ConsoleSystem.Run("gameui.show", false);
        this.LoadBackground();
        if (lastKickReason != NetError.NoError)
        {
            this.ShowInformation("Disconnected (" + lastKickReason.NiceString() + ")");
        }
        else
        {
            this.ShowInformation("Disconnected from server.");
        }
        LoadingScreen.Hide();
    }

    private void uLink_OnFailedToConnect(NetworkConnectionError ulink_error)
    {
        this.LogDisconnect(ulink_error.ToNetError(), null);
        DisableOnConnectedState.OnDisconnected();
        ConsoleSystem.Run("gameui.show", false);
        this.LoadBackground();
        if (ulink_error.ToNetError() != NetError.NoError)
        {
            this.ShowInformation("Failed to connect (" + ulink_error.ToNetError().ToString() + ")");
        }
        else
        {
            this.ShowInformation("Failed to connect.");
        }
        LoadingScreen.Hide();
    }

    private void Update()
    {
        if (NetCull.isClientRunning && Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsVisible())
            {
                this.Hide();
            }
            else
            {
                this.Show();
            }
        }
    }
}

