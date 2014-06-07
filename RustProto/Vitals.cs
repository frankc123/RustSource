namespace RustProto
{
    using Google.ProtocolBuffers;
    using Google.ProtocolBuffers.Descriptors;
    using Google.ProtocolBuffers.FieldAccess;
    using RustProto.Helpers;
    using RustProto.Proto;
    using System;
    using System.Diagnostics;
    using System.IO;

    [DebuggerNonUserCode]
    public sealed class Vitals : GeneratedMessage<Vitals, Vitals.Builder>
    {
        private static readonly string[] _vitalsFieldNames = new string[] { "bleed_max", "bleed_speed", "calories", "heal_max", "heal_speed", "health", "hydration", "radiation", "radiation_anti", "temperature" };
        private static readonly uint[] _vitalsFieldTags = new uint[] { 0x3d, 0x35, 0x1d, 0x4d, 0x45, 13, 0x15, 0x25, 0x2d, 0x55 };
        private float bleedMax_;
        public const int BleedMaxFieldNumber = 7;
        private float bleedSpeed_;
        public const int BleedSpeedFieldNumber = 6;
        private float calories_ = 1000f;
        public const int CaloriesFieldNumber = 3;
        private static readonly Vitals defaultInstance = new Vitals().MakeReadOnly();
        private bool hasBleedMax;
        private bool hasBleedSpeed;
        private bool hasCalories;
        private bool hasHealMax;
        private bool hasHealSpeed;
        private bool hasHealth;
        private bool hasHydration;
        private bool hasRadiation;
        private bool hasRadiationAnti;
        private bool hasTemperature;
        private float healMax_;
        public const int HealMaxFieldNumber = 9;
        private float healSpeed_;
        public const int HealSpeedFieldNumber = 8;
        private float health_ = 100f;
        public const int HealthFieldNumber = 1;
        private float hydration_ = 30f;
        public const int HydrationFieldNumber = 2;
        private int memoizedSerializedSize = -1;
        private float radiation_;
        private float radiationAnti_;
        public const int RadiationAntiFieldNumber = 5;
        public const int RadiationFieldNumber = 4;
        private float temperature_;
        public const int TemperatureFieldNumber = 10;

        static Vitals()
        {
            object.ReferenceEquals(Vitals.Descriptor, null);
        }

        private Vitals()
        {
        }

        public static Builder CreateBuilder()
        {
            return new Builder();
        }

        public static Builder CreateBuilder(Vitals prototype)
        {
            return new Builder(prototype);
        }

        public override Builder CreateBuilderForType()
        {
            return new Builder();
        }

        private Vitals MakeReadOnly()
        {
            return this;
        }

        public static Vitals ParseDelimitedFrom(Stream input)
        {
            return CreateBuilder().MergeDelimitedFrom(input).BuildParsed();
        }

        public static Vitals ParseDelimitedFrom(Stream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeDelimitedFrom(input, extensionRegistry).BuildParsed();
        }

        public static Vitals ParseFrom(ByteString data)
        {
            return CreateBuilder().MergeFrom(data).BuildParsed();
        }

        public static Vitals ParseFrom(byte[] data)
        {
            return CreateBuilder().MergeFrom(data).BuildParsed();
        }

        public static Vitals ParseFrom(ICodedInputStream input)
        {
            return CreateBuilder().MergeFrom(input).BuildParsed();
        }

        public static Vitals ParseFrom(Stream input)
        {
            return CreateBuilder().MergeFrom(input).BuildParsed();
        }

        public static Vitals ParseFrom(ByteString data, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
        }

        public static Vitals ParseFrom(byte[] data, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
        }

        public static Vitals ParseFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
        }

        public static Vitals ParseFrom(Stream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
        }

        public static Recycler<Vitals, Builder> Recycler()
        {
            return Recycler<Vitals, Builder>.Manufacture();
        }

        public override Builder ToBuilder()
        {
            return CreateBuilder(this);
        }

        public override void WriteTo(ICodedOutputStream output)
        {
            int serializedSize = this.SerializedSize;
            string[] strArray = _vitalsFieldNames;
            if (this.hasHealth)
            {
                output.WriteFloat(1, strArray[5], this.Health);
            }
            if (this.hasHydration)
            {
                output.WriteFloat(2, strArray[6], this.Hydration);
            }
            if (this.hasCalories)
            {
                output.WriteFloat(3, strArray[2], this.Calories);
            }
            if (this.hasRadiation)
            {
                output.WriteFloat(4, strArray[7], this.Radiation);
            }
            if (this.hasRadiationAnti)
            {
                output.WriteFloat(5, strArray[8], this.RadiationAnti);
            }
            if (this.hasBleedSpeed)
            {
                output.WriteFloat(6, strArray[1], this.BleedSpeed);
            }
            if (this.hasBleedMax)
            {
                output.WriteFloat(7, strArray[0], this.BleedMax);
            }
            if (this.hasHealSpeed)
            {
                output.WriteFloat(8, strArray[4], this.HealSpeed);
            }
            if (this.hasHealMax)
            {
                output.WriteFloat(9, strArray[3], this.HealMax);
            }
            if (this.hasTemperature)
            {
                output.WriteFloat(10, strArray[9], this.Temperature);
            }
            this.get_UnknownFields().WriteTo(output);
        }

        public float BleedMax
        {
            get
            {
                return this.bleedMax_;
            }
        }

        public float BleedSpeed
        {
            get
            {
                return this.bleedSpeed_;
            }
        }

        public float Calories
        {
            get
            {
                return this.calories_;
            }
        }

        public static Vitals DefaultInstance
        {
            get
            {
                return defaultInstance;
            }
        }

        public override Vitals DefaultInstanceForType
        {
            get
            {
                return DefaultInstance;
            }
        }

        public static MessageDescriptor Descriptor
        {
            get
            {
                return Vitals.internal__static_RustProto_Vitals__Descriptor;
            }
        }

        public bool HasBleedMax
        {
            get
            {
                return this.hasBleedMax;
            }
        }

        public bool HasBleedSpeed
        {
            get
            {
                return this.hasBleedSpeed;
            }
        }

        public bool HasCalories
        {
            get
            {
                return this.hasCalories;
            }
        }

        public bool HasHealMax
        {
            get
            {
                return this.hasHealMax;
            }
        }

        public bool HasHealSpeed
        {
            get
            {
                return this.hasHealSpeed;
            }
        }

        public bool HasHealth
        {
            get
            {
                return this.hasHealth;
            }
        }

        public bool HasHydration
        {
            get
            {
                return this.hasHydration;
            }
        }

        public bool HasRadiation
        {
            get
            {
                return this.hasRadiation;
            }
        }

        public bool HasRadiationAnti
        {
            get
            {
                return this.hasRadiationAnti;
            }
        }

        public bool HasTemperature
        {
            get
            {
                return this.hasTemperature;
            }
        }

        public float HealMax
        {
            get
            {
                return this.healMax_;
            }
        }

        public float HealSpeed
        {
            get
            {
                return this.healSpeed_;
            }
        }

        public float Health
        {
            get
            {
                return this.health_;
            }
        }

        public float Hydration
        {
            get
            {
                return this.hydration_;
            }
        }

        protected override FieldAccessorTable<Vitals, Builder> InternalFieldAccessors
        {
            get
            {
                return Vitals.internal__static_RustProto_Vitals__FieldAccessorTable;
            }
        }

        public override bool IsInitialized
        {
            get
            {
                return true;
            }
        }

        public float Radiation
        {
            get
            {
                return this.radiation_;
            }
        }

        public float RadiationAnti
        {
            get
            {
                return this.radiationAnti_;
            }
        }

        public override int SerializedSize
        {
            get
            {
                int memoizedSerializedSize = this.memoizedSerializedSize;
                if (memoizedSerializedSize == -1)
                {
                    memoizedSerializedSize = 0;
                    if (this.hasHealth)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeFloatSize(1, this.Health);
                    }
                    if (this.hasHydration)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeFloatSize(2, this.Hydration);
                    }
                    if (this.hasCalories)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeFloatSize(3, this.Calories);
                    }
                    if (this.hasRadiation)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeFloatSize(4, this.Radiation);
                    }
                    if (this.hasRadiationAnti)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeFloatSize(5, this.RadiationAnti);
                    }
                    if (this.hasBleedSpeed)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeFloatSize(6, this.BleedSpeed);
                    }
                    if (this.hasBleedMax)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeFloatSize(7, this.BleedMax);
                    }
                    if (this.hasHealSpeed)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeFloatSize(8, this.HealSpeed);
                    }
                    if (this.hasHealMax)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeFloatSize(9, this.HealMax);
                    }
                    if (this.hasTemperature)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeFloatSize(10, this.Temperature);
                    }
                    memoizedSerializedSize += this.get_UnknownFields().get_SerializedSize();
                    this.memoizedSerializedSize = memoizedSerializedSize;
                }
                return memoizedSerializedSize;
            }
        }

        public float Temperature
        {
            get
            {
                return this.temperature_;
            }
        }

        protected override Vitals ThisMessage
        {
            get
            {
                return this;
            }
        }

        [DebuggerNonUserCode]
        public sealed class Builder : GeneratedBuilder<Vitals, Vitals.Builder>
        {
            private Vitals result;
            private bool resultIsReadOnly;

            public Builder()
            {
                this.result = Vitals.DefaultInstance;
                this.resultIsReadOnly = true;
            }

            internal Builder(Vitals cloneFrom)
            {
                this.result = cloneFrom;
                this.resultIsReadOnly = true;
            }

            public override Vitals BuildPartial()
            {
                if (this.resultIsReadOnly)
                {
                    return this.result;
                }
                this.resultIsReadOnly = true;
                return this.result.MakeReadOnly();
            }

            public override Vitals.Builder Clear()
            {
                this.result = Vitals.DefaultInstance;
                this.resultIsReadOnly = true;
                return this;
            }

            public Vitals.Builder ClearBleedMax()
            {
                this.PrepareBuilder();
                this.result.hasBleedMax = false;
                this.result.bleedMax_ = 0f;
                return this;
            }

            public Vitals.Builder ClearBleedSpeed()
            {
                this.PrepareBuilder();
                this.result.hasBleedSpeed = false;
                this.result.bleedSpeed_ = 0f;
                return this;
            }

            public Vitals.Builder ClearCalories()
            {
                this.PrepareBuilder();
                this.result.hasCalories = false;
                this.result.calories_ = 1000f;
                return this;
            }

            public Vitals.Builder ClearHealMax()
            {
                this.PrepareBuilder();
                this.result.hasHealMax = false;
                this.result.healMax_ = 0f;
                return this;
            }

            public Vitals.Builder ClearHealSpeed()
            {
                this.PrepareBuilder();
                this.result.hasHealSpeed = false;
                this.result.healSpeed_ = 0f;
                return this;
            }

            public Vitals.Builder ClearHealth()
            {
                this.PrepareBuilder();
                this.result.hasHealth = false;
                this.result.health_ = 100f;
                return this;
            }

            public Vitals.Builder ClearHydration()
            {
                this.PrepareBuilder();
                this.result.hasHydration = false;
                this.result.hydration_ = 30f;
                return this;
            }

            public Vitals.Builder ClearRadiation()
            {
                this.PrepareBuilder();
                this.result.hasRadiation = false;
                this.result.radiation_ = 0f;
                return this;
            }

            public Vitals.Builder ClearRadiationAnti()
            {
                this.PrepareBuilder();
                this.result.hasRadiationAnti = false;
                this.result.radiationAnti_ = 0f;
                return this;
            }

            public Vitals.Builder ClearTemperature()
            {
                this.PrepareBuilder();
                this.result.hasTemperature = false;
                this.result.temperature_ = 0f;
                return this;
            }

            public override Vitals.Builder Clone()
            {
                if (this.resultIsReadOnly)
                {
                    return new Vitals.Builder(this.result);
                }
                return new Vitals.Builder().MergeFrom(this.result);
            }

            public override Vitals.Builder MergeFrom(ICodedInputStream input)
            {
                return this.MergeFrom(input, ExtensionRegistry.get_Empty());
            }

            public override Vitals.Builder MergeFrom(IMessage other)
            {
                if (other is Vitals)
                {
                    return this.MergeFrom((Vitals) other);
                }
                base.MergeFrom(other);
                return this;
            }

            public override Vitals.Builder MergeFrom(Vitals other)
            {
                if (other != Vitals.DefaultInstance)
                {
                    this.PrepareBuilder();
                    if (other.HasHealth)
                    {
                        this.Health = other.Health;
                    }
                    if (other.HasHydration)
                    {
                        this.Hydration = other.Hydration;
                    }
                    if (other.HasCalories)
                    {
                        this.Calories = other.Calories;
                    }
                    if (other.HasRadiation)
                    {
                        this.Radiation = other.Radiation;
                    }
                    if (other.HasRadiationAnti)
                    {
                        this.RadiationAnti = other.RadiationAnti;
                    }
                    if (other.HasBleedSpeed)
                    {
                        this.BleedSpeed = other.BleedSpeed;
                    }
                    if (other.HasBleedMax)
                    {
                        this.BleedMax = other.BleedMax;
                    }
                    if (other.HasHealSpeed)
                    {
                        this.HealSpeed = other.HealSpeed;
                    }
                    if (other.HasHealMax)
                    {
                        this.HealMax = other.HealMax;
                    }
                    if (other.HasTemperature)
                    {
                        this.Temperature = other.Temperature;
                    }
                    this.MergeUnknownFields(other.get_UnknownFields());
                }
                return this;
            }

            public override Vitals.Builder MergeFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
            {
                uint num;
                string str;
                this.PrepareBuilder();
                UnknownFieldSet.Builder builder = null;
                while (input.ReadTag(ref num, ref str))
                {
                    if ((num == 0) && (str != null))
                    {
                        int index = Array.BinarySearch<string>(Vitals._vitalsFieldNames, str, StringComparer.Ordinal);
                        if (index >= 0)
                        {
                            num = Vitals._vitalsFieldTags[index];
                        }
                        else
                        {
                            if (builder == null)
                            {
                                builder = UnknownFieldSet.CreateBuilder(this.get_UnknownFields());
                            }
                            this.ParseUnknownField(input, builder, extensionRegistry, num, str);
                            continue;
                        }
                    }
                    switch (num)
                    {
                        case 0:
                            throw InvalidProtocolBufferException.InvalidTag();

                        case 13:
                        {
                            this.result.hasHealth = input.ReadFloat(ref this.result.health_);
                            continue;
                        }
                        case 0x15:
                        {
                            this.result.hasHydration = input.ReadFloat(ref this.result.hydration_);
                            continue;
                        }
                        case 0x1d:
                        {
                            this.result.hasCalories = input.ReadFloat(ref this.result.calories_);
                            continue;
                        }
                        case 0x25:
                        {
                            this.result.hasRadiation = input.ReadFloat(ref this.result.radiation_);
                            continue;
                        }
                        case 0x2d:
                        {
                            this.result.hasRadiationAnti = input.ReadFloat(ref this.result.radiationAnti_);
                            continue;
                        }
                        case 0x35:
                        {
                            this.result.hasBleedSpeed = input.ReadFloat(ref this.result.bleedSpeed_);
                            continue;
                        }
                        case 0x3d:
                        {
                            this.result.hasBleedMax = input.ReadFloat(ref this.result.bleedMax_);
                            continue;
                        }
                        case 0x45:
                        {
                            this.result.hasHealSpeed = input.ReadFloat(ref this.result.healSpeed_);
                            continue;
                        }
                        case 0x4d:
                        {
                            this.result.hasHealMax = input.ReadFloat(ref this.result.healMax_);
                            continue;
                        }
                        case 0x55:
                            break;

                        default:
                        {
                            if (WireFormat.IsEndGroupTag(num))
                            {
                                if (builder != null)
                                {
                                    this.set_UnknownFields(builder.Build());
                                }
                                return this;
                            }
                            if (builder == null)
                            {
                                builder = UnknownFieldSet.CreateBuilder(this.get_UnknownFields());
                            }
                            this.ParseUnknownField(input, builder, extensionRegistry, num, str);
                            continue;
                        }
                    }
                    this.result.hasTemperature = input.ReadFloat(ref this.result.temperature_);
                }
                if (builder != null)
                {
                    this.set_UnknownFields(builder.Build());
                }
                return this;
            }

            private Vitals PrepareBuilder()
            {
                if (this.resultIsReadOnly)
                {
                    Vitals result = this.result;
                    this.result = new Vitals();
                    this.resultIsReadOnly = false;
                    this.MergeFrom(result);
                }
                return this.result;
            }

            public Vitals.Builder SetBleedMax(float value)
            {
                this.PrepareBuilder();
                this.result.hasBleedMax = true;
                this.result.bleedMax_ = value;
                return this;
            }

            public Vitals.Builder SetBleedSpeed(float value)
            {
                this.PrepareBuilder();
                this.result.hasBleedSpeed = true;
                this.result.bleedSpeed_ = value;
                return this;
            }

            public Vitals.Builder SetCalories(float value)
            {
                this.PrepareBuilder();
                this.result.hasCalories = true;
                this.result.calories_ = value;
                return this;
            }

            public Vitals.Builder SetHealMax(float value)
            {
                this.PrepareBuilder();
                this.result.hasHealMax = true;
                this.result.healMax_ = value;
                return this;
            }

            public Vitals.Builder SetHealSpeed(float value)
            {
                this.PrepareBuilder();
                this.result.hasHealSpeed = true;
                this.result.healSpeed_ = value;
                return this;
            }

            public Vitals.Builder SetHealth(float value)
            {
                this.PrepareBuilder();
                this.result.hasHealth = true;
                this.result.health_ = value;
                return this;
            }

            public Vitals.Builder SetHydration(float value)
            {
                this.PrepareBuilder();
                this.result.hasHydration = true;
                this.result.hydration_ = value;
                return this;
            }

            public Vitals.Builder SetRadiation(float value)
            {
                this.PrepareBuilder();
                this.result.hasRadiation = true;
                this.result.radiation_ = value;
                return this;
            }

            public Vitals.Builder SetRadiationAnti(float value)
            {
                this.PrepareBuilder();
                this.result.hasRadiationAnti = true;
                this.result.radiationAnti_ = value;
                return this;
            }

            public Vitals.Builder SetTemperature(float value)
            {
                this.PrepareBuilder();
                this.result.hasTemperature = true;
                this.result.temperature_ = value;
                return this;
            }

            public float BleedMax
            {
                get
                {
                    return this.result.BleedMax;
                }
                set
                {
                    this.SetBleedMax(value);
                }
            }

            public float BleedSpeed
            {
                get
                {
                    return this.result.BleedSpeed;
                }
                set
                {
                    this.SetBleedSpeed(value);
                }
            }

            public float Calories
            {
                get
                {
                    return this.result.Calories;
                }
                set
                {
                    this.SetCalories(value);
                }
            }

            public override Vitals DefaultInstanceForType
            {
                get
                {
                    return Vitals.DefaultInstance;
                }
            }

            public override MessageDescriptor DescriptorForType
            {
                get
                {
                    return Vitals.Descriptor;
                }
            }

            public bool HasBleedMax
            {
                get
                {
                    return this.result.hasBleedMax;
                }
            }

            public bool HasBleedSpeed
            {
                get
                {
                    return this.result.hasBleedSpeed;
                }
            }

            public bool HasCalories
            {
                get
                {
                    return this.result.hasCalories;
                }
            }

            public bool HasHealMax
            {
                get
                {
                    return this.result.hasHealMax;
                }
            }

            public bool HasHealSpeed
            {
                get
                {
                    return this.result.hasHealSpeed;
                }
            }

            public bool HasHealth
            {
                get
                {
                    return this.result.hasHealth;
                }
            }

            public bool HasHydration
            {
                get
                {
                    return this.result.hasHydration;
                }
            }

            public bool HasRadiation
            {
                get
                {
                    return this.result.hasRadiation;
                }
            }

            public bool HasRadiationAnti
            {
                get
                {
                    return this.result.hasRadiationAnti;
                }
            }

            public bool HasTemperature
            {
                get
                {
                    return this.result.hasTemperature;
                }
            }

            public float HealMax
            {
                get
                {
                    return this.result.HealMax;
                }
                set
                {
                    this.SetHealMax(value);
                }
            }

            public float HealSpeed
            {
                get
                {
                    return this.result.HealSpeed;
                }
                set
                {
                    this.SetHealSpeed(value);
                }
            }

            public float Health
            {
                get
                {
                    return this.result.Health;
                }
                set
                {
                    this.SetHealth(value);
                }
            }

            public float Hydration
            {
                get
                {
                    return this.result.Hydration;
                }
                set
                {
                    this.SetHydration(value);
                }
            }

            public override bool IsInitialized
            {
                get
                {
                    return this.result.IsInitialized;
                }
            }

            protected override Vitals MessageBeingBuilt
            {
                get
                {
                    return this.PrepareBuilder();
                }
            }

            public float Radiation
            {
                get
                {
                    return this.result.Radiation;
                }
                set
                {
                    this.SetRadiation(value);
                }
            }

            public float RadiationAnti
            {
                get
                {
                    return this.result.RadiationAnti;
                }
                set
                {
                    this.SetRadiationAnti(value);
                }
            }

            public float Temperature
            {
                get
                {
                    return this.result.Temperature;
                }
                set
                {
                    this.SetTemperature(value);
                }
            }

            protected override Vitals.Builder ThisBuilder
            {
                get
                {
                    return this;
                }
            }
        }
    }
}

