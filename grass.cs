using System;

public class grass : ConsoleSystem
{
    [Saved, Client, User]
    public static float disp_trail_seconds = 10f;
    [User, Saved, Client]
    public static bool displacement = (FPGrass.Support.Supported && !FPGrass.Support.DisplacementExpensive);
    [Saved, User, Client]
    public static bool forceredraw = false;
    [Saved, Client, User]
    public static bool on = FPGrass.Support.Supported;

    [Saved, User, Client]
    public static bool shadowcast
    {
        get
        {
            return FPGrass.castShadows;
        }
        set
        {
            FPGrass.castShadows = value;
        }
    }

    [User, Saved, Client]
    public static bool shadowreceive
    {
        get
        {
            return FPGrass.receiveShadows;
        }
        set
        {
            FPGrass.receiveShadows = value;
        }
    }
}

