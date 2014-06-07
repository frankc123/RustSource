using System;
using UnityEngine;

public class SceneChildMeshes : MonoBehaviour
{
    private static SceneChildMeshes lastFound;
    [SerializeField]
    private Mesh[] sceneMeshes;
    [SerializeField]
    private Mesh[] treeMeshes;

    private static SceneChildMeshes GetMapSingleton(bool canCreate)
    {
        if (lastFound == null)
        {
            Object[] objArray = Object.FindObjectsOfType(typeof(SceneChildMeshes));
            if (objArray.Length == 0)
            {
                if (canCreate)
                {
                    Type[] components = new Type[] { typeof(SceneChildMeshes) };
                    GameObject obj3 = new GameObject("__Scene Child Meshes", components) {
                        hideFlags = HideFlags.HideInHierarchy
                    };
                    lastFound = obj3.GetComponent<SceneChildMeshes>();
                }
            }
            else
            {
                lastFound = (SceneChildMeshes) objArray[0];
            }
        }
        return lastFound;
    }
}

