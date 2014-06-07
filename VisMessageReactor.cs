using System;
using UnityEngine;

public sealed class VisMessageReactor : VisReactor
{
    public string awareEnter = "Vis_Sight_Enter";
    public string awareExit = "Vis_Sight_Exit";
    public GameObject messageReceiver;
    public string seeAdd = "Vis_Sight_Add";
    public string seeRemove = "Vis_Sight_Remove";
    public string spectatedEnter = "Vis_Spect_Enter";
    public string spectatedExit = "Vis_Spect_Exit";
    public string spectatorAdd = "Vis_Spect_Add";
    public string spectatorRemove = "Vis_Spect_Remove";

    private void Exec(string message, VisMessageInfo.Kind kind)
    {
        this.Exec(message, null, kind);
    }

    private void Exec(string message, VisNode arg, VisMessageInfo.Kind kind)
    {
    }

    protected override void React_AwareEnter()
    {
        this.Exec(this.awareEnter, VisMessageInfo.Kind.SeeEnter);
    }

    protected override void React_AwareExit()
    {
        this.Exec(this.awareEnter, VisMessageInfo.Kind.SeeExit);
    }

    protected override void React_SeeAdd(VisNode node)
    {
        this.Exec(this.awareEnter, node, VisMessageInfo.Kind.SeeAdd);
    }

    protected override void React_SeeRemove(VisNode node)
    {
        this.Exec(this.awareEnter, node, VisMessageInfo.Kind.SeeRemove);
    }

    protected override void React_SpectatedEnter()
    {
        this.Exec(this.awareEnter, VisMessageInfo.Kind.SpectatedEnter);
    }

    protected override void React_SpectatedExit()
    {
        this.Exec(this.awareEnter, VisMessageInfo.Kind.SpectatorExit);
    }

    protected override void React_SpectatorAdd(VisNode node)
    {
        this.Exec(this.awareEnter, node, VisMessageInfo.Kind.SpectatorAdd);
    }

    protected override void React_SpectatorRemove(VisNode node)
    {
        this.Exec(this.awareEnter, node, VisMessageInfo.Kind.SpectatorRemove);
    }

    private void Reset()
    {
        base.Reset();
    }
}

