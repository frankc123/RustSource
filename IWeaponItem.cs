using System;

public interface IWeaponItem : IHeldItem, IInventoryItem
{
    void PrimaryAttack(ref HumanController.InputSample sample);
    void Reload(ref HumanController.InputSample sample);
    void SecondaryAttack(ref HumanController.InputSample sample);
    bool ValidatePrimaryMessageTime(double timestamp);

    bool canAim { get; }

    bool canPrimaryAttack { get; }

    bool canSecondaryAttack { get; }

    bool deployed { get; }

    float deployFinishedTime { get; set; }

    float nextPrimaryAttackTime { get; set; }

    float nextSecondaryAttackTime { get; set; }

    int possibleReloadCount { get; }
}

