namespace Facepunch.Procedural
{
    using System;

    [Flags]
    public enum ClockStatus : byte
    {
        DidElapse = 3,
        Elapsed = 1,
        Negative = 4,
        Unset = 0,
        WillElapse = 2
    }
}

