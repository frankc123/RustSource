using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

public class AuthorChHit : AuthorPeice
{
    [CompilerGenerated]
    private static Dictionary<string, int> <>f__switch$map1;
    [SerializeField]
    private float angularDrag = 0.05f;
    [SerializeField]
    private BodyPart bodyPart;
    [SerializeField]
    private Transform bone;
    protected static readonly Color boneGizmoColor = new Color(1f, 1f, 1f, 0.3f);
    [SerializeField]
    private int capsuleAxis = 1;
    [SerializeField]
    private Vector3 center;
    [SerializeField]
    private float damageMultiplier = 1f;
    [SerializeField]
    private float drag;
    [SerializeField]
    private float height = 2f;
    [SerializeField]
    private int hitPriority = 0x80;
    [SerializeField]
    private bool isMirror;
    [SerializeField]
    private HitShapeKind kind;
    private Rect lastPopupRect;
    [SerializeField]
    private float mass = 1f;
    [SerializeField]
    private Transform mirrored;
    [SerializeField]
    private BodyPart mirroredBodyPart;
    protected static readonly Color mirroredGizmoColor = new Color(0f, 0f, 0f, 0.3f);
    [SerializeField]
    private bool mirrorX;
    [SerializeField]
    private bool mirrorY;
    [SerializeField]
    private bool mirrorZ;
    [SerializeField]
    private AuthorChJoint[] myJoints;
    [SerializeField]
    private float radius = 0.5f;
    [SerializeField]
    private Vector3 size = Vector3.one;

    private void AddCharacterJoint()
    {
        this.AddJoint(AuthorChJoint.Kind.Character);
    }

    private void AddFixedJoint()
    {
        this.AddJoint(AuthorChJoint.Kind.Fixed);
    }

    private void AddHingeJoint()
    {
        this.AddJoint(AuthorChJoint.Kind.Hinge);
    }

    private AuthorChJoint AddJoint(AuthorChJoint.Kind kind)
    {
        AuthorChJoint joint = base.creation.CreatePeice<AuthorChJoint>(kind.ToString(), new Type[0]);
        if (joint != null)
        {
            joint.InitializeFromOwner(this, kind);
            Array.Resize<AuthorChJoint>(ref this.myJoints, (this.myJoints != null) ? (this.myJoints.Length + 1) : 1);
            this.myJoints[this.myJoints.Length - 1] = joint;
        }
        return joint;
    }

    private void AddSpringJoint()
    {
        this.AddJoint(AuthorChJoint.Kind.Spring);
    }

    public void CreateColliderOn(Transform instance, Transform root, bool addJoints)
    {
        this.CreateColliderOn(instance, root, addJoints, null);
    }

    public void CreateColliderOn(Transform instance, Transform root, bool addJoints, int? layerIndex)
    {
        if (this.bone != null)
        {
            this.CreatedCollider(this.CreateColliderOn(instance, root, this.bone, false), this.primary, addJoints, layerIndex);
        }
        if ((this.mirrored != null) && (this.mirrored != this.bone))
        {
            this.CreatedCollider(this.CreateColliderOn(instance, root, this.mirrored, true), this.secondary, addJoints, layerIndex);
        }
    }

    private Collider CreateColliderOn(Transform instanceRoot, Transform root, Transform bone, bool mirrored)
    {
        if (bone == null)
        {
            throw new ArgumentException("there was no bone");
        }
        string name = AuthorShared.CalculatePath(bone, root);
        Transform transform = instanceRoot.FindChild(name);
        if (transform == null)
        {
            throw new MissingReferenceException(name);
        }
        switch (this.kind)
        {
            case HitShapeKind.Sphere:
            {
                SphereCollider collider2 = transform.gameObject.AddComponent<SphereCollider>();
                collider2.center = this.GetCenter(mirrored);
                collider2.radius = this.radius;
                break;
            }
            case HitShapeKind.Capsule:
            {
                CapsuleCollider collider3 = transform.gameObject.AddComponent<CapsuleCollider>();
                collider3.center = this.GetCenter(mirrored);
                collider3.radius = this.radius;
                collider3.height = this.height;
                collider3.direction = this.capsuleAxis;
                break;
            }
            case HitShapeKind.Box:
            {
                BoxCollider collider = transform.gameObject.AddComponent<BoxCollider>();
                collider.center = this.GetCenter(mirrored);
                collider.size = this.size;
                break;
            }
            default:
                throw new NotSupportedException();
        }
        return transform.collider;
    }

