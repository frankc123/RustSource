namespace RustProto.Proto
{
    using Google.ProtocolBuffers;
    using Google.ProtocolBuffers.Descriptors;
    using Google.ProtocolBuffers.FieldAccess;
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    [DebuggerNonUserCode]
    public static class Vitals
    {
        [CompilerGenerated]
        private static FileDescriptor.InternalDescriptorAssigner <>f__am$cache3;
        private static FileDescriptor descriptor;
        internal static MessageDescriptor internal__static_RustProto_Vitals__Descriptor;
        internal static FieldAccessorTable<Vitals, Vitals.Builder> internal__static_RustProto_Vitals__FieldAccessorTable;

        static Vitals()
        {
            byte[] buffer = Convert.FromBase64String("ChFydXN0L3ZpdGFscy5wcm90bxIJUnVzdFByb3RvIu8BCgZWaXRhbHMSEwoGaGVhbHRoGAEgASgCOgMxMDASFQoJaHlkcmF0aW9uGAIgASgCOgIzMBIWCghjYWxvcmllcxgDIAEoAjoEMTAwMBIUCglyYWRpYXRpb24YBCABKAI6ATASGQoOcmFkaWF0aW9uX2FudGkYBSABKAI6ATASFgoLYmxlZWRfc3BlZWQYBiABKAI6ATASFAoJYmxlZWRfbWF4GAcgASgCOgEwEhUKCmhlYWxfc3BlZWQYCCABKAI6ATASEwoIaGVhbF9tYXgYCSABKAI6ATASFgoLdGVtcGVyYXR1cmUYCiABKAI6ATBCAkgB");
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = new FileDescriptor.InternalDescriptorAssigner(null, <Vitals>m__B);
            }
            FileDescriptor.InternalDescriptorAssigner assigner = <>f__am$cache3;
            FileDescriptor.InternalBuildGeneratedFileFrom(buffer, new FileDescriptor[0], assigner);
        }

        [CompilerGenerated]
        private static ExtensionRegistry <Vitals>m__B(FileDescriptor root)
        {
            descriptor = root;
            internal__static_RustProto_Vitals__Descriptor = Descriptor.get_MessageTypes()[0];
            string[] textArray1 = new string[] { "Health", "Hydration", "Calories", "Radiation", "RadiationAnti", "BleedSpeed", "BleedMax", "HealSpeed", "HealMax", "Temperature" };
            internal__static_RustProto_Vitals__FieldAccessorTable = new FieldAccessorTable<Vitals, Vitals.Builder>(internal__static_RustProto_Vitals__Descriptor, textArray1);
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

