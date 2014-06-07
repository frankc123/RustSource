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
    public sealed class Vector : GeneratedMessage<Vector, Vector.Builder>
    {
        private static readonly string[] _vectorFieldNames = new string[] { "x", "y", "z" };
        private static readonly uint[] _vectorFieldTags = new uint[] { 13, 0x15, 0x1d };
        private static readonly Vector defaultInstance = new Vector().MakeReadOnly();
        private bool hasX;
        private bool hasY;
        private bool hasZ;
        private int memoizedSerializedSize = -1;
        private float x_;
        public const int XFieldNumber = 1;
        private float y_;
        public const int YFieldNumber = 2;
        private float z_;
        public const int ZFieldNumber = 3;

        static Vector()
        {
            object.ReferenceEquals(Common.Descriptor, null);
        }

        private Vector()
        {
        }

        public static Builder CreateBuilder()
        {
            return new Builder();
        }

        public static Builder CreateBuilder(Vector prototype)
        {
            return new Builder(prototype);
        }

        public override Builder CreateBuilderForType()
        {
            return new Builder();
        }

        private Vector MakeReadOnly()
        {
            return this;
        }

        public static implicit operator Vector(Vector3 v)
        {
            using (Recycler<Vector, Builder> recycler = Recycler())
            {
                Builder builder = recycler.OpenBuilder();
                builder.SetX(v.x);
                builder.SetY(v.y);
                builder.SetZ(v.z);
                return builder.Build();
            }
        }

        public static Vector ParseDelimitedFrom(Stream input)
        {
            return CreateBuilder().MergeDelimitedFrom(input).BuildParsed();
        }

        public static Vector ParseDelimitedFrom(Stream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeDelimitedFrom(input, extensionRegistry).BuildParsed();
        }

        public static Vector ParseFrom(ByteString data)
        {
            return CreateBuilder().MergeFrom(data).BuildParsed();
        }

        public static Vector ParseFrom(byte[] data)
        {
            return CreateBuilder().MergeFrom(data).BuildParsed();
        }

        public static Vector ParseFrom(ICodedInputStream input)
        {
            return CreateBuilder().MergeFrom(input).BuildParsed();
        }

        public static Vector ParseFrom(Stream input)
        {
            return CreateBuilder().MergeFrom(input).BuildParsed();
        }

        public static Vector ParseFrom(ByteString data, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
        }

        public static Vector ParseFrom(byte[] data, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
        }

        public static Vector ParseFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
        }

        public static Vector ParseFrom(Stream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
        }

        public static Recycler<Vector, Builder> Recycler()
        {
            return Recycler<Vector, Builder>.Manufacture();
        }

        public override Builder ToBuilder()
        {
            return CreateBuilder(this);
        }

        public override void WriteTo(ICodedOutputStream output)
        {
            int serializedSize = this.SerializedSize;
            string[] strArray = _vectorFieldNames;
            if (this.hasX)
            {
                output.WriteFloat(1, strArray[0], this.X);
            }
            if (this.hasY)
            {
                output.WriteFloat(2, strArray[1], this.Y);
            }
            if (this.hasZ)
            {
                output.WriteFloat(3, strArray[2], this.Z);
            }
            this.get_UnknownFields().WriteTo(output);
        }

        public static Vector DefaultInstance
        {
            get
            {
                return defaultInstance;
            }
        }

        public override Vector DefaultInstanceForType
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
                return Common.internal__static_RustProto_Vector__Descriptor;
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

        protected override FieldAccessorTable<Vector, Builder> InternalFieldAccessors
        {
            get
            {
                return Common.internal__static_RustProto_Vector__FieldAccessorTable;
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
                    memoizedSerializedSize += this.get_UnknownFields().get_SerializedSize();
                    this.memoizedSerializedSize = memoizedSerializedSize;
                }
                return memoizedSerializedSize;
            }
        }

        protected override Vector ThisMessage
        {
            get
            {
                return this;
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
        public sealed class Builder : GeneratedBuilder<Vector, Vector.Builder>
        {
            private Vector result;
            private bool resultIsReadOnly;

            public Builder()
            {
                this.result = Vector.DefaultInstance;
                this.resultIsReadOnly = true;
            }

            internal Builder(Vector cloneFrom)
            {
                this.result = cloneFrom;
                this.resultIsReadOnly = true;
            }

            public override Vector BuildPartial()
            {
                if (this.resultIsReadOnly)
                {
                    return this.result;
                }
                this.resultIsReadOnly = true;
                return this.result.MakeReadOnly();
            }

            public override Vector.Builder Clear()
            {
                this.result = Vector.DefaultInstance;
                this.resultIsReadOnly = true;
                return this;
            }

            public Vector.Builder ClearX()
            {
                this.PrepareBuilder();
                this.result.hasX = false;
                this.result.x_ = 0f;
                return this;
            }

            public Vector.Builder ClearY()
            {
                this.PrepareBuilder();
                this.result.hasY = false;
                this.result.y_ = 0f;
                return this;
            }

            public Vector.Builder ClearZ()
            {
                this.PrepareBuilder();
                this.result.hasZ = false;
                this.result.z_ = 0f;
                return this;
            }

            public override Vector.Builder Clone()
            {
                if (this.resultIsReadOnly)
                {
                    return new Vector.Builder(this.result);
                }
                return new Vector.Builder().MergeFrom(this.result);
            }

            public override Vector.Builder MergeFrom(ICodedInputStream input)
            {
                return this.MergeFrom(input, ExtensionRegistry.get_Empty());
            }

            public override Vector.Builder MergeFrom(IMessage other)
            {
                if (other is Vector)
                {
                    return this.MergeFrom((Vector) other);
                }
                base.MergeFrom(other);
                return this;
            }

            public override Vector.Builder MergeFrom(Vector other)
            {
                if (other != Vector.DefaultInstance)
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
                    this.MergeUnknownFields(other.get_UnknownFields());
                }
                return this;
            }

            public override Vector.Builder MergeFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
            {
                uint num;
                string str;
                this.PrepareBuilder();
                UnknownFieldSet.Builder builder = null;
                while (input.ReadTag(ref num, ref str))
                {
                    if ((num == 0) && (str != null))
                    {
                        int index = Array.BinarySearch<string>(Vector._vectorFieldNames, str, StringComparer.Ordinal);
                        if (index >= 0)
                        {
                            num = Vector._vectorFieldTags[index];
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
                    this.result.hasZ = input.ReadFloat(ref this.result.z_);
                }
                if (builder != null)
                {
                    this.set_UnknownFields(builder.Build());
                }
                return this;
            }

            private Vector PrepareBuilder()
            {
                if (this.resultIsReadOnly)
                {
                    Vector result = this.result;
                    this.result = new Vector();
                    this.resultIsReadOnly = false;
                    this.MergeFrom(result);
                }
                return this.result;
            }

            public void Set(Vector3 value)
            {
                this.SetX(value.x);
                this.SetY(value.y);
                this.SetZ(value.z);
            }

            public Vector.Builder SetX(float value)
            {
                this.PrepareBuilder();
                this.result.hasX = true;
                this.result.x_ = value;
                return this;
            }

            public Vector.Builder SetY(float value)
            {
                this.PrepareBuilder();
                this.result.hasY = true;
                this.result.y_ = value;
                return this;
            }

            public Vector.Builder SetZ(float value)
            {
                this.PrepareBuilder();
                this.result.hasZ = true;
                this.result.z_ = value;
                return this;
            }

            public override Vector DefaultInstanceForType
            {
                get
                {
                    return Vector.DefaultInstance;
                }
            }

            public override MessageDescriptor DescriptorForType
            {
                get
                {
                    return Vector.Descriptor;
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

            protected override Vector MessageBeingBuilt
            {
                get
                {
                    return this.PrepareBuilder();
                }
            }

            protected override Vector.Builder ThisBuilder
            {
                get
                {
                    return this;
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

