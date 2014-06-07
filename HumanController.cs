using Facepunch.Clocks.Counters;
using Facepunch.Cursor;
using System;
using System.Runtime.InteropServices;
using uLink;
using UnityEngine;

public class HumanController : Controller, RagdollTransferInfoProvider
{
    [NonSerialized]
    private CacheRef<Inventory> __inventory;
    [NonSerialized]
    private HumanControlConfiguration _controlConfig;
    [NonSerialized]
    private bool _didControlConfigTest;
    [NonSerialized]
    private Transform _headBone;
    [NonSerialized]
    protected int badPacketCount;
    private const long clearOnDisableCharacterStateFlags = 0x14fL;
    [NonSerialized]
    private ClientVitalsSync clientVitalsSync;
    [NonSerialized]
    private SystemTimestamp clock;
    [NonSerialized]
    private ContextProbe contextProbe;
    [NonSerialized]
    private Crouchable.Smoothing crouch_smoothing;
    [NonSerialized]
    private bool crouch_was_blocked;
    [NonSerialized]
    private bool crouching;
    [NonSerialized]
    private float crouchInMulTime;
    [NonSerialized]
    private float crouchTime;
    [NonSerialized]
    private DeathTransfer deathTransfer;
    private const ushort doNotClearOnDisableCharacterStateFlags = 0x1eb0;
    [NonSerialized]
    private bool exitingCrouch;
    [NonSerialized]
    private bool exitingSprint;
    [NonSerialized]
    private bool firstState;
    [NonSerialized]
    private PlayerClient instantiatedPlayerClient;
    protected const Controller.ControllerFlags kControllerFlags = (Controller.ControllerFlags.IncompatibleAsLocalAI | Controller.ControllerFlags.DisableWhenRemoteAI | Controller.ControllerFlags.DisableWhenRemotePlayer | Controller.ControllerFlags.EnableWhenLocalPlayer);
    private const string kHeadPath = "RustPlayer_Pelvis/RustPlayer_Spine/RustPlayer_Spine1/RustPlayer_Spine2/RustPlayer_Spine4/RustPlayer_Neck1/RustPlayer_Head1";
    [NonSerialized]
    private float landingSpeedPenaltyTime;
    private Vector3 lastFrameVelocity;
    [NonSerialized]
    private LocalRadiationEffect localRadiation;
    [NonSerialized]
    private float magnitudeAir;
    private Vector3 midairStartPos;
    [NonSerialized]
    private bool onceClock;
    [NonSerialized]
    private bool onceEngaged;
    [NonSerialized]
    private PlayerProxyTest proxyTest;
    [NonSerialized]
    private Vector3 server_last_pos;
    [NonSerialized]
    private float server_next_fall_damage_time;
    [NonSerialized]
    private bool server_was_grounded;
    [NonSerialized]
    private bool sprinting;
    [NonSerialized]
    private float sprintInMulTime;
    [NonSerialized]
    private float sprintTime;
    private const bool stepMotorHere = true;
    [NonSerialized]
    private bool? thatsRightPatWeDontNeedComments;
    [NonSerialized]
    private bool wasInAir;
    [NonSerialized]
    private bool wasSprinting;

    public HumanController() : this(Controller.ControllerFlags.IncompatibleAsLocalAI | Controller.ControllerFlags.DisableWhenRemoteAI | Controller.ControllerFlags.DisableWhenRemotePlayer | Controller.ControllerFlags.EnableWhenLocalPlayer)
    {
    }

    protected HumanController(Controller.ControllerFlags controllerFlags) : base(controllerFlags)
    {
        this.firstState = true;
        this.sprintInMulTime = 1f;
        this.crouchInMulTime = 1f;
        this.server_last_pos = Vector3.zero;
        this.server_was_grounded = true;
        this.landingSpeedPenaltyTime = float.MaxValue;
    }

