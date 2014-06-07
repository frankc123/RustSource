using System;
using UnityEngine;

public class ItemModRepresentation : MonoBehaviour
{
    private ItemRepresentation _itemRep;
    private CharacterStateFlags? _lastFlags;
    private int _modSlot;
    [NonSerialized]
    protected readonly Caps caps;
    [NonSerialized]
    public GameObject instantiatedThing;
    protected const Caps kAllCaps = (Caps.BindStateFlags | Caps.Initialize | Caps.Shutdown);
    protected const Caps kNoCaps = 0;

    public ItemModRepresentation()
    {
        this._modSlot = -1;
        if (base.GetType() != typeof(ItemModRepresentation))
        {
            this.caps = Caps.BindStateFlags | Caps.Initialize | Caps.Shutdown;
        }
        else
        {
            this.caps = 0;
        }
    }

    protected ItemModRepresentation(Caps caps)
    {
        this._modSlot = -1;
        this.caps = caps;
    }

    protected virtual void BindStateFlags(CharacterStateFlags flags, Reason reason)
    {
    }

    internal void HandleChangedStateFlags(CharacterStateFlags flags, bool notFromLoading)
    {
        if ((((byte) (this.caps & Caps.BindStateFlags)) == 2) && (!this._lastFlags.HasValue || !this._lastFlags.Value.Equals(flags)))
        {
            this.BindStateFlags(flags, !notFromLoading ? Reason.Initialization : Reason.Explicit);
            this._lastFlags = new CharacterStateFlags?(flags);
        }
    }

    protected virtual void Initialize()
    {
    }

    internal void Initialize(ItemRepresentation itemRep, int modSlot, CharacterStateFlags flags)
    {
        if (this._modSlot == -1)
        {
            if (itemRep == null)
            {
                throw new ArgumentOutOfRangeException("itemRep", itemRep, "!itemRep");
            }
            if ((modSlot < 0) || (modSlot >= 5))
            {
                throw new ArgumentOutOfRangeException("modSlot", modSlot, "modSlot<0||modSlot>=MAX_SUPPORTED_ITEM_MODS");
            }
            this._itemRep = itemRep;
            this._modSlot = modSlot;
            if (((byte) (this.caps & Caps.Initialize)) == 1)
            {
                try
                {
                    this.Initialize();
                }
                catch (Exception)
                {
                    this._itemRep = null;
                    this._modSlot = -1;
                    throw;
                }
            }
            this.HandleChangedStateFlags(flags, false);
        }
        else
        {
            if (this._modSlot == -2)
            {
                throw new InvalidOperationException("This ItemModRepresentation has been destroyed");
            }
            if ((itemRep != this._itemRep) || (((modSlot < 0) && (modSlot < 5)) && (modSlot != this._modSlot)))
            {
                object[] args = new object[] { this._itemRep, this._modSlot, itemRep, modSlot };
                throw new InvalidOperationException(string.Format("The ItemModRepresentation was already initialized with {{\"item\":\"{0}\",\"slot\":{1}}} and cannot be re-initialized to use {{\"item\":\"{2|\",\"slot\":{3}}}", args));
            }
        }
    }

    [Obsolete("Do not use OnDestroy in implementing classes. Instead override Shutdown() and specify Caps.Shutdown in the constructor!")]
    private void OnDestroy()
    {
        if (this._modSlot != -2)
        {
            try
            {
                if (this._modSlot != -1)
                {
                    if (this._itemRep != null)
                    {
                        try
                        {
                            if (((byte) (this.caps & Caps.Shutdown)) == 0x80)
                            {
                                try
                                {
                                    this.Shutdown();
                                }
                                catch (Exception exception)
                                {
                                    Debug.LogError(exception, this);
                                }
                            }
                            try
                            {
                                this._itemRep.ItemModRepresentationDestroyed(this);
                            }
                            catch (Exception exception2)
                            {
                                Debug.LogError(exception2, this);
                            }
                        }
                        finally
                        {
                            this._itemRep = null;
                        }
                    }
                    else
                    {
                        this._itemRep = null;
                    }
                }
            }
            finally
            {
                this._modSlot = -2;
            }
        }
    }

    protected virtual void Shutdown()
    {
    }

    public bool destroyed
    {
        get
        {
            return (this._modSlot == -2);
        }
    }

    public bool initialized
    {
        get
        {
            return (this._modSlot != -1);
        }
    }

    public HeldItemDataBlock itemDatablock
    {
        get
        {
            return ((this._itemRep == null) ? null : this._itemRep.datablock);
        }
    }

    public ItemRepresentation itemRep
    {
        get
        {
            return this._itemRep;
        }
    }

    public ItemModDataBlock modDataBlock
    {
        get
        {
            return this._itemRep._itemMods.ItemModDataBlock(this._modSlot);
        }
    }

    public int modSlot
    {
        get
        {
            return this._modSlot;
        }
    }

    [Flags]
    protected enum Caps : byte
    {
        BindStateFlags = 2,
        Initialize = 1,
        Shutdown = 0x80
    }

    protected enum Reason
    {
        Initialization,
        Implicit,
        Explicit
    }
}

