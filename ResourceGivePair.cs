using System;
using UnityEngine;

[Serializable]
public class ResourceGivePair
{
    [NonSerialized]
    private Datablock.Ident _resourceItemDatablock;
    [NonSerialized]
    private bool _setResourceItemDatablock;
    public int amountMax;
    public int amountMin;
    private int realAmount;
    public string ResourceItemName = string.Empty;

    public int AmountLeft()
    {
        return this.realAmount;
    }

    public bool AnyLeft()
    {
        return (this.realAmount > 0);
    }

    public void CalcAmount()
    {
        this.realAmount = Random.Range(this.amountMin, this.amountMax + 1);
    }

    public void Subtract(int amount)
    {
        this.realAmount -= amount;
    }

    public ItemDataBlock ResourceItemDataBlock
    {
        get
        {
            if (!this._setResourceItemDatablock)
            {
                this._resourceItemDatablock = this.ResourceItemName;
                this._setResourceItemDatablock = true;
            }
            return (ItemDataBlock) this._resourceItemDatablock.datablock;
        }
    }
}

