using Facepunch;
using System;
using UnityEngine;

public class RPOSInventoryCell : MonoBehaviour
{
    public UISprite _amountBackground;
    public UISlicedSprite _background;
    public UISprite _darkener;
    public Inventory _displayInventory;
    public UITexture _icon;
    private bool _locked;
    public IInventoryItem _myDisplayItem;
    public static Material _myMaterial;
    public byte _mySlot;
    public UILabel _numberLabel;
    public UILabel _stackLabel;
    public UILabel _usesLabel;
    private Color backupColor = Color.cyan;
    private bool dragging;
    private RPOSInventoryCell lastLanding;
    private UIAtlas.Sprite mod_empty;
    private UIAtlas.Sprite mod_full;
    public UISprite[] modSprites;
    private bool startedNoItem;

    public bool IsItemLocked()
    {
        return this._locked;
    }

    private void MakeEmpty()
    {
        this._myDisplayItem = null;
        this._icon.enabled = false;
        this._stackLabel.text = string.Empty;
        this._usesLabel.text = string.Empty;
        if (this._amountBackground != null)
        {
            this._amountBackground.enabled = false;
        }
        if (this.modSprites.Length > 0)
        {
            for (int i = 0; i < this.modSprites.Length; i++)
            {
                this.modSprites[i].enabled = false;
            }
        }
    }

    private void OnAltClick()
    {
        if (this.slotItem != null)
        {
            RPOS.GetRightClickMenu().SetItem(this.slotItem);
        }
    }

    private void OnAltLand(GameObject landing)
    {
        RPOSInventoryCell component = landing.GetComponent<RPOSInventoryCell>();
        if (component != null)
        {
            RPOS.ItemCellAltClicked(component);
        }
    }

    private void OnClick()
    {
    }

    private void OnDragState(bool start)
    {
        if (start)
        {
            if (!this.dragging && !this.startedNoItem)
            {
                UICamera.Cursor.DropNotification = DropNotificationFlags.DragLandOutside | DropNotificationFlags.RegularHover | DropNotificationFlags.AltLand | DropNotificationFlags.DragLand;
                this.lastLanding = null;
                this.dragging = true;
                RPOS.Item_CellDragBegin(this);
                UICamera.Cursor.CurrentButton.ClickNotification = UICamera.ClickNotification.BasedOnDelta;
            }
        }
        else if (this.dragging)
        {
            this.dragging = false;
            if (this.lastLanding != null)
            {
                this.dragging = false;
                RPOS.Item_CellDragEnd(this, this.lastLanding);
                UICamera.Cursor.Buttons.LeftValue.ClickNotification = UICamera.ClickNotification.None;
            }
            else
            {
                RPOS.Item_CellReset();
            }
        }
    }

    private void OnLand(GameObject landing)
    {
        this.lastLanding = landing.GetComponent<RPOSInventoryCell>();
    }

    private void OnLandOutside()
    {
        if (this._displayInventory.gameObject == RPOS.ObservedPlayer.gameObject)
        {
            RPOS.TossItem(this._mySlot);
        }
    }

    private void OnPress(bool start)
    {
        if (start)
        {
            this.startedNoItem = (this.slotItem == null) || this.IsItemLocked();
            if (this.startedNoItem)
            {
                UICamera.Cursor.CurrentButton.ClickNotification = UICamera.ClickNotification.None;
                UICamera.Cursor.DropNotification = 0;
            }
        }
    }

    private void OnTooltip(bool show)
    {
        IInventoryItem item = (!show || (this._myDisplayItem == null)) ? null : this._myDisplayItem;
        ItemDataBlock itemdb = (!show || (this._myDisplayItem == null)) ? null : this._myDisplayItem.datablock;
        ItemToolTip.SetToolTip(itemdb, item);
    }

