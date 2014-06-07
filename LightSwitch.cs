using Facepunch;
using System;
using System.IO;
using System.Runtime.InteropServices;
using uLink;
using UnityEngine;

[NGCAutoAddScript]
public class LightSwitch : NetBehaviour, IActivatable, IActivatableToggle, IContextRequestable, IContextRequestableQuick, IContextRequestableText, IContextRequestablePointText, IComponentInterface<IActivatable, MonoBehaviour, Activatable>, IComponentInterface<IActivatable, MonoBehaviour>, IComponentInterface<IActivatable>, IComponentInterface<IContextRequestable, MonoBehaviour, Contextual>, IComponentInterface<IContextRequestable, MonoBehaviour>, IComponentInterface<IContextRequestable>
{
    [SerializeField]
    private int _randSeed;
    [SerializeField]
    private bool _startsOn;
    private double lastChangeTime;
    private sbyte lastPickedOff;
    private sbyte lastPickedOn;
    [SerializeField]
    protected float maxOffFadeDuration;
    [SerializeField]
    protected float maxOnFadeDuration;
    [SerializeField]
    protected float minOffFadeDuration;
    [SerializeField]
    protected float minOnFadeDuration;
    private bool on;
    private SeededRandom rand;
    [SerializeField]
    protected LightStyle[] randOff;
    [SerializeField]
    protected LightStyle[] randOn;
    private bool registeredConnectCallback;
    private StylistCTX[] stylistCTX;
    [SerializeField]
    protected LightStylist[] stylists;
    [SerializeField]
    protected string textTurnOff = "Flick Down";
    [SerializeField]
    protected string textTurnOn = "Flick Up";

    public ActivationToggleState ActGetToggleState()
    {
        return (!this.on ? ActivationToggleState.Off : ActivationToggleState.On);
    }

    public ActivationResult ActTrigger(Character instigator, ulong timestamp)
    {
        return this.ActTrigger(instigator, !this.on ? ActivationToggleState.On : ActivationToggleState.Off, timestamp);
    }

    public ActivationResult ActTrigger(Character instigator, ActivationToggleState toggleTarget, ulong timestamp)
    {
        ActivationToggleState state = toggleTarget;
        if (state != ActivationToggleState.On)
        {
            if (state != ActivationToggleState.Off)
            {
                return ActivationResult.Fail_BadToggle;
            }
        }
        else
        {
            if (this.on)
            {
                return ActivationResult.Fail_Redundant;
            }
            this.ServerToggle(timestamp);
            return (!this.on ? ActivationResult.Fail_Busy : ActivationResult.Success);
        }
        if (!this.on)
        {
            return ActivationResult.Fail_Redundant;
        }
        this.ServerToggle(timestamp);
        return (!this.on ? ActivationResult.Success : ActivationResult.Fail_Busy);
    }

    private void Awake()
    {
        this.rand = new SeededRandom(this.randSeed);
        MakeCTX(ref this.stylists, ref this.stylistCTX);
        if (this.stylists != null)
        {
            for (int i = 0; i < this.stylists.Length; i++)
            {
                if (this.stylists[i] != null)
                {
                    this.stylists[i] = this.stylists[i].ensuredAwake;
                }
            }
        }
    }

