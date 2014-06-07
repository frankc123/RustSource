using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using uLink;
using UnityEngine;

public sealed class InterpTimedEvent : IDisposable
{
    private static bool _forceCatchupToDate;
    public ArgList args;
    public MonoBehaviour component;
    internal static InterpTimedEvent current;
    private bool disposed;
    private static Dir dump;
    private static int dumpCount;
    private static readonly object[] emptyArgs = new object[0];
    public NetworkMessageInfo info;
    private bool inlist;
    internal Dir next;
    internal Dir prev;
    private static LList queue;
    public string tag;

    private InterpTimedEvent()
    {
    }

    public static object Argument(int index)
    {
        return current.args.parameters[index];
    }

    public static T Argument<T>(int index)
    {
        Type c = current.args.types[index];
        if (!typeof(T).IsAssignableFrom(c) && ((typeof(void) != c) || typeof(T).IsValueType))
        {
            throw new InvalidCastException(string.Format("Argument #{0} was a {1} and {2} is not assignable by {1}", index, current.args.types[index], typeof(T)));
        }
        return (T) current.args.parameters[index];
    }

    public static bool ArgumentIs<T>(int index)
    {
        Type c = current.args.types[index];
        return (typeof(T).IsAssignableFrom(c) || ((c == typeof(void)) && !typeof(T).IsValueType));
    }

    public static bool ArgumentIs(int index, Type comptype)
    {
        Type type = current.args.types[index];
        return (comptype.IsAssignableFrom(current.args.types[index]) || ((type == typeof(void)) && !comptype.IsValueType));
    }

    public static Type ArgumentType(int index)
    {
        return current.args.types[index];
    }

    public static void Catchup()
    {
        Catchup(Interpolation.timeInMillis);
    }

    public static void Catchup(ulong playhead)
    {
        _forceCatchupToDate = false;
        while (queue.Dequeue(playhead, out current))
        {
            Invoke();
        }
    }

    public static void Clear()
    {
        Clear(false);
    }

    public static void Clear(bool invokePending)
    {
        InterpTimedEvent event2;
        LList.Iterator iterator = new LList.Iterator();
        if (invokePending)
        {
            while (queue.Dequeue(ulong.MaxValue, out event2, ref iterator))
            {
                InvokeDirect(event2);
            }
        }
        else
        {
            while (queue.Dequeue(ulong.MaxValue, out event2, ref iterator))
            {
                event2.Dispose();
            }
        }
    }

    public void Dispose()
    {
        if (this.inlist)
        {
            queue.Remove(this);
        }
        if (!this.disposed)
        {
            this.prev = new Dir();
            this.next = dump;
            dump.has = true;
            dump.node = this;
            this.component = null;
            this.args.Dispose();
            this.args = null;
            this.info = null;
            this.tag = null;
            dumpCount++;
            this.disposed = true;
        }
    }

    public static void EMERGENCY_DUMP(bool TRY_TO_EXECUTE)
    {
        Debug.LogWarning("RUNNING EMERGENCY DUMP: TRY TO EXECUTE=" + TRY_TO_EXECUTE);
        try
        {
            if (TRY_TO_EXECUTE)
            {
                try
                {
                    foreach (InterpTimedEvent event2 in queue.EmergencyDump(true))
                    {
                        try
                        {
                            InvokeDirect(event2);
                        }
                        catch (Exception exception)
                        {
                            Debug.LogException(exception);
                        }
                        finally
                        {
                            try
                            {
                                event2.Dispose();
                            }
                            catch (Exception exception2)
                            {
                                Debug.LogException(exception2);
                            }
                        }
                    }
                }
                catch (Exception exception3)
                {
                    Debug.LogException(exception3);
                }
            }
            else
            {
                queue.EmergencyDump(false);
            }
        }
        catch (Exception exception4)
        {
            Debug.LogException(exception4);
        }
        finally
        {
            queue = new LList();
            dump = new Dir();
            dumpCount = 0;
        }
        Debug.LogWarning("END OF EMERGENCY DUMP: TRY TO EXECUTE=" + TRY_TO_EXECUTE);
    }

