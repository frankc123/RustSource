using System;
using System.Reflection;
using UnityEngine;

[Serializable]
public sealed class DamageTypeList
{
    [SerializeField]
    private float[] damageArray;
    private const int kDamageIndexCount = 6;

    public DamageTypeList()
    {
    }

    public DamageTypeList(DamageTypeList copyFrom) : this()
    {
        if ((copyFrom == null) || (copyFrom.damageArray == null))
        {
            this.damageArray = new float[6];
        }
        else if (copyFrom.damageArray.Length == 6)
        {
            this.damageArray = (float[]) copyFrom.damageArray.Clone();
        }
        else
        {
            this.damageArray = new float[6];
            if (copyFrom.damageArray.Length > 6)
            {
                for (int i = 0; i < 6; i++)
                {
                    this.damageArray[i] = copyFrom.damageArray[i];
                }
            }
            else
            {
                for (int j = 0; j < copyFrom.damageArray.Length; j++)
                {
                    this.damageArray[j] = copyFrom.damageArray[j];
                }
            }
        }
    }

    public DamageTypeList(float generic, float bullet, float melee, float explosion, float radiation, float cold)
    {
        this.damageArray = new float[] { generic, bullet, melee, explosion, radiation, cold };
    }

    public void SetArmorValues(DamageTypeList copyFrom)
    {
        if ((this.damageArray == null) || (this.damageArray.Length != 6))
        {
            if ((copyFrom == null) || (copyFrom.damageArray == null))
            {
                this.damageArray = new float[6];
            }
            else if (copyFrom.damageArray.Length == 6)
            {
                this.damageArray = (float[]) copyFrom.damageArray.Clone();
            }
            else
            {
                this.damageArray = new float[6];
                if (copyFrom.damageArray.Length > 6)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        this.damageArray[i] = copyFrom.damageArray[i];
                    }
                }
                else
                {
                    for (int j = 0; j < copyFrom.damageArray.Length; j++)
                    {
                        this.damageArray[j] = copyFrom.damageArray[j];
                    }
                }
            }
        }
        else if (copyFrom.damageArray == null)
        {
            if ((this.damageArray == null) || (this.damageArray.Length != 6))
            {
                this.damageArray = new float[6];
            }
            else
            {
                for (int k = 0; k < 6; k++)
                {
                    this.damageArray[k] = 0f;
                }
            }
        }
        else if (copyFrom.damageArray.Length >= 6)
        {
            for (int m = 0; m < 6; m++)
            {
                this.damageArray[m] = copyFrom.damageArray[m];
            }
        }
        else
        {
            int index = 0;
            while (index < copyFrom.damageArray.Length)
            {
                this.damageArray[index] = copyFrom.damageArray[index];
                index++;
            }
            while (index < 6)
            {
                this.damageArray[index++] = 0f;
            }
        }
    }

    public float this[int index]
    {
        get
        {
            if ((index < 0) || (index >= 6))
            {
                throw new IndexOutOfRangeException();
            }
            return (((this.damageArray != null) && (this.damageArray.Length > index)) ? this.damageArray[index] : 0f);
        }
        set
        {
            if ((index < 0) || (index >= 6))
            {
                throw new IndexOutOfRangeException();
            }
            if ((this.damageArray == null) || (this.damageArray.Length <= index))
            {
                Array.Resize<float>(ref this.damageArray, 6);
            }
            this.damageArray[index] = value;
        }
    }

    public float this[DamageTypeIndex index]
    {
        get
        {
            return this[(int) index];
        }
        set
        {
            this[(int) index] = value;
        }
    }
}

