using System;
using System.Diagnostics;
using UnityEngine;

public class ServerQuitResponder : MonoBehaviour
{
    [Conditional("ALLOW_SQR")]
    public static void WillChangeLevels()
    {
    }
}

