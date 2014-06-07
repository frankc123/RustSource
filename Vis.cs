using System;
using System.Collections.Specialized;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

public static class Vis
{
    public const int kComparisonContact = 13;
    public const int kComparisonIsSelf = 12;
    public const int kComparisonOblivious = 1;
    public const int kComparisonPrey = 9;
    public const int kComparisonStealthy = 5;
    public const int kFlagRelative = 1;
    public const int kFlagSelf = 8;
    public const int kFlagTarget = 4;
    public const Life kLifeAll = (Life.Dead | Life.Unconcious | Life.Alive);
    public const Trait kLifeBegin = Trait.Alive;
    public const int kLifeCount = 3;
    public const Trait kLifeEnd = (Trait.Dead | Trait.Unconcious);
    public const Trait kLifeFirst = Trait.Alive;
    public const Trait kLifeLast = Trait.Dead;
    public const int kLifeMask = 7;
    public const Life kLifeNone = 0;
    public const Role kRoleAll = -1;
    public const Trait kRoleBegin = Trait.Citizen;
    public const int kRoleCount = 8;
    public const Trait kRoleEnd = ((Trait) 0x20);
    public const Trait kRoleFirst = Trait.Citizen;
    public const Trait kRoleLast = Trait.Animal;
    public const int kRoleMask = -16777216;
    public const Role kRoleNone = 0;
    public const Status kStatusAll = (Status.Attacking | Status.Armed | Status.Search | Status.Alert | Status.Curious | Status.Hurt | Status.Casual);
    public const Trait kStatusBegin = Trait.Casual;
    public const int kStatusCount = 7;
    public const Trait kStatusEnd = (Trait.Attacking | Trait.Unconcious);
    public const Trait kStatusFirst = Trait.Casual;
    public const Trait kStatusLast = Trait.Attacking;
    public const int kStatusMask = 0x7f00;
    public const Status kStatusNone = 0;
    public const Trait kTraitBegin = Trait.Alive;
    public const Trait kTraitEnd = ((Trait) 0x20);
    private const int mask24b = 0xffffff;
    private const int mask24o7b = 0x1000000;
    private const int mask31b = 0x7fffffff;
    private const int mask31o1b = -2147483648;
    private const byte mask7b = 0x7f;
    private const byte mask7o1b = 0x80;
    private const uint one = 1;
    private const int OpZero = 3;

    public static int CountSeen(this Comparison comparison)
    {
        int num2 = (int) comparison;
        if ((num2 & 1) == 1)
        {
            if ((num2 & 4) == 4)
            {
                num2++;
            }
            if ((num2 & 8) == 8)
            {
                num2++;
            }
        }
        return 0;
    }

    public static bool DoesSeeTarget(this Comparison comparison)
    {
        return ((comparison & ((Comparison) 4)) == ((Comparison) 4));
    }

    internal static bool Evaluate(Op op, int f, int m)
    {
        switch (op)
        {
            case Op.Always:
                return true;

            case Op.Equals:
                return (m == f);

            case Op.All:
                return ((m & f) == f);

            case Op.Any:
                return ((m & f) != 0);

            case Op.None:
                return ((m & f) == 0);

            case Op.NotEquals:
                return (m != f);
        }
        return false;
    }

    public static VisNode.Search.Radial.Enumerator GetNodesInRadius(Vector3 point, float radius)
    {
        return new VisNode.Search.Radial.Enumerator(new VisNode.Search.PointRadiusData(point, radius));
    }

    public static VisNode.Search.Point.Visual.Enumerator GetNodesWhoCanSee(Vector3 point)
    {
        return new VisNode.Search.Point.Visual.Enumerator(new VisNode.Search.PointVisibilityData(point));
    }

    public static int GetStealth(this Comparison comparison)
    {
        int num = ((int) comparison) & 12;
        if (num == 4)
        {
            return 1;
        }
        if (num != 8)
        {
            return 0;
        }
        return -1;
    }

    public static bool IsOneWay(this Comparison comparison)
    {
        return ((comparison & Comparison.IsSelf) != Comparison.IsSelf);
    }

    public static bool IsSeenByTarget(this Comparison comparison)
    {
        return ((comparison & ((Comparison) 8)) == ((Comparison) 8));
    }

    public static bool IsTwoWay(this Comparison comparison)
    {
        return ((comparison & Comparison.Contact) != Comparison.IsSelf);
    }

    public static bool IsZeroWay(this Comparison comparison)
    {
        return ((comparison & Comparison.Contact) == Comparison.Oblivious);
    }

    public static Op Negate(Op op)
    {
        return (Op.Any - (op - 3));
    }

    public static Op<TFlags> Negate<TFlags>(Op<TFlags> op) where TFlags: struct, IConvertible, IFormattable, IComparable
    {
        op.op = Negate(op.op);
        return op;
    }

    public static void RadialMessage(Vector3 point, float radius, string message)
    {
        VisNode.Search.Radial.Enumerator nodesInRadius = GetNodesInRadius(point, radius);
        while (nodesInRadius.MoveNext())
        {
            nodesInRadius.Current.SendMessage(message, SendMessageOptions.DontRequireReceiver);
        }
    }

    public static void RadialMessage(Vector3 point, float radius, string message, object arg)
    {
        VisNode.Search.Radial.Enumerator nodesInRadius = GetNodesInRadius(point, radius);
        while (nodesInRadius.MoveNext())
        {
            nodesInRadius.Current.SendMessage(message, arg, SendMessageOptions.DontRequireReceiver);
        }
    }

    private static int SetTrue(Op op, int f, int m)
    {
        switch (op)
        {
            case Op.Always:
            case Op.Never:
                return m;

            case Op.Equals:
                return f;

            case Op.All:
                return (m | f);

            case Op.Any:
                if ((m & f) != 0)
                {
                    return m;
                }
                return (m | f);

            case Op.None:
                return (m & ~f);

            case Op.NotEquals:
                return ~f;
        }
        return m;
    }

