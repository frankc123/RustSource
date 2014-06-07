using Facepunch.Actor;
using System;
using UnityEngine;

public abstract class ArmorModel : ScriptableObject
{
    [SerializeField]
    private ActorMeshInfo _actorMeshInfo;
    [SerializeField]
    private Material[] _materials;
    [NonSerialized]
    public readonly ArmorModelSlot slot;

    internal ArmorModel(ArmorModelSlot slot)
    {
        this.slot = slot;
    }

    protected abstract ArmorModel _censored { get; }

    public ActorMeshInfo actorMeshInfo
    {
        get
        {
            return this._actorMeshInfo;
        }
    }

    public ActorRig actorRig
    {
        get
        {
            return ((this._actorMeshInfo == null) ? null : this._actorMeshInfo.actorRig);
        }
    }

    public ArmorModel censoredModel
    {
        get
        {
            return this._censored;
        }
    }

    public bool hasCensoredModel
    {
        get
        {
            return (bool) this._censored;
        }
    }

    public Material[] sharedMaterials
    {
        get
        {
            return this._materials;
        }
    }

    public Mesh sharedMesh
    {
        get
        {
            return ((this._actorMeshInfo == null) ? null : this._actorMeshInfo.sharedMesh);
        }
    }

    public ArmorModelSlotMask slotMask
    {
        get
        {
            return this.slot.ToMask();
        }
    }
}

