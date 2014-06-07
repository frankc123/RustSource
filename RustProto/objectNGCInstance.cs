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
    public sealed class objectNGCInstance : GeneratedMessage<objectNGCInstance, objectNGCInstance.Builder>
    {
        private static readonly string[] _objectNGCInstanceFieldNames = new string[] { "ID", "data" };
        private static readonly uint[] _objectNGCInstanceFieldTags = new uint[] { 8, 0x12 };
        private ByteString data_ = ByteString.get_Empty();
        public const int DataFieldNumber = 2;
        private static readonly objectNGCInstance defaultInstance = new objectNGCInstance().MakeReadOnly();
        private bool hasData;
        private bool hasID;
        private int iD_;
        public const int IDFieldNumber = 1;
        private int memoizedSerializedSize = -1;

        static objectNGCInstance()
        {
            object.ReferenceEquals(Worldsave.Descriptor, null);
        }

        private objectNGCInstance()
        {
        }

        public static Builder CreateBuilder()
        {
            return new Builder();
        }

        public static Builder CreateBuilder(objectNGCInstance prototype)
        {
            return new Builder(prototype);
        }

        public override Builder CreateBuilderForType()
        {
            return new Builder();
        }

        private objectNGCInstance MakeReadOnly()
        {
            return this;
        }

        public static objectNGCInstance ParseDelimitedFrom(Stream input)
        {
            return CreateBuilder().MergeDelimitedFrom(input).BuildParsed();
        }

        public static objectNGCInstance ParseDelimitedFrom(Stream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeDelimitedFrom(input, extensionRegistry).BuildParsed();
        }

        public static objectNGCInstance ParseFrom(ByteString data)
        {
            return CreateBuilder().MergeFrom(data).BuildParsed();
        }

        public static objectNGCInstance ParseFrom(byte[] data)
        {
            return CreateBuilder().MergeFrom(data).BuildParsed();
        }

        public static objectNGCInstance ParseFrom(ICodedInputStream input)
        {
            return CreateBuilder().MergeFrom(input).BuildParsed();
        }

        public static objectNGCInstance ParseFrom(Stream input)
        {
            return CreateBuilder().MergeFrom(input).BuildParsed();
        }

        public static objectNGCInstance ParseFrom(ByteString data, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
        }

        public static objectNGCInstance ParseFrom(byte[] data, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
        }

        public static objectNGCInstance ParseFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
        }

        public static objectNGCInstance ParseFrom(Stream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
        }

        public static Recycler<objectNGCInstance, Builder> Recycler()
        {
            return Recycler<objectNGCInstance, Builder>.Manufacture();
        }

        public override Builder ToBuilder()
        {
            return CreateBuilder(this);
        }

        public override void WriteTo(ICodedOutputStream output)
        {
            int serializedSize = this.SerializedSize;
            string[] strArray = _objectNGCInstanceFieldNames;
            if (this.hasID)
            {
                output.WriteInt32(1, strArray[0], this.ID);
            }
            if (this.hasData)
            {
                output.WriteBytes(2, strArray[1], this.Data);
            }
            this.get_UnknownFields().WriteTo(output);
        }

        public ByteString Data
        {
            get
            {
                return this.data_;
            }
        }

        public static objectNGCInstance DefaultInstance
        {
            get
            {
                return defaultInstance;
            }
        }

        public override objectNGCInstance DefaultInstanceForType
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
                return Worldsave.internal__static_RustProto_objectNGCInstance__Descriptor;
            }
        }

        public bool HasData
        {
            get
            {
                return this.hasData;
            }
        }

        public bool HasID
        {
            get
            {
                return this.hasID;
            }
        }

        public int ID
        {
            get
            {
                return this.iD_;
            }
        }

        protected override FieldAccessorTable<objectNGCInstance, Builder> InternalFieldAccessors
        {
            get
            {
                return Worldsave.internal__static_RustProto_objectNGCInstance__FieldAccessorTable;
            }
        }

        public override bool IsInitialized
        {
            get
            {
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
                    if (this.hasID)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeInt32Size(1, this.ID);
                    }
                    if (this.hasData)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeBytesSize(2, this.Data);
                    }
                    memoizedSerializedSize += this.get_UnknownFields().get_SerializedSize();
                    this.memoizedSerializedSize = memoizedSerializedSize;
                }
                return memoizedSerializedSize;
            }
        }

        protected override objectNGCInstance ThisMessage
        {
            get
            {
                return this;
            }
        }

        [DebuggerNonUserCode]
        public sealed class Builder : GeneratedBuilder<objectNGCInstance, objectNGCInstance.Builder>
        {
            private objectNGCInstance result;
            private bool resultIsReadOnly;

            public Builder()
            {
                this.result = objectNGCInstance.DefaultInstance;
                this.resultIsReadOnly = true;
            }

            internal Builder(objectNGCInstance cloneFrom)
            {
                this.result = cloneFrom;
                this.resultIsReadOnly = true;
            }

            public override objectNGCInstance BuildPartial()
            {
                if (this.resultIsReadOnly)
                {
                    return this.result;
                }
                this.resultIsReadOnly = true;
                return this.result.MakeReadOnly();
            }

            public override objectNGCInstance.Builder Clear()
            {
                this.result = objectNGCInstance.DefaultInstance;
                this.resultIsReadOnly = true;
                return this;
            }

            public objectNGCInstance.Builder ClearData()
            {
                this.PrepareBuilder();
                this.result.hasData = false;
                this.result.data_ = ByteString.get_Empty();
                return this;
            }

            public objectNGCInstance.Builder ClearID()
            {
                this.PrepareBuilder();
                this.result.hasID = false;
                this.result.iD_ = 0;
                return this;
            }

            public override objectNGCInstance.Builder Clone()
            {
                if (this.resultIsReadOnly)
                {
                    return new objectNGCInstance.Builder(this.result);
                }
                return new objectNGCInstance.Builder().MergeFrom(this.result);
            }

            public override objectNGCInstance.Builder MergeFrom(ICodedInputStream input)
            {
                return this.MergeFrom(input, ExtensionRegistry.get_Empty());
            }

            public override objectNGCInstance.Builder MergeFrom(IMessage other)
            {
                if (other is objectNGCInstance)
                {
                    return this.MergeFrom((objectNGCInstance) other);
                }
                base.MergeFrom(other);
                return this;
            }

            public override objectNGCInstance.Builder MergeFrom(objectNGCInstance other)
            {
                if (other != objectNGCInstance.DefaultInstance)
                {
                    this.PrepareBuilder();
                    if (other.HasID)
                    {
                        this.ID = other.ID;
                    }
                    if (other.HasData)
                    {
                        this.Data = other.Data;
                    }
                    this.MergeUnknownFields(other.get_UnknownFields());
                }
                return this;
            }

            public override objectNGCInstance.Builder MergeFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
            {
                uint num;
                string str;
                this.PrepareBuilder();
                UnknownFieldSet.Builder builder = null;
                while (input.ReadTag(ref num, ref str))
                {
                    if ((num == 0) && (str != null))
                    {
                        int index = Array.BinarySearch<string>(objectNGCInstance._objectNGCInstanceFieldNames, str, StringComparer.Ordinal);
                        if (index >= 0)
                        {
                            num = objectNGCInstance._objectNGCInstanceFieldTags[index];
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
                            this.result.hasID = input.ReadInt32(ref this.result.iD_);
                            continue;
                        }
                        case 0x12:
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
                    this.result.hasData = input.ReadBytes(ref this.result.data_);
                }
                if (builder != null)
                {
                    this.set_UnknownFields(builder.Build());
                }
                return this;
            }

            private objectNGCInstance PrepareBuilder()
            {
                if (this.resultIsReadOnly)
                {
                    objectNGCInstance result = this.result;
                    this.result = new objectNGCInstance();
                    this.resultIsReadOnly = false;
                    this.MergeFrom(result);
                }
                return this.result;
            }

            public objectNGCInstance.Builder SetData(ByteString value)
            {
                ThrowHelper.ThrowIfNull(value, "value");
                this.PrepareBuilder();
                this.result.hasData = true;
                this.result.data_ = value;
                return this;
            }

            public objectNGCInstance.Builder SetID(int value)
            {
                this.PrepareBuilder();
                this.result.hasID = true;
                this.result.iD_ = value;
                return this;
            }

            public ByteString Data
            {
                get
                {
                    return this.result.Data;
                }
                set
                {
                    this.SetData(value);
                }
            }

            public override objectNGCInstance DefaultInstanceForType
            {
                get
                {
                    return objectNGCInstance.DefaultInstance;
                }
            }

            public override MessageDescriptor DescriptorForType
            {
                get
                {
                    return objectNGCInstance.Descriptor;
                }
            }

            public bool HasData
            {
                get
                {
                    return this.result.hasData;
                }
            }

            public bool HasID
            {
                get
                {
                    return this.result.hasID;
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

            protected override objectNGCInstance MessageBeingBuilt
            {
                get
                {
                    return this.PrepareBuilder();
                }
            }

            protected override objectNGCInstance.Builder ThisBuilder
            {
                get
                {
                    return this;
                }
            }
        }
    }
}