    private void CheckBeltUsage()
    {
        if ((!UIUnityEvents.shouldBlockButtonInput && base.enabled) && !ConsoleWindow.IsVisible())
        {
            Inventory inventory = this.inventory;
            if (inventory != null)
            {
                InventoryHolder inventoryHolder = inventory.inventoryHolder;
                if (inventoryHolder != null)
                {
                    int beltNum = InputSample.PollItemButtons();
                    if (beltNum != -1)
                    {
                        inventoryHolder.BeltUse(beltNum);
                    }
                }
            }
        }
    }

    [RPC]
    private void GetClientMove(Vector3 origin, int encoded, ushort stateFlags, NetworkMessageInfo info)
    {
    }

    protected override void OnControlCease()
    {
        if (base.localControlled)
        {
            CameraMount componentInChildren = base.GetComponentInChildren<CameraMount>();
            if (componentInChildren != null)
            {
                componentInChildren.open = false;
            }
        }
        base.RemoveAddon<ContextProbe>(ref this.contextProbe);
        base.RemoveAddon<LocalRadiationEffect>(ref this.localRadiation);
        base.enabled = false;
        if (base.localControlled)
        {
            if (this.proxyTest != null)
            {
                this.proxyTest.treatAsProxy = true;
            }
            if (this._inventory != null)
            {
                this._inventory.DeactivateItem();
            }
        }
        base.OnControlCease();
    }

    protected override void OnControlEngauge()
    {
        base.OnControlEngauge();
        if (base.localControlled)
        {
            CameraMount componentInChildren = base.GetComponentInChildren<CameraMount>();
            if (componentInChildren != null)
            {
                componentInChildren.open = true;
            }
            this.contextProbe = (this.contextProbe == null) ? base.AddAddon<ContextProbe>() : this.contextProbe;
            this.localRadiation = (this.localRadiation == null) ? base.AddAddon<LocalRadiationEffect>() : this.localRadiation;
            if (this.onceEngaged)
            {
                if (this.proxyTest != null)
                {
                    this.proxyTest.treatAsProxy = false;
                }
            }
            else
            {
                this.proxyTest = base.GetComponent<PlayerProxyTest>();
                this.onceEngaged = true;
            }
            base.enabled = true;
        }
    }

    protected override void OnControlEnter()
    {
        base.OnControlEnter();
        if (base.localControlled)
        {
            this.clientVitalsSync = base.AddAddon<ClientVitalsSync>();
            ImageEffectManager.GetInstance<GameFullscreen>().fadeColor = Color.black;
            ImageEffectManager.GetInstance<GameFullscreen>().tintColor = Color.white;
            RPOS.DoFade(2f, 2.5f, Color.clear);
            RPOS.SetCurrentFade(Color.black);
            RPOS.HealthUpdate(base.health);
            RPOS.ObservedPlayer = base.controllable;
        }
    }

    protected override void OnControlExit()
    {
        base.RemoveAddon<ClientVitalsSync>(ref this.clientVitalsSync);
        base.OnControlExit();
    }

    protected void OnDisable()
    {
        if (Application.isPlaying)
        {
            Character idMain = base.idMain;
            if (idMain != null)
            {
                idMain.stateFlags.flags = (ushort) (idMain.stateFlags.flags & 0x1eb0);
            }
            this.SetLocalOnlyComponentsEnabled(false);
        }
        this.sprinting = false;
        this.exitingSprint = true;
    }

    protected void OnEnable()
    {
        this.SetLocalOnlyComponentsEnabled(true);
        LockCursorManager.IsLocked(true);
        this.onceClock = false;
        this.clock = SystemTimestamp.Restart;
    }

    protected override void OnLocalPlayerPreRender()
    {
        InventoryHolder inventoryHolder = this.inventoryHolder;
        if (inventoryHolder != null)
        {
            inventoryHolder.InvokeInputItemPreRender();
        }
    }

