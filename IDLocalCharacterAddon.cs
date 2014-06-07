using System;
using System.Runtime.Serialization;
using UnityEngine;

public abstract class IDLocalCharacterAddon : IDLocalCharacter
{
    private readonly AddonFlags addonFlags;
    [NonSerialized]
    private bool addonWasAdded;
    internal const byte kInitializeAddonFlag_Failed = 8;
    internal const byte kInitializeAddonFlag_PostAwake = 2;
    [NonSerialized]
    private bool removingThisAddon;

    protected IDLocalCharacterAddon(AddonFlags addonFlags)
    {
        this.addonFlags = addonFlags;
    }

    protected virtual bool CheckPrerequesits()
    {
        throw new BaseNoImplementationCalled("You should not call base.CheckPrerequesits. or define AddonFlags you do not use.");
    }

    internal byte InitializeAddon(Character idMain)
    {
        if (this.addonWasAdded)
        {
            return 0;
        }
        base.idMain = idMain;
        this.addonWasAdded = true;
        byte num = 0;
        if (((byte) (this.addonFlags & AddonFlags.PrerequisitCheck)) == 4)
        {
            try
            {
                if (!this.CheckPrerequesits())
                {
                    num = (byte) (num | 8);
                }
            }
            catch (Exception exception)
            {
                Debug.LogError(exception, this);
                if (!(exception is BaseNoImplementationCalled))
                {
                    num = (byte) (num | 8);
                }
            }
        }
        if ((num & 8) == 8)
        {
            Object.Destroy(this);
            return num;
        }
        if (((byte) (this.addonFlags & AddonFlags.FireOnAddonPostAwake)) == 2)
        {
            num = (byte) (num | 2);
        }
        if (((byte) (this.addonFlags & AddonFlags.FireOnAddonAwake)) == 1)
        {
            try
            {
                this.OnAddonAwake();
            }
            catch (Exception exception2)
            {
                Debug.Log(exception2, this);
            }
        }
        return num;
    }

    protected virtual void OnAddonAwake()
    {
        throw new BaseNoImplementationCalled("You should not call base.OnAddonAwake. or define AddonFlags you do not use.");
    }

    protected virtual void OnAddonPostAwake()
    {
        throw new BaseNoImplementationCalled("You should not call base.OnAddonPostAwake. or define AddonFlags you do not use.");
    }

    protected virtual void OnWillRemoveAddon()
    {
        throw new BaseNoImplementationCalled("You should not call base.OnWillRemoveAddon. or define AddonFlags you do not use.");
    }

    internal void PostInitializeAddon()
    {
        try
        {
            this.OnAddonPostAwake();
        }
        catch (Exception exception)
        {
            Debug.Log(exception, this);
        }
    }

    internal void RemoveAddon()
    {
        if (!this.removingThisAddon)
        {
            this.removingThisAddon = true;
            if (((byte) (this.addonFlags & AddonFlags.FireOnWillRemoveAddon)) == 8)
            {
                try
                {
                    this.OnWillRemoveAddon();
                }
                catch (Exception exception)
                {
                    Debug.LogError(exception, this);
                }
            }
            Object.Destroy(this);
        }
    }

    [Flags]
    internal protected enum AddonFlags : byte
    {
        FireOnAddonAwake = 1,
        FireOnAddonPostAwake = 2,
        FireOnWillRemoveAddon = 8,
        PrerequisitCheck = 4
    }

    [Serializable]
    private class BaseNoImplementationCalled : NotSupportedException
    {
        public BaseNoImplementationCalled()
        {
        }

        public BaseNoImplementationCalled(string message) : base(message)
        {
        }

        protected BaseNoImplementationCalled(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public BaseNoImplementationCalled(string message, Exception inner) : base(message, inner)
        {
        }
    }
}

