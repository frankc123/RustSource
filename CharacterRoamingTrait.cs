using System;
using UnityEngine;

public class CharacterRoamingTrait : CharacterTrait
{
    [SerializeField]
    private bool _allowed = true;
    [SerializeField]
    private float _fleeSpeed = 9f;
    [SerializeField]
    private float _maxFleeDistance = 40f;
    [SerializeField]
    private int _maxIdleMilliseconds = 0x1f40;
    [SerializeField]
    private float _maxRoamAngle = 180f;
    [SerializeField]
    private float _maxRoamDistance = 20f;
    [SerializeField]
    private float _minFleeDistance = 21f;
    [SerializeField]
    private int _minIdleMilliseconds = 0x7d0;
    [SerializeField]
    private float _minRoamAngle = -180f;
    [SerializeField]
    private float _minRoamDistance = 10f;
    [SerializeField]
    private int _retryFromFailureMilliseconds = 800;
    [SerializeField]
    private float _roamRadius = 80f;
    [SerializeField]
    private float _runSpeed = 6f;
    [SerializeField]
    private float _walkSpeed = 1.8f;

    public bool allowed
    {
        get
        {
            return this._allowed;
        }
    }

    public float fleeSpeed
    {
        get
        {
            return this._fleeSpeed;
        }
    }

    public float maxFleeDistance
    {
        get
        {
            return (!this._allowed ? 0f : this._maxFleeDistance);
        }
    }

    public int maxIdleMilliseconds
    {
        get
        {
            return this._maxIdleMilliseconds;
        }
    }

    public float maxIdleSeconds
    {
        get
        {
            return (float) (((double) this._maxIdleMilliseconds) / 1000.0);
        }
    }

    public float maxRoamAngle
    {
        get
        {
            return (!this._allowed ? 0f : this._maxRoamAngle);
        }
    }

    public float maxRoamDistance
    {
        get
        {
            return (!this._allowed ? 0f : this._maxRoamDistance);
        }
    }

    public float minFleeDistance
    {
        get
        {
            return (!this._allowed ? 0f : this._minFleeDistance);
        }
    }

    public int minIdleMilliseconds
    {
        get
        {
            return this._minIdleMilliseconds;
        }
    }

    public float minIdleSeconds
    {
        get
        {
            return (float) (((double) this._minIdleMilliseconds) / 1000.0);
        }
    }

    public float minRoamAngle
    {
        get
        {
            return (!this._allowed ? 0f : this._minRoamAngle);
        }
    }

    public float minRoamDistance
    {
        get
        {
            return (!this._allowed ? 0f : this._minRoamDistance);
        }
    }

    public float randomFleeDistance
    {
        get
        {
            return (!this._allowed ? 0f : ((this._minFleeDistance != this._maxFleeDistance) ? (this._minFleeDistance + ((this._maxFleeDistance - this._minFleeDistance) * Random.value)) : this._minFleeDistance));
        }
    }

    public Vector3 randomFleeVector
    {
        get
        {
            Vector3 vector;
            vector.y = 0f;
            if (this._allowed)
            {
                float randomFleeDistance = this.randomFleeDistance;
                float f = this.randomRoamAngle * 0.01745329f;
                vector.x = Mathf.Sin(f) * randomFleeDistance;
                vector.z = Mathf.Cos(f) * randomFleeDistance;
                return vector;
            }
            vector.x = 0f;
            vector.z = 0f;
            return vector;
        }
    }

    public int randomIdleMilliseconds
    {
        get
        {
            return ((this._minIdleMilliseconds != this._maxIdleMilliseconds) ? ((this._minIdleMilliseconds >= this._maxIdleMilliseconds) ? Random.Range(this._maxIdleMilliseconds, this._minIdleMilliseconds + 1) : Random.Range(this._minIdleMilliseconds, this._maxIdleMilliseconds + 1)) : this._minIdleMilliseconds);
        }
    }

    public float randomIdleSeconds
    {
        get
        {
            return (float) (((double) this.randomIdleMilliseconds) / 1000.0);
        }
    }

    public float randomRoamAngle
    {
        get
        {
            return (!this._allowed ? 0f : ((this._maxRoamAngle != this._minRoamAngle) ? (this._minRoamAngle + ((this._maxRoamAngle - this._minRoamAngle) * Random.value)) : this._minRoamAngle));
        }
    }

    public float randomRoamDistance
    {
        get
        {
            return (!this._allowed ? 0f : ((this._minRoamDistance != this._maxRoamDistance) ? (this._minRoamDistance + ((this._maxRoamDistance - this._minRoamDistance) * Random.value)) : this._minRoamDistance));
        }
    }

    public Vector3 randomRoamNormal
    {
        get
        {
            Vector3 vector;
            vector.y = 0f;
            if (this._allowed)
            {
                float f = this.randomRoamAngle * 0.01745329f;
                vector.x = Mathf.Sin(f);
                vector.z = Mathf.Cos(f);
                return vector;
            }
            vector.x = 0f;
            vector.z = 0f;
            return vector;
        }
    }

    public Vector3 randomRoamVector
    {
        get
        {
            Vector3 vector;
            vector.y = 0f;
            if (this._allowed)
            {
                float randomRoamDistance = this.randomRoamDistance;
                float f = this.randomRoamAngle * 0.01745329f;
                vector.x = Mathf.Sin(f) * randomRoamDistance;
                vector.z = Mathf.Cos(f) * randomRoamDistance;
                return vector;
            }
            vector.x = 0f;
            vector.z = 0f;
            return vector;
        }
    }

    public int retryFromFailureMilliseconds
    {
        get
        {
            return this._retryFromFailureMilliseconds;
        }
    }

    public float retryFromFailureSeconds
    {
        get
        {
            return (float) (((double) this._retryFromFailureMilliseconds) / 1000.0);
        }
    }

    public float roamRadius
    {
        get
        {
            return this._roamRadius;
        }
    }

    public float runSpeed
    {
        get
        {
            return this._runSpeed;
        }
    }

    public float walkSpeed
    {
        get
        {
            return this._walkSpeed;
        }
    }
}

