using Facepunch.MeshBatch;
using System;
using System.Runtime.InteropServices;
using uLink;
using UnityEngine;

public class DeployableItemDataBlock : HeldItemDataBlock
{
    [NonSerialized]
    private DeployableObject _deployableObject;
    [NonSerialized]
    private bool _loadedDeployableObject;
    public GameGizmo aimGizmo;
    public bool CanStackOnDeployables = true;
    public CarrierSphereCastMode carrierSphereCastMode;
    public bool checkPlacementZones;
    public string DeployableObjectPrefabName;
    public FitRequirements fitRequirements;
    public bool fitTestForcedUp;
    public bool forcePlaceable;
    public Hardpoint.hardpoint_type hardpointType;
    public float minCastRadius = 0.25f;
    public bool neverGrabCarrier;
    public DeployableOrientationMode orientationMode;
    public Material overrideMat;
    public float placeRange = 8f;
    public bool requireHardpoint;
    public float spacingRadius;
    public bool StructureOnly;
    public bool TerrainOnly;
    public bool uprightOnly;

    public bool CheckPlacement(Ray ray, out Vector3 pos, out Quaternion rot, out TransCarrier carrier)
    {
        DeployPlaceResults results;
        this.CheckPlacementResults(ray, out pos, out rot, out carrier, out results);
        return results.Valid();
    }

