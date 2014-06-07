using System;
using UnityEngine;

public class CreateCraterOnImpact : MonoBehaviour
{
    public float Depth = 10f;
    public GameObject Explosion;
    public float Noise = 0.5f;
    public float Radius = 15f;

    private void OnCollisionEnter(Collision collision)
    {
        if (this.Explosion != null)
        {
            Object.Instantiate(this.Explosion, collision.contacts[0].point, Quaternion.identity);
        }
        CraterMaker component = collision.gameObject.GetComponent<CraterMaker>();
        if (component != null)
        {
            component.Create(collision.contacts[0].point, this.Radius, this.Depth, this.Noise);
        }
        Object.Destroy(base.gameObject);
    }
}

