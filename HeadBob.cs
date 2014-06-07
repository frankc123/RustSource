using Facepunch.Precision;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

public sealed class HeadBob : MonoBehaviour, ICameraFX
{
    private float _aimPositionScalar = 1f;
    private float _aimRotationScalar = 1f;
    private bool _allow;
    [SerializeField]
    private float _globalPositionScalar = 1f;
    [SerializeField]
    private float _globalRotationScalar = 1f;
    [SerializeField]
    private float _globalScalar = 1f;
    [SerializeField]
    private CCMotor _motor;
    [SerializeField]
    private CameraMount _mount;
    private float _viewModelPositionScalar = 1f;
    private float _viewModelRotationScalar = 1f;
    private bool _wasForbidden;
    private bool added;
    private int additionalCurveCount;
    public bool allowAntiOutputs;
    private float allowFractionNormalized;
    public bool allowOnEnable = true;
    private float allowValue;
    private bool anyAdditionalCurves;
    private bool awake;
    private static double bob_scale = 1.0;
    private static double bob_scale_angular = 1.0;
    private static double bob_scale_linear = 1.0;
    public BobConfiguration cfg;
    public bool forceForbidOnDisable;
    private Vector3G groundLocalAngularVelocity;
    private double groundLocalAngularVelocityMag;
    private Vector3G groundLocalVelocity;
    private double groundLocalVelocityMag;
    private Vector3G groundWorldVelocity;
    private double groundWorldVelocityMag;
    private bool hadMotor;
    private VectorAccelSampler impulseForce;
    private VectorAccelSampler impulseTorque;
    private Vector3G inputForce;
    private double intermitFraction;
    private Weight intermitNext;
    private Weight intermitStart;
    private VectorStamp lastPosition;
    private VectorStamp lastRotation;
    private Vector3G localAngularVelocity;
    private double localAngularVelocityMag;
    private Matrix4x4G localToWorld;
    private Vector3G localVelocity;
    private double localVelocityMag;
    private Transform otherParent;
    private Vector3 preCullLP;
    private Vector3 preCullLR;
    private Weight predicted;
    private Vector3G raw_pos;
    private Vector3G raw_rot;
    public bool simStep = true;
    private double timeIntermit;
    private double timeSolve;
    private ViewModel viewModel;
    private Weight working;
    private Matrix4x4G worldToLocal;
    private Vector3G worldVelocity;
    private double worldVelocityMag;

    public bool AddEffect(BobEffect effect)
    {
        return this.working.stack.CreateInstance(effect);
    }

    private bool Advance(float dt)
    {
        bool flag = false;
        if (this._motor != null)
        {
            flag = this.CheckChanges(true, this._motor.idMain.transform);
            this.PushPosition();
            this.GatherInfo(this._motor);
        }
        else
        {
            if (base.transform.parent == null)
            {
            }
            flag = this.CheckChanges(false, base.transform);
            this.PushPosition();
        }
        if (this.cfg.additionalCurves != null)
        {
            this.anyAdditionalCurves = (this.additionalCurveCount = this.cfg.additionalCurves.Length) > 0;
        }
        else
        {
            this.additionalCurveCount = 0;
            this.anyAdditionalCurves = false;
        }
        if (this.anyAdditionalCurves)
        {
            Array.Resize<Vector3G>(ref this.working.additionalPositions, this.additionalCurveCount);
            Array.Resize<Vector3G>(ref this.predicted.additionalPositions, this.additionalCurveCount);
        }
        if (this._allow)
        {
            if (this.allowFractionNormalized < 1f)
            {
                int length = this.cfg.allowCurve.length;
                if (length == 0f)
                {
                    this.allowFractionNormalized = 1f;
                    this.allowValue = 1f;
                }
                else
                {
                    this.allowFractionNormalized += dt / ((float) length);
                    if (this.allowFractionNormalized >= 1f)
                    {
                        this.allowFractionNormalized = 1f;
                        this.allowValue = 1f;
                    }
                    else
                    {
                        this.allowValue = this.cfg.allowCurve.Evaluate(this.allowFractionNormalized * length);
                    }
                }
                flag = true;
            }
        }
        else
        {
            if (this.allowFractionNormalized > 0f)
            {
                int num2 = this.cfg.forbidCurve.length;
                if (num2 == 0f)
                {
                    this.allowFractionNormalized = 0f;
                    this.allowValue = 0f;
                }
                else
                {
                    this.allowFractionNormalized -= dt / ((float) num2);
                    if (this.allowFractionNormalized <= 0f)
                    {
                        this.allowFractionNormalized = 0f;
                        this.allowValue = 0f;
                    }
                    else
                    {
                        this.allowValue = 1f - this.cfg.forbidCurve.Evaluate((1f - this.allowFractionNormalized) * num2);
                    }
                }
                flag = true;
            }
            if (this._wasForbidden && (this.allowFractionNormalized == 0f))
            {
                base.enabled = false;
            }
        }
        if ((this.Step(dt) == 0) && !flag)
        {
            return false;
        }
        return true;
    }

