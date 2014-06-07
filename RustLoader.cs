using Facepunch;
using Facepunch.Load;
using Facepunch.Load.Downloaders;
using Facepunch.Traits;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

public class RustLoader : MonoBehaviour, IRustLoaderTasks
{
    public bool destroyGameObjectOnceLoaded;
    [NonSerialized]
    private Loader loader;
    [SerializeField]
    public GameObject[] messageReceivers;
    [NonSerialized]
    private bool preloadedJsonLoader;
    [NonSerialized]
    private string preloadedJsonLoaderError;
    [NonSerialized]
    private string preloadedJsonLoaderRoot;
    [NonSerialized]
    private string preloadedJsonLoaderText;
    [SerializeField]
    public string releaseBundleLoaderFilePath = "bundles/manifest.txt";

    public void AddMessageReceiver(GameObject newReceiver)
    {
        if (newReceiver != null)
        {
            if (this.messageReceivers == null)
            {
                this.messageReceivers = new GameObject[] { newReceiver };
            }
            else if (Array.IndexOf<GameObject>(this.messageReceivers, newReceiver) == -1)
            {
                Array.Resize<GameObject>(ref this.messageReceivers, this.messageReceivers.Length + 1);
                this.messageReceivers[this.messageReceivers.Length - 1] = newReceiver;
            }
        }
    }

    private void Callback_OnBundleAllLoaded(AssetBundle[] bundles, Item[] items)
    {
        this.DispatchLoadMessage("OnRustBundleCompleteLoaded", this);
    }

    private void Callback_OnBundleGroupLoaded(AssetBundle[] bundles, Item[] items)
    {
        this.DispatchLoadMessage("OnRustBundleGroupLoaded", this);
    }

    private void Callback_OnBundleLoaded(AssetBundle bundle, Item item)
    {
        this.DispatchLoadMessage("OnRustBundleLoaded", this);
    }

    private void DispatchLoadMessage(string message, object value)
    {
        if (this.messageReceivers != null)
        {
            foreach (GameObject obj2 in this.messageReceivers)
            {
                if (obj2 != null)
                {
                    obj2.SendMessage(message, value, SendMessageOptions.DontRequireReceiver);
                }
            }
        }
    }

    private static void OnResourcesLoaded()
    {
        foreach (BaseTraitMap map in Bundling.LoadAll<BaseTraitMap>())
        {
            if (map != null)
            {
                try
                {
                    Binder.BindMap(map);
                }
                catch (Exception exception)
                {
                    Debug.LogError(exception, map);
                }
            }
        }
        DatablockDictionary.Initialize();
        foreach (NetMainPrefab prefab in Bundling.LoadAll<NetMainPrefab>())
        {
            try
            {
                prefab.Register(true);
            }
            catch (Exception exception2)
            {
                Debug.LogException(exception2, prefab);
            }
        }
        foreach (uLinkNetworkView view in Bundling.LoadAll<uLinkNetworkView>())
        {
            try
            {
                NetCull.RegisterNetAutoPrefab(view);
            }
            catch (Exception exception3)
            {
                Debug.LogException(exception3, view);
            }
        }
        NGC.Register(NGCConfiguration.Load());
    }

    public void ServerInit()
    {
        Object.Destroy(base.GetComponent<RustLoaderInstantiateOnComplete>());
    }

    public void SetPreloadedManifest(string text, string path, string error)
    {
        if (this.loader != null)
        {
            throw new InvalidOperationException("The loader has already begun. Its too late!");
        }
        this.preloadedJsonLoaderText = text;
        if (path == null)
        {
        }
        this.preloadedJsonLoaderRoot = string.Empty;
        this.preloadedJsonLoader = true;
        this.preloadedJsonLoaderError = error;
    }

    [DebuggerHidden]
    private IEnumerator Start()
    {
        return new <Start>c__IteratorA { <>f__this = this };
    }

    bool IRustLoaderTasks.Active
    {
        get
        {
            return (this.loader != null);
        }
    }

    IDownloadTask IRustLoaderTasks.ActiveGroup
    {
        get
        {
            if (this.loader == null)
            {
                return null;
            }
            return this.loader.CurrentGroup;
        }
    }

    IEnumerable<IDownloadTask> IRustLoaderTasks.ActiveJobs
    {
        get
        {
            return new <>c__Iterator8 { <>f__this = this, $PC = -2 };
        }
    }

    IEnumerable<IDownloadTask> IRustLoaderTasks.Groups
    {
        get
        {
            return new <>c__Iterator7 { <>f__this = this, $PC = -2 };
        }
    }

