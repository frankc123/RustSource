using RustProto;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

[AddComponentMenu("ID/Local/Take Damage")]
public class TakeDamage : IDLocal, IServerSaveable
{
    private float _health;
    protected float _lastDamageTime;
    [SerializeField]
    private float _maxHealth = 100f;
    public const string DamageMessage = "OnHurt";
    public const SendMessageOptions DamageMessageOptions = SendMessageOptions.DontRequireReceiver;
    public const string KillMessage = "OnKilled";
    public const float kMinimumSetHealthValueWhenAlive = 0.001f;
    public bool playsHitNotification;
    public const string RepairMessage = "OnRepair";
    public const SendMessageOptions RepairMessageOptions = SendMessageOptions.DontRequireReceiver;
    public bool sendMessageRepair = true;
    public bool sendMessageWhenAlive = true;
    public bool sendMessageWhenDead = true;
    public bool sendMessageWhenKilled = true;
    private bool takenodamage;

    protected virtual void ApplyDamageTypeList(ref DamageEvent damage, DamageTypeList damageTypes)
    {
        for (int i = 0; i < 6; i++)
        {
            if (!Mathf.Approximately(damageTypes[i], 0f))
            {
                damage.damageTypes |= ((int) 1) << i;
                damage.amount += damageTypes[i];
            }
        }
    }

    protected void Awake()
    {
        this._maxHealth = this._health = HealthAliveValueClamp(this._maxHealth);
    }

    public static string DamageIndexToString(DamageTypeIndex index)
    {
        switch (index)
        {
            case DamageTypeIndex.damage_bullet:
                return "Bullet";

            case DamageTypeIndex.damage_melee:
                return "Melee";

            case DamageTypeIndex.damage_explosion:
                return "Explosion";

            case DamageTypeIndex.damage_radiation:
                return "Radiation";

            case DamageTypeIndex.damage_cold:
                return "Cold";
        }
        return "Generic";
    }

    public static string DamageIndexToString(int index)
    {
        return DamageIndexToString((DamageTypeIndex) index);
    }

    public RepairEvent Heal(IDBase healer, float amount)
    {
        RepairEvent event2;
        this.Heal(healer, amount, out event2);
        return event2;
    }

    public RepairStatus Heal(IDBase healer, float amount, out RepairEvent repair)
    {
        repair.doner = healer;
        repair.receiver = this;
        repair.givenAmount = amount;
        if (amount <= 0f)
        {
            repair.status = RepairStatus.Failed;
            repair.usedAmount = 0f;
            return RepairStatus.Failed;
        }
        if (this.dead)
        {
            repair.status = RepairStatus.FailedUnreparable;
            repair.usedAmount = 0f;
        }
        else if (this._health == this._maxHealth)
        {
            repair.status = RepairStatus.FailedFull;
            repair.usedAmount = 0f;
        }
        else if (this._health > (this._maxHealth - amount))
        {
            this._health = this._maxHealth;
            repair.usedAmount = this._maxHealth - this._health;
            repair.status = RepairStatus.AppliedPartial;
        }
        else
        {
            this._health += amount;
            repair.usedAmount = repair.givenAmount;
            if (this._health == this._maxHealth)
            {
                repair.status = RepairStatus.AppliedFull;
            }
            else
            {
                repair.status = RepairStatus.Applied;
            }
        }
        if (this.sendMessageRepair)
        {
            base.SendMessage("OnRepair", (RepairEvent) repair, SendMessageOptions.DontRequireReceiver);
        }
        return repair.status;
    }

    private static float HealthAliveValueClamp(float newHealth)
    {
        return ((newHealth >= 0.001f) ? newHealth : 0.001f);
    }

    protected virtual LifeStatus Hurt(ref DamageEvent damage)
    {
        if (this.dead)
        {
            damage.status = LifeStatus.IsDead;
        }
        else if (this.health > damage.amount)
        {
            damage.status = LifeStatus.IsAlive;
        }
        else
        {
            damage.status = LifeStatus.WasKilled;
        }
        this.ProcessDamageEvent(ref damage);
        if (this.ShouldRelayDamageEvent(ref damage))
        {
            base.SendMessage("OnHurt", (DamageEvent) damage, SendMessageOptions.DontRequireReceiver);
        }
        if (damage.status == LifeStatus.WasKilled)
        {
            base.SendMessage("OnKilled", (DamageEvent) damage, SendMessageOptions.DontRequireReceiver);
        }
        return damage.status;
    }

