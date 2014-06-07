using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public sealed class PrefabRenderer : IDisposable
{
    private bool disposed;
    private MeshRender[] meshes;
    private Material[] originalMaterials;
    private Mesh[] originalMeshes;
    private GameObject prefab;
    private readonly int prefabId;
    private int[] skipBits;

    private PrefabRenderer(int prefabId)
    {
        this.prefabId = prefabId;
        Runtime.Register[this.prefabId] = new WeakReference(this);
    }

    public void Dispose()
    {
        if (!this.disposed)
        {
            this.disposed = true;
            GC.SuppressFinalize(this);
            object @lock = Runtime.Lock;
            lock (@lock)
            {
                Runtime.Register.Remove(this.prefabId);
            }
        }
    }

    private static void DoNotCareResize<T>(ref T[] array, int size)
    {
        if ((array == null) || (array.Length != size))
        {
            array = new T[size];
        }
    }

    ~PrefabRenderer()
    {
        if (!this.disposed)
        {
            this.disposed = true;
            object @lock = Runtime.Lock;
            lock (@lock)
            {
                Runtime.Register.Remove(this.prefabId);
            }
        }
    }

    public Material GetMaterial(int index)
    {
        return this.originalMaterials[index];
    }

    public Material[] GetMaterialArrayCopy()
    {
        return (Material[]) this.originalMaterials.Clone();
    }

    public static PrefabRenderer GetOrCreateRender(GameObject prefab)
    {
        PrefabRenderer target;
        bool flag;
        if (prefab == null)
        {
            return null;
        }
        while (prefab.transform.parent != null)
        {
            prefab = prefab.transform.parent.gameObject;
        }
        int instanceID = prefab.GetInstanceID();
        object @lock = Runtime.Lock;
        lock (@lock)
        {
            WeakReference reference;
            if (Runtime.Register.TryGetValue(instanceID, out reference))
            {
                target = (PrefabRenderer) reference.Target;
            }
            else
            {
                target = null;
            }
            flag = target != null;
            if (!flag)
            {
                target = new PrefabRenderer(instanceID);
            }
        }
        if (!flag)
        {
            target.prefab = prefab;
            target.Refresh();
        }
        return target;
    }

    public void Refresh()
    {
        PrefabRenderer renderer = this;
        Transform transform = this.prefab.transform;
        HashSet<Material> set = new HashSet<Material>();
        HashSet<Mesh> set2 = new HashSet<Mesh>();
        List<Material[]> list = new List<Material[]>();
        List<Mesh> list2 = new List<Mesh>();
        int capacity = 0;
        Renderer[] componentsInChildren = this.prefab.GetComponentsInChildren<Renderer>(true);
        int size = 0;
        for (int i = 0; i < componentsInChildren.Length; i++)
        {
            Renderer renderer2 = componentsInChildren[i];
            if ((((renderer2 != null) && renderer2.enabled) && !renderer2.name.EndsWith("-lod", StringComparison.InvariantCultureIgnoreCase)) && !renderer2.name.EndsWith("_LOD_LOWEST", StringComparison.InvariantCultureIgnoreCase))
            {
                if (renderer2 is MeshRenderer)
                {
                    componentsInChildren[size++] = renderer2;
                    Mesh sharedMesh = renderer2.GetComponent<MeshFilter>().sharedMesh;
                    if ((sharedMesh != null) && set2.Add(sharedMesh))
                    {
                        capacity++;
                    }
                    list2.Add(sharedMesh);
                }
                else
                {
                    if (!(renderer2 is SkinnedMeshRenderer))
                    {
                        continue;
                    }
                    componentsInChildren[size++] = renderer2;
                    Mesh item = ((SkinnedMeshRenderer) renderer2).sharedMesh;
                    if ((item != null) && set2.Add(item))
                    {
                        capacity++;
                    }
                    list2.Add(item);
                }
                Material[] sharedMaterials = renderer2.sharedMaterials;
                list.Add(sharedMaterials);
                set.UnionWith(sharedMaterials);
            }
        }
        for (int j = size; j < componentsInChildren.Length; j++)
        {
            componentsInChildren[j] = null;
        }
        int count = set.Count;
        int num6 = ((count % 0x20) <= 0) ? (count / 0x20) : ((count / 0x20) + 1);
        DoNotCareResize<int>(ref renderer.skipBits, num6);
        for (int k = 0; k < num6; k++)
        {
            this.skipBits[k] = 0;
        }
        Dictionary<Material, int> dictionary = new Dictionary<Material, int>(count);
        Dictionary<Mesh, int> dictionary2 = new Dictionary<Mesh, int>(capacity);
        DoNotCareResize<Material>(ref renderer.originalMaterials, count);
        int index = 0;
        foreach (Material material in set)
        {
            if (material.GetTag("IgnorePrefabRenderer", false, "False") == "True")
            {
                this.skipBits[index / 0x20] |= ((int) 1) << (index % 0x20);
            }
            renderer.originalMaterials[index] = material;
            dictionary[material] = index++;
        }
        DoNotCareResize<Mesh>(ref renderer.originalMeshes, capacity);
        int num9 = 0;
        foreach (Mesh mesh3 in set2)
        {
            renderer.originalMeshes[num9] = mesh3;
            dictionary2[mesh3] = num9++;
        }
        DoNotCareResize<MeshRender>(ref renderer.meshes, size);
        for (int m = 0; m < size; m++)
        {
            Renderer renderer3 = componentsInChildren[m];
            Material[] materialArray2 = list[m];
            int[] materials = new int[materialArray2.Length];
            for (int n = 0; n < materialArray2.Length; n++)
            {
                materials[n] = dictionary[materialArray2[n]];
            }
            renderer.meshes[m].Set(dictionary2[list2[m]], materials, renderer3.transform.localToWorldMatrix * transform.worldToLocalMatrix, renderer3.gameObject.layer, renderer3.castShadows, renderer3.receiveShadows);
        }
    }

    public void Render(Camera camera, Matrix4x4 world, MaterialPropertyBlock props, Material[] overrideMaterials)
    {
        Material[] originalMaterials;
        if (overrideMaterials != null)
        {
            originalMaterials = overrideMaterials;
        }
        else
        {
            originalMaterials = this.originalMaterials;
        }
        foreach (MeshRender render in this.meshes)
        {
            Matrix4x4 matrix = world;
            Mesh mesh = this.originalMeshes[render.mesh];
            int num2 = 0;
            foreach (int num3 in render.materials)
            {
                if ((this.skipBits[num3 / 0x20] & (((int) 1) << (num3 % 0x20))) == 0)
                {
                    Material material = originalMaterials[num3];
                    Graphics.DrawMesh(mesh, matrix, material, render.layer, camera, num2++, props, render.castShadows, render.receiveShadows);
                }
            }
        }
    }

    public void RenderOneMaterial(Camera camera, Matrix4x4 world, MaterialPropertyBlock props, Material overrideMaterial)
    {
        if (overrideMaterial != null)
        {
            foreach (MeshRender render in this.meshes)
            {
                Matrix4x4 matrix = world;
                Mesh mesh = this.originalMeshes[render.mesh];
                int num2 = 0;
                for (int i = 0; i < render.materials.Length; i++)
                {
                    int num4 = render.materials[i];
                    if ((this.skipBits[num4 / 0x20] & (((int) 1) << (num4 % 0x20))) == 0)
                    {
                        Graphics.DrawMesh(mesh, matrix, overrideMaterial, render.layer, camera, num2++, props, render.castShadows, render.receiveShadows);
                    }
                }
            }
        }
    }

    public int materialCount
    {
        get
        {
            return this.originalMaterials.Length;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MeshRender
    {
        public int mesh;
        public Matrix4x4 transform;
        public int[] materials;
        public int layer;
        public bool castShadows;
        public bool receiveShadows;
        public void Set(int mesh, int[] materials, Matrix4x4 transform, int layer, bool castShadows, bool receiveShadows)
        {
            this.mesh = mesh;
            this.materials = materials;
            this.transform = transform;
            this.layer = layer;
            this.castShadows = castShadows;
            this.receiveShadows = receiveShadows;
        }
    }

    private static class Runtime
    {
        public static object Lock = new object();
        public static Dictionary<int, WeakReference> Register = new Dictionary<int, WeakReference>();
    }
}

