using System;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

public class ControllerClass : ScriptableObject
{
    [SerializeField]
    private string _npcName = string.Empty;
    [SerializeField]
    private ControllerClassesConfigurations classNames;
    private const Configuration kDriver_DependantVessel = Configuration.DynamicDependantVessel;
    private const Configuration kDriver_FreeVessel = Configuration.DynamicFreeVessel;
    private const Configuration kDriver_Root = Configuration.DynamicRoot;
    private const Configuration kDriver_StandaloneVessel = Configuration.DynamicStandaloneVessel;
    private const Configuration kDriverMask = Configuration.DynamicFreeVessel;
    private const Configuration kStatic_Dynamic = Configuration.DynamicRoot;
    private const Configuration kStatic_Static = Configuration.StaticRoot;
    private const Configuration kStaticMask = Configuration.StaticRoot;
    [SerializeField]
    private Configuration runtime;

    internal bool DefinesClass(bool player)
    {
        string className = this.GetClassName(player, false);
        if (className == null)
        {
        }
        return !object.ReferenceEquals(this.GetClassName(player, true), null);
    }

    internal bool DefinesClass(bool player, bool local)
    {
        return !object.ReferenceEquals(this.GetClassName(player, local), null);
    }

    internal string GetClassName(bool player, bool local)
    {
        return ((this.classNames != null) ? this.classNames.GetClassName(player, local) : null);
    }

    internal bool GetClassName(bool player, bool local, out string className)
    {
        string str;
        className = str = this.GetClassName(player, local);
        return !object.ReferenceEquals(str, null);
    }

    internal string npcName
    {
        get
        {
            return (!string.IsNullOrEmpty(this._npcName) ? this._npcName : base.name);
        }
    }

    internal bool root
    {
        get
        {
            return ((this.runtime & Configuration.DynamicFreeVessel) == Configuration.DynamicRoot);
        }
    }

    internal bool staticGroup
    {
        get
        {
            return ((this.runtime & Configuration.StaticRoot) == Configuration.StaticRoot);
        }
    }

    internal string unassignedClassName
    {
        get
        {
            return this.classNames.unassignedClassName;
        }
    }

    internal bool vessel
    {
        get
        {
            return ((this.runtime & Configuration.DynamicFreeVessel) != Configuration.DynamicRoot);
        }
    }

    internal bool vesselDependant
    {
        get
        {
            return ((this.runtime & Configuration.DynamicFreeVessel) == Configuration.DynamicDependantVessel);
        }
    }

    internal bool vesselFree
    {
        get
        {
            return ((this.runtime & Configuration.DynamicFreeVessel) == Configuration.DynamicFreeVessel);
        }
    }

    internal bool vesselStandalone
    {
        get
        {
            return ((this.runtime & Configuration.DynamicFreeVessel) == Configuration.DynamicStandaloneVessel);
        }
    }

    public enum Configuration
    {
        DynamicRoot,
        DynamicStandaloneVessel,
        DynamicDependantVessel,
        DynamicFreeVessel,
        StaticRoot,
        StaticStandaloneVessel,
        StaticDependantVessel,
        StaticFreeVessel
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Merge
    {
        private int length;
        private int hash;
        private Instance first;
        private Instance[] classes;
        public bool Add(ControllerClass @class)
        {
            Instance instance;
            if (@class == null)
            {
                return false;
            }
            instance.hash = @class.GetHashCode();
            instance.value = @class;
            if (this.length == 1)
            {
                if ((this.hash == instance.hash) && object.ReferenceEquals(this.first.value, instance.value))
                {
                    return false;
                }
            }
            else if ((this.length > 1) && ((this.hash & instance.hash) == instance.hash))
            {
                for (int i = 0; i < this.length; i++)
                {
                    if ((this.classes[i].hash == this.hash) && object.ReferenceEquals(this.classes[i].value, instance.value))
                    {
                        return false;
                    }
                }
            }
            this.hash |= instance.hash;
            int index = this.length++;
            switch (index)
            {
                case 0:
                    this.first = instance;
                    break;

                case 1:
                    this.classes = new Instance[] { this.first, instance };
                    this.first.hash = 0;
                    this.first.value = null;
                    break;

                default:
                    Array.Resize<Instance>(ref this.classes, this.length);
                    this.classes[index] = instance;
                    break;
            }
            return true;
        }

        public bool any
        {
            get
            {
                return (this.length > 0);
            }
        }
        public bool root
        {
            get
            {
                if (this.length <= 0)
                {
                    return false;
                }
                if (this.length == 1)
                {
                    return this.first.value.root;
                }
                for (int i = 0; i < this.length; i++)
                {
                    if (!this.classes[i].value.root)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        public bool vessel
        {
            get
            {
                if (this.length <= 0)
                {
                    return false;
                }
                if (this.length == 1)
                {
                    return this.first.value.vessel;
                }
                for (int i = 0; i < this.length; i++)
                {
                    if (!this.classes[i].value.vessel)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        public bool staticGroup
        {
            get
            {
                if (this.length <= 0)
                {
                    return false;
                }
                if (this.length == 1)
                {
                    return this.first.value.staticGroup;
                }
                for (int i = 0; i < this.length; i++)
                {
                    if (!this.classes[i].value.staticGroup)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        public bool vesselStandalone
        {
            get
            {
                if (this.length <= 0)
                {
                    return false;
                }
                if (this.length == 1)
                {
                    return this.first.value.vesselStandalone;
                }
                for (int i = 0; i < this.length; i++)
                {
                    if (!this.classes[i].value.vesselStandalone)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        public bool vesselDependant
        {
            get
            {
                if (this.length <= 0)
                {
                    return false;
                }
                if (this.length == 1)
                {
                    return this.first.value.vesselDependant;
                }
                for (int i = 0; i < this.length; i++)
                {
                    if (!this.classes[i].value.vesselDependant)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        public bool vesselFree
        {
            get
            {
                if (this.length <= 0)
                {
                    return false;
                }
                if (this.length == 1)
                {
                    return this.first.value.vesselFree;
                }
                for (int i = 0; i < this.length; i++)
                {
                    if (!this.classes[i].value.vesselFree)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        public bool this[bool player, bool local]
        {
            get
            {
                if (this.length <= 0)
                {
                    return false;
                }
                if (this.length == 1)
                {
                    return this.first.value.DefinesClass(player, local);
                }
                for (int i = 0; i < this.length; i++)
                {
                    if (!this.classes[i].value.DefinesClass(player, local))
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        public bool this[bool player]
        {
            get
            {
                if (this.length <= 0)
                {
                    return false;
                }
                if (this.length == 1)
                {
                    return this.first.value.DefinesClass(player);
                }
                for (int i = 0; i < this.length; i++)
                {
                    if (!this.classes[i].value.DefinesClass(player))
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        public bool multiple
        {
            get
            {
                return (this.length > 1);
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct Instance
        {
            public int hash;
            public ControllerClass value;
        }
    }
}