    public void CheckPlacementResults(Ray ray, out Vector3 pos, out Quaternion rot, out TransCarrier carrier, out DeployPlaceResults results)
    {
        RaycastHit hit;
        Vector3 position;
        Vector3 normal;
        bool flag6;
        MeshBatchInstance instance;
        Quaternion rotation;
        float placeRange = this.placeRange;
        IDMain main = null;
        bool flag = false;
        bool flag2 = false;
        bool flag3 = false;
        DeployableObject obj2 = null;
        bool flag4 = false;
        bool flag5 = false;
        bool flag7 = this.minCastRadius >= float.Epsilon;
        bool flag8 = !flag7 ? MeshBatchPhysics.Raycast(ray, out hit, placeRange, -472317957, out flag6, out instance) : MeshBatchPhysics.SphereCast(ray, this.minCastRadius, out hit, placeRange, -472317957, out flag6, out instance);
        Vector3 point = ray.GetPoint(placeRange);
        if (!flag8)
        {
            Vector3 origin = point;
            origin.y += 0.5f;
            flag4 = MeshBatchPhysics.Raycast(origin, Vector3.down, out hit, 5f, -472317957, out flag6, out instance);
        }
        if (flag8 || flag4)
        {
            main = !flag6 ? IDBase.GetMain(hit.collider) : instance.idMain;
            flag3 = (main is StructureComponent) || (main is StructureMaster);
            position = hit.point;
            normal = hit.normal;
            flag = !flag3 && ((bool) (obj2 = main as DeployableObject));
            if (((this.carrierSphereCastMode != CarrierSphereCastMode.Allowed) && flag8) && (flag7 && !NonVariantSphereCast(ray, position)))
            {
                Ray ray2;
                float num2;
                RaycastHit hit2;
                bool flag9;
                MeshBatchInstance instance2;
                bool flag10;
                if (this.carrierSphereCastMode == CarrierSphereCastMode.AdjustedRay)
                {
                    Vector3 vector5 = ray.origin;
                    Vector3 direction = hit.point - vector5;
                    num2 = direction.magnitude + (this.minCastRadius * 2f);
                    ray2 = new Ray(vector5, direction);
                    Debug.DrawLine(ray.origin, ray.GetPoint(num2), Color.cyan);
                }
                else
                {
                    num2 = placeRange + this.minCastRadius;
                    ray2 = ray;
                }
                if (!(flag10 = MeshBatchPhysics.Raycast(ray2, out hit2, num2, -472317957, out flag9, out instance2)))
                {
                    Vector3 vector8 = position;
                    vector8.y += 0.5f;
                    flag10 = MeshBatchPhysics.Raycast(vector8, Vector3.down, out hit2, 5f, -472317957, out flag9, out instance2);
                }
                if (flag10)
                {
                    IDMain main2 = !flag9 ? IDBase.GetMain(hit2.collider) : instance2.idMain;
                    carrier = (main2 == null) ? hit2.collider.GetComponent<TransCarrier>() : main2.GetLocal<TransCarrier>();
                }
                else
                {
                    carrier = null;
                }
            }
            else
            {
                carrier = ((main == null) ? hit.collider.gameObject : main.gameObject).GetComponent<TransCarrier>();
            }
            flag2 = (hit.collider is TerrainCollider) || (hit.collider.gameObject.layer == 10);
            flag5 = true;
        }
        else
        {
            position = point;
            normal = Vector3.up;
            carrier = null;
        }
        bool flag11 = false;
        Hardpoint hardpointFromRay = null;
        if (this.hardpointType != Hardpoint.hardpoint_type.None)
        {
            hardpointFromRay = Hardpoint.GetHardpointFromRay(ray, this.hardpointType);
            if (hardpointFromRay != null)
            {
                flag11 = true;
                position = hardpointFromRay.transform.position;
                normal = hardpointFromRay.transform.up;
                carrier = hardpointFromRay.GetMaster().GetTransCarrier();
                flag5 = true;
            }
        }
        bool flag12 = false;
        if (this.spacingRadius > 0f)
        {
            foreach (Collider collider in Physics.OverlapSphere(position, this.spacingRadius))
            {
                GameObject gameObject = collider.gameObject;
                IDBase component = collider.gameObject.GetComponent<IDBase>();
                if (component != null)
                {
                    gameObject = component.idMain.gameObject;
                }
                if (gameObject.CompareTag(this.ObjectToPlace.gameObject.tag) && (Vector3.Distance(position, gameObject.transform.position) < this.spacingRadius))
                {
                    flag12 = true;
                    break;
                }
            }
        }
        bool flag13 = false;
        if ((flag && !this.forcePlaceable) && obj2.cantPlaceOn)
        {
            flag13 = true;
        }
        pos = position;
        if (this.orientationMode == DeployableOrientationMode.Default)
        {
            if (this.uprightOnly)
            {
                this.orientationMode = DeployableOrientationMode.Upright;
            }
            else
            {
                this.orientationMode = DeployableOrientationMode.NormalUp;
            }
        }
        switch (this.orientationMode)
        {
            case DeployableOrientationMode.NormalUp:
                rotation = TransformHelpers.LookRotationForcedUp(ray.direction, normal);
                break;

            case DeployableOrientationMode.Upright:
                rotation = TransformHelpers.LookRotationForcedUp(ray.direction, Vector3.up);
                break;

            case DeployableOrientationMode.NormalForward:
                rotation = TransformHelpers.LookRotationForcedUp(Vector3.Cross(ray.direction, Vector3.up), normal);
                break;

            case DeployableOrientationMode.HardpointPosRot:
                if (!flag11)
                {
                    rotation = TransformHelpers.LookRotationForcedUp(ray.direction, Vector3.up);
                    break;
                }
                rotation = hardpointFromRay.transform.rotation;
                break;

            default:
                throw new NotImplementedException();
        }
        rot = rotation * this.ObjectToPlace.transform.localRotation;
        bool flag14 = false;
        if (this.checkPlacementZones)
        {
            flag14 = NoPlacementZone.ValidPos(pos);
        }
        float num4 = Vector3.Angle(normal, Vector3.up);
        results.falseFromDeployable = (!this.CanStackOnDeployables && flag) || flag13;
        results.falseFromTerrian = this.TerrainOnly && !flag2;
        results.falseFromClose = (this.spacingRadius > 0f) && flag12;
        results.falseFromHardpoint = this.requireHardpoint && !flag11;
        results.falseFromAngle = !this.requireHardpoint && (num4 >= this.ObjectToPlace.maxSlope);
        results.falseFromPlacementZone = this.checkPlacementZones && !flag14;
        results.falseFromHittingNothing = !flag5;
        results.falseFromStructure = this.StructureOnly && !flag3;
        results.falseFromFitRequirements = (this.fitRequirements != null) && !this.fitRequirements.Test(pos, !this.fitTestForcedUp ? rot : TransformHelpers.LookRotationForcedUp(rot, Vector3.up), this.ObjectToPlace.transform.localScale);
    }

