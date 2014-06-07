﻿using System;
using UnityEngine;

[RequireComponent(typeof(UICamera))]
public class RPOSFilter : MonoBehaviour
{
    private UICamera uicamera;

    private void Awake()
    {
        this.uicamera = base.GetComponent<UICamera>();
    }

    private void OnPreCull()
    {
        RPOS.BeforeRPOSRender_Internal(this.uicamera);
    }
}

