using System;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    [NonSerialized]
    private int exaustCount;
    public float radius = 40f;
    public int targetPopulation = 10;
    public float thinkDelay = 60f;
    public string[] zombiePrefabs = new string[] { "npc_zombie" };

    private ZombieSpawner()
    {
    }

    private void Awake()
    {
        Object.Destroy(base.gameObject);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.3f, 0.3f, 0.3f, 0.5f);
        Gizmos.DrawWireSphere(base.transform.position, this.radius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(base.transform.position, (Vector3) (Vector3.one * 0.5f));
    }
}

