using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;

public class Assert
{
    [Conditional("UNITY_EDITOR")]
    public static void Test(bool comparison, string message = "")
    {
        if (comparison)
        {
        }
    }

    [Conditional("UNITY_EDITOR")]
    public static void Throw(string message = "")
    {
        Debug.LogError(message);
        Debug.Break();
    }
}

