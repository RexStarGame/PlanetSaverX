using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Bevægelse")]
    // Hvor hurtigt spilleren kan bevæge sig til højre og venstre.
    public float moveSpeed = 6f;

    // Hvor meget kraft der bliver brugt, når spilleren hopper.
    public float jumpForce = 8f;

    [Header("Referencer")]
    // Reference til spillerens arm, så vi kan rotere den mod musen.
    // Armen er delt fra kroppen, så vi kun roterer armen og ikke hele spilleren.
    public Transform arm;

    [Header("Aim Indstillinger")]
    // Den mindste vinkel armen må pege i.
    // Her er det sat til -45 grader.
    public float minAngle = -45f;

    // Den største vinkel armen må pege i.
    // Her er det sat til 45 grader.
    public float maxAngle = 45f;

    // Rigidbody2D bruges til at styre spillerens fysiske bevægelse.
    private Rigidbody2D rb;

    // Holder styr på, om spilleren står på jorden.
    public bool isGrounded;

    // Holder styr på, om spilleren lige nu vender mod højre.
    // Hvis den er false, vender spilleren mod venstre.
    private bool facingRight = true;

    // Start bliver kørt én gang, når spillet starter.
    // Her henter vi Rigidbody2D-komponenten fra spilleren.
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update bliver kørt hele tiden, én gang per frame.
    // Her læser vi spillerens input og opdaterer bevægelse, hop og sigtning.
    void Update()
    {
        // Her læser vi horizontal input fra tastaturet.
        // Det giver typisk -1, 0 eller 1 alt efter om spilleren trykker venstre, ingenting eller højre.
        float move = Input.GetAxis("Horizontal");

        // Her ændrer vi spillerens fart i x-retningen.
        // Y-farten bliver beholdt, så hop og fald stadig virker normalt.
        rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);

        // Her tjekker vi, om spilleren prøver at hoppe.
        // Spilleren må kun hoppe, hvis den står på jorden.
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // Når spilleren hopper, giver vi den en ny y-fart opad.
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // Her kører vi den funktion, der styrer armens rotation og hvilken vej spilleren vender.
        HandleAimAndRotation();
    }

    // Denne funktion styrer aiming og flip af spilleren.
    void HandleAimAndRotation()
    {
        // Hvis der ikke er sat en arm ind i Inspector, stopper vi funktionen her,
        // så spillet ikke prøver at bruge noget, der ikke findes.
        if (arm == null) return;

        // Her finder vi musens position i verdenen i stedet for bare på skærmen.
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Vi sætter z til 0, fordi vi arbejder i et 2D-spil.
        // På den måde passer musens position bedre til spillerens verden.
        mousePos.z = 0f;

        // Her tjekker vi, om musen er til venstre eller højre for spilleren.
        // Hvis musen skifter side, vender spilleren også.
        if (mousePos.x < transform.position.x && facingRight)
        {
            Flip(false);
        }
        else if (mousePos.x > transform.position.x && !facingRight)
        {
            Flip(true);
        }

        // Her laver vi musens position om til en lokal position i forhold til spilleren.
        // Det gør det nemmere at regne armens vinkel ud korrekt,
        // især når spilleren bliver vendt til højre eller venstre.
        Vector3 localMouse = transform.InverseTransformPoint(mousePos);

        // Her finder vi retningen fra armens lokale position hen mod musen.
        Vector2 direction = (Vector2)localMouse - (Vector2)arm.localPosition;

        // Her regner vi vinklen ud ud fra retningen til musen.
        // Atan2 finder vinklen, og vi laver den om fra radianer til grader.
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Her låser vi vinklen, så armen kun kan pege inden for et bestemt område foran spilleren.
        // Det gør, at man ikke kan sigte hele vejen rundt bag sig.
        angle = Mathf.Clamp(angle, minAngle, maxAngle);

        // Her roterer vi armen lokalt.
        // Vi bruger localRotation, så armens rotation passer sammen med spillerens flip.
        arm.localRotation = Quaternion.Euler(0f, 0f, angle);
    }

    // Denne funktion vender spilleren til højre eller venstre.
    void Flip(bool faceRight)
    {
        // Her gemmer vi først, hvilken vej spilleren nu skal vende.
        facingRight = faceRight;

        // Vi tager spillerens nuværende scale.
        Vector3 scale = transform.localScale;

        // Hvis spilleren skal vende mod højre, sørger vi for at x er positiv.
        // Hvis spilleren skal vende mod venstre, gør vi x negativ.
        scale.x = faceRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);

        // Til sidst sætter vi den nye scale på spilleren.
        transform.localScale = scale;
    }

    // Denne funktion bliver kaldt, så længe spilleren bliver ved med at røre noget.
    void OnCollisionStay2D(Collision2D collision)
    {
        // Hvis det spilleren rører ved har tagget "Ground",
        // så ved vi, at spilleren står på jorden.
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }

    // Denne funktion bliver kaldt, når spilleren stopper med at røre noget.
    void OnCollisionExit2D(Collision2D collision)
    {
        // Hvis spilleren forlader et objekt med tagget "Ground",
        // så står spilleren ikke længere på jorden.
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }
}