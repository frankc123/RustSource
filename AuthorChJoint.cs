using System;
using UnityEngine;

public class AuthorChJoint : AuthorPeice
{
    [SerializeField]
    private Vector3 anchor;
    [SerializeField]
    private Vector3 axis = Vector3.up;
    [SerializeField]
    private float breakForce = float.PositiveInfinity;
    [SerializeField]
    private float breakTorque = float.PositiveInfinity;
    [SerializeField]
    private AuthorChHit connect;
    [SerializeField]
    private float h_limit_max;
    [SerializeField]
    private float h_limit_maxb;
    [SerializeField]
    private float h_limit_min;
    [SerializeField]
    private float h_limit_minb;
    [SerializeField]
    private float h_motor_f;
    [SerializeField]
    private bool h_motor_s;
    [SerializeField]
    private float h_motor_v;
    [SerializeField]
    private float h_spring_d;
    [SerializeField]
    private float h_spring_s;
    [SerializeField]
    private float h_spring_t;
    [SerializeField]
    private Kind kind;
    [SerializeField]
    private float limitOffset;
    [SerializeField]
    private bool reverseLink;
    [SerializeField]
    private AuthorChHit self;
    [SerializeField]
    private float spring_damper;
    [SerializeField]
    private float spring_max;
    [SerializeField]
    private float spring_min;
    [SerializeField]
    private float spring_spring;
    [SerializeField]
    private float swing1_bounce;
    [SerializeField]
    private float swing1_dampler;
    [SerializeField]
    private float swing1_limit = 20f;
    [SerializeField]
    private float swing1_spring;
    private static readonly Color swing1Color = new Color(1f, 0.4f, 1f, 0.8f);
    [SerializeField]
    private float swing2_bounce;
    [SerializeField]
    private float swing2_dampler;
    [SerializeField]
    private float swing2_limit = 20f;
    [SerializeField]
    private float swing2_spring;
    private static readonly Color swing2Color = new Color(0.4f, 1f, 1f, 0.8f);
    [SerializeField]
    private Vector3 swingAxis = Vector3.forward;
    [SerializeField]
    private float swingOffset1;
    [SerializeField]
    private float swingOffset2;
    private static readonly Color twistColor = new Color(1f, 1f, 0.4f, 0.8f);
    [SerializeField]
    private float twistH_bounce;
    [SerializeField]
    private float twistH_dampler;
    [SerializeField]
    private float twistH_limit = 70f;
    [SerializeField]
    private float twistH_spring;
    [SerializeField]
    private float twistL_bounce;
    [SerializeField]
    private float twistL_dampler;
    [SerializeField]
    private float twistL_limit = -20f;
    [SerializeField]
    private float twistL_spring;
    [SerializeField]
    private float twistOffset;
    [SerializeField]
    private bool useLimit;
    [SerializeField]
    private bool useMotor;
    [SerializeField]
    private bool useSpring;

    public Joint AddJoint(Transform root, ref AuthorChHit.Rep self)
    {
        switch (this.kind)
        {
            case Kind.Hinge:
            {
                HingeJoint joint4 = this.CreateJoint<HingeJoint>(root, ref self);
                joint4.limits = this.limit;
                joint4.useLimits = this.useLimit;
                joint4.motor = this.motor;
                joint4.useMotor = this.useMotor;
                joint4.spring = this.spring;
                joint4.useSpring = this.useSpring;
                return joint4;
            }
            case Kind.Character:
            {
                CharacterJoint joint2 = this.CreateJoint<CharacterJoint>(root, ref self);
                joint2.swingAxis = self.AxisFlip(this.swingAxis);
                joint2.lowTwistLimit = this.lowTwist;
                joint2.highTwistLimit = this.highTwist;
                joint2.lowTwistLimit = this.lowTwist;
                joint2.swing1Limit = this.swing1;
                joint2.swing2Limit = this.swing2;
                return joint2;
            }
            case Kind.Fixed:
                return this.CreateJoint<FixedJoint>(root, ref self);

            case Kind.Spring:
            {
                SpringJoint joint5 = this.CreateJoint<SpringJoint>(root, ref self);
                joint5.spring = this.spring_spring;
                joint5.damper = this.spring_damper;
                joint5.minDistance = this.spring_min;
                joint5.maxDistance = this.spring_max;
                return joint5;
            }
        }
        return null;
    }

