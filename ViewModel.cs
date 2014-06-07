using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

public class ViewModel : IDRemote, Socket.Source, Socket.Mapped, Socket.Provider
{
    private Quaternion _additiveRotation = Quaternion.identity;
    private HeadBob _headBob;
    private LazyCam _lazyCam;
    [NonSerialized]
    private Socket.Map.Member _socketMap;
    [CompilerGenerated]
    private static Dictionary<string, int> <>f__switch$mapC;
    [CompilerGenerated]
    private static Dictionary<string, int> <>f__switch$mapD;
    public LayerMask aimMask;
    public Animation animation;
    public bool barrelAiming = true;
    public float barrelAngleMaxSpeed = float.PositiveInfinity;
    public float barrelAngleSmoothDamp = 0.01f;
    public float barrelLimit;
    public float barrelLimitOffsetFactor = 1f;
    public float barrelLimitPivotFactor;
    public Vector3 barrelPivot;
    public Vector2 barrelRotation;
    public bool barrelWhileBowing;
    public bool barrelWhileZoom;
    public bool bowAllowed;
    [SerializeField]
    protected AnimationBlender.ChannelField bowChannel;
    public AnimationCurve bowCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    public bool bowCurveIs01Fraction;
    public float bowEnterDuration = 1f;
    public float bowExitDuration = 1f;
    [SerializeField]
    protected AnimationBlender.ChannelField bowMovementChannel;
    public Vector3 bowOffsetAngles;
    public Vector3 bowOffsetPoint;
    [SerializeField]
    public Socket.CameraSpace bowPivot;
    private float bowTime;
    [NonSerialized]
    private BarrelParameters bpBow;
    [NonSerialized]
    private BarrelParameters bpHip;
    [NonSerialized]
    private BarrelParameters bpZoom;
    [SerializeField]
    private SkinnedMeshRenderer[] builtinRenderers;
    public int caps;
    public Color crosshairColor = Color.white;
    public Color crosshairOutline = Color.black;
    public Texture crosshairTexture;
    [SerializeField]
    protected AnimationBlender.ChannelField crouchChannel;
    [SerializeField]
    protected AnimationBlender.ChannelField crouchMovementChannel;
    protected static readonly string[] defaultSocketNames = new string[] { "muzzle", "sight", "optics", "pivot1", "pivot2", "bowPivot" };
    public string deployAnimName = "deploy";
    private List<GameObject> destroyOnUnbind;
    public Texture dotTexture;
    private Transform eye;
    [SerializeField]
    protected AnimationBlender.ChannelField fallChannel;
    public string fireAnimName = "fire_1";
    public float fireAnimScaleSpeed = 1f;
    private bool flipped;
    private static bool force_legacy_fallback;
    private float headBobAngularTime;
    private float headBobLinearTime;
    public AnimationCurve headBobOffsetScale;
    public AnimationCurve headBobRotationScale;
    [SerializeField]
    protected AnimationBlender.ChannelField idleChannel;
    [SerializeField]
    protected AnimationBlender.ResidualField idleFrame;
    [NonSerialized]
    protected AnimationBlender.Mixer idleMixer;
    [NonSerialized]
    public IHeldItem item;
    [NonSerialized]
    public ItemRepresentation itemRep;
    public const int kCap_PerspectiveAspect = 8;
    public const int kCap_PerspectiveFar = 2;
    public const int kCap_PerspectiveFOV = 4;
    public const int kCap_PerspectiveNear = 1;
    protected const int kIdleChannel_Bow = 4;
    protected const string kIdleChannel_Bow_Name = "bowi";
    protected const int kIdleChannel_BowMovement = 5;
    protected const string kIdleChannel_BowMovement_Name = "bowm";
    protected const int kIdleChannel_Crouch = 2;
    protected const string kIdleChannel_Crouch_Name = "dcki";
    protected const int kIdleChannel_CrouchMovement = 3;
    protected const string kIdleChannel_CrouchMovement_Name = "dckm";
    protected const int kIdleChannel_Fall = 6;
    protected const string kIdleChannel_Fall_Name = "fall";
    protected const int kIdleChannel_Idle = 0;
    protected const string kIdleChannel_Idle_Name = "idle";
    protected const int kIdleChannel_IdleMovement = 1;
    protected const string kIdleChannel_IdleMovement_Name = "move";
    protected const int kIdleChannel_Slip = 7;
    protected const string kIdleChannel_Slip_Name = "slip";
    protected const int kIdleChannel_Zoom = 8;
    protected const string kIdleChannel_Zoom_Name = "zoom";
    protected const int kIdleChannelCount = 9;
    private float lastHeadBobAngular;
    private float lastHeadBobLinearFraction;
    private Vector3 lastLocalPositionOffset;
    private Vector3 lastLocalRotationOffset;
    [NonSerialized]
    private Angle2 lastLook;
    private Vector3 lastSightRotation;
    private float lastZoomFraction = float.NaN;
    public float lazyAngle = 5f;
    private bool madeProxyDict;
    [NonSerialized]
    private MeshInstance.Holder meshInstances;
    private static bool modifyAiming;
    [SerializeField]
    protected AnimationBlender.ChannelField movementIdleChannel;
    [SerializeField]
    public Socket.CameraSpace muzzle;
    public float noHitPlane = 20f;
    public Vector3 offset;
    [SerializeField]
    public Socket.CameraSpace optics;
    private Vector3 originalRootOffset;
    private Quaternion originalRootRotation;
    public float perspectiveAspectOverride = 1f;
    public float perspectiveFarOverride = 25f;
    public float perspectiveFOVOverride = 60f;
    public float perspectiveNearOverride = 0.1f;
    [SerializeField]
    public Socket.CameraSpace pivot;
    [SerializeField]
    public Socket.CameraSpace pivot2;
    private Dictionary<Socket, Transform> proxies;
    public float punchScalar = 1f;
    private float punchTime = -2000f;
    public string reloadAnimName = "reload";
    public Transform root;
    public Vector3 rotate;
    private Transform shelf;
    public bool showCrosshairNotZoomed = true;
    public bool showCrosshairZoom;
    [SerializeField]
    public Socket.CameraSpace sight;
    [SerializeField]
    protected AnimationBlender.ChannelField slipChannel;
    [NonSerialized]
    protected IEnumerable<string> socketNames = defaultSocketNames;
    [NonSerialized]
    protected int socketVersion;
    [SerializeField]
    protected AnimationBlender.ChannelField zoomChannel;
    public AnimationCurve zoomCurve;
    public float zoomFieldOfView = 40f;
    public float zoomInDuration = 0.5f;
    public Vector3 zoomOffset;
    public float zoomOutDuration = 0.4f;
    public AnimationCurve zoomPunch;
    private float zoomPunchValue;
    public Vector3 zoomRotate;
    private float zoomTime;