    private static int SetTrue(Op op, int f, ref BitVector32 bits, BitVector32.Section sect)
    {
        int num2;
        int m = bits[sect];
        if (m != (num2 = SetTrue(op, f, m)))
        {
            bits[sect] = m;
        }
        return num2;
    }

    public static void VisibleMessage(Vector3 point, string message)
    {
        VisNode.Search.Point.Visual.Enumerator nodesWhoCanSee = GetNodesWhoCanSee(point);
        while (nodesWhoCanSee.MoveNext())
        {
            nodesWhoCanSee.Current.SendMessage(message, SendMessageOptions.DontRequireReceiver);
        }
    }

    public static void VisibleMessage(Vector3 point, string message, object arg)
    {
        VisNode.Search.Point.Visual.Enumerator nodesWhoCanSee = GetNodesWhoCanSee(point);
        while (nodesWhoCanSee.MoveNext())
        {
            nodesWhoCanSee.Current.SendMessage(message, arg, SendMessageOptions.DontRequireReceiver);
        }
    }

    public enum Comparison
    {
        Contact = 13,
        IsSelf = 12,
        Oblivious = 1,
        Prey = 9,
        Stealthy = 5
    }

    private static class EnumUtil<TEnum> where TEnum: struct, IConvertible, IFormattable, IComparable
    {
        public static int ToInt(TEnum val)
        {
            return Convert.ToInt32(val);
        }
    }

    [Flags]
    public enum Flag
    {
        Relative = 1,
        Self = 8,
        Target = 4,
        Zero = 0
    }

    [Flags]
    public enum Life
    {
        Alive = 1,
        Dead = 4,
        Unconcious = 2
    }

    [StructLayout(LayoutKind.Explicit, Size=4)]
    public struct Mask
    {
        [FieldOffset(0)]
        public BitVector32 bits;
        [FieldOffset(0)]
        public int data;
        public const int kAlert = 0x800;
        public const int kAlive = 1;
        public const int kArmed = 0x2000;
        public const int kAttacking = 0x4000;
        public const int kAuthority = 0x4000000;
        public const int kCasual = 0x100;
        public const int kCriminal = 0x2000000;
        public const int kCurious = 0x400;
        public const int kDead = 4;
        public const int kHurt = 0x200;
        public const int kSearch = 0x1000;
        public const int kUnconcious = 2;
        private static BitVector32.Section s_life;
        private static BitVector32.Section s_role;
        private static BitVector32.Section s_stat;
        [FieldOffset(0)]
        public uint udata;
        public static readonly Vis.Mask zero;

        static Mask()
        {
            zero = new Vis.Mask();
            int i = 0;
            sect(0, ref i);
            BitVector32.Section? nullable = new BitVector32.Section?(sect(3, ref i));
            sect(5, ref i);
            BitVector32.Section? nullable2 = new BitVector32.Section?(sect(7, ref i));
            sect(9, ref i);
            BitVector32.Section? nullable3 = new BitVector32.Section?(sect(8, ref i));
            s_life = nullable.GetValueOrDefault();
            s_stat = nullable2.GetValueOrDefault();
            s_role = nullable3.GetValueOrDefault();
        }

        public bool All(Vis.Life f)
        {
            return ((this.life & f) == f);
        }

        public bool All(Vis.Role f)
        {
            return ((this.role & f) == f);
        }

        public bool All(Vis.Status f)
        {
            return ((this.stat & f) == f);
        }

        public bool AllMore(Vis.Life f)
        {
            Vis.Life life = this.life;
            return ((life > f) && ((life & f) == f));
        }

        public bool AllMore(Vis.Role f)
        {
            Vis.Role role = this.role;
            return ((role > f) && ((role & f) == f));
        }

        public bool AllMore(Vis.Status f)
        {
            Vis.Status stat = this.stat;
            return ((stat > f) && ((stat & f) == f));
        }

        public bool Any(Vis.Life f)
        {
            return ((this.life & f) > 0);
        }

        public bool Any(Vis.Role f)
        {
            return ((this.role & f) > 0);
        }

        public bool Any(Vis.Status f)
        {
            return ((this.stat & f) > 0);
        }

        public bool AnyLess(Vis.Life f)
        {
            return ((this.life & f) < f);
        }

        public bool AnyLess(Vis.Role f)
        {
            return ((this.role & f) < f);
        }

        public bool AnyLess(Vis.Status f)
        {
            return ((this.stat & f) < f);
        }

        public void Append(Vis.Life f)
        {
            this.life |= f;
        }

        public void Append(Vis.Role f)
        {
            this.role |= f;
        }

        public void Append(Vis.Status f)
        {
            this.stat |= f;
        }

        public Vis.Life AppendNot(Vis.Life f)
        {
            Vis.Life life = (this.life ^ f) & f;
            this.life |= life;
            return life;
        }

        public Vis.Role AppendNot(Vis.Role f)
        {
            Vis.Role role = (this.role ^ f) & f;
            this.role |= role;
            return role;
        }

        public Vis.Status AppendNot(Vis.Status f)
        {
            Vis.Status status = (this.stat ^ f) & f;
            this.stat |= status;
            return status;
        }

        public Vis.Life Apply(Vis.Op<Vis.Life> op)
        {
            return (Vis.Life) Vis.SetTrue((Vis.Op) op, op.intvalue, ref this.bits, s_life);
        }

        public Vis.Role Apply(Vis.Op<Vis.Role> op)
        {
            return (Vis.Role) Vis.SetTrue((Vis.Op) op, op.intvalue, ref this.bits, s_role);
        }

        public Vis.Status Apply(Vis.Op<Vis.Status> op)
        {
            return (Vis.Status) Vis.SetTrue((Vis.Op) op, op.intvalue, ref this.bits, s_stat);
        }

        public Vis.Life Apply(Vis.Op op, Vis.Life f)
        {
            return (Vis.Life) Vis.SetTrue(op, (int) f, ref this.bits, s_life);
        }

        public Vis.Role Apply(Vis.Op op, Vis.Role f)
        {
            return (Vis.Role) Vis.SetTrue(op, (int) f, ref this.bits, s_role);
        }

