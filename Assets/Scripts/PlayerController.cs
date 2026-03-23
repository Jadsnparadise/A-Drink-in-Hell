using System;
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

    //state flags
    public bool isTalking = false;
    private bool isGrounded = false;
    private bool isRunning = false;

    //References
    Rigidbody2D rb;
    Animator anim;
    PlayerAttack _attack;

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
        _attack = GetComponent<PlayerAttack>();
    }

    void Update()
    {
        if (isTalking)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);

            anim.SetBool("Walking", false);
            anim.SetBool("Running", false);

            if (_attack != null) _attack.enabled = false;

            return;
        }
        else{
            _attack.enabled = true;
        }

        if (_attack != null && _attack.IsAttacking())
        {
            rb.velocity = Vector2.zero;
            return;
        }
        
        GetInputs();
        Move();
        Jump();
        Flip();
        
        if (Input.GetKeyDown(KeyCode.H))
            GameManager.Instance.DamagePlayer(1);
    }

    void GetInputs()
    {
        xAxis = 0;
        if (Input.GetKey(KeyCode.A) ||  Input.GetKey(KeyCode.LeftArrow))
            xAxis -= 1;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            xAxis += 1;
        isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }

    void Flip()
    {
        if (xAxis > 0)
        {
            transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
        else if (xAxis < 0)
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
        var speed = isRunning ? runSpeed : walkSpeed;
        
        rb.velocity = new Vector2(speed * xAxis, rb.velocity.y);

        var moving = rb.velocity.x != 0 && isGrounded;
        anim.SetBool("Walking", moving);
        anim.SetBool("Running", moving && isRunning);
    }
}