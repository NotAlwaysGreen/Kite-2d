using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class KiteController : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("String")]
    public float maxDistance = 5f;
    public float stringElastic = 5f;     // how strongly kite snaps back when overstretched
    public float maxElasticStretch = 1.5f; // allow some extra stretch beyond maxDistance

    [Header("Lift")]
    public float liftFactor = 3f;        // how much upward lift from pulling
    public float maxLift = 10f;          // cap vertical speed

    [Header("Air")]
    public float drag = 1f;
    public float gravityScale = 1f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;
        rb.linearDamping = drag;
    }

    void FixedUpdate()
    {
        ApplyStringElasticity();
        ApplyLiftFromPull();
        ClampVelocity();
    }

    void ApplyStringElasticity()
    {
        Vector2 toKite = rb.position - (Vector2)player.position;
        float distance = toKite.magnitude;

        // Calculate allowed stretch
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
            // Kite is being pulled � generate upward lift
            Vector2 dir = toKite.normalized;
            Vector2 pullDirection = -dir; // towards player

            // Lift proportional to the pull and current horizontal motion
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
}