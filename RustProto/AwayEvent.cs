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
    public sealed class AwayEvent : GeneratedMessage<AwayEvent, AwayEvent.Builder>
    {
        private static readonly string[] _awayEventFieldNames = new string[] { "instigator", "timestamp", "type" };
        private static readonly uint[] _awayEventFieldTags = new uint[] { 0x18, 0x10, 8 };
        private static readonly AwayEvent defaultInstance = new AwayEvent().MakeReadOnly();
        private bool hasInstigator;
        private bool hasTimestamp;
        private bool hasType;
        private ulong instigator_;
        public const int InstigatorFieldNumber = 3;
        private int memoizedSerializedSize = -1;
        private int timestamp_;
        public const int TimestampFieldNumber = 2;
        private Types.AwayEventType type_;
        public const int TypeFieldNumber = 1;

        static AwayEvent()
        {
            object.ReferenceEquals(Avatar.Descriptor, null);
        }

        private AwayEvent()
        {
        }

        public static Builder CreateBuilder()
        {
            return new Builder();
        }

        public static Builder CreateBuilder(AwayEvent prototype)
        {
            return new Builder(prototype);
        }

        public override Builder CreateBuilderForType()
        {
            return new Builder();
        }

        private AwayEvent MakeReadOnly()
        {
            return this;
        }

        public static AwayEvent ParseDelimitedFrom(Stream input)
        {
            return CreateBuilder().MergeDelimitedFrom(input).BuildParsed();
        }

        public static AwayEvent ParseDelimitedFrom(Stream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeDelimitedFrom(input, extensionRegistry).BuildParsed();
        }

        public static AwayEvent ParseFrom(ByteString data)
        {
            return CreateBuilder().MergeFrom(data).BuildParsed();
        }

        public static AwayEvent ParseFrom(byte[] data)
        {
            return CreateBuilder().MergeFrom(data).BuildParsed();
        }

        public static AwayEvent ParseFrom(ICodedInputStream input)
        {
            return CreateBuilder().MergeFrom(input).BuildParsed();
        }

        public static AwayEvent ParseFrom(Stream input)
        {
            return CreateBuilder().MergeFrom(input).BuildParsed();
        }

        public static AwayEvent ParseFrom(ByteString data, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
        }

        public static AwayEvent ParseFrom(byte[] data, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
        }

        public static AwayEvent ParseFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
        }

        public static AwayEvent ParseFrom(Stream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
        }

        public static Recycler<AwayEvent, Builder> Recycler()
        {
            return Recycler<AwayEvent, Builder>.Manufacture();
        }

        public override Builder ToBuilder()
        {
            return CreateBuilder(this);
        }

        public override void WriteTo(ICodedOutputStream output)
        {
            int serializedSize = this.SerializedSize;
            string[] strArray = _awayEventFieldNames;
            if (this.hasType)
            {
                output.WriteEnum(1, strArray[2], (int) this.Type, this.Type);
            }
            if (this.hasTimestamp)
            {
                output.WriteInt32(2, strArray[1], this.Timestamp);
            }
            if (this.hasInstigator)
            {
                output.WriteUInt64(3, strArray[0], this.Instigator);
            }
            this.get_UnknownFields().WriteTo(output);
        }

        public static AwayEvent DefaultInstance
        {
            get
            {
                return defaultInstance;
            }
        }

        public override AwayEvent DefaultInstanceForType
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
                return Avatar.internal__static_RustProto_AwayEvent__Descriptor;
            }
        }

        public bool HasInstigator
        {
            get
            {
                return this.hasInstigator;
            }
        }

        public bool HasTimestamp
        {
            get
            {
                return this.hasTimestamp;
            }
        }

        public bool HasType
        {
            get
            {
                return this.hasType;
            }
        }

        [CLSCompliant(false)]
        public ulong Instigator
        {
            get
            {
                return this.instigator_;
            }
        }

        protected override FieldAccessorTable<AwayEvent, Builder> InternalFieldAccessors
        {
            get
            {
                return Avatar.internal__static_RustProto_AwayEvent__FieldAccessorTable;
            }
        }

        public override bool IsInitialized
        {
            get
            {
                if (!this.hasType)
                {
                    return false;
                }
                if (!this.hasTimestamp)
                {
                    return false;
                }
                return true;
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
                    if (this.hasType)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeEnumSize(1, (int) this.Type);
                    }
                    if (this.hasTimestamp)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeInt32Size(2, this.Timestamp);
                    }
                    if (this.hasInstigator)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeUInt64Size(3, this.Instigator);
                    }
                    memoizedSerializedSize += this.get_UnknownFields().get_SerializedSize();
                    this.memoizedSerializedSize = memoizedSerializedSize;
                }
                return memoizedSerializedSize;
            }
        }

        protected override AwayEvent ThisMessage
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

        public Types.AwayEventType Type
        {
            get
            {
                return this.type_;
            }
        }

        [DebuggerNonUserCode]
        public sealed class Builder : GeneratedBuilder<AwayEvent, AwayEvent.Builder>
        {
            private AwayEvent result;
            private bool resultIsReadOnly;

            public Builder()
            {
                this.result = AwayEvent.DefaultInstance;
                this.resultIsReadOnly = true;
            }

            internal Builder(AwayEvent cloneFrom)
            {
                this.result = cloneFrom;
                this.resultIsReadOnly = true;
            }

            public override AwayEvent BuildPartial()
            {
                if (this.resultIsReadOnly)
                {
                    return this.result;
                }
                this.resultIsReadOnly = true;
                return this.result.MakeReadOnly();
            }

            public override AwayEvent.Builder Clear()
            {
                this.result = AwayEvent.DefaultInstance;
                this.resultIsReadOnly = true;
                return this;
            }

            public AwayEvent.Builder ClearInstigator()
            {
                this.PrepareBuilder();
                this.result.hasInstigator = false;
                this.result.instigator_ = 0L;
                return this;
            }

            public AwayEvent.Builder ClearTimestamp()
            {
                this.PrepareBuilder();
                this.result.hasTimestamp = false;
                this.result.timestamp_ = 0;
                return this;
            }

            public AwayEvent.Builder ClearType()
            {
                this.PrepareBuilder();
                this.result.hasType = false;
                this.result.type_ = AwayEvent.Types.AwayEventType.UNKNOWN;
                return this;
            }

            public override AwayEvent.Builder Clone()
            {
                if (this.resultIsReadOnly)
                {
                    return new AwayEvent.Builder(this.result);
                }
                return new AwayEvent.Builder().MergeFrom(this.result);
            }

            public override AwayEvent.Builder MergeFrom(ICodedInputStream input)
            {
                return this.MergeFrom(input, ExtensionRegistry.get_Empty());
            }

            public override AwayEvent.Builder MergeFrom(IMessage other)
            {
                if (other is AwayEvent)
                {
                    return this.MergeFrom((AwayEvent) other);
                }
                base.MergeFrom(other);
                return this;
            }

            public override AwayEvent.Builder MergeFrom(AwayEvent other)
            {
                if (other != AwayEvent.DefaultInstance)
                {
                    this.PrepareBuilder();
                    if (other.HasType)
                    {
                        this.Type = other.Type;
                    }
                    if (other.HasTimestamp)
                    {
                        this.Timestamp = other.Timestamp;
                    }
                    if (other.HasInstigator)
                    {
                        this.Instigator = other.Instigator;
                    }
                    this.MergeUnknownFields(other.get_UnknownFields());
                }
                return this;
            }

            public override AwayEvent.Builder MergeFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
            {
                uint num;
                string str;
                this.PrepareBuilder();
                UnknownFieldSet.Builder builder = null;
                while (input.ReadTag(ref num, ref str))
                {
                    if ((num == 0) && (str != null))
                    {
                        int index = Array.BinarySearch<string>(AwayEvent._awayEventFieldNames, str, StringComparer.Ordinal);
                        if (index >= 0)
                        {
                            num = AwayEvent._awayEventFieldTags[index];
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
                            object obj2;
                            if (input.ReadEnum<AwayEvent.Types.AwayEventType>(ref this.result.type_, ref obj2))
                            {
                                this.result.hasType = true;
                            }
                            else if (obj2 is int)
                            {
                                if (builder == null)
                                {
                                    builder = UnknownFieldSet.CreateBuilder(this.get_UnknownFields());
                                }
                                builder.MergeVarintField(1, (ulong) ((int) obj2));
                            }
                            continue;
                        }
                        case 0x10:
                        {
                            this.result.hasTimestamp = input.ReadInt32(ref this.result.timestamp_);
                            continue;
                        }
                        case 0x18:
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
                    this.result.hasInstigator = input.ReadUInt64(ref this.result.instigator_);
                }
                if (builder != null)
                {
                    this.set_UnknownFields(builder.Build());
                }
                return this;
            }

            private AwayEvent PrepareBuilder()
            {
                if (this.resultIsReadOnly)
                {
                    AwayEvent result = this.result;
                    this.result = new AwayEvent();
                    this.resultIsReadOnly = false;
                    this.MergeFrom(result);
                }
                return this.result;
            }

            [CLSCompliant(false)]
            public AwayEvent.Builder SetInstigator(ulong value)
            {
                this.PrepareBuilder();
                this.result.hasInstigator = true;
                this.result.instigator_ = value;
                return this;
            }

            public AwayEvent.Builder SetTimestamp(int value)
            {
                this.PrepareBuilder();
                this.result.hasTimestamp = true;
                this.result.timestamp_ = value;
                return this;
            }

            public AwayEvent.Builder SetType(AwayEvent.Types.AwayEventType value)
            {
                this.PrepareBuilder();
                this.result.hasType = true;
                this.result.type_ = value;
                return this;
            }

            public override AwayEvent DefaultInstanceForType
            {
                get
                {
                    return AwayEvent.DefaultInstance;
                }
            }

            public override MessageDescriptor DescriptorForType
            {
                get
                {
                    return AwayEvent.Descriptor;
                }
            }

            public bool HasInstigator
            {
                get
                {
                    return this.result.hasInstigator;
                }
            }

            public bool HasTimestamp
            {
                get
                {
                    return this.result.hasTimestamp;
                }
            }

            public bool HasType
            {
                get
                {
                    return this.result.hasType;
                }
            }

            [CLSCompliant(false)]
            public ulong Instigator
            {
                get
                {
                    return this.result.Instigator;
                }
                set
                {
                    this.SetInstigator(value);
                }
            }

            public override bool IsInitialized
            {
                get
                {
                    return this.result.IsInitialized;
                }
            }

            protected override AwayEvent MessageBeingBuilt
            {
                get
                {
                    return this.PrepareBuilder();
                }
            }

            protected override AwayEvent.Builder ThisBuilder
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

            public AwayEvent.Types.AwayEventType Type
            {
                get
                {
                    return this.result.Type;
                }
                set
                {
                    this.SetType(value);
                }
            }
        }

        [DebuggerNonUserCode]
        public static class Types
        {
            public enum AwayEventType
            {
                UNKNOWN,
                SLUMBER,
                DIED
            }
        }
    }
}

