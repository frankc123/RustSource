using System;

[AttributeUsage(AttributeTargets.Field)]
public sealed class PostAuthFetchParentAttribute : PostAuthAttribute
{
    private const AuthOptions kFixedOptions = AuthOptions.SearchUp;

    public PostAuthFetchParentAttribute(AuthTarg target, bool includeThisGameObject) : this(target, includeThisGameObject, null)
    {
    }

    public PostAuthFetchParentAttribute(AuthTarg target, string nameMask) : this(target, false, nameMask)
    {
    }

    private PostAuthFetchParentAttribute(AuthTarg target, bool includeThisGameObject, string nameMask) : base(target, !includeThisGameObject ? AuthOptions.SearchUp : (AuthOptions.SearchInclusive | AuthOptions.SearchUp), nameMask)
    {
    }
}

