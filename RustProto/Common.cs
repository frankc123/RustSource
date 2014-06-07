namespace RustProto
{
    using Google.ProtocolBuffers;
    using Google.ProtocolBuffers.Descriptors;
    using Google.ProtocolBuffers.FieldAccess;
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    [DebuggerNonUserCode]
    public static class Common
    {
        [CompilerGenerated]
        private static FileDescriptor.InternalDescriptorAssigner <>f__am$cache5;
        private static FileDescriptor descriptor;
        internal static MessageDescriptor internal__static_RustProto_Quaternion__Descriptor;
        internal static FieldAccessorTable<Quaternion, Quaternion.Builder> internal__static_RustProto_Quaternion__FieldAccessorTable;
        internal static MessageDescriptor internal__static_RustProto_Vector__Descriptor;
        internal static FieldAccessorTable<Vector, Vector.Builder> internal__static_RustProto_Vector__FieldAccessorTable;

        static Common()
        {
            byte[] buffer = Convert.FromBase64String("ChFydXN0L2NvbW1vbi5wcm90bxIJUnVzdFByb3RvIjIKBlZlY3RvchIMCgF4GAEgASgCOgEwEgwKAXkYAiABKAI6ATASDAoBehgDIAEoAjoBMCJECgpRdWF0ZXJuaW9uEgwKAXgYASABKAI6ATASDAoBeRgCIAEoAjoBMBIMCgF6GAMgASgCOgEwEgwKAXcYBCABKAI6ATBCAkgB");
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = new FileDescriptor.InternalDescriptorAssigner(null, <Common>m__6);
            }
            FileDescriptor.InternalDescriptorAssigner assigner = <>f__am$cache5;
            FileDescriptor.InternalBuildGeneratedFileFrom(buffer, new FileDescriptor[0], assigner);
        }

        [CompilerGenerated]
        private static ExtensionRegistry <Common>m__6(FileDescriptor root)
        {
            descriptor = root;
            internal__static_RustProto_Vector__Descriptor = Descriptor.get_MessageTypes()[0];
            string[] textArray1 = new string[] { "X", "Y", "Z" };
            internal__static_RustProto_Vector__FieldAccessorTable = new FieldAccessorTable<Vector, Vector.Builder>(internal__static_RustProto_Vector__Descriptor, textArray1);
            internal__static_RustProto_Quaternion__Descriptor = Descriptor.get_MessageTypes()[1];
            string[] textArray2 = new string[] { "X", "Y", "Z", "W" };
            internal__static_RustProto_Quaternion__FieldAccessorTable = new FieldAccessorTable<Quaternion, Quaternion.Builder>(internal__static_RustProto_Quaternion__Descriptor, textArray2);
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

