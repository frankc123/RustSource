namespace Facepunch.Abstract
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    internal class KeyTypeInfo<Key> where Key: TraitKey
    {
        public readonly HashSet<KeyTypeInfo<Key>> AssignableTo;
        public readonly KeyTypeInfo<Key> Base;
        public readonly KeyTypeInfo<Key> Root;
        public readonly int TraitDepth;
        public readonly System.Type Type;

        private KeyTypeInfo(System.Type Type, KeyTypeInfo<Key> Base, KeyTypeInfo<Key> Root, int TraitDepth)
        {
            this.Type = Type;
            this.Base = Base;
            if (Root == null)
            {
            }
            this.Root = (KeyTypeInfo<Key>) this;
            this.TraitDepth = TraitDepth;
            if (this.Root == this)
            {
                this.AssignableTo = new HashSet<KeyTypeInfo<Key>>();
            }
            else
            {
                this.AssignableTo = new HashSet<KeyTypeInfo<Key>>(this.Base.AssignableTo);
            }
            this.AssignableTo.Add((KeyTypeInfo<Key>) this);
            Registration<Key>.Add((KeyTypeInfo<Key>) this);
        }

        public static KeyTypeInfo<Key> Find<T>() where T: Key
        {
            return KeyTypeInfo<Key, T>.Info;
        }

        public static KeyTypeInfo<Key> Find(System.Type traitType)
        {
            if (!typeof(Key).IsAssignableFrom(traitType))
            {
                throw new ArgumentOutOfRangeException("traitType", "Must be a type assignable to Key");
            }
            if (traitType == typeof(Key))
            {
                throw new KeyArgumentIsKeyTypeException("You cannot use GetTrait(typeof(Key). Must use a types inheriting Key");
            }
            return Registration<Key>.GetUnsafe(traitType);
        }

        public static bool Find<T>(out KeyTypeInfo<Key> info) where T: Key
        {
            try
            {
                info = KeyTypeInfo<Key, T>.Info;
                return true;
            }
            catch (KeyArgumentIsKeyTypeException)
            {
                info = null;
                return false;
            }
        }

        public static bool Find(System.Type traitType, out KeyTypeInfo<Key> info)
        {
            if (typeof(Key).IsAssignableFrom(traitType) && (traitType != typeof(Key)))
            {
                info = null;
                return false;
            }
            info = Registration<Key>.GetUnsafe(traitType);
            return true;
        }

        public bool IsAssignableFrom(KeyTypeInfo<Key> info)
        {
            return (((info.Root == this.Root) && (info.TraitDepth >= this.TraitDepth)) && info.AssignableTo.Contains((KeyTypeInfo<Key>) this));
        }

        public bool IsBaseTrait
        {
            get
            {
                return (this.TraitDepth == 0);
            }
        }

        public static class Comparison
        {
            public static IComparer<KeyTypeInfo<Key>> Comparer
            {
                get
                {
                    return HierarchyComparer<Key>.Singleton.Instance;
                }
            }

            public static IEqualityComparer<KeyTypeInfo<Key>> EqualityComparer
            {
                get
                {
                    return RootEqualityComparer<Key>.Singleton.Instance;
                }
            }

            internal class HierarchyComparer : Comparer<KeyTypeInfo<Key>>
            {
                private static int BaseCompare(KeyTypeInfo<Key> x, KeyTypeInfo<Key> y)
                {
                    if ((x.TraitDepth == 0) || (x == y))
                    {
                        return 0;
                    }
                    int num = KeyTypeInfo<Key>.Comparison.HierarchyComparer.BaseCompare(x.Base, y.Base);
                    if (num == 0)
                    {
                        num = KeyTypeInfo.ForcedDifCompareValue(x.Type, y.Type);
                    }
                    return num;
                }

                public override int Compare(KeyTypeInfo<Key> x, KeyTypeInfo<Key> y)
                {
                    return -this.CompareForward(x, y);
                }

                private int CompareForward(KeyTypeInfo<Key> x, KeyTypeInfo<Key> y)
                {
                    if (x.Root != y.Root)
                    {
                        return KeyTypeInfo.ForcedDifCompareValue(x.Root.Type, y.Root.Type);
                    }
                    if (x.TraitDepth == y.TraitDepth)
                    {
                        return KeyTypeInfo<Key>.Comparison.HierarchyComparer.BaseCompare(x, y);
                    }
                    return x.TraitDepth.CompareTo(y.TraitDepth);
                }

                public static class Singleton
                {
                    public static readonly IComparer<KeyTypeInfo<Key>> Instance;

                    static Singleton()
                    {
                        KeyTypeInfo<Key>.Comparison.HierarchyComparer.Singleton.Instance = new KeyTypeInfo<Key>.Comparison.HierarchyComparer();
                    }
                }
            }

            private class RootEqualityComparer : EqualityComparer<KeyTypeInfo<Key>>
            {
                private RootEqualityComparer()
                {
                }

                public override bool Equals(KeyTypeInfo<Key> x, KeyTypeInfo<Key> y)
                {
                    return (x.Root == y.Root);
                }

                public override int GetHashCode(KeyTypeInfo<Key> obj)
                {
                    return obj.Root.Type.GetHashCode();
                }

                public static class Singleton
                {
                    public static readonly IEqualityComparer<KeyTypeInfo<Key>> Instance;

                    static Singleton()
                    {
                        KeyTypeInfo<Key>.Comparison.RootEqualityComparer.Singleton.Instance = new KeyTypeInfo<Key>.Comparison.RootEqualityComparer();
                    }
                }
            }
        }

        internal static class Registration
        {
            private static readonly Dictionary<Type, KeyTypeInfo<Key>> dict;

            static Registration()
            {
                KeyTypeInfo<Key>.Registration.dict = new Dictionary<Type, KeyTypeInfo<Key>>();
            }

            public static void Add(KeyTypeInfo<Key> info)
            {
                KeyTypeInfo<Key>.Registration.dict.Add(info.Type, info);
            }

            public static KeyTypeInfo<Key> GetUnsafe(Type type)
            {
                KeyTypeInfo<Key> info;
                if (KeyTypeInfo<Key>.Registration.dict.TryGetValue(type, out info))
                {
                    return info;
                }
                Type baseType = type.BaseType;
                if (typeof(Key) == baseType)
                {
                    return new KeyTypeInfo<Key>(type, null, null, 0);
                }
                KeyTypeInfo<Key> @unsafe = KeyTypeInfo<Key>.Registration.GetUnsafe(baseType);
                return new KeyTypeInfo<Key>(type, @unsafe, @unsafe.Root, @unsafe.TraitDepth + 1);
            }
        }

        internal class TraitDictionary
        {
            [NonSerialized]
            private readonly Dictionary<KeyTypeInfo<Key>, Key> rootToKey;

            public TraitDictionary(Key[] traitKeys)
            {
                if ((traitKeys == null) || (traitKeys.Length == 0))
                {
                    this.rootToKey = new Dictionary<KeyTypeInfo<Key>, Key>(0);
                }
                else
                {
                    this.rootToKey = new Dictionary<KeyTypeInfo<Key>, Key>(traitKeys.Length, KeyTypeInfo<Key>.Comparison.EqualityComparer);
                    foreach (Key local in traitKeys)
                    {
                        if (local != null)
                        {
                            this.rootToKey.Add(KeyTypeInfo<Key>.Find(local.GetType()), local);
                        }
                    }
                }
            }

            public Key Get<T>() where T: Key
            {
                return this.Get(KeyTypeInfo<Key, T>.Info);
            }

            private Key Get(KeyTypeInfo<Key> info)
            {
                return this.rootToKey[info];
            }

            public Key Get(Type type)
            {
                return this.Get(KeyTypeInfo<Key>.Find(type));
            }

            public T GetHardCast<T>() where T: Key
            {
                return this.GetHardCast<T>(KeyTypeInfo<Key, T>.Info);
            }

            public T GetHardCast<T>(Type type) where T: Key
            {
                return this.GetHardCast<T>(KeyTypeInfo<Key>.Find(type));
            }

            private T GetHardCast<T>(KeyTypeInfo<Key> info) where T: Key
            {
                return this.Get(info);
            }

            public T GetSoftCast<T>() where T: Key
            {
                return this.GetSoftCast<T>(KeyTypeInfo<Key, T>.Info);
            }

            public T GetSoftCast<T>(Type type) where T: Key
            {
                return this.GetSoftCast<T>(KeyTypeInfo<Key>.Find(type));
            }

            private T GetSoftCast<T>(KeyTypeInfo<Key> info) where T: Key
            {
                return (this.Get(info) as T);
            }

            public void MergeUpon(KeyTypeInfo<Key>.TraitDictionary fillGaps)
            {
                foreach (KeyValuePair<KeyTypeInfo<Key>, Key> pair in this.rootToKey)
                {
                    if (!fillGaps.rootToKey.ContainsKey(pair.Key.Root))
                    {
                        fillGaps.rootToKey.Add(pair.Key, pair.Value);
                    }
                }
            }

            public Key TryGet<T>() where T: Key
            {
                Key local;
                this.TryGet<T>(out local);
                return local;
            }

            public Key TryGet(Type type)
            {
                Key local;
                this.TryGet(type, out local);
                return local;
            }

            public bool TryGet<T>(out Key key) where T: Key
            {
                return this.TryGet(KeyTypeInfo<Key, T>.Info, out key);
            }

            private bool TryGet(KeyTypeInfo<Key> info, out Key key)
            {
                return this.rootToKey.TryGetValue(info, out key);
            }

            public bool TryGet(Type traitType, out Key key)
            {
                return this.TryGet(KeyTypeInfo<Key>.Find(traitType), out key);
            }

            public T TryGetHardCast<T>() where T: Key
            {
                T local;
                this.TryGetHardCast<T>(out local);
                return local;
            }

            public bool TryGetHardCast<T>(out T key) where T: Key
            {
                return this.TryGetHardCast<T>(KeyTypeInfo<Key, T>.Info, out key);
            }

            public T TryGetHardCast<T>(Type type) where T: Key
            {
                T local;
                this.TryGetHardCast<T>(type, out local);
                return local;
            }

            public bool TryGetHardCast<T>(Type traitType, out T key) where T: Key
            {
                return this.TryGetHardCast<T>(KeyTypeInfo<Key>.Find(traitType), out key);
            }

            private bool TryGetHardCast<T>(KeyTypeInfo<Key> info, out T tkey) where T: Key
            {
                Key local;
                if (this.TryGet(info, out local))
                {
                    tkey = local;
                    return true;
                }
                tkey = null;
                return false;
            }

            public T TryGetSoftCast<T>() where T: Key
            {
                T local;
                this.TryGetSoftCast<T>(out local);
                return local;
            }

            public bool TryGetSoftCast<T>(out T key) where T: Key
            {
                return this.TryGetSoftCast<T>(KeyTypeInfo<Key, T>.Info, out key);
            }

            public T TryGetSoftCast<T>(Type type) where T: Key
            {
                T local;
                this.TryGetSoftCast<T>(type, out local);
                return local;
            }

            private bool TryGetSoftCast<T>(KeyTypeInfo<Key> info, out T tkey) where T: Key
            {
                Key local;
                if (this.TryGet(info, out local))
                {
                    tkey = local as T;
                    return true;
                }
                tkey = null;
                return false;
            }

            public bool TryGetSoftCast<T>(Type traitType, out T key) where T: Key
            {
                return this.TryGetSoftCast<T>(KeyTypeInfo<Key>.Find(traitType), out key);
            }
        }
    }
}

