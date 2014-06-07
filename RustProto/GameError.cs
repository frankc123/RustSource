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
    public sealed class GameError : GeneratedMessage<GameError, GameError.Builder>
    {
        private static readonly string[] _gameErrorFieldNames = new string[] { "error", "trace" };
        private static readonly uint[] _gameErrorFieldTags = new uint[] { 10, 0x12 };
        private static readonly GameError defaultInstance = new GameError().MakeReadOnly();
        private string error_ = string.Empty;
        public const int ErrorFieldNumber = 1;
        private bool hasError;
        private bool hasTrace;
        private int memoizedSerializedSize = -1;
        private string trace_ = string.Empty;
        public const int TraceFieldNumber = 2;

        static GameError()
        {
            object.ReferenceEquals(RustProto.Proto.Error.Descriptor, null);
        }

        private GameError()
        {
        }

        public static Builder CreateBuilder()
        {
            return new Builder();
        }

        public static Builder CreateBuilder(GameError prototype)
        {
            return new Builder(prototype);
        }

        public override Builder CreateBuilderForType()
        {
            return new Builder();
        }

        private GameError MakeReadOnly()
        {
            return this;
        }

        public static GameError ParseDelimitedFrom(Stream input)
        {
            return CreateBuilder().MergeDelimitedFrom(input).BuildParsed();
        }

        public static GameError ParseDelimitedFrom(Stream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeDelimitedFrom(input, extensionRegistry).BuildParsed();
        }

        public static GameError ParseFrom(ByteString data)
        {
            return CreateBuilder().MergeFrom(data).BuildParsed();
        }

        public static GameError ParseFrom(byte[] data)
        {
            return CreateBuilder().MergeFrom(data).BuildParsed();
        }

        public static GameError ParseFrom(ICodedInputStream input)
        {
            return CreateBuilder().MergeFrom(input).BuildParsed();
        }

        public static GameError ParseFrom(Stream input)
        {
            return CreateBuilder().MergeFrom(input).BuildParsed();
        }

        public static GameError ParseFrom(ByteString data, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
        }

        public static GameError ParseFrom(byte[] data, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
        }

        public static GameError ParseFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
        }

        public static GameError ParseFrom(Stream input, ExtensionRegistry extensionRegistry)
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
            string[] strArray = _gameErrorFieldNames;
            if (this.hasError)
            {
                output.WriteString(1, strArray[0], this.Error);
            }
            if (this.hasTrace)
            {
                output.WriteString(2, strArray[1], this.Trace);
            }
            this.get_UnknownFields().WriteTo(output);
        }

        public static GameError DefaultInstance
        {
            get
            {
                return defaultInstance;
            }
        }

        public override GameError DefaultInstanceForType
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
                return RustProto.Proto.Error.internal__static_RustProto_GameError__Descriptor;
            }
        }

        public string Error
        {
            get
            {
                return this.error_;
            }
        }

        public bool HasError
        {
            get
            {
                return this.hasError;
            }
        }

        public bool HasTrace
        {
            get
            {
                return this.hasTrace;
            }
        }

        protected override FieldAccessorTable<GameError, Builder> InternalFieldAccessors
        {
            get
            {
                return RustProto.Proto.Error.internal__static_RustProto_GameError__FieldAccessorTable;
            }
        }

        public override bool IsInitialized
        {
            get
            {
                if (!this.hasError)
                {
                    return false;
                }
                if (!this.hasTrace)
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
                    if (this.hasError)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeStringSize(1, this.Error);
                    }
                    if (this.hasTrace)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeStringSize(2, this.Trace);
                    }
                    memoizedSerializedSize += this.get_UnknownFields().get_SerializedSize();
                    this.memoizedSerializedSize = memoizedSerializedSize;
                }
                return memoizedSerializedSize;
            }
        }

        protected override GameError ThisMessage
        {
            get
            {
                return this;
            }
        }

        public string Trace
        {
            get
            {
                return this.trace_;
            }
        }

        [DebuggerNonUserCode]
        public sealed class Builder : GeneratedBuilder<GameError, GameError.Builder>
        {
            private GameError result;
            private bool resultIsReadOnly;

            public Builder()
            {
                this.result = GameError.DefaultInstance;
                this.resultIsReadOnly = true;
            }

            internal Builder(GameError cloneFrom)
            {
                this.result = cloneFrom;
                this.resultIsReadOnly = true;
            }

            public override GameError BuildPartial()
            {
                if (this.resultIsReadOnly)
                {
                    return this.result;
                }
                this.resultIsReadOnly = true;
                return this.result.MakeReadOnly();
            }

            public override GameError.Builder Clear()
            {
                this.result = GameError.DefaultInstance;
                this.resultIsReadOnly = true;
                return this;
            }

            public GameError.Builder ClearError()
            {
                this.PrepareBuilder();
                this.result.hasError = false;
                this.result.error_ = string.Empty;
                return this;
            }

            public GameError.Builder ClearTrace()
            {
                this.PrepareBuilder();
                this.result.hasTrace = false;
                this.result.trace_ = string.Empty;
                return this;
            }

            public override GameError.Builder Clone()
            {
                if (this.resultIsReadOnly)
                {
                    return new GameError.Builder(this.result);
                }
                return new GameError.Builder().MergeFrom(this.result);
            }

            public override GameError.Builder MergeFrom(ICodedInputStream input)
            {
                return this.MergeFrom(input, ExtensionRegistry.get_Empty());
            }

            public override GameError.Builder MergeFrom(IMessage other)
            {
                if (other is GameError)
                {
                    return this.MergeFrom((GameError) other);
                }
                base.MergeFrom(other);
                return this;
            }

            public override GameError.Builder MergeFrom(GameError other)
            {
                if (other != GameError.DefaultInstance)
                {
                    this.PrepareBuilder();
                    if (other.HasError)
                    {
                        this.Error = other.Error;
                    }
                    if (other.HasTrace)
                    {
                        this.Trace = other.Trace;
                    }
                    this.MergeUnknownFields(other.get_UnknownFields());
                }
                return this;
            }

            public override GameError.Builder MergeFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
            {
                uint num;
                string str;
                this.PrepareBuilder();
                UnknownFieldSet.Builder builder = null;
                while (input.ReadTag(ref num, ref str))
                {
                    if ((num == 0) && (str != null))
                    {
                        int index = Array.BinarySearch<string>(GameError._gameErrorFieldNames, str, StringComparer.Ordinal);
                        if (index >= 0)
                        {
                            num = GameError._gameErrorFieldTags[index];
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
                            this.result.hasError = input.ReadString(ref this.result.error_);
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
                    this.result.hasTrace = input.ReadString(ref this.result.trace_);
                }
                if (builder != null)
                {
                    this.set_UnknownFields(builder.Build());
                }
                return this;
            }

            private GameError PrepareBuilder()
            {
                if (this.resultIsReadOnly)
                {
                    GameError result = this.result;
                    this.result = new GameError();
                    this.resultIsReadOnly = false;
                    this.MergeFrom(result);
                }
                return this.result;
            }

            public GameError.Builder SetError(string value)
            {
                ThrowHelper.ThrowIfNull(value, "value");
                this.PrepareBuilder();
                this.result.hasError = true;
                this.result.error_ = value;
                return this;
            }

            public GameError.Builder SetTrace(string value)
            {
                ThrowHelper.ThrowIfNull(value, "value");
                this.PrepareBuilder();
                this.result.hasTrace = true;
                this.result.trace_ = value;
                return this;
            }

            public override GameError DefaultInstanceForType
            {
                get
                {
                    return GameError.DefaultInstance;
                }
            }

            public override MessageDescriptor DescriptorForType
            {
                get
                {
                    return GameError.Descriptor;
                }
            }

            public string Error
            {
                get
                {
                    return this.result.Error;
                }
                set
                {
                    this.SetError(value);
                }
            }

            public bool HasError
            {
                get
                {
                    return this.result.hasError;
                }
            }

            public bool HasTrace
            {
                get
                {
                    return this.result.hasTrace;
                }
            }

            public override bool IsInitialized
            {
                get
                {
                    return this.result.IsInitialized;
                }
            }

            protected override GameError MessageBeingBuilt
            {
                get
                {
                    return this.PrepareBuilder();
                }
            }

            protected override GameError.Builder ThisBuilder
            {
                get
                {
                    return this;
                }
            }

            public string Trace
            {
                get
                {
                    return this.result.Trace;
                }
                set
                {
                    this.SetTrace(value);
                }
            }
        }
    }
}

