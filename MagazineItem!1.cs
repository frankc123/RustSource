using System;

public abstract class MagazineItem<T> : InventoryItem<T> where T: MagazineDataBlock
{
    private string lastUsesString;
    private int? lastUsesStringCount;

    protected MagazineItem(T db) : base(db)
    {
    }

    public int numEmptyBulletSlots
    {
        get
        {
            return (base.maxUses - base.uses);
        }
    }

    public override string toolTip
    {
        get
        {
            int uses = base.uses;
            if (this.lastUsesStringCount != uses)
            {
                if (uses <= 0)
                {
                    this.lastUsesString = "Empty " + base.datablock.name;
                }
                else
                {
                    this.lastUsesString = string.Format("{0} ({1})", base.datablock.name, this.lastUsesStringCount);
                }
                this.lastUsesStringCount = new int?(uses);
            }
            return this.lastUsesString;
        }
    }
}

