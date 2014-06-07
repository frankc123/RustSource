using System;
using UnityEngine;

public class FlashlightItemRep : ItemRepresentation
{
    private GameObject lightEffect;
    public GameObject lightEffectPrefab1P;
    public GameObject lightEffectPrefab3P;

    public virtual void SetLightOn(bool on)
    {
        bool flag;
        if (base.networkViewOwner == NetCull.player)
        {
            flag = true;
        }
        else
        {
            flag = false;
        }
        if (on)
        {
            if (!flag)
            {
                Vector3 position = base.transform.position;
                Quaternion rotation = base.transform.rotation;
                this.lightEffect = Object.Instantiate(this.lightEffectPrefab3P, position, rotation) as GameObject;
                this.lightEffect.transform.localPosition = position;
                this.lightEffect.transform.localRotation = rotation;
            }
        }
        else
        {
            Object.Destroy(this.lightEffect);
        }
    }

    protected override void StateSignalReceive(Character character, bool treatedAsFirst)
    {
        this.SetLightOn(character.stateFlags.lamp);
    }
}

