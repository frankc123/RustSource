using Facepunch.Actor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

[AuthorSuiteCreation(Title="Author Hull", Description="Create a new character. Allows you to define hitboxes and fine tune ragdoll and joints.", Scripter="Pat", OutputType=typeof(Character), Ready=true)]
public class AuthorHull : AuthorCreation
{
    [CompilerGenerated]
    private static AuthorShared.ArrayFieldFunctor<Material> <>f__am$cache26;
    [SerializeField]
    private ActorRig actorRig;
    private static readonly MemberFilter actorRigSearch = new MemberFilter(AuthorHull.ActorRigSearch);
    [SerializeField]
    private bool allowBonesOutsideOfModelPrefab;
    [SerializeField]
    private BodyPartTransformMap bodyParts;
    private HitBoxSystem creatingSystem;
    [SerializeField]
    private string defaultBodyPartLayer;
    [SerializeField]
    private bool drawBones;
    [SerializeField]
    private Vector3 editingAngles;
    [SerializeField]
    private bool editingCenterToRoot;
    [SerializeField]
    private float eyeHeight;
    [SerializeField]
    private GameObject generatedHitBox;
    [SerializeField]
    private GameObject generatedRigid;
    [SerializeField]
    private string hitBoxLayerName;
    [SerializeField]
    private GameObject hitBoxOutputPrefab;
    [SerializeField]
    private string hitBoxSystemType;
    [SerializeField]
    private string hitBoxType;
    [SerializeField]
    private Vector3 hitCapsuleCenter;
    [SerializeField]
    private int hitCapsuleDirection;
    [SerializeField]
    private float hitCapsuleHeight;
    [SerializeField]
    private float hitCapsuleRadius;
    [SerializeField]
    private Transform hitCapsuleTransform;
    [SerializeField]
    private int ignoreCollisionDownSteps;
    [SerializeField]
    private int ignoreCollisionUpSteps;
    [SerializeField]
    private Material[] materials;
    [SerializeField]
    private GameObject modelPrefab;
    [SerializeField]
    private GameObject modelPrefabForHitBox;
    [SerializeField]
    private GameObject modelPrefabInstance;
    private static bool once;
    [SerializeField]
    private string previewPrototypeGUID;
    [SerializeField]
    private Character prototype;
    [SerializeField]
    private GameObject ragdollOutputPrefab;
    [SerializeField]
    private Ragdoll ragdollPrototype;
    [SerializeField]
    private bool removeAnimationFromRagdoll;
    [SerializeField]
    private Transform[] removeThese;
    [SerializeField]
    private bool savedGenerated;
    [SerializeField]
    private string saveJSONGUID;
    private bool showAllBones;
    private const string suffix_hitbox = "::HITBOX_OUTPUT::";
    private const string suffix_rigid = "::RAGDOLL_OUTPUT::";
    [SerializeField]
    private bool useMeshesFromHitBoxOnRagdoll;

    public AuthorHull() : this(typeof(Character))
    {
    }

    protected AuthorHull(Type type) : base(type)
    {
        this.hitCapsuleRadius = 1f;
        this.hitCapsuleHeight = 2.5f;
        this.ignoreCollisionDownSteps = 2;
        this.ignoreCollisionUpSteps = 1;
        this.hitBoxType = "HitBox";
        this.hitBoxSystemType = "HitBoxSystem";
        this.defaultBodyPartLayer = string.Empty;
        this.eyeHeight = 1f;
        this.editingAngles = Vector3.zero;
        this.bodyParts = new BodyPartTransformMap();
        this.hitBoxLayerName = "Hitbox";
    }

    private static bool ActorRigSearch(MemberInfo m, object filterCriteria)
    {
        return (((FieldInfo) m).FieldType == typeof(ActorRig));
    }

    private void ApplyMaterials(GameObject instance)
    {
        SkinnedMeshRenderer renderer = (instance != null) ? instance.GetComponentInChildren<SkinnedMeshRenderer>() : null;
        if (renderer != null)
        {
            renderer.sharedMaterials = this.materials;
        }
    }

    private IDMain ApplyPrototype(GameObject output, IDMain prototype)
    {
        IDMain main = null;
        if (prototype != null)
        {
            int num3;
            Component[] components = prototype.GetComponents<Component>();
            Component[] srcComponents = new Component[components.Length];
            Component[] dstComponents = new Component[components.Length];
            int length = components.Length;
            int num2 = -1;
            int num4 = 500;
            do
            {
                num3 = 0;
                for (int j = 0; j < length; j++)
                {
                    Component component = components[j];
                    if (component == null)
                    {
                        components[j] = null;
                    }
                    else if (component is Transform)
                    {
                        components[j] = null;
                        srcComponents[j] = component;
                        dstComponents[num2 = j] = output.transform;
                    }
                    else
                    {
                        Component component2 = output.AddComponent(component.GetType());
                        if (component2 != null)
                        {
                            dstComponents[j] = component2;
                            components[j] = null;
                            srcComponents[j] = component;
                            if (component2 is IDMain)
                            {
                                main = (IDMain) component2;
                            }
                        }
                        else
                        {
                            num3++;
                        }
                    }
                }
            }
            while ((num3 != 0) || (num4-- <= 0));
            if (num4 < 0)
            {
                Debug.LogError("Couldnt remake all components");
            }
            for (int i = 0; i < 2; i++)
            {
                for (int k = 0; k < length; k++)
                {
                    if (k != num2)
                    {
                        Component src = srcComponents[k];
                        Component dst = dstComponents[k];
                        if (src != null)
                        {
                            if (dst != null)
                            {
                                this.TransferComponentSettings(prototype.gameObject, output, srcComponents, dstComponents, src, dst);
                                AuthorShared.SetDirty(dst);
                            }
                            else
                            {
                                Debug.LogWarning("no dest for source " + src, src);
                            }
                        }
                        else if (dst != null)
                        {
                            Debug.LogWarning("no source for dest " + dst, dst);
                        }
                        else
                        {
                            Debug.LogWarning("no source or dest", output);
                        }
                    }
                }
            }
            output.layer = prototype.gameObject.layer;
            output.tag = prototype.gameObject.tag;
        }
        return main;
    }

    private bool ApplyRig(GameObject output)
    {
        if (this.actorRig != null)
        {
            BoneStructure.EditorOnly_AddOrUpdateBoneStructure(output, this.actorRig);
            return true;
        }
        BoneStructure component = output.GetComponent<BoneStructure>();
        if (component != null)
        {
            Object.DestroyImmediate(component, true);
        }
        return false;
    }

