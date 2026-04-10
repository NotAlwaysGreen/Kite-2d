using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
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

    [HideInInspector]
    public bool isAttached = false;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;
        rb.linearDamping = drag;
    }

    void FixedUpdate()
    {
        if (isAttached) return;

        ApplyStringElasticity();
        ApplyLiftFromPull();
        ClampVelocity();
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
}