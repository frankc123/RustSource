using Facepunch.Geometry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

[AddComponentMenu("ID/Local/CCMotor")]
public sealed class CCMotor : IDRemote
{
    private bool _grounded;
    private Vector3 _groundNormal;
    private bool _installed;
    private Vector3 _lastGroundNormal;
    [SerializeField]
    private CCMotorSettings _settings;
    internal bool canControl;
    private CCTotemPole cc;
    private static bool ccmotor_debug;
    private YawAngle currentYaw;
    public InputFrame input;
    public JumpingContext jumping = new JumpingContext(Jumping.init);
    private JumpBaseVerticalSpeedArgs jumpVerticalSpeedCalculator;
    private const float kHitEpsilon = 0.001f;
    private const float kJumpButtonDelaySeconds = 0.2f;
    private const float kResetButtonDownTime = -100f;
    private const float kYEpsilon = 0.001f;
    private const float kYMaxNotGrounded = 0.01f;
    [NonSerialized]
    public CCTotem.PositionPlacement? LastPositionPlacement;
    public float minTimeBetweenJumps;
    public MovementContext movement = new MovementContext(Movement.init);
    public MovingPlatformContext movingPlatform = new MovingPlatformContext(MovingPlatform.init);
    private YawAngle previousYaw;
    internal bool sendExternalVelocityMessage;
    internal bool sendFallMessage;
    internal bool sendJumpFailureMessage;
    internal bool sendJumpMessage;
    internal bool sendLandMessage;
    public Sliding sliding;
    public StepMode stepMode;
    private StringBuilder stringBuilder;
    internal Transform tr;

    private void ApplyGravityAndJumping(float deltaTime, ref Vector3 velocity, ref Vector3 acceleration, out bool simulate)
    {
        float time = Time.time;
        if (!this.input.jump || !this.canControl)
        {
            this.jumping.holdingJumpButton = false;
            this.jumping.lastButtonDownTime = -100f;
        }
        if ((this.input.jump && (this.jumping.lastButtonDownTime < 0f)) && this.canControl)
        {
            this.jumping.lastButtonDownTime = time;
        }
        if (this._grounded)
        {
            if (velocity.y >= 0f)
            {
                velocity.y = -this.movement.setup.gravity * deltaTime;
            }
            else
            {
                velocity.y -= this.movement.setup.gravity * deltaTime;
            }
            if (((this.jumping.setup.enable && this.canControl) && ((time - this.jumping.lastButtonDownTime) < 0.2f)) && ((this.minTimeBetweenJumps <= 0f) || ((time - this.jumping.lastLandTime) >= this.minTimeBetweenJumps)))
            {
                if ((this.minTimeBetweenJumps > 0f) && ((time - this.jumping.lastLandTime) < this.minTimeBetweenJumps))
                {
                    if (this.sendJumpFailureMessage)
                    {
                        this.RouteMessage("OnJumpFailed", SendMessageOptions.DontRequireReceiver);
                    }
                }
                else
                {
                    this._grounded = false;
                    this.jumping.jumping = true;
                    this.jumping.lastStartTime = time;
                    this.jumping.lastButtonDownTime = -100f;
                    this.jumping.holdingJumpButton = true;
                    this.jumping.jumpDir = Vector3.Slerp(Vector3.up, this._groundNormal, !this.tooSteep ? this.jumping.setup.perpAmount : this.jumping.setup.steepPerpAmount);
                    float num2 = this.jumpVerticalSpeedCalculator.CalculateVerticalSpeed(ref this.jumping.setup, ref this.movement.setup);
                    velocity.x += this.jumping.jumpDir.x * num2;
                    velocity.y = this.jumping.jumpDir.y * num2;
                    velocity.z += this.jumping.jumpDir.z * num2;
                    if (this.movingPlatform.setup.enable && ((this.movingPlatform.setup.movementTransfer == JumpMovementTransfer.InitTransfer) || (this.movingPlatform.setup.movementTransfer == JumpMovementTransfer.PermaTransfer)))
                    {
                        this.movement.frameVelocity = this.movingPlatform.platformVelocity;
                        velocity.x += this.movingPlatform.platformVelocity.x;
                        velocity.y += this.movingPlatform.platformVelocity.y;
                        velocity.z += this.movingPlatform.platformVelocity.z;
                    }
                    this.jumping.startedJumping = true;
                    if (this.sendJumpMessage)
                    {
                        this.RouteMessage("OnJump", SendMessageOptions.DontRequireReceiver);
                    }
                }
            }
            else
            {
                this.jumping.holdingJumpButton = false;
            }
            simulate = false;
        }
        else
        {
            Vector3 vector;
            acceleration.y = -this.movement.setup.gravity;
            acceleration.z = 0f;
            acceleration.x = 0f;
            if ((this.jumping.jumping && this.jumping.holdingJumpButton) && (time < (this.jumping.lastStartTime + (this.jumping.setup.extraHeight / this.jumpVerticalSpeedCalculator.CalculateVerticalSpeed(ref this.jumping.setup, ref this.movement.setup)))))
            {
                acceleration.x += this.jumping.jumpDir.x * this.movement.setup.gravity;
                acceleration.y += this.jumping.jumpDir.y * this.movement.setup.gravity;
                acceleration.z += this.jumping.jumpDir.z * this.movement.setup.gravity;
            }
            vector.x = acceleration.x * deltaTime;
            vector.y = acceleration.y * deltaTime;
            vector.z = acceleration.z * deltaTime;
            velocity.y = this.movement.velocity.y + vector.y;
            if (this.movement.setup.inputAirVelocityRatio == 1f)
            {
                velocity.x += vector.x;
                velocity.z += vector.z;
            }
            else if (this.movement.setup.inputAirVelocityRatio == 0f)
            {
                velocity.x = this.movement.velocity.x + vector.x;
                velocity.z = this.movement.velocity.z + vector.z;
            }
            else
            {
                float num3 = 1f - this.movement.setup.inputAirVelocityRatio;
                velocity.x = ((velocity.x * this.movement.setup.inputAirVelocityRatio) + (this.movement.velocity.x * num3)) + vector.x;
                velocity.z = ((velocity.z * this.movement.setup.inputAirVelocityRatio) + (this.movement.velocity.z * num3)) + vector.z;
            }
            if (-velocity.y > this.movement.setup.maxFallSpeed)
            {
                velocity.y = -this.movement.setup.maxFallSpeed;
            }
            if (this.movement.setup.maxAirHorizontalSpeed > 0f)
            {
                float f = (velocity.x * velocity.x) + (velocity.z * velocity.z);
                if (f > (this.movement.setup.maxAirHorizontalSpeed * this.movement.setup.maxAirHorizontalSpeed))
                {
                    float num5 = this.movement.setup.maxAirHorizontalSpeed / Mathf.Sqrt(f);
                    velocity.x *= num5;
                    velocity.z *= num5;
                }
            }
            simulate = true;
        }
    }