    IEnumerable<IDownloadTask> IRustLoaderTasks.Jobs
    {
        get
        {
            return new <>c__Iterator9 { <>f__this = this, $PC = -2 };
        }
    }

    IDownloadTask IRustLoaderTasks.Overall
    {
        get
        {
            return this.loader;
        }
    }

    public IRustLoaderTasks Tasks
    {
        get
        {
            return this;
        }
    }

    [CompilerGenerated]
    private sealed class <>c__Iterator7 : IDisposable, IEnumerator, IEnumerable, IEnumerable<IDownloadTask>, IEnumerator<IDownloadTask>
    {
        internal IDownloadTask $current;
        internal int $PC;
        internal Group[] <$s_68>__0;
        internal int <$s_69>__1;
        internal RustLoader <>f__this;
        internal Group <group>__2;

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
                    if (this.<>f__this.loader != null)
                    {
                        this.<$s_68>__0 = this.<>f__this.loader.Groups;
                        this.<$s_69>__1 = 0;
                        while (this.<$s_69>__1 < this.<$s_68>__0.Length)
                        {
                            this.<group>__2 = this.<$s_68>__0[this.<$s_69>__1];
                            this.$current = this.<group>__2;
                            this.$PC = 1;
                            return true;
                        Label_0083:
                            this.<$s_69>__1++;
                        }
                        this.$PC = -1;
                        break;
                    }
                    break;

