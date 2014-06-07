using System;
using System.Diagnostics;
using UnityEngine;

[ExecuteInEditMode]
public class TestTransformFind : MonoBehaviour
{
    public string find;
    private Stopwatch findSW;
    public float findTime;
    public Transform foundFind;
    public Transform foundIter;
    private Stopwatch iterSW;
    public float iterTime;

    private void Update()
    {
        if (string.IsNullOrEmpty(this.find))
        {
            this.foundFind = null;
            this.foundIter = null;
        }
        else
        {
            if (this.findSW == null)
            {
                this.findSW = new Stopwatch();
            }
            this.findSW.Reset();
            this.findSW.Start();
            this.foundFind = base.transform.Find(this.find);
            this.findSW.Stop();
            this.findTime = (float) this.findSW.Elapsed.TotalMilliseconds;
            if (this.iterSW == null)
            {
                this.iterSW = new Stopwatch();
            }
            this.iterSW.Reset();
            this.iterSW.Start();
            this.foundIter = FindChildHelper.FindChildByName(this.find, this);
            this.iterSW.Stop();
            this.iterTime = (float) this.iterSW.Elapsed.TotalMilliseconds;
        }
    }
}

