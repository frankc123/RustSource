using System;
using UnityEngine;

public class LaserModRep : WeaponModRep
{
    private static bool allow_3rd_lasers = true;
    private LaserBeam[] beams;
    private bool is_vm;
    private const float kMaxAudioDistance = 4f;
    private const float kMinAudioDistance = 1f;
    private const AudioRolloffMode kRolloffMode = AudioRolloffMode.Logarithmic;
    private const float kVolume = 1f;

    public LaserModRep() : this(ItemModRepresentation.Caps.BindStateFlags, false)
    {
    }

    protected LaserModRep(ItemModRepresentation.Caps caps) : this(caps, false)
    {
    }

    protected LaserModRep(ItemModRepresentation.Caps caps, bool defaultOn) : base(caps, defaultOn)
    {
    }

    protected override void BindStateFlags(CharacterStateFlags flags, ItemModRepresentation.Reason reason)
    {
        base.BindStateFlags(flags, reason);
        base.SetOn(flags.laser, reason);
    }

    protected override void DisableMod(ItemModRepresentation.Reason reason)
    {
        LaserBeam anyBeam = null;
        foreach (LaserBeam beam2 in this.beams)
        {
            if (beam2 != null)
            {
                anyBeam = beam2;
                beam2.enabled = false;
            }
        }
        if (reason == ItemModRepresentation.Reason.Explicit)
        {
            this.PlaySound(anyBeam, base.modDataBlock.offSound);
        }
    }

    protected override void EnableMod(ItemModRepresentation.Reason reason)
    {
        LaserBeam anyBeam = null;
        foreach (LaserBeam beam2 in this.beams)
        {
            if (beam2 != null)
            {
                anyBeam = beam2;
                beam2.enabled = this.is_vm || allow_3rd_lasers;
            }
        }
        if (reason == ItemModRepresentation.Reason.Explicit)
        {
            this.PlaySound(anyBeam, base.modDataBlock.onSound);
        }
    }

    protected override void OnAddAttached()
    {
        this.beams = base.attached.GetComponentsInChildren<LaserBeam>();
    }

    protected override void OnRemoveAttached()
    {
        this.beams = null;
    }

    private void PlaySound(LaserBeam anyBeam, AudioClip clip)
    {
        if (anyBeam != null)
        {
            clip.PlayLocal(anyBeam.transform, Vector3.zero, 1f, AudioRolloffMode.Logarithmic, 1f, 4f);
        }
        else
        {
            clip.PlayLocal(base.itemRep.transform, Vector3.zero, 1f, AudioRolloffMode.Logarithmic, 1f, 4f);
        }
    }

    public override void SetAttached(GameObject attached, bool vm)
    {
        this.is_vm = vm;
        base.SetAttached(attached, vm);
    }

    protected override bool VerifyCompatible(GameObject attachment)
    {
        return attachment.GetComponentInChildren<LaserBeam>();
    }
}

