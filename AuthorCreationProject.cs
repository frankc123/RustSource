﻿using System;
using System.IO;
using UnityEngine;

public sealed class AuthorCreationProject : ScriptableObject
{
    [SerializeField, HideInInspector]
    private string _authorName;
    [HideInInspector, SerializeField]
    private string _folder;
    [HideInInspector, SerializeField]
    private string _project;
    [SerializeField, HideInInspector]
    private string _scene;
    [SerializeField, HideInInspector]
    private string _script;

    public GameObject FindAuthorCreationGameObjectInScene()
    {
        return null;
    }

    public AuthorCreation FindAuthorCreationInScene()
    {
        return null;
    }

    public Stream GetStream(bool write, string filepath)
    {
        return null;
    }

    public Type authorCreationType
    {
        get
        {
            return null;
        }
    }

    public string authorName
    {
        get
        {
            return this._authorName;
        }
    }

    public static AuthorCreationProject current
    {
        get
        {
            return null;
        }
    }

    public string folder
    {
        get
        {
            return this._folder;
        }
    }

    public string folderPath
    {
        get
        {
            return string.Empty;
        }
    }

    public bool isCurrent
    {
        get
        {
            return false;
        }
    }

    public Object monoScript
    {
        get
        {
            return null;
        }
    }

    public string project
    {
        get
        {
            return this._project;
        }
    }

    public string scene
    {
        get
        {
            return this._scene;
        }
    }

    public string scenePath
    {
        get
        {
            return string.Empty;
        }
    }

    public string script
    {
        get
        {
            return this._script;
        }
    }

    public string scriptPath
    {
        get
        {
            return string.Empty;
        }
    }

    public string singletonName
    {
        get
        {
            return string.Empty;
        }
    }
}

