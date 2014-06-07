using System;
using UnityEngine;

[AddComponentMenu("")]
public class VisManager : MonoBehaviour
{
    private static bool isUpdatingVisiblity;

    private void Reset()
    {
        Debug.LogError("REMOVE ME NOW, I GET GENERATED AT RUN TIME", this);
    }

    private void Update()
    {
        if (!isUpdatingVisiblity)
        {
            isUpdatingVisiblity = true;
            try
            {
                VisNode.Process();
            }
            catch (Exception exception)
            {
                Debug.LogError(string.Format("{0}\n-- Vis data potentially compromised\n", exception));
            }
            finally
            {
                isUpdatingVisiblity = false;
            }
        }
    }

    public static bool guardedUpdate
    {
        get
        {
            return isUpdatingVisiblity;
        }
    }
}