    private void CreatedCollider(Collider created, Rep repFormat, bool addJoints, int? layerIndex)
    {
        if (created != null)
        {
            repFormat.bone = created.transform;
            if (addJoints)
            {
                Rigidbody rigidbody = created.rigidbody;
                if (rigidbody == null)
                {
                    rigidbody = created.gameObject.AddComponent<Rigidbody>();
                }
                rigidbody.mass = this.mass;
                rigidbody.drag = this.drag;
                rigidbody.angularDrag = this.angularDrag;
                if (this.myJoints != null)
                {
                    foreach (AuthorChJoint joint in this.myJoints)
                    {
                        if (joint != null)
                        {
                            joint.AddJoint(repFormat.bone.root, ref repFormat);
                        }
                    }
                }
            }
            if (layerIndex.HasValue)
            {
                created.gameObject.layer = layerIndex.Value;
            }
        }
    }

    public void CreateHitBoxOn(List<HitShape> list, Transform instance, Transform root)
    {
        this.CreateHitBoxOn(list, instance, root, null);
    }

    public void CreateHitBoxOn(List<HitShape> list, Transform instance, Transform root, int? layerIndex)
    {
        if (this.bone != null)
        {
            list.Add(this.CreateHitBoxOnDo(instance, root, this.bone, false, layerIndex));
        }
        if ((this.mirrored != null) && (this.mirrored != this.bone))
        {
            list.Add(this.CreateHitBoxOnDo(instance, root, this.mirrored, true, layerIndex));
        }
    }

    private HitShape CreateHitBoxOnDo(Transform instanceRoot, Transform root, Transform bone, bool mirrored, int? layerIndex)
    {
        HitBox box;
        Collider collider = this.CreateColliderOn(instanceRoot, root, bone, mirrored);
        if (layerIndex.HasValue)
        {
            collider.gameObject.layer = layerIndex.Value;
        }
        if (base.creation is AuthorHull)
        {
            box = (base.creation as AuthorHull).CreateHitBox(collider.gameObject);
        }
        else
        {
            box = null;
        }
        box.bodyPart = !mirrored ? this.bodyPart : this.mirroredBodyPart;
        box.priority = this.hitPriority;
        box.damageFactor = this.damageMultiplier;
        return new HitShape(collider);
    }

    private bool DoTransformHandles(Transform bone, ref Vector3 center, ref Vector3 size, ref float radius, ref float height, ref int capsuleAxis)
    {
        Matrix4x4 matrix = AuthorShared.Scene.matrix;
        if (bone != null)
        {
            AuthorShared.Scene.matrix = bone.transform.localToWorldMatrix;
        }
        bool flag = false;
        switch (this.kind)
        {
            case HitShapeKind.Sphere:
                flag |= AuthorShared.Scene.SphereDrag(ref center, ref radius);
                break;

            case HitShapeKind.Capsule:
                flag |= AuthorShared.Scene.CapsuleDrag(ref center, ref radius, ref height, ref capsuleAxis);
                break;

            case HitShapeKind.Box:
                flag |= AuthorShared.Scene.BoxDrag(ref center, ref size);
                break;
        }
        AuthorShared.Scene.matrix = matrix;
        return flag;
    }

