using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

public abstract class LightStyle : ScriptableObject
{
    private static Dictionary<string, WeakReference> loadedByString;
    private static bool madeLoadedByString;

    protected LightStyle()
    {
    }

    protected abstract Simulation ConstructSimulation(LightStylist stylist);
    public Simulation CreateSimulation(LightStylist stylist)
    {
        return this.CreateSimulation(time, stylist);
    }

    public Simulation CreateSimulation(double startTime, LightStylist stylist)
    {
        Simulation simulation = this.ConstructSimulation(stylist);
        if (simulation != null)
        {
            simulation.ResetTime(startTime);
        }
        return simulation;
    }

    protected abstract bool DeconstructSimulation(Simulation simulation);
    private static LightStyle MissingLightStyle(string name)
    {
        return LightStyleDefault.Singleton;
    }

    public static implicit operator string(LightStyle lightStyle)
    {
        return ((lightStyle == null) ? null : lightStyle.name);
    }

    public static implicit operator LightStyle(string name)
    {
        WeakReference reference;
        if (!madeLoadedByString)
        {
            LightStyle style = (LightStyle) Resources.Load(name, typeof(LightStyle));
            if (style != null)
            {
                loadedByString = new Dictionary<string, WeakReference>(StringComparer.InvariantCultureIgnoreCase);
                loadedByString[name] = new WeakReference(style);
                return style;
            }
            return MissingLightStyle(name);
        }
        if (loadedByString.TryGetValue(name, out reference))
        {
            object obj2 = reference.Target;
            if (reference.IsAlive && (((Object) obj2) != null))
            {
                return (LightStyle) obj2;
            }
            LightStyle style2 = (LightStyle) Resources.Load(name, typeof(LightStyle));
            if (style2 != null)
            {
                reference.Target = style2;
                return style2;
            }
            return MissingLightStyle(name);
        }
        LightStyle target = (LightStyle) Resources.Load(name, typeof(LightStyle));
        if (target != null)
        {
            reference = new WeakReference(target);
            loadedByString[name] = reference;
            return target;
        }
        return MissingLightStyle(name);
    }

