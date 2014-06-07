using Facepunch.Precision;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BobEffectStack : IDisposable
{
    private BobEffect.Context ctx;
    private List<BobEffect.Data> data = new List<BobEffect.Data>();
    private int dataCount;
    private List<BobEffectStack> forks = new List<BobEffectStack>();
    private bool isFork;
    private BobEffectStack owner;

    public bool CreateInstance(BobEffect effect)
    {
        BobEffect.Data data;
        if ((effect == null) || !effect.Create(out data))
        {
            return false;
        }
        this.data.Add(data);
        foreach (BobEffectStack stack in this.forks)
        {
            stack.data.Add(data.Clone());
        }
        return true;
    }

    private void DestroyAllEffects()
    {
        foreach (BobEffect.Data data in this.data)
        {
            this.ctx.data = data;
            if (this.ctx.data.effect != null)
            {
                this.ctx.data.effect.Destroy(ref this.ctx.data);
            }
        }
        this.ctx.data = null;
        this.data.Clear();
    }

    public void Dispose()
    {
        if (!this.isFork)
        {
            foreach (BobEffectStack stack in this.forks)
            {
                stack.DestroyAllEffects();
            }
        }
        else
        {
            this.DestroyAllEffects();
            this.owner.forks.Remove(this);
            this.owner = null;
            this.isFork = false;
        }
    }

    public BobEffectStack Fork()
    {
        BobEffectStack item = new BobEffectStack {
            isFork = true,
            owner = !this.isFork ? this : this.owner
        };
        item.owner.forks.Add(item);
        foreach (BobEffect.Data data in item.owner.data)
        {
            item.data.Add(data.Clone());
        }
        return item;
    }

    public bool IsForkOf(BobEffectStack stack)
    {
        return ((this.owner != null) && (this.owner == stack));
    }

    public void Join()
    {
        if (this.isFork)
        {
            this.dataCount = this.data.Count;
            for (int i = 0; i < this.dataCount; i++)
            {
                this.owner.data[i].CopyDataTo(this.data[i]);
            }
        }
    }

    private void RunSim(ref int i, ref Vector3G force, ref Vector3G torque)
    {
        while (i < this.dataCount)
        {
            int num;
            this.ctx.data = this.data[i];
            switch (this.ctx.data.effect.Simulate(ref this.ctx))
            {
                case BOBRES.CONTINUE:
                    force.x += this.ctx.data.force.x;
                    force.y += this.ctx.data.force.y;
                    force.z += this.ctx.data.force.z;
                    torque.x += this.ctx.data.torque.x;
                    torque.y += this.ctx.data.torque.y;
                    torque.z += this.ctx.data.torque.z;
                    goto Label_0210;

                case BOBRES.EXIT:
                    if (!this.isFork)
                    {
                        num = i++;
                        this.RunSim(ref i, ref force, ref torque);
                        if (this.ctx.data == null)
                        {
                            break;
                        }
                        if (this.ctx.data.effect != null)
                        {
                            this.ctx.data.effect.Destroy(ref this.ctx.data);
                        }
                        this.data.RemoveAt(num);
                        foreach (BobEffectStack stack in this.forks)
                        {
                            stack.data.RemoveAt(num);
                        }
                    }
                    return;

                case BOBRES.ERROR:
                    Debug.LogError("Error with effect", this.ctx.data.effect);
                    goto Label_0210;

                default:
                    goto Label_0210;
            }
            this.data.RemoveAt(num);
            return;
        Label_0210:
            i++;
        }
    }

    public void Simulate(ref double dt, ref Vector3G force, ref Vector3G torque)
    {
        this.dataCount = this.data.Count;
        if (this.dataCount > 0)
        {
            int i = 0;
            this.ctx.dt = dt;
            this.RunSim(ref i, ref force, ref torque);
        }
    }
}

