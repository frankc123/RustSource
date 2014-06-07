using Facepunch;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using uLink;
using UnityEngine;

[AddComponentMenu("")]
public sealed class NGC : MonoBehaviour
{
    [NonSerialized]
    private bool added;
    [NonSerialized]
    private NetworkMessageInfo creation;
    [NonSerialized]
    internal ushort groupNumber;
    private const string kAddRPC = "A";
    private const string kCallRPC = "C";
    private const string kDeleteRPC = "D";
    private const string kPrefabIdentifier = "!Ng";
    private static bool log_nonexistant_ngc_errors;
    [NonSerialized]
    internal NGCInternalView networkView;
    [NonSerialized]
    internal NetworkViewID networkViewID;
    [NonSerialized]
    private readonly Dictionary<ushort, NGCView> views = new Dictionary<ushort, NGCView>();

    [RPC]
    internal void A(byte[] data, NetworkMessageInfo info)
    {
        NGCView view = this.Add(data, 0, data.Length, info);
        this.views[view.innerID] = view;
        try
        {
            view.PostInstantiate();
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
        }
    }

    private NGCView Add(byte[] data, int offset, int length, NetworkMessageInfo info)
    {
        Vector3 vector;
        Vector3 vector2;
        Prefab prefab;
        int startIndex = offset;
        int index = BitConverter.ToInt32(data, startIndex);
        startIndex += 4;
        ushort num3 = BitConverter.ToUInt16(data, startIndex);
        startIndex += 2;
        vector.x = BitConverter.ToSingle(data, startIndex);
        startIndex += 4;
        vector.y = BitConverter.ToSingle(data, startIndex);
        startIndex += 4;
        vector.z = BitConverter.ToSingle(data, startIndex);
        startIndex += 4;
        vector2.x = BitConverter.ToSingle(data, startIndex);
        startIndex += 4;
        vector2.y = BitConverter.ToSingle(data, startIndex);
        startIndex += 4;
        vector2.z = BitConverter.ToSingle(data, startIndex);
        startIndex += 4;
        Quaternion rotation = Quaternion.Euler(vector2);
        NGC.Prefab.Register.Find(index, out prefab);
        NGCView view = (NGCView) Object.Instantiate(prefab.prefab, vector, rotation);
        view.creation = info;
        view.innerID = num3;
        view.prefab = prefab;
        view.outer = this;
        view.spawnPosition = vector;
        view.spawnRotation = rotation;
        int num4 = offset + length;
        if (num4 == startIndex)
        {
            view.initialData = null;
        }
        else
        {
            byte[] buffer = new byte[num4 - startIndex];
            int num5 = 0;
            do
            {
                buffer[num5++] = data[startIndex++];
            }
            while (startIndex < num4);
            view.initialData = new BitStream(buffer, false);
        }
        view.install = new Prefab.Installation.Instance(prefab.installation);
        return view;
    }

    public static callf<P0>.Block BlockArgs<P0>(P0 p0)
    {
        callf<P0>.Block block;
        block.p0 = p0;
        return block;
    }

    public static callf<P0, P1>.Block BlockArgs<P0, P1>(P0 p0, P1 p1)
    {
        callf<P0, P1>.Block block;
        block.p0 = p0;
        block.p1 = p1;
        return block;
    }

    public static callf<P0, P1, P2>.Block BlockArgs<P0, P1, P2>(P0 p0, P1 p1, P2 p2)
    {
        callf<P0, P1, P2>.Block block;
        block.p0 = p0;
        block.p1 = p1;
        block.p2 = p2;
        return block;
    }

    public static callf<P0, P1, P2, P3>.Block BlockArgs<P0, P1, P2, P3>(P0 p0, P1 p1, P2 p2, P3 p3)
    {
        callf<P0, P1, P2, P3>.Block block;
        block.p0 = p0;
        block.p1 = p1;
        block.p2 = p2;
        block.p3 = p3;
        return block;
    }

    public static callf<P0, P1, P2, P3, P4>.Block BlockArgs<P0, P1, P2, P3, P4>(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4)
    {
        callf<P0, P1, P2, P3, P4>.Block block;
        block.p0 = p0;
        block.p1 = p1;
        block.p2 = p2;
        block.p3 = p3;
        block.p4 = p4;
        return block;
    }

    public static callf<P0, P1, P2, P3, P4, P5>.Block BlockArgs<P0, P1, P2, P3, P4, P5>(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    {
        callf<P0, P1, P2, P3, P4, P5>.Block block;
        block.p0 = p0;
        block.p1 = p1;
        block.p2 = p2;
        block.p3 = p3;
        block.p4 = p4;
        block.p5 = p5;
        return block;
    }

    public static callf<P0, P1, P2, P3, P4, P5, P6>.Block BlockArgs<P0, P1, P2, P3, P4, P5, P6>(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
    {
        callf<P0, P1, P2, P3, P4, P5, P6>.Block block;
        block.p0 = p0;
        block.p1 = p1;
        block.p2 = p2;
        block.p3 = p3;
        block.p4 = p4;
        block.p5 = p5;
        block.p6 = p6;
        return block;
    }

    public static callf<P0, P1, P2, P3, P4, P5, P6, P7>.Block BlockArgs<P0, P1, P2, P3, P4, P5, P6, P7>(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
    {
        callf<P0, P1, P2, P3, P4, P5, P6, P7>.Block block;
        block.p0 = p0;
        block.p1 = p1;
        block.p2 = p2;
        block.p3 = p3;
        block.p4 = p4;
        block.p5 = p5;
        block.p6 = p6;
        block.p7 = p7;
        return block;
    }

    public static callf<P0, P1, P2, P3, P4, P5, P6, P7, P8>.Block BlockArgs<P0, P1, P2, P3, P4, P5, P6, P7, P8>(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8)
    {
        callf<P0, P1, P2, P3, P4, P5, P6, P7, P8>.Block block;
        block.p0 = p0;
        block.p1 = p1;
        block.p2 = p2;
        block.p3 = p3;
        block.p4 = p4;
        block.p5 = p5;
        block.p6 = p6;
        block.p7 = p7;
        block.p8 = p8;
        return block;
    }

    public static callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>.Block BlockArgs<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9)
    {
        callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>.Block block;
        block.p0 = p0;
        block.p1 = p1;
        block.p2 = p2;
        block.p3 = p3;
        block.p4 = p4;
        block.p5 = p5;
        block.p6 = p6;
        block.p7 = p7;
        block.p8 = p8;
        block.p9 = p9;
        return block;
    }

    public static callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>.Block BlockArgs<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10)
    {
        callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>.Block block;
        block.p0 = p0;
        block.p1 = p1;
        block.p2 = p2;
        block.p3 = p3;
        block.p4 = p4;
        block.p5 = p5;
        block.p6 = p6;
        block.p7 = p7;
        block.p8 = p8;
        block.p9 = p9;
        block.p10 = p10;
        return block;
    }

    public static callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>.Block BlockArgs<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11)
    {
        callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>.Block block;
        block.p0 = p0;
        block.p1 = p1;
        block.p2 = p2;
        block.p3 = p3;
        block.p4 = p4;
        block.p5 = p5;
        block.p6 = p6;
        block.p7 = p7;
        block.p8 = p8;
        block.p9 = p9;
        block.p10 = p10;
        block.p11 = p11;
        return block;
    }

    [RPC]
    internal void C(byte[] data, NetworkMessageInfo info)
    {
        Procedure procedure = this.Message(data, 0, data.Length, info);
        if (!procedure.Call())
        {
            if (procedure.view != null)
            {
                object[] args = new object[] { procedure.view.prefab.installation.methods[procedure.message].method.Name, procedure.view.name, procedure.view.id, procedure.message };
                Debug.LogWarning(string.Format("Did not call rpc \"{0}\" for view \"{1}\" (entid:{2},msg:{3})", args), this);
            }
            else if (log_nonexistant_ngc_errors)
            {
                Debug.LogWarning(string.Format("Did not call rpc to non existant view# {0}. ( message id was {1} )", procedure.target, procedure.message), this);
            }
        }
    }

    private static RPCName CallRPCName(NetworkFlags? flags, NGCView view, int messageID)
    {
        return new RPCName(view, messageID, "C", !flags.HasValue ? view.prefab.DefaultNetworkFlags(messageID) : flags.Value);
    }

    private static RPCName CallRPCName(NetworkFlags? flags, NGCView view, int messageID, ref IEnumerable<NetworkPlayer> targets)
    {
        return CallRPCName(flags, view, messageID);
    }

    private static RPCName CallRPCName(NetworkFlags? flags, NGCView view, int messageID, ref NetworkPlayer target)
    {
        return CallRPCName(flags, view, messageID);
    }

    private static RPCName CallRPCName(NetworkFlags? flags, NGCView view, int messageID, ref RPCMode mode)
    {
        return CallRPCName(flags, view, messageID);
    }

    [RPC]
    internal void D(ushort id, NetworkMessageInfo info)
    {
        NGCView view = this.Delete(id, info);
        this.DestroyView(view, true, true);
    }

    private NGCView Delete(ushort id, NetworkMessageInfo info)
    {
        NGCView view = this.views[id];
        this.DestroyView(view, false, false);
        this.views.Remove(id);
        return view;
    }

    private static void DestroyNGC_NetworkView(NetworkView view)
    {
        NGC component = view.GetComponent<NGC>();
        component.PreDestroy();
        Object.Destroy(component);
        NetworkInstantiator.defaultDestroyer(view);
    }

