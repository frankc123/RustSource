using MoPhoGames.USpeak.Core;
using MoPhoGames.USpeak.Core.Utils;
using MoPhoGames.USpeak.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

[AddComponentMenu("USpeak/USpeaker")]
public class USpeaker : MonoBehaviour
{
    public ThreeDMode _3DMode;
    public bool AskPermission = true;
    private ISpeechDataHandler audioHandler;
    public BandMode BandwidthMode;
    public int Codec;
    private USpeakCodecManager codecMgr;
    private string currentDeviceName = string.Empty;
    public static float CurrentVolume = 0f;
    public bool DebugPlayback;
    private string[] devicesCached;
    private int index;
    private static int InputDeviceID = 0;
    private ThreeDMode last3DMode;
    private BandMode lastBandMode;
    private int lastCodec;
    private int lastReadPos;
    private float lastTime;
    private float lastVTime;
    public static float LocalGain = 1f;
    public bool Mute;
    public static bool MuteAll = false;
    private int overlap;
    private List<float[]> pendingEncode = new List<float[]>();
    private float playDelay;
    private double played;
    private double received;
    private float[] receivedData;
    private int recFreq;
    private AudioClip recording;
    public static float RemoteGain = 1f;
    private List<USpeakFrameContainer> sendBuffer = new List<USpeakFrameContainer>();
    public SendBehavior SendingMode;
    public float SendRate = 16f;
    private float sendt = 1f;
    private float sendTimer;
    private USpeakSettingsData settings;
    private bool shouldPlay;
    public SpeakerMode SpeakerMode;
    public float SpeakerVolume = 1f;
    private IUSpeakTalkController talkController;
    private float talkTimer;
    private List<byte> tempSendBytes = new List<byte>();
    public bool UseVAD;
    public static List<USpeaker> USpeakerList = new List<USpeaker>();
    private float vadHangover = 0.5f;
    public float VolumeThreshold = 0.01f;

    private float amplitude(float[] x)
    {
        float a = 0f;
        for (int i = 0; i < x.Length; i++)
        {
            a = Mathf.Max(a, Mathf.Abs(x[i]));
        }
        return a;
    }

    private void Awake()
    {
        USpeakerList.Add(this);
        if (base.audio == null)
        {
            base.gameObject.AddComponent<AudioSource>();
        }
        base.audio.clip = AudioClip.Create("vc", this.audioFrequency * 10, 1, this.audioFrequency, this._3DMode == ThreeDMode.Full3D, false);
        base.audio.loop = true;
        this.receivedData = new float[this.audioFrequency * 10];
        this.codecMgr = USpeakCodecManager.Instance;
        this.lastBandMode = this.BandwidthMode;
        this.lastCodec = this.Codec;
        this.last3DMode = this._3DMode;
    }

    private int CalculateSamplesRead(int readPos)
    {
        if (readPos >= this.lastReadPos)
        {
            return (readPos - this.lastReadPos);
        }
        return (((this.audioFrequency * 10) - this.lastReadPos) + readPos);
    }

    private bool CheckVAD(float[] samples)
    {
        if (Time.realtimeSinceStartup < (this.lastVTime + this.vadHangover))
        {
            return true;
        }
        float a = 0f;
        foreach (float num2 in samples)
        {
            a = Mathf.Max(a, Mathf.Abs(num2));
        }
        bool flag = a >= this.VolumeThreshold;
        if (flag)
        {
            this.lastVTime = Time.realtimeSinceStartup;
        }
        return flag;
    }

    public void DrawTalkControllerUI()
    {
        if (this.talkController != null)
        {
            this.talkController.OnInspectorGUI();
        }
        else
        {
            GUILayout.Label("No component available which implements IUSpeakTalkController\nReverting to default behavior - data is always sent", new GUILayoutOption[0]);
        }
    }

    private Component FindInputHandler()
    {
        foreach (Component component in base.GetComponents<Component>())
        {
            if (component is IUSpeakTalkController)
            {
                return component;
            }
        }
        return null;
    }

    private Component FindSpeechHandler()
    {
        foreach (Component component in base.GetComponents<Component>())
        {
            if (component is ISpeechDataHandler)
            {
                return component;
            }
        }
        return null;
    }

    public static USpeaker Get(Object source)
    {
        if (source is GameObject)
        {
            return (source as GameObject).GetComponent<USpeaker>();
        }
        if (source is Transform)
        {
            return (source as Transform).GetComponent<USpeaker>();
        }
        if (source is Component)
        {
            return (source as Component).GetComponent<USpeaker>();
        }
        return null;
    }

    public void GetInputHandler()
    {
        this.talkController = (IUSpeakTalkController) this.FindInputHandler();
    }

