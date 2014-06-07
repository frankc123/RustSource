using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager _spawnMan;
    public static SpawnData[] _spawnPoints;
    private const float kRandomizeSpawnRadius = 10f;

    public virtual void AddPlayerSpawn(GameObject spawn)
    {
        ServerManagement.Get().AddPlayerSpawn(spawn);
    }

    private void Awake()
    {
        _spawnMan = this;
        this.InstallSpawns();
    }

    public SpawnManager Get()
    {
        return _spawnMan;
    }

    public static void GetCloseSpawn(Vector3 point, out Vector3 pos, out Quaternion rot)
    {
        float positiveInfinity = float.PositiveInfinity;
        int index = -1;
        for (int i = 0; i < _spawnPoints.Length; i++)
        {
            Vector3 vector;
            vector.x = point.x - _spawnPoints[i].pos.x;
            vector.y = point.y - _spawnPoints[i].pos.y;
            vector.z = point.z - _spawnPoints[i].pos.z;
            float num4 = ((vector.x * vector.x) + (vector.y * vector.y)) + (vector.z * vector.z);
            if ((num4 < positiveInfinity) && (num4 > 40f))
            {
                positiveInfinity = num4;
                index = i;
            }
        }
        if (index == -1)
        {
            GetRandomSpawn(out pos, out rot);
        }
        else
        {
            pos = _spawnPoints[index].pos;
            rot = _spawnPoints[index].rot;
        }
        RandomizeAndScanSpawnPosition(ref pos);
    }

    public static void GetClosestSpawn(Vector3 point, out Vector3 pos, out Quaternion rot)
    {
        float positiveInfinity = float.PositiveInfinity;
        int index = -1;
        for (int i = 0; i < _spawnPoints.Length; i++)
        {
            Vector3 vector;
            vector.x = point.x - _spawnPoints[i].pos.x;
            vector.y = point.y - _spawnPoints[i].pos.y;
            vector.z = point.z - _spawnPoints[i].pos.z;
            float num4 = ((vector.x * vector.x) + (vector.y * vector.y)) + (vector.z * vector.z);
            if (num4 < positiveInfinity)
            {
                positiveInfinity = num4;
                index = i;
            }
        }
        if (index == -1)
        {
            GetRandomSpawn(out pos, out rot);
        }
        else
        {
            pos = _spawnPoints[index].pos;
            rot = _spawnPoints[index].rot;
        }
        RandomizeAndScanSpawnPosition(ref pos);
    }

    public static void GetFarthestSpawn(Vector3 point, out Vector3 pos, out Quaternion rot)
    {
        float negativeInfinity = float.NegativeInfinity;
        int index = -1;
        for (int i = 0; i < _spawnPoints.Length; i++)
        {
            Vector3 vector;
            vector.x = point.x - _spawnPoints[i].pos.x;
            vector.y = point.y - _spawnPoints[i].pos.y;
            vector.z = point.z - _spawnPoints[i].pos.z;
            float num4 = ((vector.x * vector.x) + (vector.y * vector.y)) + (vector.z * vector.z);
            if (num4 > negativeInfinity)
            {
                negativeInfinity = num4;
                index = i;
            }
        }
        if (index == -1)
        {
            GetRandomSpawn(out pos, out rot);
        }
        else
        {
            pos = _spawnPoints[index].pos;
            rot = _spawnPoints[index].rot;
        }
        RandomizeAndScanSpawnPosition(ref pos);
    }

    public static void GetRandomSpawn(out Vector3 pos, out Quaternion rot)
    {
        int index = Random.Range(0, _spawnPoints.Length);
        pos = _spawnPoints[index].pos;
        rot = _spawnPoints[index].rot;
        RandomizeAndScanSpawnPosition(ref pos);
    }

    private void InstallSpawns()
    {
        _spawnPoints = new SpawnData[base.transform.childCount];
        for (int i = 0; i < base.transform.childCount; i++)
        {
            Transform child = base.transform.GetChild(i);
            _spawnPoints[i].pos = child.position;
            _spawnPoints[i].rot = child.rotation;
        }
        IEnumerator enumerator = base.transform.GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                Transform current = (Transform) enumerator.Current;
                Object.Destroy(current.gameObject);
            }
        }
        finally
        {
            IDisposable disposable = enumerator as IDisposable;
            if (disposable == null)
            {
            }
            disposable.Dispose();
        }
    }

    public static bool RandomizeAndScanSpawnPosition(ref Vector3 pos)
    {
        Vector3 vector;
        Vector3 vector2;
        RaycastHit hit;
        Vector2 vector3 = (Vector2) (Random.insideUnitCircle * 10f);
        vector2.x = vector.x = pos.x + vector3.x;
        vector.y = pos.y + 2000f;
        vector2.y = pos.y - 500f;
        vector2.z = vector.z = pos.z + vector3.y;
        if (!Physics.Linecast(vector, vector2, out hit, 0x80401))
        {
            return false;
        }
        pos = hit.point;
        pos.y += hit.normal.y * 0.25f;
        return true;
    }

    public virtual void RemovePlayerSpawn(GameObject spawn)
    {
        ServerManagement management = ServerManagement.Get();
        if (management != null)
        {
            management.RemovePlayerSpawn(spawn);
        }
    }

    private void Update()
    {
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SpawnData
    {
        public Vector3 pos;
        public Quaternion rot;
    }
}