    private void Awake()
    {
        this.awake = true;
        this.working.stack = new BobEffectStack();
        this.predicted.stack = this.working.stack.Fork();
    }

    private bool CheckChanges(bool hasMotor, Transform parent)
    {
        if ((this.hadMotor == hasMotor) && (this.otherParent == parent))
        {
            return false;
        }
        this.hadMotor = hasMotor;
        this.groundLocalVelocity = new Vector3G();
        this.groundWorldVelocity = new Vector3G();
        this.localVelocity = new Vector3G();
        this.worldVelocity = new Vector3G();
        this.impulseForce = new VectorAccelSampler();
        this.impulseTorque = new VectorAccelSampler();
        this.lastPosition = new VectorStamp();
        this.otherParent = parent;
        this.raw_pos = new Vector3G();
        this.raw_rot = new Vector3G();
        BobEffectStack stack = this.predicted.stack;
        this.predicted = new Weight();
        this.predicted.stack = stack;
        stack = this.working.stack;
        this.working = new Weight();
        this.working.stack = stack;
        return true;
    }

    private void CheckDeadZone()
    {
        if ((this.raw_pos.x >= -this.cfg.positionDeadzone.x) && (this.raw_pos.x < this.cfg.positionDeadzone.x))
        {
            this.raw_pos.x = 0.0;
        }
        if ((this.raw_pos.y >= -this.cfg.positionDeadzone.y) && (this.raw_pos.y < this.cfg.positionDeadzone.y))
        {
            this.raw_pos.y = 0.0;
        }
        if ((this.raw_pos.z >= -this.cfg.positionDeadzone.z) && (this.raw_pos.z < this.cfg.positionDeadzone.z))
        {
            this.raw_pos.z = 0.0;
        }
        if ((this.raw_rot.x >= -this.cfg.rotationDeadzone.x) && (this.raw_rot.x < this.cfg.rotationDeadzone.x))
        {
            this.raw_rot.x = 0.0;
        }
        if ((this.raw_rot.y >= -this.cfg.rotationDeadzone.y) && (this.raw_rot.y < this.cfg.rotationDeadzone.y))
        {
            this.raw_rot.y = 0.0;
        }
        if ((this.raw_rot.z >= -this.cfg.rotationDeadzone.z) && (this.raw_rot.z < this.cfg.rotationDeadzone.z))
        {
            this.raw_rot.z = 0.0;
        }
    }

    private static void DrawForceAxes(Vector3 force, Vector3 radii, Vector3 k, float boxDim)
    {
        Color color = Gizmos.color;
        Gizmos.color = color * Color.red;
        DrawForceLine(Vector3.right, force, radii, k, boxDim);
        Gizmos.color = color * Color.green;
        DrawForceLine(Vector3.up, force, radii, k, boxDim);
        Gizmos.color = color * Color.blue;
        DrawForceLine(Vector3.forward, force, radii, k, boxDim);
        Gizmos.color = color;
    }

    private static void DrawForceLine(Vector3 posdir, Vector3 force, Vector3 radii, Vector3 k, float boxDim)
    {
        Vector3 rhs = Vector3.Scale(radii, posdir);
        Vector3 vector2 = (Vector3) (rhs * 2f);
        float num = Vector3.Dot(force, posdir) / (Vector3.Dot(posdir, radii) * Vector3.Dot(posdir, k));
        if (num < 0f)
        {
            num = -num;
            rhs = -rhs;
            vector2 = -vector2;
            posdir = -posdir;
        }
        Vector3 vector3 = rhs + ((Vector3) ((vector2 - rhs) * num));
        Color color = Gizmos.color;
        Gizmos.color = color * new Color(1f, 1f, 1f, 0.5f);
        Matrix4x4 matrix = Gizmos.matrix;
        Gizmos.matrix *= Matrix4x4.TRS(Vector3.zero, Quaternion.LookRotation(posdir), new Vector3(1f, 1f, 1f));
        float num2 = Vector3.Dot(posdir, rhs);
        float z = Vector3.Dot(posdir, vector2) - num2;
        float num4 = Vector3.Dot(posdir, vector3) - num2;
        Gizmos.DrawWireCube(new Vector3(0f, 0f, num2 + (z / 2f)), new Vector3(boxDim, boxDim, z));
        Gizmos.DrawWireCube(new Vector3(0f, 0f, -(num2 + (z / 2f))), new Vector3(boxDim, boxDim, z));
        Gizmos.color = color;
        Gizmos.DrawCube(new Vector3(0f, 0f, num2 + (num4 / 2f)), new Vector3(boxDim, boxDim, num4));
        Gizmos.matrix = matrix;
    }

