using System;
using UnityEngine;

public class LootableObjectSpawner : MonoBehaviour
{
    public ChancePick[] _lootableChances;
    public bool spawnOnStart = true;
    public float spawnTimeMax = 10f;
    public float spawnTimeMin = 5f;

    private void Awake()
    {
        Object.Destroy(base.gameObject);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(base.transform.position, 0.5f);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(base.transform.position, base.transform.position + ((Vector3) (base.transform.forward * 1f)));
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(base.transform.position, 0.5f);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(base.transform.position, base.transform.position + ((Vector3) (base.transform.forward * 1f)));
    }

    [Serializable]
    public class ChancePick
    {
        public LootableObject obj;
        public float weight;
    }
}

