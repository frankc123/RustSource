using System;
using UnityEngine;

public class VisQuery : ScriptableObject
{
    [SerializeField]
    protected VisAction[] actions;
    [SerializeField]
    protected VisEval evaluation;
    [SerializeField]
    protected bool nonInstance;

    private void Enter(VisNode a, VisNode b)
    {
        IDMain idMain = a.idMain;
        IDMain instigator = !this.nonInstance ? b.idMain : null;
        for (int i = 0; i < this.actions.Length; i++)
        {
            if (this.actions[i] != null)
            {
                this.actions[i].Accomplish(idMain, instigator);
            }
        }
    }

    private void Exit(VisNode a, VisNode b)
    {
        IDMain idMain = a.idMain;
        IDMain instigator = !this.nonInstance ? b.idMain : null;
        for (int i = 0; i < this.actions.Length; i++)
        {
            if (this.actions[i] != null)
            {
                this.actions[i].UnAcomplish(idMain, instigator);
            }
        }
    }

    private bool Try(VisNode self, VisNode instigator)
    {
        Vis.Mask traitMask = self.traitMask;
        Vis.Mask mask2 = instigator.traitMask;
        return this.evaluation.Pass(traitMask, mask2);
    }

    public class Instance
    {
        private readonly HSet<VisNode> applicable;
        private readonly long bit;
        private readonly byte bitNumber;
        private int execNum;
        private int num;
        public readonly VisQuery outer;

        internal Instance(VisQuery outer, ref int bit)
        {
            this.outer = outer;
            this.applicable = new HSet<VisNode>();
            this.bit = ((int) 1) << bit;
            this.bitNumber = (byte) bit;
            bit++;
        }

        public void Clear(VisNode self)
        {
            while (--this.num >= 0)
            {
                HSetIter<VisNode> enumerator = this.applicable.GetEnumerator();
                enumerator.MoveNext();
                VisNode current = enumerator.Current;
                enumerator.Dispose();
                this.TryRemove(self, current);
            }
        }

        public void Execute(VisQuery.TryResult res, VisNode self, VisNode other)
        {
            switch (res)
            {
                case VisQuery.TryResult.Enter:
                    this.ExecuteEnter(self, other);
                    break;

                case VisQuery.TryResult.Exit:
                    this.ExecuteExit(self, other);
                    break;
            }
        }

        public void ExecuteEnter(VisNode self, VisNode other)
        {
            if ((this.execNum++ == 0) || !this.outer.nonInstance)
            {
                this.outer.Enter(self, other);
            }
        }

        public void ExecuteExit(VisNode self, VisNode other)
        {
            if ((--this.execNum == 0) || !this.outer.nonInstance)
            {
                this.outer.Exit(self, other);
            }
        }

        public bool Fits(VisNode other)
        {
            return this.applicable.Contains(other);
        }

        public bool IsActive(long mask)
        {
            return ((mask & this.bit) == this.bit);
        }

        public VisQuery.TryResult TryAdd(VisNode self, VisNode other)
        {
            if (!this.outer.Try(self, other))
            {
                return this.TryRemove(self, other);
            }
            if (this.applicable.Add(other))
            {
                this.num++;
                return VisQuery.TryResult.Enter;
            }
            return VisQuery.TryResult.Stay;
        }

        public VisQuery.TryResult TryRemove(VisNode self, VisNode other)
        {
            if (this.applicable.Remove(other))
            {
                this.num--;
                return VisQuery.TryResult.Exit;
            }
            return VisQuery.TryResult.Outside;
        }

        public int count
        {
            get
            {
                return this.num;
            }
        }
    }

    public enum TryResult
    {
        Outside,
        Enter,
        Stay,
        Exit
    }
}

