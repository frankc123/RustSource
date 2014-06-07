using System;
using UnityEngine;

public class RPOSInvCellManager : MonoBehaviour
{
    public RPOSInventoryCell[] _inventoryCells;
    public int CellIndexStart;
    public int CellOffsetX;
    public int CellOffsetY;
    public GameObject CellPrefab;
    public int CellSize = 0x60;
    public int CellSpacing = 10;
    public bool CenterFromCells;
    public Inventory displayInventory;
    private bool generatedCells;
    public bool NumberedCells;
    public int NumCellsHorizontal;
    public int NumCellsVertical;
    public bool SpawnCells;

    private void Awake()
    {
        if (this.SpawnCells)
        {
            this.CreateCellsOnGameObject(null, base.gameObject);
        }
    }

    protected virtual void CreateCellsOnGameObject(Inventory inven, GameObject parent)
    {
        int count;
        int end;
        if (inven != null)
        {
            Inventory.Slot.Range range;
            inven.GetSlotsOfKind(Inventory.Slot.Kind.Default, out range);
            count = range.Count;
            end = range.End;
        }
        else
        {
            count = this.GetNumCells();
            end = 0x7fffffff;
        }
        Array.Resize<RPOSInventoryCell>(ref this._inventoryCells, count);
        float x = this.CellPrefab.GetComponent<RPOSInventoryCell>()._background.transform.localScale.x;
        float y = this.CellPrefab.GetComponent<RPOSInventoryCell>()._background.transform.localScale.y;
        for (int i = 0; i < this.NumCellsVertical; i++)
        {
            for (int j = 0; j < this.NumCellsHorizontal; j++)
            {
                byte num7 = (byte) (this.CellIndexStart + GetIndex2D(j, i, this.NumCellsHorizontal));
                if (num7 >= end)
                {
                    return;
                }
                GameObject obj2 = NGUITools.AddChild(parent, this.CellPrefab);
                RPOSInventoryCell component = obj2.GetComponent<RPOSInventoryCell>();
                component._mySlot = num7;
                component._displayInventory = inven;
                if (this.NumberedCells)
                {
                    component._numberLabel.text = (GetIndex2D(j, i, this.NumCellsHorizontal) + 1).ToString();
                }
                obj2.transform.localPosition = new Vector3(this.CellOffsetX + ((j * x) + (j * this.CellSpacing)), -(this.CellOffsetY + ((i * y) + (i * this.CellSpacing))), -2f);
                this._inventoryCells[RPOS.GetIndex2D(j, i, this.NumCellsHorizontal)] = obj2.GetComponent<RPOSInventoryCell>();
            }
        }
        if (this.CenterFromCells)
        {
            if (this.NumCellsHorizontal > 1)
            {
                base.transform.localPosition = new Vector3((this.CellOffsetX + ((this.NumCellsHorizontal * this.CellSize) + ((this.NumCellsHorizontal - 1) * this.CellSpacing))) * -0.5f, (float) this.CellSize, 0f);
            }
            else if (this.NumCellsVertical > 1)
            {
                base.transform.localPosition = new Vector3((float) -this.CellSize, (this.CellOffsetY + (this.NumCellsVertical * this.CellSize)) * 0.5f, 0f);
            }
        }
    }

    public static int GetIndex2D(int x, int y, int width)
    {
        return (x + (y * width));
    }

    public int GetNumCells()
    {
        if (!this.SpawnCells && !this.generatedCells)
        {
            return this._inventoryCells.Length;
        }
        return (this.NumCellsHorizontal * this.NumCellsVertical);
    }

    public void SetInventory(Inventory newInv, bool spawnNewCells)
    {
        this.displayInventory = newInv;
        if (spawnNewCells && this.SpawnCells)
        {
            this.generatedCells = true;
            for (int i = 0; i < this._inventoryCells.Length; i++)
            {
                Object.Destroy(this._inventoryCells[i].gameObject);
                this._inventoryCells[i] = null;
            }
            this.NumCellsVertical = Mathf.CeilToInt(((float) newInv.slotCount) / 3f);
            this.CreateCellsOnGameObject(newInv, base.gameObject);
        }
        int num2 = 0;
        foreach (RPOSInventoryCell cell in this._inventoryCells)
        {
            cell._displayInventory = newInv;
            cell._mySlot = (byte) (this.CellIndexStart + num2);
            newInv.MarkSlotDirty(cell._mySlot);
            num2++;
        }
    }
}