    private List<KeyValuePair<MethodInfo, MonoBehaviour>> CaptureFinalizeMethods(GameObject output, string methodName)
    {
        List<KeyValuePair<MethodInfo, MonoBehaviour>> list = new List<KeyValuePair<MethodInfo, MonoBehaviour>>();
        foreach (MonoBehaviour behaviour in output.GetComponentsInChildren<MonoBehaviour>(true))
        {
            if (behaviour != null)
            {
                foreach (MethodInfo info in behaviour.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
                {
                    if (info.Name == methodName)
                    {
                        list.Add(new KeyValuePair<MethodInfo, MonoBehaviour>(info, behaviour));
                    }
                }
            }
        }
        return list;
    }

    private void ChangedEditingOptions()
    {
        if (this.modelPrefabInstance != null)
        {
            this.modelPrefabInstance.transform.localEulerAngles = this.editingAngles;
            this.modelPrefabInstance.transform.localPosition = Vector3.zero;
            if (this.editingCenterToRoot)
            {
                Transform rootBone = AuthorShared.GetRootBone(this.modelPrefabInstance.GetComponentInChildren<SkinnedMeshRenderer>());
                if (rootBone != null)
                {
                    this.modelPrefabInstance.transform.position = -rootBone.position;
                }
            }
        }
    }

    private void ChangedModelPrefab()
    {
        if (this.modelPrefabInstance != null)
        {
            Object.DestroyImmediate(this.modelPrefabInstance);
        }
        this.modelPrefabInstance = AuthorShared.InstantiatePrefab(this.modelPrefab);
        this.modelPrefabInstance.transform.localPosition = Vector3.zero;
        this.modelPrefabInstance.transform.localRotation = Quaternion.identity;
        this.modelPrefabInstance.transform.localScale = Vector3.one;
    }

    private GameObject CreateEyes(GameObject output)
    {
        return new GameObject("Eyes") { transform = { parent = output.transform, localPosition = new Vector3(0f, this.eyeHeight, 0f) } };
    }

    public HitBox CreateHitBox(GameObject target)
    {
        HitBox objSet = AuthorShared.AddComponent<HitBox>(target, this.hitBoxType);
        AuthorShared.SetSerializedProperty(objSet, "_hitBoxSystem", this.creatingSystem);
        objSet.idMain = objSet.hitBoxSystem.idMain;
        return objSet;
    }

    public HitBoxSystem CreateHitBoxSystem(GameObject target)
    {
        return AuthorShared.AddComponent<HitBoxSystem>(target, this.hitBoxSystemType);
    }

    private void DestroyRepresentations(ref GameObject stored, string suffix)
    {
        if (stored != null)
        {
            Object.DestroyImmediate(stored);
        }
        foreach (Object obj2 in Object.FindObjectsOfType(typeof(GameObject)))
        {
            if (((obj2 != null) && (((GameObject) obj2).transform.parent == null)) && obj2.name.EndsWith(suffix))
            {
                Object.DestroyImmediate(obj2);
            }
        }
    }

    public override IEnumerable<AuthorPeice> DoSceneView()
    {
        if (this.drawBones && (this.modelPrefabInstance != null))
        {
            Transform rootBone = AuthorShared.GetRootBone(this.modelPrefabInstance);
            if (rootBone != null)
            {
                Color color = AuthorShared.Scene.color;
                Color color2 = color * new Color(0.9f, 0.8f, 0.3f, 0.1f);
                List<Transform> list = rootBone.ListDecendantsByDepth();
                AuthorShared.Scene.color = color2;
                foreach (Transform transform2 in list)
                {
                    Vector3 position = transform2.parent.position;
                    Vector3 forward = transform2.position - position;
                    float magnitude = forward.magnitude;
                    if (magnitude != 0f)
                    {
                        Vector3 up = transform2.up;
                        Quaternion rot = Quaternion.LookRotation(forward, up);
                        AuthorShared.Scene.DrawBone(position, rot, magnitude, Mathf.Min((float) (magnitude / 2f), (float) 0.025f), (Vector3) (Vector3.one * Mathf.Min(magnitude, 0.05f)));
                    }
                }
                AuthorShared.Scene.color = color;
            }
        }
        return base.DoSceneView();
    }

    private bool EnsureItsAPrefab(ref Object obj)
    {
        return (obj == 0);
    }

    protected override IEnumerable<AuthorPalletObject> EnumeratePalletObjects()
    {
        AuthorPalletObject[] pallet = HitBoxItems.pallet;
        if (!once)
        {
            pallet[0].guiContent.image = AuthorShared.ObjectContent(null, typeof(BoxCollider)).image;
            pallet[1].guiContent.image = AuthorShared.ObjectContent(null, typeof(SphereCollider)).image;
            pallet[2].guiContent.image = AuthorShared.ObjectContent(null, typeof(CapsuleCollider)).image;
            once = true;
        }
        return pallet;
    }

    internal void FigureOutDefaultBodyPart(ref Transform bone, ref BodyPart bodyPart, ref Transform mirrored, ref BodyPart mirroredBodyPart)
    {
        BodyPart part = bodyPart;
        for (BodyPart part2 = BodyPart.Undefined; part2 < (BodyPart.L_Cheek | BodyPart.Neck); part2 += 1)
        {
            Transform transform;
            if (this.bodyParts.TryGetValue(part2, out transform) && (transform == bone))
            {
                part = part2;
            }
        }
        if (part != bodyPart)
        {
            bodyPart = part;
            if ((mirrored == null) && bodyPart.IsSided())
            {
                part = part.SwapSide();
                if (this.bodyParts.TryGetValue(part, out mirrored))
                {
                    mirroredBodyPart = part;
                }
            }
        }
    }

    [Conditional("LOG_GENERATE")]
    private static void GenerateLog(object text)
    {
        Debug.Log(text);
    }

    [Conditional("LOG_GENERATE")]
    private static void GenerateLog(object text, Object obj)
    {
        Debug.Log(text, obj);
    }

    private void GeneratePrefabInstances()
    {
        this.DestroyRepresentations(ref this.generatedRigid, "::RAGDOLL_OUTPUT::");
        this.generatedRigid = this.MakeColliderPrefab();
        this.DestroyRepresentations(ref this.generatedHitBox, "::HITBOX_OUTPUT::");
        this.generatedHitBox = this.MakeHitBoxPrefab();
        if ((this.generatedHitBox != null) && (this.generatedRigid != null))
        {
            AuthorShared.AttributeKeyValueList list = GenKVL(this.generatedHitBox, this.generatedRigid);
            list.Run(this.generatedHitBox);
            list.Run(this.generatedRigid);
            List<KeyValuePair<MethodInfo, MonoBehaviour>> list2 = this.CaptureFinalizeMethods(this.generatedHitBox, "OnAuthoredAsHitBoxPrefab");
            List<KeyValuePair<MethodInfo, MonoBehaviour>> list3 = this.CaptureFinalizeMethods(this.generatedRigid, "OnAuthoredAsRagdollPrefab");
            object[] parameters = new object[] { this.generatedRigid };
            foreach (KeyValuePair<MethodInfo, MonoBehaviour> pair in list2)
            {
                if (pair.Value != null)
                {
                    try
                    {
                        pair.Key.Invoke(pair.Value, parameters);
                    }
                    catch (Exception exception)
                    {
                        Debug.LogException(exception, pair.Value);
                    }
                }
            }
            object[] objArray2 = new object[] { this.generatedHitBox };
            foreach (KeyValuePair<MethodInfo, MonoBehaviour> pair2 in list3)
            {
                if (pair2.Value != null)
                {
                    try
                    {
                        pair2.Key.Invoke(pair2.Value, objArray2);
                    }
                    catch (Exception exception2)
                    {
                        Debug.LogException(exception2, pair2.Value);
                    }
                }
            }
        }
        AuthorShared.SetDirty(this.generatedRigid);
        AuthorShared.SetDirty(this.generatedHitBox);
    }

    private static AuthorShared.AttributeKeyValueList GenKVL(GameObject hitBox, GameObject rigid)
    {
        return new AuthorShared.AttributeKeyValueList(new object[] { AuthTarg.HitBox, hitBox, AuthTarg.Ragdoll, rigid });
    }

    [DebuggerHidden]
    private static IEnumerable<Collider> GetCollidersOnRigidbody(Rigidbody rb)
    {
        return new <GetCollidersOnRigidbody>c__Iterator6 { rb = rb, <$>rb = rb, $PC = -2 };
    }

    private Transform GetHitColliderParent(GameObject root)
    {
        SkinnedMeshRenderer renderer;
        Transform rootBone = AuthorShared.GetRootBone(root, out renderer);
        return (((renderer == null) || (renderer.transform.parent == null)) ? rootBone : renderer.transform.parent);
    }

    private GameObject InstantiatePrefabWithRemovedBones(GameObject prefab)
    {
        GameObject context = AuthorShared.InstantiatePrefab(prefab);
        if (this.modelPrefabInstance != null)
        {
            if (this.removeThese != null)
            {
                for (int i = 0; i < this.removeThese.Length; i++)
                {
                    if (this.removeThese[i] != null)
                    {
                        Transform transform = context.transform.FindChild(AuthorShared.CalculatePath(this.removeThese[i], this.modelPrefabInstance.transform));
                        if (transform != null)
                        {
                            Object.DestroyImmediate(transform);
                        }
                    }
                }
            }
            if (this.allowBonesOutsideOfModelPrefab || (prefab == this.modelPrefab))
            {
                return context;
            }
            foreach (Transform transform2 in context.GetComponentsInChildren<Transform>(true))
            {
                if (transform2 != null)
                {
                    string str = AuthorShared.CalculatePath(transform2, context.transform);
                    if (!string.IsNullOrEmpty(str) && (this.modelPrefabInstance.transform.Find(str) == null))
                    {
                        Debug.LogWarning("Deleted bone because it was not in the model prefab instance:" + str, context);
                        Object.DestroyImmediate(transform2.gameObject);
                    }
                }
            }
        }
        return context;
    }

    protected override void LoadSettings(JSONStream stream)
    {
        stream.ReadSkip();
    }

    private GameObject MakeColliderPrefab()
    {
        int? nullable;
        GameObject instance = this.InstantiatePrefabWithRemovedBones(this.modelPrefab);
        if (this.removeThese != null)
        {
            instance.name = instance.name + "::RAGDOLL_OUTPUT::";
        }
        foreach (Animation animation in instance.GetComponentsInChildren<Animation>())
        {
            if (animation != null)
            {
                Object.DestroyImmediate(animation, true);
            }
        }
        if ((this.useMeshesFromHitBoxOnRagdoll && (this.modelPrefabForHitBox != null)) && (this.modelPrefabForHitBox != this.modelPrefab))
        {
            foreach (Renderer renderer in instance.GetComponentsInChildren<Renderer>())
            {
                if (renderer != null)
                {
                    if (renderer is MeshRenderer)
                    {
                        MeshFilter component = renderer.GetComponent<MeshFilter>();
                        string name = AuthorShared.CalculatePath(renderer.transform, instance.transform);
                        component.sharedMesh = this.modelPrefabForHitBox.transform.FindChild(name).GetComponent<MeshFilter>().sharedMesh;
                        AuthorShared.SetDirty(component);
                    }
                    else if (renderer is SkinnedMeshRenderer)
                    {
                        ((SkinnedMeshRenderer) renderer).sharedMesh = this.modelPrefabForHitBox.transform.FindChild(AuthorShared.CalculatePath(renderer.transform, instance.transform)).GetComponent<SkinnedMeshRenderer>().sharedMesh;
                        AuthorShared.SetDirty(renderer);
                    }
                }
            }
        }
        this.ApplyMaterials(instance);
        if (string.IsNullOrEmpty(this.defaultBodyPartLayer))
        {
            nullable = null;
        }
        else
        {
            nullable = new int?(LayerMask.NameToLayer(this.defaultBodyPartLayer));
        }
        IEnumerator<AuthorPeice> enumerator = base.EnumeratePeices().GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                AuthorPeice current = enumerator.Current;
                if ((current != null) && (current is AuthorChHit))
                {
                    ((AuthorChHit) current).CreateColliderOn(instance.transform, this.modelPrefabInstance.transform, true, nullable);
                }
            }
        }
        finally
        {
            if (enumerator == null)
            {
            }
            enumerator.Dispose();
        }
        Transform rootBone = AuthorShared.GetRootBone(instance);
        Dictionary<Rigidbody, List<Collider>> dictionary = new Dictionary<Rigidbody, List<Collider>>();
        foreach (Collider collider in rootBone.GetComponentsInChildren<Collider>())
        {
            Rigidbody attachedRigidbody = collider.attachedRigidbody;
            if (attachedRigidbody != null)
            {
                List<Collider> list;
                if (!dictionary.TryGetValue(attachedRigidbody, out list))
                {
                    list = new List<Collider>();
                    dictionary[attachedRigidbody] = list;
                }
                list.Add(collider);
            }
        }
        HashSet<KeyValuePair<Collider, Collider>> set = new HashSet<KeyValuePair<Collider, Collider>>();
        foreach (KeyValuePair<Rigidbody, List<Collider>> pair in dictionary)
        {
            Rigidbody rigidbody2;
            Transform transform = pair.Key.transform;
            Transform parent = transform.parent;
            int num4 = 0;
            goto Label_0391;
        Label_02C7:
            rigidbody2 = parent.rigidbody;
            if (((rigidbody2 == null) && ((parent = parent.parent) != null)) && parent.IsChildOf(rootBone))
            {
                goto Label_02C7;
            }
            if (rigidbody2 != null)
            {
                foreach (Collider collider2 in pair.Value)
                {
                    foreach (Collider collider3 in dictionary[rigidbody2])
                    {
                        set.Add(MakeKV(collider2, collider3));
                    }
                }
            }
        Label_0391:
            if (((num4++ < this.ignoreCollisionUpSteps) && (parent != null)) && parent.IsChildOf(rootBone))
            {
                goto Label_02C7;
            }
            if (this.ignoreCollisionDownSteps > 0)
            {
                foreach (Transform transform4 in transform.ListDecendantsByDepth())
                {
                    if (transform4.rigidbody == null)
                    {
                        continue;
                    }
                    parent = transform4.parent;
                    num4 = 0;
                    while (parent != transform)
                    {
                        if ((parent.rigidbody != null) && (++num4 > this.ignoreCollisionDownSteps))
                        {
                            break;
                        }
                        parent = parent.parent;
                    }
                    if (num4 < this.ignoreCollisionDownSteps)
                    {
                        foreach (Collider collider4 in pair.Value)
                        {
                            foreach (Collider collider5 in dictionary[transform4.rigidbody])
                            {
                                set.Add(MakeKV(collider4, collider5));
                            }
                        }
                    }
                }
            }
        }
        int count = set.Count;
        if (count > 0)
        {
            Collider[] colliderArray2 = new Collider[count];
            Collider[] colliderArray3 = new Collider[count];
            int index = 0;
            foreach (KeyValuePair<Collider, Collider> pair2 in set)
            {
                colliderArray2[index] = pair2.Key;
                colliderArray3[index] = pair2.Value;
                index++;
            }
            IgnoreColliders colliders = instance.AddComponent<IgnoreColliders>();
            colliders.a = colliderArray2;
            colliders.b = colliderArray3;
        }
        this.CreateEyes(instance);
        if (this.ragdollPrototype != null)
        {
            this.ApplyPrototype(instance, this.ragdollPrototype);
        }
        this.ApplyRig(instance);
        return instance;
    }

    private GameObject MakeHitBoxPrefab()
    {
        GameObject obj5;
        try
        {
            SkinnedMeshRenderer renderer;
            int? nullable;
            GameObject instance = this.InstantiatePrefabWithRemovedBones((this.modelPrefabForHitBox == null) ? this.modelPrefab : this.modelPrefabForHitBox);
            instance.name = instance.name + "::HITBOX_OUTPUT::";
            this.ApplyMaterials(instance);
            AuthorShared.GetRootBone(instance, out renderer);
            Type[] components = new Type[] { typeof(CapsuleCollider), typeof(Rigidbody) };
            GameObject target = new GameObject("HB Hit", components) {
                layer = LayerMask.NameToLayer(this.hitBoxLayerName)
            };
            target.transform.parent = (renderer.transform.parent == null) ? instance.transform : renderer.transform.parent;
            CapsuleCollider collider = target.collider as CapsuleCollider;
            collider.center = this.hitCapsuleCenter;
            collider.height = this.hitCapsuleHeight;
            collider.radius = this.hitCapsuleRadius;
            collider.direction = this.hitCapsuleDirection;
            collider.isTrigger = false;
            collider.rigidbody.isKinematic = true;
            target.layer = LayerMask.NameToLayer("Hitbox");
            HitBoxSystem system = this.creatingSystem = this.CreateHitBoxSystem(target);
            if (system.bodyParts == null)
            {
                system.bodyParts = new IDRemoteBodyPartCollection();
            }
            List<HitShape> list = new List<HitShape>();
            if (string.IsNullOrEmpty(this.defaultBodyPartLayer))
            {
                nullable = null;
            }
            else
            {
                nullable = new int?(LayerMask.NameToLayer(this.defaultBodyPartLayer));
            }
            IEnumerator<AuthorPeice> enumerator = base.EnumeratePeices().GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    AuthorPeice current = enumerator.Current;
                    if ((current != null) && (current is AuthorChHit))
                    {
                        ((AuthorChHit) current).CreateHitBoxOn(list, instance.transform, this.modelPrefabInstance.transform, nullable);
                    }
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
            int num = 0;
            int count = list.Count;
            while (num < count)
            {
                if (list[num] == null)
                {
                    list.RemoveAt(num--);
                    count--;
                }
                num++;
            }
            list.Sort(HitShape.prioritySorter);
            system.shapes = list.ToArray();
            foreach (HitBox box in instance.GetComponentsInChildren<HitBox>())
            {
                try
                {
                    IDRemoteBodyPart part;
                    bool flag = system.bodyParts.TryGetValue(box.bodyPart, out part);
                    system.bodyParts[box.bodyPart] = box;
                    foreach (Collider collider2 in box.GetComponents<Collider>())
                    {
                        Object.DestroyImmediate(collider2);
                    }
                    if (flag)
                    {
                        Debug.LogWarning(string.Concat(new object[] { "Overwrite ", box.bodyPart, ". Was ", part, ", now ", box }), box);
                    }
                }
                catch (Exception exception)
                {
                    Debug.LogError(string.Format("{0}:{2}:{1}", box, exception, box.bodyPart));
                    throw;
                }
            }
            AuthorShared.SetDirty(system);
            this.CreateEyes(instance);
            IDMain main = null;
            main = this.ApplyPrototype(instance, this.prototype);
            this.ApplyRig(instance);
            AuthorShared.SetDirty(instance);
            obj5 = instance;
        }
        finally
        {
            this.creatingSystem = null;
        }
        return obj5;
    }

    private static KeyValuePair<Collider, Collider> MakeKV(Collider a, Collider b)
    {
        if (string.Compare(a.name, b.name) < 0)
        {
            return new KeyValuePair<Collider, Collider>(b, a);
        }
        return new KeyValuePair<Collider, Collider>(a, b);
    }

    private void OnDrawGizmosSelected()
    {
        if (this.modelPrefabInstance != null)
        {
            Gizmos.matrix = this.modelPrefabInstance.transform.localToWorldMatrix;
            Transform hitColliderParent = this.GetHitColliderParent(this.modelPrefabInstance);
            if (hitColliderParent != null)
            {
                Gizmos.matrix = hitColliderParent.localToWorldMatrix;
                Gizmos2.DrawWireCapsule(this.hitCapsuleCenter, this.hitCapsuleRadius, this.hitCapsuleHeight, this.hitCapsuleDirection);
            }
        }
    }

    protected override bool OnGUICreationSettings()
    {
        bool flag = base.OnGUICreationSettings();
        bool modelPrefab = (bool) this.modelPrefab;
        GameObject obj2 = (GameObject) AuthorShared.ObjectField("Model Prefab", this.modelPrefab, typeof(GameObject), true, new GUILayoutOption[0]);
        if (obj2 != this.modelPrefab)
        {
            if (obj2 == null)
            {
                obj2 = this.modelPrefab;
            }
            else if (AuthorShared.GetObjectKind(obj2) != AuthorShared.ObjectKind.Model)
            {
                obj2 = this.modelPrefab;
            }
            else
            {
                obj2 = AuthorShared.FindPrefabRoot(obj2);
            }
        }
        if (obj2 != this.modelPrefab)
        {
            this.modelPrefab = obj2;
            this.ChangedModelPrefab();
            this.ChangedEditingOptions();
            flag |= true;
        }
        bool enabled = GUI.enabled;
        if (!modelPrefab)
        {
            GUI.enabled = false;
        }
        bool modelPrefabForHitBox = (bool) this.modelPrefabForHitBox;
        GameObject obj3 = (GameObject) AuthorShared.ObjectField("Override Model Prefab [HitBox]", !modelPrefabForHitBox ? this.modelPrefab : this.modelPrefabForHitBox, typeof(GameObject), true, new GUILayoutOption[0]);
        GUI.enabled = enabled;
        if ((obj3 == null) || (obj3 == this.modelPrefab))
        {
            if (modelPrefab)
            {
                GUILayout.Label(guis.notOverridingContent, AuthorShared.Styles.miniLabel, new GUILayoutOption[0]);
            }
            obj3 = null;
        }
        else
        {
            GUILayout.Label(guis.overridingContent, AuthorShared.Styles.miniLabel, new GUILayoutOption[0]);
            bool flag5 = AuthorShared.Toggle("Use Meshes from Override in Ragdoll output", this.useMeshesFromHitBoxOnRagdoll, new GUILayoutOption[0]);
            if (flag5 != this.useMeshesFromHitBoxOnRagdoll)
            {
                this.useMeshesFromHitBoxOnRagdoll = flag5;
                flag = true;
            }
        }
        if (obj3 != this.modelPrefabForHitBox)
        {
            if (obj3 == null)
            {
                obj3 = this.modelPrefabForHitBox;
            }
            else if (AuthorShared.GetObjectKind(obj3) != AuthorShared.ObjectKind.Model)
            {
                obj3 = this.modelPrefabForHitBox;
            }
            else
            {
                obj3 = AuthorShared.FindPrefabRoot(obj3);
            }
        }
        if (obj3 != this.modelPrefabForHitBox)
        {
            this.modelPrefabForHitBox = obj3;
            flag |= true;
        }
        ActorRig actorRig = (ActorRig) AuthorShared.ObjectField("Actor Rig", this.actorRig, typeof(ActorRig), AuthorShared.ObjectFieldFlags.Asset, new GUILayoutOption[0]);
        if ((actorRig != this.actorRig) && (actorRig == null))
        {
            actorRig = this.actorRig;
        }
        if (actorRig != this.actorRig)
        {
            this.actorRig = actorRig;
            flag |= true;
        }
        Character prototype = (Character) AuthorShared.ObjectField("Prototype Prefab", this.prototype, typeof(IDMain), AuthorShared.ObjectFieldFlags.Prefab, new GUILayoutOption[0]);
        if (((prototype != this.prototype) && (prototype != null)) && (AuthorShared.GetObjectKind(prototype.gameObject) != AuthorShared.ObjectKind.Prefab))
        {
            prototype = this.prototype;
        }
        if (prototype != this.prototype)
        {
            this.prototype = prototype;
            flag |= true;
        }
        Ragdoll ragdollPrototype = (Ragdoll) AuthorShared.ObjectField("Prototype Ragdoll", this.ragdollPrototype, typeof(IDMain), AuthorShared.ObjectFieldFlags.Prefab, new GUILayoutOption[0]);
        if (((ragdollPrototype != this.ragdollPrototype) && (ragdollPrototype != null)) && (AuthorShared.GetObjectKind(ragdollPrototype.gameObject) != AuthorShared.ObjectKind.Prefab))
        {
            ragdollPrototype = this.ragdollPrototype;
        }
        if (ragdollPrototype != this.ragdollPrototype)
        {
            this.ragdollPrototype = ragdollPrototype;
            flag |= true;
        }
        if (this.modelPrefabInstance != null)
        {
            bool activeSelf = this.modelPrefabInstance.activeSelf;
            AuthorShared.BeginHorizontal(new GUILayoutOption[0]);
            if (AuthorShared.Toggle("Show Model Prefab", ref activeSelf, AuthorShared.Styles.miniButton, new GUILayoutOption[0]))
            {
                this.modelPrefabInstance.SetActive(activeSelf);
            }
            flag |= AuthorShared.Toggle("Render Bones", ref this.drawBones, AuthorShared.Styles.miniButton, new GUILayoutOption[0]);
            AuthorShared.EndHorizontal();
        }
        AuthorShared.BeginSubSection("Rendering", new GUILayoutOption[0]);
        if (<>f__am$cache26 == null)
        {
            <>f__am$cache26 = material => AuthorShared.ObjectField<Material>(new AuthorShared.Content(), ref material, typeof(Material), (AuthorShared.ObjectFieldFlags) 0, new GUILayoutOption[0]);
        }
        if (AuthorShared.ArrayField<Material>("Materials", ref this.materials, <>f__am$cache26))
        {
            flag = true;
            this.ApplyMaterials(this.modelPrefabInstance);
        }
        AuthorShared.EndSubSection();
        AuthorShared.BeginSubSection("Types", "AddComponent strings", new GUILayoutOption[0]);
        string str = AuthorShared.StringField("HitBox Type", this.hitBoxType, new GUILayoutOption[0]);
        string str2 = AuthorShared.StringField("HitBoxSystem Type", this.hitBoxSystemType, new GUILayoutOption[0]);
        AuthorShared.EndSubSection();
        AuthorShared.BeginSubSection("Hit Capsule", "Should be large enough to fit all boxes at any time", new GUILayoutOption[0]);
        Vector3 vector = AuthorShared.Vector3Field("Center", this.hitCapsuleCenter, new GUILayoutOption[0]);
        float num = AuthorShared.FloatField("Radius", this.hitCapsuleRadius, new GUILayoutOption[0]);
        float num2 = AuthorShared.FloatField("Height", this.hitCapsuleHeight, new GUILayoutOption[0]);
        int num3 = AuthorShared.IntField("Axis", this.hitCapsuleDirection, new GUILayoutOption[0]);
        float num4 = AuthorShared.FloatField("Eye Height", this.eyeHeight, new GUILayoutOption[0]);
        AuthorShared.EndSubSection();
        AuthorShared.BeginSubSection("Rigidbody", new GUILayoutOption[0]);
        flag |= AuthorShared.IntField("Ignore n. parent col.", ref this.ignoreCollisionUpSteps, new GUILayoutOption[0]);
        flag |= AuthorShared.IntField("Ignore n. child col.", ref this.ignoreCollisionDownSteps, new GUILayoutOption[0]);
        AuthorShared.EndSubSection();
        AuthorShared.BeginSubSection("Body Parts", new GUILayoutOption[0]);
        if (this.defaultBodyPartLayer == null)
        {
        }
        string str3 = AuthorShared.StringField("Default Hit Box Layer", string.Empty, new GUILayoutOption[0]);
        if (string.IsNullOrEmpty(this.defaultBodyPartLayer))
        {
            AuthorShared.Label("[the layer in the models will be used]", new GUILayoutOption[0]);
        }
        if (this.defaultBodyPartLayer == null)
        {
        }
        if (str3 != string.Empty)
        {
            this.defaultBodyPartLayer = str3;
            flag = true;
        }
        bool flag7 = (this.bodyParts.Count == 0) || AuthorShared.Toggle("Show Unassigned Parts", this.showAllBones, new GUILayoutOption[0]);
        for (BodyPart part = BodyPart.Undefined; part < (BodyPart.L_Cheek | BodyPart.Neck); part += 1)
        {
            Transform transform;
            if ((this.bodyParts.TryGetValue(part, out transform) || this.showAllBones) && AuthorShared.ObjectField<Transform>(part.ToString(), ref transform, AuthorShared.ObjectFieldFlags.Instance | AuthorShared.ObjectFieldFlags.AllowScene, new GUILayoutOption[0]))
            {
                if (transform != null)
                {
                    BodyPart? nullable = this.bodyParts.BodyPartOf(transform);
                    if (nullable.HasValue)
                    {
                        bool? nullable2 = AuthorShared.Ask(string.Concat(new object[] { "That transform was assigned do something else.\r\nChange it from ", nullable.Value, " to ", part, "?" }));
                        bool? nullable4 = !nullable2.HasValue ? null : new bool?(!nullable2.Value);
                        if (!nullable4.HasValue ? false : nullable4.Value)
                        {
                            continue;
                        }
                        this.bodyParts.Remove(nullable.Value);
                    }
                    this.bodyParts[part] = transform;
                }
                else
                {
                    this.bodyParts.Remove(part);
                }
                flag = true;
            }
        }
        this.showAllBones = flag7;
        AuthorShared.BeginSubSection("Destroy Children", new GUILayoutOption[0]);
        AuthorShared.BeginSubSection(guis.destroyDrop, "Remove these from generation", AuthorShared.Styles.miniLabel, new GUILayoutOption[0]);
        Transform context = (Transform) AuthorShared.ObjectField(null, typeof(Transform), AuthorShared.ObjectFieldFlags.Instance | AuthorShared.ObjectFieldFlags.Model | AuthorShared.ObjectFieldFlags.AllowScene, new GUILayoutOption[0]);
        AuthorShared.EndSubSection();
        if ((context != null) && ((this.modelPrefabInstance == null) || !context.IsChildOf(this.modelPrefabInstance.transform)))
        {
            Debug.Log("Thats not a valid selection", context);
            context = null;
        }
        bool flag8 = false;
        if ((this.removeThese != null) && (this.removeThese.Length > 0))
        {
            AuthorShared.BeginSubSection("These will be removed with generation", new GUILayoutOption[0]);
            for (int j = 0; j < this.removeThese.Length; j++)
            {
                AuthorShared.BeginHorizontal(AuthorShared.Styles.gradientOutline, new GUILayoutOption[0]);
                if (AuthorShared.Button(AuthorShared.ObjectContent<Transform>(this.removeThese[j], typeof(Transform)), AuthorShared.Styles.peiceButtonLeft, new GUILayoutOption[0]) && (this.removeThese[j] != null))
                {
                    AuthorShared.PingObject(this.removeThese[j]);
                }
                if (AuthorShared.Button(AuthorShared.Icon.delete, AuthorShared.Styles.peiceButtonRight, new GUILayoutOption[0]))
                {
                    this.removeThese[j] = null;
                    flag8 = true;
                }
                AuthorShared.EndHorizontal();
            }
            AuthorShared.EndSubSection();
        }
        AuthorShared.EndSubSection();
        AuthorShared.EndSubSection();
        AuthorShared.BeginSubSection("Output", "this is where stuff will be saved", new GUILayoutOption[0]);
        Object obj4 = AuthorShared.ObjectField("OUTPUT HITBOX", this.hitBoxOutputPrefab, typeof(GameObject), AuthorShared.ObjectFieldFlags.NotInstance | AuthorShared.ObjectFieldFlags.NotModel | AuthorShared.ObjectFieldFlags.Prefab, new GUILayoutOption[0]);
        Object obj5 = AuthorShared.ObjectField("OUTPUT RAGDOLL", this.ragdollOutputPrefab, typeof(GameObject), AuthorShared.ObjectFieldFlags.NotInstance | AuthorShared.ObjectFieldFlags.NotModel | AuthorShared.ObjectFieldFlags.Prefab, new GUILayoutOption[0]);
        AuthorShared.EndSubSection();
        AuthorShared.BeginSubSection("Authoring Helpers", "These do not output to the mesh, just are here to help you author", new GUILayoutOption[0]);
        Vector3 vector2 = AuthorShared.Vector3Field("Angles Offset", this.editingAngles, new GUILayoutOption[0]);
        bool flag9 = AuthorShared.Toggle("Origin To Root", this.editingCenterToRoot, new GUILayoutOption[0]);
        AuthorShared.EndSubSection();
        AuthorShared.BeginHorizontal(AuthorShared.Styles.box, new GUILayoutOption[0]);
        bool flag10 = GUI.enabled;
        if (obj2 == null)
        {
            GUI.enabled = false;
        }
        if (AuthorShared.Button("Generate", AuthorShared.Styles.miniButtonLeft, new GUILayoutOption[0]))
        {
            this.GeneratePrefabInstances();
            this.savedGenerated = false;
            AuthorShared.SetDirty(this);
            flag = true;
        }
        GUI.enabled = (((!this.savedGenerated && (this.generatedRigid != null)) && ((this.generatedHitBox != null) && (this.hitBoxOutputPrefab != null))) && (this.ragdollOutputPrefab != null)) && (this.ragdollOutputPrefab != this.hitBoxOutputPrefab);
        if (AuthorShared.Button("Update Prefabs", AuthorShared.Styles.miniButtonRight, new GUILayoutOption[0]) && (AuthorShared.Ask("This will overwrite any changes made to the output prefab that may have been done externally\r\nStill go ahead?") == true))
        {
            this.UpdatePrefabs();
            this.savedGenerated = true;
            flag = true;
        }
        GUI.enabled = flag10;
        AuthorShared.EndHorizontal();
        if (AuthorShared.Button("Save To JSON", new GUILayoutOption[0]))
        {
            base.SaveSettings();
        }
        if ((this.prototype != null) && AuthorShared.Button("Write JSON Serialized Properties from Prototype", new GUILayoutOption[0]))
        {
            this.PreviewPrototype();
        }
        if ((str != this.hitBoxType) || (str2 != this.hitBoxSystemType))
        {
            this.hitBoxType = str;
            this.hitBoxSystemType = str2;
            flag = true;
        }
        else if (((vector != this.hitCapsuleCenter) || (num != this.hitCapsuleRadius)) || (((num2 != this.hitCapsuleHeight) || (num3 != this.hitCapsuleDirection)) || (num4 != this.eyeHeight)))
        {
            this.hitCapsuleCenter = vector;
            this.hitCapsuleRadius = num;
            this.hitCapsuleHeight = num2;
            this.hitCapsuleDirection = num3;
            this.eyeHeight = num4;
            flag = true;
        }
        else if ((vector2 != this.editingAngles) || (this.editingCenterToRoot != flag9))
        {
            this.editingAngles = vector2;
            this.editingCenterToRoot = flag9;
            flag = true;
            this.ChangedEditingOptions();
        }
        else if (obj4 != this.hitBoxOutputPrefab)
        {
            if (this.EnsureItsAPrefab(ref obj4) && (obj4 != this.hitBoxOutputPrefab))
            {
                this.hitBoxOutputPrefab = (GameObject) obj4;
                flag = true;
            }
        }
        else if (obj5 != this.ragdollOutputPrefab)
        {
            if (this.EnsureItsAPrefab(ref obj5) && (obj5 != this.ragdollOutputPrefab))
            {
                this.ragdollOutputPrefab = (GameObject) obj5;
                flag = true;
            }
        }
        else if (context != null)
        {
            Array.Resize<Transform>(ref this.removeThese, (this.removeThese != null) ? (this.removeThese.Length + 1) : 1);
            this.removeThese[this.removeThese.Length - 1] = context;
            flag8 = true;
        }
        if (!flag8)
        {
            return flag;
        }
        int newSize = 0;
        for (int i = 0; i < this.removeThese.Length; i++)
        {
            if (this.removeThese[i] != null)
            {
                this.removeThese[newSize++] = this.removeThese[i];
            }
        }
        Array.Resize<Transform>(ref this.removeThese, newSize);
        return true;
    }

    [Conditional("EXPECT_CRASH")]
    private static void PreCrashLog(string text)
    {
        Debug.Log(text);
    }

    protected void PreviewPrototype()
    {
        AuthorCreationProject project;
        using (Stream stream = base.GetStream(true, "protoprev", out project))
        {
            if (stream != null)
            {
                using (JSONStream stream2 = JSONStream.CreateWriter(stream))
                {
                    if (stream2 == null)
                    {
                    }
                }
            }
        }
    }

    public override string RootBonePath(AuthorPeice callingPeice, Transform bone)
    {
        return AuthorShared.CalculatePath(bone, this.modelPrefabInstance.transform);
    }

    protected override void SaveSettings(JSONStream stream)
    {
        stream.WriteObjectStart();
        stream.WriteObjectStart("types");
        stream.WriteText("hitboxsystem", this.hitBoxSystemType);
        stream.WriteText("hitbox", this.hitBoxType);
        stream.WriteObjectEnd();
        stream.WriteObjectStart("assets");
        WriteJSONGUID(stream, "model", this.modelPrefabInstance);
        stream.WriteArrayStart("materials");
        if (this.materials != null)
        {
            for (int i = 0; i < this.materials.Length; i++)
            {
                WriteJSONGUID(stream, this.materials[i]);
            }
        }
        stream.WriteArrayEnd();
        stream.WriteObjectStart("bodyparts");
        foreach (BodyPartPair<Transform> pair in this.bodyParts)
        {
            string propertyName = pair.key.ToString();
            stream.WriteText(propertyName, AuthorShared.CalculatePath(pair.value, this.modelPrefabInstance.transform));
        }
        stream.WriteObjectEnd();
        stream.WriteArrayStart("peices");
        IEnumerator<AuthorPeice> enumerator = base.EnumeratePeices().GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                AuthorPeice current = enumerator.Current;
                stream.WriteObjectStart();
                stream.WriteText("type", current.GetType().AssemblyQualifiedName);
                stream.WriteText("id", current.peiceID);
                stream.WriteObjectStart("instance");
                current.SaveJsonProperties(stream);
                stream.WriteObjectEnd();
                stream.WriteObjectEnd();
            }
        }
        finally
        {
            if (enumerator == null)
            {
            }
            enumerator.Dispose();
        }
        stream.WriteArrayEnd();
        stream.WriteObjectEnd();
    }

    private void TransferComponentSettings(NavMeshAgent src, NavMeshAgent dst)
    {
        dst.radius = src.radius;
        dst.speed = src.speed;
        dst.acceleration = src.acceleration;
        dst.angularSpeed = src.angularSpeed;
        dst.stoppingDistance = src.stoppingDistance;
        dst.autoTraverseOffMeshLink = src.autoTraverseOffMeshLink;
        dst.autoRepath = src.autoRepath;
        dst.height = src.height;
        dst.baseOffset = src.baseOffset;
        dst.obstacleAvoidanceType = src.obstacleAvoidanceType;
        dst.walkableMask = src.walkableMask;
        dst.enabled = src.enabled;
    }

    private void TransferComponentSettings(GameObject srcGO, GameObject dstGO, Component[] srcComponents, Component[] dstComponents, Component src, Component dst)
    {
        if (!(src is MonoBehaviour) && (src is SkinnedMeshRenderer))
        {
            Debug.LogWarning("Cannot copy skinned mesh renderers");
        }
    }

    private void UpdatePrefabs()
    {
    }

    private static void WriteJSONGUID(JSONStream stream, Object obj)
    {
        string assemblyQualifiedName;
        string assetPath = AuthorShared.GetAssetPath(obj);
        string str2 = null;
        if (assetPath == string.Empty)
        {
            assetPath = null;
        }
        else
        {
            str2 = AuthorShared.PathToGUID(assetPath);
            if (string.IsNullOrEmpty(str2))
            {
                str2 = null;
            }
        }
        if (obj != null)
        {
            assemblyQualifiedName = obj.GetType().AssemblyQualifiedName;
        }
        else
        {
            assemblyQualifiedName = null;
        }
        stream.WriteObjectStart();
        stream.WriteText("path", assetPath);
        stream.WriteText("guid", str2);
        stream.WriteText("type", assemblyQualifiedName);
        stream.WriteObjectEnd();
    }

    private static void WriteJSONGUID(JSONStream stream, string property, Object obj)
    {
        stream.WriteProperty(property);
        WriteJSONGUID(stream, obj);
    }

    [CompilerGenerated]
    private sealed class <GetCollidersOnRigidbody>c__Iterator6 : IDisposable, IEnumerator, IEnumerable, IEnumerable<Collider>, IEnumerator<Collider>
    {
        internal Collider $current;
        internal int $PC;
        internal Rigidbody <$>rb;
        internal Collider[] <$s_33>__0;
        internal int <$s_34>__1;
        internal Collider <collider>__2;
        internal Rigidbody rb;

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
                    this.<$s_33>__0 = this.rb.GetComponentsInChildren<Collider>();
                    this.<$s_34>__1 = 0;
                    goto Label_0092;

                case 1:
                    break;

                default:
                    goto Label_00AC;
            }
        Label_0084:
            this.<$s_34>__1++;
        Label_0092:
            if (this.<$s_34>__1 < this.<$s_33>__0.Length)
            {
                this.<collider>__2 = this.<$s_33>__0[this.<$s_34>__1];
                if (this.<collider>__2.attachedRigidbody == this.rb)
                {
                    this.$current = this.<collider>__2;
                    this.$PC = 1;
                    return true;
                }
                goto Label_0084;
            }
            this.$PC = -1;
        Label_00AC:
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        IEnumerator<Collider> IEnumerable<Collider>.GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
            {
                return this;
            }
            return new AuthorHull.<GetCollidersOnRigidbody>c__Iterator6 { rb = this.<$>rb };
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.System.Collections.Generic.IEnumerable<UnityEngine.Collider>.GetEnumerator();
        }

        Collider IEnumerator<Collider>.Current
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

    private static class guis
    {
        public static readonly GUIContent destroyDrop = new GUIContent("Destroy bone", "Drag a transform off the model instance that contains no ::'s");
        public static readonly GUIContent notOverridingContent = new GUIContent("[both outputs will use the same base]", "The HitBox prefab output will use the same mesh prefab as the one for the rigidbody");
        public static readonly GUIContent overridingContent = new GUIContent("[overriding the hitbox output prefab]", "The HitBox prefab output will use the overriding mesh prefab provided, You must make sure that the heirarchy matches between the two");
    }

    private static class HitBoxItems
    {
        public static readonly AuthorPalletObject.Creator createSocketByID = new AuthorPalletObject.Creator(AuthorHull.HitBoxItems.CreateByID<AuthorChHit>);
        public static readonly AuthorPalletObject[] pallet;
        public static readonly AuthorPalletObject.Validator validateByID = new AuthorPalletObject.Validator(AuthorHull.HitBoxItems.ValidateByID);

        static HitBoxItems()
        {
            AuthorPalletObject[] objArray1 = new AuthorPalletObject[3];
            AuthorPalletObject obj2 = new AuthorPalletObject {
                guiContent = new GUIContent("Box"),
                validator = validateByID,
                creator = createSocketByID
            };
            objArray1[0] = obj2;
            obj2 = new AuthorPalletObject {
                guiContent = new GUIContent("Sphere"),
                validator = validateByID,
                creator = createSocketByID
            };
            objArray1[1] = obj2;
            obj2 = new AuthorPalletObject {
                guiContent = new GUIContent("Capsule"),
                validator = validateByID,
                creator = createSocketByID
            };
            objArray1[2] = obj2;
            pallet = objArray1;
        }

        private static AuthorPeice CreateByID<TPeice>(AuthorCreation creation, AuthorPalletObject palletObject) where TPeice: AuthorPeice
        {
            Type[] components = new Type[] { typeof(TPeice) };
            TPeice component = new GameObject(palletObject.guiContent.text, components).GetComponent<TPeice>();
            component.peiceID = palletObject.guiContent.text;
            return component;
        }

        private static bool ValidateByID(AuthorCreation creation, AuthorPalletObject palletObject)
        {
            return true;
        }
    }
}