    public static LifeStatus Hurt(IDBase attacker, IDBase victim, Quantity damageQuantity, object extraData = null)
    {
        return HurtShared(attacker, victim, damageQuantity, extraData);
    }

    public static LifeStatus Hurt(IDBase attacker, IDBase victim, Quantity damageQuantity, out DamageEvent damage, object extraData = null)
    {
        return HurtShared(attacker, victim, damageQuantity, out damage, extraData);
    }

    public static LifeStatus HurtSelf(IDBase victim, Quantity damageQuantity, object extraData = null)
    {
        return HurtShared(victim, victim, damageQuantity, extraData);
    }

    public static LifeStatus HurtSelf(IDBase victim, Quantity damageQuantity, out DamageEvent damage, object extraData = null)
    {
        return HurtShared(victim, victim, damageQuantity, out damage, extraData);
    }

    private static LifeStatus HurtShared(IDBase attacker, IDBase victim, Quantity damageQuantity, object extraData = null)
    {
        DamageEvent event2;
        return HurtShared(attacker, victim, damageQuantity, out event2, extraData);
    }

    private static LifeStatus HurtShared(IDBase attacker, IDBase victim, Quantity damageQuantity, out DamageEvent damage, object extraData = null)
    {
        if (victim != null)
        {
            IDMain idMain = victim.idMain;
            if (idMain != null)
            {
                TakeDamage takeDamage;
                if (idMain is Character)
                {
                    takeDamage = ((Character) idMain).takeDamage;
                }
                else
                {
                    takeDamage = idMain.GetLocal<TakeDamage>();
                }
                if ((takeDamage != null) && !takeDamage.takenodamage)
                {
                    takeDamage.MarkDamageTime();
                    damage.victim.id = victim;
                    damage.attacker.id = attacker;
                    damage.amount = damageQuantity.value;
                    damage.sender = takeDamage;
                    damage.status = !takeDamage.dead ? LifeStatus.IsAlive : LifeStatus.IsDead;
                    damage.damageTypes = 0;
                    damage.extraData = extraData;
                    if (((int) damageQuantity.Unit) == -1)
                    {
                        takeDamage.ApplyDamageTypeList(ref damage, damageQuantity.list);
                    }
                    takeDamage.Hurt(ref damage);
                    return damage.status;
                }
            }
        }
        damage.victim.id = null;
        damage.attacker.id = null;
        damage.amount = 0f;
        damage.sender = null;
        damage.damageTypes = 0;
        damage.status = LifeStatus.Failed;
        damage.extraData = extraData;
        return LifeStatus.Failed;
    }

    public static LifeStatus Kill(IDBase attacker, IDBase victim, object extraData = null)
    {
        return HurtShared(attacker, victim, Quantity.AllHealth, extraData);
    }

    public static LifeStatus Kill(IDBase attacker, IDBase victim, out DamageEvent damage, object extraData = null)
    {
        return HurtShared(attacker, victim, Quantity.AllHealth, out damage, extraData);
    }

    public static LifeStatus KillSelf(IDBase victim, object extraData = null)
    {
        return HurtShared(victim, victim, Quantity.AllHealth, extraData);
    }

    public static LifeStatus KillSelf(IDBase victim, out DamageEvent damage, object extraData = null)
    {
        return HurtShared(victim, victim, Quantity.AllHealth, out damage, extraData);
    }

    public virtual void LoadVitals(Vitals vitals)
    {
        this.health = vitals.Health;
        if (this.health <= 0f)
        {
            Debug.Log("LOAD VITALS - HEALTH WAS " + this.health);
            this.health = 1f;
        }
    }

    public void MarkDamageTime()
    {
        this._lastDamageTime = Time.time;
    }

    protected void ProcessDamageEvent(ref DamageEvent damage)
    {
        if (!this.takenodamage)
        {
            switch (damage.status)
            {
                case LifeStatus.IsAlive:
                    this._health -= damage.amount;
                    break;

                case LifeStatus.WasKilled:
                    this._health = 0f;
                    break;
            }
        }
    }

    public virtual void SaveVitals(ref Vitals.Builder vitals)
    {
        vitals.SetHealth(this.health);
    }

    public virtual void ServerFrame()
    {
    }

    public virtual void SetGodMode(bool on)
    {
        this.takenodamage = on;
    }

