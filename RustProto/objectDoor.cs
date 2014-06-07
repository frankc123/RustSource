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
    public sealed class objectDoor : GeneratedMessage<objectDoor, objectDoor.Builder>
    {
        private static readonly string[] _objectDoorFieldNames = new string[] { "Open", "State" };
        private static readonly uint[] _objectDoorFieldTags = new uint[] { 0x10, 8 };
        private static readonly objectDoor defaultInstance = new objectDoor().MakeReadOnly();
        private bool hasOpen;
        private bool hasState;
        private int memoizedSerializedSize = -1;
        private bool open_;
        public const int OpenFieldNumber = 2;
        private int state_;
        public const int StateFieldNumber = 1;

        static objectDoor()
        {
            object.ReferenceEquals(Worldsave.Descriptor, null);
        }

        private objectDoor()
        {
        }

        public static Builder CreateBuilder()
        {
            return new Builder();
        }

        public static Builder CreateBuilder(objectDoor prototype)
        {
            return new Builder(prototype);
        }

        public override Builder CreateBuilderForType()
        {
            return new Builder();
        }

        private objectDoor MakeReadOnly()
        {
            return this;
        }

        public static objectDoor ParseDelimitedFrom(Stream input)
        {
            return CreateBuilder().MergeDelimitedFrom(input).BuildParsed();
        }

        public static objectDoor ParseDelimitedFrom(Stream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeDelimitedFrom(input, extensionRegistry).BuildParsed();
        }

        public static objectDoor ParseFrom(ByteString data)
        {
            return CreateBuilder().MergeFrom(data).BuildParsed();
        }

        public static objectDoor ParseFrom(byte[] data)
        {
            return CreateBuilder().MergeFrom(data).BuildParsed();
        }

        public static objectDoor ParseFrom(ICodedInputStream input)
        {
            return CreateBuilder().MergeFrom(input).BuildParsed();
        }

        public static objectDoor ParseFrom(Stream input)
        {
            return CreateBuilder().MergeFrom(input).BuildParsed();
        }

        public static objectDoor ParseFrom(ByteString data, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
        }

        public static objectDoor ParseFrom(byte[] data, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
        }

        public static objectDoor ParseFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
        }

        public static objectDoor ParseFrom(Stream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
        }

        public static Recycler<objectDoor, Builder> Recycler()
        {
            return Recycler<objectDoor, Builder>.Manufacture();
        }

        public override Builder ToBuilder()
        {
            return CreateBuilder(this);
        }

        public override void WriteTo(ICodedOutputStream output)
        {
            int serializedSize = this.SerializedSize;
            string[] strArray = _objectDoorFieldNames;
            if (this.hasState)
            {
                output.WriteInt32(1, strArray[1], this.State);
            }
            if (this.hasOpen)
            {
                output.WriteBool(2, strArray[0], this.Open);
            }
            this.get_UnknownFields().WriteTo(output);
        }

        public static objectDoor DefaultInstance
        {
            get
            {
                return defaultInstance;
            }
        }

        public override objectDoor DefaultInstanceForType
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
                return Worldsave.internal__static_RustProto_objectDoor__Descriptor;
            }
        }

        public bool HasOpen
        {
            get
            {
                return this.hasOpen;
            }
        }

        public bool HasState
        {
            get
            {
                return this.hasState;
            }
        }

        protected override FieldAccessorTable<objectDoor, Builder> InternalFieldAccessors
        {
            get
            {
                return Worldsave.internal__static_RustProto_objectDoor__FieldAccessorTable;
            }
        }

        public override bool IsInitialized
        {
            get
            {
                return true;
            }
        }

        public bool Open
        {
            get
            {
                return this.open_;
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
                    if (this.hasState)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeInt32Size(1, this.State);
                    }
                    if (this.hasOpen)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeBoolSize(2, this.Open);
                    }
                    memoizedSerializedSize += this.get_UnknownFields().get_SerializedSize();
                    this.memoizedSerializedSize = memoizedSerializedSize;
                }
                return memoizedSerializedSize;
            }
        }

        public int State
        {
            get
            {
                return this.state_;
            }
        }

        protected override objectDoor ThisMessage
        {
            get
            {
                return this;
            }
        }

        [DebuggerNonUserCode]
        public sealed class Builder : GeneratedBuilder<objectDoor, objectDoor.Builder>
        {
            private objectDoor result;
            private bool resultIsReadOnly;

            public Builder()
            {
                this.result = objectDoor.DefaultInstance;
                this.resultIsReadOnly = true;
            }

            internal Builder(objectDoor cloneFrom)
            {
                this.result = cloneFrom;
                this.resultIsReadOnly = true;
            }

            public override objectDoor BuildPartial()
            {
                if (this.resultIsReadOnly)
                {
                    return this.result;
                }
                this.resultIsReadOnly = true;
                return this.result.MakeReadOnly();
            }

            public override objectDoor.Builder Clear()
            {
                this.result = objectDoor.DefaultInstance;
                this.resultIsReadOnly = true;
                return this;
            }

            public objectDoor.Builder ClearOpen()
            {
                this.PrepareBuilder();
                this.result.hasOpen = false;
                this.result.open_ = false;
                return this;
            }

            public objectDoor.Builder ClearState()
            {
                this.PrepareBuilder();
                this.result.hasState = false;
                this.result.state_ = 0;
                return this;
            }

            public override objectDoor.Builder Clone()
            {
                if (this.resultIsReadOnly)
                {
                    return new objectDoor.Builder(this.result);
                }
                return new objectDoor.Builder().MergeFrom(this.result);
            }

            public override objectDoor.Builder MergeFrom(ICodedInputStream input)
            {
                return this.MergeFrom(input, ExtensionRegistry.get_Empty());
            }

            public override objectDoor.Builder MergeFrom(IMessage other)
            {
                if (other is objectDoor)
                {
                    return this.MergeFrom((objectDoor) other);
                }
                base.MergeFrom(other);
                return this;
            }

            public override objectDoor.Builder MergeFrom(objectDoor other)
            {
                if (other != objectDoor.DefaultInstance)
                {
                    this.PrepareBuilder();
                    if (other.HasState)
                    {
                        this.State = other.State;
                    }
                    if (other.HasOpen)
                    {
                        this.Open = other.Open;
                    }
                    this.MergeUnknownFields(other.get_UnknownFields());
                }
                return this;
            }

            public override objectDoor.Builder MergeFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
            {
                uint num;
                string str;
                this.PrepareBuilder();
                UnknownFieldSet.Builder builder = null;
                while (input.ReadTag(ref num, ref str))
                {
                    if ((num == 0) && (str != null))
                    {
                        int index = Array.BinarySearch<string>(objectDoor._objectDoorFieldNames, str, StringComparer.Ordinal);
                        if (index >= 0)
                        {
                            num = objectDoor._objectDoorFieldTags[index];
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
                            this.result.hasState = input.ReadInt32(ref this.result.state_);
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
                    this.result.hasOpen = input.ReadBool(ref this.result.open_);
                }
                if (builder != null)
                {
                    this.set_UnknownFields(builder.Build());
                }
                return this;
            }

            private objectDoor PrepareBuilder()
            {
                if (this.resultIsReadOnly)
                {
                    objectDoor result = this.result;
                    this.result = new objectDoor();
                    this.resultIsReadOnly = false;
                    this.MergeFrom(result);
                }
                return this.result;
            }

            public objectDoor.Builder SetOpen(bool value)
            {
                this.PrepareBuilder();
                this.result.hasOpen = true;
                this.result.open_ = value;
                return this;
            }

            public objectDoor.Builder SetState(int value)
            {
                this.PrepareBuilder();
                this.result.hasState = true;
                this.result.state_ = value;
                return this;
            }

            public override objectDoor DefaultInstanceForType
            {
                get
                {
                    return objectDoor.DefaultInstance;
                }
            }

            public override MessageDescriptor DescriptorForType
            {
                get
                {
                    return objectDoor.Descriptor;
                }
            }

            public bool HasOpen
            {
                get
                {
                    return this.result.hasOpen;
                }
            }

            public bool HasState
            {
                get
                {
                    return this.result.hasState;
                }
            }

            public override bool IsInitialized
            {
                get
                {
                    return this.result.IsInitialized;
                }
            }

            protected override objectDoor MessageBeingBuilt
            {
                get
                {
                    return this.PrepareBuilder();
                }
            }

            public bool Open
            {
                get
                {
                    return this.result.Open;
                }
                set
                {
                    this.SetOpen(value);
                }
            }

            public int State
            {
                get
                {
                    return this.result.State;
                }
                set
                {
                    this.SetState(value);
                }
            }

            protected override objectDoor.Builder ThisBuilder
            {
                get
                {
                    return this;
                }
            }
        }
    }
}

