using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class KiteController : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("String")]
    public float maxDistance = 5f;
    public float stringElastic = 5f;
    public float maxElasticStretch = 1.5f;

    [Header("Lift")]
    public float liftFactor = 3f;
    public float maxLift = 10f;

    [Header("Air")]
    public float drag = 1f;
    public float gravityScale = 1f;

    [Header("Retract")]
    public float retractSpeed = 12f;
    public float attachDistance = 0.5f;

    [HideInInspector] public bool isAttached = false;

    private bool isRetracting = false;

    private Rigidbody2D rb;
    private Collider2D col;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        rb.gravityScale = gravityScale;
        rb.linearDamping = drag;
    }

    void Update()
    {
        //  Press Q to retract (only when not attached and not already retracting)
        if (Input.GetKeyDown(KeyCode.Q) && !isAttached && !isRetracting)
        {
            StartRetract();
        }
    }

    void FixedUpdate()
    {
        if (isAttached) return;

        if (isRetracting)
        {
            RetractToPlayer();
            return;
        }

        ApplyStringElasticity();
        ApplyLiftFromPull();
        ClampVelocity();
        ApplyHeightCorrection();
    }

    void StartRetract()
    {
        isRetracting = true;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        col.enabled = false; //  ignore collisions while retracting
    }

    void RetractToPlayer()
    {
        Vector2 target = player.position;
        Vector2 current = rb.position;

        float distance = Vector2.Distance(current, target);

        // Smooth ease-in (slows down near player)
        float speedMultiplier = Mathf.Clamp01(distance);
        float speed = retractSpeed * speedMultiplier;

        Vector2 newPos = Vector2.MoveTowards(current, target, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);

        if (distance < attachDistance)
        {
            isRetracting = false;
            isAttached = true;

            col.enabled = true; //  turn collider back on
        }
    }

    void ApplyStringElasticity()
    {
        Vector2 toKite = rb.position - (Vector2)player.position;
        float distance = toKite.magnitude;

        float allowedDistance = maxDistance * maxElasticStretch;

        if (distance > allowedDistance)
        {
            Vector2 dir = toKite.normalized;
            Vector2 pullBack = -dir * (distance - maxDistance) * stringElastic;
            rb.AddForce(pullBack);
        }
    }

    void ApplyLiftFromPull()
    {
        Vector2 toKite = rb.position - (Vector2)player.position;
        float distance = toKite.magnitude;

        if (distance > maxDistance)
        {
            Vector2 lift = Vector2.up * rb.linearVelocity.magnitude * liftFactor;
            rb.AddForce(lift);
        }
    }

    void ClampVelocity()
    {
        Vector2 vel = rb.linearVelocity;
        vel.y = Mathf.Clamp(vel.y, -Mathf.Infinity, maxLift);
        rb.linearVelocity = vel;
    }
    void ApplyHeightCorrection()
    {
        float playerY = player.position.y;
        float kiteY = rb.position.y;

        if (kiteY < playerY)
        {
            float heightDifference = playerY - kiteY;

            // Smooth, controlled lift
            float force = heightDifference * 3f;

            // Prevent extreme boosts
            force = Mathf.Clamp(force, 0.5f, 10f);

            rb.AddForce(Vector2.up * force);
        }
    }
}