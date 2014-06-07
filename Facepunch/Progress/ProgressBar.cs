namespace Facepunch.Progress
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public sealed class ProgressBar
    {
        private float bonus;
        private int count;
        private float denom;
        private readonly List<IProgress> List = new List<IProgress>();

        public void Add(IProgress IProgress)
        {
            if (!object.ReferenceEquals(IProgress, null))
            {
                this.List.Add(IProgress);
                this.count++;
                this.denom++;
            }
        }

        public void Add(AsyncOperation Progress)
        {
            if (!object.ReferenceEquals(Progress, null))
            {
                this.Add(new AsyncOperationProgress(Progress));
            }
        }

        public void AddMultiple<T>(IEnumerable<T> collection) where T: IProgress
        {
            IEnumerator<T> enumerator = collection.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    T current = enumerator.Current;
                    this.Add(current);
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
        }

        public void Clean()
        {
            float num;
            this.Update(out num);
        }

        public void Clear()
        {
            this.bonus = this.denom = 0f;
            this.List.Clear();
            this.count = 0;
        }

        public bool Update(out float progress)
        {
            if (this.count == 0)
            {
                progress = 0f;
                return false;
            }
            float num = 0f;
            int num3 = 0;
            int count = this.count;
            for (int i = count - 1; num3 < count; i--)
            {
                float num2;
                if (this.List[i].Poll(out num2) && (num2 < 1f))
                {
                    num += num2;
                }
                else if (--this.count > 0)
                {
                    this.bonus++;
                    this.List.RemoveAt(i);
                }
                else
                {
                    this.Clear();
                    progress = 1f;
                    return true;
                }
                num3++;
            }
            if ((progress = (num + this.bonus) / this.denom) > 1f)
            {
                progress = 1f;
            }
            return true;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct AsyncOperationProgress : IProgress
        {
            public readonly AsyncOperation aop;
            public AsyncOperationProgress(AsyncOperation aop)
            {
                this.aop = aop;
            }

            public float progress
            {
                get
                {
                    return (((this.aop != null) && !this.aop.isDone) ? (this.aop.progress * 0.999f) : 1f);
                }
            }
        }
    }
}

