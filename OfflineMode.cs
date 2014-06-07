using System;
using UnityEngine;

public class OfflineMode : MonoBehaviour
{
    [SerializeField]
    private CharacterPrefab characterPrefab;
    [SerializeField]
    private OfflinePlayer offlinePlayer;
    [SerializeField]
    private bool paused;
    [SerializeField]
    private bool respawn;
    [SerializeField]
    private MountedCamera sceneCameraPrefab;
    [SerializeField]
    private bool teleport;
    [SerializeField]
    private float timeScale = 1f;
    [SerializeField]
    private bool useSceneViewWhenAvailable;

    private void Start()
    {
    }
}

