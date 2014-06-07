using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SkinnedMeshRenderer))]
public class BoundHack : MonoBehaviour
{
    private SkinnedMeshRenderer renderer;
    private static HashSet<BoundHack> renders;
    public Transform rootbone;

    public static void Achieve(Vector3 centroid)
    {
        if (renders != null)
        {
            foreach (BoundHack hack in renders)
            {
                hack.renderer.localBounds = new Bounds(((hack.rootbone == null) ? hack.transform : hack.rootbone).InverseTransformPoint(centroid), new Vector3(100f, 100f, 100f));
            }
        }
    }

    private void Awake()
    {
        this.renderer = base.renderer as SkinnedMeshRenderer;
        if (renders == null)
        {
            renders = new HashSet<BoundHack>();
        }
        renders.Add(this);
    }

    private void OnDestroy()
    {
        if (renders != null)
        {
            renders.Remove(this);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (base.renderer != null)
        {
            Gizmos.DrawWireCube(base.renderer.bounds.center, base.renderer.bounds.size);
        }
        if ((this.rootbone != null) && (this.renderer != null))
        {
            Gizmos.color = new Color(0.8f, 0.8f, 1f, 0.1f);
            Gizmos.matrix = this.rootbone.localToWorldMatrix;
            Gizmos.DrawCube(this.renderer.localBounds.center, this.renderer.localBounds.size);
        }
    }
}

