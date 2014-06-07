using System;
using System.Reflection;
using UnityEngine;

public class VisEval : ScriptableObject
{
    [SerializeField]
    private int[] data;
    [NonSerialized]
    private bool expanded;
    [NonSerialized]
    private Vis.Rule[] rules;

    public bool EditorOnly_Clear()
    {
        if (this.data != null)
        {
            this.data = null;
            return true;
        }
        return false;
    }

    public bool EditorOnly_Clone(int index)
    {
        if ((index < 0) || (index >= this.ruleCount))
        {
            return false;
        }
        this.EditorOnly_New();
        for (int i = this.ruleCount - 1; i > index; i--)
        {
            int num2 = i * 4;
            int num3 = (i - 1) * 4;
            for (int j = 0; j < 4; j++)
            {
                this.data[num2] = this.data[i];
                num2++;
                num3++;
            }
        }
        return true;
    }

    public bool EditorOnly_Delete(int index)
    {
        if ((index < 0) || (index >= this.ruleCount))
        {
            return false;
        }
        for (int i = index; i < (this.ruleCount - 1); i++)
        {
            int num2 = i * 4;
            int num3 = (i + 1) * 4;
            for (int j = 0; j < 4; j++)
            {
                this.data[num2] = this.data[i];
                num2++;
                num3++;
            }
        }
        if (this.ruleCount == 1)
        {
            this.data = null;
        }
        else
        {
            Array.Resize<int>(ref this.data, this.data.Length - 4);
        }
        return true;
    }

    public bool EditorOnly_MoveBottom(int index)
    {
        if (!this.EditorOnly_MoveUp(index--))
        {
            return false;
        }
        while (this.EditorOnly_MoveUp(index--))
        {
        }
        return true;
    }

    public bool EditorOnly_MoveDown(int index)
    {
        if (index >= (this.ruleCount - 1))
        {
            return false;
        }
        this.Swap((index + 1) * 4, index * 4);
        return true;
    }

    public bool EditorOnly_MoveTop(int index)
    {
        if (!this.EditorOnly_MoveUp(index--))
        {
            return false;
        }
        while (this.EditorOnly_MoveUp(index--))
        {
        }
        return true;
    }

    public bool EditorOnly_MoveUp(int index)
    {
        if (index == 0)
        {
            return false;
        }
        if (index >= this.ruleCount)
        {
            return false;
        }
        this.Swap((index - 1) * 4, index * 4);
        return true;
    }

    public bool EditorOnly_New()
    {
        Array.Resize<int>(ref this.data, this.dataCount + 4);
        return true;
    }

    public bool GetMessage(Vis.Mask current, ref Vis.Mask previous, Vis.Mask other)
    {
        return false;
    }

    public bool Pass(Vis.Mask self, Vis.Mask instigator)
    {
        if (!this.expanded)
        {
            int ruleCount = this.ruleCount;
            if (ruleCount <= 0)
            {
                return true;
            }
            this.rules = new Vis.Rule[ruleCount];
            for (int j = 0; j < ruleCount; j++)
            {
                this.rules[j] = Vis.Rule.Decode(this.data, j * 4);
            }
            this.expanded = true;
        }
        for (int i = this.rules.Length - 1; i >= 0; i--)
        {
            if (this.rules[i].Pass(self, instigator) != Vis.Rule.Failure.None)
            {
                return false;
            }
        }
        return true;
    }

    private void Swap(int i, int j)
    {
        int num = this.data[j];
        this.data[j++] = this.data[i];
        this.data[i++] = num;
        num = this.data[j];
        this.data[j++] = this.data[i];
        this.data[i++] = num;
        num = this.data[j];
        this.data[j++] = this.data[i];
        this.data[i++] = num;
    }

    private int dataCount
    {
        get
        {
            return ((this.data != null) ? this.data.Length : 0);
        }
    }

    public Vis.Rule this[int i]
    {
        get
        {
            return Vis.Rule.Decode(this.data, i * 4);
        }
        set
        {
            Vis.Rule.Encode(ref value, this.data, i * 4);
            if (this.expanded)
            {
                this.rules[i] = value;
            }
        }
    }

    public int ruleCount
    {
        get
        {
            return (this.dataCount / 4);
        }
    }
}

