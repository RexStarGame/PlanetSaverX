using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Bevægelse")]
    public float moveSpeed = 6f;
    public float jumpForce = 8f;

    [Header("Referencer")]
    public Transform arm; // Træk din arm-child herind i Inspectoren

    [Header("Aim Indstillinger")]
    public float minAngle = -45f; // Laveste punkt (90 grader i alt hvis -45 til 45)
    public float maxAngle = 45f;

    private Rigidbody2D rb;
    public bool isGrounded;
    private bool facingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Sørg for at tyngdekraften føles god
    }

    void Update()
    {
        // 1. Horisontal bevægelse
        float move = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);

        // 2. Hop
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // 3. Aim og Flip (Samlet her for at undgå fejl)
        HandleAimAndRotation();
    }

    void HandleAimAndRotation()
    {
        if (arm == null) return;

        // Find musens position
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        // --- FLIP KROPPEN ---
        // Vi tjekker om musen er til højre eller venstre for spilleren
        if (mousePos.x < transform.position.x && facingRight)
        {
            Flip(false);
        }
        else if (mousePos.x > transform.position.x && !facingRight)
        {
            Flip(true);
        }

        // --- ROTER ARMEN ---
        // Vi bruger InverseTransformPoint for at finde musen relativt til spilleren
        // Det gør at 'angle' altid regnes ud fra den vej spilleren kigger
        Vector3 localMouse = transform.InverseTransformPoint(mousePos);
        Vector2 direction = (Vector2)localMouse - (Vector2)arm.localPosition;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Her låser vi armen til din 90 graders bue foran spilleren
        angle = Mathf.Clamp(angle, minAngle, maxAngle);

        // Vi sætter LOCAL rotation, så den følger spillerens flip automatisk
        arm.localRotation = Quaternion.Euler(0f, 0f, angle);
    }

    void Flip(bool faceRight)
    {
        facingRight = faceRight;
        Vector3 scale = transform.localScale;
        scale.x = faceRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    // Tjekker om vi rører jorden
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