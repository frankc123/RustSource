using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Pagination : MonoBehaviour
{
    public int buttonGroups = 2;
    public GameObject clickableButton;
    protected int pageCount;
    protected int pageCurrent;
    public GameObject spacerLabel;

    public event SwitchToPage OnPageSwitch;

    public void DropSpacer(ref Vector3 vPos)
    {
        if (this.spacerLabel != null)
        {
            dfControl component = base.GetComponent<dfControl>();
            dfControl child = ((GameObject) Object.Instantiate(this.spacerLabel)).GetComponent<dfControl>();
            component.AddControl(child);
            child.Position = vPos;
            vPos.x += child.Width + 5f;
        }
    }

    public void OnButtonClicked(dfControl control, dfMouseEventArgs mouseEvent)
    {
        int iCurrentPage = int.Parse(control.Tooltip);
        this.Setup(this.pageCount, iCurrentPage);
        if (this.OnPageSwitch != null)
        {
            this.OnPageSwitch(iCurrentPage);
        }
    }

    public void Setup(int iPages, int iCurrentPage)
    {
        if ((this.pageCount != iPages) || (this.pageCurrent != iCurrentPage))
        {
            this.pageCount = iPages;
            this.pageCurrent = iCurrentPage;
            foreach (dfControl control in base.gameObject.GetComponentsInChildren<dfControl>())
            {
                if (control.gameObject != base.gameObject)
                {
                    Object.Destroy(control.gameObject);
                }
            }
            if (this.pageCount > 1)
            {
                dfControl component = base.GetComponent<dfControl>();
                bool flag = true;
                Vector3 vPos = new Vector3(0f, 0f, 0f);
                for (int i = 0; i < this.pageCount; i++)
                {
                    if ((((this.buttonGroups - i) <= 0) && (i < (this.pageCount - this.buttonGroups))) && (Math.Abs((int) (i - this.pageCurrent)) > (this.buttonGroups / 2)))
                    {
                        if (flag)
                        {
                            this.DropSpacer(ref vPos);
                        }
                        flag = false;
                    }
                    else
                    {
                        dfButton child = ((GameObject) Object.Instantiate(this.clickableButton)).GetComponent<dfButton>();
                        component.AddControl(child);
                        child.Tooltip = i.ToString();
                        child.MouseDown += new MouseEventHandler(this.OnButtonClicked);
                        child.Text = (i + 1).ToString();
                        child.Invalidate();
                        if (i == this.pageCurrent)
                        {
                            child.Disable();
                        }
                        child.Position = vPos;
                        vPos.x += child.Width + 5f;
                        flag = true;
                    }
                }
                component.Width = vPos.x;
            }
        }
    }

    public delegate void SwitchToPage(int iPage);
}

