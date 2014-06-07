using System;
using UnityEngine;

public class ServerCategory : MonoBehaviour
{
    public bool activeCategory;
    public int categoryId;
    public dfLabel serverCount;

    public void CategoryChanged(int iCategory)
    {
        if (iCategory == this.categoryId)
        {
            base.GetComponent<dfControl>().Opacity = 1f;
        }
        else
        {
            base.GetComponent<dfControl>().Opacity = 0.5f;
        }
    }

    public void OnSelected()
    {
        Object.FindObjectOfType<ServerBrowser>().SwitchCategory(this.categoryId);
    }

    public void UpdateServerCount(int iCount)
    {
        if (iCount == 0)
        {
            this.serverCount.Hide();
        }
        else
        {
            this.serverCount.Show();
        }
        this.serverCount.Text = iCount.ToString("#,##0");
    }
}

