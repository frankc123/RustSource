using System;
using System.Runtime.InteropServices;
using UnityEngine;

public interface IStateInterpolatorWithVelocity : IStateInterpolatorWithLinearVelocity, IStateInterpolatorWithAngularVelocity
{
    bool SampleWorldVelocity(out Vector3 linear, out Angle2 angular);
    bool SampleWorldVelocity(double timeStamp, out Vector3 linear, out Angle2 angular);
}

