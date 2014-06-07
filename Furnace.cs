using System;

public class Furnace : FireBarrel
{
    protected override float GetCookDuration()
    {
        return 30f;
    }
}