    private void ProcessInput(ref InputSample sample)
    {
        bool isGrounded;
        bool isSliding;
        CCMotor ccmotor = base.ccmotor;
        if (ccmotor != null)
        {
            CCMotor.InputFrame frame;
            isGrounded = ccmotor.isGrounded;
            isSliding = ccmotor.isSliding;
            if (!isGrounded && !isSliding)
            {
                sample.sprint = false;
                sample.crouch = false;
                sample.aim = false;
                sample.info__crouchBlocked = false;
                if (!this.wasInAir)
                {
                    this.wasInAir = true;
                    this.magnitudeAir = ccmotor.input.moveDirection.magnitude;
                    this.midairStartPos = base.transform.position;
                }
                this.lastFrameVelocity = ccmotor.velocity;
            }
            else if (this.wasInAir)
            {
                this.wasInAir = false;
                this.magnitudeAir = 1f;
                this.landingSpeedPenaltyTime = 0f;
                if ((base.transform.position.y < this.midairStartPos.y) && (Mathf.Abs((float) (base.transform.position.y - this.midairStartPos.y)) > 2f))
                {
                    base.idMain.GetLocal<FallDamage>().SendFallImpact(this.lastFrameVelocity);
                }
                this.lastFrameVelocity = Vector3.zero;
                this.midairStartPos = Vector3.zero;
            }
            bool flag3 = sample.crouch || sample.info__crouchBlocked;
            frame.jump = sample.jump;
            frame.moveDirection.x = sample.strafe;
            frame.moveDirection.y = 0f;
            frame.moveDirection.z = sample.walk;
            frame.crouchSpeed = !sample.crouch ? 1f : -1f;
            if (frame.moveDirection != Vector3.zero)
            {
                float num2;
                float num3;
                float magnitude = frame.moveDirection.magnitude;
                if (magnitude < 1f)
                {
                    frame.moveDirection = (Vector3) (frame.moveDirection / magnitude);
                    magnitude *= magnitude;
                    frame.moveDirection = (Vector3) (frame.moveDirection * magnitude);
                }
                else if (magnitude > 1f)
                {
                    frame.moveDirection = (Vector3) (frame.moveDirection / magnitude);
                }
                if (InputSample.MovementScale < 1f)
                {
                    if (InputSample.MovementScale > 0f)
                    {
                        frame.moveDirection = (Vector3) (frame.moveDirection * InputSample.MovementScale);
                    }
                    else
                    {
                        frame.moveDirection = Vector3.zero;
                    }
                }
                Vector3 moveDirection = frame.moveDirection;
                moveDirection.x *= this.controlConfig.sprintScaleX;
                moveDirection.z *= this.controlConfig.sprintScaleY;
                if ((sample.sprint && !flag3) && !sample.aim)
                {
                    num2 = Time.deltaTime * this.sprintInMulTime;
                }
                else
                {
                    sample.sprint = false;
                    num2 = -Time.deltaTime;
                }
                frame.moveDirection += (Vector3) (moveDirection * this.controlConfig.curveSprintAddSpeedByTime.EvaluateClampedTime(ref this.sprintTime, num2));
                if (flag3)
                {
                    num3 = Time.deltaTime * this.crouchInMulTime;
                }
                else
                {
                    num3 = -Time.deltaTime;
                }
                frame.moveDirection = (Vector3) (frame.moveDirection * this.controlConfig.curveCrouchMulSpeedByTime.EvaluateClampedTime(ref this.crouchTime, num3));
                frame.moveDirection = base.transform.TransformDirection(frame.moveDirection);
                if (this.wasInAir)
                {
                    float a = frame.moveDirection.magnitude;
                    if (!Mathf.Approximately(a, this.magnitudeAir))
                    {
                        frame.moveDirection = (Vector3) (frame.moveDirection / a);
                        frame.moveDirection = (Vector3) (frame.moveDirection * this.magnitudeAir);
                    }
                }
                else
                {
                    frame.moveDirection = (Vector3) (frame.moveDirection * this.controlConfig.curveLandingSpeedPenalty.EvaluateClampedTime(ref this.landingSpeedPenaltyTime, Time.deltaTime));
                }
            }
            else
            {
                this.sprinting = false;
                this.exitingSprint = false;
                this.sprintTime = 0f;
                this.crouchTime = !sample.crouch ? 0f : this.controlConfig.curveCrouchMulSpeedByTime.GetEndTime();
                this.magnitudeAir = 1f;
            }
            if (DebugInput.GetKey(KeyCode.H))
            {
                frame.moveDirection = (Vector3) (frame.moveDirection * 100f);
            }
            ccmotor.input = frame;
            if (ccmotor.stepMode == CCMotor.StepMode.Elsewhere)
            {
                ccmotor.Step();
            }
        }
        else
        {
            isSliding = false;
            isGrounded = true;
        }
        Character idMain = base.idMain;
        Crouchable crouchable = idMain.crouchable;
        if (idMain != null)
        {
            Angle2 eyesAngles = base.eyesAngles;
            eyesAngles.yaw = Mathf.DeltaAngle(0f, base.eyesAngles.yaw + sample.yaw);
            eyesAngles.pitch = base.ClampPitch((float) (eyesAngles.pitch + sample.pitch));
            base.eyesAngles = eyesAngles;
            ushort flags = idMain.stateFlags.flags;
            if (crouchable != null)
            {
                this.crouch_smoothing.AddSeconds((double) Time.deltaTime);
                crouchable.LocalPlayerUpdateCrouchState(ccmotor, ref sample.crouch, ref sample.info__crouchBlocked, ref this.crouch_smoothing);
            }
            int num6 = ((((((((((((!sample.aim ? 0 : 4) | (!sample.sprint ? 0 : 2)) | (!sample.attack ? 0 : 8)) | (!sample.attack2 ? 0 : 0x100)) | (!sample.crouch ? 0 : 1)) | (((sample.strafe == 0f) && (sample.walk == 0f)) ? 0 : 0x40)) | (!LockCursorManager.IsLocked() ? 0x80 : 0)) | (!isGrounded ? 0x10 : 0)) | (!isSliding ? 0 : 0x20)) | (!this.bleeding ? 0 : 0x200)) | (!sample.lamp ? 0 : 0x800)) | (!sample.laser ? 0 : 0x1000)) | (!sample.info__crouchBlocked ? 0 : 0x400);
            idMain.stateFlags = num6;
            if (flags != num6)
            {
                idMain.Signal_State_FlagsChanged(false);
            }
        }
        this.crouch_was_blocked = sample.info__crouchBlocked;
        if (sample.inventory)
        {
            RPOS.Toggle();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            RPOS.Hide();
        }
    }

