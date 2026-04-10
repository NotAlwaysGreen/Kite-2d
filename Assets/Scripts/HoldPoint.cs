using UnityEngine;

public class KiteInteraction : MonoBehaviour
{
    private KiteController kiteInRange;
    private KiteController attachedKite;
    private Rigidbody2D playerRb;

    void Awake()
    {
        playerRb = GetComponentInParent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (attachedKite != null)
            {
                DetachKite();
            }
            else if (kiteInRange != null)
            {
                AttachKite(kiteInRange);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        KiteController kite = collision.GetComponent<KiteController>();

        if (kite != null)
        {
            kiteInRange = kite;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        KiteController kite = collision.GetComponent<KiteController>();

        if (kite != null && kite == kiteInRange)
        {
            kiteInRange = null;
        }
    }

    void AttachKite(KiteController kite)
    {
        attachedKite = kite;

        Rigidbody2D rb = kite.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        kite.isAttached = true;
        kite.enabled = false;

        kite.transform.SetParent(transform);
        kite.transform.localPosition = Vector3.zero;
    }

    void DetachKite()
    {
        if (attachedKite == null) return;

        Rigidbody2D rb = attachedKite.GetComponent<Rigidbody2D>();

        attachedKite.transform.SetParent(null);

        if (rb != null)
        {
            rb.simulated = true;

            Vector2 inheritedVelocity = playerRb.linearVelocity;

            Vector2 throwBoost = Vector2.zero;
            if (inheritedVelocity.magnitude > 0.1f)
            {
                throwBoost = inheritedVelocity.normalized * 2f;
            }

            rb.linearVelocity = inheritedVelocity + throwBoost;
        }

        attachedKite.isAttached = false;
        attachedKite.enabled = true;

        attachedKite = null;
    }
}