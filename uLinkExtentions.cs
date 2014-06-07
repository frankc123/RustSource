using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using uLink;

public static class uLinkExtentions
{
    private const ushort bit1234567 = 0x7f;
    private const ushort bit8 = 0x80;
    private const int kByte0 = 0;
    private const int kByte1 = 8;
    private const int kByte2 = 0x10;
    private const int kByte3 = 0x18;
    private const int kByte4 = 0x20;
    private const int kByte5 = 40;
    private const int kByte6 = 0x30;
    private const int kByte7 = 0x38;

    public static byte[] GetDataByteArray(this BitStream stream)
    {
        int num = stream._bitCount;
        int newSize = num / 8;
        if ((num % 8) != 0)
        {
            newSize++;
        }
        byte[] array = stream._data;
        if (array.Length > newSize)
        {
            Array.Resize<byte>(ref array, newSize);
        }
        return array;
    }

    public static byte[] GetDataByteArrayShiftedRight(this BitStream stream, int right)
    {
        if (right == 0)
        {
            return stream.GetDataByteArray();
        }
        int num = stream._bitCount;
        int num2 = num / 8;
        if ((num % 8) != 0)
        {
            num2++;
        }
        byte[] buffer = new byte[right + num2];
        byte[] buffer2 = stream._data;
        for (int i = 0; i < num2; i++)
        {
            buffer[right++] = buffer2[i];
        }
        return buffer;
    }

    public static void Read7BitEncodedSize(this BitStream stream, out short u)
    {
        ushort num;
        stream.Read7BitEncodedSize(out num);
        if (num > 0x7fff)
        {
            throw new InvalidOperationException("Wrong");
        }
        u = (short) num;
    }

    public static void Read7BitEncodedSize(this BitStream stream, out int u)
    {
        uint num;
        stream.Read7BitEncodedSize(out num);
        if (num > 0x7fffffff)
        {
            throw new InvalidOperationException("Wrong");
        }
        u = (int) num;
    }

    public static void Read7BitEncodedSize(this BitStream stream, out long u)
    {
        ulong num;
        stream.Read7BitEncodedSize(out num);
        if (num > 0x7fffffffffffffffL)
        {
            throw new InvalidOperationException("Wrong");
        }
        u = (long) num;
    }

    public static void Read7BitEncodedSize(this BitStream stream, out ushort u)
    {
        byte num = stream.ReadByte();
        int num2 = 0;
        u = (ushort) (num & 0x7f);
        while (((num & 0x80) == 0x80) && (num2 <= 2))
        {
            num = stream.ReadByte();
            u = (ushort) (u | ((ushort) ((num & 0x7f) << (++num2 * 7))));
        }
    }

    public static void Read7BitEncodedSize(this BitStream stream, out uint u)
    {
        byte num = stream.ReadByte();
        int num2 = 0;
        u = (uint) (num & 0x7f);
        while (((num & 0x80) == 0x80) && (num2 <= 4))
        {
            num = stream.ReadByte();
            u |= (num & 0x7f) << (++num2 * 7);
        }
    }

    public static void Read7BitEncodedSize(this BitStream stream, out ulong u)
    {
        byte num = stream.ReadByte();
        int num2 = 0;
        u = (ulong) (num & 0x7f);
        while (((num & 0x80) == 0x80) && (num2 <= 9))
        {
            num = stream.ReadByte();
            u |= (ulong) ((num & 0x7f) << (++num2 * 7));
        }
    }

