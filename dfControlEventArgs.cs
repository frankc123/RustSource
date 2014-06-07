using System;
using System.Runtime.CompilerServices;

public class dfControlEventArgs
{
    internal dfControlEventArgs(dfControl Target)
    {
        this.Source = Target;
    }

    public void Use()
    {
        this.Used = true;
    }

    public dfControl Source { get; private set; }

    public bool Used { get; private set; }
}

