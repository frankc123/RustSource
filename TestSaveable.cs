﻿using RustProto;
using System;
using UnityEngine;

public class TestSaveable : MonoBehaviour, IServerSaveable
{
    public void ReadObjectSave(ref SavedObject saveobj)
    {
    }

    public void WriteObjectSave(ref SavedObject.Builder saveobj)
    {
    }
}