    [RPC]
    private void ReadClientMove(Vector3 origin, int encoded, ushort stateFlags, float timeAfterServerReceived, NetworkMessageInfo info)
    {
        Angle2 eyesAngles = new Angle2 {
            encoded = encoded
        };
        this.UpdateStateNew(origin, eyesAngles, stateFlags, info.timestamp);
    }

    [Obsolete("Make sure the only thing calling this is Update!")]
    protected void SendToServer()
    {
        Character idMain = base.idMain;
        int num = idMain.stateFlags.flags & -24577;
        if (Time.timeScale == 1f)
        {
            if (this.thatsRightPatWeDontNeedComments.HasValue)
            {
                num |= !this.thatsRightPatWeDontNeedComments.Value ? 0x4000 : 0x2000;
                this.thatsRightPatWeDontNeedComments = new bool?(!this.thatsRightPatWeDontNeedComments.Value);
            }
            else
            {
                this.thatsRightPatWeDontNeedComments = new bool?((base.playerClient.userName.GetHashCode() & 1) == 1);
            }
        }
        else
        {
            num |= 0x6000;
        }
        object[] args = new object[] { idMain.origin, idMain.eyesAngles.encoded, (ushort) num };
        base.networkView.RPC("GetClientMove", NetworkPlayer.server, args);
    }

    private void SetLocalOnlyComponentsEnabled(bool enable)
    {
        CCMotor component = base.GetComponent<CCMotor>();
        if (component != null)
        {
            component.enabled = enable;
            CharacterController collider = base.collider as CharacterController;
            if (collider != null)
            {
                collider.enabled = enable;
            }
        }
        CameraMount componentInChildren = base.GetComponentInChildren<CameraMount>();
        if (componentInChildren != null)
        {
            componentInChildren.open = enable;
            HeadBob bob = componentInChildren.GetComponent<HeadBob>();
            if (bob != null)
            {
                bob.enabled = enable;
            }
            LazyCam cam = componentInChildren.GetComponent<LazyCam>();
            if (cam != null)
            {
                cam.enabled = enable;
            }
        }
        LocalDamageDisplay display = base.GetComponent<LocalDamageDisplay>();
        if (display != null)
        {
            display.enabled = enable;
        }
    }

