using Facepunch.MeshBatch;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using uLink;
using UnityEngine;

[RequireComponent(typeof(NetworkView))]
public class StructureMaster : IDMain, IServerSaveable, IServerSaveNotify
{
    private bool _boundsDirty;
    private Vector3 _buildingSize;
    private Bounds _containedBounds;
    private float _decayDelayRemaining;
    private static float _decayRate = 1f;
    protected List<Vector3> _foundationPoints;
    protected Dictionary<StructureComponent, HashSet<StructureComponent>> _hasWeightOn;
    protected float _lastDecayTime;
    private Bounds _localBounds;
    protected StructureMaterialType _materialType;
    private float _pentUpDecayTime;
    protected HashSet<StructureComponent> _structureComponents;
    protected Dictionary<StructureComponentKey, CompPosNode> _structureComponentsByPosition;
    protected Dictionary<StructureComponent, HashSet<StructureComponent>> _weightOnMe;
    [CompilerGenerated]
    private static Comparison<KeyValuePair<StructureMaster, float>> <>f__am$cache18;
    public ulong creatorID;
    public static Vector3 foundationSize = new Vector3(5f, 0.5f, 5f);
    private static List<StructureMaster> g_Structures = new List<StructureMaster>();
    private static List<StructureMaster> g_StructuresWithBounds = new List<StructureMaster>();
    public static float gridSpacingXZ = 2.5f;
    public static float gridSpacingY = 4f;
    [SerializeField]
    private MeshBatchGraphicalTarget meshBatchTargetGraphical;
    [SerializeField]
    private MeshBatchPhysicalTarget meshBatchTargetPhysical;
    protected int nextID;
    public ulong ownerID;

    public StructureMaster() : this(IDFlags.Unknown)
    {
    }

    protected StructureMaster(IDFlags idFlags) : base(idFlags)
    {
        this._boundsDirty = true;
    }

    public bool AddCompPositionEntry(StructureComponent comp)
    {
        CompPosNode node;
        Vector3 v = this.LocalIndexRound(base.transform.InverseTransformPoint(comp.transform.position));
        StructureComponentKey key = new StructureComponentKey(v);
        if (this._structureComponentsByPosition.TryGetValue(key, out node))
        {
            node.Add(comp);
        }
        else
        {
            node = new CompPosNode();
            node.Add(comp);
            this._structureComponentsByPosition.Add(key, node);
        }
        return true;
    }

    public void AddWeightLink(StructureComponent me, StructureComponent weight)
    {
        if (this._weightOnMe.ContainsKey(me))
        {
            this._weightOnMe[me].Add(weight);
        }
        else
        {
            this._weightOnMe.Add(me, new HashSet<StructureComponent>());
            this._weightOnMe[me].Add(weight);
        }
        if (this._hasWeightOn.ContainsKey(weight))
        {
            this._hasWeightOn[weight].Add(me);
        }
        else
        {
            this._hasWeightOn.Add(weight, new HashSet<StructureComponent>());
            this._hasWeightOn[weight].Add(me);
        }
    }

    internal void AppendStructureComponent(StructureComponent comp)
    {
        this.AppendStructureComponent(comp, false);
    }

    protected void AppendStructureComponent(StructureComponent comp, bool nobind)
    {
        if ((comp.type == StructureComponent.StructureComponentType.Foundation) && (this._materialType == StructureMaterialType.UNSET))
        {
            this.SetMaterialType(comp.GetMaterialType());
        }
        this._structureComponents.Add(comp);
        this.AddCompPositionEntry(comp);
        this.GenerateLinkForComp(comp);
        this.RecalculateStructureLinks();
        this.MarkBoundsDirty();
        if (!nobind)
        {
            try
            {
                comp.OnOwnedByMasterStructure(this);
            }
            catch (Exception exception)
            {
                Debug.LogError(exception);
            }
        }
        if (this._structureComponents.Count == 1)
        {
            g_StructuresWithBounds.Add(this);
        }
        if (this.meshBatchTargetGraphical != null)
        {
            foreach (MeshBatchInstance instance in comp.GetComponentsInChildren<MeshBatchInstance>(true))
            {
                instance.graphicalTarget = this.meshBatchTargetGraphical;
            }
        }
    }

    public bool Approx(float a, float b)
    {
        return (Mathf.Abs((float) (a - b)) < 0.001);
    }

