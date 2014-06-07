using System;

public class SilencerModRep : WeaponModRep
{
    public SilencerModRep() : this(0, true)
    {
    }

    protected SilencerModRep(ItemModRepresentation.Caps caps) : this(caps, true)
    {
    }

    protected SilencerModRep(ItemModRepresentation.Caps caps, bool defaultOn) : base(caps, defaultOn)
    {
    }

    protected override void DisableMod(ItemModRepresentation.Reason reason)
    {
    }

    protected override void EnableMod(ItemModRepresentation.Reason reason)
    {
    }
}

