using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterInterpolatorBase : IDLocalCharacterAddon, IIDLocalInterpolator
{
    [NonSerialized]
    protected int _bufferCapacity;
    [NonSerialized]
    private bool _destroying;
    [NonSerialized]
    private bool _running;
    [NonSerialized]
    protected float allowableTimeSpan;
    [NonSerialized]
    protected bool extrapolate;
    [NonSerialized]
    private Vector3 fromPos;
    [NonSerialized]
    private Quaternion fromRot;
    [NonSerialized]
    private bool initialized;
    protected const int kDefaultBufferCapacity = 0x20;
    private const IDLocalCharacterAddon.AddonFlags kRequiredAddonFlags = IDLocalCharacterAddon.AddonFlags.FireOnAddonAwake;
    [NonSerialized]
    protected int len;
    [NonSerialized]
    private float lerpStartTime;
    [NonSerialized]
    private Vector3 targetPos;
    [NonSerialized]
    private Quaternion targetRot;

    internal CharacterInterpolatorBase(IDLocalCharacterAddon.AddonFlags addonFlags) : base((IDLocalCharacterAddon.AddonFlags) ((byte) (addonFlags | IDLocalCharacterAddon.AddonFlags.FireOnAddonAwake)))
    {
        this._bufferCapacity = 0x20;
        this.allowableTimeSpan = 0.1f;
    }

    protected abstract void __Clear();
    public void Clear()
    {
        this.__Clear();
    }

    protected override void OnAddonAwake()
    {
        CharacterInterpolatorTrait trait = base.idMain.GetTrait<CharacterInterpolatorTrait>();
        if (trait != null)
        {
            if (trait.bufferCapacity > 0)
            {
                this._bufferCapacity = trait.bufferCapacity;
            }
            this.extrapolate = trait.allowExtrapolation;
            this.allowableTimeSpan = trait.allowableTimeSpan;
        }
    }

    protected void OnDestroy()
    {
        this._destroying = true;
        if (this._running)
        {
            Interpolators.SetDisabled(this);
            this._running = false;
        }
    }

    public virtual void SetGoals(Vector3 pos, Quaternion rot, double timestamp)
    {
        if (!this.initialized)
        {
            base.transform.position = pos;
            if (base.idMain is Character)
            {
                Angle2 angle = Angle2.LookDirection((Vector3) (rot * Vector3.forward));
                angle.pitch = Mathf.DeltaAngle(0f, angle.pitch);
                base.idMain.eyesAngles = angle;
            }
            else
            {
                base.transform.rotation = rot;
            }
            this.initialized = true;
        }
        this.targetPos = pos;
        this.targetRot = rot;
        this.fromPos = base.transform.position;
        if (base.idMain is Character)
        {
            this.fromRot = base.idMain.eyesAngles.quat;
        }
        else
        {
            this.fromRot = base.transform.rotation;
        }
        this.lerpStartTime = Time.realtimeSinceStartup;
    }

    protected virtual void Syncronize()
    {
        float t = (Time.realtimeSinceStartup - this.lerpStartTime) / Interpolation.@struct.totalDelaySecondsF;
        Vector3 vector = Vector3.Lerp(this.fromPos, this.targetPos, t);
        Quaternion quaternion = Quaternion.Slerp(this.fromRot, this.targetRot, t);
        if (base.idMain is Character)
        {
            Character idMain = base.idMain;
            idMain.origin = vector;
            Angle2 angle = Angle2.LookDirection((Vector3) (quaternion * Vector3.forward));
            angle.pitch = Mathf.DeltaAngle(0f, angle.pitch);
            idMain.eyesAngles = angle;
        }
        else
        {
            base.transform.position = vector;
            base.transform.rotation = quaternion;
        }
    }

    internal static void SyncronizeAll()
    {
        Interpolators.UpdateAll();
    }

    protected abstract double __newestTimeStamp { get; }

    protected abstract double __oldestTimeStamp { get; }

    protected abstract double __storedDuration { get; }

    [Obsolete("Use .running for interpolators", true)]
    public bool enabled
    {
        get
        {
            return this.running;
        }
        set
        {
            this.running = value;
        }
    }

    IDMain IIDLocalInterpolator.idMain
    {
        get
        {
            return base.idMain;
        }
    }

    IDLocal IIDLocalInterpolator.self
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
            return this.__newestTimeStamp;
        }
    }

    public double oldestTimeStamp
    {
        get
        {
            return this.__oldestTimeStamp;
        }
    }

    public bool running
    {
        get
        {
            return this._running;
        }
        set
        {
            if (this._destroying)
            {
                value = false;
            }
            if (this._running != value)
            {
                if (value)
                {
                    this._running = Interpolators.SetEnabled(this);
                }
                else
                {
                    this._running = !Interpolators.SetDisabled(this);
                }
            }
        }
    }

    public double storedDuration
    {
        get
        {
            return this.__storedDuration;
        }
    }

    private static class Interpolators
    {
        private static bool caughtIterating;
        private static readonly HashSet<CharacterInterpolatorBase> hashset1 = new HashSet<CharacterInterpolatorBase>();
        private static readonly HashSet<CharacterInterpolatorBase> hashset2 = new HashSet<CharacterInterpolatorBase>();
        private static bool iterating;
        private static bool swapped;

        public static bool SetDisabled(CharacterInterpolatorBase interpolator)
        {
            HashSet<CharacterInterpolatorBase> set;
            HashSet<CharacterInterpolatorBase> set2;
            if (!iterating)
            {
                return (!swapped ? hashset1 : hashset2).Remove(interpolator);
            }
            if (caughtIterating)
            {
                return (!swapped ? hashset2 : hashset1).Remove(interpolator);
            }
            if (swapped)
            {
                set = hashset2;
                set2 = hashset1;
            }
            else
            {
                set = hashset1;
                set2 = hashset2;
            }
            if (!set.Contains(interpolator))
            {
                return false;
            }
            caughtIterating = true;
            set2.UnionWith(set);
            return set2.Remove(interpolator);
        }

        public static bool SetEnabled(CharacterInterpolatorBase interpolator)
        {
            HashSet<CharacterInterpolatorBase> set;
            HashSet<CharacterInterpolatorBase> set2;
            if (!iterating)
            {
                return (!swapped ? hashset1 : hashset2).Add(interpolator);
            }
            if (caughtIterating)
            {
                return (!swapped ? hashset2 : hashset1).Add(interpolator);
            }
            if (swapped)
            {
                set = hashset2;
                set2 = hashset1;
            }
            else
            {
                set = hashset1;
                set2 = hashset2;
            }
            if (set.Contains(interpolator))
            {
                return false;
            }
            caughtIterating = true;
            set2.UnionWith(set);
            return set2.Add(interpolator);
        }

        public static void UpdateAll()
        {
            if (!iterating)
            {
                HashSet<CharacterInterpolatorBase> set;
                if (swapped)
                {
                    set = hashset2;
                }
                else
                {
                    set = hashset1;
                }
                try
                {
                    iterating = true;
                    foreach (CharacterInterpolatorBase base2 in set)
                    {
                        try
                        {
                            base2.Syncronize();
                        }
                        catch (Exception exception)
                        {
                            Debug.LogError(exception);
                        }
                    }
                }
                finally
                {
                    if (caughtIterating)
                    {
                        swapped = !swapped;
                        if (swapped)
                        {
                            hashset1.Clear();
                        }
                        else
                        {
                            hashset2.Clear();
                        }
                    }
                    iterating = false;
                }
            }
        }
    }
}

