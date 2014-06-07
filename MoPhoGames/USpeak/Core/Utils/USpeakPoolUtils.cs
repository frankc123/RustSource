namespace MoPhoGames.USpeak.Core.Utils
{
    using System;
    using System.Collections.Generic;

    public class USpeakPoolUtils
    {
        private static List<byte[]> BytePool = new List<byte[]>();
        private static List<float[]> FloatPool = new List<float[]>();
        private static List<short[]> ShortPool = new List<short[]>();

        public static byte[] GetByte(int length)
        {
            for (int i = 0; i < BytePool.Count; i++)
            {
                if (BytePool[i].Length == length)
                {
                    byte[] buffer = BytePool[i];
                    BytePool.RemoveAt(i);
                    return buffer;
                }
            }
            return new byte[length];
        }

        public static float[] GetFloat(int length)
        {
            for (int i = 0; i < FloatPool.Count; i++)
            {
                if (FloatPool[i].Length == length)
                {
                    float[] numArray = FloatPool[i];
                    FloatPool.RemoveAt(i);
                    return numArray;
                }
            }
            return new float[length];
        }

        public static short[] GetShort(int length)
        {
            for (int i = 0; i < ShortPool.Count; i++)
            {
                if (ShortPool[i].Length == length)
                {
                    short[] numArray = ShortPool[i];
                    ShortPool.RemoveAt(i);
                    return numArray;
                }
            }
            return new short[length];
        }

        public static void Return(byte[] d)
        {
            BytePool.Add(d);
        }

        public static void Return(short[] d)
        {
            ShortPool.Add(d);
        }

        public static void Return(float[] d)
        {
            FloatPool.Add(d);
        }
    }
}

