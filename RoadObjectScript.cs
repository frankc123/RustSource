using EasyRoads3D;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadObjectScript : MonoBehaviour
{
    public bool applyAnimation;
    public bool applySplatmap;
    public bool autoODODDQQO;
    public bool autoUpdate = true;
    public static int backupLocation;
    public string[] backupStrings = new string[] { "Outside Assets folder path", "Inside Assets folder path" };
    public bool beveledRoad;
    public bool camInit;
    private Vector3 cPos;
    public GameObject customMesh;
    public static bool disableFreeAlerts = true;
    public bool displayRoad = true;
    public string distance = "0";
    public bool doFlyOver;
    private bool doRestore;
    public bool doTerrain;
    public bool editRestore = true;
    private Vector3 ePos;
    public static string erInit = string.Empty;
    public int expand;
    private float fl;
    public float floorDepth = 2f;
    public bool flyby;
    public bool forceY;
    public float geoResolution = 5f;
    public Camera goCam;
    private bool handleInsertFlag;
    public bool handleVegetation = true;
    public float indent = 3f;
    public float lastY;
    public Vector3[] leftVecs = new Vector3[0];
    public bool lockWaterLevel = true;
    public string markerDisplayStr = "Hide Markers";
    public int markers = 1;
    private string[] materialStrings;
    public int materialType;
    private MarkerScript[] mSc;
    public bool multipleTerrains;
    public Transform obj;
    public static string[] objectStrings;
    public string objectText = "Road";
    public int objectType;
    private static string OCQCDDDOCC;
    private Transform OCQOCOCQQO;
    public GameObject[] OCQOCOCQQOs;
    public static Transform OCQQQOQOQC;
    private bool ODDCQQQQOO;
    public float ODDQCCDCDC = 1f;
    public int ODOCDOOOQQ = -1;
    public bool ODODCOCCDQ;
    public bool ODODDDOO;
    public string[] ODODDQOO;
    private bool[] ODODODCODD;
    public static string[] ODODOQQO;
    public static string[] ODODQOOQ;
    public string[] ODODQOQO;
    public int[] ODODQOQOInt;
    public bool[] ODODQQOD;
    private bool ODOQCDDCOO;
    public static string[] ODOQDOQO;
    private GameObject ODOQDQOO;
    public int ODOQOOQO;
    public int ODQDOOQO;
    private bool ODQDOQOCCD;
    public int OdQODQOD = 1;
    public string[] ODQOOCCQQO;
    private bool ODQQOQCQCO;
    public string[] ODQQQQQO;
    public float offset;
    public int offsetX;
    public int offsetY;
    private float oldfl;
    public bool OOCCDCOQCQ;
    public float OOCQDOOCQD = 2f;
    private RoadObjectScript OODCCOODCC;
    public static GUISkin OODCDOQDCC;
    public Transform OODDDCQCOC;
    public string[] OODQCQODQQ;
    public static GUISkin OODQQDDCDD;
    public string[] OOOOCOCCDC;
    private string OOQCQCDDOQ;
    public bool OOQDOOQQ;
    public OQCDQQDQCC OOQQCODOCD;
    public float OOQQQDOD;
    public float OOQQQDODLength;
    public float OOQQQDODOffset;
    public int[] OOQQQOQO;
    public float opacity = 1f;
    private bool[] OQCCDQCDDD;
    public int OQDCQDCDDD = -1;
    public bool OQOCDODDQC;
    public static Texture2D OQOOODODQD;
    private TextAnchor origAnchor;
    private Vector3 pos;
    public float raise = 1f;
    public float raiseMarkers = 0.5f;
    public bool renderRoad = true;
    public Vector3[] rightVecs = new Vector3[0];
    public Texture2D roadMaterial;
    public Material roadMaterialEdit;
    public int roadResolution = 1;
    public Texture2D roadTexture;
    public float roadWidth = 5f;
    public int selectedWaterMaterial;
    public int selectedWaterScript;
    public float smoothDistance = 1f;
    public float smoothSurDistance = 3f;
    public bool snapY = true;
    public float speed = 1f;
    public int splatmapLayer = 4;
    public int splatmapSmoothLevel;
    private Material surfaceMaterial;
    public float surfaceOpacity = 1f;
    public float surrounding = 5f;
    public static GameObject tracer;
    public float tuw = 15f;
    public static string version = string.Empty;
    public float waterLevel = 1.5f;
    public float waveHeight = 0.15f;
    public float waveSize = 1.5f;
    public float yChange;

    public bool CheckWaterHeights()
    {
        bool flag = true;
        float y = Terrain.activeTerrain.transform.position.y;
        IEnumerator enumerator = this.obj.GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                Transform current = (Transform) enumerator.Current;
                if (current.name == "Markers")
                {
                    IEnumerator enumerator2 = current.GetEnumerator();
                    try
                    {
                        while (enumerator2.MoveNext())
                        {
                            Transform transform2 = (Transform) enumerator2.Current;
                            if ((transform2.position.y - y) <= 0.1f)
                            {
                                flag = false;
                            }
                        }
                        continue;
                    }
                    finally
                    {
                        IDisposable disposable = enumerator2 as IDisposable;
                        if (disposable == null)
                        {
                        }
                        disposable.Dispose();
                    }
                }
            }
        }
        finally
        {
            IDisposable disposable2 = enumerator as IDisposable;
            if (disposable2 == null)
            {
            }
            disposable2.Dispose();
        }
        return flag;
    }

    public void OCCOCQQQDO(MarkerScript markerScript)
    {
        this.OCQOCOCQQO = markerScript.transform;
        List<GameObject> list = new List<GameObject>();
        for (int i = 0; i < this.OCQOCOCQQOs.Length; i++)
        {
            if (this.OCQOCOCQQOs[i] != markerScript.gameObject)
            {
                list.Add(this.OCQOCOCQQOs[i]);
            }
        }
        list.Add(markerScript.gameObject);
        this.OCQOCOCQQOs = list.ToArray();
        this.OCQOCOCQQO = markerScript.transform;
        this.OOQQCODOCD.OODCDQDOCO(this.OCQOCOCQQO, this.OCQOCOCQQOs, markerScript.OCCCCODCOD, markerScript.OQCQOQQDCQ, this.OODDDCQCOC, out markerScript.OCQOCOCQQOs, out markerScript.trperc, this.OCQOCOCQQOs);
        this.ODOCDOOOQQ = -1;
    }

    public void OCCOOQDCQO()
    {
        if ((!this.ODODDDOO || (this.objectType == 2)) && (this.OQCCDQCDDD != null))
        {
            for (int i = 0; i < this.OQCCDQCDDD.Length; i++)
            {
                this.OQCCDQCDDD[i] = false;
                this.ODODODCODD[i] = false;
            }
        }
    }

    public void OCOOCODDOC(float geo, bool renderMode, bool camMode)
    {
        this.OOQQCODOCD.OOODOQDODQ.Clear();
        int num = 0;
        IEnumerator enumerator = this.obj.GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                Transform current = (Transform) enumerator.Current;
                if (current.name == "Markers")
                {
                    IEnumerator enumerator2 = current.GetEnumerator();
                    try
                    {
                        while (enumerator2.MoveNext())
                        {
                            Transform marker = (Transform) enumerator2.Current;
                            MarkerScript component = marker.GetComponent<MarkerScript>();
                            component.objectScript = this.obj.GetComponent<RoadObjectScript>();
                            if (!component.OOCCDCOQCQ)
                            {
                                component.OOCCDCOQCQ = this.OOQQCODOCD.OOOCQDOCDC(marker);
                            }
                            OQDQOQDOQO oqdqoqdoqo = new OQDQOQDOQO {
                                position = marker.position,
                                num = this.OOQQCODOCD.OOODOQDODQ.Count,
                                object1 = marker,
                                object2 = component.surface,
                                tension = component.tension,
                                ri = component.ri
                            };
                            if (oqdqoqdoqo.ri < 1f)
                            {
                                oqdqoqdoqo.ri = 1f;
                            }
                            oqdqoqdoqo.li = component.li;
                            if (oqdqoqdoqo.li < 1f)
                            {
                                oqdqoqdoqo.li = 1f;
                            }
                            oqdqoqdoqo.rt = component.rt;
                            oqdqoqdoqo.lt = component.lt;
                            oqdqoqdoqo.rs = component.rs;
                            if (oqdqoqdoqo.rs < 1f)
                            {
                                oqdqoqdoqo.rs = 1f;
                            }
                            oqdqoqdoqo.OQDOOODDQD = component.rs;
                            oqdqoqdoqo.ls = component.ls;
                            if (oqdqoqdoqo.ls < 1f)
                            {
                                oqdqoqdoqo.ls = 1f;
                            }
                            oqdqoqdoqo.OOOCDQODDO = component.ls;
                            oqdqoqdoqo.renderFlag = component.bridgeObject;
                            oqdqoqdoqo.OCCOQCQDOD = component.distHeights;
                            oqdqoqdoqo.newSegment = component.newSegment;
                            oqdqoqdoqo.floorDepth = component.floorDepth;
                            oqdqoqdoqo.waterLevel = this.waterLevel;
                            oqdqoqdoqo.lockWaterLevel = component.lockWaterLevel;
                            oqdqoqdoqo.sharpCorner = component.sharpCorner;
                            oqdqoqdoqo.OQCDCODODQ = this.OOQQCODOCD;
                            component.markerNum = num;
                            component.distance = "-1";
                            component.OODDQCQQDD = "-1";
                            this.OOQQCODOCD.OOODOQDODQ.Add(oqdqoqdoqo);
                            num++;
                        }
                        continue;
                    }
                    finally
                    {
                        IDisposable disposable = enumerator2 as IDisposable;
                        if (disposable == null)
                        {
                        }
                        disposable.Dispose();
                    }
                }
            }
        }
        finally
        {
            IDisposable disposable2 = enumerator as IDisposable;
            if (disposable2 == null)
            {
            }
            disposable2.Dispose();
        }
        this.distance = "-1";
        this.OOQQCODOCD.ODQQQCQCOO = this.OODCCOODCC.roadWidth;
        this.OOQQCODOCD.ODOCODQOOC(geo, this.obj, this.OODCCOODCC.OOQDOOQQ, renderMode, camMode, this.objectType);
        if (this.OOQQCODOCD.leftVecs.Count > 0)
        {
            this.leftVecs = this.OOQQCODOCD.leftVecs.ToArray();
            this.rightVecs = this.OOQQCODOCD.rightVecs.ToArray();
        }
    }

    public void OCOQDCQOCD(MarkerScript markerScript)
    {
        if ((markerScript.OQCQOQQDCQ != markerScript.ODOOQQOO) || (markerScript.OQCQOQQDCQ != markerScript.ODOOQQOO))
        {
            this.OOQQCODOCD.OODCDQDOCO(this.OCQOCOCQQO, this.OCQOCOCQQOs, markerScript.OCCCCODCOD, markerScript.OQCQOQQDCQ, this.OODDDCQCOC, out markerScript.OCQOCOCQQOs, out markerScript.trperc, this.OCQOCOCQQOs);
            markerScript.ODQDOQOO = markerScript.OCCCCODCOD;
            markerScript.ODOOQQOO = markerScript.OQCQOQQDCQ;
        }
        if (this.OODCCOODCC.autoUpdate)
        {
            this.OCOOCODDOC(this.OODCCOODCC.geoResolution, false, false);
        }
    }

    public void OCOQDDODDQ(ArrayList arr, string[] DOODQOQO, string[] OODDQOQO)
    {
        this.ODOCOQCCOC(base.transform, arr, DOODQOQO, OODDQOQO);
    }

    public void OCQDCQDDCO()
    {
        this.OOQQCODOCD.OCQDCQDDCO(this.OODCCOODCC.applySplatmap, this.OODCCOODCC.splatmapSmoothLevel, this.OODCCOODCC.renderRoad, this.OODCCOODCC.tuw, this.OODCCOODCC.roadResolution, this.OODCCOODCC.raise, this.OODCCOODCC.opacity, this.OODCCOODCC.expand, this.OODCCOODCC.offsetX, this.OODCCOODCC.offsetY, this.OODCCOODCC.beveledRoad, this.OODCCOODCC.splatmapLayer, this.OODCCOODCC.OdQODQOD, this.OOQQQDOD, this.OOQQQDODOffset, this.OOQQQDODLength);
    }

    public ArrayList ODCOQCODCC()
    {
        ArrayList list = new ArrayList();
        IEnumerator enumerator = this.obj.GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                Transform current = (Transform) enumerator.Current;
                if (current.name == "Markers")
                {
                    IEnumerator enumerator2 = current.GetEnumerator();
                    try
                    {
                        while (enumerator2.MoveNext())
                        {
                            Transform transform2 = (Transform) enumerator2.Current;
                            MarkerScript component = transform2.GetComponent<MarkerScript>();
                            list.Add(component.ODDGDOOO);
                            list.Add(component.ODDQOODO);
                            if (transform2.name == "Marker0003")
                            {
                            }
                            list.Add(component.ODDQOOO);
                        }
                        continue;
                    }
                    finally
                    {
                        IDisposable disposable = enumerator2 as IDisposable;
                        if (disposable == null)
                        {
                        }
                        disposable.Dispose();
                    }
                }
            }
        }
        finally
        {
            IDisposable disposable2 = enumerator as IDisposable;
            if (disposable2 == null)
            {
            }
            disposable2.Dispose();
        }
        return list;
    }

    public void ODCQOCDQOC()
    {
        Object.DestroyImmediate(this.OODCCOODCC.OCQOCOCQQO.gameObject);
        this.OCQOCOCQQO = null;
        this.OOQODQOCOC();
    }

    public void ODDOOODDCQ()
    {
        RoadObjectScript[] scriptArray = (RoadObjectScript[]) Object.FindObjectsOfType(typeof(RoadObjectScript));
        ArrayList objs = new ArrayList();
        foreach (RoadObjectScript script in scriptArray)
        {
            if (script.transform != base.transform)
            {
                objs.Add(script.transform);
            }
        }
        if (this.ODODQOQO == null)
        {
            this.ODODQOQO = this.OOQQCODOCD.OCDODCOCOC();
            this.ODODQOQOInt = this.OOQQCODOCD.OCCQOQCQDO();
        }
        this.OCOOCODDOC(0.5f, true, false);
        this.OOQQCODOCD.OCOOOOCOQO(Vector3.zero, this.OODCCOODCC.raise, this.obj, this.OODCCOODCC.OOQDOOQQ, objs, this.handleVegetation);
        this.OCQDCQDDCO();
    }

    public void ODOCOQCCOC(Transform tr, ArrayList arr, string[] DOODQOQO, string[] OODDQOQO)
    {
        version = "2.4.6";
        OODCDOQDCC = (GUISkin) Resources.Load("ER3DSkin", typeof(GUISkin));
        OQOOODODQD = (Texture2D) Resources.Load("ER3DLogo", typeof(Texture2D));
        if (objectStrings == null)
        {
            objectStrings = new string[] { "Road Object", "River Object", "Procedural Mesh Object" };
        }
        this.obj = tr;
        this.OOQQCODOCD = new OQCDQQDQCC();
        this.OODCCOODCC = this.obj.GetComponent<RoadObjectScript>();
        IEnumerator enumerator = this.obj.GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                Transform current = (Transform) enumerator.Current;
                if (current.name == "Markers")
                {
                    this.OODDDCQCOC = current;
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
        OQCDQQDQCC.terrainList.Clear();
        Terrain[] terrainArray = (Terrain[]) Object.FindObjectsOfType(typeof(Terrain));
        foreach (Terrain terrain in terrainArray)
        {
            Terrains t = new Terrains {
                terrain = terrain
            };
            if (terrain.gameObject.GetComponent<EasyRoads3DTerrainID>() == null)
            {
                EasyRoads3DTerrainID nid = (EasyRoads3DTerrainID) terrain.gameObject.AddComponent("EasyRoads3DTerrainID");
                string str = Random.Range(0x5f5e100, 0x3b9ac9ff).ToString();
                nid.terrainid = str;
                t.id = str;
            }
            else
            {
                t.id = terrain.gameObject.GetComponent<EasyRoads3DTerrainID>().terrainid;
            }
            this.OOQQCODOCD.OCDQQCDOQO(t);
        }
        ODCDDDDQQD.OCDQQCDOQO();
        if (this.roadMaterialEdit == null)
        {
            this.roadMaterialEdit = (Material) Resources.Load("materials/roadMaterialEdit", typeof(Material));
        }
        if ((this.objectType == 0) && (GameObject.Find(base.gameObject.name + "/road") == null))
        {
            GameObject obj2 = new GameObject("road") {
                transform = { parent = base.transform }
            };
        }
        this.OOQQCODOCD.OODQOQCDCQ(this.obj, OCQCDDDOCC, this.OODCCOODCC.roadWidth, this.surfaceOpacity, out this.OOCCDCOQCQ, out this.indent, this.applyAnimation, this.waveSize, this.waveHeight);
        this.OOQQCODOCD.ODDQCCDCDC = this.ODDQCCDCDC;
        this.OOQQCODOCD.OOCQDOOCQD = this.OOCQDOOCQD;
        this.OOQQCODOCD.OdQODQOD = this.OdQODQOD + 1;
        this.OOQQCODOCD.OOQQQDOD = this.OOQQQDOD;
        this.OOQQCODOCD.OOQQQDODOffset = this.OOQQQDODOffset;
        this.OOQQCODOCD.OOQQQDODLength = this.OOQQQDODLength;
        this.OOQQCODOCD.objectType = this.objectType;
        this.OOQQCODOCD.snapY = this.snapY;
        this.OOQQCODOCD.terrainRendered = this.ODODCOCCDQ;
        this.OOQQCODOCD.handleVegetation = this.handleVegetation;
        this.OOQQCODOCD.raise = this.raise;
        this.OOQQCODOCD.roadResolution = this.roadResolution;
        this.OOQQCODOCD.multipleTerrains = this.multipleTerrains;
        this.OOQQCODOCD.editRestore = this.editRestore;
        this.OOQQCODOCD.roadMaterialEdit = this.roadMaterialEdit;
        if (backupLocation == 0)
        {
            OOCDQCOODC.backupFolder = "/EasyRoads3D";
        }
        else
        {
            OOCDQCOODC.backupFolder = "/Assets/EasyRoads3D/backups";
        }
        this.ODODQOQO = this.OOQQCODOCD.OCDODCOCOC();
        this.ODODQOQOInt = this.OOQQCODOCD.OCCQOQCQDO();
        if (this.ODODCOCCDQ)
        {
            this.doRestore = true;
        }
        this.OOQODQOCOC();
        if ((arr != null) || (ODODQOOQ == null))
        {
            this.OOOOOOODCD(arr, DOODQOQO, OODDQOQO);
        }
        if (!this.doRestore)
        {
        }
    }

    public void ODQDCQQDDO(Vector3 pos, bool doInsert)
    {
        if (!this.displayRoad)
        {
            this.displayRoad = true;
            this.OOQQCODOCD.OODDDCQCCQ(this.displayRoad, this.OODDDCQCOC);
        }
        int first = -1;
        int second = -1;
        float num3 = 10000f;
        float num4 = 10000f;
        Vector3 tmpPos = pos;
        OQDQOQDOQO oqdqoqdoqo2 = (OQDQOQDOQO) this.OOQQCODOCD.OOODOQDODQ[0];
        OQDQOQDOQO oqdqoqdoqo3 = (OQDQOQDOQO) this.OOQQCODOCD.OOODOQDODQ[1];
        this.OOQQCODOCD.ODDDDCCDCO(pos, out first, out second, out num3, out num4, out oqdqoqdoqo2, out oqdqoqdoqo3, out tmpPos);
        pos = tmpPos;
        if ((doInsert && (first >= 0)) && (second >= 0))
        {
            if (this.OODCCOODCC.OOQDOOQQ && (second == (this.OOQQCODOCD.OOODOQDODQ.Count - 1)))
            {
                this.OODDQODDCC(pos);
            }
            else
            {
                OQDQOQDOQO oqdqoqdoqo = (OQDQOQDOQO) this.OOQQCODOCD.OOODOQDODQ[second];
                string name = oqdqoqdoqo.object1.name;
                int num5 = second + 2;
                for (int i = second; i < (this.OOQQCODOCD.OOODOQDODQ.Count - 1); i++)
                {
                    string str2;
                    oqdqoqdoqo = (OQDQOQDOQO) this.OOQQCODOCD.OOODOQDODQ[i];
                    if (num5 < 10)
                    {
                        str2 = "Marker000" + num5.ToString();
                    }
                    else if (num5 < 100)
                    {
                        str2 = "Marker00" + num5.ToString();
                    }
                    else
                    {
                        str2 = "Marker0" + num5.ToString();
                    }
                    oqdqoqdoqo.object1.name = str2;
                    num5++;
                }
                oqdqoqdoqo = (OQDQOQDOQO) this.OOQQCODOCD.OOODOQDODQ[first];
                Transform transform = (Transform) Object.Instantiate(oqdqoqdoqo.object1.transform, pos, oqdqoqdoqo.object1.rotation);
                transform.gameObject.name = name;
                transform.parent = this.OODDDCQCOC;
                MarkerScript component = transform.GetComponent<MarkerScript>();
                component.OOCCDCOQCQ = false;
                float num7 = num3 + num4;
                float num8 = num3 / num7;
                float num9 = oqdqoqdoqo2.ri - oqdqoqdoqo3.ri;
                component.ri = oqdqoqdoqo2.ri - (num9 * num8);
                num9 = oqdqoqdoqo2.li - oqdqoqdoqo3.li;
                component.li = oqdqoqdoqo2.li - (num9 * num8);
                num9 = oqdqoqdoqo2.rt - oqdqoqdoqo3.rt;
                component.rt = oqdqoqdoqo2.rt - (num9 * num8);
                num9 = oqdqoqdoqo2.lt - oqdqoqdoqo3.lt;
                component.lt = oqdqoqdoqo2.lt - (num9 * num8);
                num9 = oqdqoqdoqo2.rs - oqdqoqdoqo3.rs;
                component.rs = oqdqoqdoqo2.rs - (num9 * num8);
                num9 = oqdqoqdoqo2.ls - oqdqoqdoqo3.ls;
                component.ls = oqdqoqdoqo2.ls - (num9 * num8);
                this.OCOOCODDOC(this.OODCCOODCC.geoResolution, false, false);
                if (this.materialType == 0)
                {
                    this.OOQQCODOCD.OOQOOCDQOD(this.materialType);
                }
                if (this.objectType == 2)
                {
                    component.surface.gameObject.SetActive(false);
                }
            }
        }
        this.OOQODQOCOC();
    }

    public void ODQDOOOCOC()
    {
        this.OCOOCODDOC(this.OODCCOODCC.geoResolution, false, false);
        if (this.OOQQCODOCD != null)
        {
            this.OOQQCODOCD.ODQDOOOCOC();
        }
        this.ODODDDOO = false;
    }

    public void OODDQODDCC(Vector3 pos)
    {
        string str;
        if (!this.displayRoad)
        {
            this.displayRoad = true;
            this.OOQQCODOCD.OODDDCQCCQ(this.displayRoad, this.OODDDCQCOC);
        }
        pos.y += this.OODCCOODCC.raiseMarkers;
        if (this.forceY && (this.ODOQDQOO != null))
        {
            float num = Vector3.Distance(pos, this.ODOQDQOO.transform.position);
            pos.y = this.ODOQDQOO.transform.position.y + (this.yChange * (num / 100f));
        }
        else if (this.forceY && (this.markers == 0))
        {
            this.lastY = pos.y;
        }
        GameObject obj2 = null;
        if (this.ODOQDQOO != null)
        {
            obj2 = (GameObject) Object.Instantiate(this.ODOQDQOO);
        }
        else
        {
            obj2 = (GameObject) Object.Instantiate(Resources.Load("marker", typeof(GameObject)));
        }
        Transform transform = obj2.transform;
        transform.position = pos;
        transform.parent = this.OODDDCQCOC;
        this.markers++;
        if (this.markers < 10)
        {
            str = "Marker000" + this.markers.ToString();
        }
        else if (this.markers < 100)
        {
            str = "Marker00" + this.markers.ToString();
        }
        else
        {
            str = "Marker0" + this.markers.ToString();
        }
        transform.gameObject.name = str;
        MarkerScript component = transform.GetComponent<MarkerScript>();
        component.OOCCDCOQCQ = false;
        component.objectScript = this.obj.GetComponent<RoadObjectScript>();
        if (this.ODOQDQOO == null)
        {
            component.waterLevel = this.OODCCOODCC.waterLevel;
            component.floorDepth = this.OODCCOODCC.floorDepth;
            component.ri = this.OODCCOODCC.indent;
            component.li = this.OODCCOODCC.indent;
            component.rs = this.OODCCOODCC.surrounding;
            component.ls = this.OODCCOODCC.surrounding;
            component.tension = 0.5f;
            if (this.objectType == 1)
            {
                pos.y -= this.waterLevel;
                transform.position = pos;
            }
        }
        if ((this.objectType == 2) && (component.surface != null))
        {
            component.surface.gameObject.SetActive(false);
        }
        this.ODOQDQOO = transform.gameObject;
        if (this.markers > 1)
        {
            this.OCOOCODDOC(this.OODCCOODCC.geoResolution, false, false);
            if (this.materialType == 0)
            {
                this.OOQQCODOCD.OOQOOCDQOD(this.materialType);
            }
        }
    }

    public void OOOOOOODCD(ArrayList arr, string[] DOODQOQO, string[] OODDQOQO)
    {
        bool flag = false;
        ODODOQQO = DOODQOQO;
        ODODQOOQ = OODDQOQO;
        ArrayList list = new ArrayList();
        if (this.obj == null)
        {
            this.ODOCOQCCOC(base.transform, null, null, null);
        }
        IEnumerator enumerator = this.obj.GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                Transform current = (Transform) enumerator.Current;
                if (current.name == "Markers")
                {
                    IEnumerator enumerator2 = current.GetEnumerator();
                    try
                    {
                        while (enumerator2.MoveNext())
                        {
                            MarkerScript component = ((Transform) enumerator2.Current).GetComponent<MarkerScript>();
                            component.OQODQQDO.Clear();
                            component.ODOQQQDO.Clear();
                            component.OQQODQQOO.Clear();
                            component.ODDOQQOO.Clear();
                            list.Add(component);
                        }
                        continue;
                    }
                    finally
                    {
                        IDisposable disposable = enumerator2 as IDisposable;
                        if (disposable == null)
                        {
                        }
                        disposable.Dispose();
                    }
                }
            }
        }
        finally
        {
            IDisposable disposable2 = enumerator as IDisposable;
            if (disposable2 == null)
            {
            }
            disposable2.Dispose();
        }
        this.mSc = (MarkerScript[]) list.ToArray(typeof(MarkerScript));
        ArrayList list2 = new ArrayList();
        int num = 0;
        int num2 = 0;
        if (this.ODQQQQQO != null)
        {
            if (arr.Count == 0)
            {
                return;
            }
            for (int k = 0; k < ODODOQQO.Length; k++)
            {
                ODODDQQO ododdqqo = (ODODDQQO) arr[k];
                for (int m = 0; m < this.ODQQQQQO.Length; m++)
                {
                    if (ODODOQQO[k] == this.ODQQQQQO[m])
                    {
                        num++;
                        if (this.ODODQQOD.Length > m)
                        {
                            list2.Add(this.ODODQQOD[m]);
                        }
                        else
                        {
                            list2.Add(false);
                        }
                        foreach (MarkerScript script2 in this.mSc)
                        {
                            int index = -1;
                            for (int n = 0; n < script2.ODDOOQDO.Length; n++)
                            {
                                if (ododdqqo.id == script2.ODDOOQDO[n])
                                {
                                    index = n;
                                    break;
                                }
                            }
                            if (index >= 0)
                            {
                                script2.OQODQQDO.Add(script2.ODDOOQDO[index]);
                                script2.ODOQQQDO.Add(script2.ODDGDOOO[index]);
                                script2.OQQODQQOO.Add(script2.ODDQOOO[index]);
                                if ((ododdqqo.sidewaysDistanceUpdate == 0) || ((ododdqqo.sidewaysDistanceUpdate == 2) && (script2.ODDQOODO[index] != ododdqqo.oldSidwaysDistance)))
                                {
                                    script2.ODDOQQOO.Add(script2.ODDQOODO[index]);
                                }
                                else
                                {
                                    script2.ODDOQQOO.Add(ododdqqo.splinePosition);
                                }
                            }
                            else
                            {
                                script2.OQODQQDO.Add(ododdqqo.id);
                                script2.ODOQQQDO.Add(ododdqqo.markerActive);
                                script2.OQQODQQOO.Add(true);
                                script2.ODDOQQOO.Add(ododdqqo.splinePosition);
                            }
                        }
                    }
                }
                if (ododdqqo.sidewaysDistanceUpdate != 0)
                {
                }
                flag = false;
            }
        }
        for (int i = 0; i < ODODOQQO.Length; i++)
        {
            ODODDQQO ododdqqo2 = (ODODDQQO) arr[i];
            bool flag2 = false;
            for (int num9 = 0; num9 < this.ODQQQQQO.Length; num9++)
            {
                if (ODODOQQO[i] == this.ODQQQQQO[num9])
                {
                    flag2 = true;
                }
            }
            if (!flag2)
            {
                num2++;
                list2.Add(false);
                foreach (MarkerScript script3 in this.mSc)
                {
                    script3.OQODQQDO.Add(ododdqqo2.id);
                    script3.ODOQQQDO.Add(ododdqqo2.markerActive);
                    script3.OQQODQQOO.Add(true);
                    script3.ODDOQQOO.Add(ododdqqo2.splinePosition);
                }
            }
        }
        this.ODODQQOD = (bool[]) list2.ToArray(typeof(bool));
        this.ODQQQQQO = new string[ODODOQQO.Length];
        ODODOQQO.CopyTo(this.ODQQQQQO, 0);
        ArrayList list3 = new ArrayList();
        for (int j = 0; j < this.ODODQQOD.Length; j++)
        {
            if (this.ODODQQOD[j])
            {
                list3.Add(j);
            }
        }
        this.OOQQQOQO = (int[]) list3.ToArray(typeof(int));
        foreach (MarkerScript script4 in this.mSc)
        {
            script4.ODDOOQDO = (string[]) script4.OQODQQDO.ToArray(typeof(string));
            script4.ODDGDOOO = (bool[]) script4.ODOQQQDO.ToArray(typeof(bool));
            script4.ODDQOOO = (bool[]) script4.OQQODQQOO.ToArray(typeof(bool));
            script4.ODDQOODO = (float[]) script4.ODDOQQOO.ToArray(typeof(float));
        }
        if (!flag)
        {
        }
    }

    public void OOOOQCDODD(MarkerScript markerScript)
    {
        if (markerScript.OQCQOQQDCQ != markerScript.ODOOQQOO)
        {
            this.OOQQCODOCD.OODCDQDOCO(this.OCQOCOCQQO, this.OCQOCOCQQOs, markerScript.OCCCCODCOD, markerScript.OQCQOQQDCQ, this.OODDDCQCOC, out markerScript.OCQOCOCQQOs, out markerScript.trperc, this.OCQOCOCQQOs);
            markerScript.ODOOQQOO = markerScript.OQCQOQQDCQ;
        }
        this.OCOOCODDOC(this.OODCCOODCC.geoResolution, false, false);
    }

    public void OOQODQOCOC()
    {
        int num = 0;
        IEnumerator enumerator = this.obj.GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                Transform current = (Transform) enumerator.Current;
                if (current.name == "Markers")
                {
                    num = 1;
                    IEnumerator enumerator2 = current.GetEnumerator();
                    try
                    {
                        while (enumerator2.MoveNext())
                        {
                            string str;
                            Transform transform2 = (Transform) enumerator2.Current;
                            if (num < 10)
                            {
                                str = "Marker000" + num.ToString();
                            }
                            else if (num < 100)
                            {
                                str = "Marker00" + num.ToString();
                            }
                            else
                            {
                                str = "Marker0" + num.ToString();
                            }
                            transform2.name = str;
                            this.ODOQDQOO = transform2.gameObject;
                            num++;
                        }
                        continue;
                    }
                    finally
                    {
                        IDisposable disposable = enumerator2 as IDisposable;
                        if (disposable == null)
                        {
                        }
                        disposable.Dispose();
                    }
                }
            }
        }
        finally
        {
            IDisposable disposable2 = enumerator as IDisposable;
            if (disposable2 == null)
            {
            }
            disposable2.Dispose();
        }
        this.markers = num - 1;
        this.OCOOCODDOC(this.OODCCOODCC.geoResolution, false, false);
    }

    public void OQCOCQDQDD()
    {
        ArrayList list = new ArrayList();
        ArrayList list2 = new ArrayList();
        ArrayList list3 = new ArrayList();
        for (int i = 0; i < ODODOQQO.Length; i++)
        {
            if (this.ODODQQOD[i])
            {
                list.Add(ODODQOOQ[i]);
                list3.Add(ODODOQQO[i]);
                list2.Add(i);
            }
        }
        this.ODODDQOO = (string[]) list.ToArray(typeof(string));
        this.OOQQQOQO = (int[]) list2.ToArray(typeof(int));
    }

    public void OQCQQDODDC()
    {
        if (this.OOQQCODOCD == null)
        {
            this.ODOCOQCCOC(base.transform, null, null, null);
        }
        OQCDQQDQCC.ODOQCCODQC = true;
        if (!this.ODODCOCCDQ)
        {
            this.geoResolution = 0.5f;
            this.ODODCOCCDQ = true;
            this.doTerrain = false;
            this.OOQODQOCOC();
            if (this.objectType < 2)
            {
                this.ODDOOODDCQ();
            }
            this.OOQQCODOCD.terrainRendered = true;
            this.OCQDCQDDCO();
        }
        if (this.displayRoad && (this.objectType < 2))
        {
            Material material = (Material) Resources.Load("roadMaterial", typeof(Material));
            if (this.OOQQCODOCD.road.renderer != null)
            {
                this.OOQQCODOCD.road.renderer.material = material;
            }
            IEnumerator enumerator = this.OOQQCODOCD.road.transform.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    if (current.gameObject.renderer != null)
                    {
                        current.gameObject.renderer.material = material;
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
            this.OOQQCODOCD.road.transform.parent = null;
            this.OOQQCODOCD.road.layer = 0;
            this.OOQQCODOCD.road.name = base.gameObject.name;
        }
        else if (this.OOQQCODOCD.road != null)
        {
            Object.DestroyImmediate(this.OOQQCODOCD.road);
        }
    }

    private void OQDODCODOQ(string ctrl, MarkerScript markerScript)
    {
        int index = 0;
        foreach (Transform transform in markerScript.OCQOCOCQQOs)
        {
            MarkerScript component = transform.GetComponent<MarkerScript>();
            if (ctrl == "rs")
            {
                component.LeftSurrounding(markerScript.rs - markerScript.ODOQQOOO, markerScript.trperc[index]);
            }
            else if (ctrl == "ls")
            {
                component.RightSurrounding(markerScript.ls - markerScript.DODOQQOO, markerScript.trperc[index]);
            }
            else if (ctrl == "ri")
            {
                component.LeftIndent(markerScript.ri - markerScript.OOQOQQOO, markerScript.trperc[index]);
            }
            else if (ctrl == "li")
            {
                component.RightIndent(markerScript.li - markerScript.ODODQQOO, markerScript.trperc[index]);
            }
            else if (ctrl == "rt")
            {
                component.LeftTilting(markerScript.rt - markerScript.ODDQODOO, markerScript.trperc[index]);
            }
            else if (ctrl == "lt")
            {
                component.RightTilting(markerScript.lt - markerScript.ODDOQOQQ, markerScript.trperc[index]);
            }
            else if (ctrl == "floorDepth")
            {
                component.FloorDepth(markerScript.floorDepth - markerScript.oldFloorDepth, markerScript.trperc[index]);
            }
            index++;
        }
    }

    public void OQOCODCDOO()
    {
        if (this.markers > 1)
        {
            this.OCOOCODDOC(this.OODCCOODCC.geoResolution, false, false);
        }
    }

    public void OQQDQCQQOC()
    {
        this.OOQQCODOCD.OQQDQCQQOC(this.OODCCOODCC.renderRoad, this.OODCCOODCC.tuw, this.OODCCOODCC.roadResolution, this.OODCCOODCC.raise, this.OODCCOODCC.beveledRoad, this.OODCCOODCC.OdQODQOD, this.OOQQQDOD, this.OOQQQDODOffset, this.OOQQQDODLength);
    }

    public void OQQOOCCQCO()
    {
        this.OOQQCODOCD.OOQDODCQOQ(12);
    }

    public ArrayList RebuildObjs()
    {
        RoadObjectScript[] scriptArray = (RoadObjectScript[]) Object.FindObjectsOfType(typeof(RoadObjectScript));
        ArrayList list = new ArrayList();
        foreach (RoadObjectScript script in scriptArray)
        {
            if (script.transform != base.transform)
            {
                list.Add(script.transform);
            }
        }
        return list;
    }

    public void ResetMaterials(MarkerScript markerScript)
    {
        if (this.OOQQCODOCD != null)
        {
            this.OOQQCODOCD.OODCDQDOCO(this.OCQOCOCQQO, this.OCQOCOCQQOs, markerScript.OCCCCODCOD, markerScript.OQCQOQQDCQ, this.OODDDCQCOC, out markerScript.OCQOCOCQQOs, out markerScript.trperc, this.OCQOCOCQQOs);
        }
    }

    public void StartCam()
    {
        this.OCOOCODDOC(0.5f, false, true);
    }

    public void UpdateBackupFolder()
    {
    }
}

