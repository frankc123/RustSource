using System;
using UnityEngine;

[ExecuteInEditMode]
public class CompassRenderProxy : MonoBehaviour
{
    public float back = 0.3f;
    public bool bindForward;
    public bool bindNorth;
    public bool bindWest;
    public Vector3 forward = Vector3.forward;
    public Vector3 north = Vector3.up;
    private MaterialPropertyBlock propBlock;
    public float scalar = 0.7f;

    private void BindFrame()
    {
        if (this.propBlock != null)
        {
            this.propBlock.Clear();
        }
        else
        {
            this.propBlock = new MaterialPropertyBlock();
        }
        Vector2 vector = base.transform.worldToLocalMatrix.MultiplyVector(this.north);
        vector.Normalize();
        Vector2 vector2 = new Vector2(-vector.y, vector.x);
        vector2 = (Vector2) (vector2 * this.scalar);
        vector = (Vector2) (vector * this.scalar);
        if (this.bindNorth)
        {
            this.propBlock.AddVector(g.kPropLensUp, vector);
        }
        if (this.bindWest)
        {
            this.propBlock.AddVector(g.kPropLensRight, vector2);
        }
        if (this.bindForward)
        {
            this.propBlock.AddVector(g.kPropLensDir, this.forward);
        }
        base.renderer.SetPropertyBlock(this.propBlock);
    }

    private void LateUpdate()
    {
        this.BindFrame();
    }

    private void OnBecameInvisible()
    {
        base.enabled = false;
    }

    private void OnBecameVisible()
    {
        base.enabled = true;
        this.BindFrame();
    }

    private static class g
    {
        public static readonly int kPropLensDir = Shader.PropertyToID("_LensForward");
        public static readonly int kPropLensRight = Shader.PropertyToID("_LensRight");
        public static readonly int kPropLensUp = Shader.PropertyToID("_LensUp");
    }
}

