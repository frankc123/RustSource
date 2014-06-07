using System;
using System.Collections.Generic;
using UnityEngine;

public class TreeColliderAdd : MonoBehaviour
{
    private Vector3[] convertedTreePositions;
    public Vector3 lastPos;
    private int pooledColliders = 500;
    public Terrain terrain;
    private TerrainData terrainData;
    private List<GameObject> treeColliderPool;
    public GameObject treeColliderPrefab;
    private List<GameObject> usedCollidersPool;

    private void AddNewColliders()
    {
        this.CleanupOldColliders();
        Vector3 position = base.transform.position;
        int num = 0;
        int index = 0;
        int count = this.treeColliderPool.Count;
        int num5 = 0;
        int length = this.convertedTreePositions.Length;
        while (num5 < length)
        {
            Vector3 vector2;
            vector2.x = this.convertedTreePositions[num5].x - position.x;
            vector2.y = this.convertedTreePositions[num5].y - position.y;
            vector2.z = this.convertedTreePositions[num5].z - position.z;
            float num3 = ((vector2.x * vector2.x) + (vector2.y * vector2.y)) + (vector2.z * vector2.z);
            if (num3 <= 40000f)
            {
                GameObject freeTreeCollider = this.GetFreeTreeCollider();
                if (freeTreeCollider == null)
                {
                    return;
                }
                Vector3 vector3 = this.convertedTreePositions[num5];
                freeTreeCollider.transform.position = vector3;
                this.usedCollidersPool.Add(freeTreeCollider);
                this.convertedTreePositions[num5] = this.convertedTreePositions[index];
                this.convertedTreePositions[index++] = vector3;
                if (--count == 0)
                {
                    break;
                }
            }
            num++;
            num5++;
        }
    }

    private void CleanupOldColliders()
    {
        foreach (GameObject obj2 in this.usedCollidersPool)
        {
            this.treeColliderPool.Add(obj2);
        }
        this.usedCollidersPool.Clear();
    }

    public GameObject GetFreeTreeCollider()
    {
        if (this.treeColliderPool.Count > 0)
        {
            GameObject obj2 = this.treeColliderPool[0];
            this.treeColliderPool.RemoveAt(0);
            return obj2;
        }
        return null;
    }

    private void Start()
    {
        this.terrainData = this.terrain.terrainData;
        this.lastPos = base.transform.position;
        this.treeColliderPool = new List<GameObject>();
        this.usedCollidersPool = new List<GameObject>();
        this.convertedTreePositions = new Vector3[this.terrainData.treeInstances.Length];
        int index = 0;
        foreach (TreeInstance instance in this.terrainData.treeInstances)
        {
            this.convertedTreePositions[index] = Vector3.Scale(instance.position, this.terrainData.size) + this.terrain.transform.position;
            index++;
        }
        Debug.Log("Tree instances length:" + this.terrainData.treeInstances.Length);
        for (int i = 0; i < this.pooledColliders; i++)
        {
            GameObject item = Object.Instantiate(this.treeColliderPrefab, new Vector3(0f, -20000f, 0f), Quaternion.identity) as GameObject;
            this.treeColliderPool.Add(item);
        }
    }

    private void Update()
    {
        Vector3 position = base.transform.position;
        if (Vector3.Distance(position, this.lastPos) >= 100f)
        {
            this.AddNewColliders();
            this.lastPos = position;
        }
    }
}

