using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public class DictionaryMultiValue<TKey, TValue> : IEnumerable, IEnumerable<MultiValue<TValue>.KeyPair<TKey>>
{
    private Dictionary<TKey, MultiValue<TValue>> dict;
    public readonly bool HasKeyComparer;
    public readonly bool HasValueComparer;
    public readonly IEqualityComparer<TValue> ValueComparer;

    public DictionaryMultiValue(IEnumerable<KeyValuePair<TKey, TValue>> dict, IEqualityComparer<TKey> keyComp, IEqualityComparer<TValue> valComp)
    {
        this.HasKeyComparer = keyComp != null;
        this.HasValueComparer = valComp != null;
        this.ValueComparer = valComp;
        this.dict = !this.HasKeyComparer ? new Dictionary<TKey, MultiValue<TValue>>() : new Dictionary<TKey, MultiValue<TValue>>(keyComp);
        this.AddRange(dict);
    }

    public bool Add(KeyValuePair<TKey, TValue> kv)
    {
        MultiValue<TValue> value2;
        if (this.GetOrCreateMultiValue(kv.Key, out value2))
        {
            return value2.Add(kv.Value);
        }
        if (value2.Add(kv.Value))
        {
            this.dict.Add(kv.Key, value2);
            return true;
        }
        return false;
    }

    public bool Add(TKey key, TValue value)
    {
        MultiValue<TValue> value2;
        if (this.GetOrCreateMultiValue(key, out value2))
        {
            return value2.Add(value);
        }
        if (value2.Add(value))
        {
            this.dict.Add(key, value2);
            return true;
        }
        return false;
    }

    public int AddRange(IEnumerable<KeyValuePair<TKey, TValue>> pairs)
    {
        int num = 0;
        IEnumerator<KeyValuePair<TKey, TValue>> enumerator = pairs.GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                KeyValuePair<TKey, TValue> current = enumerator.Current;
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

    public int AddRange<TValueEnumerable>(IEnumerable<KeyValuePair<TKey, TValueEnumerable>> pairs) where TValueEnumerable: IEnumerable<TValue>
    {
        int num = 0;
        IEnumerator<KeyValuePair<TKey, TValueEnumerable>> enumerator = pairs.GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                KeyValuePair<TKey, TValueEnumerable> current = enumerator.Current;
                num += this.AddRange<TValueEnumerable>(current);
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

    public int AddRange<TValueEnumerable>(KeyValuePair<TKey, TValueEnumerable> kv) where TValueEnumerable: IEnumerable<TValue>
    {
        MultiValue<TValue> value2;
        if (this.GetOrCreateMultiValue(kv.Key, out value2))
        {
            return value2.AddRange(kv.Value);
        }
        int count = value2.Count;
        if (count > 0)
        {
            this.dict.Add(kv.Key, value2);
        }
        return count;
    }

    public int AddRange(TKey key, IEnumerable<TValue> value)
    {
        MultiValue<TValue> value2;
        if (this.GetOrCreateMultiValue(key, out value2, value))
        {
            return value2.AddRange(value);
        }
        int count = value2.Count;
        if (count > 0)
        {
            this.dict.Add(key, value2);
        }
        return count;
    }

    private bool AreEqual(TKey l, TKey r)
    {
        IEqualityComparer<TKey> comparer = this.dict.Comparer;
        return ((comparer.GetHashCode(l) == comparer.GetHashCode(r)) && comparer.Equals(l, r));
    }

    public bool Clear(TKey key)
    {
        MultiValue<TValue> value2;
        return (this.GetMultiValue(key, out value2) && value2.Clear());
    }

    public bool Clear(TKey key, bool erase)
    {
        return (this.Clear(key) && this.dict.Remove(key));
    }

    public bool Contains(KeyValuePair<TKey, TValue> kv)
    {
        return this.Contains(kv.Key, kv.Value);
    }

    public bool Contains(TKey key, TValue value)
    {
        MultiValue<TValue> value2;
        return (this.GetMultiValue(key, out value2) && value2.Contains(value));
    }

    public bool ContainsKey(TKey key)
    {
        return this.dict.ContainsKey(key);
    }

    public bool ContainsValue(TValue value)
    {
        foreach (MultiValue<TValue> value2 in this.dict.Values)
        {
            if (value2.Contains(value))
            {
                return true;
            }
        }
        return false;
    }

    public bool ContainsValue(TKey key, TValue value)
    {
        return this.Contains(key, value);
    }

    private MultiValue<TValue> CreateMultiValue()
    {
        if (this.HasValueComparer)
        {
            return new MultiValue<TValue>(this.ValueComparer);
        }
        return new MultiValue<TValue>();
    }

    private MultiValue<TValue> CreateMultiValue(IEnumerable<TValue> enumerable)
    {
        if (this.HasValueComparer)
        {
            return new MultiValue<TValue>(enumerable, this.ValueComparer);
        }
        return new MultiValue<TValue>(enumerable);
    }

    [DebuggerHidden]
    public IEnumerator<MultiValue<TValue>.KeyPair<TKey>> GetEnumerator()
    {
        return new <GetEnumerator>c__Iterator23<TKey, TValue> { <>f__this = (DictionaryMultiValue<TKey, TValue>) this };
    }

    internal bool GetMultiValue(TKey key, out MultiValue<TValue> v)
    {
        return this.dict.TryGetValue(key, out v);
    }

    internal bool GetOrCreateMultiValue(TKey key, out MultiValue<TValue> v)
    {
        if (this.dict.TryGetValue(key, out v))
        {
            return true;
        }
        v = this.CreateMultiValue();
        return false;
    }

    internal bool GetOrCreateMultiValue(TKey key, out MultiValue<TValue> v, IEnumerable<TValue> enumerable)
    {
        if (this.dict.TryGetValue(key, out v))
        {
            return true;
        }
        v = this.CreateMultiValue(enumerable);
        return false;
    }

    public bool Remove(TKey key)
    {
        MultiValue<TValue> value2;
        return ((this.GetMultiValue(key, out value2) && this.dict.Remove(key)) && value2.Clear());
    }

    public bool RemoveAt(TKey key, int index)
    {
        MultiValue<TValue> value2;
        return (this.GetMultiValue(key, out value2) && value2.RemoveAt(index));
    }

    internal void SetMultiValue(TKey key, MultiValue<TValue> mv)
    {
        this.dict.Add(key, mv);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    public int ValueCount(TKey key)
    {
        MultiValue<TValue> value2;
        return (!this.GetMultiValue(key, out value2) ? 0 : value2.Count);
    }

    public MultiValue<TValue>.KeyPair<TKey> this[TKey key]
    {
        get
        {
            return new MultiValue<TValue>.KeyPair<TKey>((DictionaryMultiValue<TKey, TValue>) this, key);
        }
        set
        {
            if (value.Dictionary == this)
            {
                if (this.AreEqual(value.Key, key))
                {
                    MultiValue<TValue> value3;
                    if (this.GetMultiValue(value.Key, out value3))
                    {
                        MultiValue<TValue> value2;
                        if (this.GetMultiValue(key, out value2))
                        {
                            value2.Set(value3);
                        }
                        else if (value3.Count > 0)
                        {
                            this.dict.Add(value.Key, value3.Clone());
                        }
                    }
                }
                else if (value.Valid)
                {
                    MultiValue<TValue> value5;
                    if (value.Dictionary.GetMultiValue(value.Key, out value5))
                    {
                        MultiValue<TValue> value4;
                        if (this.GetMultiValue(key, out value4))
                        {
                            value4.Set(value5);
                        }
                        else if ((value5.Count > 0) && value5.Clone(this.ValueComparer, out value4))
                        {
                            this.dict.Add(value.Key, value4);
                        }
                    }
                }
                else
                {
                    MultiValue<TValue> value6;
                    if (value.Dictionary.GetMultiValue(value.Key, out value6))
                    {
                        value6.Clear();
                    }
                }
            }
        }
    }

    public IEqualityComparer<TKey> KeyComparer
    {
        get
        {
            return this.dict.Comparer;
        }
    }

    [CompilerGenerated]
    private sealed class <GetEnumerator>c__Iterator23 : IDisposable, IEnumerator, IEnumerator<MultiValue<TValue>.KeyPair<TKey>>
    {
        internal MultiValue<TValue>.KeyPair<TKey> $current;
        internal int $PC;
        internal Dictionary<TKey, MultiValue<TValue>>.KeyCollection.Enumerator <$s_212>__0;
        internal DictionaryMultiValue<TKey, TValue> <>f__this;
        internal TKey <key>__1;

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
                        this.<$s_212>__0.Dispose();
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
                    this.<$s_212>__0 = this.<>f__this.dict.Keys.GetEnumerator();
                    num = 0xfffffffd;
                    break;

                case 1:
                    break;

                default:
                    goto Label_00B9;
            }
            try
            {
                while (this.<$s_212>__0.MoveNext())
                {
                    this.<key>__1 = this.<$s_212>__0.Current;
                    this.$current = new MultiValue<TValue>.KeyPair<TKey>(this.<>f__this, this.<key>__1);
                    this.$PC = 1;
                    flag = true;
                    return true;
                }
            }
            finally
            {
                if (!flag)
                {
                }
                this.<$s_212>__0.Dispose();
            }
            this.$PC = -1;
        Label_00B9:
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        MultiValue<TValue>.KeyPair<TKey> IEnumerator<MultiValue<TValue>.KeyPair<TKey>>.Current
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
}

