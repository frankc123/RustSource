namespace Facepunch.Load
{
    using System;

    public abstract class Stream : IDisposable
    {
        protected Stream()
        {
        }

        public abstract void Dispose();

        protected static class Property
        {
            public const string ByteLength = "size";
            public const string ContentType = "content";
            public const string Path = "filename";
            public const string RelativeOrAbsoluteURL = "url";
            public const string TypeOfAssets = "type";
        }
    }
}

