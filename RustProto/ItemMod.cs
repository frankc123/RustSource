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
    public sealed class ItemMod : GeneratedMessage<ItemMod, ItemMod.Builder>
    {
        private static readonly string[] _itemModFieldNames = new string[] { "id", "name" };
        private static readonly uint[] _itemModFieldTags = new uint[] { 8, 0x12 };
        private static readonly ItemMod defaultInstance = new ItemMod().MakeReadOnly();
        private bool hasId;
        private bool hasName;
        private int id_;
        public const int IdFieldNumber = 1;
        private int memoizedSerializedSize = -1;
        private string name_ = string.Empty;
        public const int NameFieldNumber = 2;

        static ItemMod()
        {
            object.ReferenceEquals(ItemMod.Descriptor, null);
        }

        private ItemMod()
        {
        }

        public static Builder CreateBuilder()
        {
            return new Builder();
        }

        public static Builder CreateBuilder(ItemMod prototype)
        {
            return new Builder(prototype);
        }

        public override Builder CreateBuilderForType()
        {
            return new Builder();
        }

        private ItemMod MakeReadOnly()
        {
            return this;
        }

        public static ItemMod ParseDelimitedFrom(Stream input)
        {
            return CreateBuilder().MergeDelimitedFrom(input).BuildParsed();
        }

        public static ItemMod ParseDelimitedFrom(Stream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeDelimitedFrom(input, extensionRegistry).BuildParsed();
        }

        public static ItemMod ParseFrom(ByteString data)
        {
            return CreateBuilder().MergeFrom(data).BuildParsed();
        }

        public static ItemMod ParseFrom(byte[] data)
        {
            return CreateBuilder().MergeFrom(data).BuildParsed();
        }

        public static ItemMod ParseFrom(ICodedInputStream input)
        {
            return CreateBuilder().MergeFrom(input).BuildParsed();
        }

        public static ItemMod ParseFrom(Stream input)
        {
            return CreateBuilder().MergeFrom(input).BuildParsed();
        }

        public static ItemMod ParseFrom(ByteString data, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
        }

        public static ItemMod ParseFrom(byte[] data, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
        }

        public static ItemMod ParseFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
        }

        public static ItemMod ParseFrom(Stream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
        }

        public static Recycler<ItemMod, Builder> Recycler()
        {
            return Recycler<ItemMod, Builder>.Manufacture();
        }

        public override Builder ToBuilder()
        {
            return CreateBuilder(this);
        }

        public override void WriteTo(ICodedOutputStream output)
        {
            int serializedSize = this.SerializedSize;
            string[] strArray = _itemModFieldNames;
            if (this.hasId)
            {
                output.WriteInt32(1, strArray[0], this.Id);
            }
            if (this.hasName)
            {
                output.WriteString(2, strArray[1], this.Name);
            }
            this.get_UnknownFields().WriteTo(output);
        }

        public static ItemMod DefaultInstance
        {
            get
            {
                return defaultInstance;
            }
        }

        public override ItemMod DefaultInstanceForType
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
                return ItemMod.internal__static_RustProto_ItemMod__Descriptor;
            }
        }

        public bool HasId
        {
            get
            {
                return this.hasId;
            }
        }

        public bool HasName
        {
            get
            {
                return this.hasName;
            }
        }

        public int Id
        {
            get
            {
                return this.id_;
            }
        }

        protected override FieldAccessorTable<ItemMod, Builder> InternalFieldAccessors
        {
            get
            {
                return ItemMod.internal__static_RustProto_ItemMod__FieldAccessorTable;
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

        public string Name
        {
            get
            {
                return this.name_;
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
                    if (this.hasName)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeStringSize(2, this.Name);
                    }
                    memoizedSerializedSize += this.get_UnknownFields().get_SerializedSize();
                    this.memoizedSerializedSize = memoizedSerializedSize;
                }
                return memoizedSerializedSize;
            }
        }

        protected override ItemMod ThisMessage
        {
            get
            {
                return this;
            }
        }

        [DebuggerNonUserCode]
        public sealed class Builder : GeneratedBuilder<ItemMod, ItemMod.Builder>
        {
            private ItemMod result;
            private bool resultIsReadOnly;

            public Builder()
            {
                this.result = ItemMod.DefaultInstance;
                this.resultIsReadOnly = true;
            }

            internal Builder(ItemMod cloneFrom)
            {
                this.result = cloneFrom;
                this.resultIsReadOnly = true;
            }

            public override ItemMod BuildPartial()
            {
                if (this.resultIsReadOnly)
                {
                    return this.result;
                }
                this.resultIsReadOnly = true;
                return this.result.MakeReadOnly();
            }

            public override ItemMod.Builder Clear()
            {
                this.result = ItemMod.DefaultInstance;
                this.resultIsReadOnly = true;
                return this;
            }

            public ItemMod.Builder ClearId()
            {
                this.PrepareBuilder();
                this.result.hasId = false;
                this.result.id_ = 0;
                return this;
            }

            public ItemMod.Builder ClearName()
            {
                this.PrepareBuilder();
                this.result.hasName = false;
                this.result.name_ = string.Empty;
                return this;
            }

            public override ItemMod.Builder Clone()
            {
                if (this.resultIsReadOnly)
                {
                    return new ItemMod.Builder(this.result);
                }
                return new ItemMod.Builder().MergeFrom(this.result);
            }

            public override ItemMod.Builder MergeFrom(ICodedInputStream input)
            {
                return this.MergeFrom(input, ExtensionRegistry.get_Empty());
            }

            public override ItemMod.Builder MergeFrom(IMessage other)
            {
                if (other is ItemMod)
                {
                    return this.MergeFrom((ItemMod) other);
                }
                base.MergeFrom(other);
                return this;
            }

            public override ItemMod.Builder MergeFrom(ItemMod other)
            {
                if (other != ItemMod.DefaultInstance)
                {
                    this.PrepareBuilder();
                    if (other.HasId)
                    {
                        this.Id = other.Id;
                    }
                    if (other.HasName)
                    {
                        this.Name = other.Name;
                    }
                    this.MergeUnknownFields(other.get_UnknownFields());
                }
                return this;
            }

            public override ItemMod.Builder MergeFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
            {
                uint num;
                string str;
                this.PrepareBuilder();
                UnknownFieldSet.Builder builder = null;
                while (input.ReadTag(ref num, ref str))
                {
                    if ((num == 0) && (str != null))
                    {
                        int index = Array.BinarySearch<string>(ItemMod._itemModFieldNames, str, StringComparer.Ordinal);
                        if (index >= 0)
                        {
                            num = ItemMod._itemModFieldTags[index];
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
                            this.result.hasId = input.ReadInt32(ref this.result.id_);
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
                    this.result.hasName = input.ReadString(ref this.result.name_);
                }
                if (builder != null)
                {
                    this.set_UnknownFields(builder.Build());
                }
                return this;
            }

            private ItemMod PrepareBuilder()
            {
                if (this.resultIsReadOnly)
                {
                    ItemMod result = this.result;
                    this.result = new ItemMod();
                    this.resultIsReadOnly = false;
                    this.MergeFrom(result);
                }
                return this.result;
            }

            public ItemMod.Builder SetId(int value)
            {
                this.PrepareBuilder();
                this.result.hasId = true;
                this.result.id_ = value;
                return this;
            }

            public ItemMod.Builder SetName(string value)
            {
                ThrowHelper.ThrowIfNull(value, "value");
                this.PrepareBuilder();
                this.result.hasName = true;
                this.result.name_ = value;
                return this;
            }

            public override ItemMod DefaultInstanceForType
            {
                get
                {
                    return ItemMod.DefaultInstance;
                }
            }

            public override MessageDescriptor DescriptorForType
            {
                get
                {
                    return ItemMod.Descriptor;
                }
            }

            public bool HasId
            {
                get
                {
                    return this.result.hasId;
                }
            }

            public bool HasName
            {
                get
                {
                    return this.result.hasName;
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

            protected override ItemMod MessageBeingBuilt
            {
                get
                {
                    return this.PrepareBuilder();
                }
            }

            public string Name
            {
                get
                {
                    return this.result.Name;
                }
                set
                {
                    this.SetName(value);
                }
            }

            protected override ItemMod.Builder ThisBuilder
            {
                get
                {
                    return this;
                }
            }
        }
    }
}

