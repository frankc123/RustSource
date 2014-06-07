using System;
using UnityEngine;

public class IgnoreColliders : MonoBehaviour
{
    public Collider[] a;
    public Collider[] b;

    private void Awake()
    {
        if ((this.a != null) && (this.b != null))
        {
            int num = Mathf.Min(this.a.Length, this.b.Length);
            for (int i = 0; i < num; i++)
            {
                if (((this.a[i] != null) && (this.b[i] != null)) && (this.b[i] != this.a[i]))
                {
                    Physics.IgnoreCollision(this.a[i], this.b[i]);
                }
            }
            this.a = null;
            this.b = null;
        }
    }
}

