using System;

public interface IToolItem : IInventoryItem
{
    void CancelWork();
    void CompleteWork();
    void StartWork();

    bool canWork { get; }

    float workDuration { get; }
}

