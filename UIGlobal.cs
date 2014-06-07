using System;
using UnityEngine;

[AddComponentMenu("")]
public class UIGlobal : MonoBehaviour
{
    private static UIGlobal g;

    public static void EnsureGlobal()
    {
        if (Application.isPlaying && (g == null))
        {
            Type[] components = new Type[] { typeof(UIGlobal) };
            GameObject target = new GameObject("__UIGlobal", components);
            Object.DontDestroyOnLoad(target);
            g = target.GetComponent<UIGlobal>();
        }
    }

    private void LateUpdate()
    {
        UIWidget.GlobalUpdate();
        UIPanel.GlobalUpdate();
    }
}

