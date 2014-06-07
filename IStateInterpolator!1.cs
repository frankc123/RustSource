using System;

public interface IStateInterpolator<TSampleType> : IStateInterpolatorSampler<TSampleType>
{
    void SetGoals(ref TimeStamped<TSampleType> sample);
    void SetGoals(ref TSampleType sample, ref double timeStamp);
}

