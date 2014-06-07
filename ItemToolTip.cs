using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public class ItemToolTip : MonoBehaviour
{
    public UISlicedSprite _background;
    public static ItemToolTip _globalToolTip;
    public GameObject addParent;
    public GameObject basicLabelPrefab;
    public GameObject itemDescriptionPrefab;
    public GameObject itemTitlePrefab;
    private Plane planeTest;
    public GameObject progressStatPrefab;
    public GameObject sectionTitlePrefab;
    public Camera uiCamera;

    public GameObject AddBasicLabel(string text, float aboveSpace)
    {
        return this.AddBasicLabel(text, aboveSpace, Color.white);
    }

    public GameObject AddBasicLabel(string text, float aboveSpace, Color col)
    {
        float contentHeight = this.GetContentHeight();
        GameObject obj2 = NGUITools.AddChild(this.addParent, this.basicLabelPrefab);
        UILabel componentInChildren = obj2.GetComponentInChildren<UILabel>();
        componentInChildren.text = text;
        componentInChildren.color = col;
        obj2.transform.SetLocalPositionY(-(contentHeight + aboveSpace));
        return obj2;
    }

    public GameObject AddConditionInfo(IInventoryItem item)
    {
        if ((item == null) || !item.datablock.DoesLoseCondition())
        {
            return null;
        }
        Color green = Color.green;
        if (item.condition <= 0.6f)
        {
            green = Color.yellow;
        }
        else if (item.IsBroken())
        {
            green = Color.red;
        }
        float num = 100f * item.condition;
        float num2 = 100f * item.maxcondition;
        return this.AddBasicLabel("Condition : " + num.ToString("0") + "/" + num2.ToString("0"), 15f, green);
    }

    public GameObject AddItemDescription(ItemDataBlock item, float aboveSpace)
    {
        float contentHeight = this.GetContentHeight();
        GameObject obj2 = NGUITools.AddChild(this.addParent, this.itemDescriptionPrefab);
        obj2.transform.FindChild("DescText").GetComponent<UILabel>().text = item.GetItemDescription();
        obj2.transform.SetLocalPositionY(-(contentHeight + aboveSpace));
        return null;
    }

    public GameObject AddItemTitle(ItemDataBlock itemdb, IInventoryItem itemInstance = null, float aboveSpace = 0)
    {
        float contentHeight = this.GetContentHeight();
        GameObject obj2 = NGUITools.AddChild(this.addParent, this.itemTitlePrefab);
        obj2.GetComponentInChildren<UILabel>().text = (itemInstance == null) ? itemdb.name : itemInstance.toolTip;
        UITexture componentInChildren = obj2.GetComponentInChildren<UITexture>();
        componentInChildren.material = componentInChildren.material.Clone();
        componentInChildren.material.Set("_MainTex", itemdb.GetIconTexture());
        componentInChildren.color = ((itemInstance == null) || !itemInstance.IsBroken()) ? Color.white : Color.red;
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

    private void Awake()
    {
        _globalToolTip = this;
        if (this.uiCamera == null)
        {
            this.uiCamera = NGUITools.FindCameraForLayer(base.gameObject.layer);
        }
        this.planeTest = new Plane((Vector3) (this.uiCamera.transform.forward * 1f), new Vector3(0f, 0f, 2f));
    }

    public void ClearContents()
    {
        IEnumerator enumerator = this.addParent.transform.GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                Transform current = (Transform) enumerator.Current;
                Object.Destroy(current.gameObject);
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
    }

    public void FinishPopulating()
    {
    }

    public static ItemToolTip Get()
    {
        return _globalToolTip;
    }

    public float GetContentHeight()
    {
        return NGUIMath.CalculateRelativeWidgetBounds(this.addParent.transform, this.addParent.transform).size.y;
    }

    public void Internal_SetToolTip(ItemDataBlock itemdb, IInventoryItem item)
    {
        this.ClearContents();
        if (itemdb == null)
        {
            this.SetVisible(false);
        }
        else
        {
            this.RepositionAtCursor();
            itemdb.PopulateInfoWindow(this, item);
            this._background.transform.localScale = new Vector3(250f, this.GetContentHeight() + Mathf.Abs((float) (this.addParent.transform.localPosition.y * 2f)), 1f);
            this.SetVisible(true);
        }
    }

    public void RepositionAtCursor()
    {
        Vector3 lastMousePosition = (Vector3) UICamera.lastMousePosition;
        Ray ray = this.uiCamera.ScreenPointToRay(lastMousePosition);
        float enter = 0f;
        if (this.planeTest.Raycast(ray, out enter))
        {
            base.transform.position = ray.GetPoint(enter);
            base.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y, -180f);
            AABBox box = NGUIMath.CalculateRelativeWidgetBounds(base.transform);
            float num2 = (base.transform.localPosition.x + box.size.x) - Screen.width;
            if (num2 > 0f)
            {
                base.transform.SetLocalPositionX(base.transform.localPosition.x - num2);
            }
            float num3 = Mathf.Abs((float) (base.transform.localPosition.y - box.size.y)) - Screen.height;
            if (num3 > 0f)
            {
                base.transform.SetLocalPositionY(base.transform.localPosition.y + num3);
            }
        }
    }

    public static void SetToolTip(ItemDataBlock itemdb, IInventoryItem item = null)
    {
        Get().Internal_SetToolTip(itemdb, item);
        Get().RepositionAtCursor();
    }

    public void SetVisible(bool vis)
    {
        base.GetComponent<UIPanel>().enabled = vis;
    }

    private void Update()
    {
    }
}

