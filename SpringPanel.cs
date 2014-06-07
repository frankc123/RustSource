using System;
using UnityEngine;

[RequireComponent(typeof(UIPanel)), AddComponentMenu("NGUI/Internal/Spring Panel")]
public class SpringPanel : IgnoreTimeScale
{
    private UIDraggablePanel mDrag;
    private UIPanel mPanel;
    private float mThreshold;
    private Transform mTrans;
    public float strength = 10f;
    public Vector3 target = Vector3.zero;

    public static SpringPanel Begin(GameObject go, Vector3 pos, float strength)
    {
        SpringPanel component = go.GetComponent<SpringPanel>();
        if (component == null)
        {
            component = go.AddComponent<SpringPanel>();
        }
        component.target = pos;
        component.strength = strength;
        if (!component.enabled)
        {
            component.mThreshold = 0f;
            component.enabled = true;
        }
        return component;
    }

    private void Start()
    {
        this.mPanel = base.GetComponent<UIPanel>();
        this.mDrag = base.GetComponent<UIDraggablePanel>();
        this.mTrans = base.transform;
    }

    private void Update()
    {
        float deltaTime = base.UpdateRealTimeDelta();
        if (this.mThreshold == 0f)
        {
            Vector3 vector4 = this.target - this.mTrans.localPosition;
            this.mThreshold = vector4.magnitude * 0.005f;
        }
        Vector3 localPosition = this.mTrans.localPosition;
        this.mTrans.localPosition = NGUIMath.SpringLerp(this.mTrans.localPosition, this.target, this.strength, deltaTime);
        Vector3 vector2 = this.mTrans.localPosition - localPosition;
        Vector4 clipRange = this.mPanel.clipRange;
        clipRange.x -= vector2.x;
        clipRange.y -= vector2.y;
        this.mPanel.clipRange = clipRange;
        if (this.mDrag != null)
        {
            this.mDrag.UpdateScrollbars(false);
        }
        Vector3 vector5 = this.target - this.mTrans.localPosition;
        if (this.mThreshold >= vector5.magnitude)
        {
            base.enabled = false;
        }
    }
}

