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
    public sealed class Blueprint : GeneratedMessage<Blueprint, Blueprint.Builder>
    {
        private static readonly string[] _blueprintFieldNames = new string[] { "id" };
        private static readonly uint[] _blueprintFieldTags = new uint[] { 8 };
        private static readonly Blueprint defaultInstance = new Blueprint().MakeReadOnly();
        private bool hasId;
        private int id_;
        public const int IdFieldNumber = 1;
        private int memoizedSerializedSize = -1;

        static Blueprint()
        {
            object.ReferenceEquals(Blueprint.Descriptor, null);
        }

        private Blueprint()
        {
        }

        public static Builder CreateBuilder()
        {
            return new Builder();
        }

        public static Builder CreateBuilder(Blueprint prototype)
        {
            return new Builder(prototype);
        }

        public override Builder CreateBuilderForType()
        {
            return new Builder();
        }

        private Blueprint MakeReadOnly()
        {
            return this;
        }

        public static Blueprint ParseDelimitedFrom(Stream input)
        {
            return CreateBuilder().MergeDelimitedFrom(input).BuildParsed();
        }

        public static Blueprint ParseDelimitedFrom(Stream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeDelimitedFrom(input, extensionRegistry).BuildParsed();
        }

        public static Blueprint ParseFrom(ByteString data)
        {
            return CreateBuilder().MergeFrom(data).BuildParsed();
        }

        public static Blueprint ParseFrom(byte[] data)
        {
            return CreateBuilder().MergeFrom(data).BuildParsed();
        }

        public static Blueprint ParseFrom(ICodedInputStream input)
        {
            return CreateBuilder().MergeFrom(input).BuildParsed();
        }

        public static Blueprint ParseFrom(Stream input)
        {
            return CreateBuilder().MergeFrom(input).BuildParsed();
        }

        public static Blueprint ParseFrom(ByteString data, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
        }

        public static Blueprint ParseFrom(byte[] data, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
        }

        public static Blueprint ParseFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
        }

        public static Blueprint ParseFrom(Stream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
        }

        public static Recycler<Blueprint, Builder> Recycler()
        {
            return Recycler<Blueprint, Builder>.Manufacture();
        }

        public override Builder ToBuilder()
        {
            return CreateBuilder(this);
        }

        public override void WriteTo(ICodedOutputStream output)
        {
            int serializedSize = this.SerializedSize;
            string[] strArray = _blueprintFieldNames;
            if (this.hasId)
            {
                output.WriteInt32(1, strArray[0], this.Id);
            }
            this.get_UnknownFields().WriteTo(output);
        }

        public static Blueprint DefaultInstance
        {
            get
            {
                return defaultInstance;
            }
        }

        public override Blueprint DefaultInstanceForType
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
                return Blueprint.internal__static_RustProto_Blueprint__Descriptor;
            }
        }

        public bool HasId
        {
            get
            {
                return this.hasId;
            }
        }

        public int Id
        {
            get
            {
                return this.id_;
            }
        }

        protected override FieldAccessorTable<Blueprint, Builder> InternalFieldAccessors
        {
            get
            {
                return Blueprint.internal__static_RustProto_Blueprint__FieldAccessorTable;
            }
        }

        public override bool IsInitialized
        {
            get
            {
                if (!this.hasId)
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
                    if (this.hasId)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeInt32Size(1, this.Id);
                    }
                    memoizedSerializedSize += this.get_UnknownFields().get_SerializedSize();
                    this.memoizedSerializedSize = memoizedSerializedSize;
                }
                return memoizedSerializedSize;
            }
        }

        protected override Blueprint ThisMessage
        {
            get
            {
                return this;
            }
        }

        [DebuggerNonUserCode]
        public sealed class Builder : GeneratedBuilder<Blueprint, Blueprint.Builder>
        {
            private Blueprint result;
            private bool resultIsReadOnly;

            public Builder()
            {
                this.result = Blueprint.DefaultInstance;
                this.resultIsReadOnly = true;
            }

            internal Builder(Blueprint cloneFrom)
            {
                this.result = cloneFrom;
                this.resultIsReadOnly = true;
            }

            public override Blueprint BuildPartial()
            {
                if (this.resultIsReadOnly)
                {
                    return this.result;
                }
                this.resultIsReadOnly = true;
                return this.result.MakeReadOnly();
            }

            public override Blueprint.Builder Clear()
            {
                this.result = Blueprint.DefaultInstance;
                this.resultIsReadOnly = true;
                return this;
            }

            public Blueprint.Builder ClearId()
            {
                this.PrepareBuilder();
                this.result.hasId = false;
                this.result.id_ = 0;
                return this;
            }

            public override Blueprint.Builder Clone()
            {
                if (this.resultIsReadOnly)
                {
                    return new Blueprint.Builder(this.result);
                }
                return new Blueprint.Builder().MergeFrom(this.result);
            }

            public override Blueprint.Builder MergeFrom(ICodedInputStream input)
            {
                return this.MergeFrom(input, ExtensionRegistry.get_Empty());
            }

            public override Blueprint.Builder MergeFrom(IMessage other)
            {
                if (other is Blueprint)
                {
                    return this.MergeFrom((Blueprint) other);
                }
                base.MergeFrom(other);
                return this;
            }

            public override Blueprint.Builder MergeFrom(Blueprint other)
            {
                if (other != Blueprint.DefaultInstance)
                {
                    this.PrepareBuilder();
                    if (other.HasId)
                    {
                        this.Id = other.Id;
                    }
                    this.MergeUnknownFields(other.get_UnknownFields());
                }
                return this;
            }

            public override Blueprint.Builder MergeFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
            {
                uint num;
                string str;
                this.PrepareBuilder();
                UnknownFieldSet.Builder builder = null;
                while (input.ReadTag(ref num, ref str))
                {
                    if ((num == 0) && (str != null))
                    {
                        int index = Array.BinarySearch<string>(Blueprint._blueprintFieldNames, str, StringComparer.Ordinal);
                        if (index >= 0)
                        {
                            num = Blueprint._blueprintFieldTags[index];
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
                    this.result.hasId = input.ReadInt32(ref this.result.id_);
                }
                if (builder != null)
                {
                    this.set_UnknownFields(builder.Build());
                }
                return this;
            }

            private Blueprint PrepareBuilder()
            {
                if (this.resultIsReadOnly)
                {
                    Blueprint result = this.result;
                    this.result = new Blueprint();
                    this.resultIsReadOnly = false;
                    this.MergeFrom(result);
                }
                return this.result;
            }

            public Blueprint.Builder SetId(int value)
            {
                this.PrepareBuilder();
                this.result.hasId = true;
                this.result.id_ = value;
                return this;
            }

            public override Blueprint DefaultInstanceForType
            {
                get
                {
                    return Blueprint.DefaultInstance;
                }
            }

            public override MessageDescriptor DescriptorForType
            {
                get
                {
                    return Blueprint.Descriptor;
                }
            }

            public bool HasId
            {
                get
                {
                    return this.result.hasId;
                }
            }

            public int Id
            {
                get
                {
                    return this.result.Id;
                }
                set
                {
                    this.SetId(value);
                }
            }

            public override bool IsInitialized
            {
                get
                {
                    return this.result.IsInitialized;
                }
            }

            protected override Blueprint MessageBeingBuilt
            {
                get
                {
                    return this.PrepareBuilder();
                }
            }

            protected override Blueprint.Builder ThisBuilder
            {
                get
                {
                    return this;
                }
            }
        }
    }
}

