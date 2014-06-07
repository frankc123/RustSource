using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

public class LightStylist : MonoBehaviour
{
    [SerializeField]
    protected LightStyle _lightStyle;
    [SerializeField, HideInInspector]
    protected LightStyle.Mod.Mask _mask = (LightStyle.Mod.Mask.SpotAngle | LightStyle.Mod.Mask.Range | LightStyle.Mod.Mask.Intensity | LightStyle.Mod.Mask.Alpha | LightStyle.Mod.Mask.Blue | LightStyle.Mod.Mask.Green | LightStyle.Mod.Mask.Red);
    private bool awoke;
    private Dictionary<LightStyle, Clip> clips;
    private int clipsInSortingArray;
    private float crossfadeLength;
    private LightStyle crossfadeNextFrame;
    private LightStyle crossfadeThisFrame;
    private const float kDefaultFadeLength = 0.3f;
    [SerializeField]
    protected Light[] lights;
    protected LightStyle.Simulation simulationIdle;
    protected LightStyle.Simulation simulationPlaying;
    private Clip[] sortingArray;

    private void Awake()
    {
        if (!this.awoke)
        {
            this.clips = new Dictionary<LightStyle, Clip>();
            this.awoke = true;
        }
    }

    public bool Blend(LightStyle style)
    {
        return this.Blend(style, 1f, 0.3f);
    }

    public bool Blend(LightStyle style, float targetWeight)
    {
        return this.Blend(style, targetWeight, 0.3f);
    }

    public bool Blend(LightStyle style, float targetWeight, float fadeLength)
    {
        if (fadeLength <= 0f)
        {
            this.Play(style);
            return true;
        }
        targetWeight = Mathf.Clamp01(targetWeight);
        if (style == this._lightStyle)
        {
            float num;
            float current = this.CalculateSumWeight(true, out num);
            if (Mathf.Approximately(1f - current, targetWeight))
            {
                return true;
            }
            float num3 = Mathf.MoveTowards(current, 1f - targetWeight, Time.deltaTime / fadeLength);
            if (num3 <= 0f)
            {
                foreach (Clip clip in this.clips.Values)
                {
                    clip.weight = 0f;
                }
            }
            else
            {
                float num4 = num3 / current;
                foreach (Clip clip2 in this.clips.Values)
                {
                    clip2.weight *= num4;
                }
            }
        }
        else
        {
            float num5;
            Clip orMakeClip = this.GetOrMakeClip(style);
            if (Mathf.Approximately(orMakeClip.weight, targetWeight))
            {
                return true;
            }
            orMakeClip.weight = Mathf.MoveTowards(orMakeClip.weight, targetWeight, Time.deltaTime / fadeLength);
            float num6 = this.CalculateSumWeight(false, out num5);
            if ((num6 != orMakeClip.weight) && (num6 > 1f))
            {
                float num7 = num6 - orMakeClip.weight;
                foreach (Clip clip4 in this.clips.Values)
                {
                    if (clip4 != orMakeClip)
                    {
                        clip4.weight /= num7;
                        clip4.weight *= 1f - orMakeClip.weight;
                    }
                }
            }
        }
        return false;
    }

    private float CalculateSumWeight(bool normalize, out float maxWeight)
    {
        float num = 0f;
        maxWeight = 0f;
        foreach (Clip clip in this.clips.Values)
        {
            if (clip.weight > maxWeight)
            {
                maxWeight = clip.weight;
            }
            else if (clip.weight < 0f)
            {
                clip.weight = 0f;
            }
            num += clip.weight;
        }
        if (!normalize || (num <= 1f))
        {
            return num;
        }
        float num2 = num;
        maxWeight /= num2;
        foreach (Clip clip2 in this.clips.Values)
        {
            clip2.weight /= num2;
        }
        return 1f;
    }

    public bool CrossFade(LightStyle style)
    {
        return this.CrossFade(style, 0.3f);
    }

