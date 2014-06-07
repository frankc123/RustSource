using System;
using UnityEngine;

public class CharacterCrouchTrait : CharacterTrait
{
    [SerializeField]
    private AnimationCurve _crouchCurve;
    [SerializeField]
    private float _crouchToSpeedFraction;
    [SerializeField]
    private float _maxCrouchFraction;

    public CharacterCrouchTrait()
    {
        Keyframe[] keys = new Keyframe[] { new Keyframe(0f, 0f, 0f, 0f), new Keyframe(0.55f, -0.55f, 0f, 0f) };
        this._crouchCurve = new AnimationCurve(keys);
        this._crouchToSpeedFraction = 1.3f;
        this._maxCrouchFraction = 0.9f;
    }

    public bool IsCrouching(float minHeight, float maxHeight, float currentHeight)
    {
        return (Mathf.InverseLerp(minHeight, maxHeight, currentHeight) <= this._maxCrouchFraction);
    }

    public AnimationCurve crouchCurve
    {
        get
        {
            return this._crouchCurve;
        }
    }

    public float crouchInSpeed
    {
        get
        {
            return -Mathf.Abs((float) (this.crouchSpeedBase * this._crouchToSpeedFraction));
        }
    }

    public float crouchOutSpeed
    {
        get
        {
            return Mathf.Abs(this.crouchSpeedBase);
        }
    }

    private float crouchSpeedBase
    {
        get
        {
            Keyframe keyframe = this._crouchCurve[0];
            Keyframe keyframe2 = this._crouchCurve[this._crouchCurve.length - 1];
            float num = keyframe2.value - keyframe.value;
            float num2 = keyframe2.time - keyframe.time;
            return (num / num2);
        }
    }

    public float crouchToSpeedFraction
    {
        get
        {
            return this._crouchToSpeedFraction;
        }
    }
}

