using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class LaneRunnerRB : MonoBehaviour
{
    [Header("Animation")]
    public Animator animator; // Сюда перетяни компонент Animator

    [Header("Lane Settings")]
    public float laneDistance = 2f;
    public float laneSwitchSpeed = 12f;

    private int currentLane = 1; // 0 = left, 1 = center, 2 = right
    private float targetX;

    [Header("Jump Settings")]
    public float jumpForce = 8f;
    public float hoverTime = 0.18f;
    public float fallImpulse = 18f;
    public float groundY = 0f;

    private bool isJumping;
    private bool isHovering;
    private float hoverTimer;

    // Для состояния "Раненый"
    public bool isDamaged = false;
    private bool isDead = false;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        // Если забыл привязать аниматор в инспекторе, пробуем найти его сами
        if (animator == null) animator = GetComponentInChildren<Animator>();

        UpdateLaneTarget();
    }

    void Start()
    {
        // Сразу запускаем бег при старте
        if (animator != null) animator.SetBool("IsRunning", true);
    }

    // -----------------------
    // INPUT
    // -----------------------
    void Update()
    {
        if (isDead) return; // Если мертв, управление отключаем

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            MoveLeft();

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            MoveRight();

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W))
            Jump();

        // ТЕСТ: Нажми H, чтобы персонаж начал хромать (симуляция урона)
        if (Input.GetKeyDown(KeyCode.H))
            ToggleDamage();

        // ТЕСТ: Нажми K, чтобы убить персонажа
        if (Input.GetKeyDown(KeyCode.K))
            PlayerDie();
    }

    // -----------------------
    // PHYSICS
    // -----------------------
    void FixedUpdate()
    {
        if (isDead) return;

        HandleLaneMovement();
        HandleHoverAndFall();
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
        if (isJumping) return;

        isJumping = true;

        // Запуск анимации прыжка
        if (animator != null) animator.SetTrigger("Jump");

        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        hoverTimer = hoverTime;
        isHovering = true;
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

        if (rb.position.y <= groundY + 0.05f && rb.velocity.y <= 0f)
        {
            rb.MovePosition(new Vector3(rb.position.x, groundY, rb.position.z));
            isJumping = false;
            isHovering = false;
        }
    }

    // -----------------------
    // DAMAGE & DEATH (Новые методы)
    // -----------------------
    public void ToggleDamage()
    {
        isDamaged = !isDamaged;
        if (animator != null) animator.SetBool("IsDamaged", isDamaged);
    }

    public void PlayerDie()
    {
        if (isDead) return;

        isDead = true;
        if (animator != null) animator.SetTrigger("Die");

        // Вызов конца игры через твой GameManager (если он есть на сцене)
        if (GameManager.instance != null)
        {
            GameManager.instance.EndGame();
        }
    }
}