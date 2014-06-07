using System;

public class EnvDecay : IDLocal
{
    protected DeployableObject _deployable;
    protected TakeDamage _takeDamage;
    public bool ambientDecay;
    public float decayMultiplier = 1f;
    protected float lastDecayThink;

    public void Awake()
    {
        base.enabled = false;
    }
}

