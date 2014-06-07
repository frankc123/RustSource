using System;
using UnityEngine;

[AddComponentMenu("")]
public sealed class SocketProxy : Socket.Proxy
{
    protected override void UninitializeProxy()
    {
        base.transform.DetachChildren();
    }
}

