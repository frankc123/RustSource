using System;
using UnityEngine;

public class FPGrassDisplacementTrail : FPGrassDisplacementObject
{
    public TrailRenderer _trail;

    public override void DetachAndDestroy()
    {
        base.transform.parent = null;
        Object.Destroy(base.gameObject, this._trail.time * 1.5f);
    }

    public override void Initialize()
    {
        this._trail = base.GetComponent<TrailRenderer>();
    }
}

