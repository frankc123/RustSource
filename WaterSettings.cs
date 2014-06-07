using System;
using UnityEngine;

public class WaterSettings : MonoBehaviour
{
    private void OnDestroy()
    {
        GameEvent.QualitySettingsRefresh -= new GameEvent.OnGenericEvent(this.RefreshSettings);
    }

    protected void RefreshSettings()
    {
        WaterBase component = base.GetComponent<WaterBase>();
        PlanarReflection reflection = base.GetComponent<PlanarReflection>();
        if (component != null)
        {
            if (render.level > 0.8f)
            {
                component.waterQuality = WaterQuality.High;
                component.edgeBlend = true;
            }
            else if (render.level > 0.5f)
            {
                component.waterQuality = WaterQuality.Medium;
                component.edgeBlend = false;
            }
            else
            {
                component.waterQuality = WaterQuality.Low;
                component.edgeBlend = false;
            }
            if (water.level != -1)
            {
                component.waterQuality = (WaterQuality) Mathf.Clamp(water.level - 1, 0, 2);
                component.edgeBlend = water.level == 2;
            }
            if (reflection != null)
            {
                reflection.reflectionMask = 0xc81000;
                if (!water.reflection)
                {
                    reflection.reflectionMask = 0x800000;
                }
            }
        }
    }

    private void Start()
    {
        GameEvent.QualitySettingsRefresh += new GameEvent.OnGenericEvent(this.RefreshSettings);
        this.RefreshSettings();
    }
}

