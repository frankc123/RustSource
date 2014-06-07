using System;
using UnityEngine;

public class FootstepEmitter : IDLocalCharacter
{
    [NonSerialized]
    private Vector3 lastFootstepPos;
    private AudioClip lastPlayed;
    [NonSerialized]
    private float movedAmount;
    [NonSerialized]
    private float nextAllowTime;
    public bool terraincheck;
    [NonSerialized]
    private CharacterFootstepTrait trait;

    private Collider GetBelowObj()
    {
        RaycastHit hit;
        if (Physics.Raycast(new Ray(base.transform.position + new Vector3(0f, 0.25f, 0f), Vector3.down), out hit, 1f))
        {
            return hit.collider;
        }
        return null;
    }

    private void Start()
    {
        this.lastFootstepPos = base.origin;
        this.trait = base.GetTrait<CharacterFootstepTrait>();
        if (((this.trait == null) || (this.trait.defaultFootsteps == null)) || (this.trait.defaultFootsteps.Length == 0))
        {
            base.enabled = false;
        }
    }

    private void Update()
    {
        bool flag;
        if (this.terraincheck)
        {
            int textureIndex = TerrainTextureHelper.GetTextureIndex(base.origin);
        }
        if ((base.stateFlags.grounded && (!(flag = this.trait.timeLimited) || (this.nextAllowTime <= Time.time))) && ((base.masterControllable == null) || (base.masterControllable.idMain == base.idMain)))
        {
            bool crouch = base.stateFlags.crouch;
            Vector3 origin = base.origin;
            this.movedAmount += Vector3.Distance(this.lastFootstepPos, origin);
            this.lastFootstepPos = origin;
            if (this.movedAmount >= this.trait.sqrStrideDist)
            {
                this.movedAmount = 0f;
                AudioClip clip = null;
                if ((footsteps.quality >= 2) || ((footsteps.quality == 1) && base.character.localControlled))
                {
                    Collider belowObj = this.GetBelowObj();
                    if (belowObj != null)
                    {
                        SurfaceInfoObject surfaceInfoFor = SurfaceInfo.GetSurfaceInfoFor(belowObj, origin);
                        if (surfaceInfoFor != null)
                        {
                            clip = !this.trait.animal ? surfaceInfoFor.GetFootstepBiped(this.lastPlayed) : surfaceInfoFor.GetFootstepAnimal();
                            this.lastPlayed = clip;
                        }
                    }
                }
                if (clip == null)
                {
                    clip = this.trait.defaultFootsteps[Random.Range(0, this.trait.defaultFootsteps.Length)];
                    if (clip == null)
                    {
                        return;
                    }
                }
                float minAudioDist = this.trait.minAudioDist;
                float maxAudioDist = this.trait.maxAudioDist;
                if (crouch)
                {
                    clip.Play(origin, 0.2f, Random.Range((float) 0.95f, (float) 1.05f), minAudioDist * 0.333f, maxAudioDist * 0.333f, 30);
                }
                else
                {
                    clip.Play(origin, 0.65f, Random.Range((float) 0.95f, (float) 1.05f), minAudioDist, maxAudioDist, 30);
                }
                if (flag)
                {
                    this.nextAllowTime = Time.time + this.trait.minInterval;
                }
            }
        }
    }
}

