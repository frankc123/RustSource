using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

public abstract class Socket
{
    public Vector3 eulerRotate;
    private bool got_last;
    private readonly bool is_vm;
    public Vector3 offset;
    public Transform parent;
    private Quaternion quat_last;
    private Vector3 rotate_last;

    protected Socket(bool is_vm)
    {
        this.is_vm = is_vm;
    }

    public bool AddChild(Transform transform, bool snap)
    {
        if (this.is_vm)
        {
            return ((CameraSpace) this).AddChild(transform, snap);
        }
        return ((LocalSpace) this).AddChild(transform, snap);
    }

    public bool AddChildWithCoords(Transform transform, Vector3 offsetFromThisSocket)
    {
        if (this.is_vm)
        {
            return ((CameraSpace) this).AddChildWithCoords(transform, offsetFromThisSocket);
        }
        return ((LocalSpace) this).AddChildWithCoords(transform, offsetFromThisSocket);
    }

    public bool AddChildWithCoords(Transform transform, Vector3 offsetFromThisSocket, Quaternion rotationalOffsetFromThisSocket)
    {
        if (this.is_vm)
        {
            return ((CameraSpace) this).AddChildWithCoords(transform, offsetFromThisSocket, rotationalOffsetFromThisSocket);
        }
        return ((LocalSpace) this).AddChildWithCoords(transform, offsetFromThisSocket, rotationalOffsetFromThisSocket);
    }

    public bool AddChildWithCoords(Transform transform, Vector3 offsetFromThisSocket, Vector3 eulerOffsetFromThisSocket)
    {
        if (this.is_vm)
        {
            return ((CameraSpace) this).AddChildWithCoords(transform, offsetFromThisSocket, eulerOffsetFromThisSocket);
        }
        return ((LocalSpace) this).AddChildWithCoords(transform, offsetFromThisSocket, eulerOffsetFromThisSocket);
    }

    private void AddInstanceChild(Transform tr, bool snap)
    {
        if (!this.AddChild(tr, snap))
        {
            Debug.LogWarning("Could not add child!", tr);
        }
    }

    public void DrawGizmos(string icon)
    {
        Matrix4x4 matrix = Gizmos.matrix;
        if (this.parent != null)
        {
            Gizmos.matrix = this.parent.localToWorldMatrix;
        }
        Gizmos.matrix *= Matrix4x4.TRS(this.offset, this.rotate, Vector3.one);
        Color color = Gizmos.color;
        Gizmos.color = color * Color.red;
        Gizmos.DrawLine(Vector3.zero, (Vector3) (Vector3.right * 0.1f));
        if (icon != null)
        {
            Gizmos.DrawIcon(Vector3.left, icon);
        }
        Gizmos.color = color * Color.green;
        Gizmos.DrawLine(Vector3.zero, (Vector3) (Vector3.up * 0.1f));
        if (icon != null)
        {
            Gizmos.DrawIcon(Vector3.down, icon);
        }
        Gizmos.color = color * Color.blue;
        Gizmos.DrawLine(Vector3.zero, (Vector3) (Vector3.forward * 0.1f));
        Gizmos.matrix = matrix;
        Gizmos.color = color;
    }

    public TObject Instantiate<TObject>(TObject prefab) where TObject: Object
    {
        return (TObject) Object.Instantiate(prefab, this.position, this.rotation);
    }

    public GameObject InstantiateAsChild(GameObject prefab, bool snap)
    {
        GameObject obj2 = (GameObject) Object.Instantiate(prefab, this.position, this.rotation);
        this.AddInstanceChild(obj2.transform, snap);
        return obj2;
    }

    public Transform InstantiateAsChild(Transform prefab, bool snap)
    {
        Transform tr = (Transform) Object.Instantiate(prefab, this.position, this.rotation);
        this.AddInstanceChild(tr, snap);
        return tr;
    }

    public TComponent InstantiateAsChild<TComponent>(TComponent prefab, bool snap) where TComponent: Component
    {
        TComponent local = (TComponent) Object.Instantiate(prefab, this.position, this.rotation);
        this.AddInstanceChild(local.transform, snap);
        return local;
    }

    public void Rotate(Quaternion rotation)
    {
        if (this.is_vm)
        {
            ((CameraSpace) this).Rotate(rotation);
        }
        else
        {
            ((LocalSpace) this).Rotate(rotation);
        }
    }

    public void Snap()
    {
        if (this.is_vm)
        {
            ((CameraSpace) this).Snap();
        }
    }

    public void UnRotate(Quaternion rotation)
    {
        if (this.is_vm)
        {
            ((CameraSpace) this).UnRotate(rotation);
        }
        else
        {
            ((LocalSpace) this).UnRotate(rotation);
        }
    }

    public Transform attachParent
    {
        get
        {
            if (this.is_vm)
            {
                return ((CameraSpace) this).attachParent;
            }
            return this.parent;
        }
    }

    public Vector3 localPosition
    {
        get
        {
            return this.offset;
        }
    }

    public Quaternion localRotation
    {
        get
        {
            return this.rotate;
        }
    }

    public Vector3 position
    {
        get
        {
            if (this.is_vm)
            {
                return ((CameraSpace) this).position;
            }
            return ((LocalSpace) this).position;
        }
    }

