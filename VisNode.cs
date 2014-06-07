using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;

[AddComponentMenu("Vis/Node")]
public class VisNode : IDLocal
{
    [NonSerialized]
    private bool __skipOnce_;
    [SerializeField]
    private VisClass _class;
    [NonSerialized]
    private VisClass.Handle _handle;
    [NonSerialized]
    private int _seeMask;
    [NonSerialized]
    private int _sightCurrentMask;
    [HideInInspector, SerializeField]
    private int _sightMask = -1;
    [HideInInspector, SerializeField]
    private int _spectMask = -1;
    [NonSerialized]
    private Vis.Stamp _stamp;
    [SerializeField, HideInInspector]
    private int _traitMask = 0x1000001;
    [NonSerialized, HideInInspector]
    private Transform _transform;
    [NonSerialized]
    private bool active;
    [NonSerialized]
    private bool anySeenTraitChanges;
    [NonSerialized]
    private bool awake;
    public bool blind;
    private static float bX;
    private static float bY;
    private static float bZ;
    private List<VisNode> cleanList;
    [NonSerialized]
    private bool dataConstructed;
    private static ObjectDB<VisNode> db = new ObjectDB<VisNode>();
    public bool deaf;
    private const int defaultUnobstructedLayers = 1;
    private static ODBSet<VisNode> disabledLastStep = new ODBSet<VisNode>();
    [SerializeField]
    private float distance = 10f;
    private static float dot;
    private static float DOT;
    [SerializeField]
    private float dotArc = 0.75f;
    [SerializeField]
    private float dotArcBegin;
    private static float dV;
    private static float dV2;
    private static float dX;
    private static float dY;
    private static float dZ;
    private ODBSet<VisNode> enter;
    private ODBSet<VisNode> exit;
    private static bool FALLBACK_TOO_CLOSE = false;
    private static float fW;
    private static float fX;
    private static float fY;
    private static float fZ;
    [NonSerialized]
    private bool hasStatusHandler;
    [NonSerialized]
    private TraitHistory histSeen;
    [NonSerialized]
    private TraitHistory histSight;
    [NonSerialized]
    private TraitHistory histSpect;
    [NonSerialized]
    private TraitHistory histTrait;
    internal ODBItem<VisNode> item;
    private static VisManager manager;
    public bool mute;
    private static float nX;
    private static float nY;
    private static float nZ;
    private static VisNode operandA;
    private static VisNode operandB;
    private static float planeDot;
    private static float PLANEDOTSIGHT;
    private static float pX;
    private static float pY;
    private static float pZ;
    private long queriesBitMask;
    [SerializeField, PrefetchComponent]
    private VisReactor reactor;
    private static ODBSet<VisNode> recentlyDisabled = new ODBSet<VisNode>();
    private VisMem sight;
    private static float SIGHT;
    private static float SIGHT2;
    private VisMem spect;
    private IVisHandler statusHandler;
    private static int temp_bTraits;

    private bool _CanSee(VisNode other)
    {
        return ((other.spect.count >= this.sight.count) ? this.sight.list.Contains(other) : other.spect.list.Contains(this));
    }

    protected void _CB_OnHidden_()
    {
        if (this.reactor != null)
        {
            this.reactor.SPECTATED_EXIT();
        }
    }

    protected void _CB_OnHiddenFrom_(VisNode spectator)
    {
        if (this.reactor != null)
        {
            this.reactor.SPECTATOR_REMOVE(spectator);
        }
    }

    protected void _CB_OnSeen_()
    {
        if (this.reactor != null)
        {
            this.reactor.SPECTATED_ENTER();
        }
    }

    protected void _CB_OnSeenBy_(VisNode spectator)
    {
        if (this.reactor != null)
        {
            this.reactor.SPECTATOR_ADD(spectator);
        }
    }

    private bool _IsSeenBy(VisNode other)
    {
        return ((other.sight.count >= this.spect.count) ? this.spect.list.Contains(other) : other.sight.list.Contains(this));
    }

    private void _REACTOR_SEE_ADD(ODBSibling<VisNode> sib)
    {
        while (sib.has)
        {
            VisNode self = sib.item.self;
            sib = sib.item.n;
            this.reactor.SEE_ADD(self);
        }
    }

    private void _REACTOR_SEE_REMOVE(ODBSibling<VisNode> sib)
    {
        while (sib.has)
        {
            VisNode self = sib.item.self;
            sib = sib.item.n;
            this.reactor.SEE_REMOVE(self);
        }
    }

    [Conditional("UNITY_EDITOR")]
    private static void _VALIDATE(VisNode vis)
    {
        if ((vis.sight.count > 0) != vis.sight.any)
        {
            Debug.LogError(string.Format("buzz {0} {1}", vis.sight.count, vis.sight.any), vis);
        }
        if (vis.sight.list.count != vis.sight.count)
        {
            Debug.LogError(string.Format("buzz {0} {1}", vis.sight.list.count, vis.sight.count), vis);
        }
        if ((vis.spect.count > 0) != vis.spect.any)
        {
            Debug.LogError(string.Format("buzz {0} {1}", vis.spect.count, vis.spect.any), vis);
        }
        if (vis.spect.list.count != vis.spect.count)
        {
            Debug.LogError(string.Format("buzz {0} {1}", vis.spect.list.count, vis.spect.count), vis);
        }
    }

    public static bool AreAware(VisNode instigator, VisNode target)
    {
        return (CanSee(instigator, target) && instigator._IsSeenBy(target));
    }

    public static bool AreOblivious(VisNode instigator, VisNode target)
    {
        return (!CanSee(instigator, target) && !instigator._IsSeenBy(target));
    }

    public bool AttentionMessage(string message)
    {
        return false;
    }

    public bool AttentionMessage(string message, object arg)
    {
        return false;
    }

    public static bool AttentionMessage(VisNode instigator, string message)
    {
        return AttentionMessage(instigator, message, null);
    }

    public static bool AttentionMessage(VisNode instigator, string message, object arg)
    {
        return false;
    }

    public bool AudibleMessage(float radius, string message)
    {
        if ((this.mute || !base.enabled) || (radius <= 0f))
        {
            return false;
        }
        DoAudibleMessage(this, this._stamp.position, radius, message, null);
        return true;
    }

    public bool AudibleMessage(float radius, string message, object arg)
    {
        if ((this.mute || !base.enabled) || (radius <= 0f))
        {
            return false;
        }
        DoAudibleMessage(this, this._stamp.position, radius, message, arg);
        return true;
    }

    public bool AudibleMessage(Vector3 point, float radius, string message)
    {
        if ((this.mute || !base.enabled) || (radius <= 0f))
        {
            return false;
        }
        DoAudibleMessage(this, point, radius, message, null);
        return true;
    }

    public static bool AudibleMessage(VisNode instigator, float radius, string message)
    {
        if (((instigator == null) || instigator.mute) || ((radius <= 0f) || !instigator.enabled))
        {
            return false;
        }
        DoAudibleMessage(instigator, instigator._stamp.position, radius, message, null);
        return true;
    }

    public bool AudibleMessage(Vector3 point, float radius, string message, object arg)
    {
        if ((this.mute || !base.enabled) || (radius <= 0f))
        {
            return false;
        }
        DoAudibleMessage(this, point, radius, message, arg);
        return true;
    }

    public static bool AudibleMessage(VisNode instigator, float radius, string message, object arg)
    {
        if (((instigator == null) || instigator.mute) || ((radius <= 0f) || !instigator.enabled))
        {
            return false;
        }
        DoAudibleMessage(instigator, instigator._stamp.position, radius, message, arg);
        return true;
    }

    public static bool AudibleMessage(VisNode instigator, Vector3 position, float radius, string message)
    {
        if (((instigator == null) || instigator.mute) || ((radius <= 0f) || !instigator.enabled))
        {
            return false;
        }
        DoAudibleMessage(instigator, position, radius, message, null);
        return true;
    }

    public static bool AudibleMessage(VisNode instigator, Vector3 position, float radius, string message, object arg)
    {
        if (((instigator == null) || instigator.mute) || ((radius <= 0f) || !instigator.enabled))
        {
            return false;
        }
        DoAudibleMessage(instigator, position, radius, message, arg);
        return true;
    }

    private void Awake()
    {
        this.awake = true;
        if (this._transform == null)
        {
            this._transform = base.transform;
        }
        if (base.enabled)
        {
            Debug.LogWarning("VisNode was enabled prior to awake. VisNode's enabled button should always be off when the game is not running");
            this.Register();
        }
        this.histSight.last = 0;
        this.histSpect.last = this._spectMask;
        this.histTrait.last = this._traitMask;
        this.statusHandler = base.idMain as IVisHandler;
        this.hasStatusHandler = this.statusHandler != null;
        if (this._class != null)
        {
            this._handle = this._class.handle;
        }
    }

    public bool CanSee(Vis.Life life)
    {
        return ((this._seeMask & life) == life);
    }

    public bool CanSee(Vis.Mask mask)
    {
        return ((this._seeMask & mask.data) == mask.data);
    }

    public bool CanSee(Vis.Role role)
    {
        return (((this._seeMask >> 0x18) & role) == role);
    }

    public bool CanSee(Vis.Status status)
    {
        return (((this._seeMask >> 8) & status) == status);
    }

