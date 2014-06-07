using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Draggable Panel"), RequireComponent(typeof(UIPanel)), ExecuteInEditMode]
public class UIDraggablePanel : IgnoreTimeScale
{
    [SerializeField]
    private bool _calculateBoundsEveryChange = true;
    private bool _calculateNextChange;
    private bool _panelMayNeedBoundCalculation;
    public bool disableDragIfFits;
    public DragEffect dragEffect = DragEffect.MomentumAndSpring;
    public UIScrollBar horizontalScrollBar;
    private AABBox mBounds;
    private bool mCalculatedBounds;
    private bool mIgnoreCallbacks;
    private Vector3 mLastPos;
    private Vector3 mMomentum = Vector3.zero;
    public float momentumAmount = 35f;
    private UIPanel mPanel;
    private Plane mPlane;
    private bool mPressed;
    private float mScroll;
    private bool mShouldMove;
    private bool mStartedAutomatically;
    private bool mStartedManually;
    private int mTouches;
    private Transform mTrans;
    public Vector2 relativePositionOnReset = Vector2.zero;
    public bool repositionClipping;
    public bool respondHoverScroll = true;
    public bool restrictWithinPanel = true;
    public bool restrictWithinPanelWithScroll = true;
    public Vector3 scale = Vector3.one;
    public float scrollWheelFactor;
    public ShowCondition showScrollBars = ShowCondition.OnlyIfNeeded;
    public UIScrollBar verticalScrollBar;

    public event CalculatedNextChangeCallback onNextChangeCallback;

    private void Awake()
    {
        this.mTrans = base.transform;
        this.mPanel = base.GetComponent<UIPanel>();
    }

    public bool CalculateBoundsIfNeeded()
    {
        if (this._panelMayNeedBoundCalculation)
        {
            this.UpdateScrollbars(true);
            return !this._panelMayNeedBoundCalculation;
        }
        return false;
    }

    public void DisableSpring()
    {
        SpringPanel component = base.GetComponent<SpringPanel>();
        if (component != null)
        {
            component.enabled = false;
        }
    }

    public void Drag(Vector2 delta)
    {
        if ((base.enabled && base.gameObject.activeInHierarchy) && this.mShouldMove)
        {
            UICamera.currentTouch.clickNotification = UICamera.ClickNotification.BasedOnDelta;
            Ray ray = UICamera.currentCamera.ScreenPointToRay((Vector3) UICamera.currentTouch.pos);
            float enter = 0f;
            if (this.mPlane.Raycast(ray, out enter))
            {
                Vector3 point = ray.GetPoint(enter);
                Vector3 direction = point - this.mLastPos;
                this.mLastPos = point;
                if ((direction.x != 0f) || (direction.y != 0f))
                {
                    direction = this.mTrans.InverseTransformDirection(direction);
                    direction.Scale(this.scale);
                    direction = this.mTrans.TransformDirection(direction);
                }
                this.mMomentum = Vector3.Lerp(this.mMomentum, this.mMomentum + ((Vector3) (direction * (0.01f * this.momentumAmount))), 0.67f);
                this.MoveAbsolute(direction);
                if ((this.restrictWithinPanel && (this.mPanel.clipping != UIDrawCall.Clipping.None)) && (this.dragEffect != DragEffect.MomentumAndSpring))
                {
                    this.RestrictWithinBounds(false);
                }
            }
        }
    }

    private void LateUpdate()
    {
        if (!this.mPanel.enabled)
        {
            this.mMomentum = Vector3.zero;
        }
        else
        {
            if (this.mPanel.changedLastFrame)
            {
                this.OnPanelChanged();
            }
            if (this.repositionClipping)
            {
                this.repositionClipping = false;
                this.mCalculatedBounds = false;
                this.SetDragAmount(this.relativePositionOnReset.x, this.relativePositionOnReset.y, true);
            }
            if (Application.isPlaying)
            {
                float deltaTime = base.UpdateRealTimeDelta();
                if (this.showScrollBars != ShowCondition.Always)
                {
                    bool shouldMoveVertically = false;
                    bool shouldMoveHorizontally = false;
                    if ((this.showScrollBars != ShowCondition.WhenDragging) || (this.mTouches > 0))
                    {
                        shouldMoveVertically = this.shouldMoveVertically;
                        shouldMoveHorizontally = this.shouldMoveHorizontally;
                    }
                    if (this.verticalScrollBar != null)
                    {
                        float num2 = this.verticalScrollBar.alpha + (!shouldMoveVertically ? (-deltaTime * 3f) : (deltaTime * 6f));
                        num2 = Mathf.Clamp01(num2);
                        if (this.verticalScrollBar.alpha != num2)
                        {
                            this.verticalScrollBar.alpha = num2;
                        }
                    }
                    if (this.horizontalScrollBar != null)
                    {
                        float num3 = this.horizontalScrollBar.alpha + (!shouldMoveHorizontally ? (-deltaTime * 3f) : (deltaTime * 6f));
                        num3 = Mathf.Clamp01(num3);
                        if (this.horizontalScrollBar.alpha != num3)
                        {
                            this.horizontalScrollBar.alpha = num3;
                        }
                    }
                }
                if (this.mShouldMove && !this.mPressed)
                {
                    this.mMomentum += (Vector3) (this.scale * (-this.mScroll * 0.05f));
                    if (this.mMomentum.magnitude > 0.0001f)
                    {
                        this.mScroll = NGUIMath.SpringLerp(this.mScroll, 0f, 20f, deltaTime);
                        Vector3 absolute = NGUIMath.SpringDampen(ref this.mMomentum, 9f, deltaTime);
                        this.MoveAbsolute(absolute);
                        if ((this.restrictWithinPanel || this.restrictWithinPanelWithScroll) && (this.mPanel.clipping != UIDrawCall.Clipping.None))
                        {
                            this.RestrictWithinBounds(false);
                        }
                        return;
                    }
                    this.mScroll = 0f;
                }
                else
                {
                    this.mScroll = 0f;
                }
                NGUIMath.SpringDampen(ref this.mMomentum, 9f, deltaTime);
            }
        }
    }

