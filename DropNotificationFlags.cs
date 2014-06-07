using System;

[Flags]
public enum DropNotificationFlags
{
    AltDrop = 8,
    AltLand = 0x10,
    AltReverse = 0x20,
    DragDrop = 1,
    DragHover = 0x200,
    DragLand = 2,
    DragLandOutside = 0x2000,
    DragReverse = 4,
    LandHover = 0x400,
    MidDrop = 0x40,
    MidLand = 0x80,
    MidReverse = 0x100,
    RegularHover = 0x1000,
    ReverseHover = 0x800
}

