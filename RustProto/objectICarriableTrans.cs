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
    public sealed class objectICarriableTrans : GeneratedMessage<objectICarriableTrans, objectICarriableTrans.Builder>
    {
        private static readonly string[] _objectICarriableTransFieldNames = new string[] { "transCarrierID" };
        private static readonly uint[] _objectICarriableTransFieldTags = new uint[] { 8 };
        private static readonly objectICarriableTrans defaultInstance = new objectICarriableTrans().MakeReadOnly();
        private bool hasTransCarrierID;
        private int memoizedSerializedSize = -1;
        private int transCarrierID_;
        public const int TransCarrierIDFieldNumber = 1;

        static objectICarriableTrans()
        {
            object.ReferenceEquals(Worldsave.Descriptor, null);
        }

        private objectICarriableTrans()
        {
        }

        public static Builder CreateBuilder()
        {
            return new Builder();
        }

        public static Builder CreateBuilder(objectICarriableTrans prototype)
        {
            return new Builder(prototype);
        }

        public override Builder CreateBuilderForType()
        {
            return new Builder();
        }

        private objectICarriableTrans MakeReadOnly()
        {
            return this;
        }

        public static objectICarriableTrans ParseDelimitedFrom(Stream input)
        {
            return CreateBuilder().MergeDelimitedFrom(input).BuildParsed();
        }

        public static objectICarriableTrans ParseDelimitedFrom(Stream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeDelimitedFrom(input, extensionRegistry).BuildParsed();
        }

        public static objectICarriableTrans ParseFrom(ByteString data)
        {
            return CreateBuilder().MergeFrom(data).BuildParsed();
        }

        public static objectICarriableTrans ParseFrom(byte[] data)
        {
            return CreateBuilder().MergeFrom(data).BuildParsed();
        }

        public static objectICarriableTrans ParseFrom(ICodedInputStream input)
        {
            return CreateBuilder().MergeFrom(input).BuildParsed();
        }

        public static objectICarriableTrans ParseFrom(Stream input)
        {
            return CreateBuilder().MergeFrom(input).BuildParsed();
        }

        public static objectICarriableTrans ParseFrom(ByteString data, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
        }

        public static objectICarriableTrans ParseFrom(byte[] data, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
        }

        public static objectICarriableTrans ParseFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
        }

        public static objectICarriableTrans ParseFrom(Stream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
        }

        public static Recycler<objectICarriableTrans, Builder> Recycler()
        {
            return Recycler<objectICarriableTrans, Builder>.Manufacture();
        }

        public override Builder ToBuilder()
        {
            return CreateBuilder(this);
        }

        public override void WriteTo(ICodedOutputStream output)
        {
            int serializedSize = this.SerializedSize;
            string[] strArray = _objectICarriableTransFieldNames;
            if (this.hasTransCarrierID)
            {
                output.WriteInt32(1, strArray[0], this.TransCarrierID);
            }
            this.get_UnknownFields().WriteTo(output);
        }

        public static objectICarriableTrans DefaultInstance
        {
            get
            {
                return defaultInstance;
            }
        }

        public override objectICarriableTrans DefaultInstanceForType
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
                return Worldsave.internal__static_RustProto_objectICarriableTrans__Descriptor;
            }
        }

        public bool HasTransCarrierID
        {
            get
            {
                return this.hasTransCarrierID;
            }
        }

        protected override FieldAccessorTable<objectICarriableTrans, Builder> InternalFieldAccessors
        {
            get
            {
                return Worldsave.internal__static_RustProto_objectICarriableTrans__FieldAccessorTable;
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
                    if (this.hasTransCarrierID)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeInt32Size(1, this.TransCarrierID);
                    }
                    memoizedSerializedSize += this.get_UnknownFields().get_SerializedSize();
                    this.memoizedSerializedSize = memoizedSerializedSize;
                }
                return memoizedSerializedSize;
            }
        }

        protected override objectICarriableTrans ThisMessage
        {
            get
            {
                return this;
            }
        }

        public int TransCarrierID
        {
            get
            {
                return this.transCarrierID_;
            }
        }

        [DebuggerNonUserCode]
        public sealed class Builder : GeneratedBuilder<objectICarriableTrans, objectICarriableTrans.Builder>
        {
            private objectICarriableTrans result;
            private bool resultIsReadOnly;

            public Builder()
            {
                this.result = objectICarriableTrans.DefaultInstance;
                this.resultIsReadOnly = true;
            }

            internal Builder(objectICarriableTrans cloneFrom)
            {
                this.result = cloneFrom;
                this.resultIsReadOnly = true;
            }

            public override objectICarriableTrans BuildPartial()
            {
                if (this.resultIsReadOnly)
                {
                    return this.result;
                }
                this.resultIsReadOnly = true;
                return this.result.MakeReadOnly();
            }

            public override objectICarriableTrans.Builder Clear()
            {
                this.result = objectICarriableTrans.DefaultInstance;
                this.resultIsReadOnly = true;
                return this;
            }

            public objectICarriableTrans.Builder ClearTransCarrierID()
            {
                this.PrepareBuilder();
                this.result.hasTransCarrierID = false;
                this.result.transCarrierID_ = 0;
                return this;
            }

            public override objectICarriableTrans.Builder Clone()
            {
                if (this.resultIsReadOnly)
                {
                    return new objectICarriableTrans.Builder(this.result);
                }
                return new objectICarriableTrans.Builder().MergeFrom(this.result);
            }

            public override objectICarriableTrans.Builder MergeFrom(ICodedInputStream input)
            {
                return this.MergeFrom(input, ExtensionRegistry.get_Empty());
            }

            public override objectICarriableTrans.Builder MergeFrom(IMessage other)
            {
                if (other is objectICarriableTrans)
                {
                    return this.MergeFrom((objectICarriableTrans) other);
                }
                base.MergeFrom(other);
                return this;
            }

            public override objectICarriableTrans.Builder MergeFrom(objectICarriableTrans other)
            {
                if (other != objectICarriableTrans.DefaultInstance)
                {
                    this.PrepareBuilder();
                    if (other.HasTransCarrierID)
                    {
                        this.TransCarrierID = other.TransCarrierID;
                    }
                    this.MergeUnknownFields(other.get_UnknownFields());
                }
                return this;
            }

            public override objectICarriableTrans.Builder MergeFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
            {
                uint num;
                string str;
                this.PrepareBuilder();
                UnknownFieldSet.Builder builder = null;
                while (input.ReadTag(ref num, ref str))
                {
                    if ((num == 0) && (str != null))
                    {
                        int index = Array.BinarySearch<string>(objectICarriableTrans._objectICarriableTransFieldNames, str, StringComparer.Ordinal);
                        if (index >= 0)
                        {
                            num = objectICarriableTrans._objectICarriableTransFieldTags[index];
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
                    this.result.hasTransCarrierID = input.ReadInt32(ref this.result.transCarrierID_);
                }
                if (builder != null)
                {
                    this.set_UnknownFields(builder.Build());
                }
                return this;
            }

            private objectICarriableTrans PrepareBuilder()
            {
                if (this.resultIsReadOnly)
                {
                    objectICarriableTrans result = this.result;
                    this.result = new objectICarriableTrans();
                    this.resultIsReadOnly = false;
                    this.MergeFrom(result);
                }
                return this.result;
            }

            public objectICarriableTrans.Builder SetTransCarrierID(int value)
            {
                this.PrepareBuilder();
                this.result.hasTransCarrierID = true;
                this.result.transCarrierID_ = value;
                return this;
            }

            public override objectICarriableTrans DefaultInstanceForType
            {
                get
                {
                    return objectICarriableTrans.DefaultInstance;
                }
            }

            public override MessageDescriptor DescriptorForType
            {
                get
                {
                    return objectICarriableTrans.Descriptor;
                }
            }

            public bool HasTransCarrierID
            {
                get
                {
                    return this.result.hasTransCarrierID;
                }
            }

            public override bool IsInitialized
            {
                get
                {
                    return this.result.IsInitialized;
                }
            }

            protected override objectICarriableTrans MessageBeingBuilt
            {
                get
                {
                    return this.PrepareBuilder();
                }
            }

            protected override objectICarriableTrans.Builder ThisBuilder
            {
                get
                {
                    return this;
                }
            }

            public int TransCarrierID
            {
                get
                {
                    return this.result.TransCarrierID;
                }
                set
                {
                    this.SetTransCarrierID(value);
                }
            }
        }
    }
}

