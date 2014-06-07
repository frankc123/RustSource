using Facepunch;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

[InterfaceDriverComponent(typeof(IUseable), "_implementation", "implementation", SearchRoute=InterfaceSearchRoute.GameObject, UnityType=typeof(MonoBehaviour), AlwaysSaveDisabled=true)]
public sealed class Useable : MonoBehaviour, IComponentInterfaceDriver<IUseable, MonoBehaviour, Useable>
{
    [NonSerialized]
    private bool _awoke;
    [SerializeField]
    private MonoBehaviour _implementation;
    [NonSerialized]
    private bool _implemented;
    [NonSerialized]
    private Character _user;
    [NonSerialized]
    private FunctionCallState callState;
    [NonSerialized]
    private bool canCheck;
    [NonSerialized]
    private bool canUpdate;
    [NonSerialized]
    private bool canUse;
    private static bool hasException;
    [NonSerialized]
    private MonoBehaviour implementation;
    [NonSerialized]
    private bool inDestroy;
    [NonSerialized]
    private bool inKillCallback;
    private static Exception lastException;
    [NonSerialized]
    private CharacterDeathSignal onDeathCallback;
    [NonSerialized]
    private UseUpdateFlags updateFlags;
    [NonSerialized]
    private IUseable use;
    [NonSerialized]
    private IUseableChecked useCheck;
    [NonSerialized]
    private IUseableNotifyDecline useDecline;
    [NonSerialized]
    private IUseableUpdated useUpdate;
    [NonSerialized]
    private bool wantDeclines;

    public event UseExitCallback onUseExited;

    private void Awake()
    {
        if (!this._awoke)
        {
            try
            {
                this.Refresh();
            }
            finally
            {
                this._awoke = true;
            }
        }
    }

    private static void ClearException(bool got)
    {
        if (!got)
        {
            Debug.LogWarning("Nothing got previous now clearing exception \r\n" + lastException);
        }
        lastException = null;
        hasException = false;
    }

    public bool Eject()
    {
        UseExitReason manual;
        EnsureServer();
        if (((int) this.callState) != 0)
        {
            if (((int) this.callState) != 4)
            {
                Debug.LogWarning("Some how Eject got called from a call stack originating with " + this.callState + " fix your script to not do this.", this);
                return false;
            }
            manual = UseExitReason.Manual;
        }
        else
        {
            manual = !this.inDestroy ? (!this.inKillCallback ? UseExitReason.Forced : UseExitReason.Killed) : UseExitReason.Destroy;
        }
        if (this._user != null)
        {
            try
            {
                if (this.implementation != null)
                {
                    try
                    {
                        this.callState = FunctionCallState.Eject;
                        this.use.OnUseExit(this, manual);
                    }
                    finally
                    {
                        try
                        {
                            this.InvokeUseExitCallback();
                        }
                        finally
                        {
                            this.callState = FunctionCallState.None;
                        }
                    }
                }
                else
                {
                    Debug.LogError("The IUseable has been destroyed with a user on it. IUseable should ALWAYS call UseableUtility.OnDestroy within the script's OnDestroy message first thing! " + base.gameObject, this);
                }
                return true;
            }
            finally
            {
                this.UnlatchUse();
                this._user = null;
            }
        }
        return false;
    }

    private static void EnsureServer()
    {
        throw new InvalidOperationException("A function ( Enter, Exit or Eject ) in Useable was called client side. Should have only been called server side.");
    }

