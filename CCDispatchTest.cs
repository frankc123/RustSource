using System;
using UnityEngine;

public class CCDispatchTest : MonoBehaviour
{
    public float crouchHeight = 1.3f;
    public float crouchMaxSpeed = 5f;
    public float crouchSmoothing = 0.02f;
    [NonSerialized]
    private float crouchVelocity;
    [NonSerialized]
    private float currentHeight;
    public float downwardSpeed = 10f;
    [SerializeField]
    private Camera fpsCam;
    public float horizonalScalar = 4f;
    public float standingHeight = 2f;
    [SerializeField]
    private CCTotemPole totemPole;

    private void Awake()
    {
        this.currentHeight = this.crouchHeight;
        this.totemPole.OnBindPosition += new CCTotem.PositionBinder(this.BindPositions);
    }

    private void BindPositions(ref CCTotem.PositionPlacement placement, object Tag)
    {
        base.transform.position = placement.bottom;
        this.fpsCam.transform.position = placement.top - new Vector3(0f, 0.25f, 0f);
    }

    private void OnDestroy()
    {
        if (this.totemPole != null)
        {
            this.totemPole.OnBindPosition -= new CCTotem.PositionBinder(this.BindPositions);
        }
    }

    private void Update()
    {
        float num2;
        float deltaTime = Time.deltaTime;
        Vector3 motion = new Vector3(Input.GetAxis("Horizontal") * this.horizonalScalar, -this.downwardSpeed, Input.GetAxis("Vertical") * this.horizonalScalar);
        float f = (motion.x * motion.x) + (motion.z * motion.z);
        if (f > (this.horizonalScalar * this.horizonalScalar))
        {
            num2 = (this.horizonalScalar / Mathf.Sqrt(f)) * deltaTime;
        }
        else
        {
            num2 = deltaTime;
        }
        motion.x *= num2;
        motion.z *= num2;
        motion.y *= deltaTime;
        float target = !Input.GetButton("Crouch") ? this.standingHeight : this.crouchHeight;
        this.currentHeight = Mathf.SmoothDamp(this.currentHeight, target, ref this.crouchVelocity, this.crouchSmoothing, this.crouchMaxSpeed, deltaTime);
        this.totemPole.Move(motion, this.currentHeight);
    }
}

