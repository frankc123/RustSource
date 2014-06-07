using System;
using System.Collections.Generic;
using UnityEngine;

public class GameTooltipManager : MonoBehaviour
{
    public static GameTooltipManager Singleton;
    public GameObject tooltipPrefab;
    protected List<TooltipContainer> tooltips = new List<TooltipContainer>();

    protected TooltipContainer GetTipContainer(GameObject obj)
    {
        int num = Time.frameCount - 3;
        foreach (TooltipContainer container in this.tooltips)
        {
            if ((container.lastSeen >= num) && (container.target == obj))
            {
                return container;
            }
        }
        foreach (TooltipContainer container2 in this.tooltips)
        {
            if (container2.target == null)
            {
                return container2;
            }
            if (container2.lastSeen < num)
            {
                return container2;
            }
        }
        return null;
    }

    private void Start()
    {
        Singleton = this;
        for (int i = 0; i < 0x10; i++)
        {
            TooltipContainer item = new TooltipContainer();
            GameObject obj2 = (GameObject) Object.Instantiate(this.tooltipPrefab);
            obj2.transform.parent = base.transform;
            item.tooltip = obj2.GetComponent<dfControl>();
            item.tooltip_label = obj2.GetComponent<dfLabel>();
            item.lastSeen = 0;
            this.tooltips.Add(item);
        }
    }

    private void Update()
    {
        float num = Time.frameCount - 3;
        foreach (TooltipContainer container in this.tooltips)
        {
            if ((container.lastSeen <= num) && container.tooltip.IsVisible)
            {
                container.tooltip.Hide();
            }
        }
    }

    public void UpdateTip(GameObject obj, string text, Vector3 vPosition, Color color, float alpha, float fscale)
    {
        TooltipContainer tipContainer = this.GetTipContainer(obj);
        if (tipContainer != null)
        {
            if (!tipContainer.tooltip.IsVisible)
            {
                tipContainer.tooltip.Show();
            }
            dfGUIManager manager = tipContainer.tooltip.GetManager();
            Vector2 screenSize = manager.GetScreenSize();
            Camera renderCamera = manager.RenderCamera;
            Camera main = Camera.main;
            Vector3 position = Camera.main.WorldToScreenPoint(vPosition);
            position.x = screenSize.x * (position.x / main.pixelWidth);
            position.y = screenSize.y * (position.y / main.pixelHeight);
            position = (Vector3) manager.ScreenToGui(position);
            position.x -= (tipContainer.tooltip.Width / 2f) * tipContainer.tooltip.transform.localScale.x;
            position.y -= tipContainer.tooltip.Height * tipContainer.tooltip.transform.localScale.y;
            tipContainer.tooltip.RelativePosition = position;
            tipContainer.tooltip_label.Text = text;
            tipContainer.tooltip_label.Color = color;
            tipContainer.tooltip.Opacity = alpha;
            tipContainer.lastSeen = Time.frameCount;
            tipContainer.target = obj;
            tipContainer.tooltip.transform.localScale = new Vector3(fscale, fscale, fscale);
        }
    }

    protected class TooltipContainer
    {
        public int lastSeen;
        public GameObject target;
        public dfControl tooltip;
        public dfLabel tooltip_label;
    }
}

