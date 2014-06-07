using System;
using UnityEngine;

public class VisActionMessageEnterExit : VisActionMessageEnter
{
    [SerializeField]
    protected string exitInstigatorMessage = string.Empty;
    [SerializeField]
    protected bool exitInstigatorNonNull;
    [SerializeField]
    protected string exitSelfMessage = string.Empty;
    [SerializeField]
    protected bool exitSelfNonNull;
    [SerializeField]
    protected bool exitSwapMessageOrder;
    [SerializeField]
    protected bool exitWithOtherAsArg = true;

    public override void UnAcomplish(IDMain self, IDMain instigator)
    {
        string exitInstigatorMessage;
        string exitSelfMessage;
        bool flag = self == 0;
        bool flag2 = instigator == 0;
        if (flag)
        {
            if (!flag2)
            {
                if (this.exitSelfNonNull)
                {
                    return;
                }
                Debug.LogWarning("Self is null!", this);
            }
            else
            {
                Debug.LogError("Self and instgator are null", this);
            }
        }
        else if (flag2)
        {
            if (this.exitInstigatorNonNull)
            {
                return;
            }
            Debug.LogWarning("Instigator is null!", this);
        }
        if (this.exitSwapMessageOrder)
        {
            IDMain main = self;
            self = instigator;
            instigator = main;
            exitInstigatorMessage = this.exitInstigatorMessage;
            exitSelfMessage = this.exitSelfMessage;
            bool flag3 = flag;
            flag = flag2;
            flag2 = flag3;
        }
        else
        {
            exitInstigatorMessage = this.exitSelfMessage;
            exitSelfMessage = this.exitInstigatorMessage;
        }
        if (this.exitWithOtherAsArg)
        {
            if (!flag && !string.IsNullOrEmpty(exitInstigatorMessage))
            {
                self.SendMessage(exitInstigatorMessage, instigator, SendMessageOptions.DontRequireReceiver);
            }
            if (!flag2 && !string.IsNullOrEmpty(exitSelfMessage))
            {
                instigator.SendMessage(exitSelfMessage, self, SendMessageOptions.DontRequireReceiver);
            }
        }
        else
        {
            if (!flag && !string.IsNullOrEmpty(exitInstigatorMessage))
            {
                self.SendMessage(exitInstigatorMessage, SendMessageOptions.DontRequireReceiver);
            }
            if (!flag2 && !string.IsNullOrEmpty(exitSelfMessage))
            {
                instigator.SendMessage(exitSelfMessage, SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}

