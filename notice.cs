using System;

public class notice : ConsoleSystem
{
    [Client]
    public static void inventory(ref ConsoleSystem.Arg arg)
    {
        string strText = arg.GetString(0, "This is the text");
        PopupUI.singleton.CreateInventory(strText);
    }

    [Client]
    public static void popup(ref ConsoleSystem.Arg arg)
    {
        float @float = arg.GetFloat(0, 2f);
        string strIcon = arg.GetString(1, "!");
        string strText = arg.GetString(2, "This is the text");
        PopupUI.singleton.CreateNotice(@float, strIcon, strText);
    }

    [Client]
    public static void test(ref ConsoleSystem.Arg arg)
    {
        PopupUI.singleton.StartCoroutine("DoTests");
    }
}

