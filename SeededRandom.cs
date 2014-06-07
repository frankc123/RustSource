using System;
using System.Runtime.InteropServices;

public class SeededRandom
{
    private uint allocCount;
    private byte bitPos;
    private readonly byte[] byteBuffer;
    private byte bytePos;
    private const int kBitsInByte = 8;
    private const int kBufferBitSize = 0x80;
    private const int kBufferSize = 0x10;
    private const byte kMaskBitPos = 7;
    private const byte kMaskBytePos = 15;
    private const int kMaxAllocCount = 0x2000000;
    private const int kMaxAllocPos = 0x1ffffff;
    private const int kShiftBitPos = 3;
    private const int kShiftBytePos = 4;
    private Random rand;
    public readonly int Seed;

    public SeededRandom() : this(Environment.TickCount)
    {
    }

    public SeededRandom(int seed)
    {
        this.byteBuffer = new byte[0x10];
        this.Seed = seed;
        this.rand = new Random(seed);
    }

    public bool Boolean()
    {
        if ((this.bytePos == 0) && (this.bitPos == 0))
        {
            this.Fill();
            this.bitPos = (byte) (this.bitPos + 1);
            return ((this.byteBuffer[0] & 1) == 1);
        }
        bool flag = (this.byteBuffer[this.bytePos] & (((int) 1) << this.bitPos)) == (((int) 1) << this.bitPos);
        this.bitPos = (byte) (this.bitPos + 1);
        if (this.bitPos == 8)
        {
            this.bitPos = 0;
            this.bytePos = (byte) (this.bytePos + 1);
            if (this.bytePos == 0x10)
            {
                this.bytePos = 0;
            }
        }
        return flag;
    }

    private void Fill()
    {
        if (++this.allocCount == 0x2000000)
        {
            this.rand = new Random(this.Seed);
            this.allocCount = 1;
        }
        this.rand.NextBytes(this.byteBuffer);
    }

    private static double LT1(double v)
    {
        return ((v <= 9.88131291682493E-324) ? v : (v - double.Epsilon));
    }

    public T Pick<T>(T[] array)
    {
        if (array == null)
        {
            throw new ArgumentNullException("array");
        }
        return array[this.RandomIndex(array.Length)];
    }

    public bool Pick<T>(T[] array, out T value)
    {
        if ((array == null) || (array.Length == 0))
        {
            value = default(T);
            return false;
        }
        value = array[this.RandomIndex(array.Length)];
        return true;
    }

    public int RandomBits(int bitCount)
    {
        if ((bitCount < 0) || (bitCount > 0x20))
        {
            throw new ArgumentOutOfRangeException("bitCount");
        }
        int num = 0;
        for (int i = 0; bitCount-- > 0; i++)
        {
            if (this.Boolean())
            {
                num |= ((int) 1) << i;
            }
        }
        return num;
    }

    public double RandomFraction16()
    {
        uint num = 0;
        for (int i = 0; i < 0x10; i++)
        {
            if (this.Boolean())
            {
                num |= ((uint) 1) << i;
            }
        }
        return (((double) num) / 65535.0);
    }

    public double RandomFraction32()
    {
        uint num = 0;
        for (int i = 0; i < 0x20; i++)
        {
            if (this.Boolean())
            {
                num |= ((uint) 1) << i;
            }
        }
        return (((double) num) / 4294967295);
    }

    public double RandomFraction8()
    {
        uint num = 0;
        for (int i = 0; i < 8; i++)
        {
            if (this.Boolean())
            {
                num |= ((uint) 1) << i;
            }
        }
        return (((double) num) / 255.0);
    }

    public double RandomFractionBitDepth(int bitDepth)
    {
        if ((bitDepth < 1) || (bitDepth > 0x20))
        {
            throw new ArgumentOutOfRangeException("bitDepth", "!( bitDepth > 0 && bitDepth <= 32 )");
        }
        if (bitDepth == 0x20)
        {
            return this.RandomFraction32();
        }
        int num = 0;
        int num2 = 0;
        for (int i = 0; i < bitDepth; i++)
        {
            int num3 = ((int) 1) << i;
            if (this.Boolean())
            {
                num |= num3;
            }
            num2 |= num3;
        }
        return (((double) num) / ((double) num2));
    }

