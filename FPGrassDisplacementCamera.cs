using System;
using UnityEngine;

[ExecuteInEditMode]
public class FPGrassDisplacementCamera : MonoBehaviour
{
    [NonSerialized]
    public Material blitMat;

    public void Awake()
    {
    }

    public static FPGrassDisplacementCamera Get()
    {
        return singleton;
    }

    public static Material GetBlitMat()
    {
        return singleton.blitMat;
    }

    public static RenderTexture GetRT()
    {
        return singleton.camera.targetTexture;
    }

    public void OnDestroy()
    {
        Object.DestroyImmediate(base.camera.targetTexture);
        Object.DestroyImmediate(this.blitMat);
    }

    public static FPGrassDisplacementCamera singleton
    {
        get
        {
            return Global.singleton;
        }
    }

    private static class Global
    {
        public static FPGrassDisplacementCamera singleton;

        static Global()
        {
            GameObject obj2 = GameObject.FindWithTag("DisplacementCamera");
            if (obj2 != null)
            {
                singleton = obj2.GetComponent<FPGrassDisplacementCamera>();
                if (singleton != null)
                {
                    Object.DestroyImmediate(obj2);
                }
                singleton = null;
            }
            GameObject obj3 = new GameObject("FPGrassDisplacementCamera") {
                hideFlags = HideFlags.DontSave
            };
            obj3.AddComponent<Camera>();
            obj3.transform.rotation = Quaternion.Euler(new Vector3(90f, 0f, 0f));
            obj3.camera.backgroundColor = new Color(0f, 0f, 0f, 0f);
            obj3.camera.clearFlags = CameraClearFlags.Color;
            obj3.camera.orthographic = true;
            obj3.camera.orthographicSize = 50f;
            obj3.camera.nearClipPlane = 0.3f;
            obj3.camera.farClipPlane = 1000f;
            obj3.camera.renderingPath = RenderingPath.VertexLit;
            obj3.camera.enabled = false;
            obj3.camera.cullingMask = ((int) 1) << LayerMask.NameToLayer("GrassDisplacement");
            obj3.camera.tag = "DisplacementCamera";
            singleton = obj3.AddComponent<FPGrassDisplacementCamera>();
            RenderTexture texture = new RenderTexture(0x200, 0x200, 0, FPGrass.Support.ProbabilityRenderTextureFormat1Channel) {
                hideFlags = HideFlags.DontSave
            };
            texture.Create();
            texture.name = "FPGrassDisplacement_RT";
            obj3.camera.targetTexture = texture;
            Material material = new Material(Shader.Find("Custom/DisplacementBlit")) {
                hideFlags = HideFlags.DontSave
            };
            singleton.blitMat = material;
        }
    }
}

