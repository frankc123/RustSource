using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct CharacterStateFlags : IFormattable, IEquatable<CharacterStateFlags>
{
    public const ushort kCrouch = 1;
    public const ushort kSprint = 2;
    public const ushort kAim = 4;
    public const ushort kAttack = 8;
    public const ushort kAirborne = 0x10;
    public const ushort kSlipping = 0x20;
    public const ushort kMovement = 0x40;
    public const ushort kLostFocus = 0x80;
    public const ushort kAttack2 = 0x100;
    public const ushort kBleeding = 0x200;
    public const ushort kCrouchBlocked = 0x400;
    public const ushort kLamp = 0x800;
    public const ushort kLaser = 0x1000;
    public const ushort kNone = 0;
    public const ushort kMask = 0x1fff;
    private const ushort kAllMask = 0xffff;
    private const ushort kNotCrouch = 0xfffe;
    private const ushort kNotSprint = 0xfffd;
    private const ushort kNotAim = 0xfffb;
    private const ushort kNotAttack = 0xfff7;
    private const ushort kNotAirborne = 0xffef;
    private const ushort kNotSlipping = 0xffdf;
    private const ushort kNotMovement = 0xffbf;
    private const ushort kNotLostFocus = 0xff7f;
    private const ushort kNotAttack2 = 0xfeff;
    private const ushort kNotBleeding = 0xfdff;
    private const ushort kNotCrouchBlocked = 0xfbff;
    private const ushort kNotLamp = 0xf7ff;
    private const ushort kNotLaser = 0xefff;
    public ushort flags;
    public CharacterStateFlags(bool crouching, bool sprinting, bool aiming, bool attacking, bool airborne, bool slipping, bool moving, bool lostFocus, bool lamp, bool laser)
    {
        ushort num = 0;
        if (crouching)
        {
            num = (ushort) (num | 1);
        }
        if (sprinting)
        {
            num = (ushort) (num | 2);
        }
        if (aiming)
        {
            num = (ushort) (num | 4);
        }
        if (attacking)
        {
            num = (ushort) (num | 8);
        }
        if (airborne)
        {
            num = (ushort) (num | 0x10);
        }
        if (slipping)
        {
            num = (ushort) (num | 0x20);
        }
        if (moving)
        {
            num = (ushort) (num | 0x40);
        }
        if (lostFocus)
        {
            num = (ushort) (num | 0x80);
        }
        if (lamp)
        {
            num = (ushort) (num | 0x800);
        }
        if (laser)
        {
            num = (ushort) (num | 0x1000);
        }
        this.flags = num;
    }

    public bool crouch
    {
        get
        {
            return ((this.flags & 1) == 1);
        }
        set
        {
            if (value)
            {
                this.flags = (ushort) (this.flags | 1);
            }
            else
            {
                this.flags = (ushort) (this.flags & 0xfffe);
            }
        }
    }
    public bool sprint
    {
        get
        {
            return ((this.flags & 2) == 2);
        }
        set
        {
            if (value)
            {
                this.flags = (ushort) (this.flags | 2);
            }
            else
            {
                this.flags = (ushort) (this.flags & 0xfffd);
            }
        }
    }
    public bool crouchBlocked
    {
        get
        {
            return ((this.flags & 0x400) == 0x400);
        }
        set
        {
            if (value)
            {
                this.flags = (ushort) (this.flags | 0x400);
            }
            else
            {
                this.flags = (ushort) (this.flags & 0xfbff);
            }
        }
    }
    public bool aim
    {
        get
        {
            return ((this.flags & 4) == 4);
        }
        set
        {
            if (value)
            {
                this.flags = (ushort) (this.flags | 4);
            }
            else
            {
                this.flags = (ushort) (this.flags & 0xfffb);
            }
        }
    }
    public bool attack
    {
        get
        {
            return ((this.flags & 8) == 8);
        }
        set
        {
            if (value)
            {
                this.flags = (ushort) (this.flags | 8);
            }
            else
            {
                this.flags = (ushort) (this.flags & 0xfff7);
            }
        }
    }
    public bool attack2
    {
        get
        {
            return ((this.flags & 8) == 0x100);
        }
        set
        {
            if (value)
            {
                this.flags = (ushort) (this.flags | 0x100);
            }
            else
            {
                this.flags = (ushort) (this.flags & 0xfeff);
            }
        }
    }
    public bool grounded
    {
        get
        {
            return ((this.flags & 0x10) != 0x10);
        }
        set
        {
            if (value)
            {
                this.flags = (ushort) (this.flags & 0xffef);
            }
            else
            {
                this.flags = (ushort) (this.flags | 0x10);
            }
        }
    }
    public bool bleeding
    {
        get
        {
            return ((this.flags & 0x200) != 0x200);
        }
        set
        {
            if (value)
            {
                this.flags = (ushort) (this.flags & 0xfdff);
            }
            else
            {
                this.flags = (ushort) (this.flags | 0x200);
            }
        }
    }
    public bool airborne
    {
        get
        {
            return ((this.flags & 0x10) == 0x10);
        }
        set
        {
            if (value)
            {
                this.flags = (ushort) (this.flags | 0x10);
            }
            else
            {
                this.flags = (ushort) (this.flags & 0xffef);
            }
        }
    }
    public bool slipping
    {
        get
        {
            return ((this.flags & 0x20) == 0x20);
        }
        set
        {
            if (value)
            {
                this.flags = (ushort) (this.flags | 0x20);
            }
            else
            {
                this.flags = (ushort) (this.flags & 0xffdf);
            }
        }
    }
    public bool movement
    {
        get
        {
            return ((this.flags & 0x40) == 0x40);
        }
        set
        {
            if (value)
            {
                this.flags = (ushort) (this.flags | 0x40);
            }
            else
            {
                this.flags = (ushort) (this.flags & 0xffbf);
            }
        }
    }
    public bool lostFocus
    {
        get
        {
            return ((this.flags & 0x80) == 0x80);
        }
        set
        {
            if (value)
            {
                this.flags = (ushort) (this.flags | 0x80);
            }
            else
            {
                this.flags = (ushort) (this.flags & 0xff7f);
            }
        }
    }
    public bool focus
    {
        get
        {
            return ((this.flags & 0x80) != 0x80);
        }
        set
        {
            if (value)
            {
                this.flags = (ushort) (this.flags & 0xff7f);
            }
            else
            {
                this.flags = (ushort) (this.flags | 0x80);
            }
        }
    }
    public bool lamp
    {
        get
        {
            return ((this.flags & 0x800) == 0x800);
        }
        set
        {
            if (value)
            {
                this.flags = (ushort) (this.flags | 0x800);
            }
            else
            {
                this.flags = (ushort) (this.flags & 0xf7ff);
            }
        }
    }
    public bool laser
    {
        get
        {
            return ((this.flags & 0x1000) == 0x1000);
        }
        set
        {
            if (value)
            {
                this.flags = (ushort) (this.flags | 0x1000);
            }
            else
            {
                this.flags = (ushort) (this.flags & 0xefff);
            }
        }
    }
    public CharacterStateFlags off
    {
        get
        {
            CharacterStateFlags flags;
            flags.flags = (ushort) (~this.flags & 0xffff);
            return flags;
        }
        set
        {
            this.flags = (ushort) (~value.flags & 0xffff);
        }
    }
    public override bool Equals(object obj)
    {
        return ((obj is CharacterStateFlags) && (((CharacterStateFlags) obj).flags == this.flags));
    }

    public bool Equals(CharacterStateFlags other)
    {
        return (this.flags == other.flags);
    }

    public override int GetHashCode()
    {
        return this.flags.GetHashCode();
    }

    public override string ToString()
    {
        return this.flags.ToString();
    }

    public string ToString(string format, IFormatProvider formatProvider)
    {
        return this.flags.ToString(format, formatProvider);
    }

    public string ToString(string format)
    {
        return this.flags.ToString(format, null);
    }

    public static CharacterStateFlags operator &(CharacterStateFlags lhs, CharacterStateFlags rhs)
    {
        lhs.flags = (ushort) (lhs.flags & rhs.flags);
        return lhs;
    }

    public static CharacterStateFlags operator |(CharacterStateFlags lhs, CharacterStateFlags rhs)
    {
        lhs.flags = (ushort) (lhs.flags | rhs.flags);
        return lhs;
    }

    public static CharacterStateFlags operator ^(CharacterStateFlags lhs, CharacterStateFlags rhs)
    {
        lhs.flags = (ushort) (lhs.flags ^ rhs.flags);
        return lhs;
    }

    public static CharacterStateFlags operator &(CharacterStateFlags lhs, ushort rhs)
    {
        lhs.flags = (ushort) (lhs.flags & rhs);
        return lhs;
    }

    public static CharacterStateFlags operator ^(CharacterStateFlags lhs, ushort rhs)
    {
        lhs.flags = (ushort) (lhs.flags | rhs);
        return lhs;
    }

    public static CharacterStateFlags operator |(CharacterStateFlags lhs, ushort rhs)
    {
        lhs.flags = (ushort) (lhs.flags ^ rhs);
        return lhs;
    }

    public static CharacterStateFlags operator &(CharacterStateFlags lhs, int rhs)
    {
        lhs.flags = (ushort) (lhs.flags & ((ushort) (rhs & 0xffff)));
        return lhs;
    }

    public static CharacterStateFlags operator ^(CharacterStateFlags lhs, int rhs)
    {
        lhs.flags = (ushort) (lhs.flags | ((ushort) (rhs & 0xffff)));
        return lhs;
    }

    public static CharacterStateFlags operator |(CharacterStateFlags lhs, int rhs)
    {
        lhs.flags = (ushort) (lhs.flags ^ ((ushort) (rhs & 0xffff)));
        return lhs;
    }

    public static CharacterStateFlags operator &(CharacterStateFlags lhs, long rhs)
    {
        lhs.flags = (ushort) (lhs.flags & ((ushort) (rhs & 0xffffL)));
        return lhs;
    }

    public static CharacterStateFlags operator ^(CharacterStateFlags lhs, long rhs)
    {
        lhs.flags = (ushort) (lhs.flags | ((ushort) (rhs & 0xffffL)));
        return lhs;
    }

    public static CharacterStateFlags operator |(CharacterStateFlags lhs, long rhs)
    {
        lhs.flags = (ushort) (lhs.flags ^ ((ushort) (rhs & 0xffffL)));
        return lhs;
    }

    public static CharacterStateFlags operator &(CharacterStateFlags lhs, uint rhs)
    {
        lhs.flags = (ushort) (lhs.flags & ((ushort) (rhs & 0xffff)));
        return lhs;
    }

    public static CharacterStateFlags operator ^(CharacterStateFlags lhs, uint rhs)
    {
        lhs.flags = (ushort) (lhs.flags | ((ushort) (rhs & 0xffff)));
        return lhs;
    }

    public static CharacterStateFlags operator |(CharacterStateFlags lhs, uint rhs)
    {
        lhs.flags = (ushort) (lhs.flags ^ ((ushort) (rhs & 0xffff)));
        return lhs;
    }

    public static CharacterStateFlags operator &(CharacterStateFlags lhs, ulong rhs)
    {
        lhs.flags = (ushort) (lhs.flags & ((ushort) (rhs & ((ulong) 0xffffL))));
        return lhs;
    }

    public static CharacterStateFlags operator ^(CharacterStateFlags lhs, ulong rhs)
    {
        lhs.flags = (ushort) (lhs.flags | ((ushort) (rhs & ((ulong) 0xffffL))));
        return lhs;
    }

    public static CharacterStateFlags operator |(CharacterStateFlags lhs, ulong rhs)
    {
        lhs.flags = (ushort) (lhs.flags ^ ((ushort) (rhs & ((ulong) 0xffffL))));
        return lhs;
    }

    public static int operator &(int lhs, CharacterStateFlags rhs)
    {
        return (lhs & rhs.flags);
    }

    public static int operator |(int lhs, CharacterStateFlags rhs)
    {
        return (lhs | rhs.flags);
    }

    public static int operator ^(int lhs, CharacterStateFlags rhs)
    {
        return (lhs ^ rhs.flags);
    }

    public static uint operator &(uint lhs, CharacterStateFlags rhs)
    {
        return (lhs & rhs.flags);
    }

    public static uint operator |(uint lhs, CharacterStateFlags rhs)
    {
        return (lhs | rhs.flags);
    }

    public static uint operator ^(uint lhs, CharacterStateFlags rhs)
    {
        return (lhs ^ rhs.flags);
    }

    public static long operator &(long lhs, CharacterStateFlags rhs)
    {
        return (lhs & rhs.flags);
    }

    public static long operator |(long lhs, CharacterStateFlags rhs)
    {
        return (lhs | rhs.flags);
    }

    public static long operator ^(long lhs, CharacterStateFlags rhs)
    {
        return (lhs ^ rhs.flags);
    }

    public static ulong operator &(ulong lhs, CharacterStateFlags rhs)
    {
        return (lhs & rhs.flags);
    }

    public static ulong operator |(ulong lhs, CharacterStateFlags rhs)
    {
        return (lhs | rhs.flags);
    }

    public static ulong operator ^(ulong lhs, CharacterStateFlags rhs)
    {
        return (lhs ^ rhs.flags);
    }

    public static int operator &(byte lhs, CharacterStateFlags rhs)
    {
        return (lhs & rhs.flags);
    }

    public static int operator |(byte lhs, CharacterStateFlags rhs)
    {
        return (lhs | rhs.flags);
    }

    public static int operator ^(byte lhs, CharacterStateFlags rhs)
    {
        return (lhs ^ rhs.flags);
    }

    public static int operator &(sbyte lhs, CharacterStateFlags rhs)
    {
        return (lhs & rhs.flags);
    }

    public static int operator |(sbyte lhs, CharacterStateFlags rhs)
    {
        return (lhs | rhs.flags);
    }

    public static int operator ^(sbyte lhs, CharacterStateFlags rhs)
    {
        return (lhs ^ rhs.flags);
    }

    public static int operator &(short lhs, CharacterStateFlags rhs)
    {
        return (lhs & rhs.flags);
    }

    public static int operator |(short lhs, CharacterStateFlags rhs)
    {
        return (lhs | rhs.flags);
    }

    public static int operator ^(short lhs, CharacterStateFlags rhs)
    {
        return (lhs ^ rhs.flags);
    }

    public static int operator &(ushort lhs, CharacterStateFlags rhs)
    {
        return (lhs & rhs.flags);
    }

    public static int operator |(ushort lhs, CharacterStateFlags rhs)
    {
        return (lhs | rhs.flags);
    }

    public static int operator ^(ushort lhs, CharacterStateFlags rhs)
    {
        return (lhs ^ rhs.flags);
    }

    public static bool operator ==(CharacterStateFlags lhs, CharacterStateFlags rhs)
    {
        return (lhs.flags == rhs.flags);
    }

    public static bool operator ==(CharacterStateFlags lhs, byte rhs)
    {
        return (lhs.flags == rhs);
    }

    public static bool operator ==(CharacterStateFlags lhs, sbyte rhs)
    {
        return (lhs.flags == rhs);
    }

    public static bool operator ==(CharacterStateFlags lhs, short rhs)
    {
        return (lhs.flags == rhs);
    }

    public static bool operator ==(CharacterStateFlags lhs, ushort rhs)
    {
        return (lhs.flags == rhs);
    }

    public static bool operator ==(CharacterStateFlags lhs, int rhs)
    {
        return (lhs.flags == rhs);
    }

    public static bool operator ==(CharacterStateFlags lhs, uint rhs)
    {
        return (lhs.flags == rhs);
    }

    public static bool operator ==(CharacterStateFlags lhs, long rhs)
    {
        return (lhs.flags == rhs);
    }

    public static bool operator ==(CharacterStateFlags lhs, ulong rhs)
    {
        return (lhs.flags == rhs);
    }

    public static bool operator ==(CharacterStateFlags lhs, bool rhs)
    {
        return ((lhs.flags != 0) == rhs);
    }

    public static bool operator !=(CharacterStateFlags lhs, CharacterStateFlags rhs)
    {
        return (lhs.flags != rhs.flags);
    }

    public static bool operator !=(CharacterStateFlags lhs, byte rhs)
    {
        return (lhs.flags != rhs);
    }

    public static bool operator !=(CharacterStateFlags lhs, sbyte rhs)
    {
        return (lhs.flags != rhs);
    }

    public static bool operator !=(CharacterStateFlags lhs, short rhs)
    {
        return (lhs.flags != rhs);
    }

    public static bool operator !=(CharacterStateFlags lhs, ushort rhs)
    {
        return (lhs.flags != rhs);
    }

    public static bool operator !=(CharacterStateFlags lhs, int rhs)
    {
        return (lhs.flags != rhs);
    }

    public static bool operator !=(CharacterStateFlags lhs, uint rhs)
    {
        return (lhs.flags != rhs);
    }

    public static bool operator !=(CharacterStateFlags lhs, long rhs)
    {
        return (lhs.flags != rhs);
    }

    public static bool operator !=(CharacterStateFlags lhs, ulong rhs)
    {
        return (lhs.flags != rhs);
    }

    public static bool operator !=(CharacterStateFlags lhs, bool rhs)
    {
        return ((lhs.flags == 0) == rhs);
    }

    public static CharacterStateFlags operator >>(CharacterStateFlags lhs, int shift)
    {
        lhs.flags = (ushort) (lhs.flags >> shift);
        return lhs;
    }

    public static CharacterStateFlags operator <<(CharacterStateFlags lhs, int shift)
    {
        lhs.flags = (ushort) (lhs.flags >> shift);
        return lhs;
    }

    public static CharacterStateFlags operator ~(CharacterStateFlags lhs)
    {
        lhs.flags = (ushort) (~lhs.flags & 0xffff);
        return lhs;
    }

    public static bool operator true(CharacterStateFlags lhs)
    {
        return (lhs.flags != 0);
    }

    public static bool operator false(CharacterStateFlags lhs)
    {
        return (lhs.flags == 0);
    }

    public static explicit operator bool(CharacterStateFlags lhs)
    {
        return (lhs.flags != 0);
    }

    public static bool op_LogicalNot(CharacterStateFlags lhs)
    {
        return (lhs.flags == 0);
    }

    public static implicit operator ushort(CharacterStateFlags lhs)
    {
        return lhs.flags;
    }

    public static implicit operator CharacterStateFlags(ushort lhs)
    {
        CharacterStateFlags flags;
        flags.flags = lhs;
        return flags;
    }

    public static implicit operator CharacterStateFlags(int lhs)
    {
        CharacterStateFlags flags;
        flags.flags = (ushort) (lhs & 0xffff);
        return flags;
    }

    public static implicit operator CharacterStateFlags(long lhs)
    {
        CharacterStateFlags flags;
        flags.flags = (ushort) (lhs & 0xffffL);
        return flags;
    }

    public static implicit operator CharacterStateFlags(uint lhs)
    {
        CharacterStateFlags flags;
        flags.flags = (ushort) (lhs & 0xffff);
        return flags;
    }

    public static implicit operator CharacterStateFlags(ulong lhs)
    {
        CharacterStateFlags flags;
        flags.flags = (ushort) (lhs & ((ulong) 0xffffL));
        return flags;
    }
}

