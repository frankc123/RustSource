using System;
using UnityEngine;

public class RustLoaderInstantiateOnComplete : MonoBehaviour
{
    public GameObject[] prefabs;

    private void InstantiatePrefab(GameObject prefab)
    {
        try
        {
            Object.Instantiate(prefab).name = prefab.name;
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
        }
    }

    private void OnRustReady()
    {
        if (this.prefabs != null)
        {
            foreach (GameObject obj2 in this.prefabs)
            {
                if (obj2 != null)
                {
                    this.InstantiatePrefab(obj2);
                }
            }
        }
    }

    private void Reset()
    {
        Object[] objArray = Object.FindObjectsOfType(typeof(RustLoader));
        if (objArray.Length > 0)
        {
            ((RustLoader) objArray[0]).AddMessageReceiver(base.gameObject);
        }
    }
}

