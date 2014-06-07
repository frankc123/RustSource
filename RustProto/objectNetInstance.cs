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
    public sealed class objectNetInstance : GeneratedMessage<objectNetInstance, objectNetInstance.Builder>
    {
        private static readonly string[] _objectNetInstanceFieldNames = new string[] { "groupID", "ownerPrefab", "proxyPrefab", "serverPrefab" };
        private static readonly uint[] _objectNetInstanceFieldTags = new uint[] { 0x20, 0x10, 0x18, 8 };
        private static readonly objectNetInstance defaultInstance = new objectNetInstance().MakeReadOnly();
        private int groupID_;
        public const int GroupIDFieldNumber = 4;
        private bool hasGroupID;
        private bool hasOwnerPrefab;
        private bool hasProxyPrefab;
        private bool hasServerPrefab;
        private int memoizedSerializedSize = -1;
        private int ownerPrefab_;
        public const int OwnerPrefabFieldNumber = 2;
        private int proxyPrefab_;
        public const int ProxyPrefabFieldNumber = 3;
        private int serverPrefab_;
        public const int ServerPrefabFieldNumber = 1;

        static objectNetInstance()
        {
            object.ReferenceEquals(Worldsave.Descriptor, null);
        }

        private objectNetInstance()
        {
        }

        public static Builder CreateBuilder()
        {
            return new Builder();
        }

        public static Builder CreateBuilder(objectNetInstance prototype)
        {
            return new Builder(prototype);
        }

        public override Builder CreateBuilderForType()
        {
            return new Builder();
        }

        private objectNetInstance MakeReadOnly()
        {
            return this;
        }

        public static objectNetInstance ParseDelimitedFrom(Stream input)
        {
            return CreateBuilder().MergeDelimitedFrom(input).BuildParsed();
        }

        public static objectNetInstance ParseDelimitedFrom(Stream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeDelimitedFrom(input, extensionRegistry).BuildParsed();
        }

        public static objectNetInstance ParseFrom(ByteString data)
        {
            return CreateBuilder().MergeFrom(data).BuildParsed();
        }

        public static objectNetInstance ParseFrom(byte[] data)
        {
            return CreateBuilder().MergeFrom(data).BuildParsed();
        }

        public static objectNetInstance ParseFrom(ICodedInputStream input)
        {
            return CreateBuilder().MergeFrom(input).BuildParsed();
        }

        public static objectNetInstance ParseFrom(Stream input)
        {
            return CreateBuilder().MergeFrom(input).BuildParsed();
        }

        public static objectNetInstance ParseFrom(ByteString data, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
        }

        public static objectNetInstance ParseFrom(byte[] data, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
        }

        public static objectNetInstance ParseFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
        }

        public static objectNetInstance ParseFrom(Stream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
        }

        public static Recycler<objectNetInstance, Builder> Recycler()
        {
            return Recycler<objectNetInstance, Builder>.Manufacture();
        }

        public override Builder ToBuilder()
        {
            return CreateBuilder(this);
        }

        public override void WriteTo(ICodedOutputStream output)
        {
            int serializedSize = this.SerializedSize;
            string[] strArray = _objectNetInstanceFieldNames;
            if (this.hasServerPrefab)
            {
                output.WriteInt32(1, strArray[3], this.ServerPrefab);
            }
            if (this.hasOwnerPrefab)
            {
                output.WriteInt32(2, strArray[1], this.OwnerPrefab);
            }
            if (this.hasProxyPrefab)
            {
                output.WriteInt32(3, strArray[2], this.ProxyPrefab);
            }
            if (this.hasGroupID)
            {
                output.WriteInt32(4, strArray[0], this.GroupID);
            }
            this.get_UnknownFields().WriteTo(output);
        }

        public static objectNetInstance DefaultInstance
        {
            get
            {
                return defaultInstance;
            }
        }

        public override objectNetInstance DefaultInstanceForType
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
                return Worldsave.internal__static_RustProto_objectNetInstance__Descriptor;
            }
        }

        public int GroupID
        {
            get
            {
                return this.groupID_;
            }
        }

        public bool HasGroupID
        {
            get
            {
                return this.hasGroupID;
            }
        }

        public bool HasOwnerPrefab
        {
            get
            {
                return this.hasOwnerPrefab;
            }
        }

        public bool HasProxyPrefab
        {
            get
            {
                return this.hasProxyPrefab;
            }
        }

        public bool HasServerPrefab
        {
            get
            {
                return this.hasServerPrefab;
            }
        }

        protected override FieldAccessorTable<objectNetInstance, Builder> InternalFieldAccessors
        {
            get
            {
                return Worldsave.internal__static_RustProto_objectNetInstance__FieldAccessorTable;
            }
        }

        public override bool IsInitialized
        {
            get
            {
                return true;
            }
        }

        public int OwnerPrefab
        {
            get
            {
                return this.ownerPrefab_;
            }
        }

        public int ProxyPrefab
        {
            get
            {
                return this.proxyPrefab_;
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
                    if (this.hasServerPrefab)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeInt32Size(1, this.ServerPrefab);
                    }
                    if (this.hasOwnerPrefab)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeInt32Size(2, this.OwnerPrefab);
                    }
                    if (this.hasProxyPrefab)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeInt32Size(3, this.ProxyPrefab);
                    }
                    if (this.hasGroupID)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeInt32Size(4, this.GroupID);
                    }
                    memoizedSerializedSize += this.get_UnknownFields().get_SerializedSize();
                    this.memoizedSerializedSize = memoizedSerializedSize;
                }
                return memoizedSerializedSize;
            }
        }

        public int ServerPrefab
        {
            get
            {
                return this.serverPrefab_;
            }
        }

        protected override objectNetInstance ThisMessage
        {
            get
            {
                return this;
            }
        }

        [DebuggerNonUserCode]
        public sealed class Builder : GeneratedBuilder<objectNetInstance, objectNetInstance.Builder>
        {
            private objectNetInstance result;
            private bool resultIsReadOnly;

            public Builder()
            {
                this.result = objectNetInstance.DefaultInstance;
                this.resultIsReadOnly = true;
            }

            internal Builder(objectNetInstance cloneFrom)
            {
                this.result = cloneFrom;
                this.resultIsReadOnly = true;
            }

            public override objectNetInstance BuildPartial()
            {
                if (this.resultIsReadOnly)
                {
                    return this.result;
                }
                this.resultIsReadOnly = true;
                return this.result.MakeReadOnly();
            }

            public override objectNetInstance.Builder Clear()
            {
                this.result = objectNetInstance.DefaultInstance;
                this.resultIsReadOnly = true;
                return this;
            }

            public objectNetInstance.Builder ClearGroupID()
            {
                this.PrepareBuilder();
                this.result.hasGroupID = false;
                this.result.groupID_ = 0;
                return this;
            }

            public objectNetInstance.Builder ClearOwnerPrefab()
            {
                this.PrepareBuilder();
                this.result.hasOwnerPrefab = false;
                this.result.ownerPrefab_ = 0;
                return this;
            }

            public objectNetInstance.Builder ClearProxyPrefab()
            {
                this.PrepareBuilder();
                this.result.hasProxyPrefab = false;
                this.result.proxyPrefab_ = 0;
                return this;
            }

            public objectNetInstance.Builder ClearServerPrefab()
            {
                this.PrepareBuilder();
                this.result.hasServerPrefab = false;
                this.result.serverPrefab_ = 0;
                return this;
            }

            public override objectNetInstance.Builder Clone()
            {
                if (this.resultIsReadOnly)
                {
                    return new objectNetInstance.Builder(this.result);
                }
                return new objectNetInstance.Builder().MergeFrom(this.result);
            }

            public override objectNetInstance.Builder MergeFrom(ICodedInputStream input)
            {
                return this.MergeFrom(input, ExtensionRegistry.get_Empty());
            }

            public override objectNetInstance.Builder MergeFrom(IMessage other)
            {
                if (other is objectNetInstance)
                {
                    return this.MergeFrom((objectNetInstance) other);
                }
                base.MergeFrom(other);
                return this;
            }

            public override objectNetInstance.Builder MergeFrom(objectNetInstance other)
            {
                if (other != objectNetInstance.DefaultInstance)
                {
                    this.PrepareBuilder();
                    if (other.HasServerPrefab)
                    {
                        this.ServerPrefab = other.ServerPrefab;
                    }
                    if (other.HasOwnerPrefab)
                    {
                        this.OwnerPrefab = other.OwnerPrefab;
                    }
                    if (other.HasProxyPrefab)
                    {
                        this.ProxyPrefab = other.ProxyPrefab;
                    }
                    if (other.HasGroupID)
                    {
                        this.GroupID = other.GroupID;
                    }
                    this.MergeUnknownFields(other.get_UnknownFields());
                }
                return this;
            }

            public override objectNetInstance.Builder MergeFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
            {
                uint num;
                string str;
                this.PrepareBuilder();
                UnknownFieldSet.Builder builder = null;
                while (input.ReadTag(ref num, ref str))
                {
                    if ((num == 0) && (str != null))
                    {
                        int index = Array.BinarySearch<string>(objectNetInstance._objectNetInstanceFieldNames, str, StringComparer.Ordinal);
                        if (index >= 0)
                        {
                            num = objectNetInstance._objectNetInstanceFieldTags[index];
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
                            this.result.hasServerPrefab = input.ReadInt32(ref this.result.serverPrefab_);
                            continue;
                        }
                        case 0x10:
                        {
                            this.result.hasOwnerPrefab = input.ReadInt32(ref this.result.ownerPrefab_);
                            continue;
                        }
                        case 0x18:
                        {
                            this.result.hasProxyPrefab = input.ReadInt32(ref this.result.proxyPrefab_);
                            continue;
                        }
                        case 0x20:
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
                    this.result.hasGroupID = input.ReadInt32(ref this.result.groupID_);
                }
                if (builder != null)
                {
                    this.set_UnknownFields(builder.Build());
                }
                return this;
            }

            private objectNetInstance PrepareBuilder()
            {
                if (this.resultIsReadOnly)
                {
                    objectNetInstance result = this.result;
                    this.result = new objectNetInstance();
                    this.resultIsReadOnly = false;
                    this.MergeFrom(result);
                }
                return this.result;
            }

            public objectNetInstance.Builder SetGroupID(int value)
            {
                this.PrepareBuilder();
                this.result.hasGroupID = true;
                this.result.groupID_ = value;
                return this;
            }

            public objectNetInstance.Builder SetOwnerPrefab(int value)
            {
                this.PrepareBuilder();
                this.result.hasOwnerPrefab = true;
                this.result.ownerPrefab_ = value;
                return this;
            }

            public objectNetInstance.Builder SetProxyPrefab(int value)
            {
                this.PrepareBuilder();
                this.result.hasProxyPrefab = true;
                this.result.proxyPrefab_ = value;
                return this;
            }

            public objectNetInstance.Builder SetServerPrefab(int value)
            {
                this.PrepareBuilder();
                this.result.hasServerPrefab = true;
                this.result.serverPrefab_ = value;
                return this;
            }

            public override objectNetInstance DefaultInstanceForType
            {
                get
                {
                    return objectNetInstance.DefaultInstance;
                }
            }

            public override MessageDescriptor DescriptorForType
            {
                get
                {
                    return objectNetInstance.Descriptor;
                }
            }

            public int GroupID
            {
                get
                {
                    return this.result.GroupID;
                }
                set
                {
                    this.SetGroupID(value);
                }
            }

            public bool HasGroupID
            {
                get
                {
                    return this.result.hasGroupID;
                }
            }

            public bool HasOwnerPrefab
            {
                get
                {
                    return this.result.hasOwnerPrefab;
                }
            }

            public bool HasProxyPrefab
            {
                get
                {
                    return this.result.hasProxyPrefab;
                }
            }

            public bool HasServerPrefab
            {
                get
                {
                    return this.result.hasServerPrefab;
                }
            }

            public override bool IsInitialized
            {
                get
                {
                    return this.result.IsInitialized;
                }
            }

            protected override objectNetInstance MessageBeingBuilt
            {
                get
                {
                    return this.PrepareBuilder();
                }
            }

            public int OwnerPrefab
            {
                get
                {
                    return this.result.OwnerPrefab;
                }
                set
                {
                    this.SetOwnerPrefab(value);
                }
            }

            public int ProxyPrefab
            {
                get
                {
                    return this.result.ProxyPrefab;
                }
                set
                {
                    this.SetProxyPrefab(value);
                }
            }

            public int ServerPrefab
            {
                get
                {
                    return this.result.ServerPrefab;
                }
                set
                {
                    this.SetServerPrefab(value);
                }
            }

            protected override objectNetInstance.Builder ThisBuilder
            {
                get
                {
                    return this;
                }
            }
        }
    }
}

