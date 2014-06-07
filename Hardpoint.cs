using Facepunch.MeshBatch;
using System;
using UnityEngine;

public class Hardpoint : IDRemote
{
    private HardpointMaster _master;
    private DeployableObject holding;
    public hardpoint_type type = hardpoint_type.Generic;

    public void Awake()
    {
        HardpointMaster component = base.idMain.GetComponent<HardpointMaster>();
        if (component != null)
        {
            this.SetMaster(component);
        }
        base.Awake();
    }

    public static Hardpoint GetHardpointFromRay(Ray ray, hardpoint_type type)
    {
        RaycastHit hit;
        bool flag;
        MeshBatchInstance instance;
        if (MeshBatchPhysics.Raycast(ray, out hit, 10f, out flag, out instance))
        {
            IDMain main = !flag ? IDBase.GetMain(hit.collider) : instance.idMain;
            if (main != null)
            {
                HardpointMaster component = main.GetComponent<HardpointMaster>();
                if (component != null)
                {
                    return component.GetHardpointNear(hit.point, type);
                }
            }
        }
        return null;
    }

    public HardpointMaster GetMaster()
    {
        return this._master;
    }

    public bool IsFree()
    {
        return (this.holding == null);
    }

    public void OnDestroy()
    {
        base.OnDestroy();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(base.transform.position, new Vector3(0.2f, 0.2f, 0.2f));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(base.transform.position, new Vector3(0.2f, 0.2f, 0.2f));
    }

    public void SetMaster(HardpointMaster master)
    {
        this._master = master;
        master.AddHardpoint(this);
    }

    public enum hardpoint_type
    {
        None,
        Generic,
        Door,
        Turret,
        Gate,
        Window
    }
}

