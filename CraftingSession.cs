using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct CraftingSession
{
    [NonSerialized]
    public BlueprintDataBlock blueprint;
    [NonSerialized]
    public float startTime;
    [NonSerialized]
    public float duration;
    [NonSerialized]
    public float progressSeconds;
    [NonSerialized]
    public float _progressPerSec;
    [NonSerialized]
    public ulong startTimeMillis;
    [NonSerialized]
    public ulong durationMillis;
    [NonSerialized]
    public ulong secondsCraftingFor;
    [NonSerialized]
    public int amount;
    [NonSerialized]
    public bool inProgress;
    public float progressPerSec
    {
        get
        {
            return this._progressPerSec;
        }
        set
        {
            this._progressPerSec = value;
        }
    }
    public float remainingSeconds
    {
        get
        {
            return ((this.duration - this.progressSeconds) / this.progressPerSec);
        }
    }
    public double percentComplete
    {
        get
        {
            if (this.inProgress)
            {
                return (double) (this.progressSeconds / this.duration);
            }
            return 0.0;
        }
    }
    public bool Restart(Inventory inventory, int amount, BlueprintDataBlock blueprint, ulong startTimeMillis)
    {
        if ((blueprint == null) || !blueprint.CanWork(amount, inventory))
        {
            this = new CraftingSession();
            return false;
        }
        this.blueprint = blueprint;
        this.startTime = (float) (((double) startTimeMillis) / 1000.0);
        this.duration = blueprint.craftingDuration * amount;
        this.progressPerSec = 1f;
        this.progressSeconds = 0f;
        this.amount = amount;
        this.inProgress = true;
        return true;
    }
}