    private UseResponse Enter(Character attempt, UseEnterRequest request)
    {
        if (!this.canUse)
        {
            return UseResponse.Fail_NotIUseable;
        }
        EnsureServer();
        if (((int) this.callState) != 0)
        {
            Debug.LogWarning("Some how Enter got called from a call stack originating with " + this.callState + " fix your script to not do this.", this);
            return UseResponse.Fail_InvalidOperation;
        }
        if (hasException)
        {
            ClearException(false);
        }
        if (attempt == null)
        {
            return UseResponse.Fail_NullOrMissingUser;
        }
        if (attempt.signaledDeath)
        {
            return UseResponse.Fail_UserDead;
        }
        if (this._user == null)
        {
            if (this.implementation != null)
            {
                try
                {
                    UseResponse response;
                    this.callState = FunctionCallState.Enter;
                    if (this.canCheck)
                    {
                        try
                        {
                            response = (UseResponse) this.useCheck.CanUse(attempt, request);
                        }
                        catch (Exception exception)
                        {
                            lastException = exception;
                            return UseResponse.Fail_CheckException;
                        }
                        if (((int) response) != 1)
                        {
                            if (response.Succeeded())
                            {
                                Debug.LogError("A IUseableChecked return a invalid value that should have cause success [" + response + "], but it was not UseCheck.Success! fix your script.", this.implementation);
                                return UseResponse.Fail_Checked_BadResult;
                            }
                            if (this.wantDeclines)
                            {
                                try
                                {
                                    this.useDecline.OnUseDeclined(attempt, response, request);
                                }
                                catch (Exception exception2)
                                {
                                    Debug.LogError(string.Concat(new object[] { "Caught exception in OnUseDeclined \r\n (response was ", response, ")", exception2 }), this.implementation);
                                }
                            }
                            return response;
                        }
                    }
                    else
                    {
                        response = UseResponse.Pass_Unchecked;
                    }
                    try
                    {
                        this._user = attempt;
                        this.use.OnUseEnter(this);
                    }
                    catch (Exception exception3)
                    {
                        this._user = null;
                        Debug.LogError("Exception thrown during Useable.Enter. Object not set as used!\r\n" + exception3, attempt);
                        lastException = exception3;
                        return UseResponse.Fail_EnterException;
                    }
                    if (response.Succeeded())
                    {
                        this.LatchUse();
                    }
                    return response;
                }
                finally
                {
                    this.callState = FunctionCallState.None;
                }
            }
            return UseResponse.Fail_Destroyed;
        }
        if (this._user == attempt)
        {
            if (this.wantDeclines && (this.implementation != null))
            {
                try
                {
                    this.useDecline.OnUseDeclined(attempt, UseResponse.Fail_Redundant, request);
                }
                catch (Exception exception4)
                {
                    Debug.LogError("Caught exception in OnUseDeclined \r\n (response was Fail_Redundant)" + exception4, this.implementation);
                }
            }
            return UseResponse.Fail_Redundant;
        }
        if (this.wantDeclines && (this.implementation != null))
        {
            try
            {
                this.useDecline.OnUseDeclined(attempt, UseResponse.Fail_Vacancy, request);
            }
            catch (Exception exception5)
            {
                Debug.LogError("Caught exception in OnUseDeclined \r\n (response was Fail_Vacancy)" + exception5, this.implementation);
            }
        }
        return UseResponse.Fail_Vacancy;
    }

    public UseResponse EnterFromContext(Character attempt)
    {
        return this.Enter(attempt, UseEnterRequest.Context);
    }

    public UseResponse EnterFromElsewhere(Character attempt)
    {
        return this.Enter(attempt, UseEnterRequest.Elsewhere);
    }

    public bool Exit(Character attempt)
    {
        EnsureServer();
        if (((int) this.callState) != 0)
        {
            Debug.LogWarning("Some how Exit got called from a call stack originating with " + this.callState + " fix your script to not do this.", this);
            return false;
        }
        if ((attempt == this._user) && (attempt != null))
        {
            try
            {
                if (this.implementation != null)
                {
                    try
                    {
                        this.callState = FunctionCallState.Exit;
                        this.use.OnUseExit(this, UseExitReason.Manual);
                    }
                    finally
                    {
                        this.InvokeUseExitCallback();
                        this.callState = FunctionCallState.None;
                    }
                }
                return true;
            }
            finally
            {
                this._user = null;
                this.UnlatchUse();
            }
        }
        return false;
    }

    public static bool GetLastException(out Exception exception)
    {
        return GetLastException(out exception, false);
    }

    public static bool GetLastException<E>(out E exception) where E: Exception
    {
        return GetLastException<E>(out exception, false);
    }

    public static bool GetLastException(out Exception exception, bool doNotClear)
    {
        if (hasException)
        {
            exception = lastException;
            if (!doNotClear)
            {
                ClearException(true);
            }
            return true;
        }
        exception = null;
        return true;
    }

    public static bool GetLastException<E>(out E exception, bool doNotClear) where E: Exception
    {
        if (hasException && (lastException is E))
        {
            exception = (E) lastException;
            if (!doNotClear)
            {
                ClearException(true);
            }
            return true;
        }
        exception = null;
        return false;
    }

    private void InvokeUseExitCallback()
    {
        if (this.onUseExited != null)
        {
            this.onUseExited(this, ((int) this.callState) == 3);
        }
    }