    private void GatherInfo(CCMotor motor)
    {
        if (motor.isGrounded && !motor.isSliding)
        {
            this.groundLocalVelocity = this.localVelocity;
            this.groundWorldVelocity = this.worldVelocity;
            this.groundLocalAngularVelocity = this.localAngularVelocity;
            this.groundLocalVelocityMag = this.localVelocityMag;
            this.groundWorldVelocityMag = this.worldVelocityMag;
            this.groundLocalAngularVelocityMag = this.localAngularVelocityMag;
        }
        else
        {
            this.groundLocalVelocity = new Vector3G();
            this.groundWorldVelocity = new Vector3G();
            this.groundLocalAngularVelocity = new Vector3G();
            this.groundLocalVelocityMag = 0.0;
            this.groundWorldVelocityMag = 0.0;
            this.groundLocalAngularVelocityMag = 0.0;
        }
        this.inputForce.x = motor.input.moveDirection.x;
        this.inputForce.y = motor.input.moveDirection.y;
        this.inputForce.z = motor.input.moveDirection.z;
        Matrix4x4G.Mult3x3(ref this.inputForce, ref this.worldToLocal, out this.inputForce);
        this.inputForce.x *= this.cfg.inputForceMultiplier.x;
        this.inputForce.y *= this.cfg.inputForceMultiplier.y;
        this.inputForce.z *= this.cfg.inputForceMultiplier.z;
    }

    void ICameraFX.OnViewModelChange(ViewModel viewModel)
    {
        if (this.viewModel != viewModel)
        {
            Transform transform;
            Transform transform2;
            this._viewModelPositionScalar = 1f;
            this._viewModelRotationScalar = 1f;
            if (this.viewModel != null)
            {
                transform = this.viewModel.transform;
                this.viewModel.headBob = null;
            }
            else
            {
                transform = base.transform;
            }
            if (viewModel != null)
            {
                viewModel.headBob = this;
                transform2 = viewModel.transform;
            }
            else
            {
                transform2 = base.transform;
            }
            this.viewModel = viewModel;
            if (transform == null)
            {
                transform = null;
            }
            if (transform2 == null)
            {
                transform2 = null;
            }
        }
    }

    void ICameraFX.PostRender()
    {
        Transform transform1 = base.transform;
        transform1.localPosition -= this.preCullLP;
        Transform transform2 = base.transform;
        transform2.localEulerAngles -= this.preCullLR;
        if (this.added)
        {
            bool viewModel = (bool) this.viewModel;
            int num = ((this.cfg.antiOutputs != null) && this.allowAntiOutputs) ? this.cfg.antiOutputs.Length : 0;
            if (viewModel)
            {
                Transform transform = this.viewModel.transform;
                for (int i = num - 1; i >= 0; i--)
                {
                    this.cfg.antiOutputs[i].Subtract(transform);
                }
            }
            this.added = false;
        }
    }

    void ICameraFX.PreCull()
    {
        int num = ((this.cfg.antiOutputs != null) && this.allowAntiOutputs) ? this.cfg.antiOutputs.Length : 0;
        bool viewModel = (bool) this.viewModel;
        Transform transform = !viewModel ? null : this.viewModel.transform;
        if (viewModel && this.added)
        {
            for (int i = num - 1; i >= 0; i--)
            {
                this.cfg.antiOutputs[i].Subtract(transform);
            }
        }
        this.Advance(Time.deltaTime);
        this.preCullLP = this.offset;
        this.preCullLR = this.rotationOffset;
        Transform transform1 = base.transform;
        transform1.localPosition += this.preCullLP;
        Transform transform2 = base.transform;
        transform2.localEulerAngles += this.preCullLR;
        num = ((this.cfg.antiOutputs != null) && this.allowAntiOutputs) ? this.cfg.antiOutputs.Length : 0;
        if (viewModel)
        {
            this.added = true;
            for (int j = num - 1; j >= 0; j--)
            {
                this.cfg.antiOutputs[j].Add(transform, ref this.preCullLP, ref this.preCullLR);
            }
        }
    }

    private void LateUpdate()
    {
        if (base.camera == null)
        {
            if (this.Advance(Time.deltaTime))
            {
                base.transform.localPosition = this.offset;
                base.transform.localEulerAngles = this.rotationOffset;
            }
        }
        else if ((!this._allow && (this._mount != null)) && !this._mount.isActiveMount)
        {
            base.enabled = false;
        }
    }

    private void OnDestroy()
    {
        this.forceForbidOnDisable = false;
    }

