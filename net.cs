using System;
using UnityEngine;

internal class net : ConsoleSystem
{
    [Client, Help("connect to a server", "string serverurl")]
    public static void connect(ref ConsoleSystem.Arg arg)
    {
        if (Object.FindObjectOfType(typeof(ClientConnect)) != null)
        {
            Debug.Log("Connect already in progress!");
        }
        else if (NetCull.isClientRunning)
        {
            Debug.Log("Use net.disconnect before trying to connect to a new server.");
        }
        else
        {
            char[] separator = new char[] { ':' };
            string[] strArray = arg.GetString(0, string.Empty).Split(separator);
            if (strArray.Length != 2)
            {
                Debug.Log("Not a valid ip - or port missing");
            }
            else
            {
                string strURL = strArray[0];
                int iPort = int.Parse(strArray[1]);
                Debug.Log(string.Concat(new object[] { "Connecting to ", strURL, ":", iPort }));
                PlayerPrefs.SetString("net.lasturl", arg.GetString(0, string.Empty));
                if (ClientConnect.Instance().DoConnect(strURL, iPort))
                {
                    LoadingScreen.Show();
                    LoadingScreen.Update("connecting..");
                }
            }
        }
    }

    [Help("disconnect from server", ""), Client]
    public static void disconnect(ref ConsoleSystem.Arg arg)
    {
        if (!NetCull.isClientRunning)
        {
            Debug.Log("You're not connected to a server.");
        }
        else
        {
            NetCull.Disconnect();
        }
    }

    [Help("reconnect to last server", ""), Client]
    public static void reconnect(ref ConsoleSystem.Arg arg)
    {
        if (PlayerPrefs.HasKey("net.lasturl"))
        {
            ConsoleSystem.Run("net.connect " + PlayerPrefs.GetString("net.lasturl"), false);
        }
        else
        {
            Debug.Log("You havn't connected to a server yet");
        }
    }
}

