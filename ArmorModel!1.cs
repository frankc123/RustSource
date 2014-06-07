using System;
using UnityEngine;

public abstract class ArmorModel<TArmorModel> : ArmorModel where TArmorModel: ArmorModel<TArmorModel>, new()
{
    [SerializeField]
    protected TArmorModel censored;

    internal ArmorModel(ArmorModelSlot slot) : base(slot)
    {
    }

    protected sealed override ArmorModel _censored
    {
        get
        {
            return this.censored;
        }
    }

    public TArmorModel censoredModel
    {
        get
        {
            return this.censored;
        }
    }

    public bool hasCensoredModel
    {
        get
        {
            return this.censored;
        }
    }
}

