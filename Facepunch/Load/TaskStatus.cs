namespace Facepunch.Load
{
    using System;

    [Flags]
    public enum TaskStatus : byte
    {
        Complete = 4,
        Downloading = 2,
        Pending = 1
    }
}

