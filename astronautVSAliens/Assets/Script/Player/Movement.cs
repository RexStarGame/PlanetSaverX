using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Bevægelse")]
    public float moveSpeed = 6f;
    public float jumpForce = 8f;

    [Header("Referencer")]
    public Transform arm;

    [Header("Aim Indstillinger")]
    public float minAngle = -45f;
    public float maxAngle = 45f;

    private Rigidbody2D rb;

    public bool isGrounded;
    private bool facingRight = true;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        float move = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
        HandleAimAndRotation();
    }
    void HandleAimAndRotation()
    {
  
        if (arm == null) return;


        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);


        mousePos.z = 0f;

        if (mousePos.x < transform.position.x && facingRight)
        {
            Flip(false);
        }
        else if (mousePos.x > transform.position.x && !facingRight)
        {
            Flip(true);
        }
        Vector3 localMouse = transform.InverseTransformPoint(mousePos);
        Vector2 direction = (Vector2)localMouse - (Vector2)arm.localPosition;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle = Mathf.Clamp(angle, minAngle, maxAngle);
        arm.localRotation = Quaternion.Euler(0f, 0f, angle);
    }
    void Flip(bool faceRight)
    {
        facingRight = faceRight;

        Vector3 scale = transform.localScale;
        scale.x = faceRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);

        transform.localScale = scale;
    }
    void OnCollisionStay2D(Collision2D collision)
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