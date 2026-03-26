using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 2f;

    private Rigidbody2D rb;

    // SIKKERHEDSLÅS: Sikrer at vi ikke prøver at ødelægge kuglen to gange
    private bool isDestroyed = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Giv kuglen fart fremad
        rb.linearVelocity = transform.right * speed;

        // Unitys indbyggede forsinkede Destroy er memory-safe.
        // Den rydder automatisk op, hvis kuglen flyver ud i ingenting.
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        // 1. SIKKERHEDSTJEK: Hvis kuglen allerede har ramt noget, stopper vi her!
        if (isDestroyed) return;

        // 2. Tjekker om vi rammer spilleren selv (ignorer)
        if (hitInfo.CompareTag("Player")) return;

        // --- VI HAR RAMT NOGET GYLDIGT ---

        // 3. Lås kuglen, så den ikke kan ramme flere ting i samme frame
        isDestroyed = true;

        // HER KAN DU TILFØJE DAMAGE SENERE:
        // EnemyHealth enemy = hitInfo.GetComponent<EnemyHealth>();
        // if (enemy != null) { enemy.TakeDamage(10); }

        Debug.Log("Ramte: " + hitInfo.name);

        // 4. Ødelæg kuglen nu
        Destroy(gameObject);
    }
}