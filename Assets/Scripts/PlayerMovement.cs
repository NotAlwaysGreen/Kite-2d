using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float movespeed = 5f;
    public float jumpforce = 5f;
    public Transform groundcheck;
    public float groundcheckradius = 0.2f;
    public LayerMask groundLayer;


    private Rigidbody2D rb;
    private bool isGrounded;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float MoveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(MoveInput * movespeed, rb.linearVelocity.y);

        if (Input.GetKeyDown(KeyCode.Space)&& isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpforce);
        }
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundcheck.position, groundcheckradius, groundLayer);

    }
}
