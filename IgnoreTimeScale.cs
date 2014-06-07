using System;
using UnityEngine;

[AddComponentMenu("NGUI/Internal/Ignore TimeScale Behaviour")]
public class IgnoreTimeScale : MonoBehaviour
{
    private float mActual;
    private float mTimeDelta;
    private float mTimeStart;
    private bool mTimeStarted;

    private void OnEnable()
    {
        this.mTimeStarted = true;
        this.mTimeDelta = 0f;
        this.mTimeStart = Time.realtimeSinceStartup;
    }

    protected float UpdateRealTimeDelta()
    {
        if (this.mTimeStarted)
        {
            float realtimeSinceStartup = Time.realtimeSinceStartup;
            float b = realtimeSinceStartup - this.mTimeStart;
            this.mActual += Mathf.Max(0f, b);
            this.mTimeDelta = 0.001f * Mathf.Round(this.mActual * 1000f);
            this.mActual -= this.mTimeDelta;
            this.mTimeStart = realtimeSinceStartup;
        }
        else
        {
            this.mTimeStarted = true;
            this.mTimeStart = Time.realtimeSinceStartup;
            this.mTimeDelta = 0f;
        }
        return this.mTimeDelta;
    }

    public float realTimeDelta
    {
        get
        {
            return this.mTimeDelta;
        }
    }
}

