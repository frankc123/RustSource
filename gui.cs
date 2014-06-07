using System;
using UnityEngine;

public class gui : ConsoleSystem
{
    [Help("Hides all GUI (useful for taking screenshots)", ""), Client]
    public static void hide(ref ConsoleSystem.Arg args)
    {
        GUIHide.SetVisible(false);
    }

    [Help("Hides the alpha/branding on the top right", ""), Client]
    public static void hide_branding(ref ConsoleSystem.Arg args)
    {
        GameObject obj2 = GameObject.Find("BrandingPanel");
        if (obj2 != null)
        {
            obj2.GetComponent<dfPanel>().Hide();
        }
    }

    [Help("The opposite of gui.hide", ""), Client]
    public static void show(ref ConsoleSystem.Arg args)
    {
        GUIHide.SetVisible(true);
    }

    [Client, Help("The opposite of gui.hide_branding", "")]
    public static void show_branding(ref ConsoleSystem.Arg args)
    {
        GameObject obj2 = GameObject.Find("BrandingPanel");
        if (obj2 != null)
        {
            obj2.GetComponent<dfPanel>().Show();
        }
    }
}

