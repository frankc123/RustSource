using Facepunch;
using System;
using uLink;
using UnityEngine;

public class EnvironmentControlCenter : NetBehaviour
{
    public static EnvironmentControlCenter Singleton;
    private TOD_Sky sky;

    protected void Awake()
    {
        Singleton = this;
    }

    [RPC]
    private void CL_UpdateSkyState(BitStream stream)
    {
        env.daylength = stream.Read<float>(new object[0]);
        env.nightlength = stream.Read<float>(new object[0]);
        this.sky.Cycle.MoonPhase = stream.Read<float>(new object[0]);
        this.sky.Components.Animation.CloudUV = stream.Read<Vector4>(new object[0]);
        this.sky.Cycle.Year = stream.Read<int>(new object[0]);
        this.sky.Cycle.Month = stream.Read<byte>(new object[0]);
        this.sky.Cycle.Day = stream.Read<byte>(new object[0]);
        float num = stream.Read<float>(new object[0]);
        if ((Mathf.Abs((float) (this.sky.Cycle.Hour - num)) > 0.01666667f) && (this.sky.Cycle.Hour > 0.05f))
        {
            this.sky.Cycle.Hour = num;
        }
    }

    public float GetTime()
    {
        if (this.sky == null)
        {
            return 0f;
        }
        return this.sky.Cycle.Hour;
    }

    public bool IsNight()
    {
        if (this.sky == null)
        {
            return false;
        }
        return this.sky.IsNight;
    }

    private void OnDestroy()
    {
        if (Singleton == this)
        {
            Singleton = null;
        }
    }

    protected void Update()
    {
        if (this.sky == null)
        {
            this.sky = (TOD_Sky) Object.FindObjectOfType(typeof(TOD_Sky));
            if (this.sky == null)
            {
                return;
            }
        }
        float num = env.daylength * 60f;
        if (this.sky.IsNight)
        {
            num = env.nightlength * 60f;
        }
        float num2 = num / 24f;
        float num3 = Time.deltaTime / num2;
        float num4 = (Time.deltaTime / (30f * num)) * 2f;
        this.sky.Cycle.Hour += num3;
        this.sky.Cycle.MoonPhase += num4;
        if (this.sky.Cycle.MoonPhase < -1f)
        {
            this.sky.Cycle.MoonPhase += 2f;
        }
        else if (this.sky.Cycle.MoonPhase > 1f)
        {
            this.sky.Cycle.MoonPhase -= 2f;
        }
        if (this.sky.Cycle.Hour >= 24f)
        {
            this.sky.Cycle.Hour = 0f;
            int num5 = DateTime.DaysInMonth(this.sky.Cycle.Year, this.sky.Cycle.Month);
            if (++this.sky.Cycle.Day > num5)
            {
                this.sky.Cycle.Day = 1;
                if (++this.sky.Cycle.Month > 12)
                {
                    this.sky.Cycle.Month = 1;
                    this.sky.Cycle.Year++;
                }
            }
        }
    }
}

