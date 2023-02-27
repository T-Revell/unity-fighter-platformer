using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPhysics : MonoBehaviour
{

    //Variables

    //Other Scripts

    [SerializeField] Player_Controller playerController;
    [SerializeField] Animator_Handler animatorHandler;

    //General

    public Rigidbody2D rb;

    //======Movement======

    //Horizontal Movement

    [Header("Horizontal Movement")]
    public float walkSpeed;
    public float maxWalkSpeed;
    public float dashSpeed;
    public float maxDashSpeed;
    public float airChangeFraction;

    //Vertical Movement

    [Header("Vertical Movement")]
    public float jumpForce;
    public float heldJumpFraction;
    public bool jumpHeld; //distinct from jumpInput in Player_Controller, as this bool is set to and remains false once it is released during a jump, while jumpInput can be switched at any time

    //Other Movement

    public Vector2 airVelocity;

    //Physics Modification

    public float gravity;
    public float hangGravity;
    public float fallGravity;

    public float drag;

    //======Collision======

    //Platform Checks

    [Header("Platform Checks")]
    public bool groundCheck;
    public Vector2 groundCheckBox;
    public float groundCheckBoxHeight;

    public bool wallCheck;
    public Vector2 wallCheckBox;



    //Start

    private void Start()
    {
        //References

        playerController = gameObject.GetComponent<Player_Controller>();
        animatorHandler = gameObject.GetComponent<Animator_Handler>();
        rb = gameObject.GetComponent<Rigidbody2D>();

        //Horizontal Movement

        walkSpeed = 1.5f;
        maxWalkSpeed = 4f;
        dashSpeed = walkSpeed;
        maxDashSpeed = 11f;
        airChangeFraction = 0.4f;

        //Vertical Movement

        jumpForce = 16.5f;
        heldJumpFraction = 0.075f;
        jumpHeld = false;

        //Physics Modification

        gravity = 7f;
        hangGravity = 2.5f;
        fallGravity = 8f;

        //Platform Checks

        groundCheckBox = new Vector2(0.4f, 0.5f);
        groundCheckBoxHeight = 0.85f;

        wallCheckBox = new Vector2(0.6f, 0.5f);
    }

    //Physics Logic

    private void FixedUpdate()
    {

        //Horizontal Movement
        if (animatorHandler.attacking == false)
        {
            if (groundCheck) //all movement here is grounded only, and can only be done while not attacking
            {
                if (playerController.dashHeld)//---Dashing---
                {
                    if (Mathf.Abs(rb.velocity.x) < maxDashSpeed) //will only add velocity if maximum speed is not exceeded
                    {
                        rb.AddForce(new Vector2(playerController.dInput.x * dashSpeed, 0f), ForceMode2D.Impulse);//If dash is held, accelerates faster
                    }
                }

                else //---Walking---
                {
                    if (Mathf.Abs(rb.velocity.x) < maxWalkSpeed) //If horizontal velocity is less than maxSpeed,
                    {
                        rb.AddForce(new Vector2(playerController.dInput.x * walkSpeed, 0f), ForceMode2D.Impulse); //Takes x value of dinput vector2 to add force to object
                    }
                }

                if (playerController.jumpInput) //---Jump---
                {
                    Jump();
                }
            }

        }

        
        if(groundCheck == false) //---Airborne---
        {
            if (Mathf.Abs(rb.velocity.x) < maxWalkSpeed) //If horizontal velocity is less than maxSpeed,
            {
                rb.AddForce(new Vector2(playerController.dInput.x * walkSpeed * airChangeFraction, 0f), ForceMode2D.Impulse); //less speed is applied when airborne
            }
            airVelocity = rb.velocity;
        }

        //Checks

        if (Physics2D.OverlapBox(new Vector2(transform.position.x, transform.position.y - groundCheckBoxHeight), groundCheckBox, 0f, LayerMask.GetMask("Ground")) != null) //Creates a box below character that checks if it overlaps with ground
        {
            groundCheck = true;
            jumpHeld = false;
        } 
        else
        {
            groundCheck = false;
        }

        if (Physics2D.OverlapBox(transform.position, wallCheckBox, 0f, LayerMask.GetMask("Ground")) != null) //Creates a box wider than character that checks if it overlaps with walls
        {
            wallCheck = true;
        }
        else
        {
            wallCheck = false;
        }

        ModifyPhysics();//updates physics specifics each frame

    }

    void OnCollisionEnter2D(Collision2D collider)
    {
        if (collider.gameObject.tag == "Ground")
        {
            rb.velocity = new Vector2(airVelocity.x, 0f); //Preserves horizontal momentum when landing
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse); //adds jumpforce to rigidbody's y velocity
        jumpHeld = true; //sets jumpheld to true, which can only be done at the start of a grounded jump
    }

    private void ModifyPhysics()
    {
        if(groundCheck == false) // checks if airborne
        {

            rb.gravityScale = gravity;

            if (jumpHeld && playerController.jumpInput)//checks the player has not let go of jump after first pressing it
            {
                rb.AddForce(new Vector2(0f, rb.velocity.y * heldJumpFraction), ForceMode2D.Impulse);
            }
            else
            {
                jumpHeld = false; //if the player has let go of up at any point, jumpheld is set to false for the rest of the jump
            }

            if (rb.velocity.y < 1f)
            {
                rb.gravityScale = fallGravity; //if falling, character is given extra weight to make jumps feel heavier
            }

        }
        else
        {
            rb.gravityScale = gravity; //gravity is normal in most cases
        }
    }

    //Visualization of Colliders

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(new Vector2(transform.position.x, transform.position.y - groundCheckBoxHeight), new Vector3(groundCheckBox.x, groundCheckBox.y, 0)); //ground check box
        Gizmos.color = Color.grey;
        Gizmos.DrawCube(transform.position, new Vector3(wallCheckBox.x, wallCheckBox.y, 0)); //wall check box
    }

}
