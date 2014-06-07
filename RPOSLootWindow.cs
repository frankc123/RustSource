using System;
using UnityEngine;

public class RPOSLootWindow : RPOSWindowScrollable
{
    [NonSerialized]
    public bool initalized;
    [NonSerialized]
    public LootableObject myLootable;
    public UIButton TakeAllButton;

    public void Initialize()
    {
        base.GetComponentInChildren<RPOSInvCellManager>().SetInventory(this.myLootable.GetComponent<Inventory>(), true);
        base.ResetScrolling();
    }

    public virtual void LootClosed()
    {
        if (this.myLootable != null)
        {
            this.myLootable.ClientClosedLootWindow();
        }
        Object.Destroy(base.gameObject);
    }

    protected override void OnExternalClose()
    {
        this.LootClosed();
    }

    protected override void OnRPOSClosed()
    {
        base.OnRPOSClosed();
        this.LootClosed();
    }

    protected override void OnWindowHide()
    {
        try
        {
            base.OnWindowHide();
        }
        finally
        {
            this.LootClosed();
        }
    }

    public virtual void SetLootable(LootableObject lootable, bool doInit)
    {
        this.myLootable = lootable;
        this.Initialize();
    }

    public void TakeAllButtonClicked(GameObject go)
    {
        RPOS.ChangeRPOSMode(false);
        Object.Destroy(base.gameObject);
    }

    protected override void WindowAwake()
    {
        base.autoResetScrolling = false;
        base.WindowAwake();
        if (!this.initalized && (this.myLootable != null))
        {
            this.Initialize();
        }
        if (this.TakeAllButton != null)
        {
            UIEventListener listener1 = UIEventListener.Get(this.TakeAllButton.gameObject);
            listener1.onClick = (UIEventListener.VoidDelegate) Delegate.Combine(listener1.onClick, new UIEventListener.VoidDelegate(this.TakeAllButtonClicked));
        }
    }
}

