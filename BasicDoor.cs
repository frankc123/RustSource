using Facepunch;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

[NGCAutoAddScript, AddComponentMenu("")]
public abstract class BasicDoor : NetBehaviour, IServerSaveable, IActivatable, IActivatableToggle, IContextRequestable, IContextRequestableMenu, IContextRequestableQuick, IContextRequestableStatus, IContextRequestableText, IContextRequestableSoleAccess, IContextRequestablePointText, IComponentInterface<IActivatable, MonoBehaviour, Activatable>, IComponentInterface<IActivatable, MonoBehaviour>, IComponentInterface<IActivatable>, IComponentInterface<IContextRequestable, MonoBehaviour, Contextual>, IComponentInterface<IContextRequestable, MonoBehaviour>, IComponentInterface<IContextRequestable>
{
    [NonSerialized]
    private bool capturedOriginals;
    [SerializeField]
    protected AudioClip closedSound;
    [SerializeField]
    protected AudioClip closeSound;
    [SerializeField]
    protected float durationClose = 1f;
    [SerializeField]
    protected float durationOpen = 1f;
    private const sbyte kClose = 0;
    private const float kMaxDistance = 20f;
    private const float kMinDistance = 5f;
    private const sbyte kOpenBackward = 2;
    private const sbyte kOpenForward = 1;
    private const RunFlags kRF_DefaultReverse_Mask = (RunFlags.ClosedNoReverse | RunFlags.ClosedReverse);
    private const RunFlags kRF_DefaultReverse_Value = RunFlags.ClosedReverse;
    private const RunFlags kRF_DisableReverse_Mask = RunFlags.ClosedNoReverse;
    private const RunFlags kRF_DisableReverse_Value = RunFlags.ClosedNoReverse;
    private const RunFlags kRF_FixedUpdate_Mask = RunFlags.FixedUpdateClosedForward;
    private const RunFlags kRF_FixedUpdate_Value = RunFlags.FixedUpdateClosedForward;
    private const RunFlags kRF_PointText_Mask = RunFlags.ClosedForwardWithPointText;
    private const RunFlags kRF_PointText_Value = RunFlags.ClosedForwardWithPointText;
    private const RunFlags kRF_StartOpen_Mask = RunFlags.OpenedForward;
    private const RunFlags kRF_StartOpen_Value = RunFlags.OpenedForward;
    private const RunFlags kRF_WaitsTarget_Mask = RunFlags.ClosedForwardWaits;
    private const RunFlags kRF_WaitsTarget_Value = RunFlags.ClosedForwardWaits;
    private const string kRPCName_ConnectSetup = "DOc";
    private const string kRPCName_SetOpenOrClosed = "DOo";
    private const float kVolume = 1f;
    [SerializeField]
    protected float minimumTimeBetweenOpenClose = 1f;
    [SerializeField]
    protected AudioClip openedSound;
    [NonSerialized]
    private bool openingInReverse;
    [SerializeField]
    protected AudioClip openSound;
    [NonSerialized]
    protected Vector3 originalLocalPosition;
    [NonSerialized]
    protected Quaternion originalLocalRotation;
    [NonSerialized]
    protected Vector3 originalLocalScale;
    [SerializeField]
    protected Vector3 pointTextPointClosed;
    [SerializeField]
    protected Vector3 pointTextPointOpened;
    [NonSerialized]
    private ulong? serverLastTimeStamp;
    [SerializeField]
    private RunFlags startConfig;
    [NonSerialized]
    private State state;
    [NonSerialized]
    private State target;
    [SerializeField]
    protected string textClose = "Close";
    [SerializeField]
    protected string textOpen = "Open";
    [NonSerialized]
    private ulong? timeStampChanged;

    protected BasicDoor()
    {
    }

    protected ActivationToggleState ActGetToggleState()
    {
        return (!this.on ? ActivationToggleState.Off : ActivationToggleState.On);
    }

