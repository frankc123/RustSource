using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

public class JPGEncoder
{
    private BitString[] bitcode;
    private uint bytenew;
    private ByteArray byteout;
    private int bytepos;
    private int[] category;
    private int cores;
    private int[] DU;
    private float[] fdtbl_UV;
    private float[] fdtbl_Y;
    private BitmapData image;
    public bool isDone;
    private string path;
    private int sf;
    private byte[] std_ac_chrominance_nrcodes;
    private byte[] std_ac_chrominance_values;
    private byte[] std_ac_luminance_nrcodes;
    private byte[] std_ac_luminance_values;
    private byte[] std_dc_chrominance_nrcodes;
    private byte[] std_dc_chrominance_values;
    private byte[] std_dc_luminance_nrcodes;
    private byte[] std_dc_luminance_values;
    private float[] UDU;
    private BitString[] UVAC_HT;
    private BitString[] UVDC_HT;
    private int[] UVTable;
    private float[] VDU;
    private BitString[] YAC_HT;
    private BitString[] YDC_HT;
    private float[] YDU;
    private int[] YTable;
    private int[] ZigZag;

    public JPGEncoder(Texture2D texture, float quality) : this(texture, quality, string.Empty, false)
    {
    }

    public JPGEncoder(Texture2D texture, float quality, bool blocking) : this(texture, quality, string.Empty, blocking)
    {
    }

    public JPGEncoder(Texture2D texture, float quality, string path) : this(texture, quality, path, false)
    {
    }

