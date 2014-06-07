using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class EngineSoundLoop : ScriptableObject
{
    [SerializeField]
    private Phrase _dUpper = new Phrase(0.565f);
    [SerializeField]
    private Phrase _eMidLow = new Phrase(0.8f);
    [SerializeField]
    private Phrase _fMidHigh = new Phrase(0.78f);
    [SerializeField]
    private Gear[] _gears = new Gear[] { new Gear(0.7f, 1.65f), new Gear(0.85f, 1.76f), new Gear(1.17f, 1.8f), new Gear(1.25f, 1.86f) };
    [SerializeField]
    private Gear _idleShiftUp = new Gear(1.17f, 1.65f);
    [SerializeField]
    private Phrase _kPassing = new Phrase(0.565f);
    [SerializeField]
    private Phrase _lLower = new Phrase(0.61f);
    [SerializeField]
    private Gear _shiftDown = new Gear(1.44f, 1.17f);
    [SerializeField]
    private float _shiftDuration = 0.1f;
    [SerializeField]
    private Gear _shiftUp = new Gear(1.17f, 1.76f);
    [SerializeField]
    private int _topGear = 4;
    [SerializeField]
    private float _volumeFromPitchBase = 0.85f;
    [SerializeField]
    private float _volumeFromPitchRange = 0.91f;
    private const float E_PITCH = 0.89f;
    private const float E_PITCH_DELTA = 0.11f;
    private const float E_THROTTLE = 0.8f;
    private const float E_THROTTLE_DELTA = 0.2f;
    private const float F_PITCH = 0.8f;
    private const float F_PITCH_DELTA = 0.2f;
    private const float F_THROTTLE = 0.7f;
    private const float F_THROTTLE_DELTA = 0.3f;
    [NonSerialized]
    private Dictionary<Transform, Instance> instances;
    private const float kPitchDefault_High1 = 1.65f;
    private const float kPitchDefault_High2 = 1.76f;
    private const float kPitchDefault_High3 = 1.8f;
    private const float kPitchDefault_High4 = 1.86f;
    private const float kPitchDefault_Idle = 0.7f;
    private const float kPitchDefault_Low = 1.17f;
    private const float kPitchDefault_Medium = 1.25f;
    private const float kPitchDefault_Shift = 1.44f;
    private const float kPitchDefault_Start = 0.85f;
    private const float sD = 0.4f;
    private const float sE = 0.4f;
    private const float sF = 0.4f;
    private const float sK = 0.7f;
    private const float sL = 0.4f;

    private static float Coserp(float start, float end, float value)
    {
        float num = Mathf.Cos(value * 1.570796f);
        return ((num < 1f) ? ((num > 0f) ? ((start * num) + (end * (1f - num))) : end) : start);
    }

    public Instance Create(Transform attachTo)
    {
        return this.Create(attachTo, Vector3.zero);
    }

    public Instance Create(Transform attachTo, Vector3 localPosition)
    {
        Instance instance;
        if (attachTo == null)
        {
            throw new MissingReferenceException("attachTo must not be null or destroyed");
        }
        if (this.instances == null)
        {
            this.instances = new Dictionary<Transform, Instance>();
        }
        else if (this.instances.TryGetValue(attachTo, out instance))
        {
            instance.localPosition = localPosition;
            return instance;
        }
        instance = new Instance(attachTo, localPosition, this);
        this.instances[attachTo] = instance;
        return instance;
    }

    public Instance CreateWorld(Transform attachTo, Vector3 worldPosition)
    {
        return this.Create(attachTo, attachTo.InverseTransformPoint(worldPosition));
    }

    private void GearLerp(byte gear, float factor, ref float pitch, ref bool pitchChanged, ref float volume, ref bool volumeChanged)
    {
        int num;
        if ((this._gears != null) && ((num = this._gears.Length) != 0))
        {
            int num2;
            if (this._topGear < num)
            {
                num2 = this._topGear;
            }
            else
            {
                num2 = num - 1;
            }
            if (gear > num2)
            {
                this._gears[num2].CompareLerp(factor, ref pitch, ref pitchChanged, ref volume, ref volumeChanged);
            }
            else
            {
                this._gears[gear].CompareLerp(factor, ref pitch, ref pitchChanged, ref volume, ref volumeChanged);
            }
        }
    }

    private static float Sinerp(float start, float end, float value)
    {
        float num = Mathf.Sin(value * 1.570796f);
        return ((num > 0f) ? ((num < 1f) ? ((end * num) + (start * (1f - num))) : end) : start);
    }

    private sbyte VolumeFactor(float pitch, out float between)
    {
        between = (pitch - this._volumeFromPitchBase) / this._volumeFromPitchRange;
        if (between >= 1f)
        {
            between = 1f;
            return 1;
        }
        if (between <= 0f)
        {
            between = 0f;
            return -1;
        }
        return 0;
    }

    private float volumeD
    {
        get
        {
            return (this._dUpper.volume * 0.4f);
        }
    }

    private float volumeE
    {
        get
        {
            return (this._eMidLow.volume * 0.4f);
        }
    }

    private float volumeF
    {
        get
        {
            return (this._fMidHigh.volume * 0.4f);
        }
    }

    private float volumeK
    {
        get
        {
            return (this._kPassing.volume * 0.7f);
        }
    }

    private float volumeL
    {
        get
        {
            return (this._lLower.volume * 0.4f);
        }
    }

    [Serializable]
    private class Gear
    {
        public float highPitch;
        public float highVolume;
        public float lowPitch;
        public float lowVolume;

        public Gear() : this(0.7f, 1.65f)
        {
        }

        public Gear(float lower, float upper)
        {
            this.lowPitch = this.lowVolume = lower;
            this.highPitch = this.highVolume = upper;
        }

        public Gear(float lowerPitch, float lowerVolume, float upperPitch, float upperVolume)
        {
        }

        public void CompareLerp(float t, ref float pitch, ref bool pitchChanged, ref float volume, ref bool volumeChanged)
        {
            if (t <= 0f)
            {
                if (pitch != this.lowPitch)
                {
                    pitchChanged = true;
                    pitch = this.lowPitch;
                }
                if (volume != this.lowVolume)
                {
                    volumeChanged = true;
                    volume = this.lowVolume;
                }
            }
            else if (t >= 1f)
            {
                if (pitch != this.highPitch)
                {
                    pitchChanged = true;
                    pitch = this.highPitch;
                }
                if (volume != this.highVolume)
                {
                    volumeChanged = true;
                    volume = this.highVolume;
                }
            }
            else
            {
                float num3 = 1f - t;
                float num = (this.lowPitch * num3) + (this.highPitch * t);
                float num2 = (this.lowVolume * num3) + (this.highVolume * t);
                if (pitch != num)
                {
                    pitchChanged = true;
                    pitch = num;
                }
                if (volume != num2)
                {
                    volumeChanged = true;
                    volume = num2;
                }
            }
        }

        public void Lerp(float t, out float pitch, out float volume)
        {
            if (t <= 0f)
            {
                pitch = this.lowPitch;
                volume = this.lowVolume;
            }
            else if (t >= 1f)
            {
                pitch = this.highPitch;
                volume = this.highVolume;
            }
            else
            {
                float num = 1f - t;
                pitch = (this.lowPitch * num) + (this.highPitch * t);
                volume = (this.lowVolume * num) + (this.highVolume * t);
            }
        }
    }

    public class Instance : IDisposable
    {
        [NonSerialized]
        private float _dVol;
        [NonSerialized]
        private float _eVol;
        [NonSerialized]
        private float _fVol;
        [NonSerialized]
        private byte _gear;
        [NonSerialized]
        private float _kVol;
        [NonSerialized]
        private float _lastClampedThrottle;
        [NonSerialized]
        private float _lastPitchFactor;
        [NonSerialized]
        private float _lastSinerp;
        [NonSerialized]
        private float _lastVolumeFactor;
        [NonSerialized]
        private sbyte _lastVolumeFactorClamp;
        [NonSerialized]
        private float _masterVolume;
        [NonSerialized]
        private float _pitch;
        [NonSerialized]
        private float _shiftTime;
        [NonSerialized]
        private float _speedFactor;
        [NonSerialized]
        private float _throttle;
        [NonSerialized]
        private float _volume;
        [NonSerialized]
        private AudioSource D;
        [NonSerialized]
        private AudioSource E;
        [NonSerialized]
        private AudioSource F;
        [NonSerialized]
        private ushort flags;
        private const ushort FLAGS_MASK = 0xffff;
        [NonSerialized]
        private AudioSource K;
        private const ushort kD = 1;
        private const ushort kDisposed = 0x20;
        private const ushort kE = 4;
        private const ushort kF = 2;
        private const ushort kFlagOnceUpdate = 0x400;
        private const ushort kK = 0x10;
        private const ushort kL = 8;
        private const ushort kPaused = 0x80;
        private const ushort kPlaying = 0x40;
        private const ushort kPlayingOrPaused = 0xc0;
        private const ushort kShifting = 0x100;
        private const ushort kShiftingDown = 0x100;
        private const ushort kShiftingUp = 0x300;
        private const ushort kShiftingUpOrDown = 0x300;
        [NonSerialized]
        private AudioSource L;
        [NonSerialized]
        private EngineSoundLoop loop;
        private const ushort nD = 0xfffe;
        private const ushort nDisposed = 0xffdf;
        private const ushort nE = 0xfffb;
        private const ushort nF = 0xfffd;
        private const ushort nK = 0xffef;
        private const ushort nL = 0xfff7;
        private const ushort nPaused = 0xff7f;
        private const ushort nPlaying = 0xffbf;
        private const ushort nPlayingOrPaused = 0xff3f;
        private const ushort nShifting = 0xfeff;
        private const ushort nShiftingDown = 0xfeff;
        private const ushort nShiftingUp = 0xfcff;
        private const ushort nShiftingUpOrDown = 0xfcff;
        [NonSerialized]
        private Transform parent;
        [NonSerialized]
        private EngineSoundLoopPlayer player;

        internal Instance(Transform parent, Vector3 offset, EngineSoundLoop loop)
        {
            this.parent = parent;
            this.loop = loop;
            Type[] components = new Type[] { typeof(EngineSoundLoopPlayer) };
            GameObject go = new GameObject("_EnginePlayer", components);
            this.player = go.GetComponent<EngineSoundLoopPlayer>();
            this.player.instance = this;
            Setup(go, ref this.D, ref this.flags, 1, loop._dUpper, 1f);
            Setup(go, ref this.F, ref this.flags, 2, loop._fMidHigh, 1f);
            Setup(go, ref this.E, ref this.flags, 4, loop._eMidLow, 1f);
            Setup(go, ref this.L, ref this.flags, 8, loop._lLower, 1f);
            Setup(go, ref this.K, ref this.flags, 0x10, loop._kPassing, 0f);
            this._lastVolumeFactor = this._lastClampedThrottle = this._lastSinerp = this._lastPitchFactor = float.NegativeInfinity;
            this._lastVolumeFactorClamp = -128;
            this._masterVolume = 1f;
            this._pitch = loop._idleShiftUp.lowVolume;
            this._shiftTime = -3000f;
            this._speedFactor = this._dVol = this._fVol = this._eVol = this._kVol = this._throttle = 0f;
            this._gear = 0;
            Transform transform = go.transform;
            transform.parent = parent;
            transform.localPosition = offset;
            transform.localRotation = Quaternion.identity;
        }

        public void Dispose()
        {
            this.Dispose(false);
        }

        internal void Dispose(bool fromPlayer)
        {
            if ((this.flags & 0x20) != 0x20)
            {
                if ((this.loop != null) && (this.loop.instances != null))
                {
                    this.loop.instances.Remove(this.parent);
                }
                this.D = this.E = this.F = this.L = (AudioSource) (this.K = null);
                if (!fromPlayer && (this.player != null))
                {
                    try
                    {
                        this.player.instance = null;
                        Object.Destroy(this.player.gameObject);
                    }
                    catch (Exception exception)
                    {
                        Debug.LogError(exception, this.player);
                    }
                }
                this.player = null;
                this.flags = 0x20;
            }
        }

        public void Pause()
        {
            if ((this.flags & 0xe0) == 0x40)
            {
                this.PAUSE();
            }
        }

        private void PAUSE()
        {
            if ((this.flags & 1) == 1)
            {
                this.D.Pause();
            }
            if ((this.flags & 2) == 2)
            {
                this.F.Pause();
            }
            if ((this.flags & 4) == 4)
            {
                this.E.Pause();
            }
            if ((this.flags & 8) == 8)
            {
                this.L.Pause();
            }
            if ((this.flags & 0x10) == 0x10)
            {
                this.K.Pause();
            }
            this.flags = (ushort) (this.flags | 0x80);
            this.flags = (ushort) (this.flags & 0xffbf);
        }

        public void Play()
        {
            if ((this.flags & 0x60) == 0)
            {
                this.PLAY();
            }
        }

        private void PLAY()
        {
            if ((this.flags & 0x400) == 0x400)
            {
                if ((this.flags & 1) == 1)
                {
                    this.D.Play();
                }
                if ((this.flags & 2) == 2)
                {
                    this.F.Play();
                }
                if ((this.flags & 4) == 4)
                {
                    this.E.Play();
                }
                if ((this.flags & 8) == 8)
                {
                    this.L.Play();
                }
                if ((this.flags & 0x10) == 0x10)
                {
                    this.K.Play();
                }
            }
            this.flags = (ushort) (this.flags | 0x40);
            this.flags = (ushort) (this.flags & 0xff7f);
        }

        private static void Setup(GameObject go, ref AudioSource source, ref ushort flags, ushort flag, EngineSoundLoop.Phrase phrase, float volumeScalar)
        {
            if ((phrase != null) && (phrase.clip != null))
            {
                source = go.AddComponent<AudioSource>();
                source.playOnAwake = false;
                source.loop = true;
                source.clip = phrase.clip;
                source.volume = phrase.volume * volumeScalar;
                source.dopplerLevel = 0f;
                flags = (ushort) (flags | flag);
            }
        }

        public void Stop()
        {
            if (((this.flags & 0x20) == 0) && ((this.flags & 0xc0) != 0))
            {
                this.STOP();
            }
        }

        private void STOP()
        {
            if ((this.flags & 1) == 1)
            {
                this.D.Stop();
            }
            if ((this.flags & 2) == 2)
            {
                this.F.Stop();
            }
            if ((this.flags & 4) == 4)
            {
                this.E.Stop();
            }
            if ((this.flags & 8) == 8)
            {
                this.L.Stop();
            }
            if ((this.flags & 0x10) == 0x10)
            {
                this.K.Stop();
            }
            this.flags = (ushort) (this.flags & 0xff3f);
        }

        public void Update(float speedFactor, float throttle)
        {
            switch ((this.flags & 0x420))
            {
                case 0x20:
                    this._speedFactor = speedFactor;
                    this._throttle = throttle;
                    break;

                case 0x400:
                    this.UPDATE(speedFactor, throttle);
                    break;

                default:
                    this.flags = (ushort) (this.flags | 0x400);
                    this.UPDATE(speedFactor, throttle);
                    if ((this.flags & 0xc0) == 0x40)
                    {
                        this.PLAY();
                    }
                    break;
            }
        }

        private void UPDATE(float speedFactor, float throttle)
        {
            bool flag;
            bool flag5;
            bool flag6;
            byte num4;
            bool flag7;
            float num8;
            if (throttle != this._throttle)
            {
                this._throttle = throttle;
                flag = true;
            }
            else
            {
                flag = false;
            }
            float num = this._pitch;
            float num2 = this._volume;
            float num3 = this._speedFactor;
            bool doPitchAdjust = false;
            bool doVolumeAdjust = false;
            bool flag4 = !(speedFactor == num3);
            if (flag4)
            {
                this._speedFactor = speedFactor;
            }
            if ((this.flags & 0x100) == 0x100)
            {
                flag5 = flag6 = this.UPDATE_SHIFTING(ref doPitchAdjust, ref doVolumeAdjust);
            }
            else
            {
                flag5 = true;
                flag6 = false;
            }
            if (!flag5)
            {
                goto Label_02FF;
            }
        Label_0085:
            if (flag4 || flag6)
            {
                int num5 = this.loop._topGear;
                this._lastSinerp = EngineSoundLoop.Sinerp(0f, (float) num5, speedFactor);
                int num6 = (int) this._lastSinerp;
                if (num6 == this._gear)
                {
                    flag7 = false;
                    num4 = (num6 != num5) ? this._gear : ((byte) (num5 - 1));
                }
                else if (num6 < this._gear)
                {
                    if (this._gear > 0)
                    {
                        if (this._gear == num5)
                        {
                            this._gear = (byte) (this._gear - 1);
                            flag7 = false;
                            num4 = this._gear;
                        }
                        else
                        {
                            flag7 = true;
                            num4 = (byte) (this._gear - 1);
                        }
                    }
                    else
                    {
                        flag7 = false;
                        num4 = this._gear;
                    }
                }
                else if ((this._gear < 0xff) && (this._gear < num5))
                {
                    if (this._gear < (num5 - 1))
                    {
                        flag7 = true;
                        num4 = (byte) (this._gear + 1);
                    }
                    else
                    {
                        flag7 = false;
                        num4 = this._gear;
                        this._gear = (byte) (this._gear + 1);
                    }
                }
                else
                {
                    flag7 = false;
                    num4 = this._gear;
                }
            }
            else
            {
                flag7 = false;
                num4 = (this._gear != this.loop._topGear) ? this._gear : ((byte) (this._gear - 1));
            }
            float num7 = this._lastSinerp - num4;
            if (num7 == 0f)
            {
                num8 = 0f;
            }
            else if (throttle >= 0.5f)
            {
                num8 = num7;
            }
            else if (throttle <= 0f)
            {
                num8 = num7 * 0.3f;
            }
            else
            {
                num8 = num7 * (0.3f + (throttle * 0.7f));
            }
            if (flag7)
            {
                if (num4 > this._gear)
                {
                    this.flags = (ushort) (this.flags | 0x300);
                }
                else
                {
                    this.flags = (ushort) (this.flags | 0x100);
                }
                this._lastPitchFactor = num8;
                this._shiftTime = Time.time;
                if (!(flag6 = this.UPDATE_SHIFTING(ref doPitchAdjust, ref doVolumeAdjust)))
                {
                    goto Label_02FF;
                }
                goto Label_0085;
            }
            if ((num8 != this._lastPitchFactor) || flag6)
            {
                this._lastPitchFactor = num8;
                this.loop.GearLerp(num4, num8, ref this._pitch, ref doPitchAdjust, ref this._volume, ref doVolumeAdjust);
            }
        Label_02FF:
            if (doVolumeAdjust && (this._volume != num2))
            {
                this.UPDATE_PITCH_AND_OR_THROTTLE_VOLUME();
            }
            else if (flag)
            {
                this.UPDATE_THROTTLE_VOLUME();
            }
            if (flag4)
            {
                this.UPDATE_PASSING_VOLUME();
            }
            if (doPitchAdjust && (this._pitch != num))
            {
                this.UPDATE_RATES();
            }
        }

        private void UPDATE_MASTER_VOLUME()
        {
            if ((this.flags & 1) == 1)
            {
                this.D.volume = this._dVol * this._masterVolume;
            }
            if ((this.flags & 2) == 2)
            {
                this.F.volume = this._fVol * this._masterVolume;
            }
            if ((this.flags & 4) == 4)
            {
                this.E.volume = this._eVol * this._masterVolume;
            }
            if ((this.flags & 8) == 8)
            {
                this.L.volume = this.loop.volumeL * this._masterVolume;
            }
            if ((this.flags & 0x10) == 0x10)
            {
                this.K.volume = this._kVol * this._masterVolume;
            }
        }

        private void UPDATE_PASSING_VOLUME()
        {
            if ((this.flags & 0x10) == 0x10)
            {
                this.K.volume = (this._kVol = this.loop.volumeK * this._speedFactor) * this._masterVolume;
            }
        }

        private void UPDATE_PITCH_AND_OR_THROTTLE_VOLUME()
        {
            ushort num = (ushort) (this.flags & 7);
            if (num != 0)
            {
                float num2;
                bool flag;
                sbyte num3 = this.loop.VolumeFactor(this._volume, out num2);
                if ((this._lastVolumeFactorClamp != num3) || (this._lastVolumeFactor != num2))
                {
                    flag = true;
                    this._lastVolumeFactor = num2;
                    this._lastVolumeFactorClamp = num3;
                    if ((num & 1) == 1)
                    {
                        this.D.volume = (num3 != -1) ? (this._masterVolume * (this._dVol = (num3 != 1) ? (this.loop.volumeD * num2) : this.loop.volumeD)) : (this._dVol = 0f);
                    }
                }
                else
                {
                    flag = false;
                }
                num = (ushort) (num & 0xfffe);
                if (num != 0)
                {
                    float num4 = Mathf.Clamp01(this._throttle);
                    if (num4 != this._lastClampedThrottle)
                    {
                        this._lastClampedThrottle = num4;
                        flag = true;
                    }
                    if (flag)
                    {
                        sbyte num6 = num3;
                        switch ((num6 + 1))
                        {
                            case 0:
                                if ((num & 2) == 2)
                                {
                                    this.F.volume = (this._fVol = (this.loop.volumeF * 0.8f) * (0.7f + (0.3f * num4))) * this._masterVolume;
                                }
                                if ((num & 4) == 4)
                                {
                                    this.E.volume = (this._eVol = (this.loop.volumeE * 0.89f) * (0.8f + (0.2f * num4))) * this._masterVolume;
                                }
                                return;

                            case 2:
                                if ((num & 2) == 2)
                                {
                                    this.F.volume = (this._fVol = this.loop.volumeF * (0.7f + (0.3f * num4))) * this._masterVolume;
                                }
                                if ((num & 4) == 4)
                                {
                                    this.E.volume = (this._eVol = this.loop.volumeE * (0.8f + (0.2f * num4))) * this._masterVolume;
                                }
                                return;
                        }
                        if ((num & 2) == 2)
                        {
                            this.F.volume = (this._fVol = (this.loop.volumeF * (0.8f + (0.2f * num2))) * (0.7f + (0.3f * num4))) * this._masterVolume;
                        }
                        if ((num & 4) == 4)
                        {
                            this.E.volume = (this._eVol = (this.loop.volumeE * (0.89f + (0.11f * num2))) * (0.8f + (0.2f * num4))) * this._masterVolume;
                        }
                    }
                }
            }
        }

        private void UPDATE_RATES()
        {
            if ((this.flags & 1) == 1)
            {
                this.D.pitch = this._pitch;
            }
            if ((this.flags & 2) == 2)
            {
                this.F.pitch = this._pitch;
            }
            if ((this.flags & 4) == 4)
            {
                this.E.pitch = this._pitch;
            }
            if ((this.flags & 8) == 8)
            {
                this.L.pitch = this._pitch;
            }
        }

        private bool UPDATE_SHIFTING(ref bool doPitchAdjust, ref bool doVolumeAdjust)
        {
            float num3;
            EngineSoundLoop.Gear gear;
            float num = Time.time - this._shiftTime;
            if (num >= this.loop._shiftDuration)
            {
                if ((this.flags & 0x300) == 0x300)
                {
                    this._gear = (byte) (this._gear + 1);
                }
                else if (this._gear > 0)
                {
                    this._gear = (byte) (this._gear - 1);
                }
                this.flags = (ushort) (this.flags & 0xfcff);
                return true;
            }
            float num2 = num / this.loop._shiftDuration;
            if ((this.flags & 0x300) == 0x300)
            {
                num3 = this._lastPitchFactor * num2;
                if (this._gear == 0)
                {
                    gear = this.loop._idleShiftUp;
                }
                else
                {
                    gear = this.loop._shiftUp;
                }
            }
            else
            {
                num3 = num2;
                gear = this.loop._shiftDown;
            }
            gear.CompareLerp(num3, ref this._pitch, ref doPitchAdjust, ref this._volume, ref doVolumeAdjust);
            return false;
        }

        private void UPDATE_THROTTLE_VOLUME()
        {
            ushort num = (ushort) (this.flags & 6);
            if (num != 0)
            {
                float num2 = Mathf.Clamp01(this._throttle);
                if (num2 != this._lastClampedThrottle)
                {
                    float num3 = this._lastVolumeFactor;
                    this._lastClampedThrottle = num2;
                    switch ((this._lastVolumeFactorClamp + 1))
                    {
                        case 0:
                            if ((num & 2) == 2)
                            {
                                this.F.volume = (this._fVol = (this.loop.volumeF * 0.8f) * (0.7f + (0.3f * num2))) * this._masterVolume;
                            }
                            if ((num & 4) == 4)
                            {
                                this.E.volume = (this._eVol = (this.loop.volumeE * 0.89f) * (0.8f + (0.2f * num2))) * this._masterVolume;
                            }
                            return;

                        case 2:
                            if ((num & 2) == 2)
                            {
                                this.F.volume = (this._fVol = this.loop.volumeF * (0.7f + (0.3f * num2))) * this._masterVolume;
                            }
                            if ((num & 4) == 4)
                            {
                                this.E.volume = (this._eVol = this.loop.volumeE * (0.8f + (0.2f * num2))) * this._masterVolume;
                            }
                            return;
                    }
                    if ((num & 2) == 2)
                    {
                        this.F.volume = (this._fVol = (this.loop.volumeF * (0.8f + (0.2f * num3))) * (0.7f + (0.3f * num2))) * this._masterVolume;
                    }
                    if ((num & 4) == 4)
                    {
                        this.E.volume = (this._eVol = (this.loop.volumeE * (0.89f + (0.11f * num3))) * (0.8f + (0.2f * num2))) * this._masterVolume;
                    }
                }
            }
        }

        public bool anySounds
        {
            get
            {
                return (((this.flags & 0x1f) != 0) && ((this.flags & 0x20) == 0));
            }
        }

        public bool disposed
        {
            get
            {
                return ((this.flags & 0x20) == 0x20);
            }
        }

        public bool hasUpdated
        {
            get
            {
                return ((this.flags & 0x400) == 0x400);
            }
        }

        internal Vector3 localPosition
        {
            set
            {
                if ((this.flags & 0x20) != 0x20)
                {
                    this.player.transform.localPosition = value;
                }
            }
        }

        public bool paused
        {
            get
            {
                return ((this.flags & 160) == 0x80);
            }
            set
            {
                if (value)
                {
                    if ((this.flags & 0xe0) == 0x40)
                    {
                        this.PAUSE();
                    }
                }
                else if ((this.flags & 0xe0) == 0x80)
                {
                    this.PLAY();
                }
            }
        }

        public bool playing
        {
            get
            {
                return ((this.flags & 0x60) == 0x40);
            }
            set
            {
                if (value)
                {
                    if ((this.flags & 0x60) == 0)
                    {
                        this.PLAY();
                    }
                }
                else if ((this.flags & 0x60) == 0x40)
                {
                    this.PAUSE();
                }
            }
        }

        public bool playingOrPaused
        {
            get
            {
                return (((this.flags & 0x60) == 0x40) || ((this.flags & 160) == 0x80));
            }
        }

        public float speedFactor
        {
            get
            {
                return this._speedFactor;
            }
        }

        public bool stopped
        {
            get
            {
                return ((this.flags & 0xc0) == 0);
            }
        }

        public float volume
        {
            get
            {
                return this._masterVolume;
            }
            set
            {
                if (value < 0f)
                {
                    value = 0f;
                }
                if (this._masterVolume != value)
                {
                    this._masterVolume = value;
                    if ((this.flags & 0x20) == 0)
                    {
                        this.UPDATE_MASTER_VOLUME();
                    }
                }
            }
        }
    }

    [Serializable]
    private class Phrase
    {
        public AudioClip clip;
        public float volume;

        public Phrase()
        {
            this.volume = 1f;
        }

        public Phrase(float volume)
        {
            this.volume = volume;
        }
    }
}

