using System;
using System.Collections;
using UnityEngine;

public class TreeExplosion : MonoBehaviour
{
    public float BlastForce = 30000f;
    public float BlastRange = 30f;
    public GameObject DeadReplace;
    public GameObject Explosion;

    private void Explode()
    {
        Object.Instantiate(this.Explosion, base.transform.position, Quaternion.identity);
        TerrainData terrainData = Terrain.activeTerrain.terrainData;
        ArrayList list = new ArrayList();
        foreach (TreeInstance instance in terrainData.treeInstances)
        {
            if (Vector3.Distance(Vector3.Scale(instance.position, terrainData.size) + Terrain.activeTerrain.transform.position, base.transform.position) < this.BlastRange)
            {
                GameObject obj2 = Object.Instantiate(this.DeadReplace, Vector3.Scale(instance.position, terrainData.size) + Terrain.activeTerrain.transform.position, Quaternion.identity) as GameObject;
                obj2.rigidbody.maxAngularVelocity = 1f;
                obj2.rigidbody.AddExplosionForce(this.BlastForce, base.transform.position, 20f + (this.BlastRange * 5f), -20f);
            }
            else
            {
                list.Add(instance);
            }
        }
        terrainData.treeInstances = (TreeInstance[]) list.ToArray(typeof(TreeInstance));
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            this.Explode();
        }
    }
}

