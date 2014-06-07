namespace Facepunch.Hash
{
    using System;
    using System.Text;

    public static class MurmurHash2
    {
        public const uint m = 0x5bd1e995;
        public const int r = 0x18;

        public static int SINT(byte[] key, uint seed)
        {
            return (int) UINT(key, key.Length, seed);
        }

        public static int SINT(char[] key, uint seed)
        {
            return (int) UINT(key, key.Length, seed);
        }

        public static int SINT(short[] key, uint seed)
        {
            return (int) UINT(key, key.Length, seed);
        }

        public static int SINT(sbyte[] key, uint seed)
        {
            return (int) UINT(key, key.Length, seed);
        }

        public static int SINT(ushort[] key, uint seed)
        {
            return (int) UINT(key, key.Length, seed);
        }

        public static int SINT(string key, uint seed)
        {
            return (int) UINT(key, key.Length, seed);
        }

        public static int SINT(int[] key, uint seed)
        {
            return (int) UINT(key, key.Length, seed);
        }

        public static int SINT(long[] key, uint seed)
        {
            return (int) UINT(key, key.Length, seed);
        }

        public static int SINT(uint[] key, uint seed)
        {
            return (int) UINT(key, key.Length, seed);
        }

        public static int SINT(ulong[] key, uint seed)
        {
            return (int) UINT(key, key.Length, seed);
        }

        public static int SINT(byte[] key, int len, uint seed)
        {
            return (int) UINT(key, len, seed);
        }

        public static int SINT(char[] key, int len, uint seed)
        {
            return (int) UINT(key, len, seed);
        }

        public static int SINT(short[] key, int len, uint seed)
        {
            return (int) UINT(key, len, seed);
        }

        public static int SINT(string key, int len, uint seed)
        {
            return (int) UINT(key, len, seed);
        }

        public static int SINT(int[] key, int len, uint seed)
        {
            return (int) UINT(key, len, seed);
        }

        public static int SINT(long[] key, int len, uint seed)
        {
            return (int) UINT(key, len, seed);
        }

        public static int SINT(sbyte[] key, int len, uint seed)
        {
            return (int) UINT(key, len, seed);
        }

        public static int SINT(ushort[] key, int len, uint seed)
        {
            return (int) UINT(key, len, seed);
        }

        public static int SINT(uint[] key, int len, uint seed)
        {
            return (int) UINT(key, len, seed);
        }

        public static int SINT(ulong[] key, int len, uint seed)
        {
            return (int) UINT(key, len, seed);
        }

        public static int SINT(string key, Encoding encoding, uint seed)
        {
            return (int) UINT(key, encoding, seed);
        }

        public static int SINT_BLOCK(Array key, uint seed)
        {
            return (int) UINT_BLOCK(key, Buffer.ByteLength(key), seed);
        }

        public static int SINT_BLOCK(Array key, int len, uint seed)
        {
            return (int) UINT_BLOCK(key, len, seed);
        }

        public static uint UINT(byte[] key, uint seed)
        {
            return UINT(key, key.Length, seed);
        }

        public static uint UINT(char[] key, uint seed)
        {
            return UINT(key, key.Length, seed);
        }

        public static uint UINT(short[] key, uint seed)
        {
            return UINT(key, key.Length, seed);
        }

        public static uint UINT(string key, uint seed)
        {
            return UINT(key, key.Length, seed);
        }

        public static uint UINT(int[] key, uint seed)
        {
            return UINT(key, key.Length, seed);
        }

        public static uint UINT(long[] key, uint seed)
        {
            return UINT(key, key.Length, seed);
        }

        public static uint UINT(sbyte[] key, uint seed)
        {
            return UINT(key, key.Length, seed);
        }

        public static uint UINT(ushort[] key, uint seed)
        {
            return UINT(key, key.Length, seed);
        }

        public static uint UINT(uint[] key, uint seed)
        {
            return UINT(key, key.Length, seed);
        }

        public static uint UINT(ulong[] key, uint seed)
        {
            return UINT(key, key.Length, seed);
        }

        public static uint UINT(byte[] key, int len, uint seed)
        {
            uint num = seed ^ ((uint) (len * 1));
            int index = 0;
            while (len >= 4)
            {
                uint num3 = (uint) (((key[index++] | (key[index++] << 8)) | (key[index++] << 0x10)) | (key[index++] << 0x18));
                num3 *= 0x5bd1e995;
                num3 ^= num3 >> 0x18;
                num3 *= 0x5bd1e995;
                num *= 0x5bd1e995;
                num ^= num3;
                len -= 4;
            }
            switch (len)
            {
                case 1:
                    num ^= key[index];
                    num *= 0x5bd1e995;
                    break;

                case 2:
                    num ^= (uint) (key[index + 1] << 8);
                    num ^= key[index];
                    num *= 0x5bd1e995;
                    break;

                case 3:
                    num ^= (uint) (key[index + 2] << 0x10);
                    num ^= (uint) (key[index + 1] << 8);
                    num ^= key[index];
                    num *= 0x5bd1e995;
                    break;
            }
            num ^= num >> 13;
            num *= 0x5bd1e995;
            return (num ^ (num >> 15));
        }

        public static uint UINT(char[] key, int len, uint seed)
        {
            uint num = seed ^ ((uint) (len * 2));
            int index = 0;
            while (len >= 2)
            {
                uint num3 = key[index++] | (key[index++] << 0x10);
                num3 *= 0x5bd1e995;
                num3 ^= num3 >> 0x18;
                num3 *= 0x5bd1e995;
                num *= 0x5bd1e995;
                num ^= num3;
                len -= 2;
            }
            if (len == 1)
            {
                num ^= (uint) (key[index] & 0xff00);
                num ^= key[index] & '\x00ff';
                num *= 0x5bd1e995;
            }
            num ^= num >> 13;
            num *= 0x5bd1e995;
            return (num ^ (num >> 15));
        }

        public static uint UINT(short[] key, int len, uint seed)
        {
            uint num = seed ^ ((uint) (len * 2));
            int index = 0;
            while (len >= 2)
            {
                uint num3 = (uint) (((ushort) key[index++]) | (((ushort) key[index++]) << 0x10));
                num3 *= 0x5bd1e995;
                num3 ^= num3 >> 0x18;
                num3 *= 0x5bd1e995;
                num *= 0x5bd1e995;
                num ^= num3;
                len -= 2;
            }
            if (len == 1)
            {
                num ^= (uint) (key[index] & 0xff00);
                num ^= (uint) (key[index] & 0xff);
                num *= 0x5bd1e995;
            }
            num ^= num >> 13;
            num *= 0x5bd1e995;
            return (num ^ (num >> 15));
        }

        public static uint UINT(sbyte[] key, int len, uint seed)
        {
            uint num = seed ^ ((uint) (len * 1));
            int index = 0;
            while (len >= 4)
            {
                uint num3 = (uint) (((((byte) key[index++]) | (((byte) key[index++]) << 8)) | (((byte) key[index++]) << 0x10)) | (((byte) key[index++]) << 0x18));
                num3 *= 0x5bd1e995;
                num3 ^= num3 >> 0x18;
                num3 *= 0x5bd1e995;
                num *= 0x5bd1e995;
                num ^= num3;
                len -= 4;
            }
            switch (len)
            {
                case 1:
                    num ^= (uint) key[index];
                    num *= 0x5bd1e995;
                    break;

                case 2:
                    num ^= ((uint) key[index + 1]) << 8;
                    num ^= (uint) key[index];
                    num *= 0x5bd1e995;
                    break;

                case 3:
                    num ^= ((uint) key[index + 2]) << 0x10;
                    num ^= ((uint) key[index + 1]) << 8;
                    num ^= (uint) key[index];
                    num *= 0x5bd1e995;
                    break;
            }
            num ^= num >> 13;
            num *= 0x5bd1e995;
            return (num ^ (num >> 15));
        }

        public static uint UINT(ushort[] key, int len, uint seed)
        {
            uint num = seed ^ ((uint) (len * 2));
            int index = 0;
            while (len >= 2)
            {
                uint num3 = (uint) (key[index++] | (key[index++] << 0x10));
                num3 *= 0x5bd1e995;
                num3 ^= num3 >> 0x18;
                num3 *= 0x5bd1e995;
                num *= 0x5bd1e995;
                num ^= num3;
                len -= 2;
            }
            if (len == 1)
            {
                num ^= (uint) (key[index] & 0xff00);
                num ^= (uint) (key[index] & 0xff);
                num *= 0x5bd1e995;
            }
            num ^= num >> 13;
            num *= 0x5bd1e995;
            return (num ^ (num >> 15));
        }

        public static uint UINT(string key, int len, uint seed)
        {
            uint num = seed ^ ((uint) (len * 2));
            int num2 = 0;
            while (len >= 2)
            {
                uint num3 = key[num2++] | (key[num2++] << 0x10);
                num3 *= 0x5bd1e995;
                num3 ^= num3 >> 0x18;
                num3 *= 0x5bd1e995;
                num *= 0x5bd1e995;
                num ^= num3;
                len -= 2;
            }
            if (len == 1)
            {
                num ^= (uint) (key[num2] & 0xff00);
                num ^= key[num2] & '\x00ff';
                num *= 0x5bd1e995;
            }
            num ^= num >> 13;
            num *= 0x5bd1e995;
            return (num ^ (num >> 15));
        }

        public static uint UINT(int[] key, int len, uint seed)
        {
            uint num = seed ^ ((uint) (len * 4));
            int num2 = 0;
            while (len > 0)
            {
                uint num3 = (uint) key[num2++];
                num3 *= 0x5bd1e995;
                num3 ^= num3 >> 0x18;
                num3 *= 0x5bd1e995;
                num *= 0x5bd1e995;
                num ^= num3;
                len--;
            }
            num ^= num >> 13;
            num *= 0x5bd1e995;
            return (num ^ (num >> 15));
        }

        public static uint UINT(long[] key, int len, uint seed)
        {
            uint num = seed ^ ((uint) (len * 8));
            int index = 0;
            while (len > 0)
            {
                uint num3 = (uint) (((ulong) key[index]) & 0xffffffffL);
                num3 *= 0x5bd1e995;
                num3 ^= num3 >> 0x18;
                num3 *= 0x5bd1e995;
                num *= 0x5bd1e995;
                num ^= num3;
                uint num4 = (uint) (((ulong) (key[index] >> 0x20)) & 0xffffffffL);
                num4 *= 0x5bd1e995;
                num4 ^= num4 >> 0x18;
                num4 *= 0x5bd1e995;
                num *= 0x5bd1e995;
                num ^= num4;
                len--;
            }
            num ^= num >> 13;
            num *= 0x5bd1e995;
            return (num ^ (num >> 15));
        }

        public static uint UINT(uint[] key, int len, uint seed)
        {
            uint num = seed ^ ((uint) (len * 4));
            int num2 = 0;
            while (len > 0)
            {
                uint num3 = key[num2++];
                num3 *= 0x5bd1e995;
                num3 ^= num3 >> 0x18;
                num3 *= 0x5bd1e995;
                num *= 0x5bd1e995;
                num ^= num3;
                len--;
            }
            num ^= num >> 13;
            num *= 0x5bd1e995;
            return (num ^ (num >> 15));
        }

        public static uint UINT(ulong[] key, int len, uint seed)
        {
            uint num = seed ^ ((uint) (len * 8));
            int index = 0;
            while (len > 0)
            {
                uint num3 = (uint) (key[index] & 0xffffffffL);
                num3 *= 0x5bd1e995;
                num3 ^= num3 >> 0x18;
                num3 *= 0x5bd1e995;
                num *= 0x5bd1e995;
                num ^= num3;
                uint num4 = (uint) ((key[index] >> 0x20) & 0xffffffffL);
                num4 *= 0x5bd1e995;
                num4 ^= num4 >> 0x18;
                num4 *= 0x5bd1e995;
                num *= 0x5bd1e995;
                num ^= num4;
                len--;
            }
            num ^= num >> 13;
            num *= 0x5bd1e995;
            return (num ^ (num >> 15));
        }

        public static uint UINT(string key, Encoding encoding, uint seed)
        {
            return UINT(encoding.GetBytes(key), seed);
        }

        public static uint UINT_BLOCK(Array key, uint seed)
        {
            return UINT_BLOCK(key, Buffer.ByteLength(key), seed);
        }

        public static uint UINT_BLOCK(Array key, int len, uint seed)
        {
            uint num = seed ^ ((uint) (len * 1));
            int index = 0;
            while (len >= 4)
            {
                uint num3 = (uint) (((Buffer.GetByte(key, index++) | (Buffer.GetByte(key, index++) << 8)) | (Buffer.GetByte(key, index++) << 0x10)) | (Buffer.GetByte(key, index++) << 0x18));
                num3 *= 0x5bd1e995;
                num3 ^= num3 >> 0x18;
                num3 *= 0x5bd1e995;
                num *= 0x5bd1e995;
                num ^= num3;
                len -= 4;
            }
            switch (len)
            {
                case 1:
                    num ^= Buffer.GetByte(key, index);
                    num *= 0x5bd1e995;
                    break;

                case 2:
                    num ^= (uint) (Buffer.GetByte(key, index + 1) << 8);
                    num ^= Buffer.GetByte(key, index);
                    num *= 0x5bd1e995;
                    break;

                case 3:
                    num ^= (uint) (Buffer.GetByte(key, index + 2) << 0x10);
                    num ^= (uint) (Buffer.GetByte(key, index + 1) << 8);
                    num ^= Buffer.GetByte(key, index);
                    num *= 0x5bd1e995;
                    break;
            }
            num ^= num >> 13;
            num *= 0x5bd1e995;
            return (num ^ (num >> 15));
        }
    }
}

