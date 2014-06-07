using UnityEngine;

public class CharacterItemDropPrefabTrait : CharacterTrait
{
    [SerializeField]
    private GameObject _prefab;

    public GameObject prefab
    {
        get
        {
            return this._prefab;
        }
    }
}

