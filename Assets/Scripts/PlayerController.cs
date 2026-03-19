using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Horizontal Movement Settings")]
    //Movement
    [SerializeField] private float walkSpeed = 1f;
    [SerializeField] private float runSpeed = 2f;
    private float xAxis;

    [SerializeField] private float jumpForce = 1;
    private bool isGrounded = false;
    private bool isRunning = false;

    //References
    Rigidbody2D rb;
    Animator anim;

    public static PlayerController Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        Move();
        Jump();
        Flip();
        GetInputs();
    }

    void GetInputs()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        isRunning = Input.GetKey(KeyCode.LeftShift) && xAxis != 0;
    }

    void Flip()
    {
        if (xAxis < 0)
        {
            transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
        else if (xAxis > 0)
        {
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        isGrounded = false;

        foreach (ContactPoint2D contactPoint in collision.contacts)
        {
            if (contactPoint.normal.y > 0.5f)
            {
                isGrounded = true;
                break;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }

    private void Jump()
    {
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
        }

        anim.SetBool("Jumping", !isGrounded);
    }

    void Move()
    {

        float speed = isRunning ? runSpeed : walkSpeed;

        rb.velocity = new Vector2(speed * xAxis, rb.velocity.y);

        anim.SetBool("Walking", rb.velocity.x != 0 && isGrounded);
        anim.SetBool("Running", isRunning && isGrounded);
    }
}