using Facepunch.Actor;
using UnityEngine;

public class CharacterActorRigTrait : CharacterTrait
{
    [SerializeField]
    private ActorRig _actorRig;

    public ActorRig actorRig
    {
        get
        {
            return this._actorRig;
        }
    }
}

