using System;

public interface IThrowableItem : IHeldItem, IInventoryItem, IWeaponItem
{
    void BeginHoldingBack();
    void EndHoldingBack();

    float heldThrowStrength { get; }

    bool holdingBack { get; set; }

    float holdingStartTime { get; set; }

    float minReleaseTime { get; set; }
}

