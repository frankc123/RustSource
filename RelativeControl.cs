using System;

public enum RelativeControl : sbyte
{
    Assigned = -2,
    Incompatible = 0,
    IsOverriding = -1,
    OverriddenBy = 2
}

