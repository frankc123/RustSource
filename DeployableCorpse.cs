using System;

public class DeployableCorpse : IDMain
{
    private float lifeTime;

    public DeployableCorpse() : this(IDFlags.Unknown)
    {
    }

    protected DeployableCorpse(IDFlags flags) : base(flags)
    {
        this.lifeTime = 300f;
    }
}

