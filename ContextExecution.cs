using System;

[Flags]
public enum ContextExecution : byte
{
    Menu = 2,
    NotAvailable = 0,
    Quick = 1
}

