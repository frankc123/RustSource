using System;

public class RustPlayerClient : PlayerClient
{
    protected override void ClientInput()
    {
        if (!MainMenu.IsVisible() && !ConsoleWindow.IsVisible())
        {
            base.ClientInput();
        }
    }
}

