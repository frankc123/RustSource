using System;
using System.Collections.Generic;
using UnityEngine;

public class RadiationZone : MonoBehaviour
{
    public float exposurePerMin = 50f;
    [NonSerialized]
    private HashSet<Radiation> radiating;
    public float radius = 10f;
    [NonSerialized]
    private bool shuttingDown;
    public bool strongerAtCenter = true;

    internal bool CanAddToRadiation(Radiation radiation)
    {
        if (!this.shuttingDown)
        {
        }
        return ((this.radiating == null) && (this.radiating = new HashSet<Radiation>()).Add(radiation));
    }

    public float GetExposureForPos(Vector3 pos)
    {
        if (this.strongerAtCenter)
        {
            return (this.exposurePerMin * (1f - Mathf.Clamp01(Vector3.Distance(pos, base.transform.position) / this.radius)));
        }
        return this.exposurePerMin;
    }

    public Character GetFromCollider(Collider other)
    {
        IDBase base2 = IDBase.Get(other);
        if (base2 == null)
        {
            return null;
        }
        return (base2.idMain as Character);
    }

    private void OnDestroy()
    {
        this.shuttingDown = true;
        if (this.radiating != null)
        {
            foreach (Radiation radiation in this.radiating)
            {
                if (radiation != null)
                {
                    radiation.RemoveRadiationZone(this);
                }
            }
            this.radiating = null;
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.3f, 0.5f, 0.3f, 0.25f);
        Gizmos.DrawSphere(base.transform.position, this.radius);
        Gizmos.color = Color.green;
        Gizmos.DrawCube(base.transform.position, (Vector3) (Vector3.one * 0.5f));
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.3f, 0.5f, 0.3f, 0.4f);
        Gizmos.DrawWireSphere(base.transform.position, this.radius);
        Gizmos.color = Color.green;
        Gizmos.DrawCube(base.transform.position, (Vector3) (Vector3.one * 0.5f));
    }

    private void OnTriggerEnter(Collider other)
    {
        Character fromCollider = this.GetFromCollider(other);
        if (fromCollider != null)
        {
            Radiation local = fromCollider.GetLocal<Radiation>();
            if (local != null)
            {
                local.AddRadiationZone(this);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Character fromCollider = this.GetFromCollider(other);
        if (fromCollider != null)
        {
            Radiation local = fromCollider.GetLocal<Radiation>();
            if (local != null)
            {
                local.RemoveRadiationZone(this);
            }
        }
    }

    internal bool RemoveFromRadiation(Radiation radiation)
    {
        return (this.shuttingDown || ((this.radiating != null) && this.radiating.Remove(radiation)));
    }

    private void Start()
    {
        this.UpdateCollider();
    }

    [ContextMenu("Update Collider")]
    public void UpdateCollider()
    {
        base.GetComponent<SphereCollider>().radius = this.radius;
        base.collider.isTrigger = true;
    }
}

