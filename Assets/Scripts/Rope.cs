using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RopeRenderer : MonoBehaviour
{
    [Header("References")]
    public Transform ropeStart; // your empty child on player
    public Transform ropeEnd;   // your kite

    [Header("Rope Settings")]
    public int segments = 20;
    public float sagAmount = 0.5f;
    public float sagSpeed = 2f; // smooth animation

    private LineRenderer lr;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = segments;
    }

    void LateUpdate() // LateUpdate = smoother visuals
    {
        if (ropeStart == null || ropeEnd == null) return;

        DrawRope();
    }

    void DrawRope()
    {
        Vector3 start = ropeStart.position;
        Vector3 end = ropeEnd.position;

        for (int i = 0; i < segments; i++)
        {
            float t = i / (float)(segments - 1);

            // Base straight line
            Vector3 pos = Vector3.Lerp(start, end, t);

            // Sag curve (sin wave)
            float sag = Mathf.Sin(t * Mathf.PI) * sagAmount;

            // Optional: animate sag slightly (wind feel)
            sag *= 1f + Mathf.Sin(Time.time * sagSpeed) * 0.1f;

            pos.y -= sag;

            lr.SetPosition(i, pos);
        }
    }
}
