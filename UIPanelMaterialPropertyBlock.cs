using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class UIPanelMaterialPropertyBlock
{
    private int count;
    private static Node dump;
    private static int dumpCount;
    private Node first;
    private Node last;

    public void AddToMaterialPropertyBlock(MaterialPropertyBlock block)
    {
        Node first = this.first;
        int count = this.count;
        while (count-- > 0)
        {
            switch (first.type)
            {
                case PropType.Float:
                    block.AddFloat(first.property, first.value.FLOAT);
                    break;

                case PropType.Vector:
                    block.AddVector(first.property, first.value.VECTOR);
                    break;

                case PropType.Color:
                    block.AddColor(first.property, first.value.COLOR);
                    break;

                case PropType.Matrix:
                    block.AddMatrix(first.property, first.value.MATRIX);
                    break;
            }
            first = first.next;
        }
    }

    public void Clear()
    {
        if (this.count > 0)
        {
            this.first.prev = dump;
            dump = this.last;
            if (dumpCount > 0)
            {
                this.first.prev.next = this.first;
                this.first.prev.hasNext = true;
                this.first.hasPrev = true;
            }
            this.first = (Node) (this.last = null);
            dumpCount += this.count;
            this.count = 0;
        }
    }

    private static Node NewNode(UIPanelMaterialPropertyBlock block, int prop, PropType type)
    {
        Node dump;
        if (dumpCount > 0)
        {
            dump = UIPanelMaterialPropertyBlock.dump;
            UIPanelMaterialPropertyBlock.dump = dump.prev;
            dumpCount--;
            dump.disposed = false;
        }
        else
        {
            dump = new Node();
        }
        dump.property = prop;
        dump.type = type;
        if (block.count++ == 0)
        {
            block.first = block.last = dump;
            dump.hasNext = dump.hasPrev = false;
            dump.next = (Node) (dump.prev = null);
            return dump;
        }
        dump.prev = block.last;
        dump.hasPrev = true;
        dump.next = null;
        dump.hasNext = false;
        block.last = dump;
        dump.prev.next = dump;
        dump.prev.hasNext = true;
        return dump;
    }

    private static Node NewNode(UIPanelMaterialPropertyBlock block, int prop, ref float value)
    {
        Node node = NewNode(block, prop, PropType.Float);
        node.value.FLOAT = value;
        return node;
    }

    private static Node NewNode(UIPanelMaterialPropertyBlock block, int prop, ref Color value)
    {
        Node node = NewNode(block, prop, PropType.Color);
        node.value.COLOR.r = value.r;
        node.value.COLOR.g = value.g;
        node.value.COLOR.b = value.b;
        node.value.COLOR.a = value.a;
        return node;
    }

    private static Node NewNode(UIPanelMaterialPropertyBlock block, int prop, ref Matrix4x4 value)
    {
        Node node = NewNode(block, prop, PropType.Matrix);
        node.value.MATRIX.m00 = value.m00;
        node.value.MATRIX.m10 = value.m10;
        node.value.MATRIX.m20 = value.m20;
        node.value.MATRIX.m30 = value.m30;
        node.value.MATRIX.m01 = value.m01;
        node.value.MATRIX.m11 = value.m11;
        node.value.MATRIX.m21 = value.m21;
        node.value.MATRIX.m31 = value.m31;
        node.value.MATRIX.m02 = value.m02;
        node.value.MATRIX.m12 = value.m12;
        node.value.MATRIX.m22 = value.m22;
        node.value.MATRIX.m32 = value.m32;
        node.value.MATRIX.m03 = value.m03;
        node.value.MATRIX.m13 = value.m13;
        node.value.MATRIX.m23 = value.m23;
        node.value.MATRIX.m33 = value.m33;
        return node;
    }

    private static Node NewNode(UIPanelMaterialPropertyBlock block, int prop, ref Vector4 value)
    {
        Node node = NewNode(block, prop, PropType.Vector);
        node.value.VECTOR.x = value.x;
        node.value.VECTOR.y = value.y;
        node.value.VECTOR.z = value.z;
        node.value.VECTOR.w = value.w;
        return node;
    }

    public void Set(int property, float value)
    {
        NewNode(this, property, ref value);
    }

    public void Set(int property, Color value)
    {
        NewNode(this, property, ref value);
    }

    public void Set(int property, Matrix4x4 value)
    {
        NewNode(this, property, ref value);
    }

    public void Set(int property, Vector4 value)
    {
        NewNode(this, property, ref value);
    }

    public void Set(string property, float value)
    {
        this.Set(Shader.PropertyToID(property), value);
    }

    public void Set(string property, Color value)
    {
        this.Set(Shader.PropertyToID(property), value);
    }

    public void Set(string property, Matrix4x4 value)
    {
        this.Set(Shader.PropertyToID(property), value);
    }

    public void Set(string property, Vector4 value)
    {
        this.Set(Shader.PropertyToID(property), value);
    }

    private class Node
    {
        public bool disposed;
        public bool hasNext;
        public bool hasPrev;
        public UIPanelMaterialPropertyBlock.Node next;
        public UIPanelMaterialPropertyBlock.Node prev;
        public int property;
        public UIPanelMaterialPropertyBlock.PropType type;
        public UIPanelMaterialPropertyBlock.PropValue value;
    }

    private enum PropType : byte
    {
        Color = 2,
        Float = 0,
        Matrix = 3,
        Vector = 1
    }

    [StructLayout(LayoutKind.Explicit, Size=0x40)]
    private struct PropValue
    {
        [FieldOffset(0)]
        public Color COLOR;
        [FieldOffset(0)]
        public float FLOAT;
        [FieldOffset(0)]
        public Matrix4x4 MATRIX;
        [FieldOffset(0)]
        public Vector4 VECTOR;
    }
}

