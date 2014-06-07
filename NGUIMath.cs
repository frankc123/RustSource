using System;
using System.Collections.Generic;
using UnityEngine;

public static class NGUIMath
{
    public static Vector3 ApplyHalfPixelOffset(Vector3 pos)
    {
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.WindowsWebPlayer:
            case RuntimePlatform.WindowsEditor:
                pos.x -= 0.5f;
                pos.y += 0.5f;
                break;
        }
        return pos;
    }

    public static Vector3 ApplyHalfPixelOffset(Vector3 pos, Vector3 scale)
    {
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.WindowsWebPlayer:
            case RuntimePlatform.WindowsEditor:
                if (Mathf.RoundToInt(scale.x) == (Mathf.RoundToInt(scale.x * 0.5f) * 2))
                {
                    pos.x -= 0.5f;
                }
                if (Mathf.RoundToInt(scale.y) == (Mathf.RoundToInt(scale.y * 0.5f) * 2))
                {
                    pos.y += 0.5f;
                }
                break;
        }
        return pos;
    }

    public static AABBox CalculateAbsoluteWidgetBounds(Transform trans)
    {
        using (WidgetList list = GetWidgetsInChildren(trans))
        {
            if (list.empty)
            {
                return new AABBox();
            }
            AABBox box = new AABBox();
            bool flag = true;
            foreach (UIWidget widget in list)
            {
                Vector2 vector;
                Vector2 vector2;
                AABBox box2;
                Vector3 vector3;
                Vector3 vector4;
                widget.GetPivotOffsetAndRelativeSize(out vector2, out vector);
                vector3.x = (vector2.x + 0.5f) * vector.x;
                vector3.y = (vector2.y - 0.5f) * vector.y;
                vector4.x = vector3.x + (vector.x * 0.5f);
                vector4.y = vector3.y + (vector.y * 0.5f);
                vector3.x -= vector.x * 0.5f;
                vector3.y -= vector.y * 0.5f;
                vector3.z = 0f;
                vector4.z = 0f;
                AABBox box3 = new AABBox(ref vector3, ref vector4);
                Matrix4x4 localToWorldMatrix = widget.cachedTransform.localToWorldMatrix;
                box3.TransformedAABB3x4(ref localToWorldMatrix, out box2);
                if (flag)
                {
                    box = box2;
                    flag = false;
                }
                else
                {
                    box.Encapsulate(ref box2);
                }
            }
            if (flag)
            {
                return new AABBox(trans.position);
            }
            return box;
        }
    }

    public static AABBox CalculateRelativeInnerBounds(Transform root, UISlicedSprite sprite)
    {
        Vector2 vector;
        Vector2 vector2;
        Vector3 vector5;
        Vector3 vector6;
        AABBox box2;
        Transform cachedTransform = sprite.cachedTransform;
        Matrix4x4 t = root.worldToLocalMatrix * cachedTransform.localToWorldMatrix;
        sprite.GetPivotOffsetAndRelativeSize(out vector2, out vector);
        float num = (vector2.x + 0.5f) * vector.x;
        float num2 = (vector2.y - 0.5f) * vector.y;
        vector = (Vector2) (vector * 0.5f);
        Vector3 localScale = cachedTransform.localScale;
        float x = localScale.x;
        float y = localScale.y;
        Vector4 border = sprite.border;
        if (x != 0f)
        {
            border.x /= x;
            border.z /= x;
        }
        if (y != 0f)
        {
            border.y /= y;
            border.w /= y;
        }
        vector5.x = (num - vector.x) + border.x;
        vector6.x = (num + vector.x) - border.z;
        vector5.y = (num2 - vector.y) + border.y;
        vector6.y = (num2 + vector.y) - border.w;
        vector5.z = vector6.z = 0f;
        new AABBox(ref vector5, ref vector6).TransformedAABB3x4(ref t, out box2);
        return box2;
    }

    public static AABBox CalculateRelativeInnerBounds(Transform root, UISprite sprite)
    {
        if (sprite is UISlicedSprite)
        {
            return CalculateRelativeInnerBounds(root, sprite as UISlicedSprite);
        }
        return CalculateRelativeWidgetBounds(root, sprite.cachedTransform);
    }

    public static AABBox CalculateRelativeWidgetBounds(Transform trans)
    {
        return CalculateRelativeWidgetBounds(trans, trans);
    }

    public static AABBox CalculateRelativeWidgetBounds(Transform root, Transform child)
    {
        using (WidgetList list = GetWidgetsInChildren(child))
        {
            if (list.empty)
            {
                return new AABBox();
            }
            bool flag = true;
            AABBox box = new AABBox();
            Matrix4x4 worldToLocalMatrix = root.worldToLocalMatrix;
            foreach (UIWidget widget in list)
            {
                Vector2 vector;
                Vector2 vector2;
                Vector3 vector3;
                Vector3 vector4;
                AABBox box3;
                widget.GetPivotOffsetAndRelativeSize(out vector2, out vector);
                vector3.x = (vector2.x + 0.5f) * vector.x;
                vector3.y = (vector2.x - 0.5f) * vector.y;
                vector3.z = 0f;
                vector4.x = vector3.x + (vector.x * 0.5f);
                vector4.y = vector3.y + (vector.y * 0.5f);
                vector4.z = 0f;
                vector3.x -= vector.x * 0.5f;
                vector3.y -= vector.y * 0.5f;
                Matrix4x4 t = worldToLocalMatrix * widget.cachedTransform.localToWorldMatrix;
                new AABBox(ref vector3, ref vector4).TransformedAABB3x4(ref t, out box3);
                if (flag)
                {
                    box = box3;
                    flag = false;
                }
                else
                {
                    box.Encapsulate(ref box3);
                }
            }
            return box;
        }
    }

    public static int ColorToInt(Color c)
    {
        int num = 0;
        num |= Mathf.RoundToInt(c.r * 255f) << 0x18;
        num |= Mathf.RoundToInt(c.g * 255f) << 0x10;
        num |= Mathf.RoundToInt(c.b * 255f) << 8;
        return (num | Mathf.RoundToInt(c.a * 255f));
    }

    public static Vector2 ConstrainRect(Vector2 minRect, Vector2 maxRect, Vector2 minArea, Vector2 maxArea)
    {
        Vector2 zero = Vector2.zero;
        float num = maxRect.x - minRect.x;
        float num2 = maxRect.y - minRect.y;
        float num3 = maxArea.x - minArea.x;
        float num4 = maxArea.y - minArea.y;
        if (num > num3)
        {
            float num5 = num - num3;
            minArea.x -= num5;
            maxArea.x += num5;
        }
        if (num2 > num4)
        {
            float num6 = num2 - num4;
            minArea.y -= num6;
            maxArea.y += num6;
        }
        if (minRect.x < minArea.x)
        {
            zero.x += minArea.x - minRect.x;
        }
        if (maxRect.x > maxArea.x)
        {
            zero.x -= maxRect.x - maxArea.x;
        }
        if (minRect.y < minArea.y)
        {
            zero.y += minArea.y - minRect.y;
        }
        if (maxRect.y > maxArea.y)
        {
            zero.y -= maxRect.y - maxArea.y;
        }
        return zero;
    }

    public static Rect ConvertToPixels(Rect rect, int width, int height, bool round)
    {
        Rect rect2 = rect;
        if (round)
        {
            rect2.xMin = Mathf.RoundToInt(rect.xMin * width);
            rect2.xMax = Mathf.RoundToInt(rect.xMax * width);
            rect2.yMin = Mathf.RoundToInt((1f - rect.yMax) * height);
            rect2.yMax = Mathf.RoundToInt((1f - rect.yMin) * height);
            return rect2;
        }
        rect2.xMin = rect.xMin * width;
        rect2.xMax = rect.xMax * width;
        rect2.yMin = (1f - rect.yMax) * height;
        rect2.yMax = (1f - rect.yMin) * height;
        return rect2;
    }

    public static Rect ConvertToTexCoords(Rect rect, int width, int height)
    {
        Rect rect2 = rect;
        if ((width != 0f) && (height != 0f))
        {
            rect2.xMin = rect.xMin / ((float) width);
            rect2.xMax = rect.xMax / ((float) width);
            rect2.yMin = 1f - (rect.yMax / ((float) height));
            rect2.yMax = 1f - (rect.yMin / ((float) height));
        }
        return rect2;
    }

    private static void FillWidgetListWithChildren(Transform trans, ref WidgetList list, ref bool madeList)
    {
        UIWidget component = trans.GetComponent<UIWidget>();
        if (component != null)
        {
            if (!madeList)
            {
                list = WidgetList.Generate();
                madeList = true;
            }
            list.Add(component);
        }
        int childCount = trans.childCount;
        while (childCount-- > 0)
        {
            FillWidgetListWithChildren(trans.GetChild(childCount), ref list, ref madeList);
        }
    }

    private static WidgetList GetWidgetsInChildren(Transform trans)
    {
        if (trans != null)
        {
            bool madeList = false;
            WidgetList list = null;
            FillWidgetListWithChildren(trans, ref list, ref madeList);
            if (madeList)
            {
                return list;
            }
        }
        return WidgetList.Empty;
    }

    public static Color HexToColor(uint val)
    {
        return IntToColor((int) val);
    }

    public static int HexToDecimal(char ch)
    {
        char ch2 = ch;
        switch (ch2)
        {
            case '0':
                return 0;

            case '1':
                return 1;

            case '2':
                return 2;

            case '3':
                return 3;

            case '4':
                return 4;

            case '5':
                return 5;

            case '6':
                return 6;

            case '7':
                return 7;

            case '8':
                return 8;

            case '9':
                return 9;

            case 'A':
                break;

            case 'B':
                goto Label_00A5;

            case 'C':
                goto Label_00A8;

            case 'D':
                goto Label_00AB;

            case 'E':
                goto Label_00AE;

            case 'F':
                goto Label_00B1;

            default:
                switch (ch2)
                {
                    case 'a':
                        break;

                    case 'b':
                        goto Label_00A5;

                    case 'c':
                        goto Label_00A8;

                    case 'd':
                        goto Label_00AB;

                    case 'e':
                        goto Label_00AE;

                    case 'f':
                        goto Label_00B1;

                    default:
                        return 15;
                }
                break;
        }
        return 10;
    Label_00A5:
        return 11;
    Label_00A8:
        return 12;
    Label_00AB:
        return 13;
    Label_00AE:
        return 14;
    Label_00B1:
        return 15;
    }

    public static string IntToBinary(int val, int bits)
    {
        string str = string.Empty;
        int num = bits;
        while (num > 0)
        {
            switch (num)
            {
                case 8:
                case 0x10:
                case 0x18:
                    str = str + " ";
                    break;
            }
            str = str + (((val & (((int) 1) << --num)) == 0) ? '0' : '1');
        }
        return str;
    }

    public static Color IntToColor(int val)
    {
        float num = 0.003921569f;
        Color black = Color.black;
        black.r = num * ((val >> 0x18) & 0xff);
        black.g = num * ((val >> 0x10) & 0xff);
        black.b = num * ((val >> 8) & 0xff);
        black.a = num * (val & 0xff);
        return black;
    }

    public static Rect MakePixelPerfect(Rect rect)
    {
        rect.xMin = Mathf.RoundToInt(rect.xMin);
        rect.yMin = Mathf.RoundToInt(rect.yMin);
        rect.xMax = Mathf.RoundToInt(rect.xMax);
        rect.yMax = Mathf.RoundToInt(rect.yMax);
        return rect;
    }

    public static Rect MakePixelPerfect(Rect rect, int width, int height)
    {
        rect = ConvertToPixels(rect, width, height, true);
        rect.xMin = Mathf.RoundToInt(rect.xMin);
        rect.yMin = Mathf.RoundToInt(rect.yMin);
        rect.xMax = Mathf.RoundToInt(rect.xMax);
        rect.yMax = Mathf.RoundToInt(rect.yMax);
        return ConvertToTexCoords(rect, width, height);
    }

    public static float RotateTowards(float from, float to, float maxAngle)
    {
        float f = WrapAngle(to - from);
        if (Mathf.Abs(f) > maxAngle)
        {
            f = maxAngle * Mathf.Sign(f);
        }
        return (from + f);
    }

    public static Vector2 SpringDampen(ref Vector2 velocity, float strength, float deltaTime)
    {
        float num = 1f - (strength * 0.001f);
        int num2 = Mathf.RoundToInt(deltaTime * 1000f);
        Vector2 zero = Vector2.zero;
        for (int i = 0; i < num2; i++)
        {
            zero += (Vector2) (velocity * 0.06f);
            velocity = (Vector2) (velocity * num);
        }
        return zero;
    }

    public static Vector3 SpringDampen(ref Vector3 velocity, float strength, float deltaTime)
    {
        if ((Mathf.Approximately(velocity.x, 0f) && Mathf.Approximately(velocity.y, 0f)) && Mathf.Approximately(velocity.z, 0f))
        {
            velocity = Vector3.zero;
            return Vector3.zero;
        }
        float num = 1f - (strength * 0.001f);
        int num2 = Mathf.RoundToInt(deltaTime * 1000f);
        Vector3 zero = Vector3.zero;
        for (int i = 0; i < num2; i++)
        {
            zero += (Vector3) (velocity * 0.06f);
            velocity = (Vector3) (velocity * num);
        }
        return zero;
    }

    public static float SpringLerp(float strength, float deltaTime)
    {
        int num = Mathf.RoundToInt(deltaTime * 1000f);
        deltaTime = 0.001f * strength;
        float from = 0f;
        for (int i = 0; i < num; i++)
        {
            from = Mathf.Lerp(from, 1f, deltaTime);
        }
        return from;
    }

    public static float SpringLerp(float from, float to, float strength, float deltaTime)
    {
        int num = Mathf.RoundToInt(deltaTime * 1000f);
        deltaTime = 0.001f * strength;
        for (int i = 0; i < num; i++)
        {
            from = Mathf.Lerp(from, to, deltaTime);
        }
        return from;
    }

    public static Quaternion SpringLerp(Quaternion from, Quaternion to, float strength, float deltaTime)
    {
        return Quaternion.Slerp(from, to, SpringLerp(strength, deltaTime));
    }

    public static Vector2 SpringLerp(Vector2 from, Vector2 to, float strength, float deltaTime)
    {
        return Vector2.Lerp(from, to, SpringLerp(strength, deltaTime));
    }

    public static Vector3 SpringLerp(Vector3 from, Vector3 to, float strength, float deltaTime)
    {
        return Vector3.Lerp(from, to, SpringLerp(strength, deltaTime));
    }

    public static float WrapAngle(float angle)
    {
        while (angle > 180f)
        {
            angle -= 360f;
        }
        while (angle < -180f)
        {
            angle += 360f;
        }
        return angle;
    }

    private class WidgetList : List<UIWidget>, IDisposable
    {
        private bool disposed;
        public static readonly NGUIMath.WidgetList Empty = new NGUIMath.WidgetList(true);
        private readonly bool staticEmpty;
        private static Queue<NGUIMath.WidgetList> tempWidgetLists = new Queue<NGUIMath.WidgetList>();
        private static int tempWidgetListsSize;

        private WidgetList(bool staticEmpty)
        {
            this.staticEmpty = staticEmpty;
        }

        public void Add(UIWidget widget)
        {
            if (this.staticEmpty)
            {
                throw new InvalidOperationException();
            }
            base.Add(widget);
        }

        public void Dispose()
        {
            if (!this.disposed && !this.staticEmpty)
            {
                this.Clear();
                tempWidgetLists.Enqueue(this);
                tempWidgetListsSize++;
                this.disposed = true;
            }
        }

        public static NGUIMath.WidgetList Generate()
        {
            if (tempWidgetListsSize == 0)
            {
                return new NGUIMath.WidgetList(false);
            }
            NGUIMath.WidgetList list = tempWidgetLists.Dequeue();
            list.disposed = false;
            tempWidgetListsSize--;
            return list;
        }

        public bool empty
        {
            get
            {
                return this.staticEmpty;
            }
        }
    }
}

