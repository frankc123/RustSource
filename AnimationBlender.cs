using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

public static class AnimationBlender
{
    public static ChannelConfig Alias(this ChannelField Field, string Name)
    {
        return new ChannelConfig(Name, Field);
    }

    public static MixerConfig Alias(this ResidualField ResidualField, Animation Animation, params ChannelConfig[] ChannelAliases)
    {
        return new MixerConfig(Animation, ResidualField, ChannelAliases);
    }

    public static MixerConfig Alias(this ResidualField ResidualField, Animation Animation, int ChannelCount)
    {
        return new MixerConfig(Animation, ResidualField, new ChannelConfig[ChannelCount]);
    }

    public static ChannelConfig[] Alias(this ChannelField Field, ChannelConfig[] Array, int Index, string Name)
    {
        Array[Index] = Field.Alias(Name);
        return Array;
    }

    private static void ArrayResize<T>(ref T[] array, int size)
    {
        Array.Resize<T>(ref array, size);
    }

    public static Mixer Create(this MixerConfig Config)
    {
        return new Mixer(Config);
    }

    public static ChannelConfig[] Define(this ChannelConfig[] Array, int Index, string Name, ChannelField Field)
    {
        return Field.Alias(Array, Index, Name);
    }

    private static int GetClear(ref int value)
    {
        int num = value;
        value = 0;
        return num;
    }

    private static void OneWeight(ref WeightUnit weight)
    {
        weight.raw = weight.scaled = weight.normalized = 1f;
    }

    private static void OneWeightScale(ref WeightUnit weight)
    {
        weight.scaled = weight.normalized = 1f;
    }

    private static void SetWeight(ref WeightUnit weight)
    {
        weight.scaled = weight.raw;
        weight.normalized = 1f;
    }

    private static opt<T> to_opt<T>(T? nullable) where T: struct
    {
        return (nullable.HasValue ? ((opt<T>) nullable.Value) : opt<T>.none);
    }

    private static Weighted<T>[] WeightArray<T>(int size)
    {
        return new Weighted<T>[size];
    }

    private static Weighted<T>[] WeightArray<T>(T[] source)
    {
        if (object.ReferenceEquals(source, null))
        {
            return null;
        }
        int length = source.Length;
        Weighted<T>[] weightedArray = WeightArray<T>(length);
        for (int i = 0; i < length; i++)
        {
            weightedArray[i].value = source[i];
        }
        return weightedArray;
    }

    private static bool WeightOf<T>(Weighted<T>[] items, int[] index, out WeightResult result)
    {
        float num3;
        float num4;
        bool flag;
        float num = 0f;
        float num2 = 0f;
        int num7 = -1;
        int num8 = 0;
        int num9 = 0;
        int num10 = index.Length - 1;
        while (num9 <= num10)
        {
            float raw = items[index[num9]].weight.raw;
            if (raw <= 0f)
            {
                ZeroWeight(ref items[index[num9]].weight);
                int num11 = index[num10];
                index[num10--] = index[num9];
                index[num9] = num11;
            }
            else
            {
                num8++;
                if (raw >= 1f)
                {
                    raw = 1f;
                    OneWeight(ref items[index[num9]].weight);
                }
                else
                {
                    SetWeight(ref items[index[num9]].weight);
                }
                num += raw;
                if (raw > num2)
                {
                    num2 = raw;
                    num7 = num9;
                }
                num9++;
            }
        }
        if (num7 == -1)
        {
            num3 = 0f;
            num4 = 0f;
            flag = false;
        }
        else
        {
            flag = true;
            if (num8 == 1)
            {
                num3 = 1f;
                num4 = 1f;
                OneWeightScale(ref items[index[0]].weight);
            }
            else
            {
                float num6;
                if (num2 < 1f)
                {
                    num3 = 0f;
                    num6 = 1f / num2;
                    for (int i = 0; i < num8; i++)
                    {
                        num3 += items[index[i]].weight.SetScaledRecip(num6);
                    }
                }
                else
                {
                    num3 = num;
                }
                if (num3 == 1f)
                {
                    num4 = 1f;
                }
                else
                {
                    num4 = 0f;
                    num6 = 1f / num3;
                    for (int j = 0; j < num8; j++)
                    {
                        num4 += items[index[j]].weight.SetNormalizedRecip(num6);
                    }
                }
            }
        }
        result.count = num8;
        result.winner = num7;
        result.sum.raw = num;
        result.sum.scaled = num3;
        result.sum.normalized = num4;
        return flag;
    }

