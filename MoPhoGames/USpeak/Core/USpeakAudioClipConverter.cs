namespace MoPhoGames.USpeak.Core
{
    using MoPhoGames.USpeak.Core.Utils;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class USpeakAudioClipConverter
    {
        public static short[] AudioDataToShorts(float[] samples, int channels, float gain = 1f)
        {
            short[] @short = USpeakPoolUtils.GetShort(samples.Length * channels);
            for (int i = 0; i < samples.Length; i++)
            {
                float f = samples[i] * gain;
                if (Mathf.Abs(f) > 1f)
                {
                    if (f > 0f)
                    {
                        f = 1f;
                    }
                    else
                    {
                        f = -1f;
                    }
                }
                float num3 = f * 3267f;
                @short[i] = (short) num3;
            }
            return @short;
        }

        public static float[] ShortsToAudioData(short[] data, int channels, int frequency, bool threedimensional, float gain)
        {
            float[] @float = USpeakPoolUtils.GetFloat(data.Length);
            for (int i = 0; i < @float.Length; i++)
            {
                int num2 = data[i];
                @float[i] = (((float) num2) / 3267f) * gain;
            }
            return @float;
        }
    }
}

