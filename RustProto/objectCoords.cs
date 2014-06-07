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
    public sealed class objectCoords : GeneratedMessage<objectCoords, objectCoords.Builder>
    {
        private static readonly string[] _objectCoordsFieldNames = new string[] { "oldPos", "oldRot", "pos", "rot" };
        private static readonly uint[] _objectCoordsFieldTags = new uint[] { 0x12, 0x22, 10, 0x1a };
        private static readonly objectCoords defaultInstance = new objectCoords().MakeReadOnly();
        private bool hasOldPos;
        private bool hasOldRot;
        private bool hasPos;
        private bool hasRot;
        private int memoizedSerializedSize = -1;
        private Vector oldPos_;
        public const int OldPosFieldNumber = 2;
        private Quaternion oldRot_;
        public const int OldRotFieldNumber = 4;
        private Vector pos_;
        public const int PosFieldNumber = 1;
        private Quaternion rot_;
        public const int RotFieldNumber = 3;

        static objectCoords()
        {
            object.ReferenceEquals(Worldsave.Descriptor, null);
        }

        private objectCoords()
        {
        }

        public static Builder CreateBuilder()
        {
            return new Builder();
        }

        public static Builder CreateBuilder(objectCoords prototype)
        {
            return new Builder(prototype);
        }

        public override Builder CreateBuilderForType()
        {
            return new Builder();
        }

        private objectCoords MakeReadOnly()
        {
            return this;
        }

        public static objectCoords ParseDelimitedFrom(Stream input)
        {
            return CreateBuilder().MergeDelimitedFrom(input).BuildParsed();
        }

        public static objectCoords ParseDelimitedFrom(Stream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeDelimitedFrom(input, extensionRegistry).BuildParsed();
        }

        public static objectCoords ParseFrom(ByteString data)
        {
            return CreateBuilder().MergeFrom(data).BuildParsed();
        }

        public static objectCoords ParseFrom(byte[] data)
        {
            return CreateBuilder().MergeFrom(data).BuildParsed();
        }

        public static objectCoords ParseFrom(ICodedInputStream input)
        {
            return CreateBuilder().MergeFrom(input).BuildParsed();
        }

        public static objectCoords ParseFrom(Stream input)
        {
            return CreateBuilder().MergeFrom(input).BuildParsed();
        }

        public static objectCoords ParseFrom(ByteString data, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
        }

        public static objectCoords ParseFrom(byte[] data, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
        }

        public static objectCoords ParseFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
        }

        public static objectCoords ParseFrom(Stream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
        }

        public static Recycler<objectCoords, Builder> Recycler()
        {
            return Recycler<objectCoords, Builder>.Manufacture();
        }

        public override Builder ToBuilder()
        {
            return CreateBuilder(this);
        }

        public override void WriteTo(ICodedOutputStream output)
        {
            int serializedSize = this.SerializedSize;
            string[] strArray = _objectCoordsFieldNames;
            if (this.hasPos)
            {
                output.WriteMessage(1, strArray[2], this.Pos);
            }
            if (this.hasOldPos)
            {
                output.WriteMessage(2, strArray[0], this.OldPos);
            }
            if (this.hasRot)
            {
                output.WriteMessage(3, strArray[3], this.Rot);
            }
            if (this.hasOldRot)
            {
                output.WriteMessage(4, strArray[1], this.OldRot);
            }
            this.get_UnknownFields().WriteTo(output);
        }

        public static objectCoords DefaultInstance
        {
            get
            {
                return defaultInstance;
            }
        }

        public override objectCoords DefaultInstanceForType
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
                return Worldsave.internal__static_RustProto_objectCoords__Descriptor;
            }
        }

        public bool HasOldPos
        {
            get
            {
                return this.hasOldPos;
            }
        }

        public bool HasOldRot
        {
            get
            {
                return this.hasOldRot;
            }
        }

        public bool HasPos
        {
            get
            {
                return this.hasPos;
            }
        }

        public bool HasRot
        {
            get
            {
                return this.hasRot;
            }
        }

        protected override FieldAccessorTable<objectCoords, Builder> InternalFieldAccessors
        {
            get
            {
                return Worldsave.internal__static_RustProto_objectCoords__FieldAccessorTable;
            }
        }

        public override bool IsInitialized
        {
            get
            {
                return true;
            }
        }

        public Vector OldPos
        {
            get
            {
                if (this.oldPos_ == null)
                {
                }
                return Vector.DefaultInstance;
            }
        }

        public Quaternion OldRot
        {
            get
            {
                if (this.oldRot_ == null)
                {
                }
                return Quaternion.DefaultInstance;
            }
        }

        public Vector Pos
        {
            get
            {
                if (this.pos_ == null)
                {
                }
                return Vector.DefaultInstance;
            }
        }

        public Quaternion Rot
        {
            get
            {
                if (this.rot_ == null)
                {
                }
                return Quaternion.DefaultInstance;
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
                    if (this.hasPos)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeMessageSize(1, this.Pos);
                    }
                    if (this.hasOldPos)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeMessageSize(2, this.OldPos);
                    }
                    if (this.hasRot)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeMessageSize(3, this.Rot);
                    }
                    if (this.hasOldRot)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeMessageSize(4, this.OldRot);
                    }
                    memoizedSerializedSize += this.get_UnknownFields().get_SerializedSize();
                    this.memoizedSerializedSize = memoizedSerializedSize;
                }
                return memoizedSerializedSize;
            }
        }

        protected override objectCoords ThisMessage
        {
            get
            {
                return this;
            }
        }

        [DebuggerNonUserCode]
        public sealed class Builder : GeneratedBuilder<objectCoords, objectCoords.Builder>
        {
            private objectCoords result;
            private bool resultIsReadOnly;

            public Builder()
            {
                this.result = objectCoords.DefaultInstance;
                this.resultIsReadOnly = true;
            }

            internal Builder(objectCoords cloneFrom)
            {
                this.result = cloneFrom;
                this.resultIsReadOnly = true;
            }

            public override objectCoords BuildPartial()
            {
                if (this.resultIsReadOnly)
                {
                    return this.result;
                }
                this.resultIsReadOnly = true;
                return this.result.MakeReadOnly();
            }

            public override objectCoords.Builder Clear()
            {
                this.result = objectCoords.DefaultInstance;
                this.resultIsReadOnly = true;
                return this;
            }

            public objectCoords.Builder ClearOldPos()
            {
                this.PrepareBuilder();
                this.result.hasOldPos = false;
                this.result.oldPos_ = null;
                return this;
            }

            public objectCoords.Builder ClearOldRot()
            {
                this.PrepareBuilder();
                this.result.hasOldRot = false;
                this.result.oldRot_ = null;
                return this;
            }

            public objectCoords.Builder ClearPos()
            {
                this.PrepareBuilder();
                this.result.hasPos = false;
                this.result.pos_ = null;
                return this;
            }

            public objectCoords.Builder ClearRot()
            {
                this.PrepareBuilder();
                this.result.hasRot = false;
                this.result.rot_ = null;
                return this;
            }

            public override objectCoords.Builder Clone()
            {
                if (this.resultIsReadOnly)
                {
                    return new objectCoords.Builder(this.result);
                }
                return new objectCoords.Builder().MergeFrom(this.result);
            }

            public override objectCoords.Builder MergeFrom(ICodedInputStream input)
            {
                return this.MergeFrom(input, ExtensionRegistry.get_Empty());
            }

            public override objectCoords.Builder MergeFrom(IMessage other)
            {
                if (other is objectCoords)
                {
                    return this.MergeFrom((objectCoords) other);
                }
                base.MergeFrom(other);
                return this;
            }

            public override objectCoords.Builder MergeFrom(objectCoords other)
            {
                if (other != objectCoords.DefaultInstance)
                {
                    this.PrepareBuilder();
                    if (other.HasPos)
                    {
                        this.MergePos(other.Pos);
                    }
                    if (other.HasOldPos)
                    {
                        this.MergeOldPos(other.OldPos);
                    }
                    if (other.HasRot)
                    {
                        this.MergeRot(other.Rot);
                    }
                    if (other.HasOldRot)
                    {
                        this.MergeOldRot(other.OldRot);
                    }
                    this.MergeUnknownFields(other.get_UnknownFields());
                }
                return this;
            }

            public override objectCoords.Builder MergeFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
            {
                uint num;
                string str;
                this.PrepareBuilder();
                UnknownFieldSet.Builder builder = null;
                while (input.ReadTag(ref num, ref str))
                {
                    if ((num == 0) && (str != null))
                    {
                        int index = Array.BinarySearch<string>(objectCoords._objectCoordsFieldNames, str, StringComparer.Ordinal);
                        if (index >= 0)
                        {
                            num = objectCoords._objectCoordsFieldTags[index];
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
                            Vector.Builder builder2 = Vector.CreateBuilder();
                            if (this.result.hasPos)
                            {
                                builder2.MergeFrom(this.Pos);
                            }
                            input.ReadMessage(builder2, extensionRegistry);
                            this.Pos = builder2.BuildPartial();
                            continue;
                        }
                        case 0x12:
                        {
                            Vector.Builder builder3 = Vector.CreateBuilder();
                            if (this.result.hasOldPos)
                            {
                                builder3.MergeFrom(this.OldPos);
                            }
                            input.ReadMessage(builder3, extensionRegistry);
                            this.OldPos = builder3.BuildPartial();
                            continue;
                        }
                        case 0x1a:
                        {
                            Quaternion.Builder builder4 = Quaternion.CreateBuilder();
                            if (this.result.hasRot)
                            {
                                builder4.MergeFrom(this.Rot);
                            }
                            input.ReadMessage(builder4, extensionRegistry);
                            this.Rot = builder4.BuildPartial();
                            continue;
                        }
                        case 0x22:
                        {
                            Quaternion.Builder builder5 = Quaternion.CreateBuilder();
                            if (this.result.hasOldRot)
                            {
                                builder5.MergeFrom(this.OldRot);
                            }
                            input.ReadMessage(builder5, extensionRegistry);
                            this.OldRot = builder5.BuildPartial();
                            continue;
                        }
                    }
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
                }
                if (builder != null)
                {
                    this.set_UnknownFields(builder.Build());
                }
                return this;
            }

            public objectCoords.Builder MergeOldPos(Vector value)
            {
                ThrowHelper.ThrowIfNull(value, "value");
                this.PrepareBuilder();
                if (this.result.hasOldPos && (this.result.oldPos_ != Vector.DefaultInstance))
                {
                    this.result.oldPos_ = Vector.CreateBuilder(this.result.oldPos_).MergeFrom(value).BuildPartial();
                }
                else
                {
                    this.result.oldPos_ = value;
                }
                this.result.hasOldPos = true;
                return this;
            }

            public objectCoords.Builder MergeOldRot(Quaternion value)
            {
                ThrowHelper.ThrowIfNull(value, "value");
                this.PrepareBuilder();
                if (this.result.hasOldRot && (this.result.oldRot_ != Quaternion.DefaultInstance))
                {
                    this.result.oldRot_ = Quaternion.CreateBuilder(this.result.oldRot_).MergeFrom(value).BuildPartial();
                }
                else
                {
                    this.result.oldRot_ = value;
                }
                this.result.hasOldRot = true;
                return this;
            }

            public objectCoords.Builder MergePos(Vector value)
            {
                ThrowHelper.ThrowIfNull(value, "value");
                this.PrepareBuilder();
                if (this.result.hasPos && (this.result.pos_ != Vector.DefaultInstance))
                {
                    this.result.pos_ = Vector.CreateBuilder(this.result.pos_).MergeFrom(value).BuildPartial();
                }
                else
                {
                    this.result.pos_ = value;
                }
                this.result.hasPos = true;
                return this;
            }

            public objectCoords.Builder MergeRot(Quaternion value)
            {
                ThrowHelper.ThrowIfNull(value, "value");
                this.PrepareBuilder();
                if (this.result.hasRot && (this.result.rot_ != Quaternion.DefaultInstance))
                {
                    this.result.rot_ = Quaternion.CreateBuilder(this.result.rot_).MergeFrom(value).BuildPartial();
                }
                else
                {
                    this.result.rot_ = value;
                }
                this.result.hasRot = true;
                return this;
            }

            private objectCoords PrepareBuilder()
            {
                if (this.resultIsReadOnly)
                {
                    objectCoords result = this.result;
                    this.result = new objectCoords();
                    this.resultIsReadOnly = false;
                    this.MergeFrom(result);
                }
                return this.result;
            }

            public objectCoords.Builder SetOldPos(Vector value)
            {
                ThrowHelper.ThrowIfNull(value, "value");
                this.PrepareBuilder();
                this.result.hasOldPos = true;
                this.result.oldPos_ = value;
                return this;
            }

            public objectCoords.Builder SetOldPos(Vector.Builder builderForValue)
            {
                ThrowHelper.ThrowIfNull(builderForValue, "builderForValue");
                this.PrepareBuilder();
                this.result.hasOldPos = true;
                this.result.oldPos_ = builderForValue.Build();
                return this;
            }

            public objectCoords.Builder SetOldRot(Quaternion value)
            {
                ThrowHelper.ThrowIfNull(value, "value");
                this.PrepareBuilder();
                this.result.hasOldRot = true;
                this.result.oldRot_ = value;
                return this;
            }

            public objectCoords.Builder SetOldRot(Quaternion.Builder builderForValue)
            {
                ThrowHelper.ThrowIfNull(builderForValue, "builderForValue");
                this.PrepareBuilder();
                this.result.hasOldRot = true;
                this.result.oldRot_ = builderForValue.Build();
                return this;
            }

            public objectCoords.Builder SetPos(Vector value)
            {
                ThrowHelper.ThrowIfNull(value, "value");
                this.PrepareBuilder();
                this.result.hasPos = true;
                this.result.pos_ = value;
                return this;
            }

            public objectCoords.Builder SetPos(Vector.Builder builderForValue)
            {
                ThrowHelper.ThrowIfNull(builderForValue, "builderForValue");
                this.PrepareBuilder();
                this.result.hasPos = true;
                this.result.pos_ = builderForValue.Build();
                return this;
            }

            public objectCoords.Builder SetRot(Quaternion value)
            {
                ThrowHelper.ThrowIfNull(value, "value");
                this.PrepareBuilder();
                this.result.hasRot = true;
                this.result.rot_ = value;
                return this;
            }

            public objectCoords.Builder SetRot(Quaternion.Builder builderForValue)
            {
                ThrowHelper.ThrowIfNull(builderForValue, "builderForValue");
                this.PrepareBuilder();
                this.result.hasRot = true;
                this.result.rot_ = builderForValue.Build();
                return this;
            }

            public override objectCoords DefaultInstanceForType
            {
                get
                {
                    return objectCoords.DefaultInstance;
                }
            }

            public override MessageDescriptor DescriptorForType
            {
                get
                {
                    return objectCoords.Descriptor;
                }
            }

            public bool HasOldPos
            {
                get
                {
                    return this.result.hasOldPos;
                }
            }

            public bool HasOldRot
            {
                get
                {
                    return this.result.hasOldRot;
                }
            }

            public bool HasPos
            {
                get
                {
                    return this.result.hasPos;
                }
            }

            public bool HasRot
            {
                get
                {
                    return this.result.hasRot;
                }
            }

            public override bool IsInitialized
            {
                get
                {
                    return this.result.IsInitialized;
                }
            }

            protected override objectCoords MessageBeingBuilt
            {
                get
                {
                    return this.PrepareBuilder();
                }
            }

            public Vector OldPos
            {
                get
                {
                    return this.result.OldPos;
                }
                set
                {
                    this.SetOldPos(value);
                }
            }

            public Quaternion OldRot
            {
                get
                {
                    return this.result.OldRot;
                }
                set
                {
                    this.SetOldRot(value);
                }
            }

            public Vector Pos
            {
                get
                {
                    return this.result.Pos;
                }
                set
                {
                    this.SetPos(value);
                }
            }

            public Quaternion Rot
            {
                get
                {
                    return this.result.Rot;
                }
                set
                {
                    this.SetRot(value);
                }
            }

            protected override objectCoords.Builder ThisBuilder
            {
                get
                {
                    return this;
                }
            }
        }
    }
}

