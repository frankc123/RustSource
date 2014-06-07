namespace RustProto
{
    using Google.ProtocolBuffers;
    using Google.ProtocolBuffers.Descriptors;
    using Google.ProtocolBuffers.FieldAccess;
    using RustProto.Helpers;
    using System;
    using System.Diagnostics;
    using System.IO;

    [DebuggerNonUserCode]
    public sealed class objectSleepingAvatar : GeneratedMessage<objectSleepingAvatar, objectSleepingAvatar.Builder>
    {
        private static readonly string[] _objectSleepingAvatarFieldNames = new string[] { "footArmor", "headArmor", "legArmor", "timestamp", "torsoArmor", "vitals" };
        private static readonly uint[] _objectSleepingAvatarFieldTags = new uint[] { 8, 0x20, 0x10, 40, 0x18, 50 };
        private static readonly objectSleepingAvatar defaultInstance = new objectSleepingAvatar().MakeReadOnly();
        private int footArmor_;
        public const int FootArmorFieldNumber = 1;
        private bool hasFootArmor;
        private bool hasHeadArmor;
        private bool hasLegArmor;
        private bool hasTimestamp;
        private bool hasTorsoArmor;
        private bool hasVitals;
        private int headArmor_;
        public const int HeadArmorFieldNumber = 4;
        private int legArmor_;
        public const int LegArmorFieldNumber = 2;
        private int memoizedSerializedSize = -1;
        private int timestamp_;
        public const int TimestampFieldNumber = 5;
        private int torsoArmor_;
        public const int TorsoArmorFieldNumber = 3;
        private RustProto.Vitals vitals_;
        public const int VitalsFieldNumber = 6;

        static objectSleepingAvatar()
        {
            object.ReferenceEquals(Worldsave.Descriptor, null);
        }

        private objectSleepingAvatar()
        {
        }

        public static Builder CreateBuilder()
        {
            return new Builder();
        }

        public static Builder CreateBuilder(objectSleepingAvatar prototype)
        {
            return new Builder(prototype);
        }

        public override Builder CreateBuilderForType()
        {
            return new Builder();
        }

        private objectSleepingAvatar MakeReadOnly()
        {
            return this;
        }

        public static objectSleepingAvatar ParseDelimitedFrom(Stream input)
        {
            return CreateBuilder().MergeDelimitedFrom(input).BuildParsed();
        }

        public static objectSleepingAvatar ParseDelimitedFrom(Stream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeDelimitedFrom(input, extensionRegistry).BuildParsed();
        }

        public static objectSleepingAvatar ParseFrom(ByteString data)
        {
            return CreateBuilder().MergeFrom(data).BuildParsed();
        }

        public static objectSleepingAvatar ParseFrom(byte[] data)
        {
            return CreateBuilder().MergeFrom(data).BuildParsed();
        }

        public static objectSleepingAvatar ParseFrom(ICodedInputStream input)
        {
            return CreateBuilder().MergeFrom(input).BuildParsed();
        }

        public static objectSleepingAvatar ParseFrom(Stream input)
        {
            return CreateBuilder().MergeFrom(input).BuildParsed();
        }

        public static objectSleepingAvatar ParseFrom(ByteString data, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
        }

        public static objectSleepingAvatar ParseFrom(byte[] data, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
        }

        public static objectSleepingAvatar ParseFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
        }

        public static objectSleepingAvatar ParseFrom(Stream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
        }

        public static Recycler<objectSleepingAvatar, Builder> Recycler()
        {
            return Recycler<objectSleepingAvatar, Builder>.Manufacture();
        }

        public override Builder ToBuilder()
        {
            return CreateBuilder(this);
        }

        public override void WriteTo(ICodedOutputStream output)
        {
            int serializedSize = this.SerializedSize;
            string[] strArray = _objectSleepingAvatarFieldNames;
            if (this.hasFootArmor)
            {
                output.WriteInt32(1, strArray[0], this.FootArmor);
            }
            if (this.hasLegArmor)
            {
                output.WriteInt32(2, strArray[2], this.LegArmor);
            }
            if (this.hasTorsoArmor)
            {
                output.WriteInt32(3, strArray[4], this.TorsoArmor);
            }
            if (this.hasHeadArmor)
            {
                output.WriteInt32(4, strArray[1], this.HeadArmor);
            }
            if (this.hasTimestamp)
            {
                output.WriteInt32(5, strArray[3], this.Timestamp);
            }
            if (this.hasVitals)
            {
                output.WriteMessage(6, strArray[5], this.Vitals);
            }
            this.get_UnknownFields().WriteTo(output);
        }

        public static objectSleepingAvatar DefaultInstance
        {
            get
            {
                return defaultInstance;
            }
        }

        public override objectSleepingAvatar DefaultInstanceForType
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
                return Worldsave.internal__static_RustProto_objectSleepingAvatar__Descriptor;
            }
        }

        public int FootArmor
        {
            get
            {
                return this.footArmor_;
            }
        }

        public bool HasFootArmor
        {
            get
            {
                return this.hasFootArmor;
            }
        }

        public bool HasHeadArmor
        {
            get
            {
                return this.hasHeadArmor;
            }
        }

        public bool HasLegArmor
        {
            get
            {
                return this.hasLegArmor;
            }
        }

        public bool HasTimestamp
        {
            get
            {
                return this.hasTimestamp;
            }
        }

        public bool HasTorsoArmor
        {
            get
            {
                return this.hasTorsoArmor;
            }
        }

        public bool HasVitals
        {
            get
            {
                return this.hasVitals;
            }
        }

        public int HeadArmor
        {
            get
            {
                return this.headArmor_;
            }
        }

        protected override FieldAccessorTable<objectSleepingAvatar, Builder> InternalFieldAccessors
        {
            get
            {
                return Worldsave.internal__static_RustProto_objectSleepingAvatar__FieldAccessorTable;
            }
        }

        public override bool IsInitialized
        {
            get
            {
                return true;
            }
        }

        public int LegArmor
        {
            get
            {
                return this.legArmor_;
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
                    if (this.hasFootArmor)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeInt32Size(1, this.FootArmor);
                    }
                    if (this.hasLegArmor)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeInt32Size(2, this.LegArmor);
                    }
                    if (this.hasTorsoArmor)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeInt32Size(3, this.TorsoArmor);
                    }
                    if (this.hasHeadArmor)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeInt32Size(4, this.HeadArmor);
                    }
                    if (this.hasTimestamp)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeInt32Size(5, this.Timestamp);
                    }
                    if (this.hasVitals)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeMessageSize(6, this.Vitals);
                    }
                    memoizedSerializedSize += this.get_UnknownFields().get_SerializedSize();
                    this.memoizedSerializedSize = memoizedSerializedSize;
                }
                return memoizedSerializedSize;
            }
        }

        protected override objectSleepingAvatar ThisMessage
        {
            get
            {
                return this;
            }
        }

        public int Timestamp
        {
            get
            {
                return this.timestamp_;
            }
        }

        public int TorsoArmor
        {
            get
            {
                return this.torsoArmor_;
            }
        }

        public RustProto.Vitals Vitals
        {
            get
            {
                if (this.vitals_ == null)
                {
                }
                return RustProto.Vitals.DefaultInstance;
            }
        }

        [DebuggerNonUserCode]
        public sealed class Builder : GeneratedBuilder<objectSleepingAvatar, objectSleepingAvatar.Builder>
        {
            private objectSleepingAvatar result;
            private bool resultIsReadOnly;

            public Builder()
            {
                this.result = objectSleepingAvatar.DefaultInstance;
                this.resultIsReadOnly = true;
            }

            internal Builder(objectSleepingAvatar cloneFrom)
            {
                this.result = cloneFrom;
                this.resultIsReadOnly = true;
            }

            public override objectSleepingAvatar BuildPartial()
            {
                if (this.resultIsReadOnly)
                {
                    return this.result;
                }
                this.resultIsReadOnly = true;
                return this.result.MakeReadOnly();
            }

            public override objectSleepingAvatar.Builder Clear()
            {
                this.result = objectSleepingAvatar.DefaultInstance;
                this.resultIsReadOnly = true;
                return this;
            }

            public objectSleepingAvatar.Builder ClearFootArmor()
            {
                this.PrepareBuilder();
                this.result.hasFootArmor = false;
                this.result.footArmor_ = 0;
                return this;
            }

            public objectSleepingAvatar.Builder ClearHeadArmor()
            {
                this.PrepareBuilder();
                this.result.hasHeadArmor = false;
                this.result.headArmor_ = 0;
                return this;
            }

            public objectSleepingAvatar.Builder ClearLegArmor()
            {
                this.PrepareBuilder();
                this.result.hasLegArmor = false;
                this.result.legArmor_ = 0;
                return this;
            }

            public objectSleepingAvatar.Builder ClearTimestamp()
            {
                this.PrepareBuilder();
                this.result.hasTimestamp = false;
                this.result.timestamp_ = 0;
                return this;
            }

            public objectSleepingAvatar.Builder ClearTorsoArmor()
            {
                this.PrepareBuilder();
                this.result.hasTorsoArmor = false;
                this.result.torsoArmor_ = 0;
                return this;
            }

            public objectSleepingAvatar.Builder ClearVitals()
            {
                this.PrepareBuilder();
                this.result.hasVitals = false;
                this.result.vitals_ = null;
                return this;
            }

            public override objectSleepingAvatar.Builder Clone()
            {
                if (this.resultIsReadOnly)
                {
                    return new objectSleepingAvatar.Builder(this.result);
                }
                return new objectSleepingAvatar.Builder().MergeFrom(this.result);
            }

            public override objectSleepingAvatar.Builder MergeFrom(ICodedInputStream input)
            {
                return this.MergeFrom(input, ExtensionRegistry.get_Empty());
            }

            public override objectSleepingAvatar.Builder MergeFrom(IMessage other)
            {
                if (other is objectSleepingAvatar)
                {
                    return this.MergeFrom((objectSleepingAvatar) other);
                }
                base.MergeFrom(other);
                return this;
            }

            public override objectSleepingAvatar.Builder MergeFrom(objectSleepingAvatar other)
            {
                if (other != objectSleepingAvatar.DefaultInstance)
                {
                    this.PrepareBuilder();
                    if (other.HasFootArmor)
                    {
                        this.FootArmor = other.FootArmor;
                    }
                    if (other.HasLegArmor)
                    {
                        this.LegArmor = other.LegArmor;
                    }
                    if (other.HasTorsoArmor)
                    {
                        this.TorsoArmor = other.TorsoArmor;
                    }
                    if (other.HasHeadArmor)
                    {
                        this.HeadArmor = other.HeadArmor;
                    }
                    if (other.HasTimestamp)
                    {
                        this.Timestamp = other.Timestamp;
                    }
                    if (other.HasVitals)
                    {
                        this.MergeVitals(other.Vitals);
                    }
                    this.MergeUnknownFields(other.get_UnknownFields());
                }
                return this;
            }

            public override objectSleepingAvatar.Builder MergeFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
            {
                uint num;
                string str;
                this.PrepareBuilder();
                UnknownFieldSet.Builder builder = null;
                while (input.ReadTag(ref num, ref str))
                {
                    if ((num == 0) && (str != null))
                    {
                        int index = Array.BinarySearch<string>(objectSleepingAvatar._objectSleepingAvatarFieldNames, str, StringComparer.Ordinal);
                        if (index >= 0)
                        {
                            num = objectSleepingAvatar._objectSleepingAvatarFieldTags[index];
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

                        case 8:
                        {
                            this.result.hasFootArmor = input.ReadInt32(ref this.result.footArmor_);
                            continue;
                        }
                        case 0x10:
                        {
                            this.result.hasLegArmor = input.ReadInt32(ref this.result.legArmor_);
                            continue;
                        }
                        case 0x18:
                        {
                            this.result.hasTorsoArmor = input.ReadInt32(ref this.result.torsoArmor_);
                            continue;
                        }
                        case 0x20:
                        {
                            this.result.hasHeadArmor = input.ReadInt32(ref this.result.headArmor_);
                            continue;
                        }
                        case 40:
                        {
                            this.result.hasTimestamp = input.ReadInt32(ref this.result.timestamp_);
                            continue;
                        }
                        case 50:
                        {
                            RustProto.Vitals.Builder builder2 = RustProto.Vitals.CreateBuilder();
                            if (this.result.hasVitals)
                            {
                                builder2.MergeFrom(this.Vitals);
                            }
                            input.ReadMessage(builder2, extensionRegistry);
                            this.Vitals = builder2.BuildPartial();
                            continue;
                        }
                    }
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
                }
                if (builder != null)
                {
                    this.set_UnknownFields(builder.Build());
                }
                return this;
            }

            public objectSleepingAvatar.Builder MergeVitals(RustProto.Vitals value)
            {
                ThrowHelper.ThrowIfNull(value, "value");
                this.PrepareBuilder();
                if (this.result.hasVitals && (this.result.vitals_ != RustProto.Vitals.DefaultInstance))
                {
                    this.result.vitals_ = RustProto.Vitals.CreateBuilder(this.result.vitals_).MergeFrom(value).BuildPartial();
                }
                else
                {
                    this.result.vitals_ = value;
                }
                this.result.hasVitals = true;
                return this;
            }

            private objectSleepingAvatar PrepareBuilder()
            {
                if (this.resultIsReadOnly)
                {
                    objectSleepingAvatar result = this.result;
                    this.result = new objectSleepingAvatar();
                    this.resultIsReadOnly = false;
                    this.MergeFrom(result);
                }
                return this.result;
            }

            public objectSleepingAvatar.Builder SetFootArmor(int value)
            {
                this.PrepareBuilder();
                this.result.hasFootArmor = true;
                this.result.footArmor_ = value;
                return this;
            }

            public objectSleepingAvatar.Builder SetHeadArmor(int value)
            {
                this.PrepareBuilder();
                this.result.hasHeadArmor = true;
                this.result.headArmor_ = value;
                return this;
            }

            public objectSleepingAvatar.Builder SetLegArmor(int value)
            {
                this.PrepareBuilder();
                this.result.hasLegArmor = true;
                this.result.legArmor_ = value;
                return this;
            }

            public objectSleepingAvatar.Builder SetTimestamp(int value)
            {
                this.PrepareBuilder();
                this.result.hasTimestamp = true;
                this.result.timestamp_ = value;
                return this;
            }

            public objectSleepingAvatar.Builder SetTorsoArmor(int value)
            {
                this.PrepareBuilder();
                this.result.hasTorsoArmor = true;
                this.result.torsoArmor_ = value;
                return this;
            }

            public objectSleepingAvatar.Builder SetVitals(RustProto.Vitals value)
            {
                ThrowHelper.ThrowIfNull(value, "value");
                this.PrepareBuilder();
                this.result.hasVitals = true;
                this.result.vitals_ = value;
                return this;
            }

            public objectSleepingAvatar.Builder SetVitals(RustProto.Vitals.Builder builderForValue)
            {
                ThrowHelper.ThrowIfNull(builderForValue, "builderForValue");
                this.PrepareBuilder();
                this.result.hasVitals = true;
                this.result.vitals_ = builderForValue.Build();
                return this;
            }

            public override objectSleepingAvatar DefaultInstanceForType
            {
                get
                {
                    return objectSleepingAvatar.DefaultInstance;
                }
            }

            public override MessageDescriptor DescriptorForType
            {
                get
                {
                    return objectSleepingAvatar.Descriptor;
                }
            }

            public int FootArmor
            {
                get
                {
                    return this.result.FootArmor;
                }
                set
                {
                    this.SetFootArmor(value);
                }
            }

            public bool HasFootArmor
            {
                get
                {
                    return this.result.hasFootArmor;
                }
            }

            public bool HasHeadArmor
            {
                get
                {
                    return this.result.hasHeadArmor;
                }
            }

            public bool HasLegArmor
            {
                get
                {
                    return this.result.hasLegArmor;
                }
            }

            public bool HasTimestamp
            {
                get
                {
                    return this.result.hasTimestamp;
                }
            }

            public bool HasTorsoArmor
            {
                get
                {
                    return this.result.hasTorsoArmor;
                }
            }

            public bool HasVitals
            {
                get
                {
                    return this.result.hasVitals;
                }
            }

            public int HeadArmor
            {
                get
                {
                    return this.result.HeadArmor;
                }
                set
                {
                    this.SetHeadArmor(value);
                }
            }

            public override bool IsInitialized
            {
                get
                {
                    return this.result.IsInitialized;
                }
            }

            public int LegArmor
            {
                get
                {
                    return this.result.LegArmor;
                }
                set
                {
                    this.SetLegArmor(value);
                }
            }

            protected override objectSleepingAvatar MessageBeingBuilt
            {
                get
                {
                    return this.PrepareBuilder();
                }
            }

            protected override objectSleepingAvatar.Builder ThisBuilder
            {
                get
                {
                    return this;
                }
            }

            public int Timestamp
            {
                get
                {
                    return this.result.Timestamp;
                }
                set
                {
                    this.SetTimestamp(value);
                }
            }

            public int TorsoArmor
            {
                get
                {
                    return this.result.TorsoArmor;
                }
                set
                {
                    this.SetTorsoArmor(value);
                }
            }

            public RustProto.Vitals Vitals
            {
                get
                {
                    return this.result.Vitals;
                }
                set
                {
                    this.SetVitals(value);
                }
            }
        }
    }
}

