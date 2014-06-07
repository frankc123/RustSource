namespace Facepunch.Load
{
    using System;
    using System.Reflection;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct TaskStatusCount
    {
        public int Pending;
        public int Downloading;
        public int Complete;
        public static readonly TaskStatusCount OnePending;
        public static readonly TaskStatusCount OneDownloading;
        public static readonly TaskStatusCount OneComplete;
        static TaskStatusCount()
        {
            TaskStatusCount count = new TaskStatusCount {
                Pending = 1
            };
            OnePending = count;
            TaskStatusCount count2 = new TaskStatusCount {
                Downloading = 1
            };
            OneDownloading = count2;
            TaskStatusCount count3 = new TaskStatusCount {
                Complete = 1
            };
            OneComplete = count3;
        }

        public int Total
        {
            get
            {
                return ((this.Pending + this.Downloading) + this.Complete);
            }
        }
        public int Remaining
        {
            get
            {
                return (this.Pending + this.Downloading);
            }
        }
        public float PercentComplete
        {
            get
            {
                return ((this.Complete != 0) ? ((float) (((double) this.Remaining) / ((double) this.Total))) : 0f);
            }
        }
        public float PercentPending
        {
            get
            {
                return ((this.Pending != 0) ? ((float) (((double) this.Pending) / ((double) this.Total))) : 0f);
            }
        }
        public float PercentDownloading
        {
            get
            {
                return ((this.Downloading != 0f) ? ((float) (((double) this.Downloading) / ((double) this.Total))) : 0f);
            }
        }
        public bool CompletelyPending
        {
            get
            {
                return ((this.Pending > 0) && ((this.Downloading == 0) && (this.Complete == 0)));
            }
        }
        public bool CompletelyDownloading
        {
            get
            {
                return ((this.Downloading > 0) && ((this.Pending == 0) && (this.Complete == 0)));
            }
        }
        public bool CompletelyComplete
        {
            get
            {
                return ((this.Complete > 0) && ((this.Downloading == 0) && (this.Pending == 0)));
            }
        }
        public Facepunch.Load.TaskStatus TaskStatus
        {
            get
            {
                if (this.Pending > 0)
                {
                    if (this.Downloading > 0)
                    {
                        if (this.Complete > 0)
                        {
                            return (Facepunch.Load.TaskStatus.Complete | Facepunch.Load.TaskStatus.Downloading | Facepunch.Load.TaskStatus.Pending);
                        }
                        return (Facepunch.Load.TaskStatus.Downloading | Facepunch.Load.TaskStatus.Pending);
                    }
                    if (this.Complete > 0)
                    {
                        return (Facepunch.Load.TaskStatus.Complete | Facepunch.Load.TaskStatus.Pending);
                    }
                    return Facepunch.Load.TaskStatus.Pending;
                }
                if (this.Downloading > 0)
                {
                    if (this.Complete > 0)
                    {
                        return (Facepunch.Load.TaskStatus.Complete | Facepunch.Load.TaskStatus.Downloading);
                    }
                    return Facepunch.Load.TaskStatus.Downloading;
                }
                if (this.Complete > 0)
                {
                    return Facepunch.Load.TaskStatus.Complete;
                }
                return 0;
            }
        }
        public int this[Facepunch.Load.TaskStatus status]
        {
            get
            {
                switch (status)
                {
                    case Facepunch.Load.TaskStatus.Pending:
                        return this.Pending;

                    case Facepunch.Load.TaskStatus.Downloading:
                        return this.Downloading;

                    case (Facepunch.Load.TaskStatus.Downloading | Facepunch.Load.TaskStatus.Pending):
                        return (this.Pending + this.Downloading);

                    case Facepunch.Load.TaskStatus.Complete:
                        return this.Complete;

                    case (Facepunch.Load.TaskStatus.Complete | Facepunch.Load.TaskStatus.Pending):
                        return (this.Pending + this.Complete);

                    case (Facepunch.Load.TaskStatus.Complete | Facepunch.Load.TaskStatus.Downloading):
                        return (this.Pending + this.Downloading);

                    case (Facepunch.Load.TaskStatus.Complete | Facepunch.Load.TaskStatus.Downloading | Facepunch.Load.TaskStatus.Pending):
                        return ((this.Pending + this.Downloading) + this.Complete);
                }
                return 0;
            }
            set
            {
                switch (status)
                {
                    case Facepunch.Load.TaskStatus.Pending:
                        this.Pending = value;
                        break;

                    case Facepunch.Load.TaskStatus.Downloading:
                        this.Downloading = value;
                        break;

                    case (Facepunch.Load.TaskStatus.Downloading | Facepunch.Load.TaskStatus.Pending):
                        this.Downloading = this.Pending = value;
                        break;

                    case Facepunch.Load.TaskStatus.Complete:
                        this.Complete = value;
                        break;

                    case (Facepunch.Load.TaskStatus.Complete | Facepunch.Load.TaskStatus.Pending):
                        this.Complete = this.Pending = value;
                        break;

                    case (Facepunch.Load.TaskStatus.Complete | Facepunch.Load.TaskStatus.Downloading):
                        this.Downloading = this.Complete = value;
                        break;

                    case (Facepunch.Load.TaskStatus.Complete | Facepunch.Load.TaskStatus.Downloading | Facepunch.Load.TaskStatus.Pending):
                        this.Complete = this.Pending = this.Downloading = value;
                        break;
                }
            }
        }
    }
}

