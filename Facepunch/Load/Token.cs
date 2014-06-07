namespace Facepunch.Load
{
    using System;

    public enum Token
    {
        Uninitialized,
        DownloadQueueBegin,
        RandomLoadOrderAreaBegin,
        BundleListing,
        RandomLoadOrderAreaEnd,
        DownloadQueueEnd,
        End
    }
}

