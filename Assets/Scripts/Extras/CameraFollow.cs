using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance;
    private Transform player;
    private Rigidbody2D playerRb;

    [Header("Follow Settings")]
    [SerializeField] private float smoothTimeX = 0.1f;
    [SerializeField] private float smoothTimeY = 0.15f;

    [Header("Look Ahead")]
    [SerializeField] private float baseLookAheadMultiplier = 1.5f;
    [SerializeField] private float lookAheadSmooth = 1;

    [Header("Fall Behavior")]
    [SerializeField] private float fallThreshold = 0.6f;

    private float currentLookAhead;
    private float targetLookAhead;

    private float velocityX;
    private float velocityY;
    private float lookAheadVelocity;

    private bool isCameraLocked;
    
    void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        player = PlayerController.Instance.transform;
        playerRb = PlayerController.Instance.GetComponent<Rigidbody2D>();
    }

    public void LockCamera()
    {
        isCameraLocked = true;
    }

    public void UnlockCamera()
    {
        isCameraLocked = false;
    }

    void LateUpdate()
    {
        if (player == null || isCameraLocked) return;

        Vector2 playerVelocity = playerRb.velocity;

        bool isMovingHorizontally = Mathf.Abs(playerVelocity.x) > 0.1f;
        bool isFalling = playerVelocity.y < -0.1f;

        float playerScale = Mathf.Abs(player.localScale.x);

        float dynamicLookAhead = playerScale * baseLookAheadMultiplier;

        if (isMovingHorizontally)
        {
            targetLookAhead = Mathf.Sign(playerVelocity.x) * dynamicLookAhead;
        } else
        {
            targetLookAhead = 0f;
        }

        currentLookAhead = Mathf.SmoothDamp(
            currentLookAhead,
            targetLookAhead,
            ref lookAheadVelocity,
            lookAheadSmooth
        );

        float targetX = player.position.x + currentLookAhead;

        float newX = Mathf.SmoothDamp(
            transform.position.x,
            targetX,
            ref velocityX,
            smoothTimeX
        );

        // COMPORTAMENTO Y (TRAVA NA QUEDA)
        float newY;
        float playerViewportY = Camera.main.WorldToViewportPoint(player.position).y;

        if (isFalling && playerViewportY < fallThreshold)
        {
            newY = player.position.y;
        }
        else
        {
            newY = Mathf.SmoothDamp(
                transform.position.y,
                player.position.y,
                ref velocityY,
                smoothTimeY
            );
        }

        transform.position = new Vector3(newX, newY, transform.position.z);
    }
}