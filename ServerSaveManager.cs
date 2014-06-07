using System;
using UnityEngine;

[AddComponentMenu("")]
public class ServerSaveManager : MonoBehaviour
{
    [SerializeField, HideInInspector]
    private int[] keys;
    [SerializeField, HideInInspector]
    private int nextID = 1;
    [HideInInspector, SerializeField]
    private ServerSave[] values;
}

