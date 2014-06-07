using Facepunch.Intersect;
using System;

public class HitBoxSystem : HitBoxSystem
{
    protected void Awake()
    {
        base.Awake();
        this.CheckLayer();
    }

    private void CheckLayer()
    {
        if (base.gameObject.layer != 0x11)
        {
            base.gameObject.layer = 0x11;
        }
    }

    protected void Start()
    {
        this.CheckLayer();
    }
}