    public Quaternion rotate
    {
        get
        {
            if (!this.got_last || (this.rotate_last != this.eulerRotate))
            {
                this.rotate_last = this.eulerRotate;
                this.quat_last = Quaternion.Euler(this.eulerRotate);
                this.got_last = true;
            }
            return this.quat_last;
        }
    }

    public Quaternion rotation
    {
        get
        {
            if (this.is_vm)
            {
                return ((CameraSpace) this).rotation;
            }
            return ((LocalSpace) this).rotation;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CameraConversion : IEquatable<Socket.CameraConversion>
    {
        public readonly Transform Eye;
        public readonly Transform Shelf;
        public readonly bool Provided;
        public CameraConversion(Transform World, Transform Camera)
        {
            this.Eye = World;
            this.Shelf = Camera;
            this.Provided = ((World != Camera) && (World != null)) && ((bool) Camera);
        }

        public bool Valid
        {
            get
            {
                return ((this.Provided && (this.Eye != null)) && ((bool) this.Shelf));
            }
        }
        public bool Equals(Socket.CameraConversion other)
        {
            return (!this.Provided ? !other.Provided : ((other.Provided && (this.Eye == other.Eye)) && (this.Shelf == other.Shelf)));
        }

        public override bool Equals(object obj)
        {
            return ((obj is Socket.CameraConversion) && this.Equals((Socket.CameraConversion) obj));
        }

        public override string ToString()
        {
            return (!this.Valid ? (!this.Provided ? "[CameraConversion:NotProvided]" : "[CameraConversion:Invalid]") : "[CameraConversion:Valid]");
        }

        public override int GetHashCode()
        {
            return (!this.Provided ? 0 : (this.Eye.GetHashCode() ^ this.Shelf.GetHashCode()));
        }

        public static Socket.CameraConversion None
        {
            get
            {
                return new Socket.CameraConversion();
            }
        }
        public static implicit operator bool(Socket.CameraConversion cc)
        {
            return cc.Valid;
        }

        public static bool operator true(Socket.CameraConversion cc)
        {
            return cc.Valid;
        }

        public static bool operator false(Socket.CameraConversion cc)
        {
            return !cc.Valid;
        }
    }

    [Serializable]
    public sealed class CameraSpace : Socket
    {
        [NonSerialized]
        public Transform eye;
        public bool proxy;
        [NonSerialized]
        internal Transform proxyTransform;
        [NonSerialized]
        public Transform root;

        public CameraSpace() : base(true)
        {
        }

        public bool AddChild(Transform transform, bool snap)
        {
            if (!this.proxy || (this.proxyTransform == null))
            {
                return false;
            }
            if (snap)
            {
                transform.parent = this.proxyTransform;
                transform.localPosition = base.offset;
                transform.localEulerAngles = base.eulerRotate;
            }
            else
            {
                Vector3 position = transform.position;
                Vector3 forward = transform.forward;
                Vector3 up = transform.up;
                if (this.eye != null)
                {
                    position = this.eye.InverseTransformPoint(position);
                    up = this.eye.InverseTransformDirection(up);
                    forward = this.eye.InverseTransformDirection(forward);
                }
                if (this.root != null)
                {
                    position = this.root.TransformPoint(position);
                    up = this.root.TransformDirection(up);
                    forward = this.root.TransformDirection(forward);
                }
                if (base.parent != null)
                {
                    position = base.parent.InverseTransformPoint(position);
                    up = base.parent.InverseTransformDirection(up);
                    forward = base.parent.InverseTransformDirection(forward);
                }
                transform.parent = this.proxyTransform;
                transform.localPosition = position;
                transform.localRotation = Quaternion.LookRotation(forward, up);
            }
            return true;
        }

        public bool AddChildWithCoords(Transform transform, Vector3 offsetFromThisSocket)
        {
            if (this.AddChild(transform, false))
            {
                transform.localPosition = base.offset + (base.rotate * offsetFromThisSocket);
                return true;
            }
            return false;
        }

        public bool AddChildWithCoords(Transform transform, Vector3 offsetFromThisSocket, Quaternion rotationOffsetFromThisSocket)
        {
            if (this.AddChild(transform, false))
            {
                Quaternion rotate = base.rotate;
                transform.localPosition = base.offset + (rotate * offsetFromThisSocket);
                transform.localRotation = rotate * rotationOffsetFromThisSocket;
                return true;
            }
            return false;
        }

        public bool AddChildWithCoords(Transform transform, Vector3 offsetFromThisSocket, Vector3 eulerOffsetFromThisSocket)
        {
            if (this.AddChild(transform, false))
            {
                Quaternion rotate = base.rotate;
                transform.localPosition = base.offset + (rotate * offsetFromThisSocket);
                transform.localRotation = rotate * Quaternion.Euler(eulerOffsetFromThisSocket);
                return true;
            }
            return false;
        }

        public void Rotate(Quaternion rotation)
        {
            Vector3 vector;
            float num;
            rotation.ToAngleAxis(out num, out vector);
            vector = base.parent.TransformDirection(vector);
            base.parent.RotateAround(this.preEyePosition, vector, num);
        }

        public void Snap()
        {
            if ((this.proxy && (this.proxyTransform != null)) && ((this.root != null) && (this.eye != null)))
            {
                UpdateProxy(base.parent, this.proxyTransform, this.root, this.eye);
            }
        }

        public void UnRotate(Quaternion rotation)
        {
            Vector3 vector;
            float num;
            rotation.ToAngleAxis(out num, out vector);
            vector = base.parent.TransformDirection(vector);
            base.parent.RotateAround(this.preEyePosition, -vector, num);
        }

        public static void UpdateProxy(Transform key, Transform value, Transform shelf, Transform eye)
        {
            value.position = eye.TransformPoint(shelf.InverseTransformPoint(key.position));
            Vector3 forward = eye.TransformDirection(shelf.InverseTransformDirection(key.forward));
            Vector3 upwards = eye.TransformDirection(shelf.InverseTransformDirection(key.up));
            value.rotation = Quaternion.LookRotation(forward, upwards);
        }

        public Transform attachParent
        {
            get
            {
                if (this.proxy)
                {
                    return this.proxyTransform;
                }
                return this.eye;
            }
        }

        public Vector3 position
        {
            get
            {
                Vector3 offset;
                if (this.root != null)
                {
                    if ((base.parent != null) && (base.parent != this.root))
                    {
                        offset = this.root.InverseTransformPoint(base.parent.TransformPoint(base.offset));
                    }
                    else
                    {
                        offset = base.offset;
                    }
                }
                else if (base.parent != null)
                {
                    offset = base.parent.TransformPoint(base.offset);
                }
                else
                {
                    offset = base.offset;
                }
                return ((this.eye == null) ? offset : this.eye.TransformPoint(offset));
            }
        }

        public Vector3 preEyePosition
        {
            get
            {
                return ((base.parent == null) ? base.offset : base.parent.TransformPoint(base.offset));
            }
        }

        public Quaternion preEyeRotation
        {
            get
            {
                return ((base.parent == null) ? base.rotate : (base.parent.rotation * base.rotate));
            }
        }

        public Quaternion rotation
        {
            get
            {
                Quaternion rotate;
                if (this.root != null)
                {
                    if ((base.parent != null) && (base.parent != this.root))
                    {
                        rotate = Quaternion.Inverse(this.root.rotation) * (base.rotate * base.parent.rotation);
                    }
                    else
                    {
                        rotate = base.rotate;
                    }
                }
                else if (base.parent != null)
                {
                    rotate = base.rotate * base.parent.rotation;
                }
                else
                {
                    rotate = base.rotate;
                }
                if (this.eye != null)
                {
                    return (this.eye.rotation * rotate);
                }
                return rotate;
            }
        }
    }

    [Serializable]
    public sealed class ConfigBodyPart
    {
        public Vector3 eulerRotate;
        public Vector3 offset;
        public BodyPart parent;

        public static Socket.ConfigBodyPart Create(BodyPart parent, Vector3 offset, Vector3 eulerRotate)
        {
            return new Socket.ConfigBodyPart { parent = parent, offset = offset, eulerRotate = eulerRotate };
        }

        public bool Extract(ref Socket.CameraSpace space, HitBoxSystem system)
        {
            Transform transform;
            if (!this.Find(system, out transform))
            {
                return false;
            }
            if (space == null)
            {
                Socket.CameraSpace space2 = new Socket.CameraSpace {
                    parent = transform,
                    eulerRotate = this.eulerRotate,
                    offset = this.offset
                };
                space = space2;
            }
            else if (space.parent != transform)
            {
                space.parent = transform;
                space.eulerRotate = this.eulerRotate;
                space.offset = this.offset;
            }
            return true;
        }

        public bool Extract(ref Socket.LocalSpace space, HitBoxSystem system)
        {
            Transform transform;
            if (!this.Find(system, out transform))
            {
                return false;
            }
            if (space == null)
            {
                Socket.LocalSpace space2 = new Socket.LocalSpace {
                    parent = transform,
                    eulerRotate = this.eulerRotate,
                    offset = this.offset
                };
                space = space2;
            }
            else if (space.parent != transform)
            {
                space.parent = transform;
                space.eulerRotate = this.eulerRotate;
                space.offset = this.offset;
            }
            return true;
        }

        private bool Find(HitBoxSystem system, out Transform parent)
        {
            IDRemoteBodyPart part;
            if (system == null)
            {
                parent = null;
                return false;
            }
            if (!system.bodyParts.TryGetValue(this.parent, out part))
            {
                parent = null;
                return false;
            }
            parent = part.transform;
            return true;
        }
    }

    [Serializable]
    public sealed class LocalSpace : Socket
    {
        public LocalSpace() : base(false)
        {
        }

        public bool AddChild(Transform transform, bool snap)
        {
            if (transform == null)
            {
                return false;
            }
            transform.parent = base.parent;
            if (snap)
            {
                transform.localPosition = base.offset;
                transform.localEulerAngles = base.eulerRotate;
            }
            return true;
        }

        public bool AddChildWithCoords(Transform transform, Vector3 offsetFromThisSocket)
        {
            if (this.AddChild(transform, false))
            {
                transform.localPosition = base.offset + (base.rotate * offsetFromThisSocket);
                return true;
            }
            return false;
        }

        public bool AddChildWithCoords(Transform transform, Vector3 offsetFromThisSocket, Quaternion rotationOffsetFromThisSocket)
        {
            if (this.AddChild(transform, false))
            {
                Quaternion rotate = base.rotate;
                transform.localPosition = base.offset + (rotate * offsetFromThisSocket);
                transform.localRotation = rotate * rotationOffsetFromThisSocket;
                return true;
            }
            return false;
        }

        public bool AddChildWithCoords(Transform transform, Vector3 offsetFromThisSocket, Vector3 eulerOffsetFromThisSocket)
        {
            if (this.AddChild(transform, false))
            {
                Quaternion rotate = base.rotate;
                transform.localPosition = base.offset + (rotate * offsetFromThisSocket);
                transform.localRotation = rotate * Quaternion.Euler(eulerOffsetFromThisSocket);
                return true;
            }
            return false;
        }

        public void Rotate(Quaternion rotation)
        {
            Vector3 vector;
            float num;
            rotation.ToAngleAxis(out num, out vector);
            vector = base.parent.TransformDirection(vector);
            base.parent.RotateAround(this.position, vector, num);
        }

        public void Snap()
        {
        }

        public void UnRotate(Quaternion rotation)
        {
            Vector3 vector;
            float num;
            rotation.ToAngleAxis(out num, out vector);
            vector = base.parent.TransformDirection(vector);
            base.parent.RotateAround(this.position, -vector, num);
        }

        public Vector3 position
        {
            get
            {
                return ((base.parent == null) ? base.offset : base.parent.TransformPoint(base.offset));
            }
        }

        public Quaternion rotation
        {
            get
            {
                return ((base.parent == null) ? base.rotate : (base.parent.rotation * base.rotate));
            }
        }
    }

    public sealed class Map : Socket.Mapped
    {
        [NonSerialized]
        private Element[] array;
        [NonSerialized]
        private Socket.CameraConversion cameraSpace;
        [NonSerialized]
        private bool checkTransforms;
        [NonSerialized]
        private bool deleted;
        [NonSerialized]
        private Dictionary<string, int> dict;
        [NonSerialized]
        private bool forceUpdate;
        [NonSerialized]
        private bool initialized;
        [NonSerialized]
        private readonly Object script;
        [NonSerialized]
        private bool securing;
        [NonSerialized]
        private readonly Socket.Source source;
        [NonSerialized]
        private int version;

        private Map(Socket.Source source, Object script)
        {
            this.source = source;
            this.script = script;
        }

        private void CheckProxyIndex(int index, out ProxyCheck o)
        {
            if (o.isProxy = (o.isCameraSpace = !object.ReferenceEquals(o.cameraSpace = this.array[index].socket as Socket.CameraSpace, null)) && o.cameraSpace.proxy)
            {
                if (!this.array[index].madeLink)
                {
                    o.proxyLink = this.array[index].link = this.MakeProxy(o.cameraSpace, index);
                    this.array[index].madeLink = true;
                }
                else
                {
                    o.proxyLink = this.array[index].link;
                }
                o.parentOrProxy = o.proxyLink.proxy.transform;
            }
            else
            {
                o.proxyLink = null;
                o.parentOrProxy = this.array[index].socket.parent;
            }
            o.index = index;
        }

        private void CleanTransforms()
        {
            this.checkTransforms = true;
            this.cameraSpace = Socket.CameraConversion.None;
        }

        private void Delete()
        {
            if (this.initialized && !this.deleted)
            {
                this.deleted = true;
                for (int i = this.array.Length - 1; i >= 0; i--)
                {
                    if (this.array[i].madeLink)
                    {
                        this.DestroyProxyLink(this.array[i].link);
                    }
                }
            }
        }

        private void DestroyProxyLink(Socket.ProxyLink link)
        {
            if (link.linked)
            {
                link.linked = false;
                if (link.scriptAlive && (link.proxy != null))
                {
                    Object.Destroy(link.proxy);
                }
                link.proxy = null;
                if (link.gameObject != null)
                {
                    Object.Destroy(link.gameObject);
                }
                link.gameObject = null;
                link.proxy = null;
            }
        }

        private void ElementRemove(ref Element element, ref RemoveList<Socket.ProxyLink> removeList)
        {
            if (element.madeLink)
            {
                if (element.link.scriptAlive)
                {
                    removeList.Add(element.link);
                }
                element.link = null;
                element.madeLink = false;
            }
            this.dict.Remove(element.name);
        }

        private void ElementUpdate(int srcIndex, ref Element src, int dstIndex, ref Element dst, Socket newSocket)
        {
            if (srcIndex != dstIndex)
            {
                dst.name = src.name;
                dst.link = src.link;
                dst.socket = src.socket;
                dst.madeLink = src.madeLink;
                if (dst.madeLink)
                {
                    dst.link.index = dstIndex;
                }
                this.dict[dst.name] = dstIndex;
            }
            this.SocketUpdate(ref dst.socket, newSocket);
        }

        private Socket.Map EnsureMap()
        {
            return (!this.EnsureState() ? NullMap : this);
        }

        private bool EnsureState()
        {
            if ((this.script == null) || this.deleted)
            {
                return false;
            }
            if (!this.securing)
            {
                try
                {
                    this.securing = true;
                    this.SecureState();
                }
                finally
                {
                    this.securing = false;
                }
            }
            return true;
        }

        private static Socket.Map Get<TSource>(TSource source, ref Socket.Map member) where TSource: Object, Socket.Source
        {
            Socket.Map map2;
            if (object.ReferenceEquals(source, null))
            {
                throw new ArgumentNullException("source");
            }
            if (source == null)
            {
                return NullMap;
            }
            Socket.Map objA = member;
            if (object.ReferenceEquals(objA, null))
            {
                objA = new Socket.Map(source, source);
            }
            member = map2 = objA.EnsureMap();
            return map2;
        }

        private bool GetCameraSpace(out Socket.CameraConversion cameraSpace)
        {
            if (!this.EnsureState())
            {
                this.checkTransforms = false;
                this.cameraSpace = Socket.CameraConversion.None;
            }
            else if (this.checkTransforms)
            {
                this.checkTransforms = false;
                this.cameraSpace = this.source.CameraSpaceSetup();
            }
            cameraSpace = this.cameraSpace;
            return cameraSpace.Valid;
        }

        private void Initialize()
        {
            ICollection<string> is2;
            IEnumerable<string> socketNames = this.source.SocketNames;
            if (object.ReferenceEquals(socketNames, null))
            {
                is2 = new string[0];
            }
            else
            {
                is2 = socketNames as ICollection<string>;
                if (is2 == null)
                {
                    is2 = new HashSet<string>(socketNames, StringComparer.InvariantCultureIgnoreCase);
                }
            }
            int count = is2.Count;
            this.array = new Element[count];
            this.dict = new Dictionary<string, int>(count, StringComparer.InvariantCultureIgnoreCase);
            int index = 0;
            IEnumerator<string> enumerator = is2.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    string current = enumerator.Current;
                    if (this.source.GetSocket(current, out this.array[index].socket))
                    {
                        try
                        {
                            this.dict.Add(this.array[index].name = current, index);
                        }
                        catch (ArgumentException exception)
                        {
                            Debug.LogException(exception, this.script);
                            Debug.Log(current);
                            continue;
                        }
                        index++;
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
            Array.Resize<Element>(ref this.array, index);
            this.version = this.source.SocketsVersion;
        }

        private Socket.ProxyLink MakeProxy(Socket.CameraSpace socket, int index)
        {
            Socket.CameraConversion conversion;
            Type objA = this.source.ProxyScriptType(this.array[index].name);
            if (object.ReferenceEquals(objA, null))
            {
                return null;
            }
            if (!typeof(Socket.Proxy).IsAssignableFrom(objA))
            {
                throw new InvalidProgramException("SocketSource returned a type that did not extend SocketMap.Proxy");
            }
            Socket.ProxyLink top = new Socket.ProxyLink {
                map = this,
                index = index
            };
            Socket.ProxyLink.Push(top);
            Vector3 zero = Vector3.zero;
            Quaternion identity = Quaternion.identity;
            if (this.GetCameraSpace(out conversion))
            {
                socket.root = conversion.Shelf;
                socket.eye = conversion.Eye;
            }
            else
            {
                socket.eye = null;
                socket.root = null;
            }
            try
            {
                zero = socket.position;
                identity = socket.rotation;
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this.script);
            }
            try
            {
                Type[] components = new Type[] { objA };
                top.gameObject = new GameObject(this.array[index].name, components) { transform = { position = zero, rotation = identity } };
            }
            catch
            {
                top.linked = false;
                if (top.gameObject != null)
                {
                    Object.Destroy(top.gameObject);
                }
                throw;
            }
            finally
            {
                Socket.ProxyLink.EnsurePopped(top);
            }
            top.linked = true;
            socket.proxyTransform = top.proxy.transform;
            return top;
        }

        internal static Socket.Map Of(ref Socket.Map member)
        {
            Socket.Map map;
            Of(ref member, out map);
            return map;
        }

        private static bool Of(ref Socket.Map member, out Socket.Map value)
        {
            if (object.ReferenceEquals(member, null))
            {
                value = null;
                return false;
            }
            Socket.Map objA = member.EnsureMap();
            member = objA;
            value = objA;
            return !object.ReferenceEquals(objA, null);
        }

        internal void OnProxyDestroyed(object link)
        {
            this.DestroyProxyLink((Socket.ProxyLink) link);
        }

        private void OnState(Result State)
        {
            bool flag = false;
            bool flag2 = false;
            Socket.CameraConversion cameraSpace = new Socket.CameraConversion();
            for (int i = 0; i < this.array.Length; i++)
            {
                ProxyCheck check;
                this.CheckProxyIndex(i, out check);
                if (check.isCameraSpace)
                {
                    if (!flag)
                    {
                        flag2 = this.GetCameraSpace(out cameraSpace);
                        flag = true;
                    }
                    check.cameraSpace.eye = cameraSpace.Eye;
                    check.cameraSpace.root = cameraSpace.Shelf;
                    check.cameraSpace.proxyTransform = check.proxyTransform;
                }
            }
        }

        private Result PollState()
        {
            Result version;
            if (!this.initialized)
            {
                this.Initialize();
                return Result.Initialized;
            }
            int socketsVersion = this.source.SocketsVersion;
            if (this.version != socketsVersion)
            {
                this.version = socketsVersion;
                version = Result.Version;
            }
            else if (this.forceUpdate)
            {
                version = Result.Forced;
            }
            else
            {
                return Result.Nothing;
            }
            this.forceUpdate = false;
            this.Update(version);
            return version;
        }

        public bool ReplaceSocket(Socket.Slot slot, Socket value)
        {
            if (slot.index < 0)
            {
                return false;
            }
            return ((slot.BelongsTo(this) && (slot.index < this.array.Length)) && this.ValidSlotReplace(slot.index, value));
        }

        public bool ReplaceSocket(int index, Socket value)
        {
            if (index < 0)
            {
                return false;
            }
            return ((this.EnsureState() && (index < this.array.Length)) && this.ValidSlotReplace(index, value));
        }

        public bool ReplaceSocket(string name, Socket value)
        {
            int num;
            return ((this.EnsureState() && this.dict.TryGetValue(name, out num)) && this.ValidSlotReplace(num, value));
        }

        private Result SecureState()
        {
            Result state = this.PollState();
            switch (state)
            {
                case Result.Initialized:
                    this.initialized = true;
                    this.CleanTransforms();
                    break;

                case Result.Version:
                    this.CleanTransforms();
                    break;

                case Result.Forced:
                    break;

                default:
                    return state;
            }
            this.OnState(state);
            return state;
        }

        public void SnapProxies()
        {
            if (this.EnsureState())
            {
                Socket.CameraConversion conversion;
                bool cameraSpace = this.GetCameraSpace(out conversion);
                for (int i = 0; i < this.array.Length; i++)
                {
                    if ((this.array[i].madeLink && this.array[i].link.scriptAlive) && this.array[i].link.linked)
                    {
                        try
                        {
                            Socket.CameraSpace socket = (Socket.CameraSpace) this.array[i].socket;
                            socket.proxyTransform = this.array[i].link.proxy.transform;
                            socket.eye = conversion.Eye;
                            socket.root = conversion.Shelf;
                            if (cameraSpace)
                            {
                                socket.Snap();
                            }
                        }
                        catch (Exception exception)
                        {
                            Debug.LogException(exception, this.array[i].link.proxy);
                        }
                    }
                }
            }
        }

        private void SocketUpdate(ref Socket socket, Socket newSocket)
        {
            Socket objA = socket;
            if (!object.ReferenceEquals(objA, newSocket))
            {
                socket = newSocket;
                if ((objA is Socket.CameraSpace) && (newSocket is Socket.CameraSpace))
                {
                    Socket.CameraSpace space = (Socket.CameraSpace) objA;
                    Socket.CameraSpace space2 = (Socket.CameraSpace) newSocket;
                    space2.root = space.root;
                    space2.eye = space.eye;
                    space2.proxyTransform = space.proxyTransform;
                }
            }
        }

        private void Update(Result Because)
        {
            if (Because == Result.Version)
            {
                this.CleanTransforms();
            }
            int newSize = 0;
            RemoveList<Socket.ProxyLink> removeList = new RemoveList<Socket.ProxyLink>();
            for (int i = 0; i < this.array.Length; i++)
            {
                Socket socket;
                if (this.source.GetSocket(this.array[i].name, out socket))
                {
                    int dstIndex = newSize++;
                    this.ElementUpdate(i, ref this.array[i], dstIndex, ref this.array[dstIndex], socket);
                }
                else
                {
                    this.ElementRemove(ref this.array[i], ref removeList);
                }
            }
            Array.Resize<Element>(ref this.array, newSize);
        }

        private bool ValidSlotReplace(int index, Socket value)
        {
            Socket objB = this.array[index].socket;
            if (object.ReferenceEquals(value, objB))
            {
                return true;
            }
            if ((!object.ReferenceEquals(value, null) && (value.GetType() != objB.GetType())) || !this.source.ReplaceSocket(this.array[index].name, value))
            {
                return false;
            }
            this.forceUpdate = true;
            return this.EnsureState();
        }

        public Socket.CameraConversion cameraConversion
        {
            get
            {
                Socket.CameraConversion conversion;
                this.GetCameraSpace(out conversion);
                return conversion;
            }
        }

        public Socket.Slot this[int index]
        {
            get
            {
                if (((index < 0) || !this.EnsureState()) || (index >= this.array.Length))
                {
                    throw new IndexOutOfRangeException();
                }
                return new Socket.Slot(this, index);
            }
        }

        public Socket.Slot this[string name]
        {
            get
            {
                if (!this.EnsureState())
                {
                    throw new KeyNotFoundException(name);
                }
                return new Socket.Slot(this, this.dict[name]);
            }
        }

        private static Socket.Map NullMap
        {
            get
            {
                return null;
            }
        }

        Socket.Map Socket.Mapped.socketMap
        {
            get
            {
                return this.EnsureMap();
            }
        }

        public int socketCount
        {
            get
            {
                return (!this.EnsureState() ? 0 : this.array.Length);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Element
        {
            public Socket socket;
            public string name;
            public Socket.ProxyLink link;
            public bool madeLink;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Member
        {
            private Socket.Map reference;
            private bool deleted;
            public Socket.Map Get<T>(T outerInstance) where T: Object, Socket.Source
            {
                if (this.deleted)
                {
                    return null;
                }
                return Socket.Map.Get<T>(outerInstance, ref this.reference);
            }

            public bool Get<T>(T outerInstance, out Socket.Map map) where T: Object, Socket.Source
            {
                map = this.Get<T>(outerInstance);
                return !object.ReferenceEquals(map, null);
            }

            public bool DeleteBy<T>(T outerInstance) where T: Object, Socket.Source
            {
                if (this.deleted)
                {
                    return false;
                }
                if (object.ReferenceEquals(this.reference, null))
                {
                    this.deleted = true;
                }
                else
                {
                    if (!object.ReferenceEquals(outerInstance, this.reference.source))
                    {
                        throw new ArgumentException("instance did not match that of which created the map", "outerInstance");
                    }
                    this.deleted = true;
                    try
                    {
                        this.reference.Delete();
                    }
                    catch (Exception exception)
                    {
                        Debug.LogException(exception, outerInstance);
                    }
                    finally
                    {
                        this.reference = null;
                    }
                }
                return true;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ProxyCheck
        {
            public Transform parentOrProxy;
            public Socket.CameraSpace cameraSpace;
            public Socket.ProxyLink proxyLink;
            public int index;
            public bool isCameraSpace;
            public bool isProxy;
            public Transform proxyTransform
            {
                get
                {
                    return (!this.isProxy ? null : this.parentOrProxy);
                }
            }
            public Transform parent
            {
                get
                {
                    return (!this.isProxy ? this.parentOrProxy : this.cameraSpace.parent);
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct Reference
        {
            private Socket.Map reference;
            private Reference(Socket.Map reference)
            {
                this.reference = reference;
            }

            public bool Try(out Socket.Map map)
            {
                return Socket.Map.Of(ref this.reference, out map);
            }

            private bool ByIndex(int index, out Socket.Map map)
            {
                if (index < 0)
                {
                    map = null;
                }
                else if (this.Try(out map) && (index < map.array.Length))
                {
                    return true;
                }
                return false;
            }

            private bool ByKey(string name, out Socket.Map map, out int index)
            {
                if (object.ReferenceEquals(name, null))
                {
                    map = null;
                }
                else if (this.Try(out map))
                {
                    return map.dict.TryGetValue(name, out index);
                }
                index = -1;
                return false;
            }

            private static bool Socket(bool valid, int index, Socket.Map map, out Socket socket)
            {
                if (valid)
                {
                    socket = map.array[index].socket;
                    return true;
                }
                socket = null;
                return false;
            }

            private static bool Name(bool valid, int index, Socket.Map map, out string name)
            {
                if (valid)
                {
                    name = map.array[index].name;
                    return true;
                }
                name = null;
                return false;
            }

            private static bool Proxy(bool valid, int index, Socket.Map map, out Socket.ProxyLink proxyLink)
            {
                if (valid)
                {
                    proxyLink = map.array[index].link;
                    return map.array[index].madeLink;
                }
                proxyLink = null;
                return false;
            }

            public bool Socket(int index, out Socket socket)
            {
                Socket.Map map;
                return Socket(this.ByIndex(index, out map), index, map, out socket);
            }

            public Socket Socket(int index)
            {
                return this.Map.array[index].socket;
            }

            public bool Name(int index, out string name)
            {
                Socket.Map map;
                return Name(this.ByIndex(index, out map), index, map, out name);
            }

            public string Name(int index)
            {
                return this.Map.array[index].name;
            }

            public bool Proxy(int index, out Socket.ProxyLink link)
            {
                Socket.Map map;
                return Proxy(this.ByIndex(index, out map), index, map, out link);
            }

            internal Socket.ProxyLink Proxy(int index)
            {
                return this.Map.array[index].link;
            }

            public bool Socket(string key, out Socket socket)
            {
                Socket.Map map;
                int num;
                return Socket(this.ByKey(key, out map, out num), num, map, out socket);
            }

            public Socket Socket(string key)
            {
                Socket.Map map = this.Map;
                return map.array[map.dict[key]].socket;
            }

            public bool Name(string key, out string name)
            {
                Socket.Map map;
                int num;
                return Name(this.ByKey(key, out map, out num), num, map, out name);
            }

            public string Name(string key)
            {
                Socket.Map map = this.Map;
                return map.array[map.dict[key]].name;
            }

            internal bool Proxy(string key, out Socket.ProxyLink link)
            {
                Socket.Map map;
                int num;
                return Proxy(this.ByKey(key, out map, out num), num, map, out link);
            }

            internal Socket.ProxyLink Proxy(string key)
            {
                Socket.Map map = this.Map;
                return map.array[map.dict[key]].link;
            }

            public Socket.Map Map
            {
                get
                {
                    return Socket.Map.Of(ref this.reference);
                }
            }
            public bool Exists
            {
                get
                {
                    Socket.Map map;
                    return Socket.Map.Of(ref this.reference, out map);
                }
            }
            public bool RefEquals(Socket.Map map)
            {
                return object.ReferenceEquals(this.reference, map);
            }

            public bool Is(Socket.Map map)
            {
                return object.ReferenceEquals(this.Map, map);
            }

            public bool Socket<TSocket>(int index, out TSocket socket) where TSocket: Socket, new()
            {
                Socket socket2;
                bool flag = this.Socket(index, out socket2);
                socket = !flag ? null : (socket2 as TSocket);
                return (flag && (socket2 != null));
            }

            public bool Socket<TSocket>(string name, out TSocket socket) where TSocket: Socket, new()
            {
                Socket socket2;
                bool flag = this.Socket(name, out socket2);
                socket = !flag ? null : (socket2 as TSocket);
                return (flag && (socket2 != null));
            }

            public TSocket Socket<TSocket>(int index) where TSocket: Socket, new()
            {
                return (TSocket) this.Socket(index);
            }

            public TSocket Socket<TSocket>(string name) where TSocket: Socket, new()
            {
                return (TSocket) this.Socket(name);
            }

            public bool SocketIndex(string name, out int index)
            {
                Socket.Map map;
                if (this.Try(out map))
                {
                    return map.dict.TryGetValue(name, out index);
                }
                index = -1;
                return false;
            }

            public int SocketIndex(string name)
            {
                return this.Map.dict[name];
            }

            public static implicit operator Socket.Map.Reference(Socket.Map reference)
            {
                return new Socket.Map.Reference(reference);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RemoveList<T>
        {
            public bool exists;
            public List<T> list;
            public void Add(T item)
            {
                if (!this.exists)
                {
                    this.list = new List<T>();
                }
                this.list.Add(item);
            }
        }

        private enum Result
        {
            Nothing,
            Initialized,
            Version,
            Forced
        }
    }

    public interface Mapped
    {
        Socket.Map socketMap { get; }
    }

    public interface Provider : Socket.Source, Socket.Mapped
    {
    }

    public abstract class Proxy : MonoBehaviour, Socket.Mapped
    {
        [NonSerialized]
        private Transform _transform;
        [NonSerialized]
        private readonly Socket.ProxyLink link = Socket.ProxyLink.Pop();

        public Proxy()
        {
            this.link.proxy = this;
        }

        protected void Awake()
        {
            this._transform = base.transform;
            this.link.scriptAlive = true;
            try
            {
                this.InitializeProxy();
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
            }
        }

        public bool GetSocketMap(out Socket.Map map)
        {
            return this.link.map.Try(out map);
        }

        protected virtual void InitializeProxy()
        {
        }

        protected void OnDestroy()
        {
            if (this.link.scriptAlive)
            {
                this.link.scriptAlive = false;
                try
                {
                    this.UninitializeProxy();
                }
                finally
                {
                    Socket.Map map;
                    if (this.GetSocketMap(out map))
                    {
                        map.OnProxyDestroyed(this.link);
                    }
                    this.link.proxy = null;
                }
            }
        }

        protected virtual void UninitializeProxy()
        {
        }

        public Socket.CameraSpace socket
        {
            get
            {
                Socket.CameraSpace space;
                return ((!this.link.linked || !this.link.map.Socket<Socket.CameraSpace>(this.link.index, out space)) ? null : space);
            }
        }

        public bool socketExists
        {
            get
            {
                return (this.link.linked && this.link.map.Exists);
            }
        }

        public int socketIndex
        {
            get
            {
                return ((!this.link.linked || !this.link.map.Exists) ? -1 : this.link.index);
            }
        }

        public Socket.Map socketMap
        {
            get
            {
                return this.link.map.Map;
            }
        }

        public string socketName
        {
            get
            {
                string str;
                return ((!this.link.linked || !this.link.map.Name(this.link.index, out str)) ? null : str);
            }
        }

        public Transform transform
        {
            get
            {
                return this._transform;
            }
        }
    }

    internal sealed class ProxyLink
    {
        [NonSerialized]
        public GameObject gameObject;
        [NonSerialized]
        public int index = -1;
        [NonSerialized]
        public bool linked;
        [NonSerialized]
        public Socket.Map.Reference map;
        [NonSerialized]
        public Socket.Proxy proxy;
        [NonSerialized]
        public bool scriptAlive;

        public static void EnsurePopped(Socket.ProxyLink top)
        {
            if ((Usage.Stack.Count > 0) && (Usage.Stack.Peek() == top))
            {
                Usage.Stack.Pop();
            }
        }

        public static Socket.ProxyLink Pop()
        {
            return Usage.Stack.Pop();
        }

        public static void Push(Socket.ProxyLink top)
        {
            Usage.Stack.Push(top);
        }

        private static class Usage
        {
            public static readonly Stack<Socket.ProxyLink> Stack = new Stack<Socket.ProxyLink>();
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Slot
    {
        private Socket.Map.Reference m;
        public readonly int index;
        internal Slot(Socket.Map.Reference map, int index)
        {
            this.m = map;
            this.index = index;
        }

        public Socket socket
        {
            get
            {
                return this.m.Socket(this.index);
            }
            set
            {
                if (!this.ReplaceSocket(value))
                {
                    throw new InvalidOperationException("could not replace socket");
                }
            }
        }
        public Transform proxy
        {
            get
            {
                Socket.ProxyLink link;
                return ((!this.m.Proxy(this.index, out link) || (link.proxy == null)) ? null : link.proxy.transform);
            }
        }
        public string name
        {
            get
            {
                return this.m.Name(this.index);
            }
        }
        public bool BelongsTo(Socket.Map map)
        {
            return this.m.Is(map);
        }

        public bool ReplaceSocket(Socket newSocketValue)
        {
            Socket.Map map;
            return (this.m.Try(out map) && map.ReplaceSocket(this.index, newSocketValue));
        }
    }

    public interface Source
    {
        Socket.CameraConversion CameraSpaceSetup();
        bool GetSocket(string name, out Socket socket);
        Type ProxyScriptType(string name);
        bool ReplaceSocket(string name, Socket newValue);

        IEnumerable<string> SocketNames { get; }

        int SocketsVersion { get; }
    }
}

