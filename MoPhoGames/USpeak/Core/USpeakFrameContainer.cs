namespace MoPhoGames.USpeak.Core
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct USpeakFrameContainer
    {
        public ushort Samples;
        public byte[] encodedData;
        public void LoadFrom(byte[] source)
        {
            int length = BitConverter.ToInt32(source, 0);
            this.Samples = BitConverter.ToUInt16(source, 4);
            this.encodedData = new byte[length];
            Array.Copy(source, 6, this.encodedData, 0, length);
        }

        public byte[] ToByteArray()
        {
            byte[] array = new byte[6 + this.encodedData.Length];
            BitConverter.GetBytes(this.encodedData.Length).CopyTo(array, 0);
            Array.Copy(BitConverter.GetBytes(this.Samples), 0, array, 4, 2);
            for (int i = 0; i < this.encodedData.Length; i++)
            {
                array[i + 6] = this.encodedData[i];
            }
            return array;
        }
    }
}