    private TJoint ConfigJoint<TJoint>(TJoint joint, Transform root, ref AuthorChHit.Rep self) where TJoint: Joint
    {
        this.ConfigureJointShared(joint, root, ref self);
        return joint;
    }

    private void ConfigureJointShared(Joint joint, Transform root, ref AuthorChHit.Rep self)
    {
        if (this.connect != null)
        {
            AuthorChHit.Rep secondary;
            if (self.mirrored)
            {
                secondary = this.connect.secondary;
                if (!secondary.valid)
                {
                    secondary = this.connect.primary;
                }
            }
            else
            {
                secondary = this.connect.primary;
                if (!secondary.valid)
                {
                    secondary = this.connect.secondary;
                }
            }
            if (secondary.valid)
            {
                Transform transform = root.FindChild(secondary.path);
                Rigidbody rigidbody = transform.rigidbody;
                if (rigidbody == null)
                {
                    rigidbody = transform.gameObject.AddComponent<Rigidbody>();
                }
                joint.connectedBody = rigidbody;
            }
            else
            {
                Debug.LogWarning("No means of making/getting rigidbody", this.connect);
            }
        }
        joint.anchor = self.Flip(this.anchor);
        joint.axis = self.AxisFlip(this.axis);
        joint.breakForce = this.breakForce;
        joint.breakTorque = this.breakTorque;
    }

    private TJoint CreateJoint<TJoint>(Transform root, ref AuthorChHit.Rep self) where TJoint: Joint
    {
        return this.ConfigJoint<TJoint>(self.bone.gameObject.AddComponent<TJoint>(), root, ref self);
    }

    private bool DoTransformHandles(ref AuthorChHit.Rep self, ref AuthorChHit.Rep connect)
    {
        if (!self.valid)
        {
            return false;
        }
        Vector3 anchor = self.Flip(this.anchor);
        Vector3 axis = self.AxisFlip(this.axis);
        Vector3 vector3 = self.AxisFlip(this.swingAxis);
        Matrix4x4 matrix = AuthorShared.Scene.matrix;
        if (connect.valid)
        {
            AuthorShared.Scene.matrix = connect.bone.localToWorldMatrix;
            Color color = AuthorShared.Scene.color;
            AuthorShared.Scene.color = color * new Color(1f, 1f, 1f, 0.4f);
            Vector3 forward = connect.bone.InverseTransformPoint(self.bone.position);
            if (forward != Vector3.zero)
            {
                Quaternion rot = Quaternion.LookRotation(forward);
                AuthorShared.Scene.DrawBone(Vector3.zero, rot, forward.magnitude, 0.02f, new Vector3(0.05f, 0.05f, 0.5f));
            }
            AuthorShared.Scene.color = color;
        }
        AuthorShared.Scene.matrix = self.bone.localToWorldMatrix;
        bool flag = false;
        if (AuthorShared.Scene.PivotDrag(ref anchor, ref axis))
        {
            flag = true;
            this.anchor = self.Flip(anchor);
            this.axis = self.AxisFlip(axis);
        }
        switch (this.kind)
        {
            case Kind.Hinge:
                if (this.useLimit)
                {
                    JointLimits limits = this.limit;
                    if (AuthorShared.Scene.LimitDrag(anchor, axis, ref this.limitOffset, ref limits))
                    {
                        flag = true;
                        this.limit = limits;
                    }
                }
                break;

            case Kind.Character:
            {
                Color color2 = AuthorShared.Scene.color;
                AuthorShared.Scene.color = color2 * twistColor;
                SoftJointLimit lowTwist = this.lowTwist;
                SoftJointLimit highTwist = this.highTwist;
                if (AuthorShared.Scene.LimitDrag(anchor, axis, ref this.twistOffset, ref lowTwist, ref highTwist))
                {
                    flag = true;
                    this.lowTwist = lowTwist;
                    this.highTwist = highTwist;
                }
                AuthorShared.Scene.color = color2 * swing1Color;
                lowTwist = this.swing1;
                if (AuthorShared.Scene.LimitDrag(anchor, vector3, ref this.swingOffset1, ref lowTwist))
                {
                    flag = true;
                    this.swing1 = lowTwist;
                }
                AuthorShared.Scene.color = color2 * swing2Color;
                lowTwist = this.swing2;
                if (AuthorShared.Scene.LimitDrag(anchor, Vector3.Cross(vector3, axis), ref this.swingOffset2, ref lowTwist))
                {
                    flag = true;
                    this.swing2 = lowTwist;
                }
                AuthorShared.Scene.color = color2;
                break;
            }
        }
        AuthorShared.Scene.matrix = matrix;
        return flag;
    }