    public void Awake()
    {
        this._structureComponents = new HashSet<StructureComponent>();
        this._structureComponentsByPosition = new Dictionary<StructureComponentKey, CompPosNode>();
        g_Structures.Add(this);
    }

    public void CacheCreator()
    {
    }

    public bool CheckIsWall(StructureComponent wallTest)
    {
        return ((wallTest != null) && wallTest.IsWallType());
    }

    public StructureComponent CompByLocal(Vector3 localPos)
    {
        CompPosNode node;
        StructureComponentKey key = new StructureComponentKey(localPos);
        if (this._structureComponentsByPosition.TryGetValue(key, out node))
        {
            return node.GetAny();
        }
        return null;
    }

    public StructureComponent CompByLocal(Vector3 localPos, StructureComponent.StructureComponentType type)
    {
        CompPosNode node;
        StructureComponentKey key = new StructureComponentKey(localPos);
        if (this._structureComponentsByPosition.TryGetValue(key, out node))
        {
            return node.GetType(type);
        }
        return null;
    }

    public bool ComponentCarryingWeight(StructureComponent comp)
    {
        return (((this._weightOnMe != null) && this._weightOnMe.ContainsKey(comp)) && (this._weightOnMe[comp].Count > 0));
    }

    public bool CullComponent(StructureComponent component)
    {
        if ((component == null) || !this._structureComponents.Remove(component))
        {
            return false;
        }
        this.RemoveCompPositionEntry(component);
        this.RecalculateStructureLinks();
        this.MarkBoundsDirty();
        if (this._structureComponents.Count == 0)
        {
            g_StructuresWithBounds.Remove(this);
        }
        return true;
    }

    public int FindComponentID(StructureComponent component)
    {
        int num = 0;
        foreach (StructureComponent component2 in this._structureComponents)
        {
            if (component2 != component)
            {
                num++;
            }
            else
            {
                return num;
            }
        }
        return -1;
    }

