using UnityEngine;

public class LaneRunnerController : MonoBehaviour
{
    [Header("Lane Settings")]
    public float laneDistance = 2f;        // відстань між полосами
    public float laneSwitchSpeed = 10f;    // наскільки плавно переходить
    private int currentLane = 1;           // 0 = Ліво, 1 = Центр, 2 = Право

    [Header("Jump Settings")]
    public float jumpHeight = 2f;          // висота стрибка
    public float jumpSpeed = 6f;           // швидкість стрибка
    private bool isJumping = false;
    private float jumpStartY;
    private bool goingUp = true;
    public float gravity = 15f;


    private Vector3 targetPosition;

    void Start()
    {
        targetPosition = transform.position;
    }

    void Update()
    {
        HandleMovement();
        HandleJump();
        HandleInput();
    }

    private void HandleInput()
    {
        // Вліво
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            MoveLeft();

        // Вправо
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            MoveRight();

        // Стрибок
        //  if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W))
         //   Jump();

        // Підкат
        // Реалізуємо пізніше
        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("Slide — скоро додамо!");
        }
    }

    // ------------------------------------
    //          РУХ МІЖ ПОЛОСАМИ
    // ------------------------------------
    public void MoveLeft()
    {
        if (currentLane > 0)
        {
            currentLane--;
            UpdateLanePosition();
        }
    }

    public void MoveRight()
    {
        if (currentLane < 2)
        {
            currentLane++;
            UpdateLanePosition();
        }
    }

    private void UpdateLanePosition()
    {
        float xPos = (currentLane - 1) * laneDistance;
        targetPosition = new Vector3(xPos, transform.position.y, transform.position.z);
    }

    private void HandleMovement()
    {
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            Time.deltaTime * laneSwitchSpeed
        );
    }

    // ------------------------------------
    //                СТРИБОК
    // ------------------------------------
    private float verticalVelocity = 0f;

    private void Jump()
    {
        if (!isJumping)
        {
            isJumping = true;
            verticalVelocity = Mathf.Sqrt(2f * gravity * jumpHeight); // формула стрибка
        }
    }

    private void HandleJump()
    {
        if (isJumping)
        {
            // застосовуємо гравітацію
            verticalVelocity -= gravity * Time.deltaTime;

            Vector3 pos = transform.position;
            pos.y += verticalVelocity * Time.deltaTime;

            // якщо приземлився
            if (pos.y <= jumpStartY)
            {
                pos.y = jumpStartY;
                isJumping = false;
                verticalVelocity = 0f;
            }

            transform.position = pos;
        }
    }

}
