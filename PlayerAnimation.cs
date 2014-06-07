using Facepunch.Movement;
using Facepunch.Precision;
using System;
using UnityEngine;

public class PlayerAnimation : IDLocalCharacter
{
    [NonSerialized]
    private Socket.LocalSpace _itemAttachmentSocket;
    private bool _madeItemAttachment;
    private float angle;
    private double anglePrecise;
    [PrefetchComponent]
    public Animation animation;
    [NonSerialized]
    private CharacterAnimationTrait animationTrait;
    private Weights baseDecay;
    private Transform characterTransform;
    private Configuration configuration;
    private bool decaying;
    private int group_armed = 1;
    private int group_unarmed;
    [NonSerialized]
    private string idealGroupName;
    [PrefetchComponent]
    public InventoryHolder itemHolder;
    private double lastAngleSpeedPrecise;
    private Weights lastHeadingWeights;
    private Vector3 lastPos;
    private Vector3G lastPosPrecise;
    private float lastUnitScale;
    private float lastVelocityCalc;
    private Vector3 localVelocity;
    private Vector3G localVelocityPrecise;
    public const double MIN_ANIM_SPEED = 0.05;
    private Sampler movement;
    private Vector2 movementNormal;
    private Vector2G movementNormalPrecise;
    private float positionTime;
    private float speed;
    private double speedPrecise;
    private Vector4 times;
    [NonSerialized]
    private int usingGroupIndex;
    [NonSerialized]
    private string usingGroupName;
    private bool wasAirborne;

    private void Awake()
    {
        if ((this.animation == null) && ((this.animation = base.animation) == null))
        {
            Debug.LogError("There must be a animation component defined!", this);
        }
        this.animationTrait = base.GetTrait<CharacterAnimationTrait>();
        if (!this.animationTrait.movementAnimationSetup.CreateSampler(this.animation, out this.movement))
        {
            Debug.LogError("Failed to make movement sampler", this);
        }
    }

    private void CalculateVelocity()
    {
        Vector3G vectorg2;
        double num = Time.time - this.lastVelocityCalc;
        Character idMain = base.idMain;
        Vector3 v = (idMain == null) ? base.transform.position : idMain.origin;
        Vector3G vectorg = new Vector3G(ref v);
        double num2 = 1.0 / num;
        vectorg2.x = num2 * (vectorg.x - this.lastPosPrecise.x);
        vectorg2.y = num2 * (vectorg.y - this.lastPosPrecise.y);
        vectorg2.z = num2 * (vectorg.z - this.lastPosPrecise.z);
        Matrix4x4G b = new Matrix4x4G(base.transform.worldToLocalMatrix);
        Matrix4x4G.Mult3x3(ref vectorg2, ref b, out this.localVelocityPrecise);
        this.lastVelocityCalc = Time.time;
        this.speedPrecise = Math.Sqrt((this.localVelocityPrecise.x * this.localVelocityPrecise.x) + (this.localVelocityPrecise.z * this.localVelocityPrecise.z));
        if (this.speedPrecise < this.movement.configuration.minMoveSpeed)
        {
            float num3;
            this.speedPrecise = 0.0;
            this.movementNormalPrecise.x = 0.0;
            this.movementNormalPrecise.y = 0.0;
            if ((this.lastAngleSpeedPrecise > 0.0) && ((num3 = this.movement.configuration.maxTimeBetweenTurns) > 0f))
            {
                this.lastAngleSpeedPrecise -= Time.deltaTime / num3;
            }
        }
        else
        {
            double num4 = 1.0 / this.speedPrecise;
            this.movementNormalPrecise.x = this.localVelocity.x * num4;
            this.movementNormalPrecise.y = this.localVelocity.z * num4;
            double anglePrecise = this.anglePrecise;
            this.anglePrecise = (Math.Atan2(this.movementNormalPrecise.x, this.movementNormalPrecise.y) / 3.1415926535897931) * 180.0;
            float maxTurnSpeed = this.movement.configuration.maxTurnSpeed;
            if (((maxTurnSpeed > 0f) && (this.anglePrecise != anglePrecise)) && (this.lastAngleSpeedPrecise >= 0.05))
            {
                double maxDelta = Time.deltaTime * maxTurnSpeed;
                if (Precise.MoveTowardsAngle(ref anglePrecise, ref this.anglePrecise, ref maxDelta, out this.anglePrecise))
                {
                    double a = (this.anglePrecise / 180.0) * 3.1415926535897931;
                    this.movementNormalPrecise.x = Math.Sin(a);
                    this.movementNormalPrecise.y = Math.Cos(a);
                }
            }
            this.lastAngleSpeedPrecise = this.speedPrecise;
        }
        this.lastPosPrecise = vectorg;
        this.lastPos = v;
        this.movementNormal = this.movementNormalPrecise.f;
        this.speed = (float) this.speedPrecise;
        this.angle = (float) this.anglePrecise;
        this.localVelocity = this.localVelocityPrecise.f;
    }

