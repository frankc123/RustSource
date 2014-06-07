namespace MoPhoGames.USpeak.Codec
{
    using MoPhoGames.USpeak.Core.Utils;
    using System;

    [Serializable]
    public class MuLawCodec : ICodec
    {
        public short[] Decode(byte[] data, BandMode mode)
        {
            return MuLawDecoder.MuLawDecode(data);
        }

        public byte[] Encode(short[] data, BandMode mode)
        {
            return MuLawEncoder.MuLawEncode(data);
        }

        public int GetSampleSize(int recordingFrequency)
        {
            return 0;
        }

        private class MuLawDecoder
        {
            private static readonly short[] muLawToPcmMap = new short[0x100];

            static MuLawDecoder()
            {
                for (byte i = 0; i < 0xff; i = (byte) (i + 1))
                {
                    muLawToPcmMap[i] = Decode(i);
                }
            }

            private static short Decode(byte mulaw)
            {
                mulaw = ~mulaw;
                int num = mulaw & 0x80;
                int num2 = (mulaw & 0x70) >> 4;
                int num3 = mulaw & 15;
                num3 |= 0x10;
                num3 = num3 << 1;
                num3++;
                num3 = num3 << (num2 + 2);
                num3 -= 0x84;
                return ((num != 0) ? ((short) -num3) : ((short) num3));
            }

            public static short[] MuLawDecode(byte[] data)
            {
                int length = data.Length;
                short[] @short = USpeakPoolUtils.GetShort(length);
                for (int i = 0; i < length; i++)
                {
                    @short[i] = muLawToPcmMap[data[i]];
                }
                return @short;
            }
        }

        private class MuLawEncoder
        {
            public const int BIAS = 0x84;
            public const int MAX = 0x7f7b;
            private static byte[] pcmToMuLawMap = new byte[0x10000];

            static MuLawEncoder()
            {
                for (int i = -32768; i <= 0x7fff; i++)
                {
                    pcmToMuLawMap[i & 0xffff] = encode(i);
                }
            }

            private static byte encode(int pcm)
            {
                int num = (pcm & 0x8000) >> 8;
                if (num != 0)
                {
                    pcm = -pcm;
                }
                if (pcm > 0x7f7b)
                {
                    pcm = 0x7f7b;
                }
                pcm += 0x84;
                int num2 = 7;
                for (int i = 0x4000; (pcm & i) == 0; i = i >> 1)
                {
                    num2--;
                }
                int num4 = (pcm >> (num2 + 3)) & 15;
                byte num5 = (byte) ((num | (num2 << 4)) | num4);
                return ~num5;
            }

            public static byte MuLawEncode(short pcm)
            {
                return pcmToMuLawMap[pcm & 0xffff];
            }

            public static byte MuLawEncode(int pcm)
            {
                return pcmToMuLawMap[pcm & 0xffff];
            }

            public static byte[] MuLawEncode(short[] pcm)
            {
                int length = pcm.Length;
                byte[] @byte = USpeakPoolUtils.GetByte(length);
                for (int i = 0; i < length; i++)
                {
                    @byte[i] = MuLawEncode(pcm[i]);
                }
                return @byte;
            }

            public static byte[] MuLawEncode(int[] pcm)
            {
                int length = pcm.Length;
                byte[] @byte = USpeakPoolUtils.GetByte(length);
                for (int i = 0; i < length; i++)
                {
                    @byte[i] = MuLawEncode(pcm[i]);
                }
                return @byte;
            }

            public static bool ZeroTrap
            {
                get
                {
                    return (pcmToMuLawMap[0x80e8] != 0);
                }
                set
                {
                    byte num = !value ? ((byte) 0) : ((byte) 2);
                    for (int i = 0x8000; i <= 0x8484; i++)
                    {
                        pcmToMuLawMap[i] = num;
                    }
                }
            }
        }
    }
}

