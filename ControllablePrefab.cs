using Facepunch;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using uLink;

public class ControllablePrefab : CharacterPrefab
{
    private static Dictionary<string, sbyte> aiRootCompatibilityCache = new Dictionary<string, sbyte>();
    private const byte kVesselFlag_AICanControl = 0x10;
    private const byte kVesselFlag_Missing = 0x40;
    private const byte kVesselFlag_PlayerCanControl = 8;
    private const byte kVesselFlag_StaticGroup = 0x20;
    private const byte kVesselFlag_Vessel = 1;
    private const byte kVesselFlag_Vessel_Dependant = 5;
    private const byte kVesselFlag_Vessel_Free = 7;
    private const byte kVesselFlag_Vessel_Standalone = 3;
    private const byte kVesselKindMask = 7;
    private static readonly Type[] minimalRequiredIDLocals = new Type[] { typeof(Controllable) };
    private static Dictionary<string, byte> playerRootCompatibilityCache = new Dictionary<string, byte>();
    private static Dictionary<string, byte> vesselCompatibilityCache = new Dictionary<string, byte>();

    public ControllablePrefab() : this(typeof(Character), false, minimalRequiredIDLocals, false)
    {
    }

    protected ControllablePrefab(Type characterType) : this(characterType, true, null, false)
    {
    }

    protected ControllablePrefab(Type characterType, params Type[] idlocalRequired) : this(characterType, true, idlocalRequired, (idlocalRequired != null) && (idlocalRequired.Length > 0))
    {
    }

    private ControllablePrefab(Type characterType, bool typeCheck, Type[] requiredIDLocalTypes, bool mergeTypes) : base(characterType, !mergeTypes ? minimalRequiredIDLocals : CharacterPrefab.TypeArrayAppend(minimalRequiredIDLocals, requiredIDLocalTypes))
    {
    }

    public static void EnsurePrefabIsAIRootCompatible(string name, out bool staticGroup)
    {
        sbyte num;
        NetMainPrefab.EnsurePrefabName(name);
        if (!aiRootCompatibilityCache.TryGetValue(name, out num))
        {
            ControllablePrefab prefab = NetMainPrefab.Lookup<ControllablePrefab>(name);
            if (prefab == null)
            {
                num = 0;
            }
            else if (!prefab.aiRootComapatable)
            {
                num = 2;
            }
            else
            {
                num = !((Character) prefab.serverPrefab).controllable.classFlagsStaticGroup ? ((sbyte) 1) : ((sbyte) (-1));
            }
            aiRootCompatibilityCache[name] = num;
        }
        sbyte num2 = num;
        switch ((num2 + 1))
        {
            case 0:
                staticGroup = true;
                return;

            case 2:
                staticGroup = false;
                return;

            case 3:
                throw new NonAIRootControllableException(name);
        }
        throw new NonControllableException(name);
    }

    public static void EnsurePrefabIsPlayerRootCompatible(string name)
    {
        byte num;
        NetMainPrefab.EnsurePrefabName(name);
        if (!playerRootCompatibilityCache.TryGetValue(name, out num))
        {
            ControllablePrefab prefab = NetMainPrefab.Lookup<ControllablePrefab>(name);
            if (prefab == null)
            {
                num = 0;
            }
            else if (!prefab.playerRootComapatable)
            {
                num = 2;
            }
            else
            {
                num = 1;
            }
            playerRootCompatibilityCache[name] = num;
        }
        switch (num)
        {
            case 0:
                throw new NonControllableException(name);

            case 2:
                throw new NonPlayerRootControllableException(name);
        }
    }

    public static void EnsurePrefabIsVessel(string name, out VesselInfo vi)
    {
        byte vesselCompatibility = GetVesselCompatibility(name);
        if ((vesselCompatibility & 1) != 1)
        {
            if ((vesselCompatibility & 0x40) == 0x40)
            {
                throw new NonVesselControllableException(name);
            }
            throw new NonControllableException(name);
        }
        if ((vesselCompatibility & 0x18) == 0)
        {
            throw new NonControllableException("The vessel has not been marked for either ai and/or player control. not bothering to spawn it.");
        }
        vi = new VesselInfo(vesselCompatibility);
    }

    public static void EnsurePrefabIsVessel(string name, Controllable forControllable, out VesselInfo vi)
    {
        EnsurePrefabIsVessel(name, out vi);
        if ((forControllable != null) && forControllable.controlled)
        {
            if (forControllable.aiControlled)
            {
                if (!vi.supportsAI)
                {
                    throw new NonAIVesselControllableException(name);
                }
            }
            else if (forControllable.playerControlled && !vi.supportsPlayer)
            {
                throw new NonPlayerVesselControllableException(name);
            }
        }
    }