    private void ApplyHorizontalPushVelocity(ref Vector3 velocity)
    {
        Capsule capsule;
        CCTotemPole cc = this.cc;
        if (((cc != null) && cc.Exists) && this.cc.totemicObject.CCDesc.collider.GetGeometricShapeWorld(out capsule))
        {
            Sphere sphere = (Sphere) capsule;
            Vector3 vector = velocity;
            Vector vector2 = new Vector();
            bool flag = false;
            foreach (Collider collider in Physics.OverlapSphere(this.cc.totemicObject.CCDesc.worldCenter, this.cc.totemicObject.CCDesc.effectiveSkinnedHeight, 0x140000))
            {
                CCPusher component = collider.GetComponent<CCPusher>();
                if (component != null)
                {
                    Vector3 vector3 = new Vector3();
                    if (component.Push(sphere.Transform(collider.WorldToCollider()), ref vector3))
                    {
                        flag = true;
                        vector2 += collider.ColliderToWorld() * vector3;
                    }
                }
            }
            if (flag)
            {
                vector2.y = 0f;
                velocity.x += vector2.x;
                velocity.z += vector2.z;
            }
        }
    }

    private void ApplyInputVelocityChange(float deltaTime, ref Vector3 velocity, ref Vector3 acceleration)
    {
        Vector3 vector2;
        Vector3 vector = !this.canControl ? new Vector3() : this.input.moveDirection;
        if (this._grounded && this.tooSteep)
        {
            vector2.y = 0f;
            float f = (this._groundNormal.x * this._groundNormal.x) + (this._groundNormal.z * this._groundNormal.z);
            if (f == 1f)
            {
                vector2.x = this._groundNormal.x;
                vector2.z = this._groundNormal.z;
            }
            else
            {
                float num2 = Mathf.Sqrt(f);
                vector2.x = this._groundNormal.x / num2;
                vector2.z = this._groundNormal.z / num2;
            }
            Vector3 vector3 = Vector3.Project(vector, vector2);
            vector2.x += (vector3.x * this.sliding.speedControl) + ((vector.x - vector3.x) * this.sliding.sidewaysControl);
            vector2.z += (vector3.z * this.sliding.speedControl) + ((vector.z - vector3.z) * this.sliding.sidewaysControl);
            vector2.y = (vector3.y * this.sliding.speedControl) + ((vector.y - vector3.y) * this.sliding.sidewaysControl);
            vector2.x *= this.sliding.slidingSpeed;
            vector2.y *= this.sliding.slidingSpeed;
            vector2.z *= this.sliding.slidingSpeed;
        }
        else
        {
            this.DesiredHorizontalVelocity(ref vector, out vector2);
        }
        if (this.movingPlatform.setup.enable && (this.movingPlatform.setup.movementTransfer == JumpMovementTransfer.PermaTransfer))
        {
            vector2.x += this.movement.frameVelocity.x;
            vector2.z += this.movement.frameVelocity.z;
            vector2.y = 0f;
        }
        if (this._grounded)
        {
            acceleration.x = 0f;
            acceleration.y = 0f;
            acceleration.z = 0f;
            float num3 = ((vector2.x * vector2.x) + (vector2.y * vector2.y)) + (vector2.z * vector2.z);
            if (num3 != 0f)
            {
                vector2 = Vector3.Cross(Vector3.Cross(Vector3.up, vector2), this._groundNormal);
                float num4 = ((vector2.x * vector2.x) + (vector2.y * vector2.y)) + (vector2.z * vector2.z);
                if (num4 != num3)
                {
                    float num5 = Mathf.Sqrt(num3);
                    if (num4 == 1f)
                    {
                        vector2.x *= num5;
                        vector2.y *= num5;
                        vector2.z *= num5;
                    }
                    else
                    {
                        float num6 = num5 / Mathf.Sqrt(num4);
                        vector2.x *= num6;
                        vector2.y *= num6;
                        vector2.z *= num6;
                    }
                }
            }
        }
        else
        {
            acceleration.x = 0f;
            acceleration.y = 0f;
            acceleration.z = 0f;
            velocity.y = 0f;
        }
        if (this._grounded || this.canControl)
        {
            Vector3 vector4;
            float num7 = (!this._grounded ? this.movement.setup.maxAirAcceleration : this.movement.setup.maxGroundAcceleration) * deltaTime;
            vector4.x = vector2.x - velocity.x;
            vector4.y = vector2.y - velocity.y;
            vector4.z = vector2.z - velocity.z;
            float num8 = ((vector4.x * vector4.x) + (vector4.y * vector4.y)) + (vector4.z * vector4.z);
            if (num8 > (num7 * num7))
            {
                float num9 = num7 / Mathf.Sqrt(num8);
                vector4.x *= num9;
                vector4.y *= num9;
                vector4.z *= num9;
            }
            velocity += vector4;
        }
        if (this._grounded && (velocity.y > 0f))
        {
            velocity.y = 0f;
        }
    }

    private CCTotem.MoveInfo ApplyMovementDelta(ref Vector3 moveDistance, float crouchDelta)
    {
        float height = this.cc.Height + crouchDelta;
        return this.cc.Move(moveDistance, height);
    }

    private void ApplyYawDelta(float yRotation)
    {
        if (yRotation != 0f)
        {
            this.currentYaw = Mathf.DeltaAngle(0f, this.currentYaw.Degrees + yRotation);
        }
    }

