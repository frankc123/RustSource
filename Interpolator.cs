using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(uLinkNetworkView))]
public class Interpolator : IDLocal, IIDLocalInterpolator
{
    [NonSerialized]
    private bool _destroying;
    [NonSerialized]
    private bool _running;
    [NonSerialized]
    private Vector3 fromPos;
    [NonSerialized]
    private Quaternion fromRot;
    [NonSerialized]
    private bool initialized;
    [NonSerialized]
    private float lerpStartTime;
    [NonSerialized]
    private Vector3 targetPos;
    [NonSerialized]
    private Quaternion targetRot;

    IDMain IIDLocalInterpolator.get_idMain()
    {
        return base.idMain;
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
                ((Character) base.idMain).eyesAngles = angle;
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
            this.fromRot = ((Character) base.idMain).eyesAngles.quat;
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
            Character idMain = (Character) base.idMain;
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

    IDLocal IIDLocalInterpolator.self
    {
        get
        {
            return this;
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

    private static class Interpolators
    {
        private static bool caughtIterating;
        private static readonly HashSet<Interpolator> hashset1 = new HashSet<Interpolator>();
        private static readonly HashSet<Interpolator> hashset2 = new HashSet<Interpolator>();
        private static bool iterating;
        private static bool swapped;

        public static bool SetDisabled(Interpolator interpolator)
        {
            HashSet<Interpolator> set;
            HashSet<Interpolator> set2;
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

        public static bool SetEnabled(Interpolator interpolator)
        {
            HashSet<Interpolator> set;
            HashSet<Interpolator> set2;
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
                HashSet<Interpolator> set;
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
                    foreach (Interpolator interpolator in set)
                    {
                        try
                        {
                            interpolator.Syncronize();
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

