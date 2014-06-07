using System;

[AttributeUsage(AttributeTargets.Field)]
public sealed class PostAuthFetchChildAttribute : PostAuthAttribute
{
    private const AuthOptions kFixedOptions = AuthOptions.SearchDown;

    public PostAuthFetchChildAttribute(AuthTarg target, bool includeThisGameObject) : this(target, includeThisGameObject, null)
    {
    }

    public PostAuthFetchChildAttribute(AuthTarg target, string nameMask) : this(target, false, nameMask)
    {
    }

    private PostAuthFetchChildAttribute(AuthTarg target, bool includeThisGameObject, string nameMask) : base(target, !includeThisGameObject ? AuthOptions.SearchDown : (AuthOptions.SearchInclusive | AuthOptions.SearchDown), nameMask)
    {
    }
}

