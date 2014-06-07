using System;
using System.Collections.Generic;
using UnityEngine;

public class WoodBlockerTemp : MonoBehaviour
{
    public static List<WoodBlockerTemp> _blockers;
    public float numWood;

    private void Awake()
    {
        TryInitBlockers();
        this.numWood = Random.Range(10, 15);
        _blockers.Add(this);
        Object.Destroy(base.gameObject, 300f);
    }

    public void ConsumeWood(float consume)
    {
        this.numWood -= consume;
        if (this.numWood < 0f)
        {
            this.numWood = 0f;
        }
    }

    public static WoodBlockerTemp GetBlockerForPoint(Vector3 point)
    {
        TryInitBlockers();
        foreach (WoodBlockerTemp temp in _blockers)
        {
            if (Vector3.Distance(temp.transform.position, point) < 4f)
            {
                return temp;
            }
        }
        WoodBlockerTemp temp2 = (WoodBlockerTemp) GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent("WoodBlockerTemp");
        temp2.renderer.enabled = false;
        temp2.collider.enabled = false;
        temp2.transform.position = point;
        temp2.name = "WBT";
        return temp2;
    }

    public float GetWoodLeft()
    {
        return this.numWood;
    }

    public bool HasWood()
    {
        return (this.numWood >= 1f);
    }

    public void OnDestroy()
    {
        _blockers.Remove(this);
    }

    private static void TryInitBlockers()
    {
        if (_blockers == null)
        {
            _blockers = new List<WoodBlockerTemp>();
        }
    }
}