    protected override IInventoryItem ConstructItem()
    {
        return new ITEM_TYPE(this);
    }

    public override void DoAction1(BitStream stream, ItemRepresentation rep, ref NetworkMessageInfo info)
    {
    }

    private static bool NonVariantSphereCast(Ray r, Vector3 p)
    {
        Vector3 vector3;
        Vector3 origin = r.origin;
        Vector3 direction = r.direction;
        float num = (((direction.x * p.x) + (direction.y * p.y)) + (direction.z * p.z)) - (((direction.x * origin.x) + (direction.y * origin.y)) + (direction.z * origin.z));
        vector3.x = p.x - ((direction.x * num) + origin.x);
        vector3.y = p.y - ((direction.y * num) + origin.y);
        vector3.z = p.z - ((direction.z * num) + origin.z);
        return ((((vector3.x * vector3.x) + (vector3.y * vector3.y)) + (vector3.z * vector3.z)) < 0.001f);
    }

    public DeployableObject ObjectToPlace
    {
        get
        {
            if (!this._loadedDeployableObject && Application.isPlaying)
            {
                NetCull.LoadPrefabScript<DeployableObject>(this.DeployableObjectPrefabName, out this._deployableObject);
                this._loadedDeployableObject = true;
            }
            return this._deployableObject;
        }
    }

    public enum CarrierSphereCastMode
    {
        Allowed,
        AdjustedRay,
        InputRay
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DeployPlaceResults
    {
        public bool falseFromDeployable;
        public bool falseFromTerrian;
        public bool falseFromClose;
        public bool falseFromHardpoint;
        public bool falseFromAngle;
        public bool falseFromPlacementZone;
        public bool falseFromFitRequirements;
        public bool falseFromHittingNothing;
        public bool falseFromStructure;
        public bool Valid()
        {
            return ((((!this.falseFromAngle && !this.falseFromDeployable) && (!this.falseFromTerrian && !this.falseFromClose)) && ((!this.falseFromHardpoint && !this.falseFromPlacementZone) && (!this.falseFromFitRequirements && !this.falseFromHittingNothing))) && !this.falseFromStructure);
        }

        public void PrintResults()
        {
            if (this.Valid())
            {
                Debug.Log("VALID!");
            }
            else
            {
                object[] args = new object[] { this.falseFromAngle, this.falseFromDeployable, this.falseFromTerrian, this.falseFromClose, this.falseFromHardpoint, this.falseFromPlacementZone, this.falseFromFitRequirements, this.falseFromHittingNothing, this.falseFromStructure };
                Debug.Log("FAIL! - " + string.Format("Results ang:{0} dep:{1} ter:{2} close:{3} hardpoint:{4} npz:{5} fit:{6} nothin:{7} struct:{8}", args));
            }
        }
    }

    private sealed class ITEM_TYPE : DeployableItem<DeployableItemDataBlock>, IDeployableItem, IHeldItem, IInventoryItem
    {
        public ITEM_TYPE(DeployableItemDataBlock BLOCK) : base(BLOCK)
        {
        }

        void IHeldItem.AddMod(ItemModDataBlock mod)
        {
            base.AddMod(mod);
        }

        int IHeldItem.FindMod(ItemModDataBlock mod)
        {
            return base.FindMod(mod);
        }

        bool IHeldItem.get_canActivate()
        {
            return base.canActivate;
        }

        bool IHeldItem.get_canDeactivate()
        {
            return base.canDeactivate;
        }

