using System;

public abstract class ToolItem<T> : InventoryItem<T> where T: ToolDataBlock
{
    protected ToolItem(T db) : base(db)
    {
    }

    public virtual void CancelWork()
    {
    }

    public virtual void CompleteWork()
    {
        base.datablock.CompleteWork(base.iface as IToolItem, base.inventory);
    }

    public virtual void StartWork()
    {
    }

    public virtual bool canWork
    {
        get
        {
            return base.datablock.CanWork(base.iface as IToolItem, base.inventory);
        }
    }

    public virtual float workDuration
    {
        get
        {
            return base.datablock.GetWorkDuration(base.iface as IToolItem);
        }
    }
}

