using System;
using System.IO;
using System.Reflection;
using UnityEngine;

public class BrandingVersion : MonoBehaviour
{
    public dfRichTextLabel textVersion;

    private DateTime RetrieveLinkerTimestamp()
    {
        string location = Assembly.GetCallingAssembly().Location;
        byte[] buffer = new byte[0x800];
        Stream stream = null;
        try
        {
            stream = new FileStream(location, FileMode.Open, FileAccess.Read);
            stream.Read(buffer, 0, 0x800);
        }
        finally
        {
            if (stream != null)
            {
                stream.Close();
            }
        }
        int num3 = BitConverter.ToInt32(buffer, 60);
        int num4 = BitConverter.ToInt32(buffer, num3 + 8);
        DateTime time = new DateTime(0x7b2, 1, 1, 0, 0, 0);
        time = time.AddSeconds((double) num4);
        return time.AddHours((double) TimeZone.CurrentTimeZone.GetUtcOffset(time).Hours);
    }

    private void Start()
    {
        this.textVersion.Text = this.RetrieveLinkerTimestamp().ToString(@"d MMM yyyy\, h:mmtt");
    }
}

