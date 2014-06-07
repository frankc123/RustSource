using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

public class MultiValue<TValue> : IEnumerable, IEnumerable<TValue>, ICollection<TValue>, IList<TValue>, ICloneable
{
    private int count;
    private HashSet<TValue> hashSet;
    private const int kCheckHashCountMin = 0x10;
    private const bool kIsReadOnly = false;
    private List<TValue> list;

    public MultiValue()
    {
        this.list = new List<TValue>();
        this.hashSet = new HashSet<TValue>();
    }

    private MultiValue(bool ignore)
    {
    }

    public MultiValue(IEnumerable<TValue> v)
    {
        InitData<TValue> data;
        this.hashSet = new HashSet<TValue>();
        data.mv = (MultiValue<TValue>) this;
        using (data.enumerator = v.GetEnumerator())
        {
            data.RecurseInit();
        }
    }

    public MultiValue(IEqualityComparer<TValue> equalityComparer)
    {
        this.hashSet = new HashSet<TValue>(equalityComparer);
        this.list = new List<TValue>();
    }

    public MultiValue(int capacity, IEqualityComparer<TValue> equalityComparer)
    {
        this.hashSet = new HashSet<TValue>(equalityComparer);
        this.list = new List<TValue>(capacity);
    }

    public MultiValue(IEnumerable<TValue> v, IEqualityComparer<TValue> equalityComparer)
    {
        InitData<TValue> data;
        this.hashSet = new HashSet<TValue>(equalityComparer);
        data.mv = (MultiValue<TValue>) this;
        using (data.enumerator = v.GetEnumerator())
        {
            data.RecurseInit();
        }
    }

    private MultiValue(IEqualityComparer<TValue> comparer, MultiValue<TValue> mv)
    {
        this.list = new List<TValue>(mv.list);
        this.hashSet = new HashSet<TValue>(mv.hashSet, comparer);
        this.count = mv.count;
    }

    public bool Add(TValue item)
    {
        if (this.hashSet.Add(item))
        {
            this.list.Add(item);
            this.count++;
            return true;
        }
        return false;
    }

