using System;
using System.Runtime.InteropServices;

public interface IStateInterpolatorSampler<TSampleType>
{
    bool Sample(ref double timeStamp, out TSampleType sample);
}

