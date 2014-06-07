using System;
using UnityEngine;

public class SurfaceInfo : MonoBehaviour
{
    public SurfaceInfoObject surface;

    public static void DoImpact(GameObject go, SurfaceInfoObject.ImpactType type, Vector3 worldPos, Quaternion rotation)
    {
        Object.Destroy(Object.Instantiate(GetSurfaceInfoFor(go, worldPos).GetImpactEffect(type), worldPos, rotation), 1f);
    }

    public static SurfaceInfoObject GetSurfaceInfoFor(Collider collider, Vector3 worldPos)
    {
        return GetSurfaceInfoFor(collider.gameObject, worldPos);
    }

    public static SurfaceInfoObject GetSurfaceInfoFor(GameObject obj, Vector3 worldPos)
    {
        SurfaceInfo component = obj.GetComponent<SurfaceInfo>();
        if (component != null)
        {
            return component.SurfaceObj(worldPos);
        }
        IDBase base2 = obj.GetComponent<IDBase>();
        if (base2 != null)
        {
            SurfaceInfo info2 = base2.idMain.GetComponent<SurfaceInfo>();
            if (info2 != null)
            {
                return info2.SurfaceObj(worldPos);
            }
        }
        return SurfaceInfoObject.GetDefault();
    }

    public virtual SurfaceInfoObject SurfaceObj()
    {
        return this.surface;
    }

    public virtual SurfaceInfoObject SurfaceObj(Vector3 worldPos)
    {
        return this.surface;
    }
}

