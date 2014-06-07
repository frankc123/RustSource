using System;
using UnityEngine;

public class HumanControlConfiguration : ControlConfiguration
{
    [SerializeField]
    private AnimationCurve crouchMulSpeedByTime;
    [SerializeField]
    private AnimationCurve landingSpeedPenalty;
    [SerializeField]
    private AnimationCurve sprintAddSpeedByTime;
    [SerializeField]
    private Vector2 sprintScalars;

    public HumanControlConfiguration()
    {
        Keyframe[] keys = new Keyframe[] { new Keyframe(0f, 0f, 0f, 0f), new Keyframe(0.4f, 1f, 0f, 0f) };
        this.sprintAddSpeedByTime = new AnimationCurve(keys);
        Keyframe[] keyframeArray2 = new Keyframe[] { new Keyframe(0f, 1f, 0f, 0f), new Keyframe(0.4f, 0.55f, 0f, 0f) };
        this.crouchMulSpeedByTime = new AnimationCurve(keyframeArray2);
        Keyframe[] keyframeArray3 = new Keyframe[] { new Keyframe(0f, 1f, 0f, 0f), new Keyframe(0.25f, 0.5f, -2f, -2f), new Keyframe(0.75f, 1f, 0f, 0f) };
        this.landingSpeedPenalty = new AnimationCurve(keyframeArray3);
        this.sprintScalars = new Vector2(0.2f, 1f);
    }

    public AnimationCurve curveCrouchMulSpeedByTime
    {
        get
        {
            return this.crouchMulSpeedByTime;
        }
    }

    public AnimationCurve curveLandingSpeedPenalty
    {
        get
        {
            return this.landingSpeedPenalty;
        }
    }

    public AnimationCurve curveSprintAddSpeedByTime
    {
        get
        {
            return this.sprintAddSpeedByTime;
        }
    }

    public Vector2 sprintScale
    {
        get
        {
            return this.sprintScalars;
        }
    }

    public float sprintScaleX
    {
        get
        {
            return this.sprintScalars.x;
        }
    }

    public float sprintScaleY
    {
        get
        {
            return this.sprintScalars.y;
        }
    }
}

