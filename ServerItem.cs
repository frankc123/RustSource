using System;
using UnityEngine;

public class ServerItem : MonoBehaviour
{
    public dfButton btnFave;
    public ServerItem selectedItem;
    public ServerBrowser.Server server;
    public dfButton textLabel;
    public dfLabel textPing;
    public dfLabel textPlayers;

    public void Connect()
    {
        Debug.Log("> net.connect " + this.server.address + ":" + this.server.port.ToString());
        ConsoleSystem.Run("net.connect " + this.server.address + ":" + this.server.port.ToString(), false);
    }

    public void Init(ref ServerBrowser.Server s)
    {
        this.server = s;
        this.textLabel.Text = this.server.name;
        this.textPlayers.Text = this.server.currentplayers.ToString() + " / " + this.server.maxplayers.ToString();
        this.textPing.Text = this.server.ping.ToString();
        dfScrollPanel component = base.transform.parent.GetComponent<dfScrollPanel>();
        if (component != null)
        {
            base.GetComponent<dfControl>().Width = component.Width;
            base.GetComponent<dfControl>().ResetLayout(true, false);
        }
        this.UpdateColours();
    }

    public void OnClickFave()
    {
        this.server.fave = !this.server.fave;
        this.UpdateColours();
        base.SendMessageUpwards("UpdateServerList");
        if (this.server.fave)
        {
            ConsoleSystem.Run("serverfavourite.add " + this.server.address + ":" + this.server.port.ToString(), false);
        }
        else
        {
            ConsoleSystem.Run("serverfavourite.remove " + this.server.address + ":" + this.server.port.ToString(), false);
        }
        ConsoleSystem.Run("serverfavourite.save", false);
    }

    public void SelectThis()
    {
        this.selectedItem = this;
    }

    protected void UpdateColours()
    {
        if (this.server.fave)
        {
            this.btnFave.Opacity = 1f;
        }
        else
        {
            this.btnFave.Opacity = 0.2f;
        }
    }
}

