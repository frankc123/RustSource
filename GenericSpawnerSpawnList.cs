using System;
using System.Collections.Generic;
using UnityEngine;

public class GenericSpawnerSpawnList : ScriptableObject
{
    [SerializeField]
    public List<GenericSpawnInstance> _spawnList;

    public List<GenericSpawnInstance> GetCopy()
    {
        List<GenericSpawnInstance> list = new List<GenericSpawnInstance>(this._spawnList.Count);
        foreach (GenericSpawnInstance instance in this._spawnList)
        {
            list.Add(instance.Clone());
        }
        return list;
    }

    [Serializable]
    public class GenericSpawnInstance
    {
        public bool forceStaticInstantiate;
        public int numToSpawnPerTick = 1;
        public string prefabName = string.Empty;
        public List<GameObject> spawned;
        public int targetPopulation;
        public bool useNavmeshSample = true;

        public GenericSpawnerSpawnList.GenericSpawnInstance Clone()
        {
            return new GenericSpawnerSpawnList.GenericSpawnInstance { prefabName = this.prefabName, targetPopulation = this.targetPopulation, numToSpawnPerTick = this.numToSpawnPerTick, forceStaticInstantiate = this.forceStaticInstantiate, spawned = new List<GameObject>() };
        }

        public int GetNumActive()
        {
            return this.spawned.Count;
        }
    }
}

