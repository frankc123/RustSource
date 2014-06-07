namespace Facepunch.Procedural
{
    using System;

    [Flags]
    public enum Integration : byte
    {
        Ahead = 4,
        Moved = 2,
        MovedDestination = 3,
        Stationary = 1
    }
}

