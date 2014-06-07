using System;

public interface IUseableUpdated : IUseable, IComponentInterface<IUseable, MonoBehaviour, Useable>, IComponentInterface<IUseable, MonoBehaviour>, IComponentInterface<IUseable>
{
    void OnUseUpdate(Useable use);

    UseUpdateFlags UseUpdateFlags { get; }
}

