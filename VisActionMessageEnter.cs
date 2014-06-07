using System;
using UnityEngine;

public class VisActionMessageEnter : VisAction
{
    [SerializeField]
    protected string instigatorMessage = string.Empty;
    [SerializeField]
    protected bool instigatorNonNull;
    [SerializeField]
    protected string selfMessage = string.Empty;
    [SerializeField]
    protected bool selfNonNull;
    [SerializeField]
    protected bool swapMessageOrder;
    [SerializeField]
    protected bool withOtherAsArg = true;

    public override void Accomplish(IDMain self, IDMain instigator)
    {
        string instigatorMessage;
        string selfMessage;
        bool flag = self == 0;
        bool flag2 = instigator == 0;
        if (flag)
        {
            if (!flag2)
            {
                if (this.selfNonNull)
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
            if (this.instigatorNonNull)
            {
                return;
            }
            Debug.LogWarning("Instigator is null!", this);
        }
        if (this.swapMessageOrder)
        {
            IDMain main = self;
            self = instigator;
            instigator = main;
            instigatorMessage = this.instigatorMessage;
            selfMessage = this.selfMessage;
            bool flag3 = flag;
            flag = flag2;
            flag2 = flag3;
        }
        else
        {
            instigatorMessage = this.selfMessage;
            selfMessage = this.instigatorMessage;
        }
        if (this.withOtherAsArg)
        {
            if (!flag && !string.IsNullOrEmpty(instigatorMessage))
            {
                self.SendMessage(instigatorMessage, instigator, SendMessageOptions.DontRequireReceiver);
            }
            if (!flag2 && !string.IsNullOrEmpty(selfMessage))
            {
                instigator.SendMessage(selfMessage, self, SendMessageOptions.DontRequireReceiver);
            }
        }
        else
        {
            if (!flag && !string.IsNullOrEmpty(instigatorMessage))
            {
                self.SendMessage(instigatorMessage, SendMessageOptions.DontRequireReceiver);
            }
            if (!flag2 && !string.IsNullOrEmpty(selfMessage))
            {
                instigator.SendMessage(selfMessage, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    public override void UnAcomplish(IDMain self, IDMain instigator)
    {
    }
}

