namespace RustProto
{
    using Google.ProtocolBuffers;
    using Google.ProtocolBuffers.Descriptors;
    using Google.ProtocolBuffers.FieldAccess;
    using RustProto.Helpers;
    using System;
    using System.Diagnostics;
    using System.IO;
    using UnityEngine;

    [DebuggerNonUserCode]
    public sealed class Quaternion : GeneratedMessage<Quaternion, Quaternion.Builder>
    {
        private static readonly string[] _quaternionFieldNames = new string[] { "w", "x", "y", "z" };
        private static readonly uint[] _quaternionFieldTags = new uint[] { 0x25, 13, 0x15, 0x1d };
        private static readonly Quaternion defaultInstance = new Quaternion().MakeReadOnly();
        private bool hasW;
        private bool hasX;
        private bool hasY;
        private bool hasZ;
        private int memoizedSerializedSize = -1;
        private float w_;
        public const int WFieldNumber = 4;
        private float x_;
        public const int XFieldNumber = 1;
        private float y_;
        public const int YFieldNumber = 2;
        private float z_;
        public const int ZFieldNumber = 3;

        static Quaternion()
        {
            object.ReferenceEquals(Common.Descriptor, null);
        }

        private Quaternion()
        {
        }

        public static Builder CreateBuilder()
        {
            return new Builder();
        }

        public static Builder CreateBuilder(Quaternion prototype)
        {
            return new Builder(prototype);
        }

        public override Builder CreateBuilderForType()
        {
            return new Builder();
        }

        private Quaternion MakeReadOnly()
        {
            return this;
        }

        public static implicit operator Quaternion(Quaternion v)
        {
            using (Recycler<Quaternion, Builder> recycler = Recycler())
            {
                Builder builder = recycler.OpenBuilder();
                builder.SetX(v.x);
                builder.SetY(v.y);
                builder.SetZ(v.z);
                builder.SetW(v.w);
                return builder.Build();
            }
        }

        public static Quaternion ParseDelimitedFrom(Stream input)
        {
            return CreateBuilder().MergeDelimitedFrom(input).BuildParsed();
        }

        public static Quaternion ParseDelimitedFrom(Stream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeDelimitedFrom(input, extensionRegistry).BuildParsed();
        }

        public static Quaternion ParseFrom(ByteString data)
        {
            return CreateBuilder().MergeFrom(data).BuildParsed();
        }

        public static Quaternion ParseFrom(byte[] data)
        {
            return CreateBuilder().MergeFrom(data).BuildParsed();
        }

        public static Quaternion ParseFrom(ICodedInputStream input)
        {
            return CreateBuilder().MergeFrom(input).BuildParsed();
        }

        public static Quaternion ParseFrom(Stream input)
        {
            return CreateBuilder().MergeFrom(input).BuildParsed();
        }

        public static Quaternion ParseFrom(ByteString data, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
        }

        public static Quaternion ParseFrom(byte[] data, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
        }

        public static Quaternion ParseFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
        }

        public static Quaternion ParseFrom(Stream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
        }

        public static Recycler<Quaternion, Builder> Recycler()
        {
            return Recycler<Quaternion, Builder>.Manufacture();
        }

        public override Builder ToBuilder()
        {
            return CreateBuilder(this);
        }

        public override void WriteTo(ICodedOutputStream output)
        {
            int serializedSize = this.SerializedSize;
            string[] strArray = _quaternionFieldNames;
            if (this.hasX)
            {
                output.WriteFloat(1, strArray[1], this.X);
            }
            if (this.hasY)
            {
                output.WriteFloat(2, strArray[2], this.Y);
            }
            if (this.hasZ)
            {
                output.WriteFloat(3, strArray[3], this.Z);
            }
            if (this.hasW)
            {
                output.WriteFloat(4, strArray[0], this.W);
            }
            this.get_UnknownFields().WriteTo(output);
        }

        public static Quaternion DefaultInstance
        {
            get
            {
                return defaultInstance;
            }
        }

