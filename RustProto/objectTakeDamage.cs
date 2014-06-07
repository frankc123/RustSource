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
    public sealed class objectTakeDamage : GeneratedMessage<objectTakeDamage, objectTakeDamage.Builder>
    {
        private static readonly string[] _objectTakeDamageFieldNames = new string[] { "health" };
        private static readonly uint[] _objectTakeDamageFieldTags = new uint[] { 13 };
        private static readonly objectTakeDamage defaultInstance = new objectTakeDamage().MakeReadOnly();
        private bool hasHealth;
        private float health_;
        public const int HealthFieldNumber = 1;
        private int memoizedSerializedSize = -1;

        static objectTakeDamage()
        {
            object.ReferenceEquals(Worldsave.Descriptor, null);
        }

        private objectTakeDamage()
        {
        }

        public static Builder CreateBuilder()
        {
            return new Builder();
        }

        public static Builder CreateBuilder(objectTakeDamage prototype)
        {
            return new Builder(prototype);
        }

        public override Builder CreateBuilderForType()
        {
            return new Builder();
        }

        private objectTakeDamage MakeReadOnly()
        {
            return this;
        }

        public static objectTakeDamage ParseDelimitedFrom(Stream input)
        {
            return CreateBuilder().MergeDelimitedFrom(input).BuildParsed();
        }

        public static objectTakeDamage ParseDelimitedFrom(Stream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeDelimitedFrom(input, extensionRegistry).BuildParsed();
        }

        public static objectTakeDamage ParseFrom(ByteString data)
        {
            return CreateBuilder().MergeFrom(data).BuildParsed();
        }

        public static objectTakeDamage ParseFrom(byte[] data)
        {
            return CreateBuilder().MergeFrom(data).BuildParsed();
        }

        public static objectTakeDamage ParseFrom(ICodedInputStream input)
        {
            return CreateBuilder().MergeFrom(input).BuildParsed();
        }

        public static objectTakeDamage ParseFrom(Stream input)
        {
            return CreateBuilder().MergeFrom(input).BuildParsed();
        }

        public static objectTakeDamage ParseFrom(ByteString data, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
        }

        public static objectTakeDamage ParseFrom(byte[] data, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
        }

        public static objectTakeDamage ParseFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
        }

        public static objectTakeDamage ParseFrom(Stream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
        }

        public static Recycler<objectTakeDamage, Builder> Recycler()
        {
            return Recycler<objectTakeDamage, Builder>.Manufacture();
        }

        public override Builder ToBuilder()
        {
            return CreateBuilder(this);
        }

        public override void WriteTo(ICodedOutputStream output)
        {
            int serializedSize = this.SerializedSize;
            string[] strArray = _objectTakeDamageFieldNames;
            if (this.hasHealth)
            {
                output.WriteFloat(1, strArray[0], this.Health);
            }
            this.get_UnknownFields().WriteTo(output);
        }

        public static objectTakeDamage DefaultInstance
        {
            get
            {
                return defaultInstance;
            }
        }

        public override objectTakeDamage DefaultInstanceForType
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
                return Worldsave.internal__static_RustProto_objectTakeDamage__Descriptor;
            }
        }

        public bool HasHealth
        {
            get
            {
                return this.hasHealth;
            }
        }

        public float Health
        {
            get
            {
                return this.health_;
            }
        }

        protected override FieldAccessorTable<objectTakeDamage, Builder> InternalFieldAccessors
        {
            get
            {
                return Worldsave.internal__static_RustProto_objectTakeDamage__FieldAccessorTable;
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
                    if (this.hasHealth)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeFloatSize(1, this.Health);
                    }
                    memoizedSerializedSize += this.get_UnknownFields().get_SerializedSize();
                    this.memoizedSerializedSize = memoizedSerializedSize;
                }
                return memoizedSerializedSize;
            }
        }

        protected override objectTakeDamage ThisMessage
        {
            get
            {
                return this;
            }
        }

        [DebuggerNonUserCode]
        public sealed class Builder : GeneratedBuilder<objectTakeDamage, objectTakeDamage.Builder>
        {
            private objectTakeDamage result;
            private bool resultIsReadOnly;

            public Builder()
            {
                this.result = objectTakeDamage.DefaultInstance;
                this.resultIsReadOnly = true;
            }

            internal Builder(objectTakeDamage cloneFrom)
            {
                this.result = cloneFrom;
                this.resultIsReadOnly = true;
            }

            public override objectTakeDamage BuildPartial()
            {
                if (this.resultIsReadOnly)
                {
                    return this.result;
                }
                this.resultIsReadOnly = true;
                return this.result.MakeReadOnly();
            }

            public override objectTakeDamage.Builder Clear()
            {
                this.result = objectTakeDamage.DefaultInstance;
                this.resultIsReadOnly = true;
                return this;
            }

            public objectTakeDamage.Builder ClearHealth()
            {
                this.PrepareBuilder();
                this.result.hasHealth = false;
                this.result.health_ = 0f;
                return this;
            }

            public override objectTakeDamage.Builder Clone()
            {
                if (this.resultIsReadOnly)
                {
                    return new objectTakeDamage.Builder(this.result);
                }
                return new objectTakeDamage.Builder().MergeFrom(this.result);
            }

            public override objectTakeDamage.Builder MergeFrom(ICodedInputStream input)
            {
                return this.MergeFrom(input, ExtensionRegistry.get_Empty());
            }

            public override objectTakeDamage.Builder MergeFrom(IMessage other)
            {
                if (other is objectTakeDamage)
                {
                    return this.MergeFrom((objectTakeDamage) other);
                }
                base.MergeFrom(other);
                return this;
            }

            public override objectTakeDamage.Builder MergeFrom(objectTakeDamage other)
            {
                if (other != objectTakeDamage.DefaultInstance)
                {
                    this.PrepareBuilder();
                    if (other.HasHealth)
                    {
                        this.Health = other.Health;
                    }
                    this.MergeUnknownFields(other.get_UnknownFields());
                }
                return this;
            }

            public override objectTakeDamage.Builder MergeFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
            {
                uint num;
                string str;
                this.PrepareBuilder();
                UnknownFieldSet.Builder builder = null;
                while (input.ReadTag(ref num, ref str))
                {
                    if ((num == 0) && (str != null))
                    {
                        int index = Array.BinarySearch<string>(objectTakeDamage._objectTakeDamageFieldNames, str, StringComparer.Ordinal);
                        if (index >= 0)
                        {
                            num = objectTakeDamage._objectTakeDamageFieldTags[index];
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

                        case 13:
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
                    this.result.hasHealth = input.ReadFloat(ref this.result.health_);
                }
                if (builder != null)
                {
                    this.set_UnknownFields(builder.Build());
                }
                return this;
            }

            private objectTakeDamage PrepareBuilder()
            {
                if (this.resultIsReadOnly)
                {
                    objectTakeDamage result = this.result;
                    this.result = new objectTakeDamage();
                    this.resultIsReadOnly = false;
                    this.MergeFrom(result);
                }
                return this.result;
            }

            public objectTakeDamage.Builder SetHealth(float value)
            {
                this.PrepareBuilder();
                this.result.hasHealth = true;
                this.result.health_ = value;
                return this;
            }

            public override objectTakeDamage DefaultInstanceForType
            {
                get
                {
                    return objectTakeDamage.DefaultInstance;
                }
            }

            public override MessageDescriptor DescriptorForType
            {
                get
                {
                    return objectTakeDamage.Descriptor;
                }
            }

            public bool HasHealth
            {
                get
                {
                    return this.result.hasHealth;
                }
            }

            public float Health
            {
                get
                {
                    return this.result.Health;
                }
                set
                {
                    this.SetHealth(value);
                }
            }

            public override bool IsInitialized
            {
                get
                {
                    return this.result.IsInitialized;
                }
            }

            protected override objectTakeDamage MessageBeingBuilt
            {
                get
                {
                    return this.PrepareBuilder();
                }
            }

            protected override objectTakeDamage.Builder ThisBuilder
            {
                get
                {
                    return this;
                }
            }
        }
    }
}

