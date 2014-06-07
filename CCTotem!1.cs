using System;

public abstract class CCTotem<TTotemObject> : CCTotem where TTotemObject: CCTotem.TotemicObject
{
    protected internal TTotemObject totemicObject;

    internal CCTotem()
    {
    }

    internal sealed override CCTotem.TotemicObject _Object
    {
        get
        {
            return this.totemicObject;
        }
    }
}

