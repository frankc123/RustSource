using System;
using UnityEngine;

public class Tracer : MonoBehaviour
{
    private bool allowBlood;
    public GameObject bloodDecalPrefab;
    public AudioClip[] bodyImpactSounds;
    private Collider colliderToHit;
    public GameObject decalPrefab;
    public float distance;
    public float fadeDistLength = 0.25f;
    public float fadeDistStart = 0.15f;
    public GameObject fleshImpactPrefab;
    public GameObject impactPrefab;
    public AudioClip[] impactSounds;
    public float lastUpdateTime;
    private int layerMask = 0x183e1411;
    private float maxRange = 800f;
    public GameObject myMesh;
    public float speedPerSec;
    public Vector3 startScale;
    public float startTime;
    private bool thereIsABodyPart;
    private bool thereIsACollider;

    private void Awake()
    {
        this.startTime = Time.time;
        float num = Random.Range((float) 0.75f, (float) 1f);
        this.startScale = new Vector3(base.transform.localScale.x * num, base.transform.localScale.y * num, base.transform.localScale.z * Random.Range((float) 0.5f, (float) 1f));
        base.transform.localScale = new Vector3(0f, 0f, this.startScale.z);
    }

    public void Init(Component component, int layerMask, float range, bool allowBlood)
    {
        this.layerMask = layerMask;
        this.colliderToHit = !(component is Collider) ? null : ((Collider) component);
        this.thereIsACollider = (bool) base.collider;
        this.maxRange = range;
        this.allowBlood = allowBlood;
    }

    private void Start()
    {
        this.lastUpdateTime = Time.time;
    }

    private void Update()
    {
        float num = Time.time - this.lastUpdateTime;
        this.lastUpdateTime = Time.time;
        if (this.distance > this.fadeDistStart)
        {
            base.transform.localScale = Vector3.Lerp(base.transform.localScale, this.startScale, Mathf.Clamp((float) ((this.distance - this.fadeDistStart) / this.fadeDistLength), (float) 0f, (float) 1f));
        }
        RaycastHit hitInfo = new RaycastHit();
        RaycastHit2 invalid = RaycastHit2.invalid;
        Ray ray = new Ray(base.transform.position, base.transform.forward);
        float distance = this.speedPerSec * num;
        bool flag = !((this.thereIsACollider && (this.colliderToHit != null)) && this.colliderToHit.enabled);
        if (!flag ? this.colliderToHit.Raycast(ray, out hitInfo, distance) : Physics2.Raycast2(ray, out invalid, this.speedPerSec * num, this.layerMask))
        {
            Vector3 point;
            Vector3 normal;
            GameObject gameObject;
            Rigidbody rigidbody;
            if (Vector3.Distance(Camera.main.transform.position, base.transform.position) > 75f)
            {
                Object.Destroy(base.gameObject);
                return;
            }
            if (flag)
            {
                normal = invalid.normal;
                point = invalid.point;
                gameObject = invalid.gameObject;
                rigidbody = invalid.rigidbody;
            }
            else
            {
                normal = hitInfo.normal;
                point = hitInfo.point;
                gameObject = hitInfo.collider.gameObject;
                rigidbody = hitInfo.rigidbody;
            }
            Quaternion rotation = Quaternion.LookRotation(normal);
            int layer = gameObject.layer;
            GameObject impactPrefab = this.impactPrefab;
            bool flag2 = true;
            if (((rigidbody != null) && !rigidbody.isKinematic) && !rigidbody.CompareTag("Door"))
            {
                rigidbody.AddForceAtPosition((Vector3) (Vector3.up * 200f), point);
                rigidbody.AddForceAtPosition((Vector3) (ray.direction * 1000f), point);
            }
            SurfaceInfo.DoImpact(gameObject, SurfaceInfoObject.ImpactType.Bullet, point + ((Vector3) (normal * 0.01f)), rotation);
            switch (layer)
            {
                case 0x11:
                case 0x12:
                case 0x1b:
                case 0x15:
                    flag2 = false;
                    break;
            }
            Object.Destroy(base.gameObject);
            if (flag2)
            {
                this.impactSounds[Random.Range(0, this.impactSounds.Length)].Play(point, 1f, 2f, 15f, 180);
                GameObject obj4 = Object.Instantiate(this.decalPrefab, point + ((Vector3) (normal * Random.Range((float) 0.01f, (float) 0.03f))), rotation * Quaternion.Euler(0f, 0f, (float) Random.Range(-30, 30))) as GameObject;
                if (gameObject != null)
                {
                    obj4.transform.parent = gameObject.transform;
                }
                Object.Destroy(obj4, 15f);
            }
        }
        else
        {
            Transform transform = base.transform;
            transform.position += (Vector3) ((base.transform.forward * this.speedPerSec) * num);
            this.distance += this.speedPerSec * num;
        }
        if (this.distance > this.maxRange)
        {
            Object.Destroy(base.gameObject);
        }
    }
}

