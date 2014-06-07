namespace Facepunch
{
    using Facepunch.Load;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using UnityEngine;

    public static class Bundling
    {
        private static bool HasLoadedBundleMap;
        private const bool kBundleUnloadClearsEverything = true;
        private const string kUnloadedBundlesMessage = "Bundles were not loaded";
        private static LoadedBundleMap Map;
        private static OnLoadedEventHandler nextLoadEvents;

        public static  event OnLoadedEventHandler OnceLoaded
        {
            add
            {
                if (Loaded)
                {
                    value();
                }
                else
                {
                    nextLoadEvents = (OnLoadedEventHandler) Delegate.Combine(nextLoadEvents, value);
                }
            }
            remove
            {
                nextLoadEvents = (OnLoadedEventHandler) Delegate.Remove(nextLoadEvents, value);
            }
        }

        public static void BindToLoader(Loader loader)
        {
            BundleBridger bridger = new BundleBridger();
            loader.OnGroupedAssetBundlesLoaded += new MultipleAssetBundlesLoadedEventHandler(bridger.AddArrays);
            loader.OnAllAssetBundlesLoaded += new MultipleAssetBundlesLoadedEventHandler(bridger.FinalizeAndInstall);
        }

        public static T Load<T>(string path) where T: Object
        {
            T local;
            Load<T>(path, out local);
            return local;
        }

        public static bool Load<T>(string path, out T asset) where T: Object
        {
            Object obj2;
            if (Load(path, typeof(T), out obj2))
            {
                asset = (T) obj2;
                return true;
            }
            asset = null;
            return false;
        }

        public static Object Load(string path, Type type)
        {
            Object obj2;
            Load(path, type, out obj2);
            return obj2;
        }

        public static T Load<T>(string path, Type type) where T: Object
        {
            T local;
            Load<T>(path, type, out local);
            return local;
        }

        public static bool Load(string path, Type type, out Object asset)
        {
            if (!HasLoadedBundleMap)
            {
                throw new InvalidOperationException("Bundles were not loaded");
            }
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            if (path.Length == 0)
            {
                asset = null;
                return false;
            }
            return Map.Assets.Load(path, type, out asset);
        }

        public static bool Load<T>(string path, Type type, out T asset) where T: Object
        {
            Object obj2;
            if (!typeof(T).IsAssignableFrom(type))
            {
                throw new ArgumentException(string.Format("The given type ({1}) cannot cast to {0}", typeof(T), type), "type");
            }
            if (Load(path, type, out obj2))
            {
                asset = (T) obj2;
                return true;
            }
            asset = null;
            return false;
        }

        public static Object[] LoadAll()
        {
            if (!HasLoadedBundleMap)
            {
                throw new InvalidOperationException("Bundles were not loaded");
            }
            return new List<Object>(Map.Assets.LoadAll()).ToArray();
        }

        public static T[] LoadAll<T>() where T: Object
        {
            if (!HasLoadedBundleMap)
            {
                throw new InvalidOperationException("Bundles were not loaded");
            }
            List<T> list = new List<T>();
            IEnumerator<Object> enumerator = Map.Assets.LoadAll(typeof(T)).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Object current = enumerator.Current;
                    list.Add((T) current);
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
            return list.ToArray();
        }

        public static Object[] LoadAll(Type type)
        {
            if (type == typeof(Object))
            {
                return LoadAll();
            }
            if (!HasLoadedBundleMap)
            {
                throw new InvalidOperationException("Bundles were not loaded");
            }
            return new List<Object>(Map.Assets.LoadAll(type)).ToArray();
        }

        public static T[] LoadAll<T>(Type type) where T: Object
        {
            if (!typeof(T).IsAssignableFrom(type))
            {
                throw new ArgumentException(string.Format("The given type ({1}) cannot cast to {0}", typeof(T), type), "type");
            }
            if (!HasLoadedBundleMap)
            {
                throw new InvalidOperationException("Bundles were not loaded");
            }
            List<T> list = new List<T>();
            IEnumerator<Object> enumerator = Map.Assets.LoadAll(type).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Object current = enumerator.Current;
                    list.Add((T) current);
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
            return list.ToArray();
        }

        [Obsolete("This only works outside of editor for now, avoid it")]
        public static AssetBundleRequest LoadAsync<T>(string path)
        {
            return LoadAsync(path, typeof(T));
        }

        [Obsolete("This only works outside of editor for now, avoid it")]
        public static bool LoadAsync<T>(string path, out AssetBundleRequest request) where T: Object
        {
            return LoadAsync(path, typeof(T), out request);
        }

        [Obsolete("This only works outside of editor for now, avoid it")]
        public static AssetBundleRequest LoadAsync(string path, Type type)
        {
            AssetBundleRequest request;
            LoadAsync(path, type, out request);
            return request;
        }

        [Obsolete("This only works outside of editor for now, avoid it")]
        public static bool LoadAsync(string path, Type type, out AssetBundleRequest request)
        {
            if (!HasLoadedBundleMap)
            {
                throw new InvalidOperationException("Bundles were not loaded");
            }
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            if (path.Length == 0)
            {
                request = null;
                return false;
            }
            return Map.Assets.LoadAsync(path, type, out request);
        }

        public static void Unload()
        {
            if (HasLoadedBundleMap)
            {
                Map.Dispose();
            }
        }

        public static bool Loaded
        {
            get
            {
                return HasLoadedBundleMap;
            }
        }

        private class BundleBridger
        {
            private readonly Dictionary<Type, List<Bundling.LoadedBundle>> assetsMap = new Dictionary<Type, List<Bundling.LoadedBundle>>();
            private Type lastAssetMapSearchKey;
            private List<Bundling.LoadedBundle> lastAssetMapSearchValue;
            private readonly List<Bundling.LoadedBundle> scenes = new List<Bundling.LoadedBundle>();

            public void Add(AssetBundle bundle, Item item)
            {
                if (item.ContentType == ContentType.Assets)
                {
                    this.AssetListOfType(item.TypeOfAssets).Add(new Bundling.LoadedBundle(bundle, item));
                }
                else
                {
                    this.scenes.Add(new Bundling.LoadedBundle(bundle, item));
                }
            }

            public void AddArrays(AssetBundle[] bundles, Item[] items)
            {
                for (int i = 0; i < bundles.Length; i++)
                {
                    this.Add(bundles[i], items[i]);
                }
            }

            private List<Bundling.LoadedBundle> AssetListOfType(Type type)
            {
                if ((type != this.lastAssetMapSearchKey) && !this.assetsMap.TryGetValue(this.lastAssetMapSearchKey = type, out this.lastAssetMapSearchValue))
                {
                    this.assetsMap[this.lastAssetMapSearchKey] = this.lastAssetMapSearchValue = new List<Bundling.LoadedBundle>();
                }
                return this.lastAssetMapSearchValue;
            }

            public void FinalizeAndInstall(AssetBundle[] bundles, Item[] items)
            {
                Bundling.LoadedBundleMap map = new Bundling.LoadedBundleMap(this.assetsMap, this.scenes);
                if ((map == Bundling.Map) && (Bundling.nextLoadEvents != null))
                {
                    Bundling.OnLoadedEventHandler nextLoadEvents = Bundling.nextLoadEvents;
                    Bundling.nextLoadEvents = null;
                    try
                    {
                        nextLoadEvents();
                    }
                    catch (Exception exception)
                    {
                        Debug.LogException(exception);
                    }
                }
            }

            private bool Remove(Type type)
            {
                if (!this.assetsMap.Remove(type))
                {
                    return false;
                }
                if (type == this.lastAssetMapSearchKey)
                {
                    this.lastAssetMapSearchValue = null;
                    this.lastAssetMapSearchKey = null;
                }
                return true;
            }
        }

        private class LoadedBundle
        {
            private AssetBundle Bundle;
            public readonly Facepunch.Load.Item Item;

            public LoadedBundle(AssetBundle bundle, Facepunch.Load.Item item)
            {
                this.Bundle = bundle;
                this.Item = item;
            }

            public bool Contains(string path)
            {
                return this.Bundle.Contains(path);
            }

            public Object Load(string path)
            {
                return this.Bundle.Load(path);
            }

            public Object Load(string path, Type type)
            {
                return this.Bundle.Load(path, type);
            }

            public Object[] LoadAll()
            {
                return this.Bundle.LoadAll();
            }

            public Object[] LoadAll(Type type)
            {
                return this.Bundle.LoadAll(type);
            }

            public AssetBundleRequest LoadAsync(string path, Type type)
            {
                return this.Bundle.LoadAsync(path, type);
            }

            internal void Unload()
            {
                if (this.Bundle != null)
                {
                    this.Bundle.Unload(true);
                }
                this.Bundle = null;
            }
        }

        private class LoadedBundleAssetMap
        {
            [CompilerGenerated]
            private static Comparison<KeyValuePair<Type, List<Bundling.LoadedBundle>>> <>f__am$cache3;
            public readonly Bundling.LoadedBundleListOfAssets[] AllLoadedBundleAssetLists;
            private readonly short[] tempBuffer;
            private readonly Dictionary<Type, short[]> typeMap = new Dictionary<Type, short[]>();

            internal LoadedBundleAssetMap(IEnumerable<KeyValuePair<Type, List<Bundling.LoadedBundle>>> assets)
            {
                List<KeyValuePair<Type, List<Bundling.LoadedBundle>>> list = new List<KeyValuePair<Type, List<Bundling.LoadedBundle>>>(assets);
                if (<>f__am$cache3 == null)
                {
                    <>f__am$cache3 = new Comparison<KeyValuePair<Type, List<Bundling.LoadedBundle>>>(Bundling.LoadedBundleAssetMap.<LoadedBundleAssetMap>m__2);
                }
                list.Sort(<>f__am$cache3);
                this.AllLoadedBundleAssetLists = new Bundling.LoadedBundleListOfAssets[list.Count];
                for (int i = 0; i < this.AllLoadedBundleAssetLists.Length; i++)
                {
                    KeyValuePair<Type, List<Bundling.LoadedBundle>> pair = list[i];
                    this.AllLoadedBundleAssetLists[i] = new Bundling.LoadedBundleListOfAssets(pair.Key, pair.Value);
                }
                this.tempBuffer = new short[this.AllLoadedBundleAssetLists.Length];
            }

            [CompilerGenerated]
            private static int <LoadedBundleAssetMap>m__2(KeyValuePair<Type, List<Bundling.LoadedBundle>> x, KeyValuePair<Type, List<Bundling.LoadedBundle>> y)
            {
                int num = !typeof(GameObject).IsAssignableFrom(x.Key) ? (!typeof(ScriptableObject).IsAssignableFrom(x.Key) ? 2 : 1) : 0;
                int num2 = !typeof(GameObject).IsAssignableFrom(y.Key) ? (!typeof(ScriptableObject).IsAssignableFrom(y.Key) ? 2 : 1) : 0;
                return num.CompareTo(num2);
            }

            public bool Load(string path, Type type, out Object asset)
            {
                short[] numArray;
                if (!this.TypeIndices(type, out numArray))
                {
                    Debug.Log("no type index for " + type);
                    asset = null;
                    return false;
                }
                int index = 0;
                while (numArray[index] >= 0)
                {
                    if (this.AllLoadedBundleAssetLists[numArray[index]].Load(path, out asset))
                    {
                        return true;
                    }
                    if (++index >= numArray.Length)
                    {
                        asset = null;
                        return false;
                    }
                }
                while (index < numArray.Length)
                {
                    Bundling.LoadedBundleListOfAssets assets = this.AllLoadedBundleAssetLists[-(numArray[index] + 1)];
                    if (assets.Load(path, type, out asset))
                    {
                        return true;
                    }
                    index++;
                }
                asset = null;
                return false;
            }

            [DebuggerHidden]
            public IEnumerable<Object> LoadAll()
            {
                return new <LoadAll>c__Iterator15 { <>f__this = this, $PC = -2 };
            }

            [DebuggerHidden]
            public IEnumerable<Object> LoadAll(Type type)
            {
                return new <LoadAll>c__Iterator16 { type = type, <$>type = type, <>f__this = this, $PC = -2 };
            }

            public bool LoadAsync(string path, Type type, out AssetBundleRequest request)
            {
                short[] numArray;
                if (!this.TypeIndices(type, out numArray))
                {
                    request = null;
                    return false;
                }
                int index = 0;
                while (numArray[index] >= 0)
                {
                    if (this.AllLoadedBundleAssetLists[numArray[index]].LoadAsync(path, out request))
                    {
                        return true;
                    }
                    if (++index >= numArray.Length)
                    {
                        request = null;
                        return false;
                    }
                }
                while (index < numArray.Length)
                {
                    Bundling.LoadedBundleListOfAssets assets = this.AllLoadedBundleAssetLists[-(numArray[index] + 1)];
                    if (assets.LoadAsync(path, type, out request))
                    {
                        return true;
                    }
                    index++;
                }
                request = null;
                return false;
            }

            private bool TypeIndices(Type key, out short[] value)
            {
                if (key == null)
                {
                    throw new ArgumentNullException("type");
                }
                if (this.typeMap.TryGetValue(key, out value))
                {
                    return (value != null);
                }
                if (!typeof(Object).IsAssignableFrom(key))
                {
                    throw new ArgumentOutOfRangeException("type", string.Format("type {0} is not assignable to UnityEngine.Object", key));
                }
                if (typeof(Component).IsAssignableFrom(key))
                {
                    if (typeof(Component) == key)
                    {
                        bool flag = this.TypeIndices(typeof(GameObject), out value);
                        value = (short[]) value.Clone();
                        for (int k = 0; k < value.Length; k++)
                        {
                            if (value[k] >= 0)
                            {
                                value[k] = (short) -(value[k] + 1);
                            }
                        }
                        this.typeMap[key] = value;
                        return flag;
                    }
                    bool flag2 = this.TypeIndices(typeof(Component), out value);
                    this.typeMap[key] = value;
                    return flag2;
                }
                int index = 0;
                for (int i = 0; i < this.AllLoadedBundleAssetLists.Length; i++)
                {
                    if (key.IsAssignableFrom(this.AllLoadedBundleAssetLists[i].TypeOfAssets))
                    {
                        this.tempBuffer[index++] = (short) i;
                    }
                }
                int num4 = 0;
                int num5 = index;
                for (int j = 0; j < this.AllLoadedBundleAssetLists.Length; j++)
                {
                    if ((num4 < num5) && (j == this.tempBuffer[num4]))
                    {
                        num4++;
                    }
                    else if (this.AllLoadedBundleAssetLists[j].TypeOfAssets.IsAssignableFrom(key))
                    {
                        this.tempBuffer[index++] = (short) -(j + 1);
                    }
                }
                if (index == 0)
                {
                    this.typeMap[key] = (short[]) (value = null);
                    return false;
                }
                value = new short[index];
                while (--index >= 0)
                {
                    value[index] = this.tempBuffer[index];
                }
                this.typeMap[key] = value;
                return true;
            }

            internal void Unload()
            {
                foreach (Bundling.LoadedBundleListOfAssets assets in this.AllLoadedBundleAssetLists)
                {
                    assets.Unload();
                }
            }

            [CompilerGenerated]
            private sealed class <LoadAll>c__Iterator15 : IDisposable, IEnumerator, IEnumerable, IEnumerable<Object>, IEnumerator<Object>
            {
                internal Object $current;
                internal int $PC;
                internal Bundling.LoadedBundleListOfAssets[] <$s_156>__0;
                internal int <$s_157>__1;
                internal Bundling.LoadedBundle[] <$s_158>__3;
                internal int <$s_159>__4;
                internal Object[] <$s_160>__6;
                internal int <$s_161>__7;
                internal Bundling.LoadedBundleAssetMap <>f__this;
                internal Object <asset>__8;
                internal Bundling.LoadedBundle <bundle>__5;
                internal Bundling.LoadedBundleListOfAssets <listOfBundles>__2;

                [DebuggerHidden]
                public void Dispose()
                {
                    this.$PC = -1;
                }

                public bool MoveNext()
                {
                    uint num = (uint) this.$PC;
                    this.$PC = -1;
                    switch (num)
                    {
                        case 0:
                            this.<$s_156>__0 = this.<>f__this.AllLoadedBundleAssetLists;
                            this.<$s_157>__1 = 0;
                            while (this.<$s_157>__1 < this.<$s_156>__0.Length)
                            {
                                this.<listOfBundles>__2 = this.<$s_156>__0[this.<$s_157>__1];
                                this.<$s_158>__3 = this.<listOfBundles>__2.Bundles;
                                this.<$s_159>__4 = 0;
                                while (this.<$s_159>__4 < this.<$s_158>__3.Length)
                                {
                                    this.<bundle>__5 = this.<$s_158>__3[this.<$s_159>__4];
                                    this.<$s_160>__6 = this.<bundle>__5.LoadAll();
                                    this.<$s_161>__7 = 0;
                                    while (this.<$s_161>__7 < this.<$s_160>__6.Length)
                                    {
                                        this.<asset>__8 = this.<$s_160>__6[this.<$s_161>__7];
                                        this.$current = this.<asset>__8;
                                        this.$PC = 1;
                                        return true;
                                    Label_00C9:
                                        this.<$s_161>__7++;
                                    }
                                    this.<$s_159>__4++;
                                }
                                this.<$s_157>__1++;
                            }
                            this.$PC = -1;
                            break;

                        case 1:
                            goto Label_00C9;
                    }
                    return false;
                }

                [DebuggerHidden]
                public void Reset()
                {
                    throw new NotSupportedException();
                }

                [DebuggerHidden]
                IEnumerator<Object> IEnumerable<Object>.GetEnumerator()
                {
                    if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                    {
                        return this;
                    }
                    return new Bundling.LoadedBundleAssetMap.<LoadAll>c__Iterator15 { <>f__this = this.<>f__this };
                }

                [DebuggerHidden]
                IEnumerator IEnumerable.GetEnumerator()
                {
                    return this.System.Collections.Generic.IEnumerable<UnityEngine.Object>.GetEnumerator();
                }

                Object IEnumerator<Object>.Current
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

            [CompilerGenerated]
            private sealed class <LoadAll>c__Iterator16 : IDisposable, IEnumerator, IEnumerable, IEnumerable<Object>, IEnumerator<Object>
            {
                internal Object $current;
                internal int $PC;
                internal Type <$>type;
                internal Bundling.LoadedBundle[] <$s_162>__2;
                internal int <$s_163>__3;
                internal Object[] <$s_164>__5;
                internal int <$s_165>__6;
                internal Bundling.LoadedBundle[] <$s_166>__9;
                internal int <$s_167>__10;
                internal Object[] <$s_168>__12;
                internal int <$s_169>__13;
                internal Bundling.LoadedBundleAssetMap <>f__this;
                internal Object <asset>__14;
                internal Object <asset>__7;
                internal Bundling.LoadedBundle <bundle>__11;
                internal Bundling.LoadedBundle <bundle>__4;
                internal int <i>__1;
                internal short[] <indices>__0;
                internal Bundling.LoadedBundleListOfAssets <test>__8;
                internal Type type;

                [DebuggerHidden]
                public void Dispose()
                {
                    this.$PC = -1;
                }

                public bool MoveNext()
                {
                    uint num = (uint) this.$PC;
                    this.$PC = -1;
                    switch (num)
                    {
                        case 0:
                            if (this.<>f__this.TypeIndices(this.type, out this.<indices>__0))
                            {
                                this.<i>__1 = 0;
                                while (this.<indices>__0[this.<i>__1] >= 0)
                                {
                                    this.<$s_162>__2 = this.<>f__this.AllLoadedBundleAssetLists[this.<indices>__0[this.<i>__1]].Bundles;
                                    this.<$s_163>__3 = 0;
                                    while (this.<$s_163>__3 < this.<$s_162>__2.Length)
                                    {
                                        this.<bundle>__4 = this.<$s_162>__2[this.<$s_163>__3];
                                        this.<$s_164>__5 = this.<bundle>__4.LoadAll();
                                        this.<$s_165>__6 = 0;
                                        while (this.<$s_165>__6 < this.<$s_164>__5.Length)
                                        {
                                            this.<asset>__7 = this.<$s_164>__5[this.<$s_165>__6];
                                            this.$current = this.<asset>__7;
                                            this.$PC = 1;
                                            goto Label_027B;
                                        Label_00DD:
                                            this.<$s_165>__6++;
                                        }
                                        this.<$s_163>__3++;
                                    }
                                    if (++this.<i>__1 >= this.<indices>__0.Length)
                                    {
                                        goto Label_0279;
                                    }
                                }
                                while (this.<i>__1 < this.<indices>__0.Length)
                                {
                                    this.<test>__8 = this.<>f__this.AllLoadedBundleAssetLists[-(this.<indices>__0[this.<i>__1] + 1)];
                                    this.<$s_166>__9 = this.<test>__8.Bundles;
                                    this.<$s_167>__10 = 0;
                                    while (this.<$s_167>__10 < this.<$s_166>__9.Length)
                                    {
                                        this.<bundle>__11 = this.<$s_166>__9[this.<$s_167>__10];
                                        this.<$s_168>__12 = this.<bundle>__11.LoadAll(this.type);
                                        this.<$s_169>__13 = 0;
                                        while (this.<$s_169>__13 < this.<$s_168>__12.Length)
                                        {
                                            this.<asset>__14 = this.<$s_168>__12[this.<$s_169>__13];
                                            this.$current = this.<asset>__14;
                                            this.$PC = 2;
                                            goto Label_027B;
                                        Label_01FA:
                                            this.<$s_169>__13++;
                                        }
                                        this.<$s_167>__10++;
                                    }
                                    if (++this.<i>__1 >= this.<indices>__0.Length)
                                    {
                                        break;
                                    }
                                }
                                break;
                            }
                            goto Label_0279;

                        case 1:
                            goto Label_00DD;

                        case 2:
                            goto Label_01FA;

                        default:
                            goto Label_0279;
                    }
                    this.$PC = -1;
                Label_0279:
                    return false;
                Label_027B:
                    return true;
                }

                [DebuggerHidden]
                public void Reset()
                {
                    throw new NotSupportedException();
                }

                [DebuggerHidden]
                IEnumerator<Object> IEnumerable<Object>.GetEnumerator()
                {
                    if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                    {
                        return this;
                    }
                    return new Bundling.LoadedBundleAssetMap.<LoadAll>c__Iterator16 { <>f__this = this.<>f__this, type = this.<$>type };
                }

                [DebuggerHidden]
                IEnumerator IEnumerable.GetEnumerator()
                {
                    return this.System.Collections.Generic.IEnumerable<UnityEngine.Object>.GetEnumerator();
                }

                Object IEnumerator<Object>.Current
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

        private class LoadedBundleListOfAssets
        {
            public readonly Bundling.LoadedBundle[] Bundles;
            private readonly Dictionary<string, short> pathsToFoundBundles;
            public readonly Type TypeOfAssets;

            public LoadedBundleListOfAssets(Type typeOfAssets, List<Bundling.LoadedBundle> bundles)
            {
                this.TypeOfAssets = typeOfAssets;
                this.Bundles = bundles.ToArray();
                this.pathsToFoundBundles = new Dictionary<string, short>(StringComparer.InvariantCultureIgnoreCase);
            }

            public bool Load(string path, out Object asset)
            {
                short num;
                if (this.PathIndex(path, out num))
                {
                    Object obj2;
                    asset = obj2 = this.Bundles[num].Load(path);
                    return (bool) obj2;
                }
                asset = null;
                return false;
            }

            public bool Load(string path, Type type, out Object asset)
            {
                short num;
                if (this.PathIndex(path, out num))
                {
                    Object obj2;
                    asset = obj2 = this.Bundles[num].Load(path, type);
                    return (bool) obj2;
                }
                asset = null;
                return false;
            }

            [DebuggerHidden]
            public IEnumerable<Object> LoadAll()
            {
                return new <LoadAll>c__Iterator17 { <>f__this = this, $PC = -2 };
            }

            [DebuggerHidden]
            public IEnumerable<Object> LoadAll(Type type)
            {
                return new <LoadAll>c__Iterator18 { type = type, <$>type = type, <>f__this = this, $PC = -2 };
            }

            public bool LoadAsync(string path, out AssetBundleRequest request)
            {
                return this.LoadAsync(path, this.TypeOfAssets, out request);
            }

            public bool LoadAsync(string path, Type type, out AssetBundleRequest request)
            {
                short num;
                if (this.PathIndex(path, out num))
                {
                    AssetBundleRequest request2;
                    request = request2 = this.Bundles[num].LoadAsync(path, type);
                    return (request2 != null);
                }
                request = null;
                return false;
            }

            private bool PathIndex(string path, out short index)
            {
                if (!this.pathsToFoundBundles.TryGetValue(path, out index))
                {
                    for (int i = 0; i < this.Bundles.Length; i++)
                    {
                        if (this.Bundles[i].Contains(path))
                        {
                            this.pathsToFoundBundles[path] = index = (short) i;
                            return true;
                        }
                    }
                    this.pathsToFoundBundles[path] = -1;
                    return false;
                }
                if (index == -1)
                {
                    return false;
                }
                return true;
            }

            internal void Unload()
            {
                foreach (Bundling.LoadedBundle bundle in this.Bundles)
                {
                    bundle.Unload();
                }
            }

            [CompilerGenerated]
            private sealed class <LoadAll>c__Iterator17 : IDisposable, IEnumerator, IEnumerable, IEnumerable<Object>, IEnumerator<Object>
            {
                internal Object $current;
                internal int $PC;
                internal Bundling.LoadedBundle[] <$s_170>__0;
                internal int <$s_171>__1;
                internal Object[] <$s_172>__3;
                internal int <$s_173>__4;
                internal Bundling.LoadedBundleListOfAssets <>f__this;
                internal Object <asset>__5;
                internal Bundling.LoadedBundle <bundle>__2;

                [DebuggerHidden]
                public void Dispose()
                {
                    this.$PC = -1;
                }

                public bool MoveNext()
                {
                    uint num = (uint) this.$PC;
                    this.$PC = -1;
                    switch (num)
                    {
                        case 0:
                            this.<$s_170>__0 = this.<>f__this.Bundles;
                            this.<$s_171>__1 = 0;
                            while (this.<$s_171>__1 < this.<$s_170>__0.Length)
                            {
                                this.<bundle>__2 = this.<$s_170>__0[this.<$s_171>__1];
                                this.<$s_172>__3 = this.<bundle>__2.LoadAll();
                                this.<$s_173>__4 = 0;
                                while (this.<$s_173>__4 < this.<$s_172>__3.Length)
                                {
                                    this.<asset>__5 = this.<$s_172>__3[this.<$s_173>__4];
                                    this.$current = this.<asset>__5;
                                    this.$PC = 1;
                                    return true;
                                Label_0099:
                                    this.<$s_173>__4++;
                                }
                                this.<$s_171>__1++;
                            }
                            this.$PC = -1;
                            break;

                        case 1:
                            goto Label_0099;
                    }
                    return false;
                }

                [DebuggerHidden]
                public void Reset()
                {
                    throw new NotSupportedException();
                }

                [DebuggerHidden]
                IEnumerator<Object> IEnumerable<Object>.GetEnumerator()
                {
                    if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                    {
                        return this;
                    }
                    return new Bundling.LoadedBundleListOfAssets.<LoadAll>c__Iterator17 { <>f__this = this.<>f__this };
                }

                [DebuggerHidden]
                IEnumerator IEnumerable.GetEnumerator()
                {
                    return this.System.Collections.Generic.IEnumerable<UnityEngine.Object>.GetEnumerator();
                }

                Object IEnumerator<Object>.Current
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

            [CompilerGenerated]
            private sealed class <LoadAll>c__Iterator18 : IDisposable, IEnumerator, IEnumerable, IEnumerable<Object>, IEnumerator<Object>
            {
                internal Object $current;
                internal int $PC;
                internal Type <$>type;
                internal Bundling.LoadedBundle[] <$s_174>__0;
                internal int <$s_175>__1;
                internal Object[] <$s_176>__3;
                internal int <$s_177>__4;
                internal Bundling.LoadedBundleListOfAssets <>f__this;
                internal Object <asset>__5;
                internal Bundling.LoadedBundle <bundle>__2;
                internal Type type;

                [DebuggerHidden]
                public void Dispose()
                {
                    this.$PC = -1;
                }

                public bool MoveNext()
                {
                    uint num = (uint) this.$PC;
                    this.$PC = -1;
                    switch (num)
                    {
                        case 0:
                            this.<$s_174>__0 = this.<>f__this.Bundles;
                            this.<$s_175>__1 = 0;
                            while (this.<$s_175>__1 < this.<$s_174>__0.Length)
                            {
                                this.<bundle>__2 = this.<$s_174>__0[this.<$s_175>__1];
                                this.<$s_176>__3 = this.<bundle>__2.LoadAll(this.type);
                                this.<$s_177>__4 = 0;
                                while (this.<$s_177>__4 < this.<$s_176>__3.Length)
                                {
                                    this.<asset>__5 = this.<$s_176>__3[this.<$s_177>__4];
                                    this.$current = this.<asset>__5;
                                    this.$PC = 1;
                                    return true;
                                Label_009F:
                                    this.<$s_177>__4++;
                                }
                                this.<$s_175>__1++;
                            }
                            this.$PC = -1;
                            break;

                        case 1:
                            goto Label_009F;
                    }
                    return false;
                }

                [DebuggerHidden]
                public void Reset()
                {
                    throw new NotSupportedException();
                }

                [DebuggerHidden]
                IEnumerator<Object> IEnumerable<Object>.GetEnumerator()
                {
                    if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                    {
                        return this;
                    }
                    return new Bundling.LoadedBundleListOfAssets.<LoadAll>c__Iterator18 { <>f__this = this.<>f__this, type = this.<$>type };
                }

                [DebuggerHidden]
                IEnumerator IEnumerable.GetEnumerator()
                {
                    return this.System.Collections.Generic.IEnumerable<UnityEngine.Object>.GetEnumerator();
                }

                Object IEnumerator<Object>.Current
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

        private class LoadedBundleListOfScenes
        {
            public readonly Bundling.LoadedBundle[] Bundles;

            public LoadedBundleListOfScenes(IEnumerable<Bundling.LoadedBundle> bundles)
            {
                if (bundles is List<Bundling.LoadedBundle>)
                {
                    this.Bundles = ((List<Bundling.LoadedBundle>) bundles).ToArray();
                }
                else
                {
                    this.Bundles = new List<Bundling.LoadedBundle>(bundles).ToArray();
                }
            }

            internal void Unload()
            {
                foreach (Bundling.LoadedBundle bundle in this.Bundles)
                {
                    bundle.Unload();
                }
            }
        }

        private class LoadedBundleMap : IDisposable
        {
            public readonly Bundling.LoadedBundleAssetMap Assets;
            private bool disposed;
            public readonly Bundling.LoadedBundleListOfScenes Scenes;

            public LoadedBundleMap(IEnumerable<KeyValuePair<Type, List<Bundling.LoadedBundle>>> assets, IEnumerable<Bundling.LoadedBundle> scenes)
            {
                this.Assets = new Bundling.LoadedBundleAssetMap(assets);
                this.Scenes = new Bundling.LoadedBundleListOfScenes(scenes);
                Bundling.Map = this;
                Bundling.HasLoadedBundleMap = true;
            }

            public void Dispose()
            {
                if (!this.disposed)
                {
                    if (Bundling.Map == this)
                    {
                        Bundling.Map = null;
                        Bundling.HasLoadedBundleMap = false;
                    }
                    this.disposed = true;
                    this.Assets.Unload();
                    this.Scenes.Unload();
                }
            }
        }

        public delegate void OnLoadedEventHandler();
    }
}

