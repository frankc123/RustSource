using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEngine;

public abstract class CCTotem : MonoBehaviour
{
    protected internal const CollisionFlags kCF_Above = CollisionFlags.Above;
    protected internal const CollisionFlags kCF_Below = CollisionFlags.Below;
    protected internal const CollisionFlags kCF_None = CollisionFlags.None;
    protected internal const CollisionFlags kCF_Sides = CollisionFlags.Sides;
    protected internal const int kMaxTotemicFiguresPerTotemPole = 8;

    internal CCTotem()
    {
    }

    private static void DestroyCCDesc(CCTotemPole ScriptOwner, ref CCDesc CCDesc)
    {
        if (ScriptOwner != null)
        {
            ScriptOwner.DestroyCCDesc(ref CCDesc);
        }
        else
        {
            CCDesc desc = CCDesc;
            CCDesc = null;
            if (desc != null)
            {
                Object.Destroy(desc.gameObject);
            }
        }
    }

    private static string VS(Vector3 v)
    {
        return string.Format("[{0},{1},{2}]", v.x, v.y, v.z);
    }

    internal abstract TotemicObject _Object { get; }

    protected internal TotemicObject totemicObject
    {
        get
        {
            return this._Object;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal protected struct Configuration
    {
        public readonly CCTotem.Initialization totem;
        public readonly float totemMinHeight;
        public readonly float totemMaxHeight;
        public readonly float totemBottomBufferUnits;
        public readonly float figureSkinWidth;
        public readonly float figure2SkinWidth;
        public readonly float figureRadius;
        public readonly float figureSkinnedRadius;
        public readonly float figureDiameter;
        public readonly float figureSkinnedDiameter;
        public readonly float figureHeight;
        public readonly float figureSkinnedHeight;
        public readonly float figureSlideHeight;
        public readonly float figureFixedHeight;
        public readonly float poleTopBufferAmount;
        public readonly float poleBottomBufferAmount;
        public readonly float poleBottomBufferHeight;
        public readonly float poleBottomBufferUnitSize;
        public readonly float poleMostContractedHeightPossible;
        public readonly float poleMostExpandedHeightPossible;
        public readonly float poleContractedHeight;
        public readonly float poleContractedHeightFromMostContractedHeightPossible;
        public readonly float poleExpandedHeight;
        public readonly float poleExpandedHeightFromMostContractedHeightPossible;
        public readonly float poleFixedLength;
        public readonly float poleExpansionLength;
        public readonly int numRequiredTotemicFigures;
        public readonly int numSlidingTotemicFigures;
        public readonly Vector3 figureOriginOffsetBottom;
        public readonly Vector3 figureOriginOffsetTop;
        public readonly Vector3 figureOriginOffsetCenter;
        public Configuration(ref CCTotem.Initialization totem)
        {
            if (totem.figurePrefab == null)
            {
                throw new ArgumentException("figurePrefab was missing", "totem");
            }
            this.totem = totem;
            this.totemMinHeight = totem.minHeight;
            this.totemMaxHeight = totem.maxHeight;
            this.totemBottomBufferUnits = totem.bottomBufferUnits;
            if (this.totemMinHeight >= this.totemMaxHeight)
            {
                throw new ArgumentException("maxHeight is less than or equal to minHeight", "totem");
            }
            if (Mathf.Approximately(this.totemBottomBufferUnits, 0f))
            {
                this.totemBottomBufferUnits = 0f;
            }
            else if (this.totemBottomBufferUnits < 0f)
            {
                throw new ArgumentException("bottomBufferPercent must not be less than zero", "totem");
            }
            CCDesc figurePrefab = totem.figurePrefab;
            this.figureSkinWidth = figurePrefab.skinWidth;
            this.figure2SkinWidth = this.figureSkinWidth + this.figureSkinWidth;
            this.figureRadius = figurePrefab.radius;
            this.figureSkinnedRadius = this.figureRadius + this.figureSkinWidth;
            this.figureDiameter = this.figureRadius + this.figureRadius;
            this.figureSkinnedDiameter = this.figureSkinnedRadius + this.figureSkinnedRadius;
            this.figureHeight = figurePrefab.height;
            if (this.figureHeight <= this.figureDiameter)
            {
                throw new ArgumentException("The CCDesc(CharacterController) Prefab is a sphere, not a capsule. Thus cannot be expanded on the totem pole", "totem");
            }
            this.figureSkinnedHeight = this.figureHeight + this.figure2SkinWidth;
            if ((this.figureSkinnedHeight > this.totemMinHeight) && !Mathf.Approximately(this.totemMinHeight, this.figureSkinnedHeight))
            {
                throw new ArgumentException("minHeight is too small. It must be at least the size of the CCDesc(CharacterController) prefab's [height+(skinWidth*2)]", "totem");
            }
            this.figureSlideHeight = this.figureSkinnedHeight - this.figureSkinnedDiameter;
            if (this.figureSlideHeight <= 0f)
            {
                throw new ArgumentException("The CCDesc(CharacterController) Prefab has limited height availability. Thus cannot be expanded on the totem pole", "totem");
            }
            this.figureFixedHeight = this.figureSkinnedHeight - this.figureSlideHeight;
            this.poleTopBufferAmount = this.figureSkinnedRadius;
            this.poleBottomBufferUnitSize = this.figureSlideHeight * 0.5f;
            this.poleBottomBufferAmount = this.poleBottomBufferUnitSize * this.totemBottomBufferUnits;
            if (this.poleBottomBufferAmount > this.figureSlideHeight)
            {
                if (!Mathf.Approximately(this.poleBottomBufferAmount, this.figureSlideHeight))
                {
                    throw new ArgumentException("The bottomBuffer was too large and landed outside of sliding height area of the capsule", "totem");
                }
                this.poleBottomBufferAmount = this.figureSlideHeight;
                this.totemBottomBufferUnits = this.figureSlideHeight / this.poleBottomBufferUnitSize;
            }
            this.poleBottomBufferHeight = this.figureSkinnedRadius + this.poleBottomBufferAmount;
            this.poleMostContractedHeightPossible = this.figureSkinnedHeight + this.poleBottomBufferAmount;
            if (this.poleMostContractedHeightPossible > this.totemMinHeight)
            {
                if (!Mathf.Approximately(this.poleMostContractedHeightPossible, this.totemMinHeight))
                {
                    throw new ArgumentException("bottomBufferPercent value is too high with the current setup, results in contracted height greater than totem.minHeight.", "totem");
                }
                this.totemMinHeight = this.poleMostContractedHeightPossible;
            }
            this.poleContractedHeight = Mathf.Max(this.poleMostContractedHeightPossible, this.totemMinHeight);
            this.poleContractedHeightFromMostContractedHeightPossible = this.poleContractedHeight - this.poleMostContractedHeightPossible;
            this.poleExpandedHeight = Mathf.Max(this.poleContractedHeight, this.totemMaxHeight);
            this.poleExpandedHeightFromMostContractedHeightPossible = this.poleExpandedHeight - this.poleMostContractedHeightPossible;
            if (Mathf.Approximately(this.poleContractedHeightFromMostContractedHeightPossible, this.poleExpandedHeightFromMostContractedHeightPossible))
            {
                throw new ArgumentException("minHeight and maxHeight were too close to eachother to provide reliable contraction/expansion calculations.", "totem");
            }
            if ((this.poleContractedHeightFromMostContractedHeightPossible < 0f) || (this.poleExpandedHeightFromMostContractedHeightPossible < this.poleContractedHeightFromMostContractedHeightPossible))
            {
                throw new ArgumentException("Calculation error with current configuration.", "totem");
            }
            this.poleFixedLength = this.poleBottomBufferHeight + this.poleTopBufferAmount;
            this.poleExpansionLength = this.poleExpandedHeight - this.poleFixedLength;
            this.numSlidingTotemicFigures = Mathf.CeilToInt(this.poleExpansionLength / this.figureSlideHeight);
            if (this.numSlidingTotemicFigures < 1)
            {
                throw new ArgumentException("The current configuration of the CCTotem resulted in no need for more than one CCDesc(CharacterController), thus rejecting usage..", "totem");
            }
            this.poleMostExpandedHeightPossible = this.poleFixedLength + (this.numSlidingTotemicFigures * this.figureSlideHeight);
            this.numRequiredTotemicFigures = 1 + this.numSlidingTotemicFigures;
            if (this.numRequiredTotemicFigures > 8)
            {
                throw new ArgumentOutOfRangeException("totem", this.numRequiredTotemicFigures, "The current configuration of the CCTotem resulted in more than the max number of TotemicFigure's allowed :" + 8);
            }
            Vector3 center = figurePrefab.center;
            this.figureOriginOffsetCenter = new Vector3(0f - center.x, 0f - center.y, 0f - center.z);
            this.figureOriginOffsetBottom = new Vector3(this.figureOriginOffsetCenter.x, 0f - (center.y - (this.figureSkinnedHeight / 2f)), this.figureOriginOffsetCenter.z);
            this.figureOriginOffsetTop = new Vector3(this.figureOriginOffsetCenter.x, 0f - (center.y + (this.figureSkinnedHeight / 2f)), this.figureOriginOffsetCenter.z);
        }

        public override string ToString()
        {
            return CCTotem.ToStringHelper<CCTotem.Configuration>.GetString(this);
        }
    }

    public delegate void ConfigurationBinder(bool Bind, CCDesc CCDesc, object Tag);

    [StructLayout(LayoutKind.Sequential)]
    internal protected struct Contraction
    {
        public readonly float Contracted;
        public readonly float Expanded;
        public readonly float Range;
        public readonly float InverseRange;
        private Contraction(float Contracted, float Expanded, float Range, float InverseRange)
        {
            this.Contracted = Contracted;
            this.Expanded = Expanded;
            this.Range = Range;
            this.InverseRange = InverseRange;
        }

        public CCTotem.Expansion ExpansionForValue(float Value)
        {
            if (Value <= this.Contracted)
            {
                return new CCTotem.Expansion(this.Contracted, 0f, 0f);
            }
            if (Value >= this.Expanded)
            {
                return new CCTotem.Expansion(this.Expanded, 1f, this.Range);
            }
            float amount = Value - this.Contracted;
            return new CCTotem.Expansion(Value, amount / this.Range, amount);
        }

        public CCTotem.Expansion ExpansionForFraction(float FractionExpanded)
        {
            if (FractionExpanded <= 0f)
            {
                return new CCTotem.Expansion(this.Contracted, 0f, 0f);
            }
            if (FractionExpanded >= 1f)
            {
                return new CCTotem.Expansion(this.Expanded, 1f, this.Range);
            }
            float amount = FractionExpanded * this.Range;
            return new CCTotem.Expansion(this.Contracted + amount, FractionExpanded, amount);
        }

        public CCTotem.Expansion ExpansionForAmount(float Amount)
        {
            if (Amount <= 0f)
            {
                return new CCTotem.Expansion(this.Contracted, 0f, 0f);
            }
            if (Amount >= this.Range)
            {
                return new CCTotem.Expansion(this.Expanded, 1f, this.Range);
            }
            float fractionExpanded = Amount / this.Range;
            return new CCTotem.Expansion(this.Contracted + Amount, fractionExpanded, Amount);
        }

        public override string ToString()
        {
            object[] args = new object[] { this.Contracted, this.Expanded, this.Range, this.InverseRange };
            return string.Format("{{Contracted={0},Expanded={1},Range={2},InverseRange={3}}}", args);
        }

        public static CCTotem.Contraction Define(float Contracted, float Expanded)
        {
            if (Mathf.Approximately(Contracted, Expanded))
            {
                throw new ArgumentOutOfRangeException("Contracted", "approximately equal to Expanded");
            }
            float range = Expanded - Contracted;
            return new CCTotem.Contraction(Contracted, Expanded, range, (float) (1.0 / ((double) range)));
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal protected struct Direction
    {
        public readonly bool Exists;
        public readonly CCTotem.TotemicFigure TotemicFigure;
        public Direction(CCTotem.TotemicFigure TotemicFigure)
        {
            if (object.ReferenceEquals(TotemicFigure, null))
            {
                throw new ArgumentNullException("TotemicFigure");
            }
            this.TotemicFigure = TotemicFigure;
            this.Exists = true;
        }

        public static CCTotem.Direction None
        {
            get
            {
                return new CCTotem.Direction();
            }
        }
        public override string ToString()
        {
            return (!this.Exists ? "{Does Not Exist}" : string.Format("{{TotemicFigure={0}}}", this.TotemicFigure));
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal protected struct Ends<T>
    {
        public T Bottom;
        public T Top;
        public Ends(T Bottom, T Top)
        {
            this.Bottom = Bottom;
            this.Top = Top;
        }

        public override string ToString()
        {
            return string.Format("{{Bottom={0},Top={1}}}", this.Bottom, this.Top);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal protected struct Expansion
    {
        public readonly float Value;
        public readonly float FractionExpanded;
        public readonly float Amount;
        internal Expansion(float Value, float FractionExpanded, float Amount)
        {
            this.Value = Value;
            this.FractionExpanded = FractionExpanded;
            this.Amount = Amount;
        }

        public override string ToString()
        {
            return string.Format("{{Value={0},FractionExpanded={1},Amount={2}}}", this.Value, this.FractionExpanded, this.Amount);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal protected struct Initialization
    {
        public readonly CCTotemPole totemPole;
        public readonly CCDesc figurePrefab;
        public readonly float minHeight;
        public readonly float maxHeight;
        public readonly float initialHeight;
        public readonly float bottomBufferUnits;
        public readonly bool nonDefault;
        public Initialization(CCTotemPole totemPole, CCDesc figurePrefab, float minHeight, float maxHeight, float initialHeight, float bottomBufferUnits)
        {
            this.totemPole = totemPole;
            this.figurePrefab = figurePrefab;
            this.minHeight = minHeight;
            this.maxHeight = maxHeight;
            this.initialHeight = initialHeight;
            this.bottomBufferUnits = bottomBufferUnits;
            this.nonDefault = true;
        }

        public override string ToString()
        {
            return CCTotem.ToStringHelper<CCTotem.Initialization>.GetString(this);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MoveInfo
    {
        public readonly UnityEngine.CollisionFlags CollisionFlags;
        public readonly UnityEngine.CollisionFlags WorkingCollisionFlags;
        public readonly float WantedHeight;
        public readonly Vector3 BottomMovement;
        public readonly Vector3 TopMovement;
        public readonly CCTotem.PositionPlacement PositionPlacement;
        public MoveInfo(UnityEngine.CollisionFlags CollisionFlags, UnityEngine.CollisionFlags WorkingCollisionFlags, float WantedHeight, Vector3 BottomMovement, Vector3 TopMovement, CCTotem.PositionPlacement PositionPlacement)
        {
            this.CollisionFlags = CollisionFlags;
            this.WorkingCollisionFlags = WorkingCollisionFlags;
            this.WantedHeight = WantedHeight;
            this.BottomMovement = BottomMovement;
            this.TopMovement = TopMovement;
            this.PositionPlacement = PositionPlacement;
        }
    }

    public delegate void PositionBinder(ref CCTotem.PositionPlacement binding, object Tag);

    [StructLayout(LayoutKind.Sequential)]
    public struct PositionPlacement
    {
        public Vector3 bottom;
        public Vector3 top;
        public Vector3 colliderCenter;
        public float height;
        public float originalHeight;
        public Vector3 originalTop;
        public PositionPlacement(Vector3 Bottom, Vector3 Top, Vector3 ColliderPosition, float OriginalHeight)
        {
            this.bottom = Bottom;
            this.top = Top;
            this.colliderCenter = ColliderPosition;
            this.height = Top.y - Bottom.y;
            this.originalHeight = OriginalHeight;
            this.originalTop.x = Bottom.x;
            this.originalTop.y = Bottom.y + OriginalHeight;
            this.originalTop.z = Bottom.z;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal protected struct Route
    {
        public readonly CCTotem.Direction Up;
        public readonly CCTotem.Direction At;
        public readonly CCTotem.Direction Down;
        public Route(CCTotem.Direction Up, CCTotem.Direction At, CCTotem.Direction Down)
        {
            this.Up = Up;
            this.At = At;
            this.Down = Down;
        }

        public bool GetUp(out CCTotem.Route route)
        {
            if (this.Up.Exists)
            {
                route = (CCTotem.Route) this.Up.TotemicFigure[0];
                return true;
            }
            route = new CCTotem.Route(CCTotem.Direction.None, CCTotem.Direction.None, this.At);
            return false;
        }

        public bool GetDown(out CCTotem.Route route)
        {
            if (this.Down.Exists)
            {
                route = (CCTotem.Route) this.Down.TotemicFigure[0];
                return true;
            }
            route = new CCTotem.Route(this.At, CCTotem.Direction.None, CCTotem.Direction.None);
            return false;
        }

        public IEnumerable<CCTotem.TotemicFigure> EnumerateUpInclusive
        {
            get
            {
                return new <>c__Iterator27 { <>f__this = this, $PC = -2 };
            }
        }
        public IEnumerable<CCTotem.TotemicFigure> EnumerateUp
        {
            get
            {
                return new <>c__Iterator28 { <>f__this = this, $PC = -2 };
            }
        }
        public IEnumerable<CCTotem.TotemicFigure> EnumerateDownInclusive
        {
            get
            {
                return new <>c__Iterator29 { <>f__this = this, $PC = -2 };
            }
        }
        public IEnumerable<CCTotem.TotemicFigure> EnumerateDown
        {
            get
            {
                return new <>c__Iterator2A { <>f__this = this, $PC = -2 };
            }
        }
        public override string ToString()
        {
            return string.Format("{{Up={0},At={1},Down={2}}}", this.Up, this.At, this.Down);
        }
        [CompilerGenerated]
        private sealed class <>c__Iterator27 : IDisposable, IEnumerator, IEnumerable, IEnumerable<CCTotem.TotemicFigure>, IEnumerator<CCTotem.TotemicFigure>
        {
            internal CCTotem.TotemicFigure $current;
            internal int $PC;
            internal CCTotem.Route <>f__this;
            internal CCTotem.Route <it>__0;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.<it>__0 = this.<>f__this;
                        break;

                    case 1:
                        this.<it>__0.GetUp(out this.<it>__0);
                        break;

                    default:
                        goto Label_0088;
                }
                if (this.<it>__0.At.Exists)
                {
                    this.$current = this.<it>__0.At.TotemicFigure;
                    this.$PC = 1;
                    return true;
                }
                this.$PC = -1;
            Label_0088:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<CCTotem.TotemicFigure> IEnumerable<CCTotem.TotemicFigure>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new CCTotem.Route.<>c__Iterator27 { <>f__this = this.<>f__this };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.System.Collections.Generic.IEnumerable<CCTotem.TotemicFigure>.GetEnumerator();
            }

            CCTotem.TotemicFigure IEnumerator<CCTotem.TotemicFigure>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <>c__Iterator28 : IDisposable, IEnumerator, IEnumerable, IEnumerable<CCTotem.TotemicFigure>, IEnumerator<CCTotem.TotemicFigure>
        {
            internal CCTotem.TotemicFigure $current;
            internal int $PC;
            internal CCTotem.Route <>f__this;
            internal CCTotem.Route <it>__0;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        if (!this.<>f__this.GetUp(out this.<it>__0))
                        {
                            goto Label_008B;
                        }
                        break;

                    case 1:
                        this.<it>__0.GetUp(out this.<it>__0);
                        break;

                    default:
                        goto Label_0092;
                }
                if (this.<it>__0.At.Exists)
                {
                    this.$current = this.<it>__0.At.TotemicFigure;
                    this.$PC = 1;
                    return true;
                }
            Label_008B:
                this.$PC = -1;
            Label_0092:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<CCTotem.TotemicFigure> IEnumerable<CCTotem.TotemicFigure>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new CCTotem.Route.<>c__Iterator28 { <>f__this = this.<>f__this };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.System.Collections.Generic.IEnumerable<CCTotem.TotemicFigure>.GetEnumerator();
            }

            CCTotem.TotemicFigure IEnumerator<CCTotem.TotemicFigure>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <>c__Iterator29 : IDisposable, IEnumerator, IEnumerable, IEnumerable<CCTotem.TotemicFigure>, IEnumerator<CCTotem.TotemicFigure>
        {
            internal CCTotem.TotemicFigure $current;
            internal int $PC;
            internal CCTotem.Route <>f__this;
            internal CCTotem.Route <it>__0;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.<it>__0 = this.<>f__this;
                        break;

                    case 1:
                        this.<it>__0.GetDown(out this.<it>__0);
                        break;

                    default:
                        goto Label_0088;
                }
                if (this.<it>__0.At.Exists)
                {
                    this.$current = this.<it>__0.At.TotemicFigure;
                    this.$PC = 1;
                    return true;
                }
                this.$PC = -1;
            Label_0088:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<CCTotem.TotemicFigure> IEnumerable<CCTotem.TotemicFigure>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new CCTotem.Route.<>c__Iterator29 { <>f__this = this.<>f__this };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.System.Collections.Generic.IEnumerable<CCTotem.TotemicFigure>.GetEnumerator();
            }

            CCTotem.TotemicFigure IEnumerator<CCTotem.TotemicFigure>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <>c__Iterator2A : IDisposable, IEnumerator, IEnumerable, IEnumerable<CCTotem.TotemicFigure>, IEnumerator<CCTotem.TotemicFigure>
        {
            internal CCTotem.TotemicFigure $current;
            internal int $PC;
            internal CCTotem.Route <>f__this;
            internal CCTotem.Route <it>__0;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        if (!this.<>f__this.GetUp(out this.<it>__0))
                        {
                            goto Label_008B;
                        }
                        break;

                    case 1:
                        this.<it>__0.GetDown(out this.<it>__0);
                        break;

                    default:
                        goto Label_0092;
                }
                if (this.<it>__0.At.Exists)
                {
                    this.$current = this.<it>__0.At.TotemicFigure;
                    this.$PC = 1;
                    return true;
                }
            Label_008B:
                this.$PC = -1;
            Label_0092:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<CCTotem.TotemicFigure> IEnumerable<CCTotem.TotemicFigure>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new CCTotem.Route.<>c__Iterator2A { <>f__this = this.<>f__this };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.System.Collections.Generic.IEnumerable<CCTotem.TotemicFigure>.GetEnumerator();
            }

            CCTotem.TotemicFigure IEnumerator<CCTotem.TotemicFigure>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }
    }

    private static class ToStringHelper<T> where T: struct
    {
        private static readonly FieldInfo[] allFields;
        private static readonly string formatterString;
        private static readonly object[] valueHolders;

        static ToStringHelper()
        {
            CCTotem.ToStringHelper<T>.allFields = typeof(T).GetFields();
            CCTotem.ToStringHelper<T>.valueHolders = new object[CCTotem.ToStringHelper<T>.allFields.Length];
            StringBuilder sb = new StringBuilder();
            using (StringWriter writer = new StringWriter(sb))
            {
                writer.Write("{{");
                bool flag = true;
                for (int i = 0; i < CCTotem.ToStringHelper<T>.allFields.Length; i++)
                {
                    if (flag)
                    {
                        flag = false;
                    }
                    else
                    {
                        writer.Write(", ");
                    }
                    writer.Write("{0}={{{1}}}", CCTotem.ToStringHelper<T>.allFields[i].Name, i);
                }
                writer.Write("}}");
            }
            CCTotem.ToStringHelper<T>.formatterString = sb.ToString();
        }

        public static string GetString(object boxed)
        {
            string str;
            try
            {
                for (int i = 0; i < CCTotem.ToStringHelper<T>.allFields.Length; i++)
                {
                    CCTotem.ToStringHelper<T>.valueHolders[i] = CCTotem.ToStringHelper<T>.allFields[i].GetValue(boxed);
                }
                str = string.Format(CCTotem.ToStringHelper<T>.formatterString, CCTotem.ToStringHelper<T>.valueHolders);
            }
            finally
            {
                for (int j = 0; j < CCTotem.ToStringHelper<T>.allFields.Length; j++)
                {
                    CCTotem.ToStringHelper<T>.valueHolders[j] = null;
                }
            }
            return str;
        }
    }

    public sealed class TotemicFigure : CCTotem.TotemicObject<CCTotemicFigure, CCTotem.TotemicFigure>
    {
        internal readonly int BottomUpIndex;
        public CCDesc CCDesc;
        internal readonly CollisionFlags CollisionFlagsMask;
        public Vector3 PostSweepBottom;
        public Vector3 PreSweepBottom;
        public Vector3 SweepMovement;
        internal readonly int TopDownIndex;
        internal readonly CCTotem.Contraction TotemContractionBottom;
        internal readonly CCTotem.Contraction TotemContractionTop;
        internal readonly CCTotem.Route TotemicRoute;
        internal readonly CCTotem.TotemPole TotemPole;

        [Obsolete("Infrastructure", true)]
        public TotemicFigure()
        {
            throw new NotSupportedException();
        }

        private TotemicFigure(CCTotem.Direction Down) : this(Down.TotemicFigure.TotemPole, Down.TotemicFigure.BottomUpIndex + 1)
        {
            CCTotem.Direction none;
            float t = ((float) this.BottomUpIndex) / ((float) this.TotemPole.Configuration.numSlidingTotemicFigures);
            float num2 = (this.TotemPole.Configuration.numSlidingTotemicFigures != 1) ? (((float) (this.BottomUpIndex - 1)) / ((float) (this.TotemPole.Configuration.numSlidingTotemicFigures - 1))) : t;
            float contracted = Mathf.Lerp(this.TotemPole.Configuration.poleBottomBufferAmount, this.TotemPole.Configuration.poleContractedHeight - this.TotemPole.Configuration.figureSkinnedHeight, num2);
            float expanded = Mathf.Lerp(this.TotemPole.Configuration.poleBottomBufferAmount, this.TotemPole.Configuration.poleExpandedHeight - this.TotemPole.Configuration.figureSkinnedHeight, t);
            this.TotemContractionBottom = CCTotem.Contraction.Define(contracted, expanded);
            this.TotemContractionTop = CCTotem.Contraction.Define(contracted + this.TotemPole.Configuration.figureSkinnedHeight, expanded + this.TotemPole.Configuration.figureSkinnedHeight);
            CCTotem.Direction down = new CCTotem.Direction(this);
            if (this.BottomUpIndex < (this.TotemPole.Configuration.numRequiredTotemicFigures - 1))
            {
                none = new CCTotem.Direction(new CCTotem.TotemicFigure(down));
            }
            else
            {
                none = CCTotem.Direction.None;
            }
            this.TotemicRoute = new CCTotem.Route(Down, down, none);
        }

        private TotemicFigure(CCTotem.TotemPole TotemPole) : this(TotemPole, 0)
        {
            CCTotem.Direction at = new CCTotem.Direction(this);
            this.TotemicRoute = new CCTotem.Route(CCTotem.Direction.None, at, new CCTotem.Direction(new CCTotem.TotemicFigure(at)));
        }

        private TotemicFigure(CCTotem.TotemPole TotemPole, int BottomUpIndex)
        {
            this.TotemPole = TotemPole;
            this.BottomUpIndex = BottomUpIndex;
            this.TopDownIndex = this.TotemPole.Configuration.numRequiredTotemicFigures - (this.BottomUpIndex + 1);
            this.CollisionFlagsMask = CollisionFlags.Sides;
            if (this.BottomUpIndex == 0)
            {
                this.CollisionFlagsMask |= CollisionFlags.Below;
            }
            if (this.TopDownIndex == 0)
            {
                this.CollisionFlagsMask |= CollisionFlags.Above;
            }
            this.TotemPole.TotemicFigures[this.BottomUpIndex] = this;
        }

        internal override void AssignedToScript(CCTotemicFigure Script)
        {
            base.Script = Script;
        }

        internal static CCTotem.Ends<CCTotem.TotemicFigure> CreateAllTotemicFigures(CCTotem.TotemPole TotemPole)
        {
            if (!object.ReferenceEquals(TotemPole.TotemicFigures[0], null))
            {
                throw new ArgumentException("The totem pole already has totemic figures", "TotemPole");
            }
            CCTotem.TotemicFigure bottom = new CCTotem.TotemicFigure(TotemPole);
            return new CCTotem.Ends<CCTotem.TotemicFigure>(bottom, TotemPole.TotemicFigures[TotemPole.Configuration.numRequiredTotemicFigures - 1]);
        }

        internal void Delete(CCTotemPole OwnerScript)
        {
            CCTotemicFigure script = base.Script;
            base.Script = null;
            if ((script != null) && object.ReferenceEquals(script.totemicObject, this))
            {
                script.totemicObject = null;
            }
            CCTotem.DestroyCCDesc(OwnerScript, ref this.CCDesc);
            if (script != null)
            {
                Object.Destroy(script.gameObject);
            }
            if (object.ReferenceEquals(this.TotemPole.TotemicFigures[this.BottomUpIndex], this))
            {
                this.TotemPole.TotemicFigures[this.BottomUpIndex] = null;
            }
        }

        public CollisionFlags MoveSweep(Vector3 motion)
        {
            this.PreSweepBottom = this.BottomOrigin;
            CollisionFlags flags = this.MoveWorld(motion);
            this.PostSweepBottom = this.BottomOrigin;
            this.SweepMovement = this.PostSweepBottom - this.PreSweepBottom;
            return flags;
        }

        public CollisionFlags MoveWorld(Vector3 motion)
        {
            return this.CCDesc.Move(motion);
        }

        public CollisionFlags MoveWorldBottomTo(Vector3 targetBottom)
        {
            return this.MoveWorld(targetBottom - this.BottomOrigin);
        }

        public CollisionFlags MoveWorldTopTo(Vector3 targetTop)
        {
            return this.MoveWorld(targetTop - this.TopOrigin);
        }

        internal override void OnScriptDestroy(CCTotemicFigure Script)
        {
            if (object.ReferenceEquals(base.Script, Script))
            {
                base.Script = null;
                if (object.ReferenceEquals(Script.totemicObject, this))
                {
                    Script.totemicObject = null;
                }
            }
        }

        public override string ToString()
        {
            object[] args = new object[] { this.BottomUpIndex, this.TotemContractionBottom, this.TotemContractionTop, base.Script };
            return string.Format("{{Index={0},ContractionBottom={1},ContractionTop={2},Script={3}}}", args);
        }

        public Vector3 BottomOrigin
        {
            get
            {
                return this.CCDesc.worldSkinnedBottom;
            }
        }

        public Vector3 CenterOrigin
        {
            get
            {
                return this.CCDesc.worldCenter;
            }
        }

        public Vector3 SlideBottomOrigin
        {
            get
            {
                return this.CCDesc.OffsetToWorld(this.CCDesc.center - new Vector3(0f, (this.CCDesc.effectiveSkinnedHeight * 0.5f) - this.CCDesc.skinnedRadius, 0f));
            }
        }

        public Vector3 SlideTopOrigin
        {
            get
            {
                return this.CCDesc.OffsetToWorld(this.CCDesc.center + new Vector3(0f, (this.CCDesc.effectiveSkinnedHeight * 0.5f) - this.CCDesc.skinnedRadius, 0f));
            }
        }

        public Vector3 TopOrigin
        {
            get
            {
                return this.CCDesc.worldSkinnedTop;
            }
        }
    }

    public abstract class TotemicObject
    {
        internal TotemicObject()
        {
        }

        internal abstract CCTotem _Script { get; }

        protected internal CCTotem Script
        {
            get
            {
                return this._Script;
            }
        }
    }

    public abstract class TotemicObject<CCTotemScript> : CCTotem.TotemicObject where CCTotemScript: CCTotem, new()
    {
        protected internal CCTotemScript Script;

        internal TotemicObject()
        {
        }

        internal sealed override CCTotem _Script
        {
            get
            {
                return this.Script;
            }
        }
    }

    public abstract class TotemicObject<CCTotemScript, TTotemicObject> : CCTotem.TotemicObject<CCTotemScript> where CCTotemScript: CCTotem<TTotemicObject, CCTotemScript>, new() where TTotemicObject: CCTotem.TotemicObject<CCTotemScript, TTotemicObject>, new()
    {
        internal TotemicObject()
        {
        }

        internal abstract void AssignedToScript(CCTotemScript Script);
        internal abstract void OnScriptDestroy(CCTotemScript Script);
    }

    public sealed class TotemPole : CCTotem.TotemicObject<CCTotemPole, CCTotem.TotemPole>
    {
        internal CCDesc CCDesc;
        internal readonly CCTotem.Configuration Configuration;
        internal readonly CCTotem.Contraction Contraction;
        internal CCTotem.Expansion Expansion;
        private bool grounded;
        private const int kCrouch_MovingDown = -1;
        private const int kCrouch_MovingUp = 1;
        private const int kCrouch_NotModified = 0;
        internal CCTotem.Ends<Vector3> Point;
        internal readonly CCTotem.Ends<CCTotem.TotemicFigure> TotemicFigureEnds;
        internal readonly CCTotem.TotemicFigure[] TotemicFigures;

        [Obsolete("Infrastructure", true)]
        public TotemPole()
        {
            throw new NotSupportedException();
        }

        internal TotemPole(ref CCTotem.Configuration TotemConfiguration)
        {
            this.Configuration = TotemConfiguration;
            this.TotemicFigures = new CCTotem.TotemicFigure[8];
            this.TotemicFigureEnds = CCTotem.TotemicFigure.CreateAllTotemicFigures(this);
            this.Contraction = CCTotem.Contraction.Define(this.Configuration.poleContractedHeight, this.Configuration.poleExpandedHeight);
        }

        internal override void AssignedToScript(CCTotemPole Script)
        {
            base.Script = Script;
        }

        public void Create()
        {
            float initialHeight = this.Configuration.totem.initialHeight;
            this.Expansion = this.Contraction.ExpansionForValue(initialHeight);
            Vector3 worldBottom = base.Script.transform.position + this.Configuration.figureOriginOffsetBottom;
            this.CCDesc = this.InstantiateCCDesc(worldBottom, "__TotemPole");
            if (base.Script != null)
            {
                base.Script.ExecuteBinding(this.CCDesc, true);
            }
            for (int i = 0; i < this.Configuration.numRequiredTotemicFigures; i++)
            {
                this.InstantiateTotemicFigure(worldBottom, this.TotemicFigures[i]);
            }
            this.Point.Bottom = this.TotemicFigures[0].BottomOrigin;
            this.Point.Top = this.TotemicFigureEnds.Top.TopOrigin;
            this.CCDesc.ModifyHeight(this.Point.Top.y - this.Point.Bottom.y, false);
        }

        private void DeleteAllFiguresAndClearScript()
        {
            CCTotemPole script = base.Script;
            base.Script = null;
            for (int i = this.Configuration.numRequiredTotemicFigures - 1; i >= 0; i--)
            {
                CCTotem.TotemicFigure objA = this.TotemicFigures[i];
                if (!object.ReferenceEquals(objA, null))
                {
                    if (objA.TotemPole == this)
                    {
                        this.TotemicFigures[i].Delete(script);
                    }
                    else
                    {
                        this.TotemicFigures[i] = null;
                    }
                }
            }
            CCTotem.DestroyCCDesc(script, ref this.CCDesc);
            if ((script != null) && object.ReferenceEquals(script.totemicObject, this))
            {
                script.totemicObject = null;
            }
        }

        private CCDesc InstantiateCCDesc(Vector3 worldBottom, string name)
        {
            CCDesc desc = (CCDesc) Object.Instantiate(this.Configuration.totem.figurePrefab, worldBottom, Quaternion.identity);
            if (!string.IsNullOrEmpty(name))
            {
                desc.name = name;
            }
            desc.gameObject.hideFlags = HideFlags.NotEditable;
            desc.detectCollisions = false;
            return desc;
        }

        private CCTotemicFigure InstantiateTotemicFigure(Vector3 worldBottom, CCTotem.TotemicFigure target)
        {
            worldBottom.y += target.TotemContractionBottom.ExpansionForFraction(this.Expansion.FractionExpanded).Value;
            target.CCDesc = this.InstantiateCCDesc(worldBottom, string.Format("__TotemicFigure{0}", target.BottomUpIndex));
            CCTotemicFigure figure = target.CCDesc.gameObject.AddComponent<CCTotemicFigure>();
            figure.AssignTotemicObject(target);
            if (base.Script != null)
            {
                base.Script.ExecuteBinding(target.CCDesc, true);
            }
            return figure;
        }

        public CCTotem.MoveInfo Move(Vector3 motion, float height)
        {
            CCTotem.Expansion expansion = this.Contraction.ExpansionForValue(height);
            height = expansion.Value;
            CollisionFlags workingCollisionFlags = this.TotemicFigureEnds.Bottom.MoveSweep(motion) & this.TotemicFigureEnds.Bottom.CollisionFlagsMask;
            this.grounded = this.TotemicFigureEnds.Bottom.CCDesc.isGrounded;
            int index = 0;
            for (int i = this.Configuration.numRequiredTotemicFigures - 1; i >= 1; i--)
            {
                Vector3 sweepMovement = this.TotemicFigures[index].SweepMovement;
                workingCollisionFlags |= this.TotemicFigures[i].MoveSweep(sweepMovement) & this.TotemicFigures[i].CollisionFlagsMask;
                index = i;
            }
            if (this.TotemicFigures[index].SweepMovement != this.TotemicFigures[0].SweepMovement)
            {
                Vector3 vector2 = this.TotemicFigures[index].SweepMovement;
                for (int j = 0; j < this.Configuration.numRequiredTotemicFigures; j++)
                {
                    Vector3 vector3 = vector2 - this.TotemicFigures[j].SweepMovement;
                    workingCollisionFlags |= this.TotemicFigures[j].MoveSweep(vector3) & this.TotemicFigures[j].CollisionFlagsMask;
                }
            }
            this.Point.Bottom = this.TotemicFigures[0].BottomOrigin;
            this.Point.Top = this.TotemicFigureEnds.Top.TopOrigin;
            this.Expansion = this.Contraction.ExpansionForValue(this.Point.Top.y - this.Point.Bottom.y);
            if (this.Expansion.Value != expansion.Value)
            {
                Vector3 targetTop = this.Point.Bottom + new Vector3(0f, expansion.Value, 0f);
                workingCollisionFlags |= this.TotemicFigureEnds.Top.MoveWorldTopTo(targetTop) & this.TotemicFigureEnds.Top.CollisionFlagsMask;
                Vector3 topOrigin = this.TotemicFigureEnds.Top.TopOrigin;
                expansion = this.Contraction.ExpansionForValue(topOrigin.y - this.Point.Bottom.y);
                for (int k = this.Configuration.numRequiredTotemicFigures - 2; k > 0; k--)
                {
                    CCTotem.TotemicFigure figure = this.TotemicFigures[k];
                    Vector3 targetBottom = this.Point.Bottom;
                    targetBottom.y += figure.TotemContractionBottom.ExpansionForFraction(expansion.FractionExpanded).Value;
                    workingCollisionFlags |= figure.MoveWorldBottomTo(targetBottom) & figure.CollisionFlagsMask;
                }
                this.Point.Top = this.TotemicFigureEnds.Top.TopOrigin;
                this.Expansion = expansion;
            }
            float effectiveSkinnedHeight = this.CCDesc.effectiveSkinnedHeight;
            Vector3 worldSkinnedBottom = this.CCDesc.worldSkinnedBottom;
            Vector3 worldSkinnedTop = this.CCDesc.worldSkinnedTop;
            Vector3 vector9 = this.TotemicFigures[0].BottomOrigin - worldSkinnedBottom;
            CCDesc.HeightModification modification = this.CCDesc.ModifyHeight(this.Expansion.Value, false);
            CollisionFlags collisionFlags = this.CCDesc.Move(vector9);
            Vector3 bottom = this.CCDesc.worldSkinnedBottom;
            Vector3 bottomMovement = bottom - worldSkinnedBottom;
            if (vector9 != bottomMovement)
            {
                Vector3 vector12 = bottomMovement - vector9;
                for (int m = 0; m < this.Configuration.numRequiredTotemicFigures; m++)
                {
                    workingCollisionFlags |= this.TotemicFigures[m].MoveSweep(vector12) & this.TotemicFigures[m].CollisionFlagsMask;
                }
                this.Point.Bottom = this.TotemicFigures[0].BottomOrigin;
                this.Point.Top = this.TotemicFigureEnds.Top.TopOrigin;
                this.Expansion = this.Contraction.ExpansionForValue(this.Point.Top.y - this.Point.Bottom.y);
                CCDesc.HeightModification modification2 = this.CCDesc.ModifyHeight(this.Expansion.Value, false);
                bottom = this.CCDesc.worldSkinnedBottom;
                bottomMovement = bottom - worldSkinnedBottom;
            }
            Vector3 top = this.CCDesc.worldSkinnedTop;
            Vector3 topMovement = top - worldSkinnedTop;
            return new CCTotem.MoveInfo(collisionFlags, workingCollisionFlags, height, bottomMovement, topMovement, new CCTotem.PositionPlacement(bottom, top, this.CCDesc.transform.position, this.Configuration.poleExpandedHeight));
        }

        internal override void OnScriptDestroy(CCTotemPole Script)
        {
            if (object.ReferenceEquals(base.Script, Script))
            {
                this.DeleteAllFiguresAndClearScript();
            }
        }

        private CCDesc CCDescOrPrefab
        {
            get
            {
                return ((this.CCDesc == null) ? this.Configuration.totem.figurePrefab : this.CCDesc);
            }
        }

        public Vector3 center
        {
            get
            {
                return this.CCDescOrPrefab.center;
            }
        }

        public CollisionFlags collisionFlags
        {
            get
            {
                return ((this.CCDesc == null) ? CollisionFlags.None : this.CCDesc.collisionFlags);
            }
        }

        public float height
        {
            get
            {
                return this.CCDescOrPrefab.height;
            }
        }

        public bool isGrounded
        {
            get
            {
                return this.grounded;
            }
        }

        public float radius
        {
            get
            {
                return this.CCDescOrPrefab.radius;
            }
        }

        public float slopeLimit
        {
            get
            {
                return this.CCDescOrPrefab.slopeLimit;
            }
        }

        public float stepOffset
        {
            get
            {
                return this.CCDescOrPrefab.stepOffset;
            }
        }

        public Vector3 velocity
        {
            get
            {
                return ((this.CCDesc == null) ? Vector3.zero : this.CCDesc.velocity);
            }
        }
    }
}

