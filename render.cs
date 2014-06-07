using System;
using UnityEngine;

public class render : ConsoleSystem
{
    private static float distance_real = 0.2f;
    private static int frames_real = -1;
    private static bool vsync_real;

    private static void update()
    {
        QualitySettings.vSyncCount = !vsync_real ? 0 : 1;
        Application.targetFrameRate = frames_real;
        int qualityLevel = QualitySettings.GetQualityLevel();
        if (PlayerPrefs.GetInt("UnityGraphicsQualityBackup", -1) != qualityLevel)
        {
            PlayerPrefs.SetInt("UnityGraphicsQualityBackup", qualityLevel);
            switch (qualityLevel)
            {
                case 0:
                case 1:
                case 2:
                    gfx.ssaa = false;
                    gfx.bloom = false;
                    gfx.ssao = false;
                    gfx.tonemap = false;
                    gfx.shafts = false;
                    goto Label_00D1;

                case 3:
                    gfx.ssaa = false;
                    gfx.bloom = false;
                    gfx.ssao = false;
                    gfx.tonemap = false;
                    gfx.shafts = true;
                    goto Label_00D1;

                case 4:
                    goto Label_00D1;
            }
            gfx.ssaa = true;
            gfx.bloom = true;
            gfx.ssao = true;
            gfx.tonemap = true;
            gfx.shafts = true;
        }
    Label_00D1:
        GameEvent.DoQualitySettingsRefresh();
    }

    [Client, Help("Makes sure settings match their convar values. You shouldn't need to call this manually.", "")]
    public static void update(ref ConsoleSystem.Arg args)
    {
        update();
    }

    [Client, Saved, Help("The relative render distance. (0-1)", "")]
    public static float distance
    {
        get
        {
            return distance_real;
        }
        set
        {
            distance_real = Mathf.Clamp01(value);
            update();
        }
    }

    [Client, Help("The limit for how many frames may be rendered per second. (default -1 for no fps limit)", ""), Saved]
    public static int frames
    {
        get
        {
            return frames_real;
        }
        set
        {
            frames_real = value;
            update();
        }
    }

    [Help("The render quality level. (0-1)", ""), Client]
    public static float level
    {
        get
        {
            return (((float) QualitySettings.GetQualityLevel()) / ((float) (QualitySettings.names.Length - 1)));
        }
        set
        {
            QualitySettings.SetQualityLevel(Mathf.RoundToInt(value * (QualitySettings.names.Length - 1)), true);
            update();
        }
    }

    [Help("Whether VSync should be enabled or disabled", ""), Client, Saved]
    public static bool vsync
    {
        get
        {
            return vsync_real;
        }
        set
        {
            vsync_real = value;
            update();
        }
    }
}

