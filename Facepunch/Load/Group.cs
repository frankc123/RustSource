namespace Facepunch.Load
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class Group : IDownloadTask
    {
        [NonSerialized]
        public Operation _op;
        [NonSerialized]
        public int ByteLength;
        [NonSerialized]
        private string jobDesc;
        [NonSerialized]
        public Job[] Jobs;
        [NonSerialized]
        private string lastDescriptiveText;
        [NonSerialized]
        private Facepunch.Load.TaskStatusCount? lastStatusCount;

        internal void GetArrays(out AssetBundle[] assetBundles, out Item[] items)
        {
            items = new Item[this.Jobs.Length];
            assetBundles = new AssetBundle[this.Jobs.Length];
            for (int i = 0; i < this.Jobs.Length; i++)
            {
                assetBundles[i] = this.Jobs[i].AssetBundle;
                items[i] = this.Jobs[i].Item;
            }
        }

        public void Initialize()
        {
            this.ByteLength = 0;
            foreach (Job job in this.Jobs)
            {
                this.ByteLength += job.Item.ByteLength;
            }
            switch (this.Jobs.Length)
            {
                case 0:
                    this.jobDesc = "No bundles";
                    break;

                case 1:
                    this.jobDesc = "1 bundle";
                    break;

                default:
                    this.jobDesc = string.Format("{0} bundles", this.Jobs.Length);
                    break;
            }
        }

        public int ByteLengthDownloaded
        {
            get
            {
                int num = 0;
                foreach (Job job in this.Jobs)
                {
                    num += job.ByteLengthDownloaded;
                }
                return num;
            }
        }

        public string ContextualDescription
        {
            get
            {
                Facepunch.Load.TaskStatusCount taskStatusCount = this.TaskStatusCount;
                Facepunch.Load.TaskStatusCount? lastStatusCount = this.lastStatusCount;
                Facepunch.Load.TaskStatusCount count2 = !lastStatusCount.HasValue ? taskStatusCount : lastStatusCount.Value;
                if ((!this.lastStatusCount.HasValue || (count2.Pending != taskStatusCount.Pending)) || ((count2.Complete != taskStatusCount.Complete) || (count2.Downloading != taskStatusCount.Downloading)))
                {
                    this.lastStatusCount = new Facepunch.Load.TaskStatusCount?(taskStatusCount);
                    switch (((Facepunch.Load.TaskStatus) ((byte) (taskStatusCount.TaskStatus & (Facepunch.Load.TaskStatus.Complete | Facepunch.Load.TaskStatus.Downloading | Facepunch.Load.TaskStatus.Pending)))))
                    {
                        case Facepunch.Load.TaskStatus.Pending:
                            this.lastDescriptiveText = string.Format("{0} pending", taskStatusCount.Pending);
                            goto Label_01DF;

                        case Facepunch.Load.TaskStatus.Downloading:
                            this.lastDescriptiveText = string.Format("{0} downloading", taskStatusCount.Downloading);
                            goto Label_01DF;

                        case (Facepunch.Load.TaskStatus.Downloading | Facepunch.Load.TaskStatus.Pending):
                            this.lastDescriptiveText = string.Format("{0} pending, {1} downloading", taskStatusCount.Pending, taskStatusCount.Downloading);
                            goto Label_01DF;

                        case Facepunch.Load.TaskStatus.Complete:
                            this.lastDescriptiveText = string.Format("{0} complete", taskStatusCount.Complete);
                            goto Label_01DF;

                        case (Facepunch.Load.TaskStatus.Complete | Facepunch.Load.TaskStatus.Pending):
                            this.lastDescriptiveText = string.Format("{0} pending, {1} complete", taskStatusCount.Pending, taskStatusCount.Downloading);
                            goto Label_01DF;

                        case (Facepunch.Load.TaskStatus.Complete | Facepunch.Load.TaskStatus.Downloading):
                            this.lastDescriptiveText = string.Format("{0} downloading, {1} complete", taskStatusCount.Downloading, taskStatusCount.Complete);
                            goto Label_01DF;

                        case (Facepunch.Load.TaskStatus.Complete | Facepunch.Load.TaskStatus.Downloading | Facepunch.Load.TaskStatus.Pending):
                            this.lastDescriptiveText = string.Format("{0} pending, {1} downloading, {2} complete", taskStatusCount.Pending, taskStatusCount.Downloading, taskStatusCount.Complete);
                            goto Label_01DF;
                    }
                    throw new ArgumentException("TaskStatus");
                }
            Label_01DF:
                return this.lastDescriptiveText;
            }
        }

        public int Count
        {
            get
            {
                return this.Jobs.Length;
            }
        }

        public int Done
        {
            get
            {
                int num = 0;
                foreach (Job job in this.Jobs)
                {
                    if (job.TaskStatus == Facepunch.Load.TaskStatus.Complete)
                    {
                        num++;
                    }
                }
                return num;
            }
        }

        int IDownloadTask.ByteLength
        {
            get
            {
                return this.ByteLength;
            }
        }

        string IDownloadTask.Name
        {
            get
            {
                return this.jobDesc;
            }
        }

        public Facepunch.Load.Loader Loader
        {
            get
            {
                return this._op.Loader;
            }
        }

        public float PercentDone
        {
            get
            {
                return (float) (((double) this.ByteLengthDownloaded) / ((double) this.ByteLength));
            }
        }

        public Facepunch.Load.TaskStatus TaskStatus
        {
            get
            {
                Facepunch.Load.TaskStatus complete = Facepunch.Load.TaskStatus.Complete;
                foreach (Job job in this.Jobs)
                {
                    if (job.TaskStatus == Facepunch.Load.TaskStatus.Downloading)
                    {
                        return Facepunch.Load.TaskStatus.Downloading;
                    }
                    if (job.TaskStatus == Facepunch.Load.TaskStatus.Pending)
                    {
                        complete = Facepunch.Load.TaskStatus.Pending;
                    }
                }
                return complete;
            }
        }

        public Facepunch.Load.TaskStatusCount TaskStatusCount
        {
            get
            {
                Facepunch.Load.TaskStatusCount count = new Facepunch.Load.TaskStatusCount();
                foreach (Job job in this.Jobs)
                {
                    ref Facepunch.Load.TaskStatusCount countRef;
                    Facepunch.Load.TaskStatus status;
                    int num2 = countRef[status];
                    (countRef = (Facepunch.Load.TaskStatusCount) &count)[status = job.TaskStatus] = num2 + 1;
                }
                return count;
            }
        }
    }
}

