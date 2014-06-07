using System;

public interface IMeleeWeaponItem : IHeldItem, IInventoryItem, IWeaponItem
{
    void QueueMidSwing(float time);
    void QueueSwingSound(float time);

    float queuedSwingAttackTime { get; set; }

    float queuedSwingSoundTime { get; set; }
}

