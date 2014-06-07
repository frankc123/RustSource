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
    public sealed class objectFireBarrel : GeneratedMessage<objectFireBarrel, objectFireBarrel.Builder>
    {
        private static readonly string[] _objectFireBarrelFieldNames = new string[] { "OnFire" };
        private static readonly uint[] _objectFireBarrelFieldTags = new uint[] { 8 };
        private static readonly objectFireBarrel defaultInstance = new objectFireBarrel().MakeReadOnly();
        private bool hasOnFire;
        private int memoizedSerializedSize = -1;
        private bool onFire_;
        public const int OnFireFieldNumber = 1;

        static objectFireBarrel()
        {
            object.ReferenceEquals(Worldsave.Descriptor, null);
        }

        private objectFireBarrel()
        {
        }

        public static Builder CreateBuilder()
        {
            return new Builder();
        }

        public static Builder CreateBuilder(objectFireBarrel prototype)
        {
            return new Builder(prototype);
        }

        public override Builder CreateBuilderForType()
        {
            return new Builder();
        }

        private objectFireBarrel MakeReadOnly()
        {
            return this;
        }

        public static objectFireBarrel ParseDelimitedFrom(Stream input)
        {
            return CreateBuilder().MergeDelimitedFrom(input).BuildParsed();
        }

        public static objectFireBarrel ParseDelimitedFrom(Stream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeDelimitedFrom(input, extensionRegistry).BuildParsed();
        }

        public static objectFireBarrel ParseFrom(ByteString data)
        {
            return CreateBuilder().MergeFrom(data).BuildParsed();
        }

        public static objectFireBarrel ParseFrom(byte[] data)
        {
            return CreateBuilder().MergeFrom(data).BuildParsed();
        }

        public static objectFireBarrel ParseFrom(ICodedInputStream input)
        {
            return CreateBuilder().MergeFrom(input).BuildParsed();
        }

        public static objectFireBarrel ParseFrom(Stream input)
        {
            return CreateBuilder().MergeFrom(input).BuildParsed();
        }

        public static objectFireBarrel ParseFrom(ByteString data, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
        }

        public static objectFireBarrel ParseFrom(byte[] data, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
        }

        public static objectFireBarrel ParseFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
        }

        public static objectFireBarrel ParseFrom(Stream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
        }

        public static Recycler<objectFireBarrel, Builder> Recycler()
        {
            return Recycler<objectFireBarrel, Builder>.Manufacture();
        }

        public override Builder ToBuilder()
        {
            return CreateBuilder(this);
        }

        public override void WriteTo(ICodedOutputStream output)
        {
            int serializedSize = this.SerializedSize;
            string[] strArray = _objectFireBarrelFieldNames;
            if (this.hasOnFire)
            {
                output.WriteBool(1, strArray[0], this.OnFire);
            }
            this.get_UnknownFields().WriteTo(output);
        }

        public static objectFireBarrel DefaultInstance
        {
            get
            {
                return defaultInstance;
            }
        }

        public override objectFireBarrel DefaultInstanceForType
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
                return Worldsave.internal__static_RustProto_objectFireBarrel__Descriptor;
            }
        }

        public bool HasOnFire
        {
            get
            {
                return this.hasOnFire;
            }
        }

        protected override FieldAccessorTable<objectFireBarrel, Builder> InternalFieldAccessors
        {
            get
            {
                return Worldsave.internal__static_RustProto_objectFireBarrel__FieldAccessorTable;
            }
        }

        public override bool IsInitialized
        {
            get
            {
                return true;
            }
        }

        public bool OnFire
        {
            get
            {
                return this.onFire_;
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
                    if (this.hasOnFire)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeBoolSize(1, this.OnFire);
                    }
                    memoizedSerializedSize += this.get_UnknownFields().get_SerializedSize();
                    this.memoizedSerializedSize = memoizedSerializedSize;
                }
                return memoizedSerializedSize;
            }
        }

        protected override objectFireBarrel ThisMessage
        {
            get
            {
                return this;
            }
        }

        [DebuggerNonUserCode]
        public sealed class Builder : GeneratedBuilder<objectFireBarrel, objectFireBarrel.Builder>
        {
            private objectFireBarrel result;
            private bool resultIsReadOnly;

            public Builder()
            {
                this.result = objectFireBarrel.DefaultInstance;
                this.resultIsReadOnly = true;
            }

            internal Builder(objectFireBarrel cloneFrom)
            {
                this.result = cloneFrom;
                this.resultIsReadOnly = true;
            }

            public override objectFireBarrel BuildPartial()
            {
                if (this.resultIsReadOnly)
                {
                    return this.result;
                }
                this.resultIsReadOnly = true;
                return this.result.MakeReadOnly();
            }

            public override objectFireBarrel.Builder Clear()
            {
                this.result = objectFireBarrel.DefaultInstance;
                this.resultIsReadOnly = true;
                return this;
            }

            public objectFireBarrel.Builder ClearOnFire()
            {
                this.PrepareBuilder();
                this.result.hasOnFire = false;
                this.result.onFire_ = false;
                return this;
            }

            public override objectFireBarrel.Builder Clone()
            {
                if (this.resultIsReadOnly)
                {
                    return new objectFireBarrel.Builder(this.result);
                }
                return new objectFireBarrel.Builder().MergeFrom(this.result);
            }

            public override objectFireBarrel.Builder MergeFrom(ICodedInputStream input)
            {
                return this.MergeFrom(input, ExtensionRegistry.get_Empty());
            }

            public override objectFireBarrel.Builder MergeFrom(IMessage other)
            {
                if (other is objectFireBarrel)
                {
                    return this.MergeFrom((objectFireBarrel) other);
                }
                base.MergeFrom(other);
                return this;
            }

            public override objectFireBarrel.Builder MergeFrom(objectFireBarrel other)
            {
                if (other != objectFireBarrel.DefaultInstance)
                {
                    this.PrepareBuilder();
                    if (other.HasOnFire)
                    {
                        this.OnFire = other.OnFire;
                    }
                    this.MergeUnknownFields(other.get_UnknownFields());
                }
                return this;
            }

            public override objectFireBarrel.Builder MergeFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
            {
                uint num;
                string str;
                this.PrepareBuilder();
                UnknownFieldSet.Builder builder = null;
                while (input.ReadTag(ref num, ref str))
                {
                    if ((num == 0) && (str != null))
                    {
                        int index = Array.BinarySearch<string>(objectFireBarrel._objectFireBarrelFieldNames, str, StringComparer.Ordinal);
                        if (index >= 0)
                        {
                            num = objectFireBarrel._objectFireBarrelFieldTags[index];
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
                    this.result.hasOnFire = input.ReadBool(ref this.result.onFire_);
                }
                if (builder != null)
                {
                    this.set_UnknownFields(builder.Build());
                }
                return this;
            }

            private objectFireBarrel PrepareBuilder()
            {
                if (this.resultIsReadOnly)
                {
                    objectFireBarrel result = this.result;
                    this.result = new objectFireBarrel();
                    this.resultIsReadOnly = false;
                    this.MergeFrom(result);
                }
                return this.result;
            }

            public objectFireBarrel.Builder SetOnFire(bool value)
            {
                this.PrepareBuilder();
                this.result.hasOnFire = true;
                this.result.onFire_ = value;
                return this;
            }

            public override objectFireBarrel DefaultInstanceForType
            {
                get
                {
                    return objectFireBarrel.DefaultInstance;
                }
            }

            public override MessageDescriptor DescriptorForType
            {
                get
                {
                    return objectFireBarrel.Descriptor;
                }
            }

            public bool HasOnFire
            {
                get
                {
                    return this.result.hasOnFire;
                }
            }

            public override bool IsInitialized
            {
                get
                {
                    return this.result.IsInitialized;
                }
            }

            protected override objectFireBarrel MessageBeingBuilt
            {
                get
                {
                    return this.PrepareBuilder();
                }
            }

            public bool OnFire
            {
                get
                {
                    return this.result.OnFire;
                }
                set
                {
                    this.SetOnFire(value);
                }
            }

            protected override objectFireBarrel.Builder ThisBuilder
            {
                get
                {
                    return this;
                }
            }
        }
    }
}

