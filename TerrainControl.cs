using Facepunch;
using System;
using UnityEngine;

[ExecuteInEditMode]
public sealed class TerrainControl : MonoBehaviour
{
    [SerializeField]
    private float _customBasemapDistance = 10000f;
    [SerializeField]
    private Terrain _terrain;
    [SerializeField]
    private Material _terrainMaterialTemplate;
    private static TerrainControl activeTerrainControl;
    public string bundlePathToTerrainData = "Env/ter/rust_island_2013-2";
    public bool forceCustomBasemapDistance = true;
    [NonSerialized]
    private Vector3 lastCameraForward;
    [NonSerialized]
    private Vector3 lastCameraPosition;
    [NonSerialized]
    private bool quitting;
    public float reassignTerrainDataInterval;
    [NonSerialized]
    private bool running;
    [SerializeField]
    private TerrainSettingsHack settings;
    private TerrainData terrainDataFromBundle;
    [NonSerialized]
    private float timeNoticedCameraChange;

    private void BindTerrainSettings()
    {
        if (this.forceCustomBasemapDistance && (this.terrain != null))
        {
            this.terrain.basemapDistance = this.customBasemapDistance;
        }
    }

    [ContextMenu("Get settings from terrain")]
    private void CopyTerrainSettings()
    {
        this.settings.CopyFrom(this.terrain);
    }

    private bool DoReassignmentOfTerrainData(bool td, bool andFlush, bool mats, bool doNotCopySettings)
    {
        if ((this.terrainDataFromBundle == null) && !Bundling.Load<TerrainData>(this.bundlePathToTerrainData, out this.terrainDataFromBundle))
        {
            Debug.LogError("Bad terrain data path " + this.bundlePathToTerrainData);
            return true;
        }
        if (td)
        {
            if (doNotCopySettings)
            {
                this.terrain.terrainData = this.terrainDataFromBundle;
            }
            else
            {
                this.terrain.terrainData = this.terrainDataFromBundle;
                this.RestoreTerrainSettings();
            }
        }
        if (mats)
        {
            this.terrain.materialTemplate = this._terrainMaterialTemplate;
        }
        if (andFlush)
        {
            this.terrain.Flush();
            if (mats)
            {
                this.terrain.materialTemplate = this._terrainMaterialTemplate;
            }
        }
        return (this.terrainDataFromBundle == 0);
    }

    private void OnApplicationQuit()
    {
        this.quitting = true;
    }

    private void OnDisable()
    {
        if (!this.quitting && this.running)
        {
            this.running = false;
        }
    }

    private void OnEnable()
    {
        activeTerrainControl = this;
        this.quitting = false;
        if (!this.running)
        {
            this.running = true;
            this.BindTerrainSettings();
        }
        if (this.reassignTerrainDataInterval > 0f)
        {
            base.Invoke("ReassignTerrainData", this.reassignTerrainDataInterval);
        }
    }

    private void ReassignTerrainData()
    {
        if (Application.isPlaying && !terrain.manual)
        {
            if (!Bundling.Load<TerrainData>(this.bundlePathToTerrainData, out this.terrainDataFromBundle))
            {
                Debug.LogError("Bad terrain data path " + this.bundlePathToTerrainData);
            }
            try
            {
                this.terrain.terrainData = this.terrainDataFromBundle;
                this.RestoreTerrainSettings();
            }
            catch (Exception exception)
            {
                Debug.Log(exception, this);
                base.Invoke("ReassignTerrainData", this.reassignTerrainDataInterval);
            }
        }
    }

    private void Reset()
    {
        GameObject[] objArray = GameObject.FindGameObjectsWithTag("Main Terrain");
        if (objArray.Length > 0)
        {
            for (int i = 0; i < objArray.Length; i++)
            {
                this._terrain = objArray[i].GetComponent<Terrain>();
                if (this._terrain != null)
                {
                    break;
                }
            }
        }
    }

    [ContextMenu("Set settings to terrain")]
    private void RestoreTerrainSettings()
    {
        this.settings.CopyTo(this.terrain);
    }

    internal static void ter_flush()
    {
        if (activeTerrainControl != null)
        {
            activeTerrainControl.DoReassignmentOfTerrainData(false, true, false, false);
        }
    }

    internal static void ter_flushtrees()
    {
        if ((activeTerrainControl != null) && (activeTerrainControl._terrain != null))
        {
            TerrainHack.RefreshTreeTextures(activeTerrainControl._terrain);
        }
    }

