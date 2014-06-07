using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPOS_Craft_BlueprintList : MonoBehaviour
{
    public GameObject CategoryHeaderPrefab;
    public RPOSCraftWindow craftWindow;
    public GameObject ItemPlaquePrefab;
    private int lastNumBoundBPs;

    public void AddItemCategoryHeader(ItemDataBlock.ItemCategory category)
    {
    }

    public int AddItemsOfCategory(ItemDataBlock.ItemCategory category, List<BlueprintDataBlock> checkList, int yPos)
    {
        if (this.AnyOfCategoryInList(category, checkList))
        {
            GameObject obj2 = NGUITools.AddChild(base.gameObject, this.CategoryHeaderPrefab);
            obj2.transform.localPosition = new Vector3(0f, (float) yPos, -1f);
            obj2.GetComponentInChildren<UILabel>().text = category.ToString();
            yPos -= 0x10;
            foreach (BlueprintDataBlock block in checkList)
            {
                if (block.resultItem.category == category)
                {
                    GameObject go = NGUITools.AddChild(base.gameObject, this.ItemPlaquePrefab);
                    go.GetComponentInChildren<UILabel>().text = block.resultItem.name;
                    go.transform.localPosition = new Vector3(10f, (float) yPos, -1f);
                    UIEventListener listener1 = UIEventListener.Get(go);
                    listener1.onClick = (UIEventListener.VoidDelegate) Delegate.Combine(listener1.onClick, new UIEventListener.VoidDelegate(this.craftWindow.ItemClicked));
                    go.GetComponent<RPOSCraftItemEntry>().actualItemDataBlock = block.resultItem;
                    go.GetComponent<RPOSCraftItemEntry>().blueprint = block;
                    go.GetComponent<RPOSCraftItemEntry>().craftWindow = this.craftWindow;
                    go.GetComponent<RPOSCraftItemEntry>().SetSelected(false);
                    yPos -= 0x10;
                }
            }
        }
        return yPos;
    }

    public bool AnyOfCategoryInList(ItemDataBlock.ItemCategory category, List<BlueprintDataBlock> checkList)
    {
        foreach (BlueprintDataBlock block in checkList)
        {
            if (block == null)
            {
                Debug.Log("WTFFFF");
                return false;
            }
            if (block.resultItem.category == category)
            {
                return true;
            }
        }
        return false;
    }

    private void Awake()
    {
    }

    public RPOSCraftItemEntry GetEntryByBP(BlueprintDataBlock bp)
    {
        IEnumerator enumerator = base.transform.GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                object current = enumerator.Current;
                RPOSCraftItemEntry component = (current as Transform).GetComponent<RPOSCraftItemEntry>();
                if ((component != null) && (component.blueprint == bp))
                {
                    return component;
                }
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
        return null;
    }

    public void UpdateItems()
    {
        List<BlueprintDataBlock> boundBPs = RPOS.ObservedPlayer.GetComponent<PlayerInventory>().GetBoundBPs();
        int count = boundBPs.Count;
        if (boundBPs == null)
        {
            Debug.Log("BOUND BP LIST EMPTY!!!!!");
        }
        else if (this.lastNumBoundBPs != count)
        {
            this.lastNumBoundBPs = count;
            IEnumerator enumerator = base.transform.GetEnumerator();
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
            int yPos = this.AddItemsOfCategory(ItemDataBlock.ItemCategory.Survival, boundBPs, 0);
            yPos = this.AddItemsOfCategory(ItemDataBlock.ItemCategory.Resource, boundBPs, yPos);
            yPos = this.AddItemsOfCategory(ItemDataBlock.ItemCategory.Medical, boundBPs, yPos);
            yPos = this.AddItemsOfCategory(ItemDataBlock.ItemCategory.Ammo, boundBPs, yPos);
            yPos = this.AddItemsOfCategory(ItemDataBlock.ItemCategory.Weapons, boundBPs, yPos);
            yPos = this.AddItemsOfCategory(ItemDataBlock.ItemCategory.Armor, boundBPs, yPos);
            yPos = this.AddItemsOfCategory(ItemDataBlock.ItemCategory.Tools, boundBPs, yPos);
            yPos = this.AddItemsOfCategory(ItemDataBlock.ItemCategory.Mods, boundBPs, yPos);
            yPos = this.AddItemsOfCategory(ItemDataBlock.ItemCategory.Parts, boundBPs, yPos);
            yPos = this.AddItemsOfCategory(ItemDataBlock.ItemCategory.Food, boundBPs, yPos);
            yPos = this.AddItemsOfCategory(ItemDataBlock.ItemCategory.Blueprint, boundBPs, yPos);
            yPos = this.AddItemsOfCategory(ItemDataBlock.ItemCategory.Misc, boundBPs, yPos);
            base.GetComponent<UIDraggablePanel>().calculateNextChange = true;
        }
    }
}

