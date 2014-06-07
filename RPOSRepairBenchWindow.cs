using System;
using uLink;
using UnityEngine;

public class RPOSRepairBenchWindow : RPOSLootWindow
{
    public UILabel[] _amountLabels;
    private RepairBench _bench;
    private IInventoryItem _benchItem;
    public UILabel conditionLabel;
    public UILabel needsLabel;
    public UIButton repairButton;

    public void ClearRepairItem()
    {
        this._benchItem = null;
        this.UpdateGUIAmounts();
    }

    private void RepairButtonClicked(GameObject go)
    {
        if (this._benchItem != null)
        {
            NetCull.RPC((MonoBehaviour) this._bench, "DoRepair", RPCMode.Server);
        }
    }

    public override void SetLootable(LootableObject lootable, bool doInit)
    {
        base.SetLootable(lootable, doInit);
        this._bench = lootable.GetComponent<RepairBench>();
    }

    public void SetRepairItem(IInventoryItem item)
    {
        if ((item == null) || !item.datablock.isRepairable)
        {
            this.ClearRepairItem();
        }
        else
        {
            this._benchItem = item;
            this.UpdateGUIAmounts();
        }
    }

    public void Update()
    {
        IInventoryItem item = null;
        if (this._bench != null)
        {
            this._bench.GetComponent<Inventory>().GetItem(0, out item);
        }
        this.SetRepairItem(item);
    }

    public void UpdateGUIAmounts()
    {
        if (this._benchItem == null)
        {
            foreach (UILabel label in this._amountLabels)
            {
                label.text = string.Empty;
                label.color = Color.white;
            }
            this.needsLabel.enabled = false;
            this.conditionLabel.enabled = false;
            this.repairButton.gameObject.SetActive(false);
        }
        else
        {
            Controllable controllable = PlayerClient.GetLocalPlayer().controllable;
            if (controllable != null)
            {
                Inventory component = controllable.GetComponent<Inventory>();
                int index = 0;
                if (!this._benchItem.IsDamaged())
                {
                    this.needsLabel.text = "Does not need repairs";
                    this.needsLabel.color = Color.green;
                    this.needsLabel.enabled = true;
                    string str3 = (this._benchItem.condition * 100f).ToString("0");
                    string str4 = (this._benchItem.maxcondition * 100f).ToString("0");
                    this.conditionLabel.text = "Condition : " + str3 + "/" + str4;
                    this.conditionLabel.color = Color.green;
                    this.conditionLabel.enabled = true;
                    this.repairButton.gameObject.SetActive(false);
                    foreach (UILabel label2 in this._amountLabels)
                    {
                        label2.text = string.Empty;
                        label2.color = Color.white;
                    }
                }
                else
                {
                    BlueprintDataBlock block;
                    if (BlueprintDataBlock.FindBlueprintForItem<BlueprintDataBlock>(this._benchItem.datablock, out block))
                    {
                        for (int i = 0; i < block.ingredients.Length; i++)
                        {
                            if (index >= this._amountLabels.Length)
                            {
                                break;
                            }
                            BlueprintDataBlock.IngredientEntry entry = block.ingredients[i];
                            int useCount = Mathf.CeilToInt(block.ingredients[i].amount * this._bench.GetResourceScalar());
                            if (useCount > 0)
                            {
                                bool flag = component.CanConsume(block.ingredients[i].Ingredient, useCount) > 0;
                                this._amountLabels[index].text = useCount + " " + block.ingredients[i].Ingredient.name;
                                this._amountLabels[index].color = !flag ? Color.red : Color.green;
                                index++;
                            }
                        }
                    }
                    this.needsLabel.color = Color.white;
                    this.needsLabel.enabled = true;
                    this.conditionLabel.enabled = true;
                    this.repairButton.gameObject.SetActive(true);
                    string str = (this._benchItem.condition * 100f).ToString("0");
                    string str2 = (this._benchItem.maxcondition * 100f).ToString("0");
                    this.conditionLabel.text = "Condition : " + str + "/" + str2;
                    this.conditionLabel.color = (this._benchItem.condition >= 0.6f) ? Color.green : Color.yellow;
                    if (this._benchItem.IsBroken())
                    {
                        this.conditionLabel.color = Color.red;
                    }
                }
            }
        }
    }

    protected override void WindowAwake()
    {
        base.WindowAwake();
        UIEventListener listener1 = UIEventListener.Get(this.repairButton.gameObject);
        listener1.onClick = (UIEventListener.VoidDelegate) Delegate.Combine(listener1.onClick, new UIEventListener.VoidDelegate(this.RepairButtonClicked));
        this.ClearRepairItem();
    }
}

