using System;
using System.Collections.Generic;
using UnityEngine;

public class HudEnabled : MonoBehaviour
{
    private static bool GReady;
    private static bool On;

    private void Awake()
    {
        GameObject gameObject = base.gameObject;
        G.All.Add(gameObject);
        gameObject.SetActive(On);
    }

    public static void Disable()
    {
        Set(false);
    }

    public static void Enable()
    {
        Set(true);
    }

    private void OnDestroy()
    {
        if (GReady)
        {
            G.All.Remove(base.gameObject);
        }
    }

    public static void Set(bool enable)
    {
        if (On != enable)
        {
            Toggle();
        }
    }

    public static void Toggle()
    {
        On = !On;
        if (GReady)
        {
            foreach (GameObject obj2 in G.All)
            {
                obj2.SetActive(On);
            }
        }
    }

    private static class G
    {
        public static readonly HashSet<GameObject> All = new HashSet<GameObject>();

        static G()
        {
            HudEnabled.GReady = true;
        }
    }
}

