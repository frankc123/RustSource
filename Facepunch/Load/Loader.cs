namespace Facepunch.Load
{
    using LitJson;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class Loader : IDisposable, IDownloadTask
    {
        [NonSerialized]
        private int currentGroup = -1;
        [NonSerialized]
        private IDownloaderDispatch Dispatch;
        [NonSerialized]
        private bool disposed;
        [NonSerialized]
        public readonly Group[] Groups;
        [NonSerialized]
        public readonly Job[] Jobs;
        [NonSerialized]
        private int jobsCompleted;
        [NonSerialized]
        private int jobsCompletedInGroup;
        [NonSerialized]
        private readonly Group MasterGroup;

        public event MultipleAssetBundlesLoadedEventHandler OnAllAssetBundlesLoaded;

        public event AssetBundleLoadedEventHandler OnAssetBundleLoaded;

        public event MultipleAssetBundlesLoadedEventHandler OnGroupedAssetBundlesLoaded;

        private Loader(Group masterGroup, Job[] allJobs, Group[] allGroups, IDownloaderDispatch dispatch)
        {
            this.MasterGroup = masterGroup;
            this.Jobs = allJobs;
            this.Groups = allGroups;
            this.Dispatch = dispatch;
        }

        public static Loader Create(Reader reader, IDownloaderDispatch dispatch)
        {
            return Deserialize(reader, dispatch);
        }

        public static Loader CreateFromFile(string downloadListPath, IDownloaderDispatch dispatch)
        {
            using (Reader reader = Reader.CreateFromFile(downloadListPath))
            {
                return Deserialize(reader, dispatch);
            }
        }

        public static Loader CreateFromFile(string downloadListPath, string bundlePath, IDownloaderDispatch dispatch)
        {
            using (Reader reader = Reader.CreateFromFile(downloadListPath, bundlePath))
            {
                return Deserialize(reader, dispatch);
            }
        }

        public static Loader CreateFromReader(JsonReader jsonReader, string bundlePath, IDownloaderDispatch dispatch)
        {
            using (Reader reader = Reader.CreateFromReader(jsonReader, bundlePath))
            {
                return Deserialize(reader, dispatch);
            }
        }

        public static Loader CreateFromReader(TextReader textReader, string bundlePath, IDownloaderDispatch dispatch)
        {
            using (Reader reader = Reader.CreateFromReader(textReader, bundlePath))
            {
                return Deserialize(reader, dispatch);
            }
        }

        public static Loader CreateFromText(string downloadListJson, string bundlePath, IDownloaderDispatch dispatch)
        {
            using (Reader reader = Reader.CreateFromText(downloadListJson, bundlePath))
            {
                return Deserialize(reader, dispatch);
            }
        }

        private static Loader Deserialize(Reader reader, IDownloaderDispatch dispatch)
        {
            List<Item[]> list = new List<Item[]>();
            List<Item> list2 = new List<Item>();
            while (reader.Read())
            {
                switch (reader.Token)
                {
                    case Token.RandomLoadOrderAreaBegin:
                        list2.Clear();
                        break;

                    case Token.BundleListing:
                        list2.Add(reader.Item);
                        break;

                    case Token.RandomLoadOrderAreaEnd:
                        list.Add(list2.ToArray());
                        break;

                    case Token.DownloadQueueEnd:
                    {
                        Operation operation = new Operation();
                        int num = 0;
                        foreach (Item[] itemArray in list)
                        {
                            num += itemArray.Length;
                        }
                        Job[] allJobs = new Job[num];
                        int num2 = 0;
                        foreach (Item[] itemArray2 in list)
                        {
                            foreach (Item item in itemArray2)
                            {
                                Job job = new Job {
                                    _op = operation,
                                    Item = item
                                };
                                allJobs[num2++] = job;
                            }
                        }
                        Group masterGroup = new Group {
                            _op = operation,
                            Jobs = allJobs
                        };
                        masterGroup.Initialize();
                        Group[] allGroups = new Group[list.Count];
                        int num4 = 0;
                        int index = 0;
                        foreach (Item[] itemArray4 in list)
                        {
                            int length = itemArray4.Length;
                            Job[] jobArray2 = new Job[length];
                            for (int i = 0; i < length; i++)
                            {
                                jobArray2[i] = allJobs[num4++];
                            }
                            allGroups[index] = new Group();
                            allGroups[index]._op = operation;
                            allGroups[index].Jobs = jobArray2;
                            allGroups[index].Initialize();
                            for (int j = 0; j < length; j++)
                            {
                                jobArray2[j].Group = allGroups[index];
                            }
                            index++;
                        }
                        return (operation.Loader = new Loader(masterGroup, allJobs, allGroups, dispatch));
                    }
                }
            }
            throw new InvalidProgramException();
        }

        public void Dispose()
        {
            if (!this.disposed)
            {
                this.disposed = true;
                this.DisposeDispatch();
            }
        }

        private void DisposeDispatch()
        {
            if (this.Dispatch != null)
            {
                IDownloaderDispatch dispatch = this.Dispatch;
                this.Dispatch = null;
                dispatch.UnbindLoader(this);
            }
        }

        internal void OnJobCompleted(Job job, IDownloader downloader)
        {
            job.AssetBundle = downloader.GetLoadedAssetBundle(job);
            if (this.OnAssetBundleLoaded != null)
            {
                try
                {
                    this.OnAssetBundleLoaded(job.AssetBundle, job.Item);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
            downloader.OnJobCompleted(job);
            this.Dispatch.DeleteDownloader(job, downloader);
            if (++this.jobsCompleted == this.MasterGroup.Count)
            {
                if (this.OnAllAssetBundlesLoaded != null)
                {
                    Item[] itemArray;
                    AssetBundle[] bundleArray;
                    this.MasterGroup.GetArrays(out bundleArray, out itemArray);
                    try
                    {
                        this.OnAllAssetBundlesLoaded(bundleArray, itemArray);
                    }
                    catch (Exception exception2)
                    {
                        Debug.LogException(exception2);
                    }
                }
                this.DisposeDispatch();
            }
            else if (++this.jobsCompletedInGroup == this.Groups[this.currentGroup].Jobs.Length)
            {
                if (this.OnGroupedAssetBundlesLoaded != null)
                {
                    Item[] itemArray2;
                    AssetBundle[] bundleArray2;
                    this.Groups[this.currentGroup].GetArrays(out bundleArray2, out itemArray2);
                    try
                    {
                        this.OnGroupedAssetBundlesLoaded(bundleArray2, itemArray2);
                    }
                    catch (Exception exception3)
                    {
                        Debug.LogException(exception3);
                    }
                }
                this.StartNextGroup();
            }
        }

        private void StartJob(Job job)
        {
            this.Dispatch.CreateDownloaderForJob(job).BeginJob(job);
        }

        public void StartLoading()
        {
            if (this.currentGroup == -1)
            {
                this.Dispatch.BindLoader(this);
                if (this.Groups.Length > 0)
                {
                    this.StartNextGroup();
                }
            }
        }

        private void StartNextGroup()
        {
            this.jobsCompletedInGroup = 0;
            foreach (Job job in this.Groups[++this.currentGroup].Jobs)
            {
                this.StartJob(job);
            }
        }

        public int ByteLength
        {
            get
            {
                return this.MasterGroup.ByteLength;
            }
        }

        public int ByteLengthDownloaded
        {
            get
            {
                return this.MasterGroup.ByteLengthDownloaded;
            }
        }

        public int Count
        {
            get
            {
                return this.MasterGroup.Count;
            }
        }

        public Group CurrentGroup
        {
            get
            {
                return (((this.currentGroup < 0) || (this.currentGroup >= this.Groups.Length)) ? null : this.Groups[this.currentGroup]);
            }
        }

        public int Done
        {
            get
            {
                return this.MasterGroup.Done;
            }
        }

        string IDownloadTask.ContextualDescription
        {
            get
            {
                return this.MasterGroup.ContextualDescription;
            }
        }

        string IDownloadTask.Name
        {
            get
            {
                return "Loading all bundles";
            }
        }

        public float PercentDone
        {
            get
            {
                return this.MasterGroup.PercentDone;
            }
        }

        public Facepunch.Load.TaskStatus TaskStatus
        {
            get
            {
                return this.MasterGroup.TaskStatus;
            }
        }

        public Facepunch.Load.TaskStatusCount TaskStatusCount
        {
            get
            {
                return this.MasterGroup.TaskStatusCount;
            }
        }

        public IEnumerator WaitEnumerator
        {
            get
            {
                return new <>c__Iterator25 { <>f__this = this };
            }
        }

        [CompilerGenerated]
        private sealed class <>c__Iterator25 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal Loader <>f__this;

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
                    case 1:
                        if (this.<>f__this.jobsCompleted < this.<>f__this.MasterGroup.Jobs.Length)
                        {
                            this.$current = null;
                            this.$PC = 1;
                            return true;
                        }
                        this.$PC = -1;
                        break;
                }
                return false;
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
}

