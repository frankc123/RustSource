namespace RustProto.Proto
{
    using Google.ProtocolBuffers;
    using Google.ProtocolBuffers.Descriptors;
    using Google.ProtocolBuffers.FieldAccess;
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    [DebuggerNonUserCode]
    public static class User
    {
        [CompilerGenerated]
        private static FileDescriptor.InternalDescriptorAssigner <>f__am$cache3;
        private static FileDescriptor descriptor;
        internal static MessageDescriptor internal__static_RustProto_User__Descriptor;
        internal static FieldAccessorTable<User, User.Builder> internal__static_RustProto_User__FieldAccessorTable;

        static User()
        {
            byte[] buffer = Convert.FromBase64String("Cg9ydXN0L3VzZXIucHJvdG8SCVJ1c3RQcm90byKKAQoEVXNlchIOCgZ1c2VyaWQYASACKAQSEwoLZGlzcGxheW5hbWUYAiACKAkSLAoJdXNlcmdyb3VwGAMgAigOMhkuUnVzdFByb3RvLlVzZXIuVXNlckdyb3VwIi8KCVVzZXJHcm91cBILCgdSRUdVTEFSEAASCgoGQkFOTkVEEAESCQoFQURNSU4QAkICSAE=");
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = new FileDescriptor.InternalDescriptorAssigner(null, <User>m__A);
            }
            FileDescriptor.InternalDescriptorAssigner assigner = <>f__am$cache3;
            FileDescriptor.InternalBuildGeneratedFileFrom(buffer, new FileDescriptor[0], assigner);
        }

        [CompilerGenerated]
        private static ExtensionRegistry <User>m__A(FileDescriptor root)
        {
            descriptor = root;
            internal__static_RustProto_User__Descriptor = Descriptor.get_MessageTypes()[0];
            string[] textArray1 = new string[] { "Userid", "Displayname", "Usergroup" };
            internal__static_RustProto_User__FieldAccessorTable = new FieldAccessorTable<User, User.Builder>(internal__static_RustProto_User__Descriptor, textArray1);
            return null;
        }

        public static void RegisterAllExtensions(ExtensionRegistry registry)
        {
        }

        public static FileDescriptor Descriptor
        {
            get
            {
                return descriptor;
            }
        }
    }
}

