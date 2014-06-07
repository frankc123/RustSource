using System;
using System.Text;
using UnityEngine;

[ExecuteInEditMode]
public class PrefabRendererTest : MonoBehaviour
{
    public Material[] materialKeys;
    public Material[] materialValues;
    [NonSerialized]
    private bool oi;
    [NonSerialized]
    private Material[] oldMaterialKeys;
    [NonSerialized]
    private Material[] oldMaterialValues;
    [NonSerialized]
    private Material[] overrideMaterials;
    public GameObject prefab;
    [NonSerialized]
    private GameObject prefabRendering;
    [NonSerialized]
    private PrefabRenderer renderer;

    [ContextMenu("Refresh material overrides")]
    private void ApplyOverrides()
    {
        if (this.renderer != null)
        {
            this.overrideMaterials = this.renderer.GetMaterialArrayCopy();
            if (((this.overrideMaterials.Length != 0) && (this.materialKeys != null)) && (this.materialValues != null))
            {
                int num = Mathf.Min(this.overrideMaterials.Length, Mathf.Min(this.materialKeys.Length, this.materialValues.Length));
                for (int i = 0; i < num; i++)
                {
                    int index = Array.IndexOf<Material>(this.materialKeys, this.overrideMaterials[i]);
                    if ((index != -1) && (index < this.materialValues.Length))
                    {
                        this.overrideMaterials[i] = this.materialValues[index];
                    }
                }
            }
        }
    }

    [ContextMenu("List Materials")]
    private void ListMaterials()
    {
        if (this.renderer != null)
        {
            int materialCount = this.renderer.materialCount;
            for (int i = 0; i < materialCount; i++)
            {
                Debug.Log(this.renderer.GetMaterial(i), this.renderer.GetMaterial(i));
            }
        }
    }

    [ContextMenu("Print info")]
    private void PrintINfo()
    {
        if (this.renderer == null)
        {
            Debug.Log("No Renderer", this);
        }
        else
        {
            StringBuilder message = new StringBuilder();
            foreach (Material material in this.renderer.GetMaterialArrayCopy())
            {
                message.AppendLine(material.ToString());
            }
            Debug.Log(message, this);
        }
    }

    [ContextMenu("Refresh")]
    private void RefreshRenderer()
    {
        if (this.renderer != null)
        {
            this.renderer.Refresh();
        }
    }

    private void Update()
    {
        if ((this.prefabRendering != this.prefab) || !this.oi)
        {
            if (this.prefabRendering != null)
            {
                this.renderer = null;
            }
            if (this.prefab != null)
            {
                this.renderer = PrefabRenderer.GetOrCreateRender(this.prefab);
            }
            this.prefabRendering = this.prefab;
            this.oi = true;
            this.ApplyOverrides();
        }
        if (this.renderer == null)
        {
            Debug.Log("None", this);
        }
        else
        {
            this.renderer.Render(null, base.transform.localToWorldMatrix, null, this.overrideMaterials);
        }
    }
}

