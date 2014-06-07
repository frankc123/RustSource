using System;
using UnityEngine;

public class TestFlagsScript : MonoBehaviour
{
    public E1 flags;

    [Flags]
    public enum E1
    {
        bit1 = 1,
        bit3 = 4,
        bit5 = 0x10
    }
}

