using System;
using uLink;
using UnityEngine;

public class RPOSWorkbenchWindow : RPOSWindow
{
    private WorkBench _myWorkBench;
    public UIButton actionButton;
    public AudioClip finishedSound;
    public UILabel percentLabel;
    public UISlider progressBar;
    public AudioClip startSound;
    public UIButton takeAllButton;

    private void ActionButtonClicked(GameObject go)
    {
        Debug.Log("Action button clicked");
        this._myWorkBench.networkView.RPC("DoAction", RPCMode.Server, new object[0]);
        Debug.Log("Action post");
    }

    public void BenchUpdate()
    {
        if (this._myWorkBench == null)
        {
            Debug.Log("NO BENCH!?!?!");
        }
        if (this._myWorkBench.IsWorking())
        {
            this.startSound.Play();
            this.actionButton.GetComponentInChildren<UILabel>().text = "Cancel";
            this.takeAllButton.enabled = false;
            this.SetCellsLocked(true);
        }
        else
        {
            this.actionButton.GetComponentInChildren<UILabel>().text = "Start";
            this.takeAllButton.enabled = true;
            this.SetCellsLocked(false);
            if (this._myWorkBench._inventory.IsSlotOccupied(12))
            {
                this.finishedSound.Play();
            }
        }
    }

    public void Initialize()
    {
        base.GetComponentInChildren<RPOSInvCellManager>().SetInventory(this._myWorkBench.GetComponent<Inventory>(), false);
    }

    protected override void OnExternalClose()
    {
        this.WorkbenchClosed();
    }

    protected override void OnRPOSClosed()
    {
        base.OnRPOSClosed();
        this.WorkbenchClosed();
    }

    protected override void OnWindowClosed()
    {
        base.OnWindowClosed();
        this.WorkbenchClosed();
    }

    private void SetCellsLocked(bool isLocked)
    {
        RPOSInvCellManager componentInChildren = base.GetComponentInChildren<RPOSInvCellManager>();
        for (int i = 0; i < componentInChildren._inventoryCells.Length; i++)
        {
            RPOSInventoryCell cell = componentInChildren._inventoryCells[i];
            if (cell != null)
            {
                cell.SetItemLocked(isLocked);
            }
        }
    }

    public void SetWorkbench(WorkBench workbenchObj)
    {
        this._myWorkBench = workbenchObj;
        this.Initialize();
    }

    private void TakeAllButtonClicked(GameObject go)
    {
        this._myWorkBench.networkView.RPC("TakeAll", RPCMode.Server, new object[0]);
    }

    public void Update()
    {
        if ((this._myWorkBench != null) && this._myWorkBench.IsWorking())
        {
            this.percentLabel.enabled = true;
            this.progressBar.sliderValue = this._myWorkBench.GetFractionComplete();
            this.percentLabel.text = ((Mathf.Clamp01(this._myWorkBench.GetFractionComplete()) * 100f)).ToString("N0") + "%";
        }
        else
        {
            this.percentLabel.enabled = false;
            this.progressBar.sliderValue = 0f;
        }
    }

    protected override void WindowAwake()
    {
        base.WindowAwake();
        UIEventListener listener1 = UIEventListener.Get(this.actionButton.gameObject);
        listener1.onClick = (UIEventListener.VoidDelegate) Delegate.Combine(listener1.onClick, new UIEventListener.VoidDelegate(this.ActionButtonClicked));
        UIEventListener listener3 = UIEventListener.Get(this.takeAllButton.gameObject);
        listener3.onClick = (UIEventListener.VoidDelegate) Delegate.Combine(listener3.onClick, new UIEventListener.VoidDelegate(this.TakeAllButtonClicked));
    }

    public virtual void WorkbenchClosed()
    {
        if (this._myWorkBench != null)
        {
            this._myWorkBench.ClientClosedWorkbenchWindow();
        }
        Object.Destroy(base.gameObject);
    }
}

