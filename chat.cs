using System;

public class chat : ConsoleSystem
{
    [Help("Enable or disable chat displaying", ""), Client, Admin]
    public static bool enabled = true;

    [Client]
    public static void add(ref ConsoleSystem.Arg arg)
    {
        if (enabled)
        {
            string name = arg.GetString(0, string.Empty);
            string text = arg.GetString(1, string.Empty);
            if ((name != string.Empty) && (text != string.Empty))
            {
                ChatUI.AddLine(name, text);
            }
        }
    }
}