    private double RandomFractionBitDepth(int bitDepth, int bitMask)
    {
        if ((bitDepth < 1) || (bitDepth > 0x20))
        {
            throw new ArgumentOutOfRangeException("bitDepth", "!( bitDepth > 0 && bitDepth <= 32 )");
        }
        if (bitDepth == 0x20)
        {
            return this.RandomFraction32();
        }
        if (bitMask <= 0)
        {
            throw new ArgumentException("bitMask", "!(bitMask > 0)");
        }
        int num = 0;
        for (int i = 0; i < bitDepth; i++)
        {
            if (this.Boolean())
            {
                num |= ((int) 1) << i;
            }
        }
        return (((double) num) / ((double) bitMask));
    }

    public double RandomFractionBitDepthLT1(int bitDepth)
    {
        return LT1(this.RandomFractionBitDepth(bitDepth));
    }

    private double RandomFractionBitDepthLT1(int bitDepth, int bitMask)
    {
        return LT1(this.RandomFractionBitDepth(bitDepth, bitMask));
    }

    public int RandomIndex(int length)
    {
        if ((length == 0) || ((length & -2147483648) == -2147483648))
        {
            throw new ArgumentOutOfRangeException("length", "!(length <= 0)");
        }
        uint num = (uint) length;
        num = num >> 1;
        if (num == 0)
        {
            return 0;
        }
        int bitMask = 1;
        byte bitDepth = 1;
        num = num >> 1;
        if (num != 0)
        {
            do
            {
                bitDepth = (byte) (bitDepth + 1);
                bitMask = (bitMask << 1) | 1;
            }
            while ((num = num >> 1) != 0);
            return (int) Math.Floor((double) (this.RandomFractionBitDepthLT1(bitDepth, bitMask) * length));
        }
        return (!this.Boolean() ? 0 : 1);
    }

    public double Range(double minInclusive, double maxInclusive)
    {
        return this.Range(minInclusive, maxInclusive, 0x10);
    }

    public int Range(int minInclusive, int maxInclusive)
    {
        if (minInclusive > maxInclusive)
        {
            int num = maxInclusive;
            maxInclusive = minInclusive;
            minInclusive = num;
        }
        else if (maxInclusive == minInclusive)
        {
            return minInclusive;
        }
        ulong num2 = (ulong) (maxInclusive - minInclusive);
        if (num2 > 0x7fffffffL)
        {
            return (minInclusive + ((int) ((long) Math.Round((double) (num2 * this.RandomFraction32())))));
        }
        int bitCount = 0;
        int num4 = 0;
        uint num5 = (uint) num2;
        while ((num5 = num5 >> 1) != 0)
        {
            bitCount++;
            num4 = (num4 << 1) | 1;
        }
        return (minInclusive + ((int) Math.Round((double) ((((double) this.RandomBits(bitCount)) / ((double) num4)) * num2))));
    }

    public float Range(float minInclusive, float maxInclusive)
    {
        return (float) this.Range((double) minInclusive, (double) maxInclusive);
    }

    public double Range(double minInclusive, double maxInclusive, int bitDepth)
    {
        return ((minInclusive != maxInclusive) ? ((this.RandomFractionBitDepth(bitDepth) * (maxInclusive - minInclusive)) + minInclusive) : minInclusive);
    }

    public float Range(float minInclusive, float maxInclusive, int bitDepth)
    {
        return (float) this.Range((double) minInclusive, (double) maxInclusive, bitDepth);
    }

    public bool Reset()
    {
        if (this.allocCount > 0)
        {
            this.rand = new Random(this.Seed);
            this.allocCount = 0;
            return true;
        }
        return false;
    }

    public uint PositionData
    {
        get
        {
            return ((((((((this.bytePos <= 0) && (this.bitPos <= 0)) || (this.allocCount <= 0)) ? this.allocCount : (this.allocCount - 1)) << 4) | ((byte) (this.bytePos & 15))) << 7) | ((byte) (this.bitPos & 7)));
        }
        set
        {
            byte num = (byte) (value & 7);
            byte num2 = (byte) ((value = value >> 3) & 15);
            uint num3 = value = value >> 4;
            if ((num > 0) || (num2 > 0))
            {
                num3++;
            }
            if (num3 < this.allocCount)
            {
                this.allocCount = 0;
                this.rand = new Random(this.Seed);
            }
            while (this.allocCount < num3)
            {
                this.allocCount++;
                this.rand.NextBytes(this.byteBuffer);
            }
            this.bitPos = num;
            this.bytePos = num2;
        }
    }
}

