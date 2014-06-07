using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer)), InterfaceDriverComponent(typeof(IContextRequestable), "_contextRequestable", "contextRequestable", AlwaysSaveDisabled=true, SearchRoute=InterfaceSearchRoute.Parents | InterfaceSearchRoute.GameObject, AdditionalProperties="renderer;meshFilter")]
public class ContextSprite : MonoBehaviour
{
    [HideInInspector, SerializeField]
    private MonoBehaviour _contextRequestable;
    private MonoBehaviour contextRequestable;
    private bool denied;
    private static ContextSprite[] empty = new ContextSprite[0];
    private double fade;
    private static bool gInit;
    private const double kFadeDurationInFull = 0.15;
    private const double kFadeDurationOutFull = 0.15;
    private const double kFadeInRate = 8.0;
    private const double kFadeOutRate = 8.0;
    private const double kGhostFade = 0.15;
    private const double kMaxFade = 1.2;
    private const double kMinFade = 0.0;
    private const float kRayDistance = 5f;
    private float lastBoundFade = float.NegativeInfinity;
    private MaterialPropertyBlock materialProperties;
    [PrefetchComponent]
    public MeshFilter meshFilter;
    [PrefetchComponent]
    public MeshRenderer renderer;
    private IContextRequestable requestable;
    private bool requestableHasStatus;
    private bool requestableIsVisibility;
    private IContextRequestableStatus requestableStatus;
    private IContextRequestableVisibility requestableVisibility;
    private bool selfVisible;
    private float timeRemoved;
    private static readonly VisibleList visibleList = new VisibleList();

    public static IEnumerable<ContextSprite> AllVisibleForRequestable(IContextRequestableVisibility requestable)
    {
        MonoBehaviour behaviour;
        if ((g.visible.Count != 0) && ((behaviour = requestable as MonoBehaviour) != null))
        {
            return AllVisibleForRequestable(behaviour);
        }
        return empty;
    }

    [DebuggerHidden]
    private static IEnumerable<ContextSprite> AllVisibleForRequestable(MonoBehaviour requestable)
    {
        return new <AllVisibleForRequestable>c__Iterator38 { requestable = requestable, <$>requestable = requestable, $PC = -2 };
    }

    private void Awake()
    {
        this.contextRequestable = this._contextRequestable;
        if (this.contextRequestable == null)
        {
            if (!this.SearchForContextRequestable(out this.contextRequestable))
            {
                Debug.LogError("Could not locate a IContextRequestable! -- destroying self.(component)", base.gameObject);
                Object.Destroy(this);
                return;
            }
            Debug.LogWarning("Please set the interface in inspector! had to search for it!", this.contextRequestable);
        }
        else
        {
            this._contextRequestable = null;
        }
        this.requestable = this.contextRequestable as IContextRequestable;
        if (this.requestable == null)
        {
            Debug.LogError("Context Requestable is not a IContextRequestable", base.gameObject);
            Object.Destroy(this);
        }
        else
        {
            if (!base.transform.IsChildOf(this.contextRequestable.transform))
            {
                Debug.LogWarning(string.Format("Sprite for {0} is not a child of {0}.", this.contextRequestable), this);
            }
            this.requestableVisibility = this.contextRequestable as IContextRequestableVisibility;
            this.requestableIsVisibility = this.requestableVisibility != null;
            this.requestableStatus = this.contextRequestable as IContextRequestableStatus;
            this.requestableHasStatus = this.requestableStatus != null;
            this.renderer.SetPropertyBlock(this.materialProperties = new MaterialPropertyBlock());
        }
    }

    private static bool CalculateFadeDim(ref double fade, float elapsed)
    {
        if (fade < 0.15)
        {
            if (CalculateFadeIn(ref fade, elapsed))
            {
                if (fade > 0.15)
                {
                    fade = 0.15;
                }
                return true;
            }
        }
        else if ((fade > 0.15) && CalculateFadeOut(ref fade, elapsed))
        {
            if (fade < 0.15)
            {
                fade = 0.15;
            }
            return true;
        }
        return false;
    }

