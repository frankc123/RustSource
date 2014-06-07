using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

[Serializable]
public class ArmorModelCollection<T> : IEnumerable, IEnumerable<T>, IEnumerable<KeyValuePair<ArmorModelSlot, T>>
{
    public T feet;
    public T head;
    public T legs;
    public T torso;

    public ArmorModelCollection()
    {
    }

    public ArmorModelCollection(T defaultValue)
    {
        for (ArmorModelSlot slot = ArmorModelSlot.Feet; slot < ((ArmorModelSlot) 4); slot = (ArmorModelSlot) (((int) slot) + 1))
        {
            this[slot] = defaultValue;
        }
    }

    public ArmorModelCollection(ArmorModelMemberMap<T> map) : this()
    {
        for (ArmorModelSlot slot = ArmorModelSlot.Feet; slot < ((ArmorModelSlot) 4); slot = (ArmorModelSlot) (((int) slot) + 1))
        {
            this[slot] = map[slot];
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

    public void Clear(T value)
    {
        for (ArmorModelSlot slot = ArmorModelSlot.Feet; slot < ((ArmorModelSlot) 4); slot = (ArmorModelSlot) (((int) slot) + 1))
        {
            this[slot] = value;
        }
    }

    public void CopyFrom(T[] array, int offset)
    {
        for (int i = 0; i < 4; i++)
        {
            this[(ArmorModelSlot) ((byte) i)] = array[offset++];
        }
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

    public Enumerator<T> GetEnumerator()
    {
        return new Enumerator<T>((ArmorModelCollection<T>) this);
    }

    IEnumerator<KeyValuePair<ArmorModelSlot, T>> IEnumerable<KeyValuePair<ArmorModelSlot, T>>.GetEnumerator()
    {
        return new Enumerator<T>((ArmorModelCollection<T>) this);
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return new Enumerator<T>((ArmorModelCollection<T>) this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return new Enumerator<T>((ArmorModelCollection<T>) this);
    }

    public ArmorModelMemberMap<T> ToMemberMap()
    {
        ArmorModelMemberMap<T> map = new ArmorModelMemberMap<T>();
        for (ArmorModelSlot slot = ArmorModelSlot.Feet; slot < ((ArmorModelSlot) 4); slot = (ArmorModelSlot) (((int) slot) + 1))
        {
            map[slot] = this[slot];
        }
        return map;
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
        private ArmorModelCollection<T> collection;
        private int index;
        internal Enumerator(ArmorModelCollection<T> collection)
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
            this = (ArmorModelCollection<>.Enumerator) new ArmorModelCollection<T>.Enumerator();
        }
    }
}

