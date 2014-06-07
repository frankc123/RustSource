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
    public sealed class objectStructComponent : GeneratedMessage<objectStructComponent, objectStructComponent.Builder>
    {
        private static readonly string[] _objectStructComponentFieldNames = new string[] { "ID", "MasterID", "MasterViewID" };
        private static readonly uint[] _objectStructComponentFieldTags = new uint[] { 8, 0x10, 0x18 };
        private static readonly objectStructComponent defaultInstance = new objectStructComponent().MakeReadOnly();
        private bool hasID;
        private bool hasMasterID;
        private bool hasMasterViewID;
        private int iD_;
        public const int IDFieldNumber = 1;
        private int masterID_;
        public const int MasterIDFieldNumber = 2;
        private int masterViewID_;
        public const int MasterViewIDFieldNumber = 3;
        private int memoizedSerializedSize = -1;

        static objectStructComponent()
        {
            object.ReferenceEquals(Worldsave.Descriptor, null);
        }

        private objectStructComponent()
        {
        }

        public static Builder CreateBuilder()
        {
            return new Builder();
        }

        public static Builder CreateBuilder(objectStructComponent prototype)
        {
            return new Builder(prototype);
        }

        public override Builder CreateBuilderForType()
        {
            return new Builder();
        }

        private objectStructComponent MakeReadOnly()
        {
            return this;
        }

        public static objectStructComponent ParseDelimitedFrom(Stream input)
        {
            return CreateBuilder().MergeDelimitedFrom(input).BuildParsed();
        }

        public static objectStructComponent ParseDelimitedFrom(Stream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeDelimitedFrom(input, extensionRegistry).BuildParsed();
        }

        public static objectStructComponent ParseFrom(ByteString data)
        {
            return CreateBuilder().MergeFrom(data).BuildParsed();
        }

        public static objectStructComponent ParseFrom(byte[] data)
        {
            return CreateBuilder().MergeFrom(data).BuildParsed();
        }

        public static objectStructComponent ParseFrom(ICodedInputStream input)
        {
            return CreateBuilder().MergeFrom(input).BuildParsed();
        }

        public static objectStructComponent ParseFrom(Stream input)
        {
            return CreateBuilder().MergeFrom(input).BuildParsed();
        }

        public static objectStructComponent ParseFrom(ByteString data, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
        }

        public static objectStructComponent ParseFrom(byte[] data, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
        }

        public static objectStructComponent ParseFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
        }

        public static objectStructComponent ParseFrom(Stream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
        }

        public static Recycler<objectStructComponent, Builder> Recycler()
        {
            return Recycler<objectStructComponent, Builder>.Manufacture();
        }

        public override Builder ToBuilder()
        {
            return CreateBuilder(this);
        }

        public override void WriteTo(ICodedOutputStream output)
        {
            int serializedSize = this.SerializedSize;
            string[] strArray = _objectStructComponentFieldNames;
            if (this.hasID)
            {
                output.WriteInt32(1, strArray[0], this.ID);
            }
            if (this.hasMasterID)
            {
                output.WriteInt32(2, strArray[1], this.MasterID);
            }
            if (this.hasMasterViewID)
            {
                output.WriteInt32(3, strArray[2], this.MasterViewID);
            }
            this.get_UnknownFields().WriteTo(output);
        }

        public static objectStructComponent DefaultInstance
        {
            get
            {
                return defaultInstance;
            }
        }

        public override objectStructComponent DefaultInstanceForType
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
                return Worldsave.internal__static_RustProto_objectStructComponent__Descriptor;
            }
        }

        public bool HasID
        {
            get
            {
                return this.hasID;
            }
        }

        public bool HasMasterID
        {
            get
            {
                return this.hasMasterID;
            }
        }

        public bool HasMasterViewID
        {
            get
            {
                return this.hasMasterViewID;
            }
        }

        public int ID
        {
            get
            {
                return this.iD_;
            }
        }

        protected override FieldAccessorTable<objectStructComponent, Builder> InternalFieldAccessors
        {
            get
            {
                return Worldsave.internal__static_RustProto_objectStructComponent__FieldAccessorTable;
            }
        }

        public override bool IsInitialized
        {
            get
            {
                return true;
            }
        }

        public int MasterID
        {
            get
            {
                return this.masterID_;
            }
        }

        public int MasterViewID
        {
            get
            {
                return this.masterViewID_;
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
                    if (this.hasMasterID)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeInt32Size(2, this.MasterID);
                    }
                    if (this.hasMasterViewID)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeInt32Size(3, this.MasterViewID);
                    }
                    memoizedSerializedSize += this.get_UnknownFields().get_SerializedSize();
                    this.memoizedSerializedSize = memoizedSerializedSize;
                }
                return memoizedSerializedSize;
            }
        }

        protected override objectStructComponent ThisMessage
        {
            get
            {
                return this;
            }
        }

        [DebuggerNonUserCode]
        public sealed class Builder : GeneratedBuilder<objectStructComponent, objectStructComponent.Builder>
        {
            private objectStructComponent result;
            private bool resultIsReadOnly;

            public Builder()
            {
                this.result = objectStructComponent.DefaultInstance;
                this.resultIsReadOnly = true;
            }

            internal Builder(objectStructComponent cloneFrom)
            {
                this.result = cloneFrom;
                this.resultIsReadOnly = true;
            }

            public override objectStructComponent BuildPartial()
            {
                if (this.resultIsReadOnly)
                {
                    return this.result;
                }
                this.resultIsReadOnly = true;
                return this.result.MakeReadOnly();
            }

            public override objectStructComponent.Builder Clear()
            {
                this.result = objectStructComponent.DefaultInstance;
                this.resultIsReadOnly = true;
                return this;
            }

            public objectStructComponent.Builder ClearID()
            {
                this.PrepareBuilder();
                this.result.hasID = false;
                this.result.iD_ = 0;
                return this;
            }

            public objectStructComponent.Builder ClearMasterID()
            {
                this.PrepareBuilder();
                this.result.hasMasterID = false;
                this.result.masterID_ = 0;
                return this;
            }

            public objectStructComponent.Builder ClearMasterViewID()
            {
                this.PrepareBuilder();
                this.result.hasMasterViewID = false;
                this.result.masterViewID_ = 0;
                return this;
            }

            public override objectStructComponent.Builder Clone()
            {
                if (this.resultIsReadOnly)
                {
                    return new objectStructComponent.Builder(this.result);
                }
                return new objectStructComponent.Builder().MergeFrom(this.result);
            }

            public override objectStructComponent.Builder MergeFrom(ICodedInputStream input)
            {
                return this.MergeFrom(input, ExtensionRegistry.get_Empty());
            }

            public override objectStructComponent.Builder MergeFrom(IMessage other)
            {
                if (other is objectStructComponent)
                {
                    return this.MergeFrom((objectStructComponent) other);
                }
                base.MergeFrom(other);
                return this;
            }

            public override objectStructComponent.Builder MergeFrom(objectStructComponent other)
            {
                if (other != objectStructComponent.DefaultInstance)
                {
                    this.PrepareBuilder();
                    if (other.HasID)
                    {
                        this.ID = other.ID;
                    }
                    if (other.HasMasterID)
                    {
                        this.MasterID = other.MasterID;
                    }
                    if (other.HasMasterViewID)
                    {
                        this.MasterViewID = other.MasterViewID;
                    }
                    this.MergeUnknownFields(other.get_UnknownFields());
                }
                return this;
            }

            public override objectStructComponent.Builder MergeFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
            {
                uint num;
                string str;
                this.PrepareBuilder();
                UnknownFieldSet.Builder builder = null;
                while (input.ReadTag(ref num, ref str))
                {
                    if ((num == 0) && (str != null))
                    {
                        int index = Array.BinarySearch<string>(objectStructComponent._objectStructComponentFieldNames, str, StringComparer.Ordinal);
                        if (index >= 0)
                        {
                            num = objectStructComponent._objectStructComponentFieldTags[index];
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
                        case 0x10:
                        {
                            this.result.hasMasterID = input.ReadInt32(ref this.result.masterID_);
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
                    this.result.hasMasterViewID = input.ReadInt32(ref this.result.masterViewID_);
                }
                if (builder != null)
                {
                    this.set_UnknownFields(builder.Build());
                }
                return this;
            }

            private objectStructComponent PrepareBuilder()
            {
                if (this.resultIsReadOnly)
                {
                    objectStructComponent result = this.result;
                    this.result = new objectStructComponent();
                    this.resultIsReadOnly = false;
                    this.MergeFrom(result);
                }
                return this.result;
            }

            public objectStructComponent.Builder SetID(int value)
            {
                this.PrepareBuilder();
                this.result.hasID = true;
                this.result.iD_ = value;
                return this;
            }

            public objectStructComponent.Builder SetMasterID(int value)
            {
                this.PrepareBuilder();
                this.result.hasMasterID = true;
                this.result.masterID_ = value;
                return this;
            }

            public objectStructComponent.Builder SetMasterViewID(int value)
            {
                this.PrepareBuilder();
                this.result.hasMasterViewID = true;
                this.result.masterViewID_ = value;
                return this;
            }

            public override objectStructComponent DefaultInstanceForType
            {
                get
                {
                    return objectStructComponent.DefaultInstance;
                }
            }

            public override MessageDescriptor DescriptorForType
            {
                get
                {
                    return objectStructComponent.Descriptor;
                }
            }

            public bool HasID
            {
                get
                {
                    return this.result.hasID;
                }
            }

            public bool HasMasterID
            {
                get
                {
                    return this.result.hasMasterID;
                }
            }

            public bool HasMasterViewID
            {
                get
                {
                    return this.result.hasMasterViewID;
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

            public int MasterID
            {
                get
                {
                    return this.result.MasterID;
                }
                set
                {
                    this.SetMasterID(value);
                }
            }

            public int MasterViewID
            {
                get
                {
                    return this.result.MasterViewID;
                }
                set
                {
                    this.SetMasterViewID(value);
                }
            }

            protected override objectStructComponent MessageBeingBuilt
            {
                get
                {
                    return this.PrepareBuilder();
                }
            }

            protected override objectStructComponent.Builder ThisBuilder
            {
                get
                {
                    return this;
                }
            }
        }
    }
}

