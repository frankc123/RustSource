using System;
using UnityEngine;

public interface ITorchItem : IHeldItem, IInventoryItem, IThrowableItem, IWeaponItem
{
    void Extinguish();
    void Ignite();

    float forceSecondaryTime { get; set; }

    bool isLit { get; }

    GameObject light { get; set; }

    float realIgniteTime { get; set; }

    float realThrowTime { get; set; }
}

