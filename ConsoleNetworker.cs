using Facepunch;
using System;
using uLink;
using UnityEngine;

public class ConsoleNetworker : MonoBehaviour
{
    public static ConsoleNetworker singleton;

    private void Awake()
    {
        singleton = this;
    }

    [RPC]
    public void CL_ConsoleCommand(string message, NetworkMessageInfo info)
    {
        ConsoleWindow window = (ConsoleWindow) Object.FindObjectOfType(typeof(ConsoleWindow));
        if ((window != null) && !ConsoleSystem.Run(message, false))
        {
            Debug.Log("Unhandled command from server: " + message);
        }
    }

    [RPC]
    public void CL_ConsoleMessage(string message, NetworkMessageInfo info)
    {
        ConsoleWindow window = (ConsoleWindow) Object.FindObjectOfType(typeof(ConsoleWindow));
        if (window != null)
        {
            window.AddText(message, true);
        }
    }

    public static void SendCommandToServer(string strCommand)
    {
        if (singleton != null)
        {
            singleton.networkView.RPC<string>("SV_RunConsoleCommand", RPCMode.Server, strCommand);
        }
    }

    [RPC]
    public void SV_RunConsoleCommand(string cmd, NetworkMessageInfo info)
    {
    }
}