    public void GenerateLinkForComp(StructureComponent comp)
    {
        if (this._hasWeightOn == null)
        {
            this._hasWeightOn = new Dictionary<StructureComponent, HashSet<StructureComponent>>();
        }
        if (this._weightOnMe == null)
        {
            this._weightOnMe = new Dictionary<StructureComponent, HashSet<StructureComponent>>();
        }
        Vector3 vector = this.LocalIndexRound(base.transform.InverseTransformPoint(comp.transform.position));
        if (((comp.type == StructureComponent.StructureComponentType.Wall) || (comp.type == StructureComponent.StructureComponentType.Doorway)) || (comp.type == StructureComponent.StructureComponentType.WindowWall))
        {
            Vector3 worldPos = comp.transform.position + ((Vector3) ((comp.transform.rotation * -Vector3.right) * 2.5f));
            StructureComponent componentFromPositionWorld = this.GetComponentFromPositionWorld(worldPos);
            Vector3 vector3 = comp.transform.position + ((Vector3) ((comp.transform.rotation * Vector3.right) * 2.5f));
            StructureComponent me = this.GetComponentFromPositionWorld(vector3);
            if ((componentFromPositionWorld != null) && (componentFromPositionWorld.type == StructureComponent.StructureComponentType.Pillar))
            {
                this.AddWeightLink(componentFromPositionWorld, comp);
            }
            if ((me != null) && (me.type == StructureComponent.StructureComponentType.Pillar))
            {
                this.AddWeightLink(me, comp);
            }
        }
        else if (comp.type == StructureComponent.StructureComponentType.Pillar)
        {
            StructureComponent component3 = this.CompByLocal(vector - new Vector3(0f, gridSpacingY, 0f), StructureComponent.StructureComponentType.Pillar);
            if (component3 != null)
            {
                this.AddWeightLink(component3, comp);
            }
            float y = -gridSpacingY;
            Vector3[] vectorArray = new Vector3[] { new Vector3(-2.5f, y, -2.5f), new Vector3(2.5f, y, 2.5f), new Vector3(-2.5f, y, 2.5f), new Vector3(2.5f, y, -2.5f), new Vector3(2.5f, y, 0f), new Vector3(-2.5f, y, 0f), new Vector3(0f, y, 2.5f), new Vector3(0f, y, -2.5f), new Vector3(0f, y, 0f) };
            foreach (Vector3 vector4 in vectorArray)
            {
                StructureComponent component4 = this.CompByLocal(vector + vector4, StructureComponent.StructureComponentType.Foundation);
                StructureComponent component5 = this.CompByLocal(vector + vector4, StructureComponent.StructureComponentType.Ceiling);
                if (component4 != null)
                {
                    this.AddWeightLink(component4, comp);
                }
                if (component5 != null)
                {
                    this.AddWeightLink(component5, comp);
                }
            }
        }
        else if (comp.type == StructureComponent.StructureComponentType.Ceiling)
        {
            Vector3[] vectorArray3 = new Vector3[] { new Vector3(-2.5f, 0f, -2.5f), new Vector3(2.5f, 0f, 2.5f), new Vector3(-2.5f, 0f, 2.5f), new Vector3(2.5f, 0f, -2.5f) };
            foreach (Vector3 vector5 in vectorArray3)
            {
                StructureComponent component6 = this.CompByLocal(vector + vector5, StructureComponent.StructureComponentType.Pillar);
                if (component6 != null)
                {
                    this.AddWeightLink(component6, comp);
                }
            }
        }
        else if (comp.type == StructureComponent.StructureComponentType.Ramp)
        {
            StructureComponent component7 = this.CompByLocal(vector - new Vector3(0f, gridSpacingY, 0f));
            if (component7 != null)
            {
                this.AddWeightLink(component7, comp);
            }
        }
        else if (comp.type == StructureComponent.StructureComponentType.Foundation)
        {
            StructureComponent component8 = this.CompByLocal(vector - new Vector3(0f, gridSpacingY, 0f), StructureComponent.StructureComponentType.Foundation);
            if (component8 != null)
            {
                if (component8 != comp)
                {
                    this.AddWeightLink(component8, comp);
                }
                else
                {
                    Debug.Log("MAJOR FUCKUP");
                }
            }
        }
        else if (comp.type == StructureComponent.StructureComponentType.Stairs)
        {
            Vector3[] vectorArray5 = new Vector3[] { new Vector3(-2.5f, 0f, -2.5f), new Vector3(2.5f, 0f, 2.5f), new Vector3(-2.5f, 0f, 2.5f), new Vector3(2.5f, 0f, -2.5f) };
            foreach (Vector3 vector6 in vectorArray5)
            {
                StructureComponent component9 = this.CompByLocal(vector + vector6, StructureComponent.StructureComponentType.Pillar);
                if ((component9 != null) && (component9.type == StructureComponent.StructureComponentType.Pillar))
                {
                    this.AddWeightLink(component9, comp);
                }
            }
        }
    }

    public void GenerateLinks()
    {
        this._hasWeightOn = new Dictionary<StructureComponent, HashSet<StructureComponent>>();
        this._weightOnMe = new Dictionary<StructureComponent, HashSet<StructureComponent>>();
        foreach (StructureComponent component in this._structureComponents)
        {
            this.GenerateLinkForComp(component);
        }
    }

    public void GenerateLOD()
    {
        base.GetComponent<CombineChildren>().DoCombine();
    }

    public bool GetBounds(out Bounds bounds)
    {
        bounds = this.containedBounds;
        return (this._structureComponents.Count > 0);
    }

    public StructureComponent GetComponentFromPositionLocal(Vector3 localPos)
    {
        foreach (StructureComponent component in this._structureComponents)
        {
            if (Vector3.Distance(localPos, base.transform.InverseTransformPoint(component.transform.position)) < 0.01f)
            {
                return component;
            }
        }
        return null;
    }

    public StructureComponent GetComponentFromPositionWorld(Vector3 worldPos)
    {
        Vector3 localPos = this.LocalIndexRound(base.transform.InverseTransformPoint(worldPos));
        return this.CompByLocal(localPos);
    }

    public float GetDecayDelay()
    {
        return this.GetDecayDelayForType(this._materialType);
    }

    public float GetDecayDelayForType(StructureMaterialType type)
    {
        switch (type)
        {
            case StructureMaterialType.Wood:
                return 172800f;

            case StructureMaterialType.Metal:
                return 345600f;

            case StructureMaterialType.Brick:
                return 259200f;

            case StructureMaterialType.Concrete:
                return 432000f;
        }
        return 0f;
    }