                case 1:
                    goto Label_0083;
            }
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        IEnumerator<IDownloadTask> IEnumerable<IDownloadTask>.GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
            {
                return this;
            }
            return new RustLoader.<>c__Iterator7 { <>f__this = this.<>f__this };
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.System.Collections.Generic.IEnumerable<Facepunch.Load.IDownloadTask>.GetEnumerator();
        }

        IDownloadTask IEnumerator<IDownloadTask>.Current
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
    private sealed class <>c__Iterator8 : IDisposable, IEnumerator, IEnumerable, IEnumerable<IDownloadTask>, IEnumerator<IDownloadTask>
    {
        internal IDownloadTask $current;
        internal int $PC;
        internal Job[] <$s_66>__1;
        internal int <$s_67>__2;
        internal RustLoader <>f__this;
        internal Group <currentGroup>__0;
        internal Job <task>__3;

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
                    if (this.<>f__this.loader != null)
                    {
                        this.<currentGroup>__0 = this.<>f__this.loader.CurrentGroup;
                        if (this.<currentGroup>__0 != null)
                        {
                            this.<$s_66>__1 = this.<currentGroup>__0.Jobs;
                            this.<$s_67>__2 = 0;
                            while (this.<$s_67>__2 < this.<$s_66>__1.Length)
                            {
                                this.<task>__3 = this.<$s_66>__1[this.<$s_67>__2];
                                this.$current = this.<task>__3;
                                this.$PC = 1;
                                return true;
                            Label_00A4:
                                this.<$s_67>__2++;
                            }
                            this.$PC = -1;
                        }
                        break;
                    }
                    break;

                case 1:
                    goto Label_00A4;
            }
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        IEnumerator<IDownloadTask> IEnumerable<IDownloadTask>.GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
            {
                return this;
            }
            return new RustLoader.<>c__Iterator8 { <>f__this = this.<>f__this };
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.System.Collections.Generic.IEnumerable<Facepunch.Load.IDownloadTask>.GetEnumerator();
        }

        IDownloadTask IEnumerator<IDownloadTask>.Current
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
    private sealed class <>c__Iterator9 : IDisposable, IEnumerator, IEnumerable, IEnumerable<IDownloadTask>, IEnumerator<IDownloadTask>
    {
        internal IDownloadTask $current;
        internal int $PC;
        internal Job[] <$s_64>__0;
        internal int <$s_65>__1;
        internal RustLoader <>f__this;
        internal Job <task>__2;

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
                    if (this.<>f__this.loader != null)
                    {
                        this.<$s_64>__0 = this.<>f__this.loader.Jobs;
                        this.<$s_65>__1 = 0;
                        while (this.<$s_65>__1 < this.<$s_64>__0.Length)
                        {
                            this.<task>__2 = this.<$s_64>__0[this.<$s_65>__1];
                            this.$current = this.<task>__2;
                            this.$PC = 1;
                            return true;
                        Label_0083:
                            this.<$s_65>__1++;
                        }
                        this.$PC = -1;
                        break;
                    }
                    break;

                case 1:
                    goto Label_0083;
            }
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        IEnumerator<IDownloadTask> IEnumerable<IDownloadTask>.GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
            {
                return this;
            }
            return new RustLoader.<>c__Iterator9 { <>f__this = this.<>f__this };
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.System.Collections.Generic.IEnumerable<Facepunch.Load.IDownloadTask>.GetEnumerator();
        }

        IDownloadTask IEnumerator<IDownloadTask>.Current
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
    private sealed class <Start>c__IteratorA : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal RustLoader <>f__this;
        internal string <bundleDirectory>__1;
        internal Exception <e>__3;
        internal Exception <e>__4;
        internal string <loaderError>__2;
        internal string <loaderText>__0;

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
                    this.<>f__this.DispatchLoadMessage("OnRustBundleFetching", this.<>f__this);
                    if (!this.<>f__this.preloadedJsonLoader)
                    {
                        char[] anyOf = new char[] { '\\', '/' };
                        this.<bundleDirectory>__1 = this.<>f__this.releaseBundleLoaderFilePath.Remove(this.<>f__this.releaseBundleLoaderFilePath.LastIndexOfAny(anyOf) + 1);
                        try
                        {
                            this.<loaderText>__0 = File.ReadAllText(this.<>f__this.releaseBundleLoaderFilePath);
                            this.<loaderError>__2 = string.Empty;
                        }
                        catch (Exception exception)
                        {
                            this.<e>__3 = exception;
                            this.<loaderText>__0 = string.Empty;
                            this.<loaderError>__2 = this.<e>__3.ToString();
                            if (string.IsNullOrEmpty(this.<loaderError>__2))
                            {
                                this.<loaderError>__2 = "Failed";
                            }
                        }
                        break;
                    }
                    this.<loaderText>__0 = this.<>f__this.preloadedJsonLoaderText;
                    if (this.<>f__this.preloadedJsonLoaderRoot == null)
                    {
                    }
                    this.<bundleDirectory>__1 = string.Empty;
                    this.<loaderError>__2 = this.<>f__this.preloadedJsonLoaderError;
                    this.<>f__this.preloadedJsonLoaderText = null;
                    this.<>f__this.preloadedJsonLoaderRoot = null;
                    this.<>f__this.preloadedJsonLoaderError = null;
                    break;

                case 1:
                    this.<>f__this.DispatchLoadMessage("OnRustBundleLoadComplete", this.<>f__this);
                    try
                    {
                        this.<>f__this.loader.Dispose();
                    }
                    catch (Exception exception2)
                    {
                        this.<e>__4 = exception2;
                        Debug.LogException(this.<e>__4);
                    }
                    finally
                    {
                        this.<>f__this.loader = null;
                    }
                    this.$current = Resources.UnloadUnusedAssets();
                    this.$PC = 2;
                    goto Label_02FD;

                case 2:
                    this.<>f__this.BroadcastMessage("OnRustLoadedFirst", SendMessageOptions.DontRequireReceiver);
                    this.<>f__this.DispatchLoadMessage("OnRustLoaded", this.<>f__this);
                    this.<>f__this.DispatchLoadMessage("OnRustReady", this.<>f__this);
                    if (!this.<>f__this.destroyGameObjectOnceLoaded)
                    {
                        Object.Destroy(this.<>f__this);
                    }
                    else
                    {
                        Object.Destroy(this.<>f__this.gameObject);
                    }
                    this.$PC = -1;
                    goto Label_02FB;

                default:
                    goto Label_02FB;
            }
            if (!string.IsNullOrEmpty(this.<loaderError>__2))
            {
                Debug.LogError(this.<loaderError>__2);
            }
            else
            {
                this.<>f__this.loader = Loader.CreateFromText(this.<loaderText>__0, this.<bundleDirectory>__1, new FileDispatch());
                Bundling.BindToLoader(this.<>f__this.loader);
                Bundling.OnceLoaded += new Bundling.OnLoadedEventHandler(RustLoader.OnResourcesLoaded);
                this.<>f__this.DispatchLoadMessage("OnRustBundlePreLoad", this.<>f__this);
                this.<>f__this.loader.StartLoading();
                this.<>f__this.DispatchLoadMessage("OnRustBundleLoadStart", this.<>f__this);
                this.$current = this.<>f__this.StartCoroutine(this.<>f__this.loader.WaitEnumerator);
                this.$PC = 1;
                goto Label_02FD;
            }
        Label_02FB:
            return false;
        Label_02FD:
            return true;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        object IEnumerator<object>.Current
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

