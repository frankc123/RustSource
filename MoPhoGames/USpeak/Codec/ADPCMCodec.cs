namespace MoPhoGames.USpeak.Codec
{
    using MoPhoGames.USpeak.Core.Utils;
    using System;

    [Serializable]
    internal class ADPCMCodec : ICodec
    {
        private int index;
        private static int[] indexTable = new int[] { -1, -1, -1, -1, 2, 4, 6, 8, -1, -1, -1, -1, 2, 4, 6, 8 };
        private int newSample;
        private int predictedSample;
        private int stepsize = 7;
        private static int[] stepsizeTable = new int[] { 
            7, 8, 9, 10, 11, 12, 14, 0x10, 0x11, 0x13, 0x15, 0x17, 0x19, 0x1c, 0x1f, 0x22, 
            0x25, 0x29, 0x2d, 50, 0x37, 60, 0x42, 0x49, 80, 0x58, 0x61, 0x6b, 0x76, 130, 0x8f, 0x9d, 
            0xad, 190, 0xd1, 230, 0xfd, 0x117, 0x133, 0x151, 0x173, 0x198, 0x1c1, 0x1ee, 0x220, 0x256, 0x292, 0x2d4, 
            0x31c, 0x36c, 0x3c3, 0x424, 0x48e, 0x502, 0x583, 0x5f2, 0x6ab, 0x754, 0x812, 0x8e0, 0x9c3, 0xabd, 0xbd0, 0xcff, 
            0xe4c, 0xfba, 0x114c, 0x1307, 0x14ee, 0x1706, 0x1954, 0x1bdc, 0x1ea5, 0x21b6, 0x2515, 0x28ca, 0x2cdf, 0x315b, 0x364b, 0x3bb9, 
            0x41b2, 0x4844, 0x31aec, 0x5771, 0x602f, 0x69ce, 0x7462, 0x7fff
         };

        private short ADPCM_Decode(byte originalSample)
        {
            int num = 0;
            num = ((this.stepsize * originalSample) / 4) + (this.stepsize / 8);
            if ((originalSample & 4) == 4)
            {
                num += this.stepsize;
            }
            if ((originalSample & 2) == 2)
            {
                num += this.stepsize >> 1;
            }
            if ((originalSample & 1) == 1)
            {
                num += this.stepsize >> 2;
            }
            num += this.stepsize >> 3;
            if ((originalSample & 8) == 8)
            {
                num = -num;
            }
            this.newSample = num;
            if (this.newSample > 0x7fff)
            {
                this.newSample = 0x7fff;
            }
            else if (this.newSample < -32768)
            {
                this.newSample = -32768;
            }
            this.index += indexTable[originalSample];
            if (this.index < 0)
            {
                this.index = 0;
            }
            if (this.index > 0x58)
            {
                this.index = 0x58;
            }
            this.stepsize = stepsizeTable[this.index];
            return (short) this.newSample;
        }

        private byte ADPCM_Encode(short originalSample)
        {
            int num = originalSample - this.predictedSample;
            if (num >= 0)
            {
                this.newSample = 0;
            }
            else
            {
                this.newSample = 8;
                num = -num;
            }
            byte num2 = 4;
            int stepsize = this.stepsize;
            for (int i = 0; i < 3; i++)
            {
                if (num >= stepsize)
                {
                    this.newSample |= num2;
                    num -= stepsize;
                }
                stepsize = stepsize >> 1;
                num2 = (byte) (num2 >> 1);
            }
            num = this.stepsize >> 3;
            if ((this.newSample & 4) != 0)
            {
                num += this.stepsize;
            }
            if ((this.newSample & 2) != 0)
            {
                num += this.stepsize >> 1;
            }
            if ((this.newSample & 1) != 0)
            {
                num += this.stepsize >> 2;
            }
            if ((this.newSample & 8) != 0)
            {
                num = -num;
            }
            this.predictedSample += num;
            if (this.predictedSample > 0x7fff)
            {
                this.predictedSample = 0x7fff;
            }
            if (this.predictedSample < -32768)
            {
                this.predictedSample = -32768;
            }
            this.index += indexTable[this.newSample];
            if (this.index < 0)
            {
                this.index = 0;
            }
            else if (this.index > 0x58)
            {
                this.index = 0x58;
            }
            this.stepsize = stepsizeTable[this.index];
            return (byte) this.newSample;
        }

        public short[] Decode(byte[] data, BandMode mode)
        {
            this.Init();
            short[] @short = USpeakPoolUtils.GetShort(data.Length * 2);
            for (int i = 0; i < data.Length; i++)
            {
                byte num2 = data[i];
                byte originalSample = (byte) (num2 & 15);
                byte num4 = (byte) (num2 >> 4);
                @short[i * 2] = this.ADPCM_Decode(originalSample);
                @short[(i * 2) + 1] = this.ADPCM_Decode(num4);
            }
            return @short;
        }

        public byte[] Encode(short[] data, BandMode mode)
        {
            this.Init();
            int length = data.Length / 2;
            if ((length % 2) != 0)
            {
                length++;
            }
            byte[] @byte = USpeakPoolUtils.GetByte(length);
            for (int i = 0; i < @byte.Length; i++)
            {
                if ((i * 2) >= data.Length)
                {
                    return @byte;
                }
                byte num3 = this.ADPCM_Encode(data[i * 2]);
                byte num4 = 0;
                if (((i * 2) + 1) < data.Length)
                {
                    num4 = this.ADPCM_Encode(data[(i * 2) + 1]);
                }
                @byte[i] = (byte) ((num4 << 4) | num3);
            }
            return @byte;
        }

        public int GetSampleSize(int recordingFrequency)
        {
            return 0;
        }

        private void Init()
        {
            this.predictedSample = 0;
            this.stepsize = 7;
            this.index = 0;
            this.newSample = 0;
        }
    }
}