    [RPC]
    private void ConnectSetup(byte[] data)
    {
        using (MemoryStream stream = new MemoryStream(data))
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                this.Read(reader);
            }
        }
    }

    public ContextExecution ContextQuery(Controllable controllable, ulong timestamp)
    {
        return (ContextExecution.NotAvailable | ContextExecution.Quick);
    }

    public ContextResponse ContextRespondQuick(Controllable controllable, ulong timestamp)
    {
        this.ServerToggle(timestamp);
        return ContextResponse.DoneBreak;
    }

    public string ContextText(Controllable localControllable)
    {
        if (this.on)
        {
            return this.textTurnOff;
        }
        return this.textTurnOn;
    }

    public bool ContextTextPoint(out Vector3 worldPoint)
    {
        worldPoint = new Vector3();
        return false;
    }

    private static void DefaultArray(string test, ref LightStyle[] array)
    {
        if (array == null)
        {
            LightStyle style = test;
            if (style != null)
            {
                LightStyle[] styleArray1 = new LightStyle[] { style };
                array = styleArray1;
            }
            else
            {
                array = new LightStyle[0];
            }
        }
        else if (array.Length == 0)
        {
            LightStyle style2 = test;
            if (style2 != null)
            {
                LightStyle[] styleArray2 = new LightStyle[] { style2 };
                array = styleArray2;
            }
        }
    }

    private void JumpUpdate()
    {
        double time = NetCull.time - this.lastChangeTime;
        if (this.on)
        {
            int index = 0;
            int num3 = (this.randOn != null) ? this.randOn.Length : 0;
            while (index < this.stylistCTX.Length)
            {
                if (((this.stylists[index] != null) && (this.stylistCTX[index].lastOnStyle >= 0)) && ((this.stylistCTX[index].lastOnStyle < num3) && (this.randOn[(int) this.stylistCTX[index].lastOnStyle] != null)))
                {
                    this.stylists[index].Play(this.randOn[(int) this.stylistCTX[index].lastOnStyle], time);
                }
                else
                {
                    Debug.Log("Did not set on " + index, this);
                }
                index++;
            }
        }
        else
        {
            int num4 = 0;
            int num5 = (this.randOff != null) ? this.randOff.Length : 0;
            while (num4 < this.stylistCTX.Length)
            {
                if (((this.stylists[num4] != null) && (this.stylistCTX[num4].lastOffStyle >= 0)) && ((this.stylistCTX[num4].lastOffStyle < num5) && (this.randOff[(int) this.stylistCTX[num4].lastOffStyle] != null)))
                {
                    this.stylists[num4].Play(this.randOff[(int) this.stylistCTX[num4].lastOffStyle], time);
                }
                else
                {
                    Debug.Log("Did not set off " + num4, this);
                }
                num4++;
            }
        }
    }

    private static bool MakeCTX(ref LightStylist[] stylists, ref StylistCTX[] ctx)
    {
        int length;
        if (stylists == null)
        {
            length = 0;
        }
        else
        {
            length = stylists.Length;
        }
        Array.Resize<StylistCTX>(ref ctx, length);
        return (length > 0);
    }

    private void OnDestroy()
    {
        if (this.registeredConnectCallback)
        {
            GameEvent.PlayerConnected -= new GameEvent.OnPlayerConnectedHandler(this.PlayerConnected);
            this.registeredConnectCallback = false;
        }
    }

    public void PlayerConnected(PlayerClient player)
    {
        byte[] buffer = new byte[this.StreamSize];
        using (MemoryStream stream = new MemoryStream(buffer))
        {
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                this.Write(writer);
            }
        }
        NetCull.RPC<byte[]>((MonoBehaviour) this, "ConnectSetup", player.netPlayer, buffer);
    }

    private void Read(BinaryReader reader)
    {
        this.lastChangeTime = reader.ReadDouble();
        this.on = this.lastChangeTime > 0.0;
        if (!this.on)
        {
            this.lastChangeTime = -this.lastChangeTime;
        }
        int seed = reader.ReadInt32();
        uint num2 = reader.ReadUInt32();
        byte newSize = reader.ReadByte();
        Array.Resize<StylistCTX>(ref this.stylistCTX, newSize);
        Array.Resize<LightStylist>(ref this.stylists, newSize);
        for (int i = 0; i < newSize; i++)
        {
            this.stylistCTX[i].Read(reader);
        }
        if (seed != this.rand.Seed)
        {
            this._randSeed = seed;
            this.rand = new SeededRandom(seed);
        }
        this.rand.PositionData = num2;
        this.JumpUpdate();
    }

    [RPC]
    protected void ReadState(bool on, NetworkMessageInfo info)
    {
        this.lastChangeTime = info.timestampInMillis;
        this.on = on;
        if (on)
        {
            this.TurnOn();
        }
        else
        {
            this.TurnOff();
        }
    }

    private void Reset()
    {
        this._randSeed = Random.Range(0, 0x7fffffff);
        if (this.stylists == null)
        {
            this.stylists = new LightStylist[0];
        }
        DefaultArray("on", ref this.randOn);
        DefaultArray("off", ref this.randOff);
    }

    private void ServerToggle(ulong timestamp)
    {
        this.on = !this.on;
        this.lastChangeTime = ((double) timestamp) / 1000.0;
        if (this.on)
        {
            this.TurnOn();
        }
        else
        {
            this.TurnOff();
        }
        NetCull.RPC<bool>((MonoBehaviour) this, "ReadState", RPCMode.Others, this.on);
    }

    private void TurnOff()
    {
        if ((this.randOff == null) || (this.randOff.Length == 0))
        {
            Debug.LogError("Theres no light styles in randOn", this);
        }
        else
        {
            int length = this.randOff.Length;
            for (int i = 0; i < this.stylistCTX.Length; i++)
            {
                this.stylistCTX[i].lastOffStyle = (sbyte) this.rand.RandomIndex(length);
                if (this.stylists[i] != null)
                {
                    this.stylists[i].CrossFade(this.randOff[(int) this.stylistCTX[i].lastOffStyle], Random.Range(this.minOffFadeDuration, this.maxOffFadeDuration));
                }
            }
        }
    }

    private void TurnOn()
    {
        if ((this.randOn == null) || (this.randOn.Length == 0))
        {
            Debug.LogError("Theres no light styles in randOn", this);
        }
        else
        {
            int length = this.randOn.Length;
            for (int i = 0; i < this.stylistCTX.Length; i++)
            {
                this.stylistCTX[i].lastOnStyle = (sbyte) this.rand.RandomIndex(length);
                if (this.stylists[i] != null)
                {
                    this.stylists[i].CrossFade(this.randOn[(int) this.stylistCTX[i].lastOnStyle], Random.Range(this.minOnFadeDuration, this.maxOnFadeDuration));
                }
            }
        }
    }

    private void Write(BinaryWriter writer)
    {
        writer.Write(!this.on ? -this.lastChangeTime : this.lastChangeTime);
        writer.Write(this.rand.Seed);
        writer.Write(this.rand.PositionData);
        writer.Write((byte) this.stylistCTX.Length);
        for (int i = 0; i < this.stylistCTX.Length; i++)
        {
            this.stylistCTX[i].Write(writer);
        }
    }

    protected int randSeed
    {
        get
        {
            return this._randSeed;
        }
    }

    protected bool startsOn
    {
        get
        {
            return this._startsOn;
        }
        private set
        {
            this._startsOn = value;
        }
    }

    private int StreamSize
    {
        get
        {
            return (0x11 + (this.stylistCTX.Length * 2));
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct StylistCTX
    {
        public const int SIZE = 2;
        public sbyte lastOnStyle;
        public sbyte lastOffStyle;
        public void Write(BinaryWriter writer)
        {
            writer.Write(this.lastOnStyle);
            writer.Write(this.lastOffStyle);
        }

        public void Read(BinaryReader reader)
        {
            this.lastOnStyle = reader.ReadSByte();
            this.lastOffStyle = reader.ReadSByte();
        }
    }
}