    private void KilledCallback(Character user, CharacterDeathSignalReason reason)
    {
        if (user == null)
        {
            Debug.LogError("Somehow KilledCallback got a null", this);
        }
        if (user != this._user)
        {
            Debug.LogError("Some callback invoked kill callback on the Useable but it was not being used by it", user);
        }
        else
        {
            try
            {
                this.inKillCallback = true;
                if (!this.Eject())
                {
                    Debug.LogWarning("Failure to eject??", this);
                }
            }
            catch (Exception exception)
            {
                Debug.LogError("Exception in Eject while inside a killed callback\r\n" + exception, user);
            }
            finally
            {
                this.inKillCallback = false;
            }
        }
    }

    private void LatchUse()
    {
        this._user.signal_death += this.onDeathCallback;
        base.enabled = (this.updateFlags & UseUpdateFlags.UpdateWithUser) == UseUpdateFlags.UpdateWithUser;
    }

    private void OnDestroy()
    {
        this.inDestroy = true;
        if (this._user != null)
        {
            this.Eject();
        }
        this.canCheck = false;
        this.canUpdate = false;
        this.canUse = false;
        this.wantDeclines = false;
        this.use = null;
        this.useUpdate = null;
        this.useCheck = null;
        this.useDecline = null;
    }

    private void OnEnable()
    {
        Debug.LogError("Something is trying to enable useable on client.", this);
        base.enabled = false;
    }

    private void Refresh()
    {
        this.implementation = this._implementation;
        this._implementation = null;
        this.use = this.implementation as IUseable;
        this.canUse = this.use != null;
        if (this.canUse)
        {
            base.enabled = false;
            this.useDecline = null;
            this.useCheck = null;
            this.updateFlags = UseUpdateFlags.None;
            IUseableAwake implementation = this.implementation as IUseableAwake;
            if (implementation != null)
            {
                implementation.OnUseableAwake(this);
            }
        }
        else
        {
            Debug.LogWarning("implementation is null or does not implement IUseable", this);
        }
    }

    private void Reset()
    {
        foreach (MonoBehaviour behaviour in base.GetComponents<MonoBehaviour>())
        {
            if (behaviour is IUseable)
            {
                this._implementation = behaviour;
            }
        }
    }

    private void RunUpdate()
    {
        FunctionCallState callState = this.callState;
        try
        {
            this.callState = FunctionCallState.OnUseUpdate;
            this.useUpdate.OnUseUpdate(this);
        }
        catch (Exception exception)
        {
            Debug.LogError("Inside OnUseUpdate\r\n" + exception, this.implementation);
        }
        finally
        {
            this.callState = callState;
        }
    }

    private void UnlatchUse()
    {
        try
        {
            if (this._user != null)
            {
                this._user.signal_death -= this.onDeathCallback;
            }
        }
        catch (Exception exception)
        {
            Debug.LogError("Exception caught during unlatch\r\n" + exception, this);
        }
        finally
        {
            this._user = null;
        }
    }

    private void Update()
    {
        if (this._user == null)
        {
            if ((this.updateFlags & UseUpdateFlags.UpdateWithoutUser) == UseUpdateFlags.UpdateWithoutUser)
            {
                if (this.implementation != null)
                {
                    this.RunUpdate();
                }
                else
                {
                    base.enabled = false;
                }
            }
            else
            {
                Debug.LogWarning("Most likely the user was destroyed without being set up properly!", this);
                base.enabled = false;
            }
        }
        else if (this.implementation != null)
        {
            this.RunUpdate();
        }
        else
        {
            base.enabled = false;
        }
    }

    public Useable driver
    {
        get
        {
            return this;
        }
    }

    public bool exists
    {
        get
        {
            return (this._implemented && (this._implemented = (bool) this.implementation));
        }
    }

    public MonoBehaviour implementor
    {
        get
        {
            if (!this._awoke)
            {
                try
                {
                    this.Refresh();
                }
                finally
                {
                    this._awoke = true;
                }
            }
            return this.implementation;
        }
    }

    public IUseable @interface
    {
        get
        {
            if (!this._awoke)
            {
                try
                {
                    this.Refresh();
                }
                finally
                {
                    this._awoke = true;
                }
            }
            return this.use;
        }
    }

    public bool occupied
    {
        get
        {
            return (bool) this._user;
        }
    }

    public Character user
    {
        get
        {
            return this._user;
        }
    }

    private enum FunctionCallState : sbyte
    {
        Eject = 3,
        Enter = 1,
        Exit = 2,
        None = 0,
        OnUseUpdate = 4
    }

    public delegate void UseExitCallback(Useable useable, bool wasEjected);
}