    private void OnDestroy()
    {
        this.movement = null;
    }

    private void OnDrawGizmosSelected()
    {
        if (this._itemAttachmentSocket != null)
        {
            this.itemAttachment.DrawGizmos("itemAttachment");
        }
        else
        {
            Socket.ConfigBodyPart socket = base.GetTrait<CharacterItemAttachmentTrait>().socket;
            if (socket != null)
            {
                try
                {
                    if (socket.Extract(ref EditorHelper.tempSocketForGizmos, base.GetComponentInChildren<HitBoxSystem>()))
                    {
                        EditorHelper.tempSocketForGizmos.DrawGizmos("itemAttachment");
                    }
                }
                finally
                {
                    if (EditorHelper.tempSocketForGizmos != null)
                    {
                        EditorHelper.tempSocketForGizmos.parent = null;
                    }
                }
            }
        }
    }

    public bool PlayAnimation(GroupEvent GroupEvent)
    {
        return this.PlayAnimation(GroupEvent, 1f, 0f);
    }

    public bool PlayAnimation(GroupEvent GroupEvent, float animationSpeed)
    {
        return this.PlayAnimation(GroupEvent, animationSpeed, 0f);
    }

    public bool PlayAnimation(GroupEvent GroupEvent, float animationSpeed, float animationTime)
    {
        AnimationState state;
        if (this.movement == null)
        {
            Debug.Log("no Movement");
            return false;
        }
        try
        {
            if (!this.movement.GetGroupEvent(GroupEvent, out state))
            {
                return false;
            }
        }
        catch (NotImplementedException exception)
        {
            Debug.LogException(exception, this);
            return false;
        }
        if (animationTime < 0f)
        {
            state.time = -animationTime;
        }
        else
        {
            state.normalizedTime = animationTime;
        }
        if (!this.animation.Play(state.name, PlayMode.StopSameLayer))
        {
            return false;
        }
        if (state.speed != animationSpeed)
        {
            state.speed = animationSpeed;
        }
        return true;
    }

    [ContextMenu("Rebind Item Attachment")]
    private void RebindItemAttachment()
    {
        if (this._itemAttachmentSocket != null)
        {
            this._itemAttachmentSocket.eulerRotate = base.GetTrait<CharacterItemAttachmentTrait>().socket.eulerRotate;
            this._itemAttachmentSocket.offset = base.GetTrait<CharacterItemAttachmentTrait>().socket.offset;
        }
    }

    private void Start()
    {
        Character idMain = base.idMain;
        this.lastPos = (idMain == null) ? base.transform.position : idMain.origin;
        this.lastPosPrecise.f = this.lastPos;
    }