    public bool ShouldPlayHitNotification()
    {
        return (this.playsHitNotification && this.alive);
    }

    protected bool ShouldRelayDamageEvent(ref DamageEvent damage)
    {
        switch (damage.status)
        {
            case LifeStatus.IsAlive:
                return this.sendMessageWhenAlive;

            case LifeStatus.WasKilled:
                return this.sendMessageWhenKilled;

            case LifeStatus.IsDead:
                return this.sendMessageWhenDead;
        }
        Debug.LogWarning("Unhandled LifeStatus " + damage.status, this);
        return false;
    }

    public float TimeSinceHurt()
    {
        return (Time.time - this._lastDamageTime);
    }

    public override string ToString()
    {
        return string.Format("[{0}: health={1}]", base.ToString(), this.health);
    }

    public bool alive
    {
        get
        {
            return (this.health > 0f);
        }
    }

    public bool dead
    {
        get
        {
            return (this.health <= 0f);
        }
    }

    public float health
    {
        get
        {
            return this._health;
        }
        set
        {
            this._health = value;
        }
    }

    public float healthFraction
    {
        get
        {
            return (this._health / this._maxHealth);
        }
    }

    public float healthLoss
    {
        get
        {
            return (this._maxHealth - this.health);
        }
    }

    public float healthLossFraction
    {
        get
        {
            return (1f - (this._health / this._maxHealth));
        }
    }

    public float maxHealth
    {
        get
        {
            return this._maxHealth;
        }
        set
        {
            this._maxHealth = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Quantity
    {
        public readonly TakeDamage.Unit Unit;
        internal readonly float value;
        internal readonly DamageTypeList list;
        private Quantity(TakeDamage.Unit Measurement, DamageTypeList DamageTypeList, float Value)
        {
            this.Unit = Measurement;
            this.list = DamageTypeList;
            this.value = Value;
        }

        public static TakeDamage.Quantity AllHealth
        {
            get
            {
                return new TakeDamage.Quantity(TakeDamage.Unit.AllHealth, null, float.PositiveInfinity);
            }
        }
        public DamageTypeList DamageTypeList
        {
            get
            {
                if (((int) this.Unit) > 0)
                {
                    throw new InvalidOperationException("Quantity is of HealthPoints");
                }
                return this.list;
            }
        }
        public bool IsDamageTypeList
        {
            get
            {
                return (((int) this.Unit) == -1);
            }
        }
        public float HealthPoints
        {
            get
            {
                if (((int) this.Unit) < -1)
                {
                    throw new InvalidOperationException("Quantity is of DamageTypeList");
                }
                return this.value;
            }
        }
        public bool IsHealthPoints
        {
            get
            {
                return (((int) this.Unit) > 0);
            }
        }
        public bool IsAllHealthPoints
        {
            get
            {
                return (((int) this.Unit) == 2);
            }
        }
        public bool Specified
        {
            get
            {
                return (((int) this.Unit) != 0);
            }
        }
        public object BoxedValue
        {
            get
            {
                return ((((int) this.Unit) > 0) ? ((object) this.value) : ((object) this.list));
            }
        }
        public override string ToString()
        {
            return string.Format("[{0}:{1}]", this.Unit, this.BoxedValue);
        }

        public static implicit operator TakeDamage.Quantity(int HealthPoints)
        {
            return new TakeDamage.Quantity((HealthPoints != 0) ? TakeDamage.Unit.HealthPoints : TakeDamage.Unit.Unspecified, null, (HealthPoints >= 0) ? ((float) HealthPoints) : ((float) -HealthPoints));
        }

        public static implicit operator TakeDamage.Quantity(float HealthPoints)
        {
            return new TakeDamage.Quantity((HealthPoints != 0f) ? (!float.IsInfinity(HealthPoints) ? TakeDamage.Unit.HealthPoints : TakeDamage.Unit.AllHealth) : TakeDamage.Unit.Unspecified, null, HealthPoints);
        }

        public static implicit operator TakeDamage.Quantity(DamageTypeList DamageTypeList)
        {
            return new TakeDamage.Quantity(!object.ReferenceEquals(DamageTypeList, null) ? TakeDamage.Unit.List : TakeDamage.Unit.Unspecified, DamageTypeList, 0f);
        }
    }

    public enum Unit : sbyte
    {
        AllHealth = 2,
        HealthPoints = 1,
        List = -1,
        Unspecified = 0
    }
}

