using System;
using UnityEngine;

public class TOD_Time : MonoBehaviour
{
    public float DayLengthInMinutes = 30f;
    public bool ProgressDate = true;
    public bool ProgressMoonPhase = true;
    private TOD_Sky sky;

    protected void Start()
    {
        this.sky = base.GetComponent<TOD_Sky>();
    }

    protected void Update()
    {
        float num = this.DayLengthInMinutes * 60f;
        float num2 = num / 24f;
        float num3 = Time.deltaTime / num2;
        float num4 = (Time.deltaTime / (30f * num)) * 2f;
        this.sky.Cycle.Hour += num3;
        if (this.ProgressMoonPhase)
        {
            this.sky.Cycle.MoonPhase += num4;
            if (this.sky.Cycle.MoonPhase < -1f)
            {
                this.sky.Cycle.MoonPhase += 2f;
            }
            else if (this.sky.Cycle.MoonPhase > 1f)
            {
                this.sky.Cycle.MoonPhase -= 2f;
            }
        }
        if (this.sky.Cycle.Hour >= 24f)
        {
            this.sky.Cycle.Hour = 0f;
            if (this.ProgressDate)
            {
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
}