    public bool ManualStart()
    {
        if (!this.mStartedManually && !this.mStartedAutomatically)
        {
            this.Start();
            this.mStartedManually = true;
            return true;
        }
        return false;
    }

    private void MoveAbsolute(Vector3 absolute)
    {
        Vector3 vector = this.mTrans.InverseTransformPoint(absolute);
        Vector3 vector2 = this.mTrans.InverseTransformPoint(Vector3.zero);
        this.MoveRelative(vector - vector2);
    }

    private void MoveRelative(Vector3 relative)
    {
        this.mTrans.localPosition += relative;
        Vector4 clipRange = this.mPanel.clipRange;
        clipRange.x -= relative.x;
        clipRange.y -= relative.y;
        this.mPanel.clipRange = clipRange;
        this.UpdateScrollbars(false);
    }

    private void OnHorizontalBar(UIScrollBar sb)
    {
        if (!this.mIgnoreCallbacks)
        {
            float x = (this.horizontalScrollBar == null) ? 0f : this.horizontalScrollBar.scrollValue;
            float y = (this.verticalScrollBar == null) ? 0f : this.verticalScrollBar.scrollValue;
            this.SetDragAmount(x, y, false);
        }
    }

    private void OnHoverScroll(float y)
    {
        if (this.respondHoverScroll)
        {
            this.Scroll(y);
        }
    }

    private void OnPanelChanged()
    {
        if (this._calculateNextChange)
        {
            this._calculateNextChange = false;
            this.UpdateScrollbars(true);
            if (this.calculatedNextChangeCallback != null)
            {
                CalculatedNextChangeCallback calculatedNextChangeCallback = this.calculatedNextChangeCallback;
                this.calculatedNextChangeCallback = null;
                calculatedNextChangeCallback();
            }
        }
        else if (!Application.isPlaying || this._calculateBoundsEveryChange)
        {
            this.UpdateScrollbars(true);
        }
        else
        {
            this._panelMayNeedBoundCalculation = true;
        }
    }

    private void OnVerticalBar(UIScrollBar sb)
    {
        if (!this.mIgnoreCallbacks)
        {
            float x = (this.horizontalScrollBar == null) ? 0f : this.horizontalScrollBar.scrollValue;
            float y = (this.verticalScrollBar == null) ? 0f : this.verticalScrollBar.scrollValue;
            this.SetDragAmount(x, y, false);
        }
    }

    public void Press(bool pressed)
    {
        if (base.enabled && base.gameObject.activeInHierarchy)
        {
            this.mTouches += !pressed ? -1 : 1;
            this.mCalculatedBounds = false;
            this.mShouldMove = this.shouldMove;
            if (this.mShouldMove)
            {
                this.mPressed = pressed;
                if (pressed)
                {
                    this.mMomentum = Vector3.zero;
                    this.mScroll = 0f;
                    this.DisableSpring();
                    this.mLastPos = UICamera.lastHit.point;
                    this.mPlane = new Plane((Vector3) (this.mTrans.rotation * Vector3.back), this.mLastPos);
                }
                else if ((this.restrictWithinPanel && (this.mPanel.clipping != UIDrawCall.Clipping.None)) && (this.dragEffect == DragEffect.MomentumAndSpring))
                {
                    this.RestrictWithinBounds(false);
                }
            }
        }
    }

    public void ResetPosition()
    {
        this.mCalculatedBounds = false;
        this.SetDragAmount(this.relativePositionOnReset.x, this.relativePositionOnReset.y, false);
        this.SetDragAmount(this.relativePositionOnReset.x, this.relativePositionOnReset.y, true);
    }

