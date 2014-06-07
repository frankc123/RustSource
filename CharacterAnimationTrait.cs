using System;
using UnityEngine;

public class CharacterAnimationTrait : CharacterTrait
{
    [SerializeField]
    private string _defaultGroupName = "noitem";
    [SerializeField]
    private MovementAnimationSetup _movementAnimationSetup;

    public string defaultGroupName
    {
        get
        {
            return this._defaultGroupName;
        }
    }

    public MovementAnimationSetup movementAnimationSetup
    {
        get
        {
            return this._movementAnimationSetup;
        }
    }
}

