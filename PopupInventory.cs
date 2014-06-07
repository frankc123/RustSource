using System;
using UnityEngine;

public class PopupInventory : MonoBehaviour
{
    private static int iYPos;
    public dfRichTextLabel labelText;
    public dfTweenVector3 tweenOut;

    public void PlayOut()
    {
        this.tweenOut.Play();
        Object.Destroy(base.gameObject, this.tweenOut.Length);
    }

    public void Setup(float fSeconds, string strText)
    {
        Vector2 vector3;
        Vector2 size = base.transform.parent.GetComponent<dfPanel>().Size;
        dfPanel component = base.GetComponent<dfPanel>();
        Vector2 vector2 = this.labelText.Font.MeasureText(strText, this.labelText.FontSize, this.labelText.FontStyle);
        this.labelText.Width = vector2.x + 16f;
        component.Width = (this.labelText.RelativePosition.x + this.labelText.Width) + 8f;
        vector3 = new Vector2 {
            x = size.x + Random.Range((float) -16f, (float) 16f),
            y = (size.y * 0.7f) + Random.Range((float) -16f, (float) 16f),
            y = vector3.y + ((((((float) iYPos) / 6f) - 0.5f) * size.y) * 0.2f)
        };
        component.RelativePosition = (Vector3) vector3;
        iYPos++;
        if (iYPos > 5)
        {
            iYPos = 0;
        }
        Vector3 endValue = this.tweenOut.EndValue;
        endValue.y = Random.Range((float) -100f, (float) 100f);
        this.tweenOut.EndValue = endValue;
        component.BringToFront();
        this.labelText.Text = strText;
        base.Invoke("PlayOut", fSeconds);
    }
}

