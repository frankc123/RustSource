using System;

public class deathscreen : ConsoleSystem
{
    [Client]
    public static string reason = "...";

    [Client]
    public static void show(ref ConsoleSystem.Arg arg)
    {
        DeathScreen.Show();
    }
}

