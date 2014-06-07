using System;
using UnityEngine;

public abstract class CharacterInterpolatorBase<T> : CharacterInterpolatorBase
{
    private readonly bool customPusher;
    private const IDLocalCharacterAddon.AddonFlags kRequiredAddonFlags = (IDLocalCharacterAddon.AddonFlags.FireOnAddonAwake | IDLocalCharacterAddon.AddonFlags.FireOnAddonPostAwake);
    [NonSerialized]
    protected TimeStamped<T>[] tbuffer;

    protected CharacterInterpolatorBase() : this(false, IDLocalCharacterAddon.AddonFlags.FireOnAddonAwake | IDLocalCharacterAddon.AddonFlags.FireOnAddonPostAwake)
    {
    }

    protected CharacterInterpolatorBase(IDLocalCharacterAddon.AddonFlags addonFlags) : this(false, addonFlags)
    {
    }

    protected CharacterInterpolatorBase(bool customPusher) : this(customPusher, IDLocalCharacterAddon.AddonFlags.FireOnAddonAwake | IDLocalCharacterAddon.AddonFlags.FireOnAddonPostAwake)
    {
    }

    protected CharacterInterpolatorBase(bool customPusher, IDLocalCharacterAddon.AddonFlags addonFlags) : base((IDLocalCharacterAddon.AddonFlags) ((byte) (addonFlags | (IDLocalCharacterAddon.AddonFlags.FireOnAddonAwake | IDLocalCharacterAddon.AddonFlags.FireOnAddonPostAwake))))
    {
        this.customPusher = customPusher;
    }

    protected sealed override void __Clear()
    {
        this.Clear();
    }

    public void Clear()
    {
        if (base.len > 0)
        {
            if (ReferenceTypeHelper<T>.TreatAsReferenceHolder)
            {
                for (int i = 0; i < base.len; i++)
                {
                    T local = default(T);
                    this.tbuffer[this.tbuffer[i].index].value = local;
                }
            }
            base.len = 0;
        }
    }

    protected virtual bool CustomPusher(ref T state, ref double timeStamp)
    {
        throw new NotImplementedException();
    }

    protected override void OnAddonPostAwake()
    {
        this.tbuffer = new TimeStamped<T>[(base._bufferCapacity > 0) ? base._bufferCapacity : 0x20];
        base._bufferCapacity = this.tbuffer.Length;
        for (int i = 0; i < base._bufferCapacity; i++)
        {
            this.tbuffer[i].index = i;
        }
    }

    protected void Push(ref T state, ref double timeStamp)
    {
        int length = this.tbuffer.Length;
        if (base.len < length)
        {
            for (int i = 0; i < base.len; i++)
            {
                int index = this.tbuffer[i].index;
                if (this.tbuffer[index].timeStamp < timeStamp)
                {
                    for (int j = base.len; j > i; j--)
                    {
                        this.tbuffer[j].index = this.tbuffer[j - 1].index;
                    }
                    this.tbuffer[i].index = base.len;
                    this.tbuffer[base.len++].Set(ref state, ref timeStamp);
                    return;
                }
                if (this.tbuffer[index].timeStamp == timeStamp)
                {
                    this.tbuffer[index].Set(ref state, ref timeStamp);
                    return;
                }
            }
            this.tbuffer[base.len].index = base.len;
            this.tbuffer[base.len++].Set(ref state, ref timeStamp);
        }
        else
        {
            for (int k = 0; k < length; k++)
            {
                int num6 = this.tbuffer[k].index;
                if (this.tbuffer[num6].timeStamp < timeStamp)
                {
                    int num7 = this.tbuffer[length - 1].index;
                    for (int m = length - 1; m > k; m--)
                    {
                        this.tbuffer[m].index = this.tbuffer[m - 1].index;
                    }
                    this.tbuffer[k].index = num7;
                    this.tbuffer[num7].Set(ref state, ref timeStamp);
                    return;
                }
                if (this.tbuffer[num6].timeStamp == timeStamp)
                {
                    this.tbuffer[num6].Set(ref state, ref timeStamp);
                    return;
                }
            }
        }
    }

    public void SetGoals(ref TimeStamped<T> state)
    {
        double timeStamp = state.timeStamp;
        T local = state.value;
        this.SetGoals(ref local, ref timeStamp);
    }

    public void SetGoals(ref T state, ref double timeStamp)
    {
        if (this.customPusher)
        {
            double num = timeStamp;
            T local = state;
            if (this.CustomPusher(ref local, ref num))
            {
                this.Push(ref local, ref num);
            }
        }
        else
        {
            this.Push(ref state, ref timeStamp);
        }
    }

    public override void SetGoals(Vector3 pos, Quaternion rot, double timestamp)
    {
        throw new NotImplementedException("The thing using this has not implemented a way to take pos, rot to " + typeof(T));
    }

    protected sealed override double __newestTimeStamp
    {
        get
        {
            return this.newestTimeStamp;
        }
    }

    protected sealed override double __oldestTimeStamp
    {
        get
        {
            return this.oldestTimeStamp;
        }
    }

    protected sealed override double __storedDuration
    {
        get
        {
            return this.storedDuration;
        }
    }

    public int bufferCapacity
    {
        get
        {
            return this.tbuffer.Length;
        }
    }

    public CharacterInterpolatorBase interpolator
    {
        get
        {
            return this;
        }
    }

    public double newestTimeStamp
    {
        get
        {
            return ((base.len != 0) ? this.tbuffer[this.tbuffer[base.len - 1].index].timeStamp : double.PositiveInfinity);
        }
    }

    public double oldestTimeStamp
    {
        get
        {
            return ((base.len != 0) ? this.tbuffer[this.tbuffer[0].index].timeStamp : double.NegativeInfinity);
        }
    }

    public double storedDuration
    {
        get
        {
            return ((base.len >= 2) ? (this.tbuffer[this.tbuffer[0].index].timeStamp - this.tbuffer[this.tbuffer[base.len - 1].index].timeStamp) : 0.0);
        }
    }
}

