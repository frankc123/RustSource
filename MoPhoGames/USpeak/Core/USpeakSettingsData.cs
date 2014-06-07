namespace MoPhoGames.USpeak.Core
{
    using System;

    public class USpeakSettingsData
    {
        public BandMode bandMode;
        public int Codec;

        public USpeakSettingsData()
        {
            this.bandMode = BandMode.Narrow;
            this.Codec = 0;
        }

        public USpeakSettingsData(byte src)
        {
            if ((src & 1) == 1)
            {
                this.bandMode = BandMode.Narrow;
            }
            else if ((src & 2) == 2)
            {
                this.bandMode = BandMode.Wide;
            }
            else
            {
                this.bandMode = BandMode.UltraWide;
            }
            this.Codec = src >> 2;
        }

        public byte ToByte()
        {
            byte num = 0;
            if (this.bandMode == BandMode.Narrow)
            {
                num = (byte) (num | 1);
            }
            else if (this.bandMode == BandMode.Wide)
            {
                num = (byte) (num | 2);
            }
            return (byte) (num | ((byte) (this.Codec << 2)));
        }
    }
}

