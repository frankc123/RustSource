using System;
using UnityEngine;

public sealed class CCMotorSettings : ScriptableObject
{
    public float gravity = Movement_init.gravity;
    public float inputAirVelocityRatio = Movement_init.inputAirVelocityRatio;
    public float jumpBaseHeight = CCMotor.Jumping.init.baseHeight;
    public bool jumpEnable = CCMotor.Jumping.init.enable;
    public float jumpExtraHeight = CCMotor.Jumping.init.extraHeight;
    public float jumpPerpAmount = CCMotor.Jumping.init.perpAmount;
    public float jumpSteepPerpAmount = CCMotor.Jumping.init.steepPerpAmount;
    public float maxAirAcceleration = Movement_init.maxAirAcceleration;
    public float maxAirHorizontalSpeed = Movement_init.maxAirHorizontalSpeed;
    public float maxBackwardsSpeed = Movement_init.maxBackwardsSpeed;
    public float maxFallSpeed = Movement_init.maxFallSpeed;
    public float maxForwardSpeed = Movement_init.maxForwardSpeed;
    public float maxGroundAcceleration = Movement_init.maxGroundAcceleration;
    public float maxSidewaysSpeed = Movement_init.maxSidewaysSpeed;
    public float maxUnblockingHeightDifference = Movement_init.maxUnblockingHeightDifference;
    private static readonly CCMotor.Movement Movement_init = CCMotor.Movement.init;
    public bool platformMovementEnable = CCMotor.MovingPlatform.init.enable;
    public CCMotor.JumpMovementTransfer platformMovementTransfer = CCMotor.MovingPlatform.init.movementTransfer;
    public bool slidingEnable = CCMotor.Sliding.init.enable;
    public float slidingSidewaysControl = CCMotor.Sliding.init.sidewaysControl;
    public float slidingSpeed = CCMotor.Sliding.init.slidingSpeed;
    public float slidingSpeedControl = CCMotor.Sliding.init.speedControl;
    public AnimationCurve slopeSpeedMultiplier = CCMotor.Movement.init.slopeSpeedMultiplier;

    public void BindSettingsTo(CCMotor motor)
    {
        motor.jumping.setup = this.jumping;
        motor.movement.setup = this.movement;
        motor.movingPlatform.setup = this.movingPlatform;
        motor.sliding = this.sliding;
        motor.OnBindCCMotorSettings();
    }

    public void CopySettingsFrom(CCMotor motor)
    {
        this.jumping = motor.jumping.setup;
        this.movement = motor.movement.setup;
        this.movingPlatform = motor.movingPlatform.setup;
        this.sliding = motor.sliding;
    }

    public override string ToString()
    {
        object[] args = new object[] { this.movement, this.jumping, this.sliding, this.movingPlatform };
        return string.Format("[CCMotorSettings: movement={0}, jumping={1}, sliding={2}, movingPlatform={3}]", args);
    }

    public CCMotor.Jumping jumping
    {
        get
        {
            CCMotor.Jumping jumping;
            jumping.enable = this.jumpEnable;
            jumping.baseHeight = this.jumpBaseHeight;
            jumping.extraHeight = this.jumpExtraHeight;
            jumping.perpAmount = this.jumpPerpAmount;
            jumping.steepPerpAmount = this.jumpSteepPerpAmount;
            return jumping;
        }
        set
        {
            this.jumpEnable = value.enable;
            this.jumpBaseHeight = value.baseHeight;
            this.jumpExtraHeight = value.extraHeight;
            this.jumpPerpAmount = value.perpAmount;
            this.jumpSteepPerpAmount = value.steepPerpAmount;
        }
    }

    public CCMotor.Movement movement
    {
        get
        {
            CCMotor.Movement movement;
            movement.maxForwardSpeed = this.maxForwardSpeed;
            movement.maxSidewaysSpeed = this.maxSidewaysSpeed;
            movement.maxBackwardsSpeed = this.maxBackwardsSpeed;
            movement.maxGroundAcceleration = this.maxGroundAcceleration;
            movement.maxAirAcceleration = this.maxAirAcceleration;
            movement.inputAirVelocityRatio = this.inputAirVelocityRatio;
            movement.gravity = this.gravity;
            movement.maxFallSpeed = this.maxFallSpeed;
            movement.maxAirHorizontalSpeed = this.maxAirHorizontalSpeed;
            movement.maxUnblockingHeightDifference = this.maxUnblockingHeightDifference;
            movement.slopeSpeedMultiplier = new AnimationCurve(this.slopeSpeedMultiplier.keys);
            movement.slopeSpeedMultiplier.postWrapMode = this.slopeSpeedMultiplier.postWrapMode;
            movement.slopeSpeedMultiplier.preWrapMode = this.slopeSpeedMultiplier.preWrapMode;
            return movement;
        }
        set
        {
            this.maxForwardSpeed = value.maxForwardSpeed;
            this.maxSidewaysSpeed = value.maxSidewaysSpeed;
            this.maxBackwardsSpeed = value.maxBackwardsSpeed;
            this.maxGroundAcceleration = value.maxGroundAcceleration;
            this.maxAirAcceleration = value.maxAirAcceleration;
            this.inputAirVelocityRatio = value.inputAirVelocityRatio;
            this.gravity = value.gravity;
            this.maxFallSpeed = value.maxFallSpeed;
            this.maxUnblockingHeightDifference = value.maxUnblockingHeightDifference;
            this.slopeSpeedMultiplier.keys = value.slopeSpeedMultiplier.keys;
            this.slopeSpeedMultiplier.postWrapMode = value.slopeSpeedMultiplier.postWrapMode;
            this.slopeSpeedMultiplier.preWrapMode = value.slopeSpeedMultiplier.preWrapMode;
        }
    }

    public CCMotor.MovingPlatform movingPlatform
    {
        get
        {
            CCMotor.MovingPlatform platform;
            platform.enable = this.platformMovementEnable;
            platform.movementTransfer = this.platformMovementTransfer;
            return platform;
        }
        set
        {
            this.platformMovementEnable = value.enable;
            this.platformMovementTransfer = value.movementTransfer;
        }
    }

    public CCMotor.Sliding sliding
    {
        get
        {
            CCMotor.Sliding sliding;
            sliding.enable = this.slidingEnable;
            sliding.slidingSpeed = this.slidingSpeed;
            sliding.sidewaysControl = this.slidingSidewaysControl;
            sliding.speedControl = this.slidingSpeedControl;
            return sliding;
        }
        set
        {
            this.slidingEnable = value.enable;
            this.slidingSpeed = value.slidingSpeed;
            this.slidingSidewaysControl = value.sidewaysControl;
            this.slidingSpeedControl = value.speedControl;
        }
    }
}

