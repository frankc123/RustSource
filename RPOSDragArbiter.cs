﻿using System;
using UnityEngine;

public abstract class RPOSDragArbiter
{
    protected RPOSDragArbiter()
    {
    }

    public abstract void HoverEnter(GameObject landing);
    public abstract void HoverExit(GameObject landing);
    public abstract void Land(GameObject landing);

    public abstract RPOSInventoryCell Instigator { get; }

    public abstract RPOSInventoryCell Under { get; }
}

