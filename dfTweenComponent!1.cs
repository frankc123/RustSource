using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

[Serializable]
public abstract class dfTweenComponent<T> : dfTweenComponentBase
{
    private T actualEndValue;
    private T actualStartValue;
    [SerializeField]
    protected T endValue;
    [SerializeField]
    protected T startValue;

    public event TweenNotification TweenCompleted;

    public event TweenNotification TweenPaused;

    public event TweenNotification TweenReset;

    public event TweenNotification TweenResumed;

    public event TweenNotification TweenStarted;

    public event TweenNotification TweenStopped;

    protected dfTweenComponent()
    {
    }

    public abstract T evaluate(T startValue, T endValue, float time);
    [DebuggerHidden]
    protected internal IEnumerator Execute(dfObservableProperty property)
    {
        return new <Execute>c__Iterator47<T> { property = property, <$>property = property, <>f__this = (dfTweenComponent<T>) this };
    }

    private string getPath(Transform obj)
    {
        StringBuilder builder = new StringBuilder();
        while (obj != null)
        {
            if (builder.Length > 0)
            {
                builder.Insert(0, @"\");
                builder.Insert(0, obj.name);
            }
            else
            {
                builder.Append(obj.name);
            }
            obj = obj.parent;
        }
        return builder.ToString();
    }

    protected internal static float Lerp(float startValue, float endValue, float time)
    {
        return (startValue + ((endValue - startValue) * time));
    }

    public abstract T offset(T value, T offset);
    protected internal override void onCompleted()
    {
        base.SendMessage("TweenCompleted", this, SendMessageOptions.DontRequireReceiver);
        if (this.TweenCompleted != null)
        {
            this.TweenCompleted();
        }
    }

    protected internal override void onPaused()
    {
        base.SendMessage("TweenPaused", this, SendMessageOptions.DontRequireReceiver);
        if (this.TweenPaused != null)
        {
            this.TweenPaused();
        }
    }

    protected internal override void onReset()
    {
        base.SendMessage("TweenReset", this, SendMessageOptions.DontRequireReceiver);
        if (this.TweenReset != null)
        {
            this.TweenReset();
        }
    }

    protected internal override void onResumed()
    {
        base.SendMessage("TweenResumed", this, SendMessageOptions.DontRequireReceiver);
        if (this.TweenResumed != null)
        {
            this.TweenResumed();
        }
    }

    protected internal override void onStarted()
    {
        base.SendMessage("TweenStarted", this, SendMessageOptions.DontRequireReceiver);
        if (this.TweenStarted != null)
        {
            this.TweenStarted();
        }
    }

    protected internal override void onStopped()
    {
        base.SendMessage("TweenStopped", this, SendMessageOptions.DontRequireReceiver);
        if (this.TweenStopped != null)
        {
            this.TweenStopped();
        }
    }

    public void Pause()
    {
        base.IsPaused = true;
    }

    public override void Play()
    {
        if (base.isRunning)
        {
            this.Stop();
        }
        if ((base.enabled && base.gameObject.activeSelf) && base.gameObject.activeInHierarchy)
        {
            if (base.target == null)
            {
                throw new NullReferenceException("Tween target is NULL");
            }
            if (!base.target.IsValid)
            {
                object[] objArray1 = new object[] { "Invalid property binding configuration on ", this.getPath(base.gameObject.transform), " - ", base.target };
                throw new InvalidOperationException(string.Concat(objArray1));
            }
            dfObservableProperty property = base.target.GetProperty();
            base.StartCoroutine(this.Execute(property));
        }
    }

    public override void Reset()
    {
        if (base.isRunning)
        {
            base.boundProperty.Value = this.actualStartValue;
            base.StopAllCoroutines();
            base.isRunning = false;
            this.onReset();
            base.easingFunction = null;
            base.boundProperty = null;
        }
    }

    public void Resume()
    {
        base.IsPaused = false;
    }

    public override void Stop()
    {
        if (base.isRunning)
        {
            if (base.skipToEndOnStop)
            {
                base.boundProperty.Value = this.actualEndValue;
            }
            base.StopAllCoroutines();
            base.isRunning = false;
            this.onStopped();
            base.easingFunction = null;
            base.boundProperty = null;
        }
    }

    public override string ToString()
    {
        if ((base.Target != null) && base.Target.IsValid)
        {
            string name = base.target.Component.name;
            return string.Format("{0} ({1}.{2})", this.TweenName, name, base.target.MemberName);
        }
        return this.TweenName;
    }

    public T EndValue
    {
        get
        {
            return this.endValue;
        }
        set
        {
            this.endValue = value;
            if (base.isRunning)
            {
                this.Stop();
                this.Play();
            }
        }
    }

    public T StartValue
    {
        get
        {
            return this.startValue;
        }
        set
        {
            this.startValue = value;
            if (base.isRunning)
            {
                this.Stop();
                this.Play();
            }
        }
    }

    [CompilerGenerated]
    private sealed class <Execute>c__Iterator47 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal dfObservableProperty <$>property;
        internal dfTweenComponent<T> <>f__this;
        internal float <elapsed>__1;
        internal float <pingPongDirection>__2;
        internal float <startTime>__0;
        internal float <time>__3;
        internal dfObservableProperty property;

        [DebuggerHidden]
        public void Dispose()
        {
            this.$PC = -1;
        }

        public bool MoveNext()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            switch (num)
            {
                case 0:
                    this.<>f__this.isRunning = true;
                    this.<>f__this.easingFunction = dfEasingFunctions.GetFunction(this.<>f__this.easingType);
                    this.<>f__this.boundProperty = this.property;
                    this.<>f__this.onStarted();
                    this.<startTime>__0 = Time.realtimeSinceStartup;
                    this.<elapsed>__1 = 0f;
                    this.<pingPongDirection>__2 = 0f;
                    this.<>f__this.actualStartValue = this.<>f__this.startValue;
                    this.<>f__this.actualEndValue = this.<>f__this.endValue;
                    if (!this.<>f__this.syncStartWhenRun)
                    {
                        if (this.<>f__this.startValueIsOffset)
                        {
                            this.<>f__this.actualStartValue = this.<>f__this.offset(this.<>f__this.startValue, (T) this.property.Value);
                        }
                        break;
                    }
                    this.<>f__this.actualStartValue = (T) this.property.Value;
                    break;

                case 1:
                case 2:
                    goto Label_0197;

                default:
                    goto Label_0376;
            }
            if (this.<>f__this.syncEndWhenRun)
            {
                this.<>f__this.actualEndValue = (T) this.property.Value;
            }
            else if (this.<>f__this.endValueIsOffset)
            {
                this.<>f__this.actualEndValue = this.<>f__this.offset(this.<>f__this.endValue, (T) this.property.Value);
            }
        Label_0197:
            if (this.<>f__this.isPaused)
            {
                this.$current = null;
                this.$PC = 1;
            }
            else
            {
                this.<elapsed>__1 = Mathf.Min(Time.realtimeSinceStartup - this.<startTime>__0, this.<>f__this.length);
                this.<time>__3 = this.<>f__this.easingFunction(0f, 1f, Mathf.Abs((float) (this.<pingPongDirection>__2 - (this.<elapsed>__1 / this.<>f__this.length))));
                if (this.<>f__this.animCurve != null)
                {
                    this.<time>__3 = this.<>f__this.animCurve.Evaluate(this.<time>__3);
                }
                this.property.Value = this.<>f__this.evaluate(this.<>f__this.actualStartValue, this.<>f__this.actualEndValue, this.<time>__3);
                if (this.<elapsed>__1 >= this.<>f__this.length)
                {
                    if (this.<>f__this.loopType == dfTweenLoopType.Once)
                    {
                        this.<>f__this.boundProperty.Value = this.<>f__this.actualEndValue;
                        this.<>f__this.isRunning = false;
                        this.<>f__this.onCompleted();
                        this.$PC = -1;
                        goto Label_0376;
                    }
                    if (this.<>f__this.loopType == dfTweenLoopType.Loop)
                    {
                        this.<startTime>__0 = Time.realtimeSinceStartup;
                    }
                    else
                    {
                        if (this.<>f__this.loopType != dfTweenLoopType.PingPong)
                        {
                            throw new NotImplementedException();
                        }
                        this.<startTime>__0 = Time.realtimeSinceStartup;
                        if (this.<pingPongDirection>__2 == 0f)
                        {
                            this.<pingPongDirection>__2 = 1f;
                        }
                        else
                        {
                            this.<pingPongDirection>__2 = 0f;
                        }
                    }
                }
                this.$current = null;
                this.$PC = 2;
            }
            return true;
        Label_0376:
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        object IEnumerator<object>.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }

        object IEnumerator.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }
    }
}

