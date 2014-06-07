using System;
using UnityEngine;

internal interface IIDLocalInterpolator
{
    void SetGoals(Vector3 pos, Quaternion rot, double timestamp);

    IDMain idMain { get; }

    IDLocal self { get; }
}

