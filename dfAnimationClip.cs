﻿using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable, AddComponentMenu("Daikon Forge/User Interface/Animation Clip")]
public class dfAnimationClip : MonoBehaviour
{
    [SerializeField]
    private dfAtlas atlas;
    [SerializeField]
    private List<string> sprites = new List<string>();

    public dfAtlas Atlas
    {
        get
        {
            return this.atlas;
        }
        set
        {
            this.atlas = value;
        }
    }

    public List<string> Sprites
    {
        get
        {
            return this.sprites;
        }
    }
}

