using System;
using System.Runtime.InteropServices;
using uLink;
using UnityEngine;

public abstract class StructureComponentItem<T> : HeldItem<T> where T: StructureComponentDataBlock
{
    protected StructureMaster _master;
    protected MaterialPropertyBlock _materialProps;
    protected float _nextPlaceTime;
    protected Vector3 _placePos;
    protected Quaternion _placeRot;
    protected PrefabRenderer _prefabRenderer;
    public Quaternion desiredRotation;
    private static bool informedPreFrame;
    private static bool informedPreRender;
    protected bool lastFrameAttack2;
    protected bool validLocation;

    protected StructureComponentItem(T db) : base(db)
    {
        this.desiredRotation = Quaternion.identity;
    }

    public virtual bool CanPlace()
    {
        return (this.validLocation && (this._nextPlaceTime <= Time.time));
    }

    public virtual void DoPlace()
    {
        this._nextPlaceTime = Time.time + 0.5f;
        Character character = base.character;
        if (character == null)
        {
            Debug.Log("NO char for placement");
        }
        else
        {
            Ray eyesRay = character.eyesRay;
            object[] arguments = new object[] { eyesRay.origin, eyesRay.direction, this._placePos, this._placeRot, (this._master == null) ? NetworkViewID.unassigned : this._master.networkView.viewID };
            base.itemRepresentation.Action(1, RPCMode.Server, arguments);
        }
    }

    private void InformException(Exception e, string title, ref bool informedOnce, Object obj = null)
    {
        if (!informedOnce)
        {
            Debug.LogError(title + "\n" + e, obj);
            informedOnce = true;
        }
        else
        {
            Debug.LogException(e);
        }
    }

    public bool IsValidLocation()
    {
        return false;
    }

    public override void ItemPreFrame(ref HumanController.InputSample sample)
    {
        base.ItemPreFrame(ref sample);
        try
        {
            if (sample.attack2 && !this.lastFrameAttack2)
            {
                this.desiredRotation *= Quaternion.AngleAxis(90f, Vector3.up);
            }
            if (sample.attack && this.CanPlace())
            {
                this.DoPlace();
            }
            this.lastFrameAttack2 = sample.attack2;
        }
        catch (Exception exception)
        {
            this.InformException(exception, "in ItemPreFrame", ref StructureComponentItem<T>.informedPreFrame, null);
        }
    }

    public override void PreCameraRender()
    {
        try
        {
            this.RenderPlacementHelpers();
        }
        catch (Exception exception)
        {
            this.InformException(exception, "in PreCameraRender()", ref StructureComponentItem<T>.informedPreRender, null);
        }
    }

    public virtual void RenderDeployPreview(Vector3 point, Quaternion rot)
    {
        if (this._prefabRenderer == null)
        {
            StructureComponent structureToPlacePrefab = base.datablock.structureToPlacePrefab;
            if (structureToPlacePrefab == null)
            {
                return;
            }
            this._prefabRenderer = PrefabRenderer.GetOrCreateRender(structureToPlacePrefab.gameObject);
            this._materialProps = new MaterialPropertyBlock();
        }
        Material overrideMat = base.datablock.overrideMat;
        if (overrideMat != null)
        {
            this._prefabRenderer.RenderOneMaterial(MountedCamera.main.camera, Matrix4x4.TRS(point, rot, Vector3.one), this._materialProps, overrideMat);
        }
        else
        {
            this._prefabRenderer.Render(MountedCamera.main.camera, Matrix4x4.TRS(point, rot, Vector3.one), this._materialProps, null);
        }
    }

