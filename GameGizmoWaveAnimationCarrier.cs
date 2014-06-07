using System;
using UnityEngine;

public class GameGizmoWaveAnimationCarrier : GameGizmoWaveAnimation
{
    [SerializeField]
    protected Material[] carrierMaterials;
    [SerializeField]
    protected bool hideArrowWhenCarrierExists;

    protected override GameGizmo.Instance ConstructInstance()
    {
        return new Instance(this);
    }

    public class Instance : GameGizmoWaveAnimation.Instance
    {
        protected internal Instance(GameGizmoWaveAnimationCarrier gameGizmo) : base(gameGizmo)
        {
        }

        protected override void Render(bool useCamera, Camera camera)
        {
            if ((gizmos.carrier && (base.carrierRenderer != null)) && base.carrierRenderer.enabled)
            {
                Material[] carrierMaterials = ((GameGizmoWaveAnimationCarrier) base.gameGizmo).carrierMaterials;
                if ((carrierMaterials != null) && (carrierMaterials.Length > 0))
                {
                    MeshFilter component = base.carrierRenderer.GetComponent<MeshFilter>();
                    if (component != null)
                    {
                        Mesh sharedMesh = component.sharedMesh;
                        if (sharedMesh != null)
                        {
                            try
                            {
                                base.hideMesh = ((GameGizmoWaveAnimationCarrier) base.gameGizmo).hideArrowWhenCarrierExists;
                                base.Render(useCamera, camera);
                            }
                            finally
                            {
                                base.hideMesh = false;
                            }
                            foreach (Material material in carrierMaterials)
                            {
                                if (material != null)
                                {
                                    int subMeshCount = sharedMesh.subMeshCount;
                                    while (--subMeshCount >= 0)
                                    {
                                        Graphics.DrawMesh(sharedMesh, base.carrierRenderer.localToWorldMatrix, material, base.layer, camera, subMeshCount, base.propertyBlock, base.castShadows, base.receiveShadows);
                                    }
                                }
                            }
                            return;
                        }
                    }
                }
            }
            base.Render(useCamera, camera);
        }
    }
}

