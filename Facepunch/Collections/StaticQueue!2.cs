namespace Facepunch.Collections
{
    using System;
    using System.Runtime.InteropServices;

    public abstract class StaticQueue<KEY, T> where T: class
    {
        private static int count;
        private static bool reg_made;

        protected StaticQueue()
        {
        }

        protected static bool contains(ref Entry<KEY, T> state)
        {
            return state.inside;
        }

        protected static bool dequeue(T instance, ref Entry<KEY, T> state)
        {
            if (state.inside)
            {
                state.inside = false;
                return reg<KEY, T>.dispose(ref state.node);
            }
            return false;
        }

        protected static void drain()
        {
            if (StaticQueue<KEY, T>.reg_made)
            {
                reg<KEY, T>.drain();
            }
        }

        protected static bool enqueue(T instance, ref Entry<KEY, T> state)
        {
            if (!state.inside)
            {
                state.inside = true;
                state.node = reg<KEY, T>.insert_end(reg<KEY, T>.make_node(instance));
                return true;
            }
            return false;
        }

        protected static bool enrequeue(T instance, ref Entry<KEY, T> state)
        {
            return (!state.inside ? StaticQueue<KEY, T>.enqueue(instance, ref state) : StaticQueue<KEY, T>.requeue(instance, ref state));
        }

        protected static bool requeue(T instance, ref Entry<KEY, T> state)
        {
            if (!state.inside)
            {
                return false;
            }
            if (!object.ReferenceEquals(reg<KEY, T>.last, state.node))
            {
                state.node = reg<KEY, T>.insert_end(state.node);
            }
            return true;
        }

        protected static bool requeue(T instance, ref Entry<KEY, T> state, bool enqueue_if_missing)
        {
            return (!enqueue_if_missing ? StaticQueue<KEY, T>.enqueue(instance, ref state) : StaticQueue<KEY, T>.enrequeue(instance, ref state));
        }

        protected static bool validate(T instance, ref Entry<KEY, T> state, bool must_be_contained = false)
        {
            return (!state.inside ? !must_be_contained : (!object.ReferenceEquals(state.node, null) && object.ReferenceEquals(state.node.v, instance)));
        }

        protected static int num
        {
            get
            {
                return StaticQueue<KEY, T>.count;
            }
        }

        protected enum act
        {
            public const StaticQueue<KEY, T>.act back = StaticQueue<KEY, T>.act.back;,
            public const StaticQueue<KEY, T>.act delist = StaticQueue<KEY, T>.act.delist;,
            public const StaticQueue<KEY, T>.act front = StaticQueue<KEY, T>.act.front;,
            public const StaticQueue<KEY, T>.act none = StaticQueue<KEY, T>.act.none;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Entry
        {
            internal bool inside;
            internal StaticQueue<KEY, T>.node node;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct fork
        {
            public StaticQueue<KEY, T>.way p;
            public StaticQueue<KEY, T>.way n;
        }

        [StructLayout(LayoutKind.Sequential)]
        protected struct iterator
        {
            private int attempts;
            private int fail_left;
            private int position;
            private StaticQueue<KEY, T>.node node;
            private StaticQueue<KEY, T>.node next;
            public iterator(int maxIterations, int maxFailedIterations)
            {
                if ((maxIterations == 0) || (maxIterations > StaticQueue<KEY, T>.count))
                {
                    this.attempts = StaticQueue<KEY, T>.count;
                    this.fail_left = 0;
                }
                else if (maxIterations == StaticQueue<KEY, T>.count)
                {
                    this.attempts = StaticQueue<KEY, T>.count;
                    this.fail_left = 0;
                }
                else if ((maxIterations + maxFailedIterations) > StaticQueue<KEY, T>.count)
                {
                    this.attempts = maxIterations;
                    this.fail_left = StaticQueue<KEY, T>.count - maxIterations;
                }
                else
                {
                    this.attempts = maxIterations;
                    this.fail_left = maxFailedIterations;
                }
                this.position = 0;
                this.node = null;
                this.next = !StaticQueue<KEY, T>.reg_made ? null : StaticQueue<KEY, T>.reg.first;
            }

            public iterator(int maxIter) : this(maxIter, (maxIter >= StaticQueue<KEY, T>.count) ? 0 : (StaticQueue<KEY, T>.count - maxIter))
            {
            }

            public bool Start(out T v)
            {
                if (this.position++ < this.attempts)
                {
                    this.node = this.next;
                    this.next = this.node.w.n.v;
                    v = this.node.v;
                    return true;
                }
                this.node = (StaticQueue<KEY, T>.node) (this.next = null);
                v = null;
                return false;
            }

            public bool Validate(ref StaticQueue<KEY, T>.Entry key)
            {
                return key.inside;
            }

            public bool MissingNext(out T v)
            {
                if (this.fail_left-- > 0)
                {
                    this.position--;
                }
                StaticQueue<KEY, T>.reg.dispose(ref this.node);
                return this.Start(out v);
            }

            public bool Next(ref StaticQueue<KEY, T>.Entry prev_key, StaticQueue<KEY, T>.act cmd, out T v)
            {
                bool flag = object.ReferenceEquals(prev_key.node, null);
                if (!flag && !object.ReferenceEquals(prev_key.node, this.node))
                {
                    throw new ArgumentException("prev_key did not match that of what was expected", "prev_key");
                }
                if (flag)
                {
                    prev_key.inside = false;
                }
                if (!prev_key.inside)
                {
                    cmd = StaticQueue<KEY, T>.act.delist;
                    if (this.fail_left-- > 0)
                    {
                        this.position--;
                    }
                    if (flag)
                    {
                        StaticQueue<KEY, T>.reg.dispose(ref this.node);
                    }
                    else
                    {
                        StaticQueue<KEY, T>.reg.dispose(ref prev_key.node);
                    }
                }
                else
                {
                    switch (cmd)
                    {
                        case StaticQueue<KEY, T>.act.front:
                            if (StaticQueue<KEY, T>.reg.deref(this.node))
                            {
                                prev_key.node = StaticQueue<KEY, T>.reg.insert_begin(this.node);
                            }
                            break;

                        case StaticQueue<KEY, T>.act.back:
                            if (StaticQueue<KEY, T>.reg.deref(this.node))
                            {
                                prev_key.node = StaticQueue<KEY, T>.reg.insert_end(this.node);
                            }
                            break;

                        case StaticQueue<KEY, T>.act.delist:
                            StaticQueue<KEY, T>.reg.dispose(ref this.node);
                            prev_key.node = null;
                            break;
                    }
                }
                return this.Start(out v);
            }
        }

        internal class node
        {
            public bool e;
            public T v;
            public StaticQueue<KEY, T>.fork w;
        }

        private static class reg
        {
            private static StaticQueue<KEY, T>.node dump;
            private static int dump_size;
            internal static StaticQueue<KEY, T>.node first;
            internal static StaticQueue<KEY, T>.node last;

            static reg()
            {
                StaticQueue<KEY, T>.reg_made = true;
            }

            internal static void delete(StaticQueue<KEY, T>.node r)
            {
                r.v = null;
                r.w.n = new StaticQueue<KEY, T>.way();
                r.e = false;
                r.w.p.e = StaticQueue<KEY, T>.reg.dump_size++ > 0;
                r.w.p.v = StaticQueue<KEY, T>.reg.dump;
                StaticQueue<KEY, T>.reg.dump = r;
            }

            internal static bool deref(StaticQueue<KEY, T>.node node)
            {
                if (object.ReferenceEquals(node, null))
                {
                    return false;
                }
                if (!node.e)
                {
                    return false;
                }
                if (--StaticQueue<KEY, T>.count == 0)
                {
                    StaticQueue<KEY, T>.reg.first = (StaticQueue<KEY, T>.node) (StaticQueue<KEY, T>.reg.last = null);
                }
                else
                {
                    if (node.w.p.e)
                    {
                        node.w.p.v.w.n = node.w.n;
                    }
                    else if (node.w.n.e)
                    {
                        StaticQueue<KEY, T>.reg.first = node.w.n.v;
                    }
                    if (node.w.n.e)
                    {
                        node.w.n.v.w.p = node.w.p;
                    }
                    else if (node.w.p.e)
                    {
                        StaticQueue<KEY, T>.reg.last = node.w.p.v;
                    }
                    node.w = new StaticQueue<KEY, T>.fork();
                }
                node.e = false;
                return true;
            }

            internal static bool dispose(ref StaticQueue<KEY, T>.node node)
            {
                if (StaticQueue<KEY, T>.reg.deref(node))
                {
                    node.v = null;
                    StaticQueue<KEY, T>.reg.delete(node);
                    node = null;
                    return true;
                }
                return false;
            }

            public static void drain()
            {
                StaticQueue<KEY, T>.reg.dump = null;
                StaticQueue<KEY, T>.reg.dump_size = 0;
            }

            internal static StaticQueue<KEY, T>.node insert_begin(StaticQueue<KEY, T>.node node)
            {
                if (node.e)
                {
                    StaticQueue<KEY, T>.reg.deref(node);
                }
                if (StaticQueue<KEY, T>.count++ == 0)
                {
                    StaticQueue<KEY, T>.reg.last = StaticQueue<KEY, T>.reg.first = node;
                }
                else
                {
                    node.w.n.e = true;
                    node.w.n.v = StaticQueue<KEY, T>.reg.first;
                    StaticQueue<KEY, T>.reg.first.w.p.e = true;
                    StaticQueue<KEY, T>.reg.first.w.p.v = node;
                    StaticQueue<KEY, T>.reg.first = node;
                }
                node.e = true;
                return StaticQueue<KEY, T>.reg.first;
            }

            internal static StaticQueue<KEY, T>.node insert_end(StaticQueue<KEY, T>.node node)
            {
                if (node.e)
                {
                    StaticQueue<KEY, T>.reg.deref(node);
                }
                if (StaticQueue<KEY, T>.count++ == 0)
                {
                    StaticQueue<KEY, T>.reg.last = StaticQueue<KEY, T>.reg.first = node;
                }
                else
                {
                    node.w.p.e = true;
                    node.w.p.v = StaticQueue<KEY, T>.reg.last;
                    StaticQueue<KEY, T>.reg.last.w.n.e = true;
                    StaticQueue<KEY, T>.reg.last.w.n.v = node;
                    StaticQueue<KEY, T>.reg.last = node;
                }
                node.e = true;
                return StaticQueue<KEY, T>.reg.last;
            }

            internal static StaticQueue<KEY, T>.node make_node(T v)
            {
                StaticQueue<KEY, T>.node dump;
                if (StaticQueue<KEY, T>.reg.dump_size > 0)
                {
                    StaticQueue<KEY, T>.reg.dump_size--;
                    dump = StaticQueue<KEY, T>.reg.dump;
                    StaticQueue<KEY, T>.reg.dump = dump.w.p.v;
                    dump.w = new StaticQueue<KEY, T>.fork();
                }
                else
                {
                    dump = new StaticQueue<KEY, T>.node();
                }
                dump.v = v;
                dump.e = false;
                return dump;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct way
        {
            public StaticQueue<KEY, T>.node v;
            public bool e;
        }
    }
}

