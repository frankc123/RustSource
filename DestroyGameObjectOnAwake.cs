using System;
using UnityEngine;

public class DestroyGameObjectOnAwake : MonoBehaviour
{
    private void Awake()
    {
        Object.Destroy(base.gameObject);
    }
}

