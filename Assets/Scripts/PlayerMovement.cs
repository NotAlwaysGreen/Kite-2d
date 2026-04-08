using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Sprint & Stamina")]
    public float sprintMultiplier = 1.5f;        // How much faster when sprinting
    public float maxStamina = 5f;                // Maximum stamina
    public float staminaConsumptionRate = 1f;    // Stamina per second when sprinting
    public float staminaRecoveryRate = 0.5f;     // Stamina per second when not sprinting

    private Rigidbody2D rb;
    private bool isGrounded;
    private float currentStamina;
    private bool isSprinting;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentStamina = maxStamina;
    }

    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");

        // Check if sprint is requested and allowed
        isSprinting = Input.GetKey(KeyCode.LeftShift) && currentStamina > 0f && isGrounded;

        float actualSpeed = moveSpeed * (isSprinting ? sprintMultiplier : 1f);

        rb.linearVelocity = new Vector2(moveInput * actualSpeed, rb.linearVelocity.y);

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        HandleStamina();
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    void HandleStamina()
    {
        if (isSprinting)
        {
            // Consume stamina
            currentStamina -= staminaConsumptionRate * Time.deltaTime;
            if (currentStamina < 0f)
                currentStamina = 0f; // prevent negative stamina
        }
        else
        {
            // Recover stamina
            currentStamina += staminaRecoveryRate * Time.deltaTime;
            if (currentStamina > maxStamina)
                currentStamina = maxStamina; // cap at max
        }
    }

    // Optional: method to get stamina percentage for UI
    public float GetStaminaPercentage()
    {
        return currentStamina / maxStamina;
    }
}