using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class LaneRunnerRB : MonoBehaviour
{
    [Header("Lane Settings")]
    public float laneDistance = 2f;      // Відстань між смугами
    public float laneSwitchSpeed = 10f;  // Плавність переміщення

    private int currentLane = 1;         // 0 = ліво, 1 = центр, 2 = право
    private Vector3 targetPos;

    [Header("Jump Settings")]
    public float jumpForce = 8f;         // сила підйому
    public float hoverTime = 0.18f;      // скільки висить у повітрі
    public float fallImpulse = 18f;      // імпульс вниз після hover

    private bool isJumping = false;
    private bool isHovering = false;
    private float hoverTimer = 0f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // блокуємо повороти, щоб не падала модель
        rb.freezeRotation = true;

        // старт — середня смуга
        UpdateLaneTarget();
    }

    void Update()
    {
        HandleInput();
        HandleLaneMovement();
        HandleHoverAndFall();
    }

    // -----------------------
    //        INPUT
    // -----------------------
    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            MoveLeft();

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            MoveRight();

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W))
            Jump();
    }

    // -----------------------
    //     LANES SYSTEM
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
        float x = (currentLane - 1) * laneDistance;
        targetPos = new Vector3(x, transform.position.y, transform.position.z);
    }

    void HandleLaneMovement()
    {
        // рухаємо лише по X (rigidbody не чіпаємо)
        Vector3 pos = transform.position;
        pos.x = Mathf.Lerp(pos.x, targetPos.x, Time.deltaTime * laneSwitchSpeed);
        transform.position = pos;
    }

    // -----------------------
    //          JUMP
    // -----------------------
    void Jump()
    {
        if (isJumping) return;

        isJumping = true;

        // очищаємо вертикальний рух (важливо!)
        Vector3 v = rb.velocity;
        v.y = 0;
        rb.velocity = v;

        // підйом
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        // стартуємо зависання
        hoverTimer = hoverTime;
        isHovering = true;
    }

    void HandleHoverAndFall()
    {
        if (!isJumping) return;

        // коли зависаємо — "заморозити" падіння
        if (isHovering)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            hoverTimer -= Time.deltaTime;

            if (hoverTimer <= 0)
            {
                isHovering = false;

                // тут даємо різкий імпульс вниз
                rb.AddForce(Vector3.down * fallImpulse, ForceMode.Impulse);
            }
        }

        // якщо приземлився
        if (transform.position.y <= 0.05f)
        {
            Vector3 p = transform.position;
            p.y = 0;
            transform.position = p;

            isJumping = false;
            isHovering = false;
        }
    }
}