    private static bool Field(AuthorShared.Content content, ref JointMotor motor, ref bool use)
    {
        GUI.Box(AuthorShared.BeginVertical(new GUILayoutOption[0]), GUIContent.none);
        AuthorShared.PrefixLabel(content);
        bool flag = use;
        bool flag2 = AuthorShared.Change(ref use, GUILayout.Toggle(use, "Use", new GUILayoutOption[0]));
        if (flag)
        {
            float force = motor.force;
            float targetVelocity = motor.targetVelocity;
            bool freeSpin = motor.freeSpin;
            flag2 |= AuthorShared.FloatField("Force", ref force, new GUILayoutOption[0]);
            flag2 |= AuthorShared.FloatField("Target Velocity", ref targetVelocity, new GUILayoutOption[0]);
            flag2 |= AuthorShared.Change(ref freeSpin, GUILayout.Toggle(freeSpin, "Free Spin", new GUILayoutOption[0]));
            if (use && flag2)
            {
                motor.force = force;
                motor.targetVelocity = targetVelocity;
                motor.freeSpin = freeSpin;
            }
        }
        AuthorShared.EndVertical();
        return flag2;
    }

    private static bool Field(AuthorShared.Content content, ref JointSpring spring, ref bool use)
    {
        GUI.Box(AuthorShared.BeginVertical(new GUILayoutOption[0]), GUIContent.none);
        AuthorShared.PrefixLabel(content);
        bool flag = use;
        bool flag2 = AuthorShared.Change(ref use, GUILayout.Toggle(use, "Use", new GUILayoutOption[0]));
        if (flag)
        {
            float num = spring.spring;
            float targetPosition = spring.targetPosition;
            float damper = spring.damper;
            flag2 |= AuthorShared.FloatField("Spring Force", ref num, new GUILayoutOption[0]);
            flag2 |= AuthorShared.FloatField("Target Position", ref targetPosition, new GUILayoutOption[0]);
            flag2 |= AuthorShared.FloatField("Damper", ref damper, new GUILayoutOption[0]);
            if (use && flag2)
            {
                spring.spring = num;
                spring.targetPosition = targetPosition;
                spring.damper = damper;
            }
        }
        AuthorShared.EndVertical();
        return flag2;
    }

    private static bool Field(AuthorShared.Content content, ref SoftJointLimit limits, ref float offset)
    {
        GUI.Box(AuthorShared.BeginVertical(new GUILayoutOption[0]), GUIContent.none);
        AuthorShared.PrefixLabel(content);
        float limit = limits.limit;
        float spring = limits.spring;
        float damper = limits.damper;
        float bounciness = limits.bounciness;
        bool flag = AuthorShared.FloatField("Limit", ref limit, new GUILayoutOption[0]) | AuthorShared.FloatField("Spring", ref spring, new GUILayoutOption[0]);
        flag |= AuthorShared.FloatField("Damper", ref damper, new GUILayoutOption[0]);
        flag |= AuthorShared.FloatField("Bounciness", ref bounciness, new GUILayoutOption[0]);
        if (flag)
        {
            limits.limit = limit;
            limits.spring = spring;
            limits.damper = damper;
            limits.bounciness = bounciness;
        }
        Color contentColor = GUI.contentColor;
        GUI.contentColor = contentColor * new Color(1f, 1f, 1f, 0.3f);
        flag |= AuthorShared.FloatField("Offset(visual only)", ref offset, new GUILayoutOption[0]);
        GUI.contentColor = contentColor;
        AuthorShared.EndVertical();
        return flag;
    }

