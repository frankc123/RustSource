using System;
using UnityEngine;

[Serializable]
public class dfAnchorMargins
{
    [SerializeField]
    public float bottom;
    [SerializeField]
    public float left;
    [SerializeField]
    public float right;
    [SerializeField]
    public float top;

    public override string ToString()
    {
        object[] args = new object[] { this.left, this.top, this.right, this.bottom };
        return string.Format("[L:{0},T:{1},R:{2},B:{3}]", args);
    }
}

