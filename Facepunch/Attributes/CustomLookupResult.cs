namespace Facepunch.Attributes
{
    using System;

    public enum CustomLookupResult
    {
        Accept = 1,
        AcceptConfirmed = 2,
        FailCast = -2,
        FailConfirm = -6,
        FailConfirmException = -7,
        FailCustom = -4,
        FailCustomException = -5,
        FailInterface = -3,
        FailNull = -1,
        Fallback = 0
    }
}

