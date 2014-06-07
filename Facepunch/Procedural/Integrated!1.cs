namespace Facepunch.Procedural
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct Integrated<T> where T: struct
    {
        [NonSerialized]
        public MillisClock clock;
        [NonSerialized]
        public T begin;
        [NonSerialized]
        public T end;
        [NonSerialized]
        public T current;
        public void SetImmediate(ref T value)
        {
            this.begin = this.end = this.current = value;
            this.clock.SetImmediate();
        }
    }
}

