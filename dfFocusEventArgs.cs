using System;
using System.Runtime.CompilerServices;

public class dfFocusEventArgs : dfControlEventArgs
{
    internal dfFocusEventArgs(dfControl GotFocus, dfControl LostFocus) : base(GotFocus)
    {
        this.LostFocus = LostFocus;
    }

    public dfControl GotFocus
    {
        get
        {
            return base.Source;
        }
    }

    public dfControl LostFocus { get; private set; }
}

