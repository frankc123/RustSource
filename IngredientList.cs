using System;

public abstract class IngredientList
{
    public const uint seed = 0xf00dfeed;
    protected static int[] tempHash = new int[0x10];

    protected IngredientList()
    {
    }
}