        public override Quaternion DefaultInstanceForType
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
                return Common.internal__static_RustProto_Quaternion__Descriptor;
            }
        }

        public bool HasW
        {
            get
            {
                return this.hasW;
            }
        }

        public bool HasX
        {
            get
            {
                return this.hasX;
            }
        }

        public bool HasY
        {
            get
            {
                return this.hasY;
            }
        }

        public bool HasZ
        {
            get
            {
                return this.hasZ;
            }
        }

        protected override FieldAccessorTable<Quaternion, Builder> InternalFieldAccessors
        {
            get
            {
                return Common.internal__static_RustProto_Quaternion__FieldAccessorTable;
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
                    if (this.hasX)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeFloatSize(1, this.X);
                    }
                    if (this.hasY)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeFloatSize(2, this.Y);
                    }
                    if (this.hasZ)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeFloatSize(3, this.Z);
                    }
                    if (this.hasW)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeFloatSize(4, this.W);
                    }
                    memoizedSerializedSize += this.get_UnknownFields().get_SerializedSize();
                    this.memoizedSerializedSize = memoizedSerializedSize;
                }
                return memoizedSerializedSize;
            }
        }

        protected override Quaternion ThisMessage
        {
            get
            {
                return this;
            }
        }

        public float W
        {
            get
            {
                return this.w_;
            }
        }

        public float X
        {
            get
            {
                return this.x_;
            }
        }

        public float Y
        {
            get
            {
                return this.y_;
            }
        }

        public float Z
        {
            get
            {
                return this.z_;
            }
        }

        [DebuggerNonUserCode]
        public sealed class Builder : GeneratedBuilder<Quaternion, Quaternion.Builder>
        {
            private Quaternion result;
            private bool resultIsReadOnly;

            public Builder()
            {
                this.result = Quaternion.DefaultInstance;
                this.resultIsReadOnly = true;
            }

            internal Builder(Quaternion cloneFrom)
            {
                this.result = cloneFrom;
                this.resultIsReadOnly = true;
            }

            public override Quaternion BuildPartial()
            {
                if (this.resultIsReadOnly)
                {
                    return this.result;
                }
                this.resultIsReadOnly = true;
                return this.result.MakeReadOnly();
            }

            public override Quaternion.Builder Clear()
            {
                this.result = Quaternion.DefaultInstance;
                this.resultIsReadOnly = true;
                return this;
            }

            public Quaternion.Builder ClearW()
            {
                this.PrepareBuilder();
                this.result.hasW = false;
                this.result.w_ = 0f;
                return this;
            }

            public Quaternion.Builder ClearX()
            {
                this.PrepareBuilder();
                this.result.hasX = false;
                this.result.x_ = 0f;
                return this;
            }

            public Quaternion.Builder ClearY()
            {
                this.PrepareBuilder();
                this.result.hasY = false;
                this.result.y_ = 0f;
                return this;
            }

            public Quaternion.Builder ClearZ()
            {
                this.PrepareBuilder();
                this.result.hasZ = false;
                this.result.z_ = 0f;
                return this;
            }

            public override Quaternion.Builder Clone()
            {
                if (this.resultIsReadOnly)
                {
                    return new Quaternion.Builder(this.result);
                }
                return new Quaternion.Builder().MergeFrom(this.result);
            }

            public override Quaternion.Builder MergeFrom(ICodedInputStream input)
            {
                return this.MergeFrom(input, ExtensionRegistry.get_Empty());
            }

            public override Quaternion.Builder MergeFrom(IMessage other)
            {
                if (other is Quaternion)
                {
                    return this.MergeFrom((Quaternion) other);
                }
                base.MergeFrom(other);
                return this;
            }

            public override Quaternion.Builder MergeFrom(Quaternion other)
            {
                if (other != Quaternion.DefaultInstance)
                {
                    this.PrepareBuilder();
                    if (other.HasX)
                    {
                        this.X = other.X;
                    }
                    if (other.HasY)
                    {
                        this.Y = other.Y;
                    }
                    if (other.HasZ)
                    {
                        this.Z = other.Z;
                    }
                    if (other.HasW)
                    {
                        this.W = other.W;
                    }
                    this.MergeUnknownFields(other.get_UnknownFields());
                }
                return this;
            }

            public override Quaternion.Builder MergeFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
            {
                uint num;
                string str;
                this.PrepareBuilder();
                UnknownFieldSet.Builder builder = null;
                while (input.ReadTag(ref num, ref str))
                {
                    if ((num == 0) && (str != null))
                    {
                        int index = Array.BinarySearch<string>(Quaternion._quaternionFieldNames, str, StringComparer.Ordinal);
                        if (index >= 0)
                        {
                            num = Quaternion._quaternionFieldTags[index];
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
                        {
                            this.result.hasX = input.ReadFloat(ref this.result.x_);
                            continue;
                        }
                        case 0x15:
                        {
                            this.result.hasY = input.ReadFloat(ref this.result.y_);
                            continue;
                        }
                        case 0x1d:
                        {
                            this.result.hasZ = input.ReadFloat(ref this.result.z_);
                            continue;
                        }
                        case 0x25:
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
                    this.result.hasW = input.ReadFloat(ref this.result.w_);
                }
                if (builder != null)
                {
                    this.set_UnknownFields(builder.Build());
                }
                return this;
            }

            private Quaternion PrepareBuilder()
            {
                if (this.resultIsReadOnly)
                {
                    Quaternion result = this.result;
                    this.result = new Quaternion();
                    this.resultIsReadOnly = false;
                    this.MergeFrom(result);
                }
                return this.result;
            }

            public void Set(Quaternion value)
            {
                this.SetX(value.x);
                this.SetY(value.y);
                this.SetZ(value.z);
                this.SetW(value.w);
            }

            public Quaternion.Builder SetW(float value)
            {
                this.PrepareBuilder();
                this.result.hasW = true;
                this.result.w_ = value;
                return this;
            }

            public Quaternion.Builder SetX(float value)
            {
                this.PrepareBuilder();
                this.result.hasX = true;
                this.result.x_ = value;
                return this;
            }

            public Quaternion.Builder SetY(float value)
            {
                this.PrepareBuilder();
                this.result.hasY = true;
                this.result.y_ = value;
                return this;
            }

            public Quaternion.Builder SetZ(float value)
            {
                this.PrepareBuilder();
                this.result.hasZ = true;
                this.result.z_ = value;
                return this;
            }

            public override Quaternion DefaultInstanceForType
            {
                get
                {
                    return Quaternion.DefaultInstance;
                }
            }

            public override MessageDescriptor DescriptorForType
            {
                get
                {
                    return Quaternion.Descriptor;
                }
            }

            public bool HasW
            {
                get
                {
                    return this.result.hasW;
                }
            }

            public bool HasX
            {
                get
                {
                    return this.result.hasX;
                }
            }

            public bool HasY
            {
                get
                {
                    return this.result.hasY;
                }
            }

            public bool HasZ
            {
                get
                {
                    return this.result.hasZ;
                }
            }

            public override bool IsInitialized
            {
                get
                {
                    return this.result.IsInitialized;
                }
            }

            protected override Quaternion MessageBeingBuilt
            {
                get
                {
                    return this.PrepareBuilder();
                }
            }

            protected override Quaternion.Builder ThisBuilder
            {
                get
                {
                    return this;
                }
            }

            public float W
            {
                get
                {
                    return this.result.W;
                }
                set
                {
                    this.SetW(value);
                }
            }

            public float X
            {
                get
                {
                    return this.result.X;
                }
                set
                {
                    this.SetX(value);
                }
            }

            public float Y
            {
                get
                {
                    return this.result.Y;
                }
                set
                {
                    this.SetY(value);
                }
            }

            public float Z
            {
                get
                {
                    return this.result.Z;
                }
                set
                {
                    this.SetZ(value);
                }
            }
        }
    }
}

