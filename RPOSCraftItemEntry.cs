using System;
using UnityEngine;

public class RPOSCraftItemEntry : MonoBehaviour
{
    public ItemDataBlock actualItemDataBlock;
    public BlueprintDataBlock blueprint;
    public RPOSCraftWindow craftWindow;

    public void OnTooltip(bool show)
    {
        ItemToolTip.SetToolTip((!show || (this.actualItemDataBlock == null)) ? null : this.actualItemDataBlock, null);
    }

    public void SetSelected(bool selected)
    {
        Color color = !selected ? Color.white : Color.yellow;
        base.GetComponentInChildren<UILabel>().color = color;
    }

    public void Update()
    {
        if (RPOS.IsOpen)
        {
            if ((this.blueprint != null) && (this.blueprint == this.craftWindow.selectedItem))
            {
                this.SetSelected(true);
            }
            else
            {
                this.SetSelected(false);
            }
        }
    }
}

