using System;
using UnityEngine;

public class RPOSInfoWindow : RPOSWindowScrollable
{
    public GameObject addParent;
    public GameObject basicLabelPrefab;
    public GameObject itemDescriptionPrefab;
    public GameObject itemTitlePrefab;
    public GameObject progressStatPrefab;
    public GameObject sectionTitlePrefab;

    public RPOSInfoWindow()
    {
        base.neverAutoShow = true;
    }

    public GameObject AddBasicLabel(string text, float aboveSpace)
    {
        float contentHeight = this.GetContentHeight();
        GameObject obj2 = NGUITools.AddChild(this.addParent, this.basicLabelPrefab);
        obj2.GetComponentInChildren<UILabel>().text = text;
        obj2.transform.SetLocalPositionY(-(contentHeight + aboveSpace));
        return obj2;
    }

    public GameObject AddItemDescription(ItemDataBlock item, float aboveSpace)
    {
        float contentHeight = this.GetContentHeight();
        GameObject obj2 = NGUITools.AddChild(this.addParent, this.itemDescriptionPrefab);
        obj2.transform.FindChild("DescText").GetComponent<UILabel>().text = item.GetItemDescription();
        obj2.transform.SetLocalPositionY(-(contentHeight + aboveSpace));
        return null;
    }

    public GameObject AddItemTitle(ItemDataBlock item)
    {
        return this.AddItemTitle(item, 0f);
    }

    public GameObject AddItemTitle(ItemDataBlock item, float aboveSpace)
    {
        float contentHeight = this.GetContentHeight();
        GameObject obj2 = NGUITools.AddChild(this.addParent, this.itemTitlePrefab);
        obj2.GetComponentInChildren<UILabel>().text = item.name;
        UITexture componentInChildren = obj2.GetComponentInChildren<UITexture>();
        componentInChildren.material = componentInChildren.material.Clone();
        componentInChildren.material.Set("_MainTex", item.GetIconTexture());
        obj2.transform.SetLocalPositionY(-(contentHeight + aboveSpace));
        return obj2;
    }

    public GameObject AddProgressStat(string text, float currentAmount, float maxAmount, float aboveSpace)
    {
        float contentHeight = this.GetContentHeight();
        GameObject obj2 = NGUITools.AddChild(this.addParent, this.progressStatPrefab);
        UISlider componentInChildren = obj2.GetComponentInChildren<UISlider>();
        UILabel component = FindChildHelper.FindChildByName("ProgressStatTitle", obj2.gameObject).GetComponent<UILabel>();
        UILabel label2 = FindChildHelper.FindChildByName("ProgressAmountLabel", obj2.gameObject).GetComponent<UILabel>();
        component.text = text;
        label2.text = (currentAmount >= 1f) ? currentAmount.ToString("N0") : currentAmount.ToString("N2");
        componentInChildren.sliderValue = currentAmount / maxAmount;
        obj2.transform.SetLocalPositionY(-(contentHeight + aboveSpace));
        return obj2;
    }

    public GameObject AddSectionTitle(string text, float aboveSpace)
    {
        float contentHeight = this.GetContentHeight();
        GameObject obj2 = NGUITools.AddChild(this.addParent, this.sectionTitlePrefab);
        obj2.GetComponentInChildren<UILabel>().text = text;
        obj2.transform.SetLocalPositionY(-(contentHeight + aboveSpace));
        return obj2;
    }

    public void FinishPopulating()
    {
        base.ResetScrolling();
        base.showWithRPOS = base.autoShowWithRPOS;
        base.showWithoutRPOS = base.autoShowWithoutRPOS;
    }

    public float GetContentHeight()
    {
        return NGUIMath.CalculateRelativeWidgetBounds(this.addParent.transform, this.addParent.transform).size.y;
    }

    protected override void OnWindowHide()
    {
        base.OnWindowHide();
        this.SetVisible(false);
    }

    protected override void OnWindowShow()
    {
        base.OnWindowShow();
        this.SetVisible(true);
    }

    private void SetVisible(bool enable)
    {
        Debug.Log("Info RPOS opened");
        base.mainPanel.enabled = enable;
        foreach (UIPanel panel in base.GetComponentsInChildren<UIPanel>())
        {
            panel.enabled = enable;
        }
    }
}

