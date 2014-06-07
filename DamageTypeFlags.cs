using System;

[Flags]
public enum DamageTypeFlags
{
    damage_bullet = 2,
    damage_cold = 0x20,
    damage_explosion = 8,
    damage_generic = 1,
    damage_melee = 4,
    damage_radiation = 0x10
}

