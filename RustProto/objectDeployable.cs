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
    public sealed class objectDeployable : GeneratedMessage<objectDeployable, objectDeployable.Builder>
    {
        private static readonly string[] _objectDeployableFieldNames = new string[] { "CreatorID", "OwnerID" };
        private static readonly uint[] _objectDeployableFieldTags = new uint[] { 8, 0x10 };
        private ulong creatorID_;
        public const int CreatorIDFieldNumber = 1;
        private static readonly objectDeployable defaultInstance = new objectDeployable().MakeReadOnly();
        private bool hasCreatorID;
        private bool hasOwnerID;
        private int memoizedSerializedSize = -1;
        private ulong ownerID_;
        public const int OwnerIDFieldNumber = 2;

        static objectDeployable()
        {
            object.ReferenceEquals(Worldsave.Descriptor, null);
        }

        private objectDeployable()
        {
        }

        public static Builder CreateBuilder()
        {
            return new Builder();
        }

        public static Builder CreateBuilder(objectDeployable prototype)
        {
            return new Builder(prototype);
        }

        public override Builder CreateBuilderForType()
        {
            return new Builder();
        }

        private objectDeployable MakeReadOnly()
        {
            return this;
        }

        public static objectDeployable ParseDelimitedFrom(Stream input)
        {
            return CreateBuilder().MergeDelimitedFrom(input).BuildParsed();
        }

        public static objectDeployable ParseDelimitedFrom(Stream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeDelimitedFrom(input, extensionRegistry).BuildParsed();
        }

        public static objectDeployable ParseFrom(ByteString data)
        {
            return CreateBuilder().MergeFrom(data).BuildParsed();
        }

        public static objectDeployable ParseFrom(byte[] data)
        {
            return CreateBuilder().MergeFrom(data).BuildParsed();
        }

        public static objectDeployable ParseFrom(ICodedInputStream input)
        {
            return CreateBuilder().MergeFrom(input).BuildParsed();
        }

        public static objectDeployable ParseFrom(Stream input)
        {
            return CreateBuilder().MergeFrom(input).BuildParsed();
        }

        public static objectDeployable ParseFrom(ByteString data, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
        }

        public static objectDeployable ParseFrom(byte[] data, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
        }

        public static objectDeployable ParseFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
        }

        public static objectDeployable ParseFrom(Stream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
        }

        public static Recycler<objectDeployable, Builder> Recycler()
        {
            return Recycler<objectDeployable, Builder>.Manufacture();
        }

        public override Builder ToBuilder()
        {
            return CreateBuilder(this);
        }

        public override void WriteTo(ICodedOutputStream output)
        {
            int serializedSize = this.SerializedSize;
            string[] strArray = _objectDeployableFieldNames;
            if (this.hasCreatorID)
            {
                output.WriteUInt64(1, strArray[0], this.CreatorID);
            }
            if (this.hasOwnerID)
            {
                output.WriteUInt64(2, strArray[1], this.OwnerID);
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

        public static objectDeployable DefaultInstance
        {
            get
            {
                return defaultInstance;
            }
        }

        public override objectDeployable DefaultInstanceForType
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
                return Worldsave.internal__static_RustProto_objectDeployable__Descriptor;
            }
        }

        public bool HasCreatorID
        {
            get
            {
                return this.hasCreatorID;
            }
        }

        public bool HasOwnerID
        {
            get
            {
                return this.hasOwnerID;
            }
        }

        protected override FieldAccessorTable<objectDeployable, Builder> InternalFieldAccessors
        {
            get
            {
                return Worldsave.internal__static_RustProto_objectDeployable__FieldAccessorTable;
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
                    if (this.hasCreatorID)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeUInt64Size(1, this.CreatorID);
                    }
                    if (this.hasOwnerID)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeUInt64Size(2, this.OwnerID);
                    }
                    memoizedSerializedSize += this.get_UnknownFields().get_SerializedSize();
                    this.memoizedSerializedSize = memoizedSerializedSize;
                }
                return memoizedSerializedSize;
            }
        }

        protected override objectDeployable ThisMessage
        {
            get
            {
                return this;
            }
        }

        [DebuggerNonUserCode]
        public sealed class Builder : GeneratedBuilder<objectDeployable, objectDeployable.Builder>
        {
            private objectDeployable result;
            private bool resultIsReadOnly;

            public Builder()
            {
                this.result = objectDeployable.DefaultInstance;
                this.resultIsReadOnly = true;
            }

            internal Builder(objectDeployable cloneFrom)
            {
                this.result = cloneFrom;
                this.resultIsReadOnly = true;
            }

            public override objectDeployable BuildPartial()
            {
                if (this.resultIsReadOnly)
                {
                    return this.result;
                }
                this.resultIsReadOnly = true;
                return this.result.MakeReadOnly();
            }

            public override objectDeployable.Builder Clear()
            {
                this.result = objectDeployable.DefaultInstance;
                this.resultIsReadOnly = true;
                return this;
            }

            public objectDeployable.Builder ClearCreatorID()
            {
                this.PrepareBuilder();
                this.result.hasCreatorID = false;
                this.result.creatorID_ = 0L;
                return this;
            }

            public objectDeployable.Builder ClearOwnerID()
            {
                this.PrepareBuilder();
                this.result.hasOwnerID = false;
                this.result.ownerID_ = 0L;
                return this;
            }

            public override objectDeployable.Builder Clone()
            {
                if (this.resultIsReadOnly)
                {
                    return new objectDeployable.Builder(this.result);
                }
                return new objectDeployable.Builder().MergeFrom(this.result);
            }

            public override objectDeployable.Builder MergeFrom(ICodedInputStream input)
            {
                return this.MergeFrom(input, ExtensionRegistry.get_Empty());
            }

            public override objectDeployable.Builder MergeFrom(IMessage other)
            {
                if (other is objectDeployable)
                {
                    return this.MergeFrom((objectDeployable) other);
                }
                base.MergeFrom(other);
                return this;
            }

            public override objectDeployable.Builder MergeFrom(objectDeployable other)
            {
                if (other != objectDeployable.DefaultInstance)
                {
                    this.PrepareBuilder();
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

            public override objectDeployable.Builder MergeFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
            {
                uint num;
                string str;
                this.PrepareBuilder();
                UnknownFieldSet.Builder builder = null;
                while (input.ReadTag(ref num, ref str))
                {
                    if ((num == 0) && (str != null))
                    {
                        int index = Array.BinarySearch<string>(objectDeployable._objectDeployableFieldNames, str, StringComparer.Ordinal);
                        if (index >= 0)
                        {
                            num = objectDeployable._objectDeployableFieldTags[index];
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
                            this.result.hasCreatorID = input.ReadUInt64(ref this.result.creatorID_);
                            continue;
                        }
                        case 0x10:
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
                    this.result.hasOwnerID = input.ReadUInt64(ref this.result.ownerID_);
                }
                if (builder != null)
                {
                    this.set_UnknownFields(builder.Build());
                }
                return this;
            }

            private objectDeployable PrepareBuilder()
            {
                if (this.resultIsReadOnly)
                {
                    objectDeployable result = this.result;
                    this.result = new objectDeployable();
                    this.resultIsReadOnly = false;
                    this.MergeFrom(result);
                }
                return this.result;
            }

            [CLSCompliant(false)]
            public objectDeployable.Builder SetCreatorID(ulong value)
            {
                this.PrepareBuilder();
                this.result.hasCreatorID = true;
                this.result.creatorID_ = value;
                return this;
            }

            [CLSCompliant(false)]
            public objectDeployable.Builder SetOwnerID(ulong value)
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

            public override objectDeployable DefaultInstanceForType
            {
                get
                {
                    return objectDeployable.DefaultInstance;
                }
            }

            public override MessageDescriptor DescriptorForType
            {
                get
                {
                    return objectDeployable.Descriptor;
                }
            }

            public bool HasCreatorID
            {
                get
                {
                    return this.result.hasCreatorID;
                }
            }

            public bool HasOwnerID
            {
                get
                {
                    return this.result.hasOwnerID;
                }
            }

            public override bool IsInitialized
            {
                get
                {
                    return this.result.IsInitialized;
                }
            }

            protected override objectDeployable MessageBeingBuilt
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

            protected override objectDeployable.Builder ThisBuilder
            {
                get
                {
                    return this;
                }
            }
        }
    }
}

