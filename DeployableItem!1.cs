using System;
using uLink;
using UnityEngine;

public abstract class DeployableItem<T> : HeldItem<T> where T: DeployableItemDataBlock
{
    [NonSerialized]
    private GameGizmo.Instance _aimGizmo;
    [NonSerialized]
    private bool _aimHelper;
    protected float _nextPlaceTime;
    protected PrefabRenderer _prefabRenderer;

    protected DeployableItem(T db) : base(db)
    {
        this._nextPlaceTime = Time.time;
    }

    public virtual bool CanPlace()
    {
        Vector3 vector;
        Quaternion quaternion;
        TransCarrier carrier;
        if (base.datablock.ObjectToPlace == null)
        {
            return false;
        }
        bool flag = base.datablock.CheckPlacement(base.character.eyesRay, out vector, out quaternion, out carrier);
        this._aimGizmo.rotation = quaternion;
        this._aimGizmo.position = vector;
        if (!flag)
        {
            return false;
        }
        return (this._nextPlaceTime <= Time.time);
    }

    public virtual void DoPlace()
    {
        Vector3 vector;
        Quaternion quaternion;
        TransCarrier carrier;
        Ray eyesRay = base.character.eyesRay;
        base.datablock.CheckPlacement(eyesRay, out vector, out quaternion, out carrier);
        this._nextPlaceTime = Time.time + 0.5f;
        object[] arguments = new object[] { eyesRay.origin, eyesRay.direction };
        base.itemRepresentation.Action(1, RPCMode.Server, arguments);
    }

    public override void ItemPreFrame(ref HumanController.InputSample sample)
    {
        base.ItemPreFrame(ref sample);
        if (this._aimHelper && (sample.attack && this.CanPlace()))
        {
            Character idMain = base.inventory.idMain as Character;
            if ((idMain == null) || idMain.stateFlags.grounded)
            {
                this.DoPlace();
            }
        }
    }

    protected override void OnSetActive(bool isActive)
    {
        base.OnSetActive(isActive);
        if (isActive)
        {
            if (!this._aimHelper && (base.datablock.aimGizmo != null))
            {
                this._aimHelper = base.datablock.aimGizmo.Create<GameGizmo.Instance>(out this._aimGizmo);
            }
        }
        else if (this._aimHelper)
        {
            this._aimHelper = !this._aimGizmo.gameGizmo.Destroy<GameGizmo.Instance>(ref this._aimGizmo);
        }
    }

    public override void PreCameraRender()
    {
        if (this._aimHelper)
        {
            Vector3 vector;
            Quaternion quaternion;
            TransCarrier carrier;
            bool flag = base.datablock.CheckPlacement(base.character.eyesRay, out vector, out quaternion, out carrier);
            Color color = !flag ? this._aimGizmo.gameGizmo.badColor : this._aimGizmo.gameGizmo.goodColor;
            this._aimGizmo.propertyBlock.Clear();
            this._aimGizmo.propertyBlock.AddColor("_EmissionColor", color);
            Vector4 vector2 = new Vector4(1f, 1f, 0f, Mathf.Repeat(Time.time, 30f));
            this._aimGizmo.propertyBlock.AddVector("_MainTex_ST", vector2);
            this._aimGizmo.propertyBlock.AddVector("_GizmoWorldPos", vector);
            if (this._aimGizmo is GameGizmoWaveAnimation.Instance)
            {
                GameGizmoWaveAnimation.Instance instance = (GameGizmoWaveAnimation.Instance) this._aimGizmo;
                if (flag)
                {
                    instance.propertyBlock.AddFloat("_PushOut", (float) instance.value);
                }
                else
                {
                    instance.propertyBlock.AddFloat("_PushIn", (float) instance.value);
                    instance.propertyBlock.AddFloat("_PushOut", (float) -instance.value);
                }
            }
            this.RenderDeployPreview(vector, quaternion, carrier);
            this._aimGizmo.Render();
        }
    }

    public virtual void RenderDeployPreview(Vector3 point, Quaternion rot, TransCarrier carrier)
    {
        if (this._aimGizmo != null)
        {
            this._aimGizmo.rotation = rot;
            this._aimGizmo.position = point;
        }
        if (this._prefabRenderer == null)
        {
            DeployableObject objectToPlace = base.datablock.ObjectToPlace;
            if (objectToPlace == null)
            {
                return;
            }
            this._prefabRenderer = PrefabRenderer.GetOrCreateRender(objectToPlace.gameObject);
        }
        Material overrideMat = base.datablock.overrideMat;
        if (overrideMat != null)
        {
            this._prefabRenderer.RenderOneMaterial(MountedCamera.main.camera, Matrix4x4.TRS(point, rot, base.datablock.ObjectToPlace.transform.localScale), this._aimGizmo.propertyBlock, overrideMat);
        }
        else
        {
            this._prefabRenderer.Render(MountedCamera.main.camera, Matrix4x4.TRS(point, rot, base.datablock.ObjectToPlace.transform.localScale), this._aimGizmo.propertyBlock, null);
        }
        if (this._aimGizmo != null)
        {
            bool flag = false;
            if (carrier != null)
            {
                Renderer renderer = carrier.renderer;
                if (((renderer is MeshRenderer) && (renderer != null)) && renderer.enabled)
                {
                    flag = true;
                    this._aimGizmo.carrierRenderer = (MeshRenderer) renderer;
                }
            }
            if (!flag)
            {
                this._aimGizmo.carrierRenderer = null;
            }
        }
    }
}

