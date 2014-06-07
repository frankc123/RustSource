namespace RustProto.Helpers
{
    using System;
    using System.Runtime.InteropServices;

    public sealed class Recycler<TMessage, TBuilder> : IDisposable where TMessage: GeneratedMessage<TMessage, TBuilder> where TBuilder: GeneratedBuilder<TMessage, TBuilder>, new()
    {
        private TBuilder Builder;
        private bool Cleared;
        private bool Created;
        private bool Disposed;
        private Recycler<TMessage, TBuilder> Next;
        private int OpenCount;
        private static Holding<TMessage, TBuilder> Recovery;

        private Recycler()
        {
        }

        public void CloseBuilder(ref TBuilder builder)
        {
            if (this.Disposed)
            {
                throw new ObjectDisposedException("Recycler");
            }
            if (this.OpenCount == 0)
            {
                throw new InvalidOperationException("Close was called more than Open for this Recycler");
            }
            if (!object.ReferenceEquals((TBuilder) builder, this.Builder))
            {
                throw new ArgumentOutOfRangeException("builder", "Was not opened by this recycler");
            }
            builder = null;
            if ((--this.OpenCount == 0) && !this.Cleared)
            {
                this.Builder.Clear();
                this.Cleared = true;
            }
        }

        public static Recycler<TMessage, TBuilder> Manufacture()
        {
            if (Recycler<TMessage, TBuilder>.Recovery.Count == 0)
            {
                return new Recycler<TMessage, TBuilder>();
            }
            Recycler<TMessage, TBuilder> pile = Recycler<TMessage, TBuilder>.Recovery.Pile;
            if (Recycler<TMessage, TBuilder>.Recovery.Count == 1)
            {
                Recycler<TMessage, TBuilder>.Recovery = new Holding<TMessage, TBuilder>();
            }
            else
            {
                Recycler<TMessage, TBuilder>.Recovery.Count--;
                Recycler<TMessage, TBuilder>.Recovery.Pile = pile.Next;
                pile.Next = null;
            }
            pile.Disposed = false;
            return pile;
        }

        public TBuilder OpenBuilder()
        {
            if (this.OpenCount++ == 0)
            {
                if (!this.Created)
                {
                    this.Builder = Activator.CreateInstance<TBuilder>();
                    this.Created = true;
                }
                else
                {
                    this.Cleared = false;
                }
            }
            return this.Builder;
        }

        public TBuilder OpenBuilder(TMessage copyFrom)
        {
            TBuilder local = this.OpenBuilder();
            local.MergeFrom(copyFrom);
            return local;
        }

        void IDisposable.Dispose()
        {
            if (!this.Disposed)
            {
                this.Disposed = true;
                if (Recycler<TMessage, TBuilder>.Recovery.Count++ == 0)
                {
                    this.Next = null;
                    Recycler<TMessage, TBuilder>.Recovery.Pile = (Recycler<TMessage, TBuilder>) this;
                }
                else
                {
                    this.Next = Recycler<TMessage, TBuilder>.Recovery.Pile;
                    Recycler<TMessage, TBuilder>.Recovery.Pile = (Recycler<TMessage, TBuilder>) this;
                }
                this.OpenCount = 0;
                if (this.Created && !this.Cleared)
                {
                    this.Builder.Clear();
                    this.Cleared = true;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Holding
        {
            public Recycler<TMessage, TBuilder> Pile;
            public int Count;
        }
    }
}

