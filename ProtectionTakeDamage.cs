using System;
using UnityEngine;

public class ProtectionTakeDamage : TakeDamage
{
    protected DamageTypeList _armorValues;
    protected const float _maxArmorValue = 200f;
    [SerializeField]
    private DamageTypeList _startArmorValues;
    private bool initializedArmor;

    protected sealed override void ApplyDamageTypeList(ref DamageEvent damage, DamageTypeList damageTypes)
    {
        for (int i = 0; i < 6; i++)
        {
            if ((this._armorValues[i] > 0f) && (damageTypes[i] > 0f))
            {
                DamageTypeList list;
                int num2;
                float num3 = list[num2];
                (list = damageTypes)[num2 = i] = num3 * Mathf.Clamp01(1f - (this._armorValues[i] / 200f));
            }
        }
        base.ApplyDamageTypeList(ref damage, damageTypes);
    }

    protected void Awake()
    {
        if (!this.initializedArmor)
        {
            this.InitializeArmorValues();
        }
        base.Awake();
    }

    public virtual float GetArmorValue(int index)
    {
        return this._armorValues[index];
    }

    public DamageTypeList GetArmorValues()
    {
        return this._armorValues;
    }

    private void InitializeArmorValues()
    {
        this._armorValues = new DamageTypeList(this._startArmorValues);
        this.initializedArmor = true;
    }

    public virtual void SetArmorValues(DamageTypeList armor)
    {
        if (!this.initializedArmor)
        {
            this._armorValues = new DamageTypeList(armor);
            this.initializedArmor = true;
        }
        else
        {
            this._armorValues.SetArmorValues(armor);
        }
    }
}

