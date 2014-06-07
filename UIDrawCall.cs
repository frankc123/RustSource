using NGUI.Meshing;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

[AddComponentMenu("NGUI/Internal/Draw Call"), ExecuteInEditMode]
public class UIDrawCall : MonoBehaviour
{
    private static Material[] materialBuffer1 = new Material[1];
    private static Material[] materialBuffer2 = new Material[2];
    private MaterialPropertyBlock mBlock;
    private Clipping mClipping;
    private Vector4 mClipRange;
    private Vector2 mClipSoft;
    private Color[] mColors;
    private UIMaterial mDepthMat;
    private bool mDepthPass;
    private bool mEven = true;
    private MeshFilter mFilter;
    private bool mHasNext;
    private bool mHasPrev;
    private int[] mIndices;
    private Mesh mMesh0;
    private Mesh mMesh1;
    private UIDrawCall mNext;
    private UIPanelMaterialPropertyBlock mPanelPropertyBlock;
    private UIDrawCall mPrev;
    private MeshRenderer mRen;
    private bool mReset = true;
    private UIMaterial mSharedMat;
    private Transform mTrans;
    private Vector2[] mUVs;
    private Vector3[] mVerts;

    private Mesh GetMesh(ref bool rebuildIndices, int vertexCount)
    {
        this.mEven = !this.mEven;
        if (this.mEven)
        {
            if (this.mMesh0 == null)
            {
                this.mMesh0 = new Mesh();
                this.mMesh0.hideFlags = HideFlags.DontSave;
                rebuildIndices = true;
            }
            else if (rebuildIndices || (this.mMesh0.vertexCount != vertexCount))
            {
                rebuildIndices = true;
                this.mMesh0.Clear();
            }
            return this.mMesh0;
        }
        if (this.mMesh1 == null)
        {
            this.mMesh1 = new Mesh();
            this.mMesh1.hideFlags = HideFlags.DontSave;
            rebuildIndices = true;
        }
        else if (rebuildIndices || (this.mMesh1.vertexCount != vertexCount))
        {
            rebuildIndices = true;
            this.mMesh1.Clear();
        }
        return this.mMesh1;
    }

    internal void LinkedList__Insert(ref UIDrawCall list)
    {
        this.mHasPrev = false;
        this.mHasNext = (bool) list;
        this.mNext = list;
        this.mPrev = null;
        if (this.mHasNext)
        {
            list.mHasPrev = true;
            list.mPrev = this;
        }
        list = this;
    }

    internal void LinkedList__Remove()
    {
        if (this.mHasPrev)
        {
            this.mPrev.mHasNext = this.mHasNext;
            this.mPrev.mNext = this.mNext;
        }
        if (this.mHasNext)
        {
            this.mNext.mHasPrev = this.mHasPrev;
            this.mNext.mPrev = this.mPrev;
        }
        this.mHasNext = this.mHasPrev = false;
        this.mNext = (UIDrawCall) (this.mPrev = null);
    }

    private void OnDestroy()
    {
        NGUITools.DestroyImmediate(this.mMesh0);
        NGUITools.DestroyImmediate(this.mMesh1);
        NGUITools.DestroyImmediate(this.mDepthMat);
    }

    private void OnWillRenderObject()
    {
        if (this.mReset)
        {
            this.mReset = false;
            this.UpdateMaterials();
        }
        if (this.mBlock == null)
        {
            this.mBlock = new MaterialPropertyBlock();
        }
        else
        {
            this.mBlock.Clear();
        }
        if (this.mPanelPropertyBlock != null)
        {
            this.mPanelPropertyBlock.AddToMaterialPropertyBlock(this.mBlock);
        }
        if (this.mClipping != Clipping.None)
        {
            Vector4 vector;
            Vector4 vector2;
            vector.z = -this.mClipRange.x / this.mClipRange.z;
            vector.w = -this.mClipRange.y / this.mClipRange.w;
            vector.x = 1f / this.mClipRange.z;
            vector.y = 1f / this.mClipRange.w;
            this.mBlock.AddVector(FastProperties.kProp_ClippingRegion, vector);
            if (this.mClipSoft.x > 0f)
            {
                vector2.x = this.mClipRange.z / this.mClipSoft.x;
            }
            else
            {
                vector2.x = 1000f;
            }
            if (this.mClipSoft.y > 0f)
            {
                vector2.y = this.mClipRange.w / this.mClipSoft.y;
            }
            else
            {
                vector2.y = 1000f;
            }
            vector2.z = vector2.w = 0f;
            this.mBlock.AddVector(FastProperties.kProp_ClipSharpness, vector2);
        }
        base.renderer.SetPropertyBlock(this.mBlock);
    }

