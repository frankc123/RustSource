using System;
using System.Collections;
using UnityEngine;

public class RPOSCraftWindow : RPOSWindowScrollable
{
    [NonSerialized]
    private string _lastTimeStringString;
    [NonSerialized]
    private float _lastTimeStringValue = float.PositiveInfinity;
    public UILabel amountInput;
    public UISprite amountInputBackground;
    public RPOS_Craft_BlueprintList bpLister;
    public UIButton craftButton;
    public UISlider craftProgressBar;
    public AudioClip craftSound;
    public int desiredAmount = 1;
    public GameObject ingredientAnchor;
    public GameObject ingredientPlaquePrefab;
    public UIButton minusButton;
    public UIButton plusButton;
    public UILabel progressLabel;
    public UILabel requirementLabel;
    public BlueprintDataBlock selectedItem;
    private bool wasCrafting;

    public bool AtWorkbench()
    {
        return RPOS.ObservedPlayer.GetComponent<CraftingInventory>().AtWorkBench();
    }

    public void Awake()
    {
        this.ShowCraftingOptions(false);
        UIEventListener listener1 = UIEventListener.Get(this.craftButton.gameObject);
        listener1.onClick = (UIEventListener.VoidDelegate) Delegate.Combine(listener1.onClick, new UIEventListener.VoidDelegate(this.CraftButtonClicked));
        UIEventListener listener4 = UIEventListener.Get(this.plusButton.gameObject);
        listener4.onClick = (UIEventListener.VoidDelegate) Delegate.Combine(listener4.onClick, new UIEventListener.VoidDelegate(this.PlusButtonClicked));
        UIEventListener listener5 = UIEventListener.Get(this.minusButton.gameObject);
        listener5.onClick = (UIEventListener.VoidDelegate) Delegate.Combine(listener5.onClick, new UIEventListener.VoidDelegate(this.MinusButtonClicked));
        this.amountInput.text = "1";
    }

    public void CraftButtonClicked(GameObject go)
    {
        if (this.selectedItem != null)
        {
            Debug.Log("Crafting clicked");
            CraftingInventory component = RPOS.ObservedPlayer.GetComponent<CraftingInventory>();
            if (component == null)
            {
                Debug.Log("No local player inventory.. weird");
            }
            else if (component.isCrafting)
            {
                component.CancelCrafting();
            }
            else if (component.ValidateCraftRequirements(this.selectedItem))
            {
                component.StartCrafting(this.selectedItem, this.RequestedAmount());
            }
        }
    }

    public void ItemClicked(GameObject go)
    {
        if (!RPOS.ObservedPlayer.GetComponent<CraftingInventory>().isCrafting)
        {
            RPOSCraftItemEntry component = go.GetComponent<RPOSCraftItemEntry>();
            if (component != null)
            {
                BlueprintDataBlock blueprint = component.blueprint;
                if (blueprint == null)
                {
                    Debug.Log("no bp by that name");
                }
                else if (blueprint != this.selectedItem)
                {
                    this.SetSelectedItem(component.blueprint);
                    this.UpdateIngredients();
                }
            }
        }
    }

    public void ItemHovered(GameObject go, bool what)
    {
    }

    public void LocalInventoryModified()
    {
        this.bpLister.UpdateItems();
        this.UpdateIngredients();
    }

    public void MinusButtonClicked(GameObject go)
    {
        this.PlusMinusClick(-amountModifier);
    }

    protected override void OnWindowHide()
    {
        base.OnWindowHide();
    }

    protected override void OnWindowShow()
    {
        this.bpLister.UpdateItems();
        this.SetRequestedAmount(1);
        base.OnWindowShow();
    }

    public void PlusButtonClicked(GameObject go)
    {
        this.PlusMinusClick(amountModifier);
    }

    public void PlusMinusClick(int amount)
    {
        if (amount != 0)
        {
            CraftingInventory component = RPOS.ObservedPlayer.GetComponent<CraftingInventory>();
            if ((component != null) && !component.isCrafting)
            {
                this.SetRequestedAmount(this.desiredAmount + amount);
                this.UpdateIngredients();
            }
        }
    }

    public int RequestedAmount()
    {
        return this.desiredAmount;
    }

    public void SetRequestedAmount(int amount)
    {
        if (this.selectedItem == null)
        {
            this.desiredAmount = amount;
        }
        else
        {
            int num = this.selectedItem.MaxAmount(RPOS.ObservedPlayer.GetComponent<Inventory>());
            this.desiredAmount = Mathf.Clamp(amount, 1, (num > 0) ? num : 1);
        }
        this.amountInput.text = this.desiredAmount.ToString();
    }