    public void AddRenderers(SkinnedMeshRenderer[] renderers)
    {
        if (renderers != null)
        {
            foreach (SkinnedMeshRenderer renderer in renderers)
            {
                this.meshInstances.Add(renderer);
            }
        }
    }

    protected void Awake()
    {
        this.originalRootOffset = this.root.localPosition;
        this.originalRootRotation = this.root.localRotation;
        base.Awake();
        if (this.builtinRenderers != null)
        {
            foreach (SkinnedMeshRenderer renderer in this.builtinRenderers)
            {
                if (renderer != null)
                {
                    this.meshInstances.Add(renderer);
                }
            }
        }
        this.idleMixer = this.idleFrame.Alias(this.animation, new AnimationBlender.ChannelConfig[9].Define(0, "idle", this.idleChannel).Define(1, "move", this.movementIdleChannel).Define(4, "bowi", this.bowChannel).Define(5, "bowm", this.bowMovementChannel).Define(2, "dcki", this.crouchChannel).Define(3, "dckm", this.crouchMovementChannel).Define(6, "fall", this.fallChannel).Define(7, "slip", this.slipChannel).Define(8, "zoom", this.zoomChannel)).Create();
    }

    private BarrelTransform BarrelAim(Vector3 offset, ref BarrelParameters barrel)
    {
        float magnitude;
        Vector3 point;
        RaycastHit2 hit;
        float num2;
        Vector3 barrelPivot;
        Vector3 vector5;
        BarrelTransform transform;
        Ray eyesRay = this.idMain.eyesRay;
        if (Physics2.Raycast2(eyesRay, out hit, this.noHitPlane, this.aimMask.value))
        {
            magnitude = hit.distance;
            point = hit.point;
        }
        else
        {
            magnitude = this.noHitPlane;
            point = eyesRay.GetPoint(this.noHitPlane);
        }
        point = this.idMain.eyesTransformReadOnly.InverseTransformPoint(point);
        magnitude = point.magnitude;
        Vector3 inPoint = Vector3.Scale(offset + this.barrelPivot, base.transform.localScale);
        Plane plane = new Plane(this.idMain.eyesTransformReadOnly.InverseTransformDirection(eyesRay.direction), inPoint);
        Ray ray = new Ray(point, -point);
        plane.Raycast(ray, out num2);
        float num3 = Vector3.Distance(ray.GetPoint(num2), inPoint);
        float b = Vector3.Distance(point, inPoint);
        if (Mathf.Approximately(0f, b) && barrel.ir)
        {
            barrel.ir = false;
        }
        barrel.bc = b;
        barrel.ca = num3;
        barrel.a = 90f;
        SolveTriangleSSA(barrel.a, barrel.bc, barrel.ca, out barrel.ab, out barrel.c, out barrel.b);
        barrel.ir = true;
        float target = -(90f - barrel.c);
        if (!barrel.once)
        {
            barrel.once = true;
            barrel.angle = target;
        }
        else if (this.barrelAngleSmoothDamp <= 0f)
        {
            if ((this.barrelAngleMaxSpeed <= 0f) || (this.barrelAngleMaxSpeed == float.PositiveInfinity))
            {
                barrel.angle = target;
            }
            else
            {
                barrel.angle = Mathf.MoveTowardsAngle(barrel.angle, target, this.barrelAngleMaxSpeed * Time.deltaTime);
            }
        }
        else if (this.barrelAngleMaxSpeed <= 0f)
        {
            barrel.angle = Mathf.SmoothDampAngle(barrel.angle, target, ref barrel.angularVelocity, this.barrelAngleSmoothDamp);
        }
        else
        {
            barrel.angle = Mathf.SmoothDampAngle(barrel.angle, target, ref barrel.angularVelocity, this.barrelAngleSmoothDamp, this.barrelAngleMaxSpeed);
        }
        Quaternion rotation = Quaternion.Euler(-this.barrelRotation.x, this.barrelRotation.y, 0f);
        Plane plane2 = new Plane(point, inPoint, Vector3.zero);
        Quaternion quaternion2 = Quaternion.Inverse(rotation) * Quaternion.AngleAxis(barrel.angle, plane2.normal);
        if (barrel.bc < this.barrelLimit)
        {
            if (this.barrelLimitOffsetFactor != 0f)
            {
                vector5 = offset - ((Vector3) (quaternion2 * (Vector3.forward * ((this.barrelLimit - barrel.bc) * this.barrelLimitOffsetFactor))));
            }
            else
            {
                vector5 = offset;
            }
            if (this.barrelLimitPivotFactor != 0f)
            {
                barrelPivot = this.barrelPivot + ((Vector3) (rotation * (Vector3.back * ((this.barrelLimit - barrel.bc) * this.barrelLimitPivotFactor))));
            }
            else
            {
                barrelPivot = this.barrelPivot;
            }
        }
        else
        {
            barrelPivot = this.barrelPivot;
            vector5 = offset;
        }
        transform.origin = ((Vector3) (quaternion2 * -barrelPivot)) + barrelPivot;
        transform.angles = quaternion2.eulerAngles;
        transform.origin += vector5;
        transform.angles.x = Mathf.DeltaAngle(0f, transform.angles.x);
        transform.angles.y = Mathf.DeltaAngle(0f, transform.angles.y);
        transform.angles.z = Mathf.DeltaAngle(0f, transform.angles.z);
        return transform;
    }

    protected void BindCameraSpaceTransforms(Transform newShelf, Transform newEye)
    {
        Transform eye = this.eye;
        Transform shelf = this.shelf;
        this.eye = newEye;
        this.shelf = newShelf;
        if ((eye != newEye) || (shelf != newShelf))
        {
            this.socketVersion++;
        }
    }

    public void BindTransforms(Transform shelf, Transform eye)
    {
        this.punchTime = Time.time - 20f;
        this.BindCameraSpaceTransforms(shelf, eye);
    }

    private void ClearProxies()
    {
        this.DeleteSocketMap();
        if (this.destroyOnUnbind != null)
        {
            foreach (GameObject obj2 in this.destroyOnUnbind)
            {
                if (obj2 != null)
                {
                    Object.Destroy(obj2);
                }
            }
        }
        this.destroyOnUnbind = null;
    }

