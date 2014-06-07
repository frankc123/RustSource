using System;
using UnityEngine;

public class CameraSettings : MonoBehaviour
{
    public ViewDistanceLayer[] ViewDistanceLayers;

    protected void Awake()
    {
        GameEvent.QualitySettingsRefresh += new GameEvent.OnGenericEvent(this.RefreshSettings);
    }

    protected void OnDestroy()
    {
        GameEvent.QualitySettingsRefresh -= new GameEvent.OnGenericEvent(this.RefreshSettings);
    }

    private void RefreshSettings()
    {
        CameraLayerDepths component = base.GetComponent<CameraLayerDepths>();
        if (component != null)
        {
            foreach (ViewDistanceLayer layer in this.ViewDistanceLayers)
            {
                component[layer.Index] = layer.MinimumValue + (render.distance * layer.Range);
            }
        }
    }

    [Serializable]
    public class ViewDistanceLayer
    {
        public float MaximumValue;
        public float MinimumValue;
        public string Name;

        public int Index
        {
            get
            {
                return LayerMask.NameToLayer(this.Name);
            }
        }

        public float Range
        {
            get
            {
                return (this.MaximumValue - this.MinimumValue);
            }
        }
    }
}

