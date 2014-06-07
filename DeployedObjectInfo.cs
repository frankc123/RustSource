using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct DeployedObjectInfo
{
    public bool valid;
    public ulong userID;
    public PlayerClient playerClient
    {
        get
        {
            PlayerClient client;
            if (!this.valid)
            {
                return null;
            }
            PlayerClient.FindByUserID(this.userID, out client);
            return client;
        }
    }
    public Controllable playerControllable
    {
        get
        {
            if (this.valid)
            {
                PlayerClient playerClient = this.playerClient;
                if (playerClient != null)
                {
                    return playerClient.controllable;
                }
            }
            return null;
        }
    }
    public Character playerCharacter
    {
        get
        {
            if (this.valid)
            {
                Controllable playerControllable = this.playerControllable;
                if (playerControllable != null)
                {
                    return playerControllable.idMain;
                }
            }
            return null;
        }
    }
}

