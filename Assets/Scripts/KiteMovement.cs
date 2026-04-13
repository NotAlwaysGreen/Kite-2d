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

    private Rigidbody2D rb;
    private Collider2D col;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        rb.gravityScale = gravityScale;
        rb.linearDamping = drag;
    }

    

    void FixedUpdate()
    {
        if (isAttached) return;

       

        ApplyStringElasticity();
        ApplyLiftFromPull();
        ClampVelocity();
        //ApplyHeightCorrection();
        ApplyTensionLiftWhenBelowPlayer();
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

    void ApplyTensionLiftWhenBelowPlayer()
    {
        Vector2 toKite = rb.position - (Vector2)player.position;
        float distance = toKite.magnitude;

        float playerY = player.position.y;
        float kiteY = rb.position.y;

        // Only apply when stretched AND below player
        if (distance > maxDistance && kiteY < playerY-1f)
        {
            float stretchAmount = distance - maxDistance;
            float maxStretch = maxDistance * (maxElasticStretch - 1f);

            float stretchPercent = stretchAmount / maxStretch;
            stretchPercent = Mathf.Clamp01(stretchPercent);

            // Additional lift based on tension
            float liftStrength = stretchPercent * liftFactor * 10f;

            // Small minimum to avoid dead zones
            liftStrength = Mathf.Max(liftStrength, 0.1f);

            rb.AddForce(Vector2.up * liftStrength);
        }
    }
}