using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement_Mecanim : MonoBehaviour
{
    private bool bCrouching;
    private bool bIsInAir;
    private bool bSprinting;
    private float[] flAttackTimers = new float[] { 0f, 1f, 0.1f, 0.1f, 1f, 1f };
    private float flCanAttackAgainTime = -1f;
    public float flCrouchWalkSpeed = 1.54f;
    private float flPlayerAimPitch;
    public float flRotateSpeed = 9f;
    public float flSprintSpeed = 6.2f;
    private float flUpperBodyAimLayerWeight = 1f;
    public float flWalkSpeed = 2.55f;
    public int iUpperBodyAimState;
    private Animator playerAnimController;
    private CharacterController playerController;

    private void CheckLanding()
    {
        if (this.playerController.isGrounded)
        {
            this.bIsInAir = false;
            this.playerAnimController.SetTrigger("Land");
        }
    }

    private void SetUpperBodyAimState()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            this.iUpperBodyAimState = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            this.iUpperBodyAimState = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            this.iUpperBodyAimState = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            this.iUpperBodyAimState = 4;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            this.iUpperBodyAimState = 5;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            this.iUpperBodyAimState = 0;
        }
        this.playerAnimController.SetInteger("UpperBodyAimState", this.iUpperBodyAimState);
    }

    private void Start()
    {
        this.playerController = base.GetComponent<CharacterController>();
        this.playerAnimController = base.GetComponent<Animator>();
    }

    private void Update()
    {
        this.SetUpperBodyAimState();
        if (Input.GetKey(KeyCode.C))
        {
            this.bCrouching = true;
        }
        else
        {
            this.bCrouching = false;
        }
        this.playerAnimController.SetBool("Crouching", this.bCrouching);
        if ((Input.GetKey(KeyCode.LeftShift) && (Mathf.Abs(Input.GetAxis("Horizontal")) < 0.1f)) && (!this.bCrouching && (Input.GetAxis("Vertical") > 0.1f)))
        {
            this.bSprinting = true;
        }
        else
        {
            this.bSprinting = false;
        }
        if ((Input.GetKey(KeyCode.Space) && this.playerController.isGrounded) && !this.bCrouching)
        {
            this.bIsInAir = true;
            this.playerAnimController.SetTrigger("Jump");
            this.playerController.Move((Vector3) (Vector3.up * 1.5f));
        }
        if (this.bIsInAir)
        {
            this.CheckLanding();
        }
        if (Input.GetKeyDown(KeyCode.Mouse0) && (this.flCanAttackAgainTime <= Time.time))
        {
            this.flCanAttackAgainTime = Time.time + this.flAttackTimers[this.iUpperBodyAimState];
            this.playerAnimController.SetTrigger("Attack");
        }
        this.playerAnimController.SetFloat("Move_ForwardBack", Input.GetAxis("Vertical"));
        this.playerAnimController.SetFloat("Move_Strafe", Input.GetAxis("Horizontal"));
        this.flPlayerAimPitch += Input.GetAxis("Mouse Y") * this.flRotateSpeed;
        this.flPlayerAimPitch = Mathf.Clamp(this.flPlayerAimPitch, -55f, 55f);
        this.playerAnimController.SetFloat("Aim_Vertical", this.flPlayerAimPitch);
        if (this.bSprinting)
        {
            this.flUpperBodyAimLayerWeight = Mathf.Lerp(this.flUpperBodyAimLayerWeight, 0f, Time.deltaTime * 6f);
            this.playerAnimController.SetBool("Sprinting", true);
        }
        else
        {
            if (this.iUpperBodyAimState == 0)
            {
                this.flUpperBodyAimLayerWeight = Mathf.Lerp(this.flUpperBodyAimLayerWeight, 0f, Time.deltaTime * 6f);
            }
            else
            {
                this.flUpperBodyAimLayerWeight = Mathf.Lerp(this.flUpperBodyAimLayerWeight, 1f, Time.deltaTime * 6f);
            }
            this.playerAnimController.SetBool("Sprinting", false);
        }
        this.playerAnimController.SetLayerWeight(1, this.flUpperBodyAimLayerWeight);
        Vector3 vector = (Vector3) (base.transform.TransformDirection(Vector3.forward) * Input.GetAxis("Vertical"));
        Vector3 vector2 = (Vector3) (base.transform.TransformDirection(Vector3.right) * Input.GetAxis("Horizontal"));
        if (this.bSprinting)
        {
            this.playerController.SimpleMove((Vector3) (vector * this.flSprintSpeed));
        }
        else if (this.bCrouching)
        {
            this.playerController.SimpleMove((Vector3) ((vector * this.flCrouchWalkSpeed) + ((vector2 * this.flCrouchWalkSpeed) * 0.85f)));
        }
        else
        {
            this.playerController.SimpleMove((Vector3) ((vector * this.flWalkSpeed) + ((vector2 * this.flWalkSpeed) * 0.75f)));
        }
    }
}

