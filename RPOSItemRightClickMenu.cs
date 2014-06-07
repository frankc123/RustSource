using System;
using UnityEngine;

public class RPOSItemRightClickMenu : MonoBehaviour
{
    public GameObject _buttonPrefab;
    private IInventoryItem _observedItem;
    public float lastHeight;
    private static readonly InventoryItem.MenuItem[] menuItemBuffer = new InventoryItem.MenuItem[30];
    private Plane planeTest;
    public Camera uiCamera;

    public void AddRightClickEntry(string entry)
    {
        GameObject go = NGUITools.AddChild(base.gameObject, this._buttonPrefab);
        go.GetComponentInChildren<UILabel>().text = entry;
        UIEventListener listener1 = UIEventListener.Get(go);
        listener1.onClick = (UIEventListener.VoidDelegate) Delegate.Combine(listener1.onClick, new UIEventListener.VoidDelegate(this.EntryClicked));
        go.name = entry;
        Vector3 localPosition = go.transform.localPosition;
        localPosition.y = this.lastHeight;
        go.transform.localPosition = localPosition;
        this.lastHeight -= go.GetComponentInChildren<UISlicedSprite>().transform.localScale.y;
    }

    public void Awake()
    {
        if (this.uiCamera == null)
        {
            this.uiCamera = NGUITools.FindCameraForLayer(base.gameObject.layer);
        }
        this.planeTest = new Plane((Vector3) (this.uiCamera.transform.forward * 1f), new Vector3(0f, 0f, 2f));
        base.GetComponent<UIPanel>().enabled = false;
    }

    public void ClearChildren()
    {
        foreach (UIButton button in base.GetComponentsInChildren<UIButton>())
        {
            Object.Destroy(button.gameObject);
        }
        this.lastHeight = 0f;
    }

    public void EntryClicked(GameObject go)
    {
        try
        {
            if (this._observedItem != null)
            {
                InventoryItem.MenuItem? nullable;
                try
                {
                    nullable = new InventoryItem.MenuItem?((InventoryItem.MenuItem) ((byte) Enum.Parse(typeof(InventoryItem.MenuItem), go.name, true)));
                }
                catch (Exception exception)
                {
                    nullable = null;
                    Debug.LogException(exception);
                }
                if (nullable.HasValue)
                {
                    this._observedItem.OnMenuOption(nullable.Value);
                }
            }
        }
        catch (Exception exception2)
        {
            Debug.LogException(exception2);
        }
        finally
        {
            UICamera.UnPopupPanel(base.GetComponent<UIPanel>());
        }
    }

    private void PopupEnd()
    {
        base.GetComponent<UIPanel>().enabled = false;
    }

    private void PopupStart()
    {
        this.RepositionAtCursor();
        base.GetComponent<UIPanel>().enabled = true;
    }

    public void RepositionAtCursor()
    {
        Vector3 lastMousePosition = (Vector3) UICamera.lastMousePosition;
        Ray ray = this.uiCamera.ScreenPointToRay(lastMousePosition);
        float enter = 0f;
        if (this.planeTest.Raycast(ray, out enter))
        {
            base.transform.position = ray.GetPoint(enter);
            AABBox box = NGUIMath.CalculateRelativeWidgetBounds(base.transform);
            float num2 = (base.transform.localPosition.x + box.size.x) - Screen.width;
            if (num2 > 0f)
            {
                base.transform.SetLocalPositionX(base.transform.localPosition.x - num2);
            }
            float num3 = (base.transform.localPosition.y + box.size.y) - Screen.height;
            if (num3 > 0f)
            {
                base.transform.SetLocalPositionY(base.transform.localPosition.y - num3);
            }
            base.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y, -180f);
        }
    }

    public virtual void SetItem(IInventoryItem item)
    {
        this.ClearChildren();
        this._observedItem = item;
        int num = item.datablock.RetreiveMenuOptions(item, menuItemBuffer, 0);
        for (int i = 0; i < num; i++)
        {
            this.AddRightClickEntry(menuItemBuffer[i].ToString());
        }
        UICamera.PopupPanel(base.GetComponent<UIPanel>());
    }
}