    protected void RenderPlacementHelpers()
    {
        StructureComponent structureToPlacePrefab = base.datablock.structureToPlacePrefab;
        this._master = null;
        this._placePos = Vector3.zero;
        this._placeRot = Quaternion.identity;
        this.validLocation = false;
        float axis = Input.GetAxis("Mouse ScrollWheel");
        if (axis > 0f)
        {
            this.desiredRotation *= Quaternion.AngleAxis(90f, Vector3.up);
        }
        else if (axis < 0f)
        {
            this.desiredRotation *= Quaternion.AngleAxis(-90f, Vector3.up);
        }
        Character character = base.character;
        if (character != null)
        {
            RaycastHit hit;
            Ray eyesRay = character.eyesRay;
            float distance = (structureToPlacePrefab.type != StructureComponent.StructureComponentType.Ceiling) ? 8f : 4f;
            Vector3 zero = Vector3.zero;
            Vector3 up = Vector3.up;
            Vector3 vector3 = Vector3.zero;
            bool flag = false;
            if (Physics.Raycast(eyesRay, out hit, distance))
            {
                vector3 = zero = hit.point;
                up = hit.normal;
                flag = true;
            }
            else
            {
                flag = false;
                vector3 = zero = eyesRay.origin + ((Vector3) (eyesRay.direction * distance));
            }
            switch (structureToPlacePrefab.type)
            {
                case StructureComponent.StructureComponentType.Ceiling:
                case StructureComponent.StructureComponentType.Foundation:
                case StructureComponent.StructureComponentType.Ramp:
                    zero.y -= 3.5f;
                    break;
            }
            bool flag2 = false;
            bool flag3 = false;
            Vector3 vector4 = zero;
            Quaternion quaternion = TransformHelpers.LookRotationForcedUp(character.eyesAngles.forward, Vector3.up) * this.desiredRotation;
            foreach (StructureMaster master in StructureMaster.RayTestStructures(eyesRay))
            {
                if (master != null)
                {
                    int num4;
                    int num5;
                    int num6;
                    master.GetStructureSize(out num4, out num5, out num6);
                    this._placePos = StructureMaster.SnapToGrid(master.transform, zero, true);
                    this._placeRot = TransformHelpers.LookRotationForcedUp(master.transform.forward, master.transform.transform.up) * this.desiredRotation;
                    if (!flag3)
                    {
                        vector4 = this._placePos;
                        quaternion = this._placeRot;
                        flag3 = true;
                    }
                    if (structureToPlacePrefab.CheckLocation(master, this._placePos, this._placeRot))
                    {
                        this._master = master;
                        flag2 = true;
                        break;
                    }
                }
            }
            if (flag2)
            {
                this.validLocation = true;
            }
            else if (structureToPlacePrefab.type != StructureComponent.StructureComponentType.Foundation)
            {
                this._placePos = vector4;
                this._placeRot = quaternion;
                this.validLocation = false;
            }
            else if (!flag || !(hit.collider is TerrainCollider))
            {
                this._placePos = vector4;
                this._placeRot = quaternion;
                this.validLocation = false;
            }
            else
            {
                bool flag4 = false;
                foreach (StructureMaster master2 in StructureMaster.AllStructuresWithBounds)
                {
                    if (master2.containedBounds.Intersects(new Bounds(zero, new Vector3(5f, 5f, 4f))))
                    {
                        flag4 = true;
                        break;
                    }
                }
                if (!flag4)
                {
                    this._placePos = zero;
                    this._placeRot = TransformHelpers.LookRotationForcedUp(character.eyesAngles.forward, Vector3.up) * this.desiredRotation;
                    this.validLocation = true;
                }
            }
            if (!base.datablock.CheckBlockers(this._placePos))
            {
                this.validLocation = false;
            }
            Color red = Color.red;
            if (this.validLocation)
            {
                red = Color.green;
            }
            red.a = 0.5f + (Mathf.Abs(Mathf.Sin(Time.time * 8f)) * 0.25f);
            if (this._materialProps != null)
            {
                this._materialProps.Clear();
                this._materialProps.AddColor("_EmissionColor", red);
                this._materialProps.AddVector("_MainTex_ST", new Vector4(1f, 1f, 0f, Mathf.Repeat(Time.time, 30f)));
            }
            if (!this.validLocation)
            {
                this._placePos = zero;
            }
            this.RenderDeployPreview(this._placePos, this._placeRot);
        }
    }
}

