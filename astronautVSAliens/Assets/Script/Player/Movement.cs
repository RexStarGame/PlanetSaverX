using UnityEngine;

public class LowGravity2D : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float jumpForce = 8f;

    private Rigidbody2D rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Lower gravity for floaty jumps
        rb.gravityScale = 0.5f;
    }

    void Update()
    {
        float move = Input.GetAxis("Horizontal");

        // Movement
        rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);

        // Flip character
        if (move > 0)
        {
            transform.localScale = new Vector3(5, 5, 5);
        }
        else if (move < 0)
        {
            transform.localScale = new Vector3(-5, 5, 5);
        }

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}