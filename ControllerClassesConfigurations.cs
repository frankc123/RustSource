using System;
using UnityEngine;

[Serializable]
public class ControllerClassesConfigurations
{
    [SerializeField]
    public string cl_unassigned;
    [SerializeField]
    public string localAI;
    [SerializeField]
    public string localPlayer;
    [SerializeField]
    public string remoteAI;
    [SerializeField]
    public string remotePlayer;
    [SerializeField]
    public string sv_unassigned;

    internal string GetClassName(bool player, bool local)
    {
        if (player)
        {
            if (local)
            {
                return (!string.IsNullOrEmpty(this.localPlayer) ? this.localPlayer : null);
            }
            return (!string.IsNullOrEmpty(this.remotePlayer) ? this.remotePlayer : null);
        }
        if (local)
        {
            return (!string.IsNullOrEmpty(this.localAI) ? this.localAI : null);
        }
        return (!string.IsNullOrEmpty(this.remoteAI) ? this.remoteAI : null);
    }

    internal string unassignedClassName
    {
        get
        {
            string str = this.cl_unassigned;
            string str2 = this.sv_unassigned;
            return (!string.IsNullOrEmpty(str) ? str : (!string.IsNullOrEmpty(str2) ? str2 : null));
        }
    }
}

