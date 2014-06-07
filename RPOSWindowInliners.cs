﻿using System;
using System.Runtime.CompilerServices;

public static class RPOSWindowInliners
{
    public static TRPOSWindow EnsureAwake<TRPOSWindow>(this TRPOSWindow window) where TRPOSWindow: RPOSWindow
    {
        if (window != null)
        {
            RPOSWindow.EnsureAwake(window);
        }
        return window;
    }

    public static bool IsRegistered(this RPOSWindow window)
    {
        return ((window != null) && window.ready);
    }
}