    private void OnDisable()
    {
        if (this.awake)
        {
            if (this.forceForbidOnDisable && this._allow)
            {
                base.enabled = true;
                this._wasForbidden = base.enabled;
                if (this._wasForbidden)
                {
                    this._allow = false;
                    return;
                }
            }
            this.allowFractionNormalized = 0f;
            this.allowValue = 0f;
            base.transform.localPosition = Vector3.zero;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.matrix = ((base.transform.parent == null) ? ((Matrix4x4) base.transform) : ((Matrix4x4) base.transform.parent)).localToWorldMatrix;
        Gizmos.DrawLine(Vector3.zero, this.offset);
        Matrix4x4 matrix = Gizmos.matrix;
        Gizmos.matrix *= Matrix4x4.Scale(this.cfg.elipsoidRadii);
        Gizmos.DrawWireSphere(Vector3.zero, 1f);
        Gizmos.matrix = matrix;
        Gizmos.color = new Color(1f, 1f, 1f, 0.8f);
        DrawForceAxes(this.working.position.acceleration.f, this.cfg.elipsoidRadii, this.cfg.springConstant, 0.2f);
        Gizmos.color = Color.white;
        DrawForceAxes(this.working.position.acceleration.f, this.cfg.elipsoidRadii, this.cfg.maxVelocity, 0.1f);
    }

    private void OnEnable()
    {
        if (this.allowOnEnable)
        {
            this._allow = true;
        }
        this._wasForbidden = false;
    }

    private void OnLocallyAppended(IDMain main)
    {
        if (this._motor == null)
        {
            this._motor = main.GetRemote<CCMotor>();
        }
    }

    private void PushPosition()
    {
        VectorStamp stamp;
        VectorStamp stamp2;
        Vector3 eyesOrigin;
        Vector3 eulerAngles;
        Character character;
        this.worldToLocal.f = this.otherParent.worldToLocalMatrix;
        this.localToWorld.f = this.otherParent.localToWorldMatrix;
        stamp.timeStamp = Time.time;
        stamp.valid = true;
        if ((this._motor != null) && ((character = this._motor.idMain as Character) != null))
        {
            eulerAngles = character.eyesAngles.eulerAngles;
            eyesOrigin = character.eyesOrigin;
        }
        else
        {
            eulerAngles = this.otherParent.eulerAngles;
            eyesOrigin = this.otherParent.position;
        }
        stamp.vector.x = eyesOrigin.x;
        stamp.vector.y = eyesOrigin.y;
        stamp.vector.z = eyesOrigin.z;
        stamp2.vector.x = eulerAngles.x;
        stamp2.vector.y = eulerAngles.y;
        stamp2.vector.z = eulerAngles.z;
        stamp2.timeStamp = Time.time;
        stamp2.valid = true;
        if (this.lastPosition.valid && (this.lastPosition.timeStamp != stamp.timeStamp))
        {
            double num = 1.0 / ((double) (stamp.timeStamp - this.lastPosition.timeStamp));
            this.worldVelocity.x = (stamp.vector.x - this.lastPosition.vector.x) * num;
            this.worldVelocity.y = (stamp.vector.y - this.lastPosition.vector.y) * num;
            this.worldVelocity.z = (stamp.vector.z - this.lastPosition.vector.z) * num;
            Matrix4x4G.Mult3x3(ref this.worldVelocity, ref this.worldToLocal, out this.localVelocity);
        }
        this.impulseForce.Sample(ref this.localVelocity, stamp.timeStamp);
        this.lastPosition = stamp;
        if (this.lastRotation.valid && (this.lastRotation.timeStamp != stamp2.timeStamp))
        {
            double num2 = 1.0 / ((double) (stamp2.timeStamp - this.lastRotation.timeStamp));
            Precise.DeltaAngle(ref this.lastRotation.vector.x, ref stamp2.vector.x, out this.localAngularVelocity.x);
            Precise.DeltaAngle(ref this.lastRotation.vector.y, ref stamp2.vector.y, out this.localAngularVelocity.y);
            Precise.DeltaAngle(ref this.lastRotation.vector.z, ref stamp2.vector.z, out this.localAngularVelocity.z);
            this.localAngularVelocity.x *= num2;
            this.localAngularVelocity.y *= num2;
            this.localAngularVelocity.z *= num2;
        }
        this.impulseTorque.Sample(ref this.localAngularVelocity, stamp2.timeStamp);
        this.lastRotation = stamp2;
        this.localVelocityMag = Math.Sqrt(((this.localVelocity.x * this.localVelocity.x) + (this.localVelocity.y * this.localVelocity.y)) + (this.localVelocity.z * this.localVelocity.z));
        this.worldVelocityMag = Math.Sqrt(((this.worldVelocity.x * this.worldVelocity.x) + (this.worldVelocity.y * this.worldVelocity.y)) + (this.worldVelocity.z * this.worldVelocity.z));
        this.localAngularVelocityMag = Math.Sqrt(((this.localAngularVelocity.x * this.localAngularVelocity.x) + (this.localAngularVelocity.y * this.localAngularVelocity.y)) + (this.localAngularVelocity.z * this.localAngularVelocity.z));
    }

    private void Solve(ref Weight weight, ref double dt)
    {
        Vector3G vectorg;
        vectorg.x = (dt * this.groundLocalVelocity.x) * this.cfg.forceSpeedMultiplier.x;
        vectorg.y = (dt * this.groundLocalVelocity.y) * this.cfg.forceSpeedMultiplier.y;
        vectorg.z = (dt * this.groundLocalVelocity.z) * this.cfg.forceSpeedMultiplier.z;
        Vector3G fE = weight.position.fE;
        Vector3G vectorg3 = weight.rotation.fE;
        weight.position.fE = new Vector3G();
        weight.rotation.fE = new Vector3G();
        if (this.anyAdditionalCurves)
        {
            for (int i = 0; i < this.additionalCurveCount; i++)
            {
                double groundLocalVelocityMag;
                BobForceCurve curve = this.cfg.additionalCurves[i];
                switch (curve.source)
                {
                    case BobForceCurveSource.LocalMovementMagnitude:
                        groundLocalVelocityMag = this.groundLocalVelocityMag;
                        break;

                    case BobForceCurveSource.LocalMovementX:
                        groundLocalVelocityMag = this.groundLocalVelocity.x;
                        break;

                    case BobForceCurveSource.LocalMovementY:
                        groundLocalVelocityMag = this.groundLocalVelocity.y;
                        break;

                    case BobForceCurveSource.LocalMovementZ:
                        groundLocalVelocityMag = this.groundLocalVelocity.z;
                        break;

                    case BobForceCurveSource.WorldMovementMagnitude:
                        groundLocalVelocityMag = this.groundWorldVelocityMag;
                        break;

                    case BobForceCurveSource.WorldMovementX:
                        groundLocalVelocityMag = this.groundWorldVelocity.x;
                        break;

                    case BobForceCurveSource.WorldMovementY:
                        groundLocalVelocityMag = this.groundWorldVelocity.y;
                        break;

                    case BobForceCurveSource.WorldMovementZ:
                        groundLocalVelocityMag = this.groundWorldVelocity.z;
                        break;

                    case BobForceCurveSource.LocalVelocityMagnitude:
                        groundLocalVelocityMag = this.localVelocityMag;
                        break;

                    case BobForceCurveSource.LocalVelocityX:
                        groundLocalVelocityMag = this.localVelocity.x;
                        break;

                    case BobForceCurveSource.LocalVelocityY:
                        groundLocalVelocityMag = this.localVelocity.y;
                        break;

                    case BobForceCurveSource.WorldVelocityMagnitude:
                        groundLocalVelocityMag = this.worldVelocityMag;
                        break;

                    case BobForceCurveSource.WorldVelocityX:
                        groundLocalVelocityMag = this.worldVelocity.x;
                        break;

                    case BobForceCurveSource.WorldVelocityY:
                        groundLocalVelocityMag = this.worldVelocity.y;
                        break;

                    case BobForceCurveSource.WorldVelocityZ:
                        groundLocalVelocityMag = this.worldVelocity.z;
                        break;

                    case BobForceCurveSource.RotationMagnitude:
                        groundLocalVelocityMag = this.localAngularVelocityMag;
                        break;

                    case BobForceCurveSource.RotationPitch:
                        groundLocalVelocityMag = this.localAngularVelocity.x;
                        break;

                    case BobForceCurveSource.RotationYaw:
                        groundLocalVelocityMag = this.localAngularVelocity.y;
                        break;

                    case BobForceCurveSource.RotationRoll:
                        groundLocalVelocityMag = this.localAngularVelocity.z;
                        break;

                    case BobForceCurveSource.TurnMagnitude:
                        groundLocalVelocityMag = this.groundLocalAngularVelocityMag;
                        break;

                    case BobForceCurveSource.TurnPitch:
                        groundLocalVelocityMag = this.groundLocalAngularVelocity.x;
                        break;

                    case BobForceCurveSource.TurnYaw:
                        groundLocalVelocityMag = this.groundLocalAngularVelocity.y;
                        break;

                    case BobForceCurveSource.TurnRoll:
                        groundLocalVelocityMag = this.groundLocalAngularVelocity.z;
                        break;

                    default:
                        groundLocalVelocityMag = this.localVelocity.z;
                        break;
                }
                BobForceCurveTarget target = curve.target;
                if ((target == BobForceCurveTarget.Position) || (target != BobForceCurveTarget.Rotation))
                {
                    curve.Calculate(ref weight.additionalPositions[i], ref groundLocalVelocityMag, ref dt, ref weight.position.fE);
                }
                else
                {
                    curve.Calculate(ref weight.additionalPositions[i], ref groundLocalVelocityMag, ref dt, ref weight.rotation.fE);
                }
            }
        }
        if (this.cfg.impulseForceSmooth > 0f)
        {
            Vector3G.SmoothDamp(ref weight.position.fI, ref this.impulseForce.accel, ref weight.position.fIV, this.cfg.impulseForceSmooth, this.cfg.impulseForceMaxChangeAcceleration, ref dt);
        }
        else
        {
            weight.position.fI = this.impulseForce.accel;
        }
        if (this.cfg.angleImpulseForceSmooth > 0f)
        {
            Vector3G.SmoothDamp(ref weight.rotation.fI, ref this.impulseTorque.accel, ref weight.rotation.fIV, this.cfg.angleImpulseForceSmooth, this.cfg.angleImpulseForceMaxChangeAcceleration, ref dt);
        }
        else
        {
            weight.rotation.fI = this.impulseTorque.accel;
        }
        weight.position.fE.x += this.inputForce.x + (weight.position.fI.x * this.cfg.impulseForceScale.x);
        weight.position.fE.y += this.inputForce.y + (weight.position.fI.y * this.cfg.impulseForceScale.y);
        weight.position.fE.z += this.inputForce.z + (weight.position.fI.z * this.cfg.impulseForceScale.z);
        weight.rotation.fE.x += weight.rotation.fI.x * this.cfg.angularImpulseForceScale.x;
        weight.rotation.fE.y += weight.rotation.fI.y * this.cfg.angularImpulseForceScale.y;
        weight.rotation.fE.z += weight.rotation.fI.z * this.cfg.angularImpulseForceScale.z;
        Vector3G vectorg4 = weight.position.value;
        vectorg4.x /= (double) this.cfg.elipsoidRadii.x;
        vectorg4.y /= (double) this.cfg.elipsoidRadii.y;
        vectorg4.z /= (double) this.cfg.elipsoidRadii.z;
        double d = ((vectorg4.x * vectorg4.x) + (vectorg4.y * vectorg4.y)) + (vectorg4.z * vectorg4.z);
        if (d > 1.0)
        {
            d = 1.0 / Math.Sqrt(d);
            vectorg4.x *= d;
            vectorg4.y *= d;
            vectorg4.z *= d;
        }
        vectorg4.x *= this.cfg.elipsoidRadii.x;
        vectorg4.y *= this.cfg.elipsoidRadii.y;
        vectorg4.z *= this.cfg.elipsoidRadii.z;
        weight.stack.Simulate(ref dt, ref weight.position.fE, ref weight.rotation.fE);
        weight.position.acceleration.x = (weight.position.fE.x - fE.x) + (((vectorg4.x * -this.cfg.springConstant.x) - (weight.position.velocity.x * this.cfg.springDampen.x)) * this.cfg.weightMass);
        weight.position.acceleration.y = (weight.position.fE.y - fE.y) + (((vectorg4.y * -this.cfg.springConstant.y) - (weight.position.velocity.y * this.cfg.springDampen.y)) * this.cfg.weightMass);
        weight.position.acceleration.z = (weight.position.fE.z - fE.z) + (((vectorg4.z * -this.cfg.springConstant.z) - (weight.position.velocity.z * this.cfg.springDampen.z)) * this.cfg.weightMass);
        weight.position.velocity.x += weight.position.acceleration.x * dt;
        weight.position.velocity.y += weight.position.acceleration.y * dt;
        weight.position.velocity.z += weight.position.acceleration.z * dt;
        if (!float.IsInfinity(this.cfg.maxVelocity.x))
        {
            if (weight.position.velocity.x < -this.cfg.maxVelocity.x)
            {
                weight.position.value.x -= this.cfg.maxVelocity.x * dt;
            }
            else if (weight.position.velocity.x > this.cfg.maxVelocity.x)
            {
                weight.position.value.x += this.cfg.maxVelocity.x * dt;
            }
            else
            {
                weight.position.value.x += weight.position.velocity.x * dt;
            }
        }
        else
        {
            weight.position.value.x += weight.position.velocity.x * dt;
        }
        if (!float.IsInfinity(this.cfg.maxVelocity.y))
        {
            if (weight.position.velocity.y < -this.cfg.maxVelocity.y)
            {
                weight.position.value.y -= this.cfg.maxVelocity.y * dt;
            }
            else if (weight.position.velocity.y > this.cfg.maxVelocity.y)
            {
                weight.position.value.y += this.cfg.maxVelocity.y * dt;
            }
            else
            {
                weight.position.value.y += weight.position.velocity.y * dt;
            }
        }
        else
        {
            weight.position.value.y += weight.position.velocity.y * dt;
        }
        if (!float.IsInfinity(this.cfg.maxVelocity.z))
        {
            if (weight.position.velocity.z < -this.cfg.maxVelocity.z)
            {
                weight.position.value.z -= this.cfg.maxVelocity.z * dt;
            }
            else if (weight.position.velocity.z > this.cfg.maxVelocity.z)
            {
                weight.position.value.z += this.cfg.maxVelocity.z * dt;
            }
            else
            {
                weight.position.value.z += weight.position.velocity.z * dt;
            }
        }
        else
        {
            weight.position.value.z += weight.position.velocity.z * dt;
        }
        weight.rotation.acceleration.x = (weight.rotation.fE.x - vectorg3.x) + (((weight.rotation.value.x * -this.cfg.angularSpringConstant.x) - (weight.rotation.velocity.x * this.cfg.angularSpringDampen.x)) * this.cfg.angularWeightMass);
        weight.rotation.acceleration.y = (weight.rotation.fE.y - vectorg3.y) + (((weight.rotation.value.y * -this.cfg.angularSpringConstant.y) - (weight.rotation.velocity.y * this.cfg.angularSpringDampen.y)) * this.cfg.angularWeightMass);
        weight.rotation.acceleration.z = (weight.rotation.fE.z - vectorg3.z) + (((weight.rotation.value.z * -this.cfg.angularSpringConstant.z) - (weight.rotation.velocity.z * this.cfg.angularSpringDampen.z)) * this.cfg.angularWeightMass);
        weight.rotation.velocity.x += weight.rotation.acceleration.x * dt;
        weight.rotation.velocity.y += weight.rotation.acceleration.y * dt;
        weight.rotation.velocity.z += weight.rotation.acceleration.z * dt;
        weight.rotation.value.x += weight.rotation.velocity.x * dt;
        weight.rotation.value.y += weight.rotation.velocity.y * dt;
        weight.rotation.value.z += weight.rotation.velocity.z * dt;
    }

    private int Step(float dt)
    {
        int num = 0;
        int num2 = 0;
        this.timeSolve += dt;
        double d = (this.cfg.solveRate >= 0.0) ? (1.0 / ((double) this.cfg.solveRate)) : (1.0 / -((double) this.cfg.solveRate));
        double num4 = (this.cfg.intermitRate != 0.0) ? ((this.cfg.intermitRate >= 0.0) ? (1.0 / ((double) this.cfg.intermitRate)) : (1.0 / -((double) this.cfg.intermitRate))) : 0.0;
        if (double.IsInfinity(d) || (d == 0.0))
        {
            d = this.timeSolve;
        }
        bool flag = num4 > d;
        double num5 = d * this.cfg.timeScale;
        if (this.timeSolve >= d)
        {
            do
            {
                this.timeSolve -= d;
                if (flag)
                {
                    this.timeIntermit -= d;
                    if (this.timeIntermit < 0.0)
                    {
                        this.intermitStart = this.working;
                    }
                }
                this.Solve(ref this.working, ref num5);
                if (flag && (this.timeIntermit < 0.0))
                {
                    this.intermitNext = this.working;
                    this.intermitFraction = (this.timeIntermit + d) / d;
                    this.timeIntermit += num4;
                    num2++;
                }
                num++;
            }
            while (this.timeSolve >= d);
        }
        if (flag)
        {
            if (num2 > 0)
            {
                if (this.simStep)
                {
                    Vector3G.Lerp(ref this.intermitStart.position.value, ref this.intermitNext.position.value, ref this.intermitFraction, out this.raw_pos);
                    Vector3G.Lerp(ref this.intermitStart.rotation.value, ref this.intermitNext.rotation.value, ref this.intermitFraction, out this.raw_rot);
                    this.CheckDeadZone();
                    return num2;
                }
                this.raw_pos = this.intermitNext.position.value;
                this.raw_rot = this.intermitNext.rotation.value;
                this.CheckDeadZone();
            }
            return num2;
        }
        if (this.simStep)
        {
            this.working.CopyTo(ref this.predicted);
            this.Solve(ref this.predicted, ref num5);
            num = -(num + 1);
            double t = this.timeSolve / d;
            Vector3G.Lerp(ref this.working.position.value, ref this.predicted.position.value, ref t, out this.raw_pos);
            Vector3G.Lerp(ref this.working.rotation.value, ref this.predicted.rotation.value, ref t, out this.raw_rot);
            this.CheckDeadZone();
            return num;
        }
        this.raw_pos = this.working.position.value;
        this.raw_rot = this.working.rotation.value;
        this.CheckDeadZone();
        return num;
    }

    public float aimPositionScalar
    {
        get
        {
            return this._aimPositionScalar;
        }
        set
        {
            this._aimPositionScalar = value;
        }
    }

    public float aimRotationScalar
    {
        get
        {
            return this._aimRotationScalar;
        }
        set
        {
            this._aimRotationScalar = value;
        }
    }

    public bool allow
    {
        get
        {
            return (this._allow && base.enabled);
        }
        set
        {
            this._allow = value;
            if (value)
            {
                this._wasForbidden = false;
                base.enabled = true;
            }
        }
    }

    public float globalPositionScalar
    {
        get
        {
            return this._globalPositionScalar;
        }
    }

    public float globalRotationScalar
    {
        get
        {
            return this._globalRotationScalar;
        }
    }

    public float globalScalar
    {
        get
        {
            return this._globalScalar;
        }
    }

    private Vector3 offset
    {
        get
        {
            Vector3 vector;
            double num = this.allowValue * this.positionScalar;
            vector.x = (float) (this.raw_pos.x * num);
            vector.y = (float) (this.raw_pos.y * num);
            vector.z = (float) (this.raw_pos.z * num);
            return vector;
        }
    }

    public double positionScalar
    {
        get
        {
            return (((((bob_scale * bob_scale_linear) * this._globalScalar) * this._globalPositionScalar) * this._viewModelPositionScalar) * this._aimPositionScalar);
        }
    }

    private Vector3 rotationOffset
    {
        get
        {
            Vector3 vector;
            double num = this.allowValue * this.rotationScalar;
            vector.x = (float) (this.raw_rot.x * num);
            vector.y = (float) (this.raw_rot.y * num);
            vector.z = (float) (this.raw_rot.z * num);
            return vector;
        }
    }

    public double rotationScalar
    {
        get
        {
            return (((((bob_scale * bob_scale_angular) * this._globalScalar) * this._globalRotationScalar) * this._viewModelRotationScalar) * this._aimRotationScalar);
        }
    }

    public float viewModelPositionScalar
    {
        get
        {
            return this._viewModelPositionScalar;
        }
        set
        {
            this._viewModelPositionScalar = value;
        }
    }

    public float viewModelRotationScalar
    {
        get
        {
            return this._viewModelRotationScalar;
        }
        set
        {
            this._viewModelRotationScalar = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct VectorAccelSampler
    {
        public HeadBob.VectorStamp sample0;
        public HeadBob.VectorStamp sample1;
        public HeadBob.VectorStamp sample2;
        public Vector3G accel;
        public void Sample(ref Vector3G v, float timeStamp)
        {
            if (this.sample1.timeStamp < timeStamp)
            {
                this.sample2 = this.sample1;
            }
            if (this.sample0.timeStamp < timeStamp)
            {
                this.sample1 = this.sample0;
            }
            this.sample0.vector = v;
            this.sample0.timeStamp = timeStamp;
            this.sample0.valid = true;
            Vector3G difference = new Vector3G();
            double introduced3 = this.sample0.AddDifference(ref this.sample1, ref difference);
            double num = introduced3 + this.sample0.AddDifference(ref this.sample2, ref difference);
            if (num != 0.0)
            {
                num = 1.0 / num;
                this.accel.x = difference.x * num;
                this.accel.y = difference.y * num;
                this.accel.z = difference.z * num;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct VectorStamp
    {
        public Vector3G vector;
        public float timeStamp;
        public bool valid;
        public double AddDifference(ref HeadBob.VectorStamp previous, ref Vector3G difference)
        {
            if (previous.valid && (previous.timeStamp != this.timeStamp))
            {
                double num = 1.0 / ((double) (this.timeStamp - previous.timeStamp));
                difference.x += num * (this.vector.x - previous.vector.x);
                difference.y += num * (this.vector.y - previous.vector.y);
                difference.z += num * (this.vector.z - previous.vector.z);
                return 1.0;
            }
            return 0.0;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Weight
    {
        public Element position;
        public Element rotation;
        public Vector3G[] additionalPositions;
        public BobEffectStack stack;
        public void CopyTo(ref HeadBob.Weight other)
        {
            if ((other.additionalPositions != this.additionalPositions) && (this.additionalPositions != null))
            {
                Array.Copy(this.additionalPositions, other.additionalPositions, this.additionalPositions.Length);
            }
            other.rotation = this.rotation;
            other.position = this.position;
            if ((other.stack != null) && other.stack.IsForkOf(this.stack))
            {
                other.stack.Join();
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct Element
        {
            public Vector3G value;
            public Vector3G velocity;
            public Vector3G acceleration;
            public Vector3G fI;
            public Vector3G fE;
            public Vector3G fIV;
        }
    }
}

