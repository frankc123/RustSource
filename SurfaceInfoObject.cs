using Facepunch;
using System;
using UnityEngine;

public class SurfaceInfoObject : ScriptableObject
{
    public static SurfaceInfoObject _default;
    public AudioClipArray animalFootsteps;
    public AudioClipArray bipedFootsteps;
    public GameObject[] bulletEffects;
    public GameObject[] meleeEffects;

    public static SurfaceInfoObject GetDefault()
    {
        if (_default == null)
        {
            Bundling.Load<SurfaceInfoObject>("rust/effects/impact/default", out _default);
            if (_default == null)
            {
                Debug.Log("COULD NOT GET DEFAULT!");
            }
        }
        return _default;
    }

    public AudioClip GetFootstepAnimal()
    {
        return this.animalFootsteps[Random.Range(0, this.animalFootsteps.Length)];
    }

    public AudioClip GetFootstepBiped()
    {
        return this.bipedFootsteps[Random.Range(0, this.bipedFootsteps.Length)];
    }

    public AudioClip GetFootstepBiped(AudioClip last)
    {
        int num = Random.Range(0, this.bipedFootsteps.Length);
        AudioClip clip = this.bipedFootsteps[num];
        if ((last != null) && (clip == last))
        {
            if (num < (this.bipedFootsteps.Length - 1))
            {
                num++;
            }
            else if (num >= 1)
            {
                num--;
            }
            clip = this.bipedFootsteps[num];
        }
        return this.bipedFootsteps[num];
    }

    public GameObject GetImpactEffect(ImpactType type)
    {
        if (type == ImpactType.Bullet)
        {
            return this.bulletEffects[Random.Range(0, this.bulletEffects.Length)];
        }
        if (type == ImpactType.Melee)
        {
            return this.meleeEffects[Random.Range(0, this.meleeEffects.Length)];
        }
        return null;
    }

    public enum ImpactType
    {
        Melee,
        Bullet
    }
}

