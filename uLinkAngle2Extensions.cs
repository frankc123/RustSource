using System;
using System.Runtime.CompilerServices;
using uLink;

public static class uLinkAngle2Extensions
{
    private const BitStreamTypeCode bitStreamTypeCode = BitStreamTypeCode.Int32;
    private static readonly uLink.BitStreamCodec.Deserializer deserializer = new uLink.BitStreamCodec.Deserializer(uLinkAngle2Extensions.Deserializer);
    private static readonly BitStreamCodec int32Codec = BitStreamCodec.Find(typeof(int).TypeHandle);
    private static readonly uLink.BitStreamCodec.Serializer serializer = new uLink.BitStreamCodec.Serializer(uLinkAngle2Extensions.Serializer);

    static uLinkAngle2Extensions()
    {
        BitStreamCodec.Add<Angle2>(deserializer, serializer, BitStreamTypeCode.Int32, false);
    }

    private static object Deserializer(BitStream stream, params object[] codecOptions)
    {
        object obj2 = int32Codec.deserializer(stream, codecOptions);
        if (obj2 is int)
        {
            return new Angle2 { encoded = (int) obj2 };
        }
        return obj2;
    }

    public static Angle2 ReadAngle2(this BitStream stream)
    {
        return new Angle2 { encoded = stream.ReadInt32() };
    }

    public static void Register()
    {
    }

    public static void Serialize(this BitStream stream, ref Angle2 value, params object[] codecOptions)
    {
        int encoded = value.encoded;
        int num2 = encoded;
        stream.Serialize(ref num2, codecOptions);
        if (num2 != encoded)
        {
            value.encoded = num2;
        }
    }

    private static void Serializer(BitStream stream, object value, params object[] codecOptions)
    {
        Angle2 angle = (Angle2) value;
        int32Codec.serializer(stream, angle.encoded, codecOptions);
    }

    public static void WriteAngle2(this BitStream stream, Angle2 value)
    {
        stream.WriteInt32(value.encoded);
    }
}

