using Facepunch.Hash;
using System;

public sealed class IngredientList<DB> : IngredientList, IEquatable<IngredientList<DB>> where DB: Datablock, IComparable<DB>
{
    private uint hash;
    private string lastToString;
    private bool madeHashCode;
    private DB[] sorted;
    public DB[] unsorted;

    public IngredientList(DB[] unsorted)
    {
        if (unsorted == null)
        {
        }
        this.unsorted = new DB[0];
        if (unsorted.Length > 0xff)
        {
            throw new ArgumentException("items in list cannot exceed 255");
        }
        this.sorted = null;
        this.lastToString = null;
    }

    private static string Combine(DB[] dbs)
    {
        string[] strArray = new string[dbs.Length];
        for (int i = 0; i < dbs.Length; i++)
        {
            strArray[i] = Convert.ToString((bool) dbs[i]);
        }
        return string.Join(",", strArray);
    }

    public IngredientList<DB> EnsureContents(DB[] original)
    {
        if (this.unsorted != original)
        {
            this.sorted = null;
            this.lastToString = null;
            this.madeHashCode = false;
            this.unsorted = original;
        }
        return (IngredientList<DB>) this;
    }

    public override bool Equals(object obj)
    {
        return ((obj is IngredientList<DB>) && this.Equals((IngredientList<DB>) obj));
    }

    public bool Equals(IngredientList<DB> other)
    {
        if (!object.ReferenceEquals(other, this))
        {
            if (((other == null) || (this.unsorted.Length != other.unsorted.Length)) || (this.hashCode != other.hashCode))
            {
                return false;
            }
            DB[] sorted = this.sorted;
            DB[] localArray2 = other.sorted;
            int index = 0;
            int length = sorted.Length;
            while (index < length)
            {
                if (sorted[index] != localArray2[index])
                {
                    return false;
                }
                if (--length <= index)
                {
                    break;
                }
                if (sorted[length] != localArray2[length])
                {
                    return false;
                }
                index++;
            }
        }
        return true;
    }

    public override int GetHashCode()
    {
        return (int) this.hashCode;
    }

    private void ReSort()
    {
        int length = this.unsorted.Length;
        Array.Resize<DB>(ref this.sorted, length);
        Array.Copy(this.unsorted, this.sorted, length);
        if (length > 0xff)
        {
            throw new InvalidOperationException("There can't be more than 255 ingredients per blueprint");
        }
        Array.Sort<DB>(this.sorted);
    }

    public override string ToString()
    {
        object[] args = new object[] { typeof(DB).Name, this.unsorted.Length, this.hashCode, this.text };
        return string.Format("[IngredientList<{0}>[{1}]{2:X}:{3}]", args);
    }

    public uint hashCode
    {
        get
        {
            DB[] ordered;
            if (!this.madeHashCode)
            {
                ordered = this.ordered;
            }
            else if (this.needReSort)
            {
                this.ReSort();
                ordered = this.sorted;
            }
            else
            {
                return this.hash;
            }
            int length = ordered.Length;
            if (length > IngredientList.tempHash.Length)
            {
                Array.Resize<int>(ref IngredientList.tempHash, length);
            }
            for (int i = 0; i < length; i++)
            {
                IngredientList.tempHash[i] = ordered[i].uniqueID;
            }
            this.hash = MurmurHash2.UINT(IngredientList.tempHash, length, 0xf00dfeed);
            this.madeHashCode = true;
            return this.hash;
        }
    }

    private bool needReSort
    {
        get
        {
            if ((this.sorted != null) && (this.sorted.Length == this.unsorted.Length))
            {
                return false;
            }
            return true;
        }
    }

    public DB[] ordered
    {
        get
        {
            if (this.needReSort)
            {
                this.ReSort();
            }
            return this.sorted;
        }
    }

    public string text
    {
        get
        {
            if (this.lastToString == null)
            {
            }
            return (this.lastToString = IngredientList<DB>.Combine(this.ordered));
        }
    }
}

