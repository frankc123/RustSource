using System;

public class gameui : ConsoleSystem
{
    [Client]
    public static void hide(ref ConsoleSystem.Arg arg)
    {
        MainMenu.singleton.Hide();
    }

    [Client]
    public static void show(ref ConsoleSystem.Arg arg)
    {
        MainMenu.singleton.Show();
    }
}