    private void Awake()
    {
        if (this._settings != null)
        {
            this._settings.BindSettingsTo(this);
        }
    }

    private void BindCharacter()
    {
        Character idMain = (Character) base.idMain;
        idMain.origin = this.tr.position;
        float num = Mathf.DeltaAngle(this.previousYaw.Degrees, this.currentYaw.Degrees);
        if (num != 0f)
        {
            this.previousYaw = this.currentYaw;
            idMain.eyesYaw += num;
        }
    }

    private void BindPosition(ref CCTotem.PositionPlacement placement)
    {
        this.tr.position = placement.bottom;
        this.LastPositionPlacement = new CCTotem.PositionPlacement?(placement);
    }

    private float CalculateJumpVerticalSpeed(float targetJumpHeight)
    {
        return Mathf.Sqrt((2f * targetJumpHeight) * this.movement.setup.gravity);
    }

    private void DesiredHorizontalVelocity(ref Vector3 inputMoveDirection, out Vector3 desiredVelocity)
    {
        Vector3 desiredMovementDirection = this.InverseTransformDirection(inputMoveDirection);
        float num = this.MaxSpeedInDirection(ref desiredMovementDirection);
        if (this._grounded)
        {
            num *= this.movement.setup.slopeSpeedMultiplier.Evaluate(Mathf.Asin(this.movement.velocity.normalized.y) * 57.29578f);
        }
        desiredVelocity = this.TransformDirection((Vector3) (desiredMovementDirection * num));
        if (this._grounded)
        {
            this.ApplyHorizontalPushVelocity(ref desiredVelocity);
        }
    }

    private void DoPush(Rigidbody pusher, Collider pusherCollider, Collision collisionFromPusher)
    {
    }

    private void FixedUpdate()
    {
        float deltaTime = Time.deltaTime;
        if (deltaTime != 0f)
        {
            if (this.movingPlatform.setup.enable)
            {
                if (this.movingPlatform.activePlatform != null)
                {
                    Matrix4x4 localToWorldMatrix = this.movingPlatform.activePlatform.localToWorldMatrix;
                    if (!this.movingPlatform.newPlatform)
                    {
                        Vector3 vector = localToWorldMatrix.MultiplyPoint3x4(this.movingPlatform.activeLocal.point);
                        Vector3 vector2 = this.movingPlatform.lastMatrix.MultiplyPoint3x4(this.movingPlatform.activeLocal.point);
                        this.movingPlatform.platformVelocity.x = (vector.x - vector2.x) / deltaTime;
                        this.movingPlatform.platformVelocity.y = (vector.y - vector2.y) / deltaTime;
                        this.movingPlatform.platformVelocity.z = (vector.z - vector2.z) / deltaTime;
                    }
                    else
                    {
                        this.movingPlatform.newPlatform = false;
                    }
                    this.movingPlatform.lastMatrix = localToWorldMatrix;
                }
                else
                {
                    this.movingPlatform.platformVelocity = new Vector3();
                }
            }
            if (this.stepMode == StepMode.ViaFixedUpdate)
            {
                this.StepPhysics(deltaTime);
            }
        }
    }

    public void InitializeSetup(Character character, CCTotemPole cc, CharacterCCMotorTrait trait)
    {
        this.tr = base.transform;
        this.cc = cc;
        base.idMain = character;
        this.currentYaw = this.previousYaw = 0f;
        if (trait != null)
        {
            if (trait.settings != null)
            {
                this.settings = trait.settings;
            }
            this.canControl = trait.canControl;
            this.sendLandMessage = trait.sendLandMessage;
            this.sendJumpMessage = trait.sendJumpMessage;
            this.sendJumpFailureMessage = trait.sendJumpFailureMessage;
            this.sendFallMessage = trait.sendFallMessage;
            this.sendExternalVelocityMessage = trait.sendExternalVelocityMessage;
            this.stepMode = trait.stepMode;
            this.minTimeBetweenJumps = trait.minTimeBetweenJumps;
        }
        if (!this._installed && (cc != null))
        {
            this._installed = true;
            Callbacks.InstallCallbacks(this, cc);
        }
    }

    private Vector3 InverseTransformDirection(Vector3 direction)
    {
        return this.characterYawAngle.Unrotate(direction);
    }

    private Vector3 InverseTransformPoint(Vector3 point)
    {
        return this.InverseTransformDirection(this.tr.InverseTransformPoint(point));
    }

    public float MaxSpeedInDirection(ref Vector3 desiredMovementDirection)
    {
        Vector3 vector;
        if (((desiredMovementDirection.x == 0f) && (desiredMovementDirection.y == 0f)) && (desiredMovementDirection.z == 0f))
        {
            return 0f;
        }
        if (this.movement.setup.maxSidewaysSpeed == 0f)
        {
            return 0f;
        }
        float num = ((desiredMovementDirection.z <= 0f) ? this.movement.setup.maxBackwardsSpeed : this.movement.setup.maxForwardSpeed) / this.movement.setup.maxSidewaysSpeed;
        vector.x = desiredMovementDirection.x;
        vector.y = 0f;
        vector.z = desiredMovementDirection.z / num;
        float f = (vector.x * vector.x) + (vector.z * vector.z);
        if (f != 1f)
        {
            float num3 = Mathf.Sqrt(f);
            vector.x /= num3;
            vector.z /= num3;
        }
        vector.z *= num;
        return (Mathf.Sqrt((vector.x * vector.x) + (vector.z * vector.z)) * this.movement.setup.maxSidewaysSpeed);
    }

    private void MoveFromCollision(Collision collision)
    {
        PlayerPusher component = collision.gameObject.GetComponent<PlayerPusher>();
        if (component != null)
        {
            ContactPoint[] contacts = collision.contacts;
            Vector3 zero = Vector3.zero;
            Vector3 vector2 = Vector3.zero;
            Vector3 vector3 = Vector3.zero;
            for (int i = 0; i < contacts.Length; i++)
            {
                vector3 += contacts[i].point;
                vector2 += contacts[i].normal;
            }
            vector2.Normalize();
            vector3 = (Vector3) (vector3 / ((float) contacts.Length));
            Vector3 position = this.tr.position;
            position.y = vector3.y;
            zero = (Vector3) (vector2 * (component.rigidbody.GetPointVelocity(position).magnitude * Time.deltaTime));
            Vector3 start = this.tr.position;
            Debug.DrawLine(start, start + zero, Color.yellow, 60f);
            this.ApplyMovementDelta(ref zero, 0f);
            Debug.DrawLine(start, this.tr.position, Color.green, 60f);
            this.BindCharacter();
        }
    }

