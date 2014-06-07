namespace Facepunch.Load
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct Item
    {
        public string Name;
        public string Path;
        public int ByteLength;
        public Type TypeOfAssets;
        public Facepunch.Load.ContentType ContentType;
    }
}

