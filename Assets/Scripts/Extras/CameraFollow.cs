using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance;
    private PlayerController _playerController;
    private Transform player;
    private Rigidbody2D playerRb;

    [Header("Follow Settings")]
    [SerializeField] private float smoothTimeX = 0.1f;
    [SerializeField] private float smoothTimeY = 0.15f;

    [Header("Look Ahead")]
    [SerializeField] private float baseLookAheadMultiplier = 1.5f;
    [SerializeField] private float lookAheadSmooth = 1;
    
    [Header("Looking Settings")]
    [SerializeField] private float lookingDistance = 1.2f;

    [SerializeField] private float smoothTimeYLooking = 0.3f;

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
        _playerController = PlayerController.Instance;
        player = _playerController.transform;
        playerRb = _playerController.GetComponent<Rigidbody2D>();
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
        if (!player || isCameraLocked) return;

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
        
        float newY;
        float playerViewportY = Camera.main.WorldToViewportPoint(player.position).y;
        var lookModifier = _playerController.LookingFrom() * 0.5f;

        if (isFalling && playerViewportY < fallThreshold)
        {
            newY = player.position.y;
        }
        else
        {
            var time = lookModifier != 0 ? smoothTimeYLooking : smoothTimeY;
            newY = Mathf.SmoothDamp(
                transform.position.y,
                player.position.y + lookModifier,
                ref velocityY,
                time
            );
        }

        transform.position = new Vector3(newX, newY, transform.position.z);
    }
}