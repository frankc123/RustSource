using System;
using UnityEngine;

public class GUIHide : MonoBehaviour
{
    public static void SetVisible(bool bShow)
    {
        foreach (GUIHide hide in Resources.FindObjectsOfTypeAll(typeof(GUIHide)))
        {
            if (hide.gameObject == null)
            {
                return;
            }
            hide.gameObject.SetActive(bShow);
        }
    }
}

