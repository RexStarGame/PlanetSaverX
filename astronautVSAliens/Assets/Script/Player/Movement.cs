using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float jumpForce = 8f;

    public bool facingRight = true;
    public Transform arm; // Assign the arm sprite here

    private Rigidbody2D rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0.5f;
    }

    void Update()
    {
        // Player movement
        float move = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);

        // Flip player sprite based on movement
        if (move > 0 && !facingRight) Flip(true);
        else if (move < 0 && facingRight) Flip(false);

        if (Input.GetButtonDown("Jump") && isGrounded)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        // Rotate arm to follow mouse
        AimArmAtMouse();
    }

    void Flip(bool faceRight)
    {
        facingRight = faceRight;

        // Flip player horizontally
        Vector3 scale = transform.localScale;
        scale.x = facingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    void AimArmAtMouse()
    {
        if (arm == null) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        // Direction from the arm's pivot to the mouse
        Vector3 direction = mousePos - arm.position;

        // Angle in degrees for the arm's rotation
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Apply rotation to the arm
        arm.rotation = Quaternion.Euler(0f, 0f, angle);

        // Determine if the arm sprite should flip based on its local rotation
        // This makes the arm sprite flip only when it's naturally pointing behind the player
        // The angle check is more robust than just direction.x
        bool armShouldFlipSprite = (angle > 90f && angle <= 180f) || (angle < -90f && angle >= -180f);

        // This ensures the arm sprite is oriented correctly for the player's facing direction
        // For example, if 'facingRight' is true, and the arm points left (behind), flip it.
        // If 'facingRight' is false, and the arm points right (behind), flip it.
        Vector3 currentArmLocalScale = arm.localScale;
        if (facingRight)
        {
            currentArmLocalScale.x = (armShouldFlipSprite) ? -1f : 1f;
        }
        else // Player is facing left
        {
            currentArmLocalScale.x = (armShouldFlipSprite) ? 1f : -1f;
        }
        arm.localScale = currentArmLocalScale;
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
