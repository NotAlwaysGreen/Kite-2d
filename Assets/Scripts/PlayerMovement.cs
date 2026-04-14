using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public Image staminaBar;
    public KiteInteraction holdPoint; // reference to your script
    public Transform kite;

    private Rigidbody2D rb;

    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Sprint & Stamina")]
    public float sprintMultiplier = 1.5f;
    public float maxStamina = 5f;
    public float staminaConsumptionRate = 1f;
    public float staminaRecoveryRate = 0.5f;

    [Header("Pull")]
    public float ropeShortenSpeed = 0.001f;
    public float minRopeLength = 1.5f;
    public float ropeStiffness = 10f;
    public float maxPullSpeed = 10f;
    public float pullStaminaConsumptionRate = 1.2f;

    [Header("Glide")]
    public float glideGravityScale = 0.5f;
    public float glideDrag = 2f;
    public float glideStaminaConsumptionRate = 1f;

    private float currentRopeLength;
    public float currentStamina;

    private float defaultGravityScale;
    private float defaultDrag;

    private bool isGrounded;
    private bool isSprinting;
    private bool isPulling;
    private bool isGliding;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        currentStamina = maxStamina;

        defaultGravityScale = rb.gravityScale;
        defaultDrag = rb.linearDamping;

        if (kite != null)
        {
            currentRopeLength = Vector2.Distance(transform.position, kite.position);
        }
    }

    void Update()
    {
        staminaBar.fillAmount = GetStaminaPercentage();

        bool spaceHeld = Input.GetKey(KeyCode.Space);

        //  CORE LOGIC
        bool kiteAttached = holdPoint != null && holdPointHasKite();

        isPulling = spaceHeld && !kiteAttached && currentStamina > 0f;
        isGliding = spaceHeld && kiteAttached && currentStamina > 0f;

        if (!isPulling)
        {
            HandleMovement();
        }

        HandleGlide();
        HandleStamina();

        if (transform.position.y < -30f)
        {
            Die();
        }
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        if (isPulling && kite != null)
        {
            HandleRopePhysics();
            ClampPullVelocity();
        }
    }

    //  CHECK FROM HOLD POINT
    bool holdPointHasKite()
    {
        return holdPoint != null && holdPoint.GetAttachedKite() != null;
    }

    void HandleMovement()
    {
        float moveInput = Input.GetAxis("Horizontal");

        isSprinting = Input.GetKey(KeyCode.LeftShift)
                      && currentStamina > 0f
                      && isGrounded;

        float actualSpeed = moveSpeed * (isSprinting ? sprintMultiplier : 1f);

        rb.linearVelocity = new Vector2(
            moveInput * actualSpeed,
            rb.linearVelocity.y
        );
    }

    void HandleGlide()
    {
        if (isGliding)
        {
            rb.gravityScale = glideGravityScale;
            rb.linearDamping = glideDrag;

            // optional: cap fall speed
            if (rb.linearVelocity.y < -2f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, -2f);
            }
        }
        else
        {
            rb.gravityScale = defaultGravityScale;
            rb.linearDamping = defaultDrag;
        }
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
        else if (isGliding)
        {
            currentStamina -= glideStaminaConsumptionRate * Time.deltaTime;
        }
        else if (isSprinting)
        {
            currentStamina -= staminaConsumptionRate * Time.deltaTime;
        }
        else if (isGrounded)
        {
            currentStamina += staminaRecoveryRate * Time.deltaTime;
        }

        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
    }

    float GetStaminaPercentage()
    {
        return currentStamina / maxStamina;
    }

    void Die()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }
}