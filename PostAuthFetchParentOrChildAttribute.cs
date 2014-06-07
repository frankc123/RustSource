using System;

[AttributeUsage(AttributeTargets.Field)]
public sealed class PostAuthFetchParentOrChildAttribute : PostAuthAttribute
{
    private const AuthOptions kFixedOptions = (AuthOptions.SearchReverse | AuthOptions.SearchUp | AuthOptions.SearchDown);

    public PostAuthFetchParentOrChildAttribute(AuthTarg target, bool includeThisGameObject) : this(target, includeThisGameObject, null)
    {
    }

    public PostAuthFetchParentOrChildAttribute(AuthTarg target, string nameMask) : this(target, false, nameMask)
    {
    }

    private PostAuthFetchParentOrChildAttribute(AuthTarg target, bool includeThisGameObject, string nameMask) : base(target, !includeThisGameObject ? (AuthOptions.SearchReverse | AuthOptions.SearchUp | AuthOptions.SearchDown) : (AuthOptions.SearchReverse | AuthOptions.SearchInclusive | AuthOptions.SearchUp | AuthOptions.SearchDown), nameMask)
    {
    }
}

