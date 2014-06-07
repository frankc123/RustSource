namespace MoPhoGames.USpeak.Core
{
    using MoPhoGames.USpeak.Codec;
    using MoPhoGames.USpeak.Core.Utils;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class USpeakAudioClipCompressor : MonoBehaviour
    {
        private static List<byte> data = new List<byte>();
        private static List<short> tmp = new List<short>();

        public static byte[] CompressAudioData(float[] samples, int channels, out int sample_count, BandMode mode, ICodec Codec, float gain = 1f)
        {
            USpeakAudioClipCompressor.data.Clear();
            sample_count = 0;
            short[] data = USpeakAudioClipConverter.AudioDataToShorts(samples, channels, gain);
            byte[] collection = Codec.Encode(data, mode);
            USpeakPoolUtils.Return(data);
            USpeakAudioClipCompressor.data.AddRange(collection);
            USpeakPoolUtils.Return(collection);
            return USpeakAudioClipCompressor.data.ToArray();
        }

        private static void CopyStream(Stream input, Stream output)
        {
            byte[] @byte = USpeakPoolUtils.GetByte(0x8000);
            while (true)
            {
                int count = input.Read(@byte, 0, @byte.Length);
                if (count <= 0)
                {
                    break;
                }
                output.Write(@byte, 0, count);
            }
            USpeakPoolUtils.Return(@byte);
        }

        public static float[] DecompressAudio(byte[] data, int samples, int channels, bool threeD, BandMode mode, ICodec Codec, float gain)
        {
            int frequency = 0xfa0;
            if (mode == BandMode.Narrow)
            {
                frequency = 0x1f40;
            }
            else if (mode == BandMode.Wide)
            {
                frequency = 0x3e80;
            }
            byte[] buffer = data;
            short[] collection = Codec.Decode(buffer, mode);
            tmp.Clear();
            tmp.AddRange(collection);
            USpeakPoolUtils.Return(collection);
            return USpeakAudioClipConverter.ShortsToAudioData(tmp.ToArray(), channels, frequency, threeD, gain);
        }
    }
}