    public static void EmergencyDump()
    {
    }

    public static bool Execute(IInterpTimedEventReceiver receiver, string tag, ref NetworkMessageInfo info)
    {
        return QueueOrExecute(receiver, true, tag, ref info, emptyArgs);
    }

    public static bool Execute(IInterpTimedEventReceiver receiver, string tag, ref NetworkMessageInfo info, params object[] args)
    {
        return QueueOrExecute(receiver, true, tag, ref info, args);
    }

    public static void ForceCatchupToDate()
    {
        _forceCatchupToDate = true;
    }

    private static void Invoke()
    {
        MonoBehaviour component = current.component;
        if (component != null)
        {
            IInterpTimedEventReceiver receiver = component as IInterpTimedEventReceiver;
            try
            {
                receiver.OnInterpTimedEvent();
            }
            catch (Exception exception)
            {
                Debug.LogError("Exception thrown during catchup \r\n" + exception, component);
            }
        }
        else
        {
            Debug.LogWarning("A component implementing IInterpTimeEventReceiver was destroyed without properly calling InterpEvent.Remove() in OnDestroy!\r\n" + (!string.IsNullOrEmpty(current.tag) ? ("The tag was \"" + current.tag + "\"") : "There was no tag set"));
        }
        current.Dispose();
    }

    private static void InvokeDirect(InterpTimedEvent evnt)
    {
        InterpTimedEvent current = InterpTimedEvent.current;
        InterpTimedEvent.current = evnt;
        Invoke();
        InterpTimedEvent.current = current;
    }

    public static void MarkUnhandled()
    {
        Debug.LogWarning("Unhandled Timed Event :" + (!string.IsNullOrEmpty(current.tag) ? current.tag : " without a tag"), current.component);
    }

    internal static InterpTimedEvent New(MonoBehaviour receiver, string tag, ref NetworkMessageInfo info, object[] args, bool immediate)
    {
        InterpTimedEvent node;
        if (receiver == null)
        {
            Debug.LogError("receiver is null or has been destroyed", receiver);
            return null;
        }
        if (!(receiver is IInterpTimedEventReceiver))
        {
            Debug.LogError("receiver of type " + receiver.GetType() + " does not implement IInterpTimedEventReceiver", receiver);
            return null;
        }
        if (dump.has)
        {
            dumpCount--;
            node = dump.node;
            dump = node.next;
            node.next = new Dir();
            node.prev = new Dir();
            node.disposed = false;
        }
        else
        {
            node = new InterpTimedEvent();
        }
        node.args = ArgList.New(args);
        node.tag = tag;
        node.component = receiver;
        node.info = info;
        if (!immediate)
        {
            queue.Insert(node);
        }
        return node;
    }

    public static bool Queue(IInterpTimedEventReceiver receiver, string tag, ref NetworkMessageInfo info)
    {
        return QueueOrExecute(receiver, false, tag, ref info, emptyArgs);
    }

    public static bool Queue(IInterpTimedEventReceiver receiver, string tag, ref NetworkMessageInfo info, params object[] args)
    {
        return QueueOrExecute(receiver, false, tag, ref info, args);
    }

    public static bool QueueOrExecute(IInterpTimedEventReceiver receiver, bool immediate, string tag, ref NetworkMessageInfo info)
    {
        return QueueOrExecute(receiver, immediate, tag, ref info, emptyArgs);
    }

    public static bool QueueOrExecute(IInterpTimedEventReceiver receiver, bool immediate, string tag, ref NetworkMessageInfo info, params object[] args)
    {
        MonoBehaviour behaviour = receiver as MonoBehaviour;
        InterpTimedEvent evnt = New(behaviour, tag, ref info, args, immediate);
        if (evnt == null)
        {
            return false;
        }
        if (immediate)
        {
            InvokeDirect(evnt);
        }
        else if (!InterpTimedEventSyncronizer.available)
        {
            Debug.LogWarning("Not running event because theres no syncronizer available. " + tag, receiver as Object);
            return false;
        }
        return true;
    }