        public Vis.Status Apply(Vis.Op op, Vis.Status f)
        {
            return (Vis.Status) Vis.SetTrue(op, (int) f, ref this.bits, s_stat);
        }

        public bool Equals(Vis.Life f)
        {
            return (this.life == f);
        }

        public bool Equals(Vis.Role f)
        {
            return (this.role == f);
        }

        public bool Equals(Vis.Status f)
        {
            return (this.stat == f);
        }

        public bool Eval(Vis.Op<Vis.Life> op)
        {
            return (op == ((Vis.Op<Vis.Life>) this.life));
        }

        public bool Eval(Vis.Op<Vis.Role> op)
        {
            return (op == ((Vis.Op<Vis.Role>) this.role));
        }

        public bool Eval(Vis.Op<Vis.Status> op)
        {
            return (op == ((Vis.Op<Vis.Status>) this.stat));
        }

        public bool Eval(Vis.Op op, Vis.Life f)
        {
            return Vis.Evaluate(op, (int) f, this.bits[s_life]);
        }

        public bool Eval(Vis.Op op, Vis.Role f)
        {
            return Vis.Evaluate(op, (int) f, this.bits[s_role]);
        }

        public bool Eval(Vis.Op op, Vis.Status f)
        {
            return Vis.Evaluate(op, (int) f, this.bits[s_stat]);
        }

        public Vis.Life Not(Vis.Life f)
        {
            return ((this.life ^ f) & f);
        }

        public Vis.Role Not(Vis.Role f)
        {
            return ((this.role ^ f) & f);
        }

        public Vis.Status Not(Vis.Status f)
        {
            return ((this.stat ^ f) & f);
        }

        public void Remove(Vis.Life f)
        {
            this.life &= ~f;
        }

        public void Remove(Vis.Role f)
        {
            this.role &= ~f;
        }

        public void Remove(Vis.Status f)
        {
            this.stat &= ~f;
        }

        private static BitVector32.Section sect(int count, ref int i)
        {
            return sect_(count, ref i);
        }

        private static BitVector32.Section sect_(int count, ref int i)
        {
            BitVector32.Section section2;
            if (count == 0)
            {
                return new BitVector32.Section();
            }
            if (i == 0)
            {
                BitVector32.Section section = BitVector32.CreateSection((short) ((((int) 1) << count) - 1));
                i += count;
                return section;
            }
            int num = i;
            if (num >= 8)
            {
                section2 = BitVector32.CreateSection(0xff);
                num -= 8;
                while (num >= 8)
                {
                    section2 = BitVector32.CreateSection(0xff, section2);
                    num -= 8;
                }
                if (num > 0)
                {
                    section2 = BitVector32.CreateSection((short) ((((int) 1) << num) - 1), section2);
                }
            }
            else
            {
                section2 = BitVector32.CreateSection((short) ((((int) 1) << num) - 1));
            }
            BitVector32.Section section3 = BitVector32.CreateSection((short) ((((int) 1) << count) - 1), section2);
            i += count;
            return section3;
        }

        public bool this[Vis.Life mask]
        {
            get
            {
                return ((this.life & mask) != 0);
            }
            set
            {
                if (value)
                {
                    this.life |= mask;
                }
                else
                {
                    this.life &= ~mask;
                }
            }
        }

        public bool this[Vis.Status mask]
        {
            get
            {
                return ((this.stat & mask) != 0);
            }
            set
            {
                if (value)
                {
                    this.stat |= mask;
                }
                else
                {
                    this.stat &= ~mask;
                }
            }
        }

        public bool this[Vis.Role mask]
        {
            get
            {
                return ((this.role & mask) != 0);
            }
            set
            {
                if (value)
                {
                    this.role |= mask;
                }
                else
                {
                    this.role &= ~mask;
                }
            }
        }

        public bool this[Vis.Op op, Vis.Life val]
        {
            get
            {
                return this.Eval(op, val);
            }
            set
            {
                this.Apply(op, val);
            }
        }

        public bool this[Vis.Op op, Vis.Status val]
        {
            get
            {
                return this.Eval(op, val);
            }
            set
            {
                this.Apply(op, val);
            }
        }

        public bool this[Vis.Op op, Vis.Role val]
        {
            get
            {
                return this.Eval(op, val);
            }
            set
            {
                this.Apply(op, val);
            }
        }

        public bool this[Vis.Trait trait]
        {
            get
            {
                return this.bits[((int) 1) << trait];
            }
        }

        public bool this[int mask]
        {
            get
            {
                return this.bits[mask];
            }
        }

        public Vis.Op<Vis.Life>.Res this[Vis.Op<Vis.Life> op]
        {
            get
            {
                return op.Eval(this.bits[s_life]);
            }
        }

        public Vis.Op<Vis.Role>.Res this[Vis.Op<Vis.Role> op]
        {
            get
            {
                return op.Eval(this.bits[s_role]);
            }
        }

        public Vis.Op<Vis.Status>.Res this[Vis.Op<Vis.Status> op]
        {
            get
            {
                return op.Eval(this.bits[s_stat]);
            }
        }

        public Vis.Life life
        {
            get
            {
                return (Vis.Life) this.bits[s_life];
            }
            set
            {
                this.bits[s_life] = (int) (value & ((Vis.Life) s_life.Mask));
            }
        }

        public Vis.Role role
        {
            get
            {
                return (Vis.Role) this.bits[s_role];
            }
            set
            {
                this.bits[s_role] = (int) (value & ((Vis.Role) s_role.Mask));
            }
        }

        public Vis.Status stat
        {
            get
            {
                return (Vis.Status) this.bits[s_stat];
            }
            set
            {
                this.bits[s_stat] = (int) (value & ((Vis.Status) s_stat.Mask));
            }
        }
    }

