namespace RustProto.Proto
{
    using Google.ProtocolBuffers;
    using Google.ProtocolBuffers.Descriptors;
    using Google.ProtocolBuffers.FieldAccess;
    using RustProto;
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    [DebuggerNonUserCode]
    public static class Avatar
    {
        [CompilerGenerated]
        private static FileDescriptor.InternalDescriptorAssigner <>f__am$cache5;
        private static FileDescriptor descriptor;
        internal static MessageDescriptor internal__static_RustProto_Avatar__Descriptor;
        internal static FieldAccessorTable<Avatar, Avatar.Builder> internal__static_RustProto_Avatar__FieldAccessorTable;
        internal static MessageDescriptor internal__static_RustProto_AwayEvent__Descriptor;
        internal static FieldAccessorTable<AwayEvent, AwayEvent.Builder> internal__static_RustProto_AwayEvent__FieldAccessorTable;

        static Avatar()
        {
            byte[] buffer = Convert.FromBase64String("ChFydXN0L2F2YXRhci5wcm90bxIJUnVzdFByb3RvGhRydXN0L2JsdWVwcmludC5wcm90bxoPcnVzdC9pdGVtLnByb3RvGhFydXN0L2NvbW1vbi5wcm90bxoRcnVzdC92aXRhbHMucHJvdG8iqAIKBkF2YXRhchIeCgNwb3MYASABKAsyES5SdXN0UHJvdG8uVmVjdG9yEiIKA2FuZxgCIAEoCzIVLlJ1c3RQcm90by5RdWF0ZXJuaW9uEiEKBnZpdGFscxgDIAEoCzIRLlJ1c3RQcm90by5WaXRhbHMSKAoKYmx1ZXByaW50cxgEIAMoCzIULlJ1c3RQcm90by5CbHVlcHJpbnQSIgoJaW52ZW50b3J5GAUgAygLMg8uUnVzdFByb3RvLkl0ZW0SIQoId2VhcmFibGUYBiADKAsyDy5SdXN0UHJvdG8uSXRlbRIdCgRiZWx0GAcgAygLMg8uUnVzdFByb3RvLkl0ZW0SJwoJYXdheUV2ZW50GAggASgLMhQuUnVzdFByb3RvLkF3YXlFdmVudCKZAQoJQXdheUV2ZW50EjAKBHR5cGUYASACKA4yIi5SdXN0UHJvdG8uQXdheUV2ZW50LkF3YXlFdmVudFR5cGUSEQoJdGltZXN0YW1wGAIgAigFEhIKCmluc3RpZ2F0b3IYAyABKAQiMwoNQXdheUV2ZW50VHlwZRILCgdVTktOT1dOEAASCwoHU0xVTUJFUhABEggKBERJRUQQAkICSAE=");
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = new FileDescriptor.InternalDescriptorAssigner(null, <Avatar>m__4);
            }
            FileDescriptor.InternalDescriptorAssigner assigner = <>f__am$cache5;
            FileDescriptor[] descriptorArray1 = new FileDescriptor[] { Blueprint.Descriptor, Item.Descriptor, Common.Descriptor, Vitals.Descriptor };
            FileDescriptor.InternalBuildGeneratedFileFrom(buffer, descriptorArray1, assigner);
        }

        [CompilerGenerated]
        private static ExtensionRegistry <Avatar>m__4(FileDescriptor root)
        {
            descriptor = root;
            internal__static_RustProto_Avatar__Descriptor = Descriptor.get_MessageTypes()[0];
            string[] textArray1 = new string[] { "Pos", "Ang", "Vitals", "Blueprints", "Inventory", "Wearable", "Belt", "AwayEvent" };
            internal__static_RustProto_Avatar__FieldAccessorTable = new FieldAccessorTable<Avatar, Avatar.Builder>(internal__static_RustProto_Avatar__Descriptor, textArray1);
            internal__static_RustProto_AwayEvent__Descriptor = Descriptor.get_MessageTypes()[1];
            string[] textArray2 = new string[] { "Type", "Timestamp", "Instigator" };
            internal__static_RustProto_AwayEvent__FieldAccessorTable = new FieldAccessorTable<AwayEvent, AwayEvent.Builder>(internal__static_RustProto_AwayEvent__Descriptor, textArray2);
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

