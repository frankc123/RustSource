using System;
using UnityEngine;

public class BobConfiguration : ScriptableObject
{
    [SerializeField]
    public BobForceCurve[] additionalCurves;
    public AnimationCurve allowCurve;
    public float angleImpulseForceMaxChangeAcceleration = float.PositiveInfinity;
    public float angleImpulseForceSmooth = 0.02f;
    public Vector3 angularImpulseForceScale = Vector3.one;
    public Vector3 angularSpringConstant = ((Vector3) (Vector3.one * 5f));
    public Vector3 angularSpringDampen = ((Vector3) (Vector3.one * 0.1f));
    public float angularWeightMass = 5f;
    public BobAntiOutput[] antiOutputs;
    public Vector3 elipsoidRadii = Vector3.one;
    public AnimationCurve forbidCurve;
    public Vector3 forceSpeedMultiplier = Vector3.one;
    public float impulseForceMaxChangeAcceleration = float.PositiveInfinity;
    public Vector3 impulseForceScale = Vector3.one;
    public float impulseForceSmooth = 0.02f;
    public Vector3 inputForceMultiplier = Vector3.one;
    public float intermitRate = 20f;
    public Vector3 maxVelocity = ((Vector3) (Vector3.one * 20f));
    public Vector3 positionDeadzone = new Vector3(0.0001f, 0.0001f, 0.0001f);
    public Vector3 rotationDeadzone = new Vector3(0.0001f, 0.0001f, 0.0001f);
    public float solveRate = 100f;
    public Vector3 springConstant = ((Vector3) (Vector3.one * 5f));
    public Vector3 springDampen = ((Vector3) (Vector3.one * 0.1f));
    public float timeScale = 1f;
    public float weightMass = 5f;
}

