using System;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class LaneRunnerHybrid : MonoBehaviour
{
    [Header("Animator")]
    public Animator animator;

    [Header("Lane Settings")]
    public float laneDistance = 2f;
    public float laneSwitchSpeed = 20f;

    private int currentLane = 1;
    private float targetX;

    [Header("Jump Settings")]
    public float jumpForce = 12f;
    public float hoverTime = 0.18f;
    public float extraGravity = 30f;
    public float fallImpulse = 18f;
    private bool isJumping;
    private bool isHovering;
    private float hoverTimer;

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.15f;
    private bool isGrounded;

    [Header("Roll / Slide")]
    public float rollDuration = 0.6f;
    public float rollColliderHeight = 0.7f;
    private float normalColliderHeight;
    private bool isRolling;
    private float rollTimer;

    [Header("Jetpack Settings")]
    public float jetpackHeight = 7f;
    public float jetpackRiseSpeed = 5f;

    [Header("Controls")]
    public float swipeThreshold = 20f;

    private Rigidbody rb;
    private CapsuleCollider col;
    private Vector2 startTouchPos;
    private bool isSwiping;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.useGravity = true;

        normalColliderHeight = col.height;
        UpdateLaneTarget();
    }

    void Update()
    {
        HandleInput();
        CheckGround();

        // Extra gravity for faster fall
        if (!isHovering && !IsJetpacking() && rb.velocity.y < -0.1f)
        {
            rb.AddForce(Vector3.down * extraGravity * Time.deltaTime, ForceMode.VelocityChange);
        }
    }

    void FixedUpdate()
    {
        HandleLaneMovement();
        HandleHoverAndFall();
        HandleRoll();
        HandleJetpack();
    }

    // -----------------------
    // INPUT
    // -----------------------
    void HandleInput()
    {
        // Keyboard
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) ChangeLane(-1);
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) ChangeLane(1);
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) Jump();
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) StartRoll();

        // Mouse Swipe
        if (Input.GetMouseButtonDown(0)) { startTouchPos = Input.mousePosition; isSwiping = true; }
        else if (Input.GetMouseButton(0) && isSwiping) CheckSwipe((Vector2)Input.mousePosition - startTouchPos);
        else if (Input.GetMouseButtonUp(0)) isSwiping = false;

        // Touch
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began) { startTouchPos = touch.position; isSwiping = true; }
            else if (touch.phase == TouchPhase.Moved && isSwiping) CheckSwipe(touch.position - startTouchPos);
            else if (touch.phase == TouchPhase.Ended) isSwiping = false;
        }
    }

    void CheckSwipe(Vector2 diff)
    {
        if (diff.magnitude < swipeThreshold) return;
        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
            ChangeLane(diff.x > 0 ? 1 : -1);
        else
            if (diff.y > 0) Jump();
            else StartRoll();
        isSwiping = false;
    }

    // -----------------------
    // LANES
    // -----------------------
    void ChangeLane(int direction)
    {
        int targetLane = currentLane + direction;
        if (targetLane >= 0 && targetLane <= 2)
        {
            currentLane = targetLane;
            UpdateLaneTarget();
        }
    }

    void UpdateLaneTarget() => targetX = (currentLane - 1) * laneDistance;

    void HandleLaneMovement()
    {
        Vector3 pos = rb.position;
        float newX = Mathf.Lerp(pos.x, targetX, Time.fixedDeltaTime * laneSwitchSpeed);
        rb.MovePosition(new Vector3(newX, pos.y, pos.z));
    }

    // -----------------------
    // JUMP / HOVER / FALL
    // -----------------------
    void Jump()
    {
        if (!isGrounded || isRolling) return;
        animator?.SetTrigger("Jump");

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
    // ROLL / SLIDE
    // -----------------------
    void StartRoll()
    {
        if (!isGrounded || isRolling) return;
        animator?.SetTrigger("Roll");
        isRolling = true;
        rollTimer = rollDuration;

        col.height = rollColliderHeight;
        col.center = new Vector3(col.center.x, rollColliderHeight / 2f, col.center.z);
    }

    void HandleRoll()
    {
        if (!isRolling) return;
        rollTimer -= Time.fixedDeltaTime;
        if (rollTimer <= 0f)
        {
            isRolling = false;
            col.height = normalColliderHeight;
            col.center = new Vector3(col.center.x, normalColliderHeight / 2f, col.center.z);
        }
    }

    // -----------------------
    // GROUND CHECK
    // -----------------------
    void CheckGround()
    {
        isGrounded = Physics.Raycast(rb.position + Vector3.up * 0.1f, Vector3.down, groundCheckDistance, groundLayer);
    }

    // -----------------------
    // JETPACK
    // -----------------------
    void HandleJetpack()
    {
        if (!IsJetpacking()) return;
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        Vector3 pos = rb.position;
        float newY = Mathf.Lerp(pos.y, jetpackHeight, Time.fixedDeltaTime * jetpackRiseSpeed);
        rb.MovePosition(new Vector3(pos.x, newY, pos.z));
        isJumping = false;
        isHovering = false;
    }

    bool IsJetpacking()
    {
        return PlayerJetpack.instance != null && PlayerJetpack.instance.isFlying;
    }

    // -----------------------
    // DEBUG / GIZMOS
    // -----------------------
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position + Vector3.up * 0.1f, transform.position + Vector3.down * groundCheckDistance);
    }
}