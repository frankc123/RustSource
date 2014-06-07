using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public sealed class CCTotemPole : CCTotem<CCTotem.TotemPole, CCTotemPole>
{
    [SerializeField]
    private float bottomBufferUnits = 0.1f;
    [NonSerialized]
    private CCTotem.ConfigurationBinder ConfigurationBinder;
    [NonSerialized]
    private bool HasLastGoodConfiguration;
    [SerializeField]
    private float initialHeightFraction = 1f;
    [NonSerialized]
    public ArgumentException LastException;
    [NonSerialized]
    private CCTotem.Configuration LastGoodConfiguration;
    [SerializeField]
    private float maximumHeight = 2.08f;
    [SerializeField]
    private float minimumHeight = 0.6f;
    [SerializeField]
    private CCDesc prefab;
    [NonSerialized]
    public object Tag;

    public event CCTotem.PositionBinder OnBindPosition;

    public event CCTotem.ConfigurationBinder OnConfigurationBinding
    {
        add
        {
            if (!object.ReferenceEquals(this.ConfigurationBinder, value))
            {
                if (!object.ReferenceEquals(this.ConfigurationBinder, null))
                {
                    this.ExecuteAllBindings(false);
                    this.ConfigurationBinder = null;
                }
                this.ConfigurationBinder = value;
                this.ExecuteAllBindings(true);
            }
        }
        remove
        {
            if (object.ReferenceEquals(this.ConfigurationBinder, value))
            {
                this.ExecuteAllBindings(false);
                if (object.ReferenceEquals(this.ConfigurationBinder, value))
                {
                    this.ConfigurationBinder = null;
                }
            }
        }
    }

    private void Awake()
    {
        if (!this.UpdateConfiguration())
        {
            Debug.LogException(this.LastException, this);
        }
        else
        {
            this.CreatePhysics();
        }
    }

    private void BindPositions(CCTotem.PositionPlacement PositionPlacement)
    {
        if (this.OnBindPosition != null)
        {
            try
            {
                this.OnBindPosition(ref PositionPlacement, this.Tag);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
            }
        }
    }

    private void CreatePhysics()
    {
        if (!this.HasLastGoodConfiguration && !this.UpdateConfiguration())
        {
            Debug.LogException(this.LastException, this);
        }
        else
        {
            base.AssignTotemicObject(new CCTotem.TotemPole(ref this.LastGoodConfiguration));
            base.totemicObject.Create();
        }
    }

    internal void DestroyCCDesc(ref CCDesc CCDesc)
    {
        if (CCDesc != null)
        {
            CCDesc cCDesc = CCDesc;
            CCDesc = null;
            this.ExecuteBinding(cCDesc, false);
            Object.Destroy(cCDesc.gameObject);
        }
    }

    internal void ExecuteAllBindings(bool Bind)
    {
        if (this.Exists)
        {
            this.ExecuteBinding(base.totemicObject.CCDesc, Bind);
            for (int i = 0; i < base.totemicObject.Configuration.numRequiredTotemicFigures; i++)
            {
                this.ExecuteBinding(base.totemicObject.TotemicFigures[i].CCDesc, Bind);
            }
        }
    }

    internal void ExecuteBinding(CCDesc CCDesc, bool Bind)
    {
        if ((CCDesc != null) && !object.ReferenceEquals(this.ConfigurationBinder, null))
        {
            try
            {
                this.ConfigurationBinder(Bind, CCDesc, this.Tag);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
            }
        }
    }

    public CCTotem.MoveInfo Move(Vector3 motion)
    {
        return this.Move(motion, this.Height);
    }

    public CCTotem.MoveInfo Move(Vector3 motion, float height)
    {
        CCTotem.TotemPole totemicObject = base.totemicObject;
        if (object.ReferenceEquals(totemicObject, null))
        {
            throw new InvalidOperationException("Exists == false");
        }
        CCTotem.MoveInfo info = totemicObject.Move(motion, height);
        this.BindPositions(info.PositionPlacement);
        return info;
    }

    private void OnDestroy()
    {
        try
        {
            base.OnDestroy();
        }
        finally
        {
            this.OnBindPosition = null;
            this.ConfigurationBinder = null;
            this.Tag = null;
        }
    }

    public bool SmudgeTo(Vector3 worldSkinnedBottom)
    {
        if (!this.Exists)
        {
            return false;
        }
        Vector3 position = base.transform.position;
        if (position != worldSkinnedBottom)
        {
            Vector3 vector3;
            Vector3 vector7;
            Vector3 vector2 = worldSkinnedBottom - position;
            CCDesc cCDesc = base.totemicObject.CCDesc;
            if (cCDesc == null)
            {
                return false;
            }
            vector3.x = vector3.z = 0f;
            vector3.y = (cCDesc.effectiveHeight * 0.5f) - cCDesc.radius;
            Vector3 center = cCDesc.center;
            Vector3 vector5 = cCDesc.OffsetToWorld(center - vector3);
            Vector3 vector6 = cCDesc.OffsetToWorld(center + vector3);
            Vector3 vector8 = cCDesc.OffsetToWorld(center + new Vector3(cCDesc.skinnedRadius, 0f, 0f)) - cCDesc.worldCenter;
            float magnitude = vector8.magnitude;
            float distance = vector2.magnitude;
            float num3 = 1f / distance;
            vector7.x = vector2.x * num3;
            vector7.y = vector2.y * num3;
            vector7.z = vector2.z * num3;
            int layerMask = 0;
            int layer = base.gameObject.layer;
            for (int i = 0; i < 0x20; i++)
            {
                if (!Physics.GetIgnoreLayerCollision(layer, i))
                {
                    layerMask |= ((int) 1) << i;
                }
            }
            if (Physics.CapsuleCast(vector5, vector6, magnitude, vector7, distance, layerMask))
            {
                return false;
            }
            Transform transform = base.totemicObject.CCDesc.transform;
            transform.position += vector2;
            for (int j = 0; j < base.totemicObject.Configuration.numRequiredTotemicFigures; j++)
            {
                Transform transform2 = base.totemicObject.TotemicFigures[j].CCDesc.transform;
                transform2.position += vector2;
            }
            this.BindPositions(new CCTotem.PositionPlacement(base.totemicObject.CCDesc.worldSkinnedBottom, base.totemicObject.CCDesc.worldSkinnedTop, base.totemicObject.CCDesc.transform.position, base.totemicObject.Configuration.poleExpandedHeight));
        }
        return true;
    }

    public void Teleport(Vector3 origin)
    {
        if (this.Exists)
        {
            base.ClearTotemicObject();
        }
        base.transform.position = origin;
        this.CreatePhysics();
    }

    public bool UpdateConfiguration()
    {
        this.LastException = null;
        CCTotem.Initialization members = this.Members;
        try
        {
            this.LastGoodConfiguration = new CCTotem.Configuration(ref members);
            this.HasLastGoodConfiguration = true;
            return true;
        }
        catch (ArgumentException exception)
        {
            this.LastException = exception;
            return false;
        }
    }

    public Vector3 center
    {
        get
        {
            return (!this.Exists ? this.prefab.center : base.totemicObject.center);
        }
    }

    public CollisionFlags collisionFlags
    {
        get
        {
            return (!this.Exists ? CollisionFlags.None : base.totemicObject.collisionFlags);
        }
    }

    public bool Exists
    {
        get
        {
            return !object.ReferenceEquals(base.totemicObject, null);
        }
    }

    [Obsolete("this is the height of the character controller. prefer this.Height")]
    public float height
    {
        get
        {
            return (!this.Exists ? this.prefab.height : base.totemicObject.height);
        }
    }

    public float Height
    {
        get
        {
            return (!this.Exists ? (this.minimumHeight + (this.initialHeightFraction * (this.maximumHeight - this.minimumHeight))) : base.totemicObject.Expansion.Value);
        }
    }

    public bool isGrounded
    {
        get
        {
            return (this.Exists && base.totemicObject.isGrounded);
        }
    }

    public float MaximumHeight
    {
        get
        {
            return this.maximumHeight;
        }
    }

    private CCTotem.Initialization Members
    {
        get
        {
            return new CCTotem.Initialization(this, this.prefab, this.minimumHeight, this.maximumHeight, this.minimumHeight + ((this.maximumHeight - this.minimumHeight) * this.initialHeightFraction), this.bottomBufferUnits);
        }
    }

    public float MinimumHeight
    {
        get
        {
            return this.minimumHeight;
        }
    }

    public float radius
    {
        get
        {
            return (!this.Exists ? this.prefab.radius : base.totemicObject.radius);
        }
    }

    public float slopeLimit
    {
        get
        {
            return (!this.Exists ? this.prefab.slopeLimit : base.totemicObject.slopeLimit);
        }
    }

    public float stepOffset
    {
        get
        {
            return (!this.Exists ? this.prefab.stepOffset : base.totemicObject.stepOffset);
        }
    }

    public Vector3 velocity
    {
        get
        {
            return (!this.Exists ? Vector3.zero : base.totemicObject.velocity);
        }
    }
}