    internal static void ter_mat()
    {
        if (activeTerrainControl != null)
        {
            activeTerrainControl.DoReassignmentOfTerrainData(false, false, true, false);
        }
    }

    internal static void ter_reassign()
    {
        if (activeTerrainControl != null)
        {
            activeTerrainControl.DoReassignmentOfTerrainData(true, false, false, false);
        }
    }

    internal static void ter_reassign_nocopy()
    {
        if (activeTerrainControl != null)
        {
            activeTerrainControl.DoReassignmentOfTerrainData(true, false, false, true);
        }
    }

    private void Update()
    {
        bool flag;
        MountedCamera camera;
        float idleinterval = terrain.idleinterval;
        if ((idleinterval <= 0f) || ((camera = MountedCamera.main) == null))
        {
            flag = true;
        }
        else
        {
            Vector3 vector3;
            Vector3 position = camera.transform.position;
            Vector3 forward = camera.transform.forward;
            forward.Normalize();
            vector3.x = position.x - this.lastCameraPosition.x;
            vector3.y = position.y - this.lastCameraPosition.y;
            vector3.z = position.z - this.lastCameraPosition.z;
            float num2 = ((vector3.x * vector3.x) + (vector3.y * vector3.y)) + (vector3.z * vector3.z);
            if ((num2 > 5.625E-05f) || (Vector3.Angle(forward, this.lastCameraForward) > 0.5f))
            {
                this.lastCameraPosition = position;
                this.lastCameraForward = forward;
                flag = true;
            }
            else
            {
                flag = false;
            }
        }
        float realtimeSinceStartup = Time.realtimeSinceStartup;
        if (flag)
        {
            this.timeNoticedCameraChange = realtimeSinceStartup;
        }
        else
        {
            float num4 = Time.realtimeSinceStartup - this.timeNoticedCameraChange;
            if (num4 > idleinterval)
            {
                this.timeNoticedCameraChange = (idleinterval <= 0f) ? realtimeSinceStartup : (realtimeSinceStartup - (num4 % idleinterval));
                TerrainHack.RefreshTreeTextures(this._terrain);
            }
        }
    }

    public float customBasemapDistance
    {
        get
        {
            return this._customBasemapDistance;
        }
        set
        {
            this._customBasemapDistance = value;
            this.BindTerrainSettings();
        }
    }

    public Terrain terrain
    {
        get
        {
            return this._terrain;
        }
    }

    [Serializable]
    private class TerrainSettingsHack
    {
        public float basemapDistance;
        public bool castShadows;
        public float detailObjectDensity;
        public float detailObjectDistance;
        public int heightmapMaximumLOD;
        public float heightmapPixelError;
        public Material materialTemplate;
        public float treeBillboardDistance;
        public float treeCrossFadeLength;
        public float treeDistance;
        public int treeMaximumFullLODCount;

        public void CopyFrom(Terrain terrain)
        {
            this.basemapDistance = terrain.basemapDistance;
            this.castShadows = terrain.castShadows;
            this.detailObjectDensity = terrain.detailObjectDensity;
            this.detailObjectDistance = terrain.detailObjectDistance;
            this.heightmapMaximumLOD = terrain.heightmapMaximumLOD;
            this.heightmapPixelError = terrain.heightmapPixelError;
            this.materialTemplate = terrain.materialTemplate;
            this.treeBillboardDistance = terrain.treeBillboardDistance;
            this.treeCrossFadeLength = terrain.treeCrossFadeLength;
            this.treeDistance = terrain.treeDistance;
            this.treeMaximumFullLODCount = terrain.treeMaximumFullLODCount;
        }

        public void CopyTo(Terrain terrain)
        {
            terrain.basemapDistance = this.basemapDistance;
            terrain.castShadows = this.castShadows;
            terrain.detailObjectDensity = this.detailObjectDensity;
            terrain.detailObjectDistance = this.detailObjectDistance;
            terrain.heightmapMaximumLOD = this.heightmapMaximumLOD;
            terrain.heightmapPixelError = this.heightmapPixelError;
            terrain.materialTemplate = this.materialTemplate;
            terrain.treeBillboardDistance = this.treeBillboardDistance;
            terrain.treeCrossFadeLength = this.treeCrossFadeLength;
            terrain.treeDistance = this.treeDistance;
            terrain.treeMaximumFullLODCount = this.treeMaximumFullLODCount;
        }
    }
}

