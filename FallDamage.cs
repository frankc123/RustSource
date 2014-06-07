using System;

internal class falldamage : ConsoleSystem
{
    [Admin, Help("enable/disable fall damage", "")]
    public static bool enabled = true;
    [Help("Average amount of time a leg injury lasts", ""), Admin]
    public static float injury_length = 40f;
    [Help("Fall Velocity when damage of maxhealth will be applied", ""), Admin]
    public static float max_vel = 38f;
    [Help("Fall velocity to begin fall damage calculations - min 18", ""), Admin]
    public static float min_vel = 24f;
}