    private static bool CalculateFadeIn(ref double fade, float elapsed)
    {
        if (elapsed <= 0.0)
        {
            return false;
        }
        if (fade > 1.2)
        {
            fade = 1.2;
            return true;
        }
        if (fade == 1.2)
        {
            return false;
        }
        double num = ((double) elapsed) / 0.15;
        if ((1.2 - fade) <= num)
        {
            fade = 1.2;
        }
        else
        {
            fade += num;
        }
        return true;
    }

    private static bool CalculateFadeOut(ref double fade, float elapsed)
    {
        if (elapsed <= 0.0)
        {
            return false;
        }
        if (fade < 0.0)
        {
            fade = 0.0;
            return true;
        }
        if (fade == 0.0)
        {
            return false;
        }
        double num = ((double) elapsed) / 0.15;
        if (num >= fade)
        {
            fade = 0.0;
        }
        else
        {
            fade -= num;
        }
        return true;
    }

    private static bool CheckRelation(Collider collider, Rigidbody rigidbody, Transform self)
    {
        if (!collider.transform.IsChildOf(self) && !self.IsChildOf(collider.transform))
        {
            Rigidbody rigidbody2 = rigidbody;
            if (((rigidbody2 == null) || (collider.transform == rigidbody2.transform)) || (!rigidbody2.transform.IsChildOf(self) && !self.IsChildOf(rigidbody2.transform)))
            {
                return false;
            }
        }
        return true;
    }

    public static bool FindSprite(Component component, out ContextSprite sprite)
    {
        if (component is ContextSprite)
        {
            sprite = (ContextSprite) component;
            return true;
        }
        if (component is IContextRequestable)
        {
            sprite = component.GetComponentInChildren<ContextSprite>();
            return ((sprite != null) && (((sprite.contextRequestable == null) ? sprite._contextRequestable : sprite.contextRequestable) == component));
        }
        sprite = component.GetComponentInChildren<ContextSprite>();
        return (bool) sprite;
    }

    private bool IsSeeThrough(ref RaycastHit hit)
    {
        Transform transform;
        Transform parent = base.transform;
        if (this.contextRequestable != null)
        {
            Transform transform3 = this.contextRequestable.transform;
            if (parent != transform3)
            {
                if (parent.IsChildOf(transform3))
                {
                    parent = transform3;
                }
                else if (!transform3.IsChildOf(parent))
                {
                    transform = hit.collider.transform;
                    return ((((transform == transform3) || (transform == parent)) || transform.IsChildOf(parent)) || transform.IsChildOf(transform3));
                }
            }
        }
        transform = hit.collider.transform;
        return ((transform == parent) || transform.IsChildOf(parent));
    }

    private void OnBecameInvisible()
    {
        if (this.selfVisible)
        {
            this.selfVisible = false;
            g.Remove(this);
            if (this.requestableIsVisibility && (this.contextRequestable != null))
            {
                this.requestableVisibility.OnContextVisibilityChanged(this, false);
            }
        }
        else if (this.denied)
        {
            this.denied = false;
        }
    }

    private void OnBecameVisible()
    {
        if (!this.selfVisible)
        {
            g.Add(this);
            this.selfVisible = true;
            if (this.requestableIsVisibility && (this.contextRequestable != null))
            {
                this.requestableVisibility.OnContextVisibilityChanged(this, true);
            }
        }
    }

    private void OnDestroy()
    {
        try
        {
            this.OnBecameInvisible();
        }
        finally
        {
            this.contextRequestable = null;
            this.requestable = null;
            this.requestableVisibility = null;
            this.requestableIsVisibility = false;
            this.requestableStatus = null;
            this.requestableHasStatus = false;
        }
    }