    public bool CanSee(Vis.Trait trait)
    {
        return ((this._seeMask & (((int) 1) << trait)) != 0);
    }

    public bool CanSee(VisNode other)
    {
        return CanSee(this, other);
    }

    public static bool CanSee(VisNode instigator, VisNode target)
    {
        return ((instigator == target) || instigator._CanSee(target));
    }

    public bool CanSeeAny(Vis.Life life)
    {
        return ((this._seeMask & life) != 0);
    }

    public bool CanSeeAny(Vis.Mask mask)
    {
        return ((this._seeMask & mask.data) != 0);
    }

    public bool CanSeeAny(Vis.Role role)
    {
        return ((this._seeMask & (((int) role) << 0x18)) != 0);
    }

    public bool CanSeeAny(Vis.Status status)
    {
        return ((this._seeMask & (((int) status) << 8)) != 0);
    }

    public bool CanSeeOnly(Vis.Life life)
    {
        return ((this._seeMask & 7) == life);
    }

    public bool CanSeeOnly(Vis.Mask mask)
    {
        return (this._seeMask == mask.data);
    }

    public bool CanSeeOnly(Vis.Role role)
    {
        return ((this._seeMask & -16777216) == (((int) role) << 0x18));
    }

    public bool CanSeeOnly(Vis.Status status)
    {
        return ((this._seeMask & 0x7f00) == (((int) status) << 8));
    }

    public bool CanSeeOnly(Vis.Trait trait)
    {
        return (this._seeMask == (((int) 1) << trait));
    }

    public bool CanSeeUnobstructed(VisNode other)
    {
        return (this.CanSee(other) && this.Unobstructed(other));
    }

    private void CheckQueries()
    {
        this.histSeen.Upd(this._seeMask);
        if (this._handle.valid)
        {
            if (this.sight.rem)
            {
                this.DoQueryRem(this.exit.first);
            }
            if (this.anySeenTraitChanges || this.histTrait.changed)
            {
                this.DoQueryRemAdd(this.sight.list.first);
            }
            else if (this.sight.add)
            {
                this.DoQueryRemAdd(this.enter.first);
            }
        }
    }

    private void CheckReactions()
    {
        if (this.sight.rem)
        {
            this._REACTOR_SEE_REMOVE(this.exit.first);
            if (!this.sight.add && !this.sight.any)
            {
                this.reactor.AWARE_EXIT();
            }
        }
        if (this.sight.add)
        {
            if (!this.sight.had)
            {
                this.reactor.AWARE_ENTER();
            }
            this._REACTOR_SEE_ADD(this.enter.first);
        }
    }

    private static void ClearVis(ODBSibling<VisNode> iter)
    {
        do
        {
            operandA = iter.item.self;
            iter = iter.item.n;
            if (operandA.sight.any)
            {
                ODBSibling<VisNode> first = operandA.sight.last.first;
                do
                {
                    operandB = first.item.self;
                    first = first.item.n;
                    ResolveHide();
                }
                while (first.has);
                operandB = null;
            }
        }
        while (iter.has);
        operandA = null;
    }

    public static Vis.Comparison Compare(VisNode self, VisNode target)
    {
        if (self == target)
        {
            return Vis.Comparison.IsSelf;
        }
        if (self._CanSee(target))
        {
            if (self._IsSeenBy(target))
            {
                return Vis.Comparison.Contact;
            }
            return Vis.Comparison.Stealthy;
        }
        if (self._IsSeenBy(target))
        {
            return Vis.Comparison.Prey;
        }
        return Vis.Comparison.Oblivious;
    }

    public bool ComparisonMessage(Vis.Comparison comparison, string message)
    {
        return ComparisonMessage(this, comparison, message, null);
    }

    public bool ComparisonMessage(Vis.Comparison comparison, string message, object arg)
    {
        return ComparisonMessage(this, comparison, message, arg);
    }

    public static bool ComparisonMessage(VisNode instigator, Vis.Comparison comparison, string message)
    {
        return ComparisonMessage(instigator, comparison, message, null);
    }

    public static bool ComparisonMessage(VisNode instigator, Vis.Comparison comparison, string message, object arg)
    {
        switch (comparison)
        {
            case Vis.Comparison.Prey:
                return PreyMessage(instigator, message, arg);

            case Vis.Comparison.IsSelf:
                if ((instigator != null) && instigator.enabled)
                {
                    instigator.SendMessage(message, arg, SendMessageOptions.DontRequireReceiver);
                    return true;
                }
                return false;

            case Vis.Comparison.Contact:
                return ContactMessage(instigator, message, arg);

            case Vis.Comparison.Oblivious:
                return ObliviousMessage(instigator, message, arg);

            case Vis.Comparison.Stealthy:
                return StealthMessage(instigator, message, arg);
        }
        throw new ArgumentException(" do not know what to do with " + comparison, "comparison");
    }

    public bool ContactMessage(string message)
    {
        return false;
    }

    public bool ContactMessage(string message, object arg)
    {
        return false;
    }

    public static bool ContactMessage(VisNode instigator, string message)
    {
        return AttentionMessage(instigator, message, null);
    }

    public static bool ContactMessage(VisNode instigator, string message, object arg)
    {
        return false;
    }

    private static void Copy(ODBSet<VisNode> src, ODBSet<VisNode> dst)
    {
        dst.Clear();
        dst.UnionWith(src);
    }

    private static void DoAttentionMessage(VisNode instigator, string message, object arg)
    {
        RouteMessageHSet(instigator.sight.list, message, arg);
    }

    private static void DoAudibleMessage(VisNode instigator, Vector3 position, float radius, string message, object arg)
    {
        Search.Radial.Enumerator nodesInRadius = Vis.GetNodesInRadius(position, radius);
        if (!instigator.deaf)
        {
            while (nodesInRadius.MoveNext())
            {
                if (object.ReferenceEquals(nodesInRadius.Current, instigator))
                {
                    break;
                }
                nodesInRadius.Current.SendMessage(message, arg, SendMessageOptions.DontRequireReceiver);
            }
        }
        while (nodesInRadius.MoveNext())
        {
            nodesInRadius.Current.SendMessage(message, arg, SendMessageOptions.DontRequireReceiver);
        }
        nodesInRadius.Dispose();
    }

    private static void DoContactMessage(VisNode instigator, string message, object arg)
    {
        if (instigator.spect.count < instigator.sight.count)
        {
            RouteMessageOp(HSetOper.Intersect, instigator.spect.list, instigator.sight.list, message, arg);
        }
        else
        {
            RouteMessageOp(HSetOper.Intersect, instigator.sight.list, instigator.spect.list, message, arg);
        }
    }

    private static void DoGestureMessage(VisNode instigator, string message, object arg)
    {
        RouteMessageHSet(instigator.spect.list, message, arg);
    }

    private static void DoObliviousMessage(VisNode instigator, string message, object arg)
    {
        if (instigator.spect.count < instigator.sight.count)
        {
            RouteMessageOpUnionFirst(HSetOper.SymmetricExcept, instigator.spect.list, instigator.sight.list, db, message, arg);
        }
        else
        {
            RouteMessageOpUnionFirst(HSetOper.SymmetricExcept, instigator.sight.list, instigator.spect.list, db, message, arg);
        }
    }

    private static void DoPreyMessage(VisNode instigator, string message, object arg)
    {
        RouteMessageOp(HSetOper.Except, instigator.spect.list, instigator.sight.list, message, arg);
    }

    private void DoQueryRecurse(int i, VisNode other)
    {
        if (i < this._handle.Length)
        {
            VisQuery.Instance instance = this._handle[i];
            switch (instance.TryAdd(this, other))
            {
                case VisQuery.TryResult.Enter:
                    this.DoQueryRecurse(i + 1, other);
                    instance.ExecuteEnter(this, other);
                    return;

                case VisQuery.TryResult.Exit:
                    instance.ExecuteExit(this, other);
                    this.DoQueryRecurse(i + 1, other);
                    return;
            }
            this.DoQueryRecurse(i + 1, other);
        }
    }

    private void DoQueryRem(ODBSibling<VisNode> sib)
    {
        int num;
        if (this._handle.valid && ((num = this._handle.Length) > 0))
        {
            while (sib.has)
            {
                VisNode self = sib.item.self;
                sib = sib.item.n;
                for (int i = 0; i < num; i++)
                {
                    VisQuery.Instance instance = this._handle[i];
                    if (instance.TryRemove(this, self) == VisQuery.TryResult.Exit)
                    {
                        instance.ExecuteExit(this, self);
                    }
                }
            }
        }
    }

    private void DoQueryRemAdd(ODBSibling<VisNode> sib)
    {
        if (this._handle.valid && (this._handle.Length > 0))
        {
            while (sib.has)
            {
                VisNode self = sib.item.self;
                sib = sib.item.n;
                this.DoQueryRecurse(0, self);
            }
        }
    }

    private static void DoStealthMessage(VisNode instigator, string message, object arg)
    {
        RouteMessageOp(HSetOper.Except, instigator.sight.list, instigator.spect.list, message, arg);
    }

