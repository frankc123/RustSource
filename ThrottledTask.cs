using Facepunch.Clocks.Counters;
using Facepunch.Progress;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

[AddComponentMenu("")]
public abstract class ThrottledTask : MonoBehaviour
{
    [NonSerialized]
    private bool added;
    private static List<ThrottledTask> AllTasks = new List<ThrottledTask>();
    private const int kTargetMSPerFrame = 400;
    private static int numWorking;
    [NonSerialized]
    private bool working;

    protected ThrottledTask()
    {
    }

    protected void Awake()
    {
        if (!this.added)
        {
            this.added = true;
            AllTasks.Add(this);
        }
    }

    protected void OnDestroy()
    {
        if (this.added)
        {
            AllTasks.Remove(this);
        }
        else
        {
            this.added = true;
        }
        this.SetWorking(false);
    }

    private void SetWorking(bool on)
    {
        if (on != this.working)
        {
            this.working = on;
            if (on)
            {
                numWorking++;
            }
            else
            {
                numWorking--;
            }
        }
    }

    public static ThrottledTask[] AllWorkingTasks
    {
        get
        {
            ThrottledTask[] taskArray = new ThrottledTask[numWorking];
            int num = 0;
            foreach (ThrottledTask task in AllTasks)
            {
                if (task.working)
                {
                    taskArray[num++] = task;
                    if (num == numWorking)
                    {
                        return taskArray;
                    }
                }
            }
            return taskArray;
        }
    }

    public static IEnumerable<IProgress> AllWorkingTasksProgress
    {
        get
        {
            return new <>c__IteratorC { $PC = -2 };
        }
    }

    protected Timer Begin
    {
        get
        {
            return Timer.Start;
        }
    }

    public static bool Operational
    {
        get
        {
            return (numWorking > 0);
        }
    }

    public bool Working
    {
        get
        {
            return this.working;
        }
        protected set
        {
            this.SetWorking(value);
        }
    }

    [CompilerGenerated]
    private sealed class <>c__IteratorC : IDisposable, IEnumerator, IEnumerable, IEnumerable<IProgress>, IEnumerator<IProgress>
    {
        internal IProgress $current;
        internal int $PC;
        internal List<ThrottledTask>.Enumerator <$s_118>__0;
        internal ThrottledTask <task>__1;

        [DebuggerHidden]
        public void Dispose()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            switch (num)
            {
                case 1:
                    try
                    {
                    }
                    finally
                    {
                        this.<$s_118>__0.Dispose();
                    }
                    break;
            }
        }

        public bool MoveNext()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            bool flag = false;
            switch (num)
            {
                case 0:
                    this.<$s_118>__0 = ThrottledTask.AllTasks.GetEnumerator();
                    num = 0xfffffffd;
                    break;

                case 1:
                    break;

                default:
                    goto Label_00C8;
            }
            try
            {
                while (this.<$s_118>__0.MoveNext())
                {
                    this.<task>__1 = this.<$s_118>__0.Current;
                    if (this.<task>__1.working && (this.<task>__1 is IProgress))
                    {
                        this.$current = this.<task>__1 as IProgress;
                        this.$PC = 1;
                        flag = true;
                        return true;
                    }
                }
            }
            finally
            {
                if (!flag)
                {
                }
                this.<$s_118>__0.Dispose();
            }
            this.$PC = -1;
        Label_00C8:
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        IEnumerator<IProgress> IEnumerable<IProgress>.GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
            {
                return this;
            }
            return new ThrottledTask.<>c__IteratorC();
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.System.Collections.Generic.IEnumerable<Facepunch.Progress.IProgress>.GetEnumerator();
        }

        IProgress IEnumerator<IProgress>.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }

        object IEnumerator.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    protected struct Timer
    {
        private readonly SystemTimestamp clock;
        private Timer(SystemTimestamp clock)
        {
            this.clock = clock;
        }

        internal static ThrottledTask.Timer Start
        {
            get
            {
                return new ThrottledTask.Timer(SystemTimestamp.Restart);
            }
        }
        public bool Continue
        {
            get
            {
                return ((ThrottledTask.numWorking == 0) || (this.clock.Elapsed.TotalMilliseconds < (400.0 / ((double) ThrottledTask.numWorking))));
            }
        }
    }
}

