namespace Facepunch.Load.Downloaders
{
    using Facepunch.Load;
    using System;
    using UnityEngine;

    public sealed class WWWDispatch : IDownloaderDispatch
    {
        private readonly Utility.ReferenceCountedCoroutine.Runner coroutineRunner = new Utility.ReferenceCountedCoroutine.Runner("WWWDispatch");

        public void BindLoader(Loader loader)
        {
            this.coroutineRunner.Retain();
        }

        public IDownloader CreateDownloaderForJob(Job job)
        {
            return new Downloader(this);
        }

        public void DeleteDownloader(Job job, IDownloader idownloader)
        {
            if (idownloader is Downloader)
            {
                Downloader downloader = (Downloader) idownloader;
                downloader.Job = null;
                downloader.Download = null;
            }
        }

        private void DownloadBegin(Downloader downloader, Job job)
        {
            downloader.Job = job;
            downloader.Download = new WWW(job.Path);
            job.OnDownloadingBegin(downloader);
            if (downloader.Download.isDone)
            {
                this.DownloadFinished(downloader);
            }
            else
            {
                this.coroutineRunner.Install(Downloader.DownloaderRoutineCallback, downloader, downloader.Download, true);
            }
        }

        private void DownloadFinished(Downloader downloader)
        {
            downloader.Job.OnDownloadingComplete();
        }

        public void UnbindLoader(Loader loader)
        {
            this.coroutineRunner.Release();
        }

        private class Downloader : IDownloader
        {
            public readonly WWWDispatch Dispatch;
            public WWW Download;
            public static readonly Utility.ReferenceCountedCoroutine.Callback DownloaderRoutineCallback = new Utility.ReferenceCountedCoroutine.Callback(WWWDispatch.Downloader.DownloaderRoutine);
            public Facepunch.Load.Job Job;

            public Downloader(WWWDispatch dispatch)
            {
                this.Dispatch = dispatch;
            }

            public void BeginJob(Facepunch.Load.Job job)
            {
                this.Dispatch.DownloadBegin(this, job);
            }

            private static bool DownloaderRoutine(ref object yieldInstruction, ref object tag)
            {
                WWWDispatch.Downloader downloader = (WWWDispatch.Downloader) tag;
                yieldInstruction = downloader.Download;
                if (downloader.Download.isDone)
                {
                    downloader.Dispatch.DownloadFinished(downloader);
                    return false;
                }
                return true;
            }

            public float GetDownloadProgress(Facepunch.Load.Job job)
            {
                return ((this.Download != null) ? this.Download.progress : 0f);
            }

            public AssetBundle GetLoadedAssetBundle(Facepunch.Load.Job job)
            {
                return this.Download.assetBundle;
            }

            public void OnJobCompleted(Facepunch.Load.Job job)
            {
                if (this.Job == job)
                {
                    this.Download.Dispose();
                    this.Download = null;
                    this.Job = null;
                }
            }
        }
    }
}