    private static byte GetVesselCompatibility(string name)
    {
        byte vesselCompatibility;
        NetMainPrefab.EnsurePrefabName(name);
        if (!vesselCompatibilityCache.TryGetValue(name, out vesselCompatibility))
        {
            ControllablePrefab prefab = NetMainPrefab.Lookup<ControllablePrefab>(name);
            if (prefab == null)
            {
                vesselCompatibility = 0;
            }
            else
            {
                vesselCompatibility = prefab.vesselCompatibility;
            }
            vesselCompatibilityCache[name] = vesselCompatibility;
        }
        return vesselCompatibility;
    }

    protected override void StandardInitialization(bool didAppend, IDRemote appended, NetInstance instance, NetworkView view, ref NetworkMessageInfo info)
    {
        Character idMain = (Character) instance.idMain;
        Controllable controllable = idMain.controllable;
        controllable.PrepareInstantiate(view, ref info);
        base.StandardInitialization(false, appended, instance, view, ref info);
        if (didAppend)
        {
            NetMainPrefab.IssueLocallyAppended(appended, instance.idMain);
        }
        controllable.OnInstantiated();
    }

    private bool aiRootComapatable
    {
        get
        {
            Controllable controllable = ((Character) base.serverPrefab).controllable;
            if (controllable == null)
            {
                return false;
            }
            if (!controllable.classFlagsRootControllable)
            {
                return false;
            }
            if (!controllable.classFlagsAISupport)
            {
                return false;
            }
            controllable = ((Character) base.proxyPrefab).controllable;
            if (controllable == null)
            {
                return false;
            }
            if (!controllable.classFlagsRootControllable)
            {
                return false;
            }
            if (!controllable.classFlagsAISupport)
            {
                return false;
            }
            return true;
        }
    }

    private ControllerClass.Merge mergedClasses
    {
        get
        {
            ControllerClass.Merge merge = new ControllerClass.Merge();
            Controllable.MergeClasses(base.serverPrefab, ref merge);
            Controllable.MergeClasses(base.proxyPrefab, ref merge);
            Controllable.MergeClasses(base.localPrefab, ref merge);
            return merge;
        }
    }

    private bool playerRootComapatable
    {
        get
        {
            Controllable controllable = ((Character) base.serverPrefab).controllable;
            if (controllable == null)
            {
                return false;
            }
            if (!controllable.classFlagsRootControllable)
            {
                return false;
            }
            if (!controllable.classFlagsPlayerSupport)
            {
                return false;
            }
            controllable = ((Character) base.proxyPrefab).controllable;
            if (controllable == null)
            {
                return false;
            }
            if (!controllable.classFlagsRootControllable)
            {
                return false;
            }
            if (!controllable.classFlagsPlayerSupport)
            {
                return false;
            }
            return true;
        }
    }

    private byte vesselCompatibility
    {
        get
        {
            byte num;
            ControllerClass.Merge mergedClasses = this.mergedClasses;
            if (!mergedClasses.any)
            {
                return 0;
            }
            if (!mergedClasses.vessel)
            {
                return 0x40;
            }
            if (mergedClasses.vesselFree)
            {
                num = 7;
            }
            else if (mergedClasses.vesselDependant)
            {
                num = 5;
            }
            else
            {
                if (!mergedClasses.vesselStandalone)
                {
                    throw new NotImplementedException();
                }
                num = 3;
            }
            if (mergedClasses[true])
            {
                num = (byte) (num | 8);
            }
            if (mergedClasses[false])
            {
                num = (byte) (num | 0x10);
            }
            if (mergedClasses.staticGroup)
            {
                num = (byte) (num | 0x20);
            }
            return num;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct VesselInfo
    {
        private byte data;
        internal VesselInfo(byte data)
        {
            this.data = data;
        }

        public bool staticGroup
        {
            get
            {
                return ((this.data & 0x20) == 0x20);
            }
        }
        public bool supportsAI
        {
            get
            {
                return ((this.data & 0x10) == 0x10);
            }
        }
        public bool supportsPlayer
        {
            get
            {
                return ((this.data & 8) == 8);
            }
        }
        public bool canBind
        {
            get
            {
                switch ((this.data & 7))
                {
                    case 0:
                        return false;

                    case 3:
                        return false;

                    case 5:
                        return true;

                    case 7:
                        return true;
                }
                throw new NotImplementedException();
            }
        }
        public bool mustBind
        {
            get
            {
                switch ((this.data & 7))
                {
                    case 0:
                        return false;

                    case 3:
                        return false;

                    case 5:
                        return true;

                    case 7:
                        return false;
                }
                throw new NotImplementedException();
            }
        }
        public bool bindless
        {
            get
            {
                switch ((this.data & 7))
                {
                    case 0:
                        return false;

                    case 3:
                        return true;

                    case 5:
                        return false;

                    case 7:
                        return true;
                }
                throw new NotImplementedException();
            }
        }
    }
}