    private void DrawGiz(Transform bone, bool mirrored)
    {
        if (Event.current.shift && (bone != null))
        {
            Gizmos.matrix = bone.localToWorldMatrix;
            switch (this.kind)
            {
                case HitShapeKind.Sphere:
                    Gizmos.DrawWireSphere(this.GetCenter(mirrored), this.radius);
                    break;

                case HitShapeKind.Capsule:
                    Gizmos2.DrawWireCapsule(this.GetCenter(mirrored), this.radius, this.height, this.capsuleAxis);
                    break;

                case HitShapeKind.Box:
                    Gizmos.DrawWireCube(this.GetCenter(mirrored), this.size);
                    break;
            }
        }
    }

    private void FigureOutDefaultBodyPart(Transform bone, ref BodyPart part)
    {
        if (part == BodyPart.Undefined)
        {
            AuthorHull creation = base.creation as AuthorHull;
            if (creation != null)
            {
                creation.FigureOutDefaultBodyPart(ref bone, ref part, ref this.mirrored, ref this.mirroredBodyPart);
                object[] args = new object[] { bone, (BodyPart) part, this.mirrored, this.mirroredBodyPart };
                Debug.Log(string.Format("[{0}:{1}][{2}:{3}]", args), this);
            }
        }
    }

    private Vector3 GetCenter(bool mirrored)
    {
        if (mirrored)
        {
            return new Vector3(!this.mirrorX ? this.center.x : -this.center.x, !this.mirrorY ? this.center.y : -this.center.y, !this.mirrorZ ? this.center.z : -this.center.z);
        }
        return this.center;
    }

    private void OnDrawGizmos()
    {
        if (this.bone != this.mirrored)
        {
            Gizmos.color = mirroredGizmoColor;
            this.DrawGiz(this.mirrored, true);
        }
        Gizmos.color = boneGizmoColor;
        this.DrawGiz(this.bone, false);
    }

    internal void OnJointDestroy(AuthorChJoint joint)
    {
        if (this.myJoints != null)
        {
            int index = Array.IndexOf<AuthorChJoint>(this.myJoints, joint);
            if (index != -1)
            {
                for (int i = index; i < (this.myJoints.Length - 1); i++)
                {
                    this.myJoints[i] = this.myJoints[i + 1];
                }
                Array.Resize<AuthorChJoint>(ref this.myJoints, this.myJoints.Length - 1);
            }
        }
    }

    protected override void OnPeiceDestroy()
    {
        try
        {
            if (this.myJoints != null)
            {
                AuthorChJoint[] myJoints = this.myJoints;
                this.myJoints = null;
                foreach (AuthorChJoint joint in myJoints)
                {
                    if (joint != null)
                    {
                        Object.Destroy(joint);
                    }
                }
            }
        }
        finally
        {
            base.OnPeiceDestroy();
        }
    }

    protected override void OnRegistered()
    {
        string peiceID = base.peiceID;
        if (peiceID != null)
        {
            int num;
            if (<>f__switch$map1 == null)
            {
                Dictionary<string, int> dictionary = new Dictionary<string, int>(3);
                dictionary.Add("Sphere", 0);
                dictionary.Add("Box", 1);
                dictionary.Add("Capsule", 2);
                <>f__switch$map1 = dictionary;
            }
            if (<>f__switch$map1.TryGetValue(peiceID, out num))
            {
                switch (num)
                {
                    case 0:
                        this.kind = HitShapeKind.Sphere;
                        break;

                    case 1:
                        this.kind = HitShapeKind.Box;
                        break;

                    case 2:
                        this.kind = HitShapeKind.Capsule;
                        break;
                }
            }
        }
        base.OnRegistered();
    }

