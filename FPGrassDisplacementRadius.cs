using System;
using UnityEngine;

public class FPGrassDisplacementRadius : FPGrassDisplacementObject
{
    private Vector3 startScale;

    public override void DetachAndDestroy()
    {
        base.transform.parent = null;
        base.SetOn(false);
        Object.Destroy(base.gameObject, 1f);
    }

    public override void Initialize()
    {
        this.startScale = base.myTransform.localScale;
        base.myTransform.localScale = Vector3.zero;
    }

    public override void UpdateDepression()
    {
        if (!Mathf.Approximately(base.currentDepressionPercent, base.targetDepressionPercent))
        {
            float num = Mathf.Lerp(base.currentDepressionPercent, base.targetDepressionPercent, Time.deltaTime * 5f);
            base.currentDepressionPercent = num;
            base.myTransform.localScale = (Vector3) (this.startScale * base.currentDepressionPercent);
        }
    }
}

