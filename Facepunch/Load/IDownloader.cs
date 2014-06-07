namespace Facepunch.Load
{
    using System;
    using UnityEngine;

    public interface IDownloader
    {
        void BeginJob(Job job);
        float GetDownloadProgress(Job job);
        AssetBundle GetLoadedAssetBundle(Job job);
        void OnJobCompleted(Job job);
    }
}

