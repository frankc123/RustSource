using System;

public class footsteps : ConsoleSystem
{
    [Saved, Help("Footstep Quality, 0 = default sound, 1 = dynamic for local, 2 = dynamic for all. 0-2 (default 2)", ""), Client]
    public static int quality = 2;
}