    public void CrossFade(string name)
    {
        this.idleMixer.CrossFade(name);
    }

    public void CrossFade(string name, float fadeLength)
    {
        this.idleMixer.CrossFade(name, fadeLength);
    }

    public void CrossFade(string name, float fadeLength, PlayMode playMode)
    {
        this.idleMixer.CrossFade(name, fadeLength, playMode);
    }

    public void CrossFade(string name, float fadeLength, PlayMode playMode, float speed)
    {
        this.idleMixer.CrossFade(name, fadeLength, playMode, speed);
    }

    protected void DeleteSocketMap()
    {
        this._socketMap.DeleteBy<ViewModel>(this);
    }

    private void DrawShadowed(ref Rect r, Texture texture)
    {
        Color color = GUI.color;
        if (color.a > 0.5f)
        {
            Color color2;
            Rect position = r;
            position.x++;
            position.y--;
            color2.a = (color.a - 0.5f) * 2f;
            color2.a = this.crosshairOutline.a * (color2.a * color2.a);
            color2.r = this.crosshairOutline.r;
            color2.g = this.crosshairOutline.g;
            color2.b = this.crosshairOutline.b;
            GUI.color = color2;
            GUI.DrawTexture(position, texture);
            position.x -= 2f;
            GUI.DrawTexture(position, texture);
            position.y += 2f;
            GUI.DrawTexture(position, texture);
            position.x += 2f;
            GUI.DrawTexture(position, texture);
            float num = 1f - color2.a;
            color2.r = (this.crosshairColor.r * color2.a) + (this.crosshairOutline.r * num);
            color2.g = (this.crosshairColor.g * color2.a) + (this.crosshairOutline.g * num);
            color2.b = (this.crosshairColor.b * color2.a) + (this.crosshairOutline.b * num);
            color2.a = (this.crosshairColor.a * color2.a) + (this.crosshairOutline.a * num);
            GUI.color = color2;
            GUI.DrawTexture(r, texture);
        }
        else if (color.a > 0f)
        {
            float num2 = color.a * 2f;
            float num3 = num2 + (num2 - (num2 * num2));
            float num4 = 1f - num3;
            GUI.color = new Color((this.crosshairOutline.r * num3) + (this.crosshairColor.r * num4), (this.crosshairOutline.g * num3) + (this.crosshairColor.g * num4), (this.crosshairOutline.b * num3) + (this.crosshairColor.b * num4), this.crosshairOutline.a * (num2 * num2));
            GUI.DrawTexture(r, texture);
        }
        GUI.color = color;
    }

    public void Flip()
    {
        if (!this.flipped)
        {
            Vector3 localScale = base.transform.localScale;
            localScale.z = -localScale.z;
            base.transform.localScale = localScale;
            this.flipped = true;
        }
    }