    protected void SprintingStarted()
    {
        this.wasSprinting = true;
    }

    protected void SprintingStopped()
    {
        this.wasSprinting = false;
    }

    protected void uLink_OnNetworkInstantiate(NetworkMessageInfo info)
    {
        this.instantiatedPlayerClient = base.playerClient;
        if (this.instantiatedPlayerClient != null)
        {
            base.name = string.Format("{0}{1}", this.instantiatedPlayerClient.name, info.networkView.localPrefab);
        }
        try
        {
            this.deathTransfer = base.AddAddon<DeathTransfer>();
        }
        catch (Exception exception)
        {
            Debug.LogException(exception, this);
        }
        if (base.networkView.isMine)
        {
            CameraMount.ClearTemporaryCameraMount();
            Object.Destroy(base.GetComponent<ApplyCrouch>());
            base.CreateCCMotor();
            base.CreateOverlay();
        }
        else
        {
            if (base.CreateInterpolator())
            {
                base.interpolator.running = true;
            }
            Object.Destroy(base.GetComponent<LocalDamageDisplay>());
        }
    }

    protected void Update()
    {
        if (!base.dead)
        {
            try
            {
                this.UpdateInput();
            }
            finally
            {
                if ((!this.onceClock || (this.clock.ElapsedSeconds > NetCull.sendInterval)) && !base.dead)
                {
                    this.onceClock = true;
                    this.SendToServer();
                    this.clock = SystemTimestamp.Restart;
                }
            }
        }
    }

    protected void UpdateInput()
    {
        bool flag;
        bool flag2;
        InventoryHolder inventoryHolder = this.inventoryHolder;
        PlayerClient.InputFunction(base.gameObject);
        if (inventoryHolder != null)
        {
            ItemModFlags modFlags = inventoryHolder.modFlags;
            flag = (modFlags & ItemModFlags.Lamp) == ItemModFlags.Other;
            flag2 = (modFlags & ItemModFlags.Laser) == ItemModFlags.Other;
        }
        else
        {
            flag = flag2 = true;
        }
        InputSample sample = InputSample.Poll(flag, flag2);
        sample.info__crouchBlocked = this.crouch_was_blocked;
        bool flag3 = base.GetLocal<FallDamage>().GetLegInjury() > 0f;
        if (flag3)
        {
            sample.crouch = true;
            sample.jump = false;
        }
        if (((sample.walk <= 0f) || (Mathf.Abs(sample.strafe) >= 0.05f)) || ((sample.attack2 || this._inventory.isCrafting) || flag3))
        {
            sample.sprint = false;
        }
        float num = 1f;
        if (this._inventory.isCrafting)
        {
            num *= 0.5f;
        }
        if (flag3)
        {
            num *= 0.5f;
        }
        InputSample.MovementScale = num;
        if (inventoryHolder != null)
        {
            object item = inventoryHolder.InvokeInputItemPreFrame(ref sample);
            this.ProcessInput(ref sample);
            inventoryHolder.InvokeInputItemPostFrame(item, ref sample);
        }
        else
        {
            this.ProcessInput(ref sample);
        }
        this.CheckBeltUsage();
        if (this.wasSprinting && !sample.sprint)
        {
            this.SprintingStopped();
        }
        else if (!this.wasSprinting && sample.sprint)
        {
            this.SprintingStarted();
        }
    }

