using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct RepairEvent
{
    public IDBase doner;
    public TakeDamage receiver;
    public float givenAmount;
    public float usedAmount;
    public RepairStatus status;
    public IDMain beneficiary
    {
        get
        {
            return ((this.receiver == null) ? null : this.receiver.idMain);
        }
    }
    public override string ToString()
    {
        object[] args = new object[] { this.beneficiary, this.givenAmount, this.status, this.doner, this.receiver, this.usedAmount };
        return string.Format("[RepairEvent: beneficiary={0} givenAmount={1} usedAmount={5} status={2} doner={3} receiver={4}]", args);
    }
}

