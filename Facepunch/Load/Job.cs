namespace Facepunch.Load
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class Job : IDownloadTask
    {
        [NonSerialized]
        public Operation _op;
        private IDownloaderDescriptive descriptiveDownloader;
        private IDownloader downloader;
        private bool hasDescriptiveDownloader;
        [NonSerialized]
        public Facepunch.Load.Item Item;
        private string lastDescriptiveString;
        public object Tag;

        public Job()
        {
            this.TaskStatus = Facepunch.Load.TaskStatus.Pending;
        }

        public void OnDownloadingBegin(IDownloader downloader)
        {
            this.downloader = downloader;
            this.lastDescriptiveString = null;
            this.hasDescriptiveDownloader = (this.descriptiveDownloader = downloader as IDownloaderDescriptive) != null;
            this.TaskStatus = Facepunch.Load.TaskStatus.Downloading;
        }

        public void OnDownloadingComplete()
        {
            this.TaskStatus = Facepunch.Load.TaskStatus.Complete;
            IDownloader downloader = this.downloader;
            this.downloader = null;
            this.descriptiveDownloader = null;
            this.hasDescriptiveDownloader = false;
            this.Loader.OnJobCompleted(this, downloader);
            if (!object.ReferenceEquals(this.Tag, null))
            {
                Debug.LogWarning("Clearing tag manually ( should have been done by the IDownloader during the OnJobComplete callback )");
            }
            this.Tag = null;
        }

        internal UnityEngine.AssetBundle AssetBundle { get; set; }

        public int ByteLength
        {
            get
            {
                return this.Item.ByteLength;
            }
        }

        public int ByteLengthDownloaded
        {
            get
            {
                return Mathf.FloorToInt(this.PercentDone * this.ByteLength);
            }
        }

        public Facepunch.Load.ContentType ContentType
        {
            get
            {
                return this.Item.ContentType;
            }
        }

        public string ContextualDescription
        {
            get
            {
                switch (this.TaskStatus)
                {
                    case Facepunch.Load.TaskStatus.Pending:
                        return "Pending";

                    case Facepunch.Load.TaskStatus.Downloading:
                        if (this.hasDescriptiveDownloader && this.descriptiveDownloader.DescribeProgress(this, ref this.lastDescriptiveString))
                        {
                        }
                        return ((this.lastDescriptiveString != null) ? "Downloading" : string.Empty);

                    case Facepunch.Load.TaskStatus.Complete:
                        return "Complete";
                }
                throw new ArgumentOutOfRangeException("TaskStatus");
            }
        }

        int IDownloadTask.Count
        {
            get
            {
                return 1;
            }
        }

        int IDownloadTask.Done
        {
            get
            {
                return ((this.TaskStatus != Facepunch.Load.TaskStatus.Complete) ? 0 : 1);
            }
        }

        TaskStatusCount IDownloadTask.TaskStatusCount
        {
            get
            {
                switch (this.TaskStatus)
                {
                    case Facepunch.Load.TaskStatus.Pending:
                        return TaskStatusCount.OnePending;

                    case Facepunch.Load.TaskStatus.Downloading:
                        return TaskStatusCount.OneDownloading;

                    case Facepunch.Load.TaskStatus.Complete:
                        return TaskStatusCount.OneComplete;
                }
                throw new ArgumentOutOfRangeException("TaskStatus");
            }
        }

        public Facepunch.Load.Group Group { get; internal set; }

        public Facepunch.Load.Loader Loader
        {
            get
            {
                return this._op.Loader;
            }
        }

        public string Name
        {
            get
            {
                return this.Item.Name;
            }
        }

        public string Path
        {
            get
            {
                return this.Item.Path;
            }
        }

        public float PercentDone
        {
            get
            {
                switch (this.TaskStatus)
                {
                    case Facepunch.Load.TaskStatus.Pending:
                        return 0f;

                    case Facepunch.Load.TaskStatus.Downloading:
                        return this.downloader.GetDownloadProgress(this);

                    case Facepunch.Load.TaskStatus.Complete:
                        return 1f;
                }
                throw new ArgumentOutOfRangeException("TaskStatus");
            }
        }

        public Facepunch.Load.TaskStatus TaskStatus { get; private set; }

        public Type TypeOfAssets
        {
            get
            {
                return this.Item.TypeOfAssets;
            }
        }
    }
}

