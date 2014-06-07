using System;

public class RPOSArmorWindow : RPOSWindow
{
    public RPOSInvCellManager cellMan;
    public UILabel leftText;
    public UILabel rightText;

    public void ForceUpdate()
    {
        DamageTypeList armorValues;
        HumanBodyTakeDamage damage;
        if (RPOS.GetObservedPlayerComponent<HumanBodyTakeDamage>(out damage))
        {
            armorValues = damage.GetArmorValues();
        }
        else
        {
            armorValues = new DamageTypeList();
        }
        this.leftText.text = string.Empty;
        this.rightText.text = string.Empty;
        for (int i = 0; i < 6; i++)
        {
            if (armorValues[i] != 0f)
            {
                this.leftText.text = this.leftText.text + TakeDamage.DamageIndexToString((DamageTypeIndex) i) + "\n";
                string text = this.rightText.text;
                object[] objArray1 = new object[] { text, "+", (int) armorValues[i], "\n" };
                this.rightText.text = string.Concat(objArray1);
            }
        }
    }

    protected override void WindowAwake()
    {
        base.WindowAwake();
        this.cellMan = base.GetComponentInChildren<RPOSInvCellManager>();
    }
}