    public JPGEncoder(Texture2D texture, float quality, string path, bool blocking)
    {
        this.ZigZag = new int[] { 
            0, 1, 5, 6, 14, 15, 0x1b, 0x1c, 2, 4, 7, 13, 0x10, 0x1a, 0x1d, 0x2a, 
            3, 8, 12, 0x11, 0x19, 30, 0x29, 0x2b, 9, 11, 0x12, 0x18, 0x1f, 40, 0x2c, 0x35, 
            10, 0x13, 0x17, 0x20, 0x27, 0x2d, 0x34, 0x36, 20, 0x16, 0x21, 0x26, 0x2e, 0x33, 0x37, 60, 
            0x15, 0x22, 0x25, 0x2f, 50, 0x38, 0x3b, 0x3d, 0x23, 0x24, 0x30, 0x31, 0x39, 0x3a, 0x3e, 0x3f
         };
        this.YTable = new int[0x40];
        this.UVTable = new int[0x40];
        this.fdtbl_Y = new float[0x40];
        this.fdtbl_UV = new float[0x40];
        this.std_dc_luminance_nrcodes = new byte[] { 
            0, 0, 1, 5, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 
            0, 0, 0, 0
         };
        this.std_dc_luminance_values = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
        this.std_ac_luminance_nrcodes = new byte[] { 
            0, 0, 2, 1, 3, 3, 2, 4, 3, 5, 5, 4, 4, 0, 0, 1, 
            0x7d, 0, 0, 0
         };
        this.std_ac_luminance_values = new byte[] { 
            1, 2, 3, 0, 4, 0x11, 5, 0x12, 0x21, 0x31, 0x41, 6, 0x13, 0x51, 0x61, 7, 
            0x22, 0x71, 20, 50, 0x81, 0x91, 0xa1, 8, 0x23, 0x42, 0xb1, 0xc1, 0x15, 0x52, 0xd1, 240, 
            0x24, 0x33, 0x62, 0x72, 130, 9, 10, 0x16, 0x17, 0x18, 0x19, 0x1a, 0x25, 0x26, 0x27, 40, 
            0x29, 0x2a, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3a, 0x43, 0x44, 0x45, 70, 0x47, 0x48, 0x49, 
            0x4a, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 90, 0x63, 100, 0x65, 0x66, 0x67, 0x68, 0x69, 
            0x6a, 0x73, 0x74, 0x75, 0x76, 0x77, 120, 0x79, 0x7a, 0x83, 0x84, 0x85, 0x86, 0x87, 0x88, 0x89, 
            0x8a, 0x92, 0x93, 0x94, 0x95, 150, 0x97, 0x98, 0x99, 0x9a, 0xa2, 0xa3, 0xa4, 0xa5, 0xa6, 0xa7, 
            0xa8, 0xa9, 170, 0xb2, 0xb3, 180, 0xb5, 0xb6, 0xb7, 0xb8, 0xb9, 0xba, 0xc2, 0xc3, 0xc4, 0xc5, 
            0xc6, 0xc7, 200, 0xc9, 0xca, 210, 0xd3, 0xd4, 0xd5, 0xd6, 0xd7, 0xd8, 0xd9, 0xda, 0xe1, 0xe2, 
            0xe3, 0xe4, 0xe5, 230, 0xe7, 0xe8, 0xe9, 0xea, 0xf1, 0xf2, 0xf3, 0xf4, 0xf5, 0xf6, 0xf7, 0xf8, 
            0xf9, 250, 0, 0
         };
        this.std_dc_chrominance_nrcodes = new byte[] { 
            0, 0, 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 
            0, 0, 0, 0
         };
        this.std_dc_chrominance_values = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
        this.std_ac_chrominance_nrcodes = new byte[] { 
            0, 0, 2, 1, 2, 4, 4, 3, 4, 7, 5, 4, 4, 0, 1, 2, 
            0x77, 0, 0, 0
         };
        this.std_ac_chrominance_values = new byte[] { 
            0, 1, 2, 3, 0x11, 4, 5, 0x21, 0x31, 6, 0x12, 0x41, 0x51, 7, 0x61, 0x71, 
            0x13, 0x22, 50, 0x81, 8, 20, 0x42, 0x91, 0xa1, 0xb1, 0xc1, 9, 0x23, 0x33, 0x52, 240, 
            0x15, 0x62, 0x72, 0xd1, 10, 0x16, 0x24, 0x34, 0xe1, 0x25, 0xf1, 0x17, 0x18, 0x19, 0x1a, 0x26, 
            0x27, 40, 0x29, 0x2a, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3a, 0x43, 0x44, 0x45, 70, 0x47, 0x48, 
            0x49, 0x4a, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 90, 0x63, 100, 0x65, 0x66, 0x67, 0x68, 
            0x69, 0x6a, 0x73, 0x74, 0x75, 0x76, 0x77, 120, 0x79, 0x7a, 130, 0x83, 0x84, 0x85, 0x86, 0x87, 
            0x88, 0x89, 0x8a, 0x92, 0x93, 0x94, 0x95, 150, 0x97, 0x98, 0x99, 0x9a, 0xa2, 0xa3, 0xa4, 0xa5, 
            0xa6, 0xa7, 0xa8, 0xa9, 170, 0xb2, 0xb3, 180, 0xb5, 0xb6, 0xb7, 0xb8, 0xb9, 0xba, 0xc2, 0xc3, 
            0xc4, 0xc5, 0xc6, 0xc7, 200, 0xc9, 0xca, 210, 0xd3, 0xd4, 0xd5, 0xd6, 0xd7, 0xd8, 0xd9, 0xda, 
            0xe2, 0xe3, 0xe4, 0xe5, 230, 0xe7, 0xe8, 0xe9, 0xea, 0xf2, 0xf3, 0xf4, 0xf5, 0xf6, 0xf7, 0xf8, 
            0xf9, 250, 0, 0
         };
        this.bitcode = new BitString[0xffff];
        this.category = new int[0xffff];
        this.bytepos = 7;
        this.byteout = new ByteArray();
        this.DU = new int[0x40];
        this.YDU = new float[0x40];
        this.UDU = new float[0x40];
        this.VDU = new float[0x40];
        this.path = path;
        this.image = new BitmapData(texture);
        quality = Mathf.Clamp(quality, 1f, 100f);
        this.sf = (quality >= 50f) ? ((int) (200f - (quality * 2f))) : ((int) (5000f / quality));
        this.cores = SystemInfo.processorCount;
        Thread thread = new Thread(new ThreadStart(this.DoEncoding));
        thread.Start();
        if (blocking)
        {
            thread.Join();
        }
    }

    private BitString[] ComputeHuffmanTbl(byte[] nrcodes, byte[] std_table)
    {
        int num = 0;
        int index = 0;
        BitString[] strArray = new BitString[0x100];
        for (int i = 1; i <= 0x10; i++)
        {
            for (int j = 1; j <= nrcodes[i]; j++)
            {
                BitString str = new BitString();
                strArray[std_table[index]] = str;
                strArray[std_table[index]].value = num;
                strArray[std_table[index]].length = i;
                index++;
                num++;
            }
            num *= 2;
        }
        return strArray;
    }

