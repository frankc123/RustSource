using System;
using System.Security.Cryptography;
using System.Text;

public sealed class CRC32 : HashAlgorithm
{
    private uint hash;
    private const uint I = 0x100;
    private const uint J = 8;
    public const uint kDefaultPolynomial = 0xedb88320;
    public const uint kDefaultSeed = uint.MaxValue;
    public const uint kTableSize = 0x100;
    private readonly uint seed;
    private readonly uint[] table;

    public CRC32()
    {
        this.table = Default.Table;
        this.seed = uint.MaxValue;
        this.Initialize();
    }

    public CRC32(uint polynomial, uint seed)
    {
        this.table = (polynomial != 0xedb88320) ? ProcessHashTable(polynomial) : Default.Table;
        this.seed = seed;
        this.Initialize();
    }

    private void BufferHash(byte[] buffer, int start, int size)
    {
        while (size-- > 0)
        {
            this.hash = (this.hash >> 8) ^ this.table[(int) ((IntPtr) (buffer[start++] ^ (this.hash & 0xff)))];
        }
    }

    private static uint BufferHash(uint[] table, uint seed, byte[] buffer, int start, int size)
    {
        while (size-- > 0)
        {
            seed = (seed >> 8) ^ table[(int) ((IntPtr) (buffer[start++] ^ (seed & 0xff)))];
        }
        return seed;
    }

    protected sealed override void HashCore(byte[] buffer, int start, int length)
    {
        this.BufferHash(buffer, start, length);
    }

    protected sealed override byte[] HashFinal()
    {
        uint num = ~this.hash;
        byte[] buffer = new byte[] { (byte) ((num >> 0x18) & 0xff), (byte) ((num >> 0x10) & 0xff), (byte) ((num >> 8) & 0xff), (byte) (num & 0xff) };
        base.HashValue = buffer;
        return buffer;
    }

    public override void Initialize()
    {
        this.hash = this.seed;
    }

    private static uint[] ProcessHashTable(uint p)
    {
        uint[] numArray = new uint[0x100];
        for (ushort i = 0; i < 0x100; i = (ushort) (i + 1))
        {
            numArray[i] = i;
            for (uint j = 0; j < 8; j++)
            {
                numArray[i] = ((numArray[i] & 1) != 1) ? (numArray[i] >> 1) : ((numArray[i] >> 1) ^ p);
            }
        }
        return numArray;
    }

    public static uint Quick(byte[] buffer)
    {
        return ~BufferHash(Default.Table, uint.MaxValue, buffer, 0, buffer.Length);
    }

    public static uint Quick(uint seed, byte[] buffer)
    {
        return ~BufferHash(Default.Table, seed, buffer, 0, buffer.Length);
    }

    public static uint Quick(uint polynomial, uint seed, byte[] buffer)
    {
        return ~BufferHash(Default.Table, seed, buffer, 0, buffer.Length);
    }

    public static uint String(string str)
    {
        return Quick(Encoding.ASCII.GetBytes(str));
    }

    public sealed override int HashSize
    {
        get
        {
            return 0x20;
        }
    }

    private static class Default
    {
        public static readonly uint[] Table = new uint[0x100];

        static Default()
        {
            for (uint i = 0; i < 0x100; i++)
            {
                Table[i] = i;
                for (uint j = 0; j < 8; j++)
                {
                    Table[i] = ((Table[i] & 1) != 1) ? (Table[i] >> 1) : ((Table[i] >> 1) ^ 0xedb88320);
                }
            }
        }
    }
}

