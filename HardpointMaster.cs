using System;
using System.Collections.Generic;
using UnityEngine;

public class HardpointMaster : IDLocal
{
    public List<Hardpoint> _points;

    public void AddHardpoint(Hardpoint point)
    {
        this._points.Add(point);
    }

    public void Awake()
    {
        this._points = new List<Hardpoint>();
    }

    public Hardpoint GetHardpointNear(Vector3 worldPos, Hardpoint.hardpoint_type type)
    {
        return this.GetHardpointNear(worldPos, 3f, type);
    }

    public Hardpoint GetHardpointNear(Vector3 worldPos, float maxRange, Hardpoint.hardpoint_type type)
    {
        foreach (Hardpoint hardpoint in this._points)
        {
            if (((hardpoint.type == type) && hardpoint.IsFree()) && (Vector3.Distance(hardpoint.transform.position, worldPos) <= maxRange))
            {
                return hardpoint;
            }
        }
        return null;
    }

    public TransCarrier GetTransCarrier()
    {
        return base.idMain.GetLocal<TransCarrier>();
    }
}

