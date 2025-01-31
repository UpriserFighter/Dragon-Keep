using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    private float xAxis, yAxis;
    public Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sr;
    private float gravity;
    public BoxCollider2D bc;

    [Header("Horizontal Movement")]
    [SerializeField] private float walkSpeed = 1;
    [Space(5)]

    [Header("Vertical Movement")]
    [SerializeField] private float jumpForce = 35;
    private float jumpBufferCounter;
    [SerializeField] private float jumpBufferFrames;
    private float coyoteTimeCounter = 0;
    [SerializeField] private float coyoteTime;
    [Space(5)]

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckY = 0.2f;
    [SerializeField] private float groundCheckX = 0.5f;
    [SerializeField] private LayerMask whatIsGround;
    [Space(5)]

    [Header("Player States")]
    [SerializeField] private bool jumping, lookingRight, invincible, climbing;
    [SerializeField] private bool canFlash;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        gravity = rb.gravityScale;
        sr = GetComponent<SpriteRenderer>();
        bc = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {

        GetInputs();
        UpdateJumpVariables();
        Move();
        Jump();
        Flip();

    }

    void GetInputs()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        yAxis = Input.GetAxisRaw("Vertical");
    }

    void Flip()
    {
        if (xAxis < 0)
        {
            transform.localScale = new Vector2(transform.localScale.y * -1, transform.localScale.y);
            lookingRight = false;
        }
        else if (xAxis > 0)
        {
            transform.localScale = new Vector2(transform.localScale.y * 1, transform.localScale.y);
            lookingRight = true;
        }
    }

    private void Move()
    {
        if (climbing)
        {
            rb.velocity = new Vector2(walkSpeed * xAxis, walkSpeed * yAxis);
        }
        else
        {
            rb.velocity = new Vector2(walkSpeed * xAxis, rb.velocity.y);
            anim.SetBool("Walking", rb.velocity.x != 0 && Grounded());
        }
    }

    protected void OnTriggerStay2D(Collider2D _other)
    {
        if (_other.gameObject.CompareTag("Ladder"))
        {
            climbing = true;
            bc.isTrigger = true;
        }
    }

    protected void OnTriggerExit2D(Collider2D _other)
    {
        if (_other.gameObject.CompareTag("Ladder"))
        {
            climbing = false;
            bc.isTrigger = false;
        }
    }

        public bool Grounded()
    {
        if (Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void Jump()
    {

        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0 && !jumping)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce);

            jumping = true;
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 3)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);

            jumping = false;
        }

        anim.SetBool("Jumping", !Grounded());
    }

    void UpdateJumpVariables()
    {
        if (Grounded())
        {
            jumping = false;
            coyoteTimeCounter = coyoteTime;
        }

        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferFrames;
        }

        else
        {
            jumpBufferCounter = jumpBufferCounter - Time.deltaTime * 10;
        }
    }
}