    public static bool Raycast(Ray ray, out ContextSprite sprite)
    {
        bool flag = false;
        sprite = null;
        float positiveInfinity = float.PositiveInfinity;
        foreach (ContextSprite sprite2 in g.visible)
        {
            if (sprite2.contextRequestable != null)
            {
                float num3;
                Collider collider = sprite2.contextRequestable.collider;
                if (collider == null)
                {
                    collider = sprite2.collider;
                }
                if (collider != null)
                {
                    RaycastHit hit;
                    if (!collider.enabled)
                    {
                        continue;
                    }
                    if (sprite2.collider.Raycast(ray, out hit, 5f))
                    {
                        float distance = hit.distance;
                        distance *= distance;
                        if (distance < positiveInfinity)
                        {
                            flag = true;
                            positiveInfinity = distance;
                            sprite = sprite2;
                        }
                    }
                }
                if (sprite2.renderer.bounds.IntersectRay(ray, out num3))
                {
                    num3 *= num3;
                    if (num3 < positiveInfinity)
                    {
                        flag = true;
                        positiveInfinity = num3;
                        sprite = sprite2;
                    }
                }
            }
        }
        return flag;
    }

    private void Reset()
    {
        if (this.renderer == null)
        {
            this.renderer = base.renderer as MeshRenderer;
        }
        if (this.meshFilter == null)
        {
            this.meshFilter = base.GetComponent<MeshFilter>();
        }
        if ((this._contextRequestable == null) && !this.SearchForContextRequestable(out this._contextRequestable))
        {
            Debug.LogWarning("Please add a script implementing IContextRequestable on this or a parent game object", this);
        }
    }

    [DebuggerHidden]
    private IEnumerator Retry()
    {
        return new <Retry>c__Iterator37 { <>f__this = this };
    }

    private bool SearchForContextRequestable(out MonoBehaviour impl)
    {
        Contextual contextual;
        if (Contextual.FindUp(base.transform, out contextual) && ((impl = contextual.implementor) != null))
        {
            return true;
        }
        impl = null;
        return false;
    }

    private void UpdateMaterialProperties()
    {
        float num = Mathf.Clamp01((float) this.fade);
        if (num != this.lastBoundFade)
        {
            this.materialProperties.Clear();
            this.materialProperties.AddFloat(matHelper.fadeProp, num);
            this.lastBoundFade = num;
            this.renderer.SetPropertyBlock(this.materialProperties);
        }
    }

    public static void UpdateSpriteFading(Camera camera)
    {
        if (gInit && (camera != null))
        {
            g.Step(camera);
        }
    }

    public static VisibleList AllVisible
    {
        get
        {
            return visibleList;
        }
    }

    public static int layer
    {
        get
        {
            return layerinfo.index;
        }
    }

    public static int layerMask
    {
        get
        {
            return layerinfo.mask;
        }
    }

    [CompilerGenerated]
    private sealed class <AllVisibleForRequestable>c__Iterator38 : IDisposable, IEnumerator, IEnumerable, IEnumerable<ContextSprite>, IEnumerator<ContextSprite>
    {
        internal ContextSprite $current;
        internal int $PC;
        internal MonoBehaviour <$>requestable;
        internal HashSet<ContextSprite>.Enumerator <$s_398>__0;
        internal ContextSprite <sprite>__1;
        internal MonoBehaviour requestable;

        [DebuggerHidden]
        public void Dispose()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            switch (num)
            {
                case 1:
                    try
                    {
                    }
                    finally
                    {
                        this.<$s_398>__0.Dispose();
                    }
                    break;
            }
        }

