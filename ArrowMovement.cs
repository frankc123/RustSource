using System;
using UnityEngine;

public class ArrowMovement : MonoBehaviour
{
    public ItemRepresentation _myBow;
    public IBowWeaponItem _myItemInstance;
    private float distance;
    public float dropDegreesPerSec = 5f;
    public bool impacted;
    public float lastUpdateTime;
    private int layerMask = 0x183e1411;
    private float maxLifeTime = 4f;
    public float maxRange = 1000f;
    private bool reported;
    public float spawnTime;
    public float speedPerSec = 80f;

    public void Init(float arrowSpeed, ItemRepresentation itemRep, IBowWeaponItem itemInstance, bool firedLocal)
    {
        this.speedPerSec = arrowSpeed;
        if ((itemRep != null) && (itemInstance != null))
        {
            this._myBow = itemRep;
            this._myItemInstance = itemInstance;
        }
    }

    private void OnDestroy()
    {
        if (!this.impacted)
        {
            this.TryReportMiss();
        }
    }

    private void Start()
    {
        this.spawnTime = Time.time;
        this.lastUpdateTime = Time.time;
    }

    public void TryReportHit(GameObject hitGameObject)
    {
        if ((this._myItemInstance != null) && !this.reported)
        {
            this.reported = true;
            IDMain hitMain = IDBase.GetMain(hitGameObject);
            this._myItemInstance.ArrowReportHit(hitMain, this);
        }
    }

    public void TryReportMiss()
    {
        if ((this._myItemInstance != null) && !this.reported)
        {
            this.reported = true;
            this._myItemInstance.ArrowReportMiss(this);
        }
    }

    private void Update()
    {
        if (!this.impacted)
        {
            float num = Time.time - this.lastUpdateTime;
            this.lastUpdateTime = Time.time;
            RaycastHit hit = new RaycastHit();
            RaycastHit2 invalid = RaycastHit2.invalid;
            base.transform.Rotate(Vector3.right, (float) (this.dropDegreesPerSec * num));
            Ray ray = new Ray(base.transform.position, base.transform.forward);
            float num2 = this.speedPerSec * num;
            bool flag = true;
            if (!Physics2.Raycast2(ray, out invalid, this.speedPerSec * num, this.layerMask))
            {
                Transform transform = base.transform;
                transform.position += (Vector3) ((base.transform.forward * this.speedPerSec) * num);
                this.distance += this.speedPerSec * num;
            }
            else
            {
                Vector3 point;
                Vector3 normal;
                GameObject gameObject;
                Rigidbody rigidbody;
                if (flag)
                {
                    normal = invalid.normal;
                    point = invalid.point;
                    gameObject = invalid.gameObject;
                    rigidbody = invalid.rigidbody;
                }
                else
                {
                    normal = hit.normal;
                    point = hit.point;
                    gameObject = hit.collider.gameObject;
                    rigidbody = hit.rigidbody;
                }
                Quaternion rotation = Quaternion.LookRotation(normal);
                Vector3 zero = Vector3.zero;
                int layer = gameObject.layer;
                bool flag2 = true;
                if (((rigidbody != null) && !rigidbody.isKinematic) && !rigidbody.CompareTag("Door"))
                {
                    rigidbody.AddForceAtPosition((Vector3) (Vector3.up * 200f), point);
                    rigidbody.AddForceAtPosition((Vector3) (ray.direction * 1000f), point);
                }
                switch (layer)
                {
                    case 0x11:
                    case 0x12:
                    case 0x1b:
                    case 0x15:
                        flag2 = false;
                        break;

                    default:
                        zero = point + ((Vector3) (normal * 0.01f));
                        break;
                }
                this.impacted = true;
                base.transform.position = point;
                this.TryReportHit(gameObject);
                base.transform.parent = gameObject.transform;
                TrailRenderer component = base.GetComponent<TrailRenderer>();
                if (component != null)
                {
                    component.enabled = false;
                }
                base.audio.enabled = false;
                if (gameObject != null)
                {
                    SurfaceInfoObject surfaceInfoFor = SurfaceInfo.GetSurfaceInfoFor(gameObject, point);
                    surfaceInfoFor.GetImpactEffect(SurfaceInfoObject.ImpactType.Bullet);
                    Object.Destroy(Object.Instantiate(surfaceInfoFor.GetImpactEffect(SurfaceInfoObject.ImpactType.Bullet), point, rotation), 1.5f);
                    this.TryReportMiss();
                }
                Object.Destroy(base.gameObject, 20f);
            }
            if ((this.distance > this.maxRange) || ((Time.time - this.spawnTime) > this.maxLifeTime))
            {
                Object.Destroy(base.gameObject);
            }
        }
    }
}

