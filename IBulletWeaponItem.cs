using System;

public interface IBulletWeaponItem : IHeldItem, IInventoryItem, IWeaponItem
{
    void ActualReload();

    int cachedCasings { get; set; }

    int clipAmmo { get; set; }

    MagazineDataBlock clipType { get; }

    float nextCasingsTime { get; set; }
}

