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
    public sealed class User : GeneratedMessage<User, User.Builder>
    {
        private static readonly string[] _userFieldNames = new string[] { "displayname", "usergroup", "userid" };
        private static readonly uint[] _userFieldTags = new uint[] { 0x12, 0x18, 8 };
        private static readonly User defaultInstance = new User().MakeReadOnly();
        private string displayname_ = string.Empty;
        public const int DisplaynameFieldNumber = 2;
        private bool hasDisplayname;
        private bool hasUsergroup;
        private bool hasUserid;
        private int memoizedSerializedSize = -1;
        private Types.UserGroup usergroup_;
        public const int UsergroupFieldNumber = 3;
        private ulong userid_;
        public const int UseridFieldNumber = 1;

        static User()
        {
            object.ReferenceEquals(User.Descriptor, null);
        }

        private User()
        {
        }

        public static Builder CreateBuilder()
        {
            return new Builder();
        }

        public static Builder CreateBuilder(User prototype)
        {
            return new Builder(prototype);
        }

        public override Builder CreateBuilderForType()
        {
            return new Builder();
        }

        private User MakeReadOnly()
        {
            return this;
        }

        public static User ParseDelimitedFrom(Stream input)
        {
            return CreateBuilder().MergeDelimitedFrom(input).BuildParsed();
        }

        public static User ParseDelimitedFrom(Stream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeDelimitedFrom(input, extensionRegistry).BuildParsed();
        }

        public static User ParseFrom(ByteString data)
        {
            return CreateBuilder().MergeFrom(data).BuildParsed();
        }

        public static User ParseFrom(byte[] data)
        {
            return CreateBuilder().MergeFrom(data).BuildParsed();
        }

        public static User ParseFrom(ICodedInputStream input)
        {
            return CreateBuilder().MergeFrom(input).BuildParsed();
        }

        public static User ParseFrom(Stream input)
        {
            return CreateBuilder().MergeFrom(input).BuildParsed();
        }

        public static User ParseFrom(ByteString data, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
        }

        public static User ParseFrom(byte[] data, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
        }

        public static User ParseFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
        }

        public static User ParseFrom(Stream input, ExtensionRegistry extensionRegistry)
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
            string[] strArray = _userFieldNames;
            if (this.hasUserid)
            {
                output.WriteUInt64(1, strArray[2], this.Userid);
            }
            if (this.hasDisplayname)
            {
                output.WriteString(2, strArray[0], this.Displayname);
            }
            if (this.hasUsergroup)
            {
                output.WriteEnum(3, strArray[1], (int) this.Usergroup, this.Usergroup);
            }
            this.get_UnknownFields().WriteTo(output);
        }

        public static User DefaultInstance
        {
            get
            {
                return defaultInstance;
            }
        }

        public override User DefaultInstanceForType
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
                return User.internal__static_RustProto_User__Descriptor;
            }
        }

        public string Displayname
        {
            get
            {
                return this.displayname_;
            }
        }

        public bool HasDisplayname
        {
            get
            {
                return this.hasDisplayname;
            }
        }

        public bool HasUsergroup
        {
            get
            {
                return this.hasUsergroup;
            }
        }

        public bool HasUserid
        {
            get
            {
                return this.hasUserid;
            }
        }

        protected override FieldAccessorTable<User, Builder> InternalFieldAccessors
        {
            get
            {
                return User.internal__static_RustProto_User__FieldAccessorTable;
            }
        }

        public override bool IsInitialized
        {
            get
            {
                if (!this.hasUserid)
                {
                    return false;
                }
                if (!this.hasDisplayname)
                {
                    return false;
                }
                if (!this.hasUsergroup)
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
                    if (this.hasUserid)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeUInt64Size(1, this.Userid);
                    }
                    if (this.hasDisplayname)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeStringSize(2, this.Displayname);
                    }
                    if (this.hasUsergroup)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeEnumSize(3, (int) this.Usergroup);
                    }
                    memoizedSerializedSize += this.get_UnknownFields().get_SerializedSize();
                    this.memoizedSerializedSize = memoizedSerializedSize;
                }
                return memoizedSerializedSize;
            }
        }

        protected override User ThisMessage
        {
            get
            {
                return this;
            }
        }

        public Types.UserGroup Usergroup
        {
            get
            {
                return this.usergroup_;
            }
        }

        [CLSCompliant(false)]
        public ulong Userid
        {
            get
            {
                return this.userid_;
            }
        }

        [DebuggerNonUserCode]
        public sealed class Builder : GeneratedBuilder<User, User.Builder>
        {
            private User result;
            private bool resultIsReadOnly;

            public Builder()
            {
                this.result = User.DefaultInstance;
                this.resultIsReadOnly = true;
            }

            internal Builder(User cloneFrom)
            {
                this.result = cloneFrom;
                this.resultIsReadOnly = true;
            }

            public override User BuildPartial()
            {
                if (this.resultIsReadOnly)
                {
                    return this.result;
                }
                this.resultIsReadOnly = true;
                return this.result.MakeReadOnly();
            }

            public override User.Builder Clear()
            {
                this.result = User.DefaultInstance;
                this.resultIsReadOnly = true;
                return this;
            }

            public User.Builder ClearDisplayname()
            {
                this.PrepareBuilder();
                this.result.hasDisplayname = false;
                this.result.displayname_ = string.Empty;
                return this;
            }

            public User.Builder ClearUsergroup()
            {
                this.PrepareBuilder();
                this.result.hasUsergroup = false;
                this.result.usergroup_ = User.Types.UserGroup.REGULAR;
                return this;
            }

            public User.Builder ClearUserid()
            {
                this.PrepareBuilder();
                this.result.hasUserid = false;
                this.result.userid_ = 0L;
                return this;
            }

            public override User.Builder Clone()
            {
                if (this.resultIsReadOnly)
                {
                    return new User.Builder(this.result);
                }
                return new User.Builder().MergeFrom(this.result);
            }

            public override User.Builder MergeFrom(ICodedInputStream input)
            {
                return this.MergeFrom(input, ExtensionRegistry.get_Empty());
            }

            public override User.Builder MergeFrom(IMessage other)
            {
                if (other is User)
                {
                    return this.MergeFrom((User) other);
                }
                base.MergeFrom(other);
                return this;
            }

            public override User.Builder MergeFrom(User other)
            {
                if (other != User.DefaultInstance)
                {
                    this.PrepareBuilder();
                    if (other.HasUserid)
                    {
                        this.Userid = other.Userid;
                    }
                    if (other.HasDisplayname)
                    {
                        this.Displayname = other.Displayname;
                    }
                    if (other.HasUsergroup)
                    {
                        this.Usergroup = other.Usergroup;
                    }
                    this.MergeUnknownFields(other.get_UnknownFields());
                }
                return this;
            }

            public override User.Builder MergeFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
            {
                uint num;
                string str;
                this.PrepareBuilder();
                UnknownFieldSet.Builder builder = null;
                while (input.ReadTag(ref num, ref str))
                {
                    object obj2;
                    if ((num == 0) && (str != null))
                    {
                        int index = Array.BinarySearch<string>(User._userFieldNames, str, StringComparer.Ordinal);
                        if (index >= 0)
                        {
                            num = User._userFieldTags[index];
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
                            this.result.hasUserid = input.ReadUInt64(ref this.result.userid_);
                            continue;
                        }
                        case 0x12:
                        {
                            this.result.hasDisplayname = input.ReadString(ref this.result.displayname_);
                            continue;
                        }
                        case 0x18:
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
                    if (input.ReadEnum<User.Types.UserGroup>(ref this.result.usergroup_, ref obj2))
                    {
                        this.result.hasUsergroup = true;
                    }
                    else
                    {
                        if (!(obj2 is int))
                        {
                            continue;
                        }
                        if (builder == null)
                        {
                            builder = UnknownFieldSet.CreateBuilder(this.get_UnknownFields());
                        }
                        builder.MergeVarintField(3, (ulong) ((int) obj2));
                    }
                }
                if (builder != null)
                {
                    this.set_UnknownFields(builder.Build());
                }
                return this;
            }

            private User PrepareBuilder()
            {
                if (this.resultIsReadOnly)
                {
                    User result = this.result;
                    this.result = new User();
                    this.resultIsReadOnly = false;
                    this.MergeFrom(result);
                }
                return this.result;
            }

            public User.Builder SetDisplayname(string value)
            {
                ThrowHelper.ThrowIfNull(value, "value");
                this.PrepareBuilder();
                this.result.hasDisplayname = true;
                this.result.displayname_ = value;
                return this;
            }

            public User.Builder SetUsergroup(User.Types.UserGroup value)
            {
                this.PrepareBuilder();
                this.result.hasUsergroup = true;
                this.result.usergroup_ = value;
                return this;
            }

            [CLSCompliant(false)]
            public User.Builder SetUserid(ulong value)
            {
                this.PrepareBuilder();
                this.result.hasUserid = true;
                this.result.userid_ = value;
                return this;
            }

            public override User DefaultInstanceForType
            {
                get
                {
                    return User.DefaultInstance;
                }
            }

            public override MessageDescriptor DescriptorForType
            {
                get
                {
                    return User.Descriptor;
                }
            }

            public string Displayname
            {
                get
                {
                    return this.result.Displayname;
                }
                set
                {
                    this.SetDisplayname(value);
                }
            }

            public bool HasDisplayname
            {
                get
                {
                    return this.result.hasDisplayname;
                }
            }

            public bool HasUsergroup
            {
                get
                {
                    return this.result.hasUsergroup;
                }
            }

            public bool HasUserid
            {
                get
                {
                    return this.result.hasUserid;
                }
            }

            public override bool IsInitialized
            {
                get
                {
                    return this.result.IsInitialized;
                }
            }

            protected override User MessageBeingBuilt
            {
                get
                {
                    return this.PrepareBuilder();
                }
            }

            protected override User.Builder ThisBuilder
            {
                get
                {
                    return this;
                }
            }

            public User.Types.UserGroup Usergroup
            {
                get
                {
                    return this.result.Usergroup;
                }
                set
                {
                    this.SetUsergroup(value);
                }
            }

            [CLSCompliant(false)]
            public ulong Userid
            {
                get
                {
                    return this.result.Userid;
                }
                set
                {
                    this.SetUserid(value);
                }
            }
        }

        [DebuggerNonUserCode]
        public static class Types
        {
            public enum UserGroup
            {
                REGULAR,
                BANNED,
                ADMIN
            }
        }
    }
}

