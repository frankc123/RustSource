using System;
using System.Collections.Generic;
using UnityEngine;

public class Radiation : IDLocalCharacter
{
    [NonSerialized]
    private List<RadiationZone> radiationZones;

    public void AddRadiationZone(RadiationZone zone)
    {
        if (zone.CanAddToRadiation(this))
        {
            if (this.radiationZones == null)
            {
            }
            (this.radiationZones = new List<RadiationZone>()).Add(zone);
        }
    }

    public float CalculateExposure(bool countArmor)
    {
        if ((this.radiationZones == null) || (this.radiationZones.Count == 0))
        {
            return 0f;
        }
        Vector3 origin = base.origin;
        float num = 0f;
        foreach (RadiationZone zone in this.radiationZones)
        {
            num += zone.GetExposureForPos(origin);
        }
        if (countArmor)
        {
            HumanBodyTakeDamage takeDamage = base.takeDamage as HumanBodyTakeDamage;
            if (takeDamage != null)
            {
                float armorValue = takeDamage.GetArmorValue(4);
                if (armorValue > 0f)
                {
                    num *= 1f - Mathf.Clamp((float) (armorValue / 200f), (float) 0f, (float) 1f);
                }
            }
        }
        return num;
    }

    public float GetRadExposureScalar(float exposure)
    {
        return Mathf.Clamp01(exposure / 1000f);
    }

    private void OnDestroy()
    {
        if (this.radiationZones != null)
        {
            foreach (RadiationZone zone in this.radiationZones)
            {
                if (zone != null)
                {
                    zone.RemoveFromRadiation(this);
                }
            }
            this.radiationZones = null;
        }
    }

    public void RemoveRadiationZone(RadiationZone zone)
    {
        if ((this.radiationZones != null) && this.radiationZones.Remove(zone))
        {
            zone.RemoveFromRadiation(this);
        }
    }
}

