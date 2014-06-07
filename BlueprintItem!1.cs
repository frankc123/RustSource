using System;

public abstract class BlueprintItem<T> : ToolItem<T> where T: BlueprintDataBlock
{
    protected BlueprintItem(T db) : base(db)
    {
    }

    public override float workDuration
    {
        get
        {
            return base.datablock.GetWorkDuration(base.iface as IToolItem);
        }
    }
}