    public float GetDecayTimeMaxHealth()
    {
        return this.GetDecayTimeMaxHealthForType(this._materialType);
    }

    public float GetDecayTimeMaxHealthForType(StructureMaterialType type)
    {
        switch (type)
        {
            case StructureMaterialType.Wood:
                return 21600f;

            case StructureMaterialType.Metal:
                return 43200f;

            case StructureMaterialType.Brick:
                return 86400f;

            case StructureMaterialType.Concrete:
                return 259200f;
        }
        return 60f;
    }

    public bool GetFoundationForPoint(Vector3 searchPos)
    {
        foreach (StructureComponent component in this._structureComponents)
        {
            if (component.type == StructureComponent.StructureComponentType.Foundation)
            {
                Vector3 vector = component.transform.InverseTransformPoint(searchPos);
                if (((Mathf.Abs(vector.x) <= 2.51f) && (Mathf.Abs(vector.z) <= 2.51f)) && this.Approx(vector.y, 4f))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public StructureMaterialType GetMaterialType()
    {
        return this._materialType;
    }

    [RPC]
    public void GetOwnerInfo(ulong creator, ulong owner)
    {
        this.creatorID = creator;
        this.ownerID = owner;
    }

    public void GetStructureSize(out int maxWidth, out int maxLength, out int maxHeight)
    {
        Bounds containedBounds = this.containedBounds;
        float f = this._localBounds.size.x / (gridSpacingXZ * 2f);
        float num2 = this._localBounds.size.z / (gridSpacingXZ * 2f);
        float num3 = this._localBounds.size.y / gridSpacingY;
        maxWidth = Mathf.RoundToInt(f);
        maxLength = Mathf.RoundToInt(num2);
        maxHeight = Mathf.RoundToInt(num3);
    }

    void IServerSaveNotify.PostLoad()
    {
    }

    public bool IsValidFoundationSpot(Vector3 searchPos)
    {
        if (this._structureComponents.Count == 0)
        {
            return true;
        }
        foreach (StructureComponent component in this._structureComponents)
        {
            if (component.type == StructureComponent.StructureComponentType.Foundation)
            {
                Vector3 vector2;
                Vector3 vector3;
                Vector3 vector = component.transform.InverseTransformPoint(searchPos);
                bool flag = ((this.Approx(Mathf.Abs(vector.x), 5f) && this.Approx(vector.z, 0f)) || (this.Approx(Mathf.Abs(vector.z), 5f) && this.Approx(vector.x, 0f))) && this.Approx(vector.y, 0f);
                bool flag2 = false;
                if (TransformHelpers.GetGroundInfoTerrainOnly(searchPos + new Vector3(0f, 3.5f, 0f), 3.5f, out vector2, out vector3))
                {
                    flag2 = true;
                }
                if (flag && !flag2)
                {
                    flag = false;
                }
                bool flag3 = false;
                if (flag || flag3)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public Vector3 LocalIndexRound(Vector3 toRound)
    {
        return toRound;
    }

    public void MarkBoundsDirty()
    {
        this._boundsDirty = true;
    }

    public void OnDestroy()
    {
        try
        {
            g_StructuresWithBounds.Remove(this);
            g_Structures.Remove(this);
        }
        finally
        {
            base.OnDestroy();
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(this.containedBounds.center, this.containedBounds.size);
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(this.containedBounds.center, this.containedBounds.size);
        if (this._hasWeightOn != null)
        {
            foreach (KeyValuePair<StructureComponent, HashSet<StructureComponent>> pair in this._hasWeightOn)
            {
                if (pair.Key != null)
                {
                    Gizmos.color = Color.gray;
                    Gizmos.DrawWireSphere(pair.Key.transform.position + new Vector3(0f, 0.25f, 0f), 0.25f);
                    Gizmos.color = Color.green;
                    foreach (StructureComponent component in pair.Value)
                    {
                        if (component != null)
                        {
                            Gizmos.DrawLine(pair.Key.transform.position, component.transform.position);
                        }
                    }
                }
            }
        }
    }

    public static StructureMaster[] RayTestStructures(Ray ray)
    {
        return RayTestStructures(ray, 10f);
    }

    public static StructureMaster[] RayTestStructures(Ray ray, float maxDistance)
    {
        List<StructureMaster> list = null;
        bool flag = false;
        List<KeyValuePair<StructureMaster, float>> list2 = new List<KeyValuePair<StructureMaster, float>>();
        foreach (StructureMaster master in AllStructuresWithBounds)
        {
            if (master == null)
            {
                if (!flag)
                {
                    flag = true;
                    list = new List<StructureMaster>();
                }
                list.Add(master);
            }
            else
            {
                Bounds bounds;
                bool flag2;
                float distance = 0f;
                try
                {
                    flag2 = master.GetBounds(out bounds);
                }
                catch (Exception exception)
                {
                    if (!flag)
                    {
                        flag = true;
                        list = new List<StructureMaster>();
                    }
                    list.Add(master);
                    Debug.LogException(exception, master);
                    goto Label_00B3;
                }
                if ((flag2 && bounds.IntersectRay(ray, out distance)) && (distance <= maxDistance))
                {
                    list2.Add(new KeyValuePair<StructureMaster, float>(master, distance));
                }
            Label_00B3:;
            }
        }
        if (flag)
        {
            foreach (StructureMaster master2 in list)
            {
                g_Structures.Remove(master2);
                g_StructuresWithBounds.Remove(master2);
            }
        }
        if (list2.Count == 0)
        {
            return Empty.array;
        }
        if (<>f__am$cache18 == null)
        {
            <>f__am$cache18 = (x, y) => x.Value.CompareTo(y.Value);
        }
        list2.Sort(<>f__am$cache18);
        StructureMaster[] masterArray = new StructureMaster[list2.Count];
        int num2 = 0;
        foreach (KeyValuePair<StructureMaster, float> pair in list2)
        {
            masterArray[num2++] = pair.Key;
        }
        return masterArray;
    }

    public void RecalculateBounds()
    {
        this._containedBounds = new Bounds(base.transform.position, Vector3.zero);
        foreach (StructureComponent component in this._structureComponents)
        {
            this._containedBounds.Encapsulate(component.collider.bounds);
        }
        this.RecalculateStructureSize();
        this._containedBounds.Expand((float) 5f);
        this._boundsDirty = false;
    }

    public void RecalculateStructureLinks()
    {
    }

    public void RecalculateStructureSize()
    {
        Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
        foreach (StructureComponent component in this._structureComponents)
        {
            if (component.type == StructureComponent.StructureComponentType.Foundation)
            {
                Vector3 point = base.transform.InverseTransformPoint(component.transform.position);
                bounds.Encapsulate(point);
            }
            else if (component.type == StructureComponent.StructureComponentType.Pillar)
            {
            }
        }
        bounds.Expand((float) (gridSpacingXZ * 2f));
        this._localBounds = bounds;
    }

    public bool RemoveComponent(StructureComponent comp)
    {
        this.RecalculateStructureLinks();
        this.MarkBoundsDirty();
        return true;
    }

    public bool RemoveCompPositionEntry(StructureComponent comp)
    {
        CompPosNode node;
        Vector3 v = this.LocalIndexRound(base.transform.InverseTransformPoint(comp.transform.position));
        StructureComponentKey key = new StructureComponentKey(v);
        if (!this._structureComponentsByPosition.TryGetValue(key, out node))
        {
            return false;
        }
        node.Remove(comp);
        if (node.GetAny() != null)
        {
            this._structureComponentsByPosition.Remove(key);
        }
        return true;
    }

    public void RemoveLinkForComp(StructureComponent comp)
    {
        if (this._weightOnMe.ContainsKey(comp))
        {
            foreach (StructureComponent component in this._weightOnMe[comp])
            {
                if (this._hasWeightOn[component].Contains(comp))
                {
                    this._hasWeightOn[component].Remove(comp);
                    if (this._hasWeightOn[component].Count == 0)
                    {
                        this._hasWeightOn.Remove(component);
                    }
                }
            }
            this._weightOnMe.Remove(comp);
        }
        if (this._hasWeightOn.ContainsKey(comp))
        {
            foreach (StructureComponent component2 in this._hasWeightOn[comp])
            {
                if (this._weightOnMe[component2].Contains(comp))
                {
                    this._weightOnMe[component2].Remove(comp);
                    if (this._weightOnMe[component2].Count == 0)
                    {
                        this._weightOnMe.Remove(component2);
                    }
                }
            }
            this._hasWeightOn.Remove(comp);
        }
    }

    public void SetMaterialType(StructureMaterialType type)
    {
        if (this._materialType == StructureMaterialType.UNSET)
        {
            this._materialType = type;
        }
    }

    public static Vector3 SnapToGrid(Transform gridCenter, Vector3 desiredPosition, bool useHeight)
    {
        Vector3 position = gridCenter.InverseTransformPoint(desiredPosition);
        position.x = Mathf.Round(position.x / gridSpacingXZ) * gridSpacingXZ;
        position.z = Mathf.Round(position.z / gridSpacingXZ) * gridSpacingXZ;
        if (useHeight)
        {
            position.y = Mathf.Round(position.y / gridSpacingY) * gridSpacingY;
        }
        return gridCenter.TransformPoint(position);
    }

    public void Touched()
    {
        this._decayDelayRemaining = this.GetDecayDelay();
    }

    public void TryGenerateLOD()
    {
    }

    public static List<StructureMaster> AllStructures
    {
        get
        {
            return g_Structures;
        }
    }

    public static List<StructureMaster> AllStructuresWithBounds
    {
        get
        {
            return g_StructuresWithBounds;
        }
    }

    public Bounds containedBounds
    {
        get
        {
            if (this._boundsDirty)
            {
                this.RecalculateBounds();
            }
            return this._containedBounds;
        }
    }

    private static float decayRate
    {
        get
        {
            return _decayRate;
        }
        set
        {
            _decayRate = value;
        }
    }

    public class CompPosNode
    {
        public StructureComponent _ceiling;
        public StructureComponent _foundation;
        public StructureComponent _pillar;
        public StructureComponent _stairs;
        public StructureComponent _wall;

        public void Add(StructureComponent toAdd)
        {
            switch (toAdd.type)
            {
                case StructureComponent.StructureComponentType.Pillar:
                    this._pillar = toAdd;
                    break;

                case StructureComponent.StructureComponentType.Wall:
                case StructureComponent.StructureComponentType.Doorway:
                case StructureComponent.StructureComponentType.WindowWall:
                    this._wall = toAdd;
                    break;

                case StructureComponent.StructureComponentType.Ceiling:
                    this._ceiling = toAdd;
                    break;

                case StructureComponent.StructureComponentType.Stairs:
                    this._stairs = toAdd;
                    break;

                case StructureComponent.StructureComponentType.Foundation:
                    this._foundation = toAdd;
                    break;
            }
        }

        public StructureComponent GetAny()
        {
            if (this._ceiling != null)
            {
                return this._ceiling;
            }
            if (this._stairs != null)
            {
                return this._stairs;
            }
            if (this._pillar != null)
            {
                return this._pillar;
            }
            if (this._foundation != null)
            {
                return this._foundation;
            }
            if (this._wall != null)
            {
                return this._wall;
            }
            return null;
        }

        public StructureComponent GetType(StructureComponent.StructureComponentType type)
        {
            switch (type)
            {
                case StructureComponent.StructureComponentType.Pillar:
                    return this._pillar;

                case StructureComponent.StructureComponentType.Wall:
                case StructureComponent.StructureComponentType.Doorway:
                case StructureComponent.StructureComponentType.WindowWall:
                    return this._wall;

                case StructureComponent.StructureComponentType.Ceiling:
                    return this._ceiling;

                case StructureComponent.StructureComponentType.Stairs:
                    return this._stairs;

                case StructureComponent.StructureComponentType.Foundation:
                    return this._foundation;
            }
            return null;
        }

        public void Remove(StructureComponent toRemove)
        {
            if (this._wall == toRemove)
            {
                this._wall = null;
            }
            else if (this._foundation == toRemove)
            {
                this._foundation = null;
            }
            else if (this._pillar == toRemove)
            {
                this._pillar = null;
            }
            else if (this._stairs == toRemove)
            {
                this._stairs = null;
            }
            else if (this._ceiling == toRemove)
            {
                this._ceiling = null;
            }
        }
    }

    private static class Empty
    {
        public static readonly StructureMaster[] array = new StructureMaster[0];
    }

    [Serializable]
    public enum StructureMaterialType
    {
        UNSET,
        Wood,
        Metal,
        Brick,
        Concrete
    }
}

