namespace Facepunch.Attributes
{
    using System;

    [Flags]
    public enum PrefabLookupKinds
    {
        All = 0x1f,
        Bundled = 0x10,
        Character = 6,
        Controllable = 4,
        Net = 15,
        NetMain = 7,
        NGC = 8
    }
}

