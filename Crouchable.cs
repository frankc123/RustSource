using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class Crouchable : IDLocalCharacter
{
    [NonSerialized]
    private CharacterCrouchTrait _crouchTrait;
    [NonSerialized]
    private float crouchTime;
    [NonSerialized]
    private float crouchUnits;
    [NonSerialized]
    private bool didCrouchTraitTest;
    private const double kSmoothDamp = 0.5;
    private const double kSmoothDampInput = 0.0;
    private const double kSmoothInterval = 0.0032239760652016921;
    private const double kSmoothStiffness = 5.0;

    protected internal void ApplyCrouch(ref Vector3 localPosition)
    {
        localPosition.y += this.crouchUnits;
    }

    public void ApplyCrouchOffset(ref CCTotem.PositionPlacement placement)
    {
        float num = placement.bottom.y + base.initialEyesOffsetY;
        float num2 = placement.originalTop.y - num;
        float num3 = placement.top.y - num2;
        float a = num3 - num;
        this.crouchUnits = !Mathf.Approximately(a, 0f) ? a : 0f;
        base.idMain.InvalidateEyesOffset();
    }

    public void LocalPlayerUpdateCrouchState(ref CrouchState incoming, ref bool crouchFlag, ref bool crouchBlockFlag, ref Smoothing smoothing)
    {
        double initialEyesOffsetY = base.initialEyesOffsetY;
        double num2 = incoming.BottomY + initialEyesOffsetY;
        double num3 = incoming.BottomY + incoming.InitialStandingHeight;
        double num4 = num3 - num2;
        double num5 = incoming.TopY - num4;
        double num6 = num5 - incoming.BottomY;
        double target = num6 - initialEyesOffsetY;
        this.crouchUnits = smoothing.CatchUp(target);
        base.idMain.InvalidateEyesOffset();
        if (incoming.CrouchBlocked)
        {
            crouchBlockFlag = true;
            crouchFlag = true;
        }
        else
        {
            crouchBlockFlag = false;
        }
    }

    public void LocalPlayerUpdateCrouchState(CCMotor ccmotor, ref bool crouchFlag, ref bool crouchBlockFlag, ref Smoothing smoothing)
    {
        CrouchState state;
        state.CrouchBlocked = ccmotor.isCrouchBlocked;
        CCTotem.PositionPlacement? lastPositionPlacement = ccmotor.LastPositionPlacement;
        CCTotem.PositionPlacement placement = !lastPositionPlacement.HasValue ? new CCTotem.PositionPlacement(base.origin, base.origin, base.origin, ccmotor.ccTotemPole.MaximumHeight) : lastPositionPlacement.Value;
        state.BottomY = placement.bottom.y;
        state.TopY = placement.top.y;
        state.InitialStandingHeight = placement.originalHeight;
        this.LocalPlayerUpdateCrouchState(ref state, ref crouchFlag, ref crouchBlockFlag, ref smoothing);
    }

    public Crouchable crouchable
    {
        get
        {
            return this;
        }
    }

    protected AnimationCurve crouchCurve
    {
        get
        {
            return this.crouchTrait.crouchCurve;
        }
    }

    public bool crouched
    {
        get
        {
            return (this.crouchUnits < 0f);
        }
    }

    protected float crouchToSpeedFraction
    {
        get
        {
            return this.crouchTrait.crouchToSpeedFraction;
        }
    }

    protected CharacterCrouchTrait crouchTrait
    {
        get
        {
            if (!this.didCrouchTraitTest)
            {
                this._crouchTrait = base.GetTrait<CharacterCrouchTrait>();
                this.didCrouchTraitTest = true;
            }
            return this._crouchTrait;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CrouchState
    {
        public bool CrouchBlocked;
        public float BottomY;
        public float TopY;
        public float InitialStandingHeight;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Smoothing
    {
        private bool I;
        private double T;
        private double A;
        private double V;
        private double Y;
        private double Z;
        public void Reset()
        {
            this = new Crouchable.Smoothing();
        }

        public void Solve()
        {
            this.A = this.T;
            this.V = this.Z = this.Y = 0.0;
            this.I = true;
        }

        public void AddSeconds(double elapsedSeconds)
        {
            if (elapsedSeconds > 0.0)
            {
                this.Z += elapsedSeconds;
                this.Y += elapsedSeconds;
            }
        }

        public float CatchUp(double target)
        {
            double num;
            if (!this.I)
            {
                this.Y = this.V = this.Z = 0.0;
                this.I = true;
                num = this.T = this.A = target;
            }
            else
            {
                if (this.Z > 0.0)
                {
                    this.V += ((target - this.T) / this.Z) * 0.0;
                    this.Z = 0.0;
                }
                double num2 = 0.0032239760652016921;
                double y = this.Y;
                if (y >= num2)
                {
                    double num4 = this.A - (this.T = target);
                    double v = this.V;
                    double num6 = 0.5;
                    double num7 = 5.0;
                    do
                    {
                        double num9 = num4;
                        num4 += v * num2;
                        double num8 = (-num4 * num7) - (num6 * v);
                        num4 += num8 * num2;
                        v = (num4 - num9) / num2;
                    }
                    while ((y -= num2) >= num2);
                    this.A = target + num4;
                    this.V = v;
                    this.Y = y;
                }
                num = (y >= 1.4012984643248171E-45) ? (this.A + (this.V * y)) : this.A;
            }
            return (((num >= 1.4012984643248171E-45) || (num <= -1.4012984643248171E-45)) ? ((float) num) : 0f);
        }
    }
}

