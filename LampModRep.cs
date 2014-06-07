using System;
using UnityEngine;

public class LampModRep : WeaponModRep
{
    private const float kMaxAudioDistance = 4f;
    private const float kMinAudioDistance = 1f;
    private const AudioRolloffMode kRolloffMode = AudioRolloffMode.Logarithmic;
    private const float kVolume = 1f;
    private Light[] lights;

    public LampModRep() : this(ItemModRepresentation.Caps.BindStateFlags, false)
    {
    }

    protected LampModRep(ItemModRepresentation.Caps caps) : this(caps, false)
    {
    }

    protected LampModRep(ItemModRepresentation.Caps caps, bool defaultOn) : base(caps, defaultOn)
    {
    }

    protected override void BindStateFlags(CharacterStateFlags flags, ItemModRepresentation.Reason reason)
    {
        base.BindStateFlags(flags, reason);
        base.SetOn(flags.lamp, reason);
    }

    protected override void DisableMod(ItemModRepresentation.Reason reason)
    {
        Light anyLight = null;
        foreach (Light light2 in this.lights)
        {
            if (light2 != null)
            {
                light2.enabled = false;
                anyLight = light2;
            }
        }
        if (reason == ItemModRepresentation.Reason.Explicit)
        {
            this.PlaySound(anyLight, base.modDataBlock.offSound);
        }
    }

    protected override void EnableMod(ItemModRepresentation.Reason reason)
    {
        Light anyLight = null;
        foreach (Light light2 in this.lights)
        {
            if (light2 != null)
            {
                light2.enabled = true;
                anyLight = light2;
            }
        }
        if (reason == ItemModRepresentation.Reason.Explicit)
        {
            this.PlaySound(anyLight, base.modDataBlock.onSound);
        }
    }

    protected override void OnAddAttached()
    {
        this.lights = base.attached.GetComponentsInChildren<Light>();
    }

    protected override void OnRemoveAttached()
    {
        this.lights = null;
    }

    private void PlaySound(Light anyLight, AudioClip clip)
    {
        if (anyLight != null)
        {
            clip.PlayLocal(anyLight.transform, Vector3.zero, 1f, AudioRolloffMode.Logarithmic, 1f, 4f);
        }
        else
        {
            clip.PlayLocal(base.itemRep.transform, Vector3.zero, 1f, AudioRolloffMode.Logarithmic, 1f, 4f);
        }
    }

    protected override bool VerifyCompatible(GameObject attachment)
    {
        return attachment.GetComponentInChildren<Light>();
    }
}

