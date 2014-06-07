using System;
using UnityEngine;

public class DisplayRenderBounds : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Renderer renderer = base.renderer;
        if (renderer != null)
        {
            Bounds localBounds = renderer.bounds;
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(localBounds.center, localBounds.size);
            if (renderer is SkinnedMeshRenderer)
            {
                SkinnedMeshRenderer renderer2 = renderer as SkinnedMeshRenderer;
                Gizmos.color = Color.yellow;
                Gizmos.matrix = renderer2.localToWorldMatrix;
                localBounds = renderer2.localBounds;
                Gizmos.DrawWireCube(localBounds.center, localBounds.size);
            }
            else
            {
                MeshFilter component = base.GetComponent<MeshFilter>();
                if (component != null)
                {
                    Mesh sharedMesh = component.sharedMesh;
                    if (sharedMesh != null)
                    {
                        Gizmos.color = Color.magenta;
                        Gizmos.matrix = base.transform.localToWorldMatrix;
                        localBounds = sharedMesh.bounds;
                        Gizmos.DrawWireCube(localBounds.center, localBounds.size);
                    }
                }
            }
        }
    }
}

