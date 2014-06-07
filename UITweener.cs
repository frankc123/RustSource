using AnimationOrTween;
using System;
using UnityEngine;

public abstract class UITweener : IgnoreTimeScale
{
    public string callWhenFinished;
    public float delay;
    public float duration = 1f;
    public GameObject eventReceiver;
    private float mAmountPerDelta = 1f;
    private float mDuration;
    public Method method;
    private float mFactor;
    private float mStartTime;
    public bool steeperCurves;
    public Style style;
    public int tweenGroup;

    protected UITweener()
    {
    }

    [Obsolete("Use Tweener.Play instead")]
    public void Animate(bool forward)
    {
        this.Play(forward);
    }

    public static T Begin<T>(GameObject go, float duration) where T: UITweener
    {
        T component = go.GetComponent<T>();
        if (component == null)
        {
            component = go.AddComponent<T>();
        }
        component.duration = duration;
        component.mFactor = 0f;
        component.style = Style.Once;
        component.enabled = true;
        return component;
    }

    protected abstract void OnUpdate(float factor);
    public void Play(bool forward)
    {
        this.mAmountPerDelta = Mathf.Abs(this.amountPerDelta);
        if (!forward)
        {
            this.mAmountPerDelta = -this.mAmountPerDelta;
        }
        base.enabled = true;
    }

    public void Reset()
    {
        this.mFactor = (this.mAmountPerDelta >= 0f) ? 0f : 1f;
    }

    private void Start()
    {
        this.mStartTime = Time.time + this.delay;
        this.Update();
    }

    public void Toggle()
    {
        if (this.mFactor > 0f)
        {
            this.mAmountPerDelta = -this.amountPerDelta;
        }
        else
        {
            this.mAmountPerDelta = Mathf.Abs(this.amountPerDelta);
        }
        base.enabled = true;
    }

    private void Update()
    {
        if (Time.time >= this.mStartTime)
        {
            float num = base.UpdateRealTimeDelta();
            this.mFactor += this.amountPerDelta * num;
            if (this.style == Style.Loop)
            {
                if (this.mFactor > 1f)
                {
                    this.mFactor -= Mathf.Floor(this.mFactor);
                }
            }
            else if (this.style == Style.PingPong)
            {
                if (this.mFactor > 1f)
                {
                    this.mFactor = 1f - (this.mFactor - Mathf.Floor(this.mFactor));
                    this.mAmountPerDelta = -this.mAmountPerDelta;
                }
                else if (this.mFactor < 0f)
                {
                    this.mFactor = -this.mFactor;
                    this.mFactor -= Mathf.Floor(this.mFactor);
                    this.mAmountPerDelta = -this.mAmountPerDelta;
                }
            }
            float f = Mathf.Clamp01(this.mFactor);
            if (this.method == Method.EaseIn)
            {
                f = 1f - Mathf.Sin(1.570796f * (1f - f));
                if (this.steeperCurves)
                {
                    f *= f;
                }
            }
            else if (this.method == Method.EaseOut)
            {
                f = Mathf.Sin(1.570796f * f);
                if (this.steeperCurves)
                {
                    f = 1f - f;
                    f = 1f - (f * f);
                }
            }
            else if (this.method == Method.EaseInOut)
            {
                f -= Mathf.Sin(f * 6.283185f) / 6.283185f;
                if (this.steeperCurves)
                {
                    f = (f * 2f) - 1f;
                    float num4 = Mathf.Sign(f);
                    f = 1f - Mathf.Abs(f);
                    f = 1f - (f * f);
                    f = ((num4 * f) * 0.5f) + 0.5f;
                }
            }
            this.OnUpdate(f);
            if ((this.style == Style.Once) && ((this.mFactor > 1f) || (this.mFactor < 0f)))
            {
                this.mFactor = Mathf.Clamp01(this.mFactor);
                if (string.IsNullOrEmpty(this.callWhenFinished))
                {
                    base.enabled = false;
                }
                else
                {
                    if ((this.eventReceiver != null) && !string.IsNullOrEmpty(this.callWhenFinished))
                    {
                        this.eventReceiver.SendMessage(this.callWhenFinished, this, SendMessageOptions.DontRequireReceiver);
                    }
                    if (((this.mFactor == 1f) && (this.mAmountPerDelta > 0f)) || ((this.mFactor == 0f) && (this.mAmountPerDelta < 0f)))
                    {
                        base.enabled = false;
                    }
                }
            }
        }
    }

    public float amountPerDelta
    {
        get
        {
            if (this.mDuration != this.duration)
            {
                this.mDuration = this.duration;
                this.mAmountPerDelta = Mathf.Abs((this.duration <= 0f) ? 1000f : (1f / this.duration));
            }
            return this.mAmountPerDelta;
        }
    }

    public Direction direction
    {
        get
        {
            return ((this.mAmountPerDelta >= 0f) ? Direction.Forward : Direction.Reverse);
        }
    }

    public float factor
    {
        get
        {
            return this.mFactor;
        }
    }

    public enum Method
    {
        Linear,
        EaseIn,
        EaseOut,
        EaseInOut
    }

    public enum Style
    {
        Once,
        Loop,
        PingPong
    }
}

