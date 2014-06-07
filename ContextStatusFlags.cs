using System;

[Flags]
public enum ContextStatusFlags
{
    ObjectBroken = 2,
    ObjectBusy = 1,
    ObjectEmpty = 4,
    ObjectOccupied = 8,
    SpriteFlag0 = 0x20000000,
    SpriteFlag1 = 0x40000000
}