    internal void OnBindCCMotorSettings()
    {
    }

    public void OnCollisionEnter(Collision collision)
    {
        this.MoveFromCollision(collision);
    }

    public void OnCollisionStay(Collision collision)
    {
        this.MoveFromCollision(collision);
    }

    private void OnDestroy()
    {
        try
        {
            base.OnDestroy();
        }
        finally
        {
            if (this._installed)
            {
                Callbacks.UninstallCallbacks(this, this.cc);
            }
            this.cc = null;
        }
    }

    private void OnHit(ref CCDesc.Hit hit)
    {
        Vector3 normal = hit.Normal;
        Vector3 moveDirection = hit.MoveDirection;
        if (((normal.y > 0f) && (normal.y > this._groundNormal.y)) && (moveDirection.y < 0f))
        {
            Vector3 vector4;
            Vector3 point = hit.Point;
            vector4.x = point.x - this.movement.lastHitPoint.x;
            vector4.y = point.y - this.movement.lastHitPoint.y;
            vector4.z = point.z - this.movement.lastHitPoint.z;
            if ((((this._lastGroundNormal.x == 0f) && (this._lastGroundNormal.y == 0f)) && (this._lastGroundNormal.z == 0f)) || ((((vector4.x * vector4.x) + (vector4.y * vector4.y)) + (vector4.z * vector4.z)) > 0.001f))
            {
                this._groundNormal = normal;
            }
            else
            {
                this._groundNormal = this._lastGroundNormal;
            }
            this.movingPlatform.hitPlatform = hit.Collider.transform;
            this.movement.hitPoint = point;
            this.movement.frameVelocity = new Vector3();
        }
    }

    public void OnPushEnter(Rigidbody pusher, Collider pusherCollider, Collision collisionFromPusher)
    {
        this.DoPush(pusher, pusherCollider, collisionFromPusher);
    }

    public void OnPushExit(Rigidbody pusher, Collider pusherCollider, Collision collisionFromPusher)
    {
        this.DoPush(pusher, pusherCollider, collisionFromPusher);
    }

    public void OnPushStay(Rigidbody pusher, Collider pusherCollider, Collision collisionFromPusher)
    {
        this.DoPush(pusher, pusherCollider, collisionFromPusher);
    }

    private void RouteMessage(string messageName)
    {
        base.idMain.SendMessage(messageName, SendMessageOptions.DontRequireReceiver);
    }

    private void RouteMessage(string messageName, SendMessageOptions sendOptions)
    {
        base.idMain.SendMessage(messageName, sendOptions);
    }

    public void Step()
    {
        this.Step(Time.deltaTime);
    }

    public void Step(float deltaTime)
    {
        if ((deltaTime > 0f) && base.enabled)
        {
            this.StepPhysics(deltaTime);
        }
    }