    private void DoEncoding()
    {
        this.isDone = false;
        this.InitHuffmanTbl();
        this.InitCategoryfloat();
        this.InitQuantTables(this.sf);
        this.Encode();
        if (!string.IsNullOrEmpty(this.path))
        {
            File.WriteAllBytes(this.path, this.GetBytes());
        }
        this.isDone = true;
    }

    private void Encode()
    {
        this.byteout = new ByteArray();
        this.bytenew = 0;
        this.bytepos = 7;
        this.WriteWord(0xffd8);
        this.WriteAPP0();
        this.WriteDQT();
        this.WriteSOF0(this.image.width, this.image.height);
        this.WriteDHT();
        this.writeSOS();
        float dC = 0f;
        float num2 = 0f;
        float num3 = 0f;
        this.bytenew = 0;
        this.bytepos = 7;
        for (int i = 0; i < this.image.height; i += 8)
        {
            for (int j = 0; j < this.image.width; j += 8)
            {
                this.RGB2YUV(this.image, j, i);
                dC = this.ProcessDU(this.YDU, this.fdtbl_Y, dC, this.YDC_HT, this.YAC_HT);
                num2 = this.ProcessDU(this.UDU, this.fdtbl_UV, num2, this.UVDC_HT, this.UVAC_HT);
                num3 = this.ProcessDU(this.VDU, this.fdtbl_UV, num3, this.UVDC_HT, this.UVAC_HT);
                if (this.cores == 1)
                {
                    Thread.Sleep(0);
                }
            }
        }
        if (this.bytepos >= 0)
        {
            BitString bs = new BitString {
                length = this.bytepos + 1,
                value = (((int) 1) << (this.bytepos + 1)) - 1
            };
            this.WriteBits(bs);
        }
        this.WriteWord(0xffd9);
        this.isDone = true;
    }

    private float[] FDCTQuant(float[] data, float[] fdtbl)
    {
        float num;
        float num2;
        float num3;
        float num4;
        float num5;
        float num6;
        float num7;
        float num8;
        float num9;
        float num10;
        float num11;
        float num12;
        float num13;
        float num14;
        float num15;
        float num16;
        float num17;
        float num18;
        float num19;
        int num20;
        int index = 0;
        for (num20 = 0; num20 < 8; num20++)
        {
            num = data[index] + data[index + 7];
            num8 = data[index] - data[index + 7];
            num2 = data[index + 1] + data[index + 6];
            num7 = data[index + 1] - data[index + 6];
            num3 = data[index + 2] + data[index + 5];
            num6 = data[index + 2] - data[index + 5];
            num4 = data[index + 3] + data[index + 4];
            num5 = data[index + 3] - data[index + 4];
            num9 = num + num4;
            num12 = num - num4;
            num10 = num2 + num3;
            num11 = num2 - num3;
            data[index] = num9 + num10;
            data[index + 4] = num9 - num10;
            num13 = (num11 + num12) * 0.7071068f;
            data[index + 2] = num12 + num13;
            data[index + 6] = num12 - num13;
            num9 = num5 + num6;
            num10 = num6 + num7;
            num11 = num7 + num8;
            num17 = (num9 - num11) * 0.3826834f;
            num14 = (0.5411961f * num9) + num17;
            num16 = (1.306563f * num11) + num17;
            num15 = num10 * 0.7071068f;
            num18 = num8 + num15;
            num19 = num8 - num15;
            data[index + 5] = num19 + num14;
            data[index + 3] = num19 - num14;
            data[index + 1] = num18 + num16;
            data[index + 7] = num18 - num16;
            index += 8;
        }
        index = 0;
        for (num20 = 0; num20 < 8; num20++)
        {
            num = data[index] + data[index + 0x38];
            num8 = data[index] - data[index + 0x38];
            num2 = data[index + 8] + data[index + 0x30];
            num7 = data[index + 8] - data[index + 0x30];
            num3 = data[index + 0x10] + data[index + 40];
            num6 = data[index + 0x10] - data[index + 40];
            num4 = data[index + 0x18] + data[index + 0x20];
            num5 = data[index + 0x18] - data[index + 0x20];
            num9 = num + num4;
            num12 = num - num4;
            num10 = num2 + num3;
            num11 = num2 - num3;
            data[index] = num9 + num10;
            data[index + 0x20] = num9 - num10;
            num13 = (num11 + num12) * 0.7071068f;
            data[index + 0x10] = num12 + num13;
            data[index + 0x30] = num12 - num13;
            num9 = num5 + num6;
            num10 = num6 + num7;
            num11 = num7 + num8;
            num17 = (num9 - num11) * 0.3826834f;
            num14 = (0.5411961f * num9) + num17;
            num16 = (1.306563f * num11) + num17;
            num15 = num10 * 0.7071068f;
            num18 = num8 + num15;
            num19 = num8 - num15;
            data[index + 40] = num19 + num14;
            data[index + 0x18] = num19 - num14;
            data[index + 8] = num18 + num16;
            data[index + 0x38] = num18 - num16;
            index++;
        }
        for (num20 = 0; num20 < 0x40; num20++)
        {
            data[num20] = Mathf.Round(data[num20] * fdtbl[num20]);
        }
        return data;
    }

