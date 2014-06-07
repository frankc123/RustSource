using System;
using System.Runtime.CompilerServices;

public static class GameEvent
{
    public static  event OnPlayerConnectedHandler PlayerConnected;

    public static  event OnGenericEvent QualitySettingsRefresh;

    public static void DoPlayerConnected(PlayerClient player)
    {
        if (PlayerConnected != null)
        {
            PlayerConnected(player);
        }
    }

    public static void DoQualitySettingsRefresh()
    {
        if (QualitySettingsRefresh != null)
        {
            QualitySettingsRefresh();
        }
    }

    public delegate void OnGenericEvent();

    public delegate void OnPlayerConnectedHandler(PlayerClient player);
}

