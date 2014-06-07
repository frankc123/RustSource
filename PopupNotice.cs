using System;
using UnityEngine;

public class PopupNotice : MonoBehaviour
{
    public dfRichTextLabel labelIcon;
    public dfRichTextLabel labelText;
    public dfTweenVector3 tweenOut;

    public void PlayOut()
    {
        this.tweenOut.Play();
        Object.Destroy(base.gameObject, this.tweenOut.Length);
    }

    public void Setup(float fSeconds, string strIcon, string strText)
    {
        Vector2 size = base.transform.parent.GetComponent<dfPanel>().Size;
        dfPanel component = base.GetComponent<dfPanel>();
        Vector2 vector2 = this.labelText.Font.MeasureText(strText, this.labelText.FontSize, this.labelText.FontStyle);
        this.labelText.Width = vector2.x + 16f;
        component.Width = (this.labelText.RelativePosition.x + this.labelText.Width) + 8f;
        Vector2 vector3 = new Vector2 {
            x = ((size.x - component.Width) / 2f) + Random.Range((float) -32f, (float) 32f),
            y = (component.Height * -1f) + Random.Range((float) -32f, (float) 32f)
        };
        component.RelativePosition = (Vector3) vector3;
        this.labelIcon.Text = strIcon;
        this.labelText.Text = strText;
        component.BringToFront();
        base.Invoke("PlayOut", fSeconds);
    }
}

