using System;

public static class ReferenceTypeHelper<T>
{
    public static readonly bool TreatAsReferenceHolder;

    static ReferenceTypeHelper()
    {
        ReferenceTypeHelper<T>.TreatAsReferenceHolder = ReferenceTypeHelper.TreatAsReferenceHolder(typeof(T));
    }
}

