using System;
using UnityEngine;

public class BounceArrow : MonoBehaviour
{
    private void Update()
    {
        float y = 0f + (Mathf.Abs(Mathf.Sin(Time.time * 5f)) * 0.15f);
        base.transform.localPosition = new Vector3(0f, y, 0f);
    }
}

