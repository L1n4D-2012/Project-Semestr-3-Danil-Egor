using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class LaneRunnerRB : MonoBehaviour
{
    [Header("Lane Settings")]
    public float laneDistance = 2f;
    public float laneSwitchSpeed = 20f;

    private int currentLane = 1;
    private float targetX;

    [Header("Jump Settings")]
    public float jumpForce = 12f;
    public float extraGravity = 30f;

    // Таймер, чтобы не спамить прыжок
    private float jumpCooldown = 0f;

    [Header("Roll / Fast Fall")]
    public float fastFallForce = 20f;
    public float rollDuration = 1.0f;
    public float rollColliderHeight = 0.5f;
    private float originalColliderHeight;
    private Vector3 originalColliderCenter;
    private bool isRolling = false;

    [Header("Jetpack Settings")]
    public float jetpackHeight = 7f;
    public float jetpackRiseSpeed = 5f;

    [Header("Controls")]
    public float swipeThreshold = 20f;

    private bool isJumping = false;
    private Rigidbody rb;
    private CapsuleCollider col;
    private Vector2 startTouchPos;
    private bool isSwiping = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.useGravity = true;

        if (col != null)
        {
            originalColliderHeight = col.height;
            originalColliderCenter = col.center;
        }
        UpdateLaneTarget();
    }

    void Update()
    {
        if (jumpCooldown > 0) jumpCooldown -= Time.deltaTime;
        HandleInput();

        bool isFlying = PlayerJetpack.instance != null && PlayerJetpack.instance.isFlying;
        if (!isFlying && rb.velocity.y < -0.1f)
        {
            rb.AddForce(Vector3.down * extraGravity * Time.deltaTime, ForceMode.VelocityChange);
        }
    }

    void FixedUpdate()
    {
        HandleLaneMovement();
        if (PlayerJetpack.instance != null && PlayerJetpack.instance.isFlying)
        {
            HandleJetpackMovement();
            isJumping = false;
        }
        else
        {
            // Просто обновляем статус для логики анимаций, если нужно
            if (IsGrounded()) isJumping = false;
            else isJumping = true;
        }
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) ChangeLane(-1);
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) ChangeLane(1);

        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space)))
        {
            AttemptJump();
        }

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) DoRollOrFastFall();

        if (Input.GetMouseButtonDown(0)) { startTouchPos = Input.mousePosition; isSwiping = true; }
        else if (Input.GetMouseButton(0) && isSwiping) { CheckSwipe((Vector2)Input.mousePosition - startTouchPos); }
        else if (Input.GetMouseButtonUp(0)) { isSwiping = false; }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began) { startTouchPos = touch.position; isSwiping = true; }
            else if (touch.phase == TouchPhase.Moved && isSwiping) { CheckSwipe(touch.position - startTouchPos); }
            else if (touch.phase == TouchPhase.Ended) { isSwiping = false; }
        }
    }

    void CheckSwipe(Vector2 diff)
    {
        if (diff.magnitude > swipeThreshold)
        {
            float x = diff.x;
            float y = diff.y;
            if (Mathf.Abs(x) > Mathf.Abs(y)) { if (x < 0) ChangeLane(-1); else ChangeLane(1); }
            else { if (y > 0) AttemptJump(); else DoRollOrFastFall(); }
            isSwiping = false;
        }
    }

    void ChangeLane(int direction)
    {
        int targetLane = currentLane + direction;
        if (targetLane >= 0 && targetLane <= 2) { currentLane = targetLane; UpdateLaneTarget(); }
    }

    void DoRollOrFastFall()
    {
        if (!IsGrounded()) rb.velocity = new Vector3(rb.velocity.x, -fastFallForce, rb.velocity.z);
        else if (!isRolling) StartCoroutine(RollCoroutine());
    }

    IEnumerator RollCoroutine()
    {
        isRolling = true;
        col.height = rollColliderHeight;
        float heightDifference = originalColliderHeight - rollColliderHeight;
        col.center = new Vector3(originalColliderCenter.x, originalColliderCenter.y - (heightDifference / 2f), originalColliderCenter.z);
        yield return new WaitForSeconds(rollDuration);
        col.height = originalColliderHeight;
        col.center = originalColliderCenter;
        isRolling = false;
    }

    void UpdateLaneTarget() { targetX = (currentLane - 1) * laneDistance; }

    void HandleLaneMovement()
    {
        Vector3 pos = rb.position;
        float newX = Mathf.Lerp(pos.x, targetX, Time.fixedDeltaTime * laneSwitchSpeed);
        rb.MovePosition(new Vector3(newX, pos.y, pos.z));
    }

    void HandleJetpackMovement()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        Vector3 pos = rb.position;
        float newY = Mathf.Lerp(pos.y, jetpackHeight, Time.fixedDeltaTime * jetpackRiseSpeed);
        rb.MovePosition(new Vector3(pos.x, newY, pos.z));
    }

    // --- ЛОГИКА ПРЫЖКА ---
    void AttemptJump()
    {
        // 1. Таймер (чтобы не было двойных прыжков сразу)
        if (jumpCooldown > 0) return;

        // 2. Если мы уже летим вверх - не прыгаем (защита от прыжков в воздухе)
        if (rb.velocity.y > 1f) return;

        // 3. Проверка земли
        if (IsGrounded())
        {
            isJumping = true;
            jumpCooldown = 0.4f; // Блокируем прыжок на 0.4 сек
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    // --- УМНАЯ ПРОВЕРКА ЗЕМЛИ (БЕЗ СЛОЕВ) ---
    bool IsGrounded()
    {
        // Берем все объекты в радиусе 0.2 от ног
        Collider[] hits = Physics.OverlapSphere(transform.position + Vector3.up * 0.1f, 0.25f);

        foreach (var hit in hits)
        {
            // Если мы задели что-то, и это НЕ МЫ САМИ (не игрок)
            if (hit.gameObject != gameObject)
            {
                return true; // Значит мы на земле (или на дороге)
            }
        }
        return false; // Задели только воздух или себя
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * 0.1f, 0.25f);
    }
}