    public bool HasSettings()
    {
        return (this.settings != null);
    }

    public void InitializeSettings(int data)
    {
        MonoBehaviour.print("Settings changed");
        this.settings = new USpeakSettingsData((byte) data);
        this.Codec = this.settings.Codec;
    }

    private float[] normalize(float[] samples, float magnitude)
    {
        float[] numArray = new float[samples.Length];
        for (int i = 0; i < samples.Length; i++)
        {
            numArray[i] = samples[i] / magnitude;
        }
        return numArray;
    }

    private void OnAudioAvailable(float[] pcmData)
    {
        if (!this.UseVAD || this.CheckVAD(pcmData))
        {
            CurrentVolume = 0f;
            if (pcmData.Length > 0)
            {
                foreach (float num in pcmData)
                {
                    CurrentVolume += Mathf.Abs(num);
                }
                CurrentVolume /= (float) pcmData.Length;
            }
            int size = 0x500;
            foreach (float[] numArray2 in this.SplitArray(pcmData, size))
            {
                this.pendingEncode.Add(numArray2);
            }
        }
    }

    private void OnDestroy()
    {
        USpeakerList.Remove(this);
    }

    private void ProcessPendingEncode(float[] pcm)
    {
        int num;
        byte[] buffer = USpeakAudioClipCompressor.CompressAudioData(pcm, 1, out num, this.lastBandMode, this.codecMgr.Codecs[this.lastCodec], LocalGain);
        USpeakFrameContainer item = new USpeakFrameContainer {
            Samples = (ushort) num,
            encodedData = buffer
        };
        this.sendBuffer.Add(item);
    }

    private void ProcessPendingEncodeBuffer()
    {
        int num = 10;
        float num2 = ((float) num) / 1000f;
        float realtimeSinceStartup = Time.realtimeSinceStartup;
        while ((Time.realtimeSinceStartup <= (realtimeSinceStartup + num2)) && (this.pendingEncode.Count > 0))
        {
            float[] pcm = this.pendingEncode[0];
            this.pendingEncode.RemoveAt(0);
            this.ProcessPendingEncode(pcm);
        }
    }

    public void ReceiveAudio(byte[] data)
    {
        if (this.settings == null)
        {
            Debug.LogWarning("Trying to receive remote audio data without calling InitializeSettings!\nIncoming packet will be ignored");
        }
        else if ((!MuteAll && !this.Mute) && ((this.SpeakerMode != SpeakerMode.Local) || this.DebugPlayback))
        {
            byte[] @byte;
            if (this.SpeakerMode == SpeakerMode.Remote)
            {
                this.talkTimer = 1f;
            }
            for (int i = 0; i < data.Length; i += @byte.Length)
            {
                @byte = USpeakPoolUtils.GetByte(BitConverter.ToInt32(data, i) + 6);
                Array.Copy(data, i, @byte, 0, @byte.Length);
                USpeakFrameContainer container = new USpeakFrameContainer();
                container.LoadFrom(@byte);
                USpeakPoolUtils.Return(@byte);
                float[] sourceArray = USpeakAudioClipCompressor.DecompressAudio(container.encodedData, container.Samples, 1, false, this.settings.bandMode, this.codecMgr.Codecs[this.Codec], RemoteGain);
                float num3 = ((float) sourceArray.Length) / ((float) this.audioFrequency);
                this.received += num3;
                Array.Copy(sourceArray, 0, this.receivedData, this.index, sourceArray.Length);
                USpeakPoolUtils.Return(sourceArray);
                this.index += sourceArray.Length;
                if (this.index >= base.audio.clip.samples)
                {
                    this.index = 0;
                }
                base.audio.clip.SetData(this.receivedData, 0);
                if (!base.audio.isPlaying)
                {
                    this.shouldPlay = true;
                    if (this.playDelay <= 0f)
                    {
                        this.playDelay = num3 * 2f;
                    }
                }
            }
        }
    }

    private void RefreshDevices()
    {
        if (this.SpeakerMode != SpeakerMode.Local)
        {
            base.CancelInvoke("RefreshDevices");
        }
        else
        {
            this.devicesCached = Microphone.devices;
        }
    }

    public void SetInputDevice(int deviceID)
    {
        InputDeviceID = deviceID;
    }

    private List<float[]> SplitArray(float[] array, int size)
    {
        float[] numArray;
        List<float[]> list = new List<float[]>();
        for (int i = 0; i < array.Length; i += numArray.Length)
        {
            numArray = array.Skip<float>(i).Take<float>(size).ToArray<float>();
            list.Add(numArray);
        }
        return list;
    }