        public bool MoveNext()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            bool flag = false;
            switch (num)
            {
                case 0:
                    this.<$s_398>__0 = ContextSprite.g.visible.GetEnumerator();
                    num = 0xfffffffd;
                    break;

                case 1:
                    break;

                default:
                    goto Label_00BE;
            }
            try
            {
                while (this.<$s_398>__0.MoveNext())
                {
                    this.<sprite>__1 = this.<$s_398>__0.Current;
                    if (this.<sprite>__1.contextRequestable == this.requestable)
                    {
                        this.$current = this.<sprite>__1;
                        this.$PC = 1;
                        flag = true;
                        return true;
                    }
                }
            }
            finally
            {
                if (!flag)
                {
                }
                this.<$s_398>__0.Dispose();
            }
            this.$PC = -1;
        Label_00BE:
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        IEnumerator<ContextSprite> IEnumerable<ContextSprite>.GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
            {
                return this;
            }
            return new ContextSprite.<AllVisibleForRequestable>c__Iterator38 { requestable = this.<$>requestable };
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.System.Collections.Generic.IEnumerable<ContextSprite>.GetEnumerator();
        }

        ContextSprite IEnumerator<ContextSprite>.Current
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

    [CompilerGenerated]
    private sealed class <Retry>c__Iterator37 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal ContextSprite <>f__this;

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
                    this.<>f__this.renderer.enabled = false;
                    this.$current = ContextSprite.r.wait;
                    this.$PC = 1;
                    return true;

                case 1:
                    this.<>f__this.renderer.enabled = true;
                    this.$PC = -1;
                    break;
            }
            return false;
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

    private static class g
    {
        private static int count;
        public static Queue<HashSet<ContextSprite>> hashRecycle = new Queue<HashSet<ContextSprite>>();
        private const int kMaxRecycleCount = 5;
        public static Dictionary<MonoBehaviour, HashSet<ContextSprite>> requestableVisibleSprites = new Dictionary<MonoBehaviour, HashSet<ContextSprite>>();
        public static HashSet<ContextSprite> visible = new HashSet<ContextSprite>();

        static g()
        {
            ContextSprite.gInit = true;
        }

        public static void Add(ContextSprite sprite)
        {
            HashSet<ContextSprite> set;
            visible.Add(sprite);
            count++;
            if (!requestableVisibleSprites.TryGetValue(sprite.contextRequestable, out set))
            {
                set = (hashRecycle.Count <= 0) ? new HashSet<ContextSprite>() : hashRecycle.Dequeue();
                requestableVisibleSprites[sprite.contextRequestable] = set;
            }
            set.Add(sprite);
            if (ContextSprite.CalculateFadeOut(ref sprite.fade, Time.time - sprite.timeRemoved))
            {
                sprite.UpdateMaterialProperties();
            }
        }

        private static bool PhysRaycast(ref Ray ray, out RaycastHit hit, float distanceTo, int layerMask)
        {
            if (Physics.Raycast(ray, out hit, distanceTo, layerMask))
            {
                Debug.DrawLine(ray.origin, ray.GetPoint(hit.distance), Color.green);
                Debug.DrawLine(ray.GetPoint(hit.distance), ray.GetPoint(distanceTo), Color.red);
                return true;
            }
            return false;
        }

        public static void Remove(ContextSprite sprite)
        {
            HashSet<ContextSprite> set;
            visible.Remove(sprite);
            count--;
            if (requestableVisibleSprites.TryGetValue(sprite.contextRequestable, out set))
            {
                if (set.Count == 1)
                {
                    set.Clear();
                    if (hashRecycle.Count < 5)
                    {
                        hashRecycle.Enqueue(set);
                    }
                    requestableVisibleSprites.Remove(sprite.contextRequestable);
                }
                else
                {
                    set.Remove(sprite);
                }
            }
            sprite.timeRemoved = Time.time;
        }

        public static void Step(Camera camera)
        {
            if (count > 0)
            {
                float deltaTime = Time.deltaTime;
                if (deltaTime > 0f)
                {
                    int layerMask = 0x80401;
                    if (RPOS.hideSprites)
                    {
                        foreach (ContextSprite sprite in visible)
                        {
                            if (ContextSprite.CalculateFadeOut(ref sprite.fade, deltaTime))
                            {
                                sprite.UpdateMaterialProperties();
                            }
                        }
                    }
                    else
                    {
                        foreach (ContextSprite sprite2 in visible)
                        {
                            Vector3 vector;
                            RaycastHit hit;
                            if (!sprite2.requestableHasStatus)
                            {
                                goto Label_0142;
                            }
                            ContextStatusFlags flags2 = sprite2.requestableStatus.ContextStatusPoll() & (ContextStatusFlags.SpriteFlag1 | ContextStatusFlags.SpriteFlag0);
                            if (flags2 != 0)
                            {
                                if (flags2 == ContextStatusFlags.SpriteFlag0)
                                {
                                    goto Label_0117;
                                }
                                if (flags2 == ContextStatusFlags.SpriteFlag1)
                                {
                                    goto Label_011F;
                                }
                                if (flags2 == (ContextStatusFlags.SpriteFlag1 | ContextStatusFlags.SpriteFlag0))
                                {
                                    goto Label_00F9;
                                }
                            }
                            bool flag = false;
                            goto Label_0145;
                        Label_00F9:
                            if (ContextSprite.CalculateFadeIn(ref sprite2.fade, deltaTime))
                            {
                                sprite2.UpdateMaterialProperties();
                            }
                            continue;
                        Label_0117:
                            flag = true;
                            goto Label_0145;
                        Label_011F:
                            if (ContextSprite.CalculateFadeOut(ref sprite2.fade, deltaTime))
                            {
                                sprite2.UpdateMaterialProperties();
                            }
                            continue;
                        Label_0142:
                            flag = false;
                        Label_0145:
                            vector = sprite2.transform.position;
                            Vector3 position = camera.WorldToScreenPoint(vector);
                            Ray ray = camera.ScreenPointToRay(position);
                            Vector3 direction = ray.direction;
                            Vector3 origin = ray.origin;
                            float distanceTo = (((vector.x * direction.x) + (vector.y * direction.y)) + (vector.z * direction.z)) - (((origin.x * direction.x) + (origin.y * direction.y)) + (origin.z * direction.z));
                            if (((distanceTo > 0f) && (!PhysRaycast(ref ray, out hit, distanceTo, layerMask) || sprite2.IsSeeThrough(ref hit))) ? (!flag ? ContextSprite.CalculateFadeIn(ref sprite2.fade, deltaTime) : ContextSprite.CalculateFadeDim(ref sprite2.fade, deltaTime)) : ContextSprite.CalculateFadeOut(ref sprite2.fade, deltaTime))
                            {
                                sprite2.UpdateMaterialProperties();
                            }
                        }
                    }
                }
            }
        }
    }

    private static class layerinfo
    {
        public static readonly int index = LayerMask.NameToLayer("Sprite");
        public static readonly int mask = (((int) 1) << index);
    }

    private static class matHelper
    {
        public static int fadeProp = Shader.PropertyToID("_Fade");
    }

    private static class r
    {
        public static WaitForEndOfFrame wait = new WaitForEndOfFrame();
    }

    public sealed class VisibleList : IEnumerable, IEnumerable<ContextSprite>
    {
        internal VisibleList()
        {
        }

        public bool Contains(ContextSprite sprite)
        {
            return (((sprite != null) && sprite.selfVisible) && ContextSprite.g.visible.Contains(sprite));
        }

        public HashSet<ContextSprite>.Enumerator GetEnumerator()
        {
            return ContextSprite.g.visible.GetEnumerator();
        }

        IEnumerator<ContextSprite> IEnumerable<ContextSprite>.GetEnumerator()
        {
            return ContextSprite.g.visible.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ContextSprite.g.visible.GetEnumerator();
        }

        public int Count
        {
            get
            {
                return ContextSprite.g.visible.Count;
            }
        }
    }
}

