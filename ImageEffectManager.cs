using System;
using System.Collections.Generic;
using UnityEngine;

public class ImageEffectManager : MonoBehaviour
{
    private static ImageEffectManager singleton;
    private static Dictionary<Type, bool> states = new Dictionary<Type, bool>();

    public static bool GetEnabled<T>() where T: MonoBehaviour
    {
        return (!states.ContainsKey(typeof(T)) ? true : states[typeof(T)]);
    }

    public static T GetInstance<T>() where T: MonoBehaviour
    {
        return ((singleton == null) ? null : singleton.GetComponent<T>());
    }

    protected void OnDisable()
    {
        if (singleton == this)
        {
            singleton = null;
        }
    }

    protected void OnEnable()
    {
        singleton = this;
    }

    public static void SetEnabled<T>(bool value) where T: MonoBehaviour
    {
        if (GetInstance<T>() != null)
        {
            GetInstance<T>().enabled = value;
        }
        if (!states.ContainsKey(typeof(T)))
        {
            states.Add(typeof(T), value);
        }
        else
        {
            states[typeof(T)] = value;
        }
    }

    protected void Start()
    {
        foreach (MonoBehaviour behaviour in base.GetComponents<MonoBehaviour>())
        {
            if (!behaviour.enabled)
            {
                return;
            }
            Type key = behaviour.GetType();
            if (states.ContainsKey(key))
            {
                behaviour.enabled = states[key];
            }
        }
    }
}