    private void StepPhysics(float deltaTime)
    {
        bool flag;
        Vector3 vector5;
        Vector3 vector7;
        Vector3 vector8;
        float num4;
        Vector3 vector9;
        Vector3 velocity = this.movement.velocity;
        Vector3 acceleration = this.movement.acceleration;
        this.ApplyInputVelocityChange(deltaTime, ref velocity, ref acceleration);
        this.ApplyGravityAndJumping(deltaTime, ref velocity, ref acceleration, out flag);
        if (this.movingWithPlatform)
        {
            Vector3 vector4;
            Vector3 vector3 = this.movingPlatform.activePlatform.TransformPoint(this.movingPlatform.activeLocal.point);
            vector4.x = vector3.x - this.movingPlatform.activeGlobal.point.x;
            vector4.y = vector3.y - this.movingPlatform.activeGlobal.point.y;
            vector4.z = vector3.z - this.movingPlatform.activeGlobal.point.z;
            if (((vector4.x != 0f) || (vector4.y != 0f)) || (vector4.z != 0f))
            {
                this.ApplyMovementDelta(ref vector4, 0f);
            }
            Quaternion quaternion = this.movingPlatform.activePlatform.rotation * this.movingPlatform.activeLocal.rotation;
            Quaternion quaternion2 = quaternion * Quaternion.Inverse(this.movingPlatform.activeGlobal.rotation);
            float y = quaternion2.eulerAngles.y;
            if (y != 0f)
            {
                this.ApplyYawDelta(y);
            }
        }
        vector5.x = acceleration.x * deltaTime;
        vector5.y = acceleration.y * deltaTime;
        vector5.z = acceleration.z * deltaTime;
        Vector3 position = this.tr.position;
        if (flag)
        {
            vector8.x = position.x + (deltaTime * (this.movement.velocity.x + (vector5.x / 2f)));
            vector8.y = position.y + (deltaTime * (this.movement.velocity.y + (vector5.y / 2f)));
            vector8.z = position.z + (deltaTime * (this.movement.velocity.z + (vector5.z / 2f)));
            vector7.x = vector8.x - position.x;
            vector7.y = vector8.y - position.y;
            vector7.z = vector8.z - position.z;
        }
        else
        {
            vector7.x = velocity.x * deltaTime;
            vector7.y = velocity.y * deltaTime;
            vector7.z = velocity.z * deltaTime;
            vector8.x = position.x + vector7.x;
            vector8.y = position.y + vector7.y;
            vector8.z = position.z + vector7.z;
        }
        float stepOffset = this.cc.stepOffset;
        float num3 = stepOffset * stepOffset;
        float f = (vector7.x * vector7.x) + (vector7.z * vector7.z);
        if (f > num3)
        {
            num4 = Mathf.Sqrt(f);
        }
        else
        {
            num4 = stepOffset;
        }
        if (this._grounded)
        {
            vector7.y -= num4;
        }
        this.movingPlatform.hitPlatform = null;
        this._groundNormal = new Vector3();
        float crouchDelta = this.input.crouchSpeed * deltaTime;
        CCTotem.MoveInfo info = this.ApplyMovementDelta(ref vector7, crouchDelta);
        this.movement.collisionFlags = info.CollisionFlags;
        float num7 = info.WantedHeight - info.PositionPlacement.height;
        CollisionFlags flags = info.CollisionFlags | info.WorkingCollisionFlags;
        this.movement.crouchBlocked = ((this.input.crouchSpeed > 0f) && ((flags & CollisionFlags.Above) == CollisionFlags.Above)) && (num7 > this.movement.setup.maxUnblockingHeightDifference);
        this.movement.lastHitPoint = this.movement.hitPoint;
        this._lastGroundNormal = this._groundNormal;
        if ((this.movingPlatform.setup.enable && (this.movingPlatform.activePlatform != this.movingPlatform.hitPlatform)) && (this.movingPlatform.hitPlatform != null))
        {
            this.movingPlatform.activePlatform = this.movingPlatform.hitPlatform;
            this.movingPlatform.lastMatrix = this.movingPlatform.hitPlatform.localToWorldMatrix;
            this.movingPlatform.newPlatform = true;
        }
        if (this.movement.collisionFlags != CollisionFlags.None)
        {
            Vector3 vector10;
            Vector3 vector11;
            this.movement.acceleration.x = 0f;
            this.movement.acceleration.y = 0f;
            this.movement.acceleration.z = 0f;
            vector10.x = velocity.x;
            vector10.y = 0f;
            vector10.z = velocity.z;
            vector9 = this.tr.position;
            this.movement.velocity.x = (vector9.x - position.x) / deltaTime;
            this.movement.velocity.y = (vector9.y - position.y) / deltaTime;
            this.movement.velocity.z = (vector9.z - position.z) / deltaTime;
            vector11.x = this.movement.velocity.x;
            vector11.y = 0f;
            vector11.z = this.movement.velocity.z;
            if ((vector10.x == 0f) && (vector10.z == 0f))
            {
                this.movement.velocity.x = 0f;
                this.movement.velocity.z = 0f;
            }
            else
            {
                float num8 = ((vector11.x * vector10.x) + (vector11.z * vector10.z)) / ((vector10.x * vector10.x) + (vector10.z * vector10.z));
                if (num8 <= 0f)
                {
                    this.movement.velocity.x = 0f;
                    this.movement.velocity.z = 0f;
                }
                else if (num8 >= 1f)
                {
                    this.movement.velocity.x = vector10.x;
                    this.movement.velocity.z = vector10.z;
                }
                else
                {
                    this.movement.velocity.x = vector10.x * num8;
                    this.movement.velocity.z = vector10.z * num8;
                }
            }
            if (this.movement.velocity.y < (velocity.y - 0.001f))
            {
                if (this.movement.velocity.y < 0f)
                {
                    this.movement.velocity.y = velocity.y;
                }
                else
                {
                    this.jumping.holdingJumpButton = false;
                }
            }
        }
        else
        {
            vector9 = vector8;
            this.movement.velocity = velocity;
            this.movement.acceleration = acceleration;
        }
        if (this._grounded != (this._groundNormal.y > 0.01f))
        {
            if (this._grounded)
            {
                this._grounded = false;
                if (this.movingPlatform.setup.enable && ((this.movingPlatform.setup.movementTransfer == JumpMovementTransfer.InitTransfer) || (this.movingPlatform.setup.movementTransfer == JumpMovementTransfer.PermaTransfer)))
                {
                    this.movement.frameVelocity = this.movingPlatform.platformVelocity;
                    this.movement.velocity += this.movingPlatform.platformVelocity;
                }
                if (this.sendFallMessage)
                {
                    this.RouteMessage("OnFall", SendMessageOptions.DontRequireReceiver);
                }
                vector9.y += num4;
            }
            else
            {
                this._grounded = true;
                this.jumping.jumping = false;
                if (this.jumping.startedJumping)
                {
                    this.jumping.startedJumping = false;
                    this.jumping.lastLandTime = Time.time;
                }
                this.SubtractNewPlatformVelocity();
                if (this.sendLandMessage)
                {
                    this.RouteMessage("OnLand", SendMessageOptions.DontRequireReceiver);
                }
            }
        }
        if (this.movingWithPlatform)
        {
            this.movingPlatform.activeGlobal.point.x = vector9.x;
            this.movingPlatform.activeGlobal.point.y = vector9.y + ((this.cc.center.y - (this.cc.height * 0.5f)) + this.cc.radius);
            this.movingPlatform.activeGlobal.point.z = vector9.z;
            this.movingPlatform.activeLocal.point = this.movingPlatform.activePlatform.InverseTransformPoint(this.movingPlatform.activeGlobal.point);
            this.movingPlatform.activeGlobal.rotation = this.tr.rotation;
            Quaternion introduced28 = Quaternion.Inverse(this.movingPlatform.activePlatform.rotation);
            this.movingPlatform.activeLocal.rotation = introduced28 * this.movingPlatform.activeGlobal.rotation;
        }
        this.BindCharacter();
    }

    private void SubtractNewPlatformVelocity()
    {
        if (this.movingPlatform.setup.enable && ((this.movingPlatform.setup.movementTransfer == JumpMovementTransfer.InitTransfer) || (this.movingPlatform.setup.movementTransfer == JumpMovementTransfer.PermaTransfer)))
        {
            if (this.movingPlatform.newPlatform)
            {
                base.StartCoroutine(this.SubtractNewPlatformVelocityLateRoutine(this.movingPlatform.activePlatform));
            }
            else
            {
                this.movement.velocity -= this.movingPlatform.platformVelocity;
            }
        }
    }

    [DebuggerHidden]
    private IEnumerator SubtractNewPlatformVelocityLateRoutine(Transform platform)
    {
        return new <SubtractNewPlatformVelocityLateRoutine>c__Iterator26 { platform = platform, <$>platform = platform, <>f__this = this };
    }

    public void Teleport(Vector3 origin)
    {
        if (this.cc != null)
        {
            this.cc.Teleport(origin);
        }
        else
        {
            this.tr.position = origin;
        }
    }

