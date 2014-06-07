namespace Facepunch.Load
{
    using System;

    public interface IDownloaderDescriptive : IDownloader
    {
        bool DescribeProgress(Job job, ref string lastString);
    }
}