    public static double time
    {
        get
        {
            if (NetCull.isRunning)
            {
                return NetCull.time;
            }
            return (double) Time.time;
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct Mod
    {
        [FieldOffset(12)]
        public float a;
        [FieldOffset(8)]
        public float b;
        [FieldOffset(0)]
        public Color color;
        [FieldOffset(4)]
        public float g;
        [FieldOffset(0x10)]
        public float intensity;
        public const Element kElementBegin = Element.Red;
        public const Element kElementEnd = (Element.SpotAngle | Element.Green);
        public const Element kElementEnumeratorBegin = ~Element.Red;
        public const Element kElementFirst = Element.Red;
        public const Element kElementLast = Element.SpotAngle;
        public const Mask kMaskAll = (Mask.SpotAngle | Mask.Range | Mask.Intensity | Mask.Alpha | Mask.Blue | Mask.Green | Mask.Red);
        public const Mask kMaskDirectionalLight = (Mask.Intensity | Mask.Alpha | Mask.Blue | Mask.Green | Mask.Red);
        public const Mask kMaskNone = 0;
        public const Mask kMaskPointLight = (Mask.Range | Mask.Intensity | Mask.Alpha | Mask.Blue | Mask.Green | Mask.Red);
        public const Mask kMaskRGB = (Mask.Blue | Mask.Green | Mask.Red);
        public const Mask kMaskRGBA = (Mask.Alpha | Mask.Blue | Mask.Green | Mask.Red);
        public const Mask kMaskSpotLight = (Mask.SpotAngle | Mask.Range | Mask.Intensity | Mask.Alpha | Mask.Blue | Mask.Green | Mask.Red);
        [FieldOffset(0x1c)]
        public Mask mask;
        [FieldOffset(0)]
        public float r;
        [FieldOffset(20)]
        public float range;
        [FieldOffset(0x18)]
        public float spotAngle;

        public bool AllOf(Mask mask)
        {
            return ((this.mask & mask) == mask);
        }

        public bool AnyOf(Mask mask)
        {
            return ((this.mask & mask) != 0);
        }

        public void ApplyTo(Light light)
        {
            switch (light.type)
            {
                case LightType.Spot:
                    this.ApplyTo(light, Mask.SpotAngle | Mask.Range | Mask.Intensity | Mask.Alpha | Mask.Blue | Mask.Green | Mask.Red);
                    break;

                case LightType.Directional:
                    this.ApplyTo(light, Mask.Intensity | Mask.Alpha | Mask.Blue | Mask.Green | Mask.Red);
                    break;

                case LightType.Point:
                    this.ApplyTo(light, Mask.Range | Mask.Intensity | Mask.Alpha | Mask.Blue | Mask.Green | Mask.Red);
                    break;
            }
        }

        public void ApplyTo(Light light, Mask applyMask)
        {
            Mask mask = this.mask & applyMask;
            if ((mask & (Mask.Alpha | Mask.Blue | Mask.Green | Mask.Red)) != 0)
            {
                if ((mask & (Mask.Alpha | Mask.Blue | Mask.Green | Mask.Red)) == (Mask.Alpha | Mask.Blue | Mask.Green | Mask.Red))
                {
                    light.color = this.color;
                }
                else
                {
                    Color color = light.color;
                    if ((mask & Mask.Red) == Mask.Red)
                    {
                        color.r = this.r;
                    }
                    if ((mask & Mask.Green) == Mask.Green)
                    {
                        color.g = this.g;
                    }
                    if ((mask & Mask.Blue) == Mask.Blue)
                    {
                        color.b = this.b;
                    }
                    if ((mask & Mask.Alpha) == Mask.Alpha)
                    {
                        color.a = this.a;
                    }
                    light.color = color;
                }
            }
            if ((mask & Mask.Intensity) == Mask.Intensity)
            {
                light.intensity = this.intensity;
            }
            if ((mask & Mask.Range) == Mask.Range)
            {
                light.range = this.range;
            }
            if ((mask & Mask.SpotAngle) == Mask.SpotAngle)
            {
                light.spotAngle = this.spotAngle;
            }
        }

        public void ClearModify(Element element)
        {
            this.mask &= ElementToMaskNot(element);
        }

        public bool Contains(Element element)
        {
            return this.AllOf(ElementToMask(element));
        }

        public static Mask ElementToMask(Element element)
        {
            return (((Mask) (((int) 1) << element)) & (Mask.SpotAngle | Mask.Range | Mask.Intensity | Mask.Alpha | Mask.Blue | Mask.Green | Mask.Red));
        }

        public static Mask ElementToMaskNot(Element element)
        {
            return (((Mask) ~(((int) 1) << element)) & (Mask.SpotAngle | Mask.Range | Mask.Intensity | Mask.Alpha | Mask.Blue | Mask.Green | Mask.Red));
        }

        public float GetFaceValue(Element element)
        {
            switch (element)
            {
                case Element.Red:
                    return this.r;

                case Element.Green:
                    return this.g;

                case Element.Blue:
                    return this.b;

                case Element.Alpha:
                    return this.a;

                case Element.Intensity:
                    return this.intensity;

                case Element.Range:
                    return this.range;

                case Element.SpotAngle:
                    return this.spotAngle;
            }
            throw new ArgumentOutOfRangeException("element");
        }

        public static LightStyle.Mod Lerp(LightStyle.Mod a, LightStyle.Mod b, float t, Mask mask)
        {
            b.mask &= mask;
            if (b.mask != 0)
            {
                a.mask &= mask;
                if (a.mask == 0)
                {
                    return b;
                }
                Mask mask2 = a.mask & b.mask;
                if (mask2 != 0)
                {
                    float num = 1f - t;
                    if (mask != 0)
                    {
                        for (Element element = Element.Red; element < (Element.SpotAngle | Element.Green); element += 1)
                        {
                            if ((mask2 & ElementToMask(element)) == ElementToMask(element))
                            {
                                float faceValue = a.GetFaceValue(element);
                                float num3 = b.GetFaceValue(element);
                                float num4 = (faceValue * num) + (num3 * t);
                                a.SetFaceValue(element, num4);
                            }
                        }
                    }
                }
                if (mask2 != a.mask)
                {
                    a |= b;
                }
            }
            return a;
        }

        public static LightStyle.Mod operator +(LightStyle.Mod a, LightStyle.Mod b)
        {
            LightStyle.Mod mod = a;
            Mask mask = a.mask & b.mask;
            for (Element element = Element.Red; element < (Element.SpotAngle | Element.Green); element += 1)
            {
                if ((mask & ElementToMask(element)) == ElementToMask(element))
                {
                    mod.SetFaceValue(element, a.GetFaceValue(element) + b.GetFaceValue(element));
                }
            }
            return mod;
        }

        public static LightStyle.Mod operator +(LightStyle.Mod a, Element b)
        {
            a.mask |= ElementToMask(b);
            return a;
        }

        public static LightStyle.Mod operator +(LightStyle.Mod a, float b)
        {
            for (Element element = Element.Red; element < (Element.SpotAngle | Element.Green); element += 1)
            {
                if ((a.mask & ElementToMask(element)) == ElementToMask(element))
                {
                    a.SetFaceValue(element, a.GetFaceValue(element) + b);
                }
            }
            return a;
        }

        public static LightStyle.Mod operator +(LightStyle.Mod a, Color b)
        {
            for (int i = 0; i < 4; i++)
            {
                if ((a.mask & ElementToMask((Element) (0 + i))) == ElementToMask((Element) (0 + i)))
                {
                    a.SetFaceValue((Element) (0 + i), a.GetFaceValue((Element) (0 + i)) + b[i]);
                }
            }
            return a;
        }

        public static unsafe Color operator +(Color b, LightStyle.Mod a)
        {
            for (int i = 0; i < 4; i++)
            {
                if ((a.mask & ElementToMask((Element) (0 + i))) == ElementToMask((Element) (0 + i)))
                {
                    ref Color colorRef;
                    int num2;
                    float num3 = colorRef[num2];
                    (colorRef = (Color) &b)[num2 = i] = num3 + a.GetFaceValue((Element) (0 + i));
                }
            }
            return b;
        }

        public static LightStyle.Mod operator &(LightStyle.Mod a, LightStyle.Mod b)
        {
            for (Element element = Element.Red; element < (Element.SpotAngle | Element.Green); element += 1)
            {
                if (((a.mask & ElementToMask(element)) == ElementToMask(element)) && ((b.mask & ElementToMask(element)) == ElementToMask(element)))
                {
                    a.SetFaceValue(element, b.GetFaceValue(element));
                }
            }
            return a;
        }

        public static bool operator &(LightStyle.Mod a, Element b)
        {
            return a.Contains(b);
        }

        public static LightStyle.Mod operator &(LightStyle.Mod a, Mask b)
        {
            a.mask &= b;
            return a;
        }

        public static LightStyle.Mod operator |(LightStyle.Mod a, LightStyle.Mod b)
        {
            for (Element element = Element.Red; element < (Element.SpotAngle | Element.Green); element += 1)
            {
                if (((a.mask & ElementToMask(element)) != ElementToMask(element)) && ((b.mask & ElementToMask(element)) == ElementToMask(element)))
                {
                    a.SetModify(element, b.GetFaceValue(element));
                }
            }
            return a;
        }

        public static float? operator |(LightStyle.Mod a, Element b)
        {
            return a[b];
        }

        public static LightStyle.Mod operator |(LightStyle.Mod a, Mask b)
        {
            a.mask |= b;
            return a;
        }

        public static LightStyle.Mod operator /(LightStyle.Mod a, LightStyle.Mod b)
        {
            Mask mask = a.mask & b.mask;
            for (Element element = Element.Red; element < (Element.SpotAngle | Element.Green); element += 1)
            {
                if ((mask & ElementToMask(element)) == ElementToMask(element))
                {
                    a.SetFaceValue(element, a.GetFaceValue(element) / b.GetFaceValue(element));
                }
            }
            return a;
        }

        public static LightStyle.Mod operator /(LightStyle.Mod a, float b)
        {
            for (Element element = Element.Red; element < (Element.SpotAngle | Element.Green); element += 1)
            {
                if ((a.mask & ElementToMask(element)) == ElementToMask(element))
                {
                    a.SetFaceValue(element, a.GetFaceValue(element) / b);
                }
            }
            return a;
        }

        public static LightStyle.Mod operator /(LightStyle.Mod a, Color b)
        {
            for (int i = 0; i < 4; i++)
            {
                if ((a.mask & ElementToMask((Element) (0 + i))) == ElementToMask((Element) (0 + i)))
                {
                    a.SetFaceValue((Element) (0 + i), a.GetFaceValue((Element) (0 + i)) / b[i]);
                }
            }
            return a;
        }

        public static unsafe Color operator /(Color b, LightStyle.Mod a)
        {
            for (int i = 0; i < 4; i++)
            {
                if ((a.mask & ElementToMask((Element) (0 + i))) == ElementToMask((Element) (0 + i)))
                {
                    ref Color colorRef;
                    int num2;
                    float num3 = colorRef[num2];
                    (colorRef = (Color) &b)[num2 = i] = num3 / a.GetFaceValue((Element) (0 + i));
                }
            }
            return b;
        }

        public static LightStyle.Mod operator ^(LightStyle.Mod a, LightStyle.Mod b)
        {
            for (Element element = Element.Red; element < (Element.SpotAngle | Element.Green); element += 1)
            {
                if ((a.mask & ElementToMask(element)) == ElementToMask(element))
                {
                    if ((b.mask & ElementToMask(element)) == ElementToMask(element))
                    {
                        a.SetFaceValue(element, b.GetFaceValue(element));
                    }
                }
                else if ((b.mask & ElementToMask(element)) == ElementToMask(element))
                {
                    a.SetModify(element, b.GetFaceValue(element));
                }
            }
            return a;
        }

        public static float operator ^(LightStyle.Mod a, Element b)
        {
            return a.GetFaceValue(b);
        }

        public static LightStyle.Mod operator ^(LightStyle.Mod a, Mask b)
        {
            a.mask ^= b;
            return a;
        }

        public static explicit operator LightStyle.Mod(float intensity)
        {
            return new LightStyle.Mod { intensity = intensity, mask = Mask.Intensity };
        }

        public static explicit operator LightStyle.Mod(Color color)
        {
            return new LightStyle.Mod { color = color, mask = Mask.Alpha | Mask.Blue | Mask.Green | Mask.Red };
        }

        public static explicit operator LightStyle.Mod(Light light)
        {
            LightStyle.Mod mod = new LightStyle.Mod();
            if (light != null)
            {
                mod.color = light.color;
                mod.intensity = light.intensity;
                mod.range = light.range;
                mod.spotAngle = light.spotAngle;
                switch (light.type)
                {
                    case LightType.Spot:
                        mod.mask = Mask.SpotAngle | Mask.Range | Mask.Intensity | Mask.Alpha | Mask.Blue | Mask.Green | Mask.Red;
                        return mod;

                    case LightType.Directional:
                        mod.mask = Mask.Intensity | Mask.Alpha | Mask.Blue | Mask.Green | Mask.Red;
                        return mod;

                    case LightType.Point:
                        mod.mask = Mask.Range | Mask.Intensity | Mask.Alpha | Mask.Blue | Mask.Green | Mask.Red;
                        return mod;
                }
            }
            return mod;
        }

        public static bool operator false(LightStyle.Mod b)
        {
            return ((b.mask & (Mask.SpotAngle | Mask.Range | Mask.Intensity | Mask.Alpha | Mask.Blue | Mask.Green | Mask.Red)) == 0);
        }

        public static LightStyle.Mod operator *(LightStyle.Mod a, LightStyle.Mod b)
        {
            Mask mask = a.mask & b.mask;
            for (Element element = Element.Red; element < (Element.SpotAngle | Element.Green); element += 1)
            {
                if ((mask & ElementToMask(element)) == ElementToMask(element))
                {
                    a.SetFaceValue(element, a.GetFaceValue(element) * b.GetFaceValue(element));
                }
            }
            return a;
        }

        public static LightStyle.Mod operator *(LightStyle.Mod a, float b)
        {
            LightStyle.Mod mod = a;
            for (Element element = Element.Red; element < (Element.SpotAngle | Element.Green); element += 1)
            {
                if ((a.mask & ElementToMask(element)) == ElementToMask(element))
                {
                    mod.SetFaceValue(element, a.GetFaceValue(element) * b);
                }
            }
            return mod;
        }

        public static LightStyle.Mod operator *(LightStyle.Mod a, Color b)
        {
            for (int i = 0; i < 4; i++)
            {
                if ((a.mask & ElementToMask((Element) (0 + i))) == ElementToMask((Element) (0 + i)))
                {
                    a.SetFaceValue((Element) (0 + i), a.GetFaceValue((Element) (0 + i)) * b[i]);
                }
            }
            return a;
        }

        public static unsafe Color operator *(Color b, LightStyle.Mod a)
        {
            for (int i = 0; i < 4; i++)
            {
                if ((a.mask & ElementToMask((Element) (0 + i))) == ElementToMask((Element) (0 + i)))
                {
                    ref Color colorRef;
                    int num2;
                    float num3 = colorRef[num2];
                    (colorRef = (Color) &b)[num2 = i] = num3 * a.GetFaceValue((Element) (0 + i));
                }
            }
            return b;
        }

        public static LightStyle.Mod operator ~(LightStyle.Mod a)
        {
            a.mask = ~a.mask & (Mask.SpotAngle | Mask.Range | Mask.Intensity | Mask.Alpha | Mask.Blue | Mask.Green | Mask.Red);
            return a;
        }

        public static LightStyle.Mod operator -(LightStyle.Mod a, LightStyle.Mod b)
        {
            Mask mask = a.mask & b.mask;
            for (Element element = Element.Red; element < (Element.SpotAngle | Element.Green); element += 1)
            {
                if ((mask & ElementToMask(element)) == ElementToMask(element))
                {
                    a.SetFaceValue(element, a.GetFaceValue(element) - b.GetFaceValue(element));
                }
            }
            return a;
        }

        public static LightStyle.Mod operator -(LightStyle.Mod a, Element b)
        {
            a.mask &= ElementToMaskNot(b);
            return a;
        }

        public static LightStyle.Mod operator -(LightStyle.Mod a, float b)
        {
            for (Element element = Element.Red; element < (Element.SpotAngle | Element.Green); element += 1)
            {
                if ((a.mask & ElementToMask(element)) == ElementToMask(element))
                {
                    a.SetFaceValue(element, a.GetFaceValue(element) - b);
                }
            }
            return a;
        }

        public static LightStyle.Mod operator -(LightStyle.Mod a, Color b)
        {
            for (int i = 0; i < 4; i++)
            {
                if ((a.mask & ElementToMask((Element) (0 + i))) == ElementToMask((Element) (0 + i)))
                {
                    a.SetFaceValue((Element) (0 + i), a.GetFaceValue((Element) (0 + i)) - b[i]);
                }
            }
            return a;
        }

        public static unsafe Color operator -(Color b, LightStyle.Mod a)
        {
            for (int i = 0; i < 4; i++)
            {
                if ((a.mask & ElementToMask((Element) (0 + i))) == ElementToMask((Element) (0 + i)))
                {
                    ref Color colorRef;
                    int num2;
                    float num3 = colorRef[num2];
                    (colorRef = (Color) &b)[num2 = i] = num3 - a.GetFaceValue((Element) (0 + i));
                }
            }
            return b;
        }

        public static bool operator true(LightStyle.Mod a)
        {
            return ((a.mask & (Mask.SpotAngle | Mask.Range | Mask.Intensity | Mask.Alpha | Mask.Blue | Mask.Green | Mask.Red)) != 0);
        }

        public static LightStyle.Mod operator -(LightStyle.Mod a)
        {
            for (Element element = Element.Red; element < (Element.SpotAngle | Element.Green); element += 1)
            {
                a.SetFaceValue(element, -a.GetFaceValue(element));
            }
            return a;
        }

        public void SetFaceValue(Element element, float value)
        {
            switch (element)
            {
                case Element.Red:
                    this.r = value;
                    break;

                case Element.Green:
                    this.g = value;
                    break;

                case Element.Blue:
                    this.b = value;
                    break;

                case Element.Alpha:
                    this.a = value;
                    break;

                case Element.Intensity:
                    this.intensity = value;
                    break;

                case Element.Range:
                    this.range = value;
                    break;

                case Element.SpotAngle:
                    this.spotAngle = value;
                    break;

                default:
                    throw new ArgumentOutOfRangeException("element");
            }
        }

        public void SetModify(Element element)
        {
            this.mask |= ElementToMask(element);
        }

        public void SetModify(Element element, float assignValue)
        {
            this.SetFaceValue(element, assignValue);
            this.mask |= ElementToMask(element);
        }

        public void ToggleModify(Element element)
        {
            this.mask ^= ElementToMask(element);
        }

        public float? this[Element element]
        {
            get
            {
                if (this.Contains(element))
                {
                    return new float?(this.GetFaceValue(element));
                }
                return null;
            }
            set
            {
                if (value.HasValue)
                {
                    this.SetFaceValue(element, value.Value);
                    this.SetModify(element);
                }
                else
                {
                    this.ClearModify(element);
                }
            }
        }

        public enum Element
        {
            Red,
            Green,
            Blue,
            Alpha,
            Intensity,
            Range,
            SpotAngle
        }

        [Flags]
        public enum Mask
        {
            Alpha = 8,
            Blue = 4,
            Green = 2,
            Intensity = 0x10,
            Range = 0x20,
            Red = 1,
            SpotAngle = 0x40
        }
    }

    public abstract class Simulation : IDisposable
    {
        protected LightStyle creator;
        private bool destroyed;
        private bool isDisposing;
        protected double lastSimulateTime = double.NegativeInfinity;
        protected LightStyle.Mod mod;
        protected double nextBindTime = double.NegativeInfinity;
        protected double startTime;

        protected Simulation(LightStyle creator)
        {
            this.creator = creator;
        }

        public LightStyle.Mod BindMod(LightStyle.Mod.Mask mask)
        {
            if (!this.destroyed)
            {
                this.UpdateBinding();
            }
            LightStyle.Mod mod = this.mod;
            mod.mask &= mask;
            return mod;
        }

        public void BindToLight(Light light)
        {
            if (!this.destroyed)
            {
                this.UpdateBinding();
                this.mod.ApplyTo(light);
            }
        }

        public void BindToLight(Light light, LightStyle.Mod.Mask mask)
        {
            if (mask == (LightStyle.Mod.Mask.SpotAngle | LightStyle.Mod.Mask.Range | LightStyle.Mod.Mask.Intensity | LightStyle.Mod.Mask.Alpha | LightStyle.Mod.Mask.Blue | LightStyle.Mod.Mask.Green | LightStyle.Mod.Mask.Red))
            {
                this.BindToLight(light);
            }
            else if (!this.destroyed)
            {
                this.UpdateBinding();
                if ((this.mod.mask & mask) != this.mod.mask)
                {
                    LightStyle.Mod mod = this.mod;
                    mod.mask &= mask;
                    mod.ApplyTo(light);
                }
                else
                {
                    this.mod.ApplyTo(light);
                }
            }
        }

        public void Dispose()
        {
            if (!this.isDisposing && !this.destroyed)
            {
                this.isDisposing = true;
                bool flag = true;
                try
                {
                    flag = this.creator.DeconstructSimulation(this);
                }
                finally
                {
                    this.isDisposing = false;
                    this.destroyed = flag;
                }
            }
        }

        protected virtual void OnTimeReset()
        {
        }

        public void ResetTime(double time)
        {
            if (this.startTime != time)
            {
                this.startTime = time;
                this.OnTimeReset();
            }
        }

        protected abstract void Simulate(double currentTime);
        private void UpdateBinding()
        {
            double time = LightStyle.time;
            if (time >= this.nextBindTime)
            {
                this.Simulate(time);
                this.lastSimulateTime = time;
            }
        }

        public bool alive
        {
            get
            {
                return !this.destroyed;
            }
        }

        public bool disposed
        {
            get
            {
                return this.destroyed;
            }
        }
    }

    public abstract class Simulation<Style> : LightStyle.Simulation where Style: LightStyle
    {
        protected Simulation(Style creator) : base(creator)
        {
        }

        protected Style creator
        {
            get
            {
                return (Style) base.creator;
            }
            set
            {
                base.creator = value;
            }
        }
    }
}

