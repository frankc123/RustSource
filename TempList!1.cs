using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public sealed class TempList<T> : List<T>, IDisposable
{
    private bool active;
    private static int activeCount;
    private static TempList<T> dump;
    private static int dumpCount;
    private static TempList<T> firstActive;
    private bool inDump;
    private static TempList<T> lastActive;
    private bool n;
    private TempList<T> next;
    private bool p;
    private TempList<T> prev;

    private TempList()
    {
    }

    private TempList(IEnumerable<T> enumerable) : base(enumerable)
    {
    }

    private void Activate()
    {
        if (!this.active)
        {
            if (this.inDump)
            {
                throw new InvalidOperationException();
            }
            if (TempList<T>.activeCount == 0)
            {
                TempList<T>.lastActive = TempList<T>.firstActive = (TempList<T>) this;
                this.p = this.n = false;
                this.prev = (TempList<T>) (this.next = null);
            }
            else if (TempList<T>.activeCount == 1)
            {
                TempList<T>.lastActive = (TempList<T>) this;
                this.p = true;
                this.n = false;
                this.prev = TempList<T>.firstActive;
                this.next = null;
                TempList<T>.firstActive.n = true;
                TempList<T>.firstActive.next = (TempList<T>) this;
            }
            else
            {
                this.p = true;
                this.n = false;
                this.prev = TempList<T>.lastActive;
                TempList<T>.lastActive.n = true;
                TempList<T>.lastActive.next = (TempList<T>) this;
                TempList<T>.lastActive = (TempList<T>) this;
                this.next = null;
            }
            TempList<T>.activeCount++;
            this.active = true;
        }
    }

    private void Bin()
    {
        if (!this.inDump)
        {
            if (this.active)
            {
                throw new InvalidOperationException();
            }
            this.next = TempList<T>.dump;
            if (TempList<T>.dumpCount++ != 0)
            {
                TempList<T>.dump.prev = (TempList<T>) this;
            }
            TempList<T>.dump = (TempList<T>) this;
            this.inDump = true;
            this.Clear();
        }
    }

    private void Deactivate()
    {
        if (this.active)
        {
            if (this.inDump)
            {
                throw new InvalidOperationException();
            }
            if (TempList<T>.lastActive == this)
            {
                if (TempList<T>.firstActive != this)
                {
                    TempList<T>.lastActive = this.prev;
                    this.prev.n = false;
                    this.prev.next = null;
                }
                else
                {
                    TempList<T>.lastActive = null;
                    TempList<T>.firstActive = null;
                }
            }
            else if (TempList<T>.firstActive == this)
            {
                this.next.p = false;
                this.next.prev = null;
                TempList<T>.firstActive = this.next;
            }
            else
            {
                this.prev.next = this.next;
                this.next.prev = this.prev;
            }
            this.prev = null;
            this.next = null;
            this.p = false;
            this.n = false;
            this.active = false;
            TempList<T>.activeCount--;
        }
    }

    public void Dispose()
    {
        this.Deactivate();
        this.Bin();
    }

    public static TempList<T> New()
    {
        TempList<T> list;
        if (TempList<T>.Resurrect(out list))
        {
            return list;
        }
        return new TempList<T>();
    }

    public static TempList<T> New(IEnumerable<T> windows)
    {
        TempList<T> list;
        if (TempList<T>.Resurrect(out list))
        {
            list.AddRange(windows);
            return list;
        }
        return new TempList<T>(windows);
    }

    private static bool Resurrect(out TempList<T> twl)
    {
        if (TempList<T>.dumpCount != 0)
        {
            twl = TempList<T>.dump;
            TempList<T>.dump = (--TempList<T>.dumpCount != 0) ? twl.prev : null;
            twl.inDump = false;
            twl.prev = null;
            return true;
        }
        twl = null;
        return false;
    }
}

