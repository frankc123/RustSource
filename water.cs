using System;

public class water : ConsoleSystem
{
    [Client, Saved]
    public static int level = -1;
    [Saved, Client]
    public static bool reflection;
}

