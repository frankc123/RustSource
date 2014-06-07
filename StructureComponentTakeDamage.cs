public class StructureComponentTakeDamage : ProtectionTakeDamage
{
    protected override LifeStatus Hurt(ref DamageEvent damage)
    {
        return base.Hurt(ref damage);
    }
}

