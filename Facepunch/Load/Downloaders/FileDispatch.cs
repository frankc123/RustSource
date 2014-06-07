namespace Facepunch.Load.Downloaders
{
    using Facepunch.Load;
    using System;
    using System.IO;
    using UnityEngine;

    public sealed class FileDispatch : IDownloaderDispatch
    {
        private WWWDispatch wwwFallback = new WWWDispatch();

        public void BindLoader(Loader loader)
        {
            this.wwwFallback.BindLoader(loader);
        }

        public IDownloader CreateDownloaderForJob(Job job)
        {
            if (File.Exists(job.Path))
            {
                FileInfo info = new FileInfo(job.Path);
                if (info.Length == job.ByteLength)
                {
                    AssetBundle bundle = AssetBundle.CreateFromFile(Path.GetFullPath(job.Path).Replace('\\', '/'));
                    if (bundle != null)
                    {
                        return new Downloader { bundle = bundle };
                    }
                }
            }
            Debug.LogWarning("Missing Bundle " + job.Path);
            if ((job.ContentType == ContentType.Assets) && (job.TypeOfAssets != typeof(NavMesh)))
            {
                return null;
            }
            return this.wwwFallback.CreateDownloaderForJob(job);
        }

        public void DeleteDownloader(Job job, IDownloader downloader)
        {
            if (!(downloader is Downloader))
            {
                this.wwwFallback.DeleteDownloader(job, downloader);
            }
        }

        public void UnbindLoader(Loader loader)
        {
            this.wwwFallback.BindLoader(loader);
        }

        private class Downloader : IDownloader
        {
            public AssetBundle bundle;

            public void BeginJob(Job job)
            {
                job.OnDownloadingBegin(this);
                job.OnDownloadingComplete();
            }

            public float GetDownloadProgress(Job job)
            {
                return 0f;
            }

            public AssetBundle GetLoadedAssetBundle(Job job)
            {
                return this.bundle;
            }

            public void OnJobCompleted(Job job)
            {
                this.bundle = null;
            }
        }
    }
}

