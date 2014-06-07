using System;
using UnityEngine;

public class DisableOnConnectedState : MonoBehaviour
{
    protected static bool connectedStatus;
    public bool disableWhenConnected;

    protected void DoOnConnected()
    {
        base.gameObject.SetActive(!this.disableWhenConnected);
        dfControl component = base.gameObject.GetComponent<dfControl>();
        if (component != null)
        {
            if (this.disableWhenConnected)
            {
                component.Hide();
            }
            else
            {
                component.Show();
            }
        }
    }

    protected void DoOnDisconnected()
    {
        base.gameObject.SetActive(this.disableWhenConnected);
        dfControl component = base.gameObject.GetComponent<dfControl>();
        if (component != null)
        {
            if (!this.disableWhenConnected)
            {
                component.Hide();
            }
            else
            {
                component.Show();
            }
        }
    }

    public static void OnConnected()
    {
        connectedStatus = true;
        foreach (DisableOnConnectedState state in Resources.FindObjectsOfTypeAll(typeof(DisableOnConnectedState)))
        {
            if (state.gameObject == null)
            {
                return;
            }
            state.DoOnConnected();
        }
    }

    public static void OnDisconnected()
    {
        connectedStatus = false;
        foreach (DisableOnConnectedState state in Resources.FindObjectsOfTypeAll(typeof(DisableOnConnectedState)))
        {
            if (state.gameObject == null)
            {
                return;
            }
            state.DoOnDisconnected();
        }
    }

    private void Start()
    {
        if (connectedStatus)
        {
            this.DoOnConnected();
        }
        else
        {
            this.DoOnDisconnected();
        }
    }
}

