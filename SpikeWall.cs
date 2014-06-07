using System;
using System.Collections.Generic;

public class SpikeWall : IDLocal
{
    public List<TakeDamage> _touching;
    public float baseReturnDmg = 5f;
    public float dmgPerTick = 20f;
    public float returnFraction = 0.2f;
    private bool running;
}

