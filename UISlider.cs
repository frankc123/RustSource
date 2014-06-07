using System;
using UnityEngine;

[ExecuteInEditMode, AddComponentMenu("NGUI/Interaction/Slider")]
public class UISlider : IgnoreTimeScale
{
    public static UISlider current;
    public Direction direction;
    public GameObject eventReceiver;
    public Transform foreground;
    public Vector2 fullSize = Vector2.zero;
    public string functionName = "OnSliderChange";
    private BoxCollider mCol;
    private UIFilledSprite mFGFilled;
    private Transform mFGTrans;
    private UIWidget mFGWidget;
    private bool mInitDone;
    private float mStepValue = 1f;
    private Transform mTrans;
    public int numberOfSteps;
    [HideInInspector, SerializeField]
    private float rawValue = 1f;
    public Transform thumb;

    private void Awake()
    {
        this.mTrans = base.transform;
        this.mCol = base.collider as BoxCollider;
    }

    public void ForceUpdate()
    {
        this.Set(this.rawValue, true);
    }

    private void Init()
    {
        this.mInitDone = true;
        if (this.foreground != null)
        {
            this.mFGWidget = this.foreground.GetComponent<UIWidget>();
            this.mFGFilled = (this.mFGWidget == null) ? null : (this.mFGWidget as UIFilledSprite);
            this.mFGTrans = this.foreground.transform;
            if (this.fullSize == Vector2.zero)
            {
                this.fullSize = this.foreground.localScale;
            }
        }
        else if (this.mCol != null)
        {
            if (this.fullSize == Vector2.zero)
            {
                this.fullSize = this.mCol.size;
            }
        }
        else
        {
            Debug.LogWarning("UISlider expected to find a foreground object or a box collider to work with", this);
        }
    }

    private void OnDrag(Vector2 delta)
    {
        this.UpdateDrag();
    }

    private void OnDragThumb(GameObject go, Vector2 delta)
    {
        this.UpdateDrag();
    }

    private void OnKey(KeyCode key)
    {
        float num = (this.numberOfSteps <= 1f) ? 0.125f : (1f / ((float) (this.numberOfSteps - 1)));
        if (this.direction == Direction.Horizontal)
        {
            if (key == KeyCode.LeftArrow)
            {
                this.Set(this.rawValue - num, false);
            }
            else if (key == KeyCode.RightArrow)
            {
                this.Set(this.rawValue + num, false);
            }
        }
        else if (key == KeyCode.DownArrow)
        {
            this.Set(this.rawValue - num, false);
        }
        else if (key == KeyCode.UpArrow)
        {
            this.Set(this.rawValue + num, false);
        }
    }

    private void OnPress(bool pressed)
    {
        if (pressed)
        {
            this.UpdateDrag();
        }
    }

    private void OnPressThumb(GameObject go, bool pressed)
    {
        if (pressed)
        {
            this.UpdateDrag();
        }
    }

    private void Set(float input, bool force)
    {
        if (!this.mInitDone)
        {
            this.Init();
        }
        float num = Mathf.Clamp01(input);
        if (num < 0.001f)
        {
            num = 0f;
        }
        this.rawValue = num;
        if (this.numberOfSteps > 1)
        {
            num = Mathf.Round(num * (this.numberOfSteps - 1)) / ((float) (this.numberOfSteps - 1));
        }
        if (force || (this.mStepValue != num))
        {
            this.mStepValue = num;
            Vector3 fullSize = (Vector3) this.fullSize;
            if (this.direction == Direction.Horizontal)
            {
                fullSize.x *= this.mStepValue;
            }
            else
            {
                fullSize.y *= this.mStepValue;
            }
            if (this.mFGFilled != null)
            {
                this.mFGFilled.fillAmount = this.mStepValue;
            }
            else if (this.foreground != null)
            {
                this.mFGTrans.localScale = fullSize;
                if (this.mFGWidget != null)
                {
                    if (num > 0.001f)
                    {
                        this.mFGWidget.enabled = true;
                        this.mFGWidget.MarkAsChanged();
                    }
                    else
                    {
                        this.mFGWidget.enabled = false;
                    }
                }
            }
            if (this.thumb != null)
            {
                Vector3 localPosition = this.thumb.localPosition;
                if (this.mFGFilled != null)
                {
                    if (this.mFGFilled.fillDirection == UIFilledSprite.FillDirection.Horizontal)
                    {
                        localPosition.x = !this.mFGFilled.invert ? fullSize.x : (this.fullSize.x - fullSize.x);
                    }
                    else if (this.mFGFilled.fillDirection == UIFilledSprite.FillDirection.Vertical)
                    {
                        localPosition.y = !this.mFGFilled.invert ? fullSize.y : (this.fullSize.y - fullSize.y);
                    }
                }
                else if (this.direction == Direction.Horizontal)
                {
                    localPosition.x = fullSize.x;
                }
                else
                {
                    localPosition.y = fullSize.y;
                }
                this.thumb.localPosition = localPosition;
            }
            if (((this.eventReceiver != null) && !string.IsNullOrEmpty(this.functionName)) && Application.isPlaying)
            {
                current = this;
                this.eventReceiver.SendMessage(this.functionName, this.mStepValue, SendMessageOptions.DontRequireReceiver);
                current = null;
            }
        }
    }

    private void Start()
    {
        this.Init();
        if ((Application.isPlaying && (this.thumb != null)) && NGUITools.HasMeansOfClicking(this.thumb))
        {
            UIEventListener listener = UIEventListener.Get(this.thumb.gameObject);
            listener.onPress = (UIEventListener.BoolDelegate) Delegate.Combine(listener.onPress, new UIEventListener.BoolDelegate(this.OnPressThumb));
            listener.onDrag = (UIEventListener.VectorDelegate) Delegate.Combine(listener.onDrag, new UIEventListener.VectorDelegate(this.OnDragThumb));
        }
        this.Set(this.rawValue, true);
    }

    private void UpdateDrag()
    {
        if (((this.mCol != null) && (UICamera.currentCamera != null)) && UICamera.IsPressing)
        {
            float num;
            UICamera.currentTouch.clickNotification = UICamera.ClickNotification.None;
            Ray ray = UICamera.currentCamera.ScreenPointToRay((Vector3) UICamera.currentTouch.pos);
            Plane plane = new Plane((Vector3) (this.mTrans.rotation * Vector3.back), this.mTrans.position);
            if (plane.Raycast(ray, out num))
            {
                Vector3 vector = (this.mTrans.localPosition + this.mCol.center) - ((Vector3) (this.mCol.size * 0.5f));
                Vector3 vector2 = this.mTrans.localPosition - vector;
                Vector3 vector4 = this.mTrans.InverseTransformPoint(ray.GetPoint(num)) + vector2;
                this.Set((this.direction != Direction.Horizontal) ? (vector4.y / this.mCol.size.y) : (vector4.x / this.mCol.size.x), false);
            }
        }
    }

    public float sliderValue
    {
        get
        {
            return this.mStepValue;
        }
        set
        {
            this.Set(value, false);
        }
    }

    public enum Direction
    {
        Horizontal,
        Vertical
    }
}