    private void DestroyView(NGCView view, bool andGameObject, bool skipPreDestroy)
    {
        if (view != null)
        {
            if (andGameObject)
            {
                GameObject gameObject = view.gameObject;
                if (!skipPreDestroy)
                {
                    this.DestroyView(view, false, false);
                }
                Object.Destroy(gameObject);
            }
            else if (!skipPreDestroy)
            {
                try
                {
                    view.PreDestroy();
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
        }
    }

    public static NGCView Find(int id)
    {
        ushort num;
        ushort num2;
        NGC ngc;
        NGCView view;
        if (!UnpackID(id, out num, out num2))
        {
            return null;
        }
        if (!Global.byGroup.TryGetValue(num, out ngc))
        {
            return null;
        }
        ngc.views.TryGetValue(num2, out view);
        return view;
    }

    private static IExecuter FindExecuter(Type[] argumentTypes)
    {
        switch (argumentTypes.Length)
        {
            case 0:
                return (typeof(callf).GetProperty("Exec", BindingFlags.Public | BindingFlags.Static).GetValue(null, null) as IExecuter);

            case 1:
                return (typeof(callf).MakeGenericType(argumentTypes).GetProperty("Exec", BindingFlags.Public | BindingFlags.Static).GetValue(null, null) as IExecuter);

            case 2:
                return (typeof(callf).MakeGenericType(argumentTypes).GetProperty("Exec", BindingFlags.Public | BindingFlags.Static).GetValue(null, null) as IExecuter);

            case 3:
                return (typeof(callf).MakeGenericType(argumentTypes).GetProperty("Exec", BindingFlags.Public | BindingFlags.Static).GetValue(null, null) as IExecuter);

            case 4:
                return (typeof(callf).MakeGenericType(argumentTypes).GetProperty("Exec", BindingFlags.Public | BindingFlags.Static).GetValue(null, null) as IExecuter);

            case 5:
                return (typeof(callf).MakeGenericType(argumentTypes).GetProperty("Exec", BindingFlags.Public | BindingFlags.Static).GetValue(null, null) as IExecuter);

            case 6:
                return (typeof(callf).MakeGenericType(argumentTypes).GetProperty("Exec", BindingFlags.Public | BindingFlags.Static).GetValue(null, null) as IExecuter);

            case 7:
                return (typeof(callf).MakeGenericType(argumentTypes).GetProperty("Exec", BindingFlags.Public | BindingFlags.Static).GetValue(null, null) as IExecuter);

            case 8:
                return (typeof(callf).MakeGenericType(argumentTypes).GetProperty("Exec", BindingFlags.Public | BindingFlags.Static).GetValue(null, null) as IExecuter);

            case 9:
                return (typeof(callf).MakeGenericType(argumentTypes).GetProperty("Exec", BindingFlags.Public | BindingFlags.Static).GetValue(null, null) as IExecuter);

            case 10:
                return (typeof(callf).MakeGenericType(argumentTypes).GetProperty("Exec", BindingFlags.Public | BindingFlags.Static).GetValue(null, null) as IExecuter);

            case 11:
                return (typeof(callf).MakeGenericType(argumentTypes).GetProperty("Exec", BindingFlags.Public | BindingFlags.Static).GetValue(null, null) as IExecuter);

            case 12:
                return (typeof(callf).MakeGenericType(argumentTypes).GetProperty("Exec", BindingFlags.Public | BindingFlags.Static).GetValue(null, null) as IExecuter);
        }
        throw new ArgumentOutOfRangeException("argumentTypes.Length > {0}");
    }

    internal Dictionary<ushort, NGCView>.ValueCollection GetViews()
    {
        return this.views.Values;
    }

    [Obsolete("NO, Use net cull making sure the prefab name string you must use starts with ;", true)]
    public static Object Instantiate(Object obj)
    {
        return Object.Instantiate(obj);
    }

    [Obsolete("NO, Use net cull making sure the prefab name string you must use starts with ;", true)]
    public static Object Instantiate(Object obj, Vector3 position, Quaternion rotation)
    {
        return Object.Instantiate(obj, position, rotation);
    }

    private Procedure Message(int id_msg, byte[] args, int argByteSize, NetworkMessageInfo info)
    {
        return this.Message((id_msg >> 0x10) & 0xffff, id_msg & 0xffff, args, argByteSize, info);
    }

    private Procedure Message(byte[] data, int offset, int length, NetworkMessageInfo info)
    {
        int num4;
        byte[] buffer;
        int startIndex = offset;
        int num2 = BitConverter.ToInt32(data, startIndex);
        startIndex += 4;
        int num3 = offset + length;
        if (startIndex == num3)
        {
            buffer = null;
            num4 = 0;
        }
        else
        {
            num4 = num3 - startIndex;
            buffer = new byte[num4];
            int num5 = 0;
            do
            {
                buffer[num5++] = data[startIndex++];
            }
            while (startIndex < num3);
        }
        return this.Message(num2, buffer, num4, info);
    }

    private Procedure Message(int id, int msg, byte[] args, int argByteSize, NetworkMessageInfo info)
    {
        return new Procedure { outer = this, target = id, message = msg, data = args, dataLength = argByteSize, info = info };
    }

    internal void NGCViewRPC(IEnumerable<NetworkPlayer> targets, NGCView view, int messageID, byte[] arguments, int argumentsOffset, int argumentsLength)
    {
        this.ShootRPC(CallRPCName(null, view, messageID, ref targets), targets, this.RPCData(view.innerID, messageID, arguments, argumentsOffset, argumentsLength));
    }

    internal void NGCViewRPC(NetworkPlayer target, NGCView view, int messageID, byte[] arguments, int argumentsOffset, int argumentsLength)
    {
        this.ShootRPC(CallRPCName(null, view, messageID, ref target), target, this.RPCData(view.innerID, messageID, arguments, argumentsOffset, argumentsLength));
    }

    internal void NGCViewRPC(RPCMode mode, NGCView view, int messageID, byte[] arguments, int argumentsOffset, int argumentsLength)
    {
        this.ShootRPC(CallRPCName(null, view, messageID, ref mode), mode, this.RPCData(view.innerID, messageID, arguments, argumentsOffset, argumentsLength));
    }

    internal void NGCViewRPC(NetworkFlags flags, IEnumerable<NetworkPlayer> targets, NGCView view, int messageID, byte[] arguments, int argumentsOffset, int argumentsLength)
    {
        this.ShootRPC(CallRPCName(new NetworkFlags?(flags), view, messageID, ref targets), targets, this.RPCData(view.innerID, messageID, arguments, argumentsOffset, argumentsLength));
    }

    internal void NGCViewRPC(NetworkFlags flags, NetworkPlayer target, NGCView view, int messageID, byte[] arguments, int argumentsOffset, int argumentsLength)
    {
        this.ShootRPC(CallRPCName(new NetworkFlags?(flags), view, messageID, ref target), target, this.RPCData(view.innerID, messageID, arguments, argumentsOffset, argumentsLength));
    }

    internal void NGCViewRPC(NetworkFlags flags, RPCMode mode, NGCView view, int messageID, byte[] arguments, int argumentsOffset, int argumentsLength)
    {
        this.ShootRPC(CallRPCName(new NetworkFlags?(flags), view, messageID, ref mode), mode, this.RPCData(view.innerID, messageID, arguments, argumentsOffset, argumentsLength));
    }

    private void OnDestroy()
    {
        this.Release();
    }

    internal static int PackID(int groupNumber, int innerID)
    {
        if ((groupNumber >= 0) && (innerID > 0))
        {
            return (((groupNumber & 0xffff) << 0x10) | innerID);
        }
        return 0;
    }

    private void PreDestroy()
    {
        List<NGCView> list = new List<NGCView>(this.views.Values);
        foreach (NGCView view in list)
        {
            this.DestroyView(view, false, false);
        }
        foreach (NGCView view2 in list)
        {
            this.DestroyView(view2, true, true);
        }
    }

    public static void Register(NGCConfiguration configuration)
    {
        NetworkInstantiator.Add("!Ng", new NetworkInstantiator.Creator(NGC.SpawnNGC_NetworkView), new NetworkInstantiator.Destroyer(NGC.DestroyNGC_NetworkView));
        configuration.Install();
    }

    private void Release()
    {
        if (this.added)
        {
            if (Global.all.Remove(this))
            {
                Global.byGroup.Remove(this.groupNumber);
            }
            this.added = false;
        }
    }

    private byte[] RPCData(int viewID, int messageID, byte[] arguments, int argumentsOffset, int argumentsLength)
    {
        byte[] bytes = BitConverter.GetBytes((int) ((viewID << 0x10) | (messageID & 0xffff)));
        byte[] buffer = new byte[bytes.Length + argumentsLength];
        int num = 0;
        for (int i = 0; i < bytes.Length; i++)
        {
            buffer[num++] = bytes[i];
        }
        int index = argumentsOffset;
        int num4 = 0;
        while (num4 < argumentsLength)
        {
            buffer[num++] = arguments[index];
            num4++;
            index++;
        }
        return buffer;
    }

    private void ShootRPC(RPCName rpc, NetworkPlayer target, byte[] data)
    {
        if (rpc.flags == NetworkFlags.Normal)
        {
            this.networkView.RPC<byte[]>(rpc.name, target, data);
        }
        else
        {
            this.networkView.RPC<byte[]>(rpc.flags, rpc.name, target, data);
        }
    }

    private void ShootRPC(RPCName rpc, RPCMode mode, byte[] data)
    {
        if (rpc.flags == NetworkFlags.Normal)
        {
            this.networkView.RPC<byte[]>(rpc.name, mode, data);
        }
        else
        {
            this.networkView.RPC<byte[]>(rpc.flags, rpc.name, mode, data);
        }
    }

    private void ShootRPC(RPCName rpc, IEnumerable<NetworkPlayer> targets, byte[] data)
    {
        if (rpc.flags == NetworkFlags.Normal)
        {
            this.networkView.RPC<byte[]>(rpc.name, targets, data);
        }
        else
        {
            this.networkView.RPC<byte[]>(rpc.flags, rpc.name, targets, data);
        }
    }

    private static NetworkView SpawnNGC_NetworkView(string prefabName, NetworkInstantiateArgs args, NetworkMessageInfo info)
    {
        NetworkInstantiatorUtility.AutoSetupNetworkViewOnAwake(args);
        Type[] components = new Type[] { typeof(NGC), typeof(NGCInternalView) };
        GameObject obj2 = new GameObject(string.Format("__NGC-{0:X}", args.group), components) {
            hideFlags = HideFlags.HideInHierarchy
        };
        NetworkInstantiatorUtility.ClearAutoSetupNetworkViewOnAwake();
        uLinkNetworkView component = obj2.GetComponent<uLinkNetworkView>();
        NGC ngc = obj2.GetComponent<NGC>();
        component.observed = ngc;
        component.rpcReceiver = RPCReceiver.OnlyObservedComponent;
        component.stateSynchronization = NetworkStateSynchronization.Off;
        NetworkMessageInfo info2 = new NetworkMessageInfo(info, component);
        ngc.uLink_OnNetworkInstantiate(info2);
        return component;
    }

    private void uLink_OnNetworkInstantiate(NetworkMessageInfo info)
    {
        NGC ngc;
        if (Global.byGroup.TryGetValue(this.groupNumber, out ngc))
        {
            if (ngc == this)
            {
                return;
            }
            if (ngc != null)
            {
                ngc.Release();
            }
        }
        Global.all.Add(this);
        this.groupNumber = (ushort) this.networkView.group.id;
        Global.byGroup[this.groupNumber] = this;
        this.added = true;
        this.creation = info;
    }

    internal static bool UnpackID(int packed, out ushort groupNumber, out ushort innerID)
    {
        if (packed == 0)
        {
            groupNumber = 0;
            innerID = 0;
            return false;
        }
        groupNumber = (ushort) ((packed >> 0x10) & 0xffff);
        innerID = (ushort) (packed & 0xffff);
        return true;
    }

    public static class callf
    {
        public static void InvokeCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(Call), instance, method, true);
            }
            ((Call) d)();
        }