    private void Update()
    {
        CharacterStateFlags stateFlags;
        Weights lastHeadingWeights;
        State walk;
        float lastUnitScale;
        double deltaTime;
        this.CalculateVelocity();
        Character idMain = base.idMain;
        bool flag = (bool) idMain;
        if (flag)
        {
            stateFlags = idMain.stateFlags;
        }
        else
        {
            stateFlags = new CharacterStateFlags();
        }
        bool isJumping = !stateFlags.grounded;
        bool focus = stateFlags.focus;
        bool crouch = stateFlags.crouch;
        lastHeadingWeights.idle = 0f;
        if (this.movementNormal.x > 0f)
        {
            lastHeadingWeights.east = this.movementNormal.x;
            lastHeadingWeights.west = 0f;
        }
        else if (this.movementNormal.x < 0f)
        {
            lastHeadingWeights.east = 0f;
            lastHeadingWeights.west = -this.movementNormal.x;
        }
        else
        {
            lastHeadingWeights.east = lastHeadingWeights.west = 0f;
        }
        if (this.movementNormal.y > 0f)
        {
            lastHeadingWeights.north = this.movementNormal.y;
            lastHeadingWeights.south = 0f;
        }
        else if (this.movementNormal.y < 0f)
        {
            lastHeadingWeights.north = 0f;
            lastHeadingWeights.south = -this.movementNormal.y;
        }
        else
        {
            lastHeadingWeights.north = lastHeadingWeights.south = 0f;
        }
        if ((this.movementNormal.y == 0f) && (this.movementNormal.x == 0f))
        {
            lastHeadingWeights = this.lastHeadingWeights;
        }
        lastHeadingWeights.idle = 0f;
        this.lastHeadingWeights = lastHeadingWeights;
        if (isJumping)
        {
            walk = State.Walk;
        }
        else if (crouch)
        {
            walk = State.Crouch;
        }
        else if (stateFlags.sprint && (this.speedPrecise >= this.movement.configuration.runSpeed))
        {
            walk = State.Run;
        }
        else
        {
            walk = State.Walk;
        }
        string animationGroupName = this.itemHolder.animationGroupName;
        if (this.idealGroupName != animationGroupName)
        {
            this.idealGroupName = animationGroupName;
            if (animationGroupName != null)
            {
                animationGroupName = animationGroupName;
            }
            else
            {
                animationGroupName = this.animationTrait.defaultGroupName;
            }
            int? nullable = this.movement.configuration.GroupIndex(animationGroupName);
            if (!nullable.HasValue)
            {
                Debug.LogWarning("Could not find group name " + this.idealGroupName);
                this.usingGroupName = this.animationTrait.defaultGroupName;
                int? nullable2 = this.movement.configuration.GroupIndex(this.usingGroupName);
                this.usingGroupIndex = !nullable2.HasValue ? 0 : nullable2.Value;
            }
            else
            {
                this.usingGroupName = this.idealGroupName;
                this.usingGroupIndex = nullable.Value;
            }
        }
        int usingGroupIndex = this.usingGroupIndex;
        if (!stateFlags.slipping)
        {
            deltaTime = Time.deltaTime;
            this.movement.state = walk;
            this.movement.group = usingGroupIndex;
            lastUnitScale = this.movement.UpdateWeights(Time.deltaTime, isJumping, !flag || stateFlags.movement);
        }
        else
        {
            deltaTime = -Time.deltaTime;
            lastUnitScale = this.lastUnitScale;
        }
        this.wasAirborne = isJumping;
        this.lastUnitScale = lastUnitScale;
        if (!double.IsNaN(this.speedPrecise) && !double.IsInfinity(this.speedPrecise))
        {
            float positionTime = this.positionTime;
            this.positionTime = (float) ((this.positionTime + Math.Abs((double) ((lastUnitScale * this.speedPrecise) * deltaTime))) % 1.0);
            if (this.positionTime < 0f)
            {
                this.positionTime++;
            }
            else if (float.IsNaN(this.positionTime) || float.IsInfinity(this.positionTime))
            {
                this.positionTime = positionTime;
            }
            this.movement.configuration.OffsetTime(this.positionTime, out this.times);
        }
        float angle = !flag ? -base.transform.eulerAngles.x : idMain.eyesPitch;
        this.movement.SetWeights(this.animation, ref lastHeadingWeights, ref this.times, angle);
    }

    public Socket.LocalSpace itemAttachment
    {
        get
        {
            if (!this._madeItemAttachment && (base.idMain != null))
            {
                Socket.ConfigBodyPart socket = base.GetTrait<CharacterItemAttachmentTrait>().socket;
                if (socket == null)
                {
                    return null;
                }
                this._madeItemAttachment = socket.Extract(ref this._itemAttachmentSocket, base.idMain.hitBoxSystem);
            }
            return this._itemAttachmentSocket;
        }
    }

    private static class EditorHelper
    {
        public static Socket.LocalSpace tempSocketForGizmos;
    }
}