    private void DrawConnections(ODBSet<VisNode> list)
    {
        if (list != null)
        {
            ODBForwardEnumerator<VisNode> enumerator = list.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Vector3 position = enumerator.Current._stamp.position;
                Gizmos.DrawLine(this._stamp.position, position);
                Gizmos.DrawWireSphere(position, 0.5f);
            }
            enumerator.Dispose();
        }
    }

    private static void Finally()
    {
        if (disabledLastStep.any)
        {
            RunStamp(disabledLastStep.first);
            disabledLastStep.Clear();
        }
    }

    public bool GestureMessage(string message)
    {
        if (!base.enabled)
        {
            return false;
        }
        DoGestureMessage(this, message, null);
        return true;
    }

    public bool GestureMessage(string message, object arg)
    {
        if (!base.enabled)
        {
            return false;
        }
        DoGestureMessage(this, message, arg);
        return true;
    }

    public static bool GestureMessage(VisNode instigator, string message)
    {
        return GestureMessage(instigator, message, null);
    }

    public static bool GestureMessage(VisNode instigator, string message, object arg)
    {
        if ((instigator == null) || !instigator.enabled)
        {
            return false;
        }
        DoGestureMessage(instigator, message, arg);
        return true;
    }

    public static void GlobalMessage(string message, object arg)
    {
        using (ODBForwardEnumerator<VisNode> enumerator = db.GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                enumerator.Current.SendMessage(message, arg, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    public bool IsSeenBy(VisNode other)
    {
        return IsSeenBy(this, other);
    }

    public static bool IsSeenBy(VisNode instigator, VisNode target)
    {
        return ((instigator == target) || instigator._IsSeenBy(target));
    }

    public static bool IsStealthly(VisNode instigator, VisNode target)
    {
        return (CanSee(instigator, target) && !instigator._IsSeenBy(target));
    }

    private static bool LogicSight()
    {
        if (!operandB.active)
        {
            return false;
        }
        bX = operandB._stamp.position.x;
        bY = operandB._stamp.position.y;
        bZ = operandB._stamp.position.z;
        planeDot = ((bX * fX) + (bY * fY)) + (bZ * fZ);
        if ((planeDot < fW) || (planeDot > PLANEDOTSIGHT))
        {
            return false;
        }
        dX = bX - pX;
        dY = bY - pY;
        dZ = bZ - pZ;
        dV2 = ((dX * dX) + (dY * dY)) + (dZ * dZ);
        if (dV2 > SIGHT2)
        {
            return false;
        }
        if (dV2 < 4.203895E-45f)
        {
            return FALLBACK_TOO_CLOSE;
        }
        dV = Mathf.Sqrt(dV2);
        nX = dX / dV;
        nY = dY / dV;
        nZ = dZ / dV;
        dot = ((fX * nX) + (fY * nY)) + (fZ * nZ);
        return (DOT < dot);
    }

    public bool ObliviousMessage(string message)
    {
        if (!base.enabled)
        {
            return false;
        }
        ContactMessage(this, message, null);
        return true;
    }

    public bool ObliviousMessage(string message, object arg)
    {
        if (!base.enabled)
        {
            return false;
        }
        ContactMessage(this, message, arg);
        return true;
    }

    public static bool ObliviousMessage(VisNode instigator, string message)
    {
        return StealthMessage(instigator, message, null);
    }

    public static bool ObliviousMessage(VisNode instigator, string message, object arg)
    {
        if ((instigator == null) || !instigator.enabled)
        {
            return false;
        }
        DoObliviousMessage(instigator, message, arg);
        return true;
    }

    private void OnDestroy()
    {
        if (VisManager.guardedUpdate)
        {
            Debug.LogError("DESTROYING IN GUARDED UPDATE! " + base.name, this);
        }
        this.Unregister();
        RemoveNow(this);
    }

    private void OnDisable()
    {
        if (this.awake)
        {
            bool active = this.active;
            this.Unregister();
            if (active && !this.active)
            {
                recentlyDisabled.Add(this);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        VisGizmosUtility.ResetMatrixStack();
        Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
        this.DrawConnections(this.sight.list);
        Gizmos.color = new Color(0f, 0f, 1f, 0.5f);
        this.DrawConnections(this.spect.list);
        Transform transform = (this._transform == null) ? base.transform : this._transform;
        Gizmos.color = new Color(1f, 1f, 1f, 0.9f);
        Vector3 normalized = transform.forward.normalized;
        Vector3 position = transform.position;
        Vector3 to = position + ((Vector3) (normalized * this.distance));
        Gizmos.DrawLine(position, to);
        VisGizmosUtility.DrawDotArc(position, transform, this.distance, this.dotArc, this.dotArcBegin);
    }

    private void OnEnable()
    {
        if (this.awake)
        {
            this.Register();
        }
    }

    public bool PreyMessage(string message)
    {
        return false;
    }

    public static bool PreyMessage(VisNode instigator, string message)
    {
        return GestureMessage(instigator, message, null);
    }

    public static bool PreyMessage(VisNode instigator, string message, object arg)
    {
        return false;
    }

    public static void Process()
    {
        if (db.any)
        {
            if (recentlyDisabled.any)
            {
                RunStamp(db.first);
                RunStamp(recentlyDisabled.first);
                ClearVis(recentlyDisabled.first);
                UpdateVis(db.first);
                RunStat(recentlyDisabled.first);
                RunStat(db.first);
                RunHiddenCalls(recentlyDisabled.first);
                RunHiddenCalls(db.first);
                RunVoidSeenHiddenCalls(recentlyDisabled.last);
                RunVoidSeenHiddenCalls(db.last);
                RunSeenCalls(recentlyDisabled.first);
                RunSeenCalls(db.first);
                RunQueries(recentlyDisabled.last);
                RunQueries(db.last);
                Finally();
                SwapDisabled();
            }
            else
            {
                RunStamp(db.first);
                UpdateVis(db.first);
                RunStat(db.first);
                RunHiddenCalls(db.first);
                RunVoidSeenHiddenCalls(db.last);
                RunSeenCalls(db.first);
                RunQueries(db.last);
                Finally();
            }
        }
        else if (recentlyDisabled.any)
        {
            RunStamp(recentlyDisabled.first);
            ClearVis(recentlyDisabled.first);
            RunStat(recentlyDisabled.first);
            RunHiddenCalls(recentlyDisabled.first);
            RunVoidSeenHiddenCalls(recentlyDisabled.last);
            RunSeenCalls(recentlyDisabled.first);
            RunQueries(recentlyDisabled.last);
            Finally();
            SwapDisabled();
        }
    }

    private void Register()
    {
        if (this.awake && !this.active)
        {
            if (VisManager.guardedUpdate)
            {
                throw new InvalidOperationException("DO NOT INSTANTIATE WHILE VisibilityManager.isUpdatingVisibility!!");
            }
            if (manager == null)
            {
                Type[] components = new Type[] { typeof(VisManager) };
                manager = new GameObject("__Vis", components).GetComponent<VisManager>();
            }
            if (!this.dataConstructed)
            {
                this.sight.list = new ODBSet<VisNode>();
                this.sight.last = new ODBSet<VisNode>();
                this.spect.list = new ODBSet<VisNode>();
                this.spect.last = new ODBSet<VisNode>();
                this.enter = new ODBSet<VisNode>();
                this.exit = new ODBSet<VisNode>();
                this.cleanList = new List<VisNode>();
                this.dataConstructed = true;
            }
            else if (!recentlyDisabled.Remove(this))
            {
                disabledLastStep.Remove(this);
            }
            this.item = db.Register(this);
            this.active = this.item == this;
        }
    }

    private static void RemoveLinkNow(VisNode node, VisNode didSee)
    {
        if (node.sight.list.Remove(node))
        {
            node.sight.rem = true;
            didSee.spect.rem |= didSee.spect.list.Remove(node);
        }
        if (!node.sight.last.Remove(didSee))
        {
            node.enter.Remove(didSee);
        }
        else
        {
            didSee.spect.last.Remove(node);
        }
        if (--node.sight.count == 0)
        {
            node.sight.any = false;
        }
        if (--didSee.spect.count == 0)
        {
            didSee.spect.any = false;
        }
    }

    internal static void RemoveNow(VisNode node)
    {
        if (node.dataConstructed)
        {
            if (!recentlyDisabled.Remove(node))
            {
                disabledLastStep.Remove(node);
            }
            int num = 0;
            while (num < node.cleanList.Count)
            {
                node.cleanList[num].exit.Remove(node);
                num++;
            }
            ODBForwardEnumerator<VisNode> enumerator = node.exit.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.cleanList.Remove(node);
            }
            enumerator.Dispose();
            node.cleanList.Clear();
            node.cleanList.AddRange(node.sight.list);
            num = 0;
            while (num < node.cleanList.Count)
            {
                RemoveLinkNow(node, node.cleanList[num]);
                num++;
            }
            node.cleanList.Clear();
            node.cleanList.AddRange(node.spect.list);
            for (num = 0; num < node.cleanList.Count; num++)
            {
                RemoveLinkNow(node.cleanList[num], node);
            }
            node.cleanList.Clear();
        }
    }

    protected void Reset()
    {
        base.Reset();
        VisReactor component = base.GetComponent<VisReactor>();
        if (component != null)
        {
            this.reactor = component;
            this.reactor.__visNode = this;
        }
    }

    private static void ResolveHide()
    {
        if (operandA.sight.list.Remove(operandB))
        {
            operandB.spect.rem |= operandB.spect.list.Remove(operandA);
            operandA.exit.Add(operandB);
            operandB.cleanList.Add(operandA);
        }
    }

    private static void ResolveSee()
    {
        if (operandA.sight.list.Add(operandB))
        {
            operandB.spect.add |= operandB.spect.list.Add(operandA);
            operandA.sight.add = true;
            operandA.enter.Add(operandB);
        }
    }

    private static void RouteMessageHSet(ODBSet<VisNode> list, string msg, object arg)
    {
        if (list.any)
        {
            ODBSibling<VisNode> first = list.first;
            do
            {
                VisNode self = first.item.self;
                first = first.item.n;
                try
                {
                    self.SendMessage(msg, arg, SendMessageOptions.DontRequireReceiver);
                }
                catch (Exception exception)
                {
                    Debug.LogError(exception, self);
                }
            }
            while (first.has);
        }
    }

    private static void RouteMessageList(RecycleList<VisNode> list, string msg)
    {
        RouteMessageList(list, msg, null);
    }

    private static void RouteMessageList(RecycleList<VisNode> list, string msg, object arg)
    {
        using (RecycleListIter<VisNode> iter = list.MakeIter())
        {
            while (iter.MoveNext())
            {
                try
                {
                    iter.Current.SendMessage(msg, arg, SendMessageOptions.DontRequireReceiver);
                    continue;
                }
                catch (Exception exception)
                {
                    Debug.LogError(exception, iter.Current);
                    continue;
                }
            }
        }
    }

    private static void RouteMessageOp(HSetOper op, ODBSet<VisNode> a, IEnumerable<VisNode> b, string msg)
    {
        RouteMessageOp(op, a, b, msg, null);
    }

    private static void RouteMessageOp(HSetOper op, ODBSet<VisNode> a, IEnumerable<VisNode> b, string msg, object arg)
    {
        RecycleList<VisNode> list = a.OperList(op, b);
        RouteMessageList(list, msg, arg);
        list.Dispose();
    }

    private static void RouteMessageOpUnionFirst(HSetOper op, ODBSet<VisNode> a, ODBSet<VisNode> aa, IEnumerable<VisNode> b, string msg)
    {
        RouteMessageOpUnionFirst(op, a, aa, b, msg, null);
    }

    private static void RouteMessageOpUnionFirst(HSetOper op, ODBSet<VisNode> a, ODBSet<VisNode> aa, IEnumerable<VisNode> b, string msg, object arg)
    {
        ODBSet<VisNode> set = new ODBSet<VisNode>(a);
        set.UnionWith(aa);
        RouteMessageOp(op, set, b, msg, arg);
    }

    private static void RunHiddenCalls(ODBSibling<VisNode> sib)
    {
        do
        {
            operandA = sib.item.self;
            sib = sib.item.n;
            if (operandA.sight.rem)
            {
                ODBSibling<VisNode> first = operandA.exit.first;
                do
                {
                    operandB = first.item.self;
                    first = first.item.n;
                    operandB._CB_OnHiddenFrom_(operandA);
                }
                while (first.has);
                operandB = null;
            }
        }
        while (sib.has);
        operandA = null;
    }

    private static void RunQueries(ODBSibling<VisNode> sib)
    {
        do
        {
            operandA = sib.item.self;
            sib = sib.item.p;
            if (operandA.reactor != null)
            {
                operandA.CheckReactions();
            }
            operandA.CheckQueries();
        }
        while (sib.has);
        operandA = null;
    }

    private static void RunSeenCalls(ODBSibling<VisNode> sib)
    {
        do
        {
            operandA = sib.item.self;
            sib = sib.item.n;
            if (operandA.sight.add)
            {
                ODBSibling<VisNode> last = operandA.enter.last;
                do
                {
                    operandB = last.item.self;
                    last = last.item.p;
                    operandB._CB_OnSeenBy_(operandA);
                }
                while (last.has);
                operandB = null;
            }
        }
        while (sib.has);
        operandA = null;
    }

    private static void RunStamp(ODBSibling<VisNode> sib)
    {
        do
        {
            operandA = sib.item.self;
            sib = sib.item.n;
            operandA.Stamp();
        }
        while (sib.has);
        operandA = null;
    }

    private static void RunStat(ODBSibling<VisNode> sib)
    {
        do
        {
            operandA = sib.item.self;
            sib = sib.item.n;
            operandA.StatUpdate();
        }
        while (sib.has);
        operandA = null;
    }

    private static void RunVoidSeenHiddenCalls(ODBSibling<VisNode> sib)
    {
        do
        {
            operandA = sib.item.self;
            sib = sib.item.p;
            if (operandA.spect.had)
            {
                if (!operandA.spect.any)
                {
                    operandA._CB_OnHidden_();
                    operandA.spect.had = false;
                }
            }
            else if (operandA.spect.any)
            {
                operandA._CB_OnSeen_();
                operandA.spect.had = true;
            }
            operandA.sight.had = operandA.sight.any;
        }
        while (sib.has);
        operandA = null;
    }

    private void SeenHideFire()
    {
        if (this.spect.had != this.spect.any)
        {
            if (this.spect.any)
            {
                this._CB_OnSeen_();
            }
            else
            {
                this._CB_OnHidden_();
            }
            this.spect.had = this.spect.any;
        }
        this.sight.had = this.sight.any;
    }

    internal static void Stage1(VisNode self)
    {
        self.Stamp();
    }

    private void Stamp()
    {
        this._stamp.Collect(this._transform);
        Transfer(this.sight.list, this.sight.last, this.sight.add, this.sight.rem);
        Transfer(this.spect.list, this.spect.last, this.spect.add, this.spect.rem);
        if (this.sight.add)
        {
            this.enter.Clear();
            this.sight.add = false;
        }
        if (this.sight.rem)
        {
            this.exit.Clear();
            this.sight.rem = false;
        }
        this.spect.add = false;
        if (this.spect.rem)
        {
            this.spect.rem = false;
            this.cleanList.Clear();
        }
        if (this.hasStatusHandler)
        {
            this._traitMask = this.statusHandler.VisPoll(this.traitMask).data;
        }
        this.histTrait.Upd(this._traitMask);
        this._sightCurrentMask = 0;
        this.histSight.Upd(this._sightCurrentMask);
        this.histSpect.Upd(this._spectMask);
        this._seeMask = 0;
        this.anySeenTraitChanges = false;
    }

    private void StatUpdate()
    {
        this.sight.count = this.sight.list.count;
        this.sight.any = this.sight.count > 0;
        this.spect.count = this.spect.list.count;
        this.spect.any = this.spect.count > 0;
    }

    public bool StealthMessage(string message, object arg)
    {
        return false;
    }

    public static bool StealthMessage(VisNode instigator, string message)
    {
        return StealthMessage(instigator, message, null);
    }

    public static bool StealthMessage(VisNode instigator, string message, object arg)
    {
        return false;
    }

    private static void SwapDisabled()
    {
        ODBSet<VisNode> disabledLastStep = VisNode.disabledLastStep;
        VisNode.disabledLastStep = recentlyDisabled;
        recentlyDisabled = disabledLastStep;
    }

    private static void Transfer(ODBSet<VisNode> src, ODBSet<VisNode> dst, bool addAny, bool remAny)
    {
        if (addAny)
        {
            if (remAny)
            {
                Copy(src, dst);
            }
            else
            {
                dst.UnionWith(src);
            }
        }
        else if (remAny)
        {
            dst.ExceptWith(src);
        }
    }

    public bool Unobstructed(VisNode other)
    {
        return Physics.Linecast(this._stamp.position, other._stamp.position, 1);
    }

    private void Unregister()
    {
        if (this.active)
        {
            if (VisManager.guardedUpdate)
            {
                throw new InvalidOperationException("DO NOT OR DISABLE DESTROY WHILE VisibilityManager.isUpdatingVisibility!!");
            }
            db.Unregister(ref this.item);
            this.active = this.item == this;
        }
    }

    private static void UpdateVis(ODBSibling<VisNode> first_sib)
    {
        FALLBACK_TOO_CLOSE = false;
        ODBSibling<VisNode> n = first_sib;
    Label_0008:
        operandA = n.item.self;
        n = n.item.n;
        if (operandA._sightCurrentMask == 0)
        {
            if (operandA.sight.any)
            {
                ODBSibling<VisNode> sibling2 = operandA.sight.last.first;
                do
                {
                    operandB = sibling2.item.self;
                    sibling2 = sibling2.item.n;
                    ResolveHide();
                }
                while (sibling2.has);
                operandB = null;
            }
            goto Label_03F9;
        }
        pX = operandA._stamp.position.x;
        pY = operandA._stamp.position.y;
        pZ = operandA._stamp.position.z;
        fX = operandA._stamp.plane.x;
        fY = operandA._stamp.plane.y;
        fZ = operandA._stamp.plane.z;
        fW = operandA._stamp.plane.w;
        DOT = operandA.dotArc;
        SIGHT = operandA.distance;
        SIGHT2 = SIGHT * SIGHT;
        PLANEDOTSIGHT = fW + SIGHT;
        if (!operandA.sight.any)
        {
            goto Label_0354;
        }
        FALLBACK_TOO_CLOSE = true;
        ODBSibling<VisNode> first = operandA.sight.last.first;
        if (operandA.histSight.changed)
        {
            do
            {
                operandB = first.item.self;
                first = first.item.n;
                if (!operandB.active)
                {
                    ResolveHide();
                }
                else
                {
                    operandB.__skipOnce_ = true;
                    temp_bTraits = operandB._traitMask;
                    if (((temp_bTraits & operandA._sightCurrentMask) == 0) || !LogicSight())
                    {
                        ResolveHide();
                    }
                    else
                    {
                        operandA._seeMask |= temp_bTraits;
                    }
                }
            }
            while (first.has);
            goto Label_034E;
        }
        operandB = first.item.self;
    Label_027A:
        operandB = first.item.self;
        first = first.item.n;
        if (!operandB.active)
        {
            ResolveHide();
        }
        else
        {
            operandB.__skipOnce_ = true;
            temp_bTraits = operandB._traitMask;
            if (operandB.histTrait.changed)
            {
                if (((temp_bTraits & operandA._sightCurrentMask) == 0) || !LogicSight())
                {
                    ResolveHide();
                    goto Label_0342;
                }
                operandA.anySeenTraitChanges = true;
            }
            else if (!LogicSight())
            {
                ResolveHide();
                goto Label_0342;
            }
            operandA._seeMask |= temp_bTraits;
        }
    Label_0342:
        if (first.has)
        {
            goto Label_027A;
        }
    Label_034E:
        FALLBACK_TOO_CLOSE = false;
    Label_0354:
        operandA.__skipOnce_ = true;
        ODBSibling<VisNode> sibling4 = first_sib;
        do
        {
            operandB = sibling4.item.self;
            sibling4 = sibling4.item.n;
            if (operandB.__skipOnce_)
            {
                operandB.__skipOnce_ = false;
            }
            else
            {
                temp_bTraits = operandB._traitMask;
                if (((temp_bTraits & operandA._sightCurrentMask) != 0) && LogicSight())
                {
                    ResolveSee();
                    operandA._seeMask |= temp_bTraits;
                }
            }
        }
        while (sibling4.has);
        operandB = null;
    Label_03F9:
        if (n.has)
        {
            goto Label_0008;
        }
        operandA = null;
    }

    internal VisReactor __reactor
    {
        set
        {
            this.reactor = value;
        }
    }

    public bool anySight
    {
        get
        {
            return this.sight.any;
        }
    }

    public bool anySightHad
    {
        get
        {
            return this.sight.had;
        }
    }

    public bool anySightLost
    {
        get
        {
            return this.sight.rem;
        }
    }

    public bool anySightNew
    {
        get
        {
            return this.sight.add;
        }
    }

    public bool anySpectators
    {
        get
        {
            return this.spect.any;
        }
    }

    public bool anySpectatorsHad
    {
        get
        {
            return this.spect.had;
        }
    }

    public bool anySpectatorsLost
    {
        get
        {
            return this.spect.rem;
        }
    }

    public bool anySpectatorsNew
    {
        get
        {
            return this.spect.add;
        }
    }

    public float arc
    {
        get
        {
            return this.dotArc;
        }
        set
        {
            this.dotArc = Mathf.Clamp01(value);
        }
    }

    public Vector3 forward
    {
        get
        {
            return this._stamp.forward;
        }
    }

    public Transform head
    {
        get
        {
            return this._transform;
        }
        set
        {
            if (value != null)
            {
                this._transform = value;
            }
            else
            {
                this._transform = base.transform;
            }
        }
    }

    public int numSight
    {
        get
        {
            return this.sight.count;
        }
    }

    public int numSpectators
    {
        get
        {
            return this.spect.count;
        }
    }

    public Plane plane
    {
        get
        {
            Vector4 forward = this._stamp.forward;
            return new Plane(new Vector3(forward.x, forward.y, forward.z), forward.w);
        }
    }

    public Vector3 position
    {
        get
        {
            return this._stamp.position;
        }
    }

    public float radius
    {
        get
        {
            return this.distance;
        }
        set
        {
            this.distance = value;
        }
    }

    public Quaternion rotation
    {
        get
        {
            return this._stamp.rotation;
        }
    }

    public Vis.Mask seenMask
    {
        get
        {
            return new Vis.Mask { data = this._seeMask };
        }
    }

    public Vis.Mask spectMask
    {
        get
        {
            return new Vis.Mask { data = this._spectMask };
        }
        set
        {
            this._spectMask = value.data;
        }
    }

    public Vis.Stamp stamp
    {
        get
        {
            return this._stamp;
        }
    }

    public Vis.Mask traitMask
    {
        get
        {
            return new Vis.Mask { data = this._traitMask };
        }
        set
        {
            this._traitMask = value.data;
        }
    }

    public Vis.Mask viewMask
    {
        get
        {
            return new Vis.Mask { data = this._sightMask };
        }
        set
        {
            this._sightMask = value.data;
        }
    }

    public static class Search
    {
        public interface ISearch : IEnumerable, IEnumerable<VisNode>
        {
        }

        public interface ISearch<TEnumerator> : IEnumerable, VisNode.Search.ISearch, IEnumerable<VisNode> where TEnumerator: struct, IEnumerator<VisNode>
        {
            TEnumerator GetEnumerator();
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MaskCompareData
        {
            public Vis.Op op;
            public int mask;
            public MaskCompareData(Vis.Op op, Vis.Mask mask)
            {
                this.op = op;
                this.mask = mask.data;
            }

            public bool Pass(int mask)
            {
                return Vis.Evaluate(this.op, this.mask, mask);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Point : IEnumerable, VisNode.Search.ISearch, IEnumerable<VisNode>, VisNode.Search.ISearch<VisNode.Search.Point.Enumerator>
        {
            public Vector3 point;
            public Point(Vector3 point)
            {
                this.point = point;
            }

            IEnumerator<VisNode> IEnumerable<VisNode>.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            public Enumerator GetEnumerator()
            {
                return new Enumerator(new VisNode.Search.PointVisibilityData(this.point));
            }
            [StructLayout(LayoutKind.Sequential)]
            public struct Enumerator : IDisposable, IEnumerator, IEnumerator<VisNode>
            {
                public ODBForwardEnumerator<VisNode> e;
                public VisNode Current;
                private bool d;
                public VisNode.Search.PointVisibilityData data;
                public Enumerator(VisNode.Search.PointVisibilityData pv)
                {
                    this.Current = null;
                    this.d = false;
                    this.e = VisNode.db.GetEnumerator();
                    this.data = pv;
                }

                VisNode IEnumerator<VisNode>.Current
                {
                    get
                    {
                        return this.Current;
                    }
                }
                object IEnumerator.Current
                {
                    get
                    {
                        return this.Current;
                    }
                }
                public bool MoveNext()
                {
                    while (this.e.MoveNext())
                    {
                        if (this.Pass(this.e.Current))
                        {
                            return true;
                        }
                    }
                    this.Current = null;
                    return false;
                }

                public void Dispose()
                {
                    if (!this.d)
                    {
                        this.e.Dispose();
                        this.d = true;
                    }
                }

                public void Reset()
                {
                    this.Dispose();
                    this.d = false;
                    this.e = VisNode.db.GetEnumerator();
                }

                private bool Pass(VisNode cur)
                {
                    if (this.data.Pass(cur))
                    {
                        this.Current = cur;
                        return true;
                    }
                    return false;
                }
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct SightMasked : IEnumerable, VisNode.Search.ISearch, IEnumerable<VisNode>, VisNode.Search.ISearch<VisNode.Search.Point.SightMasked.Enumerator>
            {
                public Vector3 point;
                public VisNode.Search.MaskCompareData maskComp;
                public SightMasked(Vector3 point, Vis.Mask mask, Vis.Op op)
                {
                    this.point = point;
                    this.maskComp = new VisNode.Search.MaskCompareData(op, mask);
                }

                IEnumerator<VisNode> IEnumerable<VisNode>.GetEnumerator()
                {
                    return this.GetEnumerator();
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return this.GetEnumerator();
                }

                public Enumerator GetEnumerator()
                {
                    return new Enumerator(new VisNode.Search.PointVisibilityData(this.point), this.maskComp);
                }
                [StructLayout(LayoutKind.Sequential)]
                public struct Enumerator : IDisposable, IEnumerator, IEnumerator<VisNode>
                {
                    public ODBForwardEnumerator<VisNode> e;
                    public VisNode Current;
                    private bool d;
                    public VisNode.Search.PointVisibilityData data;
                    public VisNode.Search.MaskCompareData viewComp;
                    public Enumerator(VisNode.Search.PointVisibilityData pv, VisNode.Search.MaskCompareData mc)
                    {
                        this.Current = null;
                        this.d = false;
                        this.e = VisNode.db.GetEnumerator();
                        this.data = pv;
                        this.viewComp = mc;
                    }

                    VisNode IEnumerator<VisNode>.Current
                    {
                        get
                        {
                            return this.Current;
                        }
                    }
                    object IEnumerator.Current
                    {
                        get
                        {
                            return this.Current;
                        }
                    }
                    public bool MoveNext()
                    {
                        while (this.e.MoveNext())
                        {
                            if (this.Pass(this.e.Current))
                            {
                                return true;
                            }
                        }
                        this.Current = null;
                        return false;
                    }

                    public void Dispose()
                    {
                        if (!this.d)
                        {
                            this.e.Dispose();
                            this.d = true;
                        }
                    }

                    public void Reset()
                    {
                        this.Dispose();
                        this.d = false;
                        this.e = VisNode.db.GetEnumerator();
                    }

                    private bool Pass(VisNode cur)
                    {
                        if (this.viewComp.Pass(cur._sightMask) && this.data.Pass(cur))
                        {
                            this.Current = cur;
                            return true;
                        }
                        return false;
                    }
                }
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct TraitMasked : IEnumerable, VisNode.Search.ISearch, IEnumerable<VisNode>, VisNode.Search.ISearch<VisNode.Search.Point.TraitMasked.Enumerator>
            {
                public Vector3 point;
                public VisNode.Search.MaskCompareData maskComp;
                public TraitMasked(Vector3 point, Vis.Mask mask, Vis.Op op)
                {
                    this.point = point;
                    this.maskComp = new VisNode.Search.MaskCompareData(op, mask);
                }

                IEnumerator<VisNode> IEnumerable<VisNode>.GetEnumerator()
                {
                    return this.GetEnumerator();
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return this.GetEnumerator();
                }

                public Enumerator GetEnumerator()
                {
                    return new Enumerator(new VisNode.Search.PointVisibilityData(this.point), this.maskComp);
                }
                [StructLayout(LayoutKind.Sequential)]
                public struct Enumerator : IDisposable, IEnumerator, IEnumerator<VisNode>
                {
                    public ODBForwardEnumerator<VisNode> e;
                    public VisNode Current;
                    private bool d;
                    public VisNode.Search.PointVisibilityData data;
                    public VisNode.Search.MaskCompareData traitComp;
                    public Enumerator(VisNode.Search.PointVisibilityData pv, VisNode.Search.MaskCompareData mc)
                    {
                        this.Current = null;
                        this.d = false;
                        this.e = VisNode.db.GetEnumerator();
                        this.data = pv;
                        this.traitComp = mc;
                    }

                    VisNode IEnumerator<VisNode>.Current
                    {
                        get
                        {
                            return this.Current;
                        }
                    }
                    object IEnumerator.Current
                    {
                        get
                        {
                            return this.Current;
                        }
                    }
                    public bool MoveNext()
                    {
                        while (this.e.MoveNext())
                        {
                            if (this.Pass(this.e.Current))
                            {
                                return true;
                            }
                        }
                        this.Current = null;
                        return false;
                    }

                    public void Dispose()
                    {
                        if (!this.d)
                        {
                            this.e.Dispose();
                            this.d = true;
                        }
                    }

                    public void Reset()
                    {
                        this.Dispose();
                        this.d = false;
                        this.e = VisNode.db.GetEnumerator();
                    }

                    private bool Pass(VisNode cur)
                    {
                        if (this.traitComp.Pass(cur._traitMask) && this.data.Pass(cur))
                        {
                            this.Current = cur;
                            return true;
                        }
                        return false;
                    }
                }
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct Visual : IEnumerable, VisNode.Search.ISearch, IEnumerable<VisNode>, VisNode.Search.ISearch<VisNode.Search.Point.Visual.Enumerator>
            {
                public Vector3 point;
                public Visual(Vector3 point)
                {
                    this.point = point;
                }

                IEnumerator<VisNode> IEnumerable<VisNode>.GetEnumerator()
                {
                    return this.GetEnumerator();
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return this.GetEnumerator();
                }

                public Enumerator GetEnumerator()
                {
                    return new Enumerator(new VisNode.Search.PointVisibilityData(this.point));
                }
                [StructLayout(LayoutKind.Sequential)]
                public struct Enumerator : IDisposable, IEnumerator, IEnumerator<VisNode>
                {
                    public ODBForwardEnumerator<VisNode> e;
                    public VisNode Current;
                    private bool d;
                    public VisNode.Search.PointVisibilityData data;
                    public Enumerator(VisNode.Search.PointVisibilityData pv)
                    {
                        this.Current = null;
                        this.d = false;
                        this.e = VisNode.db.GetEnumerator();
                        this.data = pv;
                    }

                    VisNode IEnumerator<VisNode>.Current
                    {
                        get
                        {
                            return this.Current;
                        }
                    }
                    object IEnumerator.Current
                    {
                        get
                        {
                            return this.Current;
                        }
                    }
                    public bool MoveNext()
                    {
                        while (this.e.MoveNext())
                        {
                            if (this.Pass(this.e.Current))
                            {
                                return true;
                            }
                        }
                        this.Current = null;
                        return false;
                    }

                    public void Dispose()
                    {
                        if (!this.d)
                        {
                            this.e.Dispose();
                            this.d = true;
                        }
                    }

                    public void Reset()
                    {
                        this.Dispose();
                        this.d = false;
                        this.e = VisNode.db.GetEnumerator();
                    }

                    private bool Pass(VisNode cur)
                    {
                        return false;
                    }
                }

                [StructLayout(LayoutKind.Sequential)]
                public struct SightMasked : IEnumerable, VisNode.Search.ISearch, IEnumerable<VisNode>, VisNode.Search.ISearch<VisNode.Search.Point.Visual.SightMasked.Enumerator>
                {
                    public Vector3 point;
                    public VisNode.Search.MaskCompareData maskComp;
                    public SightMasked(Vector3 point, Vis.Mask mask, Vis.Op op)
                    {
                        this.point = point;
                        this.maskComp = new VisNode.Search.MaskCompareData(op, mask);
                    }

                    IEnumerator<VisNode> IEnumerable<VisNode>.GetEnumerator()
                    {
                        return this.GetEnumerator();
                    }

                    IEnumerator IEnumerable.GetEnumerator()
                    {
                        return this.GetEnumerator();
                    }

                    public Enumerator GetEnumerator()
                    {
                        return new Enumerator(new VisNode.Search.PointVisibilityData(this.point), this.maskComp);
                    }
                    [StructLayout(LayoutKind.Sequential)]
                    public struct Enumerator : IDisposable, IEnumerator, IEnumerator<VisNode>
                    {
                        public ODBForwardEnumerator<VisNode> e;
                        public VisNode Current;
                        private bool d;
                        public VisNode.Search.PointVisibilityData data;
                        public VisNode.Search.MaskCompareData viewComp;
                        public Enumerator(VisNode.Search.PointVisibilityData pv, VisNode.Search.MaskCompareData mc)
                        {
                            this.Current = null;
                            this.d = false;
                            this.e = VisNode.db.GetEnumerator();
                            this.data = pv;
                            this.viewComp = mc;
                        }

                        VisNode IEnumerator<VisNode>.Current
                        {
                            get
                            {
                                return this.Current;
                            }
                        }
                        object IEnumerator.Current
                        {
                            get
                            {
                                return this.Current;
                            }
                        }
                        public bool MoveNext()
                        {
                            while (this.e.MoveNext())
                            {
                                if (this.Pass(this.e.Current))
                                {
                                    return true;
                                }
                            }
                            this.Current = null;
                            return false;
                        }

                        public void Dispose()
                        {
                            if (!this.d)
                            {
                                this.e.Dispose();
                                this.d = true;
                            }
                        }

                        public void Reset()
                        {
                            this.Dispose();
                            this.d = false;
                            this.e = VisNode.db.GetEnumerator();
                        }

                        private bool Pass(VisNode cur)
                        {
                            return false;
                        }
                    }
                }

                [StructLayout(LayoutKind.Sequential)]
                public struct TraitMasked : IEnumerable, VisNode.Search.ISearch, IEnumerable<VisNode>, VisNode.Search.ISearch<VisNode.Search.Point.Visual.TraitMasked.Enumerator>
                {
                    public Vector3 point;
                    public VisNode.Search.MaskCompareData maskComp;
                    public TraitMasked(Vector3 point, Vis.Mask mask, Vis.Op op)
                    {
                        this.point = point;
                        this.maskComp = new VisNode.Search.MaskCompareData(op, mask);
                    }

                    IEnumerator<VisNode> IEnumerable<VisNode>.GetEnumerator()
                    {
                        return this.GetEnumerator();
                    }

                    IEnumerator IEnumerable.GetEnumerator()
                    {
                        return this.GetEnumerator();
                    }

                    public Enumerator GetEnumerator()
                    {
                        return new Enumerator(new VisNode.Search.PointVisibilityData(this.point), this.maskComp);
                    }
                    [StructLayout(LayoutKind.Sequential)]
                    public struct Enumerator : IDisposable, IEnumerator, IEnumerator<VisNode>
                    {
                        public ODBForwardEnumerator<VisNode> e;
                        public VisNode Current;
                        private bool d;
                        public VisNode.Search.PointVisibilityData data;
                        public VisNode.Search.MaskCompareData traitComp;
                        public Enumerator(VisNode.Search.PointVisibilityData pv, VisNode.Search.MaskCompareData mc)
                        {
                            this.Current = null;
                            this.d = false;
                            this.e = VisNode.db.GetEnumerator();
                            this.data = pv;
                            this.traitComp = mc;
                        }

                        VisNode IEnumerator<VisNode>.Current
                        {
                            get
                            {
                                return this.Current;
                            }
                        }
                        object IEnumerator.Current
                        {
                            get
                            {
                                return this.Current;
                            }
                        }
                        public bool MoveNext()
                        {
                            while (this.e.MoveNext())
                            {
                                if (this.Pass(this.e.Current))
                                {
                                    return true;
                                }
                            }
                            this.Current = null;
                            return false;
                        }

                        public void Dispose()
                        {
                            if (!this.d)
                            {
                                this.e.Dispose();
                                this.d = true;
                            }
                        }

                        public void Reset()
                        {
                            this.Dispose();
                            this.d = false;
                            this.e = VisNode.db.GetEnumerator();
                        }

                        private bool Pass(VisNode cur)
                        {
                            return false;
                        }
                    }
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PointRadiusData
        {
            public float radiusSquare;
            public float x;
            public float y;
            public float z;
            public float dX;
            public float dY;
            public float dZ;
            public float d2;
            public PointRadiusData(Vector3 pos, float radius)
            {
                this.x = pos.x;
                this.y = pos.y;
                this.z = pos.z;
                this.radiusSquare = radius * radius;
                this.dX = 0f;
                this.dY = 0f;
                this.dZ = 0f;
                this.d2 = 0f;
            }

            public bool Pass(VisNode current)
            {
                this.dX = this.x - current._stamp.position.x;
                this.dY = this.y - current._stamp.position.y;
                this.dZ = this.z - current._stamp.position.z;
                this.d2 = ((this.dX * this.dX) + (this.dY * this.dY)) + (this.dZ * this.dZ);
                return (this.d2 <= this.radiusSquare);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PointRadiusMaskData
        {
            public VisNode.Search.PointRadiusData pr;
            public VisNode.Search.MaskCompareData mc;
            public PointRadiusMaskData(Vector3 pos, float radius, Vis.Op op, Vis.Mask mask) : this(new VisNode.Search.PointRadiusData(pos, radius), new VisNode.Search.MaskCompareData(op, mask))
            {
            }

            public PointRadiusMaskData(VisNode.Search.PointRadiusData pr, VisNode.Search.MaskCompareData mc)
            {
                this.pr = pr;
                this.mc = mc;
            }

            public bool Pass(VisNode current, int mask)
            {
                return (this.mc.Pass(mask) && this.pr.Pass(current));
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PointVisibilityData
        {
            public float x;
            public float y;
            public float z;
            public float dX;
            public float dY;
            public float dZ;
            public float d2;
            public float d;
            public float nX;
            public float nY;
            public float nZ;
            public float radius;
            public float radiusSquare;
            public PointVisibilityData(Vector3 point)
            {
                this.x = point.x;
                this.y = point.y;
                this.z = point.z;
                this.dX = 0f;
                this.dY = 0f;
                this.dZ = 0f;
                this.d2 = 0f;
                this.d = 0f;
                this.nX = 0f;
                this.nY = 0f;
                this.nZ = 0f;
                this.radius = 0f;
                this.radiusSquare = 0f;
            }

            public bool Pass(VisNode Current)
            {
                this.radius = Current.distance;
                this.radiusSquare *= this.radiusSquare;
                this.dX = this.x - Current._stamp.position.x;
                this.dY = this.y - Current._stamp.position.y;
                this.dZ = this.z - Current._stamp.position.z;
                this.d2 = ((this.dX * this.dX) + (this.dY * this.dY)) + (this.dZ * this.dZ);
                if (this.d2 < 4.203895E-45f)
                {
                    return true;
                }
                this.d = Mathf.Sqrt(this.d2);
                this.nX = this.dX / this.d;
                this.nY = this.dY / this.d;
                this.nZ = this.dZ / this.d;
                VisNode.dot = ((Current._stamp.plane.x * this.nX) + (Current._stamp.plane.y * this.nY)) + (Current._stamp.plane.z * this.nZ);
                return (VisNode.dot >= Current.dotArc);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Radial : IEnumerable, VisNode.Search.ISearch, IEnumerable<VisNode>, VisNode.Search.ISearch<VisNode.Search.Radial.Enumerator>
        {
            public Vector3 point;
            public float radius;
            public Radial(Vector3 point, float radius)
            {
                this.point = point;
                this.radius = radius;
            }

            IEnumerator<VisNode> IEnumerable<VisNode>.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            public Enumerator GetEnumerator()
            {
                return new Enumerator(new VisNode.Search.PointRadiusData(this.point, this.radius));
            }
            [StructLayout(LayoutKind.Sequential)]
            public struct Audible : IEnumerable, VisNode.Search.ISearch, IEnumerable<VisNode>, VisNode.Search.ISearch<VisNode.Search.Radial.Audible.Enumerator>
            {
                public Vector3 point;
                public float radius;
                public Audible(Vector3 point, float radius)
                {
                    this.point = point;
                    this.radius = radius;
                }

                IEnumerator<VisNode> IEnumerable<VisNode>.GetEnumerator()
                {
                    return this.GetEnumerator();
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return this.GetEnumerator();
                }

                public Enumerator GetEnumerator()
                {
                    return new Enumerator(new VisNode.Search.PointRadiusData(this.point, this.radius));
                }
                [StructLayout(LayoutKind.Sequential)]
                public struct Enumerator : IDisposable, IEnumerator, IEnumerator<VisNode>
                {
                    public ODBForwardEnumerator<VisNode> e;
                    public VisNode Current;
                    private bool d;
                    public VisNode.Search.PointRadiusData data;
                    public Enumerator(VisNode.Search.PointRadiusData pr)
                    {
                        this.Current = null;
                        this.d = false;
                        this.e = VisNode.db.GetEnumerator();
                        this.data = pr;
                    }

                    VisNode IEnumerator<VisNode>.Current
                    {
                        get
                        {
                            return this.Current;
                        }
                    }
                    object IEnumerator.Current
                    {
                        get
                        {
                            return this.Current;
                        }
                    }
                    public bool MoveNext()
                    {
                        while (this.e.MoveNext())
                        {
                            if (this.Pass(this.e.Current))
                            {
                                return true;
                            }
                        }
                        this.Current = null;
                        return false;
                    }

                    public void Dispose()
                    {
                        if (!this.d)
                        {
                            this.e.Dispose();
                            this.d = true;
                        }
                    }

                    public void Reset()
                    {
                        this.Dispose();
                        this.d = false;
                        this.e = VisNode.db.GetEnumerator();
                    }

                    private bool Pass(VisNode cur)
                    {
                        if (!cur.deaf && this.data.Pass(cur))
                        {
                            this.Current = cur;
                            return true;
                        }
                        return false;
                    }
                }

                [StructLayout(LayoutKind.Sequential)]
                public struct SightMasked : IEnumerable, VisNode.Search.ISearch, IEnumerable<VisNode>, VisNode.Search.ISearch<VisNode.Search.Radial.Audible.SightMasked.Enumerator>
                {
                    public Vector3 point;
                    public float radius;
                    public VisNode.Search.MaskCompareData maskComp;
                    public SightMasked(Vector3 point, float radius, Vis.Mask mask, Vis.Op op)
                    {
                        this.point = point;
                        this.radius = radius;
                        this.maskComp = new VisNode.Search.MaskCompareData(op, mask);
                    }

                    IEnumerator<VisNode> IEnumerable<VisNode>.GetEnumerator()
                    {
                        return this.GetEnumerator();
                    }

                    IEnumerator IEnumerable.GetEnumerator()
                    {
                        return this.GetEnumerator();
                    }

                    public Enumerator GetEnumerator()
                    {
                        return new Enumerator(new VisNode.Search.PointRadiusData(this.point, this.radius), this.maskComp);
                    }
                    [StructLayout(LayoutKind.Sequential)]
                    public struct Enumerator : IDisposable, IEnumerator, IEnumerator<VisNode>
                    {
                        public ODBForwardEnumerator<VisNode> e;
                        public VisNode Current;
                        private bool d;
                        public VisNode.Search.PointRadiusData data;
                        public VisNode.Search.MaskCompareData viewComp;
                        public Enumerator(VisNode.Search.PointRadiusData pr, VisNode.Search.MaskCompareData mc)
                        {
                            this.Current = null;
                            this.d = false;
                            this.e = VisNode.db.GetEnumerator();
                            this.data = pr;
                            this.viewComp = mc;
                        }

                        VisNode IEnumerator<VisNode>.Current
                        {
                            get
                            {
                                return this.Current;
                            }
                        }
                        object IEnumerator.Current
                        {
                            get
                            {
                                return this.Current;
                            }
                        }
                        public bool MoveNext()
                        {
                            while (this.e.MoveNext())
                            {
                                if (this.Pass(this.e.Current))
                                {
                                    return true;
                                }
                            }
                            this.Current = null;
                            return false;
                        }

                        public void Dispose()
                        {
                            if (!this.d)
                            {
                                this.e.Dispose();
                                this.d = true;
                            }
                        }

                        public void Reset()
                        {
                            this.Dispose();
                            this.d = false;
                            this.e = VisNode.db.GetEnumerator();
                        }

                        private bool Pass(VisNode cur)
                        {
                            if ((!cur.deaf && this.viewComp.Pass(cur._sightMask)) && this.data.Pass(cur))
                            {
                                this.Current = cur;
                                return true;
                            }
                            return false;
                        }
                    }
                }

                [StructLayout(LayoutKind.Sequential)]
                public struct TraitMasked : IEnumerable, VisNode.Search.ISearch, IEnumerable<VisNode>, VisNode.Search.ISearch<VisNode.Search.Radial.Audible.TraitMasked.Enumerator>
                {
                    public Vector3 point;
                    public float radius;
                    public VisNode.Search.MaskCompareData maskComp;
                    public TraitMasked(Vector3 point, float radius, Vis.Mask mask, Vis.Op op)
                    {
                        this.point = point;
                        this.radius = radius;
                        this.maskComp = new VisNode.Search.MaskCompareData(op, mask);
                    }

                    IEnumerator<VisNode> IEnumerable<VisNode>.GetEnumerator()
                    {
                        return this.GetEnumerator();
                    }

                    IEnumerator IEnumerable.GetEnumerator()
                    {
                        return this.GetEnumerator();
                    }

                    public Enumerator GetEnumerator()
                    {
                        return new Enumerator(new VisNode.Search.PointRadiusData(this.point, this.radius), this.maskComp);
                    }
                    [StructLayout(LayoutKind.Sequential)]
                    public struct Enumerator : IDisposable, IEnumerator, IEnumerator<VisNode>
                    {
                        public ODBForwardEnumerator<VisNode> e;
                        public VisNode Current;
                        private bool d;
                        public VisNode.Search.PointRadiusData data;
                        public VisNode.Search.MaskCompareData traitComp;
                        public Enumerator(VisNode.Search.PointRadiusData pr, VisNode.Search.MaskCompareData mc)
                        {
                            this.Current = null;
                            this.d = false;
                            this.e = VisNode.db.GetEnumerator();
                            this.data = pr;
                            this.traitComp = mc;
                        }

                        VisNode IEnumerator<VisNode>.Current
                        {
                            get
                            {
                                return this.Current;
                            }
                        }
                        object IEnumerator.Current
                        {
                            get
                            {
                                return this.Current;
                            }
                        }
                        public bool MoveNext()
                        {
                            while (this.e.MoveNext())
                            {
                                if (this.Pass(this.e.Current))
                                {
                                    return true;
                                }
                            }
                            this.Current = null;
                            return false;
                        }

                        public void Dispose()
                        {
                            if (!this.d)
                            {
                                this.e.Dispose();
                                this.d = true;
                            }
                        }

                        public void Reset()
                        {
                            this.Dispose();
                            this.d = false;
                            this.e = VisNode.db.GetEnumerator();
                        }

                        private bool Pass(VisNode cur)
                        {
                            if ((!cur.deaf && this.traitComp.Pass(cur._traitMask)) && this.data.Pass(cur))
                            {
                                this.Current = cur;
                                return true;
                            }
                            return false;
                        }
                    }
                }
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct Enumerator : IDisposable, IEnumerator, IEnumerator<VisNode>
            {
                public ODBForwardEnumerator<VisNode> e;
                public VisNode Current;
                private bool d;
                public VisNode.Search.PointRadiusData data;
                public Enumerator(VisNode.Search.PointRadiusData pr)
                {
                    this.Current = null;
                    this.d = false;
                    this.e = VisNode.db.GetEnumerator();
                    this.data = pr;
                }

                VisNode IEnumerator<VisNode>.Current
                {
                    get
                    {
                        return this.Current;
                    }
                }
                object IEnumerator.Current
                {
                    get
                    {
                        return this.Current;
                    }
                }
                public bool MoveNext()
                {
                    while (this.e.MoveNext())
                    {
                        if (this.Pass(this.e.Current))
                        {
                            return true;
                        }
                    }
                    this.Current = null;
                    return false;
                }

                public void Dispose()
                {
                    if (!this.d)
                    {
                        this.e.Dispose();
                        this.d = true;
                    }
                }

                public void Reset()
                {
                    this.Dispose();
                    this.d = false;
                    this.e = VisNode.db.GetEnumerator();
                }

                private bool Pass(VisNode cur)
                {
                    if (this.data.Pass(cur))
                    {
                        this.Current = cur;
                        return true;
                    }
                    return false;
                }
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct SightMasked : IEnumerable, VisNode.Search.ISearch, IEnumerable<VisNode>, VisNode.Search.ISearch<VisNode.Search.Radial.SightMasked.Enumerator>
            {
                public Vector3 point;
                public float radius;
                public VisNode.Search.MaskCompareData maskComp;
                public SightMasked(Vector3 point, float radius, Vis.Mask mask, Vis.Op op)
                {
                    this.point = point;
                    this.radius = radius;
                    this.maskComp = new VisNode.Search.MaskCompareData(op, mask);
                }

                IEnumerator<VisNode> IEnumerable<VisNode>.GetEnumerator()
                {
                    return this.GetEnumerator();
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return this.GetEnumerator();
                }

                public Enumerator GetEnumerator()
                {
                    return new Enumerator(new VisNode.Search.PointRadiusData(this.point, this.radius), this.maskComp);
                }
                [StructLayout(LayoutKind.Sequential)]
                public struct Enumerator : IDisposable, IEnumerator, IEnumerator<VisNode>
                {
                    public ODBForwardEnumerator<VisNode> e;
                    public VisNode Current;
                    private bool d;
                    public VisNode.Search.PointRadiusData data;
                    public VisNode.Search.MaskCompareData viewComp;
                    public Enumerator(VisNode.Search.PointRadiusData pr, VisNode.Search.MaskCompareData mc)
                    {
                        this.Current = null;
                        this.d = false;
                        this.e = VisNode.db.GetEnumerator();
                        this.data = pr;
                        this.viewComp = mc;
                    }

                    VisNode IEnumerator<VisNode>.Current
                    {
                        get
                        {
                            return this.Current;
                        }
                    }
                    object IEnumerator.Current
                    {
                        get
                        {
                            return this.Current;
                        }
                    }
                    public bool MoveNext()
                    {
                        while (this.e.MoveNext())
                        {
                            if (this.Pass(this.e.Current))
                            {
                                return true;
                            }
                        }
                        this.Current = null;
                        return false;
                    }

                    public void Dispose()
                    {
                        if (!this.d)
                        {
                            this.e.Dispose();
                            this.d = true;
                        }
                    }

                    public void Reset()
                    {
                        this.Dispose();
                        this.d = false;
                        this.e = VisNode.db.GetEnumerator();
                    }

                    private bool Pass(VisNode cur)
                    {
                        if (this.viewComp.Pass(cur._sightMask) && this.data.Pass(cur))
                        {
                            this.Current = cur;
                            return true;
                        }
                        return false;
                    }
                }
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct TraitMasked : IEnumerable, VisNode.Search.ISearch, IEnumerable<VisNode>, VisNode.Search.ISearch<VisNode.Search.Radial.TraitMasked.Enumerator>
            {
                public Vector3 point;
                public float radius;
                public VisNode.Search.MaskCompareData maskComp;
                public TraitMasked(Vector3 point, float radius, Vis.Mask mask, Vis.Op op)
                {
                    this.point = point;
                    this.radius = radius;
                    this.maskComp = new VisNode.Search.MaskCompareData(op, mask);
                }

                IEnumerator<VisNode> IEnumerable<VisNode>.GetEnumerator()
                {
                    return this.GetEnumerator();
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return this.GetEnumerator();
                }

                public Enumerator GetEnumerator()
                {
                    return new Enumerator(new VisNode.Search.PointRadiusData(this.point, this.radius), this.maskComp);
                }
                [StructLayout(LayoutKind.Sequential)]
                public struct Enumerator : IDisposable, IEnumerator, IEnumerator<VisNode>
                {
                    public ODBForwardEnumerator<VisNode> e;
                    public VisNode Current;
                    private bool d;
                    public VisNode.Search.PointRadiusData data;
                    public VisNode.Search.MaskCompareData traitComp;
                    public Enumerator(VisNode.Search.PointRadiusData pr, VisNode.Search.MaskCompareData mc)
                    {
                        this.Current = null;
                        this.d = false;
                        this.e = VisNode.db.GetEnumerator();
                        this.data = pr;
                        this.traitComp = mc;
                    }

                    VisNode IEnumerator<VisNode>.Current
                    {
                        get
                        {
                            return this.Current;
                        }
                    }
                    object IEnumerator.Current
                    {
                        get
                        {
                            return this.Current;
                        }
                    }
                    public bool MoveNext()
                    {
                        while (this.e.MoveNext())
                        {
                            if (this.Pass(this.e.Current))
                            {
                                return true;
                            }
                        }
                        this.Current = null;
                        return false;
                    }

                    public void Dispose()
                    {
                        if (!this.d)
                        {
                            this.e.Dispose();
                            this.d = true;
                        }
                    }

                    public void Reset()
                    {
                        this.Dispose();
                        this.d = false;
                        this.e = VisNode.db.GetEnumerator();
                    }

                    private bool Pass(VisNode cur)
                    {
                        if (this.traitComp.Pass(cur._traitMask) && this.data.Pass(cur))
                        {
                            this.Current = cur;
                            return true;
                        }
                        return false;
                    }
                }
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct TraitHistory
    {
        public int last;
        public bool changed;
        public int Upd(int newTraits)
        {
            int num = newTraits ^ this.last;
            this.changed = num != 0;
            this.last = newTraits;
            return num;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct VisMem
    {
        public ODBSet<VisNode> list;
        public ODBSet<VisNode> last;
        public int count;
        public bool add;
        public bool rem;
        public bool any;
        public bool had;
    }
}

