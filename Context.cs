using Facepunch;
using System;
using uLink;
using UnityEngine;

public sealed class Context : MonoBehaviour
{
    private const string kButtonName = "WorldUse";
    public const ulong kQuickTapMillisecondLimit = 300L;
    private const string kRPC_CancelFromServer = "Context:G";
    private const string kRPC_FailedImmediateFromServer = "Context:H";
    private const string kRPC_FailedSelectionFromServer = "Context:J";
    private const string kRPC_NoOpFromServer = "Context:F";
    private const string kRPC_NoSelectionFromClient = "Context:D";
    private const string kRPC_QuickTapFromClient = "Context:B";
    private const string kRPC_ReadOptionsFromServer = "Context:E";
    private const string kRPC_RequestFromClient = "Context:A";
    private const string kRPC_RetryFromServer = "Context:M";
    private const string kRPC_SelectedOptionFromClient = "Context:C";
    private const string kRPC_StaleSelectionFromServer = "Context:L";
    private const string kRPC_SuccessImmediateFromServer = "Context:I";
    private const string kRPC_SuccessSelectionFromServer = "Context:K";
    private const string kRPCPrefix = "Context:";
    private static uLinkNetworkView network;
    private static Context self;
    private static int swallowInputCount;
    private static ContextUI ui;

    public static  event ContextClientWorkingCallback OnClientWorking
    {
        add
        {
            ContextUI.clientWorkingCallbacks = (ContextClientWorkingCallback) Delegate.Combine(ContextUI.clientWorkingCallbacks, value);
            if (Working)
            {
                value(true);
            }
        }
        remove
        {
            ContextUI.clientWorkingCallbacks = (ContextClientWorkingCallback) Delegate.Remove(ContextUI.clientWorkingCallbacks, value);
            if (Working)
            {
                value(false);
            }
        }
    }

    [RPC]
    private void A(NetEntityID hit, NetworkMessageInfo info)
    {
    }

    private void Awake()
    {
        if ((self != null) && (self != this))
        {
            Debug.LogError("More than one", this);
        }
        else
        {
            self = this;
            network = base.GetComponent<uLinkNetworkView>();
            ui = base.GetComponent<ContextUI>();
        }
    }

    [RPC]
    private void B(NetworkMessageInfo info)
    {
    }

    public static bool BeginQuery(Contextual contextual)
    {
        if (self == null)
        {
            Debug.LogWarning("Theres no instance", self);
        }
        else if (ui._clientState != ContextClientState.Off)
        {
            Debug.LogWarning("Client is already in a context menu. Wait", contextual);
        }
        else if (contextual == null)
        {
            Debug.LogWarning("null", self);
        }
        else if (!contextual.exists)
        {
            Debug.LogWarning("requestable destroyed or did not implement monobehaviour", self);
        }
        else
        {
            NetEntityID yid;
            MonoBehaviour implementor = contextual.implementor;
            if (((int) NetEntityID.Of((MonoBehaviour) contextual, out yid)) == 0)
            {
                Debug.LogWarning("requestable has no network view", implementor);
            }
            else
            {
                ui.OnServerQuerySent(implementor, yid);
                return true;
            }
        }
        return false;
    }

    [RPC]
    private void C(int name, NetworkMessageInfo info)
    {
    }

    [RPC]
    private void D(NetworkMessageInfo info)
    {
    }

    [RPC]
    private void E(ContextMenuData options, NetworkMessageInfo info)
    {
        ui.OnServerMenu(options);
    }

    public static void EndQuery()
    {
        if (((self != null) && (ui._clientState > ContextClientState.Off)) && (ui._clientState < ContextClientState.Validating))
        {
            if ((NetCull.localTimeInMillis - ui.clientQueryTime) <= 300L)
            {
                ui.OnServerQuickTapSent();
            }
            else
            {
                ui.OnServerCancelSent();
            }
        }
    }

    [RPC]
    private void F(NetworkMessageInfo info)
    {
        ui.OnServerNoOp();
    }

    [RPC]
    private void G(NetworkMessageInfo info)
    {
        ui.OnServerCancel();
    }

    [RPC]
    private void H(NetworkMessageInfo info)
    {
        ui.OnServerImmediate(false);
    }

    [RPC]
    private void I(NetworkMessageInfo info)
    {
        ui.OnServerImmediate(true);
    }

    [RPC]
    private void J(NetworkMessageInfo info)
    {
        ui.OnServerSelection(false);
    }

    [RPC]
    private void K(NetworkMessageInfo info)
    {
        ui.OnServerSelection(true);
    }

    [RPC]
    private void L(NetworkMessageInfo info)
    {
        ui.OnServerSelectionStale();
    }

    [RPC]
    private void M(NetworkMessageInfo info)
    {
        ui.OnServerRestartPolling();
    }

    private void OnDestroy()
    {
        if (self == this)
        {
            self = null;
            network = null;
            swallowInputCount = 0;
            ui = null;
        }
    }

    public static bool ButtonDown
    {
        get
        {
            if (Input.GetButtonDown("WorldUse"))
            {
                if (swallowInputCount == 0)
                {
                    return !ChatUI.IsVisible();
                }
                swallowInputCount--;
            }
            return false;
        }
    }

    public static bool ButtonUp
    {
        get
        {
            return Input.GetButtonUp("WorldUse");
        }
    }

    public static bool Working
    {
        get
        {
            return ((self != null) && (ui._clientState != ContextClientState.Off));
        }
    }

    public static bool WorkingInMenu
    {
        get
        {
            return (((self != null) && (ui._clientState > ContextClientState.Off)) && (ui._clientState < ContextClientState.Validating));
        }
    }

    internal static class UICommands
    {
        internal static bool IsButtonHeld(bool swallow)
        {
            if (!Input.GetButton("WorldUse"))
            {
                return false;
            }
            if (swallow)
            {
                Context.swallowInputCount = 1;
            }
            return true;
        }

        internal static void Issue_Cancel()
        {
            Context.network.RPC("Context:D", RPCMode.Server, new object[0]);
        }

        internal static void Issue_QuickTap()
        {
            Context.network.RPC("Context:B", RPCMode.Server, new object[0]);
        }

        internal static void Issue_Request(NetEntityID clientQueryEntID)
        {
            Context.network.RPC<NetEntityID>("Context:A", RPCMode.Server, clientQueryEntID);
        }

        internal static void Issue_Selection(int name)
        {
            Context.network.RPC<int>("Context:C", RPCMode.Server, name);
        }
    }
}

