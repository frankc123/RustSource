using System;
using System.Collections;
using UnityEngine;

public class RPOSPlaqueManager : MonoBehaviour
{
    public GameObject bleedingPlaque;
    public GameObject coldPlaque;

    public void Awake()
    {
        IEnumerator enumerator = base.transform.GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                Transform current = (Transform) enumerator.Current;
                current.gameObject.SetActive(false);
            }
        }
        finally
        {
            IDisposable disposable = enumerator as IDisposable;
            if (disposable == null)
            {
            }
            disposable.Dispose();
        }
    }

    public void SetPlaqueActive(string plaqueName, bool on)
    {
        GameObject gameObject = null;
        IEnumerator enumerator = base.transform.GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                Transform current = (Transform) enumerator.Current;
                if (current.name == plaqueName)
                {
                    gameObject = current.gameObject;
                }
            }
        }
        finally
        {
            IDisposable disposable = enumerator as IDisposable;
            if (disposable == null)
            {
            }
            disposable.Dispose();
        }
        if ((gameObject != null) && (gameObject.activeSelf != on))
        {
            gameObject.SetActive(on);
            float y = 21f;
            IEnumerator enumerator2 = base.transform.GetEnumerator();
            try
            {
                while (enumerator2.MoveNext())
                {
                    Transform transform = (Transform) enumerator2.Current;
                    if (transform.gameObject.activeSelf)
                    {
                        transform.SetLocalPositionY(y);
                        y += 28f;
                    }
                }
            }
            finally
            {
                IDisposable disposable2 = enumerator2 as IDisposable;
                if (disposable2 == null)
                {
                }
                disposable2.Dispose();
            }
        }
    }
}

