using System;
using System.Runtime.InteropServices;
using UnityEngine;

public interface IStateInterpolatorWithLinearVelocity
{
    bool SampleWorldVelocity(out Vector3 linear);
    bool SampleWorldVelocity(double timeStamp, out Vector3 linear);
}

