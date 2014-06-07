using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

[Serializable, RequireComponent(typeof(BoxCollider)), AddComponentMenu("Daikon Forge/Tweens/Sprite Animator")]
public class dfSpriteAnimation : dfTweenPlayableBase
{
    [SerializeField]
    private string animationName = "ANIMATION";
    private bool autoRunStarted;
    [SerializeField]
    private bool autoStart;
    [SerializeField]
    private dfAnimationClip clip;
    private bool isPaused;
    private bool isRunning;
    [SerializeField]
    private float length = 1f;
    [SerializeField]
    private dfTweenLoopType loopType = dfTweenLoopType.Loop;
    [SerializeField]
    private dfComponentMemberInfo memberInfo = new dfComponentMemberInfo();
    [SerializeField]
    private PlayDirection playDirection;
    [SerializeField]
    private bool skipToEndOnStop;
    private dfObservableProperty target;

    public event TweenNotification AnimationCompleted;

    public event TweenNotification AnimationPaused;

    public event TweenNotification AnimationReset;

    public event TweenNotification AnimationResumed;

    public event TweenNotification AnimationStarted;

    public event TweenNotification AnimationStopped;

    public void Awake()
    {
    }

    [DebuggerHidden]
    private IEnumerator Execute()
    {
        return new <Execute>c__Iterator46 { <>f__this = this };
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

    public void LateUpdate()
    {
        if ((this.AutoRun && !this.IsPlaying) && !this.autoRunStarted)
        {
            this.autoRunStarted = true;
            this.Play();
        }
    }

    protected void onCompleted()
    {
        base.SendMessage("AnimationCompleted", this, SendMessageOptions.DontRequireReceiver);
        if (this.AnimationCompleted != null)
        {
            this.AnimationCompleted();
        }
    }

    protected void onPaused()
    {
        base.SendMessage("AnimationPaused", this, SendMessageOptions.DontRequireReceiver);
        if (this.AnimationPaused != null)
        {
            this.AnimationPaused();
        }
    }

    protected void onReset()
    {
        base.SendMessage("AnimationReset", this, SendMessageOptions.DontRequireReceiver);
        if (this.AnimationReset != null)
        {
            this.AnimationReset();
        }
    }

    protected void onResumed()
    {
        base.SendMessage("AnimationResumed", this, SendMessageOptions.DontRequireReceiver);
        if (this.AnimationResumed != null)
        {
            this.AnimationResumed();
        }
    }

    protected void onStarted()
    {
        base.SendMessage("AnimationStarted", this, SendMessageOptions.DontRequireReceiver);
        if (this.AnimationStarted != null)
        {
            this.AnimationStarted();
        }
    }

    protected void onStopped()
    {
        base.SendMessage("AnimationStopped", this, SendMessageOptions.DontRequireReceiver);
        if (this.AnimationStopped != null)
        {
            this.AnimationStopped();
        }
    }

    public void Pause()
    {
        if (this.isRunning)
        {
            this.isPaused = true;
            this.onPaused();
        }
    }

    public override void Play()
    {
        if (this.IsPlaying)
        {
            this.Stop();
        }
        if ((base.enabled && base.gameObject.activeSelf) && base.gameObject.activeInHierarchy)
        {
            if (this.memberInfo == null)
            {
                throw new NullReferenceException("Animation target is NULL");
            }
            if (!this.memberInfo.IsValid)
            {
                object[] objArray1 = new object[] { "Invalid property binding configuration on ", this.getPath(base.gameObject.transform), " - ", this.target };
                throw new InvalidOperationException(string.Concat(objArray1));
            }
            this.target = this.memberInfo.GetProperty();
            base.StartCoroutine(this.Execute());
        }
    }

    public void PlayForward()
    {
        this.playDirection = PlayDirection.Forward;
        this.Play();
    }

    public void PlayReverse()
    {
        this.playDirection = PlayDirection.Reverse;
        this.Play();
    }

    public override void Reset()
    {
        List<string> list = (this.clip == null) ? null : this.clip.Sprites;
        if ((this.memberInfo.IsValid && (list != null)) && (list.Count > 0))
        {
            this.memberInfo.Component.SetProperty(this.memberInfo.MemberName, list[0]);
        }
        if (this.isRunning)
        {
            base.StopAllCoroutines();
            this.isRunning = false;
            this.isPaused = false;
            this.onReset();
            this.target = null;
        }
    }

    public void Resume()
    {
        if (this.isRunning && this.isPaused)
        {
            this.isPaused = false;
            this.onResumed();
        }
    }

    private void setFrame(int frameIndex)
    {
        List<string> sprites = this.clip.Sprites;
        if (sprites.Count != 0)
        {
            frameIndex = Mathf.Max(0, Mathf.Min(frameIndex, sprites.Count - 1));
            if (this.target != null)
            {
                this.target.Value = sprites[frameIndex];
            }
        }
    }

    public void Start()
    {
    }

    public override void Stop()
    {
        if (this.isRunning)
        {
            List<string> list = (this.clip == null) ? null : this.clip.Sprites;
            if (this.skipToEndOnStop && (list != null))
            {
                this.setFrame(Mathf.Max(list.Count - 1, 0));
            }
            base.StopAllCoroutines();
            this.isRunning = false;
            this.isPaused = false;
            this.onStopped();
            this.target = null;
        }
    }

    public bool AutoRun
    {
        get
        {
            return this.autoStart;
        }
        set
        {
            this.autoStart = value;
        }
    }

    public dfAnimationClip Clip
    {
        get
        {
            return this.clip;
        }
        set
        {
            this.clip = value;
        }
    }

    public PlayDirection Direction
    {
        get
        {
            return this.playDirection;
        }
        set
        {
            this.playDirection = value;
            if (this.IsPlaying)
            {
                this.Play();
            }
        }
    }

    public bool IsPaused
    {
        get
        {
            return (this.isRunning && this.isPaused);
        }
        set
        {
            if (value != this.IsPaused)
            {
                if (value)
                {
                    this.Pause();
                }
                else
                {
                    this.Resume();
                }
            }
        }
    }

    public override bool IsPlaying
    {
        get
        {
            return this.isRunning;
        }
    }

    public float Length
    {
        get
        {
            return this.length;
        }
        set
        {
            this.length = Mathf.Max(value, 0.03f);
        }
    }

    public dfTweenLoopType LoopType
    {
        get
        {
            return this.loopType;
        }
        set
        {
            this.loopType = value;
        }
    }

    public dfComponentMemberInfo Target
    {
        get
        {
            return this.memberInfo;
        }
        set
        {
            this.memberInfo = value;
        }
    }

    public override string TweenName
    {
        get
        {
            return this.animationName;
        }
        set
        {
            this.animationName = value;
        }
    }

    [CompilerGenerated]
    private sealed class <Execute>c__Iterator46 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal dfSpriteAnimation <>f__this;
        internal int <direction>__1;
        internal float <elapsed>__6;
        internal int <frameIndex>__7;
        internal int <lastFrameIndex>__2;
        internal int <maxFrameIndex>__4;
        internal List<string> <sprites>__3;
        internal float <startTime>__0;
        internal float <timeNow>__5;

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
                    if (((this.<>f__this.clip != null) && (this.<>f__this.clip.Sprites != null)) && (this.<>f__this.clip.Sprites.Count != 0))
                    {
                        this.<>f__this.isRunning = true;
                        this.<>f__this.isPaused = false;
                        this.<>f__this.onStarted();
                        this.<startTime>__0 = Time.realtimeSinceStartup;
                        this.<direction>__1 = (this.<>f__this.playDirection != dfSpriteAnimation.PlayDirection.Forward) ? -1 : 1;
                        this.<lastFrameIndex>__2 = (this.<direction>__1 != 1) ? (this.<>f__this.clip.Sprites.Count - 1) : 0;
                        this.<>f__this.setFrame(this.<lastFrameIndex>__2);
                        break;
                    }
                    goto Label_027B;

                case 1:
                    if (!this.<>f__this.IsPaused)
                    {
                        this.<sprites>__3 = this.<>f__this.clip.Sprites;
                        this.<maxFrameIndex>__4 = this.<sprites>__3.Count - 1;
                        this.<timeNow>__5 = Time.realtimeSinceStartup;
                        this.<elapsed>__6 = this.<timeNow>__5 - this.<startTime>__0;
                        this.<frameIndex>__7 = Mathf.RoundToInt(Mathf.Clamp01(this.<elapsed>__6 / this.<>f__this.length) * this.<maxFrameIndex>__4);
                        if (this.<elapsed>__6 >= this.<>f__this.length)
                        {
                            switch (this.<>f__this.loopType)
                            {
                                case dfTweenLoopType.Once:
                                    this.<>f__this.isRunning = false;
                                    this.<>f__this.onCompleted();
                                    goto Label_027B;

                                case dfTweenLoopType.Loop:
                                    this.<startTime>__0 = this.<timeNow>__5;
                                    this.<frameIndex>__7 = 0;
                                    break;

                                case dfTweenLoopType.PingPong:
                                    this.<startTime>__0 = this.<timeNow>__5;
                                    this.<direction>__1 *= -1;
                                    this.<frameIndex>__7 = 0;
                                    break;
                            }
                        }
                        if (this.<direction>__1 == -1)
                        {
                            this.<frameIndex>__7 = this.<maxFrameIndex>__4 - this.<frameIndex>__7;
                        }
                        if (this.<lastFrameIndex>__2 != this.<frameIndex>__7)
                        {
                            this.<lastFrameIndex>__2 = this.<frameIndex>__7;
                            this.<>f__this.setFrame(this.<frameIndex>__7);
                        }
                        break;
                    }
                    break;

                default:
                    goto Label_027B;
            }
            this.$current = null;
            this.$PC = 1;
            return true;
            this.$PC = -1;
        Label_027B:
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

    public enum PlayDirection
    {
        Forward,
        Reverse
    }
}

