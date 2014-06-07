namespace Facepunch.Load
{
    using System;

    public interface IDownloaderDispatch
    {
        void BindLoader(Loader loader);
        IDownloader CreateDownloaderForJob(Job job);
        void DeleteDownloader(Job job, IDownloader downloader);
        void UnbindLoader(Loader loader);
    }
}

