using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class LaneRunnerRB : MonoBehaviour
{
    [Header("Lane Settings")]
    public float laneDistance = 2f;
    public float laneSwitchSpeed = 12f;

    private int currentLane = 1; // 0 = left, 1 = center, 2 = right
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

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        UpdateLaneTarget();
    }

    // -----------------------
    // INPUT
    // -----------------------
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            MoveLeft();

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            MoveRight();

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W))
            Jump();
    }

    // -----------------------
    // PHYSICS
    // -----------------------
    void FixedUpdate()
    {
        CheckGround();
        HandleLaneMovement();
        HandleHoverAndFall();
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
        if (!isGrounded) return;

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
    // DEBUG (OPTIONAL)
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
