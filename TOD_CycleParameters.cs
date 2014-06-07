using System;
using UnityEngine;

[Serializable]
public class TOD_CycleParameters
{
    public int Day = 1;
    public float Hour = 12f;
    public float Latitude;
    public float Longitude;
    public int Month = 3;
    public float MoonPhase;
    public float UTC;
    public int Year = 0x7d0;

    public void CheckRange()
    {
        this.Year = Mathf.Clamp(this.Year, 1, 0x270f);
        this.Month = Mathf.Clamp(this.Month, 1, 12);
        this.Day = Mathf.Clamp(this.Day, 1, System.DateTime.DaysInMonth(this.Year, this.Month));
        this.Hour = Mathf.Repeat(this.Hour, 24f);
        this.Longitude = Mathf.Clamp(this.Longitude, -180f, 180f);
        this.Latitude = Mathf.Clamp(this.Latitude, -90f, 90f);
        this.MoonPhase = Mathf.Clamp(this.MoonPhase, -1f, 1f);
    }

    public System.DateTime DateTime
    {
        get
        {
            this.CheckRange();
            int hour = (int) this.Hour;
            float num2 = (this.Hour - hour) * 60f;
            int minute = (int) num2;
            float num4 = (num2 - minute) * 60f;
            return new System.DateTime(this.Year, this.Month, this.Day, hour, minute, (int) num4);
        }
        set
        {
            this.Year = value.Year;
            this.Month = value.Month;
            this.Day = value.Day;
            this.Hour = (value.Hour + (((float) value.Minute) / 60f)) + (((float) value.Second) / 3600f);
        }
    }

    public long Ticks
    {
        get
        {
            return this.DateTime.Ticks;
        }
        set
        {
            this.DateTime = new System.DateTime(value);
        }
    }
}

