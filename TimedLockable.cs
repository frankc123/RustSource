using System;

public class TimedLockable : LockableObject
{
    private float lockTime;
    private ulong ownerID;
}

