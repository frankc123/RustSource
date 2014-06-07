using System;

[AttributeUsage(AttributeTargets.Field)]
public sealed class PostAuthFetchChildOrParentAttribute : PostAuthAttribute
{
    private const AuthOptions kFixedOptions = (AuthOptions.SearchUp | AuthOptions.SearchDown);

    public PostAuthFetchChildOrParentAttribute(AuthTarg target, bool includeThisGameObject) : this(target, includeThisGameObject, null)
    {
    }

    public PostAuthFetchChildOrParentAttribute(AuthTarg target, string nameMask) : this(target, false, nameMask)
    {
    }

    private PostAuthFetchChildOrParentAttribute(AuthTarg target, bool includeThisGameObject, string nameMask) : base(target, !includeThisGameObject ? (AuthOptions.SearchUp | AuthOptions.SearchDown) : (AuthOptions.SearchInclusive | AuthOptions.SearchUp | AuthOptions.SearchDown), nameMask)
    {
    }
}

