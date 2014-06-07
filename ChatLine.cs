using System;
using UnityEngine;

public class ChatLine : MonoBehaviour
{
    public dfLabel lblName;
    public dfLabel lblText;

    public void Setup(string name, string text)
    {
        this.lblName.Text = name;
        this.lblText.Text = text;
    }
}

