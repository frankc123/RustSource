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
    public sealed class objectStructMaster : GeneratedMessage<objectStructMaster, objectStructMaster.Builder>
    {
        private static readonly string[] _objectStructMasterFieldNames = new string[] { "CreatorID", "DecayDelay", "ID", "OwnerID" };
        private static readonly uint[] _objectStructMasterFieldTags = new uint[] { 0x18, 0x15, 8, 0x20 };
        private ulong creatorID_;
        public const int CreatorIDFieldNumber = 3;
        private float decayDelay_;
        public const int DecayDelayFieldNumber = 2;
        private static readonly objectStructMaster defaultInstance = new objectStructMaster().MakeReadOnly();
        private bool hasCreatorID;
        private bool hasDecayDelay;
        private bool hasID;
        private bool hasOwnerID;
        private int iD_;
        public const int IDFieldNumber = 1;
        private int memoizedSerializedSize = -1;
        private ulong ownerID_;
        public const int OwnerIDFieldNumber = 4;

        static objectStructMaster()
        {
            object.ReferenceEquals(Worldsave.Descriptor, null);
        }

        private objectStructMaster()
        {
        }

        public static Builder CreateBuilder()
        {
            return new Builder();
        }

        public static Builder CreateBuilder(objectStructMaster prototype)
        {
            return new Builder(prototype);
        }

        public override Builder CreateBuilderForType()
        {
            return new Builder();
        }

        private objectStructMaster MakeReadOnly()
        {
            return this;
        }

        public static objectStructMaster ParseDelimitedFrom(Stream input)
        {
            return CreateBuilder().MergeDelimitedFrom(input).BuildParsed();
        }

        public static objectStructMaster ParseDelimitedFrom(Stream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeDelimitedFrom(input, extensionRegistry).BuildParsed();
        }

        public static objectStructMaster ParseFrom(ByteString data)
        {
            return CreateBuilder().MergeFrom(data).BuildParsed();
        }

        public static objectStructMaster ParseFrom(byte[] data)
        {
            return CreateBuilder().MergeFrom(data).BuildParsed();
        }

        public static objectStructMaster ParseFrom(ICodedInputStream input)
        {
            return CreateBuilder().MergeFrom(input).BuildParsed();
        }

        public static objectStructMaster ParseFrom(Stream input)
        {
            return CreateBuilder().MergeFrom(input).BuildParsed();
        }

        public static objectStructMaster ParseFrom(ByteString data, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
        }

        public static objectStructMaster ParseFrom(byte[] data, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
        }

        public static objectStructMaster ParseFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
        }

        public static objectStructMaster ParseFrom(Stream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
        }

        public static Recycler<objectStructMaster, Builder> Recycler()
        {
            return Recycler<objectStructMaster, Builder>.Manufacture();
        }

        public override Builder ToBuilder()
        {
            return CreateBuilder(this);
        }

        public override void WriteTo(ICodedOutputStream output)
        {
            int serializedSize = this.SerializedSize;
            string[] strArray = _objectStructMasterFieldNames;
            if (this.hasID)
            {
                output.WriteInt32(1, strArray[2], this.ID);
            }
            if (this.hasDecayDelay)
            {
                output.WriteFloat(2, strArray[1], this.DecayDelay);
            }
            if (this.hasCreatorID)
            {
                output.WriteUInt64(3, strArray[0], this.CreatorID);
            }
            if (this.hasOwnerID)
            {
                output.WriteUInt64(4, strArray[3], this.OwnerID);
            }
            this.get_UnknownFields().WriteTo(output);
        }

        [CLSCompliant(false)]
        public ulong CreatorID
        {
            get
            {
                return this.creatorID_;
            }
        }

        public float DecayDelay
        {
            get
            {
                return this.decayDelay_;
            }
        }

        public static objectStructMaster DefaultInstance
        {
            get
            {
                return defaultInstance;
            }
        }

        public override objectStructMaster DefaultInstanceForType
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
                return Worldsave.internal__static_RustProto_objectStructMaster__Descriptor;
            }
        }

        public bool HasCreatorID
        {
            get
            {
                return this.hasCreatorID;
            }
        }

        public bool HasDecayDelay
        {
            get
            {
                return this.hasDecayDelay;
            }
        }

        public bool HasID
        {
            get
            {
                return this.hasID;
            }
        }

        public bool HasOwnerID
        {
            get
            {
                return this.hasOwnerID;
            }
        }

        public int ID
        {
            get
            {
                return this.iD_;
            }
        }

        protected override FieldAccessorTable<objectStructMaster, Builder> InternalFieldAccessors
        {
            get
            {
                return Worldsave.internal__static_RustProto_objectStructMaster__FieldAccessorTable;
            }
        }

        public override bool IsInitialized
        {
            get
            {
                return true;
            }
        }

        [CLSCompliant(false)]
        public ulong OwnerID
        {
            get
            {
                return this.ownerID_;
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
                    if (this.hasID)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeInt32Size(1, this.ID);
                    }
                    if (this.hasDecayDelay)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeFloatSize(2, this.DecayDelay);
                    }
                    if (this.hasCreatorID)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeUInt64Size(3, this.CreatorID);
                    }
                    if (this.hasOwnerID)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeUInt64Size(4, this.OwnerID);
                    }
                    memoizedSerializedSize += this.get_UnknownFields().get_SerializedSize();
                    this.memoizedSerializedSize = memoizedSerializedSize;
                }
                return memoizedSerializedSize;
            }
        }

        protected override objectStructMaster ThisMessage
        {
            get
            {
                return this;
            }
        }

        [DebuggerNonUserCode]
        public sealed class Builder : GeneratedBuilder<objectStructMaster, objectStructMaster.Builder>
        {
            private objectStructMaster result;
            private bool resultIsReadOnly;

            public Builder()
            {
                this.result = objectStructMaster.DefaultInstance;
                this.resultIsReadOnly = true;
            }

            internal Builder(objectStructMaster cloneFrom)
            {
                this.result = cloneFrom;
                this.resultIsReadOnly = true;
            }

            public override objectStructMaster BuildPartial()
            {
                if (this.resultIsReadOnly)
                {
                    return this.result;
                }
                this.resultIsReadOnly = true;
                return this.result.MakeReadOnly();
            }

            public override objectStructMaster.Builder Clear()
            {
                this.result = objectStructMaster.DefaultInstance;
                this.resultIsReadOnly = true;
                return this;
            }

            public objectStructMaster.Builder ClearCreatorID()
            {
                this.PrepareBuilder();
                this.result.hasCreatorID = false;
                this.result.creatorID_ = 0L;
                return this;
            }

            public objectStructMaster.Builder ClearDecayDelay()
            {
                this.PrepareBuilder();
                this.result.hasDecayDelay = false;
                this.result.decayDelay_ = 0f;
                return this;
            }

            public objectStructMaster.Builder ClearID()
            {
                this.PrepareBuilder();
                this.result.hasID = false;
                this.result.iD_ = 0;
                return this;
            }

            public objectStructMaster.Builder ClearOwnerID()
            {
                this.PrepareBuilder();
                this.result.hasOwnerID = false;
                this.result.ownerID_ = 0L;
                return this;
            }

            public override objectStructMaster.Builder Clone()
            {
                if (this.resultIsReadOnly)
                {
                    return new objectStructMaster.Builder(this.result);
                }
                return new objectStructMaster.Builder().MergeFrom(this.result);
            }

            public override objectStructMaster.Builder MergeFrom(ICodedInputStream input)
            {
                return this.MergeFrom(input, ExtensionRegistry.get_Empty());
            }

            public override objectStructMaster.Builder MergeFrom(IMessage other)
            {
                if (other is objectStructMaster)
                {
                    return this.MergeFrom((objectStructMaster) other);
                }
                base.MergeFrom(other);
                return this;
            }

            public override objectStructMaster.Builder MergeFrom(objectStructMaster other)
            {
                if (other != objectStructMaster.DefaultInstance)
                {
                    this.PrepareBuilder();
                    if (other.HasID)
                    {
                        this.ID = other.ID;
                    }
                    if (other.HasDecayDelay)
                    {
                        this.DecayDelay = other.DecayDelay;
                    }
                    if (other.HasCreatorID)
                    {
                        this.CreatorID = other.CreatorID;
                    }
                    if (other.HasOwnerID)
                    {
                        this.OwnerID = other.OwnerID;
                    }
                    this.MergeUnknownFields(other.get_UnknownFields());
                }
                return this;
            }

            public override objectStructMaster.Builder MergeFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
            {
                uint num;
                string str;
                this.PrepareBuilder();
                UnknownFieldSet.Builder builder = null;
                while (input.ReadTag(ref num, ref str))
                {
                    if ((num == 0) && (str != null))
                    {
                        int index = Array.BinarySearch<string>(objectStructMaster._objectStructMasterFieldNames, str, StringComparer.Ordinal);
                        if (index >= 0)
                        {
                            num = objectStructMaster._objectStructMasterFieldTags[index];
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
                        case 0x15:
                        {
                            this.result.hasDecayDelay = input.ReadFloat(ref this.result.decayDelay_);
                            continue;
                        }
                        case 0x18:
                        {
                            this.result.hasCreatorID = input.ReadUInt64(ref this.result.creatorID_);
                            continue;
                        }
                        case 0:
                            throw InvalidProtocolBufferException.InvalidTag();

                        case 8:
                            break;

                        case 0x20:
                            goto Label_0148;

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
                    this.result.hasID = input.ReadInt32(ref this.result.iD_);
                    continue;
                Label_0148:
                    this.result.hasOwnerID = input.ReadUInt64(ref this.result.ownerID_);
                }
                if (builder != null)
                {
                    this.set_UnknownFields(builder.Build());
                }
                return this;
            }

            private objectStructMaster PrepareBuilder()
            {
                if (this.resultIsReadOnly)
                {
                    objectStructMaster result = this.result;
                    this.result = new objectStructMaster();
                    this.resultIsReadOnly = false;
                    this.MergeFrom(result);
                }
                return this.result;
            }

            [CLSCompliant(false)]
            public objectStructMaster.Builder SetCreatorID(ulong value)
            {
                this.PrepareBuilder();
                this.result.hasCreatorID = true;
                this.result.creatorID_ = value;
                return this;
            }

            public objectStructMaster.Builder SetDecayDelay(float value)
            {
                this.PrepareBuilder();
                this.result.hasDecayDelay = true;
                this.result.decayDelay_ = value;
                return this;
            }

            public objectStructMaster.Builder SetID(int value)
            {
                this.PrepareBuilder();
                this.result.hasID = true;
                this.result.iD_ = value;
                return this;
            }

            [CLSCompliant(false)]
            public objectStructMaster.Builder SetOwnerID(ulong value)
            {
                this.PrepareBuilder();
                this.result.hasOwnerID = true;
                this.result.ownerID_ = value;
                return this;
            }

            [CLSCompliant(false)]
            public ulong CreatorID
            {
                get
                {
                    return this.result.CreatorID;
                }
                set
                {
                    this.SetCreatorID(value);
                }
            }

            public float DecayDelay
            {
                get
                {
                    return this.result.DecayDelay;
                }
                set
                {
                    this.SetDecayDelay(value);
                }
            }

            public override objectStructMaster DefaultInstanceForType
            {
                get
                {
                    return objectStructMaster.DefaultInstance;
                }
            }

            public override MessageDescriptor DescriptorForType
            {
                get
                {
                    return objectStructMaster.Descriptor;
                }
            }

            public bool HasCreatorID
            {
                get
                {
                    return this.result.hasCreatorID;
                }
            }

            public bool HasDecayDelay
            {
                get
                {
                    return this.result.hasDecayDelay;
                }
            }

            public bool HasID
            {
                get
                {
                    return this.result.hasID;
                }
            }

            public bool HasOwnerID
            {
                get
                {
                    return this.result.hasOwnerID;
                }
            }

            public int ID
            {
                get
                {
                    return this.result.ID;
                }
                set
                {
                    this.SetID(value);
                }
            }

            public override bool IsInitialized
            {
                get
                {
                    return this.result.IsInitialized;
                }
            }

            protected override objectStructMaster MessageBeingBuilt
            {
                get
                {
                    return this.PrepareBuilder();
                }
            }

            [CLSCompliant(false)]
            public ulong OwnerID
            {
                get
                {
                    return this.result.OwnerID;
                }
                set
                {
                    this.SetOwnerID(value);
                }
            }

            protected override objectStructMaster.Builder ThisBuilder
            {
                get
                {
                    return this;
                }
            }
        }
    }
}