    private void UpdateStateNew(Vector3 origin, Angle2 eyesAngles, ushort stateFlags, double timestamp)
    {
        Character idMain = base.idMain;
        if (this.firstState)
        {
            this.firstState = false;
            idMain.origin = origin;
            idMain.eyesAngles = eyesAngles;
            idMain.stateFlags.flags = stateFlags;
        }
        else if (base.networkView.isMine)
        {
            idMain.origin = origin;
            idMain.eyesAngles = eyesAngles;
            idMain.stateFlags.flags = stateFlags;
            CCMotor ccmotor = base.ccmotor;
            if (ccmotor != null)
            {
                ccmotor.Teleport(origin);
            }
        }
        else
        {
            CharacterInterpolatorBase base2 = base.interpolator;
            if (base2 != null)
            {
                IStateInterpolator<CharacterStateInterpolatorData> interpolator = base2 as IStateInterpolator<CharacterStateInterpolatorData>;
                if (interpolator != null)
                {
                    CharacterStateInterpolatorData data;
                    data.origin = origin;
                    data.state.flags = stateFlags;
                    data.eyesAngles = eyesAngles;
                    interpolator.SetGoals(ref data, ref timestamp);
                }
                else
                {
                    idMain.stateFlags.flags = stateFlags;
                    base2.SetGoals(origin, eyesAngles.quat, timestamp);
                }
            }
        }
    }

    private PlayerInventory _inventory
    {
        get
        {
            return (this.inventory as PlayerInventory);
        }
    }

    public bool bleeding
    {
        get
        {
            return ((this.clientVitalsSync == null) ? base.stateFlags.bleeding : this.clientVitalsSync.bleeding);
        }
    }

    protected HumanControlConfiguration controlConfig
    {
        get
        {
            if (!this._didControlConfigTest)
            {
                this._controlConfig = base.GetTrait<HumanControlConfiguration>();
                this._didControlConfigTest = true;
            }
            return this._controlConfig;
        }
    }

    private Transform headBone
    {
        get
        {
            if (this._headBone == null)
            {
                this._headBone = base.transform.FindChild("RustPlayer_Pelvis/RustPlayer_Spine/RustPlayer_Spine1/RustPlayer_Spine2/RustPlayer_Spine4/RustPlayer_Neck1/RustPlayer_Head1");
                if (this._headBone == null)
                {
                    this._headBone = base.transform.FindChild("RustPlayer_Pelvis/RustPlayer_Spine/RustPlayer_Spine1/RustPlayer_Spine2/RustPlayer_Spine4/RustPlayer_Neck1/RustPlayer_Head1");
                    if (this._headBone == null)
                    {
                        Character idMain = base.idMain;
                        if ((idMain != null) && (idMain.eyesTransformReadOnly != null))
                        {
                            this._headBone = idMain.eyesTransformReadOnly;
                        }
                        else
                        {
                            this._headBone = base.transform;
                        }
                    }
                }
            }
            return this._headBone;
        }
    }

    public Inventory inventory
    {
        get
        {
            if (!this.__inventory.cached)
            {
                this.__inventory = base.GetLocal<Inventory>();
            }
            return this.__inventory.value;
        }
    }

    public InventoryHolder inventoryHolder
    {
        get
        {
            Inventory inventory = this.inventory;
            return ((inventory == null) ? null : inventory.inventoryHolder);
        }
    }

