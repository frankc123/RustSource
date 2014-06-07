using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct DamageEvent
{
    public DamageBeing attacker;
    public DamageBeing victim;
    public TakeDamage sender;
    public LifeStatus status;
    public DamageTypeFlags damageTypes;
    public float amount;
    public object extraData;
    public BodyPart bodyPart
    {
        get
        {
            return this.victim.bodyPart;
        }
    }
    public override string ToString()
    {
        object[] args = new object[] { this.victim, this.amount, this.status, this.attacker, this.sender };
        return string.Format("{{attacker={3}, victim={0}, amount={1}, status={2}, sender={4}}}", args);
    }
}

