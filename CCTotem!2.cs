using System;
using UnityEngine;

public abstract class CCTotem<TTotemObject, CCTotemScript> : CCTotem<TTotemObject> where TTotemObject: CCTotem.TotemicObject<CCTotemScript, TTotemObject>, new() where CCTotemScript: CCTotem<TTotemObject, CCTotemScript>, new()
{
    [NonSerialized]
    private bool destroyed;

    internal CCTotem()
    {
    }

    internal void AssignTotemicObject(TTotemObject totemObject)
    {
        if (!object.ReferenceEquals(base.totemicObject, null))
        {
            if (object.ReferenceEquals(base.totemicObject, totemObject))
            {
                return;
            }
            this.ClearTotemicObject();
        }
        base.totemicObject = totemObject;
        if (!object.ReferenceEquals(base.totemicObject, null))
        {
            if (this.destroyed)
            {
                base.totemicObject = null;
                throw new InvalidOperationException("Cannot assign non-null script during destruction");
            }
            this.totemicObject.AssignedToScript((CCTotemScript) this);
        }
    }

    protected void ClearTotemicObject()
    {
        TTotemObject totemicObject = base.totemicObject;
        try
        {
            this.totemicObject.OnScriptDestroy((CCTotemScript) this);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception, this);
        }
        finally
        {
            if (object.ReferenceEquals(totemicObject, base.totemicObject))
            {
                base.totemicObject = null;
            }
        }
    }

    protected void OnDestroy()
    {
        if (!this.destroyed)
        {
            this.destroyed = true;
            if (!object.ReferenceEquals(base.totemicObject, null))
            {
                this.ClearTotemicObject();
            }
        }
    }
}