    RagdollTransferInfo RagdollTransferInfoProvider.RagdollTransferInfo
    {
        get
        {
            return "RustPlayer_Pelvis/RustPlayer_Spine/RustPlayer_Spine1/RustPlayer_Spine2/RustPlayer_Spine4/RustPlayer_Neck1/RustPlayer_Head1";
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct InputSample
    {
        public const string kButtonAim = "Aim";
        public const string kRawYaw = "Mouse X";
        public const string kRawPitch = "Mouse Y";
        public const string kYaw = "Yaw";
        public const string kPitch = "Pitch";
        public const string kButtonUse = "WorldUse";
        public static float MovementScale;
        public float walk;
        public float strafe;
        public float yaw;
        public float pitch;
        public bool jump;
        public bool crouch;
        public bool sprint;
        public bool aim;
        public bool attack;
        public bool attack2;
        public bool reload;
        public bool inventory;
        public bool lamp;
        public bool laser;
        public bool info__crouchBlocked;
        private static float yawSensitivityJoy;
        private static float pitchSensitivityJoy;
        private static readonly string[] kUseButtons;
        static InputSample()
        {
            MovementScale = 1f;
            yawSensitivityJoy = 30f;
            pitchSensitivityJoy = 30f;
            kUseButtons = new string[] { "UseItem1", "UseItem2", "UseItem3", "UseItem4", "UseItem5", "UseItem6" };
        }

        public bool is_sprinting
        {
            get
            {
                return ((this.sprint && !this.aim) && !(this.walk == 0f));
            }
        }
        public static HumanController.InputSample Poll()
        {
            return Poll(false, false);
        }

        public static HumanController.InputSample Poll(bool noLamp, bool noLaser)
        {
            HumanController.InputSample sample;
            if (ConsoleWindow.IsVisible())
            {
                return new HumanController.InputSample();
            }
            if (MainMenu.IsVisible())
            {
                return new HumanController.InputSample();
            }
            if (ChatUI.IsVisible())
            {
                return new HumanController.InputSample();
            }
            if (LockEntry.IsVisible())
            {
                return new HumanController.InputSample();
            }
            if (!LockCursorManager.IsLocked(true))
            {
                sample = new HumanController.InputSample();
                if (!UIUnityEvents.shouldBlockButtonInput)
                {
                    sample.inventory = GameInput.GetButton("Inventory").IsPressed();
                }
                sample.lamp = saved.lamp;
                sample.laser = saved.laser;
            }
            else
            {
                float deltaTime = Time.deltaTime;
                sample.info__crouchBlocked = false;
                sample.walk = 0f;
                if (GameInput.GetButton("Up").IsDown())
                {
                    sample.walk++;
                }
                if (GameInput.GetButton("Down").IsDown())
                {
                    sample.walk--;
                }
                sample.strafe = 0f;
                if (GameInput.GetButton("Right").IsDown())
                {
                    sample.strafe++;
                }
                if (GameInput.GetButton("Left").IsDown())
                {
                    sample.strafe--;
                }
                sample.yaw = GameInput.mouseDeltaX + ((yawSensitivityJoy * Input.GetAxis("Yaw")) * deltaTime);
                sample.pitch = GameInput.mouseDeltaY + ((pitchSensitivityJoy * Input.GetAxis("Pitch")) * deltaTime);
                if (input.flipy)
                {
                    sample.pitch *= -1f;
                }
                sample.jump = GameInput.GetButton("Jump").IsDown();
                sample.crouch = GameInput.GetButton("Duck").IsDown();
                sample.sprint = GameInput.GetButton("Sprint").IsDown();
                sample.aim = false;
                sample.attack = GameInput.GetButton("Fire").IsDown();
                sample.attack2 = GameInput.GetButton("AltFire").IsDown();
                sample.reload = GameInput.GetButton("Reload").IsDown();
                sample.inventory = GameInput.GetButton("Inventory").IsPressed();
                sample.lamp = !noLamp ? saved.GetLamp(GameInput.GetButton("Flashlight").IsPressed()) : saved.lamp;
                sample.laser = !noLaser ? saved.GetLaser(GameInput.GetButton("Laser").IsPressed()) : saved.laser;
            }
            if (GameInput.GetButton("Chat").IsPressed())
            {
                ChatUI.Open();
            }
            return sample;
        }

        public static int PollItemButtons()
        {
            if (LockCursorManager.keySubsetEnabled)
            {
                for (int i = 0; i < kUseButtons.Length; i++)
                {
                    if (Input.GetButtonDown(kUseButtons[i]))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }
        private static class saved
        {
            public static bool lamp = (PlayerPrefs.GetInt("LAMP", 1) != 0);
            public static bool laser = (PlayerPrefs.GetInt("LASER", 1) != 0);

            public static bool GetLamp(bool pressed)
            {
                if (pressed)
                {
                    lamp = !lamp;
                    PlayerPrefs.SetInt("LAMP", !lamp ? 0 : 1);
                }
                return lamp;
            }

            public static bool GetLaser(bool pressed)
            {
                if (pressed)
                {
                    laser = !laser;
                    PlayerPrefs.SetInt("LASER", !laser ? 0 : 1);
                }
                return laser;
            }
        }
    }
}