    [DebuggerHidden]
    private IEnumerator Start()
    {
        return new <Start>c__Iterator14 { <>f__this = this };
    }

    private void StopPlaying()
    {
        base.audio.Stop();
        base.audio.time = 0f;
        this.index = 0;
        this.played = 0.0;
        this.received = 0.0;
        this.lastTime = 0f;
    }

    private void Update()
    {
        this.talkTimer -= Time.deltaTime;
        base.audio.volume = this.SpeakerVolume;
        if (this.last3DMode != this._3DMode)
        {
            this.last3DMode = this._3DMode;
            this.StopPlaying();
            base.audio.clip = AudioClip.Create("vc", this.audioFrequency * 10, 1, this.audioFrequency, this._3DMode == ThreeDMode.Full3D, false);
            base.audio.loop = true;
        }
        if (this._3DMode == ThreeDMode.SpeakerPan)
        {
            Transform transform = Camera.main.transform;
            Vector3 rhs = Vector3.Cross(transform.up, transform.forward);
            rhs.Normalize();
            float y = Vector3.Dot(base.transform.position - transform.position, rhs);
            float x = Vector3.Dot(base.transform.position - transform.position, transform.forward);
            float num4 = Mathf.Sin(Mathf.Atan2(y, x));
            base.audio.pan = num4;
        }
        if (base.audio.isPlaying)
        {
            if (this.lastTime > base.audio.time)
            {
                this.played += base.audio.clip.length;
            }
            this.lastTime = base.audio.time;
            if ((this.played + base.audio.time) >= this.received)
            {
                this.StopPlaying();
                this.shouldPlay = false;
            }
        }
        else if (this.shouldPlay)
        {
            this.playDelay -= Time.deltaTime;
            if (this.playDelay <= 0f)
            {
                base.audio.Play();
            }
        }
        if ((this.SpeakerMode != SpeakerMode.Remote) && (this.audioHandler != null))
        {
            if (this.devicesCached == null)
            {
                this.devicesCached = Microphone.devices;
                base.InvokeRepeating("RefreshDevices", 4.2f, 4.2f);
            }
            string[] devicesCached = this.devicesCached;
            if (devicesCached.Length != 0)
            {
                if (devicesCached[Mathf.Min(InputDeviceID, devicesCached.Length - 1)] != this.currentDeviceName)
                {
                    this.currentDeviceName = devicesCached[Mathf.Min(InputDeviceID, devicesCached.Length - 1)];
                    MonoBehaviour.print("Using input device: " + this.currentDeviceName);
                    this.recording = Microphone.Start(this.currentDeviceName, false, 0x15, this.audioFrequency);
                    this.lastReadPos = 0;
                }
                if ((this.lastBandMode != this.BandwidthMode) || (this.lastCodec != this.Codec))
                {
                    this.UpdateSettings();
                    this.lastBandMode = this.BandwidthMode;
                    this.lastCodec = this.Codec;
                }
                int position = Microphone.GetPosition(null);
                if (position >= (this.audioFrequency * 20))
                {
                    position = 0;
                    this.lastReadPos = 0;
                    Object.DestroyImmediate(this.recording);
                    Microphone.End(null);
                    this.recording = Microphone.Start(this.currentDeviceName, false, 0x15, this.audioFrequency);
                }
                if (position > this.overlap)
                {
                    bool? nullable = null;
                    try
                    {
                        int num6 = position - this.lastReadPos;
                        int sampleSize = this.codecMgr.Codecs[this.Codec].GetSampleSize(this.audioFrequency);
                        if (sampleSize == 0)
                        {
                            sampleSize = 100;
                        }
                        if (sampleSize == 0)
                        {
                            if (num6 > sampleSize)
                            {
                                float[] data = new float[num6 - 1];
                                this.recording.GetData(data, this.lastReadPos);
                                if ((this.talkController == null) || this.talkController.ShouldSend())
                                {
                                    this.talkTimer = 1f;
                                    this.OnAudioAvailable(data);
                                }
                            }
                            this.lastReadPos = position;
                        }
                        else
                        {
                            int lastReadPos = this.lastReadPos;
                            int num9 = Mathf.FloorToInt((float) (num6 / sampleSize));
                            for (int i = 0; i < num9; i++)
                            {
                                float[] @float = USpeakPoolUtils.GetFloat(sampleSize);
                                this.recording.GetData(@float, lastReadPos);
                                if (!nullable.HasValue ? (nullable = new bool?((this.talkController != null) && this.talkController.ShouldSend())).Value : nullable.Value)
                                {
                                    this.talkTimer = 1f;
                                    this.OnAudioAvailable(@float);
                                }
                                USpeakPoolUtils.Return(@float);
                                lastReadPos += sampleSize;
                            }
                            this.lastReadPos = lastReadPos;
                        }
                    }
                    catch (Exception)
                    {
                    }
                    this.ProcessPendingEncodeBuffer();
                    bool flag = true;
                    if ((this.SendingMode == SendBehavior.RecordThenSend) && (this.talkController != null))
                    {
                        flag = !(!nullable.HasValue ? (nullable = new bool?(this.talkController.ShouldSend())).Value : nullable.Value);
                    }
                    this.sendTimer += Time.deltaTime;
                    if ((this.sendTimer >= this.sendt) && flag)
                    {
                        this.sendTimer = 0f;
                        this.tempSendBytes.Clear();
                        foreach (USpeakFrameContainer container in this.sendBuffer)
                        {
                            this.tempSendBytes.AddRange(container.ToByteArray());
                        }
                        this.sendBuffer.Clear();
                        if (this.tempSendBytes.Count > 0)
                        {
                            this.audioHandler.USpeakOnSerializeAudio(this.tempSendBytes.ToArray());
                        }
                    }
                }
            }
        }
    }

