namespace RustProto
{
    using Google.ProtocolBuffers;
    using Google.ProtocolBuffers.Descriptors;
    using Google.ProtocolBuffers.FieldAccess;
    using RustProto.Proto;
    using System;
    using System.Diagnostics;
    using System.IO;

    [DebuggerNonUserCode]
    public sealed class Error : GeneratedMessage<Error, Error.Builder>
    {
        private static readonly string[] _errorFieldNames = new string[] { "message", "status" };
        private static readonly uint[] _errorFieldTags = new uint[] { 0x12, 10 };
        private static readonly Error defaultInstance = new Error().MakeReadOnly();
        private bool hasMessage;
        private bool hasStatus;
        private int memoizedSerializedSize = -1;
        private string message_ = string.Empty;
        public const int MessageFieldNumber = 2;
        private string status_ = string.Empty;
        public const int StatusFieldNumber = 1;

        static Error()
        {
            object.ReferenceEquals(Error.Descriptor, null);
        }

        private Error()
        {
        }

        public static Builder CreateBuilder()
        {
            return new Builder();
        }

        public static Builder CreateBuilder(Error prototype)
        {
            return new Builder(prototype);
        }

        public override Builder CreateBuilderForType()
        {
            return new Builder();
        }

        private Error MakeReadOnly()
        {
            return this;
        }

        public static Error ParseDelimitedFrom(Stream input)
        {
            return CreateBuilder().MergeDelimitedFrom(input).BuildParsed();
        }

        public static Error ParseDelimitedFrom(Stream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeDelimitedFrom(input, extensionRegistry).BuildParsed();
        }

        public static Error ParseFrom(ByteString data)
        {
            return CreateBuilder().MergeFrom(data).BuildParsed();
        }

        public static Error ParseFrom(byte[] data)
        {
            return CreateBuilder().MergeFrom(data).BuildParsed();
        }

        public static Error ParseFrom(ICodedInputStream input)
        {
            return CreateBuilder().MergeFrom(input).BuildParsed();
        }

        public static Error ParseFrom(Stream input)
        {
            return CreateBuilder().MergeFrom(input).BuildParsed();
        }

        public static Error ParseFrom(ByteString data, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
        }

        public static Error ParseFrom(byte[] data, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
        }

        public static Error ParseFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
        }

        public static Error ParseFrom(Stream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
        }

        public override Builder ToBuilder()
        {
            return CreateBuilder(this);
        }

        public override void WriteTo(ICodedOutputStream output)
        {
            int serializedSize = this.SerializedSize;
            string[] strArray = _errorFieldNames;
            if (this.hasStatus)
            {
                output.WriteString(1, strArray[1], this.Status);
            }
            if (this.hasMessage)
            {
                output.WriteString(2, strArray[0], this.Message);
            }
            this.get_UnknownFields().WriteTo(output);
        }

        public static Error DefaultInstance
        {
            get
            {
                return defaultInstance;
            }
        }

