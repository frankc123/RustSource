using System;

public abstract class PostAuthAttribute : Attribute
{
    private readonly string _nameMask;
    private readonly AuthOptions _options;
    private readonly AuthTarg _target;
    public const AuthOptions kOption_Down = AuthOptions.SearchDown;
    public const AuthOptions kOption_Include = AuthOptions.SearchInclusive;
    public const AuthOptions kOption_NameMask = 4;
    public const AuthOptions kOption_None = 0;
    public const AuthOptions kOption_Reverse = AuthOptions.SearchReverse;
    public const AuthOptions kOption_Up = AuthOptions.SearchUp;

    internal PostAuthAttribute(AuthTarg target, AuthOptions options, string nameMask)
    {
        this._target = target;
        if (!string.IsNullOrEmpty(nameMask))
        {
            this._options = options | 4;
            this._nameMask = nameMask;
        }
        else
        {
            this._options = options;
            this._nameMask = string.Empty;
        }
    }

    public string nameMask
    {
        get
        {
            return this._nameMask;
        }
    }

    public AuthOptions options
    {
        get
        {
            return this._options;
        }
    }

    public AuthTarg target
    {
        get
        {
            return this._target;
        }
    }
}

