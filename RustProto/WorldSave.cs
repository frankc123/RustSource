namespace RustProto
{
    using Google.ProtocolBuffers;
    using Google.ProtocolBuffers.Descriptors;
    using Google.ProtocolBuffers.FieldAccess;
    using RustProto.Proto;
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    [DebuggerNonUserCode]
    public static class Worldsave
    {
        [CompilerGenerated]
        private static FileDescriptor.InternalDescriptorAssigner <>f__am$cache1D;
        private static FileDescriptor descriptor;
        internal static MessageDescriptor internal__static_RustProto_objectCoords__Descriptor;
        internal static FieldAccessorTable<objectCoords, objectCoords.Builder> internal__static_RustProto_objectCoords__FieldAccessorTable;
        internal static MessageDescriptor internal__static_RustProto_objectDeployable__Descriptor;
        internal static FieldAccessorTable<objectDeployable, objectDeployable.Builder> internal__static_RustProto_objectDeployable__FieldAccessorTable;
        internal static MessageDescriptor internal__static_RustProto_objectDoor__Descriptor;
        internal static FieldAccessorTable<objectDoor, objectDoor.Builder> internal__static_RustProto_objectDoor__FieldAccessorTable;
        internal static MessageDescriptor internal__static_RustProto_objectFireBarrel__Descriptor;
        internal static FieldAccessorTable<objectFireBarrel, objectFireBarrel.Builder> internal__static_RustProto_objectFireBarrel__FieldAccessorTable;
        internal static MessageDescriptor internal__static_RustProto_objectICarriableTrans__Descriptor;
        internal static FieldAccessorTable<objectICarriableTrans, objectICarriableTrans.Builder> internal__static_RustProto_objectICarriableTrans__FieldAccessorTable;
        internal static MessageDescriptor internal__static_RustProto_objectLockable__Descriptor;
        internal static FieldAccessorTable<objectLockable, objectLockable.Builder> internal__static_RustProto_objectLockable__FieldAccessorTable;
        internal static MessageDescriptor internal__static_RustProto_objectNetInstance__Descriptor;
        internal static FieldAccessorTable<objectNetInstance, objectNetInstance.Builder> internal__static_RustProto_objectNetInstance__FieldAccessorTable;
        internal static MessageDescriptor internal__static_RustProto_objectNGCInstance__Descriptor;
        internal static FieldAccessorTable<objectNGCInstance, objectNGCInstance.Builder> internal__static_RustProto_objectNGCInstance__FieldAccessorTable;
        internal static MessageDescriptor internal__static_RustProto_objectSleepingAvatar__Descriptor;
        internal static FieldAccessorTable<objectSleepingAvatar, objectSleepingAvatar.Builder> internal__static_RustProto_objectSleepingAvatar__FieldAccessorTable;
        internal static MessageDescriptor internal__static_RustProto_objectStructComponent__Descriptor;
        internal static FieldAccessorTable<objectStructComponent, objectStructComponent.Builder> internal__static_RustProto_objectStructComponent__FieldAccessorTable;
        internal static MessageDescriptor internal__static_RustProto_objectStructMaster__Descriptor;
        internal static FieldAccessorTable<objectStructMaster, objectStructMaster.Builder> internal__static_RustProto_objectStructMaster__FieldAccessorTable;
        internal static MessageDescriptor internal__static_RustProto_objectTakeDamage__Descriptor;
        internal static FieldAccessorTable<objectTakeDamage, objectTakeDamage.Builder> internal__static_RustProto_objectTakeDamage__FieldAccessorTable;
        internal static MessageDescriptor internal__static_RustProto_SavedObject__Descriptor;
        internal static FieldAccessorTable<SavedObject, SavedObject.Builder> internal__static_RustProto_SavedObject__FieldAccessorTable;
        internal static MessageDescriptor internal__static_RustProto_WorldSave__Descriptor;
        internal static FieldAccessorTable<WorldSave, WorldSave.Builder> internal__static_RustProto_WorldSave__FieldAccessorTable;

        static Worldsave()
        {
            byte[] buffer = Convert.FromBase64String("ChRydXN0L3dvcmxkc2F2ZS5wcm90bxIJUnVzdFByb3RvGg9ydXN0L2l0ZW0ucHJvdG8aEXJ1c3QvY29tbW9uLnByb3RvGhFydXN0L3ZpdGFscy5wcm90byIpCgpvYmplY3REb29yEg0KBVN0YXRlGAEgASgFEgwKBE9wZW4YAiABKAgiNgoQb2JqZWN0RGVwbG95YWJsZRIRCglDcmVhdG9ySUQYASABKAQSDwoHT3duZXJJRBgCIAEoBCJYChJvYmplY3RTdHJ1Y3RNYXN0ZXISCgoCSUQYASABKAUSEgoKRGVjYXlEZWxheRgCIAEoAhIRCglDcmVhdG9ySUQYAyABKAQSDwoHT3duZXJJRBgEIAEoBCJLChVvYmplY3RTdHJ1Y3RDb21wb25lbnQSCgoCSUQYASABKAUSEAoITWFzdGVySUQYAiABKAUSFAoMTWFzdGVyVmlld0lEGAMgASgFIiIKEG9iamVjdEZpcmVCYXJyZWwSDgoGT25GaXJlGAEgASgIImQKEW9iamVjdE5ldEluc3RhbmNlEhQKDHNlcnZlclByZWZhYhgBIAEoBRITCgtvd25lclByZWZhYhgCIAEoBRITCgtwcm94eVByZWZhYhgDIAEoBRIPCgdncm91cElEGAQgASgFIi0KEW9iamVjdE5HQ0luc3RhbmNlEgoKAklEGAEgASgFEgwKBGRhdGEYAiABKAwinAEKDG9iamVjdENvb3JkcxIeCgNwb3MYASABKAsyES5SdXN0UHJvdG8uVmVjdG9yEiEKBm9sZFBvcxgCIAEoCzIRLlJ1c3RQcm90by5WZWN0b3ISIgoDcm90GAMgASgLMhUuUnVzdFByb3RvLlF1YXRlcm5pb24SJQoGb2xkUm90GAQgASgLMhUuUnVzdFByb3RvLlF1YXRlcm5pb24iLwoVb2JqZWN0SUNhcnJpYWJsZVRyYW5zEhYKDnRyYW5zQ2FycmllcklEGAEgASgFIiIKEG9iamVjdFRha2VEYW1hZ2USDgoGaGVhbHRoGAEgASgCIpgBChRvYmplY3RTbGVlcGluZ0F2YXRhchIRCglmb290QXJtb3IYASABKAUSEAoIbGVnQXJtb3IYAiABKAUSEgoKdG9yc29Bcm1vchgDIAEoBRIRCgloZWFkQXJtb3IYBCABKAUSEQoJdGltZXN0YW1wGAUgASgFEiEKBnZpdGFscxgGIAEoCzIRLlJ1c3RQcm90by5WaXRhbHMiQQoOb2JqZWN0TG9ja2FibGUSEAoIcGFzc3dvcmQYASABKAkSDgoGbG9ja2VkGAIgASgIEg0KBXVzZXJzGAMgAygEIqcFCgtTYXZlZE9iamVjdBIKCgJpZBgBIAEoBRIjCgRkb29yGAIgASgLMhUuUnVzdFByb3RvLm9iamVjdERvb3ISIgoJaW52ZW50b3J5GAMgAygLMg8uUnVzdFByb3RvLkl0ZW0SLwoKZGVwbG95YWJsZRgEIAEoCzIbLlJ1c3RQcm90by5vYmplY3REZXBsb3lhYmxlEjMKDHN0cnVjdE1hc3RlchgFIAEoCzIdLlJ1c3RQcm90by5vYmplY3RTdHJ1Y3RNYXN0ZXISOQoPc3RydWN0Q29tcG9uZW50GAYgASgLMiAuUnVzdFByb3RvLm9iamVjdFN0cnVjdENvbXBvbmVudBIvCgpmaXJlQmFycmVsGAcgASgLMhsuUnVzdFByb3RvLm9iamVjdEZpcmVCYXJyZWwSMQoLbmV0SW5zdGFuY2UYCCABKAsyHC5SdXN0UHJvdG8ub2JqZWN0TmV0SW5zdGFuY2USJwoGY29vcmRzGAkgASgLMhcuUnVzdFByb3RvLm9iamVjdENvb3JkcxIxCgtuZ2NJbnN0YW5jZRgKIAEoCzIcLlJ1c3RQcm90by5vYmplY3ROR0NJbnN0YW5jZRI4Cg5jYXJyaWFibGVUcmFucxgLIAEoCzIgLlJ1c3RQcm90by5vYmplY3RJQ2FycmlhYmxlVHJhbnMSLwoKdGFrZURhbWFnZRgMIAEoCzIbLlJ1c3RQcm90by5vYmplY3RUYWtlRGFtYWdlEhEKCXNvcnRPcmRlchgNIAEoBRI3Cg5zbGVlcGluZ0F2YXRhchgOIAEoCzIfLlJ1c3RQcm90by5vYmplY3RTbGVlcGluZ0F2YXRhchIrCghsb2NrYWJsZRgPIAEoCzIZLlJ1c3RQcm90by5vYmplY3RMb2NrYWJsZSJoCglXb3JsZFNhdmUSKwoLc2NlbmVPYmplY3QYASADKAsyFi5SdXN0UHJvdG8uU2F2ZWRPYmplY3QSLgoOaW5zdGFuY2VPYmplY3QYAiADKAsyFi5SdXN0UHJvdG8uU2F2ZWRPYmplY3RCAkgB");
            if (<>f__am$cache1D == null)
            {
                <>f__am$cache1D = new FileDescriptor.InternalDescriptorAssigner(null, <Worldsave>m__C);
            }
            FileDescriptor.InternalDescriptorAssigner assigner = <>f__am$cache1D;
            FileDescriptor[] descriptorArray1 = new FileDescriptor[] { Item.Descriptor, Common.Descriptor, Vitals.Descriptor };
            FileDescriptor.InternalBuildGeneratedFileFrom(buffer, descriptorArray1, assigner);
        }

        [CompilerGenerated]
        private static ExtensionRegistry <Worldsave>m__C(FileDescriptor root)
        {
            descriptor = root;
            internal__static_RustProto_objectDoor__Descriptor = Descriptor.get_MessageTypes()[0];
            string[] textArray1 = new string[] { "State", "Open" };
            internal__static_RustProto_objectDoor__FieldAccessorTable = new FieldAccessorTable<objectDoor, objectDoor.Builder>(internal__static_RustProto_objectDoor__Descriptor, textArray1);
            internal__static_RustProto_objectDeployable__Descriptor = Descriptor.get_MessageTypes()[1];
            string[] textArray2 = new string[] { "CreatorID", "OwnerID" };
            internal__static_RustProto_objectDeployable__FieldAccessorTable = new FieldAccessorTable<objectDeployable, objectDeployable.Builder>(internal__static_RustProto_objectDeployable__Descriptor, textArray2);
            internal__static_RustProto_objectStructMaster__Descriptor = Descriptor.get_MessageTypes()[2];
            string[] textArray3 = new string[] { "ID", "DecayDelay", "CreatorID", "OwnerID" };
            internal__static_RustProto_objectStructMaster__FieldAccessorTable = new FieldAccessorTable<objectStructMaster, objectStructMaster.Builder>(internal__static_RustProto_objectStructMaster__Descriptor, textArray3);
            internal__static_RustProto_objectStructComponent__Descriptor = Descriptor.get_MessageTypes()[3];
            string[] textArray4 = new string[] { "ID", "MasterID", "MasterViewID" };
            internal__static_RustProto_objectStructComponent__FieldAccessorTable = new FieldAccessorTable<objectStructComponent, objectStructComponent.Builder>(internal__static_RustProto_objectStructComponent__Descriptor, textArray4);
            internal__static_RustProto_objectFireBarrel__Descriptor = Descriptor.get_MessageTypes()[4];
            string[] textArray5 = new string[] { "OnFire" };
            internal__static_RustProto_objectFireBarrel__FieldAccessorTable = new FieldAccessorTable<objectFireBarrel, objectFireBarrel.Builder>(internal__static_RustProto_objectFireBarrel__Descriptor, textArray5);
            internal__static_RustProto_objectNetInstance__Descriptor = Descriptor.get_MessageTypes()[5];
            string[] textArray6 = new string[] { "ServerPrefab", "OwnerPrefab", "ProxyPrefab", "GroupID" };
            internal__static_RustProto_objectNetInstance__FieldAccessorTable = new FieldAccessorTable<objectNetInstance, objectNetInstance.Builder>(internal__static_RustProto_objectNetInstance__Descriptor, textArray6);
            internal__static_RustProto_objectNGCInstance__Descriptor = Descriptor.get_MessageTypes()[6];
            string[] textArray7 = new string[] { "ID", "Data" };
            internal__static_RustProto_objectNGCInstance__FieldAccessorTable = new FieldAccessorTable<objectNGCInstance, objectNGCInstance.Builder>(internal__static_RustProto_objectNGCInstance__Descriptor, textArray7);
            internal__static_RustProto_objectCoords__Descriptor = Descriptor.get_MessageTypes()[7];
            string[] textArray8 = new string[] { "Pos", "OldPos", "Rot", "OldRot" };
            internal__static_RustProto_objectCoords__FieldAccessorTable = new FieldAccessorTable<objectCoords, objectCoords.Builder>(internal__static_RustProto_objectCoords__Descriptor, textArray8);
            internal__static_RustProto_objectICarriableTrans__Descriptor = Descriptor.get_MessageTypes()[8];
            string[] textArray9 = new string[] { "TransCarrierID" };
            internal__static_RustProto_objectICarriableTrans__FieldAccessorTable = new FieldAccessorTable<objectICarriableTrans, objectICarriableTrans.Builder>(internal__static_RustProto_objectICarriableTrans__Descriptor, textArray9);
            internal__static_RustProto_objectTakeDamage__Descriptor = Descriptor.get_MessageTypes()[9];
            string[] textArray10 = new string[] { "Health" };
            internal__static_RustProto_objectTakeDamage__FieldAccessorTable = new FieldAccessorTable<objectTakeDamage, objectTakeDamage.Builder>(internal__static_RustProto_objectTakeDamage__Descriptor, textArray10);
            internal__static_RustProto_objectSleepingAvatar__Descriptor = Descriptor.get_MessageTypes()[10];
            string[] textArray11 = new string[] { "FootArmor", "LegArmor", "TorsoArmor", "HeadArmor", "Timestamp", "Vitals" };
            internal__static_RustProto_objectSleepingAvatar__FieldAccessorTable = new FieldAccessorTable<objectSleepingAvatar, objectSleepingAvatar.Builder>(internal__static_RustProto_objectSleepingAvatar__Descriptor, textArray11);
            internal__static_RustProto_objectLockable__Descriptor = Descriptor.get_MessageTypes()[11];
            string[] textArray12 = new string[] { "Password", "Locked", "Users" };
            internal__static_RustProto_objectLockable__FieldAccessorTable = new FieldAccessorTable<objectLockable, objectLockable.Builder>(internal__static_RustProto_objectLockable__Descriptor, textArray12);
            internal__static_RustProto_SavedObject__Descriptor = Descriptor.get_MessageTypes()[12];
            string[] textArray13 = new string[] { "Id", "Door", "Inventory", "Deployable", "StructMaster", "StructComponent", "FireBarrel", "NetInstance", "Coords", "NgcInstance", "CarriableTrans", "TakeDamage", "SortOrder", "SleepingAvatar", "Lockable" };
            internal__static_RustProto_SavedObject__FieldAccessorTable = new FieldAccessorTable<SavedObject, SavedObject.Builder>(internal__static_RustProto_SavedObject__Descriptor, textArray13);
            internal__static_RustProto_WorldSave__Descriptor = Descriptor.get_MessageTypes()[13];
            string[] textArray14 = new string[] { "SceneObject", "InstanceObject" };
            internal__static_RustProto_WorldSave__FieldAccessorTable = new FieldAccessorTable<WorldSave, WorldSave.Builder>(internal__static_RustProto_WorldSave__Descriptor, textArray14);
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

