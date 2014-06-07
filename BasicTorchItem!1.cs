using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class BasicTorchItem<T> : HeldItem<T> where T: BasicTorchItemDataBlock
{
    private float consumeAmount;
    private float lastTickTime;

    protected BasicTorchItem(T db) : base(db)
    {
        this.lastTickTime = -1f;
    }

    public virtual void Extinguish()
    {
        this.isLit = false;
        if (this.light != null)
        {
            Object.Destroy(this.light);
            this.light = null;
        }
    }

    public void Ignite()
    {
        this.isLit = true;
    }

    protected override void OnSetActive(bool isActive)
    {
        if (isActive)
        {
            this.lastTickTime = Time.time;
            this.consumeAmount = 0f;
            base.OnSetActive(isActive);
            base.datablock.DoActualIgnite(base.itemRepresentation, base.iface as IBasicTorchItem, base.viewModelInstance);
        }
        else
        {
            this.lastTickTime = -1f;
            base.datablock.DoActualExtinguish(base.itemRepresentation, base.iface as IBasicTorchItem, base.viewModelInstance);
            base.OnSetActive(isActive);
        }
    }

    public bool isLit { get; set; }

    public GameObject light { get; set; }
}

