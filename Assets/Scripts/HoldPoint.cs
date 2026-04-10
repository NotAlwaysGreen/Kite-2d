
        using UnityEngine;

public class KiteInteraction : MonoBehaviour
    {
        private KiteController kiteInRange;
        private KiteController attachedKite;

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
                Debug.Log("Kite in range");
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            KiteController kite = collision.GetComponent<KiteController>();

            if (kite != null && kite == kiteInRange)
            {
                kiteInRange = null;
                Debug.Log("Kite left range");
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

            Debug.Log("Kite attached");
        }

        void DetachKite()
        {
            Rigidbody2D rb = attachedKite.GetComponent<Rigidbody2D>();

            attachedKite.transform.SetParent(null);

            if (rb != null)
            {
                rb.simulated = true;
            }

            attachedKite.isAttached = false;
            attachedKite.enabled = true;

            Debug.Log("Kite detached");

            attachedKite = null;
        }
    }


