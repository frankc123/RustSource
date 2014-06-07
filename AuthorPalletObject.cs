using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

public class AuthorPalletObject
{
    public Creator creator;
    public GUIContent guiContent;
    public Validator validator;

    public bool Create(AuthorCreation creation, out AuthorPeice peice)
    {
        if (this.creator == null)
        {
            peice = null;
            return false;
        }
        peice = this.creator(creation, this);
        return (bool) peice;
    }

    public bool Validate(AuthorCreation creation)
    {
        return ((this.validator == null) || this.validator(creation, this));
    }

    public delegate AuthorPeice Creator(AuthorCreation creation, AuthorPalletObject obj);

    public delegate bool Validator(AuthorCreation creation, AuthorPalletObject obj);
}

