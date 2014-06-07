using System;
using System.Runtime.CompilerServices;

public static class ContextStatus
{
    public const ContextStatusFlags MASK_SPRITE = (ContextStatusFlags.SpriteFlag1 | ContextStatusFlags.SpriteFlag0);
    public const ContextStatusFlags ObjectBroken = ContextStatusFlags.ObjectBroken;
    public const ContextStatusFlags ObjectBusy = ContextStatusFlags.ObjectBusy;
    public const ContextStatusFlags ObjectEmpty = ContextStatusFlags.ObjectEmpty;
    public const ContextStatusFlags ObjectOccupied = ContextStatusFlags.ObjectOccupied;
    public const ContextStatusFlags SPRITE_ALWAYS = (ContextStatusFlags.SpriteFlag1 | ContextStatusFlags.SpriteFlag0);
    public const ContextStatusFlags SPRITE_DEFAULT = 0;
    public const ContextStatusFlags SPRITE_FRACTION = ContextStatusFlags.SpriteFlag0;
    public const ContextStatusFlags SPRITE_NEVER = ContextStatusFlags.SpriteFlag1;

    public static ContextStatusFlags CopyWithSpriteSetting(this ContextStatusFlags statusFlags, ContextStatusFlags SPRITE_SETTING)
    {
        return ((statusFlags & ~(ContextStatusFlags.SpriteFlag1 | ContextStatusFlags.SpriteFlag0)) | (SPRITE_SETTING & (ContextStatusFlags.SpriteFlag1 | ContextStatusFlags.SpriteFlag0)));
    }

    public static ContextStatusFlags GetSpriteFlags(this ContextStatusFlags statusFlags)
    {
        return (statusFlags & (ContextStatusFlags.SpriteFlag1 | ContextStatusFlags.SpriteFlag0));
    }
}

