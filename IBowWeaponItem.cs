using System;

public interface IBowWeaponItem : IHeldItem, IInventoryItem, IWeaponItem
{
    void ArrowReportHit(IDMain hitMain, ArrowMovement arrow);
    void ArrowReportMiss(ArrowMovement arrow);
    IInventoryItem FindAmmo();
    void MakeReadyIn(float delay);

    bool arrowDrawn { get; set; }

    float completeDrawTime { get; set; }

    int currentArrowID { get; set; }

    bool tired { get; set; }
}