    public void SetSelectedItem(BlueprintDataBlock newSel)
    {
        if (this.selectedItem != null)
        {
        }
        this.selectedItem = newSel;
        this.SetRequestedAmount(1);
        if (this.selectedItem != null)
        {
        }
        this.ShowCraftingOptions(this.selectedItem != null);
        this.UpdateWorkbenchRequirements();
    }

    public void ShowCraftingOptions(bool show)
    {
        this.plusButton.gameObject.SetActive(show);
        this.minusButton.gameObject.SetActive(show);
        this.amountInput.gameObject.SetActive(show);
        this.amountInputBackground.gameObject.SetActive(show);
        this.craftProgressBar.gameObject.SetActive(show);
        this.craftButton.gameObject.SetActive(show);
        this.requirementLabel.gameObject.SetActive(show);
    }

    public void Update()
    {
        if (RPOS.ObservedPlayer != null)
        {
            CraftingInventory component = RPOS.ObservedPlayer.GetComponent<CraftingInventory>();
            if (component != null)
            {
                bool isCrafting = component.isCrafting;
                if (isCrafting)
                {
                    component.CraftThink();
                }
                if (!isCrafting && this.wasCrafting)
                {
                    this.UpdateIngredients();
                }
                else if (!this.wasCrafting && isCrafting)
                {
                    this.craftSound.Play();
                }
                if (this.craftButton.gameObject.activeSelf)
                {
                    this.craftButton.GetComponentInChildren<UILabel>().text = !component.isCrafting ? "Craft" : "Cancel";
                }
                if (((this.craftProgressBar != null) && (this.craftProgressBar.gameObject != null)) && this.craftProgressBar.gameObject.activeSelf)
                {
                    float? craftingCompletePercent = component.craftingCompletePercent;
                    this.craftProgressBar.sliderValue = !craftingCompletePercent.HasValue ? 0f : craftingCompletePercent.Value;
                    float? craftingSecondsRemaining = component.craftingSecondsRemaining;
                    float num = !craftingSecondsRemaining.HasValue ? 0f : craftingSecondsRemaining.Value;
                    if (num != this._lastTimeStringValue)
                    {
                        this._lastTimeStringString = num.ToString("0.0");
                        this._lastTimeStringValue = num;
                    }
                    this.progressLabel.text = this._lastTimeStringString;
                    Color white = Color.white;
                    float craftingSpeedPerSec = component.craftingSpeedPerSec;
                    if (craftingSpeedPerSec > 1f)
                    {
                        white = Color.green;
                    }
                    else if (craftingSpeedPerSec < 1f)
                    {
                        white = Color.yellow;
                    }
                    else if (craftingSpeedPerSec < 0.5f)
                    {
                        white = Color.red;
                    }
                    this.progressLabel.color = white;
                }
                if (this.selectedItem != null)
                {
                    this.UpdateWorkbenchRequirements();
                }
                if (this.progressLabel != null)
                {
                    this.progressLabel.enabled = isCrafting;
                }
                this.wasCrafting = component.isCrafting;
            }
        }
    }

    public void UpdateIngredients()
    {
        if (this.selectedItem != null)
        {
            IEnumerator enumerator = this.ingredientAnchor.transform.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    object current = enumerator.Current;
                    Object.Destroy((current as Transform).gameObject);
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
            int num = this.RequestedAmount();
            int num2 = 0;
            foreach (BlueprintDataBlock.IngredientEntry entry in this.selectedItem.ingredients)
            {
                int totalNum = 0;
                PlayerClient.GetLocalPlayer().controllable.GetComponent<CraftingInventory>().FindItem(entry.Ingredient, out totalNum);
                int needAmount = entry.amount * num;
                GameObject obj3 = NGUITools.AddChild(this.ingredientAnchor, this.ingredientPlaquePrefab);
                obj3.GetComponent<RPOS_Craft_IngredientPlaque>().Bind(entry, needAmount, totalNum);
                obj3.transform.SetLocalPositionY((float) num2);
                num2 -= 12;
            }
        }
    }

    public void UpdateWorkbenchRequirements()
    {
        if ((this.selectedItem != null) && this.selectedItem.RequireWorkbench)
        {
            this.requirementLabel.color = !this.AtWorkbench() ? Color.red : Color.green;
            this.requirementLabel.text = "REQUIRES WORKBENCH";
        }
        else
        {
            this.requirementLabel.text = string.Empty;
        }
    }

    public char ValidateAmountInput(string text, char ch)
    {
        Debug.Log("validating input");
        if (((text.Length != 0) || (ch != '0')) && ((ch >= '0') && (ch <= '9')))
        {
            return ch;
        }
        return '\0';
    }

    private static int amountModifier
    {
        get
        {
            try
            {
                Event current = Event.current;
                if (current.control)
                {
                    return 0x7fff;
                }
                if (current.shift)
                {
                    return 10;
                }
            }
            catch
            {
            }
            return 1;
        }
    }
}