        public override Error DefaultInstanceForType
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
                return Error.internal__static_RustProto_Error__Descriptor;
            }
        }

        public bool HasMessage
        {
            get
            {
                return this.hasMessage;
            }
        }

        public bool HasStatus
        {
            get
            {
                return this.hasStatus;
            }
        }

        protected override FieldAccessorTable<Error, Builder> InternalFieldAccessors
        {
            get
            {
                return Error.internal__static_RustProto_Error__FieldAccessorTable;
            }
        }

        public override bool IsInitialized
        {
            get
            {
                if (!this.hasStatus)
                {
                    return false;
                }
                if (!this.hasMessage)
                {
                    return false;
                }
                return true;
            }
        }

        public string Message
        {
            get
            {
                return this.message_;
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
                    if (this.hasStatus)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeStringSize(1, this.Status);
                    }
                    if (this.hasMessage)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeStringSize(2, this.Message);
                    }
                    memoizedSerializedSize += this.get_UnknownFields().get_SerializedSize();
                    this.memoizedSerializedSize = memoizedSerializedSize;
                }
                return memoizedSerializedSize;
            }
        }

        public string Status
        {
            get
            {
                return this.status_;
            }
        }

        protected override Error ThisMessage
        {
            get
            {
                return this;
            }
        }

        [DebuggerNonUserCode]
        public sealed class Builder : GeneratedBuilder<Error, Error.Builder>
        {
            private Error result;
            private bool resultIsReadOnly;

            public Builder()
            {
                this.result = Error.DefaultInstance;
                this.resultIsReadOnly = true;
            }

            internal Builder(Error cloneFrom)
            {
                this.result = cloneFrom;
                this.resultIsReadOnly = true;
            }

            public override Error BuildPartial()
            {
                if (this.resultIsReadOnly)
                {
                    return this.result;
                }
                this.resultIsReadOnly = true;
                return this.result.MakeReadOnly();
            }

            public override Error.Builder Clear()
            {
                this.result = Error.DefaultInstance;
                this.resultIsReadOnly = true;
                return this;
            }

            public Error.Builder ClearMessage()
            {
                this.PrepareBuilder();
                this.result.hasMessage = false;
                this.result.message_ = string.Empty;
                return this;
            }

            public Error.Builder ClearStatus()
            {
                this.PrepareBuilder();
                this.result.hasStatus = false;
                this.result.status_ = string.Empty;
                return this;
            }

            public override Error.Builder Clone()
            {
                if (this.resultIsReadOnly)
                {
                    return new Error.Builder(this.result);
                }
                return new Error.Builder().MergeFrom(this.result);
            }

            public override Error.Builder MergeFrom(ICodedInputStream input)
            {
                return this.MergeFrom(input, ExtensionRegistry.get_Empty());
            }

            public override Error.Builder MergeFrom(IMessage other)
            {
                if (other is Error)
                {
                    return this.MergeFrom((Error) other);
                }
                base.MergeFrom(other);
                return this;
            }

            public override Error.Builder MergeFrom(Error other)
            {
                if (other != Error.DefaultInstance)
                {
                    this.PrepareBuilder();
                    if (other.HasStatus)
                    {
                        this.Status = other.Status;
                    }
                    if (other.HasMessage)
                    {
                        this.Message = other.Message;
                    }
                    this.MergeUnknownFields(other.get_UnknownFields());
                }
                return this;
            }

            public override Error.Builder MergeFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
            {
                uint num;
                string str;
                this.PrepareBuilder();
                UnknownFieldSet.Builder builder = null;
                while (input.ReadTag(ref num, ref str))
                {
                    if ((num == 0) && (str != null))
                    {
                        int index = Array.BinarySearch<string>(Error._errorFieldNames, str, StringComparer.Ordinal);
                        if (index >= 0)
                        {
                            num = Error._errorFieldTags[index];
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

                        case 10:
                        {
                            this.result.hasStatus = input.ReadString(ref this.result.status_);
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
                    this.result.hasMessage = input.ReadString(ref this.result.message_);
                }
                if (builder != null)
                {
                    this.set_UnknownFields(builder.Build());
                }
                return this;
            }

            private Error PrepareBuilder()
            {
                if (this.resultIsReadOnly)
                {
                    Error result = this.result;
                    this.result = new Error();
                    this.resultIsReadOnly = false;
                    this.MergeFrom(result);
                }
                return this.result;
            }

            public Error.Builder SetMessage(string value)
            {
                ThrowHelper.ThrowIfNull(value, "value");
                this.PrepareBuilder();
                this.result.hasMessage = true;
                this.result.message_ = value;
                return this;
            }

            public Error.Builder SetStatus(string value)
            {
                ThrowHelper.ThrowIfNull(value, "value");
                this.PrepareBuilder();
                this.result.hasStatus = true;
                this.result.status_ = value;
                return this;
            }

            public override Error DefaultInstanceForType
            {
                get
                {
                    return Error.DefaultInstance;
                }
            }

            public override MessageDescriptor DescriptorForType
            {
                get
                {
                    return Error.Descriptor;
                }
            }

            public bool HasMessage
            {
                get
                {
                    return this.result.hasMessage;
                }
            }

            public bool HasStatus
            {
                get
                {
                    return this.result.hasStatus;
                }
            }

            public override bool IsInitialized
            {
                get
                {
                    return this.result.IsInitialized;
                }
            }

            public string Message
            {
                get
                {
                    return this.result.Message;
                }
                set
                {
                    this.SetMessage(value);
                }
            }

            protected override Error MessageBeingBuilt
            {
                get
                {
                    return this.PrepareBuilder();
                }
            }

            public string Status
            {
                get
                {
                    return this.result.Status;
                }
                set
                {
                    this.SetStatus(value);
                }
            }

            protected override Error.Builder ThisBuilder
            {
                get
                {
                    return this;
                }
            }
        }
    }
}

