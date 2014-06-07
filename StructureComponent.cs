using Facepunch;
using System;
using uLink;
using UnityEngine;

[NGCAutoAddScript, RequireComponent(typeof(TakeDamage))]
public class StructureComponent : IDMain, IServerSaveable
{
    public StructureMaster _master;
    public StructureMaster.StructureMaterialType _materialType;
    [NonSerialized]
    private bool addedDestroyCallback;
    public GameObject deathEffect;
    [NonSerialized]
    private HealthDimmer healthDimmer;
    public float Height;
    private static bool logFailures;
    protected float oldHealth;
    public StructureComponentType type;
    public float Width;

    public StructureComponent() : base(IDFlags.Unknown)
    {
        this.Width = 5f;
        this.Height = 1f;
    }

    public virtual bool CheckLocation(StructureMaster master, Vector3 placePos, Quaternion placeRot)
    {
        bool flag = false;
        bool flag2 = false;
        Vector3 vector = master.transform.InverseTransformPoint(placePos);
        if ((master.GetMaterialType() != StructureMaster.StructureMaterialType.UNSET) && (master.GetMaterialType() != this.GetMaterialType()))
        {
            if (logFailures)
            {
                Debug.Log("Not proper material type, master is :" + master.GetMaterialType());
            }
            return false;
        }
        StructureComponent componentFromPositionWorld = master.GetComponentFromPositionWorld(placePos);
        if (componentFromPositionWorld != null)
        {
            if (logFailures)
            {
                Debug.Log("Occupied space", componentFromPositionWorld);
            }
            flag = true;
        }
        StructureComponent component2 = master.CompByLocal(vector - new Vector3(0f, StructureMaster.gridSpacingY, 0f));
        if ((this.type != StructureComponentType.Foundation) && master.GetFoundationForPoint(placePos))
        {
            flag2 = true;
        }
        if (((this.type == StructureComponentType.Wall) || (this.type == StructureComponentType.Doorway)) || (this.type == StructureComponentType.WindowWall))
        {
            if (!flag)
            {
                Vector3 worldPos = placePos + ((Vector3) ((placeRot * -Vector3.right) * 2.5f));
                StructureComponent context = master.GetComponentFromPositionWorld(worldPos);
                Vector3 vector3 = placePos + ((Vector3) ((placeRot * Vector3.right) * 2.5f));
                StructureComponent component4 = master.GetComponentFromPositionWorld(vector3);
                if (logFailures)
                {
                    Debug.DrawLine(worldPos, vector3, Color.cyan);
                }
                if ((context != null) && (component4 != null))
                {
                    bool flag4;
                    bool flag5;
                    if (context.type != StructureComponentType.Pillar)
                    {
                        if (logFailures)
                        {
                            Debug.Log("Left was not acceptable", context);
                        }
                        flag4 = false;
                    }
                    else
                    {
                        flag4 = true;
                    }
                    if (component4.type != StructureComponentType.Pillar)
                    {
                        if (logFailures)
                        {
                            Debug.Log("Right was not acceptable", component4);
                        }
                        flag5 = false;
                    }
                    else
                    {
                        flag5 = true;
                    }
                    return (flag4 && flag5);
                }
                if (logFailures)
                {
                    if (context == null)
                    {
                        Debug.Log("Did not find left");
                    }
                    if (component4 == null)
                    {
                        Debug.Log("Did not find right");
                    }
                }
            }
            return false;
        }
        if (this.type == StructureComponentType.Foundation)
        {
            foreach (StructureMaster master2 in StructureMaster.AllStructuresWithBounds)
            {
                if ((master2 != master) && master2.containedBounds.Intersects(new Bounds(placePos, new Vector3(5f, 5f, 4f))))
                {
                    if (logFailures)
                    {
                        Debug.Log("Too close to something");
                    }
                    return false;
                }
            }
            bool flag6 = master.IsValidFoundationSpot(placePos);
            if (logFailures)
            {
                Debug.Log(string.Concat(new object[] { "returning here : mastervalid:", flag6, "compinplace", componentFromPositionWorld }));
            }
            return (flag6 && (componentFromPositionWorld == 0));
        }
        if (this.type == StructureComponentType.Ramp)
        {
            return ((componentFromPositionWorld == null) && (master.IsValidFoundationSpot(placePos) || ((component2 != null) && ((component2.type == StructureComponentType.Ceiling) || (component2.type == StructureComponentType.Foundation)))));
        }
        if (this.type == StructureComponentType.Pillar)
        {
            return ((((component2 != null) && (component2.type == StructureComponentType.Pillar)) || flag2) && !flag);
        }
        if ((this.type != StructureComponentType.Stairs) && (this.type != StructureComponentType.Ceiling))
        {
            return false;
        }
        if (flag)
        {
            return false;
        }
        Vector3[] vectorArray = new Vector3[] { new Vector3(-2.5f, 0f, -2.5f), new Vector3(2.5f, 0f, 2.5f), new Vector3(-2.5f, 0f, 2.5f), new Vector3(2.5f, 0f, -2.5f) };
        foreach (Vector3 vector4 in vectorArray)
        {
            StructureComponent component5 = master.CompByLocal(vector + vector4);
            if ((component5 == null) || (component5.type != StructureComponentType.Pillar))
            {
                return false;
            }
        }
        return true;
    }