    public void RestrictWithinBounds(bool instant)
    {
        Vector3 relative = this.mPanel.CalculateConstrainOffset(this.bounds.min, this.bounds.max);
        if (relative.magnitude > 0.001f)
        {
            if (!instant && (this.dragEffect == DragEffect.MomentumAndSpring))
            {
                SpringPanel.Begin(this.mPanel.gameObject, this.mTrans.localPosition + relative, 13f);
            }
            else
            {
                this.MoveRelative(relative);
                this.mMomentum = Vector3.zero;
                this.mScroll = 0f;
            }
        }
        else
        {
            this.DisableSpring();
        }
    }

    public void Scroll(float delta)
    {
        if (base.enabled && base.gameObject.activeInHierarchy)
        {
            this.mShouldMove = this.shouldMove;
            if (Mathf.Sign(this.mScroll) != Mathf.Sign(delta))
            {
                this.mScroll = 0f;
            }
            this.mScroll += delta * this.scrollWheelFactor;
        }
    }

    public void SetDragAmount(float x, float y, bool updateScrollbars)
    {
        this.DisableSpring();
        AABBox bounds = this.bounds;
        if ((bounds.min.x != bounds.max.x) && (bounds.min.y != bounds.max.x))
        {
            Vector4 clipRange = this.mPanel.clipRange;
            float num = clipRange.z * 0.5f;
            float num2 = clipRange.w * 0.5f;
            float from = bounds.min.x + num;
            float to = bounds.max.x - num;
            float num5 = bounds.min.y + num2;
            float num6 = bounds.max.y - num2;
            if (this.mPanel.clipping == UIDrawCall.Clipping.SoftClip)
            {
                from -= this.mPanel.clipSoftness.x;
                to += this.mPanel.clipSoftness.x;
                num5 -= this.mPanel.clipSoftness.y;
                num6 += this.mPanel.clipSoftness.y;
            }
            float num7 = Mathf.Lerp(from, to, x);
            float num8 = Mathf.Lerp(num6, num5, y);
            if (!updateScrollbars)
            {
                Vector3 localPosition = this.mTrans.localPosition;
                if (this.scale.x != 0f)
                {
                    localPosition.x += clipRange.x - num7;
                }
                if (this.scale.y != 0f)
                {
                    localPosition.y += clipRange.y - num8;
                }
                this.mTrans.localPosition = localPosition;
            }
            clipRange.x = num7;
            clipRange.y = num8;
            this.mPanel.clipRange = clipRange;
            if (updateScrollbars)
            {
                this.UpdateScrollbars(false);
            }
        }
    }

    private void Start()
    {
        if (!this.mStartedManually)
        {
            this.UpdateScrollbars(true);
            if (this.horizontalScrollBar != null)
            {
                this.horizontalScrollBar.onChange = (UIScrollBar.OnScrollBarChange) Delegate.Combine(this.horizontalScrollBar.onChange, new UIScrollBar.OnScrollBarChange(this.OnHorizontalBar));
                this.horizontalScrollBar.alpha = ((this.showScrollBars != ShowCondition.Always) && !this.shouldMoveHorizontally) ? 0f : 1f;
            }
            if (this.verticalScrollBar != null)
            {
                this.verticalScrollBar.onChange = (UIScrollBar.OnScrollBarChange) Delegate.Combine(this.verticalScrollBar.onChange, new UIScrollBar.OnScrollBarChange(this.OnVerticalBar));
                this.verticalScrollBar.alpha = ((this.showScrollBars != ShowCondition.Always) && !this.shouldMoveVertically) ? 0f : 1f;
            }
            this.mStartedAutomatically = true;
        }
    }

