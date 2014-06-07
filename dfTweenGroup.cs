using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable, AddComponentMenu("Daikon Forge/Tweens/Group")]
public class dfTweenGroup : dfTweenPlayableBase
{
    [SerializeField]
    protected string groupName = string.Empty;
    public TweenGroupMode Mode;
    public List<dfTweenPlayableBase> Tweens = new List<dfTweenPlayableBase>();

    public event TweenNotification TweenCompleted;

    public event TweenNotification TweenReset;

    public event TweenNotification TweenStarted;

    public event TweenNotification TweenStopped;

    public void DisableTween(string TweenName)
    {
        for (int i = 0; i < this.Tweens.Count; i++)
        {
            if ((this.Tweens[i] != null) && (this.Tweens[i].name == TweenName))
            {
                this.Tweens[i].enabled = false;
                break;
            }
        }
    }

    public void EnableTween(string TweenName)
    {
        for (int i = 0; i < this.Tweens.Count; i++)
        {
            if ((this.Tweens[i] != null) && (this.Tweens[i].TweenName == TweenName))
            {
                this.Tweens[i].enabled = true;
                break;
            }
        }
    }

    protected internal void onCompleted()
    {
        base.SendMessage("TweenCompleted", this, SendMessageOptions.DontRequireReceiver);
        if (this.TweenCompleted != null)
        {
            this.TweenCompleted();
        }
    }

    protected internal void onReset()
    {
        base.SendMessage("TweenReset", this, SendMessageOptions.DontRequireReceiver);
        if (this.TweenReset != null)
        {
            this.TweenReset();
        }
    }

    protected internal void onStarted()
    {
        base.SendMessage("TweenStarted", this, SendMessageOptions.DontRequireReceiver);
        if (this.TweenStarted != null)
        {
            this.TweenStarted();
        }
    }

    protected internal void onStopped()
    {
        base.SendMessage("TweenStopped", this, SendMessageOptions.DontRequireReceiver);
        if (this.TweenStopped != null)
        {
            this.TweenStopped();
        }
    }

    public override void Play()
    {
        if (this.IsPlaying)
        {
            this.Stop();
        }
        this.onStarted();
        if (this.Mode == TweenGroupMode.Concurrent)
        {
            base.StartCoroutine(this.runConcurrent());
        }
        else
        {
            base.StartCoroutine(this.runSequence());
        }
    }

    public override void Reset()
    {
        if (this.IsPlaying)
        {
            base.StopAllCoroutines();
            for (int i = 0; i < this.Tweens.Count; i++)
            {
                if (this.Tweens[i] != null)
                {
                    this.Tweens[i].Reset();
                }
            }
            this.onReset();
        }
    }

    [HideInInspector, DebuggerHidden]
    private IEnumerator runConcurrent()
    {
        return new <runConcurrent>c__Iterator49 { <>f__this = this };
    }

    [HideInInspector, DebuggerHidden]
    private IEnumerator runSequence()
    {
        return new <runSequence>c__Iterator48 { <>f__this = this };
    }

    public override void Stop()
    {
        if (this.IsPlaying)
        {
            base.StopAllCoroutines();
            for (int i = 0; i < this.Tweens.Count; i++)
            {
                if (this.Tweens[i] != null)
                {
                    this.Tweens[i].Stop();
                }
            }
            this.onStopped();
        }
    }

    private void Update()
    {
    }

    public override bool IsPlaying
    {
        get
        {
            for (int i = 0; i < this.Tweens.Count; i++)
            {
                if (((this.Tweens[i] != null) && this.Tweens[i].enabled) && this.Tweens[i].IsPlaying)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public override string TweenName
    {
        get
        {
            return this.groupName;
        }
        set
        {
            this.groupName = value;
        }
    }

    [CompilerGenerated]
    private sealed class <runConcurrent>c__Iterator49 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        private static Func<dfTweenPlayableBase, bool> <>f__am$cache4;
        internal dfTweenGroup <>f__this;
        internal int <i>__0;

        private static bool <>m__35(dfTweenPlayableBase tween)
        {
            return ((tween != null) && tween.IsPlaying);
        }

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
                    this.<i>__0 = 0;
                    while (this.<i>__0 < this.<>f__this.Tweens.Count)
                    {
                        if ((this.<>f__this.Tweens[this.<i>__0] != null) && this.<>f__this.Tweens[this.<i>__0].enabled)
                        {
                            this.<>f__this.Tweens[this.<i>__0].Play();
                        }
                        this.<i>__0++;
                    }
                    break;

                case 1:
                    if (<>f__am$cache4 == null)
                    {
                        <>f__am$cache4 = new Func<dfTweenPlayableBase, bool>(dfTweenGroup.<runConcurrent>c__Iterator49.<>m__35);
                    }
                    if (Enumerable.Any<dfTweenPlayableBase>(this.<>f__this.Tweens, <>f__am$cache4))
                    {
                        break;
                    }
                    this.<>f__this.onCompleted();
                    this.$PC = -1;
                    goto Label_010E;

                default:
                    goto Label_010E;
            }
            this.$current = null;
            this.$PC = 1;
            return true;
        Label_010E:
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

    [CompilerGenerated]
    private sealed class <runSequence>c__Iterator48 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal dfTweenGroup <>f__this;
        internal int <i>__0;
        internal dfTweenPlayableBase <tween>__1;

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
                    this.<i>__0 = 0;
                    goto Label_00D0;

                case 1:
                    break;

                default:
                    goto Label_00FD;
            }
        Label_00B2:
            while (this.<tween>__1.IsPlaying)
            {
                this.$current = null;
                this.$PC = 1;
                return true;
            }
        Label_00C2:
            this.<i>__0++;
        Label_00D0:
            if (this.<i>__0 < this.<>f__this.Tweens.Count)
            {
                if ((this.<>f__this.Tweens[this.<i>__0] == null) || !this.<>f__this.Tweens[this.<i>__0].enabled)
                {
                    goto Label_00C2;
                }
                this.<tween>__1 = this.<>f__this.Tweens[this.<i>__0];
                this.<tween>__1.Play();
                goto Label_00B2;
            }
            this.<>f__this.onCompleted();
            this.$PC = -1;
        Label_00FD:
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

    public enum TweenGroupMode
    {
        Concurrent,
        Sequence
    }
}