    public int AddRange(IEnumerable<TValue> value)
    {
        int num = 0;
        IEnumerator<TValue> enumerator = value.GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                TValue current = enumerator.Current;
                if (this.Add(current))
                {
                    num++;
                }
            }
        }
        finally
        {
            if (enumerator == null)
            {
            }
            enumerator.Dispose();
        }
        return num;
    }

    public bool Clear()
    {
        if (this.count > 0)
        {
            this.list.Clear();
            this.hashSet.Clear();
            this.count = 0;
            return true;
        }
        return false;
    }

    public MultiValue<TValue> Clone()
    {
        return new MultiValue<TValue>(false) { hashSet = new HashSet<TValue>(this.hashSet), list = new List<TValue>(this.list), count = this.count };
    }

    public bool Clone(IEqualityComparer<TValue> valueComparer, out MultiValue<TValue> val)
    {
        if (this.count == 0)
        {
            val = null;
            return false;
        }
        if (valueComparer == this.hashSet.Comparer)
        {
            val = this.Clone();
            return true;
        }
        val = new MultiValue<TValue>(this.list, valueComparer);
        if (val.count == 0)
        {
            val = null;
            return false;
        }
        return true;
    }

    public bool Contains(TValue item)
    {
        return this.hashSet.Contains(item);
    }

    public void CopyTo(TValue[] array, int arrayIndex)
    {
        this.list.CopyTo(array, arrayIndex);
    }

    public List<TValue>.Enumerator GetEnumerator()
    {
        return this.list.GetEnumerator();
    }

    public int IndexOf(TValue item)
    {
        if ((this.count >= 0x10) && !this.hashSet.Contains(item))
        {
            return -1;
        }
        return this.list.IndexOf(item);
    }

    public int InsertOrMove(int index, TValue item)
    {
        if (index != this.count)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index", "index < 0");
            }
            if (index > this.count)
            {
                throw new ArgumentOutOfRangeException("index", "index > count");
            }
            if (this.hashSet.Add(item))
            {
                this.list.Insert(index, item);
                this.count++;
                return 1;
            }
            int num2 = this.list.IndexOf(item);
            int num3 = index - num2;
            int num6 = num3;
            switch ((num6 + 1))
            {
                case 0:
                    this.list.Reverse(num2, 2);
                    goto Label_01D8;

                case 1:
                    return 0;

                case 2:
                    this.list.Reverse(index, 2);
                    goto Label_01D8;
            }
            if (num3 <= -2)
            {
                for (int i = num2; i > index; i--)
                {
                    this.list[i] = this.list[i - 1];
                }
            }
            else if (num3 >= 2)
            {
                for (int j = num2; j < index; j++)
                {
                    this.list[j] = this.list[j + 1];
                }
            }
            this.list[index] = item;
        }
        else
        {
            if (this.hashSet.Add(item))
            {
                this.list.Add(item);
                this.count++;
                return 1;
            }
            int num = this.list.IndexOf(item);
            switch ((this.count - num))
            {
                case 1:
                    return 0;

                case 2:
                    this.list.Reverse(this.count - 2, 2);
                    break;

                default:
                    this.list.RemoveAt(num);
                    this.list.Add(item);
                    break;
            }
            return 2;
        }
    Label_01D8:
        return 2;
    }

    public int Remove(TValue item)
    {
        if (!this.hashSet.Remove(item))
        {
            return 0;
        }
        if (!this.list.Remove(item))
        {
            this.hashSet.Add(item);
            return 0;
        }
        return ((--this.count != 0) ? 1 : 2);
    }

    public bool RemoveAt(int index)
    {
        TValue item = this.list[index];
        this.list.RemoveAt(index);
        this.hashSet.Remove(item);
        return (--this.count != 0);
    }

    public void Set(MultiValue<TValue> other)
    {
        if (other != this)
        {
            this.Clear();
            foreach (TValue local in other)
            {
                this.Add(local);
            }
        }
    }

    void ICollection<TValue>.Add(TValue item)
    {
        if (this.hashSet.Add(item))
        {
            this.list.Add(item);
            this.count++;
        }
    }

    void ICollection<TValue>.Clear()
    {
        if (this.count > 0)
        {
            this.list.Clear();
            this.hashSet.Clear();
            this.count = 0;
        }
    }

    bool ICollection<TValue>.Remove(TValue value)
    {
        return (this.Remove(value) != 0);
    }

    IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
    {
        return this.list.GetEnumerator();
    }

    void IList<TValue>.Insert(int index, TValue value)
    {
        this.InsertOrMove(index, value);
    }

    void IList<TValue>.RemoveAt(int index)
    {
        TValue item = this.list[index];
        this.list.RemoveAt(index);
        this.hashSet.Remove(item);
        this.count--;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.list.GetEnumerator();
    }

    object ICloneable.Clone()
    {
        return this.Clone();
    }

    public int Count
    {
        get
        {
            return this.count;
        }
    }

    public TValue this[int index]
    {
        get
        {
            return this.list[index];
        }
        set
        {
            this.list[index] = value;
        }
    }

    bool ICollection<TValue>.IsReadOnly
    {
        get
        {
            return false;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct InitData
    {
        public MultiValue<TValue> mv;
        public IEnumerator<TValue> enumerator;
        public void RecurseInit()
        {
            while (this.enumerator.MoveNext())
            {
                TValue current = this.enumerator.Current;
                if (this.mv.hashSet.Add(current))
                {
                    this.mv.count++;
                    this.RecurseInit();
                    this.mv.list.Add(current);
                    return;
                }
            }
            this.mv.list = new List<TValue>(this.mv.count);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KeyPair<TKey> : IEnumerable, IEnumerable<TValue>, ICollection<TValue>, IList<TValue>
    {
        private readonly TKey key;
        private readonly DictionaryMultiValue<TKey, TValue> dict;
        public KeyPair(DictionaryMultiValue<TKey, TValue> dict, TKey key)
        {
            this.dict = dict;
            this.key = key;
        }

        IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
        {
            MultiValue<TValue> value2;
            if (this.GetMultiValue(out value2))
            {
                return value2.GetEnumerator();
            }
            return g<TValue, TKey>.emptyList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            MultiValue<TValue> value2;
            if (this.GetMultiValue(out value2))
            {
                return value2.GetEnumerator();
            }
            return g<TValue, TKey>.emptyList.GetEnumerator();
        }

        bool ICollection<TValue>.Remove(TValue value)
        {
            MultiValue<TValue> value2;
            return (this.GetMultiValue(out value2) && value2.Remove(value));
        }

        void IList<TValue>.RemoveAt(int index)
        {
            MultiValue<TValue> value2;
            if (!this.GetMultiValue(out value2))
            {
                g<TValue, TKey>.emptyList.RemoveAt(index);
            }
            else
            {
                value2.RemoveAt(index);
            }
        }

        void IList<TValue>.Insert(int index, TValue value)
        {
            MultiValue<TValue> value2;
            bool orCreateMultiValue = this.GetOrCreateMultiValue(out value2);
            value2.Insert(index, value);
            if (!orCreateMultiValue && (value2.count > 0))
            {
                this.Bind(value2);
            }
        }

        void ICollection<TValue>.Add(TValue item)
        {
            MultiValue<TValue> value2;
            bool orCreateMultiValue = this.GetOrCreateMultiValue(out value2);
            value2.Add(item);
            if (!orCreateMultiValue && (value2.count != 0))
            {
                this.Bind(value2);
            }
        }

        void ICollection<TValue>.Clear()
        {
            MultiValue<TValue> value2;
            if (this.GetMultiValue(out value2))
            {
                value2.Clear();
            }
        }

        bool ICollection<TValue>.IsReadOnly
        {
            get
            {
                return false;
            }
        }
        public TKey Key
        {
            get
            {
                return this.key;
            }
        }
        public DictionaryMultiValue<TKey, TValue> Dictionary
        {
            get
            {
                return this.dict;
            }
        }
        public bool Valid
        {
            get
            {
                return (this.dict != null);
            }
        }
        private bool GetMultiValue(out MultiValue<TValue> v)
        {
            if (this.dict == null)
            {
                v = null;
            }
            return this.dict.GetMultiValue(this.key, out v);
        }

        private bool GetOrCreateMultiValue(out MultiValue<TValue> v)
        {
            if (this.dict == null)
            {
                throw new InvalidOperationException("The KeyPair is invalid");
            }
            return this.dict.GetOrCreateMultiValue(this.key, out v);
        }

        private void Bind(MultiValue<TValue> v)
        {
            this.dict.SetMultiValue(this.key, v);
        }

        public List<TValue>.Enumerator GetEnumerator()
        {
            MultiValue<TValue> value2;
            if (this.GetMultiValue(out value2))
            {
                return value2.GetEnumerator();
            }
            return g<TValue, TKey>.emptyList.GetEnumerator();
        }

        public int Count
        {
            get
            {
                MultiValue<TValue> value2;
                return (!this.GetMultiValue(out value2) ? value2.Count : 0);
            }
        }
        public bool Add(TValue value)
        {
            MultiValue<TValue> value2;
            bool orCreateMultiValue = this.GetOrCreateMultiValue(out value2);
            if (!value2.Add(value))
            {
                return false;
            }
            if (!orCreateMultiValue)
            {
                this.Bind(value2);
            }
            return true;
        }

        public int InsertOrMove(int index, TValue item)
        {
            MultiValue<TValue> value2;
            bool orCreateMultiValue = this.GetOrCreateMultiValue(out value2);
            int num = value2.InsertOrMove(index, item);
            if ((num == 1) && !orCreateMultiValue)
            {
                this.Bind(value2);
            }
            return num;
        }

        public int Remove(TValue value)
        {
            MultiValue<TValue> value2;
            return (!this.GetMultiValue(out value2) ? 0 : value2.Remove(value));
        }

        public bool Contains(TValue value)
        {
            MultiValue<TValue> value2;
            return (this.GetMultiValue(out value2) && value2.Contains(value));
        }

        public bool Clear(TValue value)
        {
            MultiValue<TValue> value2;
            return (this.GetMultiValue(out value2) && value2.Clear());
        }

        public bool RemoveAt(int index)
        {
            MultiValue<TValue> value2;
            return (this.GetMultiValue(out value2) && value2.RemoveAt(index));
        }

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            MultiValue<TValue> value2;
            if (this.GetMultiValue(out value2))
            {
                value2.CopyTo(array, arrayIndex);
            }
            else
            {
                g<TValue, TKey>.emptyList.CopyTo(array, arrayIndex);
            }
        }

        public int IndexOf(TValue item)
        {
            MultiValue<TValue> value2;
            return (!this.GetMultiValue(out value2) ? -1 : value2.IndexOf(item));
        }

        public TValue this[int i]
        {
            get
            {
                MultiValue<TValue> value2;
                if (!this.GetMultiValue(out value2))
                {
                    return g<TValue, TKey>.emptyList[i];
                }
                return value2[i];
            }
            set
            {
                MultiValue<TValue> value2;
                if (!this.GetMultiValue(out value2))
                {
                    g<TValue, TKey>.emptyList[i] = value;
                }
                else
                {
                    value2[i] = value;
                }
            }
        }
        private static class g
        {
            public static readonly List<TValue> emptyList;

            static g()
            {
                MultiValue<TValue>.KeyPair<TKey>.g.emptyList = new List<TValue>(0);
            }
        }
    }
}

