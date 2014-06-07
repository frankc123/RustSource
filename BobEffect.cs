using Facepunch.Precision;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

public abstract class BobEffect : ScriptableObject
{
    [NonSerialized]
    private bool loaded;

    protected BobEffect()
    {
    }

    protected abstract void CloseData(Data data);
    public bool Create(out Data data)
    {
        if (!this.loaded)
        {
            this.InitializeNonSerializedData();
            this.loaded = true;
        }
        return this.OpenData(out data);
    }

    public void Destroy(ref Data data)
    {
        if (this.loaded && (data != null))
        {
            this.CloseData(data);
            data = null;
        }
    }

    protected abstract void InitializeNonSerializedData();
    protected abstract bool OpenData(out Data data);
    public BOBRES Simulate(ref Context ctx)
    {
        if (this.loaded)
        {
            return this.SimulateData(ref ctx);
        }
        return BOBRES.ERROR;
    }

    protected abstract BOBRES SimulateData(ref Context ctx);

    [StructLayout(LayoutKind.Sequential)]
    public struct Context
    {
        public double dt;
        public BobEffect.Data data;
    }

    public class Data
    {
        public BobEffect effect;
        public Vector3G force;
        public Vector3G torque;

        public virtual BobEffect.Data Clone()
        {
            return (BobEffect.Data) base.MemberwiseClone();
        }

        public virtual void CopyDataTo(BobEffect.Data target)
        {
            target.force = this.force;
            target.torque = this.torque;
        }
    }
}

