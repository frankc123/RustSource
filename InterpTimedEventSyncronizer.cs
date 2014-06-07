using System;
using UnityEngine;

public class InterpTimedEventSyncronizer : MonoBehaviour
{
    private static bool exists;
    private static InterpTimedEventSyncronizer singleton;
    private static bool syncronizationPaused;

    private void Awake()
    {
        if (singleton != null)
        {
            Debug.LogWarning("Destroying old singleton!", singleton.gameObject);
            Object.Destroy(singleton);
        }
        singleton = this;
        exists = true;
    }

    private void OnDestroy()
    {
        if (singleton == this)
        {
            try
            {
                InterpTimedEvent.Clear(false);
            }
            finally
            {
                singleton = null;
                exists = false;
            }
        }
    }

    private void Update()
    {
        InterpTimedEvent.Catchup();
    }

    internal static bool available
    {
        get
        {
            return exists;
        }
    }

    internal static bool paused
    {
        get
        {
            return syncronizationPaused;
        }
        set
        {
            syncronizationPaused = value;
            if (singleton != null)
            {
                singleton.enabled = !syncronizationPaused;
            }
        }
    }
}

