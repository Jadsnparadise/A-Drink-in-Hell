using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    private static readonly int Jumping = Animator.StringToHash("Jumping");
    private static readonly int Walking = Animator.StringToHash("Walking");
    private static readonly int Running = Animator.StringToHash("Running");
    private static readonly int JumpingFall = Animator.StringToHash("JumpingFall");

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 1f;
    [SerializeField] private float runSpeed = 2f;
    [SerializeField] private float jumpForce = 1;

    [Header("Looking Settings")] 
    [SerializeField] private float lookDelay = 0.3f;

    //state flags
    public bool itsMovementIsBlocked = false;
    [SerializeField] private bool isGrounded = false;
    private bool isRunning = false;
    private bool isKnockBacking = false;
    private float blinkDamageDuration = 1f;
    private float xAxis;
    private float _lookingFrom;
    private float _lookTimer = 0;
    
    //References
    private Rigidbody2D rb;
    private Collider2D collider;
    private Animator anim;
    private PlayerAttack attack;
    public MoveableObstacle currentPlatform;
    private SpriteRenderer playerSprite;
    
    // Input System
    private InputSystemActions _input;

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

        ConfigureInput();
    }

    private void ConfigureInput()
    {
        _input = new InputSystemActions();
        _input.Player.Enable();
        
        _input.Player.Jump.started += OnJumpStarted;
        _input.Player.Jump.canceled += OnJumpCanceled;
    }

    private void OnJumpStarted(InputAction.CallbackContext ctx)
    {
        if (!isGrounded) return;

        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        isGrounded = false;
    }
    
    private void OnJumpCanceled(InputAction.CallbackContext ctx)
    {
        if (rb.velocity.y > 0)
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
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

        GetMovement();
        UpdateAnimator();
        Flip();
        Move();
        Look();
    }
    
    private void GetMovement()
    {
        xAxis = _input.Player.Move.ReadValue<float>();
        isRunning = _input.Player.Run.IsPressed();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.contacts.Any(contactPoint => contactPoint.normal.y > 0.5f)) return;
        isGrounded = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.contacts.Any(contactPoint => contactPoint.normal.y > 0.5f))
        {
            isGrounded = false;
            return;
        }

        isGrounded = false;
    }

    private void Move()
    {
        var speed = isRunning ? runSpeed : walkSpeed;

        var platformVelocityX = 0f;
        if (currentPlatform)
            platformVelocityX = currentPlatform.PlatformVelocity.x;

        rb.velocity = new Vector2((speed * xAxis) + platformVelocityX, rb.velocity.y);
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

    private void SetTrigger(string trigger)
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

    private void UpdateAnimator()
    {
        var moving = xAxis != 0 && isGrounded;
        anim.SetBool(Walking, moving);
        anim.SetBool(Running, moving && isRunning);
        
        anim.SetBool(Jumping, !isGrounded);
        anim.SetBool(JumpingFall, !isGrounded && rb.velocity.y < -0.1f);
    }

    private void Flip()
    {
        transform.localScale = xAxis switch
        {
            > 0 => new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y),
            < 0 => new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y),
            _ => transform.localScale
        };
    }

    private void Look()
    {
        var moving = xAxis != 0;
        var looking = _input.Player.Look.ReadValue<float>();

        if (!moving && isGrounded && looking != 0)
        {
            _lookTimer += Time.deltaTime;
            if (_lookTimer >= lookDelay)
            {
                _lookingFrom = looking;
            }
        }
        else
        {
            _lookingFrom = 0;
            _lookTimer = 0;
        }
    }

    public void Die()
    {
        SetTrigger("Death");
        SetIgnoreEnemyLayerCollision(true);
        itsMovementIsBlocked = true;
        _input.Player.Disable();
        attack.OnDisable();
    }

    public float LookingFrom() => _lookingFrom;
}