    private Vector3 TransformDirection(Vector3 direction)
    {
        return this.characterYawAngle.Rotate(direction);
    }

    private Vector3 TransformPoint(Vector3 point)
    {
        return this.tr.TransformPoint(this.TransformDirection(point));
    }

    private void Update()
    {
        float num;
        if ((this.stepMode == StepMode.ViaUpdate) && ((num = Time.deltaTime) != 0f))
        {
            this.StepPhysics(num);
        }
    }

    private float baseHeightVerticalSpeed
    {
        get
        {
            return this.jumpVerticalSpeedCalculator.CalculateVerticalSpeed(ref this.jumping.setup, ref this.movement.setup);
        }
    }

    public CCMotor ccmotor
    {
        get
        {
            return this;
        }
    }

    public CCTotemPole ccTotemPole
    {
        get
        {
            return this.cc;
        }
    }

    private YawAngle characterYawAngle
    {
        get
        {
            Character idMain = (Character) base.idMain;
            return (idMain.eyesYaw + Mathf.DeltaAngle(this.previousYaw.Degrees, this.currentYaw.Degrees));
        }
    }

    public Vector3 currentGroundNormal
    {
        get
        {
            return this._groundNormal;
        }
    }

    public Vector3 currentHitPoint
    {
        get
        {
            return this.movement.hitPoint;
        }
    }

    public Vector3 differentVelocity
    {
        get
        {
            return this.movement.velocity;
        }
        set
        {
            if (((this.movement.velocity.x != value.x) || (this.movement.velocity.y != value.y)) || (this.movement.velocity.z != value.z))
            {
                this.velocity = value;
            }
        }
    }

    public Vector3 direction
    {
        get
        {
            return this.input.moveDirection;
        }
    }

    public bool driveable
    {
        get
        {
            return this.canControl;
        }
        set
        {
            this.canControl = value;
        }
    }

    public Vector3? fallbackCurrentGroundNormal
    {
        get
        {
            if (this._grounded)
            {
                return new Vector3?(this._groundNormal);
            }
            return null;
        }
    }

    public Vector3? fallbackPreviousGroundNormal
    {
        get
        {
            if (((this._lastGroundNormal.x == 0f) && (this._lastGroundNormal.y == 0f)) && (this._lastGroundNormal.z == 0f))
            {
                return null;
            }
            return new Vector3?(this._lastGroundNormal);
        }
    }

    public bool isCrouchBlocked
    {
        get
        {
            return this.movement.crouchBlocked;
        }
    }

    public bool isGrounded
    {
        get
        {
            return this._grounded;
        }
    }

    public bool isJumping
    {
        get
        {
            return this.jumping.jumping;
        }
    }

    public bool isSliding
    {
        get
        {
            return ((this._grounded && this.sliding.enable) && this.tooSteep);
        }
    }

    public bool isTouchingCeiling
    {
        get
        {
            return ((this.movement.collisionFlags & CollisionFlags.Above) == CollisionFlags.Above);
        }
    }

    public bool movingWithPlatform
    {
        get
        {
            return ((this.movingPlatform.setup.enable && (this._grounded || (this.movingPlatform.setup.movementTransfer == JumpMovementTransfer.PermaLocked))) && (this.movingPlatform.activePlatform != null));
        }
    }

    public Vector3 previousGroundNormal
    {
        get
        {
            return this._lastGroundNormal;
        }
    }

    public Vector3 previousHitPoint
    {
        get
        {
            return this.movement.lastHitPoint;
        }
    }

    public CCMotorSettings settings
    {
        get
        {
            return this._settings;
        }
        set
        {
            if (value != this._settings)
            {
                this._settings = value;
                if (Application.isPlaying)
                {
                    value.BindSettingsTo(this);
                }
            }
        }
    }

    public string setupString
    {
        get
        {
            object[] args = new object[] { this.movement.setup, this.jumping.setup, this.sliding, this.movingPlatform.setup };
            return string.Format("movement={0}, jumping={1}, sliding={2}, movingPlatform={3}", args);
        }
    }

    public bool tooSteep
    {
        get
        {
            return (this._groundNormal.y <= Mathf.Cos(this.cc.slopeLimit * 0.01745329f));
        }
    }

    [Obsolete("Do not query this", true)]
    public Transform transform
    {
        get
        {
            return this.tr;
        }
    }

    public Vector3 velocity
    {
        get
        {
            return this.movement.velocity;
        }
        set
        {
            this._grounded = false;
            this.movement.velocity = value;
            this.movement.frameVelocity = new Vector3();
            if (this.sendExternalVelocityMessage)
            {
                this.RouteMessage("OnExternalVelocity");
            }
        }
    }

    [CompilerGenerated]
    private sealed class <SubtractNewPlatformVelocityLateRoutine>c__Iterator26 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal Transform <$>platform;
        internal CCMotor <>f__this;
        internal Transform platform;

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
                    this.$current = new WaitForFixedUpdate();
                    this.$PC = 1;
                    goto Label_00D3;

                case 1:
                    this.$current = new WaitForFixedUpdate();
                    this.$PC = 2;
                    goto Label_00D3;

                case 2:
                    if (!this.<>f__this._grounded || (this.platform != this.<>f__this.movingPlatform.activePlatform))
                    {
                        break;
                    }
                    this.$current = 1;
                    this.$PC = 3;
                    goto Label_00D3;

                case 3:
                    break;

