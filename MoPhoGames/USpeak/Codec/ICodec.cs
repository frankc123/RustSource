namespace MoPhoGames.USpeak.Codec
{
    using System;

    public interface ICodec
    {
        short[] Decode(byte[] data, BandMode bandMode);
        byte[] Encode(short[] data, BandMode bandMode);
        int GetSampleSize(int recordingFrequency);
    }
}