    protected void LateUpdate()
    {
        Character idMain = this.idMain;
        if (idMain != null)
        {
            bool flag;
            bool flag2;
            bool movement;
            bool crouch;
            bool flag5;
            bool slipping;
            bool aim;
            Angle2 eyesAngles;
            float num2;
            float num6;
            float num7;
            Vector3 vector3;
            Vector3 vector4;
            Vector3 offset;
            Vector3 rotate;
            float deltaTime = Time.deltaTime;
            if (idMain != null)
            {
                aim = idMain.stateFlags.aim;
                flag5 = !idMain.stateFlags.grounded;
                slipping = idMain.stateFlags.slipping;
                movement = idMain.stateFlags.movement;
                flag = idMain.stateFlags.aim;
                crouch = idMain.stateFlags.crouch;
                eyesAngles = idMain.eyesAngles;
                flag2 = this.bowAllowed && (idMain.stateFlags.sprint && movement);
            }
            else
            {
                aim = false;
                flag5 = false;
                slipping = false;
                crouch = false;
                movement = false;
                flag = false;
                flag2 = false;
                eyesAngles = this.lastLook;
            }
            if (eyesAngles == this.lastLook)
            {
                num2 = 0f;
            }
            else
            {
                num2 = Angle2.AngleDistance(this.lastLook, eyesAngles) / deltaTime;
                this.lastLook = eyesAngles;
            }
            if (flag5)
            {
                this.idleMixer.SetSolo(6);
            }
            else if (slipping)
            {
                this.idleMixer.SetSolo(7);
            }
            else if (aim)
            {
                this.idleMixer.SetSolo(8);
            }
            else if (flag2)
            {
                if (movement)
                {
                    this.idleMixer.SetSolo(5);
                }
                else
                {
                    this.idleMixer.SetSolo(4);
                }
            }
            else if (crouch)
            {
                if (movement)
                {
                    this.idleMixer.SetSolo(3);
                }
                else
                {
                    this.idleMixer.SetSolo(2);
                }
            }
            else if (movement)
            {
                this.idleMixer.SetSolo(1);
            }
            else
            {
                this.idleMixer.SetSolo(0);
                if ((num2 < -2f) || (num2 > 2f))
                {
                    this.idleMixer.SetActive(0, false);
                }
            }
            float advance = Time.deltaTime / (!flag ? -this.zoomOutDuration : this.zoomInDuration);
            float fraction = this.zoomCurve.EvaluateClampedTime(ref this.zoomTime, advance);
            float f = Time.deltaTime / (!flag2 ? -this.bowExitDuration : this.bowEnterDuration);
            if (float.IsInfinity(f))
            {
                num6 = !flag2 ? 0f : 1f;
            }
            else
            {
                if (this.bowCurveIs01Fraction)
                {
                    Keyframe keyframe = this.bowCurve[0];
                    Keyframe keyframe2 = this.bowCurve[this.bowCurve.length];
                    f *= keyframe.time - keyframe2.time;
                }
                num6 = this.bowCurve.EvaluateClampedTime(ref this.bowTime, f);
            }
            if (flag2 == flag)
            {
                if (this.bowAllowed)
                {
                    if (flag2)
                    {
                        num7 = Mathf.Max(num6, advance);
                    }
                    else
                    {
                        num7 = Mathf.Min(num6, advance);
                    }
                }
                else
                {
                    num7 = advance;
                }
            }
            else if (flag || !this.bowAllowed)
            {
                num7 = advance;
            }
            else
            {
                num7 = f;
            }
            this.root.localPosition = this.originalRootOffset;
            this.root.localRotation = this.originalRootRotation;
            Vector3 preEyePosition = this.sight.preEyePosition;
            Vector3 position = this.bowPivot.preEyePosition;
            Vector3 origin = -this.root.InverseTransformPoint(preEyePosition);
            Vector3 vector7 = -this.root.InverseTransformPoint(position);
            Quaternion preEyeRotation = this.sight.preEyeRotation;
            Vector3 forward = this.root.InverseTransformDirection((Vector3) (preEyeRotation * Vector3.forward));
            Vector3 upwards = this.root.InverseTransformDirection((Vector3) (preEyeRotation * Vector3.up));
            preEyeRotation = Quaternion.Inverse(Quaternion.LookRotation(forward, upwards));
            origin = (Vector3) (preEyeRotation * origin);
            Vector3 eulerAngles = preEyeRotation.eulerAngles;
            Quaternion quaternion2 = this.bowPivot.preEyeRotation;
            Vector3 vector13 = this.root.InverseTransformPoint((Vector3) (quaternion2 * Vector3.forward));
            Vector3 vector14 = this.root.InverseTransformDirection((Vector3) (quaternion2 * Vector3.up));
            quaternion2 = Quaternion.Inverse(Quaternion.LookRotation(vector13, vector14));
            vector7 = (Vector3) (quaternion2 * vector7);
            Vector3 angles = quaternion2.eulerAngles;
            if (this.barrelAiming)
            {
                this.BarrelAim(this.offset, ref this.bpHip).Get(out offset, out rotate);
            }
            else
            {
                offset = this.offset;
                rotate = this.rotate;
            }
            if (this.barrelWhileZoom)
            {
                this.BarrelAim(origin, ref this.bpZoom).Get(out origin, out eulerAngles);
            }
            if (this.barrelWhileBowing)
            {
                this.BarrelAim(vector7, ref this.bpBow).Get(out vector7, out angles);
            }
            float num8 = 1f - fraction;
            float num9 = this.zoomPunch.Evaluate(Time.time - this.punchTime) * this.punchScalar;
            float num10 = 1f - num6;
            vector3.x = ((vector7.x + this.bowOffsetPoint.x) * num6) + ((((origin.x + this.zoomOffset.x) * fraction) + (offset.x * num8)) * num10);
            vector3.y = ((vector7.y + this.bowOffsetPoint.y) * num6) + (((origin.y + this.zoomOffset.y) * fraction) + ((offset.y * num8) * num10));
            vector3.z = ((vector7.z + this.bowOffsetPoint.z) * num6) + ((((origin.z + (this.zoomOffset.z - num9)) * fraction) + (offset.z * num8)) * num10);
            float introduced41 = Mathf.DeltaAngle(this.zoomRotate.x, eulerAngles.x);
            float introduced42 = Mathf.DeltaAngle(this.bowOffsetAngles.x, angles.x);
            vector4.x = Mathf.DeltaAngle(0f, ((((introduced41 + this.zoomRotate.x) * fraction) + (rotate.x * num8)) * num10) + ((introduced42 + this.bowOffsetAngles.x) * num6));
            float introduced43 = Mathf.DeltaAngle(this.zoomRotate.y, eulerAngles.y);
            float introduced44 = Mathf.DeltaAngle(this.bowOffsetAngles.y, angles.y);
            vector4.y = Mathf.DeltaAngle(0f, ((((introduced43 + this.zoomRotate.y) * fraction) + (rotate.y * num8)) * num10) + ((introduced44 + this.bowOffsetAngles.y) * num6));
            float introduced45 = Mathf.DeltaAngle(this.zoomRotate.z, eulerAngles.z);
            float introduced46 = Mathf.DeltaAngle(this.bowOffsetAngles.z, angles.z);
            vector4.z = Mathf.DeltaAngle(0f, ((((introduced45 + this.zoomRotate.z) * fraction) + (rotate.z * num8)) * num10) + ((introduced46 + this.bowOffsetAngles.z) * num6));
            this.lastLocalPositionOffset = vector3;
            this.lastLocalRotationOffset = vector4;
            this.root.localEulerAngles += this.lastLocalRotationOffset;
            this.root.localPosition += this.lastLocalPositionOffset;
            this.lastZoomFraction = fraction;
            if (this._headBob != null)
            {
                CameraFX mainCameraFX = CameraFX.mainCameraFX;
                if (mainCameraFX != null)
                {
                    mainCameraFX.SetFieldOfView(this.zoomFieldOfView, fraction);
                }
                else
                {
                    Debug.Log("No CamFX");
                }
            }
            this.pivot.Rotate(this._additiveRotation);
            this.pivot2.UnRotate(this._additiveRotation);
            if (this._lazyCam != null)
            {
                this._lazyCam.allow = !(flag || flag2);
            }
            if (this._headBob != null)
            {
                this._headBob.viewModelPositionScalar = this.headBobOffsetScale.EvaluateClampedTime(ref this.headBobLinearTime, advance);
                this._headBob.viewModelRotationScalar = this.headBobRotationScale.EvaluateClampedTime(ref this.headBobAngularTime, advance);
            }
        }
    }

    public void ModifyAiming(Ray ray, ref Vector3 p, ref Quaternion q)
    {
        if (modifyAiming)
        {
            float distance;
            RaycastHit2 hit;
            if (Physics2.Raycast2(ray, out hit, this.noHitPlane))
            {
                distance = hit.distance;
            }
            else
            {
                distance = this.noHitPlane;
            }
            Vector3 from = this.shelf.InverseTransformPoint((Vector3) ((this.pivot.position + this.pivot2.position) / 2f));
            Vector3 vector2 = this.shelf.InverseTransformPoint(ray.GetPoint(distance));
            float num2 = Vector3.Angle(from, this.shelf.InverseTransformPoint(ray.origin));
            float y = distance * Mathf.Cos(num2 * 0.01745329f);
            float f = Mathf.Atan2(y, vector2.magnitude);
            q *= new Quaternion(0f, Mathf.Sin(f), 0f, Mathf.Cos(f));
        }
    }

    public void ModifyPerspective(ref PerspectiveMatrixBuilder perspective)
    {
        if ((this.caps & 1) == 1)
        {
            perspective.nearPlane = this.perspectiveNearOverride;
        }
        if ((this.caps & 2) == 2)
        {
            perspective.farPlane = this.perspectiveFarOverride;
        }
        if ((this.caps & 4) == 4)
        {
            perspective.fieldOfView = this.perspectiveFOVOverride;
        }
        if ((this.caps & 8) == 8)
        {
            perspective.aspectRatio = this.perspectiveAspectOverride;
        }
    }

