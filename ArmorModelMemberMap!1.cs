using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

[Serializable, StructLayout(LayoutKind.Sequential)]
public struct ArmorModelMemberMap<T> : IEnumerable, IEnumerable<T>, IEnumerable<KeyValuePair<ArmorModelSlot, T>>
{
    public T feet;
    public T legs;
    public T torso;
    public T head;
    public ArmorModelMemberMap(T defaultValue)
    {
        this = (ArmorModelMemberMap) new ArmorModelMemberMap<T>();
        for (ArmorModelSlot slot = ArmorModelSlot.Feet; slot < ((ArmorModelSlot) 4); slot = (ArmorModelSlot) (((int) slot) + 1))
        {
            this[slot] = defaultValue;
        }
    }

    unsafe IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return new Enumerator<T>(*((ArmorModelMemberMap<T>*) this));
    }

    unsafe IEnumerator<KeyValuePair<ArmorModelSlot, T>> IEnumerable<KeyValuePair<ArmorModelSlot, T>>.GetEnumerator()
    {
        return new Enumerator<T>(*((ArmorModelMemberMap<T>*) this));
    }

    unsafe IEnumerator IEnumerable.GetEnumerator()
    {
        return new Enumerator<T>(*((ArmorModelMemberMap<T>*) this));
    }

    public void Clear(T value)
    {
        for (ArmorModelSlot slot = ArmorModelSlot.Feet; slot < ((ArmorModelSlot) 4); slot = (ArmorModelSlot) (((int) slot) + 1))
        {
            this[slot] = value;
        }
    }

    public void Clear()
    {
        for (ArmorModelSlot slot = ArmorModelSlot.Feet; slot < ((ArmorModelSlot) 4); slot = (ArmorModelSlot) (((int) slot) + 1))
        {
            T local = default(T);
            this[slot] = local;
        }
    }

    public unsafe Enumerator<T> GetEnumerator()
    {
        return new Enumerator<T>(*((ArmorModelMemberMap<T>*) this));
    }

    public int CopyTo(T[] array, int offset, int maxCount)
    {
        int num = (maxCount >= 4) ? 4 : maxCount;
        for (int i = 0; i < 4; i++)
        {
            array[offset++] = this[(ArmorModelSlot) ((byte) i)];
        }
        return offset;
    }

    public void CopyFrom(T[] array, int offset)
    {
        for (int i = 0; i < 4; i++)
        {
            this[(ArmorModelSlot) ((byte) i)] = array[offset++];
        }
    }

    public T this[ArmorModelSlot slot]
    {
        get
        {
            switch (slot)
            {
                case ArmorModelSlot.Feet:
                    return this.feet;

                case ArmorModelSlot.Legs:
                    return this.legs;

                case ArmorModelSlot.Torso:
                    return this.torso;

                case ArmorModelSlot.Head:
                    return this.head;
            }
            return default(T);
        }
        set
        {
            switch (slot)
            {
                case ArmorModelSlot.Feet:
                    this.feet = value;
                    break;

                case ArmorModelSlot.Legs:
                    this.legs = value;
                    break;

                case ArmorModelSlot.Torso:
                    this.torso = value;
                    break;

                case ArmorModelSlot.Head:
                    this.head = value;
                    break;
            }
        }
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct Enumerator : IDisposable, IEnumerator, IEnumerator<T>, IEnumerator<KeyValuePair<ArmorModelSlot, T>>
    {
        private ArmorModelMemberMap<T> collection;
        private int index;
        internal Enumerator(ArmorModelMemberMap<T> collection)
        {
            this.collection = collection;
            this.index = -1;
        }

        KeyValuePair<ArmorModelSlot, T> IEnumerator<KeyValuePair<ArmorModelSlot, T>>.Current
        {
            get
            {
                if ((this.index <= 0) || (this.index >= 4))
                {
                    throw new InvalidOperationException();
                }
                return new KeyValuePair<ArmorModelSlot, T>((ArmorModelSlot) ((byte) this.index), this.collection[(ArmorModelSlot) ((byte) this.index)]);
            }
        }
        object IEnumerator.Current
        {
            get
            {
                return this.Current;
            }
        }
        public T Current
        {
            get
            {
                return (((this.index <= 0) || (this.index >= 4)) ? default(T) : this.collection[(ArmorModelSlot) ((byte) this.index)]);
            }
        }
        public bool MoveNext()
        {
            return (++this.index < 4);
        }

        public void Reset()
        {
            this.index = -1;
        }

        public void Dispose()
        {
            this = (ArmorModelMemberMap<>.Enumerator) new ArmorModelMemberMap<T>.Enumerator();
        }
    }
}

