using System;
using UnityEngine;

[ExecuteInEditMode]
public class FPGrassLayer : MonoBehaviour
{
    [NonSerialized]
    private bool _enabled;
    [NonSerialized]
    private Plane[] _frustum;

    private void OnDisable()
    {
        this._enabled = false;
    }

    private void OnEnable()
    {
        this._enabled = true;
    }

    private void OnPreCull()
    {
        if (((Terrain.activeTerrain != null) && (Terrain.activeTerrain.terrainData != null)) && ((this._enabled && grass.on) && FPGrass.anyEnabled))
        {
            Terrain activeTerrain = Terrain.activeTerrain;
            this.UpdateDisplacement(grass.displacement);
            if (activeTerrain != null)
            {
                FPGrass.RenderArguments arguments;
                Camera camera = base.camera;
                this._frustum = GeometryUtility.CalculateFrustumPlanes(camera);
                arguments.frustum = this._frustum;
                arguments.camera = camera;
                arguments.immediate = false;
                arguments.terrain = activeTerrain;
                arguments.center = camera.transform.position;
                FPGrass.DrawAllGrass(ref arguments);
            }
        }
    }

    private void UpdateDisplacement(bool on)
    {
        if (!on)
        {
            Shader.SetGlobalVector("_DisplacementWorldMin", Vector2.zero);
            Shader.SetGlobalVector("_DisplacementWorldMax", Vector2.zero);
        }
        else
        {
            FPGrassDisplacementCamera camera = FPGrassDisplacementCamera.Get();
            Camera camera2 = (camera == null) ? null : camera.camera;
            if (camera2 != null)
            {
                Vector2 vector4;
                Vector2 vector5;
                float orthographicSize = camera2.orthographicSize;
                float num2 = orthographicSize / ((float) camera2.targetTexture.width);
                Vector3 position = base.camera.transform.position;
                if (TransformHelpers.Dist2D(position, camera2.transform.position) > 5f)
                {
                    Vector3 vector2;
                    vector2.x = Mathf.Round(position.x / num2) * num2;
                    vector2.y = Mathf.Round(position.y / num2) * num2;
                    vector2.z = Mathf.Round(position.z / num2) * num2;
                    camera2.transform.position = vector2 + new Vector3(0f, 50f, 0f);
                }
                Vector3 vector3 = camera2.transform.position;
                vector4.x = vector3.x - orthographicSize;
                vector4.y = vector3.z - orthographicSize;
                vector5.x = vector3.x + orthographicSize;
                vector5.y = vector3.z + orthographicSize;
                Shader.SetGlobalVector("_DisplacementWorldMin", vector4);
                Shader.SetGlobalVector("_DisplacementWorldMax", vector5);
                camera2.Render();
            }
        }
    }
}