    public static void ReadByteArray_MinimalCalls(this BitStream stream, out byte[] array, out int length, params object[] codecOptions)
    {
        length = stream.Read<int>(codecOptions);
        if (length == 0)
        {
            array = null;
        }
        else
        {
            array = new byte[length];
            int num = length;
            int num2 = length / 8;
            int num3 = length / 4;
            int num4 = length / 2;
            int num5 = length;
            num5 -= num4 * 2;
            while (num5-- > 0)
            {
                array[--num] = stream.Read<byte>(codecOptions);
            }
            num4 -= num3 * 2;
            while (num4-- > 0)
            {
                ushort num6 = stream.Read<ushort>(codecOptions);
                array[--num] = (byte) ((num6 >> 8) & 0xff);
                array[--num] = (byte) (num6 & 0xff);
            }
            num3 -= num2 * 2;
            while (num3-- > 0)
            {
                uint num7 = stream.Read<uint>(codecOptions);
                array[--num] = (byte) ((num7 >> 0x18) & 0xff);
                array[--num] = (byte) ((num7 >> 0x10) & 0xff);
                array[--num] = (byte) ((num7 >> 8) & 0xff);
                array[--num] = (byte) (num7 & 0xff);
            }
            while (num2-- > 0)
            {
                ulong num8 = stream.Read<ulong>(codecOptions);
                array[--num] = (byte) ((num8 >> 0x38) & ((ulong) 0xffL));
                array[--num] = (byte) ((num8 >> 0x30) & ((ulong) 0xffL));
                array[--num] = (byte) ((num8 >> 40) & ((ulong) 0xffL));
                array[--num] = (byte) ((num8 >> 0x20) & ((ulong) 0xffL));
                array[--num] = (byte) ((num8 >> 0x18) & ((ulong) 0xffL));
                array[--num] = (byte) ((num8 >> 0x10) & ((ulong) 0xffL));
                array[--num] = (byte) ((num8 >> 8) & ((ulong) 0xffL));
                array[--num] = (byte) (num8 & ((ulong) 0xffL));
            }
        }
    }

    public static void Write7BitEncodedSize(this BitStream stream, short u)
    {
        if (u < 0)
        {
            throw new ArgumentOutOfRangeException("u", "u<0");
        }
        stream.Write7BitEncodedSize((ushort) u);
    }

    public static void Write7BitEncodedSize(this BitStream stream, int u)
    {
        if (u < 0)
        {
            throw new ArgumentOutOfRangeException("u", "u<0");
        }
        stream.Write7BitEncodedSize((uint) u);
    }

    public static void Write7BitEncodedSize(this BitStream stream, long u)
    {
        if (u < 0L)
        {
            throw new ArgumentOutOfRangeException("u", "u<0");
        }
        stream.Write7BitEncodedSize((ulong) u);
    }

    public static void Write7BitEncodedSize(this BitStream stream, ushort u)
    {
        while (u >= 0x80)
        {
            stream.WriteByte((byte) ((u & 0x7f) | 0x80));
            u = (ushort) (u >> 7);
        }
        stream.WriteByte((byte) u);
    }

    public static void Write7BitEncodedSize(this BitStream stream, uint u)
    {
        while (u >= 0x80)
        {
            stream.WriteByte((byte) ((u & 0x7f) | 0x80));
            u = u >> 7;
        }
        stream.WriteByte((byte) u);
    }

    public static void Write7BitEncodedSize(this BitStream stream, ulong u)
    {
        while (u >= 0x80L)
        {
            stream.WriteByte((byte) ((u & ((ulong) 0x7fL)) | ((ulong) 0x80L)));
            u = u >> 7;
        }
        stream.WriteByte((byte) u);
    }

    public static void WriteByteArray_MinimumCalls(this BitStream stream, byte[] array, int offset, int length, params object[] codecOptions)
    {
        stream.Write<int>(length, codecOptions);
        int num = offset + length;
        int num2 = length / 8;
        int num3 = length / 4;
        int num4 = length / 2;
        int num5 = length;
        num5 -= num4 * 2;
        while (num5-- > 0)
        {
            stream.Write<byte>(array[--num], codecOptions);
        }
        num4 -= num3 * 2;
        while (num4-- > 0)
        {
            ushort num6 = (ushort) (array[--num] << 8);
            num6 = (ushort) (num6 | array[--num]);
            stream.Write<ushort>(num6, codecOptions);
        }
        num3 -= num2 * 2;
        while (num3-- > 0)
        {
            uint num7 = (uint) (array[--num] << 0x18);
            num7 |= (uint) (array[--num] << 0x10);
            num7 |= (uint) (array[--num] << 8);
            num7 |= array[--num];
            stream.Write<uint>(num7, codecOptions);
        }
        while (num2-- > 0)
        {
            ulong num8 = (ulong) (array[--num] << 0x38);
            num8 |= array[--num] << 0x30;
            num8 |= array[--num] << 40;
            num8 |= array[--num] << 0x20;
            num8 |= array[--num] << 0x18;
            num8 |= array[--num] << 0x10;
            num8 |= array[--num] << 8;
            num8 |= array[--num];
            stream.Write<ulong>(num8, codecOptions);
        }
    }
}