                default:
                    goto Label_00D1;
            }
            this.<>f__this.movement.velocity -= this.<>f__this.movingPlatform.platformVelocity;
            this.$PC = -1;
        Label_00D1:
            return false;
        Label_00D3:
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

    private static class Callbacks
    {
        public static readonly CCTotem.ConfigurationBinder ConfigurationBinder = new CCTotem.ConfigurationBinder(CCMotor.Callbacks.OnConfigurationBinding);
        public static readonly CCDesc.HitFilter HitFilter = new CCDesc.HitFilter(CCMotor.Callbacks.OnHit);
        public static readonly CCTotem.PositionBinder PositionBinder = new CCTotem.PositionBinder(CCMotor.Callbacks.OnBindPosition);

        public static void InstallCallbacks(CCMotor CCMotor, CCTotemPole CCTotemPole)
        {
            CCTotemPole.Tag = CCMotor;
            CCTotemPole.OnBindPosition += PositionBinder;
            CCTotemPole.OnConfigurationBinding += ConfigurationBinder;
        }

        private static void OnBindPosition(ref CCTotem.PositionPlacement PositionPlacement, object Tag)
        {
            CCMotor motor = (CCMotor) Tag;
            if (motor != null)
            {
                motor.BindPosition(ref PositionPlacement);
            }
        }

        private static void OnConfigurationBinding(bool Bind, CCDesc CCDesc, object Tag)
        {
            CCHitDispatch hitDispatch = CCHitDispatch.GetHitDispatch(CCDesc);
            if (hitDispatch != null)
            {
                CCDesc.HitManager hits = hitDispatch.Hits;
                if (!object.ReferenceEquals(hits, null))
                {
                    if (Bind)
                    {
                        hits.Tag = Tag;
                        hits.OnHit += HitFilter;
                    }
                    else if (object.ReferenceEquals(hits.Tag, Tag))
                    {
                        hits.Tag = null;
                        hits.OnHit -= HitFilter;
                    }
                }
            }
            if (Bind)
            {
                CCDesc.Tag = Tag;
                if (CCDesc.GetComponent<CCTotemicFigure>() == null)
                {
                    IDRemote component = CCDesc.GetComponent<IDRemote>();
                    if (component == null)
                    {
                        component = CCDesc.gameObject.AddComponent<IDRemoteDefault>();
                    }
                    component.idMain = ((CCMotor) Tag).idMain;
                    CCDesc.detectCollisions = true;
                }
            }
            else if (object.ReferenceEquals(CCDesc.Tag, Tag))
            {
                CCDesc.Tag = null;
            }
        }

        private static bool OnHit(CCDesc.HitManager HitManager, ref CCDesc.Hit hit)
        {
            CCMotor tag = (CCMotor) HitManager.Tag;
            if (CCMotor.ccmotor_debug && !(hit.Collider is TerrainCollider))
            {
                object[] args = new object[] { hit.Point.x, hit.Point.y, hit.Point.z, hit.Normal.x, hit.Normal.y, hit.Normal.z, hit.MoveDirection.x, hit.MoveDirection.y, hit.MoveDirection.z, hit.MoveLength, hit.Collider };
                Debug.Log(string.Format("{{\"ccmotor\":{{\"hit\":{{\"point\":[{0},{1},{2}],\"normal\":[{3},{4},{5}]}},\"dir\":[{6},{7},{8}],\"move\":{9},\"obj\":{10}}}}}", args), hit.GameObject);
            }
            tag.OnHit(ref hit);
            return true;
        }

        public static void UninstallCallbacks(CCMotor CCMotor, CCTotemPole CCTotemPole)
        {
            if ((CCTotemPole != null) && object.ReferenceEquals(CCTotemPole.Tag, CCMotor))
            {
                CCTotemPole.OnConfigurationBinding -= ConfigurationBinder;
                CCTotemPole.OnBindPosition -= PositionBinder;
                CCTotemPole.Tag = null;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct InputFrame
    {
        public Vector3 moveDirection;
        public bool jump;
        public float crouchSpeed;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct JumpBaseVerticalSpeedArgs
    {
        private float _baseHeight;
        private float _gravity;
        private float _verticalSpeed;
        private bool dirty;
        public float baseHeight
        {
            get
            {
                return this._baseHeight;
            }
            set
            {
                if (this._baseHeight != value)
                {
                    this.dirty = true;
                    this._baseHeight = value;
                }
            }
        }
        public float gravity
        {
            get
            {
                return this._gravity;
            }
            set
            {
                if (this._gravity != value)
                {
                    this.dirty = true;
                    this._gravity = value;
                }
            }
        }
        public float CalculateVerticalSpeed(ref CCMotor.Jumping jumping, ref CCMotor.Movement movement)
        {
            if ((this.dirty || (this._baseHeight != jumping.baseHeight)) || (this._gravity != movement.gravity))
            {
                this._baseHeight = jumping.baseHeight;
                this._gravity = movement.gravity;
                this._verticalSpeed = Mathf.Sqrt((2f * this._baseHeight) * this._gravity);
                this.dirty = false;
            }
            return this._verticalSpeed;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Jumping
    {
        public bool enable;
        public float baseHeight;
        public float extraHeight;
        public float perpAmount;
        public float steepPerpAmount;
        public static readonly CCMotor.Jumping init;
        static Jumping()
        {
            CCMotor.Jumping jumping = new CCMotor.Jumping {
                enable = true,
                baseHeight = 1f,
                extraHeight = 4.1f,
                steepPerpAmount = 0.5f
            };
            init = jumping;
        }

        public override string ToString()
        {
            object[] args = new object[] { this.enable, this.baseHeight, this.extraHeight, this.perpAmount, this.steepPerpAmount };
            return string.Format("[Jumping: enable={0}, baseHeight={1}, extraHeight={2}, perpAmount={3}, steepPerpAmount={4}]", args);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct JumpingContext
    {
        public CCMotor.Jumping setup;
        public bool jumping;
        public bool holdingJumpButton;
        public bool startedJumping;
        public float lastStartTime;
        public float lastButtonDownTime;
        public float lastLandTime;
        public Vector3 jumpDir;
        public JumpingContext(ref CCMotor.Jumping setup)
        {
            this.setup = setup;
            this.jumping = false;
            this.holdingJumpButton = false;
            this.startedJumping = false;
            this.lastStartTime = 0f;
            this.lastButtonDownTime = -100f;
            this.jumpDir.x = 0f;
            this.jumpDir.y = 1f;
            this.jumpDir.z = 0f;
            this.lastLandTime = float.MinValue;
        }

        public JumpingContext(CCMotor.Jumping setup) : this(ref setup)
        {
        }

        public static implicit operator CCMotor.Jumping(CCMotor.JumpingContext c)
        {
            return c.setup;
        }
    }

    public enum JumpMovementTransfer
    {
        None,
        InitTransfer,
        PermaTransfer,
        PermaLocked
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Movement
    {
        public float maxForwardSpeed;
        public float maxSidewaysSpeed;
        public float maxBackwardsSpeed;
        public float maxGroundAcceleration;
        public float maxAirAcceleration;
        public float inputAirVelocityRatio;
        public float gravity;
        public float maxFallSpeed;
        public float maxAirHorizontalSpeed;
        public float maxUnblockingHeightDifference;
        public AnimationCurve slopeSpeedMultiplier;
        public static CCMotor.Movement init
        {
            get
            {
                CCMotor.Movement movement;
                movement.maxForwardSpeed = 3f;
                movement.maxSidewaysSpeed = 3f;
                movement.maxBackwardsSpeed = 3f;
                movement.maxGroundAcceleration = 30f;
                movement.maxAirAcceleration = 20f;
                movement.gravity = 10f;
                movement.maxFallSpeed = 20f;
                movement.inputAirVelocityRatio = 0.8f;
                movement.maxAirHorizontalSpeed = 750f;
                movement.maxUnblockingHeightDifference = 0f;
                Keyframe[] keys = new Keyframe[] { new Keyframe(-90f, 1f), new Keyframe(0f, 1f), new Keyframe(90f, 0f) };
                movement.slopeSpeedMultiplier = new AnimationCurve(keys);
                return movement;
            }
        }
        public override string ToString()
        {
            object[] args = new object[] { this.maxForwardSpeed, this.maxSidewaysSpeed, this.maxBackwardsSpeed, this.maxGroundAcceleration, this.maxAirAcceleration, this.inputAirVelocityRatio, this.gravity, this.maxFallSpeed, this.slopeSpeedMultiplier, this.maxAirHorizontalSpeed };
            return string.Format("[Movement: maxForwardSpeed={0}, maxSidewaysSpeed={1}, maxBackwardsSpeed={2}, maxGroundAcceleration={3}, maxAirAcceleration={4}, inputAirVelocityRatio={5}, gravity={6}, maxFallSpeed={7}, slopeSpeedMultiplier={8}, maxAirHorizontalSpeed={9}]", args);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MovementContext
    {
        public CCMotor.Movement setup;
        public CollisionFlags collisionFlags;
        public bool crouchBlocked;
        public Vector3 acceleration;
        public Vector3 velocity;
        public Vector3 frameVelocity;
        public Vector3 hitPoint;
        public Vector3 lastHitPoint;
        public MovementContext(ref CCMotor.Movement setup)
        {
            this.setup = setup;
            this.collisionFlags = CollisionFlags.None;
            this.crouchBlocked = false;
            this.acceleration = new Vector3();
            this.velocity = new Vector3();
            this.frameVelocity = new Vector3();
            this.hitPoint = new Vector3();
            this.lastHitPoint.x = float.PositiveInfinity;
            this.lastHitPoint.y = 0f;
            this.lastHitPoint.z = 0f;
        }

        public MovementContext(CCMotor.Movement setup) : this(ref setup)
        {
        }

        public static implicit operator CCMotor.Movement(CCMotor.MovementContext c)
        {
            return c.setup;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MovingPlatform
    {
        public bool enable;
        public CCMotor.JumpMovementTransfer movementTransfer;
        public static readonly CCMotor.MovingPlatform init;
        static MovingPlatform()
        {
            CCMotor.MovingPlatform platform = new CCMotor.MovingPlatform {
                enable = true,
                movementTransfer = CCMotor.JumpMovementTransfer.PermaTransfer
            };
            init = platform;
        }

        public override string ToString()
        {
            return string.Format("[MovingPlatform: enable={0}, movementTransfer={1}]", this.enable, this.movementTransfer);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MovingPlatformContext
    {
        public CCMotor.MovingPlatform setup;
        public Transform hitPlatform;
        public Transform activePlatform;
        public PointAndRotation activeLocal;
        public PointAndRotation activeGlobal;
        public Matrix4x4 lastMatrix;
        public Vector3 platformVelocity;
        public bool newPlatform;
        public MovingPlatformContext(ref CCMotor.MovingPlatform setup)
        {
            this.setup = setup;
            this.hitPlatform = null;
            this.activePlatform = null;
            this.activeLocal = new PointAndRotation();
            this.activeGlobal = new PointAndRotation();
            this.lastMatrix = new Matrix4x4();
            this.platformVelocity = new Vector3();
            this.newPlatform = false;
        }

        public MovingPlatformContext(CCMotor.MovingPlatform setup) : this(ref setup)
        {
        }

        public static implicit operator CCMotor.MovingPlatform(CCMotor.MovingPlatformContext c)
        {
            return c.setup;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct PointAndRotation
        {
            public Vector3 point;
            public Quaternion rotation;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Sliding
    {
        public bool enable;
        public float slidingSpeed;
        public float sidewaysControl;
        public float speedControl;
        public static readonly CCMotor.Sliding init;
        static Sliding()
        {
            CCMotor.Sliding sliding = new CCMotor.Sliding {
                enable = true,
                slidingSpeed = 15f,
                sidewaysControl = 1f,
                speedControl = 0.4f
            };
            init = sliding;
        }

        public override string ToString()
        {
            object[] args = new object[] { this.enable, this.slidingSpeed, this.sidewaysControl, this.speedControl };
            return string.Format("[Sliding enable={0}, slidingSpeed={1}, sidewaysControl={2}, speedControl={3}]", args);
        }
    }

    public enum StepMode
    {
        ViaUpdate,
        ViaFixedUpdate,
        Elsewhere
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct YawAngle
    {
        public readonly float Degrees;
        private YawAngle(float Degrees)
        {
            this.Degrees = Degrees;
        }

        public Vector3 Rotate(Vector3 direction)
        {
            return (Vector3) (Quaternion.AngleAxis(this.Degrees, Vector3.up) * direction);
        }

        public Vector3 Unrotate(Vector3 direction)
        {
            return (Vector3) (Quaternion.AngleAxis(this.Degrees, Vector3.down) * direction);
        }

        public static implicit operator CCMotor.YawAngle(float Degrees)
        {
            return new CCMotor.YawAngle(Degrees);
        }
    }
}