    public bool CrossFade(LightStyle style, float fadeLength)
    {
        if (this.crossfadeThisFrame != style)
        {
            this.crossfadeThisFrame = style;
            this.crossfadeNextFrame = null;
            this.crossfadeLength = fadeLength;
            if (this.Blend(style, 1f, fadeLength))
            {
                this.CrossFadeDone();
                return true;
            }
        }
        return false;
    }

    private void CrossFadeDone()
    {
        Clip clip;
        if (this.clips.TryGetValue(this.crossfadeThisFrame, out clip))
        {
            this.clips.Remove(this.style);
            this.GetOrMakeClip(this._lightStyle).weight = 0f;
            this._lightStyle = this.style;
            this.simulationIdle = clip.simulation;
        }
        this.crossfadeThisFrame = null;
        this.crossfadeNextFrame = null;
    }

    public void EnsureAwake()
    {
        this.Awake();
    }

    private Clip GetOrMakeClip(LightStyle style)
    {
        Clip clip;
        if (!this.clips.TryGetValue(style, out clip))
        {
            clip = new Clip {
                simulation = style.CreateSimulation(LightStyle.time, this)
            };
            this.clips[style] = clip;
        }
        return clip;
    }

    protected void LateUpdate()
    {
        float num;
        LightStyle.Mod mod;
        if (this.crossfadeThisFrame != null)
        {
            this.crossfadeNextFrame = this.crossfadeThisFrame;
            this.crossfadeThisFrame = null;
        }
        else if ((this.crossfadeNextFrame != null) && !this.CrossFade(this.crossfadeNextFrame, this.crossfadeLength))
        {
            this.crossfadeNextFrame = this.crossfadeThisFrame;
            this.crossfadeThisFrame = null;
        }
        float num2 = this.CalculateSumWeight(true, out num);
        if (num2 == 0f)
        {
            while (this.clipsInSortingArray > 0)
            {
                this.sortingArray[--this.clipsInSortingArray] = null;
            }
            if (this._lightStyle == null)
            {
                return;
            }
            mod = this.simulationIdle.BindMod(this._mask);
        }
        else
        {
            int count = this.clips.Count;
            if (this.clipsInSortingArray != count)
            {
                if (this.clipsInSortingArray > count)
                {
                    while (this.clipsInSortingArray > count)
                    {
                        this.sortingArray[--this.clipsInSortingArray] = null;
                    }
                }
                else if ((this.sortingArray == null) || (this.sortingArray.Length < count))
                {
                    Array.Resize<Clip>(ref this.sortingArray, ((count / 4) + (((count % 4) != 0) ? 1 : 2)) * 4);
                }
            }
            int num4 = 0;
            foreach (Clip clip in this.clips.Values)
            {
                if (clip.weight > 0f)
                {
                    this.sortingArray[num4++] = clip;
                }
            }
            if (this.clipsInSortingArray < num4)
            {
                this.clipsInSortingArray = num4;
            }
            else
            {
                while (this.clipsInSortingArray > num4)
                {
                    this.sortingArray[--this.clipsInSortingArray] = null;
                }
            }
            Array.Sort<Clip>(this.sortingArray, 0, this.clipsInSortingArray);
            float weight = this.sortingArray[0].weight;
            mod = this.sortingArray[0].simulation.BindMod(this._mask);
            for (int i = 1; i < this.clipsInSortingArray; i++)
            {
                Clip clip2 = this.sortingArray[i];
                weight += clip2.weight;
                mod = LightStyle.Mod.Lerp(mod, clip2.simulation.BindMod(this._mask), clip2.weight / weight, this._mask);
            }
            if (this._lightStyle != null)
            {
                LightStyle.Mod b = this.simulationIdle.BindMod(this._mask);
                if (num2 < 1f)
                {
                    mod = LightStyle.Mod.Lerp(mod, b, 1f - num2, this._mask);
                }
                else
                {
                    mod |= b;
                }
            }
        }
        foreach (Light light in this.lights)
        {
            if (light != null)
            {
                mod.ApplyTo(light, this._mask);
            }
        }
    }