    protected ActivationResult ActTrigger(Character instigator, ActivationToggleState toggleTarget, ulong timestamp)
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
            this.ToggleStateServer(timestamp, instigator);
            return (!this.on ? ActivationResult.Fail_Busy : ActivationResult.Success);
        }
        if (!this.on)
        {
            return ActivationResult.Fail_Redundant;
        }
        this.ToggleStateServer(timestamp, instigator);
        return (!this.on ? ActivationResult.Success : ActivationResult.Fail_Busy);
    }

    protected void Awake()
    {
        this.CaptureOriginals();
        this.openingInReverse = this.defaultReversed;
        this.InitializeObstacle();
        if (this.startsOpened)
        {
            this.target = this.state = State.Opened;
            this.DoDoorFraction(1.0);
        }
        else
        {
            this.target = this.state = State.Closed;
            this.DoDoorFraction(0.0);
        }
        base.enabled = false;
    }

    private Side CalculateOpenWay()
    {
        return ((!this.openingInReverse && this.canOpenReverse) ? Side.Reverse : Side.Forward);
    }

    private Side CalculateOpenWay(Vector3 worldPoint)
    {
        IdealSide side;
        if (!this.canOpenReverse || (((int) (side = this.IdealSideForPoint(worldPoint))) == 1))
        {
            return Side.Forward;
        }
        if (((int) side) == 0)
        {
            return (!this.openingInReverse ? Side.Reverse : Side.Forward);
        }
        return Side.Reverse;
    }

    private Side CalculateOpenWay(Vector3? worldPoint)
    {
        return (!worldPoint.HasValue ? this.CalculateOpenWay() : this.CalculateOpenWay(worldPoint.Value));
    }

    private void CaptureOriginals()
    {
        if (!this.capturedOriginals)
        {
            this.originalLocalRotation = base.transform.localRotation;
            this.originalLocalPosition = base.transform.localPosition;
            this.originalLocalScale = base.transform.localScale;
            this.capturedOriginals = true;
        }
    }

    protected ContextStatusFlags ContextStatusPoll()
    {
        switch (this.state)
        {
            case State.Opened:
            case State.Closed:
                return 0;
        }
        return (ContextStatusFlags.SpriteFlag0 | ContextStatusFlags.ObjectBusy);
    }

    protected string ContextText(Controllable localControllable)
    {
        switch (this.state)
        {
            case State.Opened:
                return this.textClose;

            case State.Closed:
                return this.textOpen;
        }
        return null;
    }

    protected bool ContextTextPoint(out Vector3 worldPoint)
    {
        if (this.pointText)
        {
            switch (this.state)
            {
                case State.Opened:
                    worldPoint = base.transform.TransformPoint(this.pointTextPointOpened);
                    return true;

                case State.Closed:
                    worldPoint = base.transform.TransformPoint(this.pointTextPointClosed);
                    return true;
            }
        }
        worldPoint = new Vector3();
        return false;
    }

    protected void DisableObstacle()
    {
    }

    [RPC]
    protected void DOc(sbyte open)
    {
        long num;
        this.CaptureOriginals();
        if (open != 0)
        {
            this.state = this.target = State.Opened;
            num = (long) (this.durationOpen * 1000.0);
            this.openingInReverse = open == 2;
            this.DoDoorFraction(1.0);
        }
        else
        {
            this.state = this.target = State.Closed;
            num = (long) (this.durationOpen * 1000.0);
            this.DoDoorFraction(0.0);
        }
        ulong time = BasicDoor.time;
        if (num > time)
        {
            this.timeStampChanged = new ulong?(time - ((ulong) num));
        }
        else
        {
            this.timeStampChanged = null;
        }
    }

    protected void DoDoorFraction(double fractionOpen)
    {
        if (this.openingInReverse)
        {
            this.OnDoorFraction(-fractionOpen);
        }
        else
        {
            this.OnDoorFraction(fractionOpen);
        }
    }

    [RPC]
    protected void DOo(sbyte open, ulong timestamp)
    {
        this.CaptureOriginals();
        if (open != 0)
        {
            this.openingInReverse = open == 2;
        }
        this.StartOpeningOrClosing(open, timestamp);
    }

    private void DoorUpdate()
    {
        double elapsed = this.elapsed;
        if (elapsed > 0.0)
        {
            bool flag = this.state != this.target;
            switch (this.target)
            {
                case State.Opened:
                    if (elapsed < this.durationOpen)
                    {
                        if (this.state == State.Closed)
                        {
                            this.OnDoorStartOpen();
                        }
                        this.state = State.Opening;
                        this.DoDoorFraction(elapsed / ((double) this.durationOpen));
                        break;
                    }
                    base.enabled = false;
                    this.state = State.Opened;
                    this.DoDoorFraction(1.0);
                    if (flag)
                    {
                        this.OnDoorEndOpen();
                    }
                    break;

                case State.Closed:
                    if (elapsed < this.durationClose)
                    {
                        if (this.state == State.Opened)
                        {
                            this.OnDoorStartClose();
                        }
                        this.state = State.Closing;
                        this.DoDoorFraction(1.0 - (elapsed / ((double) this.durationClose)));
                        break;
                    }
                    base.enabled = false;
                    this.state = State.Closed;
                    this.DoDoorFraction(0.0);
                    if (flag)
                    {
                        this.OnDoorEndClose();
                    }
                    break;
            }
        }
    }

    protected void EnableObstacle()
    {
    }

    protected void FixedUpdate()
    {
        if (this.fixedUpdate)
        {
            this.DoorUpdate();
        }
    }

    ActivationResult IActivatable.ActTrigger(Character instigator, ulong timestamp)
    {
        return this.ActTrigger(instigator, !this.on ? ActivationToggleState.On : ActivationToggleState.Off, timestamp);
    }

    ActivationToggleState IActivatableToggle.ActGetToggleState()
    {
        return this.ActGetToggleState();
    }

    ActivationResult IActivatableToggle.ActTrigger(Character instigator, ActivationToggleState toggleTarget, ulong timestamp)
    {
        return this.ActTrigger(instigator, toggleTarget, timestamp);
    }

    bool IContextRequestablePointText.ContextTextPoint(out Vector3 worldPoint)
    {
        return this.ContextTextPoint(out worldPoint);
    }

    ContextStatusFlags IContextRequestableStatus.ContextStatusPoll()
    {
        return this.ContextStatusPoll();
    }

    string IContextRequestableText.ContextText(Controllable localControllable)
    {
        return this.ContextText(localControllable);
    }

    protected abstract IdealSide IdealSideForPoint(Vector3 worldPoint);
    private void InitializeObstacle()
    {
        NavMeshObstacle component = base.GetComponent<NavMeshObstacle>();
        if (component != null)
        {
            Object.Destroy(component);
        }
    }

    protected void LateUpdate()
    {
        if (!this.fixedUpdate)
        {
            this.DoorUpdate();
        }
    }

    protected void OnDestroy()
    {
    }

    protected virtual void OnDoorEndClose()
    {
        this.PlaySound(this.closedSound);
        this.EnableObstacle();
    }

    protected void OnDoorEndOpen()
    {
        this.PlaySound(this.openedSound);
        this.DisableObstacle();
    }

    protected abstract void OnDoorFraction(double fractionOpen);
    protected virtual void OnDoorStartClose()
    {
        this.PlaySound(this.closeSound);
    }

    protected void OnDoorStartOpen()
    {
        this.PlaySound(this.openSound);
    }

    protected void PlayerConnected(PlayerClient player)
    {
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            clip.Play(base.transform.position, (float) 1f, (float) 5f, (float) 20f);
        }
    }

    protected void StartOpeningOrClosing(sbyte open, ulong timestamp)
    {
        State opened;
        long num;
        bool openingInReverse = this.openingInReverse;
        if (open != 0)
        {
            if (this.state == State.Closed)
            {
                openingInReverse = this.canOpenReverse && (open == 2);
            }
            opened = State.Opened;
            if ((opened == this.state) || (opened == ((byte) (((int) this.state) + 1))))
            {
                return;
            }
            double elapsed = this.elapsed;
            double num3 = (this.durationClose > 0.0) ? ((elapsed < this.durationClose) ? (1.0 - (elapsed / ((double) this.durationClose))) : 0.0) : 0.0;
            num = (long) ((num3 * this.durationOpen) * 1000.0);
        }
        else
        {
            opened = State.Closed;
            if ((opened == this.state) || (opened == ((byte) (((int) this.state) + 1))))
            {
                return;
            }
            double num4 = this.elapsed;
            double num5 = (this.durationOpen > 0.0) ? ((num4 < this.durationOpen) ? (num4 / ((double) this.durationOpen)) : 1.0) : 1.0;
            num = (long) (((1.0 - num5) * this.durationClose) * 1000.0);
        }
        if (num > timestamp)
        {
            this.timeStampChanged = null;
        }
        else
        {
            this.timeStampChanged = new ulong?(timestamp - ((ulong) num));
        }
        base.enabled = true;
        this.openingInReverse = openingInReverse;
        this.target = opened;
    }

    private bool ToggleStateServer(ulong timestamp, Character instigator)
    {
        if (instigator != null)
        {
            return this.ToggleStateServer(new Vector3?(instigator.eyesOrigin), timestamp, null);
        }
        return this.ToggleStateServer(null, timestamp, null);
    }

    private bool ToggleStateServer(Vector3? openerPoint, ulong timestamp, bool? fallbackReverse = new bool?())
    {
        if (!this.serverLastTimeStamp.HasValue || (timestamp > this.serverLastTimeStamp.Value))
        {
            if (this.waitsTarget && ((this.state == State.Opening) || (this.state == State.Closing)))
            {
                return false;
            }
            this.serverLastTimeStamp = new ulong?(timestamp);
            State target = this.target;
            bool openingInReverse = this.openingInReverse;
            if (this.target == State.Closed)
            {
                if (openerPoint.HasValue || !fallbackReverse.HasValue)
                {
                    if (this.CalculateOpenWay(openerPoint) == Side.Forward)
                    {
                        this.StartOpeningOrClosing(1, timestamp);
                    }
                    else
                    {
                        this.StartOpeningOrClosing(2, timestamp);
                    }
                }
                else
                {
                    this.StartOpeningOrClosing(!(!fallbackReverse.HasValue ? this.defaultReversed : fallbackReverse.Value) ? ((sbyte) 1) : ((sbyte) 2), timestamp);
                }
            }
            else
            {
                this.StartOpeningOrClosing(0, timestamp);
            }
            if ((target != this.target) || (openingInReverse != this.openingInReverse))
            {
                return true;
            }
        }
        return false;
    }

    public bool canOpenReverse
    {
        get
        {
            return !this.reverseOpenDisabled;
        }
        protected set
        {
            this.reverseOpenDisabled = !value;
        }
    }

    public bool defaultReversed
    {
        get
        {
            return ((this.startConfig & (RunFlags.ClosedNoReverse | RunFlags.ClosedReverse)) == RunFlags.ClosedReverse);
        }
        protected set
        {
            if (value)
            {
                this.startConfig |= RunFlags.ClosedReverse;
            }
            else
            {
                this.startConfig &= ~RunFlags.ClosedReverse;
            }
        }
    }

    protected double elapsed
    {
        get
        {
            if (this.timeStampChanged.HasValue)
            {
                return (((double) (time - this.timeStampChanged.Value)) / 1000.0);
            }
            return double.PositiveInfinity;
        }
    }

    public bool fixedUpdate
    {
        get
        {
            return ((this.startConfig & RunFlags.FixedUpdateClosedForward) == RunFlags.FixedUpdateClosedForward);
        }
        protected set
        {
            if (value)
            {
                this.startConfig |= RunFlags.FixedUpdateClosedForward;
            }
            else
            {
                this.startConfig &= ~RunFlags.FixedUpdateClosedForward;
            }
        }
    }

    private bool on
    {
        get
        {
            return ((this.target == State.Opened) || (this.target == State.Opening));
        }
    }

    public bool pointText
    {
        get
        {
            return ((this.startConfig & RunFlags.ClosedForwardWithPointText) == RunFlags.ClosedForwardWithPointText);
        }
        protected set
        {
            if (value)
            {
                this.startConfig |= RunFlags.ClosedForwardWithPointText;
            }
            else
            {
                this.startConfig &= ~RunFlags.ClosedForwardWithPointText;
            }
        }
    }

    public bool reverseOpenDisabled
    {
        get
        {
            return ((this.startConfig & RunFlags.ClosedNoReverse) == RunFlags.ClosedNoReverse);
        }
        protected set
        {
            if (value)
            {
                this.startConfig |= RunFlags.ClosedNoReverse;
            }
            else
            {
                this.startConfig &= ~RunFlags.ClosedNoReverse;
            }
        }
    }

    public bool startsOpened
    {
        get
        {
            return ((this.startConfig & RunFlags.OpenedForward) == RunFlags.OpenedForward);
        }
        protected set
        {
            if (value)
            {
                this.startConfig |= RunFlags.OpenedForward;
            }
            else
            {
                this.startConfig &= ~RunFlags.OpenedForward;
            }
        }
    }

    protected static ulong time
    {
        get
        {
            return NetCull.timeInMillis;
        }
    }

    public bool waitsTarget
    {
        get
        {
            return ((this.startConfig & RunFlags.ClosedForwardWaits) == RunFlags.ClosedForwardWaits);
        }
        protected set
        {
            if (value)
            {
                this.startConfig |= RunFlags.ClosedForwardWaits;
            }
            else
            {
                this.startConfig &= ~RunFlags.ClosedForwardWaits;
            }
        }
    }

    protected enum IdealSide : sbyte
    {
        Forward = 1,
        Reverse = -1,
        Unknown = 0
    }

    private enum RunFlags
    {
        ClosedForward = 0,
        ClosedForwardWaits = 0x20,
        ClosedForwardWaitsWithPointText = 0x24,
        ClosedForwardWithPointText = 4,
        ClosedNoReverse = 0x10,
        ClosedNoReverseWaits = 0x30,
        ClosedNoReverseWithPointText = 20,
        ClosedNoReverseWithPointTextWaits = 0x34,
        ClosedReverse = 2,
        ClosedReverseWaits = 0x22,
        ClosedReverseWaitsWithPointText = 0x26,
        ClosedReverseWithPointText = 6,
        FixedUpdateClosedForward = 8,
        FixedUpdateClosedForwardWaits = 40,
        FixedUpdateClosedForwardWaitsWithPointText = 0x2c,
        FixedUpdateClosedForwardWithPointText = 12,
        FixedUpdateClosedNoReverse = 0x18,
        FixedUpdateClosedNoReverseWaits = 0x38,
        FixedUpdateClosedNoReverseWaitsWithPointText = 60,
        FixedUpdateClosedNoReverseWithPointText = 0x1c,
        FixedUpdateClosedReverse = 10,
        FixedUpdateClosedReverseWaits = 0x2a,
        FixedUpdateClosedReverseWaitsWithPointText = 0x2e,
        FixedUpdateClosedReverseWithPointText = 14,
        FixedUpdateOpenedForward = 9,
        FixedUpdateOpenedForwardWaits = 0x29,
        FixedUpdateOpenedForwardWaitsWithPointText = 0x2d,
        FixedUpdateOpenedForwardWithPointText = 13,
        FixedUpdateOpenedNoReverse = 0x19,
        FixedUpdateOpenedNoReverseWaits = 0x39,
        FixedUpdateOpenedNoReverseWaitsWithPointText = 0x3d,
        FixedUpdateOpenedNoReverseWithPointText = 0x1d,
        FixedUpdateOpenedReverse = 11,
        FixedUpdateOpenedReverseWaits = 0x2b,
        FixedUpdateOpenedReverseWaitsWithPointText = 0x2f,
        FixedUpdateOpenedReverseWithPointText = 15,
        OpenedForward = 1,
        OpenedForwardWaits = 0x21,
        OpenedForwardWaitsWithPointText = 0x25,
        OpenedForwardWithPointText = 5,
        OpenedNoReverse = 0x11,
        OpenedNoReverseWaits = 0x31,
        OpenedNoReverseWithPointText = 0x15,
        OpenedNoReverseWithPointTextWaits = 0x35,
        OpenedReverse = 3,
        OpenedReverseWaits = 0x23,
        OpenedReverseWaitsWithPointText = 0x27,
        OpenedReverseWithPointText = 7
    }

    private enum Side : byte
    {
        Forward = 0,
        Reverse = 1
    }

    private enum State : byte
    {
        Closed = 3,
        Closing = 2,
        Opened = 1,
        Opening = 0
    }
}

