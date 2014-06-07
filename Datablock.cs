using Facepunch.Build;
using Facepunch.Hash;
using System;
using System.Runtime.InteropServices;
using uLink;
using UnityEngine;

[UniqueBundleScriptableObject]
public class Datablock : ScriptableObject, IComparable<Datablock>
{
    [SerializeField, HideInInspector]
    private int _uniqueID;

    public int CompareTo(Datablock other)
    {
        if (object.ReferenceEquals(other, this))
        {
            return 0;
        }
        if (other == null)
        {
            return -1;
        }
        int num = this._uniqueID.CompareTo(other._uniqueID);
        if (num == 0)
        {
            return base.name.CompareTo(other.name);
        }
        return num;
    }

    public override int GetHashCode()
    {
        return this._uniqueID;
    }

    public uint SecureHash()
    {
        return this.SecureHash(0);
    }

    public uint SecureHash(uint seed)
    {
        BitStream stream = new BitStream(true);
        try
        {
            this.SecureWriteMemberValues(stream);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
        }
        return MurmurHash2.UINT(stream.GetDataByteArray(), seed);
    }

    protected virtual void SecureWriteMemberValues(BitStream stream)
    {
        stream.WriteInt32(this._uniqueID);
    }

    public int uniqueID
    {
        get
        {
            return this._uniqueID;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Ident : IEquatable<Datablock.Ident>, IEquatable<Datablock>
    {
        private const byte TYPE_NULL = 0;
        private const byte TYPE_DATABLOCK = 1;
        private const byte TYPE_INVENTORY_ITEM = 2;
        private const byte TYPE_STRING = 3;
        private const byte TYPE_HASH = 4;
        private const int FLAG_UNCONFIRMED = 0x80;
        private const int MASK_TYPE = 0x7f;
        private const byte TYPE_STRING_UNCONFIRMED = 0x83;
        private const byte TYPE_HASH_UNCONFIRMED = 0x84;
        private const byte TYPE_INVENTORY_ITEM_UNCONFIRMED = 130;
        private const byte TYPE_DATABLOCK_UNCONFIRMED = 0x81;
        private readonly object refValue;
        private readonly int uid;
        private readonly byte type_f;
        private Ident(object refValue, int uniqueID, byte type_f)
        {
            this.refValue = refValue;
            this.uid = uniqueID;
            this.type_f = type_f;
        }

        private Ident(object referenceValue, bool isNull, byte type)
        {
            if (isNull)
            {
                this = new Datablock.Ident();
            }
            else
            {
                this.refValue = referenceValue;
                this.uid = 0;
                this.type_f = type;
            }
        }

        private Ident(object referenceValue, byte type) : this(referenceValue, !object.ReferenceEquals(referenceValue, null), type)
        {
        }

        private Ident(Datablock db) : this(db, (bool) db, 0x81)
        {
        }

        private Ident(InventoryItem item) : this(item, 130)
        {
        }

        private Ident(string name) : this(name, string.IsNullOrEmpty(name), 0x83)
        {
        }

        private Ident(int uniqueID)
        {
            this.refValue = null;
            this.type_f = 0x84;
            this.uid = uniqueID;
        }

        private void Confirm()
        {
            Datablock refValue;
            switch ((this.type_f & 0x7f))
            {
                case 1:
                    refValue = (Datablock) this.refValue;
                    break;

                case 2:
                    refValue = ((InventoryItem) this.refValue).datablock;
                    break;

                case 3:
                    refValue = DatablockDictionary.GetByName((string) this.refValue);
                    break;

                case 4:
                    refValue = DatablockDictionary.GetByUniqueID(this.uid);
                    break;

                default:
                    this = new Datablock.Ident();
                    return;
            }
            if (refValue != null)
            {
                this = new Datablock.Ident(refValue, refValue.uniqueID, 1);
            }
            else
            {
                this = new Datablock.Ident();
            }
        }

        public override int GetHashCode()
        {
            return this.uid;
        }

        public Datablock datablock
        {
            get
            {
                if ((this.type_f & 0x80) == 0x80)
                {
                    this.Confirm();
                }
                return (Datablock) this.refValue;
            }
        }
        public int uniqueID
        {
            get
            {
                if ((this.type_f & 0x80) == 0x80)
                {
                    this.Confirm();
                }
                return this.uid;
            }
        }
        public int? uniqueIDIfExists
        {
            get
            {
                if ((this.type_f & 0x80) == 0x80)
                {
                    this.Confirm();
                }
                if (this.type_f != 0)
                {
                    return new int?(this.uid);
                }
                return null;
            }
        }
        public bool exists
        {
            get
            {
                if ((this.type_f & 0x80) == 0x80)
                {
                    this.Confirm();
                }
                return ((this.type_f != 0) && ((bool) ((Datablock) this.refValue)));
            }
        }
        public string name
        {
            get
            {
                if ((this.type_f & 0x80) == 0x80)
                {
                    this.Confirm();
                }
                if (this.type_f == 1)
                {
                    Datablock refValue = (Datablock) this.refValue;
                    if (refValue != null)
                    {
                        return refValue.name;
                    }
                }
                return string.Empty;
            }
        }
        public bool Equals(Datablock.Ident other)
        {
            if ((this.type_f & 0x80) == 0x80)
            {
                this.Confirm();
            }
            if ((other.type_f & 0x80) == 0x80)
            {
                other.Confirm();
            }
            return object.Equals(this.refValue, other.refValue);
        }

        public bool Equals(Datablock datablock)
        {
            if ((this.type_f & 0x80) == 0x80)
            {
                this.Confirm();
            }
            return object.Equals(this.refValue, datablock);
        }

        public override bool Equals(object obj)
        {
            if (obj is Datablock.Ident)
            {
                return this.Equals((Datablock.Ident) obj);
            }
            return ((obj is Datablock) && this.Equals((Datablock) obj));
        }

        public override string ToString()
        {
            Datablock datablock;
            if ((this.type_f & 0x80) == 0x80)
            {
                this.Confirm();
            }
            return (((this.type_f != 0) && ((datablock = (Datablock) this.refValue) != null)) ? datablock.name : "null");
        }

        public bool GetDatablock(out Datablock datablock)
        {
            if ((this.type_f & 0x80) == 0x80)
            {
                this.Confirm();
            }
            if (this.type_f == 0)
            {
                datablock = null;
                return false;
            }
            datablock = (Datablock) this.refValue;
            return (bool) datablock;
        }

        public bool GetDatablock<TDatablock>(out TDatablock datablock) where TDatablock: Datablock
        {
            if ((this.type_f & 0x80) == 0x80)
            {
                this.Confirm();
            }
            if (this.type_f == 0)
            {
                datablock = null;
                return false;
            }
            datablock = ((Datablock) this.refValue) as TDatablock;
            return (TDatablock) datablock;
        }

        public Datablock GetDatablock()
        {
            if ((this.type_f & 0x80) == 0x80)
            {
                this.Confirm();
            }
            if (this.type_f == 0)
            {
                return null;
            }
            return (Datablock) this.refValue;
        }

        public Datablock GetDatablock<TDatablock>() where TDatablock: Datablock
        {
            if ((this.type_f & 0x80) == 0x80)
            {
                this.Confirm();
            }
            if (this.type_f == 0)
            {
                throw new MissingReferenceException("this identifier is not valid");
            }
            return (TDatablock) this.refValue;
        }

        public static implicit operator Datablock.Ident(string dbName)
        {
            return new Datablock.Ident(dbName);
        }

        public static implicit operator Datablock.Ident(int dbHash)
        {
            return new Datablock.Ident(dbHash);
        }

        public static implicit operator Datablock.Ident(uint dbHash)
        {
            return new Datablock.Ident((int) dbHash);
        }

        [Obsolete("Make sure your wanting to get a dbhash from a ushort here.")]
        public static implicit operator Datablock.Ident(ushort dbHash)
        {
            return new Datablock.Ident(dbHash);
        }

        [Obsolete("Make sure your wanting to get a dbhash from a short here.")]
        public static implicit operator Datablock.Ident(short dbHash)
        {
            return new Datablock.Ident(dbHash);
        }

        [Obsolete("Make sure your wanting to get a dbhash from a byte here.")]
        public static implicit operator Datablock.Ident(byte dbHash)
        {
            return new Datablock.Ident(dbHash);
        }

        [Obsolete("Make sure your wanting to get a dbhash from a sbyte here.")]
        public static implicit operator Datablock.Ident(sbyte dbHash)
        {
            return new Datablock.Ident((int) dbHash);
        }

        public static explicit operator Datablock.Ident(ulong dbHash)
        {
            uint num = (uint) dbHash;
            return new Datablock.Ident((int) num);
        }

        public static explicit operator Datablock.Ident(long dbHash)
        {
            return new Datablock.Ident((int) dbHash);
        }

        public static explicit operator Datablock.Ident(InventoryItem item)
        {
            return new Datablock.Ident(item);
        }

        public static explicit operator Datablock.Ident(Datablock db)
        {
            if (db != null)
            {
                return new Datablock.Ident(db, db.uniqueID, 1);
            }
            return new Datablock.Ident();
        }

        public static Datablock.Ident operator +(Datablock.Ident ident)
        {
            if ((ident.type_f & 0x80) == 0x80)
            {
                ident.Confirm();
            }
            return ident;
        }

        public static bool operator ==(Datablock.Ident ident, Datablock.Ident other)
        {
            return ident.Equals(other);
        }

        public static bool operator !=(Datablock.Ident ident, Datablock.Ident other)
        {
            return !ident.Equals(other);
        }

        public static bool operator ==(Datablock.Ident ident, Datablock other)
        {
            return ident.Equals(other);
        }

        public static bool operator !=(Datablock.Ident ident, Datablock other)
        {
            return !ident.Equals(other);
        }

        public static bool operator ==(Datablock.Ident ident, string other)
        {
            if (string.IsNullOrEmpty(other))
            {
                return !ident.exists;
            }
            return (ident.name == other);
        }

        public static bool operator !=(Datablock.Ident ident, string other)
        {
            if (string.IsNullOrEmpty(other))
            {
                return ident.exists;
            }
            return (ident.name != other);
        }

        public static bool operator ==(Datablock.Ident ident, int hash)
        {
            return (ident.uniqueIDIfExists == hash);
        }

        public static bool operator !=(Datablock.Ident ident, int hash)
        {
            return (ident.uniqueIDIfExists != hash);
        }

        public static bool operator ==(Datablock.Ident ident, uint hash)
        {
            return (ident.uniqueID == hash);
        }

        public static bool operator !=(Datablock.Ident ident, uint hash)
        {
            return (ident.uniqueID != hash);
        }

        public static bool operator ==(Datablock.Ident ident, ushort hash)
        {
            return (ident.uniqueIDIfExists == hash);
        }

        public static bool operator !=(Datablock.Ident ident, ushort hash)
        {
            return (ident.uniqueIDIfExists != hash);
        }

        public static bool operator ==(Datablock.Ident ident, short hash)
        {
            return (ident.uniqueID == hash);
        }

        public static bool operator !=(Datablock.Ident ident, short hash)
        {
            return (ident.uniqueID != hash);
        }

        public static bool operator ==(Datablock.Ident ident, byte hash)
        {
            return (ident.uniqueIDIfExists == hash);
        }

        public static bool operator !=(Datablock.Ident ident, byte hash)
        {
            return (ident.uniqueIDIfExists != hash);
        }

        public static bool operator ==(Datablock.Ident ident, sbyte hash)
        {
            return (ident.uniqueID == hash);
        }

        public static bool operator !=(Datablock.Ident ident, sbyte hash)
        {
            return (ident.uniqueID != hash);
        }

        public static bool operator true(Datablock.Ident ident)
        {
            return ident.exists;
        }

        public static bool operator false(Datablock.Ident ident)
        {
            return !ident.exists;
        }
    }
}

