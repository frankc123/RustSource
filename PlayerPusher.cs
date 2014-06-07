using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public sealed class PlayerPusher : MonoBehaviour
{
    [NonSerialized]
    private bool _gotRigidbody;
    [NonSerialized]
    private Rigidbody _rigidbody;
    [NonSerialized]
    private HashSet<CCMotor> activeMotors;

    private bool AddMotor(CCMotor motor)
    {
        if (this.activeMotors == null)
        {
            this.activeMotors = new HashSet<CCMotor>();
            this.activeMotors.Add(motor);
            return true;
        }
        if (!this.activeMotors.Add(motor))
        {
            Debug.LogWarning("Already added motor?", this);
            return false;
        }
        return true;
    }

    private bool ContainsMotor(CCMotor motor)
    {
        return ((this.activeMotors != null) && this.activeMotors.Contains(motor));
    }

    private static bool GetCCMotor(Collision collision, out CCMotor ccmotor)
    {
        GameObject gameObject = collision.gameObject;
        if (gameObject.layer == 0x10)
        {
            ccmotor = gameObject.GetComponent<CCMotor>();
            return (bool) ccmotor;
        }
        ccmotor = null;
        return false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        CCMotor motor;
        if (GetCCMotor(collision, out motor) && this.AddMotor(motor))
        {
            try
            {
                motor.OnPushEnter(this.rigidbody, base.collider, collision);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        CCMotor motor;
        if (GetCCMotor(collision, out motor) && this.RemoveMotor(motor))
        {
            try
            {
                motor.OnPushExit(this.rigidbody, base.collider, collision);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        CCMotor motor;
        if (GetCCMotor(collision, out motor) && this.ContainsMotor(motor))
        {
            try
            {
                motor.OnPushStay(this.rigidbody, base.collider, collision);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
            }
        }
    }

    private bool RemoveMotor(CCMotor motor)
    {
        if ((this.activeMotors == null) || !this.activeMotors.Remove(motor))
        {
            return false;
        }
        if (this.activeMotors.Count == 0)
        {
            this.activeMotors = null;
        }
        return true;
    }

    public Rigidbody rigidbody
    {
        get
        {
            if (!this._gotRigidbody)
            {
                this._rigidbody = base.rigidbody;
                this._gotRigidbody = true;
            }
            return this._rigidbody;
        }
    }
}

