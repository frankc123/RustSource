using System;
using UnityEngine;

public class CharacterInterpolatorTrait : CharacterTrait
{
    [SerializeField]
    private float _allowableTimeSpan = 0.1f;
    [SerializeField]
    private bool _allowExtrapolation;
    [SerializeField]
    private int _bufferCapacity = -1;
    [SerializeField]
    private string _interpolatorComponentTypeName;

    public virtual Interpolator AddInterpolator(IDMain main)
    {
        if (!string.IsNullOrEmpty(this._interpolatorComponentTypeName))
        {
            Component component = main.gameObject.AddComponent(this._interpolatorComponentTypeName);
            Interpolator interpolator = component as Interpolator;
            if (interpolator != null)
            {
                interpolator.idMain = main;
                return interpolator;
            }
            Debug.LogError(this._interpolatorComponentTypeName + " is not a interpolator");
            Object.Destroy(component);
        }
        return null;
    }

    public float allowableTimeSpan
    {
        get
        {
            return this._allowableTimeSpan;
        }
    }

    public bool allowExtrapolation
    {
        get
        {
            return this._allowExtrapolation;
        }
    }

    public int bufferCapacity
    {
        get
        {
            return this._bufferCapacity;
        }
    }

    public string interpolatorComponentTypeName
    {
        get
        {
            return this._interpolatorComponentTypeName;
        }
    }
}