    public void Play(LightStyle style)
    {
        if (style == this._lightStyle)
        {
            this.clips.Clear();
        }
        else
        {
            Clip orMakeClip = this.GetOrMakeClip(style);
            this.clips.Clear();
            this.clips[style] = orMakeClip;
            orMakeClip.weight = 1f;
            orMakeClip.simulation.ResetTime(LightStyle.time);
        }
    }

    public void Play(LightStyle style, double time)
    {
        if (style == this._lightStyle)
        {
            this.clips.Clear();
        }
        else
        {
            Clip orMakeClip = this.GetOrMakeClip(style);
            this.clips.Clear();
            this.clips[style] = orMakeClip;
            orMakeClip.weight = 1f;
            orMakeClip.simulation.ResetTime(time);
        }
    }

    protected void Reset()
    {
        this.lights = base.GetComponents<Light>();
    }

    private void Start()
    {
        if (this._lightStyle == null)
        {
            this._lightStyle = LightStyleDefault.Singleton;
        }
        this.simulationIdle = this._lightStyle.CreateSimulation(LightStyle.time, this);
    }

    public LightStylist ensuredAwake
    {
        get
        {
            this.Awake();
            return this;
        }
    }

    public LightStyle style
    {
        get
        {
            return this._lightStyle;
        }
        set
        {
            if (this._lightStyle == value)
            {
                if ((value != null) && ((this.simulationIdle == null) || this.simulationIdle.disposed))
                {
                    this.simulationIdle = this._lightStyle.CreateSimulation(LightStyle.time, this);
                }
            }
            else
            {
                if (this._lightStyle != null)
                {
                    this.simulationIdle.Dispose();
                    this.simulationIdle = null;
                }
                else if (this.simulationIdle != null)
                {
                    this.simulationIdle.Dispose();
                    this.simulationIdle = null;
                }
                this._lightStyle = value;
                if (this._lightStyle != null)
                {
                    this.simulationIdle = this._lightStyle.CreateSimulation(LightStyle.time, this);
                }
            }
        }
    }

    public IEnumerable<float> Weights
    {
        get
        {
            return new <>c__Iterator40 { <>f__this = this, $PC = -2 };
        }
    }

    [CompilerGenerated]
    private sealed class <>c__Iterator40 : IDisposable, IEnumerator, IEnumerable, IEnumerable<float>, IEnumerator<float>
    {
        internal float $current;
        internal int $PC;
        internal Dictionary<LightStyle, LightStylist.Clip>.ValueCollection.Enumerator <$s_492>__0;
        internal LightStylist <>f__this;
        internal LightStylist.Clip <clip>__1;

        [DebuggerHidden]
        public void Dispose()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            switch (num)
            {
                case 1:
                    try
                    {
                    }
                    finally
                    {
                        this.<$s_492>__0.Dispose();
                    }
                    break;
            }
        }

        public bool MoveNext()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            bool flag = false;
            switch (num)
            {
                case 0:
                    this.<$s_492>__0 = this.<>f__this.clips.Values.GetEnumerator();
                    num = 0xfffffffd;
                    break;

                case 1:
                    break;

                default:
                    goto Label_00B3;
            }
            try
            {
                while (this.<$s_492>__0.MoveNext())
                {
                    this.<clip>__1 = this.<$s_492>__0.Current;
                    this.$current = this.<clip>__1.weight;
                    this.$PC = 1;
                    flag = true;
                    return true;
                }
            }
            finally
            {
                if (!flag)
                {
                }
                this.<$s_492>__0.Dispose();
            }
            this.$PC = -1;
        Label_00B3:
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        IEnumerator<float> IEnumerable<float>.GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
            {
                return this;
            }
            return new LightStylist.<>c__Iterator40 { <>f__this = this.<>f__this };
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.System.Collections.Generic.IEnumerable<float>.GetEnumerator();
        }

        float IEnumerator<float>.Current
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

    protected sealed class Clip : IComparable<LightStylist.Clip>
    {
        public LightStyle.Simulation simulation;
        public float weight;

        public int CompareTo(LightStylist.Clip other)
        {
            return this.weight.CompareTo(other.weight);
        }
    }
}