    public byte[] GetBytes()
    {
        if (!this.isDone)
        {
            Debug.LogError("JPEGEncoder not complete, cannot get bytes!");
            return null;
        }
        return this.byteout.GetAllBytes();
    }

    private void InitCategoryfloat()
    {
        int num = 1;
        int num2 = 2;
        for (int i = 1; i <= 15; i++)
        {
            BitString str;
            int num3 = num;
            while (num3 < num2)
            {
                this.category[0x7fff + num3] = i;
                str = new BitString {
                    length = i,
                    value = num3
                };
                this.bitcode[0x7fff + num3] = str;
                num3++;
            }
            for (num3 = -(num2 - 1); num3 <= -num; num3++)
            {
                this.category[0x7fff + num3] = i;
                str = new BitString {
                    length = i,
                    value = (num2 - 1) + num3
                };
                this.bitcode[0x7fff + num3] = str;
            }
            num = num << 1;
            num2 = num2 << 1;
        }
    }

    private void InitHuffmanTbl()
    {
        this.YDC_HT = this.ComputeHuffmanTbl(this.std_dc_luminance_nrcodes, this.std_dc_luminance_values);
        this.UVDC_HT = this.ComputeHuffmanTbl(this.std_dc_chrominance_nrcodes, this.std_dc_chrominance_values);
        this.YAC_HT = this.ComputeHuffmanTbl(this.std_ac_luminance_nrcodes, this.std_ac_luminance_values);
        this.UVAC_HT = this.ComputeHuffmanTbl(this.std_ac_chrominance_nrcodes, this.std_ac_chrominance_values);
    }