    public static void Remove(MonoBehaviour receiver)
    {
        Remove(receiver, false);
    }

    public static void Remove(MonoBehaviour receiver, bool invokePending)
    {
        InterpTimedEvent event2;
        LList.Iterator iterator = new LList.Iterator();
        if (invokePending)
        {
            while (queue.Dequeue(receiver, ulong.MaxValue, out event2, ref iterator))
            {
                InvokeDirect(event2);
            }
        }
        else
        {
            while (queue.Dequeue(receiver, ulong.MaxValue, out event2, ref iterator))
            {
                event2.Dispose();
            }
        }
    }

    public static object[] ArgumentArray
    {
        get
        {
            return current.args.parameters;
        }
    }

    public static int ArgumentCount
    {
        get
        {
            return current.args.length;
        }
    }

    public static Type[] ArgumentTypeArray
    {
        get
        {
            return current.args.types;
        }
    }

    public static NetworkFlags Flags
    {
        get
        {
            return current.info.flags;
        }
    }

    public static NetworkMessageInfo Info
    {
        get
        {
            return current.info;
        }
    }

    public static uLink.NetworkView NetworkView
    {
        get
        {
            return current.info.networkView;
        }
    }

    public IInterpTimedEventReceiver receiver
    {
        get
        {
            return (this.component as IInterpTimedEventReceiver);
        }
    }

    public static NetworkPlayer Sender
    {
        get
        {
            return current.info.sender;
        }
    }

    public static bool syncronizationPaused
    {
        get
        {
            return InterpTimedEventSyncronizer.paused;
        }
        set
        {
            InterpTimedEventSyncronizer.paused = value;
        }
    }

    public static string Tag
    {
        get
        {
            return current.tag;
        }
    }

    public static MonoBehaviour Target
    {
        get
        {
            return current.component;
        }
    }

    public static double Timestamp
    {
        get
        {
            return current.info.timestamp;
        }
    }

    public static ulong TimestampInMilliseconds
    {
        get
        {
            return current.info.timestampInMillis;
        }
    }

    public sealed class ArgList : IDisposable
    {
        private bool disposed;
        private InterpTimedEvent.ArgList dumpNext;
        private static Dump[] dumps = new Dump[4];
        public readonly int length;
        public readonly object[] parameters;
        public readonly Type[] types;
        private static InterpTimedEvent.ArgList voidParameters = new InterpTimedEvent.ArgList(0);

        private ArgList(int length)
        {
            this.length = length;
            this.parameters = new object[length];
            this.types = new Type[length];
        }

        private void AddToDump(ref Dump dump)
        {
            this.dumpNext = dump.last;
            dump.count++;
            dump.last = this;
        }

        public void Dispose()
        {
            if (!this.disposed && (this.length != 0))
            {
                for (int i = 0; i < this.length; i++)
                {
                    this.types[i] = null;
                    this.parameters[i] = null;
                }
                if (dumps.Length <= this.length)
                {
                    Array.Resize<Dump>(ref dumps, this.length + 1);
                }
                this.AddToDump(ref dumps[this.length]);
                this.disposed = true;
            }
        }

        public static InterpTimedEvent.ArgList New(object[] args)
        {
            InterpTimedEvent.ArgList list;
            int length = (args != null) ? args.Length : 0;
            if (length == 0)
            {
                return voidParameters;
            }
            if (dumps.Length > length)
            {
                list = Recycle(ref dumps[length], length);
            }
            else
            {
                list = new InterpTimedEvent.ArgList(length);
            }
            for (int i = 0; i < length; i++)
            {
                object obj2 = args[i];
                list.parameters[i] = obj2;
                list.types[i] = (obj2 != null) ? obj2.GetType() : typeof(void);
            }
            return list;
        }