    public void Set(MeshBuffer m)
    {
        if (this.mFilter == null)
        {
            this.mFilter = base.gameObject.GetComponent<MeshFilter>();
        }
        if (this.mFilter == null)
        {
            this.mFilter = base.gameObject.AddComponent<MeshFilter>();
        }
        if (this.mRen == null)
        {
            this.mRen = base.gameObject.GetComponent<MeshRenderer>();
        }
        if (this.mRen == null)
        {
            this.mRen = base.gameObject.AddComponent<MeshRenderer>();
            this.UpdateMaterials();
        }
        if (m.vSize < 0xfde8)
        {
            bool rebuildIndices = m.ExtractMeshBuffers(ref this.mVerts, ref this.mUVs, ref this.mColors, ref this.mIndices);
            Mesh mesh = this.GetMesh(ref rebuildIndices, m.vSize);
            mesh.vertices = this.mVerts;
            mesh.uv = this.mUVs;
            mesh.colors = this.mColors;
            mesh.triangles = this.mIndices;
            mesh.RecalculateBounds();
            this.mFilter.mesh = mesh;
        }
        else
        {
            if (this.mFilter.mesh != null)
            {
                this.mFilter.mesh.Clear();
            }
            Debug.LogError("Too many vertices on one panel: " + m.vSize);
        }
    }

    private void UpdateMaterials()
    {
        if (this.mDepthPass)
        {
            if (this.mDepthMat == null)
            {
                Material key = new Material(Shader.Find("Depth")) {
                    hideFlags = HideFlags.DontSave,
                    mainTexture = this.mSharedMat.mainTexture
                };
                this.mDepthMat = UIMaterial.Create(key, true, this.mClipping);
            }
        }
        else if (this.mDepthMat != null)
        {
            NGUITools.Destroy(this.mDepthMat);
            this.mDepthMat = null;
        }
        Material material2 = this.mSharedMat[this.mClipping];
        if (this.mDepthMat != null)
        {
            Material material3;
            materialBuffer2[0] = this.mDepthMat[this.mClipping];
            materialBuffer2[1] = material2;
            this.mRen.sharedMaterials = materialBuffer2;
            materialBuffer2[1] = (Material) (material3 = null);
            materialBuffer2[0] = material3;
        }
        else if (this.mRen.sharedMaterial != material2)
        {
            materialBuffer1[0] = material2;
            this.mRen.sharedMaterials = materialBuffer1;
            materialBuffer1[0] = null;
        }
    }

    public Transform cachedTransform
    {
        get
        {
            if (this.mTrans == null)
            {
                this.mTrans = base.transform;
            }
            return this.mTrans;
        }
    }

    public Clipping clipping
    {
        get
        {
            return this.mClipping;
        }
        set
        {
            if (this.mClipping != value)
            {
                this.mClipping = value;
                this.mReset = true;
            }
        }
    }

    public Vector4 clipRange
    {
        get
        {
            return this.mClipRange;
        }
        set
        {
            this.mClipRange = value;
        }
    }

    public Vector2 clipSoftness
    {
        get
        {
            return this.mClipSoft;
        }
        set
        {
            this.mClipSoft = value;
        }
    }

    public bool depthPass
    {
        get
        {
            return this.mDepthPass;
        }
        set
        {
            if (this.mDepthPass != value)
            {
                this.mDepthPass = value;
                this.mReset = true;
            }
        }
    }

    public UIMaterial material
    {
        get
        {
            return this.mSharedMat;
        }
        set
        {
            this.mSharedMat = value;
        }
    }

    public UIPanelMaterialPropertyBlock panelPropertyBlock
    {
        get
        {
            return this.mPanelPropertyBlock;
        }
        set
        {
            this.mPanelPropertyBlock = value;
        }
    }

    public int triangles
    {
        get
        {
            Mesh mesh = !this.mEven ? this.mMesh1 : this.mMesh0;
            return ((mesh == null) ? 0 : (mesh.vertexCount >> 1));
        }
    }

    public enum Clipping
    {
        None,
        HardClip,
        AlphaClip,
        SoftClip
    }

    private static class FastProperties
    {
        public static readonly int kProp_ClippingRegion = Shader.PropertyToID("_MainTex_ST");
        public static readonly int kProp_ClipSharpness = Shader.PropertyToID("_ClipSharpness");
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Iterator
    {
        public UIDrawCall Current;
        public bool Has;
        public UIDrawCall.Iterator Next
        {
            get
            {
                if (this.Has)
                {
                    UIDrawCall.Iterator iterator;
                    iterator.Has = this.Current.mHasNext;
                    iterator.Current = this.Current.mNext;
                    return iterator;
                }
                return new UIDrawCall.Iterator();
            }
        }
        public UIDrawCall.Iterator Prev
        {
            get
            {
                if (this.Has)
                {
                    UIDrawCall.Iterator iterator;
                    iterator.Has = this.Current.mHasPrev;
                    iterator.Current = this.Current.mPrev;
                    return iterator;
                }
                return new UIDrawCall.Iterator();
            }
        }
        public static explicit operator UIDrawCall.Iterator(UIDrawCall call)
        {
            UIDrawCall.Iterator iterator;
            iterator.Has = (bool) call;
            if (iterator.Has)
            {
                iterator.Current = call;
                return iterator;
            }
            iterator.Current = null;
            return iterator;
        }
    }
}