    private void InitQuantTables(int sf)
    {
        int num;
        float num2;
        int[] numArray = new int[] { 
            0x10, 11, 10, 0x10, 0x18, 40, 0x33, 0x3d, 12, 12, 14, 0x13, 0x1a, 0x3a, 60, 0x37, 
            14, 13, 0x10, 0x18, 40, 0x39, 0x45, 0x38, 14, 0x11, 0x16, 0x1d, 0x33, 0x57, 80, 0x3e, 
            0x12, 0x16, 0x25, 0x38, 0x44, 0x6d, 0x67, 0x4d, 0x18, 0x23, 0x37, 0x40, 0x51, 0x68, 0x71, 0x5c, 
            0x31, 0x40, 0x4e, 0x57, 0x67, 0x79, 120, 0x65, 0x48, 0x5c, 0x5f, 0x62, 0x70, 100, 0x67, 0x63
         };
        for (num = 0; num < 0x40; num++)
        {
            num2 = Mathf.Clamp(Mathf.Floor((float) (((numArray[num] * sf) + 50) / 100)), 1f, 255f);
            this.YTable[this.ZigZag[num]] = Mathf.RoundToInt(num2);
        }
        int[] numArray2 = new int[] { 
            0x11, 0x12, 0x18, 0x2f, 0x63, 0x63, 0x63, 0x63, 0x12, 0x15, 0x1a, 0x42, 0x63, 0x63, 0x63, 0x63, 
            0x18, 0x1a, 0x38, 0x63, 0x63, 0x63, 0x63, 0x63, 0x2f, 0x42, 0x63, 0x63, 0x63, 0x63, 0x63, 0x63, 
            0x63, 0x63, 0x63, 0x63, 0x63, 0x63, 0x63, 0x63, 0x63, 0x63, 0x63, 0x63, 0x63, 0x63, 0x63, 0x63, 
            0x63, 0x63, 0x63, 0x63, 0x63, 0x63, 0x63, 0x63, 0x63, 0x63, 0x63, 0x63, 0x63, 0x63, 0x63, 0x63
         };
        for (num = 0; num < 0x40; num++)
        {
            num2 = Mathf.Clamp(Mathf.Floor((float) (((numArray2[num] * sf) + 50) / 100)), 1f, 255f);
            this.UVTable[this.ZigZag[num]] = (int) num2;
        }
        float[] numArray3 = new float[] { 1f, 1.38704f, 1.306563f, 1.175876f, 1f, 0.785695f, 0.5411961f, 0.2758994f };
        num = 0;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                this.fdtbl_Y[num] = 1f / (((this.YTable[this.ZigZag[num]] * numArray3[i]) * numArray3[j]) * 8f);
                this.fdtbl_UV[num] = 1f / (((this.UVTable[this.ZigZag[num]] * numArray3[i]) * numArray3[j]) * 8f);
                num++;
            }
        }
    }

    private float ProcessDU(float[] CDU, float[] fdtbl, float DC, BitString[] HTDC, BitString[] HTAC)
    {
        int num;
        BitString bs = HTAC[0];
        BitString str2 = HTAC[240];
        float[] numArray = this.FDCTQuant(CDU, fdtbl);
        for (num = 0; num < 0x40; num++)
        {
            this.DU[this.ZigZag[num]] = (int) numArray[num];
        }
        int num2 = this.DU[0] - ((int) DC);
        DC = this.DU[0];
        if (num2 == 0)
        {
            this.WriteBits(HTDC[0]);
        }
        else
        {
            this.WriteBits(HTDC[this.category[0x7fff + num2]]);
            this.WriteBits(this.bitcode[0x7fff + num2]);
        }
        int index = 0x3f;
        while ((index > 0) && (this.DU[index] == 0))
        {
            index--;
        }
        if (index == 0)
        {
            this.WriteBits(bs);
            return DC;
        }
        for (num = 1; num <= index; num++)
        {
            int num4 = num;
            while ((this.DU[num] == 0) && (num <= index))
            {
                num++;
            }
            int num5 = num - num4;
            if (num5 >= 0x10)
            {
                for (int i = 1; i <= (num5 / 0x10); i++)
                {
                    this.WriteBits(str2);
                }
                num5 &= 15;
            }
            this.WriteBits(HTAC[(num5 * 0x10) + this.category[0x7fff + this.DU[num]]]);
            this.WriteBits(this.bitcode[0x7fff + this.DU[num]]);
        }
        if (index != 0x3f)
        {
            this.WriteBits(bs);
        }
        return DC;
    }

    private void RGB2YUV(BitmapData image, int xpos, int ypos)
    {
        int index = 0;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Color32 pixelColor = image.GetPixelColor(xpos + j, image.height - (ypos + i));
                this.YDU[index] = (((0.299f * pixelColor.r) + (0.587f * pixelColor.g)) + (0.114f * pixelColor.b)) - 128f;
                this.UDU[index] = ((-0.16874f * pixelColor.r) + (-0.33126f * pixelColor.g)) + (0.5f * pixelColor.b);
                this.VDU[index] = ((0.5f * pixelColor.r) + (-0.41869f * pixelColor.g)) + (-0.08131f * pixelColor.b);
                index++;
            }
        }
    }

    private void WriteAPP0()
    {
        this.WriteWord(0xffe0);
        this.WriteWord(0x10);
        this.WriteByte(0x4a);
        this.WriteByte(70);
        this.WriteByte(0x49);
        this.WriteByte(70);
        this.WriteByte(0);
        this.WriteByte(1);
        this.WriteByte(1);
        this.WriteByte(0);
        this.WriteWord(1);
        this.WriteWord(1);
        this.WriteByte(0);
        this.WriteByte(0);
    }

    private void WriteBits(BitString bs)
    {
        int num = bs.value;
        int num2 = bs.length - 1;
        while (num2 >= 0)
        {
            if ((num & Convert.ToUInt32((int) (((int) 1) << num2))) != 0)
            {
                this.bytenew |= Convert.ToUInt32((int) (((int) 1) << this.bytepos));
            }
            num2--;
            this.bytepos--;
            if (this.bytepos < 0)
            {
                if (this.bytenew == 0xff)
                {
                    this.WriteByte(0xff);
                    this.WriteByte(0);
                }
                else
                {
                    this.WriteByte((byte) this.bytenew);
                }
                this.bytepos = 7;
                this.bytenew = 0;
            }
        }
    }

    private void WriteByte(byte value)
    {
        this.byteout.WriteByte(value);
    }

    private void WriteDHT()
    {
        int num;
        this.WriteWord(0xffc4);
        this.WriteWord(0x1a2);
        this.WriteByte(0);
        for (num = 0; num < 0x10; num++)
        {
            this.WriteByte(this.std_dc_luminance_nrcodes[num + 1]);
        }
        for (num = 0; num <= 11; num++)
        {
            this.WriteByte(this.std_dc_luminance_values[num]);
        }
        this.WriteByte(0x10);
        for (num = 0; num < 0x10; num++)
        {
            this.WriteByte(this.std_ac_luminance_nrcodes[num + 1]);
        }
        for (num = 0; num <= 0xa1; num++)
        {
            this.WriteByte(this.std_ac_luminance_values[num]);
        }
        this.WriteByte(1);
        for (num = 0; num < 0x10; num++)
        {
            this.WriteByte(this.std_dc_chrominance_nrcodes[num + 1]);
        }
        for (num = 0; num <= 11; num++)
        {
            this.WriteByte(this.std_dc_chrominance_values[num]);
        }
        this.WriteByte(0x11);
        for (num = 0; num < 0x10; num++)
        {
            this.WriteByte(this.std_ac_chrominance_nrcodes[num + 1]);
        }
        for (num = 0; num <= 0xa1; num++)
        {
            this.WriteByte(this.std_ac_chrominance_values[num]);
        }
    }

    private void WriteDQT()
    {
        int num;
        this.WriteWord(0xffdb);
        this.WriteWord(0x84);
        this.WriteByte(0);
        for (num = 0; num < 0x40; num++)
        {
            this.WriteByte((byte) this.YTable[num]);
        }
        this.WriteByte(1);
        for (num = 0; num < 0x40; num++)
        {
            this.WriteByte((byte) this.UVTable[num]);
        }
    }

    private void WriteSOF0(int width, int height)
    {
        this.WriteWord(0xffc0);
        this.WriteWord(0x11);
        this.WriteByte(8);
        this.WriteWord(height);
        this.WriteWord(width);
        this.WriteByte(3);
        this.WriteByte(1);
        this.WriteByte(0x11);
        this.WriteByte(0);
        this.WriteByte(2);
        this.WriteByte(0x11);
        this.WriteByte(1);
        this.WriteByte(3);
        this.WriteByte(0x11);
        this.WriteByte(1);
    }

    private void writeSOS()
    {
        this.WriteWord(0xffda);
        this.WriteWord(12);
        this.WriteByte(3);
        this.WriteByte(1);
        this.WriteByte(0);
        this.WriteByte(2);
        this.WriteByte(0x11);
        this.WriteByte(3);
        this.WriteByte(0x11);
        this.WriteByte(0);
        this.WriteByte(0x3f);
        this.WriteByte(0);
    }

    private void WriteWord(int value)
    {
        this.WriteByte((byte) ((value >> 8) & 0xff));
        this.WriteByte((byte) (value & 0xff));
    }

    private class BitmapData
    {
        public int height;
        private Color32[] pixels;
        public int width;

        public BitmapData(Texture2D texture)
        {
            this.height = texture.height;
            this.width = texture.width;
            this.pixels = texture.GetPixels32();
        }

        public Color32 GetPixelColor(int x, int y)
        {
            x = Mathf.Clamp(x, 0, this.width - 1);
            y = Mathf.Clamp(y, 0, this.height - 1);
            return this.pixels[(y * this.width) + x];
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct BitString
    {
        public int length;
        public int value;
    }

    private class ByteArray
    {
        private MemoryStream stream = new MemoryStream();
        private BinaryWriter writer;

        public ByteArray()
        {
            this.writer = new BinaryWriter(this.stream);
        }

        public byte[] GetAllBytes()
        {
            byte[] buffer = new byte[this.stream.Length];
            this.stream.Position = 0L;
            this.stream.Read(buffer, 0, buffer.Length);
            return buffer;
        }

        public void WriteByte(byte value)
        {
            this.writer.Write(value);
        }
    }
}