    private void UpdateSettings()
    {
        if (Application.isPlaying)
        {
            this.settings = new USpeakSettingsData();
            this.settings.bandMode = this.BandwidthMode;
            this.settings.Codec = this.Codec;
            this.audioHandler.USpeakInitializeSettings(this.settings.ToByte());
        }
    }

    private int audioFrequency
    {
        get
        {
            if (this.recFreq == 0)
            {
                switch (this.BandwidthMode)
                {
                    case BandMode.Narrow:
                        this.recFreq = 0x1f40;
                        goto Label_0069;

                    case BandMode.Wide:
                        this.recFreq = 0x3e80;
                        goto Label_0069;

                    case BandMode.UltraWide:
                        this.recFreq = 0x7d00;
                        goto Label_0069;
                }
                this.recFreq = 0x1f40;
            }
        Label_0069:
            return this.recFreq;
        }
    }

    [Obsolete("Use USpeaker._3DMode instead")]
    public bool Is3D
    {
        get
        {
            return (this._3DMode == ThreeDMode.SpeakerPan);
        }
        set
        {
            if (value)
            {
                this._3DMode = ThreeDMode.SpeakerPan;
            }
            else
            {
                this._3DMode = ThreeDMode.None;
            }
        }
    }

    public bool IsTalking
    {
        get
        {
            return (this.talkTimer > 0f);
        }
    }

    [CompilerGenerated]
    private sealed class <Start>c__Iterator14 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal USpeaker <>f__this;
        internal string[] <devices>__0;

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
                    this.$current = null;
                    this.$PC = 1;
                    goto Label_0199;

                case 1:
                    this.<>f__this.audioHandler = (ISpeechDataHandler) this.<>f__this.FindSpeechHandler();
                    this.<>f__this.talkController = (IUSpeakTalkController) this.<>f__this.FindInputHandler();
                    if (this.<>f__this.audioHandler != null)
                    {
                        if (this.<>f__this.SpeakerMode == SpeakerMode.Remote)
                        {
                            goto Label_0197;
                        }
                        if (this.<>f__this.AskPermission && !Application.HasUserAuthorization(UserAuthorization.Microphone))
                        {
                            this.$current = Application.RequestUserAuthorization(UserAuthorization.Microphone);
                            this.$PC = 2;
                            goto Label_0199;
                        }
                        break;
                    }
                    Debug.LogError("USpeaker requires a component which implements the ISpeechDataHandler interface");
                    goto Label_0197;

                case 2:
                    break;

                default:
                    goto Label_0197;
            }
            if (!Application.HasUserAuthorization(UserAuthorization.Microphone))
            {
                Debug.LogError("Failed to start recording - user has denied microphone access");
            }
            else
            {
                this.<devices>__0 = Microphone.devices;
                if (this.<devices>__0.Length == 0)
                {
                    Debug.LogWarning("Failed to find a recording device");
                }
                else
                {
                    this.<>f__this.UpdateSettings();
                    this.<>f__this.sendt = 1f / this.<>f__this.SendRate;
                    this.<>f__this.recording = Microphone.Start(this.<>f__this.currentDeviceName, false, 0x15, this.<>f__this.audioFrequency);
                    MonoBehaviour.print(this.<devices>__0[USpeaker.InputDeviceID]);
                    this.<>f__this.currentDeviceName = this.<devices>__0[USpeaker.InputDeviceID];
                    this.$PC = -1;
                }
            }
        Label_0197:
            return false;
        Label_0199:
            return true;
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

