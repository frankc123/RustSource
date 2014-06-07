using RustProto;
using System;
using UnityEngine;

public class HumanBodyTakeDamage : ProtectionTakeDamage
{
    public float _bleedingLevel;
    public float _bleedingLevelMax = 100f;
    public float _bleedInterval = 10f;
    private float _healOverTime;
    private float _lastLevelCheckTime;
    private PlayerInventory _playerInv;
    private IDBase bleedAttacker;
    private IDBase bleedingID;
    public float checkLevelInterval = 2f;
    private const string CheckLevelMethodName = "CheckLevels";
    private const string DoBleedMethodName = "DoBleed";
    private float lastBleedTime;

    public void AddBleedingLevel(float lvl)
    {
        this.SetBleedingLevel(this._bleedingLevel + lvl);
    }

    protected void Awake()
    {
        base.Awake();
        this.checkLevelInterval = 1f;
        base.InvokeRepeating("CheckLevels", this.checkLevelInterval, this.checkLevelInterval);
        this._playerInv = base.GetComponent<PlayerInventory>();
    }

    public void Bandage(float amountToRestore)
    {
        this.SetBleedingLevel(Mathf.Clamp(this._bleedingLevel - amountToRestore, 0f, this._bleedingLevelMax));
        if (this._bleedingLevel <= 0f)
        {
            base.CancelInvoke("DoBleed");
        }
    }

    public void CheckLevels()
    {
    }

    public void DoBleed()
    {
        if (base.alive && (this._bleedingLevel > 0f))
        {
            LifeStatus status;
            float damageQuantity = this._bleedingLevel;
            Metabolism component = base.GetComponent<Metabolism>();
            if (component != null)
            {
                damageQuantity = this._bleedingLevel * (!component.IsWarm() ? 1f : 0.4f);
            }
            if ((this.bleedAttacker != null) && (this.bleedingID != null))
            {
                status = TakeDamage.Hurt(this.bleedAttacker, this.bleedingID, damageQuantity, null);
            }
            else
            {
                status = TakeDamage.HurtSelf(base.idMain, damageQuantity, null);
            }
            if (status == LifeStatus.IsAlive)
            {
                float num2 = 0.2f;
                this.SetBleedingLevel(Mathf.Clamp(this._bleedingLevel - num2, 0f, this._bleedingLevel));
            }
            else
            {
                base.CancelInvoke("DoBleed");
            }
        }
        else
        {
            base.CancelInvoke("DoBleed");
        }
    }

    public virtual void HealOverTime(float amountToHeal)
    {
    }

    protected override LifeStatus Hurt(ref DamageEvent damage)
    {
        LifeStatus status = base.Hurt(ref damage);
        if ((damage.damageTypes & (DamageTypeFlags.damage_explosion | DamageTypeFlags.damage_melee | DamageTypeFlags.damage_bullet)) != 0)
        {
            this._healOverTime = 0f;
        }
        if (status == LifeStatus.WasKilled)
        {
            base.CancelInvoke("DoBleed");
            return status;
        }
        if ((status == LifeStatus.IsAlive) && (base.healthLossFraction > 0.2f))
        {
            float num = damage.amount / base.maxHealth;
            if (((damage.damageTypes & (DamageTypeFlags.damage_melee | DamageTypeFlags.damage_bullet)) == 0) || (damage.amount <= (base.maxHealth * 0.05f)))
            {
                return status;
            }
            int max = 0;
            if (num >= 0.25f)
            {
                max = 1;
            }
            else if (num >= 0.15f)
            {
                max = 2;
            }
            else if (num >= 0.05f)
            {
                max = 3;
            }
            if ((Random.Range(0, max) == 1) || (max == 1))
            {
                this.AddBleedingLevel(Mathf.Clamp(damage.amount * 0.15f, 1f, base.maxHealth));
                this.bleedAttacker = damage.attacker.id;
                this.bleedingID = damage.victim.id;
            }
        }
        return status;
    }

    public virtual bool IsBleeding()
    {
        return (this._bleedingLevel > 0f);
    }

    public override void LoadVitals(Vitals vitals)
    {
        base.LoadVitals(vitals);
        this._bleedingLevel = vitals.BleedSpeed;
        this._healOverTime = vitals.HealSpeed;
    }

    public override void SaveVitals(ref Vitals.Builder vitals)
    {
        base.SaveVitals(ref vitals);
        vitals.SetBleedSpeed(this._bleedingLevel);
        vitals.SetHealSpeed(this._healOverTime);
    }

    public override void ServerFrame()
    {
    }

    public void SetBleedingLevel(float lvl)
    {
        this._bleedingLevel = lvl;
        if (this._bleedingLevel > 0f)
        {
            base.CancelInvoke("DoBleed");
            base.InvokeRepeating("DoBleed", this._bleedInterval, this._bleedInterval);
        }
        else
        {
            base.CancelInvoke("DoBleed");
        }
        base.SendMessage("BleedingLevelChanged", lvl, SendMessageOptions.DontRequireReceiver);
    }
}