    private static bool Field(AuthorShared.Content content, ref JointLimits limits, ref bool use, ref float offset)
    {
        GUI.Box(AuthorShared.BeginVertical(new GUILayoutOption[0]), GUIContent.none);
        AuthorShared.PrefixLabel(content);
        bool flag = use;
        bool flag2 = AuthorShared.Change(ref use, GUILayout.Toggle(use, "Use", new GUILayoutOption[0]));
        if (flag)
        {
            float min = limits.min;
            float max = limits.max;
            float minBounce = limits.minBounce;
            float maxBounce = limits.maxBounce;
            flag2 |= AuthorShared.FloatField("Min", ref min, new GUILayoutOption[0]);
            flag2 |= AuthorShared.FloatField("Max", ref max, new GUILayoutOption[0]);
            flag2 |= AuthorShared.FloatField("Min bounciness", ref minBounce, new GUILayoutOption[0]);
            flag2 |= AuthorShared.FloatField("Max bounciness", ref maxBounce, new GUILayoutOption[0]);
            if (use && flag2)
            {
                limits.min = min;
                limits.max = max;
                limits.minBounce = minBounce;
                limits.maxBounce = maxBounce;
            }
            Color contentColor = GUI.contentColor;
            GUI.contentColor = contentColor * new Color(1f, 1f, 1f, 0.3f);
            flag2 |= AuthorShared.FloatField("Offset(visual only)", ref offset, new GUILayoutOption[0]);
            GUI.contentColor = contentColor;
        }
        AuthorShared.EndVertical();
        return flag2;
    }

    internal void InitializeFromOwner(AuthorChHit self, Kind kind)
    {
        this.self = self;
        this.kind = kind;
        AuthorShared.SetDirty(this);
    }

    protected override void OnPeiceDestroy()
    {
        try
        {
            if (this.self != null)
            {
                this.self.OnJointDestroy(this);
            }
        }
        finally
        {
            base.OnPeiceDestroy();
        }
    }

    public override bool OnSceneView()
    {
        bool flag = base.OnSceneView();
        AuthorChHit.Rep primary = this.self.primary;
        AuthorChHit.Rep secondary = this.self.secondary;
        AuthorChHit.Rep connect = new AuthorChHit.Rep();
        AuthorChHit.Rep rep4 = new AuthorChHit.Rep();
        if (this.connect != null)
        {
            connect = this.connect.primary;
            rep4 = this.connect.secondary;
            if (!rep4.valid)
            {
                rep4 = connect;
            }
        }
        if (primary.valid)
        {
            flag |= this.DoTransformHandles(ref primary, ref connect);
        }
        if (secondary.valid)
        {
            flag |= this.DoTransformHandles(ref secondary, ref rep4);
        }
        return flag;
    }

    public override bool PeiceInspectorGUI()
    {
        bool flag = base.PeiceInspectorGUI();
        string peiceID = base.peiceID;
        if (AuthorShared.StringField("Title", ref peiceID, new GUILayoutOption[0]))
        {
            base.peiceID = peiceID;
            flag = true;
        }
        AuthorShared.EnumField("Kind", this.kind, new GUILayoutOption[0]);
        AuthorShared.PrefixLabel("Self");
        if (GUILayout.Button((GUIContent) AuthorShared.ObjectContent<AuthorChHit>(this.self, typeof(AuthorChHit)), new GUILayoutOption[0]))
        {
            AuthorShared.PingObject(this.self);
        }
        flag |= AuthorShared.PeiceField<AuthorChHit>("Connected", this, ref this.connect, typeof(AuthorChHit), GUI.skin.button, new GUILayoutOption[0]);
        flag |= AuthorShared.Toggle("Reverse Link", ref this.reverseLink, new GUILayoutOption[0]);
        flag |= AuthorShared.Vector3Field("Anchor", ref this.anchor, new GUILayoutOption[0]);
        flag |= AuthorShared.Vector3Field("Axis", ref this.axis, new GUILayoutOption[0]);
        switch (this.kind)
        {
            case Kind.Hinge:
            {
                JointLimits limits = this.limit;
                if (Field("Limits", ref limits, ref this.useLimit, ref this.limitOffset))
                {
                    flag = true;
                    this.limit = limits;
                }
                break;
            }
            case Kind.Character:
            {
                flag |= AuthorShared.Vector3Field("Swing Axis", ref this.swingAxis, new GUILayoutOption[0]);
                SoftJointLimit lowTwist = this.lowTwist;
                if (Field("Low Twist", ref lowTwist, ref this.twistOffset))
                {
                    flag = true;
                    this.lowTwist = lowTwist;
                }
                lowTwist = this.highTwist;
                if (Field("High Twist", ref lowTwist, ref this.twistOffset))
                {
                    flag = true;
                    this.highTwist = lowTwist;
                }
                lowTwist = this.swing1;
                if (Field("Swing 1", ref lowTwist, ref this.swingOffset1))
                {
                    flag = true;
                    this.swing1 = lowTwist;
                }
                lowTwist = this.swing2;
                if (Field("Swing 2", ref lowTwist, ref this.swingOffset2))
                {
                    flag = true;
                    this.swing2 = lowTwist;
                }
                break;
            }
        }
        flag |= AuthorShared.FloatField("Break Force", ref this.breakForce, new GUILayoutOption[0]);
        return (flag | AuthorShared.FloatField("Break Torque", ref this.breakTorque, new GUILayoutOption[0]));
    }