        int IHeldItem.get_freeModSlots()
        {
            return base.freeModSlots;
        }

        ItemModDataBlock[] IHeldItem.get_itemMods()
        {
            return base.itemMods;
        }

        ItemRepresentation IHeldItem.get_itemRepresentation()
        {
            return base.itemRepresentation;
        }

        ItemModFlags IHeldItem.get_modFlags()
        {
            return base.modFlags;
        }

        int IHeldItem.get_totalModSlots()
        {
            return base.totalModSlots;
        }

        int IHeldItem.get_usedModSlots()
        {
            return base.usedModSlots;
        }

        ViewModel IHeldItem.get_viewModelInstance()
        {
            return base.viewModelInstance;
        }

        void IHeldItem.OnActivate()
        {
            base.OnActivate();
        }

        void IHeldItem.OnDeactivate()
        {
            base.OnDeactivate();
        }

        void IHeldItem.set_itemRepresentation(ItemRepresentation value)
        {
            base.itemRepresentation = value;
        }

        void IHeldItem.SetTotalModSlotCount(int count)
        {
            base.SetTotalModSlotCount(count);
        }

        void IHeldItem.SetUsedModSlotCount(int count)
        {
            base.SetUsedModSlotCount(count);
        }

        int IInventoryItem.AddUses(int count)
        {
            return base.AddUses(count);
        }

        bool IInventoryItem.Consume(ref int count)
        {
            return base.Consume(ref count);
        }

        void IInventoryItem.Deserialize(BitStream stream)
        {
            base.Deserialize(stream);
        }

        bool IInventoryItem.get_active()
        {
            return base.active;
        }

        Character IInventoryItem.get_character()
        {
            return base.character;
        }

        float IInventoryItem.get_condition()
        {
            return base.condition;
        }

        Controllable IInventoryItem.get_controllable()
        {
            return base.controllable;
        }

        Controller IInventoryItem.get_controller()
        {
            return base.controller;
        }

        bool IInventoryItem.get_dirty()
        {
            return base.dirty;
        }

        bool IInventoryItem.get_doNotSave()
        {
            return base.doNotSave;
        }

        IDMain IInventoryItem.get_idMain()
        {
            return base.idMain;
        }

        Inventory IInventoryItem.get_inventory()
        {
            return base.inventory;
        }

        bool IInventoryItem.get_isInLocalInventory()
        {
            return base.isInLocalInventory;
        }

        float IInventoryItem.get_lastUseTime()
        {
            return base.lastUseTime;
        }

        float IInventoryItem.get_maxcondition()
        {
            return base.maxcondition;
        }

        int IInventoryItem.get_slot()
        {
            return base.slot;
        }

        int IInventoryItem.get_uses()
        {
            return base.uses;
        }

        float IInventoryItem.GetConditionPercent()
        {
            return base.GetConditionPercent();
        }

        bool IInventoryItem.IsBroken()
        {
            return base.IsBroken();
        }

        bool IInventoryItem.IsDamaged()
        {
            return base.IsDamaged();
        }

        bool IInventoryItem.MarkDirty()
        {
            return base.MarkDirty();
        }

        void IInventoryItem.Serialize(BitStream stream)
        {
            base.Serialize(stream);
        }

        void IInventoryItem.set_lastUseTime(float value)
        {
            base.lastUseTime = value;
        }

        void IInventoryItem.SetCondition(float condition)
        {
            base.SetCondition(condition);
        }

        void IInventoryItem.SetMaxCondition(float condition)
        {
            base.SetMaxCondition(condition);
        }

        void IInventoryItem.SetUses(int count)
        {
            base.SetUses(count);
        }

        bool IInventoryItem.TryConditionLoss(float probability, float percentLoss)
        {
            return base.TryConditionLoss(probability, percentLoss);
        }

        ItemDataBlock IInventoryItem.datablock
        {
            get
            {
                return base.datablock;
            }
        }
    }
}

