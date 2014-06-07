using System;

public class censor : ConsoleSystem
{
    [User, Client]
    public static bool nudity
    {
        get
        {
            return ArmorModelRenderer.Censored;
        }
        set
        {
            ArmorModelRenderer.Censored = value;
        }
    }
}

