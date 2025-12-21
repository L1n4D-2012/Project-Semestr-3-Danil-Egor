using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class LaneRunnerRB : MonoBehaviour
{
    [Header("Lane Settings")]
    public float laneDistance = 2f;
    public float laneSwitchSpeed = 12f;

    private int currentLane = 1;
    private float targetX;

    [Header("Jump Settings")]
    public float jumpForce = 7f;
    public float extraGravity = 25f;

    [Header("Jetpack Settings")]
    public float jetpackHeight = 7f;
    public float jetpackRiseSpeed = 5f;

    private bool isJumping;
    private Rigidbody rb;
    private float distToGround;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.useGravity = true;

        if (GetComponent<Collider>() != null)
        {
            distToGround = GetComponent<Collider>().bounds.extents.y;
        }
        else
        {
            distToGround = 1f;
        }

        UpdateLaneTarget();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            MoveLeft();

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            MoveRight();

        bool isFlying = PlayerJetpack.instance != null && PlayerJetpack.instance.isFlying;

        if (!isFlying)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                Jump();
        }
    }

    void FixedUpdate()
    {
        HandleLaneMovement();

        if (PlayerJetpack.instance != null && PlayerJetpack.instance.isFlying)
        {
            HandleJetpackMovement();
        }
        else
        {
            HandleGravityAndLanding();
        }
    }

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

    void HandleJetpackMovement()
    {
        isJumping = true;

        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        Vector3 pos = rb.position;
        float newY = Mathf.Lerp(pos.y, jetpackHeight, Time.fixedDeltaTime * jetpackRiseSpeed);

        rb.MovePosition(new Vector3(pos.x, newY, pos.z));
    }

    void Jump()
    {
        if (isJumping) return;

        if (IsGrounded())
        {
            isJumping = true;
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void HandleGravityAndLanding()
    {
        if (isJumping && rb.velocity.y < 0)
        {
            rb.AddForce(Vector3.down * extraGravity, ForceMode.Acceleration);
        }

        if (isJumping && rb.velocity.y <= 0.1f)
        {
            if (IsGrounded())
            {
                isJumping = false;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            }
        }
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, distToGround + 0.2f);
    }
}