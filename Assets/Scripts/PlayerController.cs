using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Horizontal Movement Settings")]
    //Movement
    [SerializeField] private float walkSpeed = 1;
    private float xAxis;

    [SerializeField] private float jumpForce = 1;
    private bool isGrounded = true;

    //References
    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Move();
        Jump();
        GetInputs();
    }

    void GetInputs()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        isGrounded = true;
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
    }

    void Move()
    {
        rb.velocity = new Vector2(walkSpeed * xAxis, rb.velocity.y);
    }

    //foreach (ContactPoint2D contactPoint in collision.contacts)
    //{
    //    if (contactPoint.normal.y > 0.5f)
    //    {
    //        isGrounded = true;
    //        break;
    //    }
    //}
}