using System;
using System.Runtime.InteropServices;

public interface IStateInterpolatorWithAngularVelocity
{
    bool SampleWorldVelocity(out Angle2 angular);
    bool SampleWorldVelocity(double timeStamp, out Angle2 angular);
}

