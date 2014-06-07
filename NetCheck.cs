using System;
using uLink;

public class NetCheck
{
    public static bool PlayerValid(NetworkPlayer ply)
    {
        return ply.isConnected;
    }
}

