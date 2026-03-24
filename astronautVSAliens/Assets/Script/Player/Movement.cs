using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float jumpForce = 8f;

    public bool facingRight = true;
    public Transform arm;

    private Rigidbody2D rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0.5f;
    }

    void Update()
    {
        float move = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);

        // Spilleren vender nu efter musen - ikke efter movement
        FaceMouseSide();

        if (Input.GetButtonDown("Jump") && isGrounded)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        AimArmAtMouse();
    }

    void FaceMouseSide()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (mousePos.x > transform.position.x && !facingRight)
        {
            Flip(true);
        }
        else if (mousePos.x < transform.position.x && facingRight)
        {
            Flip(false);
        }
    }

    void Flip(bool faceRight)
    {
        facingRight = faceRight;

        Vector3 scale = transform.localScale;
        scale.x = facingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    void AimArmAtMouse()
    {
        if (arm == null) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Vector2 direction = mousePos - arm.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (facingRight)
        {
            // Kun foran spilleren mod højre
            angle = Mathf.Clamp(angle, -90f, 90f);
            arm.rotation = Quaternion.Euler(0f, 0f, angle);

            Vector3 armScale = arm.localScale;
            armScale.y = Mathf.Abs(armScale.y);
            arm.localScale = armScale;
        }
        else
        {
            // Kun foran spilleren mod venstre
            if (angle > 0f)
                angle = Mathf.Clamp(angle, 90f, 180f);
            else
                angle = Mathf.Clamp(angle, -180f, -90f);

            arm.rotation = Quaternion.Euler(0f, 0f, angle);

            Vector3 armScale = arm.localScale;
            armScale.y = -Mathf.Abs(armScale.y);
            arm.localScale = armScale;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }
}