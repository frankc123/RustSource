using System;

public class lockentry : ConsoleSystem
{
    [Client]
    public static void hide(ref ConsoleSystem.Arg arg)
    {
        LockEntry.Hide();
    }

    [Client]
    public static void show(ref ConsoleSystem.Arg arg)
    {
        bool result = false;
        bool.TryParse(arg.Args[0], out result);
        LockEntry.Show(result);
    }
}

