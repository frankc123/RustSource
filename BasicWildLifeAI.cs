using Facepunch;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using uLink;
using UnityEngine;

public class BasicWildLifeAI : NetBehaviour, IInterpTimedEventReceiver
{
    protected TransformInterpolator _interp;
    protected Transform _myTransform;
    protected TakeDamage _takeDamage;
    protected BaseAIMovement _wildMove;
    [CompilerGenerated]
    private static Dictionary<string, int> <>f__switch$map7;
    public bool afraidOfDanger = true;
    public bool afraidOfFootsteps = true;
    [SerializeField]
    protected AudioClipArray deathSounds;
    [SerializeField]
    protected AudioClipArray fleeStartSounds;
    [SerializeField]
    protected AudioClipArray idleSounds;
    private const string RPCName_ClientDeath = "ClientDeath";
    private const string RPCName_ClientHealthChange = "ClientHealthChange";
    private const string RPCName_GetNetworkUpdate = "GetNetworkUpdate";
    private const string RPCName_Snd = "Snd";
    [SerializeField]
    protected float runAnimScalar = 1f;
    [SerializeField]
    protected float runSpeed = 3f;
    [SerializeField]
    protected float walkAnimScalar = 1f;
    [SerializeField]
    protected float walkSpeed = 1f;

    protected void Awake()
    {
        this._myTransform = base.transform;
        this._takeDamage = base.GetComponent<TakeDamage>();
        this._wildMove = base.GetComponent<BaseAIMovement>();
        this._interp = base.GetComponent<TransformInterpolator>();
        Object.Destroy(base.GetComponent<BasicWildLifeMovement>());
        Object.Destroy(base.GetComponent<VisNode>());
        this._takeDamage.enabled = false;
    }

    [RPC]
    protected void ClientDeath(Vector3 deadPos, NetworkViewID attackerID, NetworkMessageInfo info)
    {
        this.OnClientDeath(ref deadPos, attackerID, ref info);
    }

    [RPC]
    protected void ClientHealthChange(float newHealth)
    {
        if (this._takeDamage.health > newHealth)
        {
        }
        this._takeDamage.health = newHealth;
    }

    protected void DoClientDeath()
    {
        base.animation[this.GetDeathAnim()].wrapMode = WrapMode.ClampForever;
        base.animation.CrossFade(this.GetDeathAnim(), 0.2f);
        this._takeDamage.health = 0f;
    }

    public virtual string GetDeathAnim()
    {
        return "death";
    }

    protected float GetMoveSpeedForAnim()
    {
        Vector3 vector;
        this._interp.SampleWorldVelocity(out vector);
        return vector.magnitude;
    }

    [RPC]
    protected void GetNetworkUpdate(Vector3 pos, Angle2 rot, NetworkMessageInfo info)
    {
        Quaternion rotation = (Quaternion) rot;
        this.OnNetworkUpdate(ref pos, ref rotation, ref info);
    }

    protected float GetRunAnimScalar()
    {
        return this.runAnimScalar;
    }

    protected float GetWalkAnimScalar()
    {
        return this.walkAnimScalar;
    }

    void IInterpTimedEventReceiver.OnInterpTimedEvent()
    {
        if (!this.OnInterpTimedEvent())
        {
            InterpTimedEvent.MarkUnhandled();
        }
    }

    protected void OnClientDeath(ref Vector3 deathPosition, NetworkViewID attackerNetViewID, ref NetworkMessageInfo info)
    {
        Vector3 vector;
        Vector3 vector2;
        TransformHelpers.GetGroundInfo(deathPosition + new Vector3(0f, 0.25f, 0f), 10f, out vector, out vector2);
        deathPosition = vector;
        Quaternion rot = TransformHelpers.LookRotationForcedUp((Vector3) (this._myTransform.rotation * Vector3.forward), vector2);
        this._interp.SetGoals(deathPosition, rot, info.timestamp);
        if (attackerNetViewID.isMine)
        {
            this.DoClientDeath();
        }
        else
        {
            InterpTimedEvent.Queue(this, "DEATH", ref info);
        }
    }

    protected void OnDestroy()
    {
        InterpTimedEvent.Remove(this);
    }

    protected virtual bool OnInterpTimedEvent()
    {
        string tag = InterpTimedEvent.Tag;
        if (tag != null)
        {
            int num;
            if (<>f__switch$map7 == null)
            {
                Dictionary<string, int> dictionary = new Dictionary<string, int>(2);
                dictionary.Add("DEATH", 0);
                dictionary.Add("SOUND", 1);
                <>f__switch$map7 = dictionary;
            }
            if (<>f__switch$map7.TryGetValue(tag, out num))
            {
                if (num == 0)
                {
                    this.DoClientDeath();
                    return true;
                }
                if (num == 1)
                {
                    this.PlaySnd(InterpTimedEvent.Argument<int>(0));
                    return true;
                }
            }
        }
        return false;
    }

    protected void OnNetworkUpdate(ref Vector3 origin, ref Quaternion rotation, ref NetworkMessageInfo info)
    {
        this._wildMove.ProcessNetworkUpdate(ref origin, ref rotation);
        this._interp.SetGoals(origin, rotation, info.timestamp);
    }

    protected virtual bool PlaySnd(int type)
    {
        AudioClip clip = null;
        float volume = 1f;
        float minDistance = 5f;
        float maxDistance = 20f;
        if (type == 0)
        {
            if (this.idleSounds != null)
            {
                clip = this.idleSounds[Random.Range(0, this.idleSounds.Length)];
            }
            volume = 0.4f;
            minDistance = 0.25f;
            maxDistance = 8f;
        }
        else if (type == 3)
        {
            if (this.fleeStartSounds != null)
            {
                clip = this.fleeStartSounds[Random.Range(0, this.fleeStartSounds.Length)];
            }
            volume = 0.9f;
            minDistance = 1.25f;
            maxDistance = 10f;
        }
        else
        {
            if (type != 4)
            {
                return false;
            }
            if (this.deathSounds != null)
            {
                clip = this.deathSounds[Random.Range(0, this.deathSounds.Length)];
            }
            volume = 1f;
            minDistance = 2.25f;
            maxDistance = 20f;
        }
        if (clip != null)
        {
            clip.PlayLocal(this.transform, Vector3.zero, volume, minDistance, maxDistance);
        }
        return true;
    }

    [RPC]
    protected void Snd(byte type, NetworkMessageInfo info)
    {
        try
        {
            object[] args = new object[] { (int) type };
            InterpTimedEvent.Queue(this, "SOUND", ref info, args);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            Debug.LogWarning("Running emergency dump because of previous exception in Snd", this);
            InterpTimedEvent.EMERGENCY_DUMP(true);
        }
    }

    protected void uLink_OnNetworkInstantiate(NetworkMessageInfo info)
    {
        this._interp.running = true;
    }

    public Transform transform
    {
        get
        {
            return this._myTransform;
        }
    }

    public enum AISound : byte
    {
        Afraid = 3,
        Attack = 2,
        Chase = 5,
        ChaseClose = 6,
        Death = 4,
        Idle = 0,
        Warn = 1
    }
}

