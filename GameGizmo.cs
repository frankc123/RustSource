using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class GameGizmo : ScriptableObject
{
    [SerializeField]
    private Color _bad = Color.red;
    [SerializeField]
    private bool _castShadows;
    [SerializeField]
    private Color _good = Color.green;
    private HashSet<Instance> _instances;
    [SerializeField]
    private int _layer;
    [SerializeField]
    private Material[] _materials;
    [SerializeField]
    private float _maxAlpha = 1f;
    [SerializeField]
    private Mesh _mesh;
    [SerializeField]
    private float _minAlpha = 0.9f;
    [SerializeField]
    private bool _receiveShadows;
    [SerializeField]
    private Vector3 alternateArrowDirection;

    protected virtual Instance ConstructInstance()
    {
        return new Instance(this);
    }

    public bool Create<TInstance>(out TInstance instance) where TInstance: Instance
    {
        Instance instance2;
        if (this.CreateInstance(out instance2, typeof(TInstance)))
        {
            instance = (TInstance) instance2;
            return true;
        }
        instance = null;
        return false;
    }

    private bool CreateInstance(out Instance instance, Type type)
    {
        try
        {
            instance = this.ConstructInstance();
            if (object.ReferenceEquals(instance, null))
            {
                return false;
            }
            if (this._instances == null)
            {
                this._instances = new HashSet<Instance>();
            }
            this._instances.Add(instance);
            if (!type.IsAssignableFrom(instance.GetType()))
            {
                this.DestroyInstance(instance);
                throw new InvalidCastException();
            }
        }
        catch (Exception exception)
        {
            Debug.LogException(exception, this);
            instance = null;
            return false;
        }
        return true;
    }

    protected virtual void DeconstructInstance(Instance instance)
    {
    }

    public bool Destroy<TInstance>(ref TInstance instance) where TInstance: Instance
    {
        if (this.DestroyInstance((TInstance) instance))
        {
            instance = null;
            return true;
        }
        return false;
    }

    private bool DestroyInstance(Instance instance)
    {
        if ((object.ReferenceEquals(instance, null) || (this._instances == null)) || !this._instances.Remove(instance))
        {
            return false;
        }
        try
        {
            instance.ClearResources();
            this.DeconstructInstance(instance);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception, this);
        }
        return true;
    }

    public Color badColor
    {
        get
        {
            return this._bad;
        }
    }

    public Color goodColor
    {
        get
        {
            return this._good;
        }
    }

    public float maxAlpha
    {
        get
        {
            return this._maxAlpha;
        }
    }

    public float minAlpha
    {
        get
        {
            return this._minAlpha;
        }
    }

    public class Instance
    {
        private Transform _parent;
        [NonSerialized]
        public MeshRenderer carrierRenderer;
        [NonSerialized]
        public readonly GameGizmo gameGizmo;
        protected bool hideMesh;
        [NonSerialized]
        public Vector3 localPosition = Vector3.zero;
        [NonSerialized]
        public Quaternion localRotation = Quaternion.identity;
        [NonSerialized]
        public Vector3 localScale = Vector3.one;
        [NonSerialized]
        public Matrix4x4? overrideMatrix;
        [NonSerialized]
        public readonly MaterialPropertyBlock propertyBlock;
        private List<Object> resources = new List<Object>();
        protected Matrix4x4? ultimateMatrix;

        protected internal Instance(GameGizmo gizmo)
        {
            this.gameGizmo = gizmo;
            this.propertyBlock = new MaterialPropertyBlock();
        }

        public void AddResourceToDelete(Object resource)
        {
            if (resource != null)
            {
                if (this.resources == null)
                {
                }
                (this.resources = new List<Object>()).Add(resource);
            }
        }

        internal void ClearResources()
        {
            List<Object> resources = this.resources;
            if (resources != null)
            {
                this.resources = null;
                foreach (Object obj2 in resources)
                {
                    Object.Destroy(obj2);
                }
            }
        }

        protected Matrix4x4 DefaultMatrix()
        {
            Matrix4x4 matrixx = Matrix4x4.TRS(this.localPosition, this.localRotation, this.localScale);
            if (this._parent != null)
            {
                matrixx = this._parent.localToWorldMatrix * matrixx;
            }
            return matrixx;
        }

        public void Render()
        {
            this.Render(false, null);
        }

        public void Render(Camera camera)
        {
            this.Render((bool) camera, camera);
        }

        protected virtual void Render(bool useCamera, Camera camera)
        {
            if (!this.hideMesh)
            {
                Mesh mesh = this.gameGizmo._mesh;
                if (mesh != null)
                {
                    int num;
                    Material[] materialArray = this.gameGizmo._materials;
                    if ((materialArray != null) && ((num = materialArray.Length) != 0))
                    {
                        Matrix4x4? nullable2;
                        Matrix4x4? ultimateMatrix = this.ultimateMatrix;
                        Matrix4x4 matrix = !ultimateMatrix.HasValue ? (!(nullable2 = this.overrideMatrix).HasValue ? this.DefaultMatrix() : nullable2.Value) : ultimateMatrix.Value;
                        if (this.gameGizmo.alternateArrowDirection != Vector3.zero)
                        {
                            matrix *= Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(this.gameGizmo.alternateArrowDirection), Vector3.one);
                        }
                        for (int i = 0; i < mesh.subMeshCount; i++)
                        {
                            Graphics.DrawMesh(mesh, matrix, materialArray[i % num], this.gameGizmo._layer, camera, i, this.propertyBlock, this.gameGizmo._castShadows, this.gameGizmo._receiveShadows);
                        }
                    }
                }
            }
        }

        protected bool castShadows
        {
            get
            {
                return this.gameGizmo._castShadows;
            }
        }

        protected int layer
        {
            get
            {
                return this.gameGizmo._layer;
            }
        }

        public Transform parent
        {
            get
            {
                return this._parent;
            }
            set
            {
                if (value != this._parent)
                {
                    if (value != null)
                    {
                        this.localPosition = value.InverseTransformPoint(this.position);
                        this.localRotation = Quaternion.Inverse(value.rotation) * this.rotation;
                        this._parent = value;
                    }
                    else
                    {
                        this._parent = null;
                    }
                }
            }
        }

        public Vector3 position
        {
            get
            {
                return ((this._parent == null) ? this.localPosition : this._parent.TransformPoint(this.localPosition));
            }
            set
            {
                if (this._parent != null)
                {
                    this.localPosition = this._parent.InverseTransformPoint(value);
                }
                else
                {
                    this.localPosition = value;
                }
            }
        }

        protected bool receiveShadows
        {
            get
            {
                return this.gameGizmo._receiveShadows;
            }
        }

        public Quaternion rotation
        {
            get
            {
                return ((this._parent == null) ? this.localRotation : (this.localRotation * this._parent.rotation));
            }
            set
            {
                if (this._parent != null)
                {
                    this.localRotation = Quaternion.Inverse(this._parent.rotation) * value;
                }
                else
                {
                    this.localRotation = value;
                }
            }
        }
    }
}

