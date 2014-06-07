using Facepunch;
using System;
using UnityEngine;

public sealed class LocalRadiationEffect : IDLocalCharacterAddon
{
    private static AudioClip geiger0;
    private static AudioClip geiger1;
    private static AudioClip geiger2;
    private static AudioClip geiger3;
    private static GameObject geigerSoundPlayer;
    [NonSerialized]
    private Radiation radiation;

    public LocalRadiationEffect() : base(IDLocalCharacterAddon.AddonFlags.PrerequisitCheck)
    {
    }

    protected override bool CheckPrerequesits()
    {
        this.radiation = base.GetComponent<Radiation>();
        return (bool) this.radiation;
    }

    private void OnDestroy()
    {
        if (geigerSoundPlayer != null)
        {
            Object.Destroy(geigerSoundPlayer);
        }
    }

    private void Update()
    {
        if (!base.dead)
        {
            float radExposureScalar;
            float num2;
            float num3;
            float num4;
            if (this.radiation != null)
            {
                num4 = this.radiation.CalculateExposure(true);
                num3 = this.radiation.CalculateExposure(false);
                radExposureScalar = this.radiation.GetRadExposureScalar(num4);
                num2 = this.radiation.GetRadExposureScalar(num3);
            }
            else
            {
                radExposureScalar = 0f;
                num2 = 0f;
                num4 = 0f;
                num3 = 0f;
            }
            ImageEffectManager.GetInstance<NoiseAndGrain>().intensityMultiplier = 10f * radExposureScalar;
            if (geiger0 == null)
            {
                Bundling.Load<AudioClip>("content/item/sfx/geiger_low", out geiger0);
                Bundling.Load<AudioClip>("content/item/sfx/geiger_medium", out geiger1);
                Bundling.Load<AudioClip>("content/item/sfx/geiger_high", out geiger2);
                Bundling.Load<AudioClip>("content/item/sfx/geiger_ultra", out geiger3);
            }
            if (num3 >= 0.02f)
            {
                if (geigerSoundPlayer == null)
                {
                    Type[] components = new Type[] { typeof(AudioSource) };
                    geigerSoundPlayer = new GameObject("GEIGER SOUNDS", components);
                    geigerSoundPlayer.transform.position = base.transform.position;
                    geigerSoundPlayer.transform.parent = base.transform;
                    geigerSoundPlayer.audio.loop = true;
                }
                AudioClip clip = null;
                if (num2 <= 0.25f)
                {
                    clip = geiger0;
                }
                else if (num2 <= 0.5f)
                {
                    clip = geiger1;
                }
                else if (num2 <= 0.75f)
                {
                    clip = geiger2;
                }
                else
                {
                    clip = geiger3;
                }
                if (clip != geigerSoundPlayer.audio.clip)
                {
                    geigerSoundPlayer.audio.Stop();
                    geigerSoundPlayer.audio.clip = clip;
                    geigerSoundPlayer.audio.Play();
                }
            }
            else if (geigerSoundPlayer != null)
            {
                geigerSoundPlayer.audio.Stop();
                Object.Destroy(geigerSoundPlayer);
                geigerSoundPlayer = null;
            }
        }
    }
}