    public enum Op
    {
        Always,
        Equals,
        All,
        Any,
        None,
        NotEquals,
        Never
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Op<TFlags> : IEquatable<Vis.Op>, IEquatable<Vis.Op<TFlags>> where TFlags: struct, IConvertible, IFormattable, IComparable
    {
        private Vis.OpBase _;
        internal Op(byte op, int val)
        {
            this._ = new Vis.OpBase(op, val);
        }

        public Op(Vis.Op op, TFlags flags) : this((byte) op, Convert.ToInt32(flags))
        {
        }

        internal Op(int op, int flags) : this((byte) op, flags)
        {
        }

        private static int ToInt(TFlags f)
        {
            return Vis.EnumUtil<TFlags>.ToInt(f);
        }

        private int _val
        {
            get
            {
                return this._._val;
            }
            set
            {
                this._._val = value;
            }
        }
        private byte _op
        {
            get
            {
                return this._._op;
            }
            set
            {
                this._._op = value;
            }
        }
        public TFlags value
        {
            get
            {
                return (TFlags) Enum.ToObject(typeof(TFlags), (int) (this._val & 0xffffff));
            }
            set
            {
                this._val = (this._val & 0x1000000) | (Vis.Op<TFlags>.ToInt(value) & 0xffffff);
            }
        }
        public int intvalue
        {
            get
            {
                return (this._val & 0x1000000);
            }
            set
            {
                this._val = (this._val & 0x1000000) | (value & 0xffffff);
            }
        }
        public Vis.Op op
        {
            get
            {
                return (((Vis.Op) this._op) & ((Vis.Op) 0x7f));
            }
            set
            {
                this._op = (byte) ((this._op & 0x80) | (((byte) value) & 0x7f));
            }
        }
        public int data
        {
            get
            {
                return this._val;
            }
            set
            {
                this._val = value;
            }
        }
        public override int GetHashCode()
        {
            return (this._val & 0x7fffffff);
        }

        public override string ToString()
        {
            return (this.op + ':' + this.value);
        }

        public override bool Equals(object obj)
        {
            if (obj is Vis.Op<TFlags>)
            {
                return this.Equals((Vis.Op<TFlags>) obj);
            }
            if (obj is Vis.Op)
            {
                return this.Equals((Vis.Op) ((int) obj));
            }
            return obj.Equals((Vis.Op<TFlags>) this);
        }

        public bool Equals(Vis.Op<TFlags> other)
        {
            return (other._val == this);
        }

        public bool Equals(Vis.Op other)
        {
            return (other == this.op);
        }

        public unsafe Res<TFlags> Eval(int flags)
        {
            return new Res<TFlags>(*((Vis.Op<TFlags>*) this), (TFlags) Enum.ToObject(typeof(TFlags), flags), flags);
        }

        public unsafe Res<TFlags> Eval(TFlags flags)
        {
            return new Res<TFlags>(*((Vis.Op<TFlags>*) this), flags, Vis.Op<TFlags>.ToInt(flags));
        }

        public static bool operator ==(Vis.Op<TFlags> op, TFlags flags)
        {
            return Vis.Evaluate(op.op, op._val & 0xffffff, Vis.Op<TFlags>.ToInt(flags));
        }

        public static bool operator ==(TFlags flags, Vis.Op<TFlags> op)
        {
            return Vis.Evaluate(op.op, op._val & 0xffffff, Vis.Op<TFlags>.ToInt(flags));
        }

        public static bool operator !=(Vis.Op<TFlags> op, TFlags flags)
        {
            return !Vis.Evaluate(op.op, op._val & 0xffffff, Vis.Op<TFlags>.ToInt(flags));
        }

        public static bool operator !=(TFlags flags, Vis.Op<TFlags> op)
        {
            return !Vis.Evaluate(op.op, op._val & 0xffffff, Vis.Op<TFlags>.ToInt(flags));
        }

        public static Res<TFlags> operator +(Vis.Op<TFlags> op, TFlags flags)
        {
            return op.Eval(flags);
        }

        public static Res<TFlags> operator +(Vis.Op<TFlags> op, int flags)
        {
            return op.Eval(flags);
        }

        public static Res<TFlags> operator -(Vis.Op<TFlags> op, TFlags flags)
        {
            return op.Eval(flags);
        }

        public static Res<TFlags> operator -(Vis.Op<TFlags> op, int flags)
        {
            return op.Eval(flags);
        }

        public static Vis.Op<TFlags> operator -(Vis.Op<TFlags> op)
        {
            op.op = Vis.Negate(op.op);
            return op;
        }

        public static bool operator ==(Vis.Op<TFlags> L, Vis.Op R)
        {
            return (L._op == ((sbyte) R));
        }

        public static bool operator ==(Vis.Op L, Vis.Op<TFlags> R)
        {
            return (R._op == ((sbyte) L));
        }

        public static bool operator !=(Vis.Op<TFlags> L, Vis.Op R)
        {
            return (L._op != ((sbyte) R));
        }

        public static bool operator !=(Vis.Op L, Vis.Op<TFlags> R)
        {
            return (R._op != ((sbyte) L));
        }

        public static bool operator ==(Vis.Op<TFlags> L, int R)
        {
            return (L._val == R);
        }

        public static bool operator ==(int R, Vis.Op<TFlags> L)
        {
            return (L._val == R);
        }

        public static bool operator !=(Vis.Op<TFlags> L, int R)
        {
            return (L._val != R);
        }

        public static bool operator !=(int R, Vis.Op<TFlags> L)
        {
            return (L._val != R);
        }

        public static bool operator ==(Vis.Op<TFlags> L, Vis.Op<TFlags> R)
        {
            return (L._val == R._val);
        }

        public static bool operator !=(Vis.Op<TFlags> L, Vis.Op<TFlags> R)
        {
            return (L._val != R._val);
        }

        public static implicit operator Vis.Op<TFlags>(int data)
        {
            return new Vis.Op<TFlags> { _val = data };
        }

        public static implicit operator int(Vis.Op<TFlags> op)
        {
            return op._val;
        }

        public static implicit operator Vis.Op(Vis.Op<TFlags> op)
        {
            return op.op;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct Res
        {
            public readonly TFlags query;
            private readonly Vis.Op<TFlags> _op;
            internal Res(Vis.Op<TFlags> op, TFlags flags, int flagsint)
            {
                this._op = op;
                this.query = flags;
                if (Vis.Evaluate(op.op, op.intvalue, flagsint))
                {
                    this._op._val |= -2147483648;
                }
                else
                {
                    this._op._val &= 0x7fffffff;
                }
            }

            public Vis.Op<TFlags> operation
            {
                get
                {
                    return this._op;
                }
            }
            public bool passed
            {
                get
                {
                    return ((this._op._val & -2147483648) == -2147483648);
                }
            }
            public bool failed
            {
                get
                {
                    return ((this._op._val & -2147483648) != -2147483648);
                }
            }
            public TFlags value
            {
                get
                {
                    return this._op.value;
                }
            }
            public int intvalue
            {
                get
                {
                    return this._op.intvalue;
                }
            }
            public int data
            {
                get
                {
                    return this._op._val;
                }
            }
            public override int GetHashCode()
            {
                return ((-2147483648 | this._op._val) ^ Vis.Op<TFlags>.ToInt(this.query));
            }

            public override string ToString()
            {
                return string.Format("{0}({1}) == {2}", this.operation, this.query, this.passed);
            }

            public static implicit operator bool(Vis.Op<TFlags>.Res r)
            {
                return r.passed;
            }

            public static bool op_LogicalNot(Vis.Op<TFlags>.Res r)
            {
                return r.failed;
            }
        }
    }

    [StructLayout(LayoutKind.Explicit, Size=4)]
    private struct OpBase
    {
        [FieldOffset(3)]
        public byte _op;
        [FieldOffset(0)]
        public int _val;

        public OpBase(byte _op, int _val)
        {
            this._val = _val;
            this._op = _op;
        }
    }

    public enum Region
    {
        Life,
        Status,
        Role
    }

    [Flags]
    public enum Role
    {
        Animal = 0x80,
        Authority = 4,
        Citizen = 1,
        Criminal = 2,
        Entourage = 0x10,
        Player = 0x20,
        Target = 8,
        Vehicle = 0x40
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Rule
    {
        public Setup setup;
        public Vis.Mask reject;
        public Vis.Mask accept;
        public Vis.Mask conditional;
        public Vis.Mask this[Step step]
        {
            get
            {
                switch (step)
                {
                    case Step.Accept:
                        return this.accept;

                    case Step.Conditional:
                        return this.conditional;

                    case Step.Reject:
                        return this.reject;
                }
                throw new ArgumentOutOfRangeException("step");
            }
            set
            {
                switch (step)
                {
                    case Step.Accept:
                        this.accept = value;
                        break;

                    case Step.Conditional:
                        this.conditional = value;
                        break;

                    case Step.Reject:
                        this.reject = value;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException("step");
                }
            }
        }
        public Vis.Op<Vis.Life> rejectLife
        {
            get
            {
                return new Vis.Op<Vis.Life>((byte) this.setup.life.reject, (int) this.reject.life);
            }
            set
            {
                RegionSetup life = this.setup.life;
                life.reject = value.op;
                this.setup.life = life;
                this.reject.life = value.value;
            }
        }
        public Vis.Op<Vis.Status> rejectStatus
        {
            get
            {
                return new Vis.Op<Vis.Status>((byte) this.setup.stat.reject, (int) this.reject.stat);
            }
            set
            {
                RegionSetup life = this.setup.life;
                life.reject = value.op;
                this.setup.life = life;
                this.reject.stat = value.value;
            }
        }
        public Vis.Op<Vis.Role> rejectRole
        {
            get
            {
                return new Vis.Op<Vis.Role>((byte) this.setup.role.reject, (int) this.reject.role);
            }
            set
            {
                RegionSetup life = this.setup.life;
                life.reject = value.op;
                this.setup.life = life;
                this.reject.role = value.value;
            }
        }
        public Vis.Op<Vis.Life> acceptLife
        {
            get
            {
                return new Vis.Op<Vis.Life>((byte) this.setup.life.accept, (int) this.accept.life);
            }
            set
            {
                RegionSetup life = this.setup.life;
                life.accept = value.op;
                this.setup.life = life;
                this.accept.life = value.value;
            }
        }
        public Vis.Op<Vis.Status> acceptStatus
        {
            get
            {
                return new Vis.Op<Vis.Status>((byte) this.setup.stat.accept, (int) this.accept.stat);
            }
            set
            {
                RegionSetup stat = this.setup.stat;
                stat.accept = value.op;
                this.setup.stat = stat;
                this.accept.stat = value.value;
            }
        }
        public Vis.Op<Vis.Role> acceptRole
        {
            get
            {
                return new Vis.Op<Vis.Role>((byte) this.setup.role.accept, (int) this.accept.role);
            }
            set
            {
                RegionSetup role = this.setup.role;
                role.accept = value.op;
                this.setup.role = role;
                this.accept.role = value.value;
            }
        }
        public Vis.Op<Vis.Life> conditionalLife
        {
            get
            {
                return new Vis.Op<Vis.Life>((byte) this.setup.life.conditional, (int) this.conditional.life);
            }
            set
            {
                RegionSetup life = this.setup.life;
                life.conditional = value.op;
                this.setup.life = life;
                this.conditional.life = value.value;
            }
        }
        public Vis.Op<Vis.Status> conditionalStatus
        {
            get
            {
                return new Vis.Op<Vis.Status>((byte) this.setup.stat.conditional, (int) this.conditional.stat);
            }
            set
            {
                RegionSetup stat = this.setup.stat;
                stat.conditional = value.op;
                this.setup.stat = stat;
                this.conditional.stat = value.value;
            }
        }
        public Vis.Op<Vis.Role> conditionalRole
        {
            get
            {
                return new Vis.Op<Vis.Role>((byte) this.setup.role.conditional, (int) this.conditional.role);
            }
            set
            {
                RegionSetup role = this.setup.role;
                role.conditional = value.op;
                this.setup.role = role;
                this.conditional.role = value.value;
            }
        }
        private Failure Accept(Vis.Mask mask)
        {
            if (!this.setup.checkAccept)
            {
                return Failure.None;
            }
            Failure none = Failure.None;
            if (!mask.Eval(this.acceptLife))
            {
                none |= Failure.Life | Failure.Conditional;
            }
            if (!mask.Eval(this.acceptRole))
            {
                none |= Failure.Role | Failure.Conditional;
            }
            if (!mask.Eval(this.acceptStatus))
            {
                none |= Failure.Status | Failure.Conditional;
            }
            return none;
        }

        private Failure Conditional(Vis.Mask mask)
        {
            if (!this.setup.checkConditional)
            {
                return Failure.None;
            }
            Failure none = Failure.None;
            if (!mask.Eval(this.conditionalLife))
            {
                none |= Failure.Life | Failure.Conditional;
            }
            if (!mask.Eval(this.conditionalRole))
            {
                none |= Failure.Role | Failure.Conditional;
            }
            if (!mask.Eval(this.conditionalStatus))
            {
                none |= Failure.Status | Failure.Conditional;
            }
            return none;
        }

        private Failure Reject(Vis.Mask mask)
        {
            if (!this.setup.checkReject)
            {
                return Failure.None;
            }
            Failure none = Failure.None;
            if (mask.Eval(this.rejectLife))
            {
                none |= Failure.Life | Failure.Reject;
            }
            if (mask.Eval(this.rejectRole))
            {
                none |= Failure.Role | Failure.Reject;
            }
            if (mask.Eval(this.rejectStatus))
            {
                none |= Failure.Status | Failure.Reject;
            }
            return none;
        }

        private Failure Check(Vis.Mask a, Vis.Mask c, Vis.Mask r)
        {
            Failure failure = this.Accept(a);
            if (failure != Failure.None)
            {
                return failure;
            }
            failure = this.Conditional(c);
            if (failure != Failure.None)
            {
                return failure;
            }
            return this.Reject(r);
        }

        public Failure Pass(Vis.Mask self, Vis.Mask other)
        {
            switch (this.setup.option)
            {
                case Setup.Option.Inverse:
                    return this.Check(self, other, self);

                case Setup.Option.NoConditional:
                    return this.Check(other, other, other);

                case Setup.Option.AllConditional:
                    return this.Check(self, self, self);
            }
            return this.Check(other, self, other);
        }

        public static Vis.Rule Decode(int[] data, int index)
        {
            Vis.Rule rule = new Vis.Rule();
            rule.setup.data = data[index++];
            rule.accept.data = data[index++];
            rule.conditional.data = data[index++];
            rule.reject.data = data[index];
            return rule;
        }

        public static void Encode(ref Vis.Rule rule, int[] data, int index)
        {
            data[index++] = rule.setup.data;
            data[index++] = rule.accept.data;
            data[index++] = rule.conditional.data;
            data[index++] = rule.reject.data;
        }
        public enum Clearance
        {
            Outside,
            Enter,
            Stay,
            Exit
        }

        [Flags]
        public enum Failure
        {
            Accept = 1,
            Conditional = 2,
            Life = 8,
            None = 0,
            Reject = 4,
            Role = 0x10,
            Status = 0x20
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RegionSetup
        {
            private static readonly BitVector32.Section s_apt;
            private static readonly BitVector32.Section s_cnd;
            private static readonly BitVector32.Section s_rej;
            private static readonly BitVector32.Section s_whole;
            private static readonly BitVector32.Section s_region;
            private BitVector32 _;
            internal RegionSetup(int value, Vis.Region reg)
            {
                this._ = new BitVector32(value | (((int) reg) << s_region.Offset));
            }

            static RegionSetup()
            {
                BitVector32.Section previous = s_apt = BitVector32.CreateSection(7);
                previous = s_cnd = BitVector32.CreateSection(7, previous);
                previous = s_rej = BitVector32.CreateSection(7, previous);
                s_whole = BitVector32.CreateSection(0x1ff);
                previous = BitVector32.CreateSection(7, previous);
                previous = s_region = BitVector32.CreateSection(3, previous);
            }

            public Vis.Op accept
            {
                get
                {
                    return (Vis.Op) this._[s_apt];
                }
                set
                {
                    this._[s_apt] = (int) value;
                }
            }
            public Vis.Op conditional
            {
                get
                {
                    return (Vis.Op) this._[s_cnd];
                }
                set
                {
                    this._[s_cnd] = (int) value;
                }
            }
            public Vis.Op reject
            {
                get
                {
                    return (Vis.Op) this._[s_rej];
                }
                set
                {
                    this._[s_rej] = (int) value;
                }
            }
            public Vis.Region region
            {
                get
                {
                    return (Vis.Region) this._[s_region];
                }
                set
                {
                    this._[s_region] = (int) value;
                }
            }
            public Vis.Op this[Vis.Rule.Step step]
            {
                get
                {
                    switch (step)
                    {
                        case Vis.Rule.Step.Accept:
                            return this.accept;

                        case Vis.Rule.Step.Conditional:
                            return this.conditional;

                        case Vis.Rule.Step.Reject:
                            return this.reject;
                    }
                    throw new ArgumentOutOfRangeException("step");
                }
                set
                {
                    switch (step)
                    {
                        case Vis.Rule.Step.Accept:
                            this.accept = value;
                            break;

                        case Vis.Rule.Step.Conditional:
                            this.conditional = value;
                            break;

                        case Vis.Rule.Step.Reject:
                            this.reject = value;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException("step");
                    }
                }
            }
            internal int dat
            {
                get
                {
                    return this._[s_whole];
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Setup
        {
            private static readonly BitVector32.Section s_life;
            private static readonly BitVector32.Section[] s_life_;
            private static readonly BitVector32.Section s_stat;
            private static readonly BitVector32.Section[] s_stat_;
            private static readonly BitVector32.Section s_role;
            private static readonly BitVector32.Section[] s_role_;
            private static readonly BitVector32.Section s_options;
            private static readonly BitVector32.Section s_check;
            private static readonly BitVector32.Section[] s_check_;
            private BitVector32 _;
            static Setup()
            {
                BitVector32.Section previous = s_life = BitVector32.CreateSection(0x1ff);
                previous = s_stat = BitVector32.CreateSection(0x1ff, previous);
                previous = s_role = BitVector32.CreateSection(0x1ff, previous);
                previous = s_options = BitVector32.CreateSection(3, previous);
                previous = s_check = BitVector32.CreateSection(7, previous);
                s_life_ = new BitVector32.Section[3];
                s_stat_ = new BitVector32.Section[3];
                s_role_ = new BitVector32.Section[3];
                s_check_ = new BitVector32.Section[3];
                int index = 0;
                s_life_[index] = BitVector32.CreateSection(7);
                s_stat_[index] = BitVector32.CreateSection(7, s_life);
                s_role_[index] = BitVector32.CreateSection(7, s_stat);
                s_check_[index] = BitVector32.CreateSection(1, s_options);
                index++;
                while (index < 3)
                {
                    s_life_[index] = BitVector32.CreateSection(7, s_life_[index - 1]);
                    s_stat_[index] = BitVector32.CreateSection(7, s_stat_[index - 1]);
                    s_role_[index] = BitVector32.CreateSection(7, s_role_[index - 1]);
                    s_check_[index] = BitVector32.CreateSection(1, s_check_[index - 1]);
                    index++;
                }
            }

            public Vis.Rule.RegionSetup life
            {
                get
                {
                    return new Vis.Rule.RegionSetup(this._[s_life], Vis.Region.Life);
                }
                set
                {
                    this._[s_life] = value.dat;
                }
            }
            public Vis.Rule.RegionSetup stat
            {
                get
                {
                    return new Vis.Rule.RegionSetup(this._[s_stat], Vis.Region.Status);
                }
                set
                {
                    this._[s_stat] = value.dat;
                }
            }
            public Vis.Rule.RegionSetup role
            {
                get
                {
                    return new Vis.Rule.RegionSetup(this._[s_role], Vis.Region.Role);
                }
                set
                {
                    this._[s_role] = value.dat;
                }
            }
            private Vis.Rule.StepSetup Get(int i)
            {
                return new Vis.Rule.StepSetup(this._[s_life_[i]], this._[s_stat_[i]], this._[s_role_[i]], i, this._[s_check_[i]]);
            }

            private void Set(int i, Vis.Rule.StepSetup step)
            {
                this._[s_life_[i]] = (int) (step.life & ((Vis.Op) s_life_[i].Mask));
                this._[s_stat_[i]] = (int) (step.stat & ((Vis.Op) s_stat_[i].Mask));
                this._[s_role_[i]] = (int) (step.role & ((Vis.Op) s_role_[i].Mask));
                this._[s_check_[i]] = !step.enabled ? 0 : 1;
            }

            public Vis.Rule.StepSetup accept
            {
                get
                {
                    return this.Get(0);
                }
                set
                {
                    this.Set(0, value);
                }
            }
            public Vis.Rule.StepSetup conditional
            {
                get
                {
                    return this.Get(1);
                }
                set
                {
                    this.Set(1, value);
                }
            }
            public Vis.Rule.StepSetup reject
            {
                get
                {
                    return this.Get(2);
                }
                set
                {
                    this.Set(2, value);
                }
            }
            public Vis.Rule.RegionSetup this[Vis.Region region]
            {
                get
                {
                    switch (region)
                    {
                        case Vis.Region.Life:
                            return this.life;

                        case Vis.Region.Status:
                            return this.stat;

                        case Vis.Region.Role:
                            return this.role;
                    }
                    throw new ArgumentOutOfRangeException("region");
                }
                set
                {
                    switch (region)
                    {
                        case Vis.Region.Life:
                            this.life = value;
                            break;

                        case Vis.Region.Status:
                            this.stat = value;
                            break;

                        case Vis.Region.Role:
                            this.role = value;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException("region");
                    }
                }
            }
            public Vis.Rule.StepSetup this[Vis.Rule.Step step]
            {
                get
                {
                    switch (step)
                    {
                        case Vis.Rule.Step.Accept:
                            return this.accept;

                        case Vis.Rule.Step.Conditional:
                            return this.conditional;

                        case Vis.Rule.Step.Reject:
                            return this.reject;
                    }
                    throw new ArgumentOutOfRangeException("step");
                }
                set
                {
                    switch (step)
                    {
                        case Vis.Rule.Step.Accept:
                            this.accept = value;
                            break;

                        case Vis.Rule.Step.Conditional:
                            this.conditional = value;
                            break;

                        case Vis.Rule.Step.Reject:
                            this.reject = value;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException("step");
                    }
                }
            }
            public Option option
            {
                get
                {
                    return (Option) this._[s_options];
                }
                set
                {
                    this._[s_options] = (int) value;
                }
            }
            public Vis.Rule.StepCheck check
            {
                get
                {
                    return new Vis.Rule.StepCheck(this._[s_check]);
                }
                set
                {
                    this._[s_check] = value.value;
                }
            }
            public bool checkAccept
            {
                get
                {
                    return (this._[s_check_[0]] != 0);
                }
                set
                {
                    this._[s_check_[0]] = !value ? 0 : 1;
                }
            }
            public bool checkConditional
            {
                get
                {
                    return (this._[s_check_[1]] != 0);
                }
                set
                {
                    this._[s_check_[1]] = !value ? 0 : 1;
                }
            }
            public bool checkReject
            {
                get
                {
                    return (this._[s_check_[2]] != 0);
                }
                set
                {
                    this._[s_check_[2]] = !value ? 0 : 1;
                }
            }
            public void SetSetup(Vis.Rule.RegionSetup operations)
            {
                this[operations.region] = operations;
            }

            public void SetSetup(Vis.Rule.StepSetup operations)
            {
                this[operations.step] = operations;
            }

            internal int data
            {
                get
                {
                    return this._.Data;
                }
                set
                {
                    this._ = new BitVector32(value);
                }
            }
            public enum Option
            {
                Default,
                Inverse,
                NoConditional,
                AllConditional
            }
        }

        public enum Step
        {
            Accept,
            Conditional,
            Reject
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct StepCheck
        {
            private byte bits;
            internal StepCheck(int i)
            {
                this.bits = (byte) i;
            }

            public bool accept
            {
                get
                {
                    return ((this.bits & 1) == 1);
                }
                set
                {
                    this.bits = !value ? ((byte) (this.bits & 6)) : ((byte) (this.bits | 1));
                }
            }
            public bool conditional
            {
                get
                {
                    return ((this.bits & 2) == 2);
                }
                set
                {
                    this.bits = !value ? ((byte) (this.bits & 5)) : ((byte) (this.bits | 2));
                }
            }
            public bool reject
            {
                get
                {
                    return ((this.bits & 4) == 4);
                }
                set
                {
                    this.bits = !value ? ((byte) (this.bits & 3)) : ((byte) (this.bits | 4));
                }
            }
            internal int value
            {
                get
                {
                    return this.bits;
                }
            }
            public bool this[Vis.Rule.Step step]
            {
                get
                {
                    switch (step)
                    {
                        case Vis.Rule.Step.Accept:
                            return this.accept;

                        case Vis.Rule.Step.Conditional:
                            return this.conditional;

                        case Vis.Rule.Step.Reject:
                            return this.reject;
                    }
                    throw new ArgumentOutOfRangeException("step");
                }
                set
                {
                    switch (step)
                    {
                        case Vis.Rule.Step.Accept:
                            this.accept = value;
                            break;

                        case Vis.Rule.Step.Conditional:
                            this.conditional = value;
                            break;

                        case Vis.Rule.Step.Reject:
                            this.reject = value;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException("step");
                    }
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct StepSetup
        {
            private static readonly BitVector32.Section s_life;
            private static readonly BitVector32.Section s_stat;
            private static readonly BitVector32.Section s_role;
            private static readonly BitVector32.Section s_step;
            private static readonly BitVector32.Section s_enable;
            private BitVector32 _;
            internal StepSetup(int life, int stat, int role, int step, int enable)
            {
                this = new Vis.Rule.StepSetup();
                this._[s_life] = life;
                this._[s_stat] = stat;
                this._[s_role] = role;
                this._[s_step] = step;
                this._[s_enable] = enable;
            }

            static StepSetup()
            {
                BitVector32.Section previous = s_life = BitVector32.CreateSection(7);
                previous = s_step = BitVector32.CreateSection(0xff, previous);
                previous = s_enable = BitVector32.CreateSection(1, previous);
                previous = s_stat = BitVector32.CreateSection(7, previous);
                previous = BitVector32.CreateSection(0x1ff, previous);
                previous = s_role = BitVector32.CreateSection(7, previous);
            }

            public Vis.Op life
            {
                get
                {
                    return (Vis.Op) this._[s_life];
                }
                set
                {
                    this._[s_life] = (int) value;
                }
            }
            public Vis.Op stat
            {
                get
                {
                    return (Vis.Op) this._[s_stat];
                }
                set
                {
                    this._[s_stat] = (int) value;
                }
            }
            public Vis.Op role
            {
                get
                {
                    return (Vis.Op) this._[s_role];
                }
                set
                {
                    this._[s_role] = (int) value;
                }
            }
            public Vis.Rule.Step step
            {
                get
                {
                    return (Vis.Rule.Step) this._[s_step];
                }
                set
                {
                    this._[s_step] = (int) value;
                }
            }
            public bool enabled
            {
                get
                {
                    return (this._[s_enable] != 0);
                }
                set
                {
                    this._[s_enable] = !value ? 0 : 1;
                }
            }
            public Vis.Op this[Vis.Region region]
            {
                get
                {
                    switch (region)
                    {
                        case Vis.Region.Life:
                            return this.life;

                        case Vis.Region.Status:
                            return this.stat;

                        case Vis.Region.Role:
                            return this.role;
                    }
                    throw new ArgumentOutOfRangeException("region");
                }
                set
                {
                    switch (region)
                    {
                        case Vis.Region.Life:
                            this.life = value;
                            break;

                        case Vis.Region.Status:
                            this.stat = value;
                            break;

                        case Vis.Region.Role:
                            this.role = value;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException("region");
                    }
                }
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Stamp
    {
        public Vector3 position;
        public Vector4 plane;
        public Quaternion rotation;
        public Stamp(Transform transform)
        {
            this.position = transform.position;
            this.rotation = transform.rotation;
            Vector3 forward = transform.forward;
            this.plane.x = forward.x;
            this.plane.y = forward.y;
            this.plane.z = forward.z;
            this.plane.w = ((this.position.x * this.plane.x) + (this.position.y * this.plane.y)) + (this.position.z * this.plane.z);
        }

        public Vector3 forward
        {
            get
            {
                return new Vector3(this.plane.x, this.plane.y, this.plane.z);
            }
        }
        public void Collect(Transform transform)
        {
            this.position = transform.position;
            this.rotation = transform.rotation;
            Vector3 forward = transform.forward;
            this.plane.x = forward.x;
            this.plane.y = forward.y;
            this.plane.z = forward.z;
            this.plane.w = ((this.position.x * this.forward.x) + (this.position.y * this.forward.y)) + (this.position.z * this.forward.z);
        }
    }

    [Flags]
    public enum Status
    {
        Alert = 8,
        Armed = 0x20,
        Attacking = 0x40,
        Casual = 1,
        Curious = 4,
        Hurt = 2,
        Search = 0x10
    }

    public enum Trait
    {
        Alert = 11,
        Alive = 0,
        Animal = 0x1f,
        Armed = 13,
        Attacking = 14,
        Authority = 0x1a,
        Casual = 8,
        Citizen = 0x18,
        Criminal = 0x19,
        Curious = 10,
        Dead = 2,
        Entourage = 0x1c,
        Hurt = 9,
        Player = 0x1d,
        Search = 12,
        Target = 0x1b,
        Unconcious = 1,
        Vehicle = 30
    }
}

