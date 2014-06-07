using System;
using uLink;
using UnityEngine;

public class PlayerProxyTest : MonoBehaviour
{
    [PrefetchComponent]
    public ArmorModelRenderer armorRenderer;
    [PrefetchChildComponent(NameMask="Soldier")]
    public GameObject body;
    private bool hasFaked;
    private bool[] initialDisableListValues;
    private bool isFaking;
    private bool isMine;
    [PrefetchChildComponent(NameMask="HB Hit")]
    public GameObject proxyCollider;
    public MonoBehaviour[] proxyDisableList;

    private void MineInit()
    {
        if (this.body != null)
        {
            this.body.SetActive(false);
        }
        if (this.proxyCollider != null)
        {
            this.proxyCollider.SetActive(false);
        }
        if (this.armorRenderer != null)
        {
            this.armorRenderer.enabled = false;
        }
    }

    private void ProxyInit()
    {
        for (int i = 0; i < this.proxyDisableList.Length; i++)
        {
            if (this.proxyDisableList[i] != null)
            {
                this.proxyDisableList[i].enabled = false;
            }
        }
        if (this.proxyCollider != null)
        {
            this.proxyCollider.SetActive(true);
        }
    }

    private void uLink_OnNetworkInstantiate(NetworkMessageInfo info)
    {
        if (info.networkView.isMine)
        {
            this.isMine = true;
            this.MineInit();
        }
    }

    public bool treatAsProxy
    {
        get
        {
            return (!this.isMine || this.isFaking);
        }
        set
        {
            if (this.isMine && (this.isFaking != value))
            {
                if (!this.hasFaked)
                {
                    this.initialDisableListValues = new bool[this.proxyDisableList.Length];
                    this.hasFaked = true;
                }
                this.isFaking = value;
                if (value)
                {
                    for (int i = 0; i < this.initialDisableListValues.Length; i++)
                    {
                        this.initialDisableListValues[i] = (this.proxyDisableList[i] != null) && this.proxyDisableList[i].enabled;
                    }
                    if (this.body != null)
                    {
                        this.body.SetActive(true);
                    }
                    if (this.armorRenderer != null)
                    {
                        this.armorRenderer.enabled = true;
                    }
                    this.ProxyInit();
                }
                else
                {
                    for (int j = 0; j < this.initialDisableListValues.Length; j++)
                    {
                        if (this.initialDisableListValues[j] && (this.proxyDisableList[j] != null))
                        {
                            this.proxyDisableList[j].enabled = true;
                        }
                    }
                    this.MineInit();
                }
            }
        }
    }
}