    private void SetItem(IInventoryItem item)
    {
        if (item == null)
        {
            this.MakeEmpty();
        }
        else
        {
            IHeldItem item2;
            this._myDisplayItem = item;
            if (item.datablock.IsSplittable())
            {
                this._stackLabel.color = Color.white;
                if (item.uses > 1)
                {
                    this._stackLabel.text = "x" + item.uses.ToString();
                }
                else
                {
                    this._stackLabel.text = string.Empty;
                }
            }
            else
            {
                this._stackLabel.color = Color.yellow;
                this._stackLabel.text = (item.datablock._maxUses <= item.datablock.GetMinUsesForDisplay()) ? string.Empty : item.uses.ToString();
            }
            if (this._amountBackground != null)
            {
                if (this._stackLabel.text == string.Empty)
                {
                    this._amountBackground.enabled = false;
                }
                else
                {
                    Vector2 vector = this._stackLabel.font.CalculatePrintedSize(this._stackLabel.text, true, UIFont.SymbolStyle.None);
                    this._amountBackground.enabled = true;
                    this._amountBackground.transform.localScale = new Vector3((vector.x * this._stackLabel.transform.localScale.x) + 12f, 16f, 1f);
                }
            }
            if (ItemDataBlock.LoadIconOrUnknown<Texture>(item.datablock.icon, ref item.datablock.iconTex))
            {
                Material material = TextureMaterial.GetMaterial(_myMaterial, item.datablock.iconTex);
                this._icon.material = (UIMaterial) material;
                this._icon.enabled = true;
            }
            int num = ((item2 = item as IHeldItem) != null) ? item2.totalModSlots : 0;
            int num2 = (num != 0) ? item2.usedModSlots : 0;
            for (int i = 0; i < this.modSprites.Length; i++)
            {
                if (i < num)
                {
                    this.modSprites[i].enabled = true;
                    this.modSprites[i].sprite = (i >= num2) ? this.mod_empty : this.mod_full;
                    this.modSprites[i].spriteName = this.modSprites[i].sprite.name;
                }
                else
                {
                    this.modSprites[i].enabled = false;
                }
            }
            if (item.IsBroken())
            {
                this._icon.color = Color.red;
            }
            else if ((item.condition / item.maxcondition) <= 0.4f)
            {
                this._icon.color = Color.yellow;
            }
            else
            {
                this._icon.color = Color.white;
            }
        }
    }

    public void SetItemLocked(bool locked)
    {
        this._locked = locked;
        if (this._locked)
        {
            this._icon.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }
        else
        {
            this._icon.color = Color.white;
        }
    }

    private void Start()
    {
        if (_myMaterial == null)
        {
            Bundling.Load<Material>("content/item/mat/ItemIconShader", out _myMaterial);
        }
        this._icon.enabled = false;
        if (this.modSprites.Length > 0)
        {
            this.mod_empty = this.modSprites[0].atlas.GetSprite("slot_empty");
            this.mod_full = this.modSprites[0].atlas.GetSprite("slot_full");
        }
    }

    private void Update()
    {
        if (this._displayInventory != null)
        {
            if (RPOS.Item_IsClickedCell(this))
            {
                this.MakeEmpty();
            }
            else
            {
                IInventoryItem item;
                this._displayInventory.GetItem(this._mySlot, out item);
                if (this._displayInventory.MarkSlotClean(this._mySlot) || !object.ReferenceEquals(this._myDisplayItem, item))
                {
                    this.SetItem(item);
                }
            }
            if (!RPOS.IsOpen && (this._darkener != null))
            {
                if (this.backupColor == Color.cyan)
                {
                    this.backupColor = this._darkener.color;
                }
                if ((this._myDisplayItem != null) && (this._displayInventory._activeItem == this._myDisplayItem))
                {
                    this._darkener.color = Color.grey;
                }
                else
                {
                    this._darkener.color = this.backupColor;
                }
            }
        }
    }

    public IInventoryItem slotItem
    {
        get
        {
            IInventoryItem item;
            if ((this._displayInventory != null) && this._displayInventory.GetItem(this._mySlot, out item))
            {
                return item;
            }
            return null;
        }
    }
}

