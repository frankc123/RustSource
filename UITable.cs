using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Table"), ExecuteInEditMode]
public class UITable : MonoBehaviour
{
    public int columns;
    public Direction direction;
    public bool hideInactive = true;
    public bool keepWithinPanel;
    private UIDraggablePanel mDrag;
    private UIPanel mPanel;
    private bool mStarted;
    public OnReposition onReposition;
    public Vector2 padding = Vector2.zero;
    public bool repositionNow;
    public bool sorted;

    private void LateUpdate()
    {
        if (this.repositionNow)
        {
            this.repositionNow = false;
            this.Reposition();
            if (this.onReposition != null)
            {
                this.onReposition();
            }
        }
    }

    public void Reposition()
    {
        if (this.mStarted)
        {
            Transform target = base.transform;
            List<Transform> children = new List<Transform>();
            for (int i = 0; i < target.childCount; i++)
            {
                Transform child = target.GetChild(i);
                if ((child != null) && (!this.hideInactive || child.gameObject.activeInHierarchy))
                {
                    children.Add(child);
                }
            }
            if (this.sorted)
            {
                children.Sort(new Comparison<Transform>(UITable.SortByName));
            }
            if (children.Count > 0)
            {
                this.RepositionVariableSize(children);
            }
            if ((this.mPanel != null) && (this.mDrag == null))
            {
                this.mPanel.ConstrainTargetToBounds(target, true);
            }
            if (this.mDrag != null)
            {
                this.mDrag.UpdateScrollbars(true);
            }
        }
        else
        {
            this.repositionNow = true;
        }
    }

    private void RepositionVariableSize(List<Transform> children)
    {
        float num = 0f;
        float num2 = 0f;
        int num3 = (this.columns <= 0) ? 1 : ((children.Count / this.columns) + 1);
        int num4 = (this.columns <= 0) ? children.Count : this.columns;
        AABBox[,] boxArray = new AABBox[num3, num4];
        AABBox[] boxArray2 = new AABBox[num4];
        AABBox[] boxArray3 = new AABBox[num3];
        int index = 0;
        int num6 = 0;
        int num7 = 0;
        int count = children.Count;
        while (num7 < count)
        {
            Transform trans = children[num7];
            AABBox v = NGUIMath.CalculateRelativeWidgetBounds(trans);
            Vector3 localScale = trans.localScale;
            Vector3 min = Vector3.Scale(v.min, localScale);
            v.SetMinMax(min, Vector3.Scale(v.max, localScale));
            boxArray[num6, index] = v;
            boxArray2[index].Encapsulate(v);
            boxArray3[num6].Encapsulate(v);
            if ((++index >= this.columns) && (this.columns > 0))
            {
                index = 0;
                num6++;
            }
            num7++;
        }
        index = 0;
        num6 = 0;
        int num9 = 0;
        int num10 = children.Count;
        while (num9 < num10)
        {
            Transform transform2 = children[num9];
            AABBox box2 = boxArray[num6, index];
            AABBox box3 = boxArray2[index];
            AABBox box4 = boxArray3[num6];
            Vector3 localPosition = transform2.localPosition;
            Vector3 vector3 = box2.min;
            Vector3 max = box2.max;
            Vector3 vector5 = (Vector3) (box2.size * 0.5f);
            Vector3 center = box2.center;
            Vector3 vector7 = box4.min;
            Vector3 vector8 = box4.max;
            Vector3 vector9 = box3.min;
            localPosition.x = (num + vector5.x) - center.x;
            localPosition.x += (vector3.x - vector9.x) + this.padding.x;
            if (this.direction == Direction.Down)
            {
                localPosition.y = (-num2 - vector5.y) - center.y;
                localPosition.y += ((((max.y - vector3.y) - vector8.y) + vector7.y) * 0.5f) - this.padding.y;
            }
            else
            {
                localPosition.y = (num2 + vector5.y) - center.y;
                localPosition.y += ((((max.y - vector3.y) - vector8.y) + vector7.y) * 0.5f) - this.padding.y;
            }
            num += (vector9.x - vector9.x) + (this.padding.x * 2f);
            transform2.localPosition = localPosition;
            if ((++index >= this.columns) && (this.columns > 0))
            {
                index = 0;
                num6++;
                num = 0f;
                num2 += (vector5.y * 2f) + (this.padding.y * 2f);
            }
            num9++;
        }
    }

    public static int SortByName(Transform a, Transform b)
    {
        return string.Compare(a.name, b.name);
    }

    private void Start()
    {
        this.mStarted = true;
        if (this.keepWithinPanel)
        {
            this.mPanel = NGUITools.FindInParents<UIPanel>(base.gameObject);
            this.mDrag = NGUITools.FindInParents<UIDraggablePanel>(base.gameObject);
        }
        this.Reposition();
    }

    public enum Direction
    {
        Down,
        Up
    }

    public delegate void OnReposition();
}

