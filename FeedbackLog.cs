using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

public static class FeedbackLog
{
    private static MemoryStream memstream = new MemoryStream();
    public static BinaryWriter Writer = new BinaryWriter(memstream);

    [DllImport("librust")]
    private static extern void Client_Info(byte type, IntPtr pData, uint iSize);
    public static unsafe void End(TYPE message_type)
    {
        byte* pinned numPtr = ((memstream.GetBuffer() != null) && (memstream.GetBuffer().Length != 0)) ? &(memstream.GetBuffer()[0]) : null;
        IntPtr pData = (IntPtr) numPtr;
        Client_Info((byte) message_type, pData, (uint) memstream.Position);
        numPtr = null;
    }

    public static void Start(TYPE message_type)
    {
        memstream.Position = 0L;
    }

    public static void WriteObject(GameObject obj)
    {
        if (obj == null)
        {
            Writer.Write((byte) 0);
        }
        else
        {
            Character component = obj.GetComponent<Character>();
            if (component != null)
            {
                if (component.playerClient != null)
                {
                    Writer.Write((byte) 1);
                    Writer.Write(component.playerClient.userID);
                }
                else
                {
                    Writer.Write((byte) 2);
                    Writer.Write(obj.ToString());
                }
            }
            else
            {
                Writer.Write((byte) 100);
                Writer.Write(obj.ToString());
            }
        }
    }

    public static void WriteVector(Vector3 vec)
    {
        Writer.Write(vec.x);
        Writer.Write(vec.y);
        Writer.Write(vec.z);
    }

    public enum TYPE
    {
        Chat = 10,
        Connected = 2,
        Death = 9,
        HardwareInfo = 5,
        LoadProgress = 4,
        Mods = 7,
        SimpleExploit = 8,
        StartConnect = 3
    }
}

