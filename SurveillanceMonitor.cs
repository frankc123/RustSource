using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class SurveillanceMonitor : MonoBehaviour
{
    private Material[] activeSharedMaterials;
    public float aspect = 1f;
    private Texture lastTexture;
    [SerializeField]
    private int[] materialIds;
    private Material[] originalSharedMaterials;
    [NonSerialized]
    public Renderer renderer;
    private Material[] replacementMaterials;
    public SurveillanceCamera surveillanceCamera;
    public string textureName = "_MainTex";
    public float viewDistance = 30f;

    private void Awake()
    {
        this.renderer = base.renderer;
        this.originalSharedMaterials = this.renderer.sharedMaterials;
        if ((this.materialIds == null) || (this.materialIds.Length == 0))
        {
            Debug.LogWarning("Please, set the material ids for this SurveillanceMonitor. Assuming you meant to use id 0 only.", this);
            this.materialIds = new int[1];
        }
        HashSet<Material> set = new HashSet<Material>();
        int num = 0;
        int[] numArray = new int[this.materialIds.Length];
        for (int i = 0; i < this.materialIds.Length; i++)
        {
            if (set.Add(this.originalSharedMaterials[this.materialIds[i]]))
            {
                numArray[i] = i;
                num++;
            }
            else
            {
                for (int k = 0; k < i; k++)
                {
                    if (this.originalSharedMaterials[this.materialIds[k]] == this.originalSharedMaterials[this.materialIds[i]])
                    {
                        numArray[i] = k;
                    }
                }
            }
        }
        this.replacementMaterials = new Material[num];
        this.activeSharedMaterials = (Material[]) this.originalSharedMaterials.Clone();
        for (int j = 0; j < this.materialIds.Length; j++)
        {
            Material material;
            if (numArray[j] == j)
            {
                this.replacementMaterials[j] = material = new Material(this.originalSharedMaterials[this.materialIds[j]]);
            }
            else
            {
                material = this.replacementMaterials[this.materialIds[numArray[j]]];
            }
            this.activeSharedMaterials[this.materialIds[j]] = material;
        }
    }

    private void BindTexture(Texture tex)
    {
        foreach (Material material in this.replacementMaterials)
        {
            material.SetTexture(this.textureName, tex);
        }
    }

    public void DropReference(RenderTexture texture)
    {
        if (this.lastTexture == texture)
        {
            this.lastTexture = null;
        }
    }

    private void OnWillRenderObject()
    {
        if (this.surveillanceCamera != null)
        {
            Camera current = Camera.current;
            if (this.surveillanceCamera.camera != current)
            {
                Texture texture;
                Transform transform = current.transform;
                Vector3 rhs = base.transform.position - transform.position;
                if (((rhs.sqrMagnitude <= (this.viewDistance * this.viewDistance)) && (Vector3.Dot(transform.forward, rhs) > 0f)) && ((texture = this.surveillanceCamera.Render()) != null))
                {
                    foreach (Material material in this.replacementMaterials)
                    {
                        material.SetTexture(this.textureName, texture);
                    }
                    this.renderer.sharedMaterials = this.activeSharedMaterials;
                }
                else
                {
                    this.renderer.sharedMaterials = this.originalSharedMaterials;
                }
            }
        }
    }
}

