using System;
using UnityEngine;

public class RunConsoleCommand : MonoBehaviour
{
    public bool asIfTypedIntoConsole;
    public string consoleCommand = "echo Missing Console Command!";

    public void RunCommandImmediately()
    {
        if (this.asIfTypedIntoConsole)
        {
            ConsoleWindow.singleton.RunCommand(this.consoleCommand);
        }
        else
        {
            ConsoleSystem.Run(this.consoleCommand, false);
        }
    }
}

