using System;

public class env : ConsoleSystem
{
    [Help("The length of a day in real minutes", ""), Admin]
    public static float daylength = 45f;
    [Admin, Help("The length of a night in real minutes", "")]
    public static float nightlength = 15f;

    [Help("Gets or sets the current time", ""), Admin]
    public static void time(ref ConsoleSystem.Arg arg)
    {
        if (EnvironmentControlCenter.Singleton != null)
        {
            arg.ReplyWith("Current Time: " + EnvironmentControlCenter.Singleton.GetTime().ToString());
        }
    }
}

