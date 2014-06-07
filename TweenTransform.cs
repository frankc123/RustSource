using System;
using UnityEngine;

[AddComponentMenu("NGUI/Tween/Transform")]
public class TweenTransform : UITweener
{
    public Transform from;
    private Transform mTrans;
    public Transform to;

    public static TweenTransform Begin(GameObject go, float duration, Transform from, Transform to)
    {
        TweenTransform transform = UITweener.Begin<TweenTransform>(go, duration);
        transform.from = from;
        transform.to = to;
        return transform;
    }

    protected override void OnUpdate(float factor)
    {
        if ((this.from != null) && (this.to != null))
        {
            if (this.mTrans == null)
            {
                this.mTrans = base.transform;
            }
            this.mTrans.position = (Vector3) ((this.from.position * (1f - factor)) + (this.to.position * factor));
            this.mTrans.localScale = (Vector3) ((this.from.localScale * (1f - factor)) + (this.to.localScale * factor));
            this.mTrans.rotation = Quaternion.Slerp(this.from.rotation, this.to.rotation, factor);
        }
    }
}