    public void UpdateScrollbars(bool recalculateBounds)
    {
        if (this.mPanel != null)
        {
            if ((this.horizontalScrollBar != null) || (this.verticalScrollBar != null))
            {
                if (recalculateBounds)
                {
                    this.mCalculatedBounds = false;
                    this._panelMayNeedBoundCalculation = false;
                    this.mShouldMove = this.shouldMove;
                }
                if (this.horizontalScrollBar != null)
                {
                    AABBox bounds = this.bounds;
                    Vector3 size = bounds.size;
                    if (size.x > 0f)
                    {
                        Vector4 clipRange = this.mPanel.clipRange;
                        float num = clipRange.z * 0.5f;
                        float num2 = (clipRange.x - num) - bounds.min.x;
                        float num3 = (bounds.max.x - num) - clipRange.x;
                        if (this.mPanel.clipping == UIDrawCall.Clipping.SoftClip)
                        {
                            num2 += this.mPanel.clipSoftness.x;
                            num3 -= this.mPanel.clipSoftness.x;
                        }
                        num2 = Mathf.Clamp01(num2 / size.x);
                        num3 = Mathf.Clamp01(num3 / size.x);
                        float num4 = num2 + num3;
                        this.mIgnoreCallbacks = true;
                        this.horizontalScrollBar.barSize = 1f - num4;
                        this.horizontalScrollBar.scrollValue = (num4 <= 0.001f) ? 0f : (num2 / num4);
                        this.mIgnoreCallbacks = false;
                    }
                }
                if (this.verticalScrollBar != null)
                {
                    AABBox box2 = this.bounds;
                    Vector3 vector3 = box2.size;
                    if (vector3.y > 0f)
                    {
                        Vector4 vector4 = this.mPanel.clipRange;
                        float num5 = vector4.w * 0.5f;
                        float num6 = (vector4.y - num5) - box2.min.y;
                        float num7 = (box2.max.y - num5) - vector4.y;
                        if (this.mPanel.clipping == UIDrawCall.Clipping.SoftClip)
                        {
                            num6 += this.mPanel.clipSoftness.y;
                            num7 -= this.mPanel.clipSoftness.y;
                        }
                        num6 = Mathf.Clamp01(num6 / vector3.y);
                        num7 = Mathf.Clamp01(num7 / vector3.y);
                        float num8 = num6 + num7;
                        this.mIgnoreCallbacks = true;
                        this.verticalScrollBar.barSize = 1f - num8;
                        this.verticalScrollBar.scrollValue = (num8 <= 0.001f) ? 0f : (1f - (num6 / num8));
                        this.mIgnoreCallbacks = false;
                    }
                }
            }
            else if (recalculateBounds)
            {
                this.mCalculatedBounds = false;
                this._panelMayNeedBoundCalculation = false;
            }
        }
    }

    public AABBox bounds
    {
        get
        {
            if (!this.mCalculatedBounds)
            {
                this.mCalculatedBounds = true;
                this.mBounds = NGUIMath.CalculateRelativeWidgetBounds(this.mTrans, this.mTrans);
            }
            return this.mBounds;
        }
    }

    public bool calculateBoundsEveryChange
    {
        get
        {
            return this._calculateBoundsEveryChange;
        }
        set
        {
            if (value)
            {
                if (!this._calculateBoundsEveryChange)
                {
                    this.CalculateBoundsIfNeeded();
                    this._calculateBoundsEveryChange = true;
                }
            }
            else
            {
                this._calculateBoundsEveryChange = false;
            }
        }
    }

    public bool calculateNextChange
    {
        set
        {
            if (value)
            {
                this._calculateNextChange = true;
            }
        }
    }

    public Vector3 currentMomentum
    {
        get
        {
            return this.mMomentum;
        }
        set
        {
            this.mMomentum = value;
        }
    }

    public bool panelMayNeedBoundsCalculated
    {
        get
        {
            return this._panelMayNeedBoundCalculation;
        }
    }

    private bool shouldMove
    {
        get
        {
            if (!this.disableDragIfFits)
            {
                return true;
            }
            if (this.mPanel == null)
            {
                this.mPanel = base.GetComponent<UIPanel>();
            }
            Vector4 clipRange = this.mPanel.clipRange;
            AABBox bounds = this.bounds;
            float num = clipRange.z * 0.5f;
            float num2 = clipRange.w * 0.5f;
            if (!Mathf.Approximately(this.scale.x, 0f))
            {
                if (bounds.min.x < (clipRange.x - num))
                {
                    return true;
                }
                if (bounds.max.x > (clipRange.x + num))
                {
                    return true;
                }
            }
            if (!Mathf.Approximately(this.scale.y, 0f))
            {
                if (bounds.min.y < (clipRange.y - num2))
                {
                    return true;
                }
                if (bounds.max.y > (clipRange.y + num2))
                {
                    return true;
                }
            }
            return false;
        }
    }

    public bool shouldMoveHorizontally
    {
        get
        {
            float x = this.bounds.size.x;
            if (this.mPanel.clipping == UIDrawCall.Clipping.SoftClip)
            {
                x += this.mPanel.clipSoftness.x * 2f;
            }
            return (x > this.mPanel.clipRange.z);
        }
    }

    public bool shouldMoveVertically
    {
        get
        {
            float y = this.bounds.size.y;
            if (this.mPanel.clipping == UIDrawCall.Clipping.SoftClip)
            {
                y += this.mPanel.clipSoftness.y * 2f;
            }
            return (y > this.mPanel.clipRange.w);
        }
    }

    public delegate void CalculatedNextChangeCallback();

    public enum DragEffect
    {
        None,
        Momentum,
        MomentumAndSpring
    }

    public enum ShowCondition
    {
        Always,
        OnlyIfNeeded,
        WhenDragging
    }
}

