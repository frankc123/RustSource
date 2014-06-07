namespace RustProto
{
    using Google.ProtocolBuffers;
    using Google.ProtocolBuffers.Collections;
    using Google.ProtocolBuffers.Descriptors;
    using Google.ProtocolBuffers.FieldAccess;
    using RustProto.Helpers;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;

    [DebuggerNonUserCode]
    public sealed class objectLockable : GeneratedMessage<objectLockable, objectLockable.Builder>
    {
        private static readonly string[] _objectLockableFieldNames = new string[] { "locked", "password", "users" };
        private static readonly uint[] _objectLockableFieldTags = new uint[] { 0x10, 10, 0x18 };
        private static readonly objectLockable defaultInstance = new objectLockable().MakeReadOnly();
        private bool hasLocked;
        private bool hasPassword;
        private bool locked_;
        public const int LockedFieldNumber = 2;
        private int memoizedSerializedSize = -1;
        private string password_ = string.Empty;
        public const int PasswordFieldNumber = 1;
        private PopsicleList<ulong> users_ = new PopsicleList<ulong>();
        public const int UsersFieldNumber = 3;

        static objectLockable()
        {
            object.ReferenceEquals(Worldsave.Descriptor, null);
        }

        private objectLockable()
        {
        }

        public static Builder CreateBuilder()
        {
            return new Builder();
        }

        public static Builder CreateBuilder(objectLockable prototype)
        {
            return new Builder(prototype);
        }

        public override Builder CreateBuilderForType()
        {
            return new Builder();
        }

        [CLSCompliant(false)]
        public ulong GetUsers(int index)
        {
            return this.users_.get_Item(index);
        }

        private objectLockable MakeReadOnly()
        {
            this.users_.MakeReadOnly();
            return this;
        }

        public static objectLockable ParseDelimitedFrom(Stream input)
        {
            return CreateBuilder().MergeDelimitedFrom(input).BuildParsed();
        }

        public static objectLockable ParseDelimitedFrom(Stream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeDelimitedFrom(input, extensionRegistry).BuildParsed();
        }

        public static objectLockable ParseFrom(ByteString data)
        {
            return CreateBuilder().MergeFrom(data).BuildParsed();
        }

        public static objectLockable ParseFrom(byte[] data)
        {
            return CreateBuilder().MergeFrom(data).BuildParsed();
        }

        public static objectLockable ParseFrom(ICodedInputStream input)
        {
            return CreateBuilder().MergeFrom(input).BuildParsed();
        }

        public static objectLockable ParseFrom(Stream input)
        {
            return CreateBuilder().MergeFrom(input).BuildParsed();
        }

        public static objectLockable ParseFrom(ByteString data, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
        }

        public static objectLockable ParseFrom(byte[] data, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
        }

        public static objectLockable ParseFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
        }

        public static objectLockable ParseFrom(Stream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
        }

        public static Recycler<objectLockable, Builder> Recycler()
        {
            return Recycler<objectLockable, Builder>.Manufacture();
        }

        public override Builder ToBuilder()
        {
            return CreateBuilder(this);
        }

        public override void WriteTo(ICodedOutputStream output)
        {
            int serializedSize = this.SerializedSize;
            string[] strArray = _objectLockableFieldNames;
            if (this.hasPassword)
            {
                output.WriteString(1, strArray[1], this.Password);
            }
            if (this.hasLocked)
            {
                output.WriteBool(2, strArray[0], this.Locked);
            }
            if (this.users_.get_Count() > 0)
            {
                output.WriteUInt64Array(3, strArray[2], this.users_);
            }
            this.get_UnknownFields().WriteTo(output);
        }

        public static objectLockable DefaultInstance
        {
            get
            {
                return defaultInstance;
            }
        }

        public override objectLockable DefaultInstanceForType
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
                return Worldsave.internal__static_RustProto_objectLockable__Descriptor;
            }
        }

        public bool HasLocked
        {
            get
            {
                return this.hasLocked;
            }
        }

        public bool HasPassword
        {
            get
            {
                return this.hasPassword;
            }
        }

        protected override FieldAccessorTable<objectLockable, Builder> InternalFieldAccessors
        {
            get
            {
                return Worldsave.internal__static_RustProto_objectLockable__FieldAccessorTable;
            }
        }

        public override bool IsInitialized
        {
            get
            {
                return true;
            }
        }

        public bool Locked
        {
            get
            {
                return this.locked_;
            }
        }

        public string Password
        {
            get
            {
                return this.password_;
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
                    if (this.hasPassword)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeStringSize(1, this.Password);
                    }
                    if (this.hasLocked)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeBoolSize(2, this.Locked);
                    }
                    int num2 = 0;
                    IEnumerator<ulong> enumerator = this.UsersList.GetEnumerator();
                    try
                    {
                        while (enumerator.MoveNext())
                        {
                            ulong current = enumerator.Current;
                            num2 += CodedOutputStream.ComputeUInt64SizeNoTag(current);
                        }
                    }
                    finally
                    {
                        if (enumerator == null)
                        {
                        }
                        enumerator.Dispose();
                    }
                    memoizedSerializedSize += num2;
                    memoizedSerializedSize += 1 * this.users_.get_Count();
                    memoizedSerializedSize += this.get_UnknownFields().get_SerializedSize();
                    this.memoizedSerializedSize = memoizedSerializedSize;
                }
                return memoizedSerializedSize;
            }
        }

        protected override objectLockable ThisMessage
        {
            get
            {
                return this;
            }
        }

        public int UsersCount
        {
            get
            {
                return this.users_.get_Count();
            }
        }

        [CLSCompliant(false)]
        public IList<ulong> UsersList
        {
            get
            {
                return Lists.AsReadOnly<ulong>(this.users_);
            }
        }

        [DebuggerNonUserCode]
        public sealed class Builder : GeneratedBuilder<objectLockable, objectLockable.Builder>
        {
            private objectLockable result;
            private bool resultIsReadOnly;

            public Builder()
            {
                this.result = objectLockable.DefaultInstance;
                this.resultIsReadOnly = true;
            }

            internal Builder(objectLockable cloneFrom)
            {
                this.result = cloneFrom;
                this.resultIsReadOnly = true;
            }

            [CLSCompliant(false)]
            public objectLockable.Builder AddRangeUsers(IEnumerable<ulong> values)
            {
                this.PrepareBuilder();
                this.result.users_.Add(values);
                return this;
            }

            [CLSCompliant(false)]
            public objectLockable.Builder AddUsers(ulong value)
            {
                this.PrepareBuilder();
                this.result.users_.Add(value);
                return this;
            }

            public override objectLockable BuildPartial()
            {
                if (this.resultIsReadOnly)
                {
                    return this.result;
                }
                this.resultIsReadOnly = true;
                return this.result.MakeReadOnly();
            }

            public override objectLockable.Builder Clear()
            {
                this.result = objectLockable.DefaultInstance;
                this.resultIsReadOnly = true;
                return this;
            }

            public objectLockable.Builder ClearLocked()
            {
                this.PrepareBuilder();
                this.result.hasLocked = false;
                this.result.locked_ = false;
                return this;
            }

            public objectLockable.Builder ClearPassword()
            {
                this.PrepareBuilder();
                this.result.hasPassword = false;
                this.result.password_ = string.Empty;
                return this;
            }

            public objectLockable.Builder ClearUsers()
            {
                this.PrepareBuilder();
                this.result.users_.Clear();
                return this;
            }

            public override objectLockable.Builder Clone()
            {
                if (this.resultIsReadOnly)
                {
                    return new objectLockable.Builder(this.result);
                }
                return new objectLockable.Builder().MergeFrom(this.result);
            }

            [CLSCompliant(false)]
            public ulong GetUsers(int index)
            {
                return this.result.GetUsers(index);
            }

            public override objectLockable.Builder MergeFrom(ICodedInputStream input)
            {
                return this.MergeFrom(input, ExtensionRegistry.get_Empty());
            }

            public override objectLockable.Builder MergeFrom(IMessage other)
            {
                if (other is objectLockable)
                {
                    return this.MergeFrom((objectLockable) other);
                }
                base.MergeFrom(other);
                return this;
            }

            public override objectLockable.Builder MergeFrom(objectLockable other)
            {
                if (other != objectLockable.DefaultInstance)
                {
                    this.PrepareBuilder();
                    if (other.HasPassword)
                    {
                        this.Password = other.Password;
                    }
                    if (other.HasLocked)
                    {
                        this.Locked = other.Locked;
                    }
                    if (other.users_.get_Count() != 0)
                    {
                        this.result.users_.Add(other.users_);
                    }
                    this.MergeUnknownFields(other.get_UnknownFields());
                }
                return this;
            }

            public override objectLockable.Builder MergeFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
            {
                uint num;
                string str;
                this.PrepareBuilder();
                UnknownFieldSet.Builder builder = null;
                while (input.ReadTag(ref num, ref str))
                {
                    if ((num == 0) && (str != null))
                    {
                        int index = Array.BinarySearch<string>(objectLockable._objectLockableFieldNames, str, StringComparer.Ordinal);
                        if (index >= 0)
                        {
                            num = objectLockable._objectLockableFieldTags[index];
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
                        case 0x18:
                        case 0x1a:
                            goto Label_0124;

                        case 0:
                            throw InvalidProtocolBufferException.InvalidTag();

                        case 10:
                            break;

                        case 0x10:
                            goto Label_0103;

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
                    this.result.hasPassword = input.ReadString(ref this.result.password_);
                    continue;
                Label_0103:
                    this.result.hasLocked = input.ReadBool(ref this.result.locked_);
                    continue;
                Label_0124:
                    input.ReadUInt64Array(num, str, this.result.users_);
                }
                if (builder != null)
                {
                    this.set_UnknownFields(builder.Build());
                }
                return this;
            }

            private objectLockable PrepareBuilder()
            {
                if (this.resultIsReadOnly)
                {
                    objectLockable result = this.result;
                    this.result = new objectLockable();
                    this.resultIsReadOnly = false;
                    this.MergeFrom(result);
                }
                return this.result;
            }

            public objectLockable.Builder SetLocked(bool value)
            {
                this.PrepareBuilder();
                this.result.hasLocked = true;
                this.result.locked_ = value;
                return this;
            }

            public objectLockable.Builder SetPassword(string value)
            {
                ThrowHelper.ThrowIfNull(value, "value");
                this.PrepareBuilder();
                this.result.hasPassword = true;
                this.result.password_ = value;
                return this;
            }

            [CLSCompliant(false)]
            public objectLockable.Builder SetUsers(int index, ulong value)
            {
                this.PrepareBuilder();
                this.result.users_.set_Item(index, value);
                return this;
            }

            public override objectLockable DefaultInstanceForType
            {
                get
                {
                    return objectLockable.DefaultInstance;
                }
            }

            public override MessageDescriptor DescriptorForType
            {
                get
                {
                    return objectLockable.Descriptor;
                }
            }

            public bool HasLocked
            {
                get
                {
                    return this.result.hasLocked;
                }
            }

            public bool HasPassword
            {
                get
                {
                    return this.result.hasPassword;
                }
            }

            public override bool IsInitialized
            {
                get
                {
                    return this.result.IsInitialized;
                }
            }

            public bool Locked
            {
                get
                {
                    return this.result.Locked;
                }
                set
                {
                    this.SetLocked(value);
                }
            }

            protected override objectLockable MessageBeingBuilt
            {
                get
                {
                    return this.PrepareBuilder();
                }
            }

            public string Password
            {
                get
                {
                    return this.result.Password;
                }
                set
                {
                    this.SetPassword(value);
                }
            }

            protected override objectLockable.Builder ThisBuilder
            {
                get
                {
                    return this;
                }
            }

            public int UsersCount
            {
                get
                {
                    return this.result.UsersCount;
                }
            }

            [CLSCompliant(false)]
            public IPopsicleList<ulong> UsersList
            {
                get
                {
                    return this.PrepareBuilder().users_;
                }
            }
        }
    }
}

