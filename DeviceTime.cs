using System;
using UnityEngine;

public class DeviceTime : MonoBehaviour
{
    public TOD_Sky sky;

    protected void OnEnable()
    {
        if (this.sky == null)
        {
            Debug.LogError("Sky instance reference not set. Disabling script.");
            base.enabled = false;
        }
        else
        {
            DateTime now = DateTime.Now;
            this.sky.Cycle.Year = now.Year;
            this.sky.Cycle.Month = now.Month;
            this.sky.Cycle.Day = now.Day;
            this.sky.Cycle.Hour = now.Hour + (((float) now.Minute) / 60f);
        }
    }
}