    private SoftJointLimit highTwist
    {
        get
        {
            return new SoftJointLimit { limit = this.twistH_limit, damper = this.twistH_dampler, spring = this.twistH_spring, bounciness = this.twistH_bounce };
        }
        set
        {
            this.twistH_limit = value.limit;
            this.twistH_dampler = value.damper;
            this.twistH_spring = value.spring;
            this.twistH_bounce = value.bounciness;
        }
    }

    private JointLimits limit
    {
        get
        {
            return new JointLimits { min = this.h_limit_min, max = this.h_limit_max, minBounce = this.h_limit_minb, maxBounce = this.h_limit_maxb };
        }
        set
        {
            this.h_limit_min = value.min;
            this.h_limit_max = value.max;
            this.h_limit_minb = value.minBounce;
            this.h_limit_maxb = value.maxBounce;
        }
    }

    private SoftJointLimit lowTwist
    {
        get
        {
            return new SoftJointLimit { limit = this.twistL_limit, damper = this.twistL_dampler, spring = this.twistL_spring, bounciness = this.twistL_bounce };
        }
        set
        {
            this.twistL_limit = value.limit;
            this.twistL_dampler = value.damper;
            this.twistL_spring = value.spring;
            this.twistL_bounce = value.bounciness;
        }
    }

    private JointMotor motor
    {
        get
        {
            return new JointMotor { force = this.h_motor_f, targetVelocity = this.h_motor_v, freeSpin = this.h_motor_s };
        }
        set
        {
            this.h_motor_f = value.force;
            this.h_motor_v = value.targetVelocity;
            this.h_motor_s = value.freeSpin;
        }
    }

    private JointSpring spring
    {
        get
        {
            return new JointSpring { spring = this.h_spring_s, damper = this.h_spring_d, targetPosition = this.h_spring_t };
        }
        set
        {
            this.h_spring_s = value.spring;
            this.h_spring_d = value.damper;
            this.h_spring_t = value.targetPosition;
        }
    }

    private SoftJointLimit swing1
    {
        get
        {
            return new SoftJointLimit { limit = this.swing1_limit, damper = this.swing1_dampler, spring = this.swing1_spring, bounciness = this.swing1_bounce };
        }
        set
        {
            this.swing1_limit = value.limit;
            this.swing1_dampler = value.damper;
            this.swing1_spring = value.spring;
            this.swing1_bounce = value.bounciness;
        }
    }

    private SoftJointLimit swing2
    {
        get
        {
            return new SoftJointLimit { limit = this.swing2_limit, damper = this.swing2_dampler, spring = this.swing2_spring, bounciness = this.swing2_bounce };
        }
        set
        {
            this.swing2_limit = value.limit;
            this.swing2_dampler = value.damper;
            this.swing2_spring = value.spring;
            this.swing2_bounce = value.bounciness;
        }
    }

    public enum Kind
    {
        None,
        Hinge,
        Character,
        Fixed,
        Spring
    }
}

