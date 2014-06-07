using System;
using System.Collections;
using UnityEngine;

[AddComponentMenu("Mesh/Combine Children")]
public class CombineChildren : MonoBehaviour
{
    public bool combineOnStart = true;
    public bool generateTriangleStrips = true;

    public void DoCombine()
    {
        Component[] componentsInChildren = base.GetComponentsInChildren(typeof(MeshFilter));
        Matrix4x4 worldToLocalMatrix = base.transform.worldToLocalMatrix;
        Hashtable hashtable = new Hashtable();
        for (int i = 0; i < componentsInChildren.Length; i++)
        {
            MeshFilter filter = (MeshFilter) componentsInChildren[i];
            Renderer renderer = componentsInChildren[i].renderer;
            MeshCombineUtility.MeshInstance instance = new MeshCombineUtility.MeshInstance {
                mesh = filter.sharedMesh
            };
            if (((renderer != null) && renderer.enabled) && (instance.mesh != null))
            {
                instance.transform = worldToLocalMatrix * filter.transform.localToWorldMatrix;
                Material[] sharedMaterials = renderer.sharedMaterials;
                for (int j = 0; j < sharedMaterials.Length; j++)
                {
                    instance.subMeshIndex = Math.Min(j, instance.mesh.subMeshCount - 1);
                    ArrayList list = (ArrayList) hashtable[sharedMaterials[j]];
                    if (list != null)
                    {
                        list.Add(instance);
                    }
                    else
                    {
                        list = new ArrayList();
                        list.Add(instance);
                        hashtable.Add(sharedMaterials[j], list);
                    }
                }
                renderer.enabled = false;
            }
        }
        IDictionaryEnumerator enumerator = hashtable.GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                DictionaryEntry current = (DictionaryEntry) enumerator.Current;
                ArrayList list2 = (ArrayList) current.Value;
                MeshCombineUtility.MeshInstance[] combines = (MeshCombineUtility.MeshInstance[]) list2.ToArray(typeof(MeshCombineUtility.MeshInstance));
                if (hashtable.Count == 1)
                {
                    if (base.GetComponent(typeof(MeshFilter)) == null)
                    {
                        base.gameObject.AddComponent(typeof(MeshFilter));
                    }
                    if (base.GetComponent("MeshRenderer") == null)
                    {
                        base.gameObject.AddComponent("MeshRenderer");
                    }
                    MeshFilter component = (MeshFilter) base.GetComponent(typeof(MeshFilter));
                    component.mesh = MeshCombineUtility.Combine(combines, this.generateTriangleStrips);
                    base.renderer.material = (Material) current.Key;
                    base.renderer.enabled = true;
                }
                else
                {
                    GameObject obj2 = new GameObject("Combined mesh") {
                        transform = { parent = base.transform, localScale = Vector3.one, localRotation = Quaternion.identity, localPosition = Vector3.zero }
                    };
                    obj2.AddComponent(typeof(MeshFilter));
                    obj2.AddComponent("MeshRenderer");
                    obj2.renderer.material = (Material) current.Key;
                    MeshFilter filter3 = (MeshFilter) obj2.GetComponent(typeof(MeshFilter));
                    filter3.mesh = MeshCombineUtility.Combine(combines, this.generateTriangleStrips);
                }
            }
        }
        finally
        {
            IDisposable disposable = enumerator as IDisposable;
            if (disposable == null)
            {
            }
            disposable.Dispose();
        }
    }

    private void Start()
    {
        if (this.combineOnStart)
        {
            this.DoCombine();
        }
    }
}