        public static void InvokeInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(InfoCall), instance, method, true);
            }
            ((InfoCall) d)(info);
        }

        public static IEnumerator InvokeInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(InfoRoutine), instance, method, true);
            }
            return ((InfoRoutine) d)(info);
        }

        public static IEnumerator InvokeRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(Routine), instance, method, true);
            }
            return ((Routine) d)();
        }

        public static void InvokeStreamCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamCall), instance, method, true);
            }
            ((StreamCall) d)(stream);
        }

        public static void InvokeStreamInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamInfoCall), instance, method, true);
            }
            ((StreamInfoCall) d)(info, stream);
        }

        public static IEnumerator InvokeStreamInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamInfoRoutine), instance, method, true);
            }
            return ((StreamInfoRoutine) d)(info, stream);
        }

        public static IEnumerator InvokeStreamRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamRoutine), instance, method, true);
            }
            return ((StreamRoutine) d)(stream);
        }

        public static NGC.IExecuter Exec
        {
            get
            {
                return Executer.Singleton;
            }
        }

        public delegate void Call();

        private sealed class Executer : NGC.IExecuter
        {
            public static readonly NGC.IExecuter Singleton = new NGC.callf.Executer();

            public void ExecuteCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                NGC.callf.InvokeCall(stream, ref d, method, instance);
            }

            public void ExecuteInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                NGC.callf.InvokeInfoCall(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                return NGC.callf.InvokeInfoRoutine(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                return NGC.callf.InvokeRoutine(stream, ref d, method, instance);
            }

            public void ExecuteStreamCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                NGC.callf.InvokeStreamCall(stream, ref d, method, instance);
            }

            public void ExecuteStreamInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                NGC.callf.InvokeStreamInfoCall(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteStreamInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                return NGC.callf.InvokeStreamInfoRoutine(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteStreamRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                return NGC.callf.InvokeStreamRoutine(stream, ref d, method, instance);
            }
        }

        public delegate void InfoCall(NetworkMessageInfo info);

        public delegate IEnumerator InfoRoutine(NetworkMessageInfo info);

        public delegate IEnumerator Routine();

        public delegate void StreamCall(BitStream stream);

        public delegate void StreamInfoCall(NetworkMessageInfo info, BitStream stream);

        public delegate IEnumerator StreamInfoRoutine(NetworkMessageInfo info, BitStream stream);

        public delegate IEnumerator StreamRoutine(BitStream stream);
    }

    public static class callf<P0>
    {
        static callf()
        {
            BitStreamCodec.Add<Block<P0>>(new uLink.BitStreamCodec.Deserializer(NGC.callf<P0>.Deserializer), new uLink.BitStreamCodec.Serializer(NGC.callf<P0>.Serializer));
        }

        private static object Deserializer(BitStream stream, params object[] codecOptions)
        {
            Block<P0> block;
            block.p0 = stream.Read<P0>(codecOptions);
            return block;
        }

        public static void InvokeCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            P0 local = stream.Read<P0>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(Call<P0>), instance, method, true);
            }
            ((Call<P0>) d)(local);
        }

        public static void InvokeInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            P0 local = stream.Read<P0>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(InfoCall<P0>), instance, method, true);
            }
            ((InfoCall<P0>) d)(local, info);
        }

        public static IEnumerator InvokeInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            P0 local = stream.Read<P0>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(InfoRoutine<P0>), instance, method, true);
            }
            return ((InfoRoutine<P0>) d)(local, info);
        }

        public static IEnumerator InvokeRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            P0 local = stream.Read<P0>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(Routine<P0>), instance, method, true);
            }
            return ((Routine<P0>) d)(local);
        }

        public static void InvokeStreamCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            P0 local = stream.Read<P0>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamCall<P0>), instance, method, true);
            }
            ((StreamCall<P0>) d)(local, stream);
        }

        public static void InvokeStreamInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            P0 local = stream.Read<P0>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamInfoCall<P0>), instance, method, true);
            }
            ((StreamInfoCall<P0>) d)(local, info, stream);
        }

        public static IEnumerator InvokeStreamInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            P0 local = stream.Read<P0>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamInfoRoutine<P0>), instance, method, true);
            }
            return ((StreamInfoRoutine<P0>) d)(local, info, stream);
        }

        public static IEnumerator InvokeStreamRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            P0 local = stream.Read<P0>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamRoutine<P0>), instance, method, true);
            }
            return ((StreamRoutine<P0>) d)(local, stream);
        }

        private static void Serializer(BitStream stream, object value, params object[] codecOptions)
        {
            Block<P0> block = (Block<P0>) value;
            stream.Write<P0>(block.p0, codecOptions);
        }

        public static NGC.IExecuter Exec
        {
            get
            {
                return Executer<P0>.Singleton;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Block
        {
            public P0 p0;
        }

        public delegate void Call(P0 p0);

        private sealed class Executer : NGC.IExecuter
        {
            public static readonly NGC.IExecuter Singleton;

            static Executer()
            {
                NGC.callf<P0>.Executer.Singleton = new NGC.callf<P0>.Executer();
            }

            public void ExecuteCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                NGC.callf<P0>.InvokeCall(stream, ref d, method, instance);
            }

            public void ExecuteInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                NGC.callf<P0>.InvokeInfoCall(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                return NGC.callf<P0>.InvokeInfoRoutine(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                return NGC.callf<P0>.InvokeRoutine(stream, ref d, method, instance);
            }

            public void ExecuteStreamCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                NGC.callf<P0>.InvokeStreamCall(stream, ref d, method, instance);
            }

            public void ExecuteStreamInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                NGC.callf<P0>.InvokeStreamInfoCall(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteStreamInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                return NGC.callf<P0>.InvokeStreamInfoRoutine(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteStreamRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                return NGC.callf<P0>.InvokeStreamRoutine(stream, ref d, method, instance);
            }
        }

        public delegate void InfoCall(P0 p0, NetworkMessageInfo info);

        public delegate IEnumerator InfoRoutine(P0 p0, NetworkMessageInfo info);

        public delegate IEnumerator Routine(P0 p0);

        public delegate void StreamCall(P0 p0, BitStream stream);

        public delegate void StreamInfoCall(P0 p0, NetworkMessageInfo info, BitStream stream);

        public delegate IEnumerator StreamInfoRoutine(P0 p0, NetworkMessageInfo info, BitStream stream);

        public delegate IEnumerator StreamRoutine(P0 p0, BitStream stream);
    }

    public static class callf<P0, P1>
    {
        static callf()
        {
            BitStreamCodec.Add<Block<P0, P1>>(new uLink.BitStreamCodec.Deserializer(NGC.callf<P0, P1>.Deserializer), new uLink.BitStreamCodec.Serializer(NGC.callf<P0, P1>.Serializer));
        }

        private static object Deserializer(BitStream stream, params object[] codecOptions)
        {
            Block<P0, P1> block;
            block.p0 = stream.Read<P0>(codecOptions);
            block.p1 = stream.Read<P1>(codecOptions);
            return block;
        }

        public static void InvokeCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(Call<P0, P1>), instance, method, true);
            }
            ((Call<P0, P1>) d)(local, local2);
        }

        public static void InvokeInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(InfoCall<P0, P1>), instance, method, true);
            }
            ((InfoCall<P0, P1>) d)(local, local2, info);
        }

        public static IEnumerator InvokeInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(InfoRoutine<P0, P1>), instance, method, true);
            }
            return ((InfoRoutine<P0, P1>) d)(local, local2, info);
        }

        public static IEnumerator InvokeRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(Routine<P0, P1>), instance, method, true);
            }
            return ((Routine<P0, P1>) d)(local, local2);
        }

        public static void InvokeStreamCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamCall<P0, P1>), instance, method, true);
            }
            ((StreamCall<P0, P1>) d)(local, local2, stream);
        }

        public static void InvokeStreamInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamInfoCall<P0, P1>), instance, method, true);
            }
            ((StreamInfoCall<P0, P1>) d)(local, local2, info, stream);
        }

        public static IEnumerator InvokeStreamInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamInfoRoutine<P0, P1>), instance, method, true);
            }
            return ((StreamInfoRoutine<P0, P1>) d)(local, local2, info, stream);
        }

        public static IEnumerator InvokeStreamRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamRoutine<P0, P1>), instance, method, true);
            }
            return ((StreamRoutine<P0, P1>) d)(local, local2, stream);
        }

        private static void Serializer(BitStream stream, object value, params object[] codecOptions)
        {
            Block<P0, P1> block = (Block<P0, P1>) value;
            stream.Write<P0>(block.p0, codecOptions);
            stream.Write<P1>(block.p1, codecOptions);
        }

        public static NGC.IExecuter Exec
        {
            get
            {
                return Executer<P0, P1>.Singleton;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Block
        {
            public P0 p0;
            public P1 p1;
        }

        public delegate void Call(P0 p0, P1 p1);

        private sealed class Executer : NGC.IExecuter
        {
            public static readonly NGC.IExecuter Singleton;

            static Executer()
            {
                NGC.callf<P0, P1>.Executer.Singleton = new NGC.callf<P0, P1>.Executer();
            }

            public void ExecuteCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                NGC.callf<P0, P1>.InvokeCall(stream, ref d, method, instance);
            }

            public void ExecuteInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                NGC.callf<P0, P1>.InvokeInfoCall(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                return NGC.callf<P0, P1>.InvokeInfoRoutine(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                return NGC.callf<P0, P1>.InvokeRoutine(stream, ref d, method, instance);
            }

            public void ExecuteStreamCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                NGC.callf<P0, P1>.InvokeStreamCall(stream, ref d, method, instance);
            }

            public void ExecuteStreamInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                NGC.callf<P0, P1>.InvokeStreamInfoCall(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteStreamInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                return NGC.callf<P0, P1>.InvokeStreamInfoRoutine(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteStreamRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                return NGC.callf<P0, P1>.InvokeStreamRoutine(stream, ref d, method, instance);
            }
        }

        public delegate void InfoCall(P0 p0, P1 p1, NetworkMessageInfo info);

        public delegate IEnumerator InfoRoutine(P0 p0, P1 p1, NetworkMessageInfo info);

        public delegate IEnumerator Routine(P0 p0, P1 p1);

        public delegate void StreamCall(P0 p0, P1 p1, BitStream stream);

        public delegate void StreamInfoCall(P0 p0, P1 p1, NetworkMessageInfo info, BitStream stream);

        public delegate IEnumerator StreamInfoRoutine(P0 p0, P1 p1, NetworkMessageInfo info, BitStream stream);

        public delegate IEnumerator StreamRoutine(P0 p0, P1 p1, BitStream stream);
    }

    public static class callf<P0, P1, P2>
    {
        static callf()
        {
            BitStreamCodec.Add<Block<P0, P1, P2>>(new uLink.BitStreamCodec.Deserializer(NGC.callf<P0, P1, P2>.Deserializer), new uLink.BitStreamCodec.Serializer(NGC.callf<P0, P1, P2>.Serializer));
        }

        private static object Deserializer(BitStream stream, params object[] codecOptions)
        {
            Block<P0, P1, P2> block;
            block.p0 = stream.Read<P0>(codecOptions);
            block.p1 = stream.Read<P1>(codecOptions);
            block.p2 = stream.Read<P2>(codecOptions);
            return block;
        }

        public static void InvokeCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(Call<P0, P1, P2>), instance, method, true);
            }
            ((Call<P0, P1, P2>) d)(local, local2, local3);
        }

        public static void InvokeInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(InfoCall<P0, P1, P2>), instance, method, true);
            }
            ((InfoCall<P0, P1, P2>) d)(local, local2, local3, info);
        }

        public static IEnumerator InvokeInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(InfoRoutine<P0, P1, P2>), instance, method, true);
            }
            return ((InfoRoutine<P0, P1, P2>) d)(local, local2, local3, info);
        }

        public static IEnumerator InvokeRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(Routine<P0, P1, P2>), instance, method, true);
            }
            return ((Routine<P0, P1, P2>) d)(local, local2, local3);
        }

        public static void InvokeStreamCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamCall<P0, P1, P2>), instance, method, true);
            }
            ((StreamCall<P0, P1, P2>) d)(local, local2, local3, stream);
        }

        public static void InvokeStreamInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamInfoCall<P0, P1, P2>), instance, method, true);
            }
            ((StreamInfoCall<P0, P1, P2>) d)(local, local2, local3, info, stream);
        }

        public static IEnumerator InvokeStreamInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamInfoRoutine<P0, P1, P2>), instance, method, true);
            }
            return ((StreamInfoRoutine<P0, P1, P2>) d)(local, local2, local3, info, stream);
        }

        public static IEnumerator InvokeStreamRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamRoutine<P0, P1, P2>), instance, method, true);
            }
            return ((StreamRoutine<P0, P1, P2>) d)(local, local2, local3, stream);
        }

        private static void Serializer(BitStream stream, object value, params object[] codecOptions)
        {
            Block<P0, P1, P2> block = (Block<P0, P1, P2>) value;
            stream.Write<P0>(block.p0, codecOptions);
            stream.Write<P1>(block.p1, codecOptions);
            stream.Write<P2>(block.p2, codecOptions);
        }

        public static NGC.IExecuter Exec
        {
            get
            {
                return Executer<P0, P1, P2>.Singleton;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Block
        {
            public P0 p0;
            public P1 p1;
            public P2 p2;
        }

        public delegate void Call(P0 p0, P1 p1, P2 p2);

        private sealed class Executer : NGC.IExecuter
        {
            public static readonly NGC.IExecuter Singleton;

            static Executer()
            {
                NGC.callf<P0, P1, P2>.Executer.Singleton = new NGC.callf<P0, P1, P2>.Executer();
            }

            public void ExecuteCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                NGC.callf<P0, P1, P2>.InvokeCall(stream, ref d, method, instance);
            }

            public void ExecuteInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                NGC.callf<P0, P1, P2>.InvokeInfoCall(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                return NGC.callf<P0, P1, P2>.InvokeInfoRoutine(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                return NGC.callf<P0, P1, P2>.InvokeRoutine(stream, ref d, method, instance);
            }

            public void ExecuteStreamCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                NGC.callf<P0, P1, P2>.InvokeStreamCall(stream, ref d, method, instance);
            }

            public void ExecuteStreamInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                NGC.callf<P0, P1, P2>.InvokeStreamInfoCall(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteStreamInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                return NGC.callf<P0, P1, P2>.InvokeStreamInfoRoutine(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteStreamRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                return NGC.callf<P0, P1, P2>.InvokeStreamRoutine(stream, ref d, method, instance);
            }
        }

        public delegate void InfoCall(P0 p0, P1 p1, P2 p2, NetworkMessageInfo info);

        public delegate IEnumerator InfoRoutine(P0 p0, P1 p1, P2 p2, NetworkMessageInfo info);

        public delegate IEnumerator Routine(P0 p0, P1 p1, P2 p2);

        public delegate void StreamCall(P0 p0, P1 p1, P2 p2, BitStream stream);

        public delegate void StreamInfoCall(P0 p0, P1 p1, P2 p2, NetworkMessageInfo info, BitStream stream);

        public delegate IEnumerator StreamInfoRoutine(P0 p0, P1 p1, P2 p2, NetworkMessageInfo info, BitStream stream);

        public delegate IEnumerator StreamRoutine(P0 p0, P1 p1, P2 p2, BitStream stream);
    }

    public static class callf<P0, P1, P2, P3>
    {
        static callf()
        {
            BitStreamCodec.Add<Block<P0, P1, P2, P3>>(new uLink.BitStreamCodec.Deserializer(NGC.callf<P0, P1, P2, P3>.Deserializer), new uLink.BitStreamCodec.Serializer(NGC.callf<P0, P1, P2, P3>.Serializer));
        }

        private static object Deserializer(BitStream stream, params object[] codecOptions)
        {
            Block<P0, P1, P2, P3> block;
            block.p0 = stream.Read<P0>(codecOptions);
            block.p1 = stream.Read<P1>(codecOptions);
            block.p2 = stream.Read<P2>(codecOptions);
            block.p3 = stream.Read<P3>(codecOptions);
            return block;
        }

        public static void InvokeCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(Call<P0, P1, P2, P3>), instance, method, true);
            }
            ((Call<P0, P1, P2, P3>) d)(local, local2, local3, local4);
        }

        public static void InvokeInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(InfoCall<P0, P1, P2, P3>), instance, method, true);
            }
            ((InfoCall<P0, P1, P2, P3>) d)(local, local2, local3, local4, info);
        }

        public static IEnumerator InvokeInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(InfoRoutine<P0, P1, P2, P3>), instance, method, true);
            }
            return ((InfoRoutine<P0, P1, P2, P3>) d)(local, local2, local3, local4, info);
        }

        public static IEnumerator InvokeRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(Routine<P0, P1, P2, P3>), instance, method, true);
            }
            return ((Routine<P0, P1, P2, P3>) d)(local, local2, local3, local4);
        }

        public static void InvokeStreamCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamCall<P0, P1, P2, P3>), instance, method, true);
            }
            ((StreamCall<P0, P1, P2, P3>) d)(local, local2, local3, local4, stream);
        }

        public static void InvokeStreamInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamInfoCall<P0, P1, P2, P3>), instance, method, true);
            }
            ((StreamInfoCall<P0, P1, P2, P3>) d)(local, local2, local3, local4, info, stream);
        }

        public static IEnumerator InvokeStreamInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamInfoRoutine<P0, P1, P2, P3>), instance, method, true);
            }
            return ((StreamInfoRoutine<P0, P1, P2, P3>) d)(local, local2, local3, local4, info, stream);
        }

        public static IEnumerator InvokeStreamRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamRoutine<P0, P1, P2, P3>), instance, method, true);
            }
            return ((StreamRoutine<P0, P1, P2, P3>) d)(local, local2, local3, local4, stream);
        }

        private static void Serializer(BitStream stream, object value, params object[] codecOptions)
        {
            Block<P0, P1, P2, P3> block = (Block<P0, P1, P2, P3>) value;
            stream.Write<P0>(block.p0, codecOptions);
            stream.Write<P1>(block.p1, codecOptions);
            stream.Write<P2>(block.p2, codecOptions);
            stream.Write<P3>(block.p3, codecOptions);
        }

        public static NGC.IExecuter Exec
        {
            get
            {
                return Executer<P0, P1, P2, P3>.Singleton;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Block
        {
            public P0 p0;
            public P1 p1;
            public P2 p2;
            public P3 p3;
        }

        public delegate void Call(P0 p0, P1 p1, P2 p2, P3 p3);

        private sealed class Executer : NGC.IExecuter
        {
            public static readonly NGC.IExecuter Singleton;

            static Executer()
            {
                NGC.callf<P0, P1, P2, P3>.Executer.Singleton = new NGC.callf<P0, P1, P2, P3>.Executer();
            }

            public void ExecuteCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                NGC.callf<P0, P1, P2, P3>.InvokeCall(stream, ref d, method, instance);
            }

            public void ExecuteInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                NGC.callf<P0, P1, P2, P3>.InvokeInfoCall(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                return NGC.callf<P0, P1, P2, P3>.InvokeInfoRoutine(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                return NGC.callf<P0, P1, P2, P3>.InvokeRoutine(stream, ref d, method, instance);
            }

            public void ExecuteStreamCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                NGC.callf<P0, P1, P2, P3>.InvokeStreamCall(stream, ref d, method, instance);
            }

            public void ExecuteStreamInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                NGC.callf<P0, P1, P2, P3>.InvokeStreamInfoCall(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteStreamInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                return NGC.callf<P0, P1, P2, P3>.InvokeStreamInfoRoutine(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteStreamRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                return NGC.callf<P0, P1, P2, P3>.InvokeStreamRoutine(stream, ref d, method, instance);
            }
        }

        public delegate void InfoCall(P0 p0, P1 p1, P2 p2, P3 p3, NetworkMessageInfo info);

        public delegate IEnumerator InfoRoutine(P0 p0, P1 p1, P2 p2, P3 p3, NetworkMessageInfo info);

        public delegate IEnumerator Routine(P0 p0, P1 p1, P2 p2, P3 p3);

        public delegate void StreamCall(P0 p0, P1 p1, P2 p2, P3 p3, BitStream stream);

        public delegate void StreamInfoCall(P0 p0, P1 p1, P2 p2, P3 p3, NetworkMessageInfo info, BitStream stream);

        public delegate IEnumerator StreamInfoRoutine(P0 p0, P1 p1, P2 p2, P3 p3, NetworkMessageInfo info, BitStream stream);

        public delegate IEnumerator StreamRoutine(P0 p0, P1 p1, P2 p2, P3 p3, BitStream stream);
    }

    public static class callf<P0, P1, P2, P3, P4>
    {
        static callf()
        {
            BitStreamCodec.Add<Block<P0, P1, P2, P3, P4>>(new uLink.BitStreamCodec.Deserializer(NGC.callf<P0, P1, P2, P3, P4>.Deserializer), new uLink.BitStreamCodec.Serializer(NGC.callf<P0, P1, P2, P3, P4>.Serializer));
        }

        private static object Deserializer(BitStream stream, params object[] codecOptions)
        {
            Block<P0, P1, P2, P3, P4> block;
            block.p0 = stream.Read<P0>(codecOptions);
            block.p1 = stream.Read<P1>(codecOptions);
            block.p2 = stream.Read<P2>(codecOptions);
            block.p3 = stream.Read<P3>(codecOptions);
            block.p4 = stream.Read<P4>(codecOptions);
            return block;
        }

        public static void InvokeCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(Call<P0, P1, P2, P3, P4>), instance, method, true);
            }
            ((Call<P0, P1, P2, P3, P4>) d)(local, local2, local3, local4, local5);
        }

        public static void InvokeInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(InfoCall<P0, P1, P2, P3, P4>), instance, method, true);
            }
            ((InfoCall<P0, P1, P2, P3, P4>) d)(local, local2, local3, local4, local5, info);
        }

        public static IEnumerator InvokeInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(InfoRoutine<P0, P1, P2, P3, P4>), instance, method, true);
            }
            return ((InfoRoutine<P0, P1, P2, P3, P4>) d)(local, local2, local3, local4, local5, info);
        }

        public static IEnumerator InvokeRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(Routine<P0, P1, P2, P3, P4>), instance, method, true);
            }
            return ((Routine<P0, P1, P2, P3, P4>) d)(local, local2, local3, local4, local5);
        }

        public static void InvokeStreamCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamCall<P0, P1, P2, P3, P4>), instance, method, true);
            }
            ((StreamCall<P0, P1, P2, P3, P4>) d)(local, local2, local3, local4, local5, stream);
        }

        public static void InvokeStreamInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamInfoCall<P0, P1, P2, P3, P4>), instance, method, true);
            }
            ((StreamInfoCall<P0, P1, P2, P3, P4>) d)(local, local2, local3, local4, local5, info, stream);
        }

        public static IEnumerator InvokeStreamInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamInfoRoutine<P0, P1, P2, P3, P4>), instance, method, true);
            }
            return ((StreamInfoRoutine<P0, P1, P2, P3, P4>) d)(local, local2, local3, local4, local5, info, stream);
        }

        public static IEnumerator InvokeStreamRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamRoutine<P0, P1, P2, P3, P4>), instance, method, true);
            }
            return ((StreamRoutine<P0, P1, P2, P3, P4>) d)(local, local2, local3, local4, local5, stream);
        }

        private static void Serializer(BitStream stream, object value, params object[] codecOptions)
        {
            Block<P0, P1, P2, P3, P4> block = (Block<P0, P1, P2, P3, P4>) value;
            stream.Write<P0>(block.p0, codecOptions);
            stream.Write<P1>(block.p1, codecOptions);
            stream.Write<P2>(block.p2, codecOptions);
            stream.Write<P3>(block.p3, codecOptions);
            stream.Write<P4>(block.p4, codecOptions);
        }

        public static NGC.IExecuter Exec
        {
            get
            {
                return Executer<P0, P1, P2, P3, P4>.Singleton;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Block
        {
            public P0 p0;
            public P1 p1;
            public P2 p2;
            public P3 p3;
            public P4 p4;
        }

        public delegate void Call(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4);

        private sealed class Executer : NGC.IExecuter
        {
            public static readonly NGC.IExecuter Singleton;

            static Executer()
            {
                NGC.callf<P0, P1, P2, P3, P4>.Executer.Singleton = new NGC.callf<P0, P1, P2, P3, P4>.Executer();
            }

            public void ExecuteCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                NGC.callf<P0, P1, P2, P3, P4>.InvokeCall(stream, ref d, method, instance);
            }

            public void ExecuteInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                NGC.callf<P0, P1, P2, P3, P4>.InvokeInfoCall(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                return NGC.callf<P0, P1, P2, P3, P4>.InvokeInfoRoutine(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                return NGC.callf<P0, P1, P2, P3, P4>.InvokeRoutine(stream, ref d, method, instance);
            }

            public void ExecuteStreamCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                NGC.callf<P0, P1, P2, P3, P4>.InvokeStreamCall(stream, ref d, method, instance);
            }

            public void ExecuteStreamInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                NGC.callf<P0, P1, P2, P3, P4>.InvokeStreamInfoCall(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteStreamInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                return NGC.callf<P0, P1, P2, P3, P4>.InvokeStreamInfoRoutine(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteStreamRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                return NGC.callf<P0, P1, P2, P3, P4>.InvokeStreamRoutine(stream, ref d, method, instance);
            }
        }

        public delegate void InfoCall(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, NetworkMessageInfo info);

        public delegate IEnumerator InfoRoutine(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, NetworkMessageInfo info);

        public delegate IEnumerator Routine(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4);

        public delegate void StreamCall(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, BitStream stream);

        public delegate void StreamInfoCall(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, NetworkMessageInfo info, BitStream stream);

        public delegate IEnumerator StreamInfoRoutine(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, NetworkMessageInfo info, BitStream stream);

        public delegate IEnumerator StreamRoutine(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, BitStream stream);
    }

    public static class callf<P0, P1, P2, P3, P4, P5>
    {
        static callf()
        {
            BitStreamCodec.Add<Block<P0, P1, P2, P3, P4, P5>>(new uLink.BitStreamCodec.Deserializer(NGC.callf<P0, P1, P2, P3, P4, P5>.Deserializer), new uLink.BitStreamCodec.Serializer(NGC.callf<P0, P1, P2, P3, P4, P5>.Serializer));
        }

        private static object Deserializer(BitStream stream, params object[] codecOptions)
        {
            Block<P0, P1, P2, P3, P4, P5> block;
            block.p0 = stream.Read<P0>(codecOptions);
            block.p1 = stream.Read<P1>(codecOptions);
            block.p2 = stream.Read<P2>(codecOptions);
            block.p3 = stream.Read<P3>(codecOptions);
            block.p4 = stream.Read<P4>(codecOptions);
            block.p5 = stream.Read<P5>(codecOptions);
            return block;
        }

        public static void InvokeCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(Call<P0, P1, P2, P3, P4, P5>), instance, method, true);
            }
            ((Call<P0, P1, P2, P3, P4, P5>) d)(local, local2, local3, local4, local5, local6);
        }

        public static void InvokeInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(InfoCall<P0, P1, P2, P3, P4, P5>), instance, method, true);
            }
            ((InfoCall<P0, P1, P2, P3, P4, P5>) d)(local, local2, local3, local4, local5, local6, info);
        }

        public static IEnumerator InvokeInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(InfoRoutine<P0, P1, P2, P3, P4, P5>), instance, method, true);
            }
            return ((InfoRoutine<P0, P1, P2, P3, P4, P5>) d)(local, local2, local3, local4, local5, local6, info);
        }

        public static IEnumerator InvokeRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(Routine<P0, P1, P2, P3, P4, P5>), instance, method, true);
            }
            return ((Routine<P0, P1, P2, P3, P4, P5>) d)(local, local2, local3, local4, local5, local6);
        }

        public static void InvokeStreamCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamCall<P0, P1, P2, P3, P4, P5>), instance, method, true);
            }
            ((StreamCall<P0, P1, P2, P3, P4, P5>) d)(local, local2, local3, local4, local5, local6, stream);
        }

        public static void InvokeStreamInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamInfoCall<P0, P1, P2, P3, P4, P5>), instance, method, true);
            }
            ((StreamInfoCall<P0, P1, P2, P3, P4, P5>) d)(local, local2, local3, local4, local5, local6, info, stream);
        }

        public static IEnumerator InvokeStreamInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamInfoRoutine<P0, P1, P2, P3, P4, P5>), instance, method, true);
            }
            return ((StreamInfoRoutine<P0, P1, P2, P3, P4, P5>) d)(local, local2, local3, local4, local5, local6, info, stream);
        }

        public static IEnumerator InvokeStreamRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamRoutine<P0, P1, P2, P3, P4, P5>), instance, method, true);
            }
            return ((StreamRoutine<P0, P1, P2, P3, P4, P5>) d)(local, local2, local3, local4, local5, local6, stream);
        }

        private static void Serializer(BitStream stream, object value, params object[] codecOptions)
        {
            Block<P0, P1, P2, P3, P4, P5> block = (Block<P0, P1, P2, P3, P4, P5>) value;
            stream.Write<P0>(block.p0, codecOptions);
            stream.Write<P1>(block.p1, codecOptions);
            stream.Write<P2>(block.p2, codecOptions);
            stream.Write<P3>(block.p3, codecOptions);
            stream.Write<P4>(block.p4, codecOptions);
            stream.Write<P5>(block.p5, codecOptions);
        }

        public static NGC.IExecuter Exec
        {
            get
            {
                return Executer<P0, P1, P2, P3, P4, P5>.Singleton;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Block
        {
            public P0 p0;
            public P1 p1;
            public P2 p2;
            public P3 p3;
            public P4 p4;
            public P5 p5;
        }

        public delegate void Call(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5);

        private sealed class Executer : NGC.IExecuter
        {
            public static readonly NGC.IExecuter Singleton;

            static Executer()
            {
                NGC.callf<P0, P1, P2, P3, P4, P5>.Executer.Singleton = new NGC.callf<P0, P1, P2, P3, P4, P5>.Executer();
            }

            public void ExecuteCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                NGC.callf<P0, P1, P2, P3, P4, P5>.InvokeCall(stream, ref d, method, instance);
            }

            public void ExecuteInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                NGC.callf<P0, P1, P2, P3, P4, P5>.InvokeInfoCall(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                return NGC.callf<P0, P1, P2, P3, P4, P5>.InvokeInfoRoutine(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                return NGC.callf<P0, P1, P2, P3, P4, P5>.InvokeRoutine(stream, ref d, method, instance);
            }

            public void ExecuteStreamCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                NGC.callf<P0, P1, P2, P3, P4, P5>.InvokeStreamCall(stream, ref d, method, instance);
            }

            public void ExecuteStreamInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                NGC.callf<P0, P1, P2, P3, P4, P5>.InvokeStreamInfoCall(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteStreamInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                return NGC.callf<P0, P1, P2, P3, P4, P5>.InvokeStreamInfoRoutine(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteStreamRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                return NGC.callf<P0, P1, P2, P3, P4, P5>.InvokeStreamRoutine(stream, ref d, method, instance);
            }
        }

        public delegate void InfoCall(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, NetworkMessageInfo info);

        public delegate IEnumerator InfoRoutine(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, NetworkMessageInfo info);

        public delegate IEnumerator Routine(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5);

        public delegate void StreamCall(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, BitStream stream);

        public delegate void StreamInfoCall(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, NetworkMessageInfo info, BitStream stream);

        public delegate IEnumerator StreamInfoRoutine(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, NetworkMessageInfo info, BitStream stream);

        public delegate IEnumerator StreamRoutine(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, BitStream stream);
    }

    public static class callf<P0, P1, P2, P3, P4, P5, P6>
    {
        static callf()
        {
            BitStreamCodec.Add<Block<P0, P1, P2, P3, P4, P5, P6>>(new uLink.BitStreamCodec.Deserializer(NGC.callf<P0, P1, P2, P3, P4, P5, P6>.Deserializer), new uLink.BitStreamCodec.Serializer(NGC.callf<P0, P1, P2, P3, P4, P5, P6>.Serializer));
        }

        private static object Deserializer(BitStream stream, params object[] codecOptions)
        {
            Block<P0, P1, P2, P3, P4, P5, P6> block;
            block.p0 = stream.Read<P0>(codecOptions);
            block.p1 = stream.Read<P1>(codecOptions);
            block.p2 = stream.Read<P2>(codecOptions);
            block.p3 = stream.Read<P3>(codecOptions);
            block.p4 = stream.Read<P4>(codecOptions);
            block.p5 = stream.Read<P5>(codecOptions);
            block.p6 = stream.Read<P6>(codecOptions);
            return block;
        }

        public static void InvokeCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            P6 local7 = stream.Read<P6>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(Call<P0, P1, P2, P3, P4, P5, P6>), instance, method, true);
            }
            ((Call<P0, P1, P2, P3, P4, P5, P6>) d)(local, local2, local3, local4, local5, local6, local7);
        }

        public static void InvokeInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            P6 local7 = stream.Read<P6>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(InfoCall<P0, P1, P2, P3, P4, P5, P6>), instance, method, true);
            }
            ((InfoCall<P0, P1, P2, P3, P4, P5, P6>) d)(local, local2, local3, local4, local5, local6, local7, info);
        }

        public static IEnumerator InvokeInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            P6 local7 = stream.Read<P6>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(InfoRoutine<P0, P1, P2, P3, P4, P5, P6>), instance, method, true);
            }
            return ((InfoRoutine<P0, P1, P2, P3, P4, P5, P6>) d)(local, local2, local3, local4, local5, local6, local7, info);
        }

        public static IEnumerator InvokeRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            P6 local7 = stream.Read<P6>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(Routine<P0, P1, P2, P3, P4, P5, P6>), instance, method, true);
            }
            return ((Routine<P0, P1, P2, P3, P4, P5, P6>) d)(local, local2, local3, local4, local5, local6, local7);
        }

        public static void InvokeStreamCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            P6 local7 = stream.Read<P6>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamCall<P0, P1, P2, P3, P4, P5, P6>), instance, method, true);
            }
            ((StreamCall<P0, P1, P2, P3, P4, P5, P6>) d)(local, local2, local3, local4, local5, local6, local7, stream);
        }

        public static void InvokeStreamInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            P6 local7 = stream.Read<P6>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamInfoCall<P0, P1, P2, P3, P4, P5, P6>), instance, method, true);
            }
            ((StreamInfoCall<P0, P1, P2, P3, P4, P5, P6>) d)(local, local2, local3, local4, local5, local6, local7, info, stream);
        }

        public static IEnumerator InvokeStreamInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            P6 local7 = stream.Read<P6>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamInfoRoutine<P0, P1, P2, P3, P4, P5, P6>), instance, method, true);
            }
            return ((StreamInfoRoutine<P0, P1, P2, P3, P4, P5, P6>) d)(local, local2, local3, local4, local5, local6, local7, info, stream);
        }

        public static IEnumerator InvokeStreamRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            P6 local7 = stream.Read<P6>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamRoutine<P0, P1, P2, P3, P4, P5, P6>), instance, method, true);
            }
            return ((StreamRoutine<P0, P1, P2, P3, P4, P5, P6>) d)(local, local2, local3, local4, local5, local6, local7, stream);
        }

        private static void Serializer(BitStream stream, object value, params object[] codecOptions)
        {
            Block<P0, P1, P2, P3, P4, P5, P6> block = (Block<P0, P1, P2, P3, P4, P5, P6>) value;
            stream.Write<P0>(block.p0, codecOptions);
            stream.Write<P1>(block.p1, codecOptions);
            stream.Write<P2>(block.p2, codecOptions);
            stream.Write<P3>(block.p3, codecOptions);
            stream.Write<P4>(block.p4, codecOptions);
            stream.Write<P5>(block.p5, codecOptions);
            stream.Write<P6>(block.p6, codecOptions);
        }

        public static NGC.IExecuter Exec
        {
            get
            {
                return Executer<P0, P1, P2, P3, P4, P5, P6>.Singleton;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Block
        {
            public P0 p0;
            public P1 p1;
            public P2 p2;
            public P3 p3;
            public P4 p4;
            public P5 p5;
            public P6 p6;
        }

        public delegate void Call(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6);

        private sealed class Executer : NGC.IExecuter
        {
            public static readonly NGC.IExecuter Singleton;

            static Executer()
            {
                NGC.callf<P0, P1, P2, P3, P4, P5, P6>.Executer.Singleton = new NGC.callf<P0, P1, P2, P3, P4, P5, P6>.Executer();
            }

            public void ExecuteCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                NGC.callf<P0, P1, P2, P3, P4, P5, P6>.InvokeCall(stream, ref d, method, instance);
            }

            public void ExecuteInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                NGC.callf<P0, P1, P2, P3, P4, P5, P6>.InvokeInfoCall(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                return NGC.callf<P0, P1, P2, P3, P4, P5, P6>.InvokeInfoRoutine(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                return NGC.callf<P0, P1, P2, P3, P4, P5, P6>.InvokeRoutine(stream, ref d, method, instance);
            }

            public void ExecuteStreamCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                NGC.callf<P0, P1, P2, P3, P4, P5, P6>.InvokeStreamCall(stream, ref d, method, instance);
            }

            public void ExecuteStreamInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                NGC.callf<P0, P1, P2, P3, P4, P5, P6>.InvokeStreamInfoCall(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteStreamInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                return NGC.callf<P0, P1, P2, P3, P4, P5, P6>.InvokeStreamInfoRoutine(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteStreamRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                return NGC.callf<P0, P1, P2, P3, P4, P5, P6>.InvokeStreamRoutine(stream, ref d, method, instance);
            }
        }

        public delegate void InfoCall(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, NetworkMessageInfo info);

        public delegate IEnumerator InfoRoutine(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, NetworkMessageInfo info);

        public delegate IEnumerator Routine(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6);

        public delegate void StreamCall(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, BitStream stream);

        public delegate void StreamInfoCall(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, NetworkMessageInfo info, BitStream stream);

        public delegate IEnumerator StreamInfoRoutine(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, NetworkMessageInfo info, BitStream stream);

        public delegate IEnumerator StreamRoutine(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, BitStream stream);
    }

    public static class callf<P0, P1, P2, P3, P4, P5, P6, P7>
    {
        static callf()
        {
            BitStreamCodec.Add<Block<P0, P1, P2, P3, P4, P5, P6, P7>>(new uLink.BitStreamCodec.Deserializer(NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7>.Deserializer), new uLink.BitStreamCodec.Serializer(NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7>.Serializer));
        }

        private static object Deserializer(BitStream stream, params object[] codecOptions)
        {
            Block<P0, P1, P2, P3, P4, P5, P6, P7> block;
            block.p0 = stream.Read<P0>(codecOptions);
            block.p1 = stream.Read<P1>(codecOptions);
            block.p2 = stream.Read<P2>(codecOptions);
            block.p3 = stream.Read<P3>(codecOptions);
            block.p4 = stream.Read<P4>(codecOptions);
            block.p5 = stream.Read<P5>(codecOptions);
            block.p6 = stream.Read<P6>(codecOptions);
            block.p7 = stream.Read<P7>(codecOptions);
            return block;
        }

        public static void InvokeCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            P6 local7 = stream.Read<P6>(new object[0]);
            P7 local8 = stream.Read<P7>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(Call<P0, P1, P2, P3, P4, P5, P6, P7>), instance, method, true);
            }
            ((Call<P0, P1, P2, P3, P4, P5, P6, P7>) d)(local, local2, local3, local4, local5, local6, local7, local8);
        }

        public static void InvokeInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            P6 local7 = stream.Read<P6>(new object[0]);
            P7 local8 = stream.Read<P7>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(InfoCall<P0, P1, P2, P3, P4, P5, P6, P7>), instance, method, true);
            }
            ((InfoCall<P0, P1, P2, P3, P4, P5, P6, P7>) d)(local, local2, local3, local4, local5, local6, local7, local8, info);
        }

        public static IEnumerator InvokeInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            P6 local7 = stream.Read<P6>(new object[0]);
            P7 local8 = stream.Read<P7>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(InfoRoutine<P0, P1, P2, P3, P4, P5, P6, P7>), instance, method, true);
            }
            return ((InfoRoutine<P0, P1, P2, P3, P4, P5, P6, P7>) d)(local, local2, local3, local4, local5, local6, local7, local8, info);
        }

        public static IEnumerator InvokeRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            P6 local7 = stream.Read<P6>(new object[0]);
            P7 local8 = stream.Read<P7>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(Routine<P0, P1, P2, P3, P4, P5, P6, P7>), instance, method, true);
            }
            return ((Routine<P0, P1, P2, P3, P4, P5, P6, P7>) d)(local, local2, local3, local4, local5, local6, local7, local8);
        }

        public static void InvokeStreamCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            P6 local7 = stream.Read<P6>(new object[0]);
            P7 local8 = stream.Read<P7>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamCall<P0, P1, P2, P3, P4, P5, P6, P7>), instance, method, true);
            }
            ((StreamCall<P0, P1, P2, P3, P4, P5, P6, P7>) d)(local, local2, local3, local4, local5, local6, local7, local8, stream);
        }

        public static void InvokeStreamInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            P6 local7 = stream.Read<P6>(new object[0]);
            P7 local8 = stream.Read<P7>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamInfoCall<P0, P1, P2, P3, P4, P5, P6, P7>), instance, method, true);
            }
            ((StreamInfoCall<P0, P1, P2, P3, P4, P5, P6, P7>) d)(local, local2, local3, local4, local5, local6, local7, local8, info, stream);
        }

        public static IEnumerator InvokeStreamInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            P6 local7 = stream.Read<P6>(new object[0]);
            P7 local8 = stream.Read<P7>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamInfoRoutine<P0, P1, P2, P3, P4, P5, P6, P7>), instance, method, true);
            }
            return ((StreamInfoRoutine<P0, P1, P2, P3, P4, P5, P6, P7>) d)(local, local2, local3, local4, local5, local6, local7, local8, info, stream);
        }

        public static IEnumerator InvokeStreamRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            P6 local7 = stream.Read<P6>(new object[0]);
            P7 local8 = stream.Read<P7>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamRoutine<P0, P1, P2, P3, P4, P5, P6, P7>), instance, method, true);
            }
            return ((StreamRoutine<P0, P1, P2, P3, P4, P5, P6, P7>) d)(local, local2, local3, local4, local5, local6, local7, local8, stream);
        }

        private static void Serializer(BitStream stream, object value, params object[] codecOptions)
        {
            Block<P0, P1, P2, P3, P4, P5, P6, P7> block = (Block<P0, P1, P2, P3, P4, P5, P6, P7>) value;
            stream.Write<P0>(block.p0, codecOptions);
            stream.Write<P1>(block.p1, codecOptions);
            stream.Write<P2>(block.p2, codecOptions);
            stream.Write<P3>(block.p3, codecOptions);
            stream.Write<P4>(block.p4, codecOptions);
            stream.Write<P5>(block.p5, codecOptions);
            stream.Write<P6>(block.p6, codecOptions);
            stream.Write<P7>(block.p7, codecOptions);
        }

        public static NGC.IExecuter Exec
        {
            get
            {
                return Executer<P0, P1, P2, P3, P4, P5, P6, P7>.Singleton;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Block
        {
            public P0 p0;
            public P1 p1;
            public P2 p2;
            public P3 p3;
            public P4 p4;
            public P5 p5;
            public P6 p6;
            public P7 p7;
        }

        public delegate void Call(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7);

        private sealed class Executer : NGC.IExecuter
        {
            public static readonly NGC.IExecuter Singleton;

            static Executer()
            {
                NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7>.Executer.Singleton = new NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7>.Executer();
            }

            public void ExecuteCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7>.InvokeCall(stream, ref d, method, instance);
            }

            public void ExecuteInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7>.InvokeInfoCall(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                return NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7>.InvokeInfoRoutine(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                return NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7>.InvokeRoutine(stream, ref d, method, instance);
            }

            public void ExecuteStreamCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7>.InvokeStreamCall(stream, ref d, method, instance);
            }

            public void ExecuteStreamInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7>.InvokeStreamInfoCall(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteStreamInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                return NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7>.InvokeStreamInfoRoutine(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteStreamRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                return NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7>.InvokeStreamRoutine(stream, ref d, method, instance);
            }
        }

        public delegate void InfoCall(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, NetworkMessageInfo info);

        public delegate IEnumerator InfoRoutine(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, NetworkMessageInfo info);

        public delegate IEnumerator Routine(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7);

        public delegate void StreamCall(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, BitStream stream);

        public delegate void StreamInfoCall(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, NetworkMessageInfo info, BitStream stream);

        public delegate IEnumerator StreamInfoRoutine(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, NetworkMessageInfo info, BitStream stream);

        public delegate IEnumerator StreamRoutine(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, BitStream stream);
    }

    public static class callf<P0, P1, P2, P3, P4, P5, P6, P7, P8>
    {
        static callf()
        {
            BitStreamCodec.Add<Block<P0, P1, P2, P3, P4, P5, P6, P7, P8>>(new uLink.BitStreamCodec.Deserializer(NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8>.Deserializer), new uLink.BitStreamCodec.Serializer(NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8>.Serializer));
        }

        private static object Deserializer(BitStream stream, params object[] codecOptions)
        {
            Block<P0, P1, P2, P3, P4, P5, P6, P7, P8> block;
            block.p0 = stream.Read<P0>(codecOptions);
            block.p1 = stream.Read<P1>(codecOptions);
            block.p2 = stream.Read<P2>(codecOptions);
            block.p3 = stream.Read<P3>(codecOptions);
            block.p4 = stream.Read<P4>(codecOptions);
            block.p5 = stream.Read<P5>(codecOptions);
            block.p6 = stream.Read<P6>(codecOptions);
            block.p7 = stream.Read<P7>(codecOptions);
            block.p8 = stream.Read<P8>(codecOptions);
            return block;
        }

        public static void InvokeCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            P6 local7 = stream.Read<P6>(new object[0]);
            P7 local8 = stream.Read<P7>(new object[0]);
            P8 local9 = stream.Read<P8>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(Call<P0, P1, P2, P3, P4, P5, P6, P7, P8>), instance, method, true);
            }
            ((Call<P0, P1, P2, P3, P4, P5, P6, P7, P8>) d)(local, local2, local3, local4, local5, local6, local7, local8, local9);
        }

        public static void InvokeInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            P6 local7 = stream.Read<P6>(new object[0]);
            P7 local8 = stream.Read<P7>(new object[0]);
            P8 local9 = stream.Read<P8>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(InfoCall<P0, P1, P2, P3, P4, P5, P6, P7, P8>), instance, method, true);
            }
            ((InfoCall<P0, P1, P2, P3, P4, P5, P6, P7, P8>) d)(local, local2, local3, local4, local5, local6, local7, local8, local9, info);
        }

        public static IEnumerator InvokeInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            P6 local7 = stream.Read<P6>(new object[0]);
            P7 local8 = stream.Read<P7>(new object[0]);
            P8 local9 = stream.Read<P8>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(InfoRoutine<P0, P1, P2, P3, P4, P5, P6, P7, P8>), instance, method, true);
            }
            return ((InfoRoutine<P0, P1, P2, P3, P4, P5, P6, P7, P8>) d)(local, local2, local3, local4, local5, local6, local7, local8, local9, info);
        }

        public static IEnumerator InvokeRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            P6 local7 = stream.Read<P6>(new object[0]);
            P7 local8 = stream.Read<P7>(new object[0]);
            P8 local9 = stream.Read<P8>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(Routine<P0, P1, P2, P3, P4, P5, P6, P7, P8>), instance, method, true);
            }
            return ((Routine<P0, P1, P2, P3, P4, P5, P6, P7, P8>) d)(local, local2, local3, local4, local5, local6, local7, local8, local9);
        }

        public static void InvokeStreamCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            P6 local7 = stream.Read<P6>(new object[0]);
            P7 local8 = stream.Read<P7>(new object[0]);
            P8 local9 = stream.Read<P8>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamCall<P0, P1, P2, P3, P4, P5, P6, P7, P8>), instance, method, true);
            }
            ((StreamCall<P0, P1, P2, P3, P4, P5, P6, P7, P8>) d)(local, local2, local3, local4, local5, local6, local7, local8, local9, stream);
        }

        public static void InvokeStreamInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            P6 local7 = stream.Read<P6>(new object[0]);
            P7 local8 = stream.Read<P7>(new object[0]);
            P8 local9 = stream.Read<P8>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamInfoCall<P0, P1, P2, P3, P4, P5, P6, P7, P8>), instance, method, true);
            }
            ((StreamInfoCall<P0, P1, P2, P3, P4, P5, P6, P7, P8>) d)(local, local2, local3, local4, local5, local6, local7, local8, local9, info, stream);
        }

        public static IEnumerator InvokeStreamInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            P6 local7 = stream.Read<P6>(new object[0]);
            P7 local8 = stream.Read<P7>(new object[0]);
            P8 local9 = stream.Read<P8>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamInfoRoutine<P0, P1, P2, P3, P4, P5, P6, P7, P8>), instance, method, true);
            }
            return ((StreamInfoRoutine<P0, P1, P2, P3, P4, P5, P6, P7, P8>) d)(local, local2, local3, local4, local5, local6, local7, local8, local9, info, stream);
        }

        public static IEnumerator InvokeStreamRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            P6 local7 = stream.Read<P6>(new object[0]);
            P7 local8 = stream.Read<P7>(new object[0]);
            P8 local9 = stream.Read<P8>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamRoutine<P0, P1, P2, P3, P4, P5, P6, P7, P8>), instance, method, true);
            }
            return ((StreamRoutine<P0, P1, P2, P3, P4, P5, P6, P7, P8>) d)(local, local2, local3, local4, local5, local6, local7, local8, local9, stream);
        }

        private static void Serializer(BitStream stream, object value, params object[] codecOptions)
        {
            Block<P0, P1, P2, P3, P4, P5, P6, P7, P8> block = (Block<P0, P1, P2, P3, P4, P5, P6, P7, P8>) value;
            stream.Write<P0>(block.p0, codecOptions);
            stream.Write<P1>(block.p1, codecOptions);
            stream.Write<P2>(block.p2, codecOptions);
            stream.Write<P3>(block.p3, codecOptions);
            stream.Write<P4>(block.p4, codecOptions);
            stream.Write<P5>(block.p5, codecOptions);
            stream.Write<P6>(block.p6, codecOptions);
            stream.Write<P7>(block.p7, codecOptions);
            stream.Write<P8>(block.p8, codecOptions);
        }

        public static NGC.IExecuter Exec
        {
            get
            {
                return Executer<P0, P1, P2, P3, P4, P5, P6, P7, P8>.Singleton;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Block
        {
            public P0 p0;
            public P1 p1;
            public P2 p2;
            public P3 p3;
            public P4 p4;
            public P5 p5;
            public P6 p6;
            public P7 p7;
            public P8 p8;
        }

        public delegate void Call(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8);

        private sealed class Executer : NGC.IExecuter
        {
            public static readonly NGC.IExecuter Singleton;

            static Executer()
            {
                NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8>.Executer.Singleton = new NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8>.Executer();
            }

            public void ExecuteCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8>.InvokeCall(stream, ref d, method, instance);
            }

            public void ExecuteInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8>.InvokeInfoCall(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                return NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8>.InvokeInfoRoutine(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                return NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8>.InvokeRoutine(stream, ref d, method, instance);
            }

            public void ExecuteStreamCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8>.InvokeStreamCall(stream, ref d, method, instance);
            }

            public void ExecuteStreamInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8>.InvokeStreamInfoCall(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteStreamInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                return NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8>.InvokeStreamInfoRoutine(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteStreamRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                return NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8>.InvokeStreamRoutine(stream, ref d, method, instance);
            }
        }

        public delegate void InfoCall(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, NetworkMessageInfo info);

        public delegate IEnumerator InfoRoutine(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, NetworkMessageInfo info);

        public delegate IEnumerator Routine(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8);

        public delegate void StreamCall(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, BitStream stream);

        public delegate void StreamInfoCall(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, NetworkMessageInfo info, BitStream stream);

        public delegate IEnumerator StreamInfoRoutine(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, NetworkMessageInfo info, BitStream stream);

        public delegate IEnumerator StreamRoutine(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, BitStream stream);
    }

    public static class callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>
    {
        static callf()
        {
            BitStreamCodec.Add<Block<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>>(new uLink.BitStreamCodec.Deserializer(NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>.Deserializer), new uLink.BitStreamCodec.Serializer(NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>.Serializer));
        }

        private static object Deserializer(BitStream stream, params object[] codecOptions)
        {
            Block<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9> block;
            block.p0 = stream.Read<P0>(codecOptions);
            block.p1 = stream.Read<P1>(codecOptions);
            block.p2 = stream.Read<P2>(codecOptions);
            block.p3 = stream.Read<P3>(codecOptions);
            block.p4 = stream.Read<P4>(codecOptions);
            block.p5 = stream.Read<P5>(codecOptions);
            block.p6 = stream.Read<P6>(codecOptions);
            block.p7 = stream.Read<P7>(codecOptions);
            block.p8 = stream.Read<P8>(codecOptions);
            block.p9 = stream.Read<P9>(codecOptions);
            return block;
        }

        public static void InvokeCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            P6 local7 = stream.Read<P6>(new object[0]);
            P7 local8 = stream.Read<P7>(new object[0]);
            P8 local9 = stream.Read<P8>(new object[0]);
            P9 local10 = stream.Read<P9>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(Call<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>), instance, method, true);
            }
            ((Call<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>) d)(local, local2, local3, local4, local5, local6, local7, local8, local9, local10);
        }

        public static void InvokeInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            P6 local7 = stream.Read<P6>(new object[0]);
            P7 local8 = stream.Read<P7>(new object[0]);
            P8 local9 = stream.Read<P8>(new object[0]);
            P9 local10 = stream.Read<P9>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(InfoCall<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>), instance, method, true);
            }
            ((InfoCall<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>) d)(local, local2, local3, local4, local5, local6, local7, local8, local9, local10, info);
        }

        public static IEnumerator InvokeInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            P6 local7 = stream.Read<P6>(new object[0]);
            P7 local8 = stream.Read<P7>(new object[0]);
            P8 local9 = stream.Read<P8>(new object[0]);
            P9 local10 = stream.Read<P9>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(InfoRoutine<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>), instance, method, true);
            }
            return ((InfoRoutine<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>) d)(local, local2, local3, local4, local5, local6, local7, local8, local9, local10, info);
        }

        public static IEnumerator InvokeRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            P6 local7 = stream.Read<P6>(new object[0]);
            P7 local8 = stream.Read<P7>(new object[0]);
            P8 local9 = stream.Read<P8>(new object[0]);
            P9 local10 = stream.Read<P9>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(Routine<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>), instance, method, true);
            }
            return ((Routine<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>) d)(local, local2, local3, local4, local5, local6, local7, local8, local9, local10);
        }

        public static void InvokeStreamCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            P6 local7 = stream.Read<P6>(new object[0]);
            P7 local8 = stream.Read<P7>(new object[0]);
            P8 local9 = stream.Read<P8>(new object[0]);
            P9 local10 = stream.Read<P9>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamCall<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>), instance, method, true);
            }
            ((StreamCall<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>) d)(local, local2, local3, local4, local5, local6, local7, local8, local9, local10, stream);
        }

        public static void InvokeStreamInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            P6 local7 = stream.Read<P6>(new object[0]);
            P7 local8 = stream.Read<P7>(new object[0]);
            P8 local9 = stream.Read<P8>(new object[0]);
            P9 local10 = stream.Read<P9>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamInfoCall<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>), instance, method, true);
            }
            ((StreamInfoCall<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>) d)(local, local2, local3, local4, local5, local6, local7, local8, local9, local10, info, stream);
        }

        public static IEnumerator InvokeStreamInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            P6 local7 = stream.Read<P6>(new object[0]);
            P7 local8 = stream.Read<P7>(new object[0]);
            P8 local9 = stream.Read<P8>(new object[0]);
            P9 local10 = stream.Read<P9>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamInfoRoutine<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>), instance, method, true);
            }
            return ((StreamInfoRoutine<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>) d)(local, local2, local3, local4, local5, local6, local7, local8, local9, local10, info, stream);
        }

        public static IEnumerator InvokeStreamRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            P6 local7 = stream.Read<P6>(new object[0]);
            P7 local8 = stream.Read<P7>(new object[0]);
            P8 local9 = stream.Read<P8>(new object[0]);
            P9 local10 = stream.Read<P9>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamRoutine<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>), instance, method, true);
            }
            return ((StreamRoutine<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>) d)(local, local2, local3, local4, local5, local6, local7, local8, local9, local10, stream);
        }

        private static void Serializer(BitStream stream, object value, params object[] codecOptions)
        {
            Block<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9> block = (Block<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>) value;
            stream.Write<P0>(block.p0, codecOptions);
            stream.Write<P1>(block.p1, codecOptions);
            stream.Write<P2>(block.p2, codecOptions);
            stream.Write<P3>(block.p3, codecOptions);
            stream.Write<P4>(block.p4, codecOptions);
            stream.Write<P5>(block.p5, codecOptions);
            stream.Write<P6>(block.p6, codecOptions);
            stream.Write<P7>(block.p7, codecOptions);
            stream.Write<P8>(block.p8, codecOptions);
            stream.Write<P9>(block.p9, codecOptions);
        }

        public static NGC.IExecuter Exec
        {
            get
            {
                return Executer<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>.Singleton;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Block
        {
            public P0 p0;
            public P1 p1;
            public P2 p2;
            public P3 p3;
            public P4 p4;
            public P5 p5;
            public P6 p6;
            public P7 p7;
            public P8 p8;
            public P9 p9;
        }

        public delegate void Call(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9);

        private sealed class Executer : NGC.IExecuter
        {
            public static readonly NGC.IExecuter Singleton;

            static Executer()
            {
                NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>.Executer.Singleton = new NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>.Executer();
            }

            public void ExecuteCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>.InvokeCall(stream, ref d, method, instance);
            }

            public void ExecuteInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>.InvokeInfoCall(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                return NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>.InvokeInfoRoutine(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                return NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>.InvokeRoutine(stream, ref d, method, instance);
            }

            public void ExecuteStreamCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>.InvokeStreamCall(stream, ref d, method, instance);
            }

            public void ExecuteStreamInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>.InvokeStreamInfoCall(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteStreamInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                return NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>.InvokeStreamInfoRoutine(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteStreamRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                return NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>.InvokeStreamRoutine(stream, ref d, method, instance);
            }
        }

        public delegate void InfoCall(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, NetworkMessageInfo info);

        public delegate IEnumerator InfoRoutine(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, NetworkMessageInfo info);

        public delegate IEnumerator Routine(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9);

        public delegate void StreamCall(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, BitStream stream);

        public delegate void StreamInfoCall(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, NetworkMessageInfo info, BitStream stream);

        public delegate IEnumerator StreamInfoRoutine(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, NetworkMessageInfo info, BitStream stream);

        public delegate IEnumerator StreamRoutine(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, BitStream stream);
    }

    public static class callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>
    {
        static callf()
        {
            BitStreamCodec.Add<Block<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>>(new uLink.BitStreamCodec.Deserializer(NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>.Deserializer), new uLink.BitStreamCodec.Serializer(NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>.Serializer));
        }

        private static object Deserializer(BitStream stream, params object[] codecOptions)
        {
            Block<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10> block;
            block.p0 = stream.Read<P0>(codecOptions);
            block.p1 = stream.Read<P1>(codecOptions);
            block.p2 = stream.Read<P2>(codecOptions);
            block.p3 = stream.Read<P3>(codecOptions);
            block.p4 = stream.Read<P4>(codecOptions);
            block.p5 = stream.Read<P5>(codecOptions);
            block.p6 = stream.Read<P6>(codecOptions);
            block.p7 = stream.Read<P7>(codecOptions);
            block.p8 = stream.Read<P8>(codecOptions);
            block.p9 = stream.Read<P9>(codecOptions);
            block.p10 = stream.Read<P10>(codecOptions);
            return block;
        }

        public static void InvokeCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            P6 local7 = stream.Read<P6>(new object[0]);
            P7 local8 = stream.Read<P7>(new object[0]);
            P8 local9 = stream.Read<P8>(new object[0]);
            P9 local10 = stream.Read<P9>(new object[0]);
            P10 local11 = stream.Read<P10>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(Call<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>), instance, method, true);
            }
            ((Call<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>) d)(local, local2, local3, local4, local5, local6, local7, local8, local9, local10, local11);
        }

        public static void InvokeInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            P6 local7 = stream.Read<P6>(new object[0]);
            P7 local8 = stream.Read<P7>(new object[0]);
            P8 local9 = stream.Read<P8>(new object[0]);
            P9 local10 = stream.Read<P9>(new object[0]);
            P10 local11 = stream.Read<P10>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(InfoCall<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>), instance, method, true);
            }
            ((InfoCall<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>) d)(local, local2, local3, local4, local5, local6, local7, local8, local9, local10, local11, info);
        }

        public static IEnumerator InvokeInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            P6 local7 = stream.Read<P6>(new object[0]);
            P7 local8 = stream.Read<P7>(new object[0]);
            P8 local9 = stream.Read<P8>(new object[0]);
            P9 local10 = stream.Read<P9>(new object[0]);
            P10 local11 = stream.Read<P10>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(InfoRoutine<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>), instance, method, true);
            }
            return ((InfoRoutine<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>) d)(local, local2, local3, local4, local5, local6, local7, local8, local9, local10, local11, info);
        }

        public static IEnumerator InvokeRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            P6 local7 = stream.Read<P6>(new object[0]);
            P7 local8 = stream.Read<P7>(new object[0]);
            P8 local9 = stream.Read<P8>(new object[0]);
            P9 local10 = stream.Read<P9>(new object[0]);
            P10 local11 = stream.Read<P10>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(Routine<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>), instance, method, true);
            }
            return ((Routine<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>) d)(local, local2, local3, local4, local5, local6, local7, local8, local9, local10, local11);
        }

        public static void InvokeStreamCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            P6 local7 = stream.Read<P6>(new object[0]);
            P7 local8 = stream.Read<P7>(new object[0]);
            P8 local9 = stream.Read<P8>(new object[0]);
            P9 local10 = stream.Read<P9>(new object[0]);
            P10 local11 = stream.Read<P10>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamCall<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>), instance, method, true);
            }
            ((StreamCall<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>) d)(local, local2, local3, local4, local5, local6, local7, local8, local9, local10, local11, stream);
        }

        public static void InvokeStreamInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            P6 local7 = stream.Read<P6>(new object[0]);
            P7 local8 = stream.Read<P7>(new object[0]);
            P8 local9 = stream.Read<P8>(new object[0]);
            P9 local10 = stream.Read<P9>(new object[0]);
            P10 local11 = stream.Read<P10>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamInfoCall<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>), instance, method, true);
            }
            ((StreamInfoCall<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>) d)(local, local2, local3, local4, local5, local6, local7, local8, local9, local10, local11, info, stream);
        }

        public static IEnumerator InvokeStreamInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            P6 local7 = stream.Read<P6>(new object[0]);
            P7 local8 = stream.Read<P7>(new object[0]);
            P8 local9 = stream.Read<P8>(new object[0]);
            P9 local10 = stream.Read<P9>(new object[0]);
            P10 local11 = stream.Read<P10>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamInfoRoutine<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>), instance, method, true);
            }
            return ((StreamInfoRoutine<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>) d)(local, local2, local3, local4, local5, local6, local7, local8, local9, local10, local11, info, stream);
        }

        public static IEnumerator InvokeStreamRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            P6 local7 = stream.Read<P6>(new object[0]);
            P7 local8 = stream.Read<P7>(new object[0]);
            P8 local9 = stream.Read<P8>(new object[0]);
            P9 local10 = stream.Read<P9>(new object[0]);
            P10 local11 = stream.Read<P10>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamRoutine<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>), instance, method, true);
            }
            return ((StreamRoutine<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>) d)(local, local2, local3, local4, local5, local6, local7, local8, local9, local10, local11, stream);
        }

        private static void Serializer(BitStream stream, object value, params object[] codecOptions)
        {
            Block<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10> block = (Block<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>) value;
            stream.Write<P0>(block.p0, codecOptions);
            stream.Write<P1>(block.p1, codecOptions);
            stream.Write<P2>(block.p2, codecOptions);
            stream.Write<P3>(block.p3, codecOptions);
            stream.Write<P4>(block.p4, codecOptions);
            stream.Write<P5>(block.p5, codecOptions);
            stream.Write<P6>(block.p6, codecOptions);
            stream.Write<P7>(block.p7, codecOptions);
            stream.Write<P8>(block.p8, codecOptions);
            stream.Write<P9>(block.p9, codecOptions);
            stream.Write<P10>(block.p10, codecOptions);
        }

        public static NGC.IExecuter Exec
        {
            get
            {
                return Executer<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>.Singleton;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Block
        {
            public P0 p0;
            public P1 p1;
            public P2 p2;
            public P3 p3;
            public P4 p4;
            public P5 p5;
            public P6 p6;
            public P7 p7;
            public P8 p8;
            public P9 p9;
            public P10 p10;
        }

        public delegate void Call(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10);

        private sealed class Executer : NGC.IExecuter
        {
            public static readonly NGC.IExecuter Singleton;

            static Executer()
            {
                NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>.Executer.Singleton = new NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>.Executer();
            }

            public void ExecuteCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>.InvokeCall(stream, ref d, method, instance);
            }

            public void ExecuteInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>.InvokeInfoCall(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                return NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>.InvokeInfoRoutine(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                return NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>.InvokeRoutine(stream, ref d, method, instance);
            }

            public void ExecuteStreamCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>.InvokeStreamCall(stream, ref d, method, instance);
            }

            public void ExecuteStreamInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>.InvokeStreamInfoCall(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteStreamInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                return NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>.InvokeStreamInfoRoutine(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteStreamRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                return NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>.InvokeStreamRoutine(stream, ref d, method, instance);
            }
        }

        public delegate void InfoCall(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, NetworkMessageInfo info);

        public delegate IEnumerator InfoRoutine(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, NetworkMessageInfo info);

        public delegate IEnumerator Routine(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10);

        public delegate void StreamCall(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, BitStream stream);

        public delegate void StreamInfoCall(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, NetworkMessageInfo info, BitStream stream);

        public delegate IEnumerator StreamInfoRoutine(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, NetworkMessageInfo info, BitStream stream);

        public delegate IEnumerator StreamRoutine(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, BitStream stream);
    }

    public static class callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>
    {
        static callf()
        {
            BitStreamCodec.Add<Block<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>>(new uLink.BitStreamCodec.Deserializer(NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>.Deserializer), new uLink.BitStreamCodec.Serializer(NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>.Serializer));
        }

        private static object Deserializer(BitStream stream, params object[] codecOptions)
        {
            Block<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11> block;
            block.p0 = stream.Read<P0>(codecOptions);
            block.p1 = stream.Read<P1>(codecOptions);
            block.p2 = stream.Read<P2>(codecOptions);
            block.p3 = stream.Read<P3>(codecOptions);
            block.p4 = stream.Read<P4>(codecOptions);
            block.p5 = stream.Read<P5>(codecOptions);
            block.p6 = stream.Read<P6>(codecOptions);
            block.p7 = stream.Read<P7>(codecOptions);
            block.p8 = stream.Read<P8>(codecOptions);
            block.p9 = stream.Read<P9>(codecOptions);
            block.p10 = stream.Read<P10>(codecOptions);
            block.p11 = stream.Read<P11>(codecOptions);
            return block;
        }

        public static void InvokeCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            P6 local7 = stream.Read<P6>(new object[0]);
            P7 local8 = stream.Read<P7>(new object[0]);
            P8 local9 = stream.Read<P8>(new object[0]);
            P9 local10 = stream.Read<P9>(new object[0]);
            P10 local11 = stream.Read<P10>(new object[0]);
            P11 local12 = stream.Read<P11>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(Call<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>), instance, method, true);
            }
            ((Call<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>) d)(local, local2, local3, local4, local5, local6, local7, local8, local9, local10, local11, local12);
        }

        public static void InvokeInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            P6 local7 = stream.Read<P6>(new object[0]);
            P7 local8 = stream.Read<P7>(new object[0]);
            P8 local9 = stream.Read<P8>(new object[0]);
            P9 local10 = stream.Read<P9>(new object[0]);
            P10 local11 = stream.Read<P10>(new object[0]);
            P11 local12 = stream.Read<P11>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(InfoCall<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>), instance, method, true);
            }
            ((InfoCall<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>) d)(local, local2, local3, local4, local5, local6, local7, local8, local9, local10, local11, local12, info);
        }

        public static IEnumerator InvokeInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            P6 local7 = stream.Read<P6>(new object[0]);
            P7 local8 = stream.Read<P7>(new object[0]);
            P8 local9 = stream.Read<P8>(new object[0]);
            P9 local10 = stream.Read<P9>(new object[0]);
            P10 local11 = stream.Read<P10>(new object[0]);
            P11 local12 = stream.Read<P11>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(InfoRoutine<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>), instance, method, true);
            }
            return ((InfoRoutine<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>) d)(local, local2, local3, local4, local5, local6, local7, local8, local9, local10, local11, local12, info);
        }

        public static IEnumerator InvokeRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            P6 local7 = stream.Read<P6>(new object[0]);
            P7 local8 = stream.Read<P7>(new object[0]);
            P8 local9 = stream.Read<P8>(new object[0]);
            P9 local10 = stream.Read<P9>(new object[0]);
            P10 local11 = stream.Read<P10>(new object[0]);
            P11 local12 = stream.Read<P11>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(Routine<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>), instance, method, true);
            }
            return ((Routine<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>) d)(local, local2, local3, local4, local5, local6, local7, local8, local9, local10, local11, local12);
        }

        public static void InvokeStreamCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            P6 local7 = stream.Read<P6>(new object[0]);
            P7 local8 = stream.Read<P7>(new object[0]);
            P8 local9 = stream.Read<P8>(new object[0]);
            P9 local10 = stream.Read<P9>(new object[0]);
            P10 local11 = stream.Read<P10>(new object[0]);
            P11 local12 = stream.Read<P11>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamCall<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>), instance, method, true);
            }
            ((StreamCall<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>) d)(local, local2, local3, local4, local5, local6, local7, local8, local9, local10, local11, local12, stream);
        }

        public static void InvokeStreamInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            P6 local7 = stream.Read<P6>(new object[0]);
            P7 local8 = stream.Read<P7>(new object[0]);
            P8 local9 = stream.Read<P8>(new object[0]);
            P9 local10 = stream.Read<P9>(new object[0]);
            P10 local11 = stream.Read<P10>(new object[0]);
            P11 local12 = stream.Read<P11>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamInfoCall<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>), instance, method, true);
            }
            ((StreamInfoCall<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>) d)(local, local2, local3, local4, local5, local6, local7, local8, local9, local10, local11, local12, info, stream);
        }

        public static IEnumerator InvokeStreamInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            P6 local7 = stream.Read<P6>(new object[0]);
            P7 local8 = stream.Read<P7>(new object[0]);
            P8 local9 = stream.Read<P8>(new object[0]);
            P9 local10 = stream.Read<P9>(new object[0]);
            P10 local11 = stream.Read<P10>(new object[0]);
            P11 local12 = stream.Read<P11>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamInfoRoutine<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>), instance, method, true);
            }
            return ((StreamInfoRoutine<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>) d)(local, local2, local3, local4, local5, local6, local7, local8, local9, local10, local11, local12, info, stream);
        }

        public static IEnumerator InvokeStreamRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
        {
            P0 local = stream.Read<P0>(new object[0]);
            P1 local2 = stream.Read<P1>(new object[0]);
            P2 local3 = stream.Read<P2>(new object[0]);
            P3 local4 = stream.Read<P3>(new object[0]);
            P4 local5 = stream.Read<P4>(new object[0]);
            P5 local6 = stream.Read<P5>(new object[0]);
            P6 local7 = stream.Read<P6>(new object[0]);
            P7 local8 = stream.Read<P7>(new object[0]);
            P8 local9 = stream.Read<P8>(new object[0]);
            P9 local10 = stream.Read<P9>(new object[0]);
            P10 local11 = stream.Read<P10>(new object[0]);
            P11 local12 = stream.Read<P11>(new object[0]);
            if (d == null)
            {
                d = Delegate.CreateDelegate(typeof(StreamRoutine<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>), instance, method, true);
            }
            return ((StreamRoutine<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>) d)(local, local2, local3, local4, local5, local6, local7, local8, local9, local10, local11, local12, stream);
        }

        private static void Serializer(BitStream stream, object value, params object[] codecOptions)
        {
            Block<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11> block = (Block<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>) value;
            stream.Write<P0>(block.p0, codecOptions);
            stream.Write<P1>(block.p1, codecOptions);
            stream.Write<P2>(block.p2, codecOptions);
            stream.Write<P3>(block.p3, codecOptions);
            stream.Write<P4>(block.p4, codecOptions);
            stream.Write<P5>(block.p5, codecOptions);
            stream.Write<P6>(block.p6, codecOptions);
            stream.Write<P7>(block.p7, codecOptions);
            stream.Write<P8>(block.p8, codecOptions);
            stream.Write<P9>(block.p9, codecOptions);
            stream.Write<P10>(block.p10, codecOptions);
            stream.Write<P11>(block.p11, codecOptions);
        }

        public static NGC.IExecuter Exec
        {
            get
            {
                return Executer<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>.Singleton;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Block
        {
            public P0 p0;
            public P1 p1;
            public P2 p2;
            public P3 p3;
            public P4 p4;
            public P5 p5;
            public P6 p6;
            public P7 p7;
            public P8 p8;
            public P9 p9;
            public P10 p10;
            public P11 p11;
        }

        public delegate void Call(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11);

        private sealed class Executer : NGC.IExecuter
        {
            public static readonly NGC.IExecuter Singleton;

            static Executer()
            {
                NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>.Executer.Singleton = new NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>.Executer();
            }

            public void ExecuteCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>.InvokeCall(stream, ref d, method, instance);
            }

            public void ExecuteInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>.InvokeInfoCall(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                return NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>.InvokeInfoRoutine(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                return NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>.InvokeRoutine(stream, ref d, method, instance);
            }

            public void ExecuteStreamCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>.InvokeStreamCall(stream, ref d, method, instance);
            }

            public void ExecuteStreamInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>.InvokeStreamInfoCall(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteStreamInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info)
            {
                return NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>.InvokeStreamInfoRoutine(stream, ref d, method, instance, info);
            }

            public IEnumerator ExecuteStreamRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance)
            {
                return NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>.InvokeStreamRoutine(stream, ref d, method, instance);
            }
        }

        public delegate void InfoCall(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11, NetworkMessageInfo info);

        public delegate IEnumerator InfoRoutine(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11, NetworkMessageInfo info);

        public delegate IEnumerator Routine(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11);

        public delegate void StreamCall(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11, BitStream stream);

        public delegate void StreamInfoCall(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11, NetworkMessageInfo info, BitStream stream);

        public delegate IEnumerator StreamInfoRoutine(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11, NetworkMessageInfo info, BitStream stream);

        public delegate IEnumerator StreamRoutine(P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11, BitStream stream);
    }

    public delegate void EventCallback(NGCView view);

    private static class Global
    {
        public static readonly List<NGC> all = new List<NGC>();
        public static readonly Dictionary<ushort, NGC> byGroup = new Dictionary<ushort, NGC>();
    }

    public interface IExecuter
    {
        void ExecuteCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance);
        void ExecuteInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info);
        IEnumerator ExecuteInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info);
        IEnumerator ExecuteRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance);
        void ExecuteStreamCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance);
        void ExecuteStreamInfoCall(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info);
        IEnumerator ExecuteStreamInfoRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance, NetworkMessageInfo info);
        IEnumerator ExecuteStreamRoutine(BitStream stream, ref Delegate d, MethodInfo method, MonoBehaviour instance);
    }

    public sealed class Prefab
    {
        [NonSerialized]
        private Installation _installation;
        private Dictionary<string, int> cachedMessageIndices;
        [NonSerialized]
        public readonly string contentPath;
        [NonSerialized]
        public readonly string handle;
        [NonSerialized]
        public readonly int key;
        private WeakReference weakReference;

        private Prefab(string contentPath, int key, string handle)
        {
            this.contentPath = contentPath;
            this.key = key;
            this.handle = handle;
        }

        internal NetworkFlags DefaultNetworkFlags(int messageIndex)
        {
            return this.installation.methods[messageIndex].defaultNetworkFlags;
        }

        public int MessageIndex(string message)
        {
            int num;
            if ((this.cachedMessageIndices != null) && this.cachedMessageIndices.TryGetValue(message, out num))
            {
                return num;
            }
            int num2 = this.MessageIndexFind(message);
            if (num2 == -1)
            {
                throw new ArgumentException(message, "message");
            }
            if (this.cachedMessageIndices == null)
            {
                this.cachedMessageIndices = new Dictionary<string, int>();
            }
            this.cachedMessageIndices[message] = num2;
            return num2;
        }

        private int MessageIndexFind(string message)
        {
            int length = message.LastIndexOf(':');
            if (length == -1)
            {
                for (int i = 0; i < this._installation.methods.Length; i++)
                {
                    if (this._installation.methods[i].method.Name == message)
                    {
                        return i;
                    }
                }
            }
            else
            {
                for (int j = 0; j < this._installation.methods.Length; j++)
                {
                    if ((string.Compare(this._installation.methods[j].method.Name, 0, message, length + 1, message.Length - (length + 1)) == 0) && (string.Compare(this._installation.methods[j].method.DeclaringType.FullName, 0, message, 0, length) == 0))
                    {
                        return j;
                    }
                }
            }
            return -1;
        }

        public Installation installation
        {
            get
            {
                if ((this._installation == null) && (this.prefab == null))
                {
                    throw new InvalidOperationException("Could not get installation because prefab could not load");
                }
                return this._installation;
            }
        }

        public NGCView prefab
        {
            get
            {
                NGCView view;
                if (((this.weakReference == null) || ((view = (NGCView) this.weakReference.Target) == null)) || !this.weakReference.IsAlive)
                {
                    if (!Bundling.Load<NGCView>(this.contentPath, typeof(NGCView), out view))
                    {
                        throw new MissingReferenceException("Could not load NGCView at " + this.contentPath);
                    }
                    if (this._installation == null)
                    {
                        this._installation = Installation.Create(view);
                    }
                    this.weakReference = new WeakReference(view);
                }
                return view;
            }
        }

        public sealed class Installation
        {
            [CompilerGenerated]
            private static Comparison<Method> <>f__am$cache3;
            public readonly Method[] methods;
            public readonly ushort[] methodScriptIndices;
            private static readonly Dictionary<Type, Method[]> methodsForType = new Dictionary<Type, Method[]>();

            private Installation(Method[] methods, ushort[] methodScriptIndices)
            {
                this.methods = methods;
                this.methodScriptIndices = methodScriptIndices;
            }

            public static NGC.Prefab.Installation Create(NGCView view)
            {
                int num = 0;
                List<Method[]> list = new List<Method[]>();
                foreach (MonoBehaviour behaviour in view.scripts)
                {
                    Method[] methodArray;
                    Type key = behaviour.GetType();
                    if (!methodsForType.TryGetValue(key, out methodArray))
                    {
                        List<Method> list2 = new List<Method>();
                        foreach (MethodInfo info in key.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
                        {
                            bool flag = false;
                            if (info.IsDefined(typeof(RPC), true))
                            {
                                if (!info.IsDefined(typeof(NGCRPCSkipAttribute), false) || info.IsDefined(typeof(NGCRPCAttribute), true))
                                {
                                    flag = true;
                                }
                            }
                            else if (info.IsDefined(typeof(NGCRPCAttribute), true))
                            {
                                flag = true;
                            }
                            if (flag)
                            {
                                list2.Add(Method.Create(info));
                            }
                        }
                        if (<>f__am$cache3 == null)
                        {
                            <>f__am$cache3 = (x, y) => x.method.Name.CompareTo(y.method.Name);
                        }
                        list2.Sort(<>f__am$cache3);
                        methodArray = list2.ToArray();
                        methodsForType[key] = methodArray;
                    }
                    num += methodArray.Length;
                    list.Add(methodArray);
                }
                Method[] methods = new Method[num];
                ushort[] methodScriptIndices = new ushort[num];
                int index = 0;
                ushort num5 = 0;
                foreach (Method[] methodArray3 in list)
                {
                    foreach (Method method in methodArray3)
                    {
                        methods[index] = method;
                        methodScriptIndices[index] = num5;
                        index++;
                    }
                    num5 = (ushort) (num5 + 1);
                }
                return new NGC.Prefab.Installation(methods, methodScriptIndices);
            }

            public sealed class Instance
            {
                public readonly Delegate[] delegates;

                public Instance(NGC.Prefab.Installation installation)
                {
                    this.delegates = new Delegate[installation.methods.Length];
                }

                public void Invoke(NGC.Procedure procedure)
                {
                    procedure.view.prefab.installation.methods[procedure.message].Invoke(ref this.delegates[procedure.message], procedure, procedure.view.prefab.installation.methodScriptIndices[procedure.message]);
                }
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct Method
            {
                private const byte FLAG_INFO = 1;
                private const byte FLAG_STREAM = 2;
                private const byte FLAG_ENUMERATOR = 4;
                private const byte FLAG_FORCE_UNBUFFERED = 8;
                private const byte FLAG_FORCE_INSECURE = 0x10;
                private const byte FLAG_FORCE_NO_TIMESTAMP = 0x20;
                private const byte FLAG_FORCE_UNRELIABLE = 0x40;
                private const byte FLAG_FORCE_TYPE_UNSAFE = 0x80;
                private const byte INVOKE_FLAGS = 7;
                public readonly MethodInfo method;
                public readonly byte flags;
                private readonly NGC.IExecuter executer;
                private Method(MethodInfo method, byte flags, NGC.IExecuter executer)
                {
                    this.method = method;
                    this.flags = flags;
                    this.executer = executer;
                }

                public NetworkFlags defaultNetworkFlags
                {
                    get
                    {
                        NetworkFlags normal = NetworkFlags.Normal;
                        if ((this.flags & 0x21) != 1)
                        {
                            normal = (NetworkFlags) ((byte) (normal | NetworkFlags.NoTimestamp));
                        }
                        if ((this.flags & 0x80) == 0x80)
                        {
                            normal = (NetworkFlags) ((byte) (normal | NetworkFlags.TypeUnsafe));
                        }
                        if ((this.flags & 0x40) == 0x10)
                        {
                            normal = (NetworkFlags) ((byte) (normal | (NetworkFlags.Normal | NetworkFlags.Unbuffered | NetworkFlags.Unreliable)));
                        }
                        else if ((this.flags & 8) == 8)
                        {
                            normal = (NetworkFlags) ((byte) (normal | NetworkFlags.Unbuffered));
                        }
                        if ((this.flags & 0x10) == 0x10)
                        {
                            normal = (NetworkFlags) ((byte) (normal | NetworkFlags.Unencrypted));
                        }
                        return normal;
                    }
                }
                public void Invoke(ref Delegate d, NGC.Procedure procedure, ushort scriptIndex)
                {
                    IEnumerator enumerator;
                    MonoBehaviour instance = procedure.view.scripts[scriptIndex];
                    switch ((this.flags & 7))
                    {
                        case 0:
                            this.executer.ExecuteCall(procedure.CreateBitStream(), ref d, this.method, instance);
                            return;

                        case 1:
                            this.executer.ExecuteInfoCall(procedure.CreateBitStream(), ref d, this.method, instance, procedure.info);
                            return;

                        case 2:
                            this.executer.ExecuteStreamCall(procedure.CreateBitStream(), ref d, this.method, instance);
                            return;

                        case 3:
                            this.executer.ExecuteStreamInfoCall(procedure.CreateBitStream(), ref d, this.method, instance, procedure.info);
                            return;

                        case 4:
                            enumerator = this.executer.ExecuteRoutine(procedure.CreateBitStream(), ref d, this.method, instance);
                            break;

                        case 5:
                            enumerator = this.executer.ExecuteInfoRoutine(procedure.CreateBitStream(), ref d, this.method, instance, procedure.info);
                            break;

                        case 6:
                            enumerator = this.executer.ExecuteStreamRoutine(procedure.CreateBitStream(), ref d, this.method, instance);
                            break;

                        case 7:
                            enumerator = this.executer.ExecuteStreamInfoRoutine(procedure.CreateBitStream(), ref d, this.method, instance, procedure.info);
                            break;

                        default:
                        {
                            int num2 = this.flags & 7;
                            throw new NotImplementedException(num2.ToString());
                        }
                    }
                    if (enumerator != null)
                    {
                        try
                        {
                            instance.StartCoroutine(enumerator);
                        }
                        catch (Exception exception)
                        {
                            Debug.LogException(exception, instance);
                        }
                    }
                }

                public static NGC.Prefab.Installation.Method Create(MethodInfo info)
                {
                    byte num;
                    Type returnType = info.ReturnType;
                    if (returnType == typeof(void))
                    {
                        num = 0;
                    }
                    else
                    {
                        if (returnType != typeof(IEnumerator))
                        {
                            throw new InvalidOperationException(string.Format("RPC {0} returns a unhandled type: {1}", info, returnType));
                        }
                        num = 4;
                    }
                    ParameterInfo[] parameters = info.GetParameters();
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        if (parameters[i].IsOut || parameters[i].IsIn)
                        {
                            throw new InvalidOperationException(string.Format("RPC method {0} has a illegal parameter {1}", info, parameters[i]));
                        }
                    }
                    int length = parameters.Length;
                    if (length > 0)
                    {
                        Type type2;
                        if ((type2 = parameters[parameters.Length - 1].ParameterType) == typeof(NetworkMessageInfo))
                        {
                            length--;
                            if ((length > 0) && ((type2 = parameters[length - 1].ParameterType) == typeof(BitStream)))
                            {
                                length--;
                                num = (byte) (num | 3);
                            }
                            else
                            {
                                num = (byte) (num | 1);
                            }
                        }
                        else if (type2 == typeof(BitStream))
                        {
                            length--;
                            num = (byte) (num | 2);
                        }
                    }
                    Type[] argumentTypes = new Type[length];
                    for (int j = 0; j < length; j++)
                    {
                        argumentTypes[j] = parameters[j].ParameterType;
                    }
                    NGC.IExecuter executer = NGC.FindExecuter(argumentTypes);
                    if (executer == null)
                    {
                        throw new InvalidProgramException();
                    }
                    return new NGC.Prefab.Installation.Method(info, num, executer);
                }
            }
        }

        public static class Register
        {
            private static readonly Dictionary<int, NGC.Prefab> PrefabByIndex = new Dictionary<int, NGC.Prefab>();
            private static readonly Dictionary<string, NGC.Prefab> PrefabByName = new Dictionary<string, NGC.Prefab>();
            private static readonly List<NGC.Prefab> Prefabs = new List<NGC.Prefab>();

            public static bool Add(string contentPath, int index, string handle)
            {
                try
                {
                    NGC.Prefab prefab = new NGC.Prefab(contentPath, index, handle);
                    PrefabByIndex.Add(index, prefab);
                    try
                    {
                        PrefabByName.Add(handle, prefab);
                    }
                    catch
                    {
                        PrefabByIndex.Remove(index);
                        throw;
                    }
                    Prefabs.Add(prefab);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            public static bool Find(int index, out NGC.Prefab prefab)
            {
                return PrefabByIndex.TryGetValue(index, out prefab);
            }

            public static bool Find(string handle, out NGC.Prefab prefab)
            {
                return PrefabByName.TryGetValue(handle, out prefab);
            }

            public static string FindName(int iIndex)
            {
                return PrefabByIndex[iIndex].handle;
            }
        }
    }

    public sealed class Procedure
    {
        public byte[] data;
        public int dataLength;
        public NetworkMessageInfo info;
        public int message;
        public NGC outer;
        public int target;
        public NGCView view;

        public bool Call()
        {
            if (this.view == null)
            {
                try
                {
                    this.view = this.outer.views[(ushort) this.target];
                }
                catch (KeyNotFoundException)
                {
                    return false;
                }
            }
            try
            {
                this.view.install.Invoke(this);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this.view);
            }
            return true;
        }

        public BitStream CreateBitStream()
        {
            if (this.dataLength == 0)
            {
                return new BitStream(false);
            }
            return new BitStream(this.data, false);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct RPCName
    {
        public readonly string name;
        public readonly NetworkFlags flags;
        public RPCName(NGCView view, int message, string name, NetworkFlags flags)
        {
            this.name = name;
            this.flags = flags;
        }
    }
}

