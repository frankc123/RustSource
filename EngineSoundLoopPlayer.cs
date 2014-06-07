using System;
using UnityEngine;

internal sealed class EngineSoundLoopPlayer : MonoBehaviour
{
    [NonSerialized]
    internal EngineSoundLoop.Instance instance;

    private void OnDestroy()
    {
        if (this.instance != null)
        {
            EngineSoundLoop.Instance instance = this.instance;
            this.instance = null;
            instance.Dispose(true);
        }
    }
}

