using System;

[Serializable]
public class CullGridSetup
{
    public int cellSquareDimension;
    public int cellsTall;
    public int cellsWide;
    public int[] gatheringCellsBits;
    public int gatheringCellsCenter;
    public int gatheringCellsTall;
    public int gatheringCellsWide;
    public int groupBegin;

    public CullGridSetup()
    {
        this.cellSquareDimension = 200;
        this.cellsWide = 80;
        this.cellsTall = 80;
        this.groupBegin = 100;
        this.gatheringCellsWide = 3;
        this.gatheringCellsTall = 3;
        this.gatheringCellsCenter = 4;
        this.gatheringCellsBits = new int[] { -8193, -101897 };
    }

    protected CullGridSetup(CullGridSetup copyFrom)
    {
        this.cellSquareDimension = copyFrom.cellSquareDimension;
        this.cellsWide = copyFrom.cellsWide;
        this.cellsTall = copyFrom.cellsTall;
        this.groupBegin = copyFrom.groupBegin;
        this.gatheringCellsWide = copyFrom.gatheringCellsWide;
        this.gatheringCellsTall = copyFrom.gatheringCellsTall;
        this.gatheringCellsCenter = copyFrom.gatheringCellsCenter;
        this.gatheringCellsBits = (int[]) copyFrom.gatheringCellsBits.Clone();
    }

    public bool GetGatheringBit(int x, int y)
    {
        if ((x >= this.gatheringCellsWide) || (x < 0))
        {
            throw new ArgumentOutOfRangeException("x", "must be < gatheringCellsWide && >= 0");
        }
        if ((y >= this.gatheringCellsTall) || (y < 0))
        {
            throw new ArgumentOutOfRangeException("y", "must be < gatheringCellsTall && >= 0");
        }
        int num = (y * this.gatheringCellsWide) + x;
        int index = num / 0x20;
        int num3 = num % 0x20;
        return ((this.gatheringCellsBits == null) || ((this.gatheringCellsBits.Length <= index) || ((this.gatheringCellsBits[index] & (((int) 1) << num3)) == (((int) 1) << num3))));
    }

    public void SetGatheringBit(int x, int y, bool v)
    {
        if ((x >= this.gatheringCellsWide) || (x < 0))
        {
            throw new ArgumentOutOfRangeException("x", "must be < gatheringCellsWide && >= 0");
        }
        if ((y >= this.gatheringCellsTall) || (y < 0))
        {
            throw new ArgumentOutOfRangeException("y", "must be < gatheringCellsTall && >= 0");
        }
        int num = (y * this.gatheringCellsWide) + x;
        int index = num / 0x20;
        int num3 = num % 0x20;
        if (this.gatheringCellsBits == null)
        {
            Array.Resize<int>(ref this.gatheringCellsBits, index + 1);
            for (int i = 0; i < index; i++)
            {
                this.gatheringCellsBits[i] = -1;
            }
            if (!v)
            {
                this.gatheringCellsBits[index] = ~(((int) 1) << num3);
            }
            else
            {
                this.gatheringCellsBits[index] = -1;
            }
        }
        else if (this.gatheringCellsBits.Length <= index)
        {
            int length = this.gatheringCellsBits.Length;
            Array.Resize<int>(ref this.gatheringCellsBits, index + 1);
            for (int j = length + 1; j <= index; j++)
            {
                this.gatheringCellsBits[j] = -1;
            }
            if (!v)
            {
                this.gatheringCellsBits[index] &= ~(((int) 1) << num3);
            }
        }
        else if (v)
        {
            this.gatheringCellsBits[index] |= ((int) 1) << num3;
        }
        else
        {
            this.gatheringCellsBits[index] &= ~(((int) 1) << num3);
        }
    }

    public void SetGatheringDimensions(int gatheringCellsWide, int gatheringCellsTall)
    {
        if ((this.gatheringCellsWide != gatheringCellsWide) || (this.gatheringCellsTall != gatheringCellsTall))
        {
            this.gatheringCellsWide = gatheringCellsWide;
            this.gatheringCellsTall = gatheringCellsTall;
            this.gatheringCellsCenter = (this.gatheringCellsWide / 2) + ((this.gatheringCellsTall / 2) * this.gatheringCellsWide);
        }
    }

    public void ToggleGatheringBit(int x, int y)
    {
        if ((x >= this.gatheringCellsWide) || (x < 0))
        {
            throw new ArgumentOutOfRangeException("x", "must be < gatheringCellsWide && >= 0");
        }
        if ((y >= this.gatheringCellsTall) || (y < 0))
        {
            throw new ArgumentOutOfRangeException("y", "must be < gatheringCellsTall && >= 0");
        }
        int num = (y * this.gatheringCellsWide) + x;
        int index = num / 0x20;
        int num3 = num % 0x20;
        if (this.gatheringCellsBits == null)
        {
            Array.Resize<int>(ref this.gatheringCellsBits, index + 1);
            for (int i = 0; i < index; i++)
            {
                this.gatheringCellsBits[i] = -1;
            }
            this.gatheringCellsBits[index] = ~(((int) 1) << num3);
        }
        else if (this.gatheringCellsBits.Length <= index)
        {
            int length = this.gatheringCellsBits.Length;
            Array.Resize<int>(ref this.gatheringCellsBits, index + 1);
            for (int j = length + 1; j < index; j++)
            {
                this.gatheringCellsBits[j] = -1;
            }
            this.gatheringCellsBits[index] = ~(((int) 1) << num3);
        }
        else
        {
            this.gatheringCellsBits[index] ^= ((int) 1) << num3;
        }
    }
}

