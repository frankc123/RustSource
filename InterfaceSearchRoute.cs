using System;

[Flags]
public enum InterfaceSearchRoute
{
    Children = 2,
    GameObject = 1,
    Parents = 4,
    Remote = 0x10,
    Root = 8
}

