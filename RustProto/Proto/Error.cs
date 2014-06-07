namespace RustProto.Proto
{
    using Google.ProtocolBuffers;
    using Google.ProtocolBuffers.Descriptors;
    using Google.ProtocolBuffers.FieldAccess;
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    [DebuggerNonUserCode]
    public static class Error
    {
        [CompilerGenerated]
        private static FileDescriptor.InternalDescriptorAssigner <>f__am$cache5;
        private static FileDescriptor descriptor;
        internal static MessageDescriptor internal__static_RustProto_Error__Descriptor;
        internal static FieldAccessorTable<Error, Error.Builder> internal__static_RustProto_Error__FieldAccessorTable;
        internal static MessageDescriptor internal__static_RustProto_GameError__Descriptor;
        internal static FieldAccessorTable<GameError, GameError.Builder> internal__static_RustProto_GameError__FieldAccessorTable;

        static Error()
        {
            byte[] buffer = Convert.FromBase64String("ChBydXN0L2Vycm9yLnByb3RvEglSdXN0UHJvdG8iKAoFRXJyb3ISDgoGc3RhdHVzGAEgAigJEg8KB21lc3NhZ2UYAiACKAkiKQoJR2FtZUVycm9yEg0KBWVycm9yGAEgAigJEg0KBXRyYWNlGAIgAigJQgJIAQ==");
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = new FileDescriptor.InternalDescriptorAssigner(null, <Error>m__7);
            }
            FileDescriptor.InternalDescriptorAssigner assigner = <>f__am$cache5;
            FileDescriptor.InternalBuildGeneratedFileFrom(buffer, new FileDescriptor[0], assigner);
        }

        [CompilerGenerated]
        private static ExtensionRegistry <Error>m__7(FileDescriptor root)
        {
            descriptor = root;
            internal__static_RustProto_Error__Descriptor = Descriptor.get_MessageTypes()[0];
            string[] textArray1 = new string[] { "Status", "Message" };
            internal__static_RustProto_Error__FieldAccessorTable = new FieldAccessorTable<Error, Error.Builder>(internal__static_RustProto_Error__Descriptor, textArray1);
            internal__static_RustProto_GameError__Descriptor = Descriptor.get_MessageTypes()[1];
            string[] textArray2 = new string[] { "Error", "Trace" };
            internal__static_RustProto_GameError__FieldAccessorTable = new FieldAccessorTable<GameError, GameError.Builder>(internal__static_RustProto_GameError__Descriptor, textArray2);
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