        private static InterpTimedEvent.ArgList Recycle(ref Dump dump, int length)
        {
            if (dump.count > 0)
            {
                InterpTimedEvent.ArgList last = dump.last;
                dump.last = last.dumpNext;
                dump.count--;
                last.dumpNext = null;
                last.disposed = false;
                return last;
            }
            return new InterpTimedEvent.ArgList(length);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Dump
        {
            public InterpTimedEvent.ArgList last;
            public int count;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Dir
    {
        public bool has;
        public InterpTimedEvent node;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct LList
    {
        public InterpTimedEvent.Dir first;
        public InterpTimedEvent.Dir last;
        public int count;
        private HashSet<InterpTimedEvent> FAIL_SAFE_SET;
        [CompilerGenerated]
        private static Comparison<InterpTimedEvent> <>f__am$cache4;
        public bool Dequeue(ulong playhead, out InterpTimedEvent node)
        {
            Iterator iterator = new Iterator();
            return this.Dequeue(playhead, out node, ref iterator);
        }

        public bool Dequeue(ulong playhead, out InterpTimedEvent node, ref Iterator iter_)
        {
            if (this.count <= 0)
            {
                node = null;
                return false;
            }
            InterpTimedEvent.Dir dir = !iter_.started ? this.first : iter_.d;
            while (dir.has)
            {
                if (playhead < dir.node.info.timestamp)
                {
                    break;
                }
                node = dir.node;
                iter_.d = node.next;
                iter_.started = true;
                this.Remove(node);
                return true;
            }
            iter_.d = new InterpTimedEvent.Dir();
            iter_.started = true;
            node = null;
            return false;
        }

        public bool Dequeue(MonoBehaviour script, ulong playhead, out InterpTimedEvent node)
        {
            Iterator iterator = new Iterator();
            return this.Dequeue(script, playhead, out node, ref iterator);
        }

        public bool Dequeue(MonoBehaviour script, ulong playhead, out InterpTimedEvent node, ref Iterator iter_)
        {
            if (this.count <= 0)
            {
                node = null;
                return false;
            }
            for (InterpTimedEvent.Dir dir = !iter_.started ? this.first : iter_.d; dir.has; dir = dir.node.next)
            {
                if (playhead < dir.node.info.timestamp)
                {
                    break;
                }
                if (dir.node.component == script)
                {
                    node = dir.node;
                    iter_.d = node.next;
                    iter_.started = true;
                    this.Remove(node);
                    return true;
                }
            }
            iter_.d = new InterpTimedEvent.Dir();
            iter_.started = true;
            node = null;
            return false;
        }

        internal bool Remove(InterpTimedEvent node)
        {
            if (!this.RemoveUnsafe(node))
            {
                return false;
            }
            if (this.FAIL_SAFE_SET != null)
            {
                this.FAIL_SAFE_SET.Remove(node);
            }
            return true;
        }

        private bool RemoveUnsafe(InterpTimedEvent node)
        {
            if (((this.count > 0) && (node != null)) && node.inlist)
            {
                if (node.prev.has)
                {
                    if (node.next.has)
                    {
                        node.next.node.prev = node.prev;
                        node.prev.node.next = node.next;
                        this.count--;
                        node.prev = node.next = new InterpTimedEvent.Dir();
                        node.inlist = false;
                        return true;
                    }
                    this.last = node.prev;
                    this.last.node.next = new InterpTimedEvent.Dir();
                    this.count--;
                    node.prev = node.next = new InterpTimedEvent.Dir();
                    node.inlist = false;
                    return true;
                }
                if (node.next.has)
                {
                    this.first = node.next;
                    this.first.node.prev = new InterpTimedEvent.Dir();
                    this.count--;
                    node.prev = node.next = new InterpTimedEvent.Dir();
                    node.inlist = false;
                    return true;
                }
                if (this.first.node == node)
                {
                    this.first = this.last = new InterpTimedEvent.Dir();
                    this.count = 0;
                    node.prev = node.next = new InterpTimedEvent.Dir();
                    node.inlist = false;
                    return true;
                }
            }
            return false;
        }

        private bool Insert(ref InterpTimedEvent.Dir ent)
        {
            if (ent.node == null)
            {
                return false;
            }
            if (ent.node.inlist)
            {
                return false;
            }
            if (this.count == 0)
            {
                this.first = this.last = ent;
            }
            else if (this.last.node.info.timestampInMillis <= ent.node.info.timestampInMillis)
            {
                if (this.count == 1)
                {
                    this.first = this.last;
                    this.last = ent;
                    ent.node.prev = this.first;
                    this.first.node.next = this.last;
                }
                else
                {
                    ent.node.prev = this.last;
                    this.last.node.next = ent;
                    this.last = ent;
                }
            }
            else if (this.count == 1)
            {
                this.first = ent;
                this.first.node.next = this.last;
                this.last.node.prev = this.first;
            }
            else if (this.first.node.info.timestampInMillis > ent.node.info.timestampInMillis)
            {
                ent.node.next = this.first;
                this.first.node.prev = ent;
                this.first = ent;
            }
            else
            {
                InterpTimedEvent.Dir last;
                if (this.first.node.info.timestampInMillis == ent.node.info.timestampInMillis)
                {
                    for (last = this.first; last.node.next.has; last = last.node.next)
                    {
                        if (last.node.next.node.info.timestampInMillis > ent.node.info.timestampInMillis)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    last = this.last;
                    while (last.node.prev.has)
                    {
                        last = last.node.prev;
                        if (last.node.info.timestampInMillis <= ent.node.info.timestampInMillis)
                        {
                            break;
                        }
                    }
                }
                ent.node.next = last.node.next;
                ent.node.prev = last;
            }
            this.count++;
            ent.node.inlist = true;
            if (this.FAIL_SAFE_SET == null)
            {
                this.FAIL_SAFE_SET = new HashSet<InterpTimedEvent>();
            }
            this.FAIL_SAFE_SET.Add(ent.node);
            return true;
        }

        public bool Insert(InterpTimedEvent node)
        {
            InterpTimedEvent.Dir dir;
            dir.node = node;
            dir.has = true;
            return this.Insert(ref dir);
        }

        public List<InterpTimedEvent> EmergencyDump(bool botherSorting)
        {
            bool flag;
            HashSet<InterpTimedEvent> collection = new HashSet<InterpTimedEvent>();
            Iterator iterator = new Iterator();
            do
            {
                InterpTimedEvent event2;
                try
                {
                    flag = this.Dequeue(ulong.MaxValue, out event2, ref iterator);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                    break;
                }
                if (flag)
                {
                    collection.Add(event2);
                }
            }
            while (flag);
            this.first = this.last = new InterpTimedEvent.Dir();
            this.count = 0;
            HashSet<InterpTimedEvent> other = this.FAIL_SAFE_SET;
            this.FAIL_SAFE_SET = null;
            if (other != null)
            {
                collection.UnionWith(other);
            }
            List<InterpTimedEvent> list = new List<InterpTimedEvent>(collection);
            if (botherSorting)
            {
                try
                {
                    if (<>f__am$cache4 == null)
                    {
                        <>f__am$cache4 = delegate (InterpTimedEvent x, InterpTimedEvent y) {
                            if (x == null)
                            {
                                if (y == null)
                                {
                                    return 0;
                                }
                                int num = 0;
                                return num.CompareTo(1);
                            }
                            if (y == null)
                            {
                                int num2 = 1;
                                return num2.CompareTo(0);
                            }
                            return x.info.timestampInMillis.CompareTo(y.info.timestampInMillis);
                        };
                    }
                    list.Sort(<>f__am$cache4);
                }
                catch (Exception exception2)
                {
                    Debug.LogException(exception2);
                }
            }
            return list;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct Iterator
        {
            internal InterpTimedEvent.Dir d;
            internal bool started;
        }
    }
}

