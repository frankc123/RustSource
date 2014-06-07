using System;
using UnityEngine;

public interface IBasicTorchItem : IHeldItem, IInventoryItem
{
    void Extinguish();
    void Ignite();

    bool isLit { get; set; }

    GameObject light { get; set; }
}

