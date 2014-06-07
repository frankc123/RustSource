using Facepunch;
using System;
using UnityEngine;

public class BloodHelper : MonoBehaviour
{
    private static GameObject bloodDecalPrefab;

    private static void BleedDir(Vector3 startPos, Vector3 dir, int hitMask)
    {
        RaycastHit hit;
        Ray ray = new Ray(startPos + ((Vector3) (dir * 0.25f)), dir);
        if (Physics.Raycast(ray, out hit, 4f, hitMask) && ((bloodDecalPrefab != null) || Bundling.Load<GameObject>("content/effect/BloodDecal", out bloodDecalPrefab)))
        {
            Quaternion quaternion = Quaternion.LookRotation(hit.normal);
            GameObject obj2 = Object.Instantiate(bloodDecalPrefab, hit.point + ((Vector3) (hit.normal * Random.Range((float) 0.025f, (float) 0.035f))), quaternion * Quaternion.Euler(0f, 0f, (float) Random.Range(0, 360))) as GameObject;
            Object.Destroy(obj2, 12f);
        }
    }
}

