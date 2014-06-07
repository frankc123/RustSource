using System;
using UnityEngine;

public class FolderHelper : MonoBehaviour
{
    private void Awake()
    {
        base.transform.DetachChildren();
        Object.Destroy(base.gameObject);
    }
}