    private static void ZeroWeight(ref WeightUnit weight)
    {
        weight.raw = weight.scaled = weight.normalized = 0f;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Channel
    {
        [NonSerialized]
        public AnimationBlender.ChannelField field;
        [NonSerialized]
        public string name;
        [NonSerialized]
        public bool active;
        [NonSerialized]
        public bool valid;
        [NonSerialized]
        public bool wasActive;
        [NonSerialized]
        public bool startedTransition;
        [NonSerialized]
        public AnimationBlender.ChannelCurve induce;
        [NonSerialized]
        public AnimationBlender.ChannelCurve reduce;
        [NonSerialized]
        public int index;
        [NonSerialized]
        public int animationIndex;
        [NonSerialized]
        public float maxBlend;
        [NonSerialized]
        public float playbackRate;
        [NonSerialized]
        public float startTime;
        public Channel(int index, int animationIndex, string name, AnimationBlender.ChannelField field)
        {
            this.index = index;
            this.animationIndex = animationIndex;
            this.name = name;
            this.field = field;
            this.induce = new AnimationBlender.ChannelCurve(field.inCurveInfo, new AnimationBlender.State(), new AnimationBlender.Influence(), field, true);
            this.reduce = new AnimationBlender.ChannelCurve(field.outCurveInfo, new AnimationBlender.State(), new AnimationBlender.Influence(), field, false);
            this.active = false;
            this.wasActive = false;
            this.startedTransition = false;
            this.valid = animationIndex != -1;
            this.maxBlend = (field.residualBlend > 0f) ? ((field.residualBlend < 1f) ? (1f - field.residualBlend) : 0f) : 1f;
            this.startTime = field.startFrame;
            this.playbackRate = field.playbackRate;
        }

        private bool StartTransition(ref AnimationBlender.ChannelCurve from, ref AnimationBlender.ChannelCurve to, ref float dt, bool startNow)
        {
            if ((to.state.delay == 0f) && startNow)
            {
                to.state.delay = to.delayDuration;
            }
            if (to.state.delay > dt)
            {
                to.state.delay -= dt;
                return false;
            }
            dt -= to.state.delay;
            to.state.delay = 0f;
            to.influence.percent = 0f;
            to.influence.duration = from.state.percent * to.info.duration;
            to.influence.value = from.state.value;
            to.influence.active = from.state.percent > 0f;
            to.influence.timeleft = to.influence.duration;
            from.state.delay = to.delayDuration;
            from.state.active = false;
            to.state.active = true;
            if (to.induces)
            {
                from.state.time = from.info.start;
                from.state.percent = 0f;
            }
            return true;
        }

        private float Step(bool transitioning, ref AnimationBlender.ChannelCurve from, ref AnimationBlender.ChannelCurve to, ref float dt)
        {
            if (transitioning && (to.state.delay > 0f))
            {
                return from.state.value;
            }
            float num = dt;
            float num2 = dt;
            float time = to.state.time;
            if (to.induces)
            {
                to.state.time += dt;
                if (to.state.time >= to.info.end)
                {
                    num = to.state.time - to.info.end;
                    to.state.time = to.info.end;
                    to.state.percent = 1f;
                    from.state.delay = from.delayDuration;
                }
                else
                {
                    num = 0f;
                    to.state.percent = to.info.TimeToPercent(to.state.time);
                }
            }
            else if (to.influence.duration == 0f)
            {
                num = dt;
                to.state.percent = 1f;
                to.state.time = to.info.end;
            }
            else
            {
                float num4 = from.info.duration / to.influence.duration;
                to.state.time += dt * num4;
                if (to.state.time >= to.info.end)
                {
                    num = (to.state.time - to.info.end) / num4;
                    to.state.percent = 1f;
                    to.state.time = to.info.end;
                }
                else
                {
                    num = 0f;
                    to.state.percent = to.info.TimeToPercent(to.state.time);
                }
            }
            float num5 = to.info.SampleTime(to.state.time);
            if (to.influence.active)
            {
                if (to.induces)
                {
                    if (to.influence.timeleft > dt)
                    {
                        to.influence.timeleft -= dt;
                        num2 = 0f;
                        to.influence.percent = to.influence.timeleft / to.influence.duration;
                    }
                    else
                    {
                        num2 = dt - to.influence.timeleft;
                        to.influence.timeleft = 0f;
                        to.influence.percent = 0f;
                        to.influence.active = false;
                    }
                }
                else if ((to.state.percent >= 1f) && to.influence.active)
                {
                    to.influence.active = false;
                    from.state = new AnimationBlender.State();
                }
            }
            if (to.induces)
            {
                to.state.value = !to.influence.active ? num5 : (num5 + ((to.influence.value - num5) * to.influence.percent));
            }
            else
            {
                to.state.value = to.influence.value * num5;
            }
            if (num2 < num)
            {
                dt = num2;
            }
            else
            {
                dt = num;
            }
            return to.state.value;
        }

        public float Update(float dt)
        {
            bool transitioning = this.active != this.wasActive;
            if (transitioning)
            {
                bool flag3;
                bool startNow = this.startedTransition != this.active;
                this.startedTransition = this.active;
                if (this.active)
                {
                    flag3 = this.StartTransition(ref this.reduce, ref this.induce, ref dt, startNow);
                }
                else
                {
                    flag3 = this.StartTransition(ref this.induce, ref this.reduce, ref dt, startNow);
                }
                if (flag3)
                {
                    transitioning = false;
                    this.wasActive = this.active;
                }
            }
            if (this.wasActive)
            {
                return this.Step(transitioning, ref this.reduce, ref this.induce, ref dt);
            }
            return this.Step(transitioning, ref this.induce, ref this.reduce, ref dt);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ChannelConfig
    {
        [NonSerialized]
        public readonly string name;
        [NonSerialized]
        public readonly AnimationBlender.ChannelField field;
        public ChannelConfig(string name, AnimationBlender.ChannelField field)
        {
            this.name = name;
            this.field = field;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct ChannelCurve
    {
        [NonSerialized]
        public AnimationBlender.CurveInfo info;
        [NonSerialized]
        public AnimationBlender.State state;
        [NonSerialized]
        public AnimationBlender.Influence influence;
        [NonSerialized]
        public float delayDuration;
        [NonSerialized]
        public bool induces;
        public ChannelCurve(AnimationBlender.CurveInfo info, AnimationBlender.State state, AnimationBlender.Influence influence, AnimationBlender.ChannelField field, bool induces)
        {
            this.info = info;
            this.state = state;
            this.influence = influence;
            this.induces = induces;
            this.delayDuration = !induces ? field.outDelay : field.inDelay;
        }
    }

    [Serializable]
    public sealed class ChannelField : AnimationBlender.Field
    {
        [SerializeField]
        public bool blockedByAnimation;
        [SerializeField]
        public AnimationCurve inCurve;
        [SerializeField]
        public float inDelay;
        [SerializeField]
        public AnimationCurve outCurve;
        [SerializeField]
        public float outDelay;
        [SerializeField]
        public float residualBlend;

        public ChannelField()
        {
            base.clipName = string.Empty;
            base.playbackRate = 1f;
            this.inCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
            this.outCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);
        }

        public AnimationBlender.CurveInfo inCurveInfo
        {
            get
            {
                return new AnimationBlender.CurveInfo(this.inCurve);
            }
        }

        public AnimationBlender.CurveInfo outCurveInfo
        {
            get
            {
                return new AnimationBlender.CurveInfo(this.outCurve);
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CurveInfo
    {
        [NonSerialized]
        public AnimationCurve curve;
        [NonSerialized]
        public int length;
        [NonSerialized]
        public float start;
        [NonSerialized]
        public float firstTime;
        [NonSerialized]
        public float end;
        [NonSerialized]
        public float lastTime;
        [NonSerialized]
        public float duration;
        [NonSerialized]
        public float percentRate;
        public CurveInfo(AnimationCurve curve)
        {
            this.curve = curve;
            this.length = curve.length;
            if (this.length == 0)
            {
                this.start = this.firstTime = this.end = this.lastTime = this.duration = 0f;
                this.percentRate = float.PositiveInfinity;
            }
            else
            {
                Keyframe keyframe = curve[0];
                this.firstTime = keyframe.time;
                this.end = this.lastTime = (this.length != 1) ? curve[this.length - 1].time : this.firstTime;
                this.start = (this.firstTime >= 0f) ? 0f : this.firstTime;
                if (this.end < this.start)
                {
                    this.end = this.start;
                    this.start = this.lastTime;
                }
                this.duration = this.end - this.start;
                this.percentRate = 1f / this.duration;
            }
        }

        public float TimeToPercentClamped(float time)
        {
            return ((time < this.end) ? ((time > this.start) ? ((time - this.start) / this.duration) : 1f) : 1f);
        }

        public float TimeToPercent(float time)
        {
            return ((time - this.start) / this.duration);
        }

        public float PercentToTimeClamped(float percent)
        {
            return ((percent > 0f) ? ((percent < 1f) ? (this.start + (this.duration * percent)) : this.end) : this.start);
        }

        public float PercentToTime(float percent)
        {
            return (this.start + (this.duration * percent));
        }

        public float TimeClamp(float time)
        {
            return ((time < this.end) ? ((time > this.start) ? time : this.start) : this.end);
        }

        public float PercentClamp(float percent)
        {
            return ((percent > 0f) ? ((percent < 1f) ? percent : 1f) : 0f);
        }

        public float SampleTime(float time)
        {
            return this.curve.Evaluate(time);
        }

        public float SamplePercent(float percent)
        {
            return this.SampleTime(this.PercentToTime(percent));
        }

        public float SampleTimeClamped(float time)
        {
            return this.SampleTime(this.TimeClamp(time));
        }

        public float SamplePercentClamped(float percent)
        {
            return this.SamplePercent(this.PercentToTimeClamped(percent));
        }
    }

    [Serializable]
    public class Field
    {
        [SerializeField]
        public string clipName;
        [SerializeField]
        public float playbackRate;
        [SerializeField]
        public float startFrame;

        public bool defined
        {
            get
            {
                return !string.IsNullOrEmpty(this.clipName);
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Influence
    {
        [NonSerialized]
        public bool active;
        [NonSerialized]
        public float value;
        [NonSerialized]
        public float percent;
        [NonSerialized]
        public float timeleft;
        [NonSerialized]
        public float duration;
    }

    public sealed class Mixer
    {
        [NonSerialized]
        private Animation animation;
        [NonSerialized]
        private bool animationBlocking;
        [NonSerialized]
        private AnimationBlender.TrackerBlender blender;
        [NonSerialized]
        private AnimationBlendMode channelBlendMode;
        [NonSerialized]
        private int channelCount;
        [NonSerialized]
        private AnimationBlender.Weighted<AnimationBlender.Channel>[] channels;
        [NonSerialized]
        private int definedChannelCount;
        [NonSerialized]
        private int[] definedChannels;
        [NonSerialized]
        private bool hasOneShotBlendIn;
        [NonSerialized]
        private bool hasResidual;
        [NonSerialized]
        private Dictionary<string, int> nameToChannel;
        [NonSerialized]
        private AnimationState oneShotAnimation;
        [NonSerialized]
        private AnimationBlender.CurveInfo oneShotBlendIn;
        [NonSerialized]
        private float oneShotBlendInTime;
        [NonSerialized]
        private bool playingOneShot;
        [NonSerialized]
        private Queue<string> queuedAnimations = new Queue<string>();
        [NonSerialized]
        private AnimationBlendMode residualBlendMode;
        [NonSerialized]
        private AnimationBlender.ResidualField residualField;
        [NonSerialized]
        private AnimationState residualState;
        [NonSerialized]
        private float sumWeight;
        [NonSerialized]
        private int trackerCount;
        [NonSerialized]
        private AnimationBlender.Weighted<AnimationBlender.Tracker>[] trackers;

        public Mixer(AnimationBlender.MixerConfig config)
        {
            if (config.animation == null)
            {
                throw new ArgumentException("null or missing", "config.animation");
            }
            this.animation = config.animation;
            this.residualField = config.residual;
            this.hasResidual = (!object.ReferenceEquals(config.residual, null) && config.residual.defined) && ((bool) this.animation.GetClip(config.residual.clipName));
            this.oneShotBlendIn = (!this.hasResidual || object.ReferenceEquals(config.residual.introCurve, null)) ? new AnimationBlender.CurveInfo() : config.residual.introCurveInfo;
            this.hasOneShotBlendIn = this.oneShotBlendIn.duration > 0f;
            this.residualState = !this.hasResidual ? null : this.animation[config.residual.clipName];
            this.channelCount = config.channels.Length;
            this.channels = new AnimationBlender.Weighted<AnimationBlender.Channel>[this.channelCount];
            this.trackers = new AnimationBlender.Weighted<AnimationBlender.Tracker>[this.channelCount];
            this.trackerCount = 0;
            this.nameToChannel = new Dictionary<string, int>(this.channelCount);
            for (int i = 0; i < this.channelCount; i++)
            {
                AnimationClip clip;
                AnimationBlender.ChannelField field = config.channels[i].field;
                string name = config.channels[i].name;
                this.nameToChannel.Add(name, i);
                int index = -1;
                if (field.defined && ((clip = this.animation.GetClip(field.clipName)) != null))
                {
                    bool flag = false;
                    while (!flag)
                    {
                        if (flag = ++index == this.trackerCount)
                        {
                            this.trackers[index].value.clip = clip;
                            this.trackers[index].value.state = this.animation[field.clipName];
                            this.trackers[index].value.channelCount = 1;
                            this.trackerCount++;
                        }
                        else if (flag = this.trackers[index].value.clip == clip)
                        {
                            this.trackers[index].value.channelCount++;
                        }
                    }
                    this.definedChannelCount++;
                }
                this.channels[i].value = new AnimationBlender.Channel(i, index, name, field);
            }
            for (int j = 0; j < this.trackerCount; j++)
            {
                this.trackers[j].value.channels = new int[AnimationBlender.GetClear(ref this.trackers[j].value.channelCount)];
            }
            this.definedChannels = new int[AnimationBlender.GetClear(ref this.definedChannelCount)];
            for (int k = 0; k < this.channelCount; k++)
            {
                if (this.channels[k].value.animationIndex != -1)
                {
                    this.trackers[this.channels[k].value.animationIndex].value.channels[this.trackers[this.channels[k].value.animationIndex].value.channelCount++] = this.definedChannels[this.definedChannelCount++] = k;
                }
            }
            AnimationBlender.ArrayResize<AnimationBlender.Weighted<AnimationBlender.Tracker>>(ref this.trackers, this.trackerCount);
            AnimationBlender.ArrayResize<AnimationBlender.Weighted<AnimationBlender.Channel>>(ref this.channels, this.channelCount);
            AnimationBlender.ArrayResize<int>(ref this.definedChannels, this.definedChannelCount);
            for (int m = 0; m < this.trackerCount; m++)
            {
                AnimationBlender.ArrayResize<int>(ref this.trackers[m].value.channels, this.trackers[m].value.channelCount);
            }
            this.blender = new AnimationBlender.TrackerBlender(this.trackerCount);
            if (this.hasResidual)
            {
                if (this.residualField.changeAnimLayer)
                {
                    this.residualState.layer = this.residualField.animLayer;
                    for (int num6 = 0; num6 < this.trackerCount; num6++)
                    {
                        this.trackers[num6].value.state.layer = this.residualField.animLayer;
                    }
                }
                this.residualState.blendMode = this.residualBlendMode = this.residualField.residualBlend;
                this.channelBlendMode = this.residualField.channelBlend;
                for (int n = 0; n < this.trackerCount; n++)
                {
                    this.trackers[n].value.state.blendMode = this.channelBlendMode;
                }
            }
        }

        private float BindTracker(ref AnimationBlender.Weighted<AnimationBlender.Tracker> tracker, float externalBlend)
        {
            float normalized = tracker.weight.normalized;
            if (this.hasResidual)
            {
                normalized *= tracker.value.blendFraction;
            }
            if (this.blender.trackerWeight.sum.raw < 1f)
            {
                normalized *= this.blender.trackerWeight.sum.raw;
            }
            if (normalized > 0f)
            {
                if (!tracker.value.wasEnabled)
                {
                    tracker.value.state.enabled = tracker.value.wasEnabled = true;
                    tracker.value.state.time = tracker.value.startTime;
                }
                tracker.value.state.weight = normalized * externalBlend;
                tracker.value.state.speed = tracker.value.playbackRate;
                return normalized;
            }
            if (tracker.value.wasEnabled)
            {
                this.DisableTracker(ref tracker.value);
            }
            return normalized;
        }

        public bool CrossFade(string animationName)
        {
            return this.CrossFadeOpt(animationName, AnimationBlender.opt<float>.none, AnimationBlender.opt<PlayMode>.none, AnimationBlender.opt<float>.none, AnimationBlender.opt<float>.none);
        }

        public bool CrossFade(string animationName, float fadeLen)
        {
            return this.CrossFadeOpt(animationName, fadeLen, AnimationBlender.opt<PlayMode>.none, AnimationBlender.opt<float>.none, AnimationBlender.opt<float>.none);
        }

        public bool CrossFade(string animationName, float fadeLen, PlayMode playMode)
        {
            return this.CrossFadeOpt(animationName, fadeLen, playMode, AnimationBlender.opt<float>.none, AnimationBlender.opt<float>.none);
        }

        public bool CrossFade(string animationName, float fadeLen, PlayMode playMode, float speed)
        {
            return this.CrossFadeOpt(animationName, fadeLen, playMode, speed, AnimationBlender.opt<float>.none);
        }

        public bool CrossFade(string animationName, float fadeLen, PlayMode playMode, float? speed, float startTime)
        {
            return this.CrossFadeOpt(animationName, fadeLen, playMode, AnimationBlender.to_opt<float>(speed), startTime);
        }

        public bool CrossFade(string animationName, float fadeLen, PlayMode playMode, float speed, float startTime)
        {
            return this.CrossFadeOpt(animationName, fadeLen, playMode, speed, startTime);
        }

        private void CrossFadeDirect(string animationName, AnimationBlender.opt<float> fadeLength, AnimationBlender.opt<PlayMode> playMode)
        {
            if (playMode.defined)
            {
                this.animation.CrossFade(animationName, fadeLength[0.3f], playMode.value);
            }
            else if (fadeLength.defined)
            {
                this.animation.CrossFade(animationName, fadeLength.value);
            }
            else
            {
                this.animation.CrossFade(animationName);
            }
        }

        private bool CrossFadeOpt(string animationName, AnimationBlender.opt<float> fadeLength, AnimationBlender.opt<PlayMode> playMode, AnimationBlender.opt<float> speed, AnimationBlender.opt<float> startTime)
        {
            AnimationState state;
            if (string.IsNullOrEmpty(animationName) || ((state = this.animation[animationName]) == null))
            {
                return false;
            }
            if (speed.defined)
            {
                state.speed = speed.value;
            }
            this.CrossFadeDirect(animationName, fadeLength, playMode);
            this.queuedAnimations.Clear();
            this.playingOneShot = true;
            this.oneShotAnimation = state;
            if (startTime.defined)
            {
                state.time = startTime.value;
            }
            return true;
        }

        public void Debug(Rect rect, string name)
        {
            DbgGUI.TableStart(rect);
            for (int i = 0; i < this.channels.Length; i++)
            {
                if (this.channels[i].weight.any)
                {
                    DbgGUI.Label(this.channels[i].value.name);
                }
            }
            for (int j = 0; j < this.trackers.Length; j++)
            {
                if (this.trackers[j].value.enabled)
                {
                    DbgGUI.Label(this.trackers[j].value.state.name);
                }
            }
            if (this.hasResidual)
            {
                DbgGUI.Label(this.residualState.name);
            }
            DbgGUI.ColumnNext();
            for (int k = 0; k < this.channels.Length; k++)
            {
                if (this.channels[k].weight.any)
                {
                    DbgGUI.Fract(this.channels[k].weight.normalized);
                }
            }
            for (int m = 0; m < this.trackers.Length; m++)
            {
                if (this.trackers[m].value.enabled)
                {
                    DbgGUI.Fract(this.trackers[m].weight.normalized);
                }
            }
            if (this.hasResidual)
            {
                DbgGUI.Fract(this.residualState.weight);
            }
            DbgGUI.TableEnd();
        }

        private void DisableTracker(ref AnimationBlender.Tracker tracker)
        {
            if (tracker.wasEnabled)
            {
                tracker.state.enabled = tracker.wasEnabled = false;
                tracker.state.weight = 0f;
            }
        }

        private bool ManualAnimationsPlaying(bool ClearWhenNone)
        {
            if (!this.playingOneShot)
            {
                return false;
            }
            while (object.ReferenceEquals(this.oneShotAnimation, null) || !this.oneShotAnimation.enabled)
            {
                if (this.queuedAnimations.Count == 0)
                {
                    if (ClearWhenNone)
                    {
                        this.oneShotAnimation = null;
                        this.playingOneShot = false;
                    }
                    return false;
                }
                this.SetOneShotAnimation(this.queuedAnimations.Dequeue());
            }
            return true;
        }

        public bool Play(string animationName)
        {
            return this.PlayOpt(animationName, AnimationBlender.opt<PlayMode>.none, AnimationBlender.opt<float>.none, AnimationBlender.opt<float>.none);
        }

        public bool Play(string animationName, float speed)
        {
            return this.PlayOpt(animationName, AnimationBlender.opt<PlayMode>.none, speed, AnimationBlender.opt<float>.none);
        }

        public bool Play(string animationName, PlayMode playMode)
        {
            return this.PlayOpt(animationName, playMode, AnimationBlender.opt<float>.none, AnimationBlender.opt<float>.none);
        }

        public bool Play(string animationName, float? speed, float startTime)
        {
            return this.PlayOpt(animationName, AnimationBlender.opt<PlayMode>.none, AnimationBlender.to_opt<float>(speed), startTime);
        }

        public bool Play(string animationName, float speed, float startTime)
        {
            return this.PlayOpt(animationName, AnimationBlender.opt<PlayMode>.none, speed, startTime);
        }

        public bool Play(string animationName, PlayMode playMode, float speed)
        {
            return this.PlayOpt(animationName, playMode, speed, AnimationBlender.opt<float>.none);
        }

        public bool Play(string animationName, PlayMode playMode, float speed, float startTime)
        {
            return this.PlayOpt(animationName, playMode, speed, startTime);
        }

        public bool Play(string animationName, PlayMode playMode, float? speed, float startTime)
        {
            return this.PlayOpt(animationName, playMode, AnimationBlender.to_opt<float>(speed), startTime);
        }

        private bool PlayDirect(string animationName, AnimationBlender.opt<PlayMode> playMode)
        {
            PlayMode mode;
            if (playMode.check(out mode))
            {
                return this.animation.Play(animationName, mode);
            }
            return this.animation.Play(animationName);
        }

        private bool PlayOpt(string animationName, AnimationBlender.opt<PlayMode> playMode, AnimationBlender.opt<float> speed, AnimationBlender.opt<float> startTime)
        {
            AnimationState state;
            float num;
            if (string.IsNullOrEmpty(animationName) || ((state = this.animation[animationName]) == null))
            {
                return false;
            }
            if (!playMode.defined)
            {
                this.animation.Stop();
            }
            if (speed.defined)
            {
                num = state.speed;
                state.speed = speed.value;
            }
            else
            {
                num = 0f;
            }
            if (!this.PlayDirect(animationName, playMode))
            {
                if (speed.defined)
                {
                    state.speed = num;
                }
                return false;
            }
            this.queuedAnimations.Clear();
            this.playingOneShot = true;
            this.oneShotAnimation = state;
            if (startTime.defined)
            {
                state.time = startTime.value;
            }
            return true;
        }

        public bool PlayQueued(string animationName)
        {
            return this.PlayQueuedOpt(animationName, AnimationBlender.opt<QueueMode>.none, AnimationBlender.opt<PlayMode>.none);
        }

        public bool PlayQueued(string animationName, QueueMode queueMode)
        {
            return this.PlayQueuedOpt(animationName, queueMode, AnimationBlender.opt<PlayMode>.none);
        }

        public bool PlayQueued(string animationName, QueueMode queueMode, PlayMode playMode)
        {
            return this.PlayQueuedOpt(animationName, queueMode, playMode);
        }

        private bool PlayQueuedDirect(string animationName, AnimationBlender.opt<QueueMode> queueMode, AnimationBlender.opt<PlayMode> playMode)
        {
            QueueMode mode;
            PlayMode mode2;
            if (playMode.check(out mode2))
            {
                return (bool) this.animation.PlayQueued(animationName, queueMode[QueueMode.CompleteOthers], mode2);
            }
            if (queueMode.check(out mode))
            {
                return (bool) this.animation.PlayQueued(animationName, mode);
            }
            return (bool) this.animation.PlayQueued(animationName);
        }

        private bool PlayQueuedOpt(string animationName, AnimationBlender.opt<QueueMode> queueMode, AnimationBlender.opt<PlayMode> playMode)
        {
            if (string.IsNullOrEmpty(animationName) || !this.PlayQueuedDirect(animationName, queueMode, playMode))
            {
                return false;
            }
            this.StopBlendingNow();
            if (queueMode.defined && (((QueueMode) queueMode.value) == QueueMode.PlayNow))
            {
                this.queuedAnimations.Clear();
                this.playingOneShot = false;
                this.oneShotAnimation = null;
                this.SetOneShotAnimation(animationName);
            }
            else if (this.playingOneShot)
            {
                this.queuedAnimations.Enqueue(animationName);
            }
            else
            {
                this.SetOneShotAnimation(animationName);
            }
            return true;
        }

        public void SetActive(int channel, bool value)
        {
            this.channels[channel].value.active = value;
        }

        public void SetActive(string channel, bool value)
        {
            this.SetActive(this.nameToChannel[channel], value);
        }

        public bool SetOneShotAnimation(string animationName)
        {
            return (!string.IsNullOrEmpty(animationName) && this.SetOneShotAnimation(this.animation[animationName]));
        }

        public bool SetOneShotAnimation(AnimationState animationState)
        {
            if (animationState == null)
            {
                return false;
            }
            this.oneShotAnimation = animationState;
            return (this.playingOneShot = true);
        }

        public void SetSolo(int channel)
        {
            for (int i = 0; i < this.channelCount; i++)
            {
                this.SetActive(i, i == channel);
            }
        }

        public void SetSolo(string channel)
        {
            this.SetSolo(this.nameToChannel[channel]);
        }

        public void SetSolo(int channel, bool muteall)
        {
            if (muteall)
            {
                for (int i = 0; i < this.channelCount; i++)
                {
                    this.SetActive(i, false);
                }
            }
            else
            {
                this.SetSolo(channel);
            }
        }

        public void SetSolo(string channel, bool muteall)
        {
            this.SetSolo(this.nameToChannel[channel], muteall);
        }

        private void StopBlendingNow()
        {
            for (int i = 0; i < this.trackerCount; i++)
            {
                this.trackers[i].value.state.enabled = false;
            }
        }

        public void Update(float blend, float dt)
        {
            float num2;
            if (this.playingOneShot)
            {
                if (!this.ManualAnimationsPlaying(true))
                {
                    this.oneShotBlendInTime = this.oneShotBlendIn.start + dt;
                    if (this.oneShotBlendInTime > this.oneShotBlendIn.end)
                    {
                        this.oneShotBlendInTime = this.oneShotBlendIn.end;
                    }
                    this.animationBlocking = false;
                    for (int i = 0; i < this.trackerCount; i++)
                    {
                        this.trackers[i].value.wasEnabled = false;
                    }
                }
                else
                {
                    this.oneShotBlendInTime = this.oneShotBlendIn.start;
                    if (!this.hasOneShotBlendIn)
                    {
                        blend = 0f;
                    }
                    this.animationBlocking = true;
                }
            }
            else
            {
                this.animationBlocking = false;
                if ((this.oneShotBlendInTime < this.oneShotBlendIn.end) && ((this.oneShotBlendInTime += dt) > this.oneShotBlendIn.end))
                {
                    this.oneShotBlendInTime = this.oneShotBlendIn.end;
                }
            }
            if (this.hasOneShotBlendIn)
            {
                blend *= this.oneShotBlendIn.SampleTime(this.oneShotBlendInTime);
            }
            if (blend > 1f)
            {
                blend = 1f;
            }
            else if (blend < 0f)
            {
                blend = 0f;
            }
            if (this.UpdateBlender(ref this.blender, dt, blend, out num2))
            {
                if (this.hasResidual)
                {
                    bool flag = !this.residualState.enabled;
                    this.residualState.enabled = true;
                    this.residualState.weight = num2;
                    if (flag)
                    {
                        this.residualState.time = this.residualField.startFrame;
                        this.residualState.speed = this.residualField.playbackRate;
                    }
                }
            }
            else if (this.hasResidual && this.residualState.enabled)
            {
                this.residualState.enabled = false;
                this.residualState.weight = 0f;
            }
        }

        private bool UpdateBlender(ref AnimationBlender.TrackerBlender blender, float dt, float externalBlend, out float residualBlend)
        {
            float num5;
            for (int i = 0; i < this.trackerCount; i++)
            {
                this.UpdateTracker(ref this.trackers[blender.trackers[i]], dt);
            }
            bool flag = AnimationBlender.WeightOf<AnimationBlender.Tracker>(this.trackers, blender.trackers, out blender.trackerWeight);
            for (int j = blender.trackerWeight.count; j < blender.trackerCount; j++)
            {
                this.DisableTracker(ref this.trackers[blender.trackers[j]].value);
            }
            float num3 = 0f;
            for (int k = 0; k < blender.trackerWeight.count; k++)
            {
                num3 += this.BindTracker(ref this.trackers[blender.trackers[k]], externalBlend);
            }
            residualBlend = num5 = (1f - num3) * externalBlend;
            return (num5 > 0f);
        }

        private void UpdateChannel(ref AnimationBlender.Weighted<AnimationBlender.Channel> channel, float dt)
        {
            bool flag;
            if (flag = (channel.value.field.blockedByAnimation && this.animationBlocking) && channel.value.active)
            {
                channel.value.active = false;
            }
            channel.weight.raw = channel.value.Update(dt);
            if (flag)
            {
                channel.value.active = true;
            }
        }

        private void UpdateTracker(ref AnimationBlender.Weighted<AnimationBlender.Tracker> tracker, float dt)
        {
            for (int i = 0; i < tracker.value.channelCount; i++)
            {
                this.UpdateChannel(ref this.channels[tracker.value.channels[i]], dt);
            }
            if (tracker.value.enabled = AnimationBlender.WeightOf<AnimationBlender.Channel>(this.channels, tracker.value.channels, out tracker.value.channelWeight))
            {
                tracker.value.startTime = this.channels[tracker.value.channels[tracker.value.channelWeight.winner]].value.startTime;
                float num2 = 0f;
                float num3 = 0f;
                for (int j = 0; j < tracker.value.channelWeight.count; j++)
                {
                    float num4;
                    num2 += this.channels[tracker.value.channels[j]].value.playbackRate * (num4 = this.channels[tracker.value.channels[j]].weight.normalized);
                    num3 += this.channels[tracker.value.channels[j]].value.maxBlend * num4;
                }
                tracker.value.playbackRate = num2;
                tracker.value.blendFraction = num3;
            }
            tracker.weight.raw = tracker.value.channelWeight.sum.raw;
        }

        public bool isPlayingManualAnimation
        {
            get
            {
                return this.ManualAnimationsPlaying(false);
            }
        }

        private static class DbgGUI
        {
            private static readonly GUILayoutOption[] Cell = new GUILayoutOption[] { GUILayout.Height(18f) };
            private static readonly GUILayoutOption[] FirstColumn = new GUILayoutOption[] { GUILayout.Width(128f) };
            private static readonly GUILayoutOption[] OtherColumn = new GUILayoutOption[] { GUILayout.ExpandWidth(true) };

            public static void ColumnNext()
            {
                GUILayout.EndVertical();
                GUILayout.BeginVertical(OtherColumn);
            }

            public static void Fract(float frac)
            {
                GUILayout.HorizontalSlider(frac, 0f, 1f, Cell);
            }

            public static void Label(string str)
            {
                GUILayout.Label(str, Cell);
            }

            public static void TableEnd()
            {
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                GUILayout.EndArea();
            }

            public static void TableStart(Rect rect)
            {
                GUILayout.BeginArea(rect);
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.BeginVertical(FirstColumn);
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MixerConfig
    {
        [NonSerialized]
        public readonly Animation animation;
        [NonSerialized]
        public readonly AnimationBlender.ResidualField residual;
        [NonSerialized]
        public readonly AnimationBlender.ChannelConfig[] channels;
        public MixerConfig(Animation animation, AnimationBlender.ResidualField residual, params AnimationBlender.ChannelConfig[] channels)
        {
            this.animation = animation;
            this.residual = residual;
            this.channels = channels;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct opt<T>
    {
        [NonSerialized]
        public readonly T value;
        [NonSerialized]
        public readonly bool defined;
        public static readonly AnimationBlender.opt<T> none;
        private opt(T value, bool defined)
        {
            this.value = value;
            this.defined = defined;
        }

        static opt()
        {
            AnimationBlender.opt<T>.none = new AnimationBlender.opt<T>();
        }

        public bool check(out T value)
        {
            value = this.value;
            return this.defined;
        }

        public T this[T fallback]
        {
            get
            {
                return (!this.defined ? fallback : this.value);
            }
        }
        public static implicit operator AnimationBlender.opt<T>(T value)
        {
            return new AnimationBlender.opt<T>(value, true);
        }
    }

    [Serializable]
    public sealed class ResidualField : AnimationBlender.Field
    {
        [SerializeField]
        public int animLayer;
        [SerializeField]
        public bool changeAnimLayer;
        [SerializeField]
        public AnimationBlendMode channelBlend;
        [SerializeField]
        public AnimationCurve introCurve;
        [SerializeField]
        public AnimationBlendMode residualBlend;

        public ResidualField()
        {
            base.clipName = string.Empty;
            base.playbackRate = 1f;
            this.changeAnimLayer = false;
            this.channelBlend = this.residualBlend = AnimationBlendMode.Blend;
        }

        public AnimationBlender.CurveInfo introCurveInfo
        {
            get
            {
                return new AnimationBlender.CurveInfo(this.introCurve);
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct State
    {
        [NonSerialized]
        public bool active;
        [NonSerialized]
        public float time;
        [NonSerialized]
        public float percent;
        [NonSerialized]
        public float delay;
        [NonSerialized]
        public float value;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Tracker
    {
        [NonSerialized]
        public AnimationClip clip;
        [NonSerialized]
        public AnimationState state;
        [NonSerialized]
        public int[] channels;
        [NonSerialized]
        public int channelCount;
        [NonSerialized]
        public AnimationBlender.WeightResult channelWeight;
        [NonSerialized]
        public float playbackRate;
        [NonSerialized]
        public float blendFraction;
        [NonSerialized]
        public float startTime;
        [NonSerialized]
        public bool enabled;
        [NonSerialized]
        public bool wasEnabled;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct TrackerBlender
    {
        [NonSerialized]
        public int[] trackers;
        [NonSerialized]
        public int trackerCount;
        [NonSerialized]
        public AnimationBlender.WeightResult trackerWeight;
        public TrackerBlender(int count)
        {
            this.trackers = new int[count];
            this.trackerCount = count;
            this.trackerWeight = new AnimationBlender.WeightResult();
            for (int i = 0; i < count; i++)
            {
                this.trackers[i] = i;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Weighted<T>
    {
        [NonSerialized]
        public AnimationBlender.WeightUnit weight;
        [NonSerialized]
        public T value;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct WeightResult
    {
        [NonSerialized]
        public int count;
        [NonSerialized]
        public int winner;
        public AnimationBlender.WeightUnit sum;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct WeightUnit
    {
        [NonSerialized]
        public float raw;
        [NonSerialized]
        public float scaled;
        [NonSerialized]
        public float normalized;
        public bool any
        {
            get
            {
                return (this.raw > 0f);
            }
        }
        public float SetScaledRecip(float recip)
        {
            return (this.normalized = this.scaled = this.raw * recip);
        }

        public float SetNormalizedRecip(float recip)
        {
            return (this.normalized = this.scaled * recip);
        }
    }
}

