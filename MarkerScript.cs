using EasyRoads3D;
using System;
using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class MarkerScript : MonoBehaviour
{
    public bool autoUpdate;
    public bool bridgeObject;
    public bool changed;
    private float currentstamp;
    public string distance = "0";
    public bool distHeights;
    public float DODOQQOO;
    public ArrayList DQQOQQOO = new ArrayList();
    public float floorDepth = 2f;
    private int frameCount;
    public float li;
    public bool lockWaterLevel = true;
    private Vector3 lookAtPoint;
    public float ls;
    public float lt;
    public int markerNum;
    private bool mousedown;
    public bool newSegment;
    private float newstamp;
    public RoadObjectScript objectScript;
    public bool OCCCCODCOD;
    public Transform[] OCQOCOCQQOs;
    public ArrayList ODDDDQOO = new ArrayList();
    public bool[] ODDGDOOO;
    public string[] ODDOOQDO;
    public float[] ODDOQDO;
    public float ODDOQOQQ;
    public ArrayList ODDOQQOO = new ArrayList();
    public float ODDQODOO;
    public float[] ODDQOODO;
    public bool[] ODDQOOO;
    public float ODODQQOO;
    public float ODOOQQOO;
    public float[] ODOQODOO;
    public float ODOQQOOO;
    public ArrayList ODOQQQDO = new ArrayList();
    public bool ODQDOQOO;
    public float oldFloorDepth = 2f;
    public Vector3 oldPos = Vector3.zero;
    public bool OOCCDCOQCQ;
    public string OODDQCQQDD = "0";
    public float OOQOQQOO;
    public float OQCQOQQDCQ;
    public ArrayList OQODQQDO = new ArrayList();
    public string OQOQODQCQC = "0";
    public ArrayList OQQODQQOO = new ArrayList();
    private Vector3 position;
    public float ri;
    public float rs;
    public float rt;
    public bool sharpCorner;
    public Transform surface;
    public float tension = 0.5f;
    public float[] trperc;
    private bool updated;
    public float waterLevel = 0.5f;

    public void FloorDepth(float change, float perc)
    {
        this.floorDepth += change * perc;
        if (this.floorDepth > 0f)
        {
            this.floorDepth = 0f;
        }
        this.oldFloorDepth = this.floorDepth;
    }

    public bool InSelected()
    {
        for (int i = 0; i < this.objectScript.OCQOCOCQQOs.Length; i++)
        {
            if (this.objectScript.OCQOCOCQQOs[i] == base.gameObject)
            {
                return true;
            }
        }
        return false;
    }

    public void LeftIndent(float change, float perc)
    {
        this.ri += change * perc;
        if (this.ri < this.objectScript.indent)
        {
            this.ri = this.objectScript.indent;
        }
        this.OOQOQQOO = this.ri;
    }

    public void LeftSurrounding(float change, float perc)
    {
        this.rs += change * perc;
        if (this.rs < this.objectScript.indent)
        {
            this.rs = this.objectScript.indent;
        }
        this.ODOQQOOO = this.rs;
    }

    public void LeftTilting(float change, float perc)
    {
        this.rt += change * perc;
        if (this.rt < 0f)
        {
            this.rt = 0f;
        }
        this.ODDQODOO = this.rt;
    }

    private void OnDrawGizmos()
    {
        if (this.objectScript != null)
        {
            if (!this.objectScript.ODODCOCCDQ)
            {
                Vector3 vector = base.transform.position - this.oldPos;
                if ((this.OCCCCODCOD && (this.oldPos != Vector3.zero)) && (vector != Vector3.zero))
                {
                    int index = 0;
                    foreach (Transform transform in this.OCQOCOCQQOs)
                    {
                        transform.position += (Vector3) (vector * this.trperc[index]);
                        index++;
                    }
                }
                if ((this.oldPos != Vector3.zero) && (vector != Vector3.zero))
                {
                    this.changed = true;
                    if (this.objectScript.ODODCOCCDQ)
                    {
                        this.objectScript.OOQQCODOCD.specialRoadMaterial = true;
                    }
                }
                this.oldPos = base.transform.position;
            }
            else if (this.objectScript.ODODDDOO)
            {
                base.transform.position = this.oldPos;
            }
        }
    }

    public void RightIndent(float change, float perc)
    {
        this.li += change * perc;
        if (this.li < this.objectScript.indent)
        {
            this.li = this.objectScript.indent;
        }
        this.ODODQQOO = this.li;
    }

    public void RightSurrounding(float change, float perc)
    {
        this.ls += change * perc;
        if (this.ls < this.objectScript.indent)
        {
            this.ls = this.objectScript.indent;
        }
        this.DODOQQOO = this.ls;
    }

    public void RightTilting(float change, float perc)
    {
        this.lt += change * perc;
        if (this.lt < 0f)
        {
            this.lt = 0f;
        }
        this.ODDOQOQQ = this.lt;
    }

    private void SetObjectScript()
    {
        this.objectScript = base.transform.parent.parent.GetComponent<RoadObjectScript>();
        if (this.objectScript.OOQQCODOCD == null)
        {
            ArrayList arr = ODODDCCOQO.OCDCQOOODO(false);
            this.objectScript.OCOQDDODDQ(arr, ODODDCCOQO.OOQOOQODQQ(arr), ODODDCCOQO.OQQDOODOOQ(arr));
        }
    }

    private void Start()
    {
        IEnumerator enumerator = base.transform.GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                Transform current = (Transform) enumerator.Current;
                this.surface = current;
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
}

