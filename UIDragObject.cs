using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Drag Object")]
public class UIDragObject : IgnoreTimeScale
{
    public DragEffect dragEffect = DragEffect.MomentumAndSpring;
    private AABBox mBounds;
    private Vector3 mLastPos;
    private Vector3 mMomentum = Vector3.zero;
    public float momentumAmount = 35f;
    private UIPanel mPanel;
    private Plane mPlane;
    private bool mPressed;
    private float mScroll;
    public bool restrictToScreen;
    public bool restrictWithinPanel;
    public Vector3 scale = Vector3.one;
    public float scrollWheelFactor;
    public Transform sizeParent;
    public Transform target;

    private void FindPanel()
    {
        this.mPanel = (this.target == null) ? null : UIPanel.Find(this.target.transform, false);
        if (this.mPanel == null)
        {
            this.restrictWithinPanel = false;
        }
    }

    private void LateUpdate()
    {
        float deltaTime = base.UpdateRealTimeDelta();
        if (this.target != null)
        {
            if (this.mPressed)
            {
                SpringPosition component = this.target.GetComponent<SpringPosition>();
                if (component != null)
                {
                    component.enabled = false;
                }
                this.mScroll = 0f;
            }
            else
            {
                this.mMomentum += (Vector3) (this.scale * (-this.mScroll * 0.05f));
                this.mScroll = NGUIMath.SpringLerp(this.mScroll, 0f, 20f, deltaTime);
                if (this.mMomentum.magnitude > 0.0001f)
                {
                    if (this.mPanel == null)
                    {
                        this.FindPanel();
                    }
                    if (this.mPanel != null)
                    {
                        this.target.position += NGUIMath.SpringDampen(ref this.mMomentum, 9f, deltaTime);
                        if (this.restrictWithinPanel && (this.mPanel.clipping != UIDrawCall.Clipping.None))
                        {
                            this.mBounds = NGUIMath.CalculateRelativeWidgetBounds(this.mPanel.cachedTransform, this.target);
                            if (!this.mPanel.ConstrainTargetToBounds(this.target, ref this.mBounds, this.dragEffect == DragEffect.None))
                            {
                                SpringPosition position2 = this.target.GetComponent<SpringPosition>();
                                if (position2 != null)
                                {
                                    position2.enabled = false;
                                }
                            }
                        }
                        return;
                    }
                }
                else
                {
                    this.mScroll = 0f;
                }
            }
            NGUIMath.SpringDampen(ref this.mMomentum, 9f, deltaTime);
        }
    }

    private void OnDrag(Vector2 delta)
    {
        if ((base.enabled && base.gameObject.activeInHierarchy) && (this.target != null))
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
                    direction = this.target.InverseTransformDirection(direction);
                    direction.Scale(this.scale);
                    direction = this.target.TransformDirection(direction);
                }
                this.mMomentum = Vector3.Lerp(this.mMomentum, this.mMomentum + ((Vector3) (direction * (0.01f * this.momentumAmount))), 0.67f);
                if (this.restrictWithinPanel)
                {
                    Vector3 localPosition = this.target.localPosition;
                    this.target.position += direction;
                    this.mBounds.center += this.target.localPosition - localPosition;
                    if (((this.dragEffect != DragEffect.MomentumAndSpring) && (this.mPanel.clipping != UIDrawCall.Clipping.None)) && this.mPanel.ConstrainTargetToBounds(this.target, ref this.mBounds, true))
                    {
                        this.mMomentum = Vector3.zero;
                        this.mScroll = 0f;
                    }
                }
                else if (this.restrictToScreen)
                {
                    Vector2 localScale;
                    this.target.position += direction;
                    if (this.sizeParent != null)
                    {
                        localScale = this.sizeParent.transform.localScale;
                    }
                    else
                    {
                        localScale = NGUIMath.CalculateRelativeWidgetBounds(this.target).size;
                    }
                    Rect rect = screenBorder.Add(new Rect(0f, (float) -Screen.height, (float) Screen.width, (float) Screen.height));
                    Vector3 vector5 = this.target.localPosition;
                    bool flag = true;
                    if ((vector5.x + localScale.x) > rect.xMax)
                    {
                        vector5.x = rect.xMax - localScale.x;
                    }
                    else if (vector5.x < rect.xMin)
                    {
                        vector5.x = rect.xMin;
                    }
                    else
                    {
                        flag = false;
                    }
                    bool flag2 = true;
                    if (vector5.y > rect.yMax)
                    {
                        vector5.y = rect.yMax;
                    }
                    else if ((vector5.y - localScale.y) < rect.yMin)
                    {
                        vector5.y = rect.yMin + localScale.y;
                    }
                    else
                    {
                        flag2 = false;
                    }
                    if (flag || flag2)
                    {
                        this.target.localPosition = vector5;
                    }
                }
            }
        }
    }

    private void OnPress(bool pressed)
    {
        if ((base.enabled && base.gameObject.activeInHierarchy) && (this.target != null))
        {
            this.mPressed = pressed;
            if (pressed)
            {
                if ((this.restrictWithinPanel || this.restrictToScreen) && (this.mPanel == null))
                {
                    this.FindPanel();
                }
                if (this.restrictWithinPanel)
                {
                    this.mBounds = NGUIMath.CalculateRelativeWidgetBounds(this.mPanel.cachedTransform, this.target);
                }
                if (this.restrictToScreen)
                {
                    UICamera camera = UICamera.FindCameraForLayer(base.gameObject.layer);
                    Rect rect = screenBorder.Add(camera.camera.pixelRect);
                    this.mBounds = AABBox.CenterAndSize((Vector3) rect.center, new Vector3(rect.width, rect.height, 0f));
                }
                this.mMomentum = Vector3.zero;
                this.mScroll = 0f;
                SpringPosition component = this.target.GetComponent<SpringPosition>();
                if (component != null)
                {
                    component.enabled = false;
                }
                this.mLastPos = UICamera.lastHit.point;
                Transform transform = UICamera.currentCamera.transform;
                this.mPlane = new Plane((Vector3) (((this.mPanel == null) ? transform.rotation : this.mPanel.cachedTransform.rotation) * Vector3.back), this.mLastPos);
            }
            else if ((this.restrictWithinPanel && (this.mPanel.clipping != UIDrawCall.Clipping.None)) && (this.dragEffect == DragEffect.MomentumAndSpring))
            {
                this.mPanel.ConstrainTargetToBounds(this.target, ref this.mBounds, false);
            }
        }
    }

    private void OnScroll(float delta)
    {
        if (base.enabled && base.gameObject.activeInHierarchy)
        {
            if (Mathf.Sign(this.mScroll) != Mathf.Sign(delta))
            {
                this.mScroll = 0f;
            }
            this.mScroll += delta * this.scrollWheelFactor;
        }
    }

    public static RectOffset screenBorder
    {
        get
        {
            return new RectOffset(0, -64, 0, 0);
        }
    }

    public enum DragEffect
    {
        None,
        Momentum,
        MomentumAndSpring
    }
}

