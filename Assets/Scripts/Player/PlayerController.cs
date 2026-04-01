using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    private static readonly int Jumping = Animator.StringToHash("Jumping");
    private static readonly int Walking = Animator.StringToHash("Walking");
    private static readonly int Running = Animator.StringToHash("Running");
    private static readonly int JumpingFall = Animator.StringToHash("JumpingFall");
    private static readonly int JumpingFinish = Animator.StringToHash("JumpingFinish");

    [Header("Horizontal Movement Settings")]
    //Movement
    [SerializeField] private float walkSpeed = 1f;
    [SerializeField] private float runSpeed = 2f;
    private float xAxis;

    [SerializeField] private float jumpForce = 1;

    //state flags
    public bool itsMovementIsBlocked = false;
    [SerializeField] private bool isGrounded = false;
    private bool isRunning = false;
    private bool isKnockBacking = false;
    private float blinkDamageDuration = 1f;

    //References
    private Rigidbody2D rb;
    private Collider2D collider;
    private Animator anim;
    private PlayerAttack attack;
    public MoveableObstacle currentPlatform;
    private SpriteRenderer playerSprite;

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
        rb = GetComponentInParent<Rigidbody2D>();
        anim = GetComponentInParent<Animator>();
        attack = GetComponentInParent<PlayerAttack>();
        playerSprite = GetComponentInParent<SpriteRenderer>();
        collider = GetComponentInParent<Collider2D>();
    }

    void Update()
    {
        if (isKnockBacking) return;

        if (itsMovementIsBlocked)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);

            anim.SetBool(Walking, false);
            anim.SetBool(Running, false);
            anim.SetBool(Jumping, false);
            anim.SetBool(JumpingFall, false);

            if (attack) attack.enabled = false;

            return;
        }
        else
        {
            attack.enabled = true;
        }

        if (attack && attack.IsAttacking())
        {
            rb.velocity = Vector2.zero;
            return;
        }

        GetInputs();
        Move();
        Jump();
        Flip();
    }

    void GetInputs()
    {
        xAxis = 0;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
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
        foreach (ContactPoint2D contactPoint in collision.contacts)
        {
            if (contactPoint.normal.y > 0.5f)
            {
                isGrounded = true;
                return;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        foreach (ContactPoint2D contactPoint in collision.contacts)
        {
            if (contactPoint.normal.y > 0.5f)
            {
                isGrounded = false;
                return;
            }
        }

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

        if (anim.GetBool(Jumping) && anim.GetBool(JumpingFall) && isGrounded)
        {
            anim.SetTrigger(JumpingFinish);
            anim.SetBool(Jumping, false);
            anim.SetBool(JumpingFall, false);
        }
        else
        {
            anim.SetBool(Jumping, !isGrounded);
            anim.SetBool(JumpingFall, !isGrounded && rb.velocity.y < -0.1f);
        }
    }

    void Move()
    {
        var speed = isRunning ? runSpeed : walkSpeed;

        float platformVelocityX = 0f;
        if (currentPlatform != null)
        {
            platformVelocityX = currentPlatform.PlatformVelocity.x;
        }

        rb.velocity = new Vector2((speed * xAxis) + platformVelocityX, rb.velocity.y);

        var moving = xAxis != 0 && isGrounded;
        anim.SetBool(Walking, moving);
        anim.SetBool(Running, moving && isRunning);
    }

    public void MultiplySpeed(float multiplier)
    {
        walkSpeed *= multiplier;
        runSpeed *= multiplier;
    }

    public void MultiplyJumpForce(float multiplier)
    {
        jumpForce *= multiplier;
    }

    private Coroutine _currentKnockbackRoutine;

    public void ApplyKnockback(Vector2 direction, float force, float duration)
    {
        if (_currentKnockbackRoutine != null)
        {
            StopCoroutine(_currentKnockbackRoutine);
        }
        _currentKnockbackRoutine = StartCoroutine(KnockbackRoutine(direction, force, duration));
    }

    private IEnumerator KnockbackRoutine(Vector2 direction, float force, float duration)
    {
        isKnockBacking = true;
        rb.velocity = Vector2.zero;
        rb.AddForce(direction * force, ForceMode2D.Impulse);

        yield return new WaitForSeconds(duration);

        isKnockBacking = false;
    }

    public void BlinkDamageFeedback()
    {
        StartCoroutine(nameof(BlinkRoutine));
    }

    private IEnumerator BlinkRoutine()
    {
        float elapsed = 0f;
        float blinkInterval = 0.05f;
        Color originalColor = playerSprite.color;

        while (elapsed < blinkDamageDuration)
        {
            playerSprite.color = Color.red;
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;

            if (elapsed >= blinkDamageDuration) break;

            playerSprite.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f);
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        playerSprite.color = originalColor;
    }

    private void OnFinishJumpAnimationEnd()
    {
        anim.SetBool(Jumping, !isGrounded);
        anim.SetBool(JumpingFall, !isGrounded && rb.velocity.y < -0.1f);
    }

    private void OnDeathAnimationEnd()
    {
        GameManager.Instance.ShowGameOver();
    }

    public void SetTrigger(string trigger)
    {
        anim.SetTrigger(trigger);
    }

    public static void SetIgnoreEnemyLayerCollision(bool ignore)
    {
        Physics2D.IgnoreLayerCollision(
            LayerMask.NameToLayer("Player"),
            LayerMask.NameToLayer("Enemy"),
            ignore
        );
    }
}