    protected void OnDestroy()
    {
        base.OnDestroy();
        this.UnBindTransforms();
        this.meshInstances.Dispose();
    }

    private void OnDrawGizmosSelected()
    {
        if (this.root != null)
        {
            this.pivot.DrawGizmos("pivot1");
            this.pivot2.DrawGizmos("pivot2");
            this.muzzle.DrawGizmos("muzzle");
            this.sight.DrawGizmos("sights");
            this.bowPivot.DrawGizmos("bow");
            Gizmos.matrix = this.root.localToWorldMatrix;
            Vector3 vector = Angle2.Direction(this.barrelRotation.x, this.barrelRotation.y);
            Gizmos.DrawSphere(this.barrelPivot, 0.001f);
            Gizmos.DrawLine(this.barrelPivot, this.barrelPivot + vector);
            Gizmos.matrix *= Matrix4x4.TRS(this.barrelPivot, Quaternion.Euler(-this.barrelRotation.x, this.barrelRotation.y, 0f), Vector3.one);
            Gizmos.DrawWireCube((Vector3) (Vector3.forward * (this.barrelLimit * 0.5f)), new Vector3(0.02f, 0.02f, this.barrelLimit));
            float a = this.bpHip.a;
            float b = this.bpHip.b;
            float c = this.bpHip.c;
            float ab = this.bpHip.ab;
            float bc = this.bpHip.bc;
            float ca = this.bpHip.ca;
            Quaternion quaternion = Quaternion.Euler(0f, 0f, a);
            Quaternion quaternion2 = Quaternion.Euler(0f, 0f, a + b);
            Quaternion quaternion3 = Quaternion.Euler(0f, 0f, (a + b) + c);
            Vector3 point = (Vector3) (quaternion * (Vector3.up * ab));
            Vector3 vector3 = point + ((Vector3) (quaternion2 * (Vector3.up * bc)));
            Vector3 vector4 = vector3 + ((Vector3) (quaternion3 * (Vector3.up * ca)));
            Bounds bounds = new Bounds();
            bounds.Encapsulate(vector4);
            bounds.Encapsulate(point);
            bounds.Encapsulate(vector3);
            Gizmos.matrix = Matrix4x4.TRS(-vector3, Quaternion.identity, Vector3.one);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(vector4, 0.01f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(vector4, point);
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(point, 0.01f);
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(point, vector3);
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(vector3, 0.01f);
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(vector3, vector4);
            Gizmos.color = Color.black;
            float num7 = ca;
            float f = a * 0.01745329f;
            float num9 = num7 * Mathf.Sin(f);
            float num10 = Mathf.Sqrt((num9 * num9) + (num7 * num7));
            Gizmos.color = Color.black;
            Vector3 from = (Vector3) (quaternion2 * (Vector3.up * num9));
            Gizmos.DrawLine(from, vector3);
            Gizmos.color = Color.gray;
            from = (Vector3) (quaternion * (Vector3.up * num10));
            Gizmos.DrawLine(from, vector3);
        }
    }

    private void OnGUI()
    {
        if (((Event.current.type == EventType.Repaint) && !RPOS.IsOpen) && ((this.drawCrosshair && (this.crosshairTexture != null)) && (this.dotTexture != null)))
        {
            Camera camera;
            if (this._headBob != null)
            {
                camera = this._headBob.camera;
            }
            else if (this._lazyCam != null)
            {
                camera = this._lazyCam.camera;
            }
            else
            {
                return;
            }
            if ((camera != null) && (camera.enabled || MountedCamera.IsCameraBeingUsed(camera)))
            {
                Color color;
                color.r = 1f;
                color.g = 1f;
                color.b = 1f;
                if (this.showCrosshairNotZoomed)
                {
                    if (this.showCrosshairZoom)
                    {
                        color.a = 1f;
                    }
                    else
                    {
                        color.a = Mathf.Clamp01(1f - this.lastZoomFraction);
                    }
                }
                else if (this.showCrosshairZoom)
                {
                    color.a = this.lastZoomFraction;
                }
                else
                {
                    color.a = 1f;
                }
                if (color.a != 0f)
                {
                    float num;
                    RaycastHit2 hit;
                    GUI.color = color;
                    Ray ray = camera.ViewportPointToRay((Vector3) (Vector3.one * 0.5f));
                    new Plane(-camera.transform.forward, camera.transform.position + ((Vector3) (camera.transform.forward * this.noHitPlane))).Raycast(ray, out num);
                    Vector3? nullable = CameraFX.World2Screen(ray.GetPoint(num));
                    if (nullable.HasValue)
                    {
                        Vector3 vector2 = nullable.Value;
                        vector2.y = Screen.height - (vector2.y + 1f);
                        Rect r = new Rect(vector2.x - (((float) this.crosshairTexture.width) / 2f), vector2.y - (((float) this.crosshairTexture.height) / 2f), (float) this.crosshairTexture.width, (float) this.crosshairTexture.height);
                        this.DrawShadowed(ref r, this.crosshairTexture);
                    }
                    if (Physics2.Raycast2(ray, out hit))
                    {
                        nullable = CameraFX.World2Screen(hit.point);
                        if (nullable.HasValue)
                        {
                            Vector3 vector3 = nullable.Value;
                            vector3.y = Screen.height - (vector3.y + 1f);
                            Rect rect2 = new Rect(vector3.x - (((float) this.dotTexture.width) / 2f), vector3.y - (((float) this.dotTexture.height) / 2f), (float) this.dotTexture.width, (float) this.dotTexture.height);
                            this.DrawShadowed(ref rect2, this.dotTexture);
                        }
                    }
                }
            }
        }
    }

    public bool Play(string name)
    {
        return this.idleMixer.Play(name);
    }

    public bool Play(string name, float speed)
    {
        return this.idleMixer.Play(name, speed);
    }

    public bool Play(string name, PlayMode playMode)
    {
        return this.idleMixer.Play(name, playMode);
    }

    public bool Play(string name, float speed, float time)
    {
        return this.idleMixer.Play(name, speed, time);
    }

    public bool Play(string name, PlayMode playMode, float speed)
    {
        return this.idleMixer.Play(name, playMode, speed);
    }

    public bool Play(string name, PlayMode playMode, float speed, float time)
    {
        return this.idleMixer.Play(name, playMode, speed, time);
    }

    public void PlayDeployAnimation()
    {
        this.Play(this.deployAnimName);
    }

    public void PlayFireAnimation()
    {
        this.PlayFireAnimation(this.fireAnimScaleSpeed);
    }

    public void PlayFireAnimation(float speed)
    {
        this.Play(this.fireAnimName, speed);
        this.punchTime = Time.time;
    }

    public bool PlayQueued(string name)
    {
        return this.idleMixer.PlayQueued(name);
    }

    public bool PlayQueued(string name, QueueMode queueMode)
    {
        return this.idleMixer.PlayQueued(name, queueMode);
    }

    public bool PlayQueued(string name, QueueMode queueMode, PlayMode playMode)
    {
        return this.idleMixer.PlayQueued(name, queueMode, playMode);
    }

    public void PlayReloadAnimation()
    {
        this.Play(this.reloadAnimName);
    }

    public void RemoveRenderers(SkinnedMeshRenderer[] renderers)
    {
        if (renderers != null)
        {
            foreach (SkinnedMeshRenderer renderer in renderers)
            {
                this.meshInstances.Delete(renderer);
            }
        }
    }

    [ContextMenu("Set as current view model")]
    private void SetAsCurrentViewModel()
    {
        if (base.enabled)
        {
            CameraFX.ReplaceViewModel(this, this.itemRep, this.item, false);
        }
    }

    Socket.CameraConversion Socket.Source.CameraSpaceSetup()
    {
        return new Socket.CameraConversion(this.eye, this.shelf);
    }

    bool Socket.Source.GetSocket(string name, out Socket socket)
    {
        string key = name;
        if (key != null)
        {
            int num;
            if (<>f__switch$mapD == null)
            {
                Dictionary<string, int> dictionary = new Dictionary<string, int>(6);
                dictionary.Add("muzzle", 0);
                dictionary.Add("sight", 1);
                dictionary.Add("optics", 2);
                dictionary.Add("pivot1", 3);
                dictionary.Add("pivot2", 4);
                dictionary.Add("bowPivot", 5);
                <>f__switch$mapD = dictionary;
            }
            if (<>f__switch$mapD.TryGetValue(key, out num))
            {
                switch (num)
                {
                    case 0:
                        socket = this.muzzle;
                        return true;

                    case 1:
                        socket = this.sight;
                        return true;

                    case 2:
                        socket = this.optics;
                        return true;

                    case 3:
                        socket = this.pivot;
                        return true;

                    case 4:
                        socket = this.pivot2;
                        return true;

                    case 5:
                        socket = this.bowPivot;
                        return true;
                }
            }
        }
        socket = null;
        return false;
    }

    Type Socket.Source.ProxyScriptType(string name)
    {
        return typeof(SocketProxy);
    }

    bool Socket.Source.ReplaceSocket(string name, Socket socket)
    {
        Socket.CameraSpace space = (Socket.CameraSpace) socket;
        string key = name;
        if (key != null)
        {
            int num;
            if (<>f__switch$mapC == null)
            {
                Dictionary<string, int> dictionary = new Dictionary<string, int>(6);
                dictionary.Add("muzzle", 0);
                dictionary.Add("sight", 1);
                dictionary.Add("optics", 2);
                dictionary.Add("pivot1", 3);
                dictionary.Add("pivot2", 4);
                dictionary.Add("bowPivot", 5);
                <>f__switch$mapC = dictionary;
            }
            if (<>f__switch$mapC.TryGetValue(key, out num))
            {
                switch (num)
                {
                    case 0:
                        this.muzzle = space;
                        return true;

                    case 1:
                        this.sight = space;
                        return true;

                    case 2:
                        this.optics = space;
                        return true;

                    case 3:
                        this.pivot = space;
                        return true;

                    case 4:
                        this.pivot2 = space;
                        return true;

                    case 5:
                        this.bowPivot = space;
                        return true;
                }
            }
        }
        return false;
    }

    private static void SolveTriangleSAS(float angleA, float lengthB, float lengthC, out float lengthA, out float angleB, out float angleC)
    {
        lengthA = Mathf.Sqrt(((lengthB * lengthB) + (lengthC * lengthC)) - (((2f * lengthB) * lengthC) * Mathf.Cos(angleA * 0.01745329f)));
        if ((angleA >= 90f) || (lengthB < lengthC))
        {
            angleB = Mathf.Asin((Mathf.Sin(angleA * 0.01745329f) * lengthB) / lengthA) * 57.29578f;
            angleC = 180f - (angleA + angleB);
        }
        else
        {
            angleC = Mathf.Asin((Mathf.Sin(angleA * 0.01745329f) * lengthC) / lengthA) * 57.29578f;
            angleB = 180f - (angleA + angleC);
        }
    }

    private static void SolveTriangleSSA(float angleB, float lengthB, float lengthC, out float lengthA, out float angleA, out float angleC)
    {
        float num = Mathf.Sin(angleB * 0.01745329f);
        angleC = Mathf.Asin((num * lengthC) / lengthB) * 57.29578f;
        angleA = (180f - angleC) - angleB;
        if ((angleA < 0f) || (angleA > 180f))
        {
            angleA += 180f;
        }
        lengthA = (Mathf.Sin(angleA * 0.01745329f) * lengthB) / num;
    }

    public void UnBindTransforms()
    {
        this.ClearProxies();
        if (CameraFX.mainViewModel == this)
        {
            CameraFX mainCameraFX = CameraFX.mainCameraFX;
            if (mainCameraFX != null)
            {
                mainCameraFX.SetFieldOfView(320432f, 0f);
            }
        }
    }

    protected void Update()
    {
        this.idleMixer.Update(1f, Time.deltaTime);
    }

    public void UpdateProxies()
    {
        Socket.Map socketMap = this.socketMap;
        if (!object.ReferenceEquals(socketMap, null))
        {
            socketMap.SnapProxies();
        }
    }

    public bool drawCrosshair
    {
        get
        {
            return (this.showCrosshairZoom || this.showCrosshairNotZoomed);
        }
    }

    public HeadBob headBob
    {
        get
        {
            return this._headBob;
        }
        set
        {
            this._headBob = value;
        }
    }

    public Character idMain
    {
        get
        {
            return (Character) base.idMain;
        }
    }

    public LazyCam lazyCam
    {
        get
        {
            return this._lazyCam;
        }
        set
        {
            this._lazyCam = value;
        }
    }

    public Quaternion lazyRotation
    {
        get
        {
            return this._additiveRotation;
        }
        set
        {
            if (this._additiveRotation != value)
            {
                this.pivot2.Rotate(this._additiveRotation);
                this.pivot.UnRotate(this._additiveRotation);
                this.pivot.Rotate(value);
                this.pivot2.UnRotate(value);
                this._additiveRotation = value;
            }
        }
    }

    public Vector3 muzzlePosition
    {
        get
        {
            return this.muzzle.position;
        }
    }

    public Quaternion muzzleRotation
    {
        get
        {
            return this.muzzle.rotation;
        }
    }

    IEnumerable<string> Socket.Source.SocketNames
    {
        get
        {
            return this.socketNames;
        }
    }

    int Socket.Source.SocketsVersion
    {
        get
        {
            return this.socketVersion;
        }
    }

    public Socket.Map socketMap
    {
        get
        {
            return this._socketMap.Get<ViewModel>(this);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct BarrelParameters
    {
        public float a;
        public float b;
        public float c;
        public float bc;
        public float ca;
        public float ab;
        public bool once;
        public bool ir;
        public float angle;
        public float angularVelocity;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct BarrelTransform
    {
        public Vector3 origin;
        public Vector3 angles;
        public void Get(out Vector3 origin, out Vector3 angles)
        {
            origin = this.origin;
            angles = this.angles;
        }
    }

    private class MeshInstance
    {
        public bool disposed;
        private static ViewModel.MeshInstance dump;
        private static int dumpCount;
        public bool hasNext;
        private const int kMaxDumpCount = 8;
        public bool legacy;
        private Material[] modifiedMaterials;
        public ViewModel.MeshInstance next;
        private Material[] originalMaterials;
        public ReplacementRenderer postdraw;
        public ReplacementRenderer predraw;
        public SkinnedMeshRenderer renderer;

        private MeshInstance()
        {
        }

        private void Delete()
        {
            if (!this.disposed)
            {
                this.disposed = true;
                this.predraw.Shutdown();
                this.postdraw.Shutdown();
                if (this.renderer != null)
                {
                    this.renderer.sharedMaterials = this.originalMaterials;
                }
                this.renderer = null;
                if (dumpCount < 8)
                {
                    this.next = dump;
                    dump = this;
                    this.hasNext = dumpCount++ > 0;
                }
                else
                {
                    this.next = null;
                    this.hasNext = false;
                }
            }
        }

        private static bool New(ViewModel.MeshInstance ptr, SkinnedMeshRenderer renderer, out ViewModel.MeshInstance newInstance)
        {
            if (renderer == null)
            {
                newInstance = null;
                return false;
            }
            if (dumpCount > 0)
            {
                newInstance = dump;
                if (--dumpCount > 0)
                {
                    dump = newInstance.next;
                }
                else
                {
                    dump = null;
                }
                newInstance.next = null;
                newInstance.hasNext = false;
                newInstance.disposed = false;
                newInstance.renderer = null;
            }
            else
            {
                newInstance = new ViewModel.MeshInstance();
            }
            if (ptr != null)
            {
                newInstance.hasNext = ptr.hasNext;
                newInstance.next = ptr.next;
                ptr.hasNext = true;
                ptr.next = newInstance;
            }
            else
            {
                newInstance.hasNext = false;
                newInstance.next = null;
            }
            newInstance.renderer = renderer;
            newInstance.originalMaterials = renderer.sharedMaterials;
            int subMeshCount = renderer.sharedMesh.subMeshCount;
            if ((newInstance.originalMaterials.Length % subMeshCount) != 0)
            {
                Array.Resize<Material>(ref newInstance.originalMaterials, ((newInstance.originalMaterials.Length / subMeshCount) + 1) * subMeshCount);
            }
            newInstance.modifiedMaterials = newInstance.originalMaterials;
            return true;
        }

        public void SetPostdrawMaterial(Material mat)
        {
            this.SetReplacementRenderMaterial(ref this.postdraw, 2, mat);
        }

        public void SetPredrawMaterial(Material mat)
        {
            this.SetReplacementRenderMaterial(ref this.predraw, 1, mat);
        }

        private void SetReplacementRenderMaterial(ref ReplacementRenderer rr, int itsa, Material mat)
        {
            if (!this.disposed)
            {
                if (!rr.initialized)
                {
                    this.legacy = ViewModel.force_legacy_fallback || (this.renderer.sharedMesh.subMeshCount > 1);
                    rr.Initialize(this.renderer, this.renderer, this.originalMaterials, mat, itsa, this.legacy);
                }
                else
                {
                    rr.SetOverride(this.originalMaterials, mat, itsa);
                }
                Material[] materialArray = rr.UpdateMaterials(this.legacy);
                if (!this.legacy)
                {
                    if (materialArray == null)
                    {
                        if (rr.offset != 0)
                        {
                            int offset = rr.offset;
                            for (int i = rr.offset + this.originalMaterials.Length; i < this.modifiedMaterials.Length; i++)
                            {
                                this.modifiedMaterials[offset] = this.modifiedMaterials[i];
                                offset++;
                            }
                            Array.Resize<Material>(ref this.modifiedMaterials, this.modifiedMaterials.Length - this.originalMaterials.Length);
                            rr.offset = 0;
                        }
                    }
                    else
                    {
                        if (rr.offset == 0)
                        {
                            rr.offset = this.modifiedMaterials.Length;
                            Array.Resize<Material>(ref this.modifiedMaterials, this.modifiedMaterials.Length + this.originalMaterials.Length);
                        }
                        int index = rr.offset;
                        for (int j = 0; j < this.originalMaterials.Length; j++)
                        {
                            this.modifiedMaterials[index] = materialArray[j];
                            index++;
                        }
                    }
                    this.renderer.sharedMaterials = this.modifiedMaterials;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Holder : IDisposable
        {
            public ViewModel.MeshInstance first;
            public int count;
            public bool disposed;
            private void IterDelete(ViewModel.MeshInstance iter)
            {
                ViewModel.MeshInstance next = iter.next;
                iter.hasNext = next.hasNext;
                iter.next = next.next;
                this.InstanceDeleteShared(next);
            }

            private void FirstDelete()
            {
                ViewModel.MeshInstance first = this.first;
                this.first = this.first.next;
                this.InstanceDeleteShared(first);
            }

            private void InstanceDeleteShared(ViewModel.MeshInstance instance)
            {
                this.count--;
                instance.Delete();
            }

            public bool Delete(ViewModel.MeshInstance instance)
            {
                if (!this.disposed && (((this.count > 0) && (instance != null)) && !instance.disposed))
                {
                    if (instance == this.first)
                    {
                        this.FirstDelete();
                        return true;
                    }
                    int num = this.count - 1;
                    ViewModel.MeshInstance first = this.first;
                    for (int i = 0; i < num; i++)
                    {
                        if (first.next == instance)
                        {
                            this.IterDelete(first);
                            return true;
                        }
                    }
                }
                return false;
            }

            public bool Delete(SkinnedMeshRenderer renderer)
            {
                if (!this.disposed && (this.count > 0))
                {
                    if (object.ReferenceEquals(renderer, this.first.renderer))
                    {
                        this.FirstDelete();
                        return true;
                    }
                    int num = this.count - 1;
                    ViewModel.MeshInstance first = this.first;
                    for (int i = 0; i < num; i++)
                    {
                        if (object.ReferenceEquals(first.next.renderer, renderer))
                        {
                            this.IterDelete(first);
                            return true;
                        }
                        first = first.next;
                    }
                }
                return false;
            }

            private bool AddShared(bool didIt, ViewModel.MeshInstance meshInstance)
            {
                if (didIt && (this.count++ == 0))
                {
                    this.first = meshInstance;
                }
                CameraFX mainCameraFX = CameraFX.mainCameraFX;
                if (mainCameraFX != null)
                {
                    Material predrawMaterial = mainCameraFX.predrawMaterial;
                    if (predrawMaterial != null)
                    {
                        meshInstance.SetPredrawMaterial(predrawMaterial);
                    }
                    predrawMaterial = mainCameraFX.postdrawMaterial;
                    if (predrawMaterial != null)
                    {
                        meshInstance.SetPostdrawMaterial(predrawMaterial);
                    }
                }
                return didIt;
            }

            public bool Add(SkinnedMeshRenderer renderer)
            {
                ViewModel.MeshInstance instance;
                return ((renderer != null) && this.Add(renderer, out instance));
            }

            public bool Add(SkinnedMeshRenderer renderer, out ViewModel.MeshInstance newOrExistingInstance)
            {
                if (this.disposed)
                {
                    newOrExistingInstance = null;
                    return false;
                }
                if (this.count == 0)
                {
                    return this.AddShared(ViewModel.MeshInstance.New(null, renderer, out newOrExistingInstance), newOrExistingInstance);
                }
                if (object.ReferenceEquals(this.first.renderer, renderer))
                {
                    newOrExistingInstance = this.first;
                    return false;
                }
                int num = this.count - 1;
                ViewModel.MeshInstance first = this.first;
                ViewModel.MeshInstance instance2 = first;
                for (int i = 0; i < num; i++)
                {
                    if (object.ReferenceEquals(first.next.renderer, renderer))
                    {
                        newOrExistingInstance = first.next;
                        return false;
                    }
                    first = first.next;
                }
                return this.AddShared(ViewModel.MeshInstance.New(first, renderer, out newOrExistingInstance), newOrExistingInstance);
            }

            public void Clear()
            {
                while (this.count > 0)
                {
                    this.FirstDelete();
                }
            }

            public void Dispose()
            {
                if (!this.disposed)
                {
                    try
                    {
                        this.Clear();
                    }
                    finally
                    {
                        this.disposed = true;
                    }
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ReplacementRenderer
        {
            public const int kItsaPreDraw = 1;
            public const int kItsaPostDraw = 2;
            public Material[] materials;
            public SkinnedMeshRenderer renderer;
            public bool initialized;
            public int offset;
            public Material[] UpdateMaterials(bool legacy)
            {
                if (!this.initialized)
                {
                    return null;
                }
                if (legacy && (this.renderer != null))
                {
                    this.renderer.sharedMaterials = this.materials;
                }
                return this.materials;
            }

            public void Shutdown()
            {
                if (this.initialized)
                {
                    if (this.renderer != null)
                    {
                        Object.Destroy(this.renderer.gameObject);
                    }
                    this = new ViewModel.MeshInstance.ReplacementRenderer();
                }
            }

            public void Initialize(SkinnedMeshRenderer owner, SkinnedMeshRenderer source, Material[] originalMaterials, Material overrideMaterial, int itsa, bool legacy)
            {
                this.Shutdown();
                if (legacy)
                {
                    Transform transform = owner.transform;
                    this.renderer = (SkinnedMeshRenderer) Object.Instantiate(source);
                    Transform transform2 = this.renderer.transform;
                    transform2.parent = transform.parent;
                    transform2.localPosition = transform.localPosition;
                    transform2.localRotation = transform.localRotation;
                    transform2.localScale = transform.localScale;
                    this.materials = (Material[]) originalMaterials.Clone();
                    this.initialized = true;
                    this.SetOverride(originalMaterials, overrideMaterial, itsa);
                    this.UpdateMaterials(true);
                }
                else
                {
                    this.materials = (Material[]) originalMaterials.Clone();
                    this.initialized = true;
                    if (!this.SetOverride(originalMaterials, overrideMaterial, itsa))
                    {
                        this.materials = null;
                    }
                }
            }

            public bool SetOverride(Material[] originals, Material material, int itsa)
            {
                bool flag = false;
                if (this.initialized)
                {
                    switch (itsa)
                    {
                        case 1:
                            for (int j = 0; j < originals.Length; j++)
                            {
                                if (originals[j] != null)
                                {
                                    if (originals[j].GetTag("SkipViewModelPredraw", false, "False") == "True")
                                    {
                                        this.materials[j] = null;
                                    }
                                    else
                                    {
                                        this.materials[j] = material;
                                        flag = true;
                                    }
                                }
                            }
                            return flag;

                        case 2:
                            for (int k = 0; k < originals.Length; k++)
                            {
                                if (originals[k] != null)
                                {
                                    if (originals[k].GetTag("SkipViewModelPostdraw", false, "False") == "True")
                                    {
                                        this.materials[k] = null;
                                    }
                                    else
                                    {
                                        this.materials[k] = material;
                                        flag = true;
                                    }
                                }
                            }
                            return flag;
                    }
                    for (int i = 0; i < originals.Length; i++)
                    {
                        if (originals[i] != null)
                        {
                            this.materials[i] = material;
                        }
                        flag = true;
                    }
                }
                return flag;
            }
        }
    }
}

