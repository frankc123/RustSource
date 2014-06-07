using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PopupUI : MonoBehaviour
{
    protected dfPanel panelLocal;
    public Object prefabInventory;
    public Object prefabNotice;
    public static PopupUI singleton;

    public void CreateInventory(string strText)
    {
        GameObject obj2 = (GameObject) Object.Instantiate(this.prefabInventory);
        this.panelLocal.AddControl(obj2.GetComponent<dfPanel>());
        obj2.GetComponent<PopupInventory>().Setup(1.5f, strText);
    }

    public void CreateNotice(float fSeconds, string strIcon, string strText)
    {
        GameObject obj2 = (GameObject) Object.Instantiate(this.prefabNotice);
        this.panelLocal.AddControl(obj2.GetComponent<dfPanel>());
        obj2.GetComponent<PopupNotice>().Setup(fSeconds, strIcon, strText);
    }

    [DebuggerHidden]
    public IEnumerator DoTests()
    {
        return new <DoTests>c__Iterator34 { <>f__this = this };
    }

    private void Start()
    {
        singleton = this;
        this.panelLocal = base.GetComponent<dfPanel>();
    }

    [CompilerGenerated]
    private sealed class <DoTests>c__Iterator34 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal PopupUI <>f__this;

        [DebuggerHidden]
        public void Dispose()
        {
            this.$PC = -1;
        }

        public bool MoveNext()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            switch (num)
            {
                case 0:
                    this.<>f__this.CreateNotice(10f, "", "You've woken up from 24 days of unconsciousness.");
                    this.$current = new WaitForSeconds(1f);
                    this.$PC = 1;
                    goto Label_0410;

                case 1:
                    this.<>f__this.CreateNotice(3f, "", "ONE");
                    this.<>f__this.CreateInventory("10 x Wood");
                    this.$current = new WaitForSeconds(1f);
                    this.$PC = 2;
                    goto Label_0410;

                case 2:
                    this.<>f__this.CreateNotice(3f, "", "You TWO.");
                    this.$current = new WaitForSeconds(1f);
                    this.$PC = 3;
                    goto Label_0410;

                case 3:
                    this.<>f__this.CreateNotice(3f, "", "TGHREEEE wank.");
                    this.$current = new WaitForSeconds(1f);
                    this.$PC = 4;
                    goto Label_0410;

                case 4:
                    this.<>f__this.CreateInventory("10 x Wood");
                    this.<>f__this.CreateNotice(3f, "", "FOUR wank.");
                    this.$current = new WaitForSeconds(1f);
                    this.$PC = 5;
                    goto Label_0410;

                case 5:
                    this.<>f__this.CreateNotice(3f, "", "FIVE wank.");
                    this.$current = new WaitForSeconds(1f);
                    this.$PC = 6;
                    goto Label_0410;

                case 6:
                    this.<>f__this.CreateInventory("1 x Rock");
                    this.$current = new WaitForSeconds(0.2f);
                    this.$PC = 7;
                    goto Label_0410;

                case 7:
                    this.<>f__this.CreateInventory("10 x Wood");
                    this.$current = new WaitForSeconds(1.2f);
                    this.$PC = 8;
                    goto Label_0410;

                case 8:
                    this.<>f__this.CreateInventory("7 x Rock");
                    this.$current = new WaitForSeconds(1.2f);
                    this.$PC = 9;
                    goto Label_0410;

                case 9:
                    this.<>f__this.CreateInventory("10 x Wood");
                    this.$current = new WaitForSeconds(1.3f);
                    this.$PC = 10;
                    goto Label_0410;

                case 10:
                    this.<>f__this.CreateInventory("1 x Rock");
                    this.$current = new WaitForSeconds(0.1f);
                    this.$PC = 11;
                    goto Label_0410;

                case 11:
                    this.<>f__this.CreateInventory("10 x Wood");
                    this.$current = new WaitForSeconds(0.7f);
                    this.$PC = 12;
                    goto Label_0410;

                case 12:
                    this.<>f__this.CreateInventory("7 x Rock");
                    this.$current = new WaitForSeconds(0.3f);
                    this.$PC = 13;
                    goto Label_0410;

                case 13:
                    this.<>f__this.CreateInventory("10 x Wood");
                    this.$current = new WaitForSeconds(2.4f);
                    this.$PC = 14;
                    goto Label_0410;

                case 14:
                    this.<>f__this.CreateInventory("1 x Rock");
                    this.$current = new WaitForSeconds(0.5f);
                    this.$PC = 15;
                    goto Label_0410;

                case 15:
                    this.<>f__this.CreateInventory("10 x Wood");
                    this.$current = new WaitForSeconds(0.3f);
                    this.$PC = 0x10;
                    goto Label_0410;

                case 0x10:
                    this.<>f__this.CreateInventory("7 x Rock");
                    this.$current = new WaitForSeconds(0.3f);
                    this.$PC = 0x11;
                    goto Label_0410;

                case 0x11:
                    this.<>f__this.CreateNotice(3f, "", "Big sweaty testicles");
                    this.$current = new WaitForSeconds(1f);
                    this.$PC = 0x12;
                    goto Label_0410;

                case 0x12:
                    this.<>f__this.CreateNotice(3f, "", "Dry testicles");
                    this.$PC = -1;
                    break;
            }
            return false;
        Label_0410:
            return true;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        object IEnumerator<object>.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }

        object IEnumerator.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }
    }
}

