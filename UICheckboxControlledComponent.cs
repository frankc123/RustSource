using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Checkbox Controlled Component")]
public class UICheckboxControlledComponent : MonoBehaviour
{
    public bool inverse;
    public MonoBehaviour target;

    private void OnActivate(bool isActive)
    {
        if (base.enabled && (this.target != null))
        {
            this.target.enabled = !this.inverse ? isActive : !isActive;
        }
    }
}

