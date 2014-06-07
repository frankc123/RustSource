namespace NGUI.Meshing
{
    using System;

    public enum PrimitiveKind : byte
    {
        Grid1x1 = 1,
        Grid1x2 = 3,
        Grid1x3 = 5,
        Grid2x1 = 2,
        Grid2x2 = 4,
        Grid2x3 = 8,
        Grid3x1 = 6,
        Grid3x2 = 7,
        Grid3x3 = 9,
        Hole3x3 = 10,
        Invalid = 0xff,
        Quad = 1,
        Triangle = 0
    }
}