    private void cl_predestroy(NGCView view)
    {
        if (this._master != null)
        {
            this._master.CullComponent(this);
        }
    }

    [RPC]
    public void ClientHealthUpdate(float newHealth)
    {
        this.healthDimmer.UpdateHealthAmount(this, newHealth, false);
    }

    [RPC]
    public void ClientKilled()
    {
        if (this.deathEffect != null)
        {
            GameObject obj2 = Object.Instantiate(this.deathEffect, base.transform.position, base.transform.rotation) as GameObject;
            Object.Destroy(obj2, 5f);
        }
    }

    public StructureMaster.StructureMaterialType GetMaterialType()
    {
        return this._materialType;
    }

    public bool IsPillar()
    {
        return (this.type == StructureComponentType.Pillar);
    }

    public bool IsType(StructureComponentType checkType)
    {
        return (this.type == checkType);
    }

    public bool IsWallType()
    {
        return (((this.type == StructureComponentType.Wall) || (this.type == StructureComponentType.Doorway)) || (this.type == StructureComponentType.WindowWall));
    }

    public void OnHurt(DamageEvent damage)
    {
    }

    protected internal virtual void OnOwnedByMasterStructure(StructureMaster master)
    {
        this._master = master;
        NGCView component = base.GetComponent<NGCView>();
        if (((component != null) && !this.addedDestroyCallback) && (component != null))
        {
            this.addedDestroyCallback = true;
            component.OnPreDestroy += new NGC.EventCallback(this.cl_predestroy);
        }
    }

    protected void OnPoolRetire()
    {
        this.oldHealth = 0f;
        this.addedDestroyCallback = false;
        this.healthDimmer.Reset();
    }

    public void OnRepair()
    {
    }

    [Obsolete("Do not call manually", true), RPC]
    protected void SMSet(NetworkViewID masterViewID)
    {
        NetworkView context = NetworkView.Find(masterViewID);
        if (context != null)
        {
            StructureMaster component = context.GetComponent<StructureMaster>();
            if (component != null)
            {
                component.AppendStructureComponent(this);
            }
            else
            {
                Debug.LogWarning("No Master On GO", context);
            }
        }
        else
        {
            Debug.LogWarning("Couldnt find master view", this);
        }
    }

    public void Touched()
    {
        this._master.Touched();
    }

    [Serializable]
    public enum StructureComponentType
    {
        Pillar,
        Wall,
        Doorway,
        Ceiling,
        Stairs,
        Foundation,
        WindowWall,
        Ramp,
        Last
    }
}

