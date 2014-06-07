using System;
using UnityEngine;

public abstract class dfTweenPlayableBase : MonoBehaviour
{
    protected dfTweenPlayableBase()
    {
    }

    public void Disable()
    {
        base.enabled = false;
    }

    public void Enable()
    {
        base.enabled = true;
    }

    public abstract void Play();
    public abstract void Reset();
    public abstract void Stop();
    public override string ToString()
    {
        return (this.TweenName + " - " + base.ToString());
    }

    public abstract bool IsPlaying { get; }

    public abstract string TweenName { get; set; }
}