    public override bool OnSceneView()
    {
        bool flag = base.OnSceneView() | this.DoTransformHandles(this.bone, ref this.center, ref this.size, ref this.radius, ref this.height, ref this.capsuleAxis);
        if ((this.mirrored != null) && (this.mirrored != this.bone))
        {
            Vector3 vector;
            vector.x = !this.mirrorX ? this.center.x : -this.center.x;
            vector.y = !this.mirrorY ? this.center.y : -this.center.y;
            vector.z = !this.mirrorZ ? this.center.z : -this.center.z;
            if (this.DoTransformHandles(this.mirrored, ref vector, ref this.size, ref this.radius, ref this.height, ref this.capsuleAxis))
            {
                this.center.x = !this.mirrorX ? vector.x : -vector.x;
                this.center.y = !this.mirrorY ? vector.y : -vector.y;
                this.center.z = !this.mirrorZ ? vector.z : -vector.z;
                flag = true;
            }
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
        bool flag2 = (this.mirrored != null) && (this.mirrored != this.bone);
        bool bone = (bool) this.bone;
        BodyPart bodyPart = this.bodyPart;
        if (AuthorShared.ObjectField<Transform>("Bone", ref this.bone, AuthorShared.ObjectFieldFlags.Instance | AuthorShared.ObjectFieldFlags.Model | AuthorShared.ObjectFieldFlags.AllowScene, new GUILayoutOption[0]))
        {
            if (!bone)
            {
                this.FigureOutDefaultBodyPart(this.bone, ref bodyPart);
            }
            flag = true;
        }
        BodyPart mirroredBodyPart = this.mirroredBodyPart;
        if (bone)
        {
            bodyPart = (BodyPart) AuthorShared.EnumField("Body Part", bodyPart, new GUILayoutOption[0]);
        }
        GUI.Box(AuthorShared.BeginVertical(new GUILayoutOption[0]), GUIContent.none);
        flag |= AuthorShared.ObjectField<Transform>("Mirrored Bone", ref this.mirrored, AuthorShared.ObjectFieldFlags.Instance | AuthorShared.ObjectFieldFlags.Model | AuthorShared.ObjectFieldFlags.AllowScene, new GUILayoutOption[0]);
        if (flag2)
        {
            mirroredBodyPart = (BodyPart) AuthorShared.EnumField("Body Part", mirroredBodyPart, new GUILayoutOption[0]);
            AuthorShared.BeginHorizontal(new GUILayoutOption[0]);
            bool flag4 = GUILayout.Toggle(this.mirrorX, "Mirror X", new GUILayoutOption[0]);
            bool flag5 = GUILayout.Toggle(this.mirrorY, "Mirror Y", new GUILayoutOption[0]);
            bool flag6 = GUILayout.Toggle(this.mirrorZ, "Mirror Z", new GUILayoutOption[0]);
            AuthorShared.EndHorizontal();
            if (((flag4 != this.mirrorX) || (flag5 != this.mirrorY)) || (flag6 != this.mirrorZ))
            {
                this.mirrorX = flag4;
                this.mirrorY = flag5;
                this.mirrorZ = flag6;
                flag = true;
            }
        }
        AuthorShared.EndVertical();
        Vector3 center = this.center;
        float radius = this.radius;
        float height = this.height;
        Vector3 size = this.size;
        int capsuleAxis = this.capsuleAxis;
        AuthorShared.BeginSubSection("Shape", new GUILayoutOption[0]);
        HitShapeKind kind = (HitShapeKind) AuthorShared.EnumField("Kind", this.kind, new GUILayoutOption[0]);
        switch (this.kind)
        {
            case HitShapeKind.Sphere:
                center = AuthorShared.Vector3Field("Center", this.center, new GUILayoutOption[0]);
                radius = Mathf.Max(AuthorShared.FloatField("Radius", this.radius, new GUILayoutOption[0]), 0.001f);
                break;

            case HitShapeKind.Capsule:
                center = AuthorShared.Vector3Field("Center", this.center, new GUILayoutOption[0]);
                radius = Mathf.Max(AuthorShared.FloatField("Radius", this.radius, new GUILayoutOption[0]), 0.001f);
                height = Mathf.Max(AuthorShared.FloatField("Height", this.height, new GUILayoutOption[0]), 0.001f);
                capsuleAxis = Mathf.Clamp(AuthorShared.IntField("Height Axis", this.capsuleAxis, new GUILayoutOption[0]), 0, 2);
                break;

            case HitShapeKind.Box:
                center = AuthorShared.Vector3Field("Center", this.center, new GUILayoutOption[0]);
                size = AuthorShared.Vector3Field("Size", this.size, new GUILayoutOption[0]);
                break;
        }
        AuthorShared.EndSubSection();
        AuthorShared.BeginSubSection("Rigidbody", new GUILayoutOption[0]);
        float num4 = Mathf.Max(AuthorShared.FloatField("Mass", this.mass, new GUILayoutOption[0]), 0.001f);
        float num5 = Mathf.Max(AuthorShared.FloatField("Drag", this.drag, new GUILayoutOption[0]), 0f);
        float num6 = Mathf.Max(AuthorShared.FloatField("Angular Drag", this.angularDrag, new GUILayoutOption[0]), 0f);
        AuthorShared.EndSubSection();
        AuthorShared.BeginSubSection("Hit Box", new GUILayoutOption[0]);
        int hitPriority = this.hitPriority;
        float damageMultiplier = this.damageMultiplier;
        if (flag2 || bone)
        {
            hitPriority = AuthorShared.IntField("Hit Priority", hitPriority, new GUILayoutOption[0]);
            damageMultiplier = AuthorShared.FloatField("Damage Mult.", damageMultiplier, new GUILayoutOption[0]);
        }
        AuthorShared.EndSubSection();
        bool flag7 = GUILayout.Button("Add Joint", new GUILayoutOption[0]);
        if (Event.current.type == EventType.Repaint)
        {
            this.lastPopupRect = GUILayoutUtility.GetLastRect();
        }
        if (flag7)
        {
            AuthorShared.CustomMenu(this.lastPopupRect, JointMenu.options, 0, new AuthorShared.CustomMenuProc(JointMenu.Callback), this);
        }
        if (((((kind != this.kind) || (center != this.center)) || ((size != this.size) || (radius != this.radius))) || (((height != this.height) || (capsuleAxis != this.capsuleAxis)) || ((num4 != this.mass) || (num5 != this.drag)))) || (((num6 != this.angularDrag) || (bodyPart != this.bodyPart)) || (((mirroredBodyPart != this.mirroredBodyPart) || (this.hitPriority != hitPriority)) || (damageMultiplier != this.damageMultiplier))))
        {
            flag = true;
            this.kind = kind;
            this.center = center;
            this.size = size;
            this.radius = radius;
            this.height = height;
            this.capsuleAxis = capsuleAxis;
            this.mass = num4;
            this.drag = num5;
            this.angularDrag = num6;
            this.bodyPart = bodyPart;
            this.mirroredBodyPart = mirroredBodyPart;
            this.hitPriority = hitPriority;
            this.damageMultiplier = damageMultiplier;
        }
        return flag;
    }

    public override void SaveJsonProperties(JSONStream stream)
    {
        base.SaveJsonProperties(stream);
        stream.WriteText("bone", base.FromRootBonePath(this.bone));
        stream.WriteEnum("bonepart", this.bodyPart);
        stream.WriteBoolean("mirror", this.isMirror);
        stream.WriteText("mirrorbone", base.FromRootBonePath(this.mirrored));
        stream.WriteEnum("mirrorbonepart", this.mirroredBodyPart);
        stream.WriteArrayStart("mirrorboneflip");
        stream.WriteBoolean(this.mirrorX);
        stream.WriteBoolean(this.mirrorY);
        stream.WriteBoolean(this.mirrorZ);
        stream.WriteArrayEnd();
        stream.WriteEnum("kind", this.kind);
        stream.WriteVector3("center", this.center);
        stream.WriteVector3("size", this.size);
        stream.WriteNumber("radius", this.radius);
        stream.WriteNumber("height", this.height);
        stream.WriteInteger("capsuleaxis", this.capsuleAxis);
        stream.WriteNumber("damagemul", this.damageMultiplier);
        stream.WriteInteger("hitpriority", this.hitPriority);
        stream.WriteNumber("mass", this.mass);
        stream.WriteNumber("drag", this.drag);
        stream.WriteNumber("adrag", this.angularDrag);
    }

    public Rep primary
    {
        get
        {
            Rep rep;
            rep.hit = this;
            rep.bone = this.bone;
            rep.mirrored = false;
            rep.flipX = rep.flipY = rep.flipZ = false;
            rep.center = this.center;
            rep.size = this.size;
            rep.radius = this.radius;
            rep.height = this.height;
            rep.capsuleAxis = this.capsuleAxis;
            rep.valid = (bool) rep.bone;
            return rep;
        }
        set
        {
            this.bone = value.bone;
            this.center = value.center;
            this.size = value.size;
            this.radius = value.radius;
            this.height = value.height;
            this.capsuleAxis = value.capsuleAxis;
        }
    }

    public Rep secondary
    {
        get
        {
            Rep rep;
            rep.hit = this;
            rep.bone = (this.mirrored != this.bone) ? this.mirrored : null;
            rep.mirrored = true;
            rep.flipX = this.mirrorX;
            rep.flipY = this.mirrorY;
            rep.flipZ = this.mirrorZ;
            rep.center = this.GetCenter(true);
            rep.size = this.size;
            rep.radius = this.radius;
            rep.height = this.height;
            rep.capsuleAxis = this.capsuleAxis;
            rep.valid = (bool) rep.bone;
            return rep;
        }
        set
        {
            this.mirrored = value.bone;
            this.center = value.Flip(value.center);
            this.size = value.size;
            this.radius = value.radius;
            this.height = value.height;
            this.capsuleAxis = value.capsuleAxis;
        }
    }

    private static class JointMenu
    {
        public static readonly GUIContent[] options = new GUIContent[] { new GUIContent("Nevermind"), new GUIContent("Add Hinge Joint"), new GUIContent("Add Character Joint"), new GUIContent("Add Fixed Joint"), new GUIContent("Add Spring Joint") };

        public static void Callback(object userData, string[] options, int selected)
        {
            AuthorChHit hit = userData as AuthorChHit;
            switch (selected)
            {
                case 1:
                    hit.AddHingeJoint();
                    break;

                case 2:
                    hit.AddCharacterJoint();
                    break;

                case 3:
                    hit.AddFixedJoint();
                    break;

                case 4:
                    hit.AddSpringJoint();
                    break;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Rep
    {
        public AuthorChHit hit;
        public Transform bone;
        public bool mirrored;
        public bool flipX;
        public bool flipY;
        public bool flipZ;
        public Vector3 center;
        public Vector3 size;
        public float radius;
        public float height;
        public int capsuleAxis;
        public bool valid;
        public Vector3 Flip(Vector3 v)
        {
            if (this.flipX)
            {
                v.x = -v.x;
            }
            if (this.flipY)
            {
                v.y = -v.y;
            }
            if (this.flipZ)
            {
                v.z = -v.z;
            }
            return v;
        }

        public Vector3 AxisFlip(Vector3 v)
        {
            if (this.flipX == this.mirrored)
            {
                v.x = -v.x;
            }
            if (this.flipY == this.mirrored)
            {
                v.y = -v.y;
            }
            if (this.flipZ == this.mirrored)
            {
                v.z = -v.z;
            }
            return v;
        }

        public string path
        {
            get
            {
                if (!this.valid)
                {
                    return null;
                }
                return AuthorShared.CalculatePath(this.bone, this.bone.root);
            }
        }
    }
}

