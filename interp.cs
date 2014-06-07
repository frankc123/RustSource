using System;

public class interp : ConsoleSystem
{
    [Help("This value adds a fixed amount of delay ( in milliseconds ) to interp delay ( on clients ).", ""), Admin]
    public static int delayms
    {
        get
        {
            ulong delayMillis = Interpolation.delayMillis;
            if (delayMillis > 0x7fffffffL)
            {
                return 0x7fffffff;
            }
            return (int) delayMillis;
        }
        set
        {
            if (value < 0)
            {
                Interpolation.delayMillis = 0L;
            }
            else
            {
                Interpolation.delayMillis = (ulong) value;
            }
        }
    }

    [Admin, Help("This value determins how much time to append to interp delay ( on clients ) based on server.sendrate", "")]
    public static float ratio
    {
        get
        {
            return Interpolation.sendRateRatiof;
        }
        set
        {
            Interpolation.sendRateRatiof = value;
        }
    }
}

