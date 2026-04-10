using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public Image staminaBar;
    public int coin = 0;

    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Kite Rope")]
    public Transform kite;
    public float ropeShortenSpeed = 0.001f;
    public float minRopeLength = 1.5f;

    [Header("Rope Physics")]
    public float ropeStiffness = 10f;
    public float maxPullSpeed = 10f;

    private float currentRopeLength;
    private bool isPulling;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Sprint & Stamina")]
    public float sprintMultiplier = 1.5f;
    public float maxStamina = 5f;
    public float staminaConsumptionRate = 1f;
    public float staminaRecoveryRate = 0.5f;

    [Header("Pull Stamina")]
    public float pullStaminaConsumptionRate = 1.2f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private float currentStamina;
    private bool isSprinting;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentStamina = maxStamina;

        if (kite != null)
        {
            currentRopeLength = Vector2.Distance(transform.position, kite.position);
        }
    }

    void Update()
    {
        staminaBar.fillAmount = GetStaminaPercentage();

        isPulling = Input.GetKey(KeyCode.Space) && currentStamina > 0f;

        if (!isPulling)
        {
            HandleMovement();
        }

        HandleStamina();

        if (transform.position.y < -30f)
        {
            Die();
        }
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isPulling && kite != null)
        {
            HandleRopePhysics();
            ClampPullVelocity();
        }
    }

    void HandleMovement()
    {
        float moveInput = Input.GetAxis("Horizontal");

        isSprinting = Input.GetKey(KeyCode.LeftShift) && currentStamina > 0.5f && isGrounded;

        float actualSpeed = moveSpeed * (isSprinting ? sprintMultiplier : 1f);

        rb.linearVelocity = new Vector2(moveInput * actualSpeed, rb.linearVelocity.y);
    }

    void HandleRopePhysics()
    {
        Vector2 playerPos = rb.position;
        Vector2 kitePos = kite.position;

        Vector2 toKite = kitePos - playerPos;
        float distance = toKite.magnitude;
        Vector2 direction = toKite.normalized;

        currentRopeLength -= ropeShortenSpeed * Time.fixedDeltaTime;
        currentRopeLength = Mathf.Max(currentRopeLength, minRopeLength);

        if (distance > currentRopeLength)
        {
            float stretchAmount = distance - currentRopeLength;
            Vector2 force = direction * stretchAmount * ropeStiffness;

            rb.AddForce(force, ForceMode2D.Force);
        }
    }

    void ClampPullVelocity()
    {
        float speed = rb.linearVelocity.magnitude;

        if (speed > maxPullSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxPullSpeed;
        }
    }

    void HandleStamina()
    {
        if (isPulling)
        {
            currentStamina -= pullStaminaConsumptionRate * Time.deltaTime;
        }
        else if (isSprinting)
        {
            currentStamina -= staminaConsumptionRate * Time.deltaTime;
        }
        else
        {
            currentStamina += staminaRecoveryRate * Time.deltaTime;
        }

        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
    }

    public float GetStaminaPercentage()
    {
        return currentStamina / maxStamina;
    }

    private void Die()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
        Time.timeScale = 1f;
    }
}