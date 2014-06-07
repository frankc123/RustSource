using System;
using UnityEngine;

public class RPOS_Craft_IngredientPlaque : MonoBehaviour
{
    [PrefetchChildComponent(NameMask="checkmark")]
    public UISprite checkIcon;
    [PrefetchChildComponent(NameMask="HaveLabel")]
    public UILabel have;
    [PrefetchChildComponent(NameMask="ItemName")]
    public UILabel itemName;
    [PrefetchChildComponent(NameMask="NeedLabel")]
    public UILabel need;
    [PrefetchChildComponent(NameMask="xmark")]
    public UISprite xIcon;

    public void Bind(BlueprintDataBlock.IngredientEntry ingredient, int needAmount, int haveAmount)
    {
        Color green;
        ItemDataBlock block = ingredient.Ingredient;
        if (needAmount <= haveAmount)
        {
            this.checkIcon.enabled = true;
            this.xIcon.enabled = false;
            green = Color.green;
        }
        else
        {
            this.checkIcon.enabled = false;
            this.xIcon.enabled = true;
            green = Color.red;
        }
        Color color2 = green;
        this.have.color = color2;
        this.need.color = color2;
        this.itemName.text = block.name;
        this.need.text = needAmount.ToString("N0");
        this.have.text = haveAmount.ToString("N0");
    }
}

