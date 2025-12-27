using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class LaneRunnerRB : MonoBehaviour
{
    [Header("Lane Settings")]
    public float laneDistance = 2f;
    public float laneSwitchSpeed = 12f;

    private int currentLane = 1; 
    private float targetX;

    [Header("Jump Settings")]
    public float jumpForce = 8f;
    public float hoverTime = 0.18f;
    public float fallImpulse = 18f;

    private bool isJumping;
    private bool isHovering;
    private float hoverTimer;

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.15f;
    private bool isGrounded;

    [Header("Slide")]
    public float slideDuration = 0.6f;
    public float slideColliderHeight = 0.7f;
    private float normalColliderHeight;
    private bool isSliding;
    private float slideTimer;
    private CapsuleCollider capsule;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();

        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        normalColliderHeight = capsule.height;

        UpdateLaneTarget();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            MoveLeft();

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            MoveRight();

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W))
            Jump();
        
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            Debug.Log("S");
            Slide();
    }

    void FixedUpdate()
    {
        CheckGround();
        HandleLaneMovement();
        HandleHoverAndFall();
        HandleSlide();
    }

    // -----------------------
    // GROUND CHECK
    // -----------------------
    void CheckGround()
    {
        isGrounded = Physics.Raycast(
            rb.position + Vector3.up * 0.1f,
            Vector3.down,
            groundCheckDistance,
            groundLayer
        );
    }

    // -----------------------
    // LANES
    // -----------------------
    void MoveLeft()
    {
        if (currentLane > 0)
        {
            currentLane--;
            UpdateLaneTarget();
        }
    }

    void MoveRight()
    {
        if (currentLane < 2)
        {
            currentLane++;
            UpdateLaneTarget();
        }
    }

    void UpdateLaneTarget()
    {
        targetX = (currentLane - 1) * laneDistance;
    }

    void HandleLaneMovement()
    {
        Vector3 pos = rb.position;
        float newX = Mathf.Lerp(pos.x, targetX, Time.fixedDeltaTime * laneSwitchSpeed);
        rb.MovePosition(new Vector3(newX, pos.y, pos.z));
    }

    // -----------------------
    // JUMP
    // -----------------------
    void Jump()
    {
        if (!isGrounded || isSliding) return;

        isJumping = true;
        isHovering = true;

        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        hoverTimer = hoverTime;
    }

    void HandleHoverAndFall()
    {
        if (!isJumping) return;

        if (isHovering)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            hoverTimer -= Time.fixedDeltaTime;

            if (hoverTimer <= 0f)
            {
                isHovering = false;
                rb.AddForce(Vector3.down * fallImpulse, ForceMode.Impulse);
            }
        }

        if (isGrounded && rb.velocity.y <= 0f)
        {
            isJumping = false;
            isHovering = false;
        }
    }

    // -----------------------
    // SLIDE 
    // -----------------------
    void Slide()
    {
        if (!isGrounded || isSliding) return;

        isSliding = true;
        slideTimer = slideDuration;

        capsule.height = slideColliderHeight; 
        capsule.center = new Vector3(capsule.center.x, slideColliderHeight / 2f, capsule.center.z);
    }

    void HandleSlide()
    {
        if (!isSliding) return;

        slideTimer -= Time.fixedDeltaTime;

        if (slideTimer <= 0f)
        {
            isSliding = false;
            capsule.height = normalColliderHeight;
            capsule.center = new Vector3(capsule.center.x, normalColliderHeight / 2f, capsule.center.z);
        }
    }

    // -----------------------
    // DEBUG
    // -----------------------
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(
            transform.position + Vector3.up * 0.1f,
            transform.position + Vector3.down * groundCheckDistance
        );
    }
}
