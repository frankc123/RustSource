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
    public sealed class SavedObject : GeneratedMessage<SavedObject, SavedObject.Builder>
    {
        private static readonly string[] _savedObjectFieldNames = new string[] { "carriableTrans", "coords", "deployable", "door", "fireBarrel", "id", "inventory", "lockable", "netInstance", "ngcInstance", "sleepingAvatar", "sortOrder", "structComponent", "structMaster", "takeDamage" };
        private static readonly uint[] _savedObjectFieldTags = new uint[] { 90, 0x4a, 0x22, 0x12, 0x3a, 8, 0x1a, 0x7a, 0x42, 0x52, 0x72, 0x68, 50, 0x2a, 0x62 };
        private objectICarriableTrans carriableTrans_;
        public const int CarriableTransFieldNumber = 11;
        private objectCoords coords_;
        public const int CoordsFieldNumber = 9;
        private static readonly SavedObject defaultInstance = new SavedObject().MakeReadOnly();
        private objectDeployable deployable_;
        public const int DeployableFieldNumber = 4;
        private objectDoor door_;
        public const int DoorFieldNumber = 2;
        private objectFireBarrel fireBarrel_;
        public const int FireBarrelFieldNumber = 7;
        private bool hasCarriableTrans;
        private bool hasCoords;
        private bool hasDeployable;
        private bool hasDoor;
        private bool hasFireBarrel;
        private bool hasId;
        private bool hasLockable;
        private bool hasNetInstance;
        private bool hasNgcInstance;
        private bool hasSleepingAvatar;
        private bool hasSortOrder;
        private bool hasStructComponent;
        private bool hasStructMaster;
        private bool hasTakeDamage;
        private int id_;
        public const int IdFieldNumber = 1;
        private PopsicleList<Item> inventory_ = new PopsicleList<Item>();
        public const int InventoryFieldNumber = 3;
        private objectLockable lockable_;
        public const int LockableFieldNumber = 15;
        private int memoizedSerializedSize = -1;
        private objectNetInstance netInstance_;
        public const int NetInstanceFieldNumber = 8;
        private objectNGCInstance ngcInstance_;
        public const int NgcInstanceFieldNumber = 10;
        private objectSleepingAvatar sleepingAvatar_;
        public const int SleepingAvatarFieldNumber = 14;
        private int sortOrder_;
        public const int SortOrderFieldNumber = 13;
        private objectStructComponent structComponent_;
        public const int StructComponentFieldNumber = 6;
        private objectStructMaster structMaster_;
        public const int StructMasterFieldNumber = 5;
        private objectTakeDamage takeDamage_;
        public const int TakeDamageFieldNumber = 12;

        static SavedObject()
        {
            object.ReferenceEquals(Worldsave.Descriptor, null);
        }

        private SavedObject()
        {
        }

        public static Builder CreateBuilder()
        {
            return new Builder();
        }

        public static Builder CreateBuilder(SavedObject prototype)
        {
            return new Builder(prototype);
        }

        public override Builder CreateBuilderForType()
        {
            return new Builder();
        }

        public Item GetInventory(int index)
        {
            return this.inventory_.get_Item(index);
        }

        private SavedObject MakeReadOnly()
        {
            this.inventory_.MakeReadOnly();
            return this;
        }

        public static SavedObject ParseDelimitedFrom(Stream input)
        {
            return CreateBuilder().MergeDelimitedFrom(input).BuildParsed();
        }

        public static SavedObject ParseDelimitedFrom(Stream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeDelimitedFrom(input, extensionRegistry).BuildParsed();
        }

        public static SavedObject ParseFrom(ByteString data)
        {
            return CreateBuilder().MergeFrom(data).BuildParsed();
        }

        public static SavedObject ParseFrom(byte[] data)
        {
            return CreateBuilder().MergeFrom(data).BuildParsed();
        }

        public static SavedObject ParseFrom(ICodedInputStream input)
        {
            return CreateBuilder().MergeFrom(input).BuildParsed();
        }

        public static SavedObject ParseFrom(Stream input)
        {
            return CreateBuilder().MergeFrom(input).BuildParsed();
        }

        public static SavedObject ParseFrom(ByteString data, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
        }

        public static SavedObject ParseFrom(byte[] data, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
        }

        public static SavedObject ParseFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
        }

        public static SavedObject ParseFrom(Stream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
        }

        public static Recycler<SavedObject, Builder> Recycler()
        {
            return Recycler<SavedObject, Builder>.Manufacture();
        }

        public override Builder ToBuilder()
        {
            return CreateBuilder(this);
        }

        public override void WriteTo(ICodedOutputStream output)
        {
            int serializedSize = this.SerializedSize;
            string[] strArray = _savedObjectFieldNames;
            if (this.hasId)
            {
                output.WriteInt32(1, strArray[5], this.Id);
            }
            if (this.hasDoor)
            {
                output.WriteMessage(2, strArray[3], this.Door);
            }
            if (this.inventory_.get_Count() > 0)
            {
                output.WriteMessageArray<Item>(3, strArray[6], this.inventory_);
            }
            if (this.hasDeployable)
            {
                output.WriteMessage(4, strArray[2], this.Deployable);
            }
            if (this.hasStructMaster)
            {
                output.WriteMessage(5, strArray[13], this.StructMaster);
            }
            if (this.hasStructComponent)
            {
                output.WriteMessage(6, strArray[12], this.StructComponent);
            }
            if (this.hasFireBarrel)
            {
                output.WriteMessage(7, strArray[4], this.FireBarrel);
            }
            if (this.hasNetInstance)
            {
                output.WriteMessage(8, strArray[8], this.NetInstance);
            }
            if (this.hasCoords)
            {
                output.WriteMessage(9, strArray[1], this.Coords);
            }
            if (this.hasNgcInstance)
            {
                output.WriteMessage(10, strArray[9], this.NgcInstance);
            }
            if (this.hasCarriableTrans)
            {
                output.WriteMessage(11, strArray[0], this.CarriableTrans);
            }
            if (this.hasTakeDamage)
            {
                output.WriteMessage(12, strArray[14], this.TakeDamage);
            }
            if (this.hasSortOrder)
            {
                output.WriteInt32(13, strArray[11], this.SortOrder);
            }
            if (this.hasSleepingAvatar)
            {
                output.WriteMessage(14, strArray[10], this.SleepingAvatar);
            }
            if (this.hasLockable)
            {
                output.WriteMessage(15, strArray[7], this.Lockable);
            }
            this.get_UnknownFields().WriteTo(output);
        }

        public objectICarriableTrans CarriableTrans
        {
            get
            {
                if (this.carriableTrans_ == null)
                {
                }
                return objectICarriableTrans.DefaultInstance;
            }
        }

        public objectCoords Coords
        {
            get
            {
                if (this.coords_ == null)
                {
                }
                return objectCoords.DefaultInstance;
            }
        }

        public static SavedObject DefaultInstance
        {
            get
            {
                return defaultInstance;
            }
        }

        public override SavedObject DefaultInstanceForType
        {
            get
            {
                return DefaultInstance;
            }
        }

        public objectDeployable Deployable
        {
            get
            {
                if (this.deployable_ == null)
                {
                }
                return objectDeployable.DefaultInstance;
            }
        }

        public static MessageDescriptor Descriptor
        {
            get
            {
                return Worldsave.internal__static_RustProto_SavedObject__Descriptor;
            }
        }

        public objectDoor Door
        {
            get
            {
                if (this.door_ == null)
                {
                }
                return objectDoor.DefaultInstance;
            }
        }

        public objectFireBarrel FireBarrel
        {
            get
            {
                if (this.fireBarrel_ == null)
                {
                }
                return objectFireBarrel.DefaultInstance;
            }
        }

        public bool HasCarriableTrans
        {
            get
            {
                return this.hasCarriableTrans;
            }
        }

        public bool HasCoords
        {
            get
            {
                return this.hasCoords;
            }
        }

        public bool HasDeployable
        {
            get
            {
                return this.hasDeployable;
            }
        }

        public bool HasDoor
        {
            get
            {
                return this.hasDoor;
            }
        }

        public bool HasFireBarrel
        {
            get
            {
                return this.hasFireBarrel;
            }
        }

        public bool HasId
        {
            get
            {
                return this.hasId;
            }
        }

        public bool HasLockable
        {
            get
            {
                return this.hasLockable;
            }
        }

        public bool HasNetInstance
        {
            get
            {
                return this.hasNetInstance;
            }
        }

        public bool HasNgcInstance
        {
            get
            {
                return this.hasNgcInstance;
            }
        }

        public bool HasSleepingAvatar
        {
            get
            {
                return this.hasSleepingAvatar;
            }
        }

        public bool HasSortOrder
        {
            get
            {
                return this.hasSortOrder;
            }
        }

        public bool HasStructComponent
        {
            get
            {
                return this.hasStructComponent;
            }
        }

        public bool HasStructMaster
        {
            get
            {
                return this.hasStructMaster;
            }
        }

        public bool HasTakeDamage
        {
            get
            {
                return this.hasTakeDamage;
            }
        }

        public int Id
        {
            get
            {
                return this.id_;
            }
        }

        protected override FieldAccessorTable<SavedObject, Builder> InternalFieldAccessors
        {
            get
            {
                return Worldsave.internal__static_RustProto_SavedObject__FieldAccessorTable;
            }
        }

        public int InventoryCount
        {
            get
            {
                return this.inventory_.get_Count();
            }
        }

        public IList<Item> InventoryList
        {
            get
            {
                return this.inventory_;
            }
        }

        public override bool IsInitialized
        {
            get
            {
                IEnumerator<Item> enumerator = this.InventoryList.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        Item current = enumerator.Current;
                        if (!current.IsInitialized)
                        {
                            return false;
                        }
                    }
                }
                finally
                {
                    if (enumerator == null)
                    {
                    }
                    enumerator.Dispose();
                }
                return true;
            }
        }

        public objectLockable Lockable
        {
            get
            {
                if (this.lockable_ == null)
                {
                }
                return objectLockable.DefaultInstance;
            }
        }

        public objectNetInstance NetInstance
        {
            get
            {
                if (this.netInstance_ == null)
                {
                }
                return objectNetInstance.DefaultInstance;
            }
        }

        public objectNGCInstance NgcInstance
        {
            get
            {
                if (this.ngcInstance_ == null)
                {
                }
                return objectNGCInstance.DefaultInstance;
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
                    if (this.hasId)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeInt32Size(1, this.Id);
                    }
                    if (this.hasDoor)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeMessageSize(2, this.Door);
                    }
                    IEnumerator<Item> enumerator = this.InventoryList.GetEnumerator();
                    try
                    {
                        while (enumerator.MoveNext())
                        {
                            Item current = enumerator.Current;
                            memoizedSerializedSize += CodedOutputStream.ComputeMessageSize(3, current);
                        }
                    }
                    finally
                    {
                        if (enumerator == null)
                        {
                        }
                        enumerator.Dispose();
                    }
                    if (this.hasDeployable)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeMessageSize(4, this.Deployable);
                    }
                    if (this.hasStructMaster)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeMessageSize(5, this.StructMaster);
                    }
                    if (this.hasStructComponent)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeMessageSize(6, this.StructComponent);
                    }
                    if (this.hasFireBarrel)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeMessageSize(7, this.FireBarrel);
                    }
                    if (this.hasNetInstance)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeMessageSize(8, this.NetInstance);
                    }
                    if (this.hasCoords)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeMessageSize(9, this.Coords);
                    }
                    if (this.hasNgcInstance)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeMessageSize(10, this.NgcInstance);
                    }
                    if (this.hasCarriableTrans)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeMessageSize(11, this.CarriableTrans);
                    }
                    if (this.hasTakeDamage)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeMessageSize(12, this.TakeDamage);
                    }
                    if (this.hasSortOrder)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeInt32Size(13, this.SortOrder);
                    }
                    if (this.hasSleepingAvatar)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeMessageSize(14, this.SleepingAvatar);
                    }
                    if (this.hasLockable)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeMessageSize(15, this.Lockable);
                    }
                    memoizedSerializedSize += this.get_UnknownFields().get_SerializedSize();
                    this.memoizedSerializedSize = memoizedSerializedSize;
                }
                return memoizedSerializedSize;
            }
        }

        public objectSleepingAvatar SleepingAvatar
        {
            get
            {
                if (this.sleepingAvatar_ == null)
                {
                }
                return objectSleepingAvatar.DefaultInstance;
            }
        }

        public int SortOrder
        {
            get
            {
                return this.sortOrder_;
            }
        }

        public objectStructComponent StructComponent
        {
            get
            {
                if (this.structComponent_ == null)
                {
                }
                return objectStructComponent.DefaultInstance;
            }
        }

        public objectStructMaster StructMaster
        {
            get
            {
                if (this.structMaster_ == null)
                {
                }
                return objectStructMaster.DefaultInstance;
            }
        }

        public objectTakeDamage TakeDamage
        {
            get
            {
                if (this.takeDamage_ == null)
                {
                }
                return objectTakeDamage.DefaultInstance;
            }
        }

        protected override SavedObject ThisMessage
        {
            get
            {
                return this;
            }
        }

        [DebuggerNonUserCode]
        public sealed class Builder : GeneratedBuilder<SavedObject, SavedObject.Builder>
        {
            private SavedObject result;
            private bool resultIsReadOnly;

            public Builder()
            {
                this.result = SavedObject.DefaultInstance;
                this.resultIsReadOnly = true;
            }

            internal Builder(SavedObject cloneFrom)
            {
                this.result = cloneFrom;
                this.resultIsReadOnly = true;
            }

            public SavedObject.Builder AddInventory(Item value)
            {
                ThrowHelper.ThrowIfNull(value, "value");
                this.PrepareBuilder();
                this.result.inventory_.Add(value);
                return this;
            }

            public SavedObject.Builder AddInventory(Item.Builder builderForValue)
            {
                ThrowHelper.ThrowIfNull(builderForValue, "builderForValue");
                this.PrepareBuilder();
                this.result.inventory_.Add(builderForValue.Build());
                return this;
            }

            public SavedObject.Builder AddRangeInventory(IEnumerable<Item> values)
            {
                this.PrepareBuilder();
                this.result.inventory_.Add(values);
                return this;
            }

            public override SavedObject BuildPartial()
            {
                if (this.resultIsReadOnly)
                {
                    return this.result;
                }
                this.resultIsReadOnly = true;
                return this.result.MakeReadOnly();
            }

            public override SavedObject.Builder Clear()
            {
                this.result = SavedObject.DefaultInstance;
                this.resultIsReadOnly = true;
                return this;
            }

            public SavedObject.Builder ClearCarriableTrans()
            {
                this.PrepareBuilder();
                this.result.hasCarriableTrans = false;
                this.result.carriableTrans_ = null;
                return this;
            }

            public SavedObject.Builder ClearCoords()
            {
                this.PrepareBuilder();
                this.result.hasCoords = false;
                this.result.coords_ = null;
                return this;
            }

            public SavedObject.Builder ClearDeployable()
            {
                this.PrepareBuilder();
                this.result.hasDeployable = false;
                this.result.deployable_ = null;
                return this;
            }

            public SavedObject.Builder ClearDoor()
            {
                this.PrepareBuilder();
                this.result.hasDoor = false;
                this.result.door_ = null;
                return this;
            }

            public SavedObject.Builder ClearFireBarrel()
            {
                this.PrepareBuilder();
                this.result.hasFireBarrel = false;
                this.result.fireBarrel_ = null;
                return this;
            }

            public SavedObject.Builder ClearId()
            {
                this.PrepareBuilder();
                this.result.hasId = false;
                this.result.id_ = 0;
                return this;
            }

            public SavedObject.Builder ClearInventory()
            {
                this.PrepareBuilder();
                this.result.inventory_.Clear();
                return this;
            }

            public SavedObject.Builder ClearLockable()
            {
                this.PrepareBuilder();
                this.result.hasLockable = false;
                this.result.lockable_ = null;
                return this;
            }

            public SavedObject.Builder ClearNetInstance()
            {
                this.PrepareBuilder();
                this.result.hasNetInstance = false;
                this.result.netInstance_ = null;
                return this;
            }

            public SavedObject.Builder ClearNgcInstance()
            {
                this.PrepareBuilder();
                this.result.hasNgcInstance = false;
                this.result.ngcInstance_ = null;
                return this;
            }

            public SavedObject.Builder ClearSleepingAvatar()
            {
                this.PrepareBuilder();
                this.result.hasSleepingAvatar = false;
                this.result.sleepingAvatar_ = null;
                return this;
            }

            public SavedObject.Builder ClearSortOrder()
            {
                this.PrepareBuilder();
                this.result.hasSortOrder = false;
                this.result.sortOrder_ = 0;
                return this;
            }

            public SavedObject.Builder ClearStructComponent()
            {
                this.PrepareBuilder();
                this.result.hasStructComponent = false;
                this.result.structComponent_ = null;
                return this;
            }

            public SavedObject.Builder ClearStructMaster()
            {
                this.PrepareBuilder();
                this.result.hasStructMaster = false;
                this.result.structMaster_ = null;
                return this;
            }

            public SavedObject.Builder ClearTakeDamage()
            {
                this.PrepareBuilder();
                this.result.hasTakeDamage = false;
                this.result.takeDamage_ = null;
                return this;
            }

            public override SavedObject.Builder Clone()
            {
                if (this.resultIsReadOnly)
                {
                    return new SavedObject.Builder(this.result);
                }
                return new SavedObject.Builder().MergeFrom(this.result);
            }

            public Item GetInventory(int index)
            {
                return this.result.GetInventory(index);
            }

            public SavedObject.Builder MergeCarriableTrans(objectICarriableTrans value)
            {
                ThrowHelper.ThrowIfNull(value, "value");
                this.PrepareBuilder();
                if (this.result.hasCarriableTrans && (this.result.carriableTrans_ != objectICarriableTrans.DefaultInstance))
                {
                    this.result.carriableTrans_ = objectICarriableTrans.CreateBuilder(this.result.carriableTrans_).MergeFrom(value).BuildPartial();
                }
                else
                {
                    this.result.carriableTrans_ = value;
                }
                this.result.hasCarriableTrans = true;
                return this;
            }

            public SavedObject.Builder MergeCoords(objectCoords value)
            {
                ThrowHelper.ThrowIfNull(value, "value");
                this.PrepareBuilder();
                if (this.result.hasCoords && (this.result.coords_ != objectCoords.DefaultInstance))
                {
                    this.result.coords_ = objectCoords.CreateBuilder(this.result.coords_).MergeFrom(value).BuildPartial();
                }
                else
                {
                    this.result.coords_ = value;
                }
                this.result.hasCoords = true;
                return this;
            }

            public SavedObject.Builder MergeDeployable(objectDeployable value)
            {
                ThrowHelper.ThrowIfNull(value, "value");
                this.PrepareBuilder();
                if (this.result.hasDeployable && (this.result.deployable_ != objectDeployable.DefaultInstance))
                {
                    this.result.deployable_ = objectDeployable.CreateBuilder(this.result.deployable_).MergeFrom(value).BuildPartial();
                }
                else
                {
                    this.result.deployable_ = value;
                }
                this.result.hasDeployable = true;
                return this;
            }

            public SavedObject.Builder MergeDoor(objectDoor value)
            {
                ThrowHelper.ThrowIfNull(value, "value");
                this.PrepareBuilder();
                if (this.result.hasDoor && (this.result.door_ != objectDoor.DefaultInstance))
                {
                    this.result.door_ = objectDoor.CreateBuilder(this.result.door_).MergeFrom(value).BuildPartial();
                }
                else
                {
                    this.result.door_ = value;
                }
                this.result.hasDoor = true;
                return this;
            }

            public SavedObject.Builder MergeFireBarrel(objectFireBarrel value)
            {
                ThrowHelper.ThrowIfNull(value, "value");
                this.PrepareBuilder();
                if (this.result.hasFireBarrel && (this.result.fireBarrel_ != objectFireBarrel.DefaultInstance))
                {
                    this.result.fireBarrel_ = objectFireBarrel.CreateBuilder(this.result.fireBarrel_).MergeFrom(value).BuildPartial();
                }
                else
                {
                    this.result.fireBarrel_ = value;
                }
                this.result.hasFireBarrel = true;
                return this;
            }

            public override SavedObject.Builder MergeFrom(ICodedInputStream input)
            {
                return this.MergeFrom(input, ExtensionRegistry.get_Empty());
            }

            public override SavedObject.Builder MergeFrom(IMessage other)
            {
                if (other is SavedObject)
                {
                    return this.MergeFrom((SavedObject) other);
                }
                base.MergeFrom(other);
                return this;
            }

            public override SavedObject.Builder MergeFrom(SavedObject other)
            {
                if (other != SavedObject.DefaultInstance)
                {
                    this.PrepareBuilder();
                    if (other.HasId)
                    {
                        this.Id = other.Id;
                    }
                    if (other.HasDoor)
                    {
                        this.MergeDoor(other.Door);
                    }
                    if (other.inventory_.get_Count() != 0)
                    {
                        this.result.inventory_.Add(other.inventory_);
                    }
                    if (other.HasDeployable)
                    {
                        this.MergeDeployable(other.Deployable);
                    }
                    if (other.HasStructMaster)
                    {
                        this.MergeStructMaster(other.StructMaster);
                    }
                    if (other.HasStructComponent)
                    {
                        this.MergeStructComponent(other.StructComponent);
                    }
                    if (other.HasFireBarrel)
                    {
                        this.MergeFireBarrel(other.FireBarrel);
                    }
                    if (other.HasNetInstance)
                    {
                        this.MergeNetInstance(other.NetInstance);
                    }
                    if (other.HasCoords)
                    {
                        this.MergeCoords(other.Coords);
                    }
                    if (other.HasNgcInstance)
                    {
                        this.MergeNgcInstance(other.NgcInstance);
                    }
                    if (other.HasCarriableTrans)
                    {
                        this.MergeCarriableTrans(other.CarriableTrans);
                    }
                    if (other.HasTakeDamage)
                    {
                        this.MergeTakeDamage(other.TakeDamage);
                    }
                    if (other.HasSortOrder)
                    {
                        this.SortOrder = other.SortOrder;
                    }
                    if (other.HasSleepingAvatar)
                    {
                        this.MergeSleepingAvatar(other.SleepingAvatar);
                    }
                    if (other.HasLockable)
                    {
                        this.MergeLockable(other.Lockable);
                    }
                    this.MergeUnknownFields(other.get_UnknownFields());
                }
                return this;
            }

            public override SavedObject.Builder MergeFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
            {
                uint num;
                string str;
                this.PrepareBuilder();
                UnknownFieldSet.Builder builder = null;
                while (input.ReadTag(ref num, ref str))
                {
                    if ((num == 0) && (str != null))
                    {
                        int index = Array.BinarySearch<string>(SavedObject._savedObjectFieldNames, str, StringComparer.Ordinal);
                        if (index >= 0)
                        {
                            num = SavedObject._savedObjectFieldTags[index];
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
                            this.result.hasId = input.ReadInt32(ref this.result.id_);
                            continue;
                        }
                        case 0x12:
                        {
                            objectDoor.Builder builder2 = objectDoor.CreateBuilder();
                            if (this.result.hasDoor)
                            {
                                builder2.MergeFrom(this.Door);
                            }
                            input.ReadMessage(builder2, extensionRegistry);
                            this.Door = builder2.BuildPartial();
                            continue;
                        }
                        case 0x1a:
                        {
                            input.ReadMessageArray<Item>(num, str, this.result.inventory_, Item.DefaultInstance, extensionRegistry);
                            continue;
                        }
                        case 0x22:
                        {
                            objectDeployable.Builder builder3 = objectDeployable.CreateBuilder();
                            if (this.result.hasDeployable)
                            {
                                builder3.MergeFrom(this.Deployable);
                            }
                            input.ReadMessage(builder3, extensionRegistry);
                            this.Deployable = builder3.BuildPartial();
                            continue;
                        }
                        case 0x2a:
                        {
                            objectStructMaster.Builder builder4 = objectStructMaster.CreateBuilder();
                            if (this.result.hasStructMaster)
                            {
                                builder4.MergeFrom(this.StructMaster);
                            }
                            input.ReadMessage(builder4, extensionRegistry);
                            this.StructMaster = builder4.BuildPartial();
                            continue;
                        }
                        case 50:
                        {
                            objectStructComponent.Builder builder5 = objectStructComponent.CreateBuilder();
                            if (this.result.hasStructComponent)
                            {
                                builder5.MergeFrom(this.StructComponent);
                            }
                            input.ReadMessage(builder5, extensionRegistry);
                            this.StructComponent = builder5.BuildPartial();
                            continue;
                        }
                        case 0x3a:
                        {
                            objectFireBarrel.Builder builder6 = objectFireBarrel.CreateBuilder();
                            if (this.result.hasFireBarrel)
                            {
                                builder6.MergeFrom(this.FireBarrel);
                            }
                            input.ReadMessage(builder6, extensionRegistry);
                            this.FireBarrel = builder6.BuildPartial();
                            continue;
                        }
                        case 0x42:
                        {
                            objectNetInstance.Builder builder7 = objectNetInstance.CreateBuilder();
                            if (this.result.hasNetInstance)
                            {
                                builder7.MergeFrom(this.NetInstance);
                            }
                            input.ReadMessage(builder7, extensionRegistry);
                            this.NetInstance = builder7.BuildPartial();
                            continue;
                        }
                        case 0x4a:
                        {
                            objectCoords.Builder builder8 = objectCoords.CreateBuilder();
                            if (this.result.hasCoords)
                            {
                                builder8.MergeFrom(this.Coords);
                            }
                            input.ReadMessage(builder8, extensionRegistry);
                            this.Coords = builder8.BuildPartial();
                            continue;
                        }
                        case 0x52:
                        {
                            objectNGCInstance.Builder builder9 = objectNGCInstance.CreateBuilder();
                            if (this.result.hasNgcInstance)
                            {
                                builder9.MergeFrom(this.NgcInstance);
                            }
                            input.ReadMessage(builder9, extensionRegistry);
                            this.NgcInstance = builder9.BuildPartial();
                            continue;
                        }
                        case 90:
                        {
                            objectICarriableTrans.Builder builder10 = objectICarriableTrans.CreateBuilder();
                            if (this.result.hasCarriableTrans)
                            {
                                builder10.MergeFrom(this.CarriableTrans);
                            }
                            input.ReadMessage(builder10, extensionRegistry);
                            this.CarriableTrans = builder10.BuildPartial();
                            continue;
                        }
                        case 0x62:
                        {
                            objectTakeDamage.Builder builder11 = objectTakeDamage.CreateBuilder();
                            if (this.result.hasTakeDamage)
                            {
                                builder11.MergeFrom(this.TakeDamage);
                            }
                            input.ReadMessage(builder11, extensionRegistry);
                            this.TakeDamage = builder11.BuildPartial();
                            continue;
                        }
                        case 0x68:
                        {
                            this.result.hasSortOrder = input.ReadInt32(ref this.result.sortOrder_);
                            continue;
                        }
                        case 0x72:
                        {
                            objectSleepingAvatar.Builder builder12 = objectSleepingAvatar.CreateBuilder();
                            if (this.result.hasSleepingAvatar)
                            {
                                builder12.MergeFrom(this.SleepingAvatar);
                            }
                            input.ReadMessage(builder12, extensionRegistry);
                            this.SleepingAvatar = builder12.BuildPartial();
                            continue;
                        }
                        case 0x7a:
                        {
                            objectLockable.Builder builder13 = objectLockable.CreateBuilder();
                            if (this.result.hasLockable)
                            {
                                builder13.MergeFrom(this.Lockable);
                            }
                            input.ReadMessage(builder13, extensionRegistry);
                            this.Lockable = builder13.BuildPartial();
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

            public SavedObject.Builder MergeLockable(objectLockable value)
            {
                ThrowHelper.ThrowIfNull(value, "value");
                this.PrepareBuilder();
                if (this.result.hasLockable && (this.result.lockable_ != objectLockable.DefaultInstance))
                {
                    this.result.lockable_ = objectLockable.CreateBuilder(this.result.lockable_).MergeFrom(value).BuildPartial();
                }
                else
                {
                    this.result.lockable_ = value;
                }
                this.result.hasLockable = true;
                return this;
            }

            public SavedObject.Builder MergeNetInstance(objectNetInstance value)
            {
                ThrowHelper.ThrowIfNull(value, "value");
                this.PrepareBuilder();
                if (this.result.hasNetInstance && (this.result.netInstance_ != objectNetInstance.DefaultInstance))
                {
                    this.result.netInstance_ = objectNetInstance.CreateBuilder(this.result.netInstance_).MergeFrom(value).BuildPartial();
                }
                else
                {
                    this.result.netInstance_ = value;
                }
                this.result.hasNetInstance = true;
                return this;
            }

            public SavedObject.Builder MergeNgcInstance(objectNGCInstance value)
            {
                ThrowHelper.ThrowIfNull(value, "value");
                this.PrepareBuilder();
                if (this.result.hasNgcInstance && (this.result.ngcInstance_ != objectNGCInstance.DefaultInstance))
                {
                    this.result.ngcInstance_ = objectNGCInstance.CreateBuilder(this.result.ngcInstance_).MergeFrom(value).BuildPartial();
                }
                else
                {
                    this.result.ngcInstance_ = value;
                }
                this.result.hasNgcInstance = true;
                return this;
            }

            public SavedObject.Builder MergeSleepingAvatar(objectSleepingAvatar value)
            {
                ThrowHelper.ThrowIfNull(value, "value");
                this.PrepareBuilder();
                if (this.result.hasSleepingAvatar && (this.result.sleepingAvatar_ != objectSleepingAvatar.DefaultInstance))
                {
                    this.result.sleepingAvatar_ = objectSleepingAvatar.CreateBuilder(this.result.sleepingAvatar_).MergeFrom(value).BuildPartial();
                }
                else
                {
                    this.result.sleepingAvatar_ = value;
                }
                this.result.hasSleepingAvatar = true;
                return this;
            }

            public SavedObject.Builder MergeStructComponent(objectStructComponent value)
            {
                ThrowHelper.ThrowIfNull(value, "value");
                this.PrepareBuilder();
                if (this.result.hasStructComponent && (this.result.structComponent_ != objectStructComponent.DefaultInstance))
                {
                    this.result.structComponent_ = objectStructComponent.CreateBuilder(this.result.structComponent_).MergeFrom(value).BuildPartial();
                }
                else
                {
                    this.result.structComponent_ = value;
                }
                this.result.hasStructComponent = true;
                return this;
            }

            public SavedObject.Builder MergeStructMaster(objectStructMaster value)
            {
                ThrowHelper.ThrowIfNull(value, "value");
                this.PrepareBuilder();
                if (this.result.hasStructMaster && (this.result.structMaster_ != objectStructMaster.DefaultInstance))
                {
                    this.result.structMaster_ = objectStructMaster.CreateBuilder(this.result.structMaster_).MergeFrom(value).BuildPartial();
                }
                else
                {
                    this.result.structMaster_ = value;
                }
                this.result.hasStructMaster = true;
                return this;
            }

            public SavedObject.Builder MergeTakeDamage(objectTakeDamage value)
            {
                ThrowHelper.ThrowIfNull(value, "value");
                this.PrepareBuilder();
                if (this.result.hasTakeDamage && (this.result.takeDamage_ != objectTakeDamage.DefaultInstance))
                {
                    this.result.takeDamage_ = objectTakeDamage.CreateBuilder(this.result.takeDamage_).MergeFrom(value).BuildPartial();
                }
                else
                {
                    this.result.takeDamage_ = value;
                }
                this.result.hasTakeDamage = true;
                return this;
            }

            private SavedObject PrepareBuilder()
            {
                if (this.resultIsReadOnly)
                {
                    SavedObject result = this.result;
                    this.result = new SavedObject();
                    this.resultIsReadOnly = false;
                    this.MergeFrom(result);
                }
                return this.result;
            }

            public SavedObject.Builder SetCarriableTrans(objectICarriableTrans value)
            {
                ThrowHelper.ThrowIfNull(value, "value");
                this.PrepareBuilder();
                this.result.hasCarriableTrans = true;
                this.result.carriableTrans_ = value;
                return this;
            }

            public SavedObject.Builder SetCarriableTrans(objectICarriableTrans.Builder builderForValue)
            {
                ThrowHelper.ThrowIfNull(builderForValue, "builderForValue");
                this.PrepareBuilder();
                this.result.hasCarriableTrans = true;
                this.result.carriableTrans_ = builderForValue.Build();
                return this;
            }

            public SavedObject.Builder SetCoords(objectCoords value)
            {
                ThrowHelper.ThrowIfNull(value, "value");
                this.PrepareBuilder();
                this.result.hasCoords = true;
                this.result.coords_ = value;
                return this;
            }

            public SavedObject.Builder SetCoords(objectCoords.Builder builderForValue)
            {
                ThrowHelper.ThrowIfNull(builderForValue, "builderForValue");
                this.PrepareBuilder();
                this.result.hasCoords = true;
                this.result.coords_ = builderForValue.Build();
                return this;
            }

            public SavedObject.Builder SetDeployable(objectDeployable value)
            {
                ThrowHelper.ThrowIfNull(value, "value");
                this.PrepareBuilder();
                this.result.hasDeployable = true;
                this.result.deployable_ = value;
                return this;
            }

            public SavedObject.Builder SetDeployable(objectDeployable.Builder builderForValue)
            {
                ThrowHelper.ThrowIfNull(builderForValue, "builderForValue");
                this.PrepareBuilder();
                this.result.hasDeployable = true;
                this.result.deployable_ = builderForValue.Build();
                return this;
            }

            public SavedObject.Builder SetDoor(objectDoor value)
            {
                ThrowHelper.ThrowIfNull(value, "value");
                this.PrepareBuilder();
                this.result.hasDoor = true;
                this.result.door_ = value;
                return this;
            }

            public SavedObject.Builder SetDoor(objectDoor.Builder builderForValue)
            {
                ThrowHelper.ThrowIfNull(builderForValue, "builderForValue");
                this.PrepareBuilder();
                this.result.hasDoor = true;
                this.result.door_ = builderForValue.Build();
                return this;
            }

            public SavedObject.Builder SetFireBarrel(objectFireBarrel value)
            {
                ThrowHelper.ThrowIfNull(value, "value");
                this.PrepareBuilder();
                this.result.hasFireBarrel = true;
                this.result.fireBarrel_ = value;
                return this;
            }

            public SavedObject.Builder SetFireBarrel(objectFireBarrel.Builder builderForValue)
            {
                ThrowHelper.ThrowIfNull(builderForValue, "builderForValue");
                this.PrepareBuilder();
                this.result.hasFireBarrel = true;
                this.result.fireBarrel_ = builderForValue.Build();
                return this;
            }

            public SavedObject.Builder SetId(int value)
            {
                this.PrepareBuilder();
                this.result.hasId = true;
                this.result.id_ = value;
                return this;
            }

            public SavedObject.Builder SetInventory(int index, Item value)
            {
                ThrowHelper.ThrowIfNull(value, "value");
                this.PrepareBuilder();
                this.result.inventory_.set_Item(index, value);
                return this;
            }

            public SavedObject.Builder SetInventory(int index, Item.Builder builderForValue)
            {
                ThrowHelper.ThrowIfNull(builderForValue, "builderForValue");
                this.PrepareBuilder();
                this.result.inventory_.set_Item(index, builderForValue.Build());
                return this;
            }

            public SavedObject.Builder SetLockable(objectLockable value)
            {
                ThrowHelper.ThrowIfNull(value, "value");
                this.PrepareBuilder();
                this.result.hasLockable = true;
                this.result.lockable_ = value;
                return this;
            }

            public SavedObject.Builder SetLockable(objectLockable.Builder builderForValue)
            {
                ThrowHelper.ThrowIfNull(builderForValue, "builderForValue");
                this.PrepareBuilder();
                this.result.hasLockable = true;
                this.result.lockable_ = builderForValue.Build();
                return this;
            }

            public SavedObject.Builder SetNetInstance(objectNetInstance value)
            {
                ThrowHelper.ThrowIfNull(value, "value");
                this.PrepareBuilder();
                this.result.hasNetInstance = true;
                this.result.netInstance_ = value;
                return this;
            }

            public SavedObject.Builder SetNetInstance(objectNetInstance.Builder builderForValue)
            {
                ThrowHelper.ThrowIfNull(builderForValue, "builderForValue");
                this.PrepareBuilder();
                this.result.hasNetInstance = true;
                this.result.netInstance_ = builderForValue.Build();
                return this;
            }

            public SavedObject.Builder SetNgcInstance(objectNGCInstance value)
            {
                ThrowHelper.ThrowIfNull(value, "value");
                this.PrepareBuilder();
                this.result.hasNgcInstance = true;
                this.result.ngcInstance_ = value;
                return this;
            }

            public SavedObject.Builder SetNgcInstance(objectNGCInstance.Builder builderForValue)
            {
                ThrowHelper.ThrowIfNull(builderForValue, "builderForValue");
                this.PrepareBuilder();
                this.result.hasNgcInstance = true;
                this.result.ngcInstance_ = builderForValue.Build();
                return this;
            }

            public SavedObject.Builder SetSleepingAvatar(objectSleepingAvatar value)
            {
                ThrowHelper.ThrowIfNull(value, "value");
                this.PrepareBuilder();
                this.result.hasSleepingAvatar = true;
                this.result.sleepingAvatar_ = value;
                return this;
            }

            public SavedObject.Builder SetSleepingAvatar(objectSleepingAvatar.Builder builderForValue)
            {
                ThrowHelper.ThrowIfNull(builderForValue, "builderForValue");
                this.PrepareBuilder();
                this.result.hasSleepingAvatar = true;
                this.result.sleepingAvatar_ = builderForValue.Build();
                return this;
            }

            public SavedObject.Builder SetSortOrder(int value)
            {
                this.PrepareBuilder();
                this.result.hasSortOrder = true;
                this.result.sortOrder_ = value;
                return this;
            }

            public SavedObject.Builder SetStructComponent(objectStructComponent value)
            {
                ThrowHelper.ThrowIfNull(value, "value");
                this.PrepareBuilder();
                this.result.hasStructComponent = true;
                this.result.structComponent_ = value;
                return this;
            }

            public SavedObject.Builder SetStructComponent(objectStructComponent.Builder builderForValue)
            {
                ThrowHelper.ThrowIfNull(builderForValue, "builderForValue");
                this.PrepareBuilder();
                this.result.hasStructComponent = true;
                this.result.structComponent_ = builderForValue.Build();
                return this;
            }

            public SavedObject.Builder SetStructMaster(objectStructMaster value)
            {
                ThrowHelper.ThrowIfNull(value, "value");
                this.PrepareBuilder();
                this.result.hasStructMaster = true;
                this.result.structMaster_ = value;
                return this;
            }

            public SavedObject.Builder SetStructMaster(objectStructMaster.Builder builderForValue)
            {
                ThrowHelper.ThrowIfNull(builderForValue, "builderForValue");
                this.PrepareBuilder();
                this.result.hasStructMaster = true;
                this.result.structMaster_ = builderForValue.Build();
                return this;
            }

            public SavedObject.Builder SetTakeDamage(objectTakeDamage value)
            {
                ThrowHelper.ThrowIfNull(value, "value");
                this.PrepareBuilder();
                this.result.hasTakeDamage = true;
                this.result.takeDamage_ = value;
                return this;
            }

            public SavedObject.Builder SetTakeDamage(objectTakeDamage.Builder builderForValue)
            {
                ThrowHelper.ThrowIfNull(builderForValue, "builderForValue");
                this.PrepareBuilder();
                this.result.hasTakeDamage = true;
                this.result.takeDamage_ = builderForValue.Build();
                return this;
            }

            public objectICarriableTrans CarriableTrans
            {
                get
                {
                    return this.result.CarriableTrans;
                }
                set
                {
                    this.SetCarriableTrans(value);
                }
            }

            public objectCoords Coords
            {
                get
                {
                    return this.result.Coords;
                }
                set
                {
                    this.SetCoords(value);
                }
            }

            public override SavedObject DefaultInstanceForType
            {
                get
                {
                    return SavedObject.DefaultInstance;
                }
            }

            public objectDeployable Deployable
            {
                get
                {
                    return this.result.Deployable;
                }
                set
                {
                    this.SetDeployable(value);
                }
            }

            public override MessageDescriptor DescriptorForType
            {
                get
                {
                    return SavedObject.Descriptor;
                }
            }

            public objectDoor Door
            {
                get
                {
                    return this.result.Door;
                }
                set
                {
                    this.SetDoor(value);
                }
            }

            public objectFireBarrel FireBarrel
            {
                get
                {
                    return this.result.FireBarrel;
                }
                set
                {
                    this.SetFireBarrel(value);
                }
            }

            public bool HasCarriableTrans
            {
                get
                {
                    return this.result.hasCarriableTrans;
                }
            }

            public bool HasCoords
            {
                get
                {
                    return this.result.hasCoords;
                }
            }

            public bool HasDeployable
            {
                get
                {
                    return this.result.hasDeployable;
                }
            }

            public bool HasDoor
            {
                get
                {
                    return this.result.hasDoor;
                }
            }

            public bool HasFireBarrel
            {
                get
                {
                    return this.result.hasFireBarrel;
                }
            }

            public bool HasId
            {
                get
                {
                    return this.result.hasId;
                }
            }

            public bool HasLockable
            {
                get
                {
                    return this.result.hasLockable;
                }
            }

            public bool HasNetInstance
            {
                get
                {
                    return this.result.hasNetInstance;
                }
            }

            public bool HasNgcInstance
            {
                get
                {
                    return this.result.hasNgcInstance;
                }
            }

            public bool HasSleepingAvatar
            {
                get
                {
                    return this.result.hasSleepingAvatar;
                }
            }

            public bool HasSortOrder
            {
                get
                {
                    return this.result.hasSortOrder;
                }
            }

            public bool HasStructComponent
            {
                get
                {
                    return this.result.hasStructComponent;
                }
            }

            public bool HasStructMaster
            {
                get
                {
                    return this.result.hasStructMaster;
                }
            }

            public bool HasTakeDamage
            {
                get
                {
                    return this.result.hasTakeDamage;
                }
            }

            public int Id
            {
                get
                {
                    return this.result.Id;
                }
                set
                {
                    this.SetId(value);
                }
            }

            public int InventoryCount
            {
                get
                {
                    return this.result.InventoryCount;
                }
            }

            public IPopsicleList<Item> InventoryList
            {
                get
                {
                    return this.PrepareBuilder().inventory_;
                }
            }

            public override bool IsInitialized
            {
                get
                {
                    return this.result.IsInitialized;
                }
            }

            public objectLockable Lockable
            {
                get
                {
                    return this.result.Lockable;
                }
                set
                {
                    this.SetLockable(value);
                }
            }

            protected override SavedObject MessageBeingBuilt
            {
                get
                {
                    return this.PrepareBuilder();
                }
            }

            public objectNetInstance NetInstance
            {
                get
                {
                    return this.result.NetInstance;
                }
                set
                {
                    this.SetNetInstance(value);
                }
            }

            public objectNGCInstance NgcInstance
            {
                get
                {
                    return this.result.NgcInstance;
                }
                set
                {
                    this.SetNgcInstance(value);
                }
            }

            public objectSleepingAvatar SleepingAvatar
            {
                get
                {
                    return this.result.SleepingAvatar;
                }
                set
                {
                    this.SetSleepingAvatar(value);
                }
            }

            public int SortOrder
            {
                get
                {
                    return this.result.SortOrder;
                }
                set
                {
                    this.SetSortOrder(value);
                }
            }

            public objectStructComponent StructComponent
            {
                get
                {
                    return this.result.StructComponent;
                }
                set
                {
                    this.SetStructComponent(value);
                }
            }

            public objectStructMaster StructMaster
            {
                get
                {
                    return this.result.StructMaster;
                }
                set
                {
                    this.SetStructMaster(value);
                }
            }

            public objectTakeDamage TakeDamage
            {
                get
                {
                    return this.result.TakeDamage;
                }
                set
                {
                    this.SetTakeDamage(value);
                }
            }

            protected override SavedObject.Builder ThisBuilder
            {
                get
                {
                    return this;
                }
            }
        }
    }